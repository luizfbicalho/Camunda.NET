using System;

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
	using InvocationContext = org.camunda.bpm.application.InvocationContext;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using NoOpJavaDelegate = org.camunda.bpm.integrationtest.functional.context.beans.NoOpJavaDelegate;
	using SignalableTask = org.camunda.bpm.integrationtest.functional.context.beans.SignalableTask;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using OperateOnDeployment = org.jboss.arquillian.container.test.api.OperateOnDeployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using After = org.junit.After;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	/// <summary>
	/// Checks if the process application is invoked with an invocation context.
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class InvocationContextTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class InvocationContextTest : AbstractFoxPlatformIntegrationTest
	{
		[Deployment(name : "app")]
		public static WebArchive createDeployment()
		{
		return ShrinkWrap.create(typeof(WebArchive), "app.war").addAsResource("META-INF/processes.xml").addClass(typeof(AbstractFoxPlatformIntegrationTest)).addClass(typeof(ProcessApplicationWithInvocationContext)).addClass(typeof(NoOpJavaDelegate)).addClass(typeof(SignalableTask)).addAsResource("org/camunda/bpm/integrationtest/functional/context/InvocationContextTest-timer.bpmn").addAsResource("org/camunda/bpm/integrationtest/functional/context/InvocationContextTest-message.bpmn").addAsResource("org/camunda/bpm/integrationtest/functional/context/InvocationContextTest-signalTask.bpmn");
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanUp()
	  public virtual void cleanUp()
	  {
		ClockUtil.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("app") public void testInvokeProcessApplicationWithContextOnStart()
	  public virtual void testInvokeProcessApplicationWithContextOnStart()
	  {

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("messageProcess");

		InvocationContext invocationContext = ProcessApplicationWithInvocationContext.InvocationContext;
		assertThat(invocationContext, @is(notNullValue()));
		assertThat(invocationContext.Execution, @is(notNullValue()));
		assertThat(invocationContext.Execution.Id, @is(pi.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("app") public void testInvokeProcessApplicationWithContextOnAsyncExecution()
	  public virtual void testInvokeProcessApplicationWithContextOnAsyncExecution()
	  {

		runtimeService.startProcessInstanceByKey("timerProcess");
		ProcessApplicationWithInvocationContext.clearInvocationContext();

		Job timer = managementService.createJobQuery().timers().singleResult();
		assertThat(timer, @is(notNullValue()));

		long dueDate = timer.Duedate.Ticks;
		DateTime afterDueDate = new DateTime(dueDate + 1000 * 60);

		ClockUtil.CurrentTime = afterDueDate;
		waitForJobExecutorToProcessAllJobs();

		InvocationContext invocationContext = ProcessApplicationWithInvocationContext.InvocationContext;
		assertThat(invocationContext, @is(notNullValue()));
		assertThat(invocationContext.Execution, @is(notNullValue()));
		assertThat(invocationContext.Execution.Id, @is(timer.ExecutionId));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("app") public void testInvokeProcessApplicationWithContextOnMessageReceived()
	  public virtual void testInvokeProcessApplicationWithContextOnMessageReceived()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("messageProcess");
		ProcessApplicationWithInvocationContext.clearInvocationContext();

		EventSubscription messageSubscription = runtimeService.createEventSubscriptionQuery().eventType("message").processInstanceId(processInstance.Id).singleResult();
		assertThat(messageSubscription, @is(notNullValue()));

		runtimeService.messageEventReceived(messageSubscription.EventName, messageSubscription.ExecutionId);

		InvocationContext invocationContext = ProcessApplicationWithInvocationContext.InvocationContext;
		assertThat(invocationContext, @is(notNullValue()));
		assertThat(invocationContext.Execution, @is(notNullValue()));
		assertThat(invocationContext.Execution.Id, @is(messageSubscription.ExecutionId));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("app") public void testInvokeProcessApplicationWithContextOnSignalTask()
	  public virtual void testInvokeProcessApplicationWithContextOnSignalTask()
	  {

		runtimeService.startProcessInstanceByKey("signalableProcess");
		ProcessApplicationWithInvocationContext.clearInvocationContext();

		Execution execution = runtimeService.createExecutionQuery().activityId("waitingTask").singleResult();
		assertThat(execution, @is(notNullValue()));

		runtimeService.signal(execution.Id);

		InvocationContext invocationContext = ProcessApplicationWithInvocationContext.InvocationContext;
		assertThat(invocationContext, @is(notNullValue()));
		assertThat(invocationContext.Execution, @is(notNullValue()));
		assertThat(invocationContext.Execution.Id, @is(execution.Id));
	  }

	}

}