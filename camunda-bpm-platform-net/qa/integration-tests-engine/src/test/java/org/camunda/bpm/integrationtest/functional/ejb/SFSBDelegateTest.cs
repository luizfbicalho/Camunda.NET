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
namespace org.camunda.bpm.integrationtest.functional.ejb
{
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using SFSBClientDelegate = org.camunda.bpm.integrationtest.functional.ejb.beans.SFSBClientDelegate;
	using SFSBDelegate = org.camunda.bpm.integrationtest.functional.ejb.beans.SFSBDelegate;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;



	/// <summary>
	/// Testcase verifying various ways to use a SFSB as a JavaDelegate
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class SFSBDelegateTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class SFSBDelegateTest : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive processArchive()
		public static WebArchive processArchive()
		{
		return initWebArchiveDeployment().addClass(typeof(SFSBDelegate)).addClass(typeof(SFSBClientDelegate)).addAsResource("org/camunda/bpm/integrationtest/functional/ejb/SFSBDelegateTest.testBeanResolution.bpmn20.xml").addAsResource("org/camunda/bpm/integrationtest/functional/ejb/SFSBDelegateTest.testBeanResolutionFromClient.bpmn20.xml");
		}


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBeanResolution()
	  public virtual void testBeanResolution()
	  {

		// this testcase first resolves the SFSB synchronouly and then from the JobExecutor

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testBeanResolution");

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Assert.assertEquals(true, runtimeService.getVariable(pi.Id, typeof(SFSBDelegate).FullName));

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		runtimeService.setVariable(pi.Id, typeof(SFSBDelegate).FullName, false);

		taskService.complete(taskService.createTaskQuery().processInstanceId(pi.Id).singleResult().Id);

		waitForJobExecutorToProcessAllJobs();

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Assert.assertEquals(true, runtimeService.getVariable(pi.Id, typeof(SFSBDelegate).FullName));

		taskService.complete(taskService.createTaskQuery().processInstanceId(pi.Id).singleResult().Id);

	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBeanResolutionfromClient()
	  public virtual void testBeanResolutionfromClient()
	  {

		// this testcase invokes a CDI bean that injects the EJB

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testBeanResolutionfromClient");

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Assert.assertEquals(true, runtimeService.getVariable(pi.Id, typeof(SFSBDelegate).FullName));

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		runtimeService.setVariable(pi.Id, typeof(SFSBDelegate).FullName, false);

		taskService.complete(taskService.createTaskQuery().processInstanceId(pi.Id).singleResult().Id);

		waitForJobExecutorToProcessAllJobs();

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Assert.assertEquals(true, runtimeService.getVariable(pi.Id, typeof(SFSBDelegate).FullName));

		taskService.complete(taskService.createTaskQuery().processInstanceId(pi.Id).singleResult().Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMultipleInvocations()
	  public virtual void testMultipleInvocations()
	  {

		// this is greater than any Datasource- / EJB- / Thread-Pool size -> make sure all resources are released properly.
		int instances = 100;
		string[] ids = new string[instances];

		for (int i = 0; i < instances; i++)
		{
		  ids[i] = runtimeService.startProcessInstanceByKey("testBeanResolutionfromClient").Id;
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  Assert.assertEquals("Incovation=" + i, true, runtimeService.getVariable(ids[i], typeof(SFSBDelegate).FullName));
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  runtimeService.setVariable(ids[i], typeof(SFSBDelegate).FullName, false);
		  taskService.complete(taskService.createTaskQuery().processInstanceId(ids[i]).singleResult().Id);
		}

		waitForJobExecutorToProcessAllJobs(60 * 1000);

		for (int i = 0; i < instances; i++)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  Assert.assertEquals("Incovation=" + i, true, runtimeService.getVariable(ids[i], typeof(SFSBDelegate).FullName));
		  taskService.complete(taskService.createTaskQuery().processInstanceId(ids[i]).singleResult().Id);
		}

	  }

	}

}