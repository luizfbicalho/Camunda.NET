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
//	import static org.junit.Assert.*;

	using PooledDataSource = org.apache.ibatis.datasource.pooled.PooledDataSource;
	using ProcessEngineImpl = org.camunda.bpm.engine.impl.ProcessEngineImpl;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using StandaloneInMemProcessEngineConfiguration = org.camunda.bpm.engine.impl.cfg.StandaloneInMemProcessEngineConfiguration;
	using ReflectUtil = org.camunda.bpm.engine.impl.util.ReflectUtil;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class DatabaseTablePrefixTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPerformDatabaseSchemaOperationCreate() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void shouldPerformDatabaseSchemaOperationCreate()
	  {

		// both process engines will be using this datasource.
		PooledDataSource pooledDataSource = new PooledDataSource(ReflectUtil.ClassLoader, "org.h2.Driver", "jdbc:h2:mem:DatabaseTablePrefixTest;DB_CLOSE_DELAY=1000", "sa", "");

		// create two schemas is the database
		Connection connection = pooledDataSource.Connection;
		connection.createStatement().execute("drop schema if exists SCHEMA1");
		connection.createStatement().execute("drop schema if exists SCHEMA2");
		connection.createStatement().execute("create schema SCHEMA1");
		connection.createStatement().execute("create schema SCHEMA2");
		connection.close();

		// configure & build two different process engines, each having a separate table prefix
		ProcessEngineConfigurationImpl config1 = createCustomProcessEngineConfiguration().setProcessEngineName("DatabaseTablePrefixTest-engine1").setDataSource(pooledDataSource).setDbMetricsReporterActivate(false).setDatabaseSchemaUpdate("NO_CHECK"); // disable auto create/drop schema
		config1.DatabaseTablePrefix = "SCHEMA1.";
		config1.UseSharedSqlSessionFactory = true;
		ProcessEngine engine1 = config1.buildProcessEngine();

		ProcessEngineConfigurationImpl config2 = createCustomProcessEngineConfiguration().setProcessEngineName("DatabaseTablePrefixTest-engine2").setDataSource(pooledDataSource).setDbMetricsReporterActivate(false).setDatabaseSchemaUpdate("NO_CHECK"); // disable auto create/drop schema
		config2.DatabaseTablePrefix = "SCHEMA2.";
		config2.UseSharedSqlSessionFactory = true;
		ProcessEngine engine2 = config2.buildProcessEngine();

		// create the tables in SCHEMA1
		connection = pooledDataSource.Connection;
		connection.createStatement().execute("set schema SCHEMA1");
		engine1.ManagementService.databaseSchemaUpgrade(connection, "", "SCHEMA1");
		connection.close();

		// create the tables in SCHEMA2
		connection = pooledDataSource.Connection;
		connection.createStatement().execute("set schema SCHEMA2");
		engine2.ManagementService.databaseSchemaUpgrade(connection, "", "SCHEMA2");
		connection.close();

		// if I deploy a process to one engine, it is not visible to the other
		// engine:
		try
		{
		  engine1.RepositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/cfg/oneJobProcess.bpmn20.xml").deploy();

		  assertEquals(1, engine1.RepositoryService.createDeploymentQuery().count());
		  assertEquals(0, engine2.RepositoryService.createDeploymentQuery().count());

		}
		finally
		{
		  engine1.close();
		  engine2.close();
		  ProcessEngineConfigurationImpl.cachedSqlSessionFactory = null;
		}
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
			private readonly DatabaseTablePrefixTest.CustomStandaloneInMemProcessEngineConfiguration outerInstance;

		  public NoSchemaProcessEngineImpl(DatabaseTablePrefixTest.CustomStandaloneInMemProcessEngineConfiguration outerInstance, ProcessEngineConfigurationImpl processEngineConfiguration) : base(processEngineConfiguration)
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