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
	using DeploymentWithDefinitions = org.camunda.bpm.engine.repository.DeploymentWithDefinitions;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Attachment = org.camunda.bpm.engine.task.Attachment;
	using Comment = org.camunda.bpm.engine.task.Comment;
	using Task = org.camunda.bpm.engine.task.Task;
	using TestPojo = org.camunda.bpm.engine.test.dmn.businessruletask.TestPojo;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Test = org.junit.Test;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_REMOVAL_TIME_STRATEGY_END;
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
	public class RemovalTimeStrategyEndTest : AbstractRemovalTimeTest
	{
		private bool InstanceFieldsInitialized = false;

		public RemovalTimeStrategyEndTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			CALLED_PROCESS = Bpmn.createExecutableProcess(CALLED_PROCESS_KEY).startEvent().userTask("userTask").name("userTask").endEvent().done();
			CALLING_PROCESS = Bpmn.createExecutableProcess(CALLING_PROCESS_KEY).camundaHistoryTimeToLive(5).startEvent().callActivity().calledElement(CALLED_PROCESS_KEY).endEvent().done();
		}


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		processEngineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_END).setHistoryRemovalTimeProvider(new DefaultHistoryRemovalTimeProvider()).initHistoryRemovalTime();
	  }

	  protected internal readonly string CALLED_PROCESS_KEY = "calledProcess";
	  protected internal BpmnModelInstance CALLED_PROCESS;

	  protected internal readonly string CALLING_PROCESS_KEY = "callingProcess";
	  protected internal BpmnModelInstance CALLING_PROCESS;

	  protected internal readonly DateTime START_DATE = new DateTime(1363607000000L);
	  protected internal readonly DateTime END_DATE = new DateTime(1363608000000L);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldResolveHistoricDecisionInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldResolveHistoricDecisionInstance()
	  {
		// given
		testRule.deploy(Bpmn.createExecutableProcess("process").camundaHistoryTimeToLive(5).startEvent().businessRuleTask().camundaAsyncAfter().camundaDecisionRef("dish-decision").endEvent().done());

		ClockUtil.CurrentTime = START_DATE;

		runtimeService.startProcessInstanceByKey("process", Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend"));

		ClockUtil.CurrentTime = END_DATE;

		IList<HistoricDecisionInstance> historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().list();

		// assume
		assertThat(historicDecisionInstances[0].RemovalTime, nullValue());
		assertThat(historicDecisionInstances[1].RemovalTime, nullValue());
		assertThat(historicDecisionInstances[2].RemovalTime, nullValue());

		string jobId = managementService.createJobQuery().singleResult().Id;

		// when
		managementService.executeJob(jobId);

		historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().list();

		DateTime removalTime = addDays(END_DATE, 5);

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
		testRule.deploy(Bpmn.createExecutableProcess("process").camundaHistoryTimeToLive(5).startEvent().businessRuleTask().camundaAsyncAfter().camundaDecisionRef("dish-decision").endEvent().done());

		ClockUtil.CurrentTime = START_DATE;

		runtimeService.startProcessInstanceByKey("process", Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend"));

		ClockUtil.CurrentTime = END_DATE;

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().includeInputs().singleResult();

		IList<HistoricDecisionInputInstance> historicDecisionInputInstances = historicDecisionInstance.Inputs;

		// assume
		assertThat(historicDecisionInputInstances[0].RemovalTime, nullValue());
		assertThat(historicDecisionInputInstances[1].RemovalTime, nullValue());

		string jobId = managementService.createJobQuery().singleResult().Id;

		// when
		managementService.executeJob(jobId);

		historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().includeInputs().singleResult();

		historicDecisionInputInstances = historicDecisionInstance.Inputs;

		DateTime removalTime = addDays(END_DATE, 5);

		// then
		assertThat(historicDecisionInputInstances[0].RemovalTime, @is(removalTime));
		assertThat(historicDecisionInputInstances[1].RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldResolveHistoricDecisionOutputInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldResolveHistoricDecisionOutputInstance()
	  {
		// given
		testRule.deploy(Bpmn.createExecutableProcess("process").camundaHistoryTimeToLive(5).startEvent().businessRuleTask().camundaAsyncAfter().camundaDecisionRef("dish-decision").endEvent().done());

		ClockUtil.CurrentTime = START_DATE;

		runtimeService.startProcessInstanceByKey("process", Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend"));

		ClockUtil.CurrentTime = END_DATE;

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().includeOutputs().singleResult();

		IList<HistoricDecisionOutputInstance> historicDecisionOutputInstances = historicDecisionInstance.Outputs;

		// assume
		assertThat(historicDecisionOutputInstances[0].RemovalTime, nullValue());

		string jobId = managementService.createJobQuery().singleResult().Id;

		// when
		managementService.executeJob(jobId);

		historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().includeOutputs().singleResult();

		historicDecisionOutputInstances = historicDecisionInstance.Outputs;

		DateTime removalTime = addDays(END_DATE, 5);

		// then
		assertThat(historicDecisionOutputInstances[0].RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveHistoricProcessInstance()
	  public virtual void shouldResolveHistoricProcessInstance()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		ClockUtil.CurrentTime = START_DATE;

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().activeActivityIdIn("userTask").singleResult();

		// assume
		assertThat(historicProcessInstance.RemovalTime, nullValue());

		string taskId = historyService.createHistoricTaskInstanceQuery().singleResult().Id;

		ClockUtil.CurrentTime = END_DATE;

		// when
		taskService.complete(taskId);

		historicProcessInstance = historyService.createHistoricProcessInstanceQuery().processDefinitionKey(CALLED_PROCESS_KEY).singleResult();

		DateTime removalTime = addDays(END_DATE, 5);

		// then
		assertThat(historicProcessInstance.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveHistoricActivityInstance()
	  public virtual void shouldResolveHistoricActivityInstance()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		DeploymentWithDefinitions deployment = testRule.deploy(CALLED_PROCESS);

		ClockUtil.CurrentTime = START_DATE;

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string taskId = historyService.createHistoricTaskInstanceQuery().singleResult().Id;

		ClockUtil.CurrentTime = END_DATE;

		HistoricActivityInstance historicActivityInstance = historyService.createHistoricActivityInstanceQuery().activityId("userTask").singleResult();

		// assume
		assertThat(historicActivityInstance.RemovalTime, nullValue());

		// when
		taskService.complete(taskId);

		historicActivityInstance = historyService.createHistoricActivityInstanceQuery().activityId("userTask").singleResult();

		DateTime removalTime = addDays(END_DATE, 5);

		// then
		assertThat(historicActivityInstance.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveHistoricTaskInstance()
	  public virtual void shouldResolveHistoricTaskInstance()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		ClockUtil.CurrentTime = START_DATE;

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		HistoricTaskInstance historicTaskInstance = historyService.createHistoricTaskInstanceQuery().singleResult();

		// assume
		assertThat(historicTaskInstance.RemovalTime, nullValue());

		ClockUtil.CurrentTime = END_DATE;

		string taskId = historyService.createHistoricTaskInstanceQuery().singleResult().Id;

		// when
		taskService.complete(taskId);

		historicTaskInstance = historyService.createHistoricTaskInstanceQuery().singleResult();

		DateTime removalTime = addDays(END_DATE, 5);

		// then
		assertThat(historicTaskInstance.RemovalTime, @is(removalTime));
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

		runtimeService.setVariable(processInstance.Id, "aVariableName", Variables.stringValue("anotherVariableValue"));

		HistoricVariableInstance historicVariableInstance = historyService.createHistoricVariableInstanceQuery().singleResult();

		// assume
		assertThat(historicVariableInstance.RemovalTime, nullValue());

		ClockUtil.CurrentTime = END_DATE;

		string taskId = historyService.createHistoricTaskInstanceQuery().singleResult().Id;

		// when
		taskService.complete(taskId);

		historicVariableInstance = historyService.createHistoricVariableInstanceQuery().singleResult();

		DateTime removalTime = addDays(END_DATE, 5);

		// then
		assertThat(historicVariableInstance.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveHistoricDetailByVariableInstanceUpdate()
	  public virtual void shouldResolveHistoricDetailByVariableInstanceUpdate()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		ClockUtil.CurrentTime = START_DATE;

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY, Variables.createVariables().putValue("aVariableName", Variables.stringValue("aVariableValue")));

		runtimeService.setVariable(processInstance.Id, "aVariableName", Variables.stringValue("anotherVariableValue"));

		IList<HistoricDetail> historicDetails = historyService.createHistoricDetailQuery().variableUpdates().list();

		// assume
		assertThat(historicDetails[0].RemovalTime, nullValue());
		assertThat(historicDetails[1].RemovalTime, nullValue());

		ClockUtil.CurrentTime = END_DATE;

		string taskId = historyService.createHistoricTaskInstanceQuery().singleResult().Id;

		// when
		taskService.complete(taskId);

		historicDetails = historyService.createHistoricDetailQuery().variableUpdates().list();

		DateTime removalTime = addDays(END_DATE, 5);

		// then
		assertThat(historicDetails[0].RemovalTime, @is(removalTime));
		assertThat(historicDetails[1].RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveHistoricDetailByFormProperty()
	  public virtual void shouldResolveHistoricDetailByFormProperty()
	  {
		// given
		DeploymentWithDefinitions deployment = testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		string processDefinitionId = deployment.DeployedProcessDefinitions[0].Id;
		IDictionary<string, object> properties = new Dictionary<string, object>();
		properties["aFormProperty"] = "aFormPropertyValue";

		ClockUtil.CurrentTime = START_DATE;

		formService.submitStartForm(processDefinitionId, properties);

		HistoricDetail historicDetail = historyService.createHistoricDetailQuery().formFields().singleResult();

		// assume
		assertThat(historicDetail.RemovalTime, nullValue());

		ClockUtil.CurrentTime = END_DATE;

		string taskId = historyService.createHistoricTaskInstanceQuery().singleResult().Id;

		// when
		taskService.complete(taskId);

		historicDetail = historyService.createHistoricDetailQuery().formFields().singleResult();

		DateTime removalTime = addDays(END_DATE, 5);

		// then
		assertThat(historicDetail.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveIncident()
	  public virtual void shouldResolveIncident()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(Bpmn.createExecutableProcess(CALLED_PROCESS_KEY).startEvent().scriptTask().camundaAsyncBefore().scriptFormat("groovy").scriptText("if(execution.getIncidents().size() == 0) throw new RuntimeException()").userTask().endEvent().done());

		ClockUtil.CurrentTime = START_DATE;

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string jobId = managementService.createJobQuery().singleResult().Id;

		managementService.setJobRetries(jobId, 0);

		try
		{
		  managementService.executeJob(jobId);
		}
		catch (Exception)
		{
		}

		IList<HistoricIncident> historicIncidents = historyService.createHistoricIncidentQuery().list();

		// assume
		assertThat(historicIncidents[0].RemovalTime, nullValue());
		assertThat(historicIncidents[1].RemovalTime, nullValue());

		ClockUtil.CurrentTime = END_DATE;

		string taskId = historyService.createHistoricTaskInstanceQuery().singleResult().Id;

		// when
		taskService.complete(taskId);

		historicIncidents = historyService.createHistoricIncidentQuery().list();

		DateTime removalTime = addDays(END_DATE, 5);

		// then
		assertThat(historicIncidents[0].RemovalTime, @is(removalTime));
		assertThat(historicIncidents[1].RemovalTime, @is(removalTime));
	  }

	  /// <summary>
	  /// See https://app.camunda.com/jira/browse/CAM-9505
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveIncidentWithPreservedCreateTime()
	  public virtual void shouldResolveIncidentWithPreservedCreateTime()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(Bpmn.createExecutableProcess(CALLED_PROCESS_KEY).startEvent().scriptTask().camundaAsyncBefore().scriptFormat("groovy").scriptText("if(execution.getIncidents().size() == 0) throw new RuntimeException()").userTask().endEvent().done());

		ClockUtil.CurrentTime = START_DATE;

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string jobId = managementService.createJobQuery().singleResult().Id;

		managementService.setJobRetries(jobId, 0);

		try
		{
		  managementService.executeJob(jobId);
		}
		catch (Exception)
		{
		}

		string taskId = historyService.createHistoricTaskInstanceQuery().singleResult().Id;

		// when
		taskService.complete(taskId);

		IList<HistoricIncident> historicIncidents = historyService.createHistoricIncidentQuery().list();

		// then
		assertThat(historicIncidents[0].CreateTime, @is(START_DATE));
		assertThat(historicIncidents[1].CreateTime, @is(START_DATE));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotResolveStandaloneIncident()
	  public virtual void shouldNotResolveStandaloneIncident()
	  {
		// given
		ClockUtil.CurrentTime = END_DATE;

		testRule.deploy(CALLED_PROCESS);

		repositoryService.suspendProcessDefinitionByKey(CALLED_PROCESS_KEY, true, DateTime.Now);

		string jobId = managementService.createJobQuery().singleResult().Id;

		managementService.setJobRetries(jobId, 0);

		// when
		managementService.executeJob(jobId);

		HistoricIncident historicIncident = historyService.createHistoricIncidentQuery().singleResult();

		// assume
		assertThat(historicIncident, notNullValue());

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
		testRule.deploy(Bpmn.createExecutableProcess("calledProcess").startEvent().serviceTask().camundaExternalTask("anExternalTaskTopic").endEvent().done());

		testRule.deploy(Bpmn.createExecutableProcess("callingProcess").camundaHistoryTimeToLive(5).startEvent().callActivity().calledElement("calledProcess").endEvent().done());

		ClockUtil.CurrentTime = START_DATE;

		runtimeService.startProcessInstanceByKey("callingProcess");

		LockedExternalTask externalTask = externalTaskService.fetchAndLock(1, "aWorkerId").topic("anExternalTaskTopic", 3000).execute()[0];

		HistoricExternalTaskLog externalTaskLog = historyService.createHistoricExternalTaskLogQuery().singleResult();

		// assume
		assertThat(externalTaskLog.RemovalTime, nullValue());

		ClockUtil.CurrentTime = END_DATE;

		// when
		externalTaskService.complete(externalTask.Id, "aWorkerId");

		DateTime removalTime = addDays(END_DATE, 5);

		IList<HistoricExternalTaskLog> externalTaskLogs = historyService.createHistoricExternalTaskLogQuery().list();

		// then
		assertThat(externalTaskLogs[0].RemovalTime, @is(removalTime));
		assertThat(externalTaskLogs[1].RemovalTime, @is(removalTime));
	  }

	  /// <summary>
	  /// See https://app.camunda.com/jira/browse/CAM-9505
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveExternalTaskLogWithTimestampPreserved()
	  public virtual void shouldResolveExternalTaskLogWithTimestampPreserved()
	  {
		// given
		testRule.deploy(Bpmn.createExecutableProcess("calledProcess").startEvent().serviceTask().camundaExternalTask("anExternalTaskTopic").endEvent().done());

		testRule.deploy(Bpmn.createExecutableProcess("callingProcess").camundaHistoryTimeToLive(5).startEvent().callActivity().calledElement("calledProcess").endEvent().done());

		ClockUtil.CurrentTime = START_DATE;

		runtimeService.startProcessInstanceByKey("callingProcess");

		LockedExternalTask externalTask = externalTaskService.fetchAndLock(1, "aWorkerId").topic("anExternalTaskTopic", 3000).execute()[0];

		// when
		externalTaskService.complete(externalTask.Id, "aWorkerId");

		IList<HistoricExternalTaskLog> externalTaskLogs = historyService.createHistoricExternalTaskLogQuery().list();

		// then
		assertThat(externalTaskLogs[0].Timestamp, @is(START_DATE));
		assertThat(externalTaskLogs[1].Timestamp, @is(START_DATE));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveJobLog()
	  public virtual void shouldResolveJobLog()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(Bpmn.createExecutableProcess(CALLED_PROCESS_KEY).startEvent().camundaAsyncBefore().userTask("userTask").name("userTask").endEvent().done());

		ClockUtil.CurrentTime = START_DATE;

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string jobId = managementService.createJobQuery().singleResult().Id;

		try
		{
		  managementService.executeJob(jobId);
		}
		catch (Exception)
		{
		}

		IList<HistoricJobLog> jobLog = historyService.createHistoricJobLogQuery().list();

		// assume
		assertThat(jobLog[0].RemovalTime, nullValue());
		assertThat(jobLog[1].RemovalTime, nullValue());

		ClockUtil.CurrentTime = END_DATE;

		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.complete(taskId);

		jobLog = historyService.createHistoricJobLogQuery().list();

		DateTime removalTime = addDays(END_DATE, 5);

		// then
		assertThat(jobLog[0].RemovalTime, @is(removalTime));
		assertThat(jobLog[1].RemovalTime, @is(removalTime));
	  }

	  /// <summary>
	  /// See https://app.camunda.com/jira/browse/CAM-9505
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveJobLogWithTimestampPreserved()
	  public virtual void shouldResolveJobLogWithTimestampPreserved()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(Bpmn.createExecutableProcess(CALLED_PROCESS_KEY).startEvent().camundaAsyncBefore().userTask("userTask").name("userTask").endEvent().done());

		ClockUtil.CurrentTime = START_DATE;

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string jobId = managementService.createJobQuery().singleResult().Id;

		try
		{
		  managementService.executeJob(jobId);
		}
		catch (Exception)
		{
		}

		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.complete(taskId);

		IList<HistoricJobLog> jobLog = historyService.createHistoricJobLogQuery().list();

		// then
		assertThat(jobLog[0].Timestamp, @is(START_DATE));
		assertThat(jobLog[1].Timestamp, @is(START_DATE));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveUserOperationLog_SetJobRetries()
	  public virtual void shouldResolveUserOperationLog_SetJobRetries()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(Bpmn.createExecutableProcess(CALLED_PROCESS_KEY).startEvent().camundaAsyncBefore().userTask("userTask").name("userTask").endEvent().done());

		ClockUtil.CurrentTime = START_DATE;

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string jobId = managementService.createJobQuery().singleResult().Id;

		identityService.AuthenticatedUserId = "aUserId";
		managementService.setJobRetries(jobId, 65);
		identityService.clearAuthentication();

		UserOperationLogEntry userOperationLog = historyService.createUserOperationLogQuery().singleResult();

		// assume
		assertThat(userOperationLog.RemovalTime, nullValue());

		managementService.executeJob(jobId);

		ClockUtil.CurrentTime = END_DATE;

		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.complete(taskId);

		userOperationLog = historyService.createUserOperationLogQuery().singleResult();

		DateTime removalTime = addDays(END_DATE, 5);

		// then
		assertThat(userOperationLog.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveUserOperationLog_SetExternalTaskRetries()
	  public virtual void shouldResolveUserOperationLog_SetExternalTaskRetries()
	  {
		// given
		testRule.deploy(Bpmn.createExecutableProcess("calledProcess").startEvent().serviceTask().camundaExternalTask("anExternalTaskTopic").endEvent().done());

		testRule.deploy(Bpmn.createExecutableProcess("callingProcess").camundaHistoryTimeToLive(5).startEvent().callActivity().calledElement("calledProcess").endEvent().done());

		ClockUtil.CurrentTime = START_DATE;

		runtimeService.startProcessInstanceByKey("callingProcess");

		string externalTaskId = externalTaskService.createExternalTaskQuery().singleResult().Id;

		identityService.AuthenticatedUserId = "aUserId";
		externalTaskService.setRetries(externalTaskId, 65);
		identityService.clearAuthentication();

		UserOperationLogEntry userOperationLog = historyService.createUserOperationLogQuery().singleResult();

		// assume
		assertThat(userOperationLog.RemovalTime, nullValue());

		LockedExternalTask externalTask = externalTaskService.fetchAndLock(1, "aWorkerId").topic("anExternalTaskTopic", 2000).execute()[0];

		ClockUtil.CurrentTime = END_DATE;

		// when
		externalTaskService.complete(externalTask.Id, "aWorkerId");

		DateTime removalTime = addDays(END_DATE, 5);

		userOperationLog = historyService.createUserOperationLogQuery().singleResult();

		// then
		assertThat(userOperationLog.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveUserOperationLog_ClaimTask()
	  public virtual void shouldResolveUserOperationLog_ClaimTask()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		ClockUtil.CurrentTime = START_DATE;

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string taskId = taskService.createTaskQuery().singleResult().Id;

		identityService.AuthenticatedUserId = "aUserId";
		taskService.claim(taskId, "aUserId");
		identityService.clearAuthentication();

		UserOperationLogEntry userOperationLog = historyService.createUserOperationLogQuery().singleResult();

		// assume
		assertThat(userOperationLog.RemovalTime, nullValue());

		ClockUtil.CurrentTime = END_DATE;

		// when
		taskService.complete(taskId);

		userOperationLog = historyService.createUserOperationLogQuery().singleResult();

		DateTime removalTime = addDays(END_DATE, 5);

		// then
		assertThat(userOperationLog.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveUserOperationLog_CreateAttachment()
	  public virtual void shouldResolveUserOperationLog_CreateAttachment()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		ClockUtil.CurrentTime = START_DATE;

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string processInstanceId = runtimeService.createProcessInstanceQuery().activityIdIn("userTask").singleResult().Id;

		identityService.AuthenticatedUserId = "aUserId";
		taskService.createAttachment(null, null, processInstanceId, null, null, "http://camunda.com");
		identityService.clearAuthentication();

		UserOperationLogEntry userOperationLog = historyService.createUserOperationLogQuery().singleResult();

		// assume
		assertThat(userOperationLog.RemovalTime, nullValue());

		string taskId = taskService.createTaskQuery().singleResult().Id;

		ClockUtil.CurrentTime = END_DATE;

		// when
		taskService.complete(taskId);

		userOperationLog = historyService.createUserOperationLogQuery().singleResult();

		DateTime removalTime = addDays(END_DATE, 5);

		// then
		assertThat(userOperationLog.RemovalTime, @is(removalTime));
	  }

	  /// <summary>
	  /// See https://app.camunda.com/jira/browse/CAM-9505
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveUserOperationLogWithTimestampPreserved()
	  public virtual void shouldResolveUserOperationLogWithTimestampPreserved()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		ClockUtil.CurrentTime = START_DATE;

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string processInstanceId = runtimeService.createProcessInstanceQuery().activityIdIn("userTask").singleResult().Id;

		identityService.AuthenticatedUserId = "aUserId";
		taskService.createAttachment(null, null, processInstanceId, null, null, "http://camunda.com");
		identityService.clearAuthentication();

		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.complete(taskId);

		UserOperationLogEntry userOperationLog = historyService.createUserOperationLogQuery().singleResult();

		// then
		assertThat(userOperationLog.Timestamp, @is(START_DATE));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveIdentityLink_AddCandidateUser()
	  public virtual void shouldResolveIdentityLink_AddCandidateUser()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		ClockUtil.CurrentTime = START_DATE;

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string taskId = taskService.createTaskQuery().singleResult().Id;

		taskService.addCandidateUser(taskId, "aUserId");

		HistoricIdentityLinkLog historicIdentityLinkLog = historyService.createHistoricIdentityLinkLogQuery().singleResult();

		// assume
		assertThat(historicIdentityLinkLog.RemovalTime, nullValue());

		ClockUtil.CurrentTime = END_DATE;

		// when
		taskService.complete(taskId);

		historicIdentityLinkLog = historyService.createHistoricIdentityLinkLogQuery().singleResult();

		DateTime removalTime = addDays(END_DATE, 5);

		// then
		assertThat(historicIdentityLinkLog.RemovalTime, @is(removalTime));
	  }

	  /// <summary>
	  /// See https://app.camunda.com/jira/browse/CAM-9505
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveIdentityLinkWithTimePreserved()
	  public virtual void shouldResolveIdentityLinkWithTimePreserved()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		ClockUtil.CurrentTime = START_DATE;

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string taskId = taskService.createTaskQuery().singleResult().Id;

		taskService.addCandidateUser(taskId, "aUserId");

		// when
		taskService.complete(taskId);

		HistoricIdentityLinkLog historicIdentityLinkLog = historyService.createHistoricIdentityLinkLogQuery().singleResult();

		// then
		assertThat(historicIdentityLinkLog.Time, @is(START_DATE));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotResolveIdentityLink_AddCandidateUser()
	  public virtual void shouldNotResolveIdentityLink_AddCandidateUser()
	  {
		// given
		ClockUtil.CurrentTime = END_DATE;

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
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		ClockUtil.CurrentTime = START_DATE;

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string processInstanceId = runtimeService.createProcessInstanceQuery().activityIdIn("userTask").singleResult().Id;

		taskService.createComment(null, processInstanceId, "aMessage");

		Comment comment = taskService.getProcessInstanceComments(processInstanceId)[0];

		// assume
		assertThat(comment.RemovalTime, nullValue());

		string taskId = taskService.createTaskQuery().singleResult().Id;

		ClockUtil.CurrentTime = END_DATE;

		// when
		taskService.complete(taskId);

		comment = taskService.getProcessInstanceComments(processInstanceId)[0];

		DateTime removalTime = addDays(END_DATE, 5);

		// then
		assertThat(comment.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveCommentByTaskId()
	  public virtual void shouldResolveCommentByTaskId()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		ClockUtil.CurrentTime = START_DATE;

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string taskId = taskService.createTaskQuery().singleResult().Id;

		taskService.createComment(taskId, null, "aMessage");

		Comment comment = taskService.getTaskComments(taskId)[0];

		// assume
		assertThat(comment.RemovalTime, nullValue());

		ClockUtil.CurrentTime = END_DATE;

		// when
		taskService.complete(taskId);

		comment = taskService.getTaskComments(taskId)[0];

		DateTime removalTime = addDays(END_DATE, 5);

		// then
		assertThat(comment.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotResolveCommentByWrongTaskIdAndProcessInstanceId()
	  public virtual void shouldNotResolveCommentByWrongTaskIdAndProcessInstanceId()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		ClockUtil.CurrentTime = START_DATE;

		string processInstanceId = runtimeService.createProcessInstanceQuery().activityIdIn("userTask").singleResult().Id;

		taskService.createComment("aNonExistentTaskId", processInstanceId, "aMessage");

		Comment comment = taskService.getProcessInstanceComments(processInstanceId)[0];

		ClockUtil.CurrentTime = END_DATE;

		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.complete(taskId);

		// then
		assertThat(comment.RemovalTime, nullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveCommentByTaskIdAndWrongProcessInstanceId()
	  public virtual void shouldResolveCommentByTaskIdAndWrongProcessInstanceId()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		ClockUtil.CurrentTime = START_DATE;

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string taskId = taskService.createTaskQuery().singleResult().Id;

		taskService.createComment(taskId, "aNonExistentProcessInstanceId", "aMessage");

		Comment comment = taskService.getTaskComments(taskId)[0];

		// assume
		assertThat(comment.RemovalTime, nullValue());

		ClockUtil.CurrentTime = END_DATE;

		// when
		taskService.complete(taskId);

		comment = taskService.getTaskComments(taskId)[0];

		DateTime removalTime = addDays(END_DATE, 5);

		// then
		assertThat(comment.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveAttachmentByProcessInstanceId()
	  public virtual void shouldResolveAttachmentByProcessInstanceId()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		ClockUtil.CurrentTime = START_DATE;

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string processInstanceId = runtimeService.createProcessInstanceQuery().activityIdIn("userTask").singleResult().Id;

		string attachmentId = taskService.createAttachment(null, null, processInstanceId, null, null, "http://camunda.com").Id;

		Attachment attachment = taskService.getAttachment(attachmentId);

		// assume
		assertThat(attachment.RemovalTime, nullValue());

		string taskId = taskService.createTaskQuery().singleResult().Id;

		ClockUtil.CurrentTime = END_DATE;

		// when
		taskService.complete(taskId);

		DateTime removalTime = addDays(END_DATE, 5);

		attachment = taskService.getAttachment(attachmentId);

		// then
		assertThat(attachment.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveAttachmentByTaskId()
	  public virtual void shouldResolveAttachmentByTaskId()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		ClockUtil.CurrentTime = START_DATE;

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string taskId = taskService.createTaskQuery().singleResult().Id;

		string attachmentId = taskService.createAttachment(null, taskId, null, null, null, "http://camunda.com").Id;

		Attachment attachment = taskService.getAttachment(attachmentId);

		// assume
		assertThat(attachment.RemovalTime, nullValue());

		ClockUtil.CurrentTime = END_DATE;

		// when
		taskService.complete(taskId);

		attachment = taskService.getAttachment(attachmentId);

		DateTime removalTime = addDays(END_DATE, 5);

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

		ClockUtil.CurrentTime = START_DATE;

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string processInstanceId = runtimeService.createProcessInstanceQuery().activityIdIn("userTask").singleResult().Id;

		string attachmentId = taskService.createAttachment(null, "aWrongTaskId", processInstanceId, null, null, "http://camunda.com").Id;

		Attachment attachment = taskService.getAttachment(attachmentId);

		ClockUtil.CurrentTime = END_DATE;

		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.complete(taskId);

		// then
		assertThat(attachment.RemovalTime, nullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveAttachmentByTaskIdAndWrongProcessInstanceId()
	  public virtual void shouldResolveAttachmentByTaskIdAndWrongProcessInstanceId()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		ClockUtil.CurrentTime = START_DATE;

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string taskId = taskService.createTaskQuery().singleResult().Id;

		string attachmentId = taskService.createAttachment(null, taskId, "aWrongProcessInstanceId", null, null, "http://camunda.com").Id;

		Attachment attachment = taskService.getAttachment(attachmentId);

		// assume
		assertThat(attachment.RemovalTime, nullValue());

		ClockUtil.CurrentTime = END_DATE;

		// when
		taskService.complete(taskId);

		attachment = taskService.getAttachment(attachmentId);

		DateTime removalTime = addDays(END_DATE, 5);

		// then
		assertThat(attachment.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveByteArray_CreateAttachmentByTask()
	  public virtual void shouldResolveByteArray_CreateAttachmentByTask()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		ClockUtil.CurrentTime = START_DATE;

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string taskId = taskService.createTaskQuery().singleResult().Id;

		AttachmentEntity attachment = (AttachmentEntity) taskService.createAttachment(null, taskId, null, null, null, new MemoryStream("hello world".GetBytes()));

		ByteArrayEntity byteArray = findByteArrayById(attachment.ContentId);

		// assume
		assertThat(byteArray.RemovalTime, nullValue());

		ClockUtil.CurrentTime = END_DATE;

		// when
		taskService.complete(taskId);

		byteArray = findByteArrayById(attachment.ContentId);

		DateTime removalTime = addDays(END_DATE, 5);

		// then
		assertThat(byteArray.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveByteArray_CreateAttachmentByProcessInstance()
	  public virtual void shouldResolveByteArray_CreateAttachmentByProcessInstance()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		ClockUtil.CurrentTime = START_DATE;

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string calledProcessInstanceId = runtimeService.createProcessInstanceQuery().activityIdIn("userTask").singleResult().Id;

		AttachmentEntity attachment = (AttachmentEntity) taskService.createAttachment(null, null, calledProcessInstanceId, null, null, new MemoryStream("hello world".GetBytes()));

		ByteArrayEntity byteArray = findByteArrayById(attachment.ContentId);

		// assume
		assertThat(byteArray.RemovalTime, nullValue());

		ClockUtil.CurrentTime = END_DATE;

		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.complete(taskId);

		byteArray = findByteArrayById(attachment.ContentId);

		DateTime removalTime = addDays(END_DATE, 5);

		// then
		assertThat(byteArray.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveByteArray_SetVariable()
	  public virtual void shouldResolveByteArray_SetVariable()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		ClockUtil.CurrentTime = START_DATE;

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		runtimeService.setVariable(processInstance.Id, "aVariableName", new MemoryStream("hello world".GetBytes()));

		HistoricVariableInstanceEntity historicVariableInstance = (HistoricVariableInstanceEntity) historyService.createHistoricVariableInstanceQuery().singleResult();

		ByteArrayEntity byteArray = findByteArrayById(historicVariableInstance.ByteArrayId);

		// assume
		assertThat(byteArray.RemovalTime, nullValue());

		string taskId = taskService.createTaskQuery().singleResult().Id;

		ClockUtil.CurrentTime = END_DATE;

		// when
		taskService.complete(taskId);

		byteArray = findByteArrayById(historicVariableInstance.ByteArrayId);

		DateTime removalTime = addDays(END_DATE, 5);

		// then
		assertThat(byteArray.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveByteArray_UpdateVariable()
	  public virtual void shouldResolveByteArray_UpdateVariable()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS);

		ClockUtil.CurrentTime = START_DATE;

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY, Variables.createVariables().putValue("aVariableName", Variables.stringValue("aVariableValue")));

		runtimeService.setVariable(processInstance.Id, "aVariableName", new MemoryStream("hello world".GetBytes()));

		HistoricDetailVariableInstanceUpdateEntity historicDetails = (HistoricDetailVariableInstanceUpdateEntity) historyService.createHistoricDetailQuery().variableUpdates().variableTypeIn("Bytes").singleResult();

		ByteArrayEntity byteArray = findByteArrayById(historicDetails.ByteArrayValueId);

		// assume
		assertThat(byteArray.RemovalTime, nullValue());

		ClockUtil.CurrentTime = END_DATE;

		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.complete(taskId);

		byteArray = findByteArrayById(historicDetails.ByteArrayValueId);

		DateTime removalTime = addDays(END_DATE, 5);

		// then
		assertThat(byteArray.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveByteArray_JobLog()
	  public virtual void shouldResolveByteArray_JobLog()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(Bpmn.createExecutableProcess(CALLED_PROCESS_KEY).startEvent().scriptTask().camundaAsyncBefore().scriptFormat("groovy").scriptText("if(execution.getIncidents().size() == 0) throw new RuntimeException(\"I'm supposed to fail!\")").endEvent().done());

		ClockUtil.CurrentTime = START_DATE;

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string jobId = managementService.createJobQuery().singleResult().Id;

		try
		{
		  managementService.executeJob(jobId);
		}
		catch (Exception)
		{
		}

		HistoricJobLogEventEntity jobLog = (HistoricJobLogEventEntity) historyService.createHistoricJobLogQuery().failureLog().singleResult();

		ByteArrayEntity byteArray = findByteArrayById(jobLog.ExceptionByteArrayId);

		// assume
		assertThat(byteArray.RemovalTime, nullValue());

		ClockUtil.CurrentTime = END_DATE;

		managementService.setJobRetries(jobId, 0);

		try
		{
		  // when
		  managementService.executeJob(jobId);
		}
		catch (Exception)
		{
		}

		DateTime removalTime = addDays(END_DATE, 5);

		byteArray = findByteArrayById(jobLog.ExceptionByteArrayId);

		// then
		assertThat(byteArray.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveByteArray_ExternalTaskLog()
	  public virtual void shouldResolveByteArray_ExternalTaskLog()
	  {
		// given
		testRule.deploy(Bpmn.createExecutableProcess("calledProcess").startEvent().serviceTask().camundaExternalTask("aTopicName").endEvent().done());

		testRule.deploy(Bpmn.createExecutableProcess("callingProcess").camundaHistoryTimeToLive(5).startEvent().callActivity().calledElement("calledProcess").endEvent().done());

		ClockUtil.CurrentTime = START_DATE;

		runtimeService.startProcessInstanceByKey("callingProcess");

		string taskId = externalTaskService.fetchAndLock(5, "aWorkerId").topic("aTopicName", int.MaxValue).execute()[0].Id;

		externalTaskService.handleFailure(taskId, "aWorkerId", null, "errorDetails", 5, 3000L);

		HistoricExternalTaskLogEntity externalTaskLog = (HistoricExternalTaskLogEntity) historyService.createHistoricExternalTaskLogQuery().failureLog().singleResult();

		ByteArrayEntity byteArrayEntity = findByteArrayById(externalTaskLog.ErrorDetailsByteArrayId);

		// assume
		assertThat(byteArrayEntity.RemovalTime, nullValue());

		ClockUtil.CurrentTime = END_DATE;

		// when
		externalTaskService.complete(taskId, "aWorkerId");

		byteArrayEntity = findByteArrayById(externalTaskLog.ErrorDetailsByteArrayId);

		DateTime removalTime = addDays(END_DATE, 5);

		// then
		assertThat(byteArrayEntity.RemovalTime, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml" }) public void shouldResolveByteArray_DecisionInput()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml" })]
	  public virtual void shouldResolveByteArray_DecisionInput()
	  {
		// given
		testRule.deploy(Bpmn.createExecutableProcess(CALLING_PROCESS_KEY).camundaHistoryTimeToLive(5).startEvent().businessRuleTask().camundaDecisionRef("testDecision").userTask().endEvent().done());

		ClockUtil.CurrentTime = START_DATE;

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY, Variables.createVariables().putValue("pojo", new TestPojo("okay", 13.37)));

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().includeInputs().singleResult();

		HistoricDecisionInputInstanceEntity historicDecisionInputInstanceEntity = (HistoricDecisionInputInstanceEntity) historicDecisionInstance.Inputs[0];

		ByteArrayEntity byteArrayEntity = findByteArrayById(historicDecisionInputInstanceEntity.ByteArrayValueId);

		// assume
		assertThat(byteArrayEntity.RemovalTime, nullValue());

		string taskId = taskService.createTaskQuery().singleResult().Id;

		ClockUtil.CurrentTime = END_DATE;

		// when
		taskService.complete(taskId);

		byteArrayEntity = findByteArrayById(historicDecisionInputInstanceEntity.ByteArrayValueId);

		DateTime removalTime = addDays(END_DATE, 5);

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

		testRule.deploy(Bpmn.createExecutableProcess(CALLING_PROCESS_KEY).camundaHistoryTimeToLive(5).startEvent().businessRuleTask().camundaDecisionRef("testDecision").userTask().endEvent().done());

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY, Variables.createVariables().putValue("pojo", new TestPojo("okay", 13.37)));

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().includeOutputs().singleResult();

		HistoricDecisionOutputInstanceEntity historicDecisionOutputInstanceEntity = (HistoricDecisionOutputInstanceEntity) historicDecisionInstance.Outputs[0];

		ByteArrayEntity byteArrayEntity = findByteArrayById(historicDecisionOutputInstanceEntity.ByteArrayValueId);

		// assume
		assertThat(byteArrayEntity.RemovalTime, nullValue());

		ClockUtil.CurrentTime = END_DATE;

		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.complete(taskId);

		byteArrayEntity = findByteArrayById(historicDecisionOutputInstanceEntity.ByteArrayValueId);

		DateTime removalTime = addDays(END_DATE, 5);

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

		testRule.deploy(Bpmn.createExecutableProcess(CALLING_PROCESS_KEY).camundaHistoryTimeToLive(5).startEvent().businessRuleTask().camundaDecisionRef("testDecision").userTask().endEvent().done());

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY, Variables.createVariables().putValue("pojo", new TestPojo("okay", 13.37)));

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().includeOutputs().singleResult();

		HistoricDecisionOutputInstanceEntity historicDecisionOutputInstanceEntity = (HistoricDecisionOutputInstanceEntity) historicDecisionInstance.Outputs[0];

		ByteArrayEntity byteArrayEntity = findByteArrayById(historicDecisionOutputInstanceEntity.ByteArrayValueId);

		// assume
		assertThat(byteArrayEntity.RemovalTime, nullValue());

		string taskId = taskService.createTaskQuery().singleResult().Id;

		ClockUtil.CurrentTime = END_DATE;

		// when
		taskService.complete(taskId);

		byteArrayEntity = findByteArrayById(historicDecisionOutputInstanceEntity.ByteArrayValueId);

		DateTime removalTime = addDays(END_DATE, 5);

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

		Batch batch = runtimeService.deleteProcessInstancesAsync(Collections.singletonList(processInstanceId), "aDeleteReason");

		ClockUtil.CurrentTime = END_DATE;

		string jobId = managementService.createJobQuery().singleResult().Id;
		managementService.executeJob(jobId);

		IList<Job> jobs = managementService.createJobQuery().list();
		foreach (Job job in jobs)
		{
		  managementService.executeJob(job.Id);
		}

		HistoricBatch historicBatch = historyService.createHistoricBatchQuery().singleResult();

		// then
		assertThat(historicBatch.RemovalTime, @is(addDays(END_DATE, 5)));

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

		Batch batch = runtimeService.deleteProcessInstancesAsync(Collections.singletonList(processInstanceId), "aDeleteReason");

		ClockUtil.CurrentTime = END_DATE;

		string jobId = managementService.createJobQuery().singleResult().Id;
		managementService.executeJob(jobId);

		IList<Job> jobs = managementService.createJobQuery().list();
		foreach (Job job in jobs)
		{
		  managementService.executeJob(job.Id);
		}

		IList<HistoricJobLog> jobLogs = historyService.createHistoricJobLogQuery().list();

		// then
		assertThat(jobLogs[0].RemovalTime, @is(addDays(END_DATE, 5)));
		assertThat(jobLogs[1].RemovalTime, @is(addDays(END_DATE, 5)));
		assertThat(jobLogs[2].RemovalTime, @is(addDays(END_DATE, 5)));

		// cleanup
		historyService.deleteHistoricBatch(batch.Id);
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

		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		Batch batch = runtimeService.deleteProcessInstancesAsync(Collections.singletonList(processInstanceId), "aDeleteReason");

		ClockUtil.CurrentTime = END_DATE;

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

		jobs = managementService.createJobQuery().list();
		foreach (Job job in jobs)
		{
		  managementService.executeJob(job.Id);
		}

		HistoricJobLogEventEntity jobLog = (HistoricJobLogEventEntity)historyService.createHistoricJobLogQuery().failureLog().singleResult();

		string byteArrayId = jobLog.ExceptionByteArrayId;

		ByteArrayEntity byteArray = findByteArrayById(byteArrayId);

		// then
		assertThat(byteArray.RemovalTime, @is(addDays(END_DATE, 5)));

		// cleanup
		historyService.deleteHistoricBatch(batch.Id);
		FailingExecutionListener.shouldFail = false;
	  }

	  /// <summary>
	  /// See https://app.camunda.com/jira/browse/CAM-9505
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResolveBatchJobLogWithTimestampPreserved()
	  public virtual void shouldResolveBatchJobLogWithTimestampPreserved()
	  {
		// given
		processEngineConfiguration.BatchOperationHistoryTimeToLive = "P5D";
		processEngineConfiguration.initHistoryCleanup();

		testRule.deploy(CALLED_PROCESS);

		testRule.deploy(CALLING_PROCESS);

		ClockUtil.CurrentTime = START_DATE;

		string processInstanceId = runtimeService.startProcessInstanceByKey(CALLED_PROCESS_KEY).Id;

		Batch batch = runtimeService.deleteProcessInstancesAsync(Collections.singletonList(processInstanceId), "aDeleteReason");

		string jobId = managementService.createJobQuery().singleResult().Id;
		managementService.executeJob(jobId);

		IList<Job> jobs = managementService.createJobQuery().list();

		managementService.executeJob(jobs[0].Id);

		// when
		managementService.executeJob(jobs[1].Id);

		IList<HistoricJobLog> jobLogs = historyService.createHistoricJobLogQuery().list();

		// then
		assertThat(jobLogs[0].Timestamp, @is(START_DATE));
		assertThat(jobLogs[1].Timestamp, @is(START_DATE));
		assertThat(jobLogs[2].Timestamp, @is(START_DATE));

		// cleanup
		historyService.deleteHistoricBatch(batch.Id);
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
		assertThat(jobLog.RemovalTime, nullValue());

		managementService.setJobRetries(jobLog.JobId, 0);

		managementService.executeJob(jobLog.JobId);

		IList<Job> jobs = managementService.createJobQuery().list();
		managementService.executeJob(jobs[0].Id);
		managementService.executeJob(jobs[1].Id);

		HistoricIncident historicIncident = historyService.createHistoricIncidentQuery().singleResult();

		// then
		assertThat(historicIncident.RemovalTime, @is(addDays(START_DATE, 5)));

		// cleanup
		historyService.deleteHistoricBatch(batch.Id);
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
		assertThat(jobLog.RemovalTime, nullValue());

		// when
		runtimeService.deleteProcessInstance(processInstanceId, "aDeleteReason");

		managementService.executeJob(jobLog.JobId);

		string jobId = managementService.createJobQuery().jobDefinitionId(batch.BatchJobDefinitionId).singleResult().Id;

		managementService.setJobRetries(jobId, 0);

		managementService.deleteJob(jobId);

		jobId = managementService.createJobQuery().jobDefinitionId(batch.MonitorJobDefinitionId).singleResult().Id;

		managementService.executeJob(jobId);

		HistoricIncident historicIncident = historyService.createHistoricIncidentQuery().singleResult();

		// then
		assertThat(historicIncident.RemovalTime, @is(addDays(START_DATE, 5)));

		// cleanup
		historyService.deleteHistoricBatch(batch.Id);
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
		assertThat(jobLog.RemovalTime, nullValue());

		managementService.executeJob(jobLog.JobId);

		string jobId = managementService.createJobQuery().jobDefinitionId(batch.BatchJobDefinitionId).singleResult().Id;
		managementService.executeJob(jobId);

		jobId = managementService.createJobQuery().jobDefinitionId(batch.MonitorJobDefinitionId).singleResult().Id;
		managementService.setJobRetries(jobId, 0);
		managementService.executeJob(jobId);

		HistoricIncident historicIncident = historyService.createHistoricIncidentQuery().singleResult();

		// then
		assertThat(historicIncident.RemovalTime, @is(addDays(START_DATE, 5)));

		// cleanup
		historyService.deleteHistoricBatch(batch.Id);
	  }

	  /// <summary>
	  /// See https://app.camunda.com/jira/browse/CAM-9505
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotUpdateCreateTimeForIncidentRelatedToBatch()
	  public virtual void shouldNotUpdateCreateTimeForIncidentRelatedToBatch()
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
		assertThat(jobLog.RemovalTime, nullValue());

		managementService.setJobRetries(jobLog.JobId, 0);

		managementService.executeJob(jobLog.JobId);

		IList<Job> jobs = managementService.createJobQuery().list();
		managementService.executeJob(jobs[0].Id);
		managementService.executeJob(jobs[1].Id);

		HistoricIncident historicIncident = historyService.createHistoricIncidentQuery().singleResult();

		// then
		assertThat(historicIncident.CreateTime, @is(START_DATE));

		// cleanup
		historyService.deleteHistoricBatch(batch.Id);
	  }

	}

}