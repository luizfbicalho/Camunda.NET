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
namespace org.camunda.bpm.engine.test.api.cfg
{

	using HistoryLevelSetupCommand = org.camunda.bpm.engine.impl.HistoryLevelSetupCommand;
	using ProcessEngineImpl = org.camunda.bpm.engine.impl.ProcessEngineImpl;
	using SchemaOperationsProcessEngineBuild = org.camunda.bpm.engine.impl.SchemaOperationsProcessEngineBuild;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using StandaloneInMemProcessEngineConfiguration = org.camunda.bpm.engine.impl.cfg.StandaloneInMemProcessEngineConfiguration;
	using TestHelper = org.camunda.bpm.engine.impl.test.TestHelper;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Christian Lipphardt
	/// </summary>
	public class DatabaseHistoryPropertyTest
	{


	  private static ProcessEngineImpl processEngineImpl;

	  // make sure schema is dropped
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanup()
	  public virtual void cleanup()
	  {
		TestHelper.dropSchema(processEngineImpl.ProcessEngineConfiguration);
		processEngineImpl.close();
		processEngineImpl = null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void schemaCreatedByEngineAndDatabaseSchemaUpdateTrue()
	  public virtual void schemaCreatedByEngineAndDatabaseSchemaUpdateTrue()
	  {
		processEngineImpl = createProcessEngineImpl("true", true);

		assertHistoryLevel();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void schemaCreatedByUserAndDatabaseSchemaUpdateTrue()
	  public virtual void schemaCreatedByUserAndDatabaseSchemaUpdateTrue()
	  {
		processEngineImpl = createProcessEngineImpl("true", false);
		// simulate manual schema creation by user
		TestHelper.createSchema(processEngineImpl.ProcessEngineConfiguration);

		// let the engine do their schema operations thing
		processEngineImpl.ProcessEngineConfiguration.CommandExecutorSchemaOperations.execute(new SchemaOperationsProcessEngineBuild());

		processEngineImpl.ProcessEngineConfiguration.CommandExecutorSchemaOperations.execute(new HistoryLevelSetupCommand());

		assertHistoryLevel();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void schemaCreatedByUserAndDatabaseSchemaUpdateFalse()
	  public virtual void schemaCreatedByUserAndDatabaseSchemaUpdateFalse()
	  {
		processEngineImpl = createProcessEngineImpl("false", false);
		// simulate manual schema creation by user
		TestHelper.createSchema(processEngineImpl.ProcessEngineConfiguration);

		// let the engine do their schema operations thing
		processEngineImpl.ProcessEngineConfiguration.CommandExecutorSchemaOperations.execute(new SchemaOperationsProcessEngineBuild());

		processEngineImpl.ProcessEngineConfiguration.CommandExecutorSchemaOperations.execute(new HistoryLevelSetupCommand());

		assertHistoryLevel();
	  }

	  private void assertHistoryLevel()
	  {
		IDictionary<string, string> properties = processEngineImpl.ManagementService.Properties;
		string historyLevel = properties["historyLevel"];
		Assert.assertNotNull("historyLevel is null -> not set in database", historyLevel);
		Assert.assertEquals(ProcessEngineConfigurationImpl.HISTORYLEVEL_FULL, int.Parse(historyLevel));
	  }


	  //----------------------- TEST HELPERS -----------------------

	  private class CreateSchemaProcessEngineImpl : ProcessEngineImpl
	  {
		public CreateSchemaProcessEngineImpl(ProcessEngineConfigurationImpl processEngineConfiguration) : base(processEngineConfiguration)
		{
		}

		protected internal override void executeSchemaOperations()
		{
		  base.executeSchemaOperations();
		}
	  }

	  private class CreateNoSchemaProcessEngineImpl : ProcessEngineImpl
	  {
		public CreateNoSchemaProcessEngineImpl(ProcessEngineConfigurationImpl processEngineConfiguration) : base(processEngineConfiguration)
		{
		}

		protected internal override void executeSchemaOperations()
		{
		  // nop - do not execute create schema operations
		}
	  }

	  // allows to return a process engine configuration which doesn't create a schema when it's build.
	  private class CustomStandaloneInMemProcessEngineConfiguration : StandaloneInMemProcessEngineConfiguration
	  {

		internal bool executeSchemaOperations;

		public override ProcessEngine buildProcessEngine()
		{
		  init();
		  if (executeSchemaOperations)
		  {
			return new CreateSchemaProcessEngineImpl(this);
		  }
		  else
		  {
			return new CreateNoSchemaProcessEngineImpl(this);
		  }
		}

		public virtual ProcessEngineConfigurationImpl setExecuteSchemaOperations(bool executeSchemaOperations)
		{
		  this.executeSchemaOperations = executeSchemaOperations;
		  return this;
		}
	  }

	  private static ProcessEngineImpl createProcessEngineImpl(string databaseSchemaUpdate, bool executeSchemaOperations)
	  {
		ProcessEngineImpl processEngine = (ProcessEngineImpl) (new CustomStandaloneInMemProcessEngineConfiguration()).setExecuteSchemaOperations(executeSchemaOperations).setProcessEngineName("database-history-test-engine").setDatabaseSchemaUpdate(databaseSchemaUpdate).setHistory(ProcessEngineConfiguration.HISTORY_FULL).setJdbcUrl("jdbc:h2:mem:DatabaseHistoryPropertyTest").buildProcessEngine();

		return processEngine;
	  }

	}

}