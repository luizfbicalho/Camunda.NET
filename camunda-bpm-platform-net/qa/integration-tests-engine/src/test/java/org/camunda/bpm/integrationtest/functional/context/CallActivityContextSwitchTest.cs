using System.Collections.Generic;

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
namespace org.camunda.bpm.integrationtest.functional.context
{


	using Assert = org.junit.Assert;

	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using CalledProcessDelegate = org.camunda.bpm.integrationtest.functional.context.beans.CalledProcessDelegate;
	using DelegateAfter = org.camunda.bpm.integrationtest.functional.context.beans.DelegateAfter;
	using DelegateBefore = org.camunda.bpm.integrationtest.functional.context.beans.DelegateBefore;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using OperateOnDeployment = org.jboss.arquillian.container.test.api.OperateOnDeployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;


	/// <summary>
	/// <para>This test ensures that if a call activity calls a process
	/// from a different process archive than the calling process,
	/// we perform the appropriate context switch</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class CallActivityContextSwitchTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class CallActivityContextSwitchTest : AbstractFoxPlatformIntegrationTest
	{
		[Deployment(name:"mainDeployment")]
		public static WebArchive createProcessArchiveDeplyoment()
		{
		return initWebArchiveDeployment("mainDeployment.war").addClass(typeof(DelegateBefore)).addClass(typeof(DelegateAfter)).addAsResource("org/camunda/bpm/integrationtest/functional/context/CallActivityContextSwitchTest.mainProcessSync.bpmn20.xml").addAsResource("org/camunda/bpm/integrationtest/functional/context/CallActivityContextSwitchTest.mainProcessSyncNoWait.bpmn20.xml").addAsResource("org/camunda/bpm/integrationtest/functional/context/CallActivityContextSwitchTest.mainProcessASync.bpmn20.xml").addAsResource("org/camunda/bpm/integrationtest/functional/context/CallActivityContextSwitchTest.mainProcessASyncBefore.bpmn20.xml").addAsResource("org/camunda/bpm/integrationtest/functional/context/CallActivityContextSwitchTest.mainProcessASyncAfter.bpmn20.xml");
		}

	  [Deployment(name:"calledDeployment")]
	  public static WebArchive createSecondProcessArchiveDeployment()
	  {
		return initWebArchiveDeployment("calledDeployment.war").addClass(typeof(CalledProcessDelegate)).addAsResource("org/camunda/bpm/integrationtest/functional/context/CallActivityContextSwitchTest.calledProcessSync.bpmn20.xml").addAsResource("org/camunda/bpm/integrationtest/functional/context/CallActivityContextSwitchTest.calledProcessSyncNoWait.bpmn20.xml").addAsResource("org/camunda/bpm/integrationtest/functional/context/CallActivityContextSwitchTest.calledProcessASync.bpmn20.xml");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject private javax.enterprise.inject.spi.BeanManager beanManager;
	  private BeanManager beanManager;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("mainDeployment") public void testNoWaitState()
	  public virtual void testNoWaitState()
	  {

		// this test makes sure the delegate invoked by the called process can be resolved (context switch necessary).

		// we cannot load the class
		try
		{
		  new CalledProcessDelegate();
		  Assert.fail("exception expected");
		}
		catch (NoClassDefFoundError)
		{
		  // expected
		}

		// our bean manager does not know this bean
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.Set<javax.enterprise.inject.spi.Bean< ? >> beans = beanManager.getBeans("calledProcessDelegate");
		ISet<Bean<object>> beans = beanManager.getBeans("calledProcessDelegate");
		Assert.assertEquals(0, beans.Count);

		// but when we execute the process, we perform the context switch to the corresponding deployment
		// and there the class can be resolved and the bean is known.
		IDictionary<string, object> processVariables = new Dictionary<string, object>();
		processVariables["calledElement"] = "calledProcessSyncNoWait";
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("mainProcessSyncNoWait", processVariables);

		Assert.assertNull(runtimeService.createProcessInstanceQuery().processDefinitionId(pi.Id).singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("mainDeployment") public void testMainSyncCalledSync()
	  public virtual void testMainSyncCalledSync()
	  {

		IDictionary<string, object> processVariables = new Dictionary<string, object>();
		processVariables["calledElement"] = "calledProcessSync";

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("mainProcessSync", processVariables);

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Assert.assertEquals(true, runtimeService.getVariable(pi.Id, typeof(DelegateBefore).FullName));

		ProcessInstance calledPi = runtimeService.createProcessInstanceQuery().processDefinitionKey("calledProcessSync").singleResult();
		Assert.assertEquals(true, runtimeService.getVariable(calledPi.Id, "calledDelegate"));

		taskService.complete(taskService.createTaskQuery().processInstanceId(calledPi.Id).singleResult().Id);

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Assert.assertEquals(true, runtimeService.getVariable(pi.Id, typeof(DelegateAfter).FullName));

		taskService.complete(taskService.createTaskQuery().processInstanceId(pi.Id).singleResult().Id);

		Assert.assertNull(runtimeService.createProcessInstanceQuery().processDefinitionId(pi.Id).singleResult());
		Assert.assertNull(runtimeService.createProcessInstanceQuery().processDefinitionId(calledPi.Id).singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("mainDeployment") public void testMainASyncCalledSync()
	  public virtual void testMainASyncCalledSync()
	  {

		IDictionary<string, object> processVariables = new Dictionary<string, object>();
		processVariables["calledElement"] = "calledProcessSync";

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("mainProcessASync", processVariables);

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Assert.assertEquals(true, runtimeService.getVariable(pi.Id, typeof(DelegateBefore).FullName));

		waitForJobExecutorToProcessAllJobs();

		ProcessInstance calledPi = runtimeService.createProcessInstanceQuery().processDefinitionKey("calledProcessSync").singleResult();
		Assert.assertEquals(true, runtimeService.getVariable(calledPi.Id, "calledDelegate"));

		taskService.complete(taskService.createTaskQuery().processInstanceId(calledPi.Id).singleResult().Id);

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Assert.assertEquals(true, runtimeService.getVariable(pi.Id, typeof(DelegateAfter).FullName));

		taskService.complete(taskService.createTaskQuery().processInstanceId(pi.Id).singleResult().Id);

		Assert.assertNull(runtimeService.createProcessInstanceQuery().processDefinitionId(pi.Id).singleResult());
		Assert.assertNull(runtimeService.createProcessInstanceQuery().processDefinitionId(calledPi.Id).singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("mainDeployment") public void testMainASyncBeforeCalledSync()
	  public virtual void testMainASyncBeforeCalledSync()
	  {

		IDictionary<string, object> processVariables = new Dictionary<string, object>();
		processVariables["calledElement"] = "calledProcessSync";

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("mainProcessASyncBefore", processVariables);

		waitForJobExecutorToProcessAllJobs();

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Assert.assertEquals(true, runtimeService.getVariable(pi.Id, typeof(DelegateBefore).FullName));

		ProcessInstance calledPi = runtimeService.createProcessInstanceQuery().processDefinitionKey("calledProcessSync").singleResult();
		Assert.assertEquals(true, runtimeService.getVariable(calledPi.Id, "calledDelegate"));

		taskService.complete(taskService.createTaskQuery().processInstanceId(calledPi.Id).singleResult().Id);

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Assert.assertEquals(true, runtimeService.getVariable(pi.Id, typeof(DelegateAfter).FullName));

		taskService.complete(taskService.createTaskQuery().processInstanceId(pi.Id).singleResult().Id);

		Assert.assertNull(runtimeService.createProcessInstanceQuery().processDefinitionId(pi.Id).singleResult());
		Assert.assertNull(runtimeService.createProcessInstanceQuery().processDefinitionId(calledPi.Id).singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("mainDeployment") public void testMainASyncAfterCalledSync()
	  public virtual void testMainASyncAfterCalledSync()
	  {

		IDictionary<string, object> processVariables = new Dictionary<string, object>();
		processVariables["calledElement"] = "calledProcessSync";

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("mainProcessASyncAfter", processVariables);

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Assert.assertEquals(true, runtimeService.getVariable(pi.Id, typeof(DelegateBefore).FullName));

		ProcessInstance calledPi = runtimeService.createProcessInstanceQuery().processDefinitionKey("calledProcessSync").singleResult();
		Assert.assertEquals(true, runtimeService.getVariable(calledPi.Id, "calledDelegate"));

		taskService.complete(taskService.createTaskQuery().processInstanceId(calledPi.Id).singleResult().Id);

		waitForJobExecutorToProcessAllJobs();

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Assert.assertEquals(true, runtimeService.getVariable(pi.Id, typeof(DelegateAfter).FullName));

		taskService.complete(taskService.createTaskQuery().processInstanceId(pi.Id).singleResult().Id);

		Assert.assertNull(runtimeService.createProcessInstanceQuery().processDefinitionId(pi.Id).singleResult());
		Assert.assertNull(runtimeService.createProcessInstanceQuery().processDefinitionId(calledPi.Id).singleResult());
	  }

	  // the same in main process but called process async

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("mainDeployment") public void testMainSyncCalledASync()
	  public virtual void testMainSyncCalledASync()
	  {

		IDictionary<string, object> processVariables = new Dictionary<string, object>();
		processVariables["calledElement"] = "calledProcessASync";

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("mainProcessSync", processVariables);

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Assert.assertEquals(true, runtimeService.getVariable(pi.Id, typeof(DelegateBefore).FullName));

		ProcessInstance calledPi = runtimeService.createProcessInstanceQuery().processDefinitionKey("calledProcessASync").singleResult();

		Assert.assertNotNull(calledPi);
		Assert.assertNull(runtimeService.getVariable(calledPi.Id, "calledDelegate"));

		waitForJobExecutorToProcessAllJobs();

		Assert.assertEquals(true, runtimeService.getVariable(calledPi.Id, "calledDelegate"));

		taskService.complete(taskService.createTaskQuery().processInstanceId(calledPi.Id).singleResult().Id);

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Assert.assertEquals(true, runtimeService.getVariable(pi.Id, typeof(DelegateAfter).FullName));

		taskService.complete(taskService.createTaskQuery().processInstanceId(pi.Id).singleResult().Id);

		Assert.assertNull(runtimeService.createProcessInstanceQuery().processDefinitionId(pi.Id).singleResult());
		Assert.assertNull(runtimeService.createProcessInstanceQuery().processDefinitionId(calledPi.Id).singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("mainDeployment") public void testMainASyncCalledASync()
	  public virtual void testMainASyncCalledASync()
	  {

		IDictionary<string, object> processVariables = new Dictionary<string, object>();
		processVariables["calledElement"] = "calledProcessASync";

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("mainProcessASync", processVariables);

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Assert.assertEquals(true, runtimeService.getVariable(pi.Id, typeof(DelegateBefore).FullName));

		waitForJobExecutorToProcessAllJobs();

		ProcessInstance calledPi = runtimeService.createProcessInstanceQuery().processDefinitionKey("calledProcessASync").singleResult();
		Assert.assertEquals(true, runtimeService.getVariable(calledPi.Id, "calledDelegate"));

		taskService.complete(taskService.createTaskQuery().processInstanceId(calledPi.Id).singleResult().Id);

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Assert.assertEquals(true, runtimeService.getVariable(pi.Id, typeof(DelegateAfter).FullName));

		taskService.complete(taskService.createTaskQuery().processInstanceId(pi.Id).singleResult().Id);

		Assert.assertNull(runtimeService.createProcessInstanceQuery().processDefinitionId(pi.Id).singleResult());
		Assert.assertNull(runtimeService.createProcessInstanceQuery().processDefinitionId(calledPi.Id).singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("mainDeployment") public void testMainASyncBeforeCalledASync()
	  public virtual void testMainASyncBeforeCalledASync()
	  {

		IDictionary<string, object> processVariables = new Dictionary<string, object>();
		processVariables["calledElement"] = "calledProcessASync";

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("mainProcessASyncBefore", processVariables);

		waitForJobExecutorToProcessAllJobs();

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Assert.assertEquals(true, runtimeService.getVariable(pi.Id, typeof(DelegateBefore).FullName));

		ProcessInstance calledPi = runtimeService.createProcessInstanceQuery().processDefinitionKey("calledProcessASync").singleResult();
		Assert.assertEquals(true, runtimeService.getVariable(calledPi.Id, "calledDelegate"));

		taskService.complete(taskService.createTaskQuery().processInstanceId(calledPi.Id).singleResult().Id);

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Assert.assertEquals(true, runtimeService.getVariable(pi.Id, typeof(DelegateAfter).FullName));

		taskService.complete(taskService.createTaskQuery().processInstanceId(pi.Id).singleResult().Id);

		Assert.assertNull(runtimeService.createProcessInstanceQuery().processDefinitionId(pi.Id).singleResult());
		Assert.assertNull(runtimeService.createProcessInstanceQuery().processDefinitionId(calledPi.Id).singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("mainDeployment") public void testMainASyncAfterCalledASync()
	  public virtual void testMainASyncAfterCalledASync()
	  {

		IDictionary<string, object> processVariables = new Dictionary<string, object>();
		processVariables["calledElement"] = "calledProcessASync";

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("mainProcessASyncAfter", processVariables);

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Assert.assertEquals(true, runtimeService.getVariable(pi.Id, typeof(DelegateBefore).FullName));

		waitForJobExecutorToProcessAllJobs();

		ProcessInstance calledPi = runtimeService.createProcessInstanceQuery().processDefinitionKey("calledProcessASync").singleResult();
		Assert.assertEquals(true, runtimeService.getVariable(calledPi.Id, "calledDelegate"));

		taskService.complete(taskService.createTaskQuery().processInstanceId(calledPi.Id).singleResult().Id);

		waitForJobExecutorToProcessAllJobs();

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Assert.assertEquals(true, runtimeService.getVariable(pi.Id, typeof(DelegateAfter).FullName));

		taskService.complete(taskService.createTaskQuery().processInstanceId(pi.Id).singleResult().Id);

		Assert.assertNull(runtimeService.createProcessInstanceQuery().processDefinitionId(pi.Id).singleResult());
		Assert.assertNull(runtimeService.createProcessInstanceQuery().processDefinitionId(calledPi.Id).singleResult());
	  }



	}

}