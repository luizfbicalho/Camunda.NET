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
namespace org.camunda.bpm.engine.spring.test.configuration
{

	using Bean = org.springframework.context.annotation.Bean;
	using ComponentScan = org.springframework.context.annotation.ComponentScan;
	using Configuration = org.springframework.context.annotation.Configuration;
	using DataSourceTransactionManager = org.springframework.jdbc.datasource.DataSourceTransactionManager;
	using SimpleDriverDataSource = org.springframework.jdbc.datasource.SimpleDriverDataSource;
	using PlatformTransactionManager = org.springframework.transaction.PlatformTransactionManager;

	/// <summary>
	/// Base Java Config for the process engine that uses In-Memory database.
	/// 
	/// @author Philipp Ossler
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Configuration @ComponentScan("org.camunda.bpm.engine.spring.test.configuration") public class InMemProcessEngineConfiguration
	public class InMemProcessEngineConfiguration
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Bean public javax.sql.DataSource dataSource()
		public virtual DataSource dataSource()
		{
		SimpleDriverDataSource dataSource = new SimpleDriverDataSource();
		dataSource.DriverClass = typeof(org.h2.Driver);
		dataSource.Url = "jdbc:h2:mem:camunda-test;DB_CLOSE_DELAY=-1";
		dataSource.Username = "sa";
		dataSource.Password = "";
		return dataSource;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Bean public org.springframework.transaction.PlatformTransactionManager transactionManager()
	  public virtual PlatformTransactionManager transactionManager()
	  {
		return new DataSourceTransactionManager(dataSource());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Bean public org.camunda.bpm.engine.spring.SpringProcessEngineConfiguration processEngineConfiguration()
	  public virtual SpringProcessEngineConfiguration processEngineConfiguration()
	  {
		SpringProcessEngineConfiguration config = new SpringProcessEngineConfiguration();

		config.DataSource = dataSource();
		config.DatabaseSchemaUpdate = "true";

		config.TransactionManager = transactionManager();

		config.History = "audit";
		config.JobExecutorActivate = false;
		config.DbMetricsReporterActivate = false;

		return config;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Bean public org.camunda.bpm.engine.spring.ProcessEngineFactoryBean processEngine()
	  public virtual ProcessEngineFactoryBean processEngine()
	  {
		ProcessEngineFactoryBean factoryBean = new ProcessEngineFactoryBean();
		factoryBean.ProcessEngineConfiguration = processEngineConfiguration();
		return factoryBean;
	  }

	}

}