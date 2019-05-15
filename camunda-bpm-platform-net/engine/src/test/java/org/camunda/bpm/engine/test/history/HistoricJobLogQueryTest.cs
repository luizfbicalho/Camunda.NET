using System;
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
namespace org.camunda.bpm.engine.test.history
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.historicJobLogByActivityId;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.historicJobLogByDeploymentId;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.historicJobLogByExecutionId;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.historicJobLogByJobDefinitionId;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.historicJobLogByJobDueDate;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.historicJobLogByJobId;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.historicJobLogByJobPriority;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.historicJobLogByJobRetries;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.historicJobLogByProcessDefinitionId;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.historicJobLogByProcessDefinitionKey;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.historicJobLogByProcessInstanceId;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.historicJobLogByTimestamp;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.historicJobLogPartiallyByOccurence;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.inverted;


	using HistoricJobLog = org.camunda.bpm.engine.history.HistoricJobLog;
	using HistoricJobLogQuery = org.camunda.bpm.engine.history.HistoricJobLogQuery;
	using AsyncContinuationJobHandler = org.camunda.bpm.engine.impl.jobexecutor.AsyncContinuationJobHandler;
	using MessageJobDeclaration = org.camunda.bpm.engine.impl.jobexecutor.MessageJobDeclaration;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using FailingDelegate = org.camunda.bpm.engine.test.api.runtime.FailingDelegate;
	using TestOrderingUtil = org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil;
	using NullTolerantComparator = org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.NullTolerantComparator;
	using Variables = org.camunda.bpm.engine.variable.Variables;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoricJobLogQueryTest : PluggableProcessEngineTestCase
	{
		[Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
		public virtual void testQuery()
		{
		runtimeService.startProcessInstanceByKey("process");
		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery();

		verifyQueryResults(query, 1);
		}

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testQueryByLogId()
	  {
		runtimeService.startProcessInstanceByKey("process");
		string logId = historyService.createHistoricJobLogQuery().singleResult().Id;

		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().logId(logId);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidLogId()
	  {
		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().logId("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.logId(null);
		  fail();
		}
		catch (Exception)
		{
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testQueryByJobId()
	  {
		runtimeService.startProcessInstanceByKey("process");
		string jobId = managementService.createJobQuery().singleResult().Id;

		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().jobId(jobId);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidJobId()
	  {
		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().jobId("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.jobId(null);
		  fail();
		}
		catch (Exception)
		{
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testQueryByJobExceptionMessage()
	  {
		runtimeService.startProcessInstanceByKey("process");
		string jobId = managementService.createJobQuery().singleResult().Id;
		try
		{
		  managementService.executeJob(jobId);
		  fail();
		}
		catch (Exception)
		{
		  // expected
		}

		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().jobExceptionMessage(FailingDelegate.EXCEPTION_MESSAGE);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidJobExceptionMessage()
	  {
		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().jobExceptionMessage("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.jobExceptionMessage(null);
		  fail();
		}
		catch (Exception)
		{
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testQueryByJobDefinitionId()
	  {
		runtimeService.startProcessInstanceByKey("process");
		string jobDefinitionId = managementService.createJobQuery().singleResult().JobDefinitionId;

		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().jobDefinitionId(jobDefinitionId);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidJobDefinitionId()
	  {
		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().jobDefinitionId("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.jobDefinitionId(null);
		  fail();
		}
		catch (Exception)
		{
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testQueryByJobDefinitionType()
	  {
		runtimeService.startProcessInstanceByKey("process");

		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().jobDefinitionType(AsyncContinuationJobHandler.TYPE);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidJobDefinitionType()
	  {
		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().jobDefinitionType("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.jobDefinitionType(null);
		  fail();
		}
		catch (Exception)
		{
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testQueryByJobDefinitionConfiguration()
	  {
		runtimeService.startProcessInstanceByKey("process");

		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().jobDefinitionConfiguration(MessageJobDeclaration.ASYNC_BEFORE);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidJobDefinitionConfiguration()
	  {
		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().jobDefinitionConfiguration("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.jobDefinitionConfiguration(null);
		  fail();
		}
		catch (Exception)
		{
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testQueryByActivityId()
	  {
		runtimeService.startProcessInstanceByKey("process");

		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().activityIdIn("serviceTask");

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidActivityId()
	  {
		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().activityIdIn("invalid");

		verifyQueryResults(query, 0);

		string[] nullValue = null;

		try
		{
		  query.activityIdIn(nullValue);
		  fail();
		}
		catch (Exception)
		{
		}

		string[] activityIdsContainsNull = new string[] {"a", null, "b"};

		try
		{
		  query.activityIdIn(activityIdsContainsNull);
		  fail();
		}
		catch (Exception)
		{
		}

		string[] activityIdsContainsEmptyString = new string[] {"a", "", "b"};

		try
		{
		  query.activityIdIn(activityIdsContainsEmptyString);
		  fail();
		}
		catch (Exception)
		{
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testQueryByExecutionId()
	  {
		runtimeService.startProcessInstanceByKey("process");
		string executionId = managementService.createJobQuery().singleResult().ExecutionId;

		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().executionIdIn(executionId);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidExecutionId()
	  {
		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().executionIdIn("invalid");

		verifyQueryResults(query, 0);

		string[] nullValue = null;

		try
		{
		  query.executionIdIn(nullValue);
		  fail();
		}
		catch (Exception)
		{
		}

		string[] executionIdsContainsNull = new string[] {"a", null, "b"};

		try
		{
		  query.executionIdIn(executionIdsContainsNull);
		  fail();
		}
		catch (Exception)
		{
		}

		string[] executionIdsContainsEmptyString = new string[] {"a", "", "b"};

		try
		{
		  query.executionIdIn(executionIdsContainsEmptyString);
		  fail();
		}
		catch (Exception)
		{
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testQueryByProcessInstanceId()
	  {
		runtimeService.startProcessInstanceByKey("process");
		string processInstanceId = managementService.createJobQuery().singleResult().ProcessInstanceId;

		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().processInstanceId(processInstanceId);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidProcessInstanceId()
	  {
		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().processInstanceId("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.processInstanceId(null);
		  fail();
		}
		catch (Exception)
		{
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testQueryByProcessDefinitionId()
	  {
		runtimeService.startProcessInstanceByKey("process");
		string processDefinitionId = managementService.createJobQuery().singleResult().ProcessDefinitionId;

		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().processDefinitionId(processDefinitionId);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidProcessDefinitionId()
	  {
		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().processDefinitionId("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.processDefinitionId(null);
		  fail();
		}
		catch (Exception)
		{
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testQueryByProcessDefinitionKey()
	  {
		runtimeService.startProcessInstanceByKey("process");
		string processDefinitionKey = managementService.createJobQuery().singleResult().ProcessDefinitionKey;

		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().processDefinitionKey(processDefinitionKey);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidProcessDefinitionKey()
	  {
		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().processDefinitionKey("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.processDefinitionKey(null);
		  fail();
		}
		catch (Exception)
		{
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testQueryByDeploymentId()
	  {
		runtimeService.startProcessInstanceByKey("process");
		string deploymentId = managementService.createJobQuery().singleResult().DeploymentId;

		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().deploymentId(deploymentId);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidDeploymentId()
	  {
		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().deploymentId("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.deploymentId(null);
		  fail();
		}
		catch (Exception)
		{
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testQueryByJobPriority()
	  public virtual void testQueryByJobPriority()
	  {
		// given 5 process instances with 5 jobs
		IList<ProcessInstance> processInstances = new List<ProcessInstance>();

		for (int i = 0; i < 5; i++)
		{
		  processInstances.Add(runtimeService.startProcessInstanceByKey("process", Variables.createVariables().putValue("priority", i)));
		}

		// then the creation logs can be filtered by priority of the jobs
		// (1) lower than or equal a priority
		IList<HistoricJobLog> jobLogs = historyService.createHistoricJobLogQuery().jobPriorityLowerThanOrEquals(2L).orderByJobPriority().asc().list();

		assertEquals(3, jobLogs.Count);
		foreach (HistoricJobLog log in jobLogs)
		{
		  assertTrue(log.JobPriority <= 2);
		}

		// (2) higher than or equal a given priorty
		jobLogs = historyService.createHistoricJobLogQuery().jobPriorityHigherThanOrEquals(3L).orderByJobPriority().asc().list();

		assertEquals(2, jobLogs.Count);
		foreach (HistoricJobLog log in jobLogs)
		{
		  assertTrue(log.JobPriority >= 3);
		}

		// (3) lower and higher than or equal
		jobLogs = historyService.createHistoricJobLogQuery().jobPriorityHigherThanOrEquals(1L).jobPriorityLowerThanOrEquals(3L).orderByJobPriority().asc().list();

		assertEquals(3, jobLogs.Count);
		foreach (HistoricJobLog log in jobLogs)
		{
		  assertTrue(log.JobPriority >= 1 && log.JobPriority <= 3);
		}

		// (4) lower and higher than or equal are disjunctive
		jobLogs = historyService.createHistoricJobLogQuery().jobPriorityHigherThanOrEquals(3).jobPriorityLowerThanOrEquals(1).orderByJobPriority().asc().list();
		assertEquals(0, jobLogs.Count);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testQueryByCreationLog()
	  {
		runtimeService.startProcessInstanceByKey("process");

		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().creationLog();

		verifyQueryResults(query, 1);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testQueryByFailureLog()
	  {
		runtimeService.startProcessInstanceByKey("process");
		string jobId = managementService.createJobQuery().singleResult().Id;
		try
		{
		  managementService.executeJob(jobId);
		  fail();
		}
		catch (Exception)
		{
		  // expected
		}

		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().failureLog();

		verifyQueryResults(query, 1);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testQueryBySuccessLog()
	  {
		runtimeService.startProcessInstanceByKey("process", Variables.createVariables().putValue("fail", false));
		string jobId = managementService.createJobQuery().singleResult().Id;
		managementService.executeJob(jobId);

		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().successLog();

		verifyQueryResults(query, 1);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testQueryByDeletionLog()
	  {
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;
		runtimeService.deleteProcessInstance(processInstanceId, null);

		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().deletionLog();

		verifyQueryResults(query, 1);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testQuerySorting()
	  {
		for (int i = 0; i < 10; i++)
		{
		  runtimeService.startProcessInstanceByKey("process");
		}

		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery();

		// asc
		query.orderByTimestamp().asc();
		verifyQueryWithOrdering(query, 10, historicJobLogByTimestamp());

		query = historyService.createHistoricJobLogQuery();

		query.orderByJobId().asc();
		verifyQueryWithOrdering(query, 10, historicJobLogByJobId());

		query = historyService.createHistoricJobLogQuery();

		query.orderByJobDefinitionId().asc();
		verifyQueryWithOrdering(query, 10, historicJobLogByJobDefinitionId());

		query = historyService.createHistoricJobLogQuery();

		query.orderByJobDueDate().asc();
		verifyQueryWithOrdering(query, 10, historicJobLogByJobDueDate());

		query = historyService.createHistoricJobLogQuery();

		query.orderByJobRetries().asc();
		verifyQueryWithOrdering(query, 10, historicJobLogByJobRetries());

		query = historyService.createHistoricJobLogQuery();

		query.orderByActivityId().asc();
		verifyQueryWithOrdering(query, 10, historicJobLogByActivityId());

		query = historyService.createHistoricJobLogQuery();

		query.orderByExecutionId().asc();
		verifyQueryWithOrdering(query, 10, historicJobLogByExecutionId());

		query = historyService.createHistoricJobLogQuery();

		query.orderByProcessInstanceId().asc();
		verifyQueryWithOrdering(query, 10, historicJobLogByProcessInstanceId());

		query = historyService.createHistoricJobLogQuery();

		query.orderByProcessDefinitionId().asc();
		verifyQueryWithOrdering(query, 10, historicJobLogByProcessDefinitionId());

		query = historyService.createHistoricJobLogQuery();

		query.orderByProcessDefinitionKey().asc();
		verifyQueryWithOrdering(query, 10, historicJobLogByProcessDefinitionKey(processEngine));

		query = historyService.createHistoricJobLogQuery();

		query.orderByDeploymentId().asc();
		verifyQueryWithOrdering(query, 10, historicJobLogByDeploymentId());

		query = historyService.createHistoricJobLogQuery();

		query.orderByJobPriority().asc();

		verifyQueryWithOrdering(query, 10, historicJobLogByJobPriority());

		// desc
		query.orderByTimestamp().desc();
		verifyQueryWithOrdering(query, 10, inverted(historicJobLogByTimestamp()));

		query = historyService.createHistoricJobLogQuery();

		query.orderByJobId().desc();
		verifyQueryWithOrdering(query, 10, inverted(historicJobLogByJobId()));

		query = historyService.createHistoricJobLogQuery();

		query.orderByJobDefinitionId().asc();
		verifyQueryWithOrdering(query, 10, inverted(historicJobLogByJobDefinitionId()));

		query = historyService.createHistoricJobLogQuery();

		query.orderByJobDueDate().desc();
		verifyQueryWithOrdering(query, 10, inverted(historicJobLogByJobDueDate()));

		query = historyService.createHistoricJobLogQuery();

		query.orderByJobRetries().desc();
		verifyQueryWithOrdering(query, 10, inverted(historicJobLogByJobRetries()));

		query = historyService.createHistoricJobLogQuery();

		query.orderByActivityId().desc();
		verifyQueryWithOrdering(query, 10, inverted(historicJobLogByActivityId()));

		query = historyService.createHistoricJobLogQuery();

		query.orderByExecutionId().desc();
		verifyQueryWithOrdering(query, 10, inverted(historicJobLogByExecutionId()));

		query = historyService.createHistoricJobLogQuery();

		query.orderByProcessInstanceId().desc();
		verifyQueryWithOrdering(query, 10, inverted(historicJobLogByProcessInstanceId()));

		query = historyService.createHistoricJobLogQuery();

		query.orderByProcessDefinitionId().desc();
		verifyQueryWithOrdering(query, 10, inverted(historicJobLogByProcessDefinitionId()));

		query = historyService.createHistoricJobLogQuery();

		query.orderByProcessDefinitionKey().desc();
		verifyQueryWithOrdering(query, 10, inverted(historicJobLogByProcessDefinitionKey(processEngine)));

		query = historyService.createHistoricJobLogQuery();

		query.orderByDeploymentId().desc();
		verifyQueryWithOrdering(query, 10, inverted(historicJobLogByDeploymentId()));

		query = historyService.createHistoricJobLogQuery();

		query.orderByJobPriority().desc();

	  verifyQueryWithOrdering(query, 10, inverted(historicJobLogByJobPriority()));
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testQuerySortingPartiallyByOccurrence()
	  {
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;
		string jobId = managementService.createJobQuery().singleResult().Id;

		executeAvailableJobs();
		runtimeService.setVariable(processInstanceId, "fail", false);
		managementService.executeJob(jobId);

		// asc
		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().jobId(jobId).orderPartiallyByOccurrence().asc();

		verifyQueryWithOrdering(query, 5, historicJobLogPartiallyByOccurence());

		// desc
		query = historyService.createHistoricJobLogQuery().jobId(jobId).orderPartiallyByOccurrence().desc();

		verifyQueryWithOrdering(query, 5, inverted(historicJobLogPartiallyByOccurence()));

		runtimeService.deleteProcessInstance(processInstanceId, null);

		// delete job /////////////////////////////////////////////////////////

		processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;
		jobId = managementService.createJobQuery().singleResult().Id;

		executeAvailableJobs();
		managementService.deleteJob(jobId);

		// asc
		query = historyService.createHistoricJobLogQuery().jobId(jobId).orderPartiallyByOccurrence().asc();

		verifyQueryWithOrdering(query, 5, historicJobLogPartiallyByOccurence());

		// desc
		query = historyService.createHistoricJobLogQuery().jobId(jobId).orderPartiallyByOccurrence().desc();

		verifyQueryWithOrdering(query, 5, inverted(historicJobLogPartiallyByOccurence()));
	  }

	  protected internal virtual void verifyQueryResults(HistoricJobLogQuery query, int countExpected)
	  {
		assertEquals(countExpected, query.list().size());
		assertEquals(countExpected, query.count());

		if (countExpected == 1)
		{
		  assertNotNull(query.singleResult());
		}
		else if (countExpected > 1)
		{
		  verifySingleResultFails(query);
		}
		else if (countExpected == 0)
		{
		  assertNull(query.singleResult());
		}
	  }

	  protected internal virtual void verifyQueryWithOrdering(HistoricJobLogQuery query, int countExpected, TestOrderingUtil.NullTolerantComparator<HistoricJobLog> expectedOrdering)
	  {
		verifyQueryResults(query, countExpected);
		TestOrderingUtil.verifySorting(query.list(), expectedOrdering);
	  }

	  protected internal virtual void verifySingleResultFails(HistoricJobLogQuery query)
	  {
		try
		{
		  query.singleResult();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	}

}