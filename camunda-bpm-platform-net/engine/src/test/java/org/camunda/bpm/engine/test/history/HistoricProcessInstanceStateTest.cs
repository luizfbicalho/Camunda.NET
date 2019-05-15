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
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using EndEvent = org.camunda.bpm.model.bpmn.instance.EndEvent;
	using TerminateEventDefinition = org.camunda.bpm.model.bpmn.instance.TerminateEventDefinition;
	using Ignore = org.junit.Ignore;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.fail;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.IsNull.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.IsNull.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	/// <summary>
	/// @author Askar Akhmerov
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_ACTIVITY)]
	public class HistoricProcessInstanceStateTest
	{
		private bool InstanceFieldsInitialized = false;

		public HistoricProcessInstanceStateTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			processEngineTestRule = new ProcessEngineTestRule(processEngineRule);
			ruleChain = RuleChain.outerRule(processEngineTestRule).around(processEngineRule);
		}


	  public const string TERMINATION = "termination";
	  public const string PROCESS_ID = "process1";
	  public const string REASON = "very important reason";

	  public ProcessEngineRule processEngineRule = new ProvidedProcessEngineRule();
	  public ProcessEngineTestRule processEngineTestRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(processEngineTestRule).around(processEngineRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTerminatedInternalWithGateway()
	  public virtual void testTerminatedInternalWithGateway()
	  {
		BpmnModelInstance instance = Bpmn.createExecutableProcess(PROCESS_ID).startEvent().parallelGateway().endEvent().moveToLastGateway().endEvent(TERMINATION).done();
		initEndEvent(instance, TERMINATION);
		ProcessDefinition processDefinition = processEngineTestRule.deployAndGetDefinition(instance);
		processEngineRule.RuntimeService.startProcessInstanceById(processDefinition.Id);
		HistoricProcessInstance entity = getHistoricProcessInstanceWithAssertion(processDefinition);
		assertThat(entity.State, @is(org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_COMPLETED));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompletedOnEndEvent()
	  public virtual void testCompletedOnEndEvent()
	  {
		BpmnModelInstance instance = Bpmn.createExecutableProcess(PROCESS_ID).startEvent().endEvent().done();
		ProcessDefinition processDefinition = processEngineTestRule.deployAndGetDefinition(instance);
		processEngineRule.RuntimeService.startProcessInstanceById(processDefinition.Id);
		HistoricProcessInstance entity = getHistoricProcessInstanceWithAssertion(processDefinition);
		assertThat(entity.State, @is(org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_COMPLETED));
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompletionWithSuspension()
	  public virtual void testCompletionWithSuspension()
	  {
		BpmnModelInstance instance = Bpmn.createExecutableProcess(PROCESS_ID).startEvent().userTask().endEvent().done();
		ProcessDefinition processDefinition = processEngineTestRule.deployAndGetDefinition(instance);
		ProcessInstance processInstance = processEngineRule.RuntimeService.startProcessInstanceById(processDefinition.Id);
		HistoricProcessInstance entity = getHistoricProcessInstanceWithAssertion(processDefinition);
		assertThat(entity.State, @is(org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_ACTIVE));

		//suspend
		processEngineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessInstanceId(processInstance.Id).suspend();

		entity = getHistoricProcessInstanceWithAssertion(processDefinition);
		assertThat(entity.State, @is(org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_SUSPENDED));

		//activate
		processEngineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessInstanceId(processInstance.Id).activate();

		entity = getHistoricProcessInstanceWithAssertion(processDefinition);
		assertThat(entity.State, @is(org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_ACTIVE));

		//complete task
		processEngineRule.TaskService.complete(processEngineRule.TaskService.createTaskQuery().active().singleResult().Id);

		//make sure happy path ended
		entity = getHistoricProcessInstanceWithAssertion(processDefinition);
		assertThat(entity.State, @is(org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_COMPLETED));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspensionByProcessDefinition()
	  public virtual void testSuspensionByProcessDefinition()
	  {
		BpmnModelInstance instance = Bpmn.createExecutableProcess(PROCESS_ID).startEvent().userTask().endEvent().done();
		ProcessDefinition processDefinition = processEngineTestRule.deployAndGetDefinition(instance);
		ProcessInstance processInstance1 = processEngineRule.RuntimeService.startProcessInstanceById(processDefinition.Id);

		ProcessInstance processInstance2 = processEngineRule.RuntimeService.startProcessInstanceById(processDefinition.Id);

		//suspend all
		processEngineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionId(processDefinition.Id).suspend();

		HistoricProcessInstance hpi1 = processEngineRule.HistoryService.createHistoricProcessInstanceQuery().processInstanceId(processInstance1.Id).singleResult();

		HistoricProcessInstance hpi2 = processEngineRule.HistoryService.createHistoricProcessInstanceQuery().processInstanceId(processInstance2.Id).singleResult();

		assertThat(hpi1.State, @is(org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_SUSPENDED));
		assertThat(hpi2.State, @is(org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_SUSPENDED));
		assertEquals(2, processEngineRule.HistoryService.createHistoricProcessInstanceQuery().suspended().count());

		//activate all
		processEngineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(processDefinition.Key).activate();

		hpi1 = processEngineRule.HistoryService.createHistoricProcessInstanceQuery().processInstanceId(processInstance1.Id).singleResult();

		hpi2 = processEngineRule.HistoryService.createHistoricProcessInstanceQuery().processInstanceId(processInstance2.Id).singleResult();

		assertThat(hpi1.State, @is(org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_ACTIVE));
		assertThat(hpi2.State, @is(org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_ACTIVE));
		assertEquals(2, processEngineRule.HistoryService.createHistoricProcessInstanceQuery().active().count());
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCancellationState()
	  public virtual void testCancellationState()
	  {
		BpmnModelInstance instance = Bpmn.createExecutableProcess(PROCESS_ID).startEvent().userTask().endEvent().done();
		ProcessDefinition processDefinition = processEngineTestRule.deployAndGetDefinition(instance);
		ProcessInstance processInstance = processEngineRule.RuntimeService.startProcessInstanceById(processDefinition.Id);
		HistoricProcessInstance entity = getHistoricProcessInstanceWithAssertion(processDefinition);
		assertThat(entity.State, @is(org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_ACTIVE));

		//same call as in ProcessInstanceResourceImpl
		processEngineRule.RuntimeService.deleteProcessInstance(processInstance.Id, REASON, false, true);
		entity = getHistoricProcessInstanceWithAssertion(processDefinition);
		assertThat(entity.State, @is(org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_EXTERNALLY_TERMINATED));
		assertEquals(1, processEngineRule.HistoryService.createHistoricProcessInstanceQuery().externallyTerminated().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSateOfScriptTaskProcessWithTransactionCommitAndException()
	  public virtual void testSateOfScriptTaskProcessWithTransactionCommitAndException()
	  {
		BpmnModelInstance instance = Bpmn.createExecutableProcess(PROCESS_ID).startEvent().camundaAsyncAfter().scriptTask().scriptText("throw new RuntimeException()").scriptFormat("groovy").endEvent().done();
		ProcessDefinition processDefinition = processEngineTestRule.deployAndGetDefinition(instance);


		try
		{
		  ProcessInstance pi = processEngineRule.RuntimeService.startProcessInstanceById(processDefinition.Id);
		  processEngineRule.ManagementService.executeJob(processEngineRule.ManagementService.createJobQuery().executable().singleResult().Id);
		  fail("exception expected");
		}
		catch (Exception)
		{
		  //expected
		}

		assertThat(processEngineRule.RuntimeService.createProcessInstanceQuery().active().list().size(), @is(1));
		HistoricProcessInstance entity = getHistoricProcessInstanceWithAssertion(processDefinition);
		assertThat(entity.State, @is(org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_ACTIVE));
		assertEquals(1, processEngineRule.HistoryService.createHistoricProcessInstanceQuery().active().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testErrorEndEvent()
	  public virtual void testErrorEndEvent()
	  {
		BpmnModelInstance process1 = Bpmn.createExecutableProcess(PROCESS_ID).startEvent().endEvent().error("1").done();

		ProcessDefinition processDefinition = processEngineTestRule.deployAndGetDefinition(process1);
		processEngineRule.RuntimeService.startProcessInstanceById(processDefinition.Id);
		HistoricProcessInstance entity = getHistoricProcessInstanceWithAssertion(processDefinition);
		assertThat(entity.State, @is(org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_COMPLETED));
		assertEquals(1, processEngineRule.HistoryService.createHistoricProcessInstanceQuery().completed().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/history/HistoricProcessInstanceStateTest.testWithCallActivity.bpmn"}) public void testWithCallActivity()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricProcessInstanceStateTest.testWithCallActivity.bpmn"})]
	  public virtual void testWithCallActivity()
	  {
		processEngineRule.RuntimeService.startProcessInstanceByKey("Main_Process");
		assertThat(processEngineRule.RuntimeService.createProcessInstanceQuery().active().list().size(), @is(0));

		HistoricProcessInstance entity1 = processEngineRule.HistoryService.createHistoricProcessInstanceQuery().processDefinitionKey("Main_Process").singleResult();

		HistoricProcessInstance entity2 = processEngineRule.HistoryService.createHistoricProcessInstanceQuery().processDefinitionKey("Sub_Process").singleResult();

		assertThat(entity1, @is(notNullValue()));
		assertThat(entity2, @is(notNullValue()));
		assertThat(entity1.State, @is(org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_COMPLETED));
		assertEquals(1, processEngineRule.HistoryService.createHistoricProcessInstanceQuery().completed().count());
		assertThat(entity2.State, @is(org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_INTERNALLY_TERMINATED));
		assertEquals(1, processEngineRule.HistoryService.createHistoricProcessInstanceQuery().internallyTerminated().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Ignore("CAM-9934") @Deployment(resources = {"org/camunda/bpm/engine/test/history/CAM-9934.bpmn"}) public void test()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/CAM-9934.bpmn"})]
	  public virtual void test()
	  {
		// given
		processEngineRule.RuntimeService.startProcessInstanceByKey("Process_1");

		string jobId = processEngineRule.ManagementService.createJobQuery().timers().executable().singleResult().Id;

		// when
		processEngineRule.ManagementService.executeJob(jobId);

		HistoricProcessInstance historicProcessInstance = processEngineRule.HistoryService.createHistoricProcessInstanceQuery().singleResult();

		// then
		assertThat(historicProcessInstance.State, @is(org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_ACTIVE));
		assertThat(historicProcessInstance.EndTime, nullValue());
	  }

	  private HistoricProcessInstance getHistoricProcessInstanceWithAssertion(ProcessDefinition processDefinition)
	  {
		IList<HistoricProcessInstance> entities = processEngineRule.HistoryService.createHistoricProcessInstanceQuery().processDefinitionId(processDefinition.Id).list();
		assertThat(entities, @is(notNullValue()));
		assertThat(entities.Count, @is(1));
		return entities[0];
	  }

	  protected internal static void initEndEvent(BpmnModelInstance modelInstance, string endEventId)
	  {
		EndEvent endEvent = modelInstance.getModelElementById(endEventId);
		TerminateEventDefinition terminateDefinition = modelInstance.newInstance(typeof(TerminateEventDefinition));
		endEvent.addChildElement(terminateDefinition);
	  }
	}

}