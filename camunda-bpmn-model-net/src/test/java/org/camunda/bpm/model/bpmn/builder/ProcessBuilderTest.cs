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
namespace org.camunda.bpm.model.bpmn.builder
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Fail.fail;
	using static org.camunda.bpm.model.bpmn.BpmnTestConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN20_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;


	using org.camunda.bpm.model.bpmn.instance;
	using CamundaExecutionListener = org.camunda.bpm.model.bpmn.instance.camunda.CamundaExecutionListener;
	using CamundaFailedJobRetryTimeCycle = org.camunda.bpm.model.bpmn.instance.camunda.CamundaFailedJobRetryTimeCycle;
	using CamundaFormData = org.camunda.bpm.model.bpmn.instance.camunda.CamundaFormData;
	using CamundaFormField = org.camunda.bpm.model.bpmn.instance.camunda.CamundaFormField;
	using CamundaIn = org.camunda.bpm.model.bpmn.instance.camunda.CamundaIn;
	using CamundaInputOutput = org.camunda.bpm.model.bpmn.instance.camunda.CamundaInputOutput;
	using CamundaInputParameter = org.camunda.bpm.model.bpmn.instance.camunda.CamundaInputParameter;
	using CamundaOut = org.camunda.bpm.model.bpmn.instance.camunda.CamundaOut;
	using CamundaOutputParameter = org.camunda.bpm.model.bpmn.instance.camunda.CamundaOutputParameter;
	using CamundaTaskListener = org.camunda.bpm.model.bpmn.instance.camunda.CamundaTaskListener;
	using Model = org.camunda.bpm.model.xml.Model;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;
	using After = org.junit.After;
	using BeforeClass = org.junit.BeforeClass;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;


	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class ProcessBuilderTest
	{

	  public const string TIMER_DATE = "2011-03-11T12:13:14Z";
	  public const string TIMER_DURATION = "P10D";
	  public const string TIMER_CYCLE = "R3/PT10H";

	  public const string FAILED_JOB_RETRY_TIME_CYCLE = "R5/PT1M";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  private BpmnModelInstance modelInstance;
	  private static ModelElementType taskType;
	  private static ModelElementType gatewayType;
	  private static ModelElementType eventType;
	  private static ModelElementType processType;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @BeforeClass public static void getElementTypes()
	  public static void getElementTypes()
	  {
		Model model = Bpmn.createEmptyModel().Model;
		taskType = model.getType(typeof(Task));
		gatewayType = model.getType(typeof(Gateway));
		eventType = model.getType(typeof(Event));
		processType = model.getType(typeof(Process));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void validateModel() throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void validateModel()
	  {
		if (modelInstance != null)
		{
		  Bpmn.validateModel(modelInstance);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateEmptyProcess()
	  public virtual void testCreateEmptyProcess()
	  {
		modelInstance = Bpmn.createProcess().done();

		Definitions definitions = modelInstance.Definitions;
		assertThat(definitions).NotNull;
		assertThat(definitions.TargetNamespace).isEqualTo(BPMN20_NS);

		ICollection<ModelElementInstance> processes = modelInstance.getModelElementsByType(processType);
		assertThat(processes).hasSize(1);

		Process process = (Process) processes.GetEnumerator().next();
		assertThat(process.Id).NotNull;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetElement()
	  public virtual void testGetElement()
	  {
		// Make sure this method is publicly available
		Process process = Bpmn.createProcess().Element;
		assertThat(process).NotNull;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateProcessWithStartEvent()
	  public virtual void testCreateProcessWithStartEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent().done();

		assertThat(modelInstance.getModelElementsByType(eventType)).hasSize(1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateProcessWithEndEvent()
	  public virtual void testCreateProcessWithEndEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent().endEvent().done();

		assertThat(modelInstance.getModelElementsByType(eventType)).hasSize(2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateProcessWithServiceTask()
	  public virtual void testCreateProcessWithServiceTask()
	  {
		modelInstance = Bpmn.createProcess().startEvent().serviceTask().endEvent().done();

		assertThat(modelInstance.getModelElementsByType(eventType)).hasSize(2);
		assertThat(modelInstance.getModelElementsByType(taskType)).hasSize(1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateProcessWithSendTask()
	  public virtual void testCreateProcessWithSendTask()
	  {
		modelInstance = Bpmn.createProcess().startEvent().sendTask().endEvent().done();

		assertThat(modelInstance.getModelElementsByType(eventType)).hasSize(2);
		assertThat(modelInstance.getModelElementsByType(taskType)).hasSize(1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateProcessWithUserTask()
	  public virtual void testCreateProcessWithUserTask()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask().endEvent().done();

		assertThat(modelInstance.getModelElementsByType(eventType)).hasSize(2);
		assertThat(modelInstance.getModelElementsByType(taskType)).hasSize(1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateProcessWithBusinessRuleTask()
	  public virtual void testCreateProcessWithBusinessRuleTask()
	  {
		modelInstance = Bpmn.createProcess().startEvent().businessRuleTask().endEvent().done();

		assertThat(modelInstance.getModelElementsByType(eventType)).hasSize(2);
		assertThat(modelInstance.getModelElementsByType(taskType)).hasSize(1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateProcessWithScriptTask()
	  public virtual void testCreateProcessWithScriptTask()
	  {
		modelInstance = Bpmn.createProcess().startEvent().scriptTask().endEvent().done();

		assertThat(modelInstance.getModelElementsByType(eventType)).hasSize(2);
		assertThat(modelInstance.getModelElementsByType(taskType)).hasSize(1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateProcessWithReceiveTask()
	  public virtual void testCreateProcessWithReceiveTask()
	  {
		modelInstance = Bpmn.createProcess().startEvent().receiveTask().endEvent().done();

		assertThat(modelInstance.getModelElementsByType(eventType)).hasSize(2);
		assertThat(modelInstance.getModelElementsByType(taskType)).hasSize(1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateProcessWithManualTask()
	  public virtual void testCreateProcessWithManualTask()
	  {
		modelInstance = Bpmn.createProcess().startEvent().manualTask().endEvent().done();

		assertThat(modelInstance.getModelElementsByType(eventType)).hasSize(2);
		assertThat(modelInstance.getModelElementsByType(taskType)).hasSize(1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateProcessWithParallelGateway()
	  public virtual void testCreateProcessWithParallelGateway()
	  {
		modelInstance = Bpmn.createProcess().startEvent().parallelGateway().scriptTask().endEvent().moveToLastGateway().userTask().endEvent().done();

		assertThat(modelInstance.getModelElementsByType(eventType)).hasSize(3);
		assertThat(modelInstance.getModelElementsByType(taskType)).hasSize(2);
		assertThat(modelInstance.getModelElementsByType(gatewayType)).hasSize(1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateProcessWithExclusiveGateway()
	  public virtual void testCreateProcessWithExclusiveGateway()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask().exclusiveGateway().condition("approved", "${approved}").serviceTask().endEvent().moveToLastGateway().condition("not approved", "${!approved}").scriptTask().endEvent().done();

		assertThat(modelInstance.getModelElementsByType(eventType)).hasSize(3);
		assertThat(modelInstance.getModelElementsByType(taskType)).hasSize(3);
		assertThat(modelInstance.getModelElementsByType(gatewayType)).hasSize(1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateProcessWithInclusiveGateway()
	  public virtual void testCreateProcessWithInclusiveGateway()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask().inclusiveGateway().condition("approved", "${approved}").serviceTask().endEvent().moveToLastGateway().condition("not approved", "${!approved}").scriptTask().endEvent().done();

		ModelElementType inclusiveGwType = modelInstance.Model.getType(typeof(InclusiveGateway));

		assertThat(modelInstance.getModelElementsByType(eventType)).hasSize(3);
		assertThat(modelInstance.getModelElementsByType(taskType)).hasSize(3);
		assertThat(modelInstance.getModelElementsByType(inclusiveGwType)).hasSize(1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateProcessWithForkAndJoin()
	  public virtual void testCreateProcessWithForkAndJoin()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask().parallelGateway().serviceTask().parallelGateway().id("join").moveToLastGateway().scriptTask().connectTo("join").userTask().endEvent().done();

		assertThat(modelInstance.getModelElementsByType(eventType)).hasSize(2);
		assertThat(modelInstance.getModelElementsByType(taskType)).hasSize(4);
		assertThat(modelInstance.getModelElementsByType(gatewayType)).hasSize(2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateProcessWithMultipleParallelTask()
	  public virtual void testCreateProcessWithMultipleParallelTask()
	  {
		modelInstance = Bpmn.createProcess().startEvent().parallelGateway("fork").userTask().parallelGateway("join").moveToNode("fork").serviceTask().connectTo("join").moveToNode("fork").userTask().connectTo("join").moveToNode("fork").scriptTask().connectTo("join").endEvent().done();

		assertThat(modelInstance.getModelElementsByType(eventType)).hasSize(2);
		assertThat(modelInstance.getModelElementsByType(taskType)).hasSize(4);
		assertThat(modelInstance.getModelElementsByType(gatewayType)).hasSize(2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExtend()
	  public virtual void testExtend()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask().id("task1").serviceTask().endEvent().done();

		assertThat(modelInstance.getModelElementsByType(taskType)).hasSize(2);

		UserTask userTask = modelInstance.getModelElementById("task1");
		SequenceFlow outgoingSequenceFlow = userTask.Outgoing.GetEnumerator().next();
		FlowNode serviceTask = outgoingSequenceFlow.Target;
		userTask.Outgoing.remove(outgoingSequenceFlow);
		userTask.builder().scriptTask().userTask().connectTo(serviceTask.Id);

		assertThat(modelInstance.getModelElementsByType(taskType)).hasSize(4);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateInvoiceProcess()
	  public virtual void testCreateInvoiceProcess()
	  {
		modelInstance = Bpmn.createProcess().executable().startEvent().name("Invoice received").camundaFormKey("embedded:app:forms/start-form.html").userTask().name("Assign Approver").camundaFormKey("embedded:app:forms/assign-approver.html").camundaAssignee("demo").userTask("approveInvoice").name("Approve Invoice").camundaFormKey("embedded:app:forms/approve-invoice.html").camundaAssignee("${approver}").exclusiveGateway().name("Invoice approved?").gatewayDirection(GatewayDirection.Diverging).condition("yes", "${approved}").userTask().name("Prepare Bank Transfer").camundaFormKey("embedded:app:forms/prepare-bank-transfer.html").camundaCandidateGroups("accounting").serviceTask().name("Archive Invoice").camundaClass("org.camunda.bpm.example.invoice.service.ArchiveInvoiceService").endEvent().name("Invoice processed").moveToLastGateway().condition("no", "${!approved}").userTask().name("Review Invoice").camundaFormKey("embedded:app:forms/review-invoice.html").camundaAssignee("demo").exclusiveGateway().name("Review successful?").gatewayDirection(GatewayDirection.Diverging).condition("no", "${!clarified}").endEvent().name("Invoice not processed").moveToLastGateway().condition("yes", "${clarified}").connectTo("approveInvoice").done();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessCamundaExtensions()
	  public virtual void testProcessCamundaExtensions()
	  {
		modelInstance = Bpmn.createProcess(PROCESS_ID).camundaJobPriority("${somePriority}").camundaTaskPriority(TEST_PROCESS_TASK_PRIORITY).camundaHistoryTimeToLive(TEST_HISTORY_TIME_TO_LIVE).camundaStartableInTasklist(TEST_STARTABLE_IN_TASKLIST).camundaVersionTag(TEST_VERSION_TAG).startEvent().endEvent().done();

		Process process = modelInstance.getModelElementById(PROCESS_ID);
		assertThat(process.CamundaJobPriority).isEqualTo("${somePriority}");
		assertThat(process.CamundaTaskPriority).isEqualTo(TEST_PROCESS_TASK_PRIORITY);
		assertThat(process.getCamundaHistoryTimeToLive()).isEqualTo(TEST_HISTORY_TIME_TO_LIVE);
		assertThat(process.CamundaStartableInTasklist).isEqualTo(TEST_STARTABLE_IN_TASKLIST);
		assertThat(process.CamundaVersionTag).isEqualTo(TEST_VERSION_TAG);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessStartableInTasklist()
	  public virtual void testProcessStartableInTasklist()
	  {
		modelInstance = Bpmn.createProcess(PROCESS_ID).startEvent().endEvent().done();

		Process process = modelInstance.getModelElementById(PROCESS_ID);
		assertThat(process.CamundaStartableInTasklist).isEqualTo(true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskCamundaExternalTask()
	  public virtual void testTaskCamundaExternalTask()
	  {
		modelInstance = Bpmn.createProcess().startEvent().serviceTask(EXTERNAL_TASK_ID).camundaExternalTask(TEST_EXTERNAL_TASK_TOPIC).endEvent().done();

		ServiceTask serviceTask = modelInstance.getModelElementById(EXTERNAL_TASK_ID);
		assertThat(serviceTask.CamundaType).isEqualTo("external");
		assertThat(serviceTask.CamundaTopic).isEqualTo(TEST_EXTERNAL_TASK_TOPIC);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskCamundaExtensions()
	  public virtual void testTaskCamundaExtensions()
	  {
		modelInstance = Bpmn.createProcess().startEvent().serviceTask(TASK_ID).camundaAsyncBefore().notCamundaExclusive().camundaJobPriority("${somePriority}").camundaTaskPriority(TEST_SERVICE_TASK_PRIORITY).camundaFailedJobRetryTimeCycle(FAILED_JOB_RETRY_TIME_CYCLE).endEvent().done();

		ServiceTask serviceTask = modelInstance.getModelElementById(TASK_ID);
		assertThat(serviceTask.CamundaAsyncBefore).True;
		assertThat(serviceTask.CamundaExclusive).False;
		assertThat(serviceTask.CamundaJobPriority).isEqualTo("${somePriority}");
		assertThat(serviceTask.CamundaTaskPriority).isEqualTo(TEST_SERVICE_TASK_PRIORITY);

		assertCamundaFailedJobRetryTimeCycle(serviceTask);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testServiceTaskCamundaExtensions()
	  public virtual void testServiceTaskCamundaExtensions()
	  {
		modelInstance = Bpmn.createProcess().startEvent().serviceTask(TASK_ID).camundaClass(TEST_CLASS_API).camundaDelegateExpression(TEST_DELEGATE_EXPRESSION_API).camundaExpression(TEST_EXPRESSION_API).camundaResultVariable(TEST_STRING_API).camundaTopic(TEST_STRING_API).camundaType(TEST_STRING_API).camundaTaskPriority(TEST_SERVICE_TASK_PRIORITY).camundaFailedJobRetryTimeCycle(FAILED_JOB_RETRY_TIME_CYCLE).done();

		ServiceTask serviceTask = modelInstance.getModelElementById(TASK_ID);
		assertThat(serviceTask.CamundaClass).isEqualTo(TEST_CLASS_API);
		assertThat(serviceTask.CamundaDelegateExpression).isEqualTo(TEST_DELEGATE_EXPRESSION_API);
		assertThat(serviceTask.CamundaExpression).isEqualTo(TEST_EXPRESSION_API);
		assertThat(serviceTask.CamundaResultVariable).isEqualTo(TEST_STRING_API);
		assertThat(serviceTask.CamundaTopic).isEqualTo(TEST_STRING_API);
		assertThat(serviceTask.CamundaType).isEqualTo(TEST_STRING_API);
		assertThat(serviceTask.CamundaTaskPriority).isEqualTo(TEST_SERVICE_TASK_PRIORITY);

		assertCamundaFailedJobRetryTimeCycle(serviceTask);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testServiceTaskCamundaClass()
	  public virtual void testServiceTaskCamundaClass()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		modelInstance = Bpmn.createProcess().startEvent().serviceTask(TASK_ID).camundaClass(this.GetType().FullName).done();

		ServiceTask serviceTask = modelInstance.getModelElementById(TASK_ID);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		assertThat(serviceTask.CamundaClass).isEqualTo(this.GetType().FullName);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSendTaskCamundaExtensions()
	  public virtual void testSendTaskCamundaExtensions()
	  {
		modelInstance = Bpmn.createProcess().startEvent().sendTask(TASK_ID).camundaClass(TEST_CLASS_API).camundaDelegateExpression(TEST_DELEGATE_EXPRESSION_API).camundaExpression(TEST_EXPRESSION_API).camundaResultVariable(TEST_STRING_API).camundaTopic(TEST_STRING_API).camundaType(TEST_STRING_API).camundaTaskPriority(TEST_SERVICE_TASK_PRIORITY).camundaFailedJobRetryTimeCycle(FAILED_JOB_RETRY_TIME_CYCLE).endEvent().done();

		SendTask sendTask = modelInstance.getModelElementById(TASK_ID);
		assertThat(sendTask.CamundaClass).isEqualTo(TEST_CLASS_API);
		assertThat(sendTask.CamundaDelegateExpression).isEqualTo(TEST_DELEGATE_EXPRESSION_API);
		assertThat(sendTask.CamundaExpression).isEqualTo(TEST_EXPRESSION_API);
		assertThat(sendTask.CamundaResultVariable).isEqualTo(TEST_STRING_API);
		assertThat(sendTask.CamundaTopic).isEqualTo(TEST_STRING_API);
		assertThat(sendTask.CamundaType).isEqualTo(TEST_STRING_API);
		assertThat(sendTask.CamundaTaskPriority).isEqualTo(TEST_SERVICE_TASK_PRIORITY);

		assertCamundaFailedJobRetryTimeCycle(sendTask);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSendTaskCamundaClass()
	  public virtual void testSendTaskCamundaClass()
	  {
		modelInstance = Bpmn.createProcess().startEvent().sendTask(TASK_ID).camundaClass(this.GetType()).endEvent().done();

		SendTask sendTask = modelInstance.getModelElementById(TASK_ID);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		assertThat(sendTask.CamundaClass).isEqualTo(this.GetType().FullName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUserTaskCamundaExtensions()
	  public virtual void testUserTaskCamundaExtensions()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask(TASK_ID).camundaAssignee(TEST_STRING_API).camundaCandidateGroups(TEST_GROUPS_API).camundaCandidateUsers(TEST_USERS_LIST_API).camundaDueDate(TEST_DUE_DATE_API).camundaFollowUpDate(TEST_FOLLOW_UP_DATE_API).camundaFormHandlerClass(TEST_CLASS_API).camundaFormKey(TEST_STRING_API).camundaPriority(TEST_PRIORITY_API).camundaFailedJobRetryTimeCycle(FAILED_JOB_RETRY_TIME_CYCLE).endEvent().done();

		UserTask userTask = modelInstance.getModelElementById(TASK_ID);
		assertThat(userTask.CamundaAssignee).isEqualTo(TEST_STRING_API);
		assertThat(userTask.CamundaCandidateGroups).isEqualTo(TEST_GROUPS_API);
		assertThat(userTask.CamundaCandidateGroupsList).containsAll(TEST_GROUPS_LIST_API);
		assertThat(userTask.CamundaCandidateUsers).isEqualTo(TEST_USERS_API);
		assertThat(userTask.CamundaCandidateUsersList).containsAll(TEST_USERS_LIST_API);
		assertThat(userTask.CamundaDueDate).isEqualTo(TEST_DUE_DATE_API);
		assertThat(userTask.CamundaFollowUpDate).isEqualTo(TEST_FOLLOW_UP_DATE_API);
		assertThat(userTask.CamundaFormHandlerClass).isEqualTo(TEST_CLASS_API);
		assertThat(userTask.CamundaFormKey).isEqualTo(TEST_STRING_API);
		assertThat(userTask.CamundaPriority).isEqualTo(TEST_PRIORITY_API);

		assertCamundaFailedJobRetryTimeCycle(userTask);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBusinessRuleTaskCamundaExtensions()
	  public virtual void testBusinessRuleTaskCamundaExtensions()
	  {
		modelInstance = Bpmn.createProcess().startEvent().businessRuleTask(TASK_ID).camundaClass(TEST_CLASS_API).camundaDelegateExpression(TEST_DELEGATE_EXPRESSION_API).camundaExpression(TEST_EXPRESSION_API).camundaResultVariable("resultVar").camundaTopic("topic").camundaType("type").camundaDecisionRef("decisionRef").camundaDecisionRefBinding("latest").camundaDecisionRefVersion("7").camundaDecisionRefVersionTag("0.1.0").camundaDecisionRefTenantId("tenantId").camundaMapDecisionResult("singleEntry").camundaTaskPriority(TEST_SERVICE_TASK_PRIORITY).camundaFailedJobRetryTimeCycle(FAILED_JOB_RETRY_TIME_CYCLE).endEvent().done();

		BusinessRuleTask businessRuleTask = modelInstance.getModelElementById(TASK_ID);
		assertThat(businessRuleTask.CamundaClass).isEqualTo(TEST_CLASS_API);
		assertThat(businessRuleTask.CamundaDelegateExpression).isEqualTo(TEST_DELEGATE_EXPRESSION_API);
		assertThat(businessRuleTask.CamundaExpression).isEqualTo(TEST_EXPRESSION_API);
		assertThat(businessRuleTask.CamundaResultVariable).isEqualTo("resultVar");
		assertThat(businessRuleTask.CamundaTopic).isEqualTo("topic");
		assertThat(businessRuleTask.CamundaType).isEqualTo("type");
		assertThat(businessRuleTask.CamundaDecisionRef).isEqualTo("decisionRef");
		assertThat(businessRuleTask.CamundaDecisionRefBinding).isEqualTo("latest");
		assertThat(businessRuleTask.CamundaDecisionRefVersion).isEqualTo("7");
		assertThat(businessRuleTask.CamundaDecisionRefVersionTag).isEqualTo("0.1.0");
		assertThat(businessRuleTask.CamundaDecisionRefTenantId).isEqualTo("tenantId");
		assertThat(businessRuleTask.CamundaMapDecisionResult).isEqualTo("singleEntry");
		assertThat(businessRuleTask.CamundaTaskPriority).isEqualTo(TEST_SERVICE_TASK_PRIORITY);

		assertCamundaFailedJobRetryTimeCycle(businessRuleTask);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBusinessRuleTaskCamundaClass()
	  public virtual void testBusinessRuleTaskCamundaClass()
	  {
		modelInstance = Bpmn.createProcess().startEvent().businessRuleTask(TASK_ID).camundaClass(typeof(Bpmn)).endEvent().done();

		BusinessRuleTask businessRuleTask = modelInstance.getModelElementById(TASK_ID);
		assertThat(businessRuleTask.CamundaClass).isEqualTo("org.camunda.bpm.model.bpmn.Bpmn");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testScriptTaskCamundaExtensions()
	  public virtual void testScriptTaskCamundaExtensions()
	  {
		modelInstance = Bpmn.createProcess().startEvent().scriptTask(TASK_ID).camundaResultVariable(TEST_STRING_API).camundaResource(TEST_STRING_API).camundaFailedJobRetryTimeCycle(FAILED_JOB_RETRY_TIME_CYCLE).endEvent().done();

		ScriptTask scriptTask = modelInstance.getModelElementById(TASK_ID);
		assertThat(scriptTask.CamundaResultVariable).isEqualTo(TEST_STRING_API);
		assertThat(scriptTask.CamundaResource).isEqualTo(TEST_STRING_API);

		assertCamundaFailedJobRetryTimeCycle(scriptTask);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStartEventCamundaExtensions()
	  public virtual void testStartEventCamundaExtensions()
	  {
		modelInstance = Bpmn.createProcess().startEvent(START_EVENT_ID).camundaAsyncBefore().notCamundaExclusive().camundaFormHandlerClass(TEST_CLASS_API).camundaFormKey(TEST_STRING_API).camundaInitiator(TEST_STRING_API).camundaFailedJobRetryTimeCycle(FAILED_JOB_RETRY_TIME_CYCLE).done();

		StartEvent startEvent = modelInstance.getModelElementById(START_EVENT_ID);
		assertThat(startEvent.CamundaAsyncBefore).True;
		assertThat(startEvent.CamundaExclusive).False;
		assertThat(startEvent.CamundaFormHandlerClass).isEqualTo(TEST_CLASS_API);
		assertThat(startEvent.CamundaFormKey).isEqualTo(TEST_STRING_API);
		assertThat(startEvent.CamundaInitiator).isEqualTo(TEST_STRING_API);

		assertCamundaFailedJobRetryTimeCycle(startEvent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testErrorDefinitionsForStartEvent()
	  public virtual void testErrorDefinitionsForStartEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent("start").errorEventDefinition("event").errorCodeVariable("errorCodeVariable").errorMessageVariable("errorMessageVariable").error("errorCode").errorEventDefinitionDone().endEvent().done();

		assertErrorEventDefinition("start", "errorCode");
		assertErrorEventDefinitionForErrorVariables("start", "errorCodeVariable", "errorMessageVariable");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testErrorDefinitionsForStartEventWithoutEventDefinitionId()
	  public virtual void testErrorDefinitionsForStartEventWithoutEventDefinitionId()
	  {
		modelInstance = Bpmn.createProcess().startEvent("start").errorEventDefinition().errorCodeVariable("errorCodeVariable").errorMessageVariable("errorMessageVariable").error("errorCode").errorEventDefinitionDone().endEvent().done();

		assertErrorEventDefinition("start", "errorCode");
		assertErrorEventDefinitionForErrorVariables("start", "errorCodeVariable", "errorMessageVariable");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCallActivityCamundaExtension()
	  public virtual void testCallActivityCamundaExtension()
	  {
		modelInstance = Bpmn.createProcess().startEvent().callActivity(CALL_ACTIVITY_ID).calledElement(TEST_STRING_API).camundaAsyncBefore().camundaCalledElementBinding("version").camundaCalledElementVersion("1.0").camundaCalledElementVersionTag("ver-1.0").camundaCalledElementTenantId("t1").camundaCaseRef("case").camundaCaseBinding("deployment").camundaCaseVersion("2").camundaCaseTenantId("t2").camundaIn("in-source", "in-target").camundaOut("out-source", "out-target").camundaVariableMappingClass(TEST_CLASS_API).camundaVariableMappingDelegateExpression(TEST_DELEGATE_EXPRESSION_API).notCamundaExclusive().camundaFailedJobRetryTimeCycle(FAILED_JOB_RETRY_TIME_CYCLE).endEvent().done();

		CallActivity callActivity = modelInstance.getModelElementById(CALL_ACTIVITY_ID);
		assertThat(callActivity.CalledElement).isEqualTo(TEST_STRING_API);
		assertThat(callActivity.CamundaAsyncBefore).True;
		assertThat(callActivity.CamundaCalledElementBinding).isEqualTo("version");
		assertThat(callActivity.CamundaCalledElementVersion).isEqualTo("1.0");
		assertThat(callActivity.CamundaCalledElementVersionTag).isEqualTo("ver-1.0");
		assertThat(callActivity.CamundaCalledElementTenantId).isEqualTo("t1");
		assertThat(callActivity.CamundaCaseRef).isEqualTo("case");
		assertThat(callActivity.CamundaCaseBinding).isEqualTo("deployment");
		assertThat(callActivity.CamundaCaseVersion).isEqualTo("2");
		assertThat(callActivity.CamundaCaseTenantId).isEqualTo("t2");
		assertThat(callActivity.CamundaExclusive).False;

		CamundaIn camundaIn = (CamundaIn) callActivity.ExtensionElements.getUniqueChildElementByType(typeof(CamundaIn));
		assertThat(camundaIn.CamundaSource).isEqualTo("in-source");
		assertThat(camundaIn.CamundaTarget).isEqualTo("in-target");

		CamundaOut camundaOut = (CamundaOut) callActivity.ExtensionElements.getUniqueChildElementByType(typeof(CamundaOut));
		assertThat(camundaOut.CamundaSource).isEqualTo("out-source");
		assertThat(camundaOut.CamundaTarget).isEqualTo("out-target");

		assertThat(callActivity.CamundaVariableMappingClass).isEqualTo(TEST_CLASS_API);
		assertThat(callActivity.CamundaVariableMappingDelegateExpression).isEqualTo(TEST_DELEGATE_EXPRESSION_API);
		assertCamundaFailedJobRetryTimeCycle(callActivity);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCallActivityCamundaVariableMappingClass()
	  public virtual void testCallActivityCamundaVariableMappingClass()
	  {
		modelInstance = Bpmn.createProcess().startEvent().callActivity(CALL_ACTIVITY_ID).camundaVariableMappingClass(this.GetType()).endEvent().done();

		CallActivity callActivity = modelInstance.getModelElementById(CALL_ACTIVITY_ID);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		assertThat(callActivity.CamundaVariableMappingClass).isEqualTo(this.GetType().FullName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubProcessBuilder()
	  public virtual void testSubProcessBuilder()
	  {
		BpmnModelInstance modelInstance = Bpmn.createProcess().startEvent().subProcess(SUB_PROCESS_ID).camundaAsyncBefore().embeddedSubProcess().startEvent().userTask().endEvent().subProcessDone().serviceTask(SERVICE_TASK_ID).endEvent().done();

		SubProcess subProcess = modelInstance.getModelElementById(SUB_PROCESS_ID);
		ServiceTask serviceTask = modelInstance.getModelElementById(SERVICE_TASK_ID);
		assertThat(subProcess.CamundaAsyncBefore).True;
		assertThat(subProcess.CamundaExclusive).True;
		assertThat(subProcess.getChildElementsByType(typeof(Event))).hasSize(2);
		assertThat(subProcess.getChildElementsByType(typeof(Task))).hasSize(1);
		assertThat(subProcess.FlowElements).hasSize(5);
		assertThat(subProcess.SucceedingNodes.singleResult()).isEqualTo(serviceTask);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubProcessBuilderDetached()
	  public virtual void testSubProcessBuilderDetached()
	  {
		modelInstance = Bpmn.createProcess().startEvent().subProcess(SUB_PROCESS_ID).serviceTask(SERVICE_TASK_ID).endEvent().done();

		SubProcess subProcess = modelInstance.getModelElementById(SUB_PROCESS_ID);

		subProcess.builder().camundaAsyncBefore().embeddedSubProcess().startEvent().userTask().endEvent();

		ServiceTask serviceTask = modelInstance.getModelElementById(SERVICE_TASK_ID);
		assertThat(subProcess.CamundaAsyncBefore).True;
		assertThat(subProcess.CamundaExclusive).True;
		assertThat(subProcess.getChildElementsByType(typeof(Event))).hasSize(2);
		assertThat(subProcess.getChildElementsByType(typeof(Task))).hasSize(1);
		assertThat(subProcess.FlowElements).hasSize(5);
		assertThat(subProcess.SucceedingNodes.singleResult()).isEqualTo(serviceTask);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubProcessBuilderNested()
	  public virtual void testSubProcessBuilderNested()
	  {
		modelInstance = Bpmn.createProcess().startEvent().subProcess(SUB_PROCESS_ID + 1).camundaAsyncBefore().embeddedSubProcess().startEvent().userTask().subProcess(SUB_PROCESS_ID + 2).camundaAsyncBefore().notCamundaExclusive().embeddedSubProcess().startEvent().userTask().endEvent().subProcessDone().serviceTask(SERVICE_TASK_ID + 1).endEvent().subProcessDone().serviceTask(SERVICE_TASK_ID + 2).endEvent().done();

		SubProcess subProcess = modelInstance.getModelElementById(SUB_PROCESS_ID + 1);
		ServiceTask serviceTask = modelInstance.getModelElementById(SERVICE_TASK_ID + 2);
		assertThat(subProcess.CamundaAsyncBefore).True;
		assertThat(subProcess.CamundaExclusive).True;
		assertThat(subProcess.getChildElementsByType(typeof(Event))).hasSize(2);
		assertThat(subProcess.getChildElementsByType(typeof(Task))).hasSize(2);
		assertThat(subProcess.getChildElementsByType(typeof(SubProcess))).hasSize(1);
		assertThat(subProcess.FlowElements).hasSize(9);
		assertThat(subProcess.SucceedingNodes.singleResult()).isEqualTo(serviceTask);

		SubProcess nestedSubProcess = modelInstance.getModelElementById(SUB_PROCESS_ID + 2);
		ServiceTask nestedServiceTask = modelInstance.getModelElementById(SERVICE_TASK_ID + 1);
		assertThat(nestedSubProcess.CamundaAsyncBefore).True;
		assertThat(nestedSubProcess.CamundaExclusive).False;
		assertThat(nestedSubProcess.getChildElementsByType(typeof(Event))).hasSize(2);
		assertThat(nestedSubProcess.getChildElementsByType(typeof(Task))).hasSize(1);
		assertThat(nestedSubProcess.FlowElements).hasSize(5);
		assertThat(nestedSubProcess.SucceedingNodes.singleResult()).isEqualTo(nestedServiceTask);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubProcessBuilderWrongScope()
	  public virtual void testSubProcessBuilderWrongScope()
	  {
		try
		{
		  modelInstance = Bpmn.createProcess().startEvent().subProcessDone().endEvent().done();
		  fail("Exception expected");
		}
		catch (Exception e)
		{
		  assertThat(e).isInstanceOf(typeof(BpmnModelException));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTransactionBuilder()
	  public virtual void testTransactionBuilder()
	  {
		BpmnModelInstance modelInstance = Bpmn.createProcess().startEvent().transaction(TRANSACTION_ID).camundaAsyncBefore().method(TransactionMethod.Image).embeddedSubProcess().startEvent().userTask().endEvent().transactionDone().serviceTask(SERVICE_TASK_ID).endEvent().done();

		Transaction transaction = modelInstance.getModelElementById(TRANSACTION_ID);
		ServiceTask serviceTask = modelInstance.getModelElementById(SERVICE_TASK_ID);
		assertThat(transaction.CamundaAsyncBefore).True;
		assertThat(transaction.CamundaExclusive).True;
		assertThat(transaction.Method).isEqualTo(TransactionMethod.Image);
		assertThat(transaction.getChildElementsByType(typeof(Event))).hasSize(2);
		assertThat(transaction.getChildElementsByType(typeof(Task))).hasSize(1);
		assertThat(transaction.FlowElements).hasSize(5);
		assertThat(transaction.SucceedingNodes.singleResult()).isEqualTo(serviceTask);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTransactionBuilderDetached()
	  public virtual void testTransactionBuilderDetached()
	  {
		modelInstance = Bpmn.createProcess().startEvent().transaction(TRANSACTION_ID).serviceTask(SERVICE_TASK_ID).endEvent().done();

		Transaction transaction = modelInstance.getModelElementById(TRANSACTION_ID);

		transaction.builder().camundaAsyncBefore().embeddedSubProcess().startEvent().userTask().endEvent();

		ServiceTask serviceTask = modelInstance.getModelElementById(SERVICE_TASK_ID);
		assertThat(transaction.CamundaAsyncBefore).True;
		assertThat(transaction.CamundaExclusive).True;
		assertThat(transaction.getChildElementsByType(typeof(Event))).hasSize(2);
		assertThat(transaction.getChildElementsByType(typeof(Task))).hasSize(1);
		assertThat(transaction.FlowElements).hasSize(5);
		assertThat(transaction.SucceedingNodes.singleResult()).isEqualTo(serviceTask);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testScriptText()
	  public virtual void testScriptText()
	  {
		modelInstance = Bpmn.createProcess().startEvent().scriptTask("script").scriptFormat("groovy").scriptText("println \"hello, world\";").endEvent().done();

		ScriptTask scriptTask = modelInstance.getModelElementById("script");
		assertThat(scriptTask.ScriptFormat).isEqualTo("groovy");
		assertThat(scriptTask.Script.TextContent).isEqualTo("println \"hello, world\";");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEventBasedGatewayAsyncAfter()
	  public virtual void testEventBasedGatewayAsyncAfter()
	  {
		try
		{
		  modelInstance = Bpmn.createProcess().startEvent().eventBasedGateway().camundaAsyncAfter().done();

		  fail("Expected UnsupportedOperationException");
		}
		catch (System.NotSupportedException)
		{
		  // happy path
		}

		try
		{
		  modelInstance = Bpmn.createProcess().startEvent().eventBasedGateway().camundaAsyncAfter(true).endEvent().done();
		  fail("Expected UnsupportedOperationException");
		}
		catch (System.NotSupportedException)
		{
		  // happy ending :D
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMessageStartEvent()
	  public virtual void testMessageStartEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent("start").message("message").done();

		assertMessageEventDefinition("start", "message");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMessageStartEventWithExistingMessage()
	  public virtual void testMessageStartEventWithExistingMessage()
	  {
		modelInstance = Bpmn.createProcess().startEvent("start").message("message").subProcess().triggerByEvent().embeddedSubProcess().startEvent("subStart").message("message").subProcessDone().done();

		Message message = assertMessageEventDefinition("start", "message");
		Message subMessage = assertMessageEventDefinition("subStart", "message");

		assertThat(message).isEqualTo(subMessage);

		assertOnlyOneMessageExists("message");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIntermediateMessageCatchEvent()
	  public virtual void testIntermediateMessageCatchEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent().intermediateCatchEvent("catch").message("message").done();

		assertMessageEventDefinition("catch", "message");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIntermediateMessageCatchEventWithExistingMessage()
	  public virtual void testIntermediateMessageCatchEventWithExistingMessage()
	  {
		modelInstance = Bpmn.createProcess().startEvent().intermediateCatchEvent("catch1").message("message").intermediateCatchEvent("catch2").message("message").done();

		Message message1 = assertMessageEventDefinition("catch1", "message");
		Message message2 = assertMessageEventDefinition("catch2", "message");

		assertThat(message1).isEqualTo(message2);

		assertOnlyOneMessageExists("message");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMessageEndEvent()
	  public virtual void testMessageEndEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent().endEvent("end").message("message").done();

		assertMessageEventDefinition("end", "message");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMessageEventDefintionEndEvent()
	  public virtual void testMessageEventDefintionEndEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent().endEvent("end").messageEventDefinition().message("message").done();

		assertMessageEventDefinition("end", "message");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMessageEndEventWithExistingMessage()
	  public virtual void testMessageEndEventWithExistingMessage()
	  {
		modelInstance = Bpmn.createProcess().startEvent().parallelGateway().endEvent("end1").message("message").moveToLastGateway().endEvent("end2").message("message").done();

		Message message1 = assertMessageEventDefinition("end1", "message");
		Message message2 = assertMessageEventDefinition("end2", "message");

		assertThat(message1).isEqualTo(message2);

		assertOnlyOneMessageExists("message");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMessageEventDefinitionEndEventWithExistingMessage()
	  public virtual void testMessageEventDefinitionEndEventWithExistingMessage()
	  {
		modelInstance = Bpmn.createProcess().startEvent().parallelGateway().endEvent("end1").messageEventDefinition().message("message").messageEventDefinitionDone().moveToLastGateway().endEvent("end2").messageEventDefinition().message("message").done();

		Message message1 = assertMessageEventDefinition("end1", "message");
		Message message2 = assertMessageEventDefinition("end2", "message");

		assertThat(message1).isEqualTo(message2);

		assertOnlyOneMessageExists("message");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIntermediateMessageThrowEvent()
	  public virtual void testIntermediateMessageThrowEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent().intermediateThrowEvent("throw").message("message").done();

		assertMessageEventDefinition("throw", "message");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIntermediateMessageEventDefintionThrowEvent()
	  public virtual void testIntermediateMessageEventDefintionThrowEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent().intermediateThrowEvent("throw").messageEventDefinition().message("message").done();

		assertMessageEventDefinition("throw", "message");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIntermediateMessageThrowEventWithExistingMessage()
	  public virtual void testIntermediateMessageThrowEventWithExistingMessage()
	  {
		modelInstance = Bpmn.createProcess().startEvent().intermediateThrowEvent("throw1").message("message").intermediateThrowEvent("throw2").message("message").done();

		Message message1 = assertMessageEventDefinition("throw1", "message");
		Message message2 = assertMessageEventDefinition("throw2", "message");

		assertThat(message1).isEqualTo(message2);
		assertOnlyOneMessageExists("message");
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIntermediateMessageEventDefintionThrowEventWithExistingMessage()
	  public virtual void testIntermediateMessageEventDefintionThrowEventWithExistingMessage()
	  {
		modelInstance = Bpmn.createProcess().startEvent().intermediateThrowEvent("throw1").messageEventDefinition().message("message").messageEventDefinitionDone().intermediateThrowEvent("throw2").messageEventDefinition().message("message").messageEventDefinitionDone().done();

		Message message1 = assertMessageEventDefinition("throw1", "message");
		Message message2 = assertMessageEventDefinition("throw2", "message");

		assertThat(message1).isEqualTo(message2);
		assertOnlyOneMessageExists("message");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIntermediateMessageThrowEventWithMessageDefinition()
	  public virtual void testIntermediateMessageThrowEventWithMessageDefinition()
	  {
		modelInstance = Bpmn.createProcess().startEvent().intermediateThrowEvent("throw1").messageEventDefinition().id("messageEventDefinition").message("message").camundaTaskPriority(TEST_SERVICE_TASK_PRIORITY).camundaType("external").camundaTopic("TOPIC").done();

		MessageEventDefinition @event = modelInstance.getModelElementById("messageEventDefinition");
		assertThat(@event.CamundaTaskPriority).isEqualTo(TEST_SERVICE_TASK_PRIORITY);
		assertThat(@event.CamundaTopic).isEqualTo("TOPIC");
		assertThat(@event.CamundaType).isEqualTo("external");
		assertThat(@event.Message.Name).isEqualTo("message");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIntermediateMessageThrowEventWithTaskPriority()
	  public virtual void testIntermediateMessageThrowEventWithTaskPriority()
	  {
		modelInstance = Bpmn.createProcess().startEvent().intermediateThrowEvent("throw1").messageEventDefinition("messageEventDefinition").camundaTaskPriority(TEST_SERVICE_TASK_PRIORITY).done();

		MessageEventDefinition @event = modelInstance.getModelElementById("messageEventDefinition");
		assertThat(@event.CamundaTaskPriority).isEqualTo(TEST_SERVICE_TASK_PRIORITY);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEndEventWithTaskPriority()
	  public virtual void testEndEventWithTaskPriority()
	  {
		modelInstance = Bpmn.createProcess().startEvent().endEvent("end").messageEventDefinition("messageEventDefinition").camundaTaskPriority(TEST_SERVICE_TASK_PRIORITY).done();

		MessageEventDefinition @event = modelInstance.getModelElementById("messageEventDefinition");
		assertThat(@event.CamundaTaskPriority).isEqualTo(TEST_SERVICE_TASK_PRIORITY);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMessageEventDefinitionWithID()
	  public virtual void testMessageEventDefinitionWithID()
	  {
		modelInstance = Bpmn.createProcess().startEvent().intermediateThrowEvent("throw1").messageEventDefinition("messageEventDefinition").done();

		MessageEventDefinition @event = modelInstance.getModelElementById("messageEventDefinition");
		assertThat(@event).NotNull;

		modelInstance = Bpmn.createProcess().startEvent().intermediateThrowEvent("throw2").messageEventDefinition().id("messageEventDefinition1").done();

		//========================================
		//==============end event=================
		//========================================
		@event = modelInstance.getModelElementById("messageEventDefinition1");
		assertThat(@event).NotNull;
		modelInstance = Bpmn.createProcess().startEvent().endEvent("end1").messageEventDefinition("messageEventDefinition").done();

		@event = modelInstance.getModelElementById("messageEventDefinition");
		assertThat(@event).NotNull;

		modelInstance = Bpmn.createProcess().startEvent().endEvent("end2").messageEventDefinition().id("messageEventDefinition1").done();

		@event = modelInstance.getModelElementById("messageEventDefinition1");
		assertThat(@event).NotNull;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReceiveTaskMessage()
	  public virtual void testReceiveTaskMessage()
	  {
		modelInstance = Bpmn.createProcess().startEvent().receiveTask("receive").message("message").done();

		ReceiveTask receiveTask = modelInstance.getModelElementById("receive");

		Message message = receiveTask.Message;
		assertThat(message).NotNull;
		assertThat(message.Name).isEqualTo("message");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReceiveTaskWithExistingMessage()
	  public virtual void testReceiveTaskWithExistingMessage()
	  {
		modelInstance = Bpmn.createProcess().startEvent().receiveTask("receive1").message("message").receiveTask("receive2").message("message").done();

		ReceiveTask receiveTask1 = modelInstance.getModelElementById("receive1");
		Message message1 = receiveTask1.Message;

		ReceiveTask receiveTask2 = modelInstance.getModelElementById("receive2");
		Message message2 = receiveTask2.Message;

		assertThat(message1).isEqualTo(message2);

		assertOnlyOneMessageExists("message");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSendTaskMessage()
	  public virtual void testSendTaskMessage()
	  {
		modelInstance = Bpmn.createProcess().startEvent().sendTask("send").message("message").done();

		SendTask sendTask = modelInstance.getModelElementById("send");

		Message message = sendTask.Message;
		assertThat(message).NotNull;
		assertThat(message.Name).isEqualTo("message");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSendTaskWithExistingMessage()
	  public virtual void testSendTaskWithExistingMessage()
	  {
		modelInstance = Bpmn.createProcess().startEvent().sendTask("send1").message("message").sendTask("send2").message("message").done();

		SendTask sendTask1 = modelInstance.getModelElementById("send1");
		Message message1 = sendTask1.Message;

		SendTask sendTask2 = modelInstance.getModelElementById("send2");
		Message message2 = sendTask2.Message;

		assertThat(message1).isEqualTo(message2);

		assertOnlyOneMessageExists("message");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSignalStartEvent()
	  public virtual void testSignalStartEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent("start").signal("signal").done();

		assertSignalEventDefinition("start", "signal");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSignalStartEventWithExistingSignal()
	  public virtual void testSignalStartEventWithExistingSignal()
	  {
		modelInstance = Bpmn.createProcess().startEvent("start").signal("signal").subProcess().triggerByEvent().embeddedSubProcess().startEvent("subStart").signal("signal").subProcessDone().done();

		Signal signal = assertSignalEventDefinition("start", "signal");
		Signal subSignal = assertSignalEventDefinition("subStart", "signal");

		assertThat(signal).isEqualTo(subSignal);

		assertOnlyOneSignalExists("signal");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIntermediateSignalCatchEvent()
	  public virtual void testIntermediateSignalCatchEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent().intermediateCatchEvent("catch").signal("signal").done();

		assertSignalEventDefinition("catch", "signal");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIntermediateSignalCatchEventWithExistingSignal()
	  public virtual void testIntermediateSignalCatchEventWithExistingSignal()
	  {
		modelInstance = Bpmn.createProcess().startEvent().intermediateCatchEvent("catch1").signal("signal").intermediateCatchEvent("catch2").signal("signal").done();

		Signal signal1 = assertSignalEventDefinition("catch1", "signal");
		Signal signal2 = assertSignalEventDefinition("catch2", "signal");

		assertThat(signal1).isEqualTo(signal2);

		assertOnlyOneSignalExists("signal");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSignalEndEvent()
	  public virtual void testSignalEndEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent().endEvent("end").signal("signal").done();

		assertSignalEventDefinition("end", "signal");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSignalEndEventWithExistingSignal()
	  public virtual void testSignalEndEventWithExistingSignal()
	  {
		modelInstance = Bpmn.createProcess().startEvent().parallelGateway().endEvent("end1").signal("signal").moveToLastGateway().endEvent("end2").signal("signal").done();

		Signal signal1 = assertSignalEventDefinition("end1", "signal");
		Signal signal2 = assertSignalEventDefinition("end2", "signal");

		assertThat(signal1).isEqualTo(signal2);

		assertOnlyOneSignalExists("signal");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIntermediateSignalThrowEvent()
	  public virtual void testIntermediateSignalThrowEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent().intermediateThrowEvent("throw").signal("signal").done();

		assertSignalEventDefinition("throw", "signal");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIntermediateSignalThrowEventWithExistingSignal()
	  public virtual void testIntermediateSignalThrowEventWithExistingSignal()
	  {
		modelInstance = Bpmn.createProcess().startEvent().intermediateThrowEvent("throw1").signal("signal").intermediateThrowEvent("throw2").signal("signal").done();

		Signal signal1 = assertSignalEventDefinition("throw1", "signal");
		Signal signal2 = assertSignalEventDefinition("throw2", "signal");

		assertThat(signal1).isEqualTo(signal2);

		assertOnlyOneSignalExists("signal");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIntermediateSignalThrowEventWithPayloadLocalVar()
	  public virtual void testIntermediateSignalThrowEventWithPayloadLocalVar()
	  {
		modelInstance = Bpmn.createProcess().startEvent().intermediateThrowEvent("throw").signalEventDefinition("signal").camundaInSourceTarget("source", "target1").camundaInSourceExpressionTarget("${'sourceExpression'}", "target2").camundaInAllVariables("all", true).camundaInBusinessKey("aBusinessKey").throwEventDefinitionDone().endEvent().done();

		assertSignalEventDefinition("throw", "signal");
		SignalEventDefinition signalEventDefinition = assertAndGetSingleEventDefinition("throw", typeof(SignalEventDefinition));

		assertThat(signalEventDefinition.Signal.Name).isEqualTo("signal");

		IList<CamundaIn> camundaInParams = signalEventDefinition.ExtensionElements.ElementsQuery.filterByType(typeof(CamundaIn)).list();
		assertThat(camundaInParams.Count).isEqualTo(4);

		int paramCounter = 0;
		foreach (CamundaIn inParam in camundaInParams)
		{
		  if (!string.ReferenceEquals(inParam.CamundaVariables, null))
		  {
			assertThat(inParam.CamundaVariables).isEqualTo("all");
			if (inParam.CamundaLocal)
			{
			  paramCounter++;
			}
		  }
		  else if (!string.ReferenceEquals(inParam.CamundaBusinessKey, null))
		  {
			assertThat(inParam.CamundaBusinessKey).isEqualTo("aBusinessKey");
			paramCounter++;
		  }
		  else if (!string.ReferenceEquals(inParam.CamundaSourceExpression, null))
		  {
			assertThat(inParam.CamundaSourceExpression).isEqualTo("${'sourceExpression'}");
			assertThat(inParam.CamundaTarget).isEqualTo("target2");
			paramCounter++;
		  }
		  else if (!string.ReferenceEquals(inParam.CamundaSource, null))
		  {
			assertThat(inParam.CamundaSource).isEqualTo("source");
			assertThat(inParam.CamundaTarget).isEqualTo("target1");
			paramCounter++;
		  }
		}
		assertThat(paramCounter).isEqualTo(camundaInParams.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIntermediateSignalThrowEventWithPayload()
	  public virtual void testIntermediateSignalThrowEventWithPayload()
	  {
		modelInstance = Bpmn.createProcess().startEvent().intermediateThrowEvent("throw").signalEventDefinition("signal").camundaInAllVariables("all").throwEventDefinitionDone().endEvent().done();

		SignalEventDefinition signalEventDefinition = assertAndGetSingleEventDefinition("throw", typeof(SignalEventDefinition));

		IList<CamundaIn> camundaInParams = signalEventDefinition.ExtensionElements.ElementsQuery.filterByType(typeof(CamundaIn)).list();
		assertThat(camundaInParams.Count).isEqualTo(1);

		assertThat(camundaInParams[0].CamundaVariables).isEqualTo("all");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMessageBoundaryEvent()
	  public virtual void testMessageBoundaryEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask("task").endEvent().moveToActivity("task").boundaryEvent("boundary").message("message").endEvent("boundaryEnd").done();

		assertMessageEventDefinition("boundary", "message");

		UserTask userTask = modelInstance.getModelElementById("task");
		BoundaryEvent boundaryEvent = modelInstance.getModelElementById("boundary");
		EndEvent boundaryEnd = modelInstance.getModelElementById("boundaryEnd");

		// boundary event is attached to the user task
		assertThat(boundaryEvent.AttachedTo).isEqualTo(userTask);

		// boundary event has no incoming sequence flows
		assertThat(boundaryEvent.Incoming).Empty;

		// the next flow node is the boundary end event
		IList<FlowNode> succeedingNodes = boundaryEvent.SucceedingNodes.list();
		assertThat(succeedingNodes).containsOnly(boundaryEnd);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMultipleBoundaryEvents()
	  public virtual void testMultipleBoundaryEvents()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask("task").endEvent().moveToActivity("task").boundaryEvent("boundary1").message("message").endEvent("boundaryEnd1").moveToActivity("task").boundaryEvent("boundary2").signal("signal").endEvent("boundaryEnd2").done();

		assertMessageEventDefinition("boundary1", "message");
		assertSignalEventDefinition("boundary2", "signal");

		UserTask userTask = modelInstance.getModelElementById("task");
		BoundaryEvent boundaryEvent1 = modelInstance.getModelElementById("boundary1");
		EndEvent boundaryEnd1 = modelInstance.getModelElementById("boundaryEnd1");
		BoundaryEvent boundaryEvent2 = modelInstance.getModelElementById("boundary2");
		EndEvent boundaryEnd2 = modelInstance.getModelElementById("boundaryEnd2");

		// boundary events are attached to the user task
		assertThat(boundaryEvent1.AttachedTo).isEqualTo(userTask);
		assertThat(boundaryEvent2.AttachedTo).isEqualTo(userTask);

		// boundary events have no incoming sequence flows
		assertThat(boundaryEvent1.Incoming).Empty;
		assertThat(boundaryEvent2.Incoming).Empty;

		// the next flow node is the boundary end event
		IList<FlowNode> succeedingNodes = boundaryEvent1.SucceedingNodes.list();
		assertThat(succeedingNodes).containsOnly(boundaryEnd1);
		succeedingNodes = boundaryEvent2.SucceedingNodes.list();
		assertThat(succeedingNodes).containsOnly(boundaryEnd2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCamundaTaskListenerByClassName()
	  public virtual void testCamundaTaskListenerByClassName()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask("task").camundaTaskListenerClass("start", "aClass").endEvent().done();

		UserTask userTask = modelInstance.getModelElementById("task");
		ExtensionElements extensionElements = userTask.ExtensionElements;
		ICollection<CamundaTaskListener> taskListeners = extensionElements.getChildElementsByType(typeof(CamundaTaskListener));
		assertThat(taskListeners).hasSize(1);

		CamundaTaskListener taskListener = taskListeners.GetEnumerator().next();
		assertThat(taskListener.CamundaClass).isEqualTo("aClass");
		assertThat(taskListener.CamundaEvent).isEqualTo("start");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCamundaTaskListenerByClass()
	  public virtual void testCamundaTaskListenerByClass()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask("task").camundaTaskListenerClass("start", this.GetType()).endEvent().done();

		UserTask userTask = modelInstance.getModelElementById("task");
		ExtensionElements extensionElements = userTask.ExtensionElements;
		ICollection<CamundaTaskListener> taskListeners = extensionElements.getChildElementsByType(typeof(CamundaTaskListener));
		assertThat(taskListeners).hasSize(1);

		CamundaTaskListener taskListener = taskListeners.GetEnumerator().next();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		assertThat(taskListener.CamundaClass).isEqualTo(this.GetType().FullName);
		assertThat(taskListener.CamundaEvent).isEqualTo("start");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCamundaTaskListenerByExpression()
	  public virtual void testCamundaTaskListenerByExpression()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask("task").camundaTaskListenerExpression("start", "anExpression").endEvent().done();

		UserTask userTask = modelInstance.getModelElementById("task");
		ExtensionElements extensionElements = userTask.ExtensionElements;
		ICollection<CamundaTaskListener> taskListeners = extensionElements.getChildElementsByType(typeof(CamundaTaskListener));
		assertThat(taskListeners).hasSize(1);

		CamundaTaskListener taskListener = taskListeners.GetEnumerator().next();
		assertThat(taskListener.CamundaExpression).isEqualTo("anExpression");
		assertThat(taskListener.CamundaEvent).isEqualTo("start");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCamundaTaskListenerByDelegateExpression()
	  public virtual void testCamundaTaskListenerByDelegateExpression()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask("task").camundaTaskListenerDelegateExpression("start", "aDelegate").endEvent().done();

		UserTask userTask = modelInstance.getModelElementById("task");
		ExtensionElements extensionElements = userTask.ExtensionElements;
		ICollection<CamundaTaskListener> taskListeners = extensionElements.getChildElementsByType(typeof(CamundaTaskListener));
		assertThat(taskListeners).hasSize(1);

		CamundaTaskListener taskListener = taskListeners.GetEnumerator().next();
		assertThat(taskListener.CamundaDelegateExpression).isEqualTo("aDelegate");
		assertThat(taskListener.CamundaEvent).isEqualTo("start");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCamundaExecutionListenerByClassName()
	  public virtual void testCamundaExecutionListenerByClassName()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask("task").camundaExecutionListenerClass("start", "aClass").endEvent().done();

		UserTask userTask = modelInstance.getModelElementById("task");
		ExtensionElements extensionElements = userTask.ExtensionElements;
		ICollection<CamundaExecutionListener> executionListeners = extensionElements.getChildElementsByType(typeof(CamundaExecutionListener));
		assertThat(executionListeners).hasSize(1);

		CamundaExecutionListener executionListener = executionListeners.GetEnumerator().next();
		assertThat(executionListener.CamundaClass).isEqualTo("aClass");
		assertThat(executionListener.CamundaEvent).isEqualTo("start");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCamundaExecutionListenerByClass()
	  public virtual void testCamundaExecutionListenerByClass()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask("task").camundaExecutionListenerClass("start", this.GetType()).endEvent().done();

		UserTask userTask = modelInstance.getModelElementById("task");
		ExtensionElements extensionElements = userTask.ExtensionElements;
		ICollection<CamundaExecutionListener> executionListeners = extensionElements.getChildElementsByType(typeof(CamundaExecutionListener));
		assertThat(executionListeners).hasSize(1);

		CamundaExecutionListener executionListener = executionListeners.GetEnumerator().next();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		assertThat(executionListener.CamundaClass).isEqualTo(this.GetType().FullName);
		assertThat(executionListener.CamundaEvent).isEqualTo("start");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCamundaExecutionListenerByExpression()
	  public virtual void testCamundaExecutionListenerByExpression()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask("task").camundaExecutionListenerExpression("start", "anExpression").endEvent().done();

		UserTask userTask = modelInstance.getModelElementById("task");
		ExtensionElements extensionElements = userTask.ExtensionElements;
		ICollection<CamundaExecutionListener> executionListeners = extensionElements.getChildElementsByType(typeof(CamundaExecutionListener));
		assertThat(executionListeners).hasSize(1);

		CamundaExecutionListener executionListener = executionListeners.GetEnumerator().next();
		assertThat(executionListener.CamundaExpression).isEqualTo("anExpression");
		assertThat(executionListener.CamundaEvent).isEqualTo("start");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCamundaExecutionListenerByDelegateExpression()
	  public virtual void testCamundaExecutionListenerByDelegateExpression()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask("task").camundaExecutionListenerDelegateExpression("start", "aDelegateExpression").endEvent().done();

		UserTask userTask = modelInstance.getModelElementById("task");
		ExtensionElements extensionElements = userTask.ExtensionElements;
		ICollection<CamundaExecutionListener> executionListeners = extensionElements.getChildElementsByType(typeof(CamundaExecutionListener));
		assertThat(executionListeners).hasSize(1);

		CamundaExecutionListener executionListener = executionListeners.GetEnumerator().next();
		assertThat(executionListener.CamundaDelegateExpression).isEqualTo("aDelegateExpression");
		assertThat(executionListener.CamundaEvent).isEqualTo("start");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMultiInstanceLoopCharacteristicsSequential()
	  public virtual void testMultiInstanceLoopCharacteristicsSequential()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask("task").multiInstance().sequential().cardinality("card").completionCondition("compl").camundaCollection("coll").camundaElementVariable("element").multiInstanceDone().endEvent().done();

		UserTask userTask = modelInstance.getModelElementById("task");
		ICollection<MultiInstanceLoopCharacteristics> miCharacteristics = userTask.getChildElementsByType(typeof(MultiInstanceLoopCharacteristics));

		assertThat(miCharacteristics).hasSize(1);

		MultiInstanceLoopCharacteristics miCharacteristic = miCharacteristics.GetEnumerator().next();
		assertThat(miCharacteristic.Sequential).True;
		assertThat(miCharacteristic.LoopCardinality.TextContent).isEqualTo("card");
		assertThat(miCharacteristic.CompletionCondition.TextContent).isEqualTo("compl");
		assertThat(miCharacteristic.CamundaCollection).isEqualTo("coll");
		assertThat(miCharacteristic.CamundaElementVariable).isEqualTo("element");

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMultiInstanceLoopCharacteristicsParallel()
	  public virtual void testMultiInstanceLoopCharacteristicsParallel()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask("task").multiInstance().parallel().multiInstanceDone().endEvent().done();

		UserTask userTask = modelInstance.getModelElementById("task");
		ICollection<MultiInstanceLoopCharacteristics> miCharacteristics = userTask.getChildElementsByType(typeof(MultiInstanceLoopCharacteristics));

		assertThat(miCharacteristics).hasSize(1);

		MultiInstanceLoopCharacteristics miCharacteristic = miCharacteristics.GetEnumerator().next();
		assertThat(miCharacteristic.Sequential).False;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskWithCamundaInputOutput()
	  public virtual void testTaskWithCamundaInputOutput()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask("task").camundaInputParameter("foo", "bar").camundaInputParameter("yoo", "hoo").camundaOutputParameter("one", "two").camundaOutputParameter("three", "four").endEvent().done();

		UserTask task = modelInstance.getModelElementById("task");
		assertCamundaInputOutputParameter(task);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskWithCamundaInputOutputWithExistingExtensionElements()
	  public virtual void testTaskWithCamundaInputOutputWithExistingExtensionElements()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask("task").camundaExecutionListenerExpression("end", "${true}").camundaInputParameter("foo", "bar").camundaInputParameter("yoo", "hoo").camundaOutputParameter("one", "two").camundaOutputParameter("three", "four").endEvent().done();

		UserTask task = modelInstance.getModelElementById("task");
		assertCamundaInputOutputParameter(task);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskWithCamundaInputOutputWithExistingCamundaInputOutput()
	  public virtual void testTaskWithCamundaInputOutputWithExistingCamundaInputOutput()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask("task").camundaInputParameter("foo", "bar").camundaOutputParameter("one", "two").endEvent().done();

		UserTask task = modelInstance.getModelElementById("task");

		task.builder().camundaInputParameter("yoo", "hoo").camundaOutputParameter("three", "four");

		assertCamundaInputOutputParameter(task);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubProcessWithCamundaInputOutput()
	  public virtual void testSubProcessWithCamundaInputOutput()
	  {
		modelInstance = Bpmn.createProcess().startEvent().subProcess("subProcess").camundaInputParameter("foo", "bar").camundaInputParameter("yoo", "hoo").camundaOutputParameter("one", "two").camundaOutputParameter("three", "four").embeddedSubProcess().startEvent().endEvent().subProcessDone().endEvent().done();

		SubProcess subProcess = modelInstance.getModelElementById("subProcess");
		assertCamundaInputOutputParameter(subProcess);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubProcessWithCamundaInputOutputWithExistingExtensionElements()
	  public virtual void testSubProcessWithCamundaInputOutputWithExistingExtensionElements()
	  {
		modelInstance = Bpmn.createProcess().startEvent().subProcess("subProcess").camundaExecutionListenerExpression("end", "${true}").camundaInputParameter("foo", "bar").camundaInputParameter("yoo", "hoo").camundaOutputParameter("one", "two").camundaOutputParameter("three", "four").embeddedSubProcess().startEvent().endEvent().subProcessDone().endEvent().done();

		SubProcess subProcess = modelInstance.getModelElementById("subProcess");
		assertCamundaInputOutputParameter(subProcess);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubProcessWithCamundaInputOutputWithExistingCamundaInputOutput()
	  public virtual void testSubProcessWithCamundaInputOutputWithExistingCamundaInputOutput()
	  {
		modelInstance = Bpmn.createProcess().startEvent().subProcess("subProcess").camundaInputParameter("foo", "bar").camundaOutputParameter("one", "two").embeddedSubProcess().startEvent().endEvent().subProcessDone().endEvent().done();

		SubProcess subProcess = modelInstance.getModelElementById("subProcess");

		subProcess.builder().camundaInputParameter("yoo", "hoo").camundaOutputParameter("three", "four");

		assertCamundaInputOutputParameter(subProcess);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTimerStartEventWithDate()
	  public virtual void testTimerStartEventWithDate()
	  {
		modelInstance = Bpmn.createProcess().startEvent("start").timerWithDate(TIMER_DATE).done();

		assertTimerWithDate("start", TIMER_DATE);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTimerStartEventWithDuration()
	  public virtual void testTimerStartEventWithDuration()
	  {
		modelInstance = Bpmn.createProcess().startEvent("start").timerWithDuration(TIMER_DURATION).done();

		assertTimerWithDuration("start", TIMER_DURATION);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTimerStartEventWithCycle()
	  public virtual void testTimerStartEventWithCycle()
	  {
		modelInstance = Bpmn.createProcess().startEvent("start").timerWithCycle(TIMER_CYCLE).done();

		assertTimerWithCycle("start", TIMER_CYCLE);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIntermediateTimerCatchEventWithDate()
	  public virtual void testIntermediateTimerCatchEventWithDate()
	  {
		modelInstance = Bpmn.createProcess().startEvent().intermediateCatchEvent("catch").timerWithDate(TIMER_DATE).done();

		assertTimerWithDate("catch", TIMER_DATE);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIntermediateTimerCatchEventWithDuration()
	  public virtual void testIntermediateTimerCatchEventWithDuration()
	  {
		modelInstance = Bpmn.createProcess().startEvent().intermediateCatchEvent("catch").timerWithDuration(TIMER_DURATION).done();

		assertTimerWithDuration("catch", TIMER_DURATION);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIntermediateTimerCatchEventWithCycle()
	  public virtual void testIntermediateTimerCatchEventWithCycle()
	  {
		modelInstance = Bpmn.createProcess().startEvent().intermediateCatchEvent("catch").timerWithCycle(TIMER_CYCLE).done();

		assertTimerWithCycle("catch", TIMER_CYCLE);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTimerBoundaryEventWithDate()
	  public virtual void testTimerBoundaryEventWithDate()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask("task").endEvent().moveToActivity("task").boundaryEvent("boundary").timerWithDate(TIMER_DATE).done();

		assertTimerWithDate("boundary", TIMER_DATE);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTimerBoundaryEventWithDuration()
	  public virtual void testTimerBoundaryEventWithDuration()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask("task").endEvent().moveToActivity("task").boundaryEvent("boundary").timerWithDuration(TIMER_DURATION).done();

		assertTimerWithDuration("boundary", TIMER_DURATION);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTimerBoundaryEventWithCycle()
	  public virtual void testTimerBoundaryEventWithCycle()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask("task").endEvent().moveToActivity("task").boundaryEvent("boundary").timerWithCycle(TIMER_CYCLE).done();

		assertTimerWithCycle("boundary", TIMER_CYCLE);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNotCancelingBoundaryEvent()
	  public virtual void testNotCancelingBoundaryEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask().boundaryEvent("boundary").cancelActivity(false).done();

		BoundaryEvent boundaryEvent = modelInstance.getModelElementById("boundary");
		assertThat(boundaryEvent.cancelActivity()).False;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCatchAllErrorBoundaryEvent()
	  public virtual void testCatchAllErrorBoundaryEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask("task").endEvent().moveToActivity("task").boundaryEvent("boundary").error().endEvent("boundaryEnd").done();

		ErrorEventDefinition errorEventDefinition = assertAndGetSingleEventDefinition("boundary", typeof(ErrorEventDefinition));
		assertThat(errorEventDefinition.Error).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompensationTask()
	  public virtual void testCompensationTask()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask("task").boundaryEvent("boundary").compensateEventDefinition().compensateEventDefinitionDone().compensationStart().userTask("compensate").name("compensate").compensationDone().endEvent("theend").done();

		// Checking Association
		ICollection<Association> associations = modelInstance.getModelElementsByType(typeof(Association));
		assertThat(associations).hasSize(1);
		Association association = associations.GetEnumerator().next();
		assertThat(association.Source.Id).isEqualTo("boundary");
		assertThat(association.Target.Id).isEqualTo("compensate");
		assertThat(association.AssociationDirection).isEqualTo(AssociationDirection.One);

		// Checking Sequence flow
		UserTask task = modelInstance.getModelElementById("task");
		ICollection<SequenceFlow> outgoing = task.Outgoing;
		assertThat(outgoing).hasSize(1);
		SequenceFlow flow = outgoing.GetEnumerator().next();
		assertThat(flow.Source.Id).isEqualTo("task");
		assertThat(flow.Target.Id).isEqualTo("theend");

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOnlyOneCompensateBoundaryEventAllowed()
	  public virtual void testOnlyOneCompensateBoundaryEventAllowed()
	  {
		// given
		UserTaskBuilder builder = Bpmn.createProcess().startEvent().userTask("task").boundaryEvent("boundary").compensateEventDefinition().compensateEventDefinitionDone().compensationStart().userTask("compensate").name("compensate");

		// then
		thrown.expect(typeof(BpmnModelException));
		thrown.expectMessage("Only single compensation handler allowed. Call compensationDone() to continue main flow.");

		// when
		builder.userTask();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidCompensationStartCall()
	  public virtual void testInvalidCompensationStartCall()
	  {
		// given
		StartEventBuilder builder = Bpmn.createProcess().startEvent();

		// then
		thrown.expect(typeof(BpmnModelException));
		thrown.expectMessage("Compensation can only be started on a boundary event with a compensation event definition");

		// when
		builder.compensationStart();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidCompensationDoneCall()
	  public virtual void testInvalidCompensationDoneCall()
	  {
		// given
		AbstractFlowNodeBuilder builder = Bpmn.createProcess().startEvent().userTask("task").boundaryEvent("boundary").compensateEventDefinition().compensateEventDefinitionDone();

		// then
		thrown.expect(typeof(BpmnModelException));
		thrown.expectMessage("No compensation in progress. Call compensationStart() first.");

		// when
		builder.compensationDone();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testErrorBoundaryEvent()
	  public virtual void testErrorBoundaryEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask("task").endEvent().moveToActivity("task").boundaryEvent("boundary").error("myErrorCode").endEvent("boundaryEnd").done();

		assertErrorEventDefinition("boundary", "myErrorCode");

		UserTask userTask = modelInstance.getModelElementById("task");
		BoundaryEvent boundaryEvent = modelInstance.getModelElementById("boundary");
		EndEvent boundaryEnd = modelInstance.getModelElementById("boundaryEnd");

		// boundary event is attached to the user task
		assertThat(boundaryEvent.AttachedTo).isEqualTo(userTask);

		// boundary event has no incoming sequence flows
		assertThat(boundaryEvent.Incoming).Empty;

		// the next flow node is the boundary end event
		IList<FlowNode> succeedingNodes = boundaryEvent.SucceedingNodes.list();
		assertThat(succeedingNodes).containsOnly(boundaryEnd);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testErrorDefinitionForBoundaryEvent()
	  public virtual void testErrorDefinitionForBoundaryEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask("task").endEvent().moveToActivity("task").boundaryEvent("boundary").errorEventDefinition("event").errorCodeVariable("errorCodeVariable").errorMessageVariable("errorMessageVariable").error("errorCode").errorEventDefinitionDone().endEvent("boundaryEnd").done();

		assertErrorEventDefinition("boundary", "errorCode");
		assertErrorEventDefinitionForErrorVariables("boundary", "errorCodeVariable", "errorMessageVariable");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testErrorDefinitionForBoundaryEventWithoutEventDefinitionId()
	  public virtual void testErrorDefinitionForBoundaryEventWithoutEventDefinitionId()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask("task").endEvent().moveToActivity("task").boundaryEvent("boundary").errorEventDefinition().errorCodeVariable("errorCodeVariable").errorMessageVariable("errorMessageVariable").error("errorCode").errorEventDefinitionDone().endEvent("boundaryEnd").done();

		assertErrorEventDefinition("boundary", "errorCode");
		assertErrorEventDefinitionForErrorVariables("boundary", "errorCodeVariable", "errorMessageVariable");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testErrorEndEvent()
	  public virtual void testErrorEndEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent().endEvent("end").error("myErrorCode").done();

		assertErrorEventDefinition("end", "myErrorCode");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testErrorEndEventWithExistingError()
	  public virtual void testErrorEndEventWithExistingError()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask("task").endEvent("end").error("myErrorCode").moveToActivity("task").boundaryEvent("boundary").error("myErrorCode").endEvent("boundaryEnd").done();

		Error boundaryError = assertErrorEventDefinition("boundary", "myErrorCode");
		Error endError = assertErrorEventDefinition("end", "myErrorCode");

		assertThat(boundaryError).isEqualTo(endError);

		assertOnlyOneErrorExists("myErrorCode");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testErrorStartEvent()
	  public virtual void testErrorStartEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent().endEvent().subProcess().triggerByEvent().embeddedSubProcess().startEvent("subProcessStart").error("myErrorCode").endEvent().done();

		assertErrorEventDefinition("subProcessStart", "myErrorCode");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCatchAllErrorStartEvent()
	  public virtual void testCatchAllErrorStartEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent().endEvent().subProcess().triggerByEvent().embeddedSubProcess().startEvent("subProcessStart").error().endEvent().done();

		ErrorEventDefinition errorEventDefinition = assertAndGetSingleEventDefinition("subProcessStart", typeof(ErrorEventDefinition));
		assertThat(errorEventDefinition.Error).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCatchAllEscalationBoundaryEvent()
	  public virtual void testCatchAllEscalationBoundaryEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask("task").endEvent().moveToActivity("task").boundaryEvent("boundary").escalation().endEvent("boundaryEnd").done();

		EscalationEventDefinition escalationEventDefinition = assertAndGetSingleEventDefinition("boundary", typeof(EscalationEventDefinition));
		assertThat(escalationEventDefinition.Escalation).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEscalationBoundaryEvent()
	  public virtual void testEscalationBoundaryEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent().subProcess("subProcess").endEvent().moveToActivity("subProcess").boundaryEvent("boundary").escalation("myEscalationCode").endEvent("boundaryEnd").done();

		assertEscalationEventDefinition("boundary", "myEscalationCode");

		SubProcess subProcess = modelInstance.getModelElementById("subProcess");
		BoundaryEvent boundaryEvent = modelInstance.getModelElementById("boundary");
		EndEvent boundaryEnd = modelInstance.getModelElementById("boundaryEnd");

		// boundary event is attached to the sub process
		assertThat(boundaryEvent.AttachedTo).isEqualTo(subProcess);

		// boundary event has no incoming sequence flows
		assertThat(boundaryEvent.Incoming).Empty;

		// the next flow node is the boundary end event
		IList<FlowNode> succeedingNodes = boundaryEvent.SucceedingNodes.list();
		assertThat(succeedingNodes).containsOnly(boundaryEnd);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEscalationEndEvent()
	  public virtual void testEscalationEndEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent().endEvent("end").escalation("myEscalationCode").done();

		assertEscalationEventDefinition("end", "myEscalationCode");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEscalationStartEvent()
	  public virtual void testEscalationStartEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent().endEvent().subProcess().triggerByEvent().embeddedSubProcess().startEvent("subProcessStart").escalation("myEscalationCode").endEvent().done();

		assertEscalationEventDefinition("subProcessStart", "myEscalationCode");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCatchAllEscalationStartEvent()
	  public virtual void testCatchAllEscalationStartEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent().endEvent().subProcess().triggerByEvent().embeddedSubProcess().startEvent("subProcessStart").escalation().endEvent().done();

		EscalationEventDefinition escalationEventDefinition = assertAndGetSingleEventDefinition("subProcessStart", typeof(EscalationEventDefinition));
		assertThat(escalationEventDefinition.Escalation).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIntermediateEscalationThrowEvent()
	  public virtual void testIntermediateEscalationThrowEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent().intermediateThrowEvent("throw").escalation("myEscalationCode").endEvent().done();

		assertEscalationEventDefinition("throw", "myEscalationCode");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEscalationEndEventWithExistingEscalation()
	  public virtual void testEscalationEndEventWithExistingEscalation()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask("task").endEvent("end").escalation("myEscalationCode").moveToActivity("task").boundaryEvent("boundary").escalation("myEscalationCode").endEvent("boundaryEnd").done();

		Escalation boundaryEscalation = assertEscalationEventDefinition("boundary", "myEscalationCode");
		Escalation endEscalation = assertEscalationEventDefinition("end", "myEscalationCode");

		assertThat(boundaryEscalation).isEqualTo(endEscalation);

		assertOnlyOneEscalationExists("myEscalationCode");

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompensationStartEvent()
	  public virtual void testCompensationStartEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent().endEvent().subProcess().triggerByEvent().embeddedSubProcess().startEvent("subProcessStart").compensation().endEvent().done();

		assertCompensationEventDefinition("subProcessStart");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInterruptingStartEvent()
	  public virtual void testInterruptingStartEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent().endEvent().subProcess().triggerByEvent().embeddedSubProcess().startEvent("subProcessStart").interrupting(true).error().endEvent().done();

		StartEvent startEvent = modelInstance.getModelElementById("subProcessStart");
		assertThat(startEvent).NotNull;
		assertThat(startEvent.Interrupting).True;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingStartEvent()
	  public virtual void testNonInterruptingStartEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent().endEvent().subProcess().triggerByEvent().embeddedSubProcess().startEvent("subProcessStart").interrupting(false).error().endEvent().done();

		StartEvent startEvent = modelInstance.getModelElementById("subProcessStart");
		assertThat(startEvent).NotNull;
		assertThat(startEvent.Interrupting).False;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUserTaskCamundaFormField()
	  public virtual void testUserTaskCamundaFormField()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask(TASK_ID).camundaFormField().camundaId("myFormField_1").camundaLabel("Form Field One").camundaType("string").camundaDefaultValue("myDefaultVal_1").camundaFormFieldDone().camundaFormField().camundaId("myFormField_2").camundaLabel("Form Field Two").camundaType("integer").camundaDefaultValue("myDefaultVal_2").camundaFormFieldDone().endEvent().done();

		UserTask userTask = modelInstance.getModelElementById(TASK_ID);
		assertCamundaFormField(userTask);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUserTaskCamundaFormFieldWithExistingCamundaFormData()
	  public virtual void testUserTaskCamundaFormFieldWithExistingCamundaFormData()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask(TASK_ID).camundaFormField().camundaId("myFormField_1").camundaLabel("Form Field One").camundaType("string").camundaDefaultValue("myDefaultVal_1").camundaFormFieldDone().endEvent().done();

		UserTask userTask = modelInstance.getModelElementById(TASK_ID);

		userTask.builder().camundaFormField().camundaId("myFormField_2").camundaLabel("Form Field Two").camundaType("integer").camundaDefaultValue("myDefaultVal_2").camundaFormFieldDone();

		assertCamundaFormField(userTask);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStartEventCamundaFormField()
	  public virtual void testStartEventCamundaFormField()
	  {
		modelInstance = Bpmn.createProcess().startEvent(START_EVENT_ID).camundaFormField().camundaId("myFormField_1").camundaLabel("Form Field One").camundaType("string").camundaDefaultValue("myDefaultVal_1").camundaFormFieldDone().camundaFormField().camundaId("myFormField_2").camundaLabel("Form Field Two").camundaType("integer").camundaDefaultValue("myDefaultVal_2").camundaFormFieldDone().endEvent().done();

		StartEvent startEvent = modelInstance.getModelElementById(START_EVENT_ID);
		assertCamundaFormField(startEvent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompensateEventDefintionCatchStartEvent()
	  public virtual void testCompensateEventDefintionCatchStartEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent("start").compensateEventDefinition().waitForCompletion(false).compensateEventDefinitionDone().userTask("userTask").endEvent("end").done();

		CompensateEventDefinition eventDefinition = assertAndGetSingleEventDefinition("start", typeof(CompensateEventDefinition));
		Activity activity = eventDefinition.Activity;
		assertThat(activity).Null;
		assertThat(eventDefinition.WaitForCompletion).False;
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompensateEventDefintionCatchBoundaryEvent()
	  public virtual void testCompensateEventDefintionCatchBoundaryEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask("userTask").boundaryEvent("catch").compensateEventDefinition().waitForCompletion(false).compensateEventDefinitionDone().endEvent("end").done();

		CompensateEventDefinition eventDefinition = assertAndGetSingleEventDefinition("catch", typeof(CompensateEventDefinition));
		Activity activity = eventDefinition.Activity;
		assertThat(activity).Null;
		assertThat(eventDefinition.WaitForCompletion).False;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompensateEventDefintionCatchBoundaryEventWithId()
	  public virtual void testCompensateEventDefintionCatchBoundaryEventWithId()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask("userTask").boundaryEvent("catch").compensateEventDefinition("foo").waitForCompletion(false).compensateEventDefinitionDone().endEvent("end").done();

		CompensateEventDefinition eventDefinition = assertAndGetSingleEventDefinition("catch", typeof(CompensateEventDefinition));
		assertThat(eventDefinition.Id).isEqualTo("foo");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompensateEventDefintionThrowEndEvent()
	  public virtual void testCompensateEventDefintionThrowEndEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask("userTask").endEvent("end").compensateEventDefinition().activityRef("userTask").waitForCompletion(true).compensateEventDefinitionDone().done();

		CompensateEventDefinition eventDefinition = assertAndGetSingleEventDefinition("end", typeof(CompensateEventDefinition));
		Activity activity = eventDefinition.Activity;
		assertThat(activity).isEqualTo(modelInstance.getModelElementById("userTask"));
		assertThat(eventDefinition.WaitForCompletion).True;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompensateEventDefintionThrowIntermediateEvent()
	  public virtual void testCompensateEventDefintionThrowIntermediateEvent()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask("userTask").intermediateThrowEvent("throw").compensateEventDefinition().activityRef("userTask").waitForCompletion(true).compensateEventDefinitionDone().endEvent("end").done();

		CompensateEventDefinition eventDefinition = assertAndGetSingleEventDefinition("throw", typeof(CompensateEventDefinition));
		Activity activity = eventDefinition.Activity;
		assertThat(activity).isEqualTo(modelInstance.getModelElementById("userTask"));
		assertThat(eventDefinition.WaitForCompletion).True;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompensateEventDefintionThrowIntermediateEventWithId()
	  public virtual void testCompensateEventDefintionThrowIntermediateEventWithId()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask("userTask").intermediateCatchEvent("throw").compensateEventDefinition("foo").activityRef("userTask").waitForCompletion(true).compensateEventDefinitionDone().endEvent("end").done();

		CompensateEventDefinition eventDefinition = assertAndGetSingleEventDefinition("throw", typeof(CompensateEventDefinition));
		assertThat(eventDefinition.Id).isEqualTo("foo");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompensateEventDefintionReferencesNonExistingActivity()
	  public virtual void testCompensateEventDefintionReferencesNonExistingActivity()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask("userTask").endEvent("end").done();

		UserTask userTask = modelInstance.getModelElementById("userTask");
		UserTaskBuilder userTaskBuilder = userTask.builder();

		try
		{
		  userTaskBuilder.boundaryEvent().compensateEventDefinition().activityRef("nonExistingTask").done();
		  fail("should fail");
		}
		catch (BpmnModelException e)
		{
		  assertThat(e).hasMessageContaining("Activity with id 'nonExistingTask' does not exist");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompensateEventDefintionReferencesActivityInDifferentScope()
	  public virtual void testCompensateEventDefintionReferencesActivityInDifferentScope()
	  {
		modelInstance = Bpmn.createProcess().startEvent().userTask("userTask").subProcess().embeddedSubProcess().startEvent().userTask("subProcessTask").endEvent().subProcessDone().endEvent("end").done();

		UserTask userTask = modelInstance.getModelElementById("userTask");
		UserTaskBuilder userTaskBuilder = userTask.builder();

		try
		{
		  userTaskBuilder.boundaryEvent("boundary").compensateEventDefinition().activityRef("subProcessTask").done();
		  fail("should fail");
		}
		catch (BpmnModelException e)
		{
		  assertThat(e).hasMessageContaining("Activity with id 'subProcessTask' must be in the same scope as 'boundary'");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testConditionalEventDefinitionCamundaExtensions()
	  public virtual void testConditionalEventDefinitionCamundaExtensions()
	  {
		modelInstance = Bpmn.createProcess().startEvent().intermediateCatchEvent().conditionalEventDefinition(CONDITION_ID).condition(TEST_CONDITION).camundaVariableEvents(TEST_CONDITIONAL_VARIABLE_EVENTS).camundaVariableEvents(TEST_CONDITIONAL_VARIABLE_EVENTS_LIST).camundaVariableName(TEST_CONDITIONAL_VARIABLE_NAME).conditionalEventDefinitionDone().endEvent().done();

		ConditionalEventDefinition conditionalEventDef = modelInstance.getModelElementById(CONDITION_ID);
		assertThat(conditionalEventDef.CamundaVariableEvents).isEqualTo(TEST_CONDITIONAL_VARIABLE_EVENTS);
		assertThat(conditionalEventDef.CamundaVariableEventsList).containsAll(TEST_CONDITIONAL_VARIABLE_EVENTS_LIST);
		assertThat(conditionalEventDef.CamundaVariableName).isEqualTo(TEST_CONDITIONAL_VARIABLE_NAME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIntermediateConditionalEventDefinition()
	  public virtual void testIntermediateConditionalEventDefinition()
	  {

		modelInstance = Bpmn.createProcess().startEvent().intermediateCatchEvent(CATCH_ID).conditionalEventDefinition(CONDITION_ID).condition(TEST_CONDITION).conditionalEventDefinitionDone().endEvent().done();

		ConditionalEventDefinition eventDefinition = assertAndGetSingleEventDefinition(CATCH_ID, typeof(ConditionalEventDefinition));
		assertThat(eventDefinition.Id).isEqualTo(CONDITION_ID);
		assertThat(eventDefinition.Condition.TextContent).isEqualTo(TEST_CONDITION);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIntermediateConditionalEventDefinitionShortCut()
	  public virtual void testIntermediateConditionalEventDefinitionShortCut()
	  {

		modelInstance = Bpmn.createProcess().startEvent().intermediateCatchEvent(CATCH_ID).condition(TEST_CONDITION).endEvent().done();

		ConditionalEventDefinition eventDefinition = assertAndGetSingleEventDefinition(CATCH_ID, typeof(ConditionalEventDefinition));
		assertThat(eventDefinition.Condition.TextContent).isEqualTo(TEST_CONDITION);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBoundaryConditionalEventDefinition()
	  public virtual void testBoundaryConditionalEventDefinition()
	  {

		modelInstance = Bpmn.createProcess().startEvent().userTask(USER_TASK_ID).endEvent().moveToActivity(USER_TASK_ID).boundaryEvent(BOUNDARY_ID).conditionalEventDefinition(CONDITION_ID).condition(TEST_CONDITION).conditionalEventDefinitionDone().endEvent().done();

		ConditionalEventDefinition eventDefinition = assertAndGetSingleEventDefinition(BOUNDARY_ID, typeof(ConditionalEventDefinition));
		assertThat(eventDefinition.Id).isEqualTo(CONDITION_ID);
		assertThat(eventDefinition.Condition.TextContent).isEqualTo(TEST_CONDITION);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEventSubProcessConditionalStartEvent()
	  public virtual void testEventSubProcessConditionalStartEvent()
	  {

		modelInstance = Bpmn.createProcess().startEvent().userTask().endEvent().subProcess().triggerByEvent().embeddedSubProcess().startEvent(START_EVENT_ID).conditionalEventDefinition(CONDITION_ID).condition(TEST_CONDITION).conditionalEventDefinitionDone().endEvent().done();

		ConditionalEventDefinition eventDefinition = assertAndGetSingleEventDefinition(START_EVENT_ID, typeof(ConditionalEventDefinition));
		assertThat(eventDefinition.Id).isEqualTo(CONDITION_ID);
		assertThat(eventDefinition.Condition.TextContent).isEqualTo(TEST_CONDITION);
	  }

	  protected internal virtual Message assertMessageEventDefinition(string elementId, string messageName)
	  {
		MessageEventDefinition messageEventDefinition = assertAndGetSingleEventDefinition(elementId, typeof(MessageEventDefinition));
		Message message = messageEventDefinition.Message;
		assertThat(message).NotNull;
		assertThat(message.Name).isEqualTo(messageName);

		return message;
	  }

	  protected internal virtual void assertOnlyOneMessageExists(string messageName)
	  {
		ICollection<Message> messages = modelInstance.getModelElementsByType(typeof(Message));
		assertThat(messages).extracting("name").containsOnlyOnce(messageName);
	  }

	  protected internal virtual Signal assertSignalEventDefinition(string elementId, string signalName)
	  {
		SignalEventDefinition signalEventDefinition = assertAndGetSingleEventDefinition(elementId, typeof(SignalEventDefinition));
		Signal signal = signalEventDefinition.Signal;
		assertThat(signal).NotNull;
		assertThat(signal.Name).isEqualTo(signalName);

		return signal;
	  }

	  protected internal virtual void assertOnlyOneSignalExists(string signalName)
	  {
		ICollection<Signal> signals = modelInstance.getModelElementsByType(typeof(Signal));
		assertThat(signals).extracting("name").containsOnlyOnce(signalName);
	  }

	  protected internal virtual Error assertErrorEventDefinition(string elementId, string errorCode)
	  {
		ErrorEventDefinition errorEventDefinition = assertAndGetSingleEventDefinition(elementId, typeof(ErrorEventDefinition));
		Error error = errorEventDefinition.Error;
		assertThat(error).NotNull;
		assertThat(error.ErrorCode).isEqualTo(errorCode);

		return error;
	  }

	  protected internal virtual void assertErrorEventDefinitionForErrorVariables(string elementId, string errorCodeVariable, string errorMessageVariable)
	  {
		ErrorEventDefinition errorEventDefinition = assertAndGetSingleEventDefinition(elementId, typeof(ErrorEventDefinition));
		assertThat(errorEventDefinition).NotNull;
		if (!string.ReferenceEquals(errorCodeVariable, null))
		{
		  assertThat(errorEventDefinition.CamundaErrorCodeVariable).isEqualTo(errorCodeVariable);
		}
		if (!string.ReferenceEquals(errorMessageVariable, null))
		{
		  assertThat(errorEventDefinition.CamundaErrorMessageVariable).isEqualTo(errorMessageVariable);
		}
	  }

	  protected internal virtual void assertOnlyOneErrorExists(string errorCode)
	  {
		ICollection<Error> errors = modelInstance.getModelElementsByType(typeof(Error));
		assertThat(errors).extracting("errorCode").containsOnlyOnce(errorCode);
	  }

	  protected internal virtual Escalation assertEscalationEventDefinition(string elementId, string escalationCode)
	  {
		EscalationEventDefinition escalationEventDefinition = assertAndGetSingleEventDefinition(elementId, typeof(EscalationEventDefinition));
		Escalation escalation = escalationEventDefinition.Escalation;
		assertThat(escalation).NotNull;
		assertThat(escalation.EscalationCode).isEqualTo(escalationCode);

		return escalation;
	  }

	  protected internal virtual void assertOnlyOneEscalationExists(string escalationCode)
	  {
		ICollection<Escalation> escalations = modelInstance.getModelElementsByType(typeof(Escalation));
		assertThat(escalations).extracting("escalationCode").containsOnlyOnce(escalationCode);
	  }

	  protected internal virtual void assertCompensationEventDefinition(string elementId)
	  {
		assertAndGetSingleEventDefinition(elementId, typeof(CompensateEventDefinition));
	  }

	  protected internal virtual void assertCamundaInputOutputParameter(BaseElement element)
	  {
		CamundaInputOutput camundaInputOutput = element.ExtensionElements.ElementsQuery.filterByType(typeof(CamundaInputOutput)).singleResult();
		assertThat(camundaInputOutput).NotNull;

		IList<CamundaInputParameter> camundaInputParameters = new List<CamundaInputParameter>(camundaInputOutput.CamundaInputParameters);
		assertThat(camundaInputParameters).hasSize(2);

		CamundaInputParameter camundaInputParameter = camundaInputParameters[0];
		assertThat(camundaInputParameter.CamundaName).isEqualTo("foo");
		assertThat(camundaInputParameter.TextContent).isEqualTo("bar");

		camundaInputParameter = camundaInputParameters[1];
		assertThat(camundaInputParameter.CamundaName).isEqualTo("yoo");
		assertThat(camundaInputParameter.TextContent).isEqualTo("hoo");

		IList<CamundaOutputParameter> camundaOutputParameters = new List<CamundaOutputParameter>(camundaInputOutput.CamundaOutputParameters);
		assertThat(camundaOutputParameters).hasSize(2);

		CamundaOutputParameter camundaOutputParameter = camundaOutputParameters[0];
		assertThat(camundaOutputParameter.CamundaName).isEqualTo("one");
		assertThat(camundaOutputParameter.TextContent).isEqualTo("two");

		camundaOutputParameter = camundaOutputParameters[1];
		assertThat(camundaOutputParameter.CamundaName).isEqualTo("three");
		assertThat(camundaOutputParameter.TextContent).isEqualTo("four");
	  }

	  protected internal virtual void assertTimerWithDate(string elementId, string timerDate)
	  {
		TimerEventDefinition timerEventDefinition = assertAndGetSingleEventDefinition(elementId, typeof(TimerEventDefinition));
		TimeDate timeDate = timerEventDefinition.TimeDate;
		assertThat(timeDate).NotNull;
		assertThat(timeDate.TextContent).isEqualTo(timerDate);
	  }

	  protected internal virtual void assertTimerWithDuration(string elementId, string timerDuration)
	  {
		TimerEventDefinition timerEventDefinition = assertAndGetSingleEventDefinition(elementId, typeof(TimerEventDefinition));
		TimeDuration timeDuration = timerEventDefinition.TimeDuration;
		assertThat(timeDuration).NotNull;
		assertThat(timeDuration.TextContent).isEqualTo(timerDuration);
	  }

	  protected internal virtual void assertTimerWithCycle(string elementId, string timerCycle)
	  {
		TimerEventDefinition timerEventDefinition = assertAndGetSingleEventDefinition(elementId, typeof(TimerEventDefinition));
		TimeCycle timeCycle = timerEventDefinition.TimeCycle;
		assertThat(timeCycle).NotNull;
		assertThat(timeCycle.TextContent).isEqualTo(timerCycle);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected <T extends EventDefinition> T assertAndGetSingleEventDefinition(String elementId, Class<T> eventDefinitionType)
	  protected internal virtual T assertAndGetSingleEventDefinition<T>(string elementId, Type<T> eventDefinitionType) where T : EventDefinition
	  {
		BpmnModelElementInstance element = modelInstance.getModelElementById(elementId);
		assertThat(element).NotNull;
		ICollection<EventDefinition> eventDefinitions = element.getChildElementsByType(typeof(EventDefinition));
		assertThat(eventDefinitions).hasSize(1);

		EventDefinition eventDefinition = eventDefinitions.GetEnumerator().next();
		assertThat(eventDefinition).NotNull.isInstanceOf(eventDefinitionType);
		return (T) eventDefinition;
	  }

	  protected internal virtual void assertCamundaFormField(BaseElement element)
	  {
		assertThat(element.ExtensionElements).NotNull;

		CamundaFormData camundaFormData = element.ExtensionElements.ElementsQuery.filterByType(typeof(CamundaFormData)).singleResult();
		assertThat(camundaFormData).NotNull;

		IList<CamundaFormField> camundaFormFields = new List<CamundaFormField>(camundaFormData.CamundaFormFields);
		assertThat(camundaFormFields).hasSize(2);

		CamundaFormField camundaFormField = camundaFormFields[0];
		assertThat(camundaFormField.CamundaId).isEqualTo("myFormField_1");
		assertThat(camundaFormField.CamundaLabel).isEqualTo("Form Field One");
		assertThat(camundaFormField.CamundaType).isEqualTo("string");
		assertThat(camundaFormField.CamundaDefaultValue).isEqualTo("myDefaultVal_1");

		camundaFormField = camundaFormFields[1];
		assertThat(camundaFormField.CamundaId).isEqualTo("myFormField_2");
		assertThat(camundaFormField.CamundaLabel).isEqualTo("Form Field Two");
		assertThat(camundaFormField.CamundaType).isEqualTo("integer");
		assertThat(camundaFormField.CamundaDefaultValue).isEqualTo("myDefaultVal_2");

	  }

	  protected internal virtual void assertCamundaFailedJobRetryTimeCycle(BaseElement element)
	  {
		assertThat(element.ExtensionElements).NotNull;

		CamundaFailedJobRetryTimeCycle camundaFailedJobRetryTimeCycle = element.ExtensionElements.ElementsQuery.filterByType(typeof(CamundaFailedJobRetryTimeCycle)).singleResult();
		assertThat(camundaFailedJobRetryTimeCycle).NotNull;
		assertThat(camundaFailedJobRetryTimeCycle.TextContent).isEqualTo(FAILED_JOB_RETRY_TIME_CYCLE);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateEventSubProcess()
	  public virtual void testCreateEventSubProcess()
	  {
		ProcessBuilder process = Bpmn.createProcess();
		modelInstance = process.startEvent().sendTask().endEvent().done();

		EventSubProcessBuilder eventSubProcess = process.eventSubProcess();
		eventSubProcess.startEvent().userTask().endEvent();

		SubProcess subProcess = eventSubProcess.Element;

		// no input or output from the sub process
		assertThat(subProcess.Incoming.Count == 0);
		assertThat(subProcess.Outgoing.Count == 0);

		// subProcess was triggered by event
		assertThat(eventSubProcess.Element.triggeredByEvent());

		// subProcess contains startEvent, sendTask and endEvent
		assertThat(subProcess.getChildElementsByType(typeof(StartEvent))).NotNull;
		assertThat(subProcess.getChildElementsByType(typeof(UserTask))).NotNull;
		assertThat(subProcess.getChildElementsByType(typeof(EndEvent))).NotNull;
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateEventSubProcessInSubProcess()
	  public virtual void testCreateEventSubProcessInSubProcess()
	  {
		ProcessBuilder process = Bpmn.createProcess();
		modelInstance = process.startEvent().subProcess("mysubprocess").embeddedSubProcess().startEvent().userTask().endEvent().subProcessDone().userTask().endEvent().done();

		SubProcess subprocess = modelInstance.getModelElementById("mysubprocess");
		subprocess.builder().embeddedSubProcess().eventSubProcess("myeventsubprocess").startEvent().userTask().endEvent().subProcessDone();

		SubProcess eventSubProcess = modelInstance.getModelElementById("myeventsubprocess");

		// no input or output from the sub process
		assertThat(eventSubProcess.Incoming.Count == 0);
		assertThat(eventSubProcess.Outgoing.Count == 0);

		// subProcess was triggered by event
		assertThat(eventSubProcess.triggeredByEvent());

		// subProcess contains startEvent, sendTask and endEvent
		assertThat(eventSubProcess.getChildElementsByType(typeof(StartEvent))).NotNull;
		assertThat(eventSubProcess.getChildElementsByType(typeof(UserTask))).NotNull;
		assertThat(eventSubProcess.getChildElementsByType(typeof(EndEvent))).NotNull;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateEventSubProcessError()
	  public virtual void testCreateEventSubProcessError()
	  {
		ProcessBuilder process = Bpmn.createProcess();
		modelInstance = process.startEvent().sendTask().endEvent().done();

		EventSubProcessBuilder eventSubProcess = process.eventSubProcess();
		eventSubProcess.startEvent().userTask().endEvent();

		try
		{
		  eventSubProcess.subProcessDone();
		  fail("eventSubProcess has returned a builder after completion");
		}
		catch (BpmnModelException e)
		{
		  assertThat(e).hasMessageContaining("Unable to find a parent subProcess.");

		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetIdAsDefaultNameForFlowElements()
	  public virtual void testSetIdAsDefaultNameForFlowElements()
	  {
		BpmnModelInstance instance = Bpmn.createExecutableProcess("process").startEvent("start").userTask("user").endEvent("end").name("name").done();

		string startName = ((FlowElement) instance.getModelElementById("start")).Name;
		assertEquals("start", startName);
		string userName = ((FlowElement) instance.getModelElementById("user")).Name;
		assertEquals("user", userName);
		string endName = ((FlowElement) instance.getModelElementById("end")).Name;
		assertEquals("name", endName);
	  }

	}

}