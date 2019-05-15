using System;

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
namespace org.camunda.bpm.engine.test.bpmn.@event.conditional
{
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Ignore = org.junit.Ignore;
	using Test = org.junit.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;

	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class TriggerConditionalEventOnStartAtActivityTest : AbstractConditionalEventTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTriggerGlobalEventSubProcess()
	  public virtual void testTriggerGlobalEventSubProcess()
	  {
		//given
		deployConditionalEventSubProcess(TASK_MODEL, CONDITIONAL_EVENT_PROCESS_KEY, true);

		//when
		runtimeService.createProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY).startBeforeActivity(TASK_BEFORE_CONDITION_ID).setVariable(VARIABLE_NAME, "1").executeWithVariablesInReturn();

		//then
		tasksAfterVariableIsSet = taskService.createTaskQuery().list();
		assertEquals(1, tasksAfterVariableIsSet.Count);
		assertEquals(TASK_AFTER_CONDITION_ID, tasksAfterVariableIsSet[0].TaskDefinitionKey);
	  }

	  private class ConditionalEventProcessSpecifierAnonymousInnerClass2 : ConditionalEventProcessSpecifier
	  {
		  private readonly TriggerConditionalEventFromDelegationCodeTest outerInstance;

		  public ConditionalEventProcessSpecifierAnonymousInnerClass2(TriggerConditionalEventFromDelegationCodeTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Type DelegateClass
		  {
			  get
			  {
				return typeof(SetMultipleSameVariableDelegate);
			  }
		  }

		  public int ExpectedInterruptingCount
		  {
			  get
			  {
				return 1;
			  }
		  }

		  public int ExpectedNonInterruptingCount
		  {
			  get
			  {
				return 3;
			  }
		  }


		  public string Condition
		  {
			  get
			  {
				return "${variable2 == 1}";
			  }
		  }

		  public override string ToString()
		  {
			return "SetMultipleVariableInDelegate";
		  }
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingTriggerGlobalEventSubProcess()
	  public virtual void testNonInterruptingTriggerGlobalEventSubProcess()
	  {
		//given
		deployConditionalEventSubProcess(TASK_MODEL, CONDITIONAL_EVENT_PROCESS_KEY, false);

		//when
		runtimeService.createProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY).startBeforeActivity(TASK_BEFORE_CONDITION_ID).setVariable(VARIABLE_NAME, "1").executeWithVariablesInReturn();

		//then
		tasksAfterVariableIsSet = taskService.createTaskQuery().list();
		assertEquals(2, tasksAfterVariableIsSet.Count);
		assertEquals(1, taskService.createTaskQuery().taskName(TASK_AFTER_CONDITION).count());
		assertEquals(1, conditionEventSubscriptionQuery.count());
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTriggerInnerEventSubProcess()
	  public virtual void testTriggerInnerEventSubProcess()
	  {
		//given
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).endEvent().subProcessDone().endEvent().done();
		deployConditionalEventSubProcess(modelInstance, SUB_PROCESS_ID, true);


		//when
		runtimeService.createProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY).startBeforeActivity(TASK_BEFORE_CONDITION_ID).setVariable(VARIABLE_NAME, "1").executeWithVariablesInReturn();

		//then
		tasksAfterVariableIsSet = taskService.createTaskQuery().list();
		assertEquals(1, tasksAfterVariableIsSet.Count);
		assertEquals(TASK_AFTER_CONDITION_ID, tasksAfterVariableIsSet[0].TaskDefinitionKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingTriggerInnerEventSubProcess()
	  public virtual void testNonInterruptingTriggerInnerEventSubProcess()
	  {
		//given
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).endEvent().subProcessDone().endEvent().done();
		deployConditionalEventSubProcess(modelInstance, SUB_PROCESS_ID, false);


		//when
		runtimeService.createProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY).startBeforeActivity(TASK_BEFORE_CONDITION_ID).setVariable(VARIABLE_NAME, "1").executeWithVariablesInReturn();

		//then
		tasksAfterVariableIsSet = taskService.createTaskQuery().list();
		assertEquals(2, tasksAfterVariableIsSet.Count);
		assertEquals(1, taskService.createTaskQuery().taskName(TASK_AFTER_CONDITION).count());
		assertEquals(1, conditionEventSubscriptionQuery.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTriggerGlobalEventSubProcessFromInnerSubProcess()
	  public virtual void testTriggerGlobalEventSubProcessFromInnerSubProcess()
	  {
		//given
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).endEvent().subProcessDone().endEvent().done();
		deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, true);

		//when
		runtimeService.createProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY).startBeforeActivity(TASK_BEFORE_CONDITION_ID).setVariable(VARIABLE_NAME, "1").executeWithVariablesInReturn();

		//then
		tasksAfterVariableIsSet = taskService.createTaskQuery().list();
		assertEquals(1, tasksAfterVariableIsSet.Count);
		assertEquals(TASK_AFTER_CONDITION_ID, tasksAfterVariableIsSet[0].TaskDefinitionKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingTriggerGlobalEventSubProcessFromInnerSubProcess()
	  public virtual void testNonInterruptingTriggerGlobalEventSubProcessFromInnerSubProcess()
	  {
		//given
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).endEvent().subProcessDone().endEvent().done();
		deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, false);

		//when
		runtimeService.createProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY).startBeforeActivity(TASK_BEFORE_CONDITION_ID).setVariable(VARIABLE_NAME, "1").executeWithVariablesInReturn();

		//then
		tasksAfterVariableIsSet = taskService.createTaskQuery().list();
		assertEquals(2, tasksAfterVariableIsSet.Count);
		assertEquals(1, taskService.createTaskQuery().taskName(TASK_AFTER_CONDITION).count());
		assertEquals(1, conditionEventSubscriptionQuery.count());
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTriggerGlobalAndInnerEventSubProcessFromInnerSubProcess()
	  public virtual void testTriggerGlobalAndInnerEventSubProcessFromInnerSubProcess()
	  {
		//given
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).endEvent().subProcessDone().endEvent().done();
		modelInstance = addConditionalEventSubProcess(modelInstance, SUB_PROCESS_ID, TASK_AFTER_CONDITION_ID + 1, true);
		deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, true);

		//when
		runtimeService.createProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY).startBeforeActivity(TASK_BEFORE_CONDITION_ID).setVariable(VARIABLE_NAME, "1").executeWithVariablesInReturn();

		//then
		tasksAfterVariableIsSet = taskService.createTaskQuery().list();
		assertEquals(1, tasksAfterVariableIsSet.Count);
		assertEquals(TASK_AFTER_CONDITION_ID, tasksAfterVariableIsSet[0].TaskDefinitionKey);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingTriggerGlobalAndInnerEventSubProcessFromInnerSubProcess()
	  public virtual void testNonInterruptingTriggerGlobalAndInnerEventSubProcessFromInnerSubProcess()
	  {
		//given
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).endEvent().subProcessDone().endEvent().done();
		modelInstance = addConditionalEventSubProcess(modelInstance, SUB_PROCESS_ID, TASK_AFTER_CONDITION_ID + 1, false);
		deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, false);

		//when
		runtimeService.createProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY).startBeforeActivity(TASK_BEFORE_CONDITION_ID).setVariable(VARIABLE_NAME, "1").executeWithVariablesInReturn();

		//then
		tasksAfterVariableIsSet = taskService.createTaskQuery().list();
		assertEquals(3, tasksAfterVariableIsSet.Count);
		assertEquals(2, taskService.createTaskQuery().taskName(TASK_AFTER_CONDITION).count());
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey(TASK_AFTER_CONDITION_ID).count());
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey(TASK_AFTER_CONDITION_ID + 1).count());
		assertEquals(2, conditionEventSubscriptionQuery.count());
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTriggerBoundaryEvent()
	  public virtual void testTriggerBoundaryEvent()
	  {
		//given
		deployConditionalBoundaryEventProcess(TASK_MODEL, TASK_BEFORE_CONDITION_ID, true);

		//when
		runtimeService.createProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY).startBeforeActivity(TASK_BEFORE_CONDITION_ID).setVariable(VARIABLE_NAME, "1").executeWithVariablesInReturn();

		//then
		tasksAfterVariableIsSet = taskService.createTaskQuery().list();
		assertEquals(1, tasksAfterVariableIsSet.Count);
		assertEquals(TASK_AFTER_CONDITION_ID, tasksAfterVariableIsSet[0].TaskDefinitionKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingTriggerBoundaryEvent()
	  public virtual void testNonInterruptingTriggerBoundaryEvent()
	  {
		//given
		deployConditionalBoundaryEventProcess(TASK_MODEL, TASK_BEFORE_CONDITION_ID, false);

		//when
		runtimeService.createProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY).startBeforeActivity(TASK_BEFORE_CONDITION_ID).setVariable(VARIABLE_NAME, "1").executeWithVariablesInReturn();

		//then
		tasksAfterVariableIsSet = taskService.createTaskQuery().list();
		assertEquals(2, tasksAfterVariableIsSet.Count);
		assertEquals(1, taskService.createTaskQuery().taskName(TASK_AFTER_CONDITION).count());
		assertEquals(1, conditionEventSubscriptionQuery.count());
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTriggerBoundaryEventFromInnerSubProcess()
	  public virtual void testTriggerBoundaryEventFromInnerSubProcess()
	  {
		//given
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).endEvent().subProcessDone().endEvent().done();
		deployConditionalBoundaryEventProcess(modelInstance, SUB_PROCESS_ID, true);


		//when
		runtimeService.createProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY).startBeforeActivity(TASK_BEFORE_CONDITION_ID).setVariable(VARIABLE_NAME, "1").executeWithVariablesInReturn();

		//then
		tasksAfterVariableIsSet = taskService.createTaskQuery().list();
		assertEquals(1, tasksAfterVariableIsSet.Count);
		assertEquals(TASK_AFTER_CONDITION_ID, tasksAfterVariableIsSet[0].TaskDefinitionKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingTriggerBoundaryEventFromInnerSubProcess()
	  public virtual void testNonInterruptingTriggerBoundaryEventFromInnerSubProcess()
	  {
		//given
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).endEvent().subProcessDone().endEvent().done();
		deployConditionalBoundaryEventProcess(modelInstance, SUB_PROCESS_ID, false);


		//when
		runtimeService.createProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY).startBeforeActivity(TASK_BEFORE_CONDITION_ID).setVariable(VARIABLE_NAME, "1").executeWithVariablesInReturn();

		//then
		tasksAfterVariableIsSet = taskService.createTaskQuery().list();
		assertEquals(2, tasksAfterVariableIsSet.Count);
		assertEquals(1, taskService.createTaskQuery().taskName(TASK_AFTER_CONDITION).count());
		assertEquals(1, conditionEventSubscriptionQuery.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTriggerUserAndSubProcessBoundaryEventFromInnerSubProcess()
	  public virtual void testTriggerUserAndSubProcessBoundaryEventFromInnerSubProcess()
	  {
		//given
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).endEvent().subProcessDone().endEvent().done();
		modelInstance = addConditionalBoundaryEvent(modelInstance, TASK_BEFORE_CONDITION_ID, TASK_AFTER_CONDITION_ID + 1, true);
		deployConditionalBoundaryEventProcess(modelInstance, SUB_PROCESS_ID, true);


		//when
		runtimeService.createProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY).startBeforeActivity(TASK_BEFORE_CONDITION_ID).setVariable(VARIABLE_NAME, "1").executeWithVariablesInReturn();

		//then
		tasksAfterVariableIsSet = taskService.createTaskQuery().list();
		assertEquals(1, tasksAfterVariableIsSet.Count);
		assertEquals(TASK_AFTER_CONDITION_ID, tasksAfterVariableIsSet[0].TaskDefinitionKey);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingTriggerUserAndSubProcessBoundaryEventFromInnerSubProcess()
	  public virtual void testNonInterruptingTriggerUserAndSubProcessBoundaryEventFromInnerSubProcess()
	  {
		//given
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).endEvent().subProcessDone().endEvent().done();
		modelInstance = addConditionalBoundaryEvent(modelInstance, TASK_BEFORE_CONDITION_ID, TASK_AFTER_CONDITION_ID + 1, false);
		deployConditionalBoundaryEventProcess(modelInstance, SUB_PROCESS_ID, false);


		//when
		runtimeService.createProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY).startBeforeActivity(TASK_BEFORE_CONDITION_ID).setVariable(VARIABLE_NAME, "1").executeWithVariablesInReturn();

		//then
		tasksAfterVariableIsSet = taskService.createTaskQuery().list();
		assertEquals(3, tasksAfterVariableIsSet.Count);
		assertEquals(2, taskService.createTaskQuery().taskName(TASK_AFTER_CONDITION).count());
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey(TASK_AFTER_CONDITION_ID + 1).count());
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey(TASK_AFTER_CONDITION_ID).count());
		assertEquals(2, conditionEventSubscriptionQuery.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTriggerMixedProcess()
	  public virtual void testTriggerMixedProcess()
	  {
		//given
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).endEvent().subProcessDone().endEvent().done();
		bool isInterrupting = true;
		modelInstance = addConditionalBoundaryEvent(modelInstance, TASK_BEFORE_CONDITION_ID, TASK_AFTER_CONDITION_ID + 1, isInterrupting);
		modelInstance = addConditionalBoundaryEvent(modelInstance, SUB_PROCESS_ID, TASK_AFTER_CONDITION_ID + 2, isInterrupting);
		modelInstance = addConditionalEventSubProcess(modelInstance, SUB_PROCESS_ID, TASK_AFTER_CONDITION_ID + 3, isInterrupting);
		deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, isInterrupting);

		//when
		runtimeService.createProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY).startBeforeActivity(TASK_BEFORE_CONDITION_ID).setVariable(VARIABLE_NAME, "1").executeWithVariablesInReturn();

		//then
		tasksAfterVariableIsSet = taskService.createTaskQuery().list();
		assertEquals(1, tasksAfterVariableIsSet.Count);
		assertEquals(TASK_AFTER_CONDITION_ID, tasksAfterVariableIsSet[0].TaskDefinitionKey);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingTriggerMixedProcess()
	  public virtual void testNonInterruptingTriggerMixedProcess()
	  {
		//given
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).endEvent().subProcessDone().endEvent().done();
		bool isInterrupting = false;
		modelInstance = addConditionalBoundaryEvent(modelInstance, TASK_BEFORE_CONDITION_ID, TASK_AFTER_CONDITION_ID + 1, isInterrupting);
		modelInstance = addConditionalBoundaryEvent(modelInstance, SUB_PROCESS_ID, TASK_AFTER_CONDITION_ID + 2, isInterrupting);
		modelInstance = addConditionalEventSubProcess(modelInstance, SUB_PROCESS_ID, TASK_AFTER_CONDITION_ID + 3, isInterrupting);
		deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, isInterrupting);

		//when
		runtimeService.createProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY).startBeforeActivity(TASK_BEFORE_CONDITION_ID).setVariable(VARIABLE_NAME, "1").executeWithVariablesInReturn();

		//then
		tasksAfterVariableIsSet = taskService.createTaskQuery().list();
		assertEquals(5, tasksAfterVariableIsSet.Count);
		assertEquals(4, taskService.createTaskQuery().taskName(TASK_AFTER_CONDITION).count());
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey(TASK_AFTER_CONDITION_ID + 1).count());
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey(TASK_AFTER_CONDITION_ID + 2).count());
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey(TASK_AFTER_CONDITION_ID + 3).count());
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey(TASK_AFTER_CONDITION_ID).count());
		assertEquals(4, conditionEventSubscriptionQuery.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Ignore public void testTwoInstructions()
	  public virtual void testTwoInstructions()
	  {
		//given
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent("start").subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).endEvent().subProcessDone().endEvent().moveToNode("start").subProcess(SUB_PROCESS_ID + 1).embeddedSubProcess().startEvent().userTask(TASK_BEFORE_CONDITION_ID + 1).name(TASK_BEFORE_CONDITION + 1).endEvent().subProcessDone().endEvent().done();
		bool isInterrupting = true;
		modelInstance = addConditionalBoundaryEvent(modelInstance, SUB_PROCESS_ID, TASK_AFTER_CONDITION_ID, isInterrupting);
		modelInstance = addConditionalBoundaryEvent(modelInstance, SUB_PROCESS_ID + 1, TASK_AFTER_CONDITION_ID + 1, isInterrupting);
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());

		//when
		runtimeService.createProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY).startBeforeActivity(TASK_BEFORE_CONDITION_ID).setVariable(VARIABLE_NAME, "1").startBeforeActivity(TASK_BEFORE_CONDITION_ID + 1).executeWithVariablesInReturn();

		//then
		tasksAfterVariableIsSet = taskService.createTaskQuery().list();
		assertTaskNames(tasksAfterVariableIsSet, TASK_AFTER_CONDITION_ID, TASK_AFTER_CONDITION_ID + 1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void testSubProcessNonInterruptingTriggerGlobalEventSubProcess()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testSubProcessNonInterruptingTriggerGlobalEventSubProcess()
	  {
		// given
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent("start").userTask("beforeSubProcess").subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).endEvent().subProcessDone().endEvent().done();

		modelInstance = addConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, TASK_AFTER_CONDITION_ID, false);

		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());

		// when
		runtimeService.createProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY).startBeforeActivity(TASK_BEFORE_CONDITION_ID).setVariable(VARIABLE_NAME, "1").executeWithVariablesInReturn();

		// then
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().count());
		assertEquals("variable", historyService.createHistoricVariableInstanceQuery().singleResult().Name);

		tasksAfterVariableIsSet = taskService.createTaskQuery().list();
		assertEquals(2, tasksAfterVariableIsSet.Count);
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey(TASK_BEFORE_CONDITION_ID).count());
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey(TASK_AFTER_CONDITION_ID).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void testSubProcessInterruptingTriggerGlobalEventSubProcess()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testSubProcessInterruptingTriggerGlobalEventSubProcess()
	  {
		// given
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent("start").userTask("beforeSubProcess").subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).endEvent().subProcessDone().endEvent().done();

		modelInstance = addConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, TASK_AFTER_CONDITION_ID, true);

		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());

		// when
		runtimeService.createProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY).startBeforeActivity(TASK_BEFORE_CONDITION_ID).setVariable(VARIABLE_NAME, "1").executeWithVariablesInReturn();

		// then
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().count());
		assertEquals("variable", historyService.createHistoricVariableInstanceQuery().singleResult().Name);

		tasksAfterVariableIsSet = taskService.createTaskQuery().list();
		assertEquals(1, tasksAfterVariableIsSet.Count);
		assertEquals(TASK_AFTER_CONDITION_ID, tasksAfterVariableIsSet[0].TaskDefinitionKey);
	  }
	}

}