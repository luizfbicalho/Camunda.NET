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
	using ProcessEngineImpl = org.camunda.bpm.engine.impl.ProcessEngineImpl;
	using SchemaOperationsProcessEngineBuild = org.camunda.bpm.engine.impl.SchemaOperationsProcessEngineBuild;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using StandaloneInMemProcessEngineConfiguration = org.camunda.bpm.engine.impl.cfg.StandaloneInMemProcessEngineConfiguration;
	using TestHelper = org.camunda.bpm.engine.impl.test.TestHelper;
	using After = org.junit.After;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class DmnDisabledTest
	{

	  protected internal static ProcessEngineImpl processEngineImpl;

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
//ORIGINAL LINE: @Test public void disabledDmn()
	  public virtual void disabledDmn()
	  {
		processEngineImpl = createProcessEngineImpl(false);

		// simulate manual schema creation by user
		TestHelper.createSchema(processEngineImpl.ProcessEngineConfiguration);

		// let the engine do their schema operations thing
		processEngineImpl.ProcessEngineConfiguration.CommandExecutorSchemaOperations.execute(new SchemaOperationsProcessEngineBuild());
	  }

	  // allows to return a process engine configuration which doesn't create a schema when it's build.
	  protected internal class CustomStandaloneInMemProcessEngineConfiguration : StandaloneInMemProcessEngineConfiguration
	  {

		public override ProcessEngine buildProcessEngine()
		{
		  init();
		  return new CreateNoSchemaProcessEngineImpl(this);
		}
	  }

	  protected internal class CreateNoSchemaProcessEngineImpl : ProcessEngineImpl
	  {

		public CreateNoSchemaProcessEngineImpl(ProcessEngineConfigurationImpl processEngineConfiguration) : base(processEngineConfiguration)
		{
		}

		protected internal override void executeSchemaOperations()
		{
		  // noop - do not execute create schema operations
		}
	  }

	  protected internal static ProcessEngineImpl createProcessEngineImpl(bool dmnEnabled)
	  {
		StandaloneInMemProcessEngineConfiguration config = (StandaloneInMemProcessEngineConfiguration) (new CustomStandaloneInMemProcessEngineConfiguration()).setProcessEngineName("database-dmn-test-engine").setDatabaseSchemaUpdate("false").setHistory(ProcessEngineConfiguration.HISTORY_FULL).setJdbcUrl("jdbc:h2:mem:DatabaseDmnTest");

		config.DmnEnabled = dmnEnabled;

		return (ProcessEngineImpl) config.buildProcessEngine();
	  }

	}

}