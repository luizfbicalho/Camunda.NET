using System;
using System.Collections.Generic;

/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.engine.test.standalone.db
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.isOneOf;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;


	using StringUtils = org.apache.commons.lang3.StringUtils;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Miklas Boskamp
	/// 
	/// </summary>
	public class SchemaLogUpgradeScriptPatternTest : SchemaLogTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOnlyValidUpgradeFilePatterns()
	  public virtual void testOnlyValidUpgradeFilePatterns()
	  {
		/// <summary>
		/// valid patterns: 
		/// h2_engine_7.2_to_7.3.sql,
		/// oracle_engine_7.3_patch_7.3.0_to_7.3.1.sql,
		/// postgres_engine_7.3_patch_7.3.2_to_7.3.3_1.sql,
		/// </summary>
		foreach (string file in folderContents[UPGRADE_SCRIPT_FOLDER])
		{
		  Console.WriteLine(file);

		  assertTrue("unexpected file format for file: " + file, file.EndsWith(".sql", StringComparison.Ordinal));
		  // get rid of the .sql ending as it makes splitting easier
		  file = file.Substring(0, file.Length - 4);

		  string[] nameParts = file.Split("_", true);
		  assertThat(nameParts[0], isOneOf(DATABASES));
		  assertThat(nameParts[1], @is("engine"));
		  string minorVersion = nameParts[2];
		  assertTrue(isMinorLevel(minorVersion));
		  if (nameParts[3].Equals("to"))
		  {
			// minor update
			assertThat(nameParts[4], isOneOf(getPossibleNextVersions(minorVersion)));

			assertThat(nameParts.Length, @is(5));
		  }
		  else if (nameParts[3].Equals("patch"))
		  {
			// patch update
			string basePatchVersion = nameParts[4];
			assertTrue("unexpected patch version pattern for file: " + file, isPatchLevel(basePatchVersion));
			assertThat(minorVersion, @is(getMinorLevelFromPatchVersion(basePatchVersion)));
			assertThat(nameParts[5], @is("to"));
			assertThat(nameParts[6], isOneOf(getPossibleNextVersions(basePatchVersion)));

			if (nameParts.Length == 8)
			{
			  // check that script version is integer only
			  int.Parse(nameParts[7]);
			}
			else
			{
			  assertThat(nameParts.Length, @is(7));
			}
		  }
		  else
		  {
			fail("unexpected pattern for file: " + file);
		  }
		}
	  }

	  private string getMinorLevelFromPatchVersion(string minorVersion)
	  {
		string[] versionParts = minorVersion.Split("\\.", true);
		return StringUtils.join(versionParts, ".", 0, 2);
	  }

	  private string[] getPossibleNextVersions(string version)
	  {
		IList<string> versions = new List<string>();
		string[] versionParts = version.Split("\\.", true);
		if (isPatchLevel(version))
		{
		  // next patch version
		  versions.Add(versionParts[0] + "." + versionParts[1] + "." + (int.Parse(versionParts[2]) + 1));
		}
		else if (isMinorLevel(version))
		{
		  // next minor version
		  versions.Add(versionParts[0] + "." + (int.Parse(versionParts[1]) + 1));
		  // next major version
		  versions.Add((int.Parse(versionParts[0]) + 1) + ".0");
		}
		else
		{
		  fail("unexpected pattern for version: " + version);
		}
		return versions.ToArray();
	  }
	}
}