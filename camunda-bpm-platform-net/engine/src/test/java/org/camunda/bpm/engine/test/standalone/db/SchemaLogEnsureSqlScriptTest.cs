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


	using SchemaLogEntry = org.camunda.bpm.engine.management.SchemaLogEntry;
	using Before = org.junit.Before;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Miklas Boskamp
	/// 
	/// </summary>
	public class SchemaLogEnsureSqlScriptTest : SchemaLogTestCase
	{

	  protected internal string currentSchemaVersion;
	  protected internal string dataBaseType;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @Before public void init()
	  public override void init()
	  {
		base.init();

		SchemaLogEntry latestEntry = processEngine.ManagementService.createSchemaLogQuery().orderByTimestamp().desc().listPage(0, 1).get(0);
		currentSchemaVersion = latestEntry.Version;

		dataBaseType = processEngine.ProcessEngineConfiguration.DatabaseType;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void ensureUpgradeScriptsUpdateSchemaLogVersion()
	  public virtual void ensureUpgradeScriptsUpdateSchemaLogVersion()
	  {
		IList<string> scriptsForDB = new List<string>();
		foreach (string file in folderContents[UPGRADE_SCRIPT_FOLDER])
		{
		  if (file.StartsWith(dataBaseType, StringComparison.Ordinal))
		  {
			scriptsForDB.Add(file);
		  }
		}
		assertThat(getLatestTargetVersion(scriptsForDB), @is(currentSchemaVersion));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void ensureOnlyScriptsForValidDatabaseTypes()
	  public virtual void ensureOnlyScriptsForValidDatabaseTypes()
	  {
		foreach (string file in folderContents[UPGRADE_SCRIPT_FOLDER])
		{
		  assertThat(file.Split("_", true)[0], isOneOf(DATABASES));
		}
	  }

	  private string getTargetVersionForScript(string file)
	  {
		string targetVersion = file.Substring(file.IndexOf("to_", StringComparison.Ordinal) + 3).Replace(".sql", "");
		if (isMinorLevel(targetVersion))
		{
		  targetVersion += ".0";
		}
		return targetVersion;
	  }

	  private string getLatestTargetVersion(IList<string> scriptFiles)
	  {
		string latestVersion = null;
		foreach (string file in scriptFiles)
		{
		  if (string.ReferenceEquals(latestVersion, null))
		  {
			latestVersion = getTargetVersionForScript(file);
		  }
		  else
		  {
			string targetVersion = getTargetVersionForScript(file);
			if (isLaterVersionThan(targetVersion, latestVersion))
			{
			  latestVersion = targetVersion;
			}
		  }
		}
		return latestVersion;
	  }

	  private bool isLaterVersionThan(string v1, string v2)
	  {
		string[] v1_ = v1.Split("\\.|_", true);
		string[] v2_ = v2.Split("\\.|_", true);

		int length = Math.Max(v1_.Length, v2_.Length);
		for (int i = 0; i < length; i++)
		{
		  int v1Part = i < v1_.Length ? int.Parse(v1_[i]) : 0;
		  int v2Part = i < v2_.Length ? int.Parse(v2_[i]) : 0;
		  if (v1Part != v2Part)
		  {
			return v1Part > v2Part;
		  }
		}
		return false;
	  }
	}
}