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
namespace org.camunda.bpm.engine.test.api.variables
{
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class ExecutionVariablesTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTreeCompactionWithLocalVariableOnConcurrentExecution()
	  public virtual void testTreeCompactionWithLocalVariableOnConcurrentExecution()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		Execution innerTaskExecution = runtimeService.createExecutionQuery().activityId("innerTask").singleResult();

		Execution subProcessConcurrentExecution = runtimeService.createExecutionQuery().executionId(((ExecutionEntity) innerTaskExecution).ParentId).singleResult();

		Task task = taskService.createTaskQuery().taskDefinitionKey("task").singleResult();

		// when
		runtimeService.setVariableLocal(subProcessConcurrentExecution.Id, "foo", "bar");
		// and completing the concurrent task, thereby pruning the sub process concurrent execution
		taskService.complete(task.Id);

		// then the variable still exists
		VariableInstance variable = runtimeService.createVariableInstanceQuery().singleResult();
		assertNotNull(variable);
		assertEquals("foo", variable.Name);
		assertEquals(processInstance.Id, variable.ExecutionId);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/variables/ExecutionVariablesTest.testTreeCompactionWithLocalVariableOnConcurrentExecution.bpmn20.xml")]
	  public virtual void testStableVariableInstanceIdsOnCompaction()
	  {
		runtimeService.startProcessInstanceByKey("process");

		Execution innerTaskExecution = runtimeService.createExecutionQuery().activityId("innerTask").singleResult();

		Execution subProcessConcurrentExecution = runtimeService.createExecutionQuery().executionId(((ExecutionEntity) innerTaskExecution).ParentId).singleResult();

		Task task = taskService.createTaskQuery().taskDefinitionKey("task").singleResult();

		// when
		runtimeService.setVariableLocal(subProcessConcurrentExecution.Id, "foo", "bar");
		VariableInstance variableBeforeCompaction = runtimeService.createVariableInstanceQuery().singleResult();

		// and completing the concurrent task, thereby pruning the sub process concurrent execution
		taskService.complete(task.Id);

		// then the variable still exists
		VariableInstance variableAfterCompaction = runtimeService.createVariableInstanceQuery().singleResult();
		assertEquals(variableBeforeCompaction.Id, variableAfterCompaction.Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/variables/ExecutionVariablesTest.testTreeCompactionForkParallelGateway.bpmn20.xml")]
	  public virtual void testStableVariableInstanceIdsOnCompactionAndExpansion()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		Execution task1Execution = runtimeService.createExecutionQuery().activityId("task1").singleResult();

		Task task2 = taskService.createTaskQuery().taskDefinitionKey("task2").singleResult();

		// when
		runtimeService.setVariableLocal(task1Execution.Id, "foo", "bar");
		VariableInstance variableBeforeCompaction = runtimeService.createVariableInstanceQuery().singleResult();

		// compacting the tree
		taskService.complete(task2.Id);

		// expanding the tree
		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("task2").execute();

		// then the variable still exists
		VariableInstance variableAfterCompaction = runtimeService.createVariableInstanceQuery().singleResult();
		assertEquals(variableBeforeCompaction.Id, variableAfterCompaction.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTreeCompactionForkParallelGateway()
	  public virtual void testTreeCompactionForkParallelGateway()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		Task task1 = taskService.createTaskQuery().taskDefinitionKey("task1").singleResult();

		Execution task2Execution = runtimeService.createExecutionQuery().activityId("task2").singleResult();

		// when
		runtimeService.setVariableLocal(task2Execution.Id, "foo", "bar");
		// and completing the other task, thereby pruning the concurrent execution
		taskService.complete(task1.Id);

		// then the variable still exists
		VariableInstance variable = runtimeService.createVariableInstanceQuery().singleResult();
		assertNotNull(variable);
		assertEquals("foo", variable.Name);
		assertEquals(processInstance.Id, variable.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTreeCompactionNestedForkParallelGateway()
	  public virtual void testTreeCompactionNestedForkParallelGateway()
	  {
		// given
		runtimeService.startProcessInstanceByKey("process");

		Task task1 = taskService.createTaskQuery().taskDefinitionKey("task1").singleResult();

		Execution task2Execution = runtimeService.createExecutionQuery().activityId("task2").singleResult();
		string subProcessScopeExecutionId = ((ExecutionEntity) task2Execution).ParentId;

		// when
		runtimeService.setVariableLocal(task2Execution.Id, "foo", "bar");
		// and completing the other task, thereby pruning the concurrent execution
		taskService.complete(task1.Id);

		// then the variable still exists on the subprocess scope execution
		VariableInstance variable = runtimeService.createVariableInstanceQuery().singleResult();
		assertNotNull(variable);
		assertEquals("foo", variable.Name);
		assertEquals(subProcessScopeExecutionId, variable.ExecutionId);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/variables/ExecutionVariablesTest.testTreeCompactionForkParallelGateway.bpmn20.xml")]
	  public virtual void testTreeCompactionWithVariablesOnScopeAndConcurrentExecution()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		Task task1 = taskService.createTaskQuery().taskDefinitionKey("task1").singleResult();

		Execution task2Execution = runtimeService.createExecutionQuery().activityId("task2").singleResult();

		// when
		runtimeService.setVariable(processInstance.Id, "foo", "baz");
		runtimeService.setVariableLocal(task2Execution.Id, "foo", "bar");
		// and completing the other task, thereby pruning the concurrent execution
		taskService.complete(task1.Id);

		// then something happens
		VariableInstance variable = runtimeService.createVariableInstanceQuery().singleResult();
		assertNotNull(variable);
		assertEquals("foo", variable.Name);
		assertEquals(processInstance.Id, variable.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testForkWithThreeBranchesAndJoinOfTwoBranchesParallelGateway()
	  public virtual void testForkWithThreeBranchesAndJoinOfTwoBranchesParallelGateway()
	  {
		// given
		runtimeService.startProcessInstanceByKey("process");

		Execution task2Execution = runtimeService.createExecutionQuery().activityId("task2").singleResult();

		// when
		runtimeService.setVariableLocal(task2Execution.Id, "foo", "bar");
		taskService.complete(taskService.createTaskQuery().taskDefinitionKey("task1").singleResult().Id);
		taskService.complete(taskService.createTaskQuery().taskDefinitionKey("task2").singleResult().Id);

		// then
		assertEquals(0, runtimeService.createVariableInstanceQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testForkWithThreeBranchesAndJoinOfTwoBranchesInclusiveGateway()
	  public virtual void testForkWithThreeBranchesAndJoinOfTwoBranchesInclusiveGateway()
	  {
		// given
		runtimeService.startProcessInstanceByKey("process");

		Execution task2Execution = runtimeService.createExecutionQuery().activityId("task2").singleResult();

		// when
		runtimeService.setVariableLocal(task2Execution.Id, "foo", "bar");
		taskService.complete(taskService.createTaskQuery().taskDefinitionKey("task1").singleResult().Id);
		taskService.complete(taskService.createTaskQuery().taskDefinitionKey("task2").singleResult().Id);

		// then
		assertEquals(0, runtimeService.createVariableInstanceQuery().count());
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/variables/ExecutionVariablesTest.testTreeCompactionForkParallelGateway.bpmn20.xml")]
	  public virtual void testTreeCompactionAndExpansionWithConcurrentLocalVariables()
	  {

		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		Execution task1Execution = runtimeService.createExecutionQuery().activityId("task1").singleResult();
		Task task2 = taskService.createTaskQuery().taskDefinitionKey("task2").singleResult();

		runtimeService.setVariableLocal(task1Execution.Id, "var", "value");

		// when compacting the tree
		taskService.complete(task2.Id);

		// and expanding again
		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("task2").execute();

		// then the variable is again assigned to task1's concurrent execution
		Task task1 = taskService.createTaskQuery().taskDefinitionKey("task1").singleResult();
		VariableInstance variable = runtimeService.createVariableInstanceQuery().singleResult();

		assertEquals(task1.ExecutionId, variable.ExecutionId);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/variables/ExecutionVariablesTest.testTreeCompactionForkParallelGateway.bpmn20.xml")]
	  public virtual void testTreeCompactionAndExpansionWithScopeExecutionVariables()
	  {

		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		Task task2 = taskService.createTaskQuery().taskDefinitionKey("task2").singleResult();

		runtimeService.setVariableLocal(processInstance.Id, "var", "value");

		// when compacting the tree
		taskService.complete(task2.Id);

		// and expanding again
		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("task2").execute();

		// then the variable is still assigned to the scope execution execution
		VariableInstance variable = runtimeService.createVariableInstanceQuery().singleResult();

		assertEquals(processInstance.Id, variable.ExecutionId);
	  }

	}

}