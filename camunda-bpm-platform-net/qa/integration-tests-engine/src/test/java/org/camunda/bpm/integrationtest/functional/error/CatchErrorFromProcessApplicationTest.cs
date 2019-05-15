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
namespace org.camunda.bpm.integrationtest.functional.error
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;


	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Task = org.camunda.bpm.engine.task.Task;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using DeploymentHelper = org.camunda.bpm.integrationtest.util.DeploymentHelper;
	using TestContainer = org.camunda.bpm.integrationtest.util.TestContainer;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using OperateOnDeployment = org.jboss.arquillian.container.test.api.OperateOnDeployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using EmptyAsset = org.jboss.shrinkwrap.api.asset.EmptyAsset;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class CatchErrorFromProcessApplicationTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class CatchErrorFromProcessApplicationTest : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive createProcessArchiveDeployment()
		public static WebArchive createProcessArchiveDeployment()
		{
		return initWebArchiveDeployment().addClass(typeof(ThrowErrorDelegate)).addClass(typeof(MyBusinessException)).addAsResource("org/camunda/bpm/integrationtest/functional/error/CatchErrorFromProcessApplicationTest.bpmn20.xml").addAsResource("org/camunda/bpm/integrationtest/functional/error/CatchErrorFromProcessApplicationTest.delegateExpression.bpmn20.xml").addAsResource("org/camunda/bpm/integrationtest/functional/error/CatchErrorFromProcessApplicationTest.sequentialMultiInstance.bpmn20.xml").addAsResource("org/camunda/bpm/integrationtest/functional/error/CatchErrorFromProcessApplicationTest.delegateExpression.sequentialMultiInstance.bpmn20.xml").addAsResource("org/camunda/bpm/integrationtest/functional/error/CatchErrorFromProcessApplicationTest.parallelMultiInstance.bpmn20.xml").addAsResource("org/camunda/bpm/integrationtest/functional/error/CatchErrorFromProcessApplicationTest.delegateExpression.parallelMultiInstance.bpmn20.xml");
		}

	  [Deployment(name:"clientDeployment")]
	  public static WebArchive clientDeployment()
	  {
		WebArchive deployment = ShrinkWrap.create(typeof(WebArchive), "client.war").addAsWebInfResource(EmptyAsset.INSTANCE, "beans.xml").addClass(typeof(AbstractFoxPlatformIntegrationTest)).addAsLibraries(DeploymentHelper.EngineCdi);

		TestContainer.addContainerSpecificResourcesForNonPa(deployment);

		return deployment;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testThrowExceptionInExecute()
	  public virtual void testThrowExceptionInExecute()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcess", throwException()).Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskException", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testThrowErrorInExecute()
	  public virtual void testThrowErrorInExecute()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcess", throwError()).Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskError", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testThrowExceptionInSignal()
	  public virtual void testThrowExceptionInSignal()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcess").Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Execution serviceTask = runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").singleResult();
		assertNotNull(serviceTask);

		runtimeService.setVariables(pi, throwException());
		runtimeService.signal(serviceTask.Id);

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertTrue((bool?) runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskException", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testThrowErrorInSignal()
	  public virtual void testThrowErrorInSignal()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcess").Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Execution serviceTask = runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").singleResult();
		assertNotNull(serviceTask);

		runtimeService.setVariables(pi, throwError());
		runtimeService.signal(serviceTask.Id);

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertTrue((bool?) runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskError", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testThrowExceptionInExecuteSequentialMultiInstance()
	  public virtual void testThrowExceptionInExecuteSequentialMultiInstance()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcessSequentialMI", throwException()).Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskException", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testThrowErrorInExecuteSequentialMultiInstance()
	  public virtual void testThrowErrorInExecuteSequentialMultiInstance()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcessSequentialMI", throwError()).Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskError", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testThrowExceptionInSignalSequentialMultiInstance()
	  public virtual void testThrowExceptionInSignalSequentialMultiInstance()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcessSequentialMI").Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		// signal 2 times to execute first sequential behaviors
		runtimeService.setVariables(pi, leaveExecution());
		runtimeService.signal(runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").singleResult().Id);
		runtimeService.setVariables(pi, leaveExecution());

		Execution serviceTask = runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").singleResult();
		assertNotNull(serviceTask);

		runtimeService.setVariables(pi, throwException());
		runtimeService.signal(serviceTask.Id);

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertTrue((bool?) runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskException", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testThrowErrorInSignalSequentialMultiInstance()
	  public virtual void testThrowErrorInSignalSequentialMultiInstance()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcessSequentialMI").Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		// signal 2 times to execute first sequential behaviors
		runtimeService.setVariables(pi, leaveExecution());
		runtimeService.signal(runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").singleResult().Id);
		runtimeService.setVariables(pi, leaveExecution());

		runtimeService.signal(runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").singleResult().Id);
		Execution serviceTask = runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").singleResult();
		assertNotNull(serviceTask);

		runtimeService.setVariables(pi, throwError());
		runtimeService.signal(serviceTask.Id);

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertTrue((bool?) runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskError", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testThrowExceptionInExecuteParallelMultiInstance()
	  public virtual void testThrowExceptionInExecuteParallelMultiInstance()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcessParallelMI", throwException()).Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskException", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testThrowErrorInExecuteParallelMultiInstance()
	  public virtual void testThrowErrorInExecuteParallelMultiInstance()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcessParallelMI", throwError()).Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskError", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testThrowExceptionInSignalParallelMultiInstance()
	  public virtual void testThrowExceptionInSignalParallelMultiInstance()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcessParallelMI").Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Execution serviceTask = runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").list().get(3);
		assertNotNull(serviceTask);

		runtimeService.setVariables(pi, throwException());
		runtimeService.signal(serviceTask.Id);

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertTrue((bool?) runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskException", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testThrowErrorInSignalParallelMultiInstance()
	  public virtual void testThrowErrorInSignalParallelMultiInstance()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcessParallelMI").Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Execution serviceTask = runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").list().get(3);
		assertNotNull(serviceTask);

		runtimeService.setVariables(pi, throwError());
		runtimeService.signal(serviceTask.Id);

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertTrue((bool?) runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskError", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testThrowExceptionInDelegateExpressionExecute()
	  public virtual void testThrowExceptionInDelegateExpressionExecute()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcess", throwException()).Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskException", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testThrowErrorInDelegateExpressionExecute()
	  public virtual void testThrowErrorInDelegateExpressionExecute()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcess", throwError()).Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskError", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testThrowExceptionInDelegateExpressionSignal()
	  public virtual void testThrowExceptionInDelegateExpressionSignal()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcess").Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Execution serviceTask = runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").singleResult();
		assertNotNull(serviceTask);

		runtimeService.setVariables(pi, throwException());
		runtimeService.signal(serviceTask.Id);

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertTrue((bool?) runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskException", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testThrowErrorInDelegateExpressionSignal()
	  public virtual void testThrowErrorInDelegateExpressionSignal()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcess").Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Execution serviceTask = runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").singleResult();
		assertNotNull(serviceTask);

		runtimeService.setVariables(pi, throwError());
		runtimeService.signal(serviceTask.Id);

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertTrue((bool?) runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskError", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testThrowExceptionInDelegateExpressionExecuteSequentialMultiInstance()
	  public virtual void testThrowExceptionInDelegateExpressionExecuteSequentialMultiInstance()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcessSequentialMI", throwException()).Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskException", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testThrowErrorInDelegateExpressionExecuteSequentialMultiInstance()
	  public virtual void testThrowErrorInDelegateExpressionExecuteSequentialMultiInstance()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcessSequentialMI", throwError()).Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskError", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testThrowExceptionInDelegateExpressionSignalSequentialMultiInstance()
	  public virtual void testThrowExceptionInDelegateExpressionSignalSequentialMultiInstance()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcessSequentialMI").Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		// signal 2 times to execute first sequential behaviors
		runtimeService.setVariables(pi, leaveExecution());
		runtimeService.signal(runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").singleResult().Id);
		runtimeService.setVariables(pi, leaveExecution());

		Execution serviceTask = runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").singleResult();
		assertNotNull(serviceTask);

		runtimeService.setVariables(pi, throwException());
		runtimeService.signal(serviceTask.Id);

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertTrue((bool?) runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskException", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testThrowErrorInDelegateExpressionSignalSequentialMultiInstance()
	  public virtual void testThrowErrorInDelegateExpressionSignalSequentialMultiInstance()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcessSequentialMI").Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		// signal 2 times to execute first sequential behaviors
		runtimeService.setVariables(pi, leaveExecution());
		runtimeService.signal(runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").singleResult().Id);
		runtimeService.setVariables(pi, leaveExecution());

		runtimeService.signal(runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").singleResult().Id);
		Execution serviceTask = runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").singleResult();
		assertNotNull(serviceTask);

		runtimeService.setVariables(pi, throwError());
		runtimeService.signal(serviceTask.Id);

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertTrue((bool?) runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskError", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testThrowExceptionInDelegateExpressionExecuteParallelMultiInstance()
	  public virtual void testThrowExceptionInDelegateExpressionExecuteParallelMultiInstance()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcessParallelMI", throwException()).Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskException", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testThrowErrorInDelegateExpressionExecuteParallelMultiInstance()
	  public virtual void testThrowErrorInDelegateExpressionExecuteParallelMultiInstance()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcessParallelMI", throwError()).Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskError", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testThrowExceptionInDelegateExpressionSignalParallelMultiInstance()
	  public virtual void testThrowExceptionInDelegateExpressionSignalParallelMultiInstance()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcessParallelMI").Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Execution serviceTask = runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").list().get(3);
		assertNotNull(serviceTask);

		runtimeService.setVariables(pi, throwException());
		runtimeService.signal(serviceTask.Id);

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertTrue((bool?) runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskException", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testThrowErrorInDelegateExpressionSignalParallelMultiInstance()
	  public virtual void testThrowErrorInDelegateExpressionSignalParallelMultiInstance()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcessParallelMI").Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Execution serviceTask = runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").list().get(3);
		assertNotNull(serviceTask);

		runtimeService.setVariables(pi, throwError());
		runtimeService.signal(serviceTask.Id);

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertTrue((bool?) runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskError", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

	  public virtual IDictionary<string, object> throwError()
	  {
		return Collections.singletonMap("type", (object) "error");
	  }

	  public virtual IDictionary<string, object> throwException()
	  {
		return Collections.singletonMap("type", (object) "exception");
	  }

	  public virtual IDictionary<string, object> leaveExecution()
	  {
		return Collections.singletonMap("type", (object) "leave");
	  }

	}

}