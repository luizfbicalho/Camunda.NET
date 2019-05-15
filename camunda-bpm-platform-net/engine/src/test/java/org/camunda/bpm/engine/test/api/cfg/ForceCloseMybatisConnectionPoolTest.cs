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
	using PoolState = org.apache.ibatis.datasource.pooled.PoolState;
	using PooledDataSource = org.apache.ibatis.datasource.pooled.PooledDataSource;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using StandaloneInMemProcessEngineConfiguration = org.camunda.bpm.engine.impl.cfg.StandaloneInMemProcessEngineConfiguration;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ForceCloseMybatisConnectionPoolTest
	{


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testForceCloseMybatisConnectionPoolTrue()
	  public virtual void testForceCloseMybatisConnectionPoolTrue()
	  {

		// given
		// that the process engine is configured with forceCloseMybatisConnectionPool = true
		ProcessEngineConfigurationImpl configurationImpl = (new StandaloneInMemProcessEngineConfiguration()).setJdbcUrl("jdbc:h2:mem:camunda-forceclose").setProcessEngineName("engine-forceclose").setForceCloseMybatisConnectionPool(true);

		ProcessEngine processEngine = configurationImpl.buildProcessEngine();

		PooledDataSource pooledDataSource = (PooledDataSource) configurationImpl.DataSource;
		PoolState state = pooledDataSource.PoolState;


		// then
		// if the process engine is closed
		processEngine.close();

		// the idle connections are closed
		Assert.assertTrue(state.IdleConnectionCount == 0);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testForceCloseMybatisConnectionPoolFalse()
	  public virtual void testForceCloseMybatisConnectionPoolFalse()
	  {

		// given
		// that the process engine is configured with forceCloseMybatisConnectionPool = false
		ProcessEngineConfigurationImpl configurationImpl = (new StandaloneInMemProcessEngineConfiguration()).setJdbcUrl("jdbc:h2:mem:camunda-forceclose").setProcessEngineName("engine-forceclose").setForceCloseMybatisConnectionPool(false);

		ProcessEngine processEngine = configurationImpl.buildProcessEngine();

		PooledDataSource pooledDataSource = (PooledDataSource) configurationImpl.DataSource;
		PoolState state = pooledDataSource.PoolState;
		int idleConnections = state.IdleConnectionCount;


		// then
		// if the process engine is closed
		processEngine.close();

		// the idle connections are not closed
		Assert.assertEquals(state.IdleConnectionCount, idleConnections);

		pooledDataSource.forceCloseAll();

		Assert.assertTrue(state.IdleConnectionCount == 0);
	  }

	}

}