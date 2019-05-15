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

	using PooledDataSource = org.apache.ibatis.datasource.pooled.PooledDataSource;
	using Configuration = org.apache.ibatis.session.Configuration;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using PvmTestCase = org.camunda.bpm.engine.impl.test.PvmTestCase;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class ConnectionPoolTest : PvmTestCase
	{

	  public virtual void testMyBatisConnectionPoolProperlyConfigured()
	  {
		ProcessEngineConfigurationImpl config = (ProcessEngineConfigurationImpl) ProcessEngineConfiguration.createProcessEngineConfigurationFromResource("org/camunda/bpm/engine/test/api/cfg/connection-pool.camunda.cfg.xml");

		ProcessEngine engine = config.buildProcessEngine();

		// Expected values
		int maxActive = 25;
		int maxIdle = 10;
		int maxCheckoutTime = 30000;
		int maxWaitTime = 25000;
		int? jdbcStatementTimeout = 300;

		assertEquals(maxActive, config.JdbcMaxActiveConnections);
		assertEquals(maxIdle, config.JdbcMaxIdleConnections);
		assertEquals(maxCheckoutTime, config.JdbcMaxCheckoutTime);
		assertEquals(maxWaitTime, config.JdbcMaxWaitTime);
		assertEquals(jdbcStatementTimeout, config.JdbcStatementTimeout);

		// Verify that these properties are correctly set in the MyBatis datasource
		Configuration sessionFactoryConfiguration = config.DbSqlSessionFactory.SqlSessionFactory.Configuration;
		DataSource datasource = sessionFactoryConfiguration.Environment.DataSource;
		assertTrue(datasource is PooledDataSource);

		PooledDataSource pooledDataSource = (PooledDataSource) datasource;
		assertEquals(maxActive, pooledDataSource.PoolMaximumActiveConnections);
		assertEquals(maxIdle, pooledDataSource.PoolMaximumIdleConnections);
		assertEquals(maxCheckoutTime, pooledDataSource.PoolMaximumCheckoutTime);
		assertEquals(maxWaitTime, pooledDataSource.PoolTimeToWait);

		assertEquals(jdbcStatementTimeout, sessionFactoryConfiguration.DefaultStatementTimeout);

		engine.close();
	  }

	}

}