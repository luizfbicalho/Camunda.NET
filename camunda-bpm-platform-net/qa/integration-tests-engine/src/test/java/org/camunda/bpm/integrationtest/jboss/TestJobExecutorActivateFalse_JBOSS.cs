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
namespace org.camunda.bpm.integrationtest.jboss
{
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using ProcessEngineConfiguration = org.camunda.bpm.engine.ProcessEngineConfiguration;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using JobExecutor = org.camunda.bpm.engine.impl.jobexecutor.JobExecutor;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.*;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class TestJobExecutorActivateFalse_JBOSS extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class TestJobExecutorActivateFalse_JBOSS : AbstractFoxPlatformIntegrationTest
	{
		[Deployment(name:"deployment1")]
		public static WebArchive processArchive()
		{
		return initWebArchiveDeployment("test.war", "jobExecutorActivate-processes.xml");
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotActiateJobExecutor()
	  public virtual void shouldNotActiateJobExecutor()
	  {

		ProcessEngine processEngine = processEngineService.getProcessEngine("jobExecutorActivate-FALSE-engine");
		ProcessEngineConfiguration configuration = processEngine.ProcessEngineConfiguration;
		JobExecutor jobExecutor = ((ProcessEngineConfigurationImpl)configuration).JobExecutor;
		assertFalse(jobExecutor.Active);

		processEngine = processEngineService.getProcessEngine("jobExecutorActivate-UNDEFINED-engine");
		configuration = processEngine.ProcessEngineConfiguration;
		jobExecutor = ((ProcessEngineConfigurationImpl)configuration).JobExecutor;
		assertTrue(jobExecutor.Active);

	  }
	}

}