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
namespace org.camunda.bpm.engine.spring.test
{
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using StandaloneInMemProcessEngineConfiguration = org.camunda.bpm.engine.impl.cfg.StandaloneInMemProcessEngineConfiguration;
	using Bean = org.springframework.context.annotation.Bean;
	using Configuration = org.springframework.context.annotation.Configuration;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Configuration public class ExampleConfiguration
	public class ExampleConfiguration
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Bean public org.camunda.bpm.engine.ProcessEngineConfiguration processEngineConfiguration()
		public virtual ProcessEngineConfiguration processEngineConfiguration()
		{
		return (new StandaloneInMemProcessEngineConfiguration()).setJobExecutorDeploymentAware(true);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Bean public org.camunda.bpm.engine.spring.ProcessEngineFactoryBean processEngine() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual ProcessEngineFactoryBean processEngine()
	  {
		ProcessEngineFactoryBean engineFactoryBean = new ProcessEngineFactoryBean();
		engineFactoryBean.ProcessEngineConfiguration = (ProcessEngineConfigurationImpl) processEngineConfiguration();

		return engineFactoryBean;
	  }
	}

}