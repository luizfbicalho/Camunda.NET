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
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;


	using DbSqlSessionFactory = org.camunda.bpm.engine.impl.db.sql.DbSqlSessionFactory;
	using CoreMatchers = org.hamcrest.CoreMatchers;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;

	/// <summary>
	/// @author Miklas Boskamp
	/// 
	/// </summary>
	public class SchemaLogTestCase
	{
	  protected internal const string BASE_PATH = "org/camunda/bpm/engine/db";
	  protected internal static readonly string CREATE_SCRIPT_FOLDER = BASE_PATH + "/create";
	  protected internal static readonly string UPGRADE_SCRIPT_FOLDER = BASE_PATH + "/upgrade";
	  protected internal static readonly IList<string> SCRIPT_FOLDERS = Arrays.asList(CREATE_SCRIPT_FOLDER, UPGRADE_SCRIPT_FOLDER);
	  protected internal static readonly string[] DATABASES = DbSqlSessionFactory.SUPPORTED_DATABASES;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule rule = new org.camunda.bpm.engine.test.ProcessEngineRule();
	  public ProcessEngineRule rule = new ProcessEngineRule();
	  public ProcessEngine processEngine;

	  protected internal string folderPath;
	  protected internal IDictionary<string, IList<string>> folderContents;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		processEngine = rule.ProcessEngine;

		folderContents = new Dictionary<string, IList<string>>();
		foreach (string folder in SCRIPT_FOLDERS)
		{
		  folderContents[folder] = readFolderContent(folder);
		}
	  }

	  private IList<string> readFolderContent(string path)
	  {
		ClassLoader classLoader = this.GetType().ClassLoader;
		URL resource = classLoader.getResource(path);
		assertThat(resource, CoreMatchers.notNullValue());

		File folder = new File(resource.File);
		assertTrue(folder.Directory);

		return Arrays.asList(folder.list());
	  }

	  public virtual bool isMinorLevel(string version)
	  {
		// 7.10 -> true, 7.10.1 -> false
		return version.Split("\\.", true).length == 2;
	  }

	  public virtual bool isPatchLevel(string version)
	  {
		// 7.10.0 -> true, 7.10.1 -> true, 7.10 -> false
		return version.Split("\\.", true).length == 3;
	  }
	}

}