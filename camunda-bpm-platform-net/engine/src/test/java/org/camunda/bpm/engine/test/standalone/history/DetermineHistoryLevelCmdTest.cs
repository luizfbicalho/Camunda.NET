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
namespace org.camunda.bpm.engine.test.standalone.history
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using ProcessEngineImpl = org.camunda.bpm.engine.impl.ProcessEngineImpl;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using StandaloneInMemProcessEngineConfiguration = org.camunda.bpm.engine.impl.cfg.StandaloneInMemProcessEngineConfiguration;
	using DetermineHistoryLevelCmd = org.camunda.bpm.engine.impl.cmd.DetermineHistoryLevelCmd;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using HistoryEventType = org.camunda.bpm.engine.impl.history.@event.HistoryEventType;
	using TestHelper = org.camunda.bpm.engine.impl.test.TestHelper;
	using CoreMatchers = org.hamcrest.CoreMatchers;
	using After = org.junit.After;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;

	public class DetermineHistoryLevelCmdTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public final org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public readonly ExpectedException thrown = ExpectedException.none();

	  private ProcessEngineImpl processEngineImpl;


//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private static org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl config(final String historyLevel)
	  private static ProcessEngineConfigurationImpl config(string historyLevel)
	  {
		return config("false", historyLevel);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private static org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl config(final String schemaUpdate, final String historyLevel)
	  private static ProcessEngineConfigurationImpl config(string schemaUpdate, string historyLevel)
	  {
		StandaloneInMemProcessEngineConfiguration engineConfiguration = new StandaloneInMemProcessEngineConfiguration();
		engineConfiguration.ProcessEngineName = System.Guid.randomUUID().ToString();
		engineConfiguration.DatabaseSchemaUpdate = schemaUpdate;
		engineConfiguration.History = historyLevel;
		engineConfiguration.DbMetricsReporterActivate = false;
		engineConfiguration.JdbcUrl = "jdbc:h2:mem:DatabaseHistoryPropertyAutoTest";

		return engineConfiguration;
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void readLevelFullfromDB() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void readLevelFullfromDB()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl config = config("true", org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL);
		ProcessEngineConfigurationImpl config = config("true", ProcessEngineConfiguration.HISTORY_FULL);

		// init the db with level=full
		processEngineImpl = (ProcessEngineImpl) config.buildProcessEngine();

		HistoryLevel historyLevel = config.CommandExecutorSchemaOperations.execute(new DetermineHistoryLevelCmd(config.HistoryLevels));

		assertThat(historyLevel, CoreMatchers.equalTo(org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_FULL));
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void useDefaultLevelAudit() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void useDefaultLevelAudit()
	  {
		ProcessEngineConfigurationImpl config = config("true", ProcessEngineConfiguration.HISTORY_AUTO);

		// init the db with level=auto -> audit
		processEngineImpl = (ProcessEngineImpl) config.buildProcessEngine();
		// the history Level has been overwritten with audit
		assertThat(config.HistoryLevel, CoreMatchers.equalTo(org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_AUDIT));

		// and this is written to the database
		HistoryLevel databaseLevel = config.CommandExecutorSchemaOperations.execute(new DetermineHistoryLevelCmd(config.HistoryLevels));
		assertThat(databaseLevel, CoreMatchers.equalTo(org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_AUDIT));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failWhenExistingHistoryLevelIsNotRegistered()
	  public virtual void failWhenExistingHistoryLevelIsNotRegistered()
	  {
		// init the db with custom level
		HistoryLevel customLevel = new HistoryLevelAnonymousInnerClass(this);
		ProcessEngineConfigurationImpl config = config("true", "custom");
		config.CustomHistoryLevels = Arrays.asList(customLevel);
		processEngineImpl = (ProcessEngineImpl) config.buildProcessEngine();

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("The configured history level with id='99' is not registered in this config.");

		config.CommandExecutorSchemaOperations.execute(new DetermineHistoryLevelCmd(System.Linq.Enumerable.Empty<HistoryLevel>()));
	  }

	  private class HistoryLevelAnonymousInnerClass : HistoryLevel
	  {
		  private readonly DetermineHistoryLevelCmdTest outerInstance;

		  public HistoryLevelAnonymousInnerClass(DetermineHistoryLevelCmdTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public int Id
		  {
			  get
			  {
				return 99;
			  }
		  }

		  public string Name
		  {
			  get
			  {
				return "custom";
			  }
		  }

		  public bool isHistoryEventProduced(HistoryEventType eventType, object entity)
		  {
			return false;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void after()
	  public virtual void after()
	  {
		TestHelper.dropSchema(processEngineImpl.ProcessEngineConfiguration);
		processEngineImpl.close();
		processEngineImpl = null;
	  }
	}
}