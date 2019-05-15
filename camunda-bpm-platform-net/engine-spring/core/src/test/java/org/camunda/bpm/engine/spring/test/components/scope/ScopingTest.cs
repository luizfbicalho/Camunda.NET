using System.Collections.Generic;
using System.Threading;

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
namespace org.camunda.bpm.engine.spring.test.components.scope
{
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;
	using Autowired = org.springframework.beans.factory.annotation.Autowired;
	using ContextConfiguration = org.springframework.test.context.ContextConfiguration;
	using SpringJUnit4ClassRunner = org.springframework.test.context.junit4.SpringJUnit4ClassRunner;
	using StringUtils = org.springframework.util.StringUtils;


	/// <summary>
	/// tests the scoped beans
	/// 
	/// @author Josh Long
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(SpringJUnit4ClassRunner.class) @ContextConfiguration("classpath:org/camunda/bpm/engine/spring/test/components/ScopingTests-context.xml") public class ScopingTest
	public class ScopingTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Autowired private org.camunda.bpm.engine.spring.test.components.ProcessInitiatingPojo processInitiatingPojo;
		private ProcessInitiatingPojo processInitiatingPojo;

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		private Logger logger = Logger.getLogger(this.GetType().FullName);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Autowired private org.camunda.bpm.engine.ProcessEngine processEngine;
		private ProcessEngine processEngine;

		private RepositoryService repositoryService;
		private TaskService taskService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void before() throws Throwable
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public virtual void before()
		{
		  this.repositoryService = this.processEngine.RepositoryService;
			this.taskService = this.processEngine.TaskService;

			repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/spring/test/autodeployment/autodeploy.b.bpmn20.xml").addClasspathResource("org/camunda/bpm/engine/spring/test/components/waiter.bpmn20.xml").addClasspathResource("org/camunda/bpm/engine/spring/test/components/spring-component-waiter.bpmn20.xml").deploy();
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void after()
		public virtual void after()
		{
		  foreach (Deployment deployment in repositoryService.createDeploymentQuery().list())
		  {
			repositoryService.deleteDeployment(deployment.Id, true);
		  }
		  processEngine.close();
		  processEngine = null;
		  repositoryService = null;
		  taskService = null;
		  processInitiatingPojo = null;
		}

		public static long CUSTOMER_ID_PROC_VAR_VALUE = 343;

		public static string customerIdProcVarName = "customerId";

		/// <summary>
		/// this code instantiates a business process that in turn delegates to a few Spring beans that in turn inject a process scoped object, <seealso cref="StatefulObject"/>.
		/// </summary>
		/// <returns> the StatefulObject that was injected across different components, that all share the same state. </returns>
		/// <exception cref="Throwable"> if anythign goes wrong </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private StatefulObject run() throws Throwable
		private StatefulObject run()
		{
			logger.info("----------------------------------------------");
			IDictionary<string, object> vars = new Dictionary<string, object>();
			vars[customerIdProcVarName] = CUSTOMER_ID_PROC_VAR_VALUE;
			ProcessInstance processInstance = processEngine.RuntimeService.startProcessInstanceByKey("component-waiter", vars);
			StatefulObject scopedObject = (StatefulObject) processEngine.RuntimeService.getVariable(processInstance.Id, "scopedTarget.c1");
			Assert.assertNotNull("the scopedObject can't be null", scopedObject);
			Assert.assertTrue("the 'name' property can't be null.", StringUtils.hasText(scopedObject.Name));
			Assert.assertEquals(scopedObject.VisitedCount, 2);

			// the process has paused
			string procId = processInstance.ProcessInstanceId;

			IList<Task> tasks = taskService.createTaskQuery().executionId(procId).list();

			Assert.assertEquals("there should be 1 (one) task enqueued at this point.", tasks.Count, 1);

			Task t = tasks.GetEnumerator().next();

			this.taskService.claim(t.Id, "me");

			logger.info("sleeping for 10 seconds while a user performs his task. " + "The first transaction has committed. A new one will start in 10 seconds");

			Thread.Sleep(1000 * 5);

			this.taskService.complete(t.Id);

			scopedObject = (StatefulObject) processEngine.RuntimeService.getVariable(processInstance.Id, "scopedTarget.c1");
			Assert.assertEquals(scopedObject.VisitedCount, 3);

			Assert.assertEquals("the customerId injected should " + "be what was given as a processVariable parameter.", ScopingTest.CUSTOMER_ID_PROC_VAR_VALUE, scopedObject.CustomerId);
			return scopedObject;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUsingAnInjectedScopedProxy() throws Throwable
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public virtual void testUsingAnInjectedScopedProxy()
		{
			logger.info("Running 'component-waiter' process instance with scoped beans.");
			StatefulObject one = run();
			StatefulObject two = run();
			Assert.assertNotSame(one.Name, two.Name);
			Assert.assertEquals(one.VisitedCount, two.VisitedCount);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStartingAProcessWithScopedBeans() throws Throwable
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public virtual void testStartingAProcessWithScopedBeans()
		{
			this.processInitiatingPojo.startScopedProcess(3243);
		}


	}

}