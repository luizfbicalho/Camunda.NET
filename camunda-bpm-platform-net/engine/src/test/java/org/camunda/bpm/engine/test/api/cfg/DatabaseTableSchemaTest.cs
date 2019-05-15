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

	using TestCase = junit.framework.TestCase;

	using PooledDataSource = org.apache.ibatis.datasource.pooled.PooledDataSource;
	using ProcessEngineImpl = org.camunda.bpm.engine.impl.ProcessEngineImpl;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using StandaloneInMemProcessEngineConfiguration = org.camunda.bpm.engine.impl.cfg.StandaloneInMemProcessEngineConfiguration;
	using DbSqlSession = org.camunda.bpm.engine.impl.db.sql.DbSqlSession;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using ReflectUtil = org.camunda.bpm.engine.impl.util.ReflectUtil;

	/// <summary>
	/// @author Ronny Bräunlich
	/// </summary>
	public class DatabaseTableSchemaTest : TestCase
	{

	  private const string SCHEMA_NAME = "SCHEMA1";
	  private const string PREFIX_NAME = "PREFIX1_";

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testPerformDatabaseSchemaOperationCreateTwice() throws Exception
	  public virtual void testPerformDatabaseSchemaOperationCreateTwice()
	  {

		// both process engines will be using this datasource.
		PooledDataSource pooledDataSource = new PooledDataSource(ReflectUtil.ClassLoader, "org.h2.Driver", "jdbc:h2:mem:DatabaseTablePrefixTest;DB_CLOSE_DELAY=1000", "sa", "");

		Connection connection = pooledDataSource.Connection;
		connection.createStatement().execute("drop schema if exists " + SCHEMA_NAME);
		connection.createStatement().execute("create schema " + SCHEMA_NAME);
		connection.close();

		ProcessEngineConfigurationImpl config1 = createCustomProcessEngineConfiguration().setProcessEngineName("DatabaseTablePrefixTest-engine1").setDataSource(pooledDataSource).setDatabaseSchemaUpdate("NO_CHECK");
		config1.DatabaseTablePrefix = SCHEMA_NAME + ".";
		config1.DatabaseSchema = SCHEMA_NAME;
		config1.DbMetricsReporterActivate = false;
		ProcessEngine engine1 = config1.buildProcessEngine();

		// create the tables for the first time
		connection = pooledDataSource.Connection;
		connection.createStatement().execute("set schema " + SCHEMA_NAME);
		engine1.ManagementService.databaseSchemaUpgrade(connection, "", SCHEMA_NAME);
		connection.close();
		// create the tables for the second time; here we shouldn't crash since the
		// session should tell us that the tables are already present and
		// databaseSchemaUpdate is set to noop
		connection = pooledDataSource.Connection;
		connection.createStatement().execute("set schema " + SCHEMA_NAME);
		engine1.ManagementService.databaseSchemaUpgrade(connection, "", SCHEMA_NAME);
		engine1.close();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testTablePresentWithSchemaAndPrefix() throws java.sql.SQLException
	  public virtual void testTablePresentWithSchemaAndPrefix()
	  {
		PooledDataSource pooledDataSource = new PooledDataSource(ReflectUtil.ClassLoader, "org.h2.Driver", "jdbc:h2:mem:DatabaseTablePrefixTest;DB_CLOSE_DELAY=1000", "sa", "");

		Connection connection = pooledDataSource.Connection;
		connection.createStatement().execute("drop schema if exists " + SCHEMA_NAME);
		connection.createStatement().execute("create schema " + SCHEMA_NAME);
		connection.createStatement().execute("create table " + SCHEMA_NAME + "." + PREFIX_NAME + "SOME_TABLE(id varchar(64));");
		connection.close();

		ProcessEngineConfigurationImpl config1 = createCustomProcessEngineConfiguration().setProcessEngineName("DatabaseTablePrefixTest-engine1").setDataSource(pooledDataSource).setDatabaseSchemaUpdate("NO_CHECK");
		config1.DatabaseTablePrefix = SCHEMA_NAME + "." + PREFIX_NAME;
		config1.DatabaseSchema = SCHEMA_NAME;
		config1.DbMetricsReporterActivate = false;
		ProcessEngine engine = config1.buildProcessEngine();
		CommandExecutor commandExecutor = config1.CommandExecutorTxRequired;

		commandExecutor.execute(new CommandAnonymousInnerClass(this));

		engine.close();

	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly DatabaseTableSchemaTest outerInstance;

		  public CommandAnonymousInnerClass(DatabaseTableSchemaTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			DbSqlSession sqlSession = commandContext.getSession(typeof(DbSqlSession));
			assertTrue(sqlSession.isTablePresent("SOME_TABLE"));
			return null;
		  }
	  }

	  public virtual void testCreateConfigurationWithMismatchtingSchemaAndPrefix()
	  {
		try
		{
		  StandaloneInMemProcessEngineConfiguration configuration = new StandaloneInMemProcessEngineConfiguration();
		  configuration.DatabaseSchema = "foo";
		  configuration.DatabaseTablePrefix = "bar";
		  configuration.buildProcessEngine();
		  fail("Should throw exception");
		}
		catch (ProcessEngineException e)
		{
		  // as expected
		  assertTrue(e.Message.contains("When setting a schema the prefix has to be schema + '.'"));
		}
	  }

	  public virtual void testCreateConfigurationWithMissingDotInSchemaAndPrefix()
	  {
		try
		{
		  StandaloneInMemProcessEngineConfiguration configuration = new StandaloneInMemProcessEngineConfiguration();
		  configuration.DatabaseSchema = "foo";
		  configuration.DatabaseTablePrefix = "foo";
		  configuration.buildProcessEngine();
		  fail("Should throw exception");
		}
		catch (ProcessEngineException e)
		{
		  // as expected
		  assertTrue(e.Message.contains("When setting a schema the prefix has to be schema + '.'"));
		}
	  }

	  // ----------------------- TEST HELPERS -----------------------

	  // allows to return a process engine configuration which doesn't create a
	  // schema when it's build.
	  private class CustomStandaloneInMemProcessEngineConfiguration : StandaloneInMemProcessEngineConfiguration
	  {

		public override ProcessEngine buildProcessEngine()
		{
		  init();
		  return new NoSchemaProcessEngineImpl(this, this);
		}

		internal class NoSchemaProcessEngineImpl : ProcessEngineImpl
		{
			private readonly DatabaseTableSchemaTest.CustomStandaloneInMemProcessEngineConfiguration outerInstance;

		  public NoSchemaProcessEngineImpl(DatabaseTableSchemaTest.CustomStandaloneInMemProcessEngineConfiguration outerInstance, ProcessEngineConfigurationImpl processEngineConfiguration) : base(processEngineConfiguration)
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