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
namespace org.camunda.bpm.engine.spring.test.transaction
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using Autowired = org.springframework.beans.factory.annotation.Autowired;
	using JdbcTemplate = org.springframework.jdbc.core.JdbcTemplate;
	using ContextConfiguration = org.springframework.test.context.ContextConfiguration;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ContextConfiguration("classpath:org/camunda/bpm/engine/spring/test/transaction/SpringTransactionIntegrationTest-context.xml") public class SpringTransactionIntegrationTest extends org.camunda.bpm.engine.spring.test.SpringProcessEngineTestCase
	public class SpringTransactionIntegrationTest : SpringProcessEngineTestCase
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Autowired protected UserBean userBean;
		protected internal UserBean userBean;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Autowired protected javax.sql.DataSource dataSource;
	  protected internal DataSource dataSource;

	  private static long WAIT_TIME_MILLIS = TimeUnit.MILLISECONDS.convert(20L, TimeUnit.SECONDS);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testBasicActivitiSpringIntegration()
	  public virtual void testBasicActivitiSpringIntegration()
	  {
		userBean.hello();

		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		assertEquals("Hello from Printer!", runtimeService.getVariable(processInstance.Id, "myVar"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testRollbackTransactionOnActivitiException()
	  public virtual void testRollbackTransactionOnActivitiException()
	  {

		// Create a table that the userBean is supposed to fill with some data
		JdbcTemplate jdbcTemplate = new JdbcTemplate(dataSource);
		jdbcTemplate.execute("create table MY_TABLE (MY_TEXT varchar);");

		// The hello() method will start the process. The process will wait in a user task
		userBean.hello();
		int results = jdbcTemplate.queryForObject("select count(*) from MY_TABLE", typeof(Integer));
		assertEquals(0, results);

		// The completeTask() method will write a record to the 'MY_TABLE' table and complete the user task
		try
		{
		  userBean.completeTask(taskService.createTaskQuery().singleResult().Id);
		  fail();
		}
		catch (Exception)
		{
		}

		// Since the service task after the user tasks throws an exception, both
		// the record and the process must be rolled back !
		assertEquals("My Task", taskService.createTaskQuery().singleResult().Name);
		results = jdbcTemplate.queryForObject("select count(*) from MY_TABLE", typeof(Integer));
		assertEquals(0, results);

		// Cleanup
		jdbcTemplate.execute("drop table MY_TABLE if exists;");
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/spring/test/transaction/SpringTransactionIntegrationTest.testErrorPropagationOnExceptionInTransaction.bpmn20.xml", "org/camunda/bpm/engine/spring/test/transaction/SpringTransactionIntegrationTest.throwExceptionProcess.bpmn20.xml" })]
	  public virtual void testErrorPropagationOnExceptionInTransaction()
	  {
		  runtimeService.startProcessInstanceByKey("process");
		  waitForJobExecutorToProcessAllJobs(WAIT_TIME_MILLIS);
		  Incident incident = runtimeService.createIncidentQuery().activityId("servicetask").singleResult();
		  assertThat(incident.IncidentMessage, @is("error"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTransactionRollbackInServiceTask() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testTransactionRollbackInServiceTask()
	  {

		runtimeService.startProcessInstanceByKey("txRollbackServiceTask");

		waitForJobExecutorToProcessAllJobs(WAIT_TIME_MILLIS);

		Job job = managementService.createJobQuery().singleResult();

		assertNotNull(job);
		assertEquals(0, job.Retries);
		assertEquals("Transaction rolled back because it has been marked as rollback-only", job.ExceptionMessage);

		string stacktrace = managementService.getJobExceptionStacktrace(job.Id);
		assertNotNull(stacktrace);
		assertTrue("unexpected stacktrace, was <" + stacktrace + ">", stacktrace.Contains("Transaction rolled back because it has been marked as rollback-only"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTransactionRollbackInServiceTaskWithCustomRetryCycle() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testTransactionRollbackInServiceTaskWithCustomRetryCycle()
	  {

		runtimeService.startProcessInstanceByKey("txRollbackServiceTaskWithCustomRetryCycle");

		waitForJobExecutorToProcessAllJobs(WAIT_TIME_MILLIS);

		Job job = managementService.createJobQuery().singleResult();

		assertNotNull(job);
		assertEquals(0, job.Retries);
		assertEquals("Transaction rolled back because it has been marked as rollback-only", job.ExceptionMessage);

		string stacktrace = managementService.getJobExceptionStacktrace(job.Id);
		assertNotNull(stacktrace);
		assertTrue("unexpected stacktrace, was <" + stacktrace + ">", stacktrace.Contains("Transaction rolled back because it has been marked as rollback-only"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testFailingTransactionListener() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testFailingTransactionListener()
	  {

		runtimeService.startProcessInstanceByKey("failingTransactionListener");

		waitForJobExecutorToProcessAllJobs(WAIT_TIME_MILLIS);

		Job job = managementService.createJobQuery().singleResult();

		assertNotNull(job);
		assertEquals(0, job.Retries);
		assertEquals("exception in transaction listener", job.ExceptionMessage);

		string stacktrace = managementService.getJobExceptionStacktrace(job.Id);
		assertNotNull(stacktrace);
		assertTrue("unexpected stacktrace, was <" + stacktrace + ">", stacktrace.Contains("java.lang.RuntimeException: exception in transaction listener"));
	  }


	}

}