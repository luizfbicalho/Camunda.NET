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
namespace org.camunda.bpm.engine.test.api.runtime.migration
{
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using SetVariableDelegate = org.camunda.bpm.engine.test.bpmn.@event.conditional.SetVariableDelegate;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;
	using static org.camunda.bpm.engine.test.api.runtime.migration.models.ConditionalModels;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.models.EventSubProcessModels.EVENT_SUB_PROCESS_START_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.bpmn.@event.conditional.AbstractConditionalEventTestCase.TASK_AFTER_CONDITION_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;

	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class MigrationWithoutTriggerConditionTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationWithoutTriggerConditionTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testHelper = new MigrationTestRule(rule);
			ruleChain = RuleChain.outerRule(rule).around(testHelper);
		}


	  protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
	  protected internal MigrationTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(rule).around(testHelper);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIntermediateConditionalEventWithSetVariableOnEndListener()
	  public virtual void testIntermediateConditionalEventWithSetVariableOnEndListener()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(Bpmn.createExecutableProcess().startEvent().subProcess().camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, typeof(SetVariableDelegate).FullName).embeddedSubProcess().startEvent().intermediateCatchEvent(CONDITION_ID).conditionalEventDefinition().condition(VAR_CONDITION).conditionalEventDefinitionDone().userTask(TASK_AFTER_CONDITION_ID).endEvent().subProcessDone().endEvent().done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(Bpmn.createExecutableProcess().startEvent().intermediateCatchEvent(CONDITION_ID).conditionalEventDefinition().condition(VAR_CONDITION).conditionalEventDefinitionDone().userTask(TASK_AFTER_CONDITION_ID).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(CONDITION_ID, CONDITION_ID).updateEventTrigger().build();

		//when sub process is removed, end listener is called and sets variable
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);
		testHelper.assertEventSubscriptionMigrated(CONDITION_ID, CONDITION_ID, null);
		assertEquals(1, rule.RuntimeService.getVariable(processInstance.Id, VARIABLE_NAME));

		//then conditional event is not triggered
		assertNull(rule.TaskService.createTaskQuery().singleResult());

		//when any var is set
		testHelper.AnyVariable = processInstance.Id;

		//then condition is satisfied, since variable is already set which satisfies condition
		testHelper.completeTask(TASK_AFTER_CONDITION_ID);
		testHelper.assertProcessEnded(processInstance.Id);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIntermediateConditionalEventWithSetVariableOnStartListener()
	  public virtual void testIntermediateConditionalEventWithSetVariableOnStartListener()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(Bpmn.createExecutableProcess().startEvent().intermediateCatchEvent(CONDITION_ID).conditionalEventDefinition().condition(VAR_CONDITION).conditionalEventDefinitionDone().userTask(TASK_AFTER_CONDITION_ID).endEvent().done());

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(Bpmn.createExecutableProcess().startEvent().subProcess().camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, typeof(SetVariableDelegate).FullName).embeddedSubProcess().startEvent().intermediateCatchEvent(CONDITION_ID).conditionalEventDefinition().condition(VAR_CONDITION).conditionalEventDefinitionDone().userTask(TASK_AFTER_CONDITION_ID).endEvent().subProcessDone().endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(CONDITION_ID, CONDITION_ID).updateEventTrigger().build();

		//when sub process is added, start listener is called and sets variable
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);
		testHelper.assertEventSubscriptionMigrated(CONDITION_ID, CONDITION_ID, null);
		assertEquals(1, rule.RuntimeService.getVariable(processInstance.Id, VARIABLE_NAME));

		//then conditional event is not triggered
		assertNull(rule.TaskService.createTaskQuery().singleResult());

		//when any var is set
		testHelper.AnyVariable = processInstance.Id;

		//then condition is satisfied, since variable is already set which satisfies condition
		testHelper.completeTask(TASK_AFTER_CONDITION_ID);
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBoundaryConditionalEventWithSetVariableOnStartListener()
	  public virtual void testBoundaryConditionalEventWithSetVariableOnStartListener()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.ONE_TASK_PROCESS).userTaskBuilder(USER_TASK_ID).boundaryEvent(BOUNDARY_ID).condition(VAR_CONDITION).userTask(TASK_AFTER_CONDITION_ID).endEvent().done());

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(Bpmn.createExecutableProcess().startEvent().subProcess(SUB_PROCESS_ID).camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, typeof(SetVariableDelegate).FullName).embeddedSubProcess().startEvent().userTask(USER_TASK_ID).endEvent().subProcessDone().endEvent().done()).userTaskBuilder(USER_TASK_ID).boundaryEvent(BOUNDARY_ID).condition(VAR_CONDITION).endEvent().moveToActivity(SUB_PROCESS_ID).boundaryEvent().condition(VAR_CONDITION).userTask(TASK_AFTER_CONDITION_ID).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_ID, USER_TASK_ID).mapActivities(BOUNDARY_ID, BOUNDARY_ID).updateEventTrigger().build();

		//when sub process is added, start listener is called and sets variable
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);
		testHelper.assertEventSubscriptionMigrated(BOUNDARY_ID, BOUNDARY_ID, null);
		assertEquals(1, rule.RuntimeService.getVariable(processInstance.Id, VARIABLE_NAME));

		//then conditional event is not triggered
		assertEquals(USER_TASK_ID, rule.TaskService.createTaskQuery().singleResult().TaskDefinitionKey);

		//when any var is set
		testHelper.AnyVariable = processInstance.Id;

		//then condition is satisfied, since variable is already set which satisfies condition
		testHelper.completeTask(TASK_AFTER_CONDITION_ID);
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBoundaryConditionalEventWithSetVariableOnEndListener()
	  public virtual void testBoundaryConditionalEventWithSetVariableOnEndListener()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(Bpmn.createExecutableProcess(PROC_DEF_KEY).startEvent().subProcess(SUB_PROCESS_ID).camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, typeof(SetVariableDelegate).FullName).embeddedSubProcess().startEvent().userTask(USER_TASK_ID).endEvent().subProcessDone().endEvent().done()).userTaskBuilder(USER_TASK_ID).boundaryEvent(BOUNDARY_ID).condition(VAR_CONDITION).endEvent().moveToActivity(SUB_PROCESS_ID).boundaryEvent().condition(VAR_CONDITION).userTask(TASK_AFTER_CONDITION_ID).endEvent().done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.ONE_TASK_PROCESS).userTaskBuilder(USER_TASK_ID).boundaryEvent(BOUNDARY_ID).condition(VAR_CONDITION).userTask(TASK_AFTER_CONDITION_ID).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_ID, USER_TASK_ID).mapActivities(BOUNDARY_ID, BOUNDARY_ID).updateEventTrigger().build();

		//when sub process is removed, end listener is called and sets variable
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);
		testHelper.assertEventSubscriptionMigrated(BOUNDARY_ID, BOUNDARY_ID, null);
		assertEquals(1, rule.RuntimeService.getVariable(processInstance.Id, VARIABLE_NAME));

		//then conditional event is not triggered
		assertEquals(USER_TASK_ID, rule.TaskService.createTaskQuery().singleResult().TaskDefinitionKey);

		//when any var is set
		testHelper.AnyVariable = processInstance.Id;

		//then condition is satisfied, since variable is already set which satisfies condition
		testHelper.completeTask(TASK_AFTER_CONDITION_ID);
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void tesConditionalEventSubProcessWithSetVariableOnStartListener()
	  public virtual void tesConditionalEventSubProcessWithSetVariableOnStartListener()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.ONE_TASK_PROCESS).addSubProcessTo(PROC_DEF_KEY).triggerByEvent().embeddedSubProcess().startEvent(EVENT_SUB_PROCESS_START_ID).condition(VAR_CONDITION).userTask(TASK_AFTER_CONDITION_ID).endEvent().done());

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance targetModel = modify(Bpmn.createExecutableProcess(PROC_DEF_KEY).startEvent().subProcess(SUB_PROCESS_ID).camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, typeof(SetVariableDelegate).FullName).embeddedSubProcess().startEvent().userTask(USER_TASK_ID).endEvent().subProcessDone().endEvent().done()).addSubProcessTo(SUB_PROCESS_ID).triggerByEvent().embeddedSubProcess().startEvent().condition(VAR_CONDITION).endEvent().done();

		targetModel = modify(targetModel).addSubProcessTo(PROC_DEF_KEY).triggerByEvent().embeddedSubProcess().startEvent(EVENT_SUB_PROCESS_START_ID).condition(VAR_CONDITION).userTask(TASK_AFTER_CONDITION_ID).endEvent().done();
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetModel);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_ID, USER_TASK_ID).mapActivities(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID).updateEventTrigger().build();

		//when sub process is added, start listener is called and sets variable
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);
		testHelper.assertEventSubscriptionMigrated(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID, null);
		assertEquals(1, rule.RuntimeService.getVariable(processInstance.Id, VARIABLE_NAME));

		//then conditional event is not triggered
		assertEquals(USER_TASK_ID, rule.TaskService.createTaskQuery().singleResult().TaskDefinitionKey);

		//when any var is set
		testHelper.AnyVariable = processInstance.Id;

		//then condition is satisfied, since variable is already set which satisfies condition
		testHelper.completeTask(TASK_AFTER_CONDITION_ID);
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testConditionalEventSubProcessWithSetVariableOnEndListener()
	  public virtual void testConditionalEventSubProcessWithSetVariableOnEndListener()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance sourceModel = modify(Bpmn.createExecutableProcess(PROC_DEF_KEY).startEvent().subProcess(SUB_PROCESS_ID).camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, typeof(SetVariableDelegate).FullName).embeddedSubProcess().startEvent().userTask(USER_TASK_ID).endEvent().subProcessDone().endEvent().done()).addSubProcessTo(PROC_DEF_KEY).triggerByEvent().embeddedSubProcess().startEvent(EVENT_SUB_PROCESS_START_ID).condition(VAR_CONDITION).endEvent().done();

		sourceModel = modify(sourceModel).addSubProcessTo(SUB_PROCESS_ID).triggerByEvent().embeddedSubProcess().startEvent().condition(VAR_CONDITION).userTask(TASK_AFTER_CONDITION_ID).endEvent().done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceModel);

		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.ONE_TASK_PROCESS).addSubProcessTo(PROC_DEF_KEY).triggerByEvent().embeddedSubProcess().startEvent(EVENT_SUB_PROCESS_START_ID).condition(VAR_CONDITION).userTask(TASK_AFTER_CONDITION_ID).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_ID, USER_TASK_ID).mapActivities(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID).updateEventTrigger().build();

		//when sub process is removed, end listener is called and sets variable
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);
		testHelper.assertEventSubscriptionMigrated(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID, null);
		assertEquals(1, rule.RuntimeService.getVariable(processInstance.Id, VARIABLE_NAME));

		//then conditional event is not triggered
		assertEquals(USER_TASK_ID, rule.TaskService.createTaskQuery().singleResult().TaskDefinitionKey);

		//when any var is set
		testHelper.AnyVariable = processInstance.Id;

		//then condition is satisfied, since variable is already set which satisfies condition
		testHelper.completeTask(TASK_AFTER_CONDITION_ID);
		testHelper.assertProcessEnded(processInstance.Id);
	  }
	}

}