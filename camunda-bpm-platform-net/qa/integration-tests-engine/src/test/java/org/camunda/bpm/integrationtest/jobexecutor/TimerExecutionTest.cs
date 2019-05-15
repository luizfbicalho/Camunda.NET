﻿using System.Collections.Generic;

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
namespace org.camunda.bpm.integrationtest.jobexecutor
{

	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using SampleServiceBean = org.camunda.bpm.integrationtest.jobexecutor.beans.SampleServiceBean;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

	/// 
	/// <summary>
	/// @author nico.rehwaldt
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class TimerExecutionTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class TimerExecutionTest : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive processArchive()
		public static WebArchive processArchive()
		{
		WebArchive archive = initWebArchiveDeployment().addAsResource("org/camunda/bpm/integrationtest/jobexecutor/TimerExecution.bpmn20.xml").addClass(typeof(SampleServiceBean));

		return archive;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessExecution()
	  public virtual void testProcessExecution()
	  {

		ProcessInstance instance = runtimeService.startProcessInstanceByKey("TimerExecutionProcess");

		waitForJobExecutorToProcessAllJobs();

		IList<ProcessInstance> finallyRunningInstances = runtimeService.createProcessInstanceQuery().processInstanceId(instance.Id).list();
		Assert.assertEquals(0, finallyRunningInstances.Count);

	  }
	}

}