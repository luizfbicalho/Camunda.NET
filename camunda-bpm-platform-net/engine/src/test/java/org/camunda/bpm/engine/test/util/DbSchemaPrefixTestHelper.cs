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
namespace org.camunda.bpm.engine.test.util
{

	using PooledDataSource = org.apache.ibatis.datasource.pooled.PooledDataSource;
	using ProcessEngineImpl = org.camunda.bpm.engine.impl.ProcessEngineImpl;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using StandaloneInMemProcessEngineConfiguration = org.camunda.bpm.engine.impl.cfg.StandaloneInMemProcessEngineConfiguration;
	using ReflectUtil = org.camunda.bpm.engine.impl.util.ReflectUtil;
	using DisposableBean = org.springframework.beans.factory.DisposableBean;
	using InitializingBean = org.springframework.beans.factory.InitializingBean;

	/// <summary>
	/// <para>Test utility allowing to run the testsuite with a database
	/// table prefix</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class DbSchemaPrefixTestHelper : InitializingBean, DisposableBean
	{

	  private PooledDataSource dataSource;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void afterPropertiesSet() throws Exception
	  public virtual void afterPropertiesSet()
	  {

		dataSource = new PooledDataSource(ReflectUtil.ClassLoader, "org.h2.Driver", "jdbc:h2:mem:DatabaseTablePrefixTest;DB_CLOSE_DELAY=1000;MVCC=TRUE;", "sa", "");

		// create schema in the
		Connection connection = dataSource.Connection;
		connection.createStatement().execute("drop schema if exists SCHEMA1");
		connection.createStatement().execute("create schema SCHEMA1");
		connection.close();

		ProcessEngineConfigurationImpl config1 = createCustomProcessEngineConfiguration().setProcessEngineName("DatabaseTablePrefixTest-engine1").setDataSource(dataSource).setDbMetricsReporterActivate(false).setDatabaseSchemaUpdate("NO_CHECK"); // disable auto create/drop schema
		config1.DatabaseTablePrefix = "SCHEMA1.";
		ProcessEngine engine1 = config1.buildProcessEngine();

		// create the tables in SCHEMA1
		connection = dataSource.Connection;
		connection.createStatement().execute("set schema SCHEMA1");
		engine1.ManagementService.databaseSchemaUpgrade(connection, "", "SCHEMA1");
		connection.close();

		engine1.close();

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void destroy() throws Exception
	  public virtual void destroy()
	  {
		Connection connection = dataSource.Connection;
		connection.createStatement().execute("drop schema if exists SCHEMA1");
		connection.close();
	  }

	  //----------------------- TEST HELPERS -----------------------

	  // allows to return a process engine configuration which doesn't create a schema when it's build.
	  private class CustomStandaloneInMemProcessEngineConfiguration : StandaloneInMemProcessEngineConfiguration
	  {

		public override ProcessEngine buildProcessEngine()
		{
		  init();
		  return new NoSchemaProcessEngineImpl(this, this);
		}

		internal class NoSchemaProcessEngineImpl : ProcessEngineImpl
		{
			private readonly DbSchemaPrefixTestHelper.CustomStandaloneInMemProcessEngineConfiguration outerInstance;

		  public NoSchemaProcessEngineImpl(DbSchemaPrefixTestHelper.CustomStandaloneInMemProcessEngineConfiguration outerInstance, ProcessEngineConfigurationImpl processEngineConfiguration) : base(processEngineConfiguration)
		  {
			  this.outerInstance = outerInstance;
		  }

		  protected internal override void executeSchemaOperations()
		  {
			// nop - do not execute create schema operations
		  }
		}

	  }

	  private static ProcessEngineConfigurationImpl createCustomProcessEngineConfiguration()
	  {
		return (new CustomStandaloneInMemProcessEngineConfiguration()).setHistory(ProcessEngineConfiguration.HISTORY_FULL);
	  }

	}

}