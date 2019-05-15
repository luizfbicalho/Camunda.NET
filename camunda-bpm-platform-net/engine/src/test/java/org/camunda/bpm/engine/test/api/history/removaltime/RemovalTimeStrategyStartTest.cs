using System;
using System.Collections.Generic;
using System.IO;

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
namespace org.camunda.bpm.engine.test.api.history.removaltime
{
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using HistoricDecisionInputInstance = org.camunda.bpm.engine.history.HistoricDecisionInputInstance;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricDecisionOutputInstance = org.camunda.bpm.engine.history.HistoricDecisionOutputInstance;
	using HistoricDetail = org.camunda.bpm.engine.history.HistoricDetail;
	using HistoricExternalTaskLog = org.camunda.bpm.engine.history.HistoricExternalTaskLog;
	using HistoricIdentityLinkLog = org.camunda.bpm.engine.history.HistoricIdentityLinkLog;
	using HistoricIncident = org.camunda.bpm.engine.history.HistoricIncident;
	using HistoricJobLog = org.camunda.bpm.engine.history.HistoricJobLog;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricTaskInstance = org.camunda.bpm.engine.history.HistoricTaskInstance;
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using DefaultHistoryRemovalTimeProvider = org.camunda.bpm.engine.impl.history.DefaultHistoryRemovalTimeProvider;
	using HistoricDecisionInputInstanceEntity = org.camunda.bpm.engine.impl.history.@event.HistoricDecisionInputInstanceEntity;
	using HistoricDecisionOutputInstanceEntity = org.camunda.bpm.engine.impl.history.@event.HistoricDecisionOutputInstanceEntity;
	using HistoricExternalTaskLogEntity = org.camunda.bpm.engine.impl.history.@event.HistoricExternalTaskLogEntity;
	using AttachmentEntity = org.camunda.bpm.engine.impl.persistence.entity.AttachmentEntity;
	using ByteArrayEntity = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayEntity;
	using HistoricDetailVariableInstanceUpdateEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricDetailVariableInstanceUpdateEntity;
	using HistoricJobLogEventEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricJobLogEventEntity;
	using HistoricVariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricVariableInstanceEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using DeploymentWithDefinitions = org.camunda.bpm.engine.repository.DeploymentWithDefinitions;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Attachment = org.camunda.bpm.engine.task.Attachment;
	using Comment = org.camunda.bpm.engine.task.Comment;
	using Task = org.camunda.bpm.engine.task.Task;
	using FailingDelegate = org.camunda.bpm.engine.test.bpmn.async.FailingDelegate;
	using TestPojo = org.camunda.bpm.engine.test.dmn.businessruletask.TestPojo;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Test = org.junit.Test;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_REMOVAL_TIME_STRATEGY_START;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.MatcherAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.IsNull.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.IsNull.nullValue;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class RemovalTimeStrategyStartTest : AbstractRemovalTimeTest
	{
		private bool InstanceFieldsInitialized = false;

		public RemovalTimeStrategyStartTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			CALLED_PROCESS = Bpmn.createExecutableProcess(CALLED_PROCESS_KEY).startEvent().userTask("userTask").name("userTask").serviceTask().camundaAsyncBefore().camundaClass(typeof(FailingDelegate).FullName).endEvent().done();
			CALLING_PROCESS = Bpmn.createExecutableProcess(CALLING_PROCESS_KEY).camundaHistoryTimeToLive(5).startEvent().callActivity().calledElement(CALLED_PROCESS_KEY).endEvent().done();
		}


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		processEngineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_START).setHistoryRemovalTimeProvider(new DefaultHistoryRemovalTimeProvider()).initHistoryRemovalTime();
	  }

	  protected internal readonly string CALLED_PROCESS_KEY = "calledProcess";
	  protected internal BpmnModelInstance CALLED_PROCESS;

	  protected internal readonly string CALLING_PROCESS_KEY = "callingProcess";
	  protected internal BpmnModelInstance CALLING_PROCESS;

	  protected internal readonly DateTime START_DATE = new DateTime(1363608000000L);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldResolveHistoricDecisionInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldResolveHistoricDecisionInstance()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(Bpmn.createExecutableProcess(CALLING_PROCESS_KEY).camundaHistoryTimeToLive(5).startEvent().businessRuleTask().camundaDecisionRef("dish-decision").endEvent().done());

		// when
		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY, Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend"));

		IList<HistoricDecisionInstance> historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().list();

		// assume
		assertThat(historicDecisionInstances.Count, @is(3));

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(historicDecisionInstances[0].RemovalTime, @is(removalTime));
		assertThat(historicDecisionInstances[1].RemovalTime, @is(removalTime));
		assertThat(historicDecisionInstances[2].RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldResolveStandaloneHistoricDecisionInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldResolveStandaloneHistoricDecisionInstance()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;
		DecisionDefinition decisionDefinition = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey("dish-decision").singleResult();
		repositoryService.updateDecisionDefinitionHistoryTimeToLive(decisionDefinition.Id, 5);

		// when
		decisionService.evaluateDecisionTableByKey("dish-decision", Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend"));

		IList<HistoricDecisionInstance> historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().list();

		// assume
		assertThat(historicDecisionInstances.Count, @is(3));

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(historicDecisionInstances[0].RemovalTime, @is(removalTime));
		assertThat(historicDecisionInstances[1].RemovalTime, @is(removalTime));
		assertThat(historicDecisionInstances[2].RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldResolveHistoricDecisionInputInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldResolveHistoricDecisionInputInstance()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(Bpmn.createExecutableProcess(CALLING_PROCESS_KEY).camundaHistoryTimeToLive(5).startEvent().businessRuleTask().camundaDecisionRef("dish-decision").endEvent().done());

		// when
		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY, Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend"));

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().includeInputs().singleResult();

		// assume
		assertThat(historicDecisionInstance, notNullValue());

		IList<HistoricDecisionInputInstance> historicDecisionInputInstances = historicDecisionInstance.Inputs;

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(historicDecisionInputInstances[0].RemovalTime, @is(removalTime));
		assertThat(historicDecisionInputInstances[1].RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldResolveStandaloneHistoricDecisionInputInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldResolveStandaloneHistoricDecisionInputInstance()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;
		DecisionDefinition decisionDefinition = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey("dish-decision").singleResult();
		repositoryService.updateDecisionDefinitionHistoryTimeToLive(decisionDefinition.Id, 5);

		// when
		decisionService.evaluateDecisionTableByKey("dish-decision", Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend"));

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().includeInputs().singleResult();

		// assume
		assertThat(historicDecisionInstance, notNullValue());

		IList<HistoricDecisionInputInstance> historicDecisionInputInstances = historicDecisionInstance.Inputs;

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(historicDecisionInputInstances[0].RemovalTime, @is(removalTime));
		assertThat(historicDecisionInputInstances[1].RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldNotResolveHistoricDecisionInputInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldNotResolveHistoricDecisionInputInstance()
	  {
		// given

		// when
		decisionService.evaluateDecisionTableByKey("dish-decision", Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend"));

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().includeInputs().singleResult();

		// assume
		assertThat(historicDecisionInstance, notNullValue());

		IList<HistoricDecisionInputInstance> historicDecisionInputInstances = historicDecisionInstance.Inputs;

		// then
		assertThat(historicDecisionInputInstances[0].RemovalTime, nullValue());
		assertThat(historicDecisionInputInstances[1].RemovalTime, nullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldResolveHistoricDecisionOutputInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldResolveHistoricDecisionOutputInstance()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(Bpmn.createExecutableProcess(CALLING_PROCESS_KEY).camundaHistoryTimeToLive(5).startEvent().businessRuleTask().camundaDecisionRef("dish-decision").endEvent().done());

		// when
		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY, Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend"));

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().includeOutputs().singleResult();

		// assume
		assertThat(historicDecisionInstance, notNullValue());

		IList<HistoricDecisionOutputInstance> historicDecisionOutputInstances = historicDecisionInstance.Outputs;

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(historicDecisionOutputInstances[0].RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldResolveStandaloneHistoricDecisionOutputInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldResolveStandaloneHistoricDecisionOutputInstance()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(Bpmn.createExecutableProcess(CALLING_PROCESS_KEY).camundaHistoryTimeToLive(5).startEvent().businessRuleTask().camundaDecisionRef("dish-decision").endEvent().done());

		// when
		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY, Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend"));

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().includeOutputs().singleResult();

		// assume
		assertThat(historicDecisionInstance, notNullValue());

		IList<HistoricDecisionOutputInstance> historicDecisionOutputInstances = historicDecisionInstance.Outputs;

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(historicDecisionOutputInstances[0].RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldNotResolveHistoricDecisionOutputInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldNotResolveHistoricDecisionOutputInstance()
	  {
		// given

		// when
		decisionService.evaluateDecisionTableByKey("dish-decision", Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend"));

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().includeOutputs().singleResult();

		// assume
		assertThat(historicDecisionInstance, notNullValue());

		IList<HistoricDecisionOutputInstance> historicDecisionOutputInstances = historicDecisionInstance.Outputs;

		// then
		assertThat(historicDecisionOutputInstances[0].RemovalTime, nullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveHistoricProcessInstance()
	  public virtual void shouldResolveHistoricProcessInstance()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		// when
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().activeActivityIdIn("userTask").singleResult();

		// assume
		assertThat(historicProcessInstance, notNullValue());

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(historicProcessInstance.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveHistoricActivityInstance()
	  public virtual void shouldResolveHistoricActivityInstance()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		// when
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		HistoricActivityInstance historicActivityInstance = historyService.createHistoricActivityInstanceQuery().activityId("userTask").singleResult();

		// assume
		assertThat(historicActivityInstance, notNullValue());

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(historicActivityInstance.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveHistoricTaskInstance()
	  public virtual void shouldResolveHistoricTaskInstance()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		// when
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		HistoricTaskInstance historicTaskInstance = historyService.createHistoricTaskInstanceQuery().taskName("userTask").singleResult();

		// assume
		assertThat(historicTaskInstance, notNullValue());

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(historicTaskInstance.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotResolveHistoricTaskInstance()
	  public virtual void shouldNotResolveHistoricTaskInstance()
	  {
		// given
		Task task = taskService.newTask();

		// when
		taskService.saveTask(task);

		HistoricTaskInstance historicTaskInstance = historyService.createHistoricTaskInstanceQuery().singleResult();

		// assume
		assertThat(historicTaskInstance, notNullValue());

		// then
		assertThat(historicTaskInstance.RemovalTime, nullValue());

		// cleanup
		taskService.deleteTask(task.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveVariableInstance()
	  public virtual void shouldResolveVariableInstance()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		ClockUtil.CurrentTime = START_DATE;

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY, Variables.createVariables().putValue("aVariableName", Variables.stringValue("aVariableValue")));

		// when
		runtimeService.setVariable(processInstance.Id, "aVariableName", Variables.stringValue("anotherVariableValue"));

		HistoricVariableInstance historicVariableInstance = historyService.createHistoricVariableInstanceQuery().singleResult();

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(historicVariableInstance.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveHistoricDetailByVariableInstanceUpdate()
	  public virtual void shouldResolveHistoricDetailByVariableInstanceUpdate()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY, Variables.createVariables().putValue("aVariableName", Variables.stringValue("aVariableValue")));

		// when
		runtimeService.setVariable(processInstance.Id, "aVariableName", Variables.stringValue("anotherVariableValue"));

		IList<HistoricDetail> historicDetails = historyService.createHistoricDetailQuery().variableUpdates().list();

		// assume
		assertThat(historicDetails.Count, @is(2));

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(historicDetails[0].RemovalTime, @is(removalTime));
		assertThat(historicDetails[1].RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveHistoricDetailByFormProperty()
	  public virtual void shouldResolveHistoricDetailByFormProperty()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		DeploymentWithDefinitions deployment = testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		string processDefinitionId = deployment.DeployedProcessDefinitions[0].Id;
		IDictionary<string, object> properties = new Dictionary<string, object>();
		properties["aFormProperty"] = "aFormPropertyValue";

		// when
		formService.submitStartForm(processDefinitionId, properties);

		HistoricDetail historicDetail = historyService.createHistoricDetailQuery().formFields().singleResult();

		// assume
		assertThat(historicDetail, notNullValue());

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(historicDetail.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveIncident()
	  public virtual void shouldResolveIncident()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);
		taskService.complete(taskService.createTaskQuery().singleResult().Id);

		string jobId = managementService.createJobQuery().singleResult().Id;

		managementService.setJobRetries(jobId, 0);

		try
		{
		  // when
		  managementService.executeJob(jobId);
		}
		catch (Exception)
		{
		}

		IList<HistoricIncident> historicIncidents = historyService.createHistoricIncidentQuery().list();

		// assume
		assertThat(historicIncidents.Count, @is(2));

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(historicIncidents[0].RemovalTime, @is(removalTime));
		assertThat(historicIncidents[1].RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotResolveStandaloneIncident()
	  public virtual void shouldNotResolveStandaloneIncident()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(CALLED_PROCESS);

		repositoryService.suspendProcessDefinitionByKey(CALLED_PROCESS_KEY, true, new DateTime(1363608000000L));

		string jobId = managementService.createJobQuery().singleResult().Id;

		managementService.setJobRetries(jobId, 0);

		try
		{
		  // when
		  managementService.executeJob(jobId);
		}
		catch (Exception)
		{
		}

		HistoricIncident historicIncident = historyService.createHistoricIncidentQuery().singleResult();

		// assume
		assertThat(historicIncident, notNullValue());

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(historicIncident.RemovalTime, nullValue());

		// cleanup
		clearJobLog(jobId);
		clearHistoricIncident(historicIncident);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveExternalTaskLog()
	  public virtual void shouldResolveExternalTaskLog()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(Bpmn.createExecutableProcess("calledProcess").startEvent().serviceTask().camundaExternalTask("anExternalTaskTopic").endEvent().done());

		testRule.deploy(Bpmn.createExecutableProcess("callingProcess").camundaHistoryTimeToLive(5).startEvent().callActivity().calledElement("calledProcess").endEvent().done());

		// when
		runtimeService.startProcessInstanceByKey("callingProcess");

		HistoricExternalTaskLog externalTaskLog = historyService.createHistoricExternalTaskLogQuery().singleResult();

		// assume
		assertThat(externalTaskLog, notNullValue());

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(externalTaskLog.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveJobLog()
	  public virtual void shouldResolveJobLog()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);
		taskService.complete(taskService.createTaskQuery().singleResult().Id);

		string jobId = managementService.createJobQuery().singleResult().Id;

		try
		{
		  // when
		  managementService.executeJob(jobId);
		}
		catch (Exception)
		{
		}

		IList<HistoricJobLog> jobLog = historyService.createHistoricJobLogQuery().list();

		// assume
		assertThat(jobLog.Count, @is(2));

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(jobLog[0].RemovalTime, @is(removalTime));
		assertThat(jobLog[1].RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotResolveJobLog()
	  public virtual void shouldNotResolveJobLog()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(CALLED_PROCESS);

		repositoryService.suspendProcessDefinitionByKey(CALLED_PROCESS_KEY, true, new DateTime(1363608000000L));

		// when
		HistoricJobLog jobLog = historyService.createHistoricJobLogQuery().singleResult();

		// assume
		assertThat(jobLog, notNullValue());

		// then
		assertThat(jobLog.RemovalTime, nullValue());

		// cleanup
		managementService.deleteJob(jobLog.JobId);
		clearJobLog(jobLog.JobId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveUserOperationLog_SetJobRetries()
	  public virtual void shouldResolveUserOperationLog_SetJobRetries()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);
		taskService.complete(taskService.createTaskQuery().singleResult().Id);

		string jobId = managementService.createJobQuery().singleResult().Id;

		// when
		identityService.AuthenticatedUserId = "aUserId";
		managementService.setJobRetries(jobId, 65);
		identityService.clearAuthentication();

		UserOperationLogEntry userOperationLog = historyService.createUserOperationLogQuery().singleResult();

		// assume
		assertThat(userOperationLog, notNullValue());

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(userOperationLog.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveUserOperationLog_SetExternalTaskRetries()
	  public virtual void shouldResolveUserOperationLog_SetExternalTaskRetries()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(Bpmn.createExecutableProcess("calledProcess").startEvent().serviceTask().camundaExternalTask("anExternalTaskTopic").endEvent().done());

		testRule.deploy(Bpmn.createExecutableProcess("callingProcess").camundaHistoryTimeToLive(5).startEvent().callActivity().calledElement("calledProcess").endEvent().done());

		runtimeService.startProcessInstanceByKey("callingProcess");

		// when
		identityService.AuthenticatedUserId = "aUserId";
		externalTaskService.setRetries(externalTaskService.createExternalTaskQuery().singleResult().Id, 65);
		identityService.clearAuthentication();

		UserOperationLogEntry userOperationLog = historyService.createUserOperationLogQuery().singleResult();

		// assume
		assertThat(userOperationLog, notNullValue());

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(userOperationLog.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveUserOperationLog_ClaimTask()
	  public virtual void shouldResolveUserOperationLog_ClaimTask()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		// when
		identityService.AuthenticatedUserId = "aUserId";
		taskService.claim(taskService.createTaskQuery().singleResult().Id, "aUserId");
		identityService.clearAuthentication();

		UserOperationLogEntry userOperationLog = historyService.createUserOperationLogQuery().singleResult();

		// assume
		assertThat(userOperationLog, notNullValue());

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(userOperationLog.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveUserOperationLog_CreateAttachment()
	  public virtual void shouldResolveUserOperationLog_CreateAttachment()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		// when
		identityService.AuthenticatedUserId = "aUserId";
		taskService.createAttachment(null, null, runtimeService.createProcessInstanceQuery().activityIdIn("userTask").singleResult().Id, null, null, "http://camunda.com");
		identityService.clearAuthentication();

		UserOperationLogEntry userOperationLog = historyService.createUserOperationLogQuery().singleResult();

		// assume
		assertThat(userOperationLog, notNullValue());

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(userOperationLog.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveIdentityLink_AddCandidateUser()
	  public virtual void shouldResolveIdentityLink_AddCandidateUser()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		// when
		taskService.addCandidateUser(taskService.createTaskQuery().singleResult().Id, "aUserId");

		HistoricIdentityLinkLog historicIdentityLinkLog = historyService.createHistoricIdentityLinkLogQuery().singleResult();

		// assume
		assertThat(historicIdentityLinkLog, notNullValue());

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(historicIdentityLinkLog.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotResolveIdentityLink_AddCandidateUser()
	  public virtual void shouldNotResolveIdentityLink_AddCandidateUser()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		Task aTask = taskService.newTask();
		taskService.saveTask(aTask);

		// when
		taskService.addCandidateUser(aTask.Id, "aUserId");

		HistoricIdentityLinkLog historicIdentityLinkLog = historyService.createHistoricIdentityLinkLogQuery().singleResult();

		// assume
		assertThat(historicIdentityLinkLog, notNullValue());

		// then
		assertThat(historicIdentityLinkLog.RemovalTime, nullValue());

		// cleanup
		taskService.complete(aTask.Id);
		clearHistoricTaskInst(aTask.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveCommentByProcessInstanceId()
	  public virtual void shouldResolveCommentByProcessInstanceId()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string processInstanceId = runtimeService.createProcessInstanceQuery().activityIdIn("userTask").singleResult().Id;

		// when
		taskService.createComment(null, processInstanceId, "aMessage");

		Comment comment = taskService.getProcessInstanceComments(processInstanceId)[0];

		// assume
		assertThat(comment, notNullValue());

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(comment.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveCommentByTaskId()
	  public virtual void shouldResolveCommentByTaskId()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.createComment(taskId, null, "aMessage");

		Comment comment = taskService.getTaskComments(taskId)[0];

		// assume
		assertThat(comment, notNullValue());

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(comment.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotResolveCommentByWrongTaskIdAndProcessInstanceId()
	  public virtual void shouldNotResolveCommentByWrongTaskIdAndProcessInstanceId()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string processInstanceId = runtimeService.createProcessInstanceQuery().activityIdIn("userTask").singleResult().Id;

		// when
		taskService.createComment("aNonExistentTaskId", processInstanceId, "aMessage");

		Comment comment = taskService.getProcessInstanceComments(processInstanceId)[0];

		// assume
		assertThat(comment, notNullValue());

		// then
		assertThat(comment.RemovalTime, nullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveCommentByTaskIdAndWrongProcessInstanceId()
	  public virtual void shouldResolveCommentByTaskIdAndWrongProcessInstanceId()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.createComment(taskId, "aNonExistentProcessInstanceId", "aMessage");

		Comment comment = taskService.getTaskComments(taskId)[0];

		// assume
		assertThat(comment, notNullValue());

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(comment.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotResolveCommentByWrongProcessInstanceId()
	  public virtual void shouldNotResolveCommentByWrongProcessInstanceId()
	  {
		// given

		// when
		taskService.createComment(null, "aNonExistentProcessInstanceId", "aMessage");

		Comment comment = taskService.getProcessInstanceComments("aNonExistentProcessInstanceId")[0];

		// assume
		assertThat(comment, notNullValue());

		// then
		assertThat(comment.RemovalTime, nullValue());

		// cleanup
		clearCommentByProcessInstanceId("aNonExistentProcessInstanceId");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotResolveCommentByWrongTaskId()
	  public virtual void shouldNotResolveCommentByWrongTaskId()
	  {
		// given

		// when
		taskService.createComment("aNonExistentTaskId", null, "aMessage");

		Comment comment = taskService.getTaskComments("aNonExistentTaskId")[0];

		// assume
		assertThat(comment, notNullValue());

		// then
		assertThat(comment.RemovalTime, nullValue());

		// cleanup
		clearCommentByTaskId("aNonExistentTaskId");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveAttachmentByProcessInstanceId()
	  public virtual void shouldResolveAttachmentByProcessInstanceId()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string processInstanceId = runtimeService.createProcessInstanceQuery().activityIdIn("userTask").singleResult().Id;

		// when
		string attachmentId = taskService.createAttachment(null, null, processInstanceId, null, null, "http://camunda.com").Id;

		Attachment attachment = taskService.getAttachment(attachmentId);

		// assume
		assertThat(attachment, notNullValue());

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(attachment.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveAttachmentByTaskId()
	  public virtual void shouldResolveAttachmentByTaskId()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		string attachmentId = taskService.createAttachment(null, taskId, null, null, null, "http://camunda.com").Id;

		Attachment attachment = taskService.getAttachment(attachmentId);

		// assume
		assertThat(attachment, notNullValue());

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(attachment.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotResolveAttachmentByWrongTaskIdAndProcessInstanceId()
	  public virtual void shouldNotResolveAttachmentByWrongTaskIdAndProcessInstanceId()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string processInstanceId = runtimeService.createProcessInstanceQuery().activityIdIn("userTask").singleResult().Id;

		// when
		string attachmentId = taskService.createAttachment(null, "aWrongTaskId", processInstanceId, null, null, "http://camunda.com").Id;

		Attachment attachment = taskService.getAttachment(attachmentId);

		// assume
		assertThat(attachment, notNullValue());

		// then
		assertThat(attachment.RemovalTime, nullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveAttachmentByTaskIdAndWrongProcessInstanceId()
	  public virtual void shouldResolveAttachmentByTaskIdAndWrongProcessInstanceId()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		string attachmentId = taskService.createAttachment(null, taskId, "aWrongProcessInstanceId", null, null, "http://camunda.com").Id;

		Attachment attachment = taskService.getAttachment(attachmentId);

		// assume
		assertThat(attachment, notNullValue());

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(attachment.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotResolveAttachmentByWrongTaskId()
	  public virtual void shouldNotResolveAttachmentByWrongTaskId()
	  {
		// given

		// when
		string attachmentId = taskService.createAttachment(null, "aWrongTaskId", null, null, null, "http://camunda.com").Id;

		Attachment attachment = taskService.getAttachment(attachmentId);

		// assume
		assertThat(attachment, notNullValue());

		// then
		assertThat(attachment.RemovalTime, nullValue());

		// cleanup
		clearAttachment(attachment);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveByteArray_CreateAttachmentByTask()
	  public virtual void shouldResolveByteArray_CreateAttachmentByTask()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		AttachmentEntity attachment = (AttachmentEntity) taskService.createAttachment(null, taskId, null, null, null, new MemoryStream("hello world".GetBytes()));

		ByteArrayEntity byteArray = findByteArrayById(attachment.ContentId);

		// assume
		assertThat(byteArray, notNullValue());

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(byteArray.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveByteArray_CreateAttachmentByProcessInstance()
	  public virtual void shouldResolveByteArray_CreateAttachmentByProcessInstance()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string calledProcessInstanceId = runtimeService.createProcessInstanceQuery().activityIdIn("userTask").singleResult().Id;

		// when
		AttachmentEntity attachment = (AttachmentEntity) taskService.createAttachment(null, null, calledProcessInstanceId, null, null, new MemoryStream("hello world".GetBytes()));

		ByteArrayEntity byteArray = findByteArrayById(attachment.ContentId);

		// assume
		assertThat(byteArray, notNullValue());

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(byteArray.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveByteArray_SetVariable()
	  public virtual void shouldResolveByteArray_SetVariable()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		// when
		runtimeService.setVariable(processInstance.Id, "aVariableName", new MemoryStream("hello world".GetBytes()));

		HistoricVariableInstanceEntity historicVariableInstance = (HistoricVariableInstanceEntity) historyService.createHistoricVariableInstanceQuery().singleResult();

		ByteArrayEntity byteArray = findByteArrayById(historicVariableInstance.ByteArrayId);

		// assume
		assertThat(byteArray, notNullValue());

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(byteArray.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveByteArray_UpdateVariable()
	  public virtual void shouldResolveByteArray_UpdateVariable()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY, Variables.createVariables().putValue("aVariableName", Variables.stringValue("aVariableValue")));

		// when
		runtimeService.setVariable(processInstance.Id, "aVariableName", new MemoryStream("hello world".GetBytes()));

		HistoricDetailVariableInstanceUpdateEntity historicDetails = (HistoricDetailVariableInstanceUpdateEntity) historyService.createHistoricDetailQuery().variableUpdates().variableTypeIn("Bytes").singleResult();

		// assume
		ByteArrayEntity byteArray = findByteArrayById(historicDetails.ByteArrayValueId);

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(byteArray.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveByteArray_JobLog()
	  public virtual void shouldResolveByteArray_JobLog()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		taskService.complete(taskService.createTaskQuery().singleResult().Id);

		string jobId = managementService.createJobQuery().singleResult().Id;

		try
		{
		  // when
		  managementService.executeJob(jobId);
		}
		catch (Exception)
		{
		}

		HistoricJobLogEventEntity jobLog = (HistoricJobLogEventEntity) historyService.createHistoricJobLogQuery().jobExceptionMessage("I'm supposed to fail!").singleResult();

		// assume
		assertThat(jobLog, notNullValue());

		ByteArrayEntity byteArray = findByteArrayById(jobLog.ExceptionByteArrayId);

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(byteArray.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveByteArray_ExternalTaskLog()
	  public virtual void shouldResolveByteArray_ExternalTaskLog()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(Bpmn.createExecutableProcess("calledProcess").startEvent().serviceTask().camundaExternalTask("aTopicName").endEvent().done());

		testRule.deploy(Bpmn.createExecutableProcess("callingProcess").camundaHistoryTimeToLive(5).startEvent().callActivity().calledElement("calledProcess").endEvent().done());

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("callingProcess");

		IList<LockedExternalTask> tasks = externalTaskService.fetchAndLock(5, "aWorkerId").topic("aTopicName", int.MaxValue).execute();

		// when
		externalTaskService.handleFailure(tasks[0].Id, "aWorkerId", null, "errorDetails", 5, 3000L);

		HistoricExternalTaskLogEntity externalTaskLog = (HistoricExternalTaskLogEntity) historyService.createHistoricExternalTaskLogQuery().failureLog().singleResult();

		// assume
		assertThat(externalTaskLog, notNullValue());

		ByteArrayEntity byteArrayEntity = findByteArrayById(externalTaskLog.ErrorDetailsByteArrayId);

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(byteArrayEntity.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml" }) public void shouldResolveByteArray_DecisionInput()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml" })]
	  public virtual void shouldResolveByteArray_DecisionInput()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(Bpmn.createExecutableProcess(CALLING_PROCESS_KEY).camundaHistoryTimeToLive(5).startEvent().businessRuleTask().camundaDecisionRef("testDecision").endEvent().done());

		// when
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY, Variables.createVariables().putValue("pojo", new TestPojo("okay", 13.37)));

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().includeInputs().singleResult();

		// assume
		assertThat(historicDecisionInstance, notNullValue());

		HistoricDecisionInputInstanceEntity historicDecisionInputInstanceEntity = (HistoricDecisionInputInstanceEntity) historicDecisionInstance.Inputs[0];

		ByteArrayEntity byteArrayEntity = findByteArrayById(historicDecisionInputInstanceEntity.ByteArrayValueId);

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(byteArrayEntity.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml" }) public void shouldResolveByteArray_StandaloneDecisionInput()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml" })]
	  public virtual void shouldResolveByteArray_StandaloneDecisionInput()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;
		DecisionDefinition decisionDefinition = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey("testDecision").singleResult();
		repositoryService.updateDecisionDefinitionHistoryTimeToLive(decisionDefinition.Id, 5);

		// when
		decisionService.evaluateDecisionTableByKey("testDecision", Variables.createVariables().putValue("pojo", new TestPojo("okay", 13.37)));

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().includeInputs().singleResult();

		// assume
		assertThat(historicDecisionInstance, notNullValue());

		HistoricDecisionInputInstanceEntity historicDecisionInputInstanceEntity = (HistoricDecisionInputInstanceEntity) historicDecisionInstance.Inputs[0];

		ByteArrayEntity byteArrayEntity = findByteArrayById(historicDecisionInputInstanceEntity.ByteArrayValueId);

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(byteArrayEntity.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml" }) public void shouldResolveByteArray_DecisionOutput()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml" })]
	  public virtual void shouldResolveByteArray_DecisionOutput()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(Bpmn.createExecutableProcess(CALLING_PROCESS_KEY).camundaHistoryTimeToLive(5).startEvent().businessRuleTask().camundaDecisionRef("testDecision").endEvent().done());

		// when
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY, Variables.createVariables().putValue("pojo", new TestPojo("okay", 13.37)));

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().includeOutputs().singleResult();

		// assume
		assertThat(historicDecisionInstance, notNullValue());

		HistoricDecisionOutputInstanceEntity historicDecisionOutputInstanceEntity = (HistoricDecisionOutputInstanceEntity) historicDecisionInstance.Outputs[0];

		ByteArrayEntity byteArrayEntity = findByteArrayById(historicDecisionOutputInstanceEntity.ByteArrayValueId);

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(byteArrayEntity.RemovalTime, @is(removalTime));
	  }
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml" }) public void shouldResolveByteArray_StandaloneDecisionOutput()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml" })]
	  public virtual void shouldResolveByteArray_StandaloneDecisionOutput()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;
		DecisionDefinition decisionDefinition = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey("testDecision").singleResult();
		repositoryService.updateDecisionDefinitionHistoryTimeToLive(decisionDefinition.Id, 5);

		// when
		decisionService.evaluateDecisionTableByKey("testDecision", Variables.createVariables().putValue("pojo", new TestPojo("okay", 13.37)));

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().includeOutputs().singleResult();

		// assume
		assertThat(historicDecisionInstance, notNullValue());

		HistoricDecisionOutputInstanceEntity historicDecisionOutputInstanceEntity = (HistoricDecisionOutputInstanceEntity) historicDecisionInstance.Outputs[0];

		ByteArrayEntity byteArrayEntity = findByteArrayById(historicDecisionOutputInstanceEntity.ByteArrayValueId);

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(byteArrayEntity.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/history/removaltime/HistoricRootProcessInstanceTest.shouldResolveByteArray_DecisionOutputLiteralExpression.dmn" }) public void shouldResolveByteArray_DecisionOutputLiteralExpression()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/history/removaltime/HistoricRootProcessInstanceTest.shouldResolveByteArray_DecisionOutputLiteralExpression.dmn" })]
	  public virtual void shouldResolveByteArray_DecisionOutputLiteralExpression()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;

		testRule.deploy(Bpmn.createExecutableProcess(CALLING_PROCESS_KEY).camundaHistoryTimeToLive(5).startEvent().businessRuleTask().camundaDecisionRef("testDecision").endEvent().done());

		// when
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY, Variables.createVariables().putValue("pojo", new TestPojo("okay", 13.37)));

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().includeOutputs().singleResult();

		// assume
		assertThat(historicDecisionInstance, notNullValue());

		HistoricDecisionOutputInstanceEntity historicDecisionOutputInstanceEntity = (HistoricDecisionOutputInstanceEntity) historicDecisionInstance.Outputs[0];

		ByteArrayEntity byteArrayEntity = findByteArrayById(historicDecisionOutputInstanceEntity.ByteArrayValueId);

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(byteArrayEntity.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/history/removaltime/HistoricRootProcessInstanceTest.shouldResolveByteArray_DecisionOutputLiteralExpression.dmn" }) public void shouldResolveByteArray_StandaloneDecisionOutputLiteralExpression()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/history/removaltime/HistoricRootProcessInstanceTest.shouldResolveByteArray_DecisionOutputLiteralExpression.dmn" })]
	  public virtual void shouldResolveByteArray_StandaloneDecisionOutputLiteralExpression()
	  {
		// given
		ClockUtil.CurrentTime = START_DATE;
		DecisionDefinition decisionDefinition = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey("testDecision").singleResult();
		repositoryService.updateDecisionDefinitionHistoryTimeToLive(decisionDefinition.Id, 5);

		// when
		decisionService.evaluateDecisionTableByKey("testDecision", Variables.createVariables().putValue("pojo", new TestPojo("okay", 13.37)));

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().includeOutputs().singleResult();

		// assume
		assertThat(historicDecisionInstance, notNullValue());

		HistoricDecisionOutputInstanceEntity historicDecisionOutputInstanceEntity = (HistoricDecisionOutputInstanceEntity) historicDecisionInstance.Outputs[0];

		ByteArrayEntity byteArrayEntity = findByteArrayById(historicDecisionOutputInstanceEntity.ByteArrayValueId);

		DateTime removalTime = addDays(START_DATE, 5);

		// then
		assertThat(byteArrayEntity.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveBatch()
	  public virtual void shouldResolveBatch()
	  {
		// given
		processEngineConfiguration.BatchOperationHistoryTimeToLive = "P5D";
		processEngineConfiguration.initHistoryCleanup();

		testRule.deploy(CALLED_PROCESS);

		testRule.deploy(CALLING_PROCESS);

		string processInstanceId = runtimeService.startProcessInstanceByKey(CALLED_PROCESS_KEY).Id;

		ClockUtil.CurrentTime = START_DATE;

		// when batch is started
		Batch batch = runtimeService.deleteProcessInstancesAsync(Collections.singletonList(processInstanceId), "aDeleteReason");

		HistoricBatch historicBatch = historyService.createHistoricBatchQuery().singleResult();

		// then removal time is set
		assertThat(historicBatch.RemovalTime, @is(addDays(START_DATE, 5)));

		string seedJobId = managementService.createJobQuery().singleResult().Id;
		managementService.executeJob(seedJobId);

		string jobId = managementService.createJobQuery().list().get(0).Id;
		managementService.executeJob(jobId);

		string monitorJobId = managementService.createJobQuery().singleResult().Id;

		// when batch is ended
		managementService.executeJob(monitorJobId);

		historicBatch = historyService.createHistoricBatchQuery().singleResult();

		// then removal time is still set
		assertThat(historicBatch.RemovalTime, @is(addDays(START_DATE, 5)));

		// cleanup
		historyService.deleteHistoricBatch(batch.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveBatchJobLog()
	  public virtual void shouldResolveBatchJobLog()
	  {
		// given
		processEngineConfiguration.BatchOperationHistoryTimeToLive = "P5D";
		processEngineConfiguration.initHistoryCleanup();

		testRule.deploy(CALLED_PROCESS);

		testRule.deploy(CALLING_PROCESS);

		string processInstanceId = runtimeService.startProcessInstanceByKey(CALLED_PROCESS_KEY).Id;

		ClockUtil.CurrentTime = START_DATE;

		Batch batch = runtimeService.deleteProcessInstancesAsync(Collections.singletonList(processInstanceId), "aDeleteReason");

		HistoricJobLog jobLog = historyService.createHistoricJobLogQuery().singleResult();

		// assume
		assertThat(jobLog.RemovalTime, @is(addDays(START_DATE, 5)));

		// when
		managementService.executeJob(jobLog.JobId);

		IList<HistoricJobLog> jobLogs = historyService.createHistoricJobLogQuery().list();

		// then
		assertThat(jobLogs[0].RemovalTime, @is(addDays(START_DATE, 5)));
		assertThat(jobLogs[1].RemovalTime, @is(addDays(START_DATE, 5)));

		// cleanup
		managementService.deleteBatch(batch.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveBatchJobLog_ByteArray()
	  public virtual void shouldResolveBatchJobLog_ByteArray()
	  {
		// given
		processEngineConfiguration.BatchOperationHistoryTimeToLive = "P5D";
		processEngineConfiguration.initHistoryCleanup();

		FailingExecutionListener.shouldFail = true;

		testRule.deploy(Bpmn.createExecutableProcess("process").startEvent().userTask().camundaExecutionListenerClass("end", typeof(FailingExecutionListener)).endEvent().done());

		ClockUtil.CurrentTime = START_DATE;

		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;
		Batch batch = runtimeService.deleteProcessInstancesAsync(Collections.singletonList(processInstanceId), "aDeleteReason");

		string jobId = managementService.createJobQuery().singleResult().Id;

		managementService.executeJob(jobId);

		IList<Job> jobs = managementService.createJobQuery().list();
		foreach (Job job in jobs)
		{
		  try
		  {
			managementService.executeJob(job.Id);
		  }
		  catch (Exception)
		  {
		  }
		}

		HistoricJobLogEventEntity jobLog = (HistoricJobLogEventEntity)historyService.createHistoricJobLogQuery().failureLog().singleResult();

		string byteArrayId = jobLog.ExceptionByteArrayId;

		ByteArrayEntity byteArray = findByteArrayById(byteArrayId);

		// then
		assertThat(byteArray.RemovalTime, @is(addDays(START_DATE, 5)));

		// cleanup
		managementService.deleteBatch(batch.Id, true);
		FailingExecutionListener.shouldFail = false;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveBatchIncident_SeedJob()
	  public virtual void shouldResolveBatchIncident_SeedJob()
	  {
		// given
		processEngineConfiguration.BatchOperationHistoryTimeToLive = "P5D";
		processEngineConfiguration.initHistoryCleanup();

		testRule.deploy(CALLED_PROCESS);
		testRule.deploy(CALLING_PROCESS);

		string processInstanceId = runtimeService.startProcessInstanceByKey(CALLED_PROCESS_KEY).Id;

		ClockUtil.CurrentTime = START_DATE;

		Batch batch = runtimeService.deleteProcessInstancesAsync(Collections.singletonList(processInstanceId), "aDeleteReason");

		HistoricJobLog jobLog = historyService.createHistoricJobLogQuery().singleResult();

		// assume
		assertThat(jobLog.RemovalTime, @is(addDays(START_DATE, 5)));

		// when
		managementService.setJobRetries(jobLog.JobId, 0);

		HistoricIncident historicIncident = historyService.createHistoricIncidentQuery().singleResult();

		// then
		assertThat(historicIncident.RemovalTime, @is(addDays(START_DATE, 5)));

		// cleanup
		managementService.deleteBatch(batch.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveBatchIncident_BatchJob()
	  public virtual void shouldResolveBatchIncident_BatchJob()
	  {
		// given
		processEngineConfiguration.BatchOperationHistoryTimeToLive = "P5D";
		processEngineConfiguration.initHistoryCleanup();

		testRule.deploy(CALLED_PROCESS);
		testRule.deploy(CALLING_PROCESS);

		string processInstanceId = runtimeService.startProcessInstanceByKey(CALLED_PROCESS_KEY).Id;

		ClockUtil.CurrentTime = START_DATE;

		Batch batch = runtimeService.deleteProcessInstancesAsync(Collections.singletonList(processInstanceId), "aDeleteReason");

		HistoricJobLog jobLog = historyService.createHistoricJobLogQuery().singleResult();

		// assume
		assertThat(jobLog.RemovalTime, @is(addDays(START_DATE, 5)));

		runtimeService.deleteProcessInstance(processInstanceId, "aDeleteReason");

		managementService.executeJob(jobLog.JobId);

		string jobId = managementService.createJobQuery().jobDefinitionId(batch.BatchJobDefinitionId).singleResult().Id;

		// when
		managementService.setJobRetries(jobId, 0);

		HistoricIncident historicIncident = historyService.createHistoricIncidentQuery().singleResult();

		// then
		assertThat(historicIncident.RemovalTime, @is(addDays(START_DATE, 5)));

		// cleanup
		managementService.deleteBatch(batch.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveBatchIncident_MonitorJob()
	  public virtual void shouldResolveBatchIncident_MonitorJob()
	  {
		// given
		processEngineConfiguration.BatchOperationHistoryTimeToLive = "P5D";
		processEngineConfiguration.initHistoryCleanup();

		testRule.deploy(CALLED_PROCESS);
		testRule.deploy(CALLING_PROCESS);

		string processInstanceId = runtimeService.startProcessInstanceByKey(CALLED_PROCESS_KEY).Id;

		ClockUtil.CurrentTime = START_DATE;

		Batch batch = runtimeService.deleteProcessInstancesAsync(Collections.singletonList(processInstanceId), "aDeleteReason");

		HistoricJobLog jobLog = historyService.createHistoricJobLogQuery().singleResult();

		// assume
		assertThat(jobLog.RemovalTime, @is(addDays(START_DATE, 5)));

		managementService.executeJob(jobLog.JobId);

		string jobId = managementService.createJobQuery().jobDefinitionId(batch.BatchJobDefinitionId).singleResult().Id;
		managementService.executeJob(jobId);

		jobId = managementService.createJobQuery().jobDefinitionId(batch.MonitorJobDefinitionId).singleResult().Id;

		// when
		managementService.setJobRetries(jobId, 0);

		HistoricIncident historicIncident = historyService.createHistoricIncidentQuery().singleResult();

		// then
		assertThat(historicIncident.RemovalTime, @is(addDays(START_DATE, 5)));

		// cleanup
		managementService.deleteBatch(batch.Id, true);
	  }

	}

}