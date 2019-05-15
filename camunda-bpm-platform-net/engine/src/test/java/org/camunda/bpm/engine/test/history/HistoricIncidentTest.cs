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

	using HistoricIncident = org.camunda.bpm.engine.history.HistoricIncident;
	using HistoricIncidentQuery = org.camunda.bpm.engine.history.HistoricIncidentQuery;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;

	/// 
	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoricIncidentTest : PluggableProcessEngineTestCase
	{

	  private static string PROCESS_DEFINITION_KEY = "oneFailingServiceTaskProcess";

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"})]
	  public virtual void testPropertiesOfHistoricIncident()
	  {
		startProcessInstance(PROCESS_DEFINITION_KEY);

		Incident incident = runtimeService.createIncidentQuery().singleResult();
		assertNotNull(incident);

		HistoricIncident historicIncident = historyService.createHistoricIncidentQuery().singleResult();
		assertNotNull(historicIncident);

		assertEquals(incident.Id, historicIncident.Id);
		assertEquals(incident.IncidentTimestamp, historicIncident.CreateTime);
		assertNull(historicIncident.EndTime);
		assertEquals(incident.IncidentType, historicIncident.IncidentType);
		assertEquals(incident.IncidentMessage, historicIncident.IncidentMessage);
		assertEquals(incident.ExecutionId, historicIncident.ExecutionId);
		assertEquals(incident.ActivityId, historicIncident.ActivityId);
		assertEquals(incident.ProcessInstanceId, historicIncident.ProcessInstanceId);
		assertEquals(incident.ProcessDefinitionId, historicIncident.ProcessDefinitionId);
		assertEquals(PROCESS_DEFINITION_KEY, historicIncident.ProcessDefinitionKey);
		assertEquals(incident.CauseIncidentId, historicIncident.CauseIncidentId);
		assertEquals(incident.RootCauseIncidentId, historicIncident.RootCauseIncidentId);
		assertEquals(incident.Configuration, historicIncident.Configuration);
		assertEquals(incident.JobDefinitionId, historicIncident.JobDefinitionId);

		assertTrue(historicIncident.Open);
		assertFalse(historicIncident.Deleted);
		assertFalse(historicIncident.Resolved);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"})]
	  public virtual void testCreateSecondHistoricIncident()
	  {
		startProcessInstance(PROCESS_DEFINITION_KEY);

		string jobId = managementService.createJobQuery().singleResult().Id;
		managementService.setJobRetries(jobId, 1);

		executeAvailableJobs();

		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery();
		assertEquals(2, query.count());

		// the first historic incident has been resolved
		assertEquals(1, query.resolved().count());

		query = historyService.createHistoricIncidentQuery();
		// a new historic incident exists which is open
		assertEquals(1, query.open().count());
	  }


	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"})]
	  public virtual void testSetHistoricIncidentToResolved()
	  {
		startProcessInstance(PROCESS_DEFINITION_KEY);

		string jobId = managementService.createJobQuery().singleResult().Id;
		managementService.setJobRetries(jobId, 1);

		HistoricIncident historicIncident = historyService.createHistoricIncidentQuery().singleResult();
		assertNotNull(historicIncident);

		assertNotNull(historicIncident.EndTime);

		assertFalse(historicIncident.Open);
		assertFalse(historicIncident.Deleted);
		assertTrue(historicIncident.Resolved);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/history/HistoricIncidentQueryTest.testQueryByCauseIncidentId.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"})]
	  public virtual void testSetHistoricIncidentToResolvedRecursive()
	  {
		startProcessInstance("process");

		string jobId = managementService.createJobQuery().singleResult().Id;
		managementService.setJobRetries(jobId, 1);

		IList<HistoricIncident> historicIncidents = historyService.createHistoricIncidentQuery().list();

		foreach (HistoricIncident historicIncident in historicIncidents)
		{
		  assertNotNull(historicIncident.EndTime);

		  assertFalse(historicIncident.Open);
		  assertFalse(historicIncident.Deleted);
		  assertTrue(historicIncident.Resolved);
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"})]
	  public virtual void testSetHistoricIncidentToDeleted()
	  {
		startProcessInstance(PROCESS_DEFINITION_KEY);

		string processInstanceId = runtimeService.createProcessInstanceQuery().singleResult().Id;
		runtimeService.deleteProcessInstance(processInstanceId, null);

		HistoricIncident historicIncident = historyService.createHistoricIncidentQuery().singleResult();
		assertNotNull(historicIncident);

		assertNotNull(historicIncident.EndTime);

		assertFalse(historicIncident.Open);
		assertTrue(historicIncident.Deleted);
		assertFalse(historicIncident.Resolved);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/history/HistoricIncidentQueryTest.testQueryByCauseIncidentId.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"})]
	  public virtual void testSetHistoricIncidentToDeletedRecursive()
	  {
		startProcessInstance("process");

		string processInstanceId = runtimeService.createProcessInstanceQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).singleResult().Id;
		runtimeService.deleteProcessInstance(processInstanceId, null);

		IList<HistoricIncident> historicIncidents = historyService.createHistoricIncidentQuery().list();

		foreach (HistoricIncident historicIncident in historicIncidents)
		{
		  assertNotNull(historicIncident.EndTime);

		  assertFalse(historicIncident.Open);
		  assertTrue(historicIncident.Deleted);
		  assertFalse(historicIncident.Resolved);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCreateHistoricIncidentForNestedExecution()
	  public virtual void testCreateHistoricIncidentForNestedExecution()
	  {
		startProcessInstance("process");

		Execution execution = runtimeService.createExecutionQuery().activityId("serviceTask").singleResult();
		assertNotNull(execution);

		HistoricIncident historicIncident = historyService.createHistoricIncidentQuery().singleResult();
		assertNotNull(historicIncident);

		assertEquals(execution.Id, historicIncident.ExecutionId);
		assertEquals("serviceTask", historicIncident.ActivityId);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/history/HistoricIncidentQueryTest.testQueryByCauseIncidentId.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"})]
	  public virtual void testCreateRecursiveHistoricIncidents()
	  {
		startProcessInstance("process");

		ProcessInstance pi1 = runtimeService.createProcessInstanceQuery().processDefinitionKey("process").singleResult();
		assertNotNull(pi1);

		ProcessInstance pi2 = runtimeService.createProcessInstanceQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).singleResult();
		assertNotNull(pi2);

		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery();

		HistoricIncident rootCauseHistoricIncident = query.processInstanceId(pi2.Id).singleResult();
		assertNotNull(rootCauseHistoricIncident);

		// cause and root cause id is equal to the id of the root incident
		assertEquals(rootCauseHistoricIncident.Id, rootCauseHistoricIncident.CauseIncidentId);
		assertEquals(rootCauseHistoricIncident.Id, rootCauseHistoricIncident.RootCauseIncidentId);

		HistoricIncident historicIncident = query.processInstanceId(pi1.Id).singleResult();
		assertNotNull(historicIncident);

		// cause and root cause id is equal to the id of the root incident
		assertEquals(rootCauseHistoricIncident.Id, historicIncident.CauseIncidentId);
		assertEquals(rootCauseHistoricIncident.Id, historicIncident.RootCauseIncidentId);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/history/HistoricIncidentTest.testCreateRecursiveHistoricIncidentsForNestedCallActivities.bpmn20.xml", "org/camunda/bpm/engine/test/history/HistoricIncidentQueryTest.testQueryByCauseIncidentId.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"})]
	  public virtual void testCreateRecursiveHistoricIncidentsForNestedCallActivities()
	  {
		startProcessInstance("process1");

		ProcessInstance pi1 = runtimeService.createProcessInstanceQuery().processDefinitionKey("process1").singleResult();
		assertNotNull(pi1);

		ProcessInstance pi2 = runtimeService.createProcessInstanceQuery().processDefinitionKey("process").singleResult();
		assertNotNull(pi2);

		ProcessInstance pi3 = runtimeService.createProcessInstanceQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).singleResult();
		assertNotNull(pi3);

		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery();

		HistoricIncident rootCauseHistoricIncident = query.processInstanceId(pi3.Id).singleResult();
		assertNotNull(rootCauseHistoricIncident);

		// cause and root cause id is equal to the id of the root incident
		assertEquals(rootCauseHistoricIncident.Id, rootCauseHistoricIncident.CauseIncidentId);
		assertEquals(rootCauseHistoricIncident.Id, rootCauseHistoricIncident.RootCauseIncidentId);

		HistoricIncident causeHistoricIncident = query.processInstanceId(pi2.Id).singleResult();
		assertNotNull(causeHistoricIncident);

		// cause and root cause id is equal to the id of the root incident
		assertEquals(rootCauseHistoricIncident.Id, causeHistoricIncident.CauseIncidentId);
		assertEquals(rootCauseHistoricIncident.Id, causeHistoricIncident.RootCauseIncidentId);

		HistoricIncident historicIncident = query.processInstanceId(pi1.Id).singleResult();
		assertNotNull(historicIncident);

		// cause and root cause id is equal to the id of the root incident
		assertEquals(causeHistoricIncident.Id, historicIncident.CauseIncidentId);
		assertEquals(rootCauseHistoricIncident.Id, historicIncident.RootCauseIncidentId);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"})]
	  public virtual void testDoNotCreateNewIncident()
	  {
		startProcessInstance(PROCESS_DEFINITION_KEY);

		ProcessInstance pi = runtimeService.createProcessInstanceQuery().singleResult();

		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery().processInstanceId(pi.Id);
		HistoricIncident incident = query.singleResult();
		assertNotNull(incident);

		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// set retries to 1 by job definition id
		managementService.setJobRetriesByJobDefinitionId(jobDefinition.Id, 1);

		// the incident still exists
		HistoricIncident tmp = query.singleResult();
		assertEquals(incident.Id, tmp.Id);
		assertNull(tmp.EndTime);
		assertTrue(tmp.Open);

		// execute the available job (should fail again)
		executeAvailableJobs();

		// the incident still exists and there
		// should be not a new incident
		assertEquals(1, query.count());
		tmp = query.singleResult();
		assertEquals(incident.Id, tmp.Id);
		assertNull(tmp.EndTime);
		assertTrue(tmp.Open);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"})]
	  public virtual void testSetRetriesByJobDefinitionIdResolveIncident()
	  {
		startProcessInstance(PROCESS_DEFINITION_KEY);

		ProcessInstance pi = runtimeService.createProcessInstanceQuery().singleResult();

		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery().processInstanceId(pi.Id);
		HistoricIncident incident = query.singleResult();
		assertNotNull(incident);

		runtimeService.setVariable(pi.Id, "fail", false);

		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// set retries to 1 by job definition id
		managementService.setJobRetriesByJobDefinitionId(jobDefinition.Id, 1);

		// the incident still exists
		HistoricIncident tmp = query.singleResult();
		assertEquals(incident.Id, tmp.Id);
		assertNull(tmp.EndTime);
		assertTrue(tmp.Open);

		// execute the available job (should fail again)
		executeAvailableJobs();

		// the incident still exists and there
		// should be not a new incident
		assertEquals(1, query.count());
		tmp = query.singleResult();
		assertEquals(incident.Id, tmp.Id);
		assertNotNull(tmp.EndTime);
		assertTrue(tmp.Resolved);

		assertProcessEnded(pi.Id);
	  }

	  protected internal virtual void startProcessInstance(string key)
	  {
		startProcessInstances(key, 1);
	  }

	  protected internal virtual void startProcessInstances(string key, int numberOfInstances)
	  {
		for (int i = 0; i < numberOfInstances; i++)
		{
		  runtimeService.startProcessInstanceByKey(key);
		}

		executeAvailableJobs();
	  }

	}

}