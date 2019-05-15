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
namespace org.camunda.bpm.engine.test.bpmn.@event.conditional
{
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;

	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class ConditionalEventWithSpecificVariableEventTest extends AbstractConditionalEventTestCase
	public class ConditionalEventWithSpecificVariableEventTest : AbstractConditionalEventTestCase
	{

	  private interface ConditionalProcessVarSpecification
	  {
		BpmnModelInstance getProcessWithVarName(bool interrupting, string condition);
		BpmnModelInstance getProcessWithVarNameAndEvents(bool interrupting, string varEvent);
		BpmnModelInstance getProcessWithVarEvents(bool interrupting, string varEvent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameters(name = "{0}") public static java.util.Collection<Object[]> data()
	  public static ICollection<object[]> data()
	  {
//JAVA TO C# CONVERTER TODO TASK: The following anonymous inner class could not be converted:
//		return java.util.Arrays.asList(new Object[][] { { new ConditionalProcessVarSpecification()
	//	{
	//		@@Override public BpmnModelInstance getProcessWithVarName(boolean interrupting, String condition)
	//		{
	//		  return modify(TASK_MODEL).userTaskBuilder(TASK_BEFORE_CONDITION_ID).boundaryEvent().cancelActivity(interrupting).conditionalEventDefinition(CONDITIONAL_EVENT).condition(condition).camundaVariableName(VARIABLE_NAME).conditionalEventDefinitionDone().userTask().name(TASK_AFTER_CONDITION).endEvent().done();
	//		}
	//
	//		@@Override public BpmnModelInstance getProcessWithVarNameAndEvents(boolean interrupting, String varEvent)
	//		{
	//		  return modify(TASK_MODEL).userTaskBuilder(TASK_BEFORE_CONDITION_ID).boundaryEvent().cancelActivity(interrupting).conditionalEventDefinition(CONDITIONAL_EVENT).condition(CONDITION_EXPR).camundaVariableName(VARIABLE_NAME).camundaVariableEvents(varEvent).conditionalEventDefinitionDone().userTask().name(TASK_AFTER_CONDITION).endEvent().done();
	//		}
	//
	//		@@Override public BpmnModelInstance getProcessWithVarEvents(boolean interrupting, String varEvent)
	//		{
	//		  return modify(TASK_MODEL).userTaskBuilder(TASK_BEFORE_CONDITION_ID).boundaryEvent().cancelActivity(interrupting).conditionalEventDefinition(CONDITIONAL_EVENT).condition(CONDITION_EXPR).camundaVariableEvents(varEvent).conditionalEventDefinitionDone().userTask().name(TASK_AFTER_CONDITION).endEvent().done();
	//		}
	//
	//		  @@Override public String toString()
	//		  {
	//			return "ConditionalBoundaryEventWithVarEvents";
	//		  }
	//		}
	  }

	  private class ConditionalEventProcessSpecifierAnonymousInnerClass2 : ConditionalEventProcessSpecifier
	  {
		  private readonly ConditionalEventTriggeredByExecutionListenerTest outerInstance;

		  public ConditionalEventProcessSpecifierAnonymousInnerClass2(ConditionalEventTriggeredByExecutionListenerTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public BpmnModelInstance specifyConditionalProcess(BpmnModelInstance modelInstance, bool isInterrupting)
		  {
			modelInstance = modify(modelInstance).addSubProcessTo(CONDITIONAL_EVENT_PROCESS_KEY).triggerByEvent().embeddedSubProcess().startEvent().interrupting(isInterrupting).conditionalEventDefinition().condition(CONDITION_EXPR).conditionalEventDefinitionDone().userTask().name(TASK_AFTER_CONDITIONAL_START_EVENT).endEvent().done();

			return modify(modelInstance).activityBuilder(TASK_WITH_CONDITION_ID).boundaryEvent().cancelActivity(isInterrupting).conditionalEventDefinition().condition(CONDITION_EXPR).conditionalEventDefinitionDone().userTask().name(TASK_AFTER_CONDITIONAL_BOUNDARY_EVENT).endEvent().done();
		  }

		  public void assertTaskNames(IList<Task> tasks, bool isInterrupting, bool isAyncBefore)
		  {
			assertNotNull(tasks);
			if (isInterrupting || isAyncBefore)
			{
			  ConditionalEventTriggeredByExecutionListenerTest.assertTaskNames(tasks, TASK_AFTER_CONDITIONAL_START_EVENT);
			}
			else
			{
			  ConditionalEventTriggeredByExecutionListenerTest.assertTaskNames(tasks, TASK_WITH_CONDITION, TASK_AFTER_CONDITIONAL_BOUNDARY_EVENT, TASK_AFTER_CONDITIONAL_START_EVENT);

			}
		  }

		  public int expectedSubscriptions()
		  {
			return 2;
		  }

		  public int expectedTaskCount()
		  {
			return 3;
		  }

		  public override string ToString()
		  {
			return "MixedConditionalProcess";
		  }
	  }
	 ,

	 {
		  //conditional start event of event sub process
			  new ConditionalProcessVarSpecificationAnonymousInnerClass2(this)
	 }
	}
	   );
}


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter public ConditionalProcessVarSpecification specifier;
	  public ConditionalProcessVarSpecification specifier;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableConditionWithVariableName()
	  public void testVariableConditionWithVariableName()
	  {

		//given process with boundary conditional event and defined variable name
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = specifier.getProcessWithVarName(true, CONDITION_EXPR);
		BpmnModelInstance modelInstance = specifier.getProcessWithVarName(true, CONDITION_EXPR);
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());

		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);

		//when variable with name `variable1` is set on execution
		taskService.setVariable(task.Id, VARIABLE_NAME + 1, 1);

		//then nothing happens
		task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);
		assertEquals(1, conditionEventSubscriptionQuery.list().size());

		//when variable with name `variable` is set on execution
		taskService.setVariable(task.Id, VARIABLE_NAME, 1);

		//then execution is at user task after conditional event
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITION, tasksAfterVariableIsSet.get(0).Name);
		assertEquals(0, conditionEventSubscriptionQuery.list().size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableConditionWithVariableNameAndEvent()
	  public void testVariableConditionWithVariableNameAndEvent()
	  {

		//given process with boundary conditional event and defined variable name and event
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = specifier.getProcessWithVarNameAndEvents(true, CONDITIONAL_VAR_EVENT_UPDATE);
		BpmnModelInstance modelInstance = specifier.getProcessWithVarNameAndEvents(true, CONDITIONAL_VAR_EVENT_UPDATE);
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());

		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);

		//when variable with name `variable` is set on execution
		taskService.setVariable(task.Id, VARIABLE_NAME, 1);

		//then nothing happens
		task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);
		assertEquals(1, conditionEventSubscriptionQuery.list().size());

		//when variable with name `variable` is updated
		taskService.setVariable(task.Id, VARIABLE_NAME, 1);

		//then execution is at user task after conditional event
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITION, tasksAfterVariableIsSet.get(0).Name);
		assertEquals(0, conditionEventSubscriptionQuery.list().size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingVariableConditionWithVariableName()
	  public void testNonInterruptingVariableConditionWithVariableName()
	  {

		//given process with non interrupting boundary conditional event and defined variable name and true condition
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = specifier.getProcessWithVarName(false, TRUE_CONDITION);
		BpmnModelInstance modelInstance = specifier.getProcessWithVarName(false, TRUE_CONDITION);
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());

		//when process is started
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);

		//then first event is triggered since condition is true
		IList<Task> tasks = taskQuery.list();
		assertEquals(2, tasks.Count);

		//when variable with name `variable1` is set on execution
		runtimeService.setVariable(procInst.Id, VARIABLE_NAME + 1, 1);

		//then nothing happens
		tasks = taskQuery.list();
		assertEquals(2, tasks.Count);
		assertEquals(1, conditionEventSubscriptionQuery.list().size());

		//when variable with name `variable` is set, updated and deleted
		runtimeService.setVariable(procInst.Id, VARIABLE_NAME, 1); //create
		runtimeService.setVariable(procInst.Id, VARIABLE_NAME, 1); //update
		runtimeService.removeVariable(procInst.Id, VARIABLE_NAME); //delete

		//then execution is for four times at user task after conditional event
		//one from default behavior and three times from the variable events
		assertEquals(4, taskService.createTaskQuery().taskName(TASK_AFTER_CONDITION).count());
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(5, tasksAfterVariableIsSet.size());
		assertEquals(1, conditionEventSubscriptionQuery.list().size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingVariableConditionWithVariableNameAndEvents()
	  public void testNonInterruptingVariableConditionWithVariableNameAndEvents()
	  {

		//given process with non interrupting boundary conditional event and defined variable name and events
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = specifier.getProcessWithVarNameAndEvents(false, CONDITIONAL_VAR_EVENTS);
		BpmnModelInstance modelInstance = specifier.getProcessWithVarNameAndEvents(false, CONDITIONAL_VAR_EVENTS);
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());

		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);

		//when variable with name `variable` is set, updated and deleted
		taskService.setVariable(task.Id, VARIABLE_NAME, 1); //create
		taskService.setVariable(task.Id, VARIABLE_NAME, 1); //update
		taskService.removeVariable(task.Id, VARIABLE_NAME); //delete

		//then execution is for two times at user task after conditional start event
		assertEquals(2, taskService.createTaskQuery().taskName(TASK_AFTER_CONDITION).count());
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(3, tasksAfterVariableIsSet.size());
		assertEquals(1, conditionEventSubscriptionQuery.list().size());
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableConditionWithVariableEvent()
	  public void testVariableConditionWithVariableEvent()
	  {

		//given process with boundary conditional event and defined variable event
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = specifier.getProcessWithVarEvents(true, CONDITIONAL_VAR_EVENT_UPDATE);
		BpmnModelInstance modelInstance = specifier.getProcessWithVarEvents(true, CONDITIONAL_VAR_EVENT_UPDATE);
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());

		IDictionary<string, object> variables = Variables.createVariables();
		variables[VARIABLE_NAME + 1] = 0;
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY, variables);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);

		//when variable with name `variable` is set on execution
		runtimeService.setVariable(procInst.Id, VARIABLE_NAME, 1);

		//then nothing happens
		task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);
		assertEquals(1, conditionEventSubscriptionQuery.list().size());

		//when variable with name `variable1` is updated
		runtimeService.setVariable(procInst.Id, VARIABLE_NAME + 1, 1);

		//then execution is at user task after conditional intermediate event
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITION, tasksAfterVariableIsSet.get(0).Name);
		assertEquals(0, conditionEventSubscriptionQuery.list().size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingVariableConditionWithVariableEvent()
	  public void testNonInterruptingVariableConditionWithVariableEvent()
	  {

		//given process with non interrupting boundary conditional event and defined variable event
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = specifier.getProcessWithVarEvents(false, CONDITIONAL_VAR_EVENT_UPDATE);
		BpmnModelInstance modelInstance = specifier.getProcessWithVarEvents(false, CONDITIONAL_VAR_EVENT_UPDATE);
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());

		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);

		//when variable with name `variable` is set
		taskService.setVariable(task.Id, VARIABLE_NAME, 1); //create

		//then nothing happens
		task = taskQuery.singleResult();
		assertNotNull(task);

		//when variable is updated twice
		taskService.setVariable(task.Id, VARIABLE_NAME, 1); //update
		taskService.setVariable(task.Id, VARIABLE_NAME, 1); //update

		//then execution is for two times at user task after conditional event
		assertEquals(2, taskQuery.taskName(TASK_AFTER_CONDITION).count());
		tasksAfterVariableIsSet = taskService.createTaskQuery().list();
		assertEquals(3, tasksAfterVariableIsSet.size());
		assertEquals(1, conditionEventSubscriptionQuery.list().size());
	  }

	}

}