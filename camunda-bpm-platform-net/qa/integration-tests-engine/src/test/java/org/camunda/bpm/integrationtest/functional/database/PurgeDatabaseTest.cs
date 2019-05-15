using System.Collections.Generic;
using System.Text;

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
namespace org.camunda.bpm.integrationtest.functional.database
{
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using HistoryLevelSetupCommand = org.camunda.bpm.engine.impl.HistoryLevelSetupCommand;
	using ManagementServiceImpl = org.camunda.bpm.engine.impl.ManagementServiceImpl;
	using ProcessEngineImpl = org.camunda.bpm.engine.impl.ProcessEngineImpl;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using PersistenceSession = org.camunda.bpm.engine.impl.db.PersistenceSession;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;


	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class PurgeDatabaseTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class PurgeDatabaseTest : AbstractFoxPlatformIntegrationTest
	{

	  public static readonly IList<string> TABLENAMES_EXCLUDED_FROM_DB_CLEAN_CHECK = Arrays.asList("ACT_GE_PROPERTY", "ACT_GE_SCHEMA_LOG");

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive processArchive()
	  public static WebArchive processArchive()
	  {
		return initWebArchiveDeployment().addAsResource("org/camunda/bpm/integrationtest/testDeployProcessArchive.bpmn20.xml");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPurgeDatabase()
	  public virtual void testPurgeDatabase()
	  {
		Assert.assertNotNull(processEngine);
		VariableMap variableMap = Variables.putValue("var", "value");
		runtimeService.startProcessInstanceByKey("testDeployProcessArchive", variableMap);
		runtimeService.startProcessInstanceByKey("testDeployProcessArchive", variableMap);

		ManagementServiceImpl managementServiceImpl = (ManagementServiceImpl) managementService;
		managementServiceImpl.purge();

		assertAndEnsureCleanDb(processEngine);
	  }

	  /// <summary>
	  /// Ensures that the database is clean after the test. This means the test has to remove
	  /// all resources it entered to the database.
	  /// If the DB is not clean, it is cleaned by performing a create a drop.
	  /// </summary>
	  /// <param name="processEngine"> the <seealso cref="ProcessEngine"/> to check </param>
	  /// <param name="fail"> if true the method will throw an <seealso cref="AssertionError"/> if the database is not clean </param>
	  /// <returns> the database summary if fail is set to false or null if database was clean </returns>
	  /// <exception cref="AssertionError"> if the database was not clean and fail is set to true </exception>
	  public static void assertAndEnsureCleanDb(ProcessEngine processEngine)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = ((ProcessEngineImpl) processEngine).ProcessEngineConfiguration;
		string databaseTablePrefix = processEngineConfiguration.DatabaseTablePrefix.Trim();

		IDictionary<string, long> tableCounts = processEngine.ManagementService.TableCount;

		StringBuilder outputMessage = new StringBuilder();
		foreach (string tableName in tableCounts.Keys)
		{
		  string tableNameWithoutPrefix = tableName.Replace(databaseTablePrefix, "");
		  if (!TABLENAMES_EXCLUDED_FROM_DB_CLEAN_CHECK.Contains(tableNameWithoutPrefix))
		  {
			long? count = tableCounts[tableName];
			if (count != 0L)
			{
			  outputMessage.Append("\t").Append(tableName).Append(": ").Append(count).Append(" record(s)\n");
			}
		  }
		}

		if (outputMessage.Length > 0)
		{
		  outputMessage.Insert(0, "DB NOT CLEAN: \n");
		  /// <summary>
		  /// skip drop and recreate if a table prefix is used </summary>
		  if (databaseTablePrefix.Length == 0)
		  {
			processEngineConfiguration.CommandExecutorSchemaOperations.execute(new CommandAnonymousInnerClass());
		  }
		  Assert.fail(outputMessage.ToString());
		}
	  }

	  private class CommandAnonymousInnerClass : Command<object>
	  {
		  public object execute(CommandContext commandContext)
		  {
			PersistenceSession persistenceSession = commandContext.getSession(typeof(PersistenceSession));
			persistenceSession.dbSchemaDrop();
			persistenceSession.dbSchemaCreate();
			HistoryLevelSetupCommand.dbCreateHistoryLevel(commandContext);
			return null;
		  }
	  }

	}

}