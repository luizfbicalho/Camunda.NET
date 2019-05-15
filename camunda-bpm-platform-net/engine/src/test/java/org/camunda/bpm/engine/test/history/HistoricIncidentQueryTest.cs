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
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;


	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using HistoricIncident = org.camunda.bpm.engine.history.HistoricIncident;
	using HistoricIncidentQuery = org.camunda.bpm.engine.history.HistoricIncidentQuery;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using FailingDelegate = org.camunda.bpm.engine.test.api.runtime.FailingDelegate;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using CoreMatchers = org.hamcrest.CoreMatchers;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// 
	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoricIncidentQueryTest
	{
		private bool InstanceFieldsInitialized = false;

		public HistoricIncidentQueryTest()
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


	  protected internal RuntimeService runtimeService;
	  protected internal ManagementService managementService;
	  protected internal HistoryService historyService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		runtimeService = engineRule.RuntimeService;
		managementService = engineRule.ManagementService;
		historyService = engineRule.HistoryService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"}) public void testQueryByIncidentId()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"})]
	  public virtual void testQueryByIncidentId()
	  {
		startProcessInstance(PROCESS_DEFINITION_KEY);

		string incidentId = historyService.createHistoricIncidentQuery().singleResult().Id;

		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery().incidentId(incidentId);

		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"}) public void testQueryByInvalidIncidentId()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"})]
	  public virtual void testQueryByInvalidIncidentId()
	  {
		startProcessInstance(PROCESS_DEFINITION_KEY);

		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery();

		assertEquals(0, query.incidentId("invalid").list().size());
		assertEquals(0, query.incidentId("invalid").count());

		try
		{
		  query.incidentId(null);
		  fail("It was possible to set a null value as incidentId.");
		}
		catch (ProcessEngineException)
		{
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"}) public void testQueryByIncidentType()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"})]
	  public virtual void testQueryByIncidentType()
	  {
		startProcessInstance(PROCESS_DEFINITION_KEY);

		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery().incidentType(org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE);

		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidIncidentType()
	  public virtual void testQueryByInvalidIncidentType()
	  {
		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery();

		assertEquals(0, query.incidentType("invalid").list().size());
		assertEquals(0, query.incidentType("invalid").count());

		try
		{
		  query.incidentType(null);
		  fail("It was possible to set a null value as incidentType.");
		}
		catch (ProcessEngineException)
		{
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"}) public void testQueryByIncidentMessage()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"})]
	  public virtual void testQueryByIncidentMessage()
	  {
		startProcessInstance(PROCESS_DEFINITION_KEY);

		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery().incidentMessage("exception0");

		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidIncidentMessage()
	  public virtual void testQueryByInvalidIncidentMessage()
	  {
		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery();

		assertEquals(0, query.incidentMessage("invalid").list().size());
		assertEquals(0, query.incidentMessage("invalid").count());

		try
		{
		  query.incidentMessage(null);
		  fail("It was possible to set a null value as incidentMessage.");
		}
		catch (ProcessEngineException)
		{
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"}) public void testQueryByProcessDefinitionId()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"})]
	  public virtual void testQueryByProcessDefinitionId()
	  {
		startProcessInstance(PROCESS_DEFINITION_KEY);

		ProcessInstance pi = runtimeService.createProcessInstanceQuery().singleResult();

		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery().processDefinitionId(pi.ProcessDefinitionId);

		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidProcessDefinitionId()
	  public virtual void testQueryByInvalidProcessDefinitionId()
	  {
		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery();

		assertEquals(0, query.processDefinitionId("invalid").list().size());
		assertEquals(0, query.processDefinitionId("invalid").count());

		try
		{
		  query.processDefinitionId(null);
		  fail("It was possible to set a null value as processDefinitionId.");
		}
		catch (ProcessEngineException)
		{
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"}) public void testQueryByProcessInstanceId()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"})]
	  public virtual void testQueryByProcessInstanceId()
	  {
		startProcessInstance(PROCESS_DEFINITION_KEY);

		ProcessInstance pi = runtimeService.createProcessInstanceQuery().singleResult();

		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery().processInstanceId(pi.Id);

		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidProcessInstanceId()
	  public virtual void testQueryByInvalidProcessInstanceId()
	  {
		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery();

		assertEquals(0, query.processInstanceId("invalid").list().size());
		assertEquals(0, query.processInstanceId("invalid").count());

		try
		{
		  query.processInstanceId(null);
		  fail("It was possible to set a null value as processInstanceId.");
		}
		catch (ProcessEngineException)
		{
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"}) public void testQueryByExecutionId()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"})]
	  public virtual void testQueryByExecutionId()
	  {
		startProcessInstance(PROCESS_DEFINITION_KEY);

		ProcessInstance pi = runtimeService.createProcessInstanceQuery().singleResult();

		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery().executionId(pi.Id);

		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidExecutionId()
	  public virtual void testQueryByInvalidExecutionId()
	  {
		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery();

		assertEquals(0, query.executionId("invalid").list().size());
		assertEquals(0, query.executionId("invalid").count());

		try
		{
		  query.executionId(null);
		  fail("It was possible to set a null value as executionId.");
		}
		catch (ProcessEngineException)
		{
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"}) public void testQueryByActivityId()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"})]
	  public virtual void testQueryByActivityId()
	  {
		startProcessInstance(PROCESS_DEFINITION_KEY);

		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery().activityId("theServiceTask");

		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidActivityId()
	  public virtual void testQueryByInvalidActivityId()
	  {
		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery();

		assertEquals(0, query.activityId("invalid").list().size());
		assertEquals(0, query.activityId("invalid").count());

		try
		{
		  query.activityId(null);
		  fail("It was possible to set a null value as activityId.");
		}
		catch (ProcessEngineException)
		{
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/history/HistoricIncidentQueryTest.testQueryByCauseIncidentId.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"}) public void testQueryByCauseIncidentId()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/history/HistoricIncidentQueryTest.testQueryByCauseIncidentId.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"})]
	  public virtual void testQueryByCauseIncidentId()
	  {
		startProcessInstance("process");

		string processInstanceId = runtimeService.createProcessInstanceQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).singleResult().Id;

		Incident incident = runtimeService.createIncidentQuery().processInstanceId(processInstanceId).singleResult();

		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery().causeIncidentId(incident.Id);

		assertEquals(2, query.list().size());
		assertEquals(2, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidCauseIncidentId()
	  public virtual void testQueryByInvalidCauseIncidentId()
	  {
		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery();

		assertEquals(0, query.causeIncidentId("invalid").list().size());
		assertEquals(0, query.causeIncidentId("invalid").count());

		try
		{
		  query.causeIncidentId(null);
		  fail("It was possible to set a null value as causeIncidentId.");
		}
		catch (ProcessEngineException)
		{
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/history/HistoricIncidentQueryTest.testQueryByCauseIncidentId.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"}) public void testQueryByRootCauseIncidentId()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/history/HistoricIncidentQueryTest.testQueryByCauseIncidentId.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"})]
	  public virtual void testQueryByRootCauseIncidentId()
	  {
		startProcessInstance("process");

		string processInstanceId = runtimeService.createProcessInstanceQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).singleResult().Id;

		Incident incident = runtimeService.createIncidentQuery().processInstanceId(processInstanceId).singleResult();

		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery().rootCauseIncidentId(incident.Id);

		assertEquals(2, query.list().size());
		assertEquals(2, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidRootCauseIncidentId()
	  public virtual void testQueryByInvalidRootCauseIncidentId()
	  {
		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery();

		assertEquals(0, query.rootCauseIncidentId("invalid").list().size());
		assertEquals(0, query.rootCauseIncidentId("invalid").count());

		try
		{
		  query.rootCauseIncidentId(null);
		  fail("It was possible to set a null value as rootCauseIncidentId.");
		}
		catch (ProcessEngineException)
		{
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"}) public void testQueryByConfiguration()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"})]
	  public virtual void testQueryByConfiguration()
	  {
		startProcessInstance(PROCESS_DEFINITION_KEY);

		string configuration = managementService.createJobQuery().singleResult().Id;

		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery().configuration(configuration);

		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidConfigurationId()
	  public virtual void testQueryByInvalidConfigurationId()
	  {
		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery();

		assertEquals(0, query.configuration("invalid").list().size());
		assertEquals(0, query.configuration("invalid").count());

		try
		{
		  query.configuration(null);
		  fail("It was possible to set a null value as configuration.");
		}
		catch (ProcessEngineException)
		{
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"}) public void testQueryByOpen()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"})]
	  public virtual void testQueryByOpen()
	  {
		startProcessInstance(PROCESS_DEFINITION_KEY);

		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery().open();

		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidOpen()
	  public virtual void testQueryByInvalidOpen()
	  {
		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery();

		try
		{
		  query.open().open();
		  fail("It was possible to set a the open flag twice.");
		}
		catch (ProcessEngineException)
		{
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"}) public void testQueryByResolved()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"})]
	  public virtual void testQueryByResolved()
	  {
		startProcessInstance(PROCESS_DEFINITION_KEY);

		string jobId = managementService.createJobQuery().singleResult().Id;
		managementService.setJobRetries(jobId, 1);

		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery().resolved();

		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidResolved()
	  public virtual void testQueryByInvalidResolved()
	  {
		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery();

		try
		{
		  query.resolved().resolved();
		  fail("It was possible to set a the resolved flag twice.");
		}
		catch (ProcessEngineException)
		{
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"}) public void testQueryByDeleted()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"})]
	  public virtual void testQueryByDeleted()
	  {
		startProcessInstance(PROCESS_DEFINITION_KEY);

		string processInstanceId = runtimeService.createProcessInstanceQuery().singleResult().Id;
		runtimeService.deleteProcessInstance(processInstanceId, null);

		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery().deleted();

		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidDeleted()
	  public virtual void testQueryByInvalidDeleted()
	  {
		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery();

		try
		{
		  query.deleted().deleted();
		  fail("It was possible to set a the deleted flag twice.");
		}
		catch (ProcessEngineException)
		{
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

		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery().jobDefinitionIdIn(jobDefinitionId1, jobDefinitionId2);

		assertEquals(2, query.list().size());
		assertEquals(2, query.count());

		query = historyService.createHistoricIncidentQuery().jobDefinitionIdIn(jobDefinitionId1);

		assertEquals(1, query.list().size());
		assertEquals(1, query.count());

		query = historyService.createHistoricIncidentQuery().jobDefinitionIdIn(jobDefinitionId2);

		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByUnknownJobDefinitionId()
	  public virtual void testQueryByUnknownJobDefinitionId()
	  {
		string processDefinitionId = testHelper.deployAndGetDefinition(FAILING_SERVICE_TASK_MODEL).Id;

		runtimeService.startProcessInstanceById(processDefinitionId);
		testHelper.executeAvailableJobs();

		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery().jobDefinitionIdIn("unknown");

		assertEquals(0, query.list().size());
		assertEquals(0, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByNullJobDefinitionId()
	  public virtual void testQueryByNullJobDefinitionId()
	  {
		try
		{
		  historyService.createHistoricIncidentQuery().jobDefinitionIdIn((string) null);
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
		  historyService.createHistoricIncidentQuery().jobDefinitionIdIn((string[]) null);
		  fail("Should fail");
		}
		catch (NullValueException e)
		{
		  assertThat(e.Message, CoreMatchers.containsString("jobDefinitionIds is null"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"}) public void testQueryPaging()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"})]
	  public virtual void testQueryPaging()
	  {
		startProcessInstances(PROCESS_DEFINITION_KEY, 4);

		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery();

		assertEquals(4, query.listPage(0, 4).size());
		assertEquals(1, query.listPage(2, 1).size());
		assertEquals(2, query.listPage(1, 2).size());
		assertEquals(3, query.listPage(1, 4).size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"}) public void testQuerySorting()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"})]
	  public virtual void testQuerySorting()
	  {
		startProcessInstances(PROCESS_DEFINITION_KEY, 4);

		assertEquals(4, historyService.createHistoricIncidentQuery().orderByIncidentId().asc().list().size());
		assertEquals(4, historyService.createHistoricIncidentQuery().orderByCreateTime().asc().list().size());
		assertEquals(4, historyService.createHistoricIncidentQuery().orderByEndTime().asc().list().size());
		assertEquals(4, historyService.createHistoricIncidentQuery().orderByIncidentType().asc().list().size());
		assertEquals(4, historyService.createHistoricIncidentQuery().orderByExecutionId().asc().list().size());
		assertEquals(4, historyService.createHistoricIncidentQuery().orderByActivityId().asc().list().size());
		assertEquals(4, historyService.createHistoricIncidentQuery().orderByProcessInstanceId().asc().list().size());
		assertEquals(4, historyService.createHistoricIncidentQuery().orderByProcessDefinitionId().asc().list().size());
		assertEquals(4, historyService.createHistoricIncidentQuery().orderByCauseIncidentId().asc().list().size());
		assertEquals(4, historyService.createHistoricIncidentQuery().orderByRootCauseIncidentId().asc().list().size());
		assertEquals(4, historyService.createHistoricIncidentQuery().orderByConfiguration().asc().list().size());
		assertEquals(4, historyService.createHistoricIncidentQuery().orderByIncidentState().asc().list().size());

		assertEquals(4, historyService.createHistoricIncidentQuery().orderByIncidentId().desc().list().size());
		assertEquals(4, historyService.createHistoricIncidentQuery().orderByCreateTime().desc().list().size());
		assertEquals(4, historyService.createHistoricIncidentQuery().orderByEndTime().desc().list().size());
		assertEquals(4, historyService.createHistoricIncidentQuery().orderByIncidentType().desc().list().size());
		assertEquals(4, historyService.createHistoricIncidentQuery().orderByExecutionId().desc().list().size());
		assertEquals(4, historyService.createHistoricIncidentQuery().orderByActivityId().desc().list().size());
		assertEquals(4, historyService.createHistoricIncidentQuery().orderByProcessInstanceId().desc().list().size());
		assertEquals(4, historyService.createHistoricIncidentQuery().orderByProcessDefinitionId().desc().list().size());
		assertEquals(4, historyService.createHistoricIncidentQuery().orderByCauseIncidentId().desc().list().size());
		assertEquals(4, historyService.createHistoricIncidentQuery().orderByRootCauseIncidentId().desc().list().size());
		assertEquals(4, historyService.createHistoricIncidentQuery().orderByConfiguration().desc().list().size());
		assertEquals(4, historyService.createHistoricIncidentQuery().orderByIncidentState().desc().list().size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"}) public void testQuerySortingByIncidentMessage()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml"})]
	  public virtual void testQuerySortingByIncidentMessage()
	  {
		// given
		startProcessInstances(PROCESS_DEFINITION_KEY, 4);

		// when
		IList<HistoricIncident> ascending = historyService.createHistoricIncidentQuery().orderByIncidentMessage().asc().list();
		IList<HistoricIncident> descending = historyService.createHistoricIncidentQuery().orderByIncidentMessage().desc().list();

		// then
		assertThat(ascending).extracting("incidentMessage").containsExactly("exception0", "exception1", "exception2", "exception3");
		assertThat(descending).extracting("incidentMessage").containsExactly("exception3", "exception2", "exception1", "exception0");
	  }

	  protected internal virtual void startProcessInstance(string key)
	  {
		startProcessInstances(key, 1);
	  }

	  protected internal virtual void startProcessInstances(string key, int numberOfInstances)
	  {

		for (int i = 0; i < numberOfInstances; i++)
		{
		  IDictionary<string, object> variables = Collections.singletonMap<string, object>("message", "exception" + i);

		  runtimeService.startProcessInstanceByKey(key, i + "", variables);
		}

		testHelper.executeAvailableJobs();
	  }
	}

}