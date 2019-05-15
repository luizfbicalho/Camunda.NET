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
	using SqlSessionFactory = org.apache.ibatis.session.SqlSessionFactory;
	using TransactionFactory = org.apache.ibatis.transaction.TransactionFactory;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using StandaloneInMemProcessEngineConfiguration = org.camunda.bpm.engine.impl.cfg.StandaloneInMemProcessEngineConfiguration;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Test = org.junit.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.*;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.*;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class SharedSqlSessionFactoryCfgTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before @After public void cleanCachedSessionFactory()
	  public virtual void cleanCachedSessionFactory()
	  {
		ProcessEngineConfigurationImpl.cachedSqlSessionFactory = null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotReuseSqlSessionFactoryByDefault()
	  public virtual void shouldNotReuseSqlSessionFactoryByDefault()
	  {
		assertFalse((new StandaloneInMemProcessEngineConfiguration()).UseSharedSqlSessionFactory);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCacheDbSqlSessionFactoryIfConfigured()
	  public virtual void shouldCacheDbSqlSessionFactoryIfConfigured()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TestEngineCfg cfg = new TestEngineCfg();
		TestEngineCfg cfg = new TestEngineCfg();

		// given
		cfg.UseSharedSqlSessionFactory = true;

		// if
		cfg.initSqlSessionFactory();

		// then
		assertNotNull(ProcessEngineConfigurationImpl.cachedSqlSessionFactory);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotCacheDbSqlSessionFactoryIfNotConfigured()
	  public virtual void shouldNotCacheDbSqlSessionFactoryIfNotConfigured()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TestEngineCfg cfg = new TestEngineCfg();
		TestEngineCfg cfg = new TestEngineCfg();

		// if
		cfg.initSqlSessionFactory();

		// then
		assertNull(ProcessEngineConfigurationImpl.cachedSqlSessionFactory);
		assertNotNull(cfg.SqlSessionFactory);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldReuseCachedSqlSessionFactoryIfConfigured()
	  public virtual void shouldReuseCachedSqlSessionFactoryIfConfigured()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TestEngineCfg cfg = new TestEngineCfg();
		TestEngineCfg cfg = new TestEngineCfg();
		SqlSessionFactory existingSessionFactory = mock(typeof(SqlSessionFactory));

		// given
		ProcessEngineConfigurationImpl.cachedSqlSessionFactory = existingSessionFactory;
		cfg.UseSharedSqlSessionFactory = true;

		// if
		cfg.initSqlSessionFactory();

		// then
		assertSame(existingSessionFactory, ProcessEngineConfigurationImpl.cachedSqlSessionFactory);
		assertSame(existingSessionFactory, cfg.SqlSessionFactory);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotReuseCachedSqlSessionIfNotConfigured()
	  public virtual void shouldNotReuseCachedSqlSessionIfNotConfigured()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TestEngineCfg cfg = new TestEngineCfg();
		TestEngineCfg cfg = new TestEngineCfg();
		SqlSessionFactory existingSessionFactory = mock(typeof(SqlSessionFactory));

		// given
		ProcessEngineConfigurationImpl.cachedSqlSessionFactory = existingSessionFactory;

		// if
		cfg.initSqlSessionFactory();

		// then
		assertSame(existingSessionFactory, ProcessEngineConfigurationImpl.cachedSqlSessionFactory);
		assertNotSame(existingSessionFactory, cfg.SqlSessionFactory);
	  }

	  internal class TestEngineCfg : StandaloneInMemProcessEngineConfiguration
	  {

		public TestEngineCfg()
		{
		  dataSource = mock(typeof(DataSource));
		  transactionFactory = mock(typeof(TransactionFactory));
		}

		public override void initSqlSessionFactory()
		{
		  base.initSqlSessionFactory();
		}

		public override SqlSessionFactory SqlSessionFactory
		{
			get
			{
			  return base.SqlSessionFactory;
			}
		}

	  }

	}

}