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
	using EventSubscriptionQueryImpl = org.camunda.bpm.engine.impl.EventSubscriptionQueryImpl;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using EventType = org.camunda.bpm.engine.impl.@event.EventType;
	using Task = org.camunda.bpm.engine.task.Task;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using ExpectedException = org.junit.rules.ExpectedException;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.*;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public abstract class AbstractConditionalEventTestCase
	{

	  protected internal const string CONDITIONAL_EVENT_PROCESS_KEY = "conditionalEventProcess";
	  protected internal const string CONDITIONAL_EVENT = "conditionalEvent";
	  protected internal const string CONDITION_EXPR = "${variable == 1}";
	  protected internal const string EXPR_SET_VARIABLE = "${execution.setVariable(\"variable\", 1)}";
	  protected internal const string EXPR_SET_VARIABLE_ON_PARENT = "${execution.getParent().setVariable(\"variable\", 1)}";
	  protected internal const string CONDITIONAL_MODEL = "conditionalModel.bpmn20.xml";
	  protected internal const string CONDITIONAL_VAR_EVENTS = "create, update";
	  protected internal const string CONDITIONAL_VAR_EVENT_UPDATE = "update";

	  protected internal const string TASK_BEFORE_CONDITION = "Before Condition";
	  protected internal const string TASK_BEFORE_CONDITION_ID = "beforeConditionId";
	  protected internal const string TASK_AFTER_CONDITION = "After Condition";
	  public const string TASK_AFTER_CONDITION_ID = "afterConditionId";
	  protected internal const string TASK_AFTER_SERVICE_TASK = "afterServiceTask";
	  protected internal const string TASK_IN_SUB_PROCESS_ID = "taskInSubProcess";
	  protected internal const string TASK_IN_SUB_PROCESS = "Task in Subprocess";
	  protected internal const string TASK_WITH_CONDITION = "Task with condition";
	  protected internal const string TASK_WITH_CONDITION_ID = "taskWithCondition";
	  protected internal const string AFTER_TASK = "After Task";

	  protected internal const string VARIABLE_NAME = "variable";
	  protected internal const string TRUE_CONDITION = "${true}";
	  protected internal const string SUB_PROCESS_ID = "subProcess";
	  protected internal const string FLOW_ID = "flow";
	  protected internal const string DELEGATED_PROCESS_KEY = "delegatedProcess";

	  protected internal static readonly BpmnModelInstance TASK_MODEL = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).endEvent().done();
	  protected internal static readonly BpmnModelInstance DELEGATED_PROCESS = Bpmn.createExecutableProcess(DELEGATED_PROCESS_KEY).startEvent().serviceTask().camundaExpression(EXPR_SET_VARIABLE).endEvent().done();
	  protected internal const string TASK_AFTER_OUTPUT_MAPPING = "afterOutputMapping";

	  protected internal IList<Task> tasksAfterVariableIsSet;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public final org.camunda.bpm.engine.test.ProcessEngineRule engine = new org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule();
	  public readonly ProcessEngineRule engine = new ProvidedProcessEngineRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException expectException = org.junit.rules.ExpectedException.none();
	  public ExpectedException expectException = ExpectedException.none();

	  protected internal RuntimeService runtimeService;
	  protected internal TaskService taskService;
	  protected internal RepositoryService repositoryService;
	  protected internal HistoryService historyService;
	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;
	  protected internal EventSubscriptionQueryImpl conditionEventSubscriptionQuery;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		this.runtimeService = engine.RuntimeService;
		this.taskService = engine.TaskService;
		this.repositoryService = engine.RepositoryService;
		this.historyService = engine.HistoryService;
		this.processEngineConfiguration = engine.ProcessEngineConfiguration;
		this.conditionEventSubscriptionQuery = (new EventSubscriptionQueryImpl(processEngineConfiguration.CommandExecutorTxRequired)).eventType(EventType.CONDITONAL.name());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void checkIfProcessCanBeFinished()
	  public virtual void checkIfProcessCanBeFinished()
	  {
		//given tasks after variable was set
		assertNotNull(tasksAfterVariableIsSet);

		//when tasks are completed
		foreach (Task task in tasksAfterVariableIsSet)
		{
		  taskService.complete(task.Id);
		}

		//then
		assertEquals(0, conditionEventSubscriptionQuery.list().Count);
		assertNull(taskService.createTaskQuery().singleResult());
		assertNull(runtimeService.createProcessInstanceQuery().singleResult());
		tasksAfterVariableIsSet = null;
	  }



	  public static void assertTaskNames(IList<Task> actualTasks, params string[] expectedTaskNames)
	  {
		IList<string> expectedNames = new List<string>(Arrays.asList(expectedTaskNames));
		foreach (Task task in actualTasks)
		{
		  string actualTaskName = task.Name;
		  if (expectedNames.Contains(actualTaskName))
		  {
			expectedNames.Remove(actualTaskName);
		  }
		}
		assertTrue(expectedNames.Count == 0);
	  }

	  // conditional event sub process //////////////////////////////////////////////////////////////////////////////////////////

	  protected internal virtual void deployConditionalEventSubProcess(BpmnModelInstance model, string parentId, bool isInterrupting)
	  {
		deployConditionalEventSubProcess(model, parentId, CONDITION_EXPR, isInterrupting);
	  }

	  protected internal virtual void deployConditionalEventSubProcess(BpmnModelInstance model, string parentId, string conditionExpr, bool isInterrupting)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = addConditionalEventSubProcess(model, parentId, conditionExpr, TASK_AFTER_CONDITION_ID, isInterrupting);
		BpmnModelInstance modelInstance = addConditionalEventSubProcess(model, parentId, conditionExpr, TASK_AFTER_CONDITION_ID, isInterrupting);
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());
	  }


	  protected internal virtual BpmnModelInstance addConditionalEventSubProcess(BpmnModelInstance model, string parentId, string userTaskId, bool isInterrupting)
	  {
		return addConditionalEventSubProcess(model, parentId, CONDITION_EXPR, userTaskId, isInterrupting);
	  }

	  protected internal virtual BpmnModelInstance addConditionalEventSubProcess(BpmnModelInstance model, string parentId, string conditionExpr, string userTaskId, bool isInterrupting)
	  {
		return modify(model).addSubProcessTo(parentId).triggerByEvent().embeddedSubProcess().startEvent().interrupting(isInterrupting).condition(conditionExpr).userTask(userTaskId).name(TASK_AFTER_CONDITION).endEvent().done();
	  }

	  // conditional boundary event //////////////////////////////////////////////////////////////////////////////////////////


	  protected internal virtual void deployConditionalBoundaryEventProcess(BpmnModelInstance model, string activityId, bool isInterrupting)
	  {
		deployConditionalBoundaryEventProcess(model, activityId, CONDITION_EXPR, isInterrupting);
	  }

	  protected internal virtual void deployConditionalBoundaryEventProcess(BpmnModelInstance model, string activityId, string conditionExpr, bool isInterrupting)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = addConditionalBoundaryEvent(model, activityId, conditionExpr, TASK_AFTER_CONDITION_ID, isInterrupting);
		BpmnModelInstance modelInstance = addConditionalBoundaryEvent(model, activityId, conditionExpr, TASK_AFTER_CONDITION_ID, isInterrupting);
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());
	  }

	  protected internal virtual BpmnModelInstance addConditionalBoundaryEvent(BpmnModelInstance model, string activityId, string userTaskId, bool isInterrupting)
	  {
		return addConditionalBoundaryEvent(model, activityId, CONDITION_EXPR, userTaskId, isInterrupting);
	  }

	  protected internal virtual BpmnModelInstance addConditionalBoundaryEvent(BpmnModelInstance model, string activityId, string conditionExpr, string userTaskId, bool isInterrupting)
	  {
		return modify(model).activityBuilder(activityId).boundaryEvent().cancelActivity(isInterrupting).condition(conditionExpr).userTask(userTaskId).name(TASK_AFTER_CONDITION).endEvent().done();
	  }
	}

}