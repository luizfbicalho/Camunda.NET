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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using HistoryLevelSetupCommand = org.camunda.bpm.engine.impl.HistoryLevelSetupCommand;
	using ProcessEngineImpl = org.camunda.bpm.engine.impl.ProcessEngineImpl;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using StandaloneInMemProcessEngineConfiguration = org.camunda.bpm.engine.impl.cfg.StandaloneInMemProcessEngineConfiguration;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using After = org.junit.After;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;

	public class DatabaseHistoryPropertyAutoTest
	{

	  protected internal IList<ProcessEngineImpl> processEngines = new List<ProcessEngineImpl>();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public final org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public readonly ExpectedException thrown = ExpectedException.none();

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
//ORIGINAL LINE: @Test public void failWhenSecondEngineDoesNotHaveTheSameHistoryLevel()
	  public virtual void failWhenSecondEngineDoesNotHaveTheSameHistoryLevel()
	  {
		buildEngine(config("true", ProcessEngineConfiguration.HISTORY_FULL));

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("historyLevel mismatch: configuration says HistoryLevelAudit(name=audit, id=2) and database says HistoryLevelFull(name=full, id=3)");

		buildEngine(config(ProcessEngineConfiguration.HISTORY_AUDIT));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void secondEngineCopiesHistoryLevelFromFirst()
	  public virtual void secondEngineCopiesHistoryLevelFromFirst()
	  {
		// given
		buildEngine(config("true", ProcessEngineConfiguration.HISTORY_FULL));

		// when
		ProcessEngineImpl processEngineTwo = buildEngine(config("true", ProcessEngineConfiguration.HISTORY_AUTO));

		// then
		assertThat(processEngineTwo.ProcessEngineConfiguration.History, @is(ProcessEngineConfiguration.HISTORY_AUTO));
		assertThat(processEngineTwo.ProcessEngineConfiguration.HistoryLevel, @is(org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_FULL));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void usesDefaultValueAuditWhenNoValueIsConfigured()
	  public virtual void usesDefaultValueAuditWhenNoValueIsConfigured()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl config = config("true", org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_AUTO);
		ProcessEngineConfigurationImpl config = config("true", ProcessEngineConfiguration.HISTORY_AUTO);
		ProcessEngineImpl processEngine = buildEngine(config);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final System.Nullable<int> level = config.getCommandExecutorSchemaOperations().execute(new org.camunda.bpm.engine.impl.interceptor.Command<int>()
		int? level = config.CommandExecutorSchemaOperations.execute(new CommandAnonymousInnerClass(this));

		assertThat(level, equalTo(org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_AUDIT.Id));

		assertThat(processEngine.ProcessEngineConfiguration.HistoryLevel, equalTo(org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_AUDIT));
	  }

	  private class CommandAnonymousInnerClass : Command<int>
	  {
		  private readonly DatabaseHistoryPropertyAutoTest outerInstance;

		  public CommandAnonymousInnerClass(DatabaseHistoryPropertyAutoTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public int? execute(CommandContext commandContext)
		  {
			return HistoryLevelSetupCommand.databaseHistoryLevel(commandContext);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void after()
	  public virtual void after()
	  {
		foreach (ProcessEngineImpl engine in processEngines)
		{
		  // no need to drop schema when testing with h2
		  engine.close();
		}

		processEngines.Clear();
	  }

	  protected internal virtual ProcessEngineImpl buildEngine(ProcessEngineConfigurationImpl engineConfiguration)
	  {
		ProcessEngineImpl engine = (ProcessEngineImpl) engineConfiguration.buildProcessEngine();
		processEngines.Add(engine);

		return engine;
	  }

	}

}