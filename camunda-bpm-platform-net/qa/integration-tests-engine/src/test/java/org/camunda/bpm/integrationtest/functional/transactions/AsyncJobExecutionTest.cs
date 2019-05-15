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
namespace org.camunda.bpm.integrationtest.functional.transactions
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;


	using RuntimeService = org.camunda.bpm.engine.RuntimeService;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using FailingTransactionListenerDelegate = org.camunda.bpm.integrationtest.functional.transactions.beans.FailingTransactionListenerDelegate;
	using GetVersionInfoDelegate = org.camunda.bpm.integrationtest.functional.transactions.beans.GetVersionInfoDelegate;
	using TransactionRollbackDelegate = org.camunda.bpm.integrationtest.functional.transactions.beans.TransactionRollbackDelegate;
	using UpdateRouterConfiguration = org.camunda.bpm.integrationtest.functional.transactions.beans.UpdateRouterConfiguration;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using After = org.junit.After;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class AsyncJobExecutionTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class AsyncJobExecutionTest : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive processArchive()
		public static WebArchive processArchive()
		{
		return initWebArchiveDeployment().addClass(typeof(GetVersionInfoDelegate)).addClass(typeof(UpdateRouterConfiguration)).addClass(typeof(TransactionRollbackDelegate)).addClass(typeof(FailingTransactionListenerDelegate)).addAsResource("org/camunda/bpm/integrationtest/functional/transactions/AsyncJobExecutionTest.testAsyncServiceTasks.bpmn20.xml").addAsResource("org/camunda/bpm/integrationtest/functional/transactions/AsyncJobExecutionTest.transactionRollbackInServiceTask.bpmn20.xml").addAsResource("org/camunda/bpm/integrationtest/functional/transactions/AsyncJobExecutionTest.transactionRollbackInServiceTaskWithCustomRetryCycle.bpmn20.xml").addAsResource("org/camunda/bpm/integrationtest/functional/transactions/AsyncJobExecutionTest.failingTransactionListener.bpmn20.xml").addAsWebInfResource("persistence.xml", "classes/META-INF/persistence.xml");
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject private org.camunda.bpm.engine.RuntimeService runtimeService;
	  private new RuntimeService runtimeService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanUp()
	  public virtual void cleanUp()
	  {
		foreach (ProcessInstance processInstance in runtimeService.createProcessInstanceQuery().list())
		{
		  runtimeService.deleteProcessInstance(processInstance.Id, "test ended", true);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAsyncServiceTasks()
	  public virtual void testAsyncServiceTasks()
	  {
		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables["serialnumber"] = "23";
		runtimeService.startProcessInstanceByKey("configure-router", variables);

		waitForJobExecutorToProcessAllJobs();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTransactionRollbackInServiceTask() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testTransactionRollbackInServiceTask()
	  {

		runtimeService.startProcessInstanceByKey("txRollbackServiceTask");

		waitForJobExecutorToProcessAllJobs(10000);

		Job job = managementService.createJobQuery().singleResult();

		assertNotNull(job);
		assertEquals(0, job.Retries);
		assertNotNull(job.ExceptionMessage);
		assertNotNull(managementService.getJobExceptionStacktrace(job.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTransactionRollbackInServiceTaskWithCustomRetryCycle() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testTransactionRollbackInServiceTaskWithCustomRetryCycle()
	  {

		runtimeService.startProcessInstanceByKey("txRollbackServiceTaskWithCustomRetryCycle");

		waitForJobExecutorToProcessAllJobs(10000);

		Job job = managementService.createJobQuery().singleResult();

		assertNotNull(job);
		assertEquals(0, job.Retries);
		assertNotNull(job.ExceptionMessage);
		assertNotNull(managementService.getJobExceptionStacktrace(job.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailingTransactionListener() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testFailingTransactionListener()
	  {

		runtimeService.startProcessInstanceByKey("failingTransactionListener");

		waitForJobExecutorToProcessAllJobs(10000);

		Job job = managementService.createJobQuery().singleResult();

		assertNotNull(job);
		assertEquals(0, job.Retries);
		assertNotNull(job.ExceptionMessage);
		assertNotNull(managementService.getJobExceptionStacktrace(job.Id));
	  }

	}

}