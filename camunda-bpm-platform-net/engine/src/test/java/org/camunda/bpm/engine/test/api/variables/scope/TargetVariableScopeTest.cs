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
namespace org.camunda.bpm.engine.test.api.variables.scope
{
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using DelegateTask = org.camunda.bpm.engine.@delegate.DelegateTask;
	using ProcessInstanceWithVariablesImpl = org.camunda.bpm.engine.impl.persistence.entity.ProcessInstanceWithVariablesImpl;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using SequenceFlow = org.camunda.bpm.model.bpmn.instance.SequenceFlow;
	using CamundaExecutionListener = org.camunda.bpm.model.bpmn.instance.camunda.CamundaExecutionListener;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.startsWith;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.IsNull.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	/// <summary>
	/// @author Askar Akhmerov
	/// @author Tassilo Weidner
	/// </summary>
	public class TargetVariableScopeTest
	{
		private bool InstanceFieldsInitialized = false;

		public TargetVariableScopeTest()
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
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.ProcessEngineRule(true);
	  public ProcessEngineRule engineRule = new ProcessEngineRule(true);
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.util.ProcessEngineTestRule testHelper = new org.camunda.bpm.engine.test.util.ProcessEngineTestRule(engineRule);
	  public ProcessEngineTestRule testHelper;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/variables/scope/TargetVariableScopeTest.testExecutionWithDelegateProcess.bpmn","org/camunda/bpm/engine/test/api/variables/scope/doer.bpmn"}) public void testExecutionWithDelegateProcess()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/variables/scope/TargetVariableScopeTest.testExecutionWithDelegateProcess.bpmn","org/camunda/bpm/engine/test/api/variables/scope/doer.bpmn"})]
	  public virtual void testExecutionWithDelegateProcess()
	  {
		// Given we create a new process instance
		VariableMap variables = Variables.createVariables().putValue("orderIds", Arrays.asList(new int[]{1, 2, 3}));
		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceByKey("Process_MultiInstanceCallAcitivity",variables);

		// it runs without any problems
		assertThat(processInstance.Ended,@is(true));
		assertThat(((ProcessInstanceWithVariablesImpl) processInstance).Variables.containsKey("targetOrderId"),@is(false));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/variables/scope/TargetVariableScopeTest.testExecutionWithScriptTargetScope.bpmn","org/camunda/bpm/engine/test/api/variables/scope/doer.bpmn"}) public void testExecutionWithScriptTargetScope()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/variables/scope/TargetVariableScopeTest.testExecutionWithScriptTargetScope.bpmn","org/camunda/bpm/engine/test/api/variables/scope/doer.bpmn"})]
	  public virtual void testExecutionWithScriptTargetScope()
	  {
		VariableMap variables = Variables.createVariables().putValue("orderIds", Arrays.asList(new int[]{1, 2, 3}));
		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceByKey("Process_MultiInstanceCallAcitivity",variables);

		// it runs without any problems
		assertThat(processInstance.Ended,@is(true));
		assertThat(((ProcessInstanceWithVariablesImpl) processInstance).Variables.containsKey("targetOrderId"),@is(false));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/variables/scope/TargetVariableScopeTest.testExecutionWithoutProperTargetScope.bpmn","org/camunda/bpm/engine/test/api/variables/scope/doer.bpmn"}) public void testExecutionWithoutProperTargetScope()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/variables/scope/TargetVariableScopeTest.testExecutionWithoutProperTargetScope.bpmn","org/camunda/bpm/engine/test/api/variables/scope/doer.bpmn"})]
	  public virtual void testExecutionWithoutProperTargetScope()
	  {
		VariableMap variables = Variables.createVariables().putValue("orderIds", Arrays.asList(new int[]{1, 2, 3}));
		//fails due to inappropriate variable scope target
		thrown.expect(typeof(ScriptEvaluationException));
		ProcessDefinition processDefinition = engineRule.RepositoryService.createProcessDefinitionQuery().processDefinitionKey("Process_MultiInstanceCallAcitivity").singleResult();
		thrown.expectMessage(startsWith("Unable to evaluate script while executing activity 'CallActivity_1' in the process definition with id '" + processDefinition.Id + "': org.camunda.bpm.engine.ProcessEngineException: ENGINE-20011 Scope with specified activity Id NOT_EXISTING and execution"));
		engineRule.RuntimeService.startProcessInstanceByKey("Process_MultiInstanceCallAcitivity",variables);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/variables/scope/doer.bpmn"}) public void testWithDelegateVariableMapping()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/variables/scope/doer.bpmn"})]
	  public virtual void testWithDelegateVariableMapping()
	  {
		BpmnModelInstance instance = Bpmn.createExecutableProcess("process1").startEvent().subProcess("SubProcess_1").embeddedSubProcess().startEvent().callActivity().calledElement("Process_StuffDoer").camundaVariableMappingClass("org.camunda.bpm.engine.test.api.variables.scope.SetVariableMappingDelegate").serviceTask().camundaClass("org.camunda.bpm.engine.test.api.variables.scope.AssertVariableScopeDelegate").endEvent().subProcessDone().endEvent().done();
		instance = modify(instance).activityBuilder("SubProcess_1").multiInstance().parallel().camundaCollection("orderIds").camundaElementVariable("orderId").done();

		ProcessDefinition processDefinition = testHelper.deployAndGetDefinition(instance);
		VariableMap variables = Variables.createVariables().putValue("orderIds", Arrays.asList(new int[]{1, 2, 3}));
		engineRule.RuntimeService.startProcessInstanceById(processDefinition.Id,variables);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/variables/scope/doer.bpmn"}) public void testWithDelegateVariableMappingAndChildScope()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/variables/scope/doer.bpmn"})]
	  public virtual void testWithDelegateVariableMappingAndChildScope()
	  {
		BpmnModelInstance instance = Bpmn.createExecutableProcess("process1").startEvent().parallelGateway().subProcess("SubProcess_1").embeddedSubProcess().startEvent().callActivity().calledElement("Process_StuffDoer").camundaVariableMappingClass("org.camunda.bpm.engine.test.api.variables.scope.SetVariableToChildMappingDelegate").serviceTask().camundaClass("org.camunda.bpm.engine.test.api.variables.scope.AssertVariableScopeDelegate").endEvent().subProcessDone().moveToLastGateway().subProcess("SubProcess_2").embeddedSubProcess().startEvent().userTask("ut").endEvent().subProcessDone().endEvent().done();
		instance = modify(instance).activityBuilder("SubProcess_1").multiInstance().parallel().camundaCollection("orderIds").camundaElementVariable("orderId").done();

		ProcessDefinition processDefinition = testHelper.deployAndGetDefinition(instance);
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage(startsWith("org.camunda.bpm.engine.ProcessEngineException: ENGINE-20011 Scope with specified activity Id SubProcess_2 and execution"));
		VariableMap variables = Variables.createVariables().putValue("orderIds", Arrays.asList(new int[]{1, 2, 3}));
		engineRule.RuntimeService.startProcessInstanceById(processDefinition.Id,variables);
	  }

	  public class JavaDelegate : org.camunda.bpm.engine.@delegate.JavaDelegate
	  {

		public virtual void execute(DelegateExecution execution)
		{
		  execution.setVariable("varName", "varValue", "activityId");
		  assertThat(execution.getVariableLocal("varName"), @is(notNullValue()));
		}

	  }

	  public class ExecutionListener : org.camunda.bpm.engine.@delegate.ExecutionListener
	  {

		public virtual void notify(DelegateExecution execution)
		{
		  execution.setVariable("varName", "varValue", "activityId");
		  assertThat(execution.getVariableLocal("varName"), @is(notNullValue()));
		}

	  }

	  public class TaskListener : org.camunda.bpm.engine.@delegate.TaskListener
	  {

		public virtual void notify(DelegateTask delegateTask)
		{
		  DelegateExecution execution = delegateTask.Execution;
		  execution.setVariable("varName", "varValue", "activityId");
		  assertThat(execution.getVariableLocal("varName"), @is(notNullValue()));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetLocalScopeWithJavaDelegate()
	  public virtual void testSetLocalScopeWithJavaDelegate()
	  {
		testHelper.deploy(Bpmn.createExecutableProcess("process").startEvent().serviceTask().id("activityId").camundaClass(typeof(JavaDelegate)).endEvent().done());

		engineRule.RuntimeService.startProcessInstanceByKey("process");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetLocalScopeWithExecutionListenerStart()
	  public virtual void testSetLocalScopeWithExecutionListenerStart()
	  {
		testHelper.deploy(Bpmn.createExecutableProcess("process").startEvent().id("activityId").camundaExecutionListenerClass(ExecutionListener.EVENTNAME_START, typeof(ExecutionListener)).endEvent().done());

		engineRule.RuntimeService.startProcessInstanceByKey("process");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetLocalScopeWithExecutionListenerEnd()
	  public virtual void testSetLocalScopeWithExecutionListenerEnd()
	  {
		testHelper.deploy(Bpmn.createExecutableProcess("process").startEvent().endEvent().id("activityId").camundaExecutionListenerClass(ExecutionListener.EVENTNAME_END, typeof(ExecutionListener)).done());

		engineRule.RuntimeService.startProcessInstanceByKey("process");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetLocalScopeWithExecutionListenerTake()
	  public virtual void testSetLocalScopeWithExecutionListenerTake()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess("process").startEvent().id("activityId").sequenceFlowId("sequenceFlow").endEvent().done();

		CamundaExecutionListener listener = modelInstance.newInstance(typeof(CamundaExecutionListener));
		listener.CamundaEvent = ExecutionListener.EVENTNAME_TAKE;
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		listener.CamundaClass = typeof(ExecutionListener).FullName;
		modelInstance.getModelElementById<SequenceFlow>("sequenceFlow").builder().addExtensionElement(listener);

		testHelper.deploy(modelInstance);
		engineRule.RuntimeService.startProcessInstanceByKey("process");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetLocalScopeWithTaskListener()
	  public virtual void testSetLocalScopeWithTaskListener()
	  {
		testHelper.deploy(Bpmn.createExecutableProcess("process").startEvent().userTask().id("activityId").camundaTaskListenerClass(TaskListener.EVENTNAME_CREATE, typeof(TaskListener)).endEvent().done());

		engineRule.RuntimeService.startProcessInstanceByKey("process");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetLocalScopeInSubprocessWithJavaDelegate()
	  public virtual void testSetLocalScopeInSubprocessWithJavaDelegate()
	  {
		testHelper.deploy(Bpmn.createExecutableProcess("process").startEvent().subProcess().embeddedSubProcess().startEvent().serviceTask().id("activityId").camundaClass(typeof(JavaDelegate)).endEvent().subProcessDone().endEvent().done());

		engineRule.RuntimeService.startProcessInstanceByKey("process");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetLocalScopeInSubprocessWithStartExecutionListener()
	  public virtual void testSetLocalScopeInSubprocessWithStartExecutionListener()
	  {
		testHelper.deploy(Bpmn.createExecutableProcess("process").startEvent().subProcess().embeddedSubProcess().startEvent().id("activityId").camundaExecutionListenerClass(ExecutionListener.EVENTNAME_START, typeof(ExecutionListener)).endEvent().subProcessDone().endEvent().done());

		engineRule.RuntimeService.startProcessInstanceByKey("process");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetLocalScopeInSubprocessWithEndExecutionListener()
	  public virtual void testSetLocalScopeInSubprocessWithEndExecutionListener()
	  {
		testHelper.deploy(Bpmn.createExecutableProcess("process").startEvent().subProcess().embeddedSubProcess().startEvent().endEvent().id("activityId").camundaExecutionListenerClass(ExecutionListener.EVENTNAME_END, typeof(ExecutionListener)).subProcessDone().endEvent().done());

		engineRule.RuntimeService.startProcessInstanceByKey("process");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetLocalScopeInSubprocessWithTaskListener()
	  public virtual void testSetLocalScopeInSubprocessWithTaskListener()
	  {
		testHelper.deploy(Bpmn.createExecutableProcess("process").startEvent().subProcess().embeddedSubProcess().startEvent().userTask().id("activityId").camundaTaskListenerClass(TaskListener.EVENTNAME_CREATE, typeof(TaskListener)).endEvent().subProcessDone().endEvent().done());

		engineRule.RuntimeService.startProcessInstanceByKey("process");
	  }

	}

}