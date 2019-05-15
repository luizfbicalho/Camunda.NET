using System;

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
namespace org.camunda.bpm.qa.upgrade.scenarios.job
{

	using SqlSession = org.apache.ibatis.session.SqlSession;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using DbSqlSessionFactory = org.camunda.bpm.engine.impl.db.sql.DbSqlSessionFactory;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using TimerStartEventJobHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerStartEventJobHandler;

	/// <summary>
	/// This actually simulates creation of a job in Camunda 7.0;
	/// we use 7.3
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class JobMigrationScenario
	{

	  [DescribesScenario("createJob")]
	  public static ScenarioSetup triggerEntryCriterion()
	  {
		return new ScenarioSetupAnonymousInnerClass();
	  }

	  private class ScenarioSetupAnonymousInnerClass : ScenarioSetup
	  {
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void execute(org.camunda.bpm.engine.ProcessEngine engine, final String scenarioName)
		  public void execute(ProcessEngine engine, string scenarioName)
		  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl engineConfiguration = (org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl) engine.getProcessEngineConfiguration();
			ProcessEngineConfigurationImpl engineConfiguration = (ProcessEngineConfigurationImpl) engine.ProcessEngineConfiguration;
			CommandExecutor commandExecutor = engineConfiguration.CommandExecutorTxRequired;

			// create a job with the scenario name as id and a null suspension state
			commandExecutor.execute(new CommandAnonymousInnerClass(this, scenarioName, engineConfiguration));

		  }

		  private class CommandAnonymousInnerClass : Command<Void>
		  {
			  private readonly ScenarioSetupAnonymousInnerClass outerInstance;

			  private string scenarioName;
			  private ProcessEngineConfigurationImpl engineConfiguration;

			  public CommandAnonymousInnerClass(ScenarioSetupAnonymousInnerClass outerInstance, string scenarioName, ProcessEngineConfigurationImpl engineConfiguration)
			  {
				  this.outerInstance = outerInstance;
				  this.scenarioName = scenarioName;
				  this.engineConfiguration = engineConfiguration;
			  }

			  public Void execute(CommandContext commandContext)
			  {
				Connection connection = null;
				Statement statement = null;
				ResultSet rs = null;

				try
				{
				  SqlSession sqlSession = commandContext.DbSqlSession.SqlSession;
				  connection = sqlSession.Connection;
				  statement = connection.createStatement();
				  statement.executeUpdate("INSERT INTO ACT_RU_JOB(ID_, REV_, RETRIES_, TYPE_, EXCLUSIVE_, HANDLER_TYPE_) " + "VALUES (" + "'" + scenarioName + "'," + "1," + "3," + "'timer'," + DbSqlSessionFactory.databaseSpecificTrueConstant[engineConfiguration.DatabaseType] + "," + "'" + TimerStartEventJobHandler.TYPE + "'" + ")");
				  connection.commit();
				  statement.close();
				}
				catch (SQLException e)
				{
				  throw new Exception(e);
				}
				finally
				{
				  try
				  {
					if (statement != null)
					{
					  statement.close();
					}
					if (rs != null)
					{
					  rs.close();
					}
					if (connection != null)
					{
					  connection.close();
					}
				  }
				  catch (SQLException e)
				  {
					throw new Exception(e);
				  }
				}
				return null;
			  }
		  }
	  }
	}

}