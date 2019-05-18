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
namespace org.camunda.bpm.model.bpmn
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.BUSINESS_RULE_TASK;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.CALL_ACTIVITY_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.END_EVENT_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.PROCESS_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.SCRIPT_TASK_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.SEND_TASK_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.SEQUENCE_FLOW_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.SERVICE_TASK_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.START_EVENT_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TEST_CLASS_API;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TEST_CLASS_XML;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TEST_DELEGATE_EXPRESSION_API;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TEST_DELEGATE_EXPRESSION_XML;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TEST_DUE_DATE_API;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TEST_DUE_DATE_XML;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TEST_EXECUTION_EVENT_API;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TEST_EXECUTION_EVENT_XML;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TEST_EXPRESSION_API;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TEST_EXPRESSION_XML;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TEST_FLOW_NODE_JOB_PRIORITY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TEST_GROUPS_API;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TEST_GROUPS_LIST_API;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TEST_GROUPS_LIST_XML;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TEST_GROUPS_XML;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TEST_HISTORY_TIME_TO_LIVE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TEST_PRIORITY_API;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TEST_PRIORITY_XML;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TEST_PROCESS_JOB_PRIORITY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TEST_PROCESS_TASK_PRIORITY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TEST_SERVICE_TASK_PRIORITY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TEST_STRING_API;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TEST_STRING_XML;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TEST_TASK_EVENT_API;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TEST_TASK_EVENT_XML;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TEST_TYPE_API;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TEST_TYPE_XML;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TEST_USERS_API;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TEST_USERS_LIST_API;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TEST_USERS_LIST_XML;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TEST_USERS_XML;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.USER_TASK_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.ACTIVITI_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_ERROR_CODE_VARIABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_ERROR_MESSAGE_VARIABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_NS;


	using BaseElement = org.camunda.bpm.model.bpmn.instance.BaseElement;
	using BpmnModelElementInstance = org.camunda.bpm.model.bpmn.instance.BpmnModelElementInstance;
	using BusinessRuleTask = org.camunda.bpm.model.bpmn.instance.BusinessRuleTask;
	using CallActivity = org.camunda.bpm.model.bpmn.instance.CallActivity;
	using EndEvent = org.camunda.bpm.model.bpmn.instance.EndEvent;
	using ErrorEventDefinition = org.camunda.bpm.model.bpmn.instance.ErrorEventDefinition;
	using Expression = org.camunda.bpm.model.bpmn.instance.Expression;
	using MessageEventDefinition = org.camunda.bpm.model.bpmn.instance.MessageEventDefinition;
	using ParallelGateway = org.camunda.bpm.model.bpmn.instance.ParallelGateway;
	using Process = org.camunda.bpm.model.bpmn.instance.Process;
	using ScriptTask = org.camunda.bpm.model.bpmn.instance.ScriptTask;
	using SendTask = org.camunda.bpm.model.bpmn.instance.SendTask;
	using SequenceFlow = org.camunda.bpm.model.bpmn.instance.SequenceFlow;
	using ServiceTask = org.camunda.bpm.model.bpmn.instance.ServiceTask;
	using StartEvent = org.camunda.bpm.model.bpmn.instance.StartEvent;
	using UserTask = org.camunda.bpm.model.bpmn.instance.UserTask;
	using CamundaConnector = org.camunda.bpm.model.bpmn.instance.camunda.CamundaConnector;
	using CamundaConnectorId = org.camunda.bpm.model.bpmn.instance.camunda.CamundaConnectorId;
	using CamundaConstraint = org.camunda.bpm.model.bpmn.instance.camunda.CamundaConstraint;
	using CamundaEntry = org.camunda.bpm.model.bpmn.instance.camunda.CamundaEntry;
	using CamundaExecutionListener = org.camunda.bpm.model.bpmn.instance.camunda.CamundaExecutionListener;
	using CamundaFailedJobRetryTimeCycle = org.camunda.bpm.model.bpmn.instance.camunda.CamundaFailedJobRetryTimeCycle;
	using CamundaField = org.camunda.bpm.model.bpmn.instance.camunda.CamundaField;
	using CamundaFormData = org.camunda.bpm.model.bpmn.instance.camunda.CamundaFormData;
	using CamundaFormField = org.camunda.bpm.model.bpmn.instance.camunda.CamundaFormField;
	using CamundaFormProperty = org.camunda.bpm.model.bpmn.instance.camunda.CamundaFormProperty;
	using CamundaIn = org.camunda.bpm.model.bpmn.instance.camunda.CamundaIn;
	using CamundaInputOutput = org.camunda.bpm.model.bpmn.instance.camunda.CamundaInputOutput;
	using CamundaInputParameter = org.camunda.bpm.model.bpmn.instance.camunda.CamundaInputParameter;
	using CamundaList = org.camunda.bpm.model.bpmn.instance.camunda.CamundaList;
	using CamundaMap = org.camunda.bpm.model.bpmn.instance.camunda.CamundaMap;
	using CamundaOut = org.camunda.bpm.model.bpmn.instance.camunda.CamundaOut;
	using CamundaOutputParameter = org.camunda.bpm.model.bpmn.instance.camunda.CamundaOutputParameter;
	using CamundaPotentialStarter = org.camunda.bpm.model.bpmn.instance.camunda.CamundaPotentialStarter;
	using CamundaProperties = org.camunda.bpm.model.bpmn.instance.camunda.CamundaProperties;
	using CamundaProperty = org.camunda.bpm.model.bpmn.instance.camunda.CamundaProperty;
	using CamundaScript = org.camunda.bpm.model.bpmn.instance.camunda.CamundaScript;
	using CamundaTaskListener = org.camunda.bpm.model.bpmn.instance.camunda.CamundaTaskListener;
	using CamundaValue = org.camunda.bpm.model.bpmn.instance.camunda.CamundaValue;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;
	using Parameters = org.junit.runners.Parameterized.Parameters;

	/// <summary>
	/// @author Sebastian Menski
	/// @author Ronny Bräunlich
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class CamundaExtensionsTest
	public class CamundaExtensionsTest
	{

	  private Process process;
	  private StartEvent startEvent;
	  private SequenceFlow sequenceFlow;
	  private UserTask userTask;
	  private ServiceTask serviceTask;
	  private SendTask sendTask;
	  private ScriptTask scriptTask;
	  private CallActivity callActivity;
	  private BusinessRuleTask businessRuleTask;
	  private EndEvent endEvent;
	  private MessageEventDefinition messageEventDefinition;
	  private ParallelGateway parallelGateway;
	  private string @namespace;
	  private BpmnModelInstance originalModelInstance;
	  private BpmnModelInstance modelInstance;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters(name="Namespace: {0}") public static java.util.Collection<Object[]> parameters()
	  public static ICollection<object[]> parameters()
	  {
		return Arrays.asList(new object[][]
		{
			new object[] {CAMUNDA_NS, Bpmn.readModelFromStream(typeof(CamundaExtensionsTest).getResourceAsStream("CamundaExtensionsTest.xml"))},
			new object[] {ACTIVITI_NS, Bpmn.readModelFromStream(typeof(CamundaExtensionsTest).getResourceAsStream("CamundaExtensionsCompatabilityTest.xml"))}
		});
	  }

	  public CamundaExtensionsTest(string @namespace, BpmnModelInstance modelInstance)
	  {
		this.@namespace = @namespace;
		this.originalModelInstance = modelInstance;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		modelInstance = originalModelInstance.clone();
		process = modelInstance.getModelElementById(PROCESS_ID);
		startEvent = modelInstance.getModelElementById(START_EVENT_ID);
		sequenceFlow = modelInstance.getModelElementById(SEQUENCE_FLOW_ID);
		userTask = modelInstance.getModelElementById(USER_TASK_ID);
		serviceTask = modelInstance.getModelElementById(SERVICE_TASK_ID);
		sendTask = modelInstance.getModelElementById(SEND_TASK_ID);
		scriptTask = modelInstance.getModelElementById(SCRIPT_TASK_ID);
		callActivity = modelInstance.getModelElementById(CALL_ACTIVITY_ID);
		businessRuleTask = modelInstance.getModelElementById(BUSINESS_RULE_TASK);
		endEvent = modelInstance.getModelElementById(END_EVENT_ID);
		messageEventDefinition = (MessageEventDefinition) endEvent.EventDefinitions.GetEnumerator().next();
		parallelGateway = modelInstance.getModelElementById("parallelGateway");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAssignee()
	  public virtual void testAssignee()
	  {
		assertThat(userTask.CamundaAssignee).isEqualTo(TEST_STRING_XML);
		userTask.CamundaAssignee = TEST_STRING_API;
		assertThat(userTask.CamundaAssignee).isEqualTo(TEST_STRING_API);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAsync()
	  public virtual void testAsync()
	  {
		assertThat(startEvent.CamundaAsync).False;
		assertThat(userTask.CamundaAsync).True;
		assertThat(parallelGateway.CamundaAsync).True;

		startEvent.CamundaAsync = true;
		userTask.CamundaAsync = false;
		parallelGateway.CamundaAsync = false;

		assertThat(startEvent.CamundaAsync).True;
		assertThat(userTask.CamundaAsync).False;
		assertThat(parallelGateway.CamundaAsync).False;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAsyncBefore()
	  public virtual void testAsyncBefore()
	  {
		assertThat(startEvent.CamundaAsyncBefore).True;
		assertThat(endEvent.CamundaAsyncBefore).True;
		assertThat(userTask.CamundaAsyncBefore).True;
		assertThat(parallelGateway.CamundaAsyncBefore).True;

		startEvent.CamundaAsyncBefore = false;
		endEvent.CamundaAsyncBefore = false;
		userTask.CamundaAsyncBefore = false;
		parallelGateway.CamundaAsyncBefore = false;

		assertThat(startEvent.CamundaAsyncBefore).False;
		assertThat(endEvent.CamundaAsyncBefore).False;
		assertThat(userTask.CamundaAsyncBefore).False;
		assertThat(parallelGateway.CamundaAsyncBefore).False;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAsyncAfter()
	  public virtual void testAsyncAfter()
	  {
		assertThat(startEvent.CamundaAsyncAfter).True;
		assertThat(endEvent.CamundaAsyncAfter).True;
		assertThat(userTask.CamundaAsyncAfter).True;
		assertThat(parallelGateway.CamundaAsyncAfter).True;

		startEvent.CamundaAsyncAfter = false;
		endEvent.CamundaAsyncAfter = false;
		userTask.CamundaAsyncAfter = false;
		parallelGateway.CamundaAsyncAfter = false;

		assertThat(startEvent.CamundaAsyncAfter).False;
		assertThat(endEvent.CamundaAsyncAfter).False;
		assertThat(userTask.CamundaAsyncAfter).False;
		assertThat(parallelGateway.CamundaAsyncAfter).False;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFlowNodeJobPriority()
	  public virtual void testFlowNodeJobPriority()
	  {
		assertThat(startEvent.CamundaJobPriority).isEqualTo(TEST_FLOW_NODE_JOB_PRIORITY);
		assertThat(endEvent.CamundaJobPriority).isEqualTo(TEST_FLOW_NODE_JOB_PRIORITY);
		assertThat(userTask.CamundaJobPriority).isEqualTo(TEST_FLOW_NODE_JOB_PRIORITY);
		assertThat(parallelGateway.CamundaJobPriority).isEqualTo(TEST_FLOW_NODE_JOB_PRIORITY);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessJobPriority()
	  public virtual void testProcessJobPriority()
	  {
		assertThat(process.CamundaJobPriority).isEqualTo(TEST_PROCESS_JOB_PRIORITY);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessTaskPriority()
	  public virtual void testProcessTaskPriority()
	  {
		assertThat(process.CamundaTaskPriority).isEqualTo(TEST_PROCESS_TASK_PRIORITY);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoryTimeToLive()
	  public virtual void testHistoryTimeToLive()
	  {
		assertThat(process.getCamundaHistoryTimeToLive()).isEqualTo(TEST_HISTORY_TIME_TO_LIVE);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIsStartableInTasklist()
	  public virtual void testIsStartableInTasklist()
	  {
		assertThat(process.CamundaStartableInTasklist).isEqualTo(false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVersionTag()
	  public virtual void testVersionTag()
	  {
		assertThat(process.CamundaVersionTag).isEqualTo("v1.0.0");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testServiceTaskPriority()
	  public virtual void testServiceTaskPriority()
	  {
		assertThat(serviceTask.CamundaTaskPriority).isEqualTo(TEST_SERVICE_TASK_PRIORITY);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCalledElementBinding()
	  public virtual void testCalledElementBinding()
	  {
		assertThat(callActivity.CamundaCalledElementBinding).isEqualTo(TEST_STRING_XML);
		callActivity.CamundaCalledElementBinding = TEST_STRING_API;
		assertThat(callActivity.CamundaCalledElementBinding).isEqualTo(TEST_STRING_API);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCalledElementVersion()
	  public virtual void testCalledElementVersion()
	  {
		assertThat(callActivity.CamundaCalledElementVersion).isEqualTo(TEST_STRING_XML);
		callActivity.CamundaCalledElementVersion = TEST_STRING_API;
		assertThat(callActivity.CamundaCalledElementVersion).isEqualTo(TEST_STRING_API);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCalledElementVersionTag()
	  public virtual void testCalledElementVersionTag()
	  {
		assertThat(callActivity.CamundaCalledElementVersionTag).isEqualTo(TEST_STRING_XML);
		callActivity.CamundaCalledElementVersionTag = TEST_STRING_API;
		assertThat(callActivity.CamundaCalledElementVersionTag).isEqualTo(TEST_STRING_API);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCalledElementTenantId()
	  public virtual void testCalledElementTenantId()
	  {
		assertThat(callActivity.CamundaCalledElementTenantId).isEqualTo(TEST_STRING_XML);
		callActivity.CamundaCalledElementTenantId = TEST_STRING_API;
		assertThat(callActivity.CamundaCalledElementTenantId).isEqualTo(TEST_STRING_API);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseRef()
	  public virtual void testCaseRef()
	  {
		assertThat(callActivity.CamundaCaseRef).isEqualTo(TEST_STRING_XML);
		callActivity.CamundaCaseRef = TEST_STRING_API;
		assertThat(callActivity.CamundaCaseRef).isEqualTo(TEST_STRING_API);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseBinding()
	  public virtual void testCaseBinding()
	  {
		assertThat(callActivity.CamundaCaseBinding).isEqualTo(TEST_STRING_XML);
		callActivity.CamundaCaseBinding = TEST_STRING_API;
		assertThat(callActivity.CamundaCaseBinding).isEqualTo(TEST_STRING_API);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseVersion()
	  public virtual void testCaseVersion()
	  {
		assertThat(callActivity.CamundaCaseVersion).isEqualTo(TEST_STRING_XML);
		callActivity.CamundaCaseVersion = TEST_STRING_API;
		assertThat(callActivity.CamundaCaseVersion).isEqualTo(TEST_STRING_API);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseTenantId()
	  public virtual void testCaseTenantId()
	  {
		assertThat(callActivity.CamundaCaseTenantId).isEqualTo(TEST_STRING_XML);
		callActivity.CamundaCaseTenantId = TEST_STRING_API;
		assertThat(callActivity.CamundaCaseTenantId).isEqualTo(TEST_STRING_API);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDecisionRef()
	  public virtual void testDecisionRef()
	  {
		assertThat(businessRuleTask.CamundaDecisionRef).isEqualTo(TEST_STRING_XML);
		businessRuleTask.CamundaDecisionRef = TEST_STRING_API;
		assertThat(businessRuleTask.CamundaDecisionRef).isEqualTo(TEST_STRING_API);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDecisionRefBinding()
	  public virtual void testDecisionRefBinding()
	  {
		assertThat(businessRuleTask.CamundaDecisionRefBinding).isEqualTo(TEST_STRING_XML);
		businessRuleTask.CamundaDecisionRefBinding = TEST_STRING_API;
		assertThat(businessRuleTask.CamundaDecisionRefBinding).isEqualTo(TEST_STRING_API);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDecisionRefVersion()
	  public virtual void testDecisionRefVersion()
	  {
		assertThat(businessRuleTask.CamundaDecisionRefVersion).isEqualTo(TEST_STRING_XML);
		businessRuleTask.CamundaDecisionRefVersion = TEST_STRING_API;
		assertThat(businessRuleTask.CamundaDecisionRefVersion).isEqualTo(TEST_STRING_API);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDecisionRefVersionTag()
	  public virtual void testDecisionRefVersionTag()
	  {
		assertThat(businessRuleTask.CamundaDecisionRefVersionTag).isEqualTo(TEST_STRING_XML);
		businessRuleTask.CamundaDecisionRefVersionTag = TEST_STRING_API;
		assertThat(businessRuleTask.CamundaDecisionRefVersionTag).isEqualTo(TEST_STRING_API);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDecisionRefTenantId()
	  public virtual void testDecisionRefTenantId()
	  {
		assertThat(businessRuleTask.CamundaDecisionRefTenantId).isEqualTo(TEST_STRING_XML);
		businessRuleTask.CamundaDecisionRefTenantId = TEST_STRING_API;
		assertThat(businessRuleTask.CamundaDecisionRefTenantId).isEqualTo(TEST_STRING_API);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapDecisionResult()
	  public virtual void testMapDecisionResult()
	  {
		assertThat(businessRuleTask.CamundaMapDecisionResult).isEqualTo(TEST_STRING_XML);
		businessRuleTask.CamundaMapDecisionResult = TEST_STRING_API;
		assertThat(businessRuleTask.CamundaMapDecisionResult).isEqualTo(TEST_STRING_API);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskPriority()
	  public virtual void testTaskPriority()
	  {
		assertThat(businessRuleTask.CamundaTaskPriority).isEqualTo(TEST_STRING_XML);
		businessRuleTask.CamundaTaskPriority = TEST_SERVICE_TASK_PRIORITY;
		assertThat(businessRuleTask.CamundaTaskPriority).isEqualTo(TEST_SERVICE_TASK_PRIORITY);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCandidateGroups()
	  public virtual void testCandidateGroups()
	  {
		assertThat(userTask.CamundaCandidateGroups).isEqualTo(TEST_GROUPS_XML);
		assertThat(userTask.CamundaCandidateGroupsList).containsAll(TEST_GROUPS_LIST_XML);
		userTask.CamundaCandidateGroups = TEST_GROUPS_API;
		assertThat(userTask.CamundaCandidateGroups).isEqualTo(TEST_GROUPS_API);
		assertThat(userTask.CamundaCandidateGroupsList).containsAll(TEST_GROUPS_LIST_API);
		userTask.CamundaCandidateGroupsList = TEST_GROUPS_LIST_XML;
		assertThat(userTask.CamundaCandidateGroups).isEqualTo(TEST_GROUPS_XML);
		assertThat(userTask.CamundaCandidateGroupsList).containsAll(TEST_GROUPS_LIST_XML);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCandidateStarterGroups()
	  public virtual void testCandidateStarterGroups()
	  {
		assertThat(process.CamundaCandidateStarterGroups).isEqualTo(TEST_GROUPS_XML);
		assertThat(process.CamundaCandidateStarterGroupsList).containsAll(TEST_GROUPS_LIST_XML);
		process.CamundaCandidateStarterGroups = TEST_GROUPS_API;
		assertThat(process.CamundaCandidateStarterGroups).isEqualTo(TEST_GROUPS_API);
		assertThat(process.CamundaCandidateStarterGroupsList).containsAll(TEST_GROUPS_LIST_API);
		process.CamundaCandidateStarterGroupsList = TEST_GROUPS_LIST_XML;
		assertThat(process.CamundaCandidateStarterGroups).isEqualTo(TEST_GROUPS_XML);
		assertThat(process.CamundaCandidateStarterGroupsList).containsAll(TEST_GROUPS_LIST_XML);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCandidateStarterUsers()
	  public virtual void testCandidateStarterUsers()
	  {
		assertThat(process.CamundaCandidateStarterUsers).isEqualTo(TEST_USERS_XML);
		assertThat(process.CamundaCandidateStarterUsersList).containsAll(TEST_USERS_LIST_XML);
		process.CamundaCandidateStarterUsers = TEST_USERS_API;
		assertThat(process.CamundaCandidateStarterUsers).isEqualTo(TEST_USERS_API);
		assertThat(process.CamundaCandidateStarterUsersList).containsAll(TEST_USERS_LIST_API);
		process.CamundaCandidateStarterUsersList = TEST_USERS_LIST_XML;
		assertThat(process.CamundaCandidateStarterUsers).isEqualTo(TEST_USERS_XML);
		assertThat(process.CamundaCandidateStarterUsersList).containsAll(TEST_USERS_LIST_XML);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCandidateUsers()
	  public virtual void testCandidateUsers()
	  {
		assertThat(userTask.CamundaCandidateUsers).isEqualTo(TEST_USERS_XML);
		assertThat(userTask.CamundaCandidateUsersList).containsAll(TEST_USERS_LIST_XML);
		userTask.CamundaCandidateUsers = TEST_USERS_API;
		assertThat(userTask.CamundaCandidateUsers).isEqualTo(TEST_USERS_API);
		assertThat(userTask.CamundaCandidateUsersList).containsAll(TEST_USERS_LIST_API);
		userTask.CamundaCandidateUsersList = TEST_USERS_LIST_XML;
		assertThat(userTask.CamundaCandidateUsers).isEqualTo(TEST_USERS_XML);
		assertThat(userTask.CamundaCandidateUsersList).containsAll(TEST_USERS_LIST_XML);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testClass()
	  public virtual void testClass()
	  {
		assertThat(serviceTask.CamundaClass).isEqualTo(TEST_CLASS_XML);
		assertThat(messageEventDefinition.CamundaClass).isEqualTo(TEST_CLASS_XML);

		serviceTask.CamundaClass = TEST_CLASS_API;
		messageEventDefinition.CamundaClass = TEST_CLASS_API;

		assertThat(serviceTask.CamundaClass).isEqualTo(TEST_CLASS_API);
		assertThat(messageEventDefinition.CamundaClass).isEqualTo(TEST_CLASS_API);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDelegateExpression()
	  public virtual void testDelegateExpression()
	  {
		assertThat(serviceTask.CamundaDelegateExpression).isEqualTo(TEST_DELEGATE_EXPRESSION_XML);
		assertThat(messageEventDefinition.CamundaDelegateExpression).isEqualTo(TEST_DELEGATE_EXPRESSION_XML);

		serviceTask.CamundaDelegateExpression = TEST_DELEGATE_EXPRESSION_API;
		messageEventDefinition.CamundaDelegateExpression = TEST_DELEGATE_EXPRESSION_API;

		assertThat(serviceTask.CamundaDelegateExpression).isEqualTo(TEST_DELEGATE_EXPRESSION_API);
		assertThat(messageEventDefinition.CamundaDelegateExpression).isEqualTo(TEST_DELEGATE_EXPRESSION_API);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDueDate()
	  public virtual void testDueDate()
	  {
		assertThat(userTask.CamundaDueDate).isEqualTo(TEST_DUE_DATE_XML);
		userTask.CamundaDueDate = TEST_DUE_DATE_API;
		assertThat(userTask.CamundaDueDate).isEqualTo(TEST_DUE_DATE_API);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testErrorCodeVariable()
	  public virtual void testErrorCodeVariable()
	  {
		ErrorEventDefinition errorEventDefinition = startEvent.getChildElementsByType(typeof(ErrorEventDefinition)).GetEnumerator().next();
		assertThat(errorEventDefinition.getAttributeValueNs(@namespace, CAMUNDA_ATTRIBUTE_ERROR_CODE_VARIABLE)).isEqualTo("errorVariable");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testErrorMessageVariable()
	  public virtual void testErrorMessageVariable()
	  {
		ErrorEventDefinition errorEventDefinition = startEvent.getChildElementsByType(typeof(ErrorEventDefinition)).GetEnumerator().next();
		assertThat(errorEventDefinition.getAttributeValueNs(@namespace, CAMUNDA_ATTRIBUTE_ERROR_MESSAGE_VARIABLE)).isEqualTo("errorMessageVariable");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExclusive()
	  public virtual void testExclusive()
	  {
		assertThat(startEvent.CamundaExclusive).True;
		assertThat(userTask.CamundaExclusive).False;
		userTask.CamundaExclusive = true;
		assertThat(userTask.CamundaExclusive).True;
		assertThat(parallelGateway.CamundaExclusive).True;
		parallelGateway.CamundaExclusive = false;
		assertThat(parallelGateway.CamundaExclusive).False;

		assertThat(callActivity.CamundaExclusive).False;
		callActivity.CamundaExclusive = true;
		assertThat(callActivity.CamundaExclusive).True;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExpression()
	  public virtual void testExpression()
	  {
		assertThat(serviceTask.CamundaExpression).isEqualTo(TEST_EXPRESSION_XML);
		assertThat(messageEventDefinition.CamundaExpression).isEqualTo(TEST_EXPRESSION_XML);
		serviceTask.CamundaExpression = TEST_EXPRESSION_API;
		messageEventDefinition.CamundaExpression = TEST_EXPRESSION_API;
		assertThat(serviceTask.CamundaExpression).isEqualTo(TEST_EXPRESSION_API);
		assertThat(messageEventDefinition.CamundaExpression).isEqualTo(TEST_EXPRESSION_API);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFormHandlerClass()
	  public virtual void testFormHandlerClass()
	  {
		assertThat(startEvent.CamundaFormHandlerClass).isEqualTo(TEST_CLASS_XML);
		assertThat(userTask.CamundaFormHandlerClass).isEqualTo(TEST_CLASS_XML);
		startEvent.CamundaFormHandlerClass = TEST_CLASS_API;
		userTask.CamundaFormHandlerClass = TEST_CLASS_API;
		assertThat(startEvent.CamundaFormHandlerClass).isEqualTo(TEST_CLASS_API);
		assertThat(userTask.CamundaFormHandlerClass).isEqualTo(TEST_CLASS_API);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFormKey()
	  public virtual void testFormKey()
	  {
		assertThat(startEvent.CamundaFormKey).isEqualTo(TEST_STRING_XML);
		assertThat(userTask.CamundaFormKey).isEqualTo(TEST_STRING_XML);
		startEvent.CamundaFormKey = TEST_STRING_API;
		userTask.CamundaFormKey = TEST_STRING_API;
		assertThat(startEvent.CamundaFormKey).isEqualTo(TEST_STRING_API);
		assertThat(userTask.CamundaFormKey).isEqualTo(TEST_STRING_API);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInitiator()
	  public virtual void testInitiator()
	  {
		assertThat(startEvent.CamundaInitiator).isEqualTo(TEST_STRING_XML);
		startEvent.CamundaInitiator = TEST_STRING_API;
		assertThat(startEvent.CamundaInitiator).isEqualTo(TEST_STRING_API);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPriority()
	  public virtual void testPriority()
	  {
		assertThat(userTask.CamundaPriority).isEqualTo(TEST_PRIORITY_XML);
		userTask.CamundaPriority = TEST_PRIORITY_API;
		assertThat(userTask.CamundaPriority).isEqualTo(TEST_PRIORITY_API);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testResultVariable()
	  public virtual void testResultVariable()
	  {
		assertThat(serviceTask.CamundaResultVariable).isEqualTo(TEST_STRING_XML);
		assertThat(messageEventDefinition.CamundaResultVariable).isEqualTo(TEST_STRING_XML);
		serviceTask.CamundaResultVariable = TEST_STRING_API;
		messageEventDefinition.CamundaResultVariable = TEST_STRING_API;
		assertThat(serviceTask.CamundaResultVariable).isEqualTo(TEST_STRING_API);
		assertThat(messageEventDefinition.CamundaResultVariable).isEqualTo(TEST_STRING_API);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testType()
	  public virtual void testType()
	  {
		assertThat(serviceTask.CamundaType).isEqualTo(TEST_TYPE_XML);
		assertThat(messageEventDefinition.CamundaType).isEqualTo(TEST_STRING_XML);
		serviceTask.CamundaType = TEST_TYPE_API;
		messageEventDefinition.CamundaType = TEST_STRING_API;
		assertThat(serviceTask.CamundaType).isEqualTo(TEST_TYPE_API);
		assertThat(messageEventDefinition.CamundaType).isEqualTo(TEST_STRING_API);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTopic()
	  public virtual void testTopic()
	  {
		assertThat(serviceTask.CamundaTopic).isEqualTo(TEST_STRING_XML);
		assertThat(messageEventDefinition.CamundaTopic).isEqualTo(TEST_STRING_XML);
		serviceTask.CamundaTopic = TEST_TYPE_API;
		messageEventDefinition.CamundaTopic = TEST_STRING_API;
		assertThat(serviceTask.CamundaTopic).isEqualTo(TEST_TYPE_API);
		assertThat(messageEventDefinition.CamundaTopic).isEqualTo(TEST_STRING_API);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableMappingClass()
	  public virtual void testVariableMappingClass()
	  {
		assertThat(callActivity.CamundaVariableMappingClass).isEqualTo(TEST_CLASS_XML);
		callActivity.CamundaVariableMappingClass = TEST_CLASS_API;
		assertThat(callActivity.CamundaVariableMappingClass).isEqualTo(TEST_CLASS_API);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableMappingDelegateExpression()
	  public virtual void testVariableMappingDelegateExpression()
	  {
		assertThat(callActivity.CamundaVariableMappingDelegateExpression).isEqualTo(TEST_DELEGATE_EXPRESSION_XML);
		callActivity.CamundaVariableMappingDelegateExpression = TEST_DELEGATE_EXPRESSION_API;
		assertThat(callActivity.CamundaVariableMappingDelegateExpression).isEqualTo(TEST_DELEGATE_EXPRESSION_API);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecutionListenerExtension()
	  public virtual void testExecutionListenerExtension()
	  {
		CamundaExecutionListener processListener = process.ExtensionElements.ElementsQuery.filterByType(typeof(CamundaExecutionListener)).singleResult();
		CamundaExecutionListener startEventListener = startEvent.ExtensionElements.ElementsQuery.filterByType(typeof(CamundaExecutionListener)).singleResult();
		CamundaExecutionListener serviceTaskListener = serviceTask.ExtensionElements.ElementsQuery.filterByType(typeof(CamundaExecutionListener)).singleResult();
		assertThat(processListener.CamundaClass).isEqualTo(TEST_CLASS_XML);
		assertThat(processListener.CamundaEvent).isEqualTo(TEST_EXECUTION_EVENT_XML);
		assertThat(startEventListener.CamundaExpression).isEqualTo(TEST_EXPRESSION_XML);
		assertThat(startEventListener.CamundaEvent).isEqualTo(TEST_EXECUTION_EVENT_XML);
		assertThat(serviceTaskListener.CamundaDelegateExpression).isEqualTo(TEST_DELEGATE_EXPRESSION_XML);
		assertThat(serviceTaskListener.CamundaEvent).isEqualTo(TEST_EXECUTION_EVENT_XML);
		processListener.CamundaClass = TEST_CLASS_API;
		processListener.CamundaEvent = TEST_EXECUTION_EVENT_API;
		startEventListener.CamundaExpression = TEST_EXPRESSION_API;
		startEventListener.CamundaEvent = TEST_EXECUTION_EVENT_API;
		serviceTaskListener.CamundaDelegateExpression = TEST_DELEGATE_EXPRESSION_API;
		serviceTaskListener.CamundaEvent = TEST_EXECUTION_EVENT_API;
		assertThat(processListener.CamundaClass).isEqualTo(TEST_CLASS_API);
		assertThat(processListener.CamundaEvent).isEqualTo(TEST_EXECUTION_EVENT_API);
		assertThat(startEventListener.CamundaExpression).isEqualTo(TEST_EXPRESSION_API);
		assertThat(startEventListener.CamundaEvent).isEqualTo(TEST_EXECUTION_EVENT_API);
		assertThat(serviceTaskListener.CamundaDelegateExpression).isEqualTo(TEST_DELEGATE_EXPRESSION_API);
		assertThat(serviceTaskListener.CamundaEvent).isEqualTo(TEST_EXECUTION_EVENT_API);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCamundaScriptExecutionListener()
	  public virtual void testCamundaScriptExecutionListener()
	  {
		CamundaExecutionListener sequenceFlowListener = sequenceFlow.ExtensionElements.ElementsQuery.filterByType(typeof(CamundaExecutionListener)).singleResult();

		CamundaScript script = sequenceFlowListener.CamundaScript;
		assertThat(script.CamundaScriptFormat).isEqualTo("groovy");
		assertThat(script.CamundaResource).Null;
		assertThat(script.TextContent).isEqualTo("println 'Hello World'");

		CamundaScript newScript = modelInstance.newInstance(typeof(CamundaScript));
		newScript.CamundaScriptFormat = "groovy";
		newScript.CamundaResource = "test.groovy";
		sequenceFlowListener.CamundaScript = newScript;

		script = sequenceFlowListener.CamundaScript;
		assertThat(script.CamundaScriptFormat).isEqualTo("groovy");
		assertThat(script.CamundaResource).isEqualTo("test.groovy");
		assertThat(script.TextContent).Empty;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailedJobRetryTimeCycleExtension()
	  public virtual void testFailedJobRetryTimeCycleExtension()
	  {
		CamundaFailedJobRetryTimeCycle timeCycle = sendTask.ExtensionElements.ElementsQuery.filterByType(typeof(CamundaFailedJobRetryTimeCycle)).singleResult();
		assertThat(timeCycle.TextContent).isEqualTo(TEST_STRING_XML);
		timeCycle.TextContent = TEST_STRING_API;
		assertThat(timeCycle.TextContent).isEqualTo(TEST_STRING_API);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFieldExtension()
	  public virtual void testFieldExtension()
	  {
		CamundaField field = sendTask.ExtensionElements.ElementsQuery.filterByType(typeof(CamundaField)).singleResult();
		assertThat(field.CamundaName).isEqualTo(TEST_STRING_XML);
		assertThat(field.CamundaExpression).isEqualTo(TEST_EXPRESSION_XML);
		assertThat(field.CamundaStringValue).isEqualTo(TEST_STRING_XML);
		assertThat(field.CamundaExpressionChild.TextContent).isEqualTo(TEST_EXPRESSION_XML);
		assertThat(field.CamundaString.TextContent).isEqualTo(TEST_STRING_XML);
		field.CamundaName = TEST_STRING_API;
		field.CamundaExpression = TEST_EXPRESSION_API;
		field.CamundaStringValue = TEST_STRING_API;
		field.CamundaExpressionChild.TextContent = TEST_EXPRESSION_API;
		field.CamundaString.TextContent = TEST_STRING_API;
		assertThat(field.CamundaName).isEqualTo(TEST_STRING_API);
		assertThat(field.CamundaExpression).isEqualTo(TEST_EXPRESSION_API);
		assertThat(field.CamundaStringValue).isEqualTo(TEST_STRING_API);
		assertThat(field.CamundaExpressionChild.TextContent).isEqualTo(TEST_EXPRESSION_API);
		assertThat(field.CamundaString.TextContent).isEqualTo(TEST_STRING_API);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFormData()
	  public virtual void testFormData()
	  {
		CamundaFormData formData = userTask.ExtensionElements.ElementsQuery.filterByType(typeof(CamundaFormData)).singleResult();
		CamundaFormField formField = formData.CamundaFormFields.GetEnumerator().next();
		assertThat(formField.CamundaId).isEqualTo(TEST_STRING_XML);
		assertThat(formField.CamundaLabel).isEqualTo(TEST_STRING_XML);
		assertThat(formField.CamundaType).isEqualTo(TEST_STRING_XML);
		assertThat(formField.CamundaDatePattern).isEqualTo(TEST_STRING_XML);
		assertThat(formField.CamundaDefaultValue).isEqualTo(TEST_STRING_XML);
		formField.CamundaId = TEST_STRING_API;
		formField.CamundaLabel = TEST_STRING_API;
		formField.CamundaType = TEST_STRING_API;
		formField.CamundaDatePattern = TEST_STRING_API;
		formField.CamundaDefaultValue = TEST_STRING_API;
		assertThat(formField.CamundaId).isEqualTo(TEST_STRING_API);
		assertThat(formField.CamundaLabel).isEqualTo(TEST_STRING_API);
		assertThat(formField.CamundaType).isEqualTo(TEST_STRING_API);
		assertThat(formField.CamundaDatePattern).isEqualTo(TEST_STRING_API);
		assertThat(formField.CamundaDefaultValue).isEqualTo(TEST_STRING_API);

		CamundaProperty property = formField.CamundaProperties.CamundaProperties.GetEnumerator().next();
		assertThat(property.CamundaId).isEqualTo(TEST_STRING_XML);
		assertThat(property.CamundaValue).isEqualTo(TEST_STRING_XML);
		property.CamundaId = TEST_STRING_API;
		property.CamundaValue = TEST_STRING_API;
		assertThat(property.CamundaId).isEqualTo(TEST_STRING_API);
		assertThat(property.CamundaValue).isEqualTo(TEST_STRING_API);

		CamundaConstraint constraint = formField.CamundaValidation.CamundaConstraints.GetEnumerator().next();
		assertThat(constraint.CamundaName).isEqualTo(TEST_STRING_XML);
		assertThat(constraint.CamundaConfig).isEqualTo(TEST_STRING_XML);
		constraint.CamundaName = TEST_STRING_API;
		constraint.CamundaConfig = TEST_STRING_API;
		assertThat(constraint.CamundaName).isEqualTo(TEST_STRING_API);
		assertThat(constraint.CamundaConfig).isEqualTo(TEST_STRING_API);

		CamundaValue value = formField.CamundaValues.GetEnumerator().next();
		assertThat(value.CamundaId).isEqualTo(TEST_STRING_XML);
		assertThat(value.CamundaName).isEqualTo(TEST_STRING_XML);
		value.CamundaId = TEST_STRING_API;
		value.CamundaName = TEST_STRING_API;
		assertThat(value.CamundaId).isEqualTo(TEST_STRING_API);
		assertThat(value.CamundaName).isEqualTo(TEST_STRING_API);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFormProperty()
	  public virtual void testFormProperty()
	  {
		CamundaFormProperty formProperty = startEvent.ExtensionElements.ElementsQuery.filterByType(typeof(CamundaFormProperty)).singleResult();
		assertThat(formProperty.CamundaId).isEqualTo(TEST_STRING_XML);
		assertThat(formProperty.CamundaName).isEqualTo(TEST_STRING_XML);
		assertThat(formProperty.CamundaType).isEqualTo(TEST_STRING_XML);
		assertThat(formProperty.CamundaRequired).False;
		assertThat(formProperty.CamundaReadable).True;
		assertThat(formProperty.CamundaWriteable).True;
		assertThat(formProperty.CamundaVariable).isEqualTo(TEST_STRING_XML);
		assertThat(formProperty.CamundaExpression).isEqualTo(TEST_EXPRESSION_XML);
		assertThat(formProperty.CamundaDatePattern).isEqualTo(TEST_STRING_XML);
		assertThat(formProperty.CamundaDefault).isEqualTo(TEST_STRING_XML);
		formProperty.CamundaId = TEST_STRING_API;
		formProperty.CamundaName = TEST_STRING_API;
		formProperty.CamundaType = TEST_STRING_API;
		formProperty.CamundaRequired = true;
		formProperty.CamundaReadable = false;
		formProperty.CamundaWriteable = false;
		formProperty.CamundaVariable = TEST_STRING_API;
		formProperty.CamundaExpression = TEST_EXPRESSION_API;
		formProperty.CamundaDatePattern = TEST_STRING_API;
		formProperty.CamundaDefault = TEST_STRING_API;
		assertThat(formProperty.CamundaId).isEqualTo(TEST_STRING_API);
		assertThat(formProperty.CamundaName).isEqualTo(TEST_STRING_API);
		assertThat(formProperty.CamundaType).isEqualTo(TEST_STRING_API);
		assertThat(formProperty.CamundaRequired).True;
		assertThat(formProperty.CamundaReadable).False;
		assertThat(formProperty.CamundaWriteable).False;
		assertThat(formProperty.CamundaVariable).isEqualTo(TEST_STRING_API);
		assertThat(formProperty.CamundaExpression).isEqualTo(TEST_EXPRESSION_API);
		assertThat(formProperty.CamundaDatePattern).isEqualTo(TEST_STRING_API);
		assertThat(formProperty.CamundaDefault).isEqualTo(TEST_STRING_API);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInExtension()
	  public virtual void testInExtension()
	  {
		CamundaIn @in = callActivity.ExtensionElements.ElementsQuery.filterByType(typeof(CamundaIn)).singleResult();
		assertThat(@in.CamundaSource).isEqualTo(TEST_STRING_XML);
		assertThat(@in.CamundaSourceExpression).isEqualTo(TEST_EXPRESSION_XML);
		assertThat(@in.CamundaVariables).isEqualTo(TEST_STRING_XML);
		assertThat(@in.CamundaTarget).isEqualTo(TEST_STRING_XML);
		assertThat(@in.CamundaBusinessKey).isEqualTo(TEST_EXPRESSION_XML);
		assertThat(@in.CamundaLocal).True;
		@in.CamundaSource = TEST_STRING_API;
		@in.CamundaSourceExpression = TEST_EXPRESSION_API;
		@in.CamundaVariables = TEST_STRING_API;
		@in.CamundaTarget = TEST_STRING_API;
		@in.CamundaBusinessKey = TEST_EXPRESSION_API;
		@in.CamundaLocal = false;
		assertThat(@in.CamundaSource).isEqualTo(TEST_STRING_API);
		assertThat(@in.CamundaSourceExpression).isEqualTo(TEST_EXPRESSION_API);
		assertThat(@in.CamundaVariables).isEqualTo(TEST_STRING_API);
		assertThat(@in.CamundaTarget).isEqualTo(TEST_STRING_API);
		assertThat(@in.CamundaBusinessKey).isEqualTo(TEST_EXPRESSION_API);
		assertThat(@in.CamundaLocal).False;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOutExtension()
	  public virtual void testOutExtension()
	  {
		CamundaOut @out = callActivity.ExtensionElements.ElementsQuery.filterByType(typeof(CamundaOut)).singleResult();
		assertThat(@out.CamundaSource).isEqualTo(TEST_STRING_XML);
		assertThat(@out.CamundaSourceExpression).isEqualTo(TEST_EXPRESSION_XML);
		assertThat(@out.CamundaVariables).isEqualTo(TEST_STRING_XML);
		assertThat(@out.CamundaTarget).isEqualTo(TEST_STRING_XML);
		assertThat(@out.CamundaLocal).True;
		@out.CamundaSource = TEST_STRING_API;
		@out.CamundaSourceExpression = TEST_EXPRESSION_API;
		@out.CamundaVariables = TEST_STRING_API;
		@out.CamundaTarget = TEST_STRING_API;
		@out.CamundaLocal = false;
		assertThat(@out.CamundaSource).isEqualTo(TEST_STRING_API);
		assertThat(@out.CamundaSourceExpression).isEqualTo(TEST_EXPRESSION_API);
		assertThat(@out.CamundaVariables).isEqualTo(TEST_STRING_API);
		assertThat(@out.CamundaTarget).isEqualTo(TEST_STRING_API);
		assertThat(@out.CamundaLocal).False;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPotentialStarter()
	  public virtual void testPotentialStarter()
	  {
		CamundaPotentialStarter potentialStarter = startEvent.ExtensionElements.ElementsQuery.filterByType(typeof(CamundaPotentialStarter)).singleResult();
		Expression expression = potentialStarter.ResourceAssignmentExpression.Expression;
		assertThat(expression.TextContent).isEqualTo(TEST_GROUPS_XML);
		expression.TextContent = TEST_GROUPS_API;
		assertThat(expression.TextContent).isEqualTo(TEST_GROUPS_API);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskListener()
	  public virtual void testTaskListener()
	  {
		CamundaTaskListener taskListener = userTask.ExtensionElements.ElementsQuery.filterByType(typeof(CamundaTaskListener)).list().get(0);
		assertThat(taskListener.CamundaEvent).isEqualTo(TEST_TASK_EVENT_XML);
		assertThat(taskListener.CamundaClass).isEqualTo(TEST_CLASS_XML);
		assertThat(taskListener.CamundaExpression).isEqualTo(TEST_EXPRESSION_XML);
		assertThat(taskListener.CamundaDelegateExpression).isEqualTo(TEST_DELEGATE_EXPRESSION_XML);
		taskListener.CamundaEvent = TEST_TASK_EVENT_API;
		taskListener.CamundaClass = TEST_CLASS_API;
		taskListener.CamundaExpression = TEST_EXPRESSION_API;
		taskListener.CamundaDelegateExpression = TEST_DELEGATE_EXPRESSION_API;
		assertThat(taskListener.CamundaEvent).isEqualTo(TEST_TASK_EVENT_API);
		assertThat(taskListener.CamundaClass).isEqualTo(TEST_CLASS_API);
		assertThat(taskListener.CamundaExpression).isEqualTo(TEST_EXPRESSION_API);
		assertThat(taskListener.CamundaDelegateExpression).isEqualTo(TEST_DELEGATE_EXPRESSION_API);

		CamundaField field = taskListener.CamundaFields.GetEnumerator().next();
		assertThat(field.CamundaName).isEqualTo(TEST_STRING_XML);
		assertThat(field.CamundaString.TextContent).isEqualTo(TEST_STRING_XML);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCamundaScriptTaskListener()
	  public virtual void testCamundaScriptTaskListener()
	  {
		CamundaTaskListener taskListener = userTask.ExtensionElements.ElementsQuery.filterByType(typeof(CamundaTaskListener)).list().get(1);

		CamundaScript script = taskListener.CamundaScript;
		assertThat(script.CamundaScriptFormat).isEqualTo("groovy");
		assertThat(script.CamundaResource).isEqualTo("test.groovy");
		assertThat(script.TextContent).Empty;

		CamundaScript newScript = modelInstance.newInstance(typeof(CamundaScript));
		newScript.CamundaScriptFormat = "groovy";
		newScript.TextContent = "println 'Hello World'";
		taskListener.CamundaScript = newScript;

		script = taskListener.CamundaScript;
		assertThat(script.CamundaScriptFormat).isEqualTo("groovy");
		assertThat(script.CamundaResource).Null;
		assertThat(script.TextContent).isEqualTo("println 'Hello World'");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCamundaModelerProperties()
	  public virtual void testCamundaModelerProperties()
	  {
		CamundaProperties camundaProperties = endEvent.ExtensionElements.ElementsQuery.filterByType(typeof(CamundaProperties)).singleResult();
		assertThat(camundaProperties).NotNull;
		assertThat(camundaProperties.getCamundaProperties()).hasSize(2);

		foreach (CamundaProperty camundaProperty in camundaProperties.getCamundaProperties())
		{
		  assertThat(camundaProperty.CamundaId).Null;
		  assertThat(camundaProperty.CamundaName).StartsWith("name");
		  assertThat(camundaProperty.CamundaValue).StartsWith("value");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetNonExistingCamundaCandidateUsers()
	  public virtual void testGetNonExistingCamundaCandidateUsers()
	  {
		userTask.removeAttributeNs(@namespace, "candidateUsers");
		assertThat(userTask.CamundaCandidateUsers).Null;
		assertThat(userTask.CamundaCandidateUsersList).Empty;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetNullCamundaCandidateUsers()
	  public virtual void testSetNullCamundaCandidateUsers()
	  {
		assertThat(userTask.CamundaCandidateUsers).NotEmpty;
		assertThat(userTask.CamundaCandidateUsersList).NotEmpty;
		userTask.CamundaCandidateUsers = null;
		assertThat(userTask.CamundaCandidateUsers).Null;
		assertThat(userTask.CamundaCandidateUsersList).Empty;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyCamundaCandidateUsers()
	  public virtual void testEmptyCamundaCandidateUsers()
	  {
		assertThat(userTask.CamundaCandidateUsers).NotEmpty;
		assertThat(userTask.CamundaCandidateUsersList).NotEmpty;
		userTask.CamundaCandidateUsers = "";
		assertThat(userTask.CamundaCandidateUsers).Null;
		assertThat(userTask.CamundaCandidateUsersList).Empty;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetNullCamundaCandidateUsersList()
	  public virtual void testSetNullCamundaCandidateUsersList()
	  {
		assertThat(userTask.CamundaCandidateUsers).NotEmpty;
		assertThat(userTask.CamundaCandidateUsersList).NotEmpty;
		userTask.CamundaCandidateUsersList = null;
		assertThat(userTask.CamundaCandidateUsers).Null;
		assertThat(userTask.CamundaCandidateUsersList).Empty;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyCamundaCandidateUsersList()
	  public virtual void testEmptyCamundaCandidateUsersList()
	  {
		assertThat(userTask.CamundaCandidateUsers).NotEmpty;
		assertThat(userTask.CamundaCandidateUsersList).NotEmpty;
		userTask.CamundaCandidateUsersList = System.Linq.Enumerable.Empty<string>();
		assertThat(userTask.CamundaCandidateUsers).Null;
		assertThat(userTask.CamundaCandidateUsersList).Empty;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testScriptResource()
	  public virtual void testScriptResource()
	  {
		assertThat(scriptTask.ScriptFormat).isEqualTo("groovy");
		assertThat(scriptTask.CamundaResource).isEqualTo("test.groovy");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCamundaConnector()
	  public virtual void testCamundaConnector()
	  {
		CamundaConnector camundaConnector = serviceTask.ExtensionElements.ElementsQuery.filterByType(typeof(CamundaConnector)).singleResult();
		assertThat(camundaConnector).NotNull;

		CamundaConnectorId camundaConnectorId = camundaConnector.CamundaConnectorId;
		assertThat(camundaConnectorId).NotNull;
		assertThat(camundaConnectorId.TextContent).isEqualTo("soap-http-connector");

		CamundaInputOutput camundaInputOutput = camundaConnector.CamundaInputOutput;

		ICollection<CamundaInputParameter> inputParameters = camundaInputOutput.CamundaInputParameters;
		assertThat(inputParameters).hasSize(1);

		CamundaInputParameter inputParameter = inputParameters.GetEnumerator().next();
		assertThat(inputParameter.CamundaName).isEqualTo("endpointUrl");
		assertThat(inputParameter.TextContent).isEqualTo("http://example.com/webservice");

		ICollection<CamundaOutputParameter> outputParameters = camundaInputOutput.CamundaOutputParameters;
		assertThat(outputParameters).hasSize(1);

		CamundaOutputParameter outputParameter = outputParameters.GetEnumerator().next();
		assertThat(outputParameter.CamundaName).isEqualTo("result");
		assertThat(outputParameter.TextContent).isEqualTo("output");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCamundaInputOutput()
	  public virtual void testCamundaInputOutput()
	  {
		CamundaInputOutput camundaInputOutput = serviceTask.ExtensionElements.ElementsQuery.filterByType(typeof(CamundaInputOutput)).singleResult();
		assertThat(camundaInputOutput).NotNull;
		assertThat(camundaInputOutput.CamundaInputParameters).hasSize(6);
		assertThat(camundaInputOutput.CamundaOutputParameters).hasSize(1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCamundaInputParameter()
	  public virtual void testCamundaInputParameter()
	  {
		// find existing
		CamundaInputParameter inputParameter = findInputParameterByName(serviceTask, "shouldBeConstant");

		// modify existing
		inputParameter.CamundaName = "hello";
		inputParameter.TextContent = "world";
		inputParameter = findInputParameterByName(serviceTask, "hello");
		assertThat(inputParameter.TextContent).isEqualTo("world");

		// add new one
		inputParameter = modelInstance.newInstance(typeof(CamundaInputParameter));
		inputParameter.CamundaName = "abc";
		inputParameter.TextContent = "def";
		serviceTask.ExtensionElements.ElementsQuery.filterByType(typeof(CamundaInputOutput)).singleResult().addChildElement(inputParameter);

		// search for new one
		inputParameter = findInputParameterByName(serviceTask, "abc");
		assertThat(inputParameter.CamundaName).isEqualTo("abc");
		assertThat(inputParameter.TextContent).isEqualTo("def");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCamundaNullInputParameter()
	  public virtual void testCamundaNullInputParameter()
	  {
		CamundaInputParameter inputParameter = findInputParameterByName(serviceTask, "shouldBeNull");
		assertThat(inputParameter.CamundaName).isEqualTo("shouldBeNull");
		assertThat(inputParameter.TextContent).Empty;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCamundaConstantInputParameter()
	  public virtual void testCamundaConstantInputParameter()
	  {
		CamundaInputParameter inputParameter = findInputParameterByName(serviceTask, "shouldBeConstant");
		assertThat(inputParameter.CamundaName).isEqualTo("shouldBeConstant");
		assertThat(inputParameter.TextContent).isEqualTo("foo");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCamundaExpressionInputParameter()
	  public virtual void testCamundaExpressionInputParameter()
	  {
		CamundaInputParameter inputParameter = findInputParameterByName(serviceTask, "shouldBeExpression");
		assertThat(inputParameter.CamundaName).isEqualTo("shouldBeExpression");
		assertThat(inputParameter.TextContent).isEqualTo("${1 + 1}");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCamundaListInputParameter()
	  public virtual void testCamundaListInputParameter()
	  {
		CamundaInputParameter inputParameter = findInputParameterByName(serviceTask, "shouldBeList");
		assertThat(inputParameter.CamundaName).isEqualTo("shouldBeList");
		assertThat(inputParameter.TextContent).NotEmpty;
		assertThat(inputParameter.getUniqueChildElementByNameNs(CAMUNDA_NS, "list")).NotNull;

		CamundaList list = inputParameter.Value;
		assertThat(list.Values).hasSize(3);
		foreach (BpmnModelElementInstance values in list.Values)
		{
		  assertThat(values.TextContent).isIn("a", "b", "c");
		}

		list = modelInstance.newInstance(typeof(CamundaList));
		for (int i = 0; i < 4; i++)
		{
		  CamundaValue value = modelInstance.newInstance(typeof(CamundaValue));
		  value.TextContent = "test";
		  list.Values.Add(value);
		}
		ICollection<CamundaValue> testValues = Arrays.asList(modelInstance.newInstance(typeof(CamundaValue)), modelInstance.newInstance(typeof(CamundaValue)));
		list.Values.addAll(testValues);
		inputParameter.Value = list;

		list = inputParameter.Value;
		assertThat(list.Values).hasSize(6);
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
		list.Values.removeAll(testValues);
		List<BpmnModelElementInstance> camundaValues = new List<BpmnModelElementInstance>(list.Values);
		assertThat(camundaValues).hasSize(4);
		foreach (BpmnModelElementInstance value in camundaValues)
		{
		  assertThat(value.TextContent).isEqualTo("test");
		}

		list.Values.remove(camundaValues[1]);
		assertThat(list.Values).hasSize(3);

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
		list.Values.removeAll(Arrays.asList(camundaValues[0], camundaValues[3]));
		assertThat(list.Values).hasSize(1);

		list.Values.Clear();
		assertThat(list.Values).Empty;

		// test standard list interactions
		ICollection<BpmnModelElementInstance> elements = list.Values;

		CamundaValue value = modelInstance.newInstance(typeof(CamundaValue));
		elements.Add(value);

		IList<CamundaValue> newValues = new List<CamundaValue>();
		newValues.Add(modelInstance.newInstance(typeof(CamundaValue)));
		newValues.Add(modelInstance.newInstance(typeof(CamundaValue)));
		elements.addAll(newValues);
		assertThat(elements).hasSize(3);

		assertThat(elements).doesNotContain(modelInstance.newInstance(typeof(CamundaValue)));
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
		assertThat(elements.containsAll(Arrays.asList(modelInstance.newInstance(typeof(CamundaValue))))).False;

		assertThat(elements.remove(modelInstance.newInstance(typeof(CamundaValue)))).False;
		assertThat(elements).hasSize(3);

		assertThat(elements.remove(value)).True;
		assertThat(elements).hasSize(2);

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
		assertThat(elements.removeAll(newValues)).True;
		assertThat(elements).Empty;

		elements.Add(modelInstance.newInstance(typeof(CamundaValue)));
		elements.Clear();
		assertThat(elements).Empty;

		inputParameter.removeValue();
		assertThat(inputParameter.Value).Null;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCamundaMapInputParameter()
	  public virtual void testCamundaMapInputParameter()
	  {
		CamundaInputParameter inputParameter = findInputParameterByName(serviceTask, "shouldBeMap");
		assertThat(inputParameter.CamundaName).isEqualTo("shouldBeMap");
		assertThat(inputParameter.TextContent).NotEmpty;
		assertThat(inputParameter.getUniqueChildElementByNameNs(CAMUNDA_NS, "map")).NotNull;

		CamundaMap map = inputParameter.Value;
		assertThat(map.CamundaEntries).hasSize(2);
		foreach (CamundaEntry entry in map.CamundaEntries)
		{
		  if (entry.CamundaKey.Equals("foo"))
		  {
			assertThat(entry.TextContent).isEqualTo("bar");
		  }
		  else
		  {
			assertThat(entry.CamundaKey).isEqualTo("hello");
			assertThat(entry.TextContent).isEqualTo("world");
		  }
		}

		map = modelInstance.newInstance(typeof(CamundaMap));
		CamundaEntry entry = modelInstance.newInstance(typeof(CamundaEntry));
		entry.CamundaKey = "test";
		entry.TextContent = "value";
		map.CamundaEntries.Add(entry);

		inputParameter.Value = map;
		map = inputParameter.Value;
		assertThat(map.CamundaEntries).hasSize(1);
		entry = map.CamundaEntries.GetEnumerator().next();
		assertThat(entry.CamundaKey).isEqualTo("test");
		assertThat(entry.TextContent).isEqualTo("value");

		ICollection<CamundaEntry> entries = map.CamundaEntries;
		entries.Add(modelInstance.newInstance(typeof(CamundaEntry)));
		assertThat(entries).hasSize(2);

		inputParameter.removeValue();
		assertThat(inputParameter.Value).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCamundaScriptInputParameter()
	  public virtual void testCamundaScriptInputParameter()
	  {
		CamundaInputParameter inputParameter = findInputParameterByName(serviceTask, "shouldBeScript");
		assertThat(inputParameter.CamundaName).isEqualTo("shouldBeScript");
		assertThat(inputParameter.TextContent).NotEmpty;
		assertThat(inputParameter.getUniqueChildElementByNameNs(CAMUNDA_NS, "script")).NotNull;
		assertThat(inputParameter.getUniqueChildElementByType(typeof(CamundaScript))).NotNull;

		CamundaScript script = inputParameter.Value;
		assertThat(script.CamundaScriptFormat).isEqualTo("groovy");
		assertThat(script.CamundaResource).Null;
		assertThat(script.TextContent).isEqualTo("1 + 1");

		script = modelInstance.newInstance(typeof(CamundaScript));
		script.CamundaScriptFormat = "python";
		script.CamundaResource = "script.py";

		inputParameter.Value = script;

		script = inputParameter.Value;
		assertThat(script.CamundaScriptFormat).isEqualTo("python");
		assertThat(script.CamundaResource).isEqualTo("script.py");
		assertThat(script.TextContent).Empty;

		inputParameter.removeValue();
		assertThat(inputParameter.Value).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCamundaNestedOutputParameter()
	  public virtual void testCamundaNestedOutputParameter()
	  {
		CamundaOutputParameter camundaOutputParameter = serviceTask.ExtensionElements.ElementsQuery.filterByType(typeof(CamundaInputOutput)).singleResult().CamundaOutputParameters.GetEnumerator().next();

		assertThat(camundaOutputParameter).NotNull;
		assertThat(camundaOutputParameter.CamundaName).isEqualTo("nested");
		CamundaList list = camundaOutputParameter.Value;
		assertThat(list).NotNull;
		assertThat(list.Values).hasSize(2);
		IEnumerator<BpmnModelElementInstance> iterator = list.Values.GetEnumerator();

		// nested list
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		CamundaList nestedList = (CamundaList) iterator.next().getUniqueChildElementByType(typeof(CamundaList));
		assertThat(nestedList).NotNull;
		assertThat(nestedList.Values).hasSize(2);
		foreach (BpmnModelElementInstance value in nestedList.Values)
		{
		  assertThat(value.TextContent).isEqualTo("list");
		}

		// nested map
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		CamundaMap nestedMap = (CamundaMap) iterator.next().getUniqueChildElementByType(typeof(CamundaMap));
		assertThat(nestedMap).NotNull;
		assertThat(nestedMap.CamundaEntries).hasSize(2);
		IEnumerator<CamundaEntry> mapIterator = nestedMap.CamundaEntries.GetEnumerator();

		// nested list in nested map
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		CamundaEntry nestedListEntry = mapIterator.next();
		assertThat(nestedListEntry).NotNull;
		assertThat(nestedListEntry.CamundaKey).isEqualTo("list");
		CamundaList nestedNestedList = nestedListEntry.Value;
		foreach (BpmnModelElementInstance value in nestedNestedList.Values)
		{
		  assertThat(value.TextContent).isEqualTo("map");
		}

		// nested map in nested map
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		CamundaEntry nestedMapEntry = mapIterator.next();
		assertThat(nestedMapEntry).NotNull;
		assertThat(nestedMapEntry.CamundaKey).isEqualTo("map");
		CamundaMap nestedNestedMap = nestedMapEntry.Value;
		CamundaEntry entry = nestedNestedMap.CamundaEntries.GetEnumerator().next();
		assertThat(entry.CamundaKey).isEqualTo("so");
		assertThat(entry.TextContent).isEqualTo("nested");
	  }

	  protected internal virtual CamundaInputParameter findInputParameterByName(BaseElement baseElement, string name)
	  {
		ICollection<CamundaInputParameter> camundaInputParameters = baseElement.ExtensionElements.ElementsQuery.filterByType(typeof(CamundaInputOutput)).singleResult().CamundaInputParameters;
		foreach (CamundaInputParameter camundaInputParameter in camundaInputParameters)
		{
		  if (camundaInputParameter.CamundaName.Equals(name))
		  {
			return camundaInputParameter;
		  }
		}
		throw new BpmnModelException("Unable to find camunda:inputParameter with name '" + name + "' for element with id '" + baseElement.Id + "'");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void validateModel()
	  public virtual void validateModel()
	  {
		Bpmn.validateModel(modelInstance);
	  }
	}

}