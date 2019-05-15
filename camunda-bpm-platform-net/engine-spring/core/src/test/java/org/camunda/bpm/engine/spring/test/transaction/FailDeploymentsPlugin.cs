﻿/*
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
namespace org.camunda.bpm.engine.spring.test.transaction
{
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ProcessEnginePlugin = org.camunda.bpm.engine.impl.cfg.ProcessEnginePlugin;
	using CommandInterceptor = org.camunda.bpm.engine.impl.interceptor.CommandInterceptor;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class FailDeploymentsPlugin : ProcessEnginePlugin
	{

	  public virtual void preInit(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
		processEngineConfiguration.CustomPostCommandInterceptorsTxRequired = Arrays.asList<CommandInterceptor>(new FailDeploymentCommandInterceptor());
		processEngineConfiguration.CustomPostCommandInterceptorsTxRequiresNew = Arrays.asList<CommandInterceptor>(new FailDeploymentCommandInterceptor());
	  }

	  public virtual void postInit(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
	  }

	  public virtual void postProcessEngineBuild(ProcessEngine processEngine)
	  {
	  }

	}

}