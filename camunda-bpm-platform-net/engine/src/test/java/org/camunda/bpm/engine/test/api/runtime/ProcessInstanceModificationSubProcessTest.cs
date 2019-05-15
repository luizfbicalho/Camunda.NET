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
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ExecutionAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ExecutionAssert.describeExecutionTree;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ExecutionTree = org.camunda.bpm.engine.test.util.ExecutionTree;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Ignore = org.junit.Ignore;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Svetlana Dorokhova.
	/// </summary>
	public class ProcessInstanceModificationSubProcessTest
	{
		private bool InstanceFieldsInitialized = false;

		public ProcessInstanceModificationSubProcessTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testHelper = new ProcessEngineTestRule(rule);
			ruleChain = RuleChain.outerRule(rule).around(testHelper);
		}


	  protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(rule).around(testHelper);
	  public RuleChain ruleChain;

	  private RuntimeService runtimeService;
	  private RepositoryService repositoryService;
	  private TaskService taskService;
	  private HistoryService historyService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		repositoryService = rule.RepositoryService;
		runtimeService = rule.RuntimeService;
		taskService = rule.TaskService;
		historyService = rule.HistoryService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Ignore("CAM-9354") @Test public void shouldHaveEqualParentActivityInstanceId()
	  public virtual void shouldHaveEqualParentActivityInstanceId()
	  {
		// given
		testHelper.deploy(Bpmn.createExecutableProcess("process").startEvent().subProcess("subprocess").embeddedSubProcess().startEvent().scriptTask("scriptTaskInSubprocess").scriptFormat("groovy").scriptText("throw new org.camunda.bpm.engine.delegate.BpmnError(\"anErrorCode\");").userTask().endEvent().subProcessDone().endEvent().moveToActivity("subprocess").boundaryEvent("boundary").error("anErrorCode").userTask("userTaskAfterBoundaryEvent").endEvent().done());

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		// when
		runtimeService.createModification(processInstance.ProcessDefinitionId).startAfterActivity("scriptTaskInSubprocess").processInstanceIds(processInstance.Id).execute();

		ActivityInstance activityInstance = runtimeService.getActivityInstance(processInstance.Id).getActivityInstances("subprocess")[0];

		HistoricActivityInstance historicActivityInstance = historyService.createHistoricActivityInstanceQuery().activityId("subprocess").unfinished().singleResult();

		// assume
		assertNotNull(activityInstance);
		assertNotNull(historicActivityInstance);

		// then
		assertEquals(historicActivityInstance.ParentActivityInstanceId, activityInstance.ParentActivityInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCompleteParentProcess()
	  public virtual void shouldCompleteParentProcess()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance parentProcessInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess("parentProcess").startEvent().callActivity("callActivity").calledElement("subprocess").endEvent().done();
		BpmnModelInstance parentProcessInstance = Bpmn.createExecutableProcess("parentProcess").startEvent().callActivity("callActivity").calledElement("subprocess").endEvent().done();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance subprocessInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess("subprocess").startEvent().userTask("userTask").endEvent("subEnd").done();
		BpmnModelInstance subprocessInstance = Bpmn.createExecutableProcess("subprocess").startEvent().userTask("userTask").endEvent("subEnd").done();

		testHelper.deploy(parentProcessInstance, subprocessInstance);

		// given I start the process, which wait as user task in subprocess
		runtimeService.startProcessInstanceByKey("parentProcess");

		assertNotNull(taskService.createTaskQuery().taskName("userTask").singleResult());

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.runtime.ProcessInstance subprocess = runtimeService.createProcessInstanceQuery().processDefinitionKey("subprocess").singleResult();
		ProcessInstance subprocess = runtimeService.createProcessInstanceQuery().processDefinitionKey("subprocess").singleResult();
		assertNotNull(subprocess);

		// when I do process instance modification
		runtimeService.createProcessInstanceModification(subprocess.ProcessInstanceId).cancelAllForActivity("userTask").startAfterActivity("userTask").execute();

		// then the process should be finished
		assertThat(runtimeService.createProcessInstanceQuery().count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldContinueParentProcess()
	  public virtual void shouldContinueParentProcess()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance parentProcessInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess("parentProcess").startEvent().callActivity("callActivity").calledElement("subprocess").userTask().endEvent().done();
		BpmnModelInstance parentProcessInstance = Bpmn.createExecutableProcess("parentProcess").startEvent().callActivity("callActivity").calledElement("subprocess").userTask().endEvent().done();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance subprocessInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess("subprocess").startEvent().userTask("userTask").endEvent("subEnd").done();
		BpmnModelInstance subprocessInstance = Bpmn.createExecutableProcess("subprocess").startEvent().userTask("userTask").endEvent("subEnd").done();

		testHelper.deploy(parentProcessInstance, subprocessInstance);

		// given I start the process, which wait as user task in subprocess
		ProcessInstance parentPI = runtimeService.startProcessInstanceByKey("parentProcess");

		assertNotNull(taskService.createTaskQuery().taskName("userTask").singleResult());

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.runtime.ProcessInstance subprocess = runtimeService.createProcessInstanceQuery().processDefinitionKey("subprocess").singleResult();
		ProcessInstance subprocess = runtimeService.createProcessInstanceQuery().processDefinitionKey("subprocess").singleResult();
		assertNotNull(subprocess);

		// when I do process instance modification
		runtimeService.createProcessInstanceModification(subprocess.ProcessInstanceId).cancelAllForActivity("userTask").startAfterActivity("userTask").execute();

		// then the parent process instance is still active
		assertThat(runtimeService.createProcessInstanceQuery().count(), @is(1L));

		Task task = taskService.createTaskQuery().singleResult();
		assertThat(task.ProcessInstanceId, @is(parentPI.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCompleteParentProcessWithParallelGateway()
	  public virtual void shouldCompleteParentProcessWithParallelGateway()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess("parentProcess").startEvent().parallelGateway().serviceTask("doNothingServiceTask").camundaExpression("${true}").moveToLastGateway().callActivity("callActivity").calledElement("subprocess").parallelGateway("mergingParallelGateway").endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess("parentProcess").startEvent().parallelGateway().serviceTask("doNothingServiceTask").camundaExpression("${true}").moveToLastGateway().callActivity("callActivity").calledElement("subprocess").parallelGateway("mergingParallelGateway").endEvent().done();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance parentProcessInstance = modify(modelInstance).flowNodeBuilder("doNothingServiceTask").connectTo("mergingParallelGateway").done();
		BpmnModelInstance parentProcessInstance = modify(modelInstance).flowNodeBuilder("doNothingServiceTask").connectTo("mergingParallelGateway").done();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance subprocessInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess("subprocess").startEvent().userTask("userTask").endEvent("subEnd").done();
		BpmnModelInstance subprocessInstance = Bpmn.createExecutableProcess("subprocess").startEvent().userTask("userTask").endEvent("subEnd").done();

		testHelper.deploy(parentProcessInstance, subprocessInstance);

		// given I start the process, which waits at user task in subprocess
		runtimeService.startProcessInstanceByKey("parentProcess");

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.runtime.ProcessInstance subprocess = runtimeService.createProcessInstanceQuery().processDefinitionKey("subprocess").singleResult();
		ProcessInstance subprocess = runtimeService.createProcessInstanceQuery().processDefinitionKey("subprocess").singleResult();
		assertNotNull(subprocess);

		assertNotNull(taskService.createTaskQuery().taskName("userTask").singleResult());

		// when I do process instance modification
		runtimeService.createProcessInstanceModification(subprocess.ProcessInstanceId).cancelAllForActivity("userTask").startAfterActivity("userTask").execute();

		// then the process should be finished
		assertThat(runtimeService.createProcessInstanceQuery().count(), @is(0L));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldContinueParentProcessWithParallelGateway()
	  public virtual void shouldContinueParentProcessWithParallelGateway()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess("parentProcess").startEvent().parallelGateway().serviceTask("doNothingServiceTask").camundaExpression("${true}").moveToLastGateway().callActivity("callActivity").calledElement("subprocess").parallelGateway("mergingParallelGateway").userTask().endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess("parentProcess").startEvent().parallelGateway().serviceTask("doNothingServiceTask").camundaExpression("${true}").moveToLastGateway().callActivity("callActivity").calledElement("subprocess").parallelGateway("mergingParallelGateway").userTask().endEvent().done();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance parentProcessInstance = modify(modelInstance).flowNodeBuilder("doNothingServiceTask").connectTo("mergingParallelGateway").done();
		BpmnModelInstance parentProcessInstance = modify(modelInstance).flowNodeBuilder("doNothingServiceTask").connectTo("mergingParallelGateway").done();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance subprocessInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess("subprocess").startEvent().userTask("userTask").endEvent("subEnd").done();
		BpmnModelInstance subprocessInstance = Bpmn.createExecutableProcess("subprocess").startEvent().userTask("userTask").endEvent("subEnd").done();

		testHelper.deploy(parentProcessInstance, subprocessInstance);

		// given I start the process, which waits at user task in subprocess
		ProcessInstance parentPI = runtimeService.startProcessInstanceByKey("parentProcess");

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.runtime.ProcessInstance subprocess = runtimeService.createProcessInstanceQuery().processDefinitionKey("subprocess").singleResult();
		ProcessInstance subprocess = runtimeService.createProcessInstanceQuery().processDefinitionKey("subprocess").singleResult();
		assertNotNull(subprocess);

		assertNotNull(taskService.createTaskQuery().taskName("userTask").singleResult());

		// when I do process instance modification
		runtimeService.createProcessInstanceModification(subprocess.ProcessInstanceId).cancelAllForActivity("userTask").startAfterActivity("userTask").execute();

		// then the parent process instance is still active
		assertThat(runtimeService.createProcessInstanceQuery().count(), @is(1L));

		Task task = taskService.createTaskQuery().singleResult();
		assertThat(task.ProcessInstanceId, @is(parentPI.Id));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCompleteParentProcessWithMultiInstance()
	  public virtual void shouldCompleteParentProcessWithMultiInstance()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance parentProcessInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess("parentProcess").startEvent().callActivity("callActivity").calledElement("subprocess").multiInstance().cardinality("3").multiInstanceDone().endEvent().done();
		BpmnModelInstance parentProcessInstance = Bpmn.createExecutableProcess("parentProcess").startEvent().callActivity("callActivity").calledElement("subprocess").multiInstance().cardinality("3").multiInstanceDone().endEvent().done();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance subprocessInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess("subprocess").startEvent().userTask("userTask").endEvent("subEnd").done();
		BpmnModelInstance subprocessInstance = Bpmn.createExecutableProcess("subprocess").startEvent().userTask("userTask").endEvent("subEnd").done();

		testHelper.deploy(parentProcessInstance, subprocessInstance);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String subprocessPrDefId = repositoryService.createProcessDefinitionQuery().processDefinitionKey("subprocess").singleResult().getId();
		string subprocessPrDefId = repositoryService.createProcessDefinitionQuery().processDefinitionKey("subprocess").singleResult().Id;

		// given I start the process, which waits at user task inside multiinstance subprocess
		runtimeService.startProcessInstanceByKey("parentProcess");

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.runtime.ProcessInstance> subprocesses = runtimeService.createProcessInstanceQuery().processDefinitionKey("subprocess").list();
		IList<ProcessInstance> subprocesses = runtimeService.createProcessInstanceQuery().processDefinitionKey("subprocess").list();
		assertEquals(3, subprocesses.Count);

		// when I do process instance modification
		runtimeService.createModification(subprocessPrDefId).cancelAllForActivity("userTask").startAfterActivity("userTask").processInstanceIds(collectIds(subprocesses)).execute();

		// then the process should be finished
		assertThat(runtimeService.createProcessInstanceQuery().count(), @is(0L));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldContinueParentProcessWithMultiInstance()
	  public virtual void shouldContinueParentProcessWithMultiInstance()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance parentProcessInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess("parentProcess").startEvent().callActivity("callActivity").calledElement("subprocess").multiInstance().cardinality("3").multiInstanceDone().userTask().endEvent().done();
		BpmnModelInstance parentProcessInstance = Bpmn.createExecutableProcess("parentProcess").startEvent().callActivity("callActivity").calledElement("subprocess").multiInstance().cardinality("3").multiInstanceDone().userTask().endEvent().done();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance subprocessInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess("subprocess").startEvent().userTask("userTask").endEvent("subEnd").done();
		BpmnModelInstance subprocessInstance = Bpmn.createExecutableProcess("subprocess").startEvent().userTask("userTask").endEvent("subEnd").done();

		testHelper.deploy(parentProcessInstance, subprocessInstance);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String subprocessPrDefId = repositoryService.createProcessDefinitionQuery().processDefinitionKey("subprocess").singleResult().getId();
		string subprocessPrDefId = repositoryService.createProcessDefinitionQuery().processDefinitionKey("subprocess").singleResult().Id;

		// given I start the process, which waits at user task inside multiinstance subprocess
		ProcessInstance parentPI = runtimeService.startProcessInstanceByKey("parentProcess");

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.runtime.ProcessInstance> subprocesses = runtimeService.createProcessInstanceQuery().processDefinitionKey("subprocess").list();
		IList<ProcessInstance> subprocesses = runtimeService.createProcessInstanceQuery().processDefinitionKey("subprocess").list();
		assertEquals(3, subprocesses.Count);

		// when I do process instance modification
		runtimeService.createModification(subprocessPrDefId).cancelAllForActivity("userTask").startAfterActivity("userTask").processInstanceIds(collectIds(subprocesses)).execute();

		// then the parent process instance is still active
		assertThat(runtimeService.createProcessInstanceQuery().count(), @is(1L));

		Task task = taskService.createTaskQuery().singleResult();
		assertThat(task.ProcessInstanceId, @is(parentPI.Id));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCompleteParentProcessWithMultiInstanceInsideEmbeddedSubProcess()
	  public virtual void shouldCompleteParentProcessWithMultiInstanceInsideEmbeddedSubProcess()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance parentProcessInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess("parentProcess").startEvent().subProcess().embeddedSubProcess().startEvent().callActivity("callActivity").calledElement("subprocess").multiInstance().cardinality("3").multiInstanceDone().endEvent().subProcessDone().endEvent().done();
		BpmnModelInstance parentProcessInstance = Bpmn.createExecutableProcess("parentProcess").startEvent().subProcess().embeddedSubProcess().startEvent().callActivity("callActivity").calledElement("subprocess").multiInstance().cardinality("3").multiInstanceDone().endEvent().subProcessDone().endEvent().done();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance subprocessInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess("subprocess").startEvent().userTask("userTask").endEvent("subEnd").done();
		BpmnModelInstance subprocessInstance = Bpmn.createExecutableProcess("subprocess").startEvent().userTask("userTask").endEvent("subEnd").done();

		testHelper.deploy(parentProcessInstance, subprocessInstance);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String subprocessPrDefId = repositoryService.createProcessDefinitionQuery().processDefinitionKey("subprocess").singleResult().getId();
		string subprocessPrDefId = repositoryService.createProcessDefinitionQuery().processDefinitionKey("subprocess").singleResult().Id;

		// given I start the process, which waits at user task inside multiinstance subprocess
		runtimeService.startProcessInstanceByKey("parentProcess");

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.runtime.ProcessInstance> subprocesses = runtimeService.createProcessInstanceQuery().processDefinitionKey("subprocess").list();
		IList<ProcessInstance> subprocesses = runtimeService.createProcessInstanceQuery().processDefinitionKey("subprocess").list();
		assertEquals(3, subprocesses.Count);

		// when I do process instance modification
		runtimeService.createModification(subprocessPrDefId).cancelAllForActivity("userTask").startAfterActivity("userTask").processInstanceIds(collectIds(subprocesses)).execute();

		// then the process should be finished
		assertThat(runtimeService.createProcessInstanceQuery().count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldContinueParentProcessWithMultiInstanceInsideEmbeddedSubProcess()
	  public virtual void shouldContinueParentProcessWithMultiInstanceInsideEmbeddedSubProcess()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance parentProcessInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess("parentProcess").startEvent().subProcess().embeddedSubProcess().startEvent().callActivity("callActivity").calledElement("subprocess").multiInstance().cardinality("3").multiInstanceDone().endEvent().subProcessDone().userTask().endEvent().done();
		BpmnModelInstance parentProcessInstance = Bpmn.createExecutableProcess("parentProcess").startEvent().subProcess().embeddedSubProcess().startEvent().callActivity("callActivity").calledElement("subprocess").multiInstance().cardinality("3").multiInstanceDone().endEvent().subProcessDone().userTask().endEvent().done();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance subprocessInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess("subprocess").startEvent().userTask("userTask").endEvent("subEnd").done();
		BpmnModelInstance subprocessInstance = Bpmn.createExecutableProcess("subprocess").startEvent().userTask("userTask").endEvent("subEnd").done();

		testHelper.deploy(parentProcessInstance, subprocessInstance);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String subprocessPrDefId = repositoryService.createProcessDefinitionQuery().processDefinitionKey("subprocess").singleResult().getId();
		string subprocessPrDefId = repositoryService.createProcessDefinitionQuery().processDefinitionKey("subprocess").singleResult().Id;

		// given I start the process, which waits at user task inside multiinstance subprocess
		ProcessInstance parentPI = runtimeService.startProcessInstanceByKey("parentProcess");

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.runtime.ProcessInstance> subprocesses = runtimeService.createProcessInstanceQuery().processDefinitionKey("subprocess").list();
		IList<ProcessInstance> subprocesses = runtimeService.createProcessInstanceQuery().processDefinitionKey("subprocess").list();
		assertEquals(3, subprocesses.Count);

		// when I do process instance modification
		runtimeService.createModification(subprocessPrDefId).cancelAllForActivity("userTask").startAfterActivity("userTask").processInstanceIds(collectIds(subprocesses)).execute();

		// then the parent process instance is still active
		assertThat(runtimeService.createProcessInstanceQuery().count(), @is(1L));

		Task task = taskService.createTaskQuery().singleResult();
		assertThat(task.ProcessInstanceId, @is(parentPI.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCompleteParentProcessWithMultiInstanceEmbeddedSubProcess()
	  public virtual void shouldCompleteParentProcessWithMultiInstanceEmbeddedSubProcess()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance parentProcessInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess("parentProcess").startEvent().subProcess().embeddedSubProcess().startEvent().callActivity("callActivity").calledElement("subprocess").endEvent().subProcessDone().multiInstance().cardinality("3").multiInstanceDone().endEvent().done();
		BpmnModelInstance parentProcessInstance = Bpmn.createExecutableProcess("parentProcess").startEvent().subProcess().embeddedSubProcess().startEvent().callActivity("callActivity").calledElement("subprocess").endEvent().subProcessDone().multiInstance().cardinality("3").multiInstanceDone().endEvent().done();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance subprocessInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess("subprocess").startEvent().userTask("userTask").endEvent("subEnd").done();
		BpmnModelInstance subprocessInstance = Bpmn.createExecutableProcess("subprocess").startEvent().userTask("userTask").endEvent("subEnd").done();

		testHelper.deploy(parentProcessInstance, subprocessInstance);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String subprocessPrDefId = repositoryService.createProcessDefinitionQuery().processDefinitionKey("subprocess").singleResult().getId();
		string subprocessPrDefId = repositoryService.createProcessDefinitionQuery().processDefinitionKey("subprocess").singleResult().Id;

		// given I start the process, which waits at user task inside multiinstance subprocess
		runtimeService.startProcessInstanceByKey("parentProcess");

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.runtime.ProcessInstance> subprocesses = runtimeService.createProcessInstanceQuery().processDefinitionKey("subprocess").list();
		IList<ProcessInstance> subprocesses = runtimeService.createProcessInstanceQuery().processDefinitionKey("subprocess").list();
		assertEquals(3, subprocesses.Count);

		// when I do process instance modification
		runtimeService.createModification(subprocessPrDefId).cancelAllForActivity("userTask").startAfterActivity("userTask").processInstanceIds(collectIds(subprocesses)).execute();

		// then the process should be finished
		assertThat(runtimeService.createProcessInstanceQuery().count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldContinueParentProcessWithMultiInstanceEmbeddedSubProcess()
	  public virtual void shouldContinueParentProcessWithMultiInstanceEmbeddedSubProcess()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance parentProcessInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess("parentProcess").startEvent().subProcess().embeddedSubProcess().startEvent().callActivity("callActivity").calledElement("subprocess").endEvent().subProcessDone().multiInstance().cardinality("3").multiInstanceDone().userTask().endEvent().done();
		BpmnModelInstance parentProcessInstance = Bpmn.createExecutableProcess("parentProcess").startEvent().subProcess().embeddedSubProcess().startEvent().callActivity("callActivity").calledElement("subprocess").endEvent().subProcessDone().multiInstance().cardinality("3").multiInstanceDone().userTask().endEvent().done();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance subprocessInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess("subprocess").startEvent().userTask("userTask").endEvent("subEnd").done();
		BpmnModelInstance subprocessInstance = Bpmn.createExecutableProcess("subprocess").startEvent().userTask("userTask").endEvent("subEnd").done();

		testHelper.deploy(parentProcessInstance, subprocessInstance);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String subprocessPrDefId = repositoryService.createProcessDefinitionQuery().processDefinitionKey("subprocess").singleResult().getId();
		string subprocessPrDefId = repositoryService.createProcessDefinitionQuery().processDefinitionKey("subprocess").singleResult().Id;

		// given I start the process, which waits at user task inside multiinstance subprocess
		ProcessInstance parentPI = runtimeService.startProcessInstanceByKey("parentProcess");

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.runtime.ProcessInstance> subprocesses = runtimeService.createProcessInstanceQuery().processDefinitionKey("subprocess").list();
		IList<ProcessInstance> subprocesses = runtimeService.createProcessInstanceQuery().processDefinitionKey("subprocess").list();
		assertEquals(3, subprocesses.Count);

		// when I do process instance modification
		runtimeService.createModification(subprocessPrDefId).cancelAllForActivity("userTask").startAfterActivity("userTask").processInstanceIds(collectIds(subprocesses)).execute();

		// then the parent process instance is still active
		assertThat(runtimeService.createProcessInstanceQuery().count(), @is(1L));

		Task task = taskService.createTaskQuery().singleResult();
		assertThat(task.ProcessInstanceId, @is(parentPI.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCancelParentProcessWithMultiInstanceCallActivity()
	  public virtual void shouldCancelParentProcessWithMultiInstanceCallActivity()
	  {
		BpmnModelInstance parentProcess = Bpmn.createExecutableProcess("parentProcess").startEvent().callActivity("callActivity").calledElement("subprocess").multiInstance().cardinality("3").multiInstanceDone().endEvent().userTask().endEvent().done();

		BpmnModelInstance subProcess = Bpmn.createExecutableProcess("subprocess").startEvent().userTask("userTask").endEvent("subEnd").done();

		testHelper.deploy(parentProcess, subProcess);
		ProcessDefinition subProcessDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("subprocess").singleResult();

		// given
		runtimeService.startProcessInstanceByKey("parentProcess");

		// assume
		IList<ProcessInstance> subProcessInstances = runtimeService.createProcessInstanceQuery().processDefinitionKey("subprocess").list();
		assertEquals(3, subProcessInstances.Count);

		// when
		runtimeService.createModification(subProcessDefinition.Id).startAfterActivity("userTask").cancelAllForActivity("userTask").processInstanceIds(collectIds(subProcessInstances)).execute();

		// then
		assertThat(runtimeService.createProcessInstanceQuery().count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCancelParentProcessWithCallActivityInMultiInstanceEmbeddedSubprocess()
	  public virtual void shouldCancelParentProcessWithCallActivityInMultiInstanceEmbeddedSubprocess()
	  {
		BpmnModelInstance parentProcess = Bpmn.createExecutableProcess("parentProcess").startEvent().subProcess().embeddedSubProcess().startEvent().callActivity("callActivity").calledElement("subprocess").endEvent().subProcessDone().multiInstance().cardinality("3").multiInstanceDone().endEvent().userTask().endEvent().done();

		BpmnModelInstance subProcess = Bpmn.createExecutableProcess("subprocess").startEvent().userTask("userTask").endEvent("subEnd").done();

		testHelper.deploy(parentProcess, subProcess);
		ProcessDefinition subProcessDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("subprocess").singleResult();

		// given
		runtimeService.startProcessInstanceByKey("parentProcess");

		// assume
		IList<ProcessInstance> subProcessInstances = runtimeService.createProcessInstanceQuery().processDefinitionKey("subprocess").list();
		assertEquals(3, subProcessInstances.Count);

		// when
		runtimeService.createModification(subProcessDefinition.Id).startAfterActivity("userTask").cancelAllForActivity("userTask").processInstanceIds(collectIds(subProcessInstances)).execute();

		// then
		assertThat(runtimeService.createProcessInstanceQuery().count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCancelConcurrentExecutionInCallingProcess()
	  public virtual void shouldCancelConcurrentExecutionInCallingProcess()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance parentProcessInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess("parentProcess").startEvent().parallelGateway("split").callActivity("callActivity").calledElement("subprocess").endEvent().moveToLastGateway().userTask("parentUserTask").endEvent().done();
		BpmnModelInstance parentProcessInstance = Bpmn.createExecutableProcess("parentProcess").startEvent().parallelGateway("split").callActivity("callActivity").calledElement("subprocess").endEvent().moveToLastGateway().userTask("parentUserTask").endEvent().done();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance subprocessInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess("subprocess").startEvent().userTask("childUserTask").endEvent("subEnd").done();
		  BpmnModelInstance subprocessInstance = Bpmn.createExecutableProcess("subprocess").startEvent().userTask("childUserTask").endEvent("subEnd").done();

		  testHelper.deploy(parentProcessInstance, subprocessInstance);

		  ProcessInstance callingInstance = runtimeService.startProcessInstanceByKey("parentProcess");
		  ProcessInstance calledInstance = runtimeService.createProcessInstanceQuery().superProcessInstanceId(callingInstance.Id).singleResult();

		  // when
		  runtimeService.createProcessInstanceModification(calledInstance.Id).cancelAllForActivity("childUserTask").execute();

		  // then
		  ProcessInstance calledInstanceAfterModification = runtimeService.createProcessInstanceQuery().processInstanceId(calledInstance.Id).singleResult();

		  Assert.assertNull(calledInstanceAfterModification);

		  ExecutionTree executionTree = ExecutionTree.forExecution(callingInstance.Id, rule.ProcessEngine);
		  assertThat(executionTree).matches(describeExecutionTree("parentUserTask").scope().done());
	  }


	  private IList<string> collectIds(IList<ProcessInstance> processInstances)
	  {
		IList<string> supbrocessIds = new List<string>();
		foreach (ProcessInstance processInstance in processInstances)
		{
		  supbrocessIds.Add(processInstance.Id);
		}
		return supbrocessIds;
	  }

	}

}