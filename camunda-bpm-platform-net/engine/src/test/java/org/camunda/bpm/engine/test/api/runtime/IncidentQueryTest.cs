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
namespace org.camunda.bpm.engine.test.api.runtime
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;


	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using IncidentQuery = org.camunda.bpm.engine.runtime.IncidentQuery;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using CoreMatchers = org.hamcrest.CoreMatchers;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author roman.smirnov
	/// </summary>
	public class IncidentQueryTest
	{
		private bool InstanceFieldsInitialized = false;

		public IncidentQueryTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testHelper = new ProcessEngineTestRule(engineRule);
			chain = RuleChain.outerRule(engineRule).around(testHelper);
		}


	  public static string PROCESS_DEFINITION_KEY = "oneFailingServiceTaskProcess";
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  public static BpmnModelInstance FAILING_SERVICE_TASK_MODEL = Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent("start").serviceTask("task").camundaAsyncBefore().camundaClass(typeof(FailingDelegate).FullName).endEvent("end").done();

	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  public ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain chain = org.junit.rules.RuleChain.outerRule(engineRule).around(testHelper);
	  public RuleChain chain;

	  private IList<string> processInstanceIds;

	  protected internal RuntimeService runtimeService;
	  protected internal ManagementService managementService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		runtimeService = engineRule.RuntimeService;
		managementService = engineRule.ManagementService;
	  }

	  /// <summary>
	  /// Setup starts 4 process instances of oneFailingServiceTaskProcess.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void startProcessInstances() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void startProcessInstances()
	  {
		testHelper.deploy(FAILING_SERVICE_TASK_MODEL);

		processInstanceIds = new List<>();
		for (int i = 0; i < 4; i++)
		{
		  IDictionary<string, object> variables = Collections.singletonMap<string, object>("message", "exception" + i);

		  processInstanceIds.Add(engineRule.RuntimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY, i + "", variables).Id);
		}

		testHelper.executeAvailableJobs();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuery()
	  public virtual void testQuery()
	  {
		IncidentQuery query = runtimeService.createIncidentQuery();
		assertEquals(4, query.count());

		IList<Incident> incidents = query.list();
		assertFalse(incidents.Count == 0);
		assertEquals(4, incidents.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByIncidentType()
	  public virtual void testQueryByIncidentType()
	  {
		IncidentQuery query = runtimeService.createIncidentQuery().incidentType(org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE);
		assertEquals(4, query.count());

		IList<Incident> incidents = query.list();
		assertFalse(incidents.Count == 0);
		assertEquals(4, incidents.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidIncidentType()
	  public virtual void testQueryByInvalidIncidentType()
	  {
		IncidentQuery query = runtimeService.createIncidentQuery().incidentType("invalid");

		assertEquals(0, query.count());

		IList<Incident> incidents = query.list();
		assertTrue(incidents.Count == 0);

		Incident incident = query.singleResult();
		assertNull(incident);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByIncidentMessage()
	  public virtual void testQueryByIncidentMessage()
	  {
		IncidentQuery query = runtimeService.createIncidentQuery().incidentMessage("exception0");
		assertEquals(1, query.count());

		IList<Incident> incidents = query.list();
		assertFalse(incidents.Count == 0);
		assertEquals(1, incidents.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidIncidentMessage()
	  public virtual void testQueryByInvalidIncidentMessage()
	  {
		IncidentQuery query = runtimeService.createIncidentQuery().incidentMessage("invalid");

		assertEquals(0, query.count());

		IList<Incident> incidents = query.list();
		assertTrue(incidents.Count == 0);

		Incident incident = query.singleResult();
		assertNull(incident);
	  }

	  public virtual void testQueryByProcessDefinitionId()
	  {
		string processDefinitionId = engineRule.RepositoryService.createProcessDefinitionQuery().singleResult().Id;

		IncidentQuery query = runtimeService.createIncidentQuery().processDefinitionId(processDefinitionId);
		assertEquals(4, query.count());

		IList<Incident> incidents = query.list();
		assertFalse(incidents.Count == 0);
		assertEquals(4, incidents.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidProcessDefinitionId()
	  public virtual void testQueryByInvalidProcessDefinitionId()
	  {
		IncidentQuery query = runtimeService.createIncidentQuery().processDefinitionId("invalid");

		assertEquals(0, query.count());

		IList<Incident> incidents = query.list();
		assertTrue(incidents.Count == 0);

		Incident incident = query.singleResult();
		assertNull(incident);
	  }

	  public virtual void testQueryByProcessInstanceId()
	  {
		IncidentQuery query = runtimeService.createIncidentQuery().processInstanceId(processInstanceIds[0]);

		assertEquals(1, query.count());

		IList<Incident> incidents = query.list();
		assertFalse(incidents.Count == 0);
		assertEquals(1, incidents.Count);

		Incident incident = query.singleResult();
		assertNotNull(incident);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidProcessInstanceId()
	  public virtual void testQueryByInvalidProcessInstanceId()
	  {
		IncidentQuery query = runtimeService.createIncidentQuery().processInstanceId("invalid");

		assertEquals(0, query.count());

		IList<Incident> incidents = query.list();
		assertTrue(incidents.Count == 0);

		Incident incident = query.singleResult();
		assertNull(incident);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByIncidentId()
	  public virtual void testQueryByIncidentId()
	  {
		Incident incident = runtimeService.createIncidentQuery().processInstanceId(processInstanceIds[0]).singleResult();
		assertNotNull(incident);

		IncidentQuery query = runtimeService.createIncidentQuery().incidentId(incident.Id);

		assertEquals(1, query.count());

		IList<Incident> incidents = query.list();
		assertFalse(incidents.Count == 0);
		assertEquals(1, incidents.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidIncidentId()
	  public virtual void testQueryByInvalidIncidentId()
	  {
		IncidentQuery query = runtimeService.createIncidentQuery().incidentId("invalid");

		assertEquals(0, query.count());

		IList<Incident> incidents = query.list();
		assertTrue(incidents.Count == 0);

		Incident incident = query.singleResult();
		assertNull(incident);
	  }

	  public virtual void testQueryByExecutionId()
	  {
		Execution execution = runtimeService.createExecutionQuery().processInstanceId(processInstanceIds[0]).singleResult();
		assertNotNull(execution);

		IncidentQuery query = runtimeService.createIncidentQuery().executionId(execution.Id);

		assertEquals(1, query.count());

		IList<Incident> incidents = query.list();
		assertFalse(incidents.Count == 0);
		assertEquals(1, incidents.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidExecutionId()
	  public virtual void testQueryByInvalidExecutionId()
	  {
		IncidentQuery query = runtimeService.createIncidentQuery().executionId("invalid");

		assertEquals(0, query.count());

		IList<Incident> incidents = query.list();
		assertTrue(incidents.Count == 0);

		Incident incident = query.singleResult();
		assertNull(incident);
	  }

	  public virtual void testQueryByActivityId()
	  {
		IncidentQuery query = runtimeService.createIncidentQuery().activityId("theServiceTask");
		assertEquals(4, query.count());

		IList<Incident> incidents = query.list();
		assertFalse(incidents.Count == 0);
		assertEquals(4, incidents.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidActivityId()
	  public virtual void testQueryByInvalidActivityId()
	  {
		IncidentQuery query = runtimeService.createIncidentQuery().activityId("invalid");

		assertEquals(0, query.count());

		IList<Incident> incidents = query.list();
		assertTrue(incidents.Count == 0);

		Incident incident = query.singleResult();
		assertNull(incident);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByConfiguration()
	  public virtual void testQueryByConfiguration()
	  {
		string jobId = managementService.createJobQuery().processInstanceId(processInstanceIds[0]).singleResult().Id;

		IncidentQuery query = runtimeService.createIncidentQuery().configuration(jobId);
		assertEquals(1, query.count());

		IList<Incident> incidents = query.list();
		assertFalse(incidents.Count == 0);
		assertEquals(1, incidents.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidConfiguration()
	  public virtual void testQueryByInvalidConfiguration()
	  {
		IncidentQuery query = runtimeService.createIncidentQuery().configuration("invalid");

		assertEquals(0, query.count());

		IList<Incident> incidents = query.list();
		assertTrue(incidents.Count == 0);

		Incident incident = query.singleResult();
		assertNull(incident);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByCauseIncidentIdEqualsNull()
	  public virtual void testQueryByCauseIncidentIdEqualsNull()
	  {
		IncidentQuery query = runtimeService.createIncidentQuery().causeIncidentId(null);
		assertEquals(4, query.count());

		IList<Incident> incidents = query.list();
		assertFalse(incidents.Count == 0);
		assertEquals(4, incidents.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidCauseIncidentId()
	  public virtual void testQueryByInvalidCauseIncidentId()
	  {
		IncidentQuery query = runtimeService.createIncidentQuery().causeIncidentId("invalid");
		assertEquals(0, query.count());

		IList<Incident> incidents = query.list();
		assertTrue(incidents.Count == 0);
		assertEquals(0, incidents.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/runtime/IncidentQueryTest.testQueryByCauseIncidentId.bpmn"}) public void testQueryByCauseIncidentId()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/IncidentQueryTest.testQueryByCauseIncidentId.bpmn"})]
	  public virtual void testQueryByCauseIncidentId()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("callFailingProcess");

		testHelper.executeAvailableJobs();

		ProcessInstance subProcessInstance = runtimeService.createProcessInstanceQuery().superProcessInstanceId(processInstance.Id).singleResult();
		assertNotNull(subProcessInstance);

		Incident causeIncident = runtimeService.createIncidentQuery().processInstanceId(subProcessInstance.Id).singleResult();
		assertNotNull(causeIncident);

		IncidentQuery query = runtimeService.createIncidentQuery().causeIncidentId(causeIncident.Id);
		assertEquals(2, query.count());

		IList<Incident> incidents = query.list();
		assertFalse(incidents.Count == 0);
		assertEquals(2, incidents.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByRootCauseIncidentIdEqualsNull()
	  public virtual void testQueryByRootCauseIncidentIdEqualsNull()
	  {
		IncidentQuery query = runtimeService.createIncidentQuery().rootCauseIncidentId(null);
		assertEquals(4, query.count());

		IList<Incident> incidents = query.list();
		assertFalse(incidents.Count == 0);
		assertEquals(4, incidents.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByRootInvalidCauseIncidentId()
	  public virtual void testQueryByRootInvalidCauseIncidentId()
	  {
		IncidentQuery query = runtimeService.createIncidentQuery().rootCauseIncidentId("invalid");
		assertEquals(0, query.count());

		IList<Incident> incidents = query.list();
		assertTrue(incidents.Count == 0);
		assertEquals(0, incidents.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/runtime/IncidentQueryTest.testQueryByRootCauseIncidentId.bpmn", "org/camunda/bpm/engine/test/api/runtime/IncidentQueryTest.testQueryByCauseIncidentId.bpmn"}) public void testQueryByRootCauseIncidentId()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/IncidentQueryTest.testQueryByRootCauseIncidentId.bpmn", "org/camunda/bpm/engine/test/api/runtime/IncidentQueryTest.testQueryByCauseIncidentId.bpmn"})]
	  public virtual void testQueryByRootCauseIncidentId()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("callFailingCallActivity");

		testHelper.executeAvailableJobs();

		ProcessInstance subProcessInstance = runtimeService.createProcessInstanceQuery().superProcessInstanceId(processInstance.Id).singleResult();
		assertNotNull(subProcessInstance);

		ProcessInstance failingSubProcessInstance = runtimeService.createProcessInstanceQuery().superProcessInstanceId(subProcessInstance.Id).singleResult();
		assertNotNull(subProcessInstance);

		Incident incident = runtimeService.createIncidentQuery().processInstanceId(failingSubProcessInstance.Id).singleResult();
		assertNotNull(incident);

		IncidentQuery query = runtimeService.createIncidentQuery().rootCauseIncidentId(incident.Id);
		assertEquals(3, query.count());

		IList<Incident> incidents = query.list();
		assertFalse(incidents.Count == 0);
		assertEquals(3, incidents.Count);

		try
		{
		  query.singleResult();
		  fail();
		}
		catch (ProcessEngineException)
		{
		  // Exception is expected
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByJobDefinitionId()
	  public virtual void testQueryByJobDefinitionId()
	  {
		string processDefinitionId1 = testHelper.deployAndGetDefinition(FAILING_SERVICE_TASK_MODEL).Id;
		string processDefinitionId2 = testHelper.deployAndGetDefinition(FAILING_SERVICE_TASK_MODEL).Id;

		runtimeService.startProcessInstanceById(processDefinitionId1);
		runtimeService.startProcessInstanceById(processDefinitionId2);
		testHelper.executeAvailableJobs();

		string jobDefinitionId1 = managementService.createJobQuery().processDefinitionId(processDefinitionId1).singleResult().JobDefinitionId;
		string jobDefinitionId2 = managementService.createJobQuery().processDefinitionId(processDefinitionId2).singleResult().JobDefinitionId;

		IncidentQuery query = runtimeService.createIncidentQuery().jobDefinitionIdIn(jobDefinitionId1, jobDefinitionId2);

		assertEquals(2, query.list().size());
		assertEquals(2, query.count());

		query = runtimeService.createIncidentQuery().jobDefinitionIdIn(jobDefinitionId1);

		assertEquals(1, query.list().size());
		assertEquals(1, query.count());

		query = runtimeService.createIncidentQuery().jobDefinitionIdIn(jobDefinitionId2);

		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByUnknownJobDefinitionId()
	  public virtual void testQueryByUnknownJobDefinitionId()
	  {
		IncidentQuery query = runtimeService.createIncidentQuery().jobDefinitionIdIn("unknown");
		assertEquals(0, query.count());

		IList<Incident> incidents = query.list();
		assertEquals(0, incidents.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByNullJobDefinitionId()
	  public virtual void testQueryByNullJobDefinitionId()
	  {
		try
		{
		  runtimeService.createIncidentQuery().jobDefinitionIdIn((string) null);
		  fail("Should fail");
		}
		catch (NullValueException e)
		{
		  assertThat(e.Message, CoreMatchers.containsString("jobDefinitionIds contains null value"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByNullJobDefinitionIds()
	  public virtual void testQueryByNullJobDefinitionIds()
	  {
		try
		{
		  runtimeService.createIncidentQuery().jobDefinitionIdIn((string[]) null);
		  fail("Should fail");
		}
		catch (NullValueException e)
		{
		  assertThat(e.Message, CoreMatchers.containsString("jobDefinitionIds is null"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryPaging()
	  public virtual void testQueryPaging()
	  {
		assertEquals(4, runtimeService.createIncidentQuery().listPage(0, 4).size());
		assertEquals(1, runtimeService.createIncidentQuery().listPage(2, 1).size());
		assertEquals(2, runtimeService.createIncidentQuery().listPage(1, 2).size());
		assertEquals(3, runtimeService.createIncidentQuery().listPage(1, 4).size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySorting()
	  public virtual void testQuerySorting()
	  {
		assertEquals(4, runtimeService.createIncidentQuery().orderByIncidentId().asc().list().size());
		assertEquals(4, runtimeService.createIncidentQuery().orderByIncidentTimestamp().asc().list().size());
		assertEquals(4, runtimeService.createIncidentQuery().orderByIncidentType().asc().list().size());
		assertEquals(4, runtimeService.createIncidentQuery().orderByExecutionId().asc().list().size());
		assertEquals(4, runtimeService.createIncidentQuery().orderByActivityId().asc().list().size());
		assertEquals(4, runtimeService.createIncidentQuery().orderByProcessInstanceId().asc().list().size());
		assertEquals(4, runtimeService.createIncidentQuery().orderByProcessDefinitionId().asc().list().size());
		assertEquals(4, runtimeService.createIncidentQuery().orderByCauseIncidentId().asc().list().size());
		assertEquals(4, runtimeService.createIncidentQuery().orderByRootCauseIncidentId().asc().list().size());
		assertEquals(4, runtimeService.createIncidentQuery().orderByConfiguration().asc().list().size());

		assertEquals(4, runtimeService.createIncidentQuery().orderByIncidentId().desc().list().size());
		assertEquals(4, runtimeService.createIncidentQuery().orderByIncidentTimestamp().desc().list().size());
		assertEquals(4, runtimeService.createIncidentQuery().orderByIncidentType().desc().list().size());
		assertEquals(4, runtimeService.createIncidentQuery().orderByExecutionId().desc().list().size());
		assertEquals(4, runtimeService.createIncidentQuery().orderByActivityId().desc().list().size());
		assertEquals(4, runtimeService.createIncidentQuery().orderByProcessInstanceId().desc().list().size());
		assertEquals(4, runtimeService.createIncidentQuery().orderByProcessDefinitionId().desc().list().size());
		assertEquals(4, runtimeService.createIncidentQuery().orderByCauseIncidentId().desc().list().size());
		assertEquals(4, runtimeService.createIncidentQuery().orderByRootCauseIncidentId().desc().list().size());
		assertEquals(4, runtimeService.createIncidentQuery().orderByConfiguration().desc().list().size());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySortingByIncidentMessage()
	  public virtual void testQuerySortingByIncidentMessage()
	  {
		// given

		// when
		IList<Incident> ascending = runtimeService.createIncidentQuery().orderByIncidentMessage().asc().list();
		IList<Incident> descending = runtimeService.createIncidentQuery().orderByIncidentMessage().desc().list();

		// then
		assertThat(ascending).extracting("incidentMessage").containsExactly("exception0", "exception1", "exception2", "exception3");
		assertThat(descending).extracting("incidentMessage").containsExactly("exception3", "exception2", "exception1", "exception0");
	  }
	}

}