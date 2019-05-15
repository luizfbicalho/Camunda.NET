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
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.describeActivityInstanceTree;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ExecutionAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ExecutionAssert.describeExecutionTree;


	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ActivityStatistics = org.camunda.bpm.engine.management.ActivityStatistics;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ExecutionTree = org.camunda.bpm.engine.test.util.ExecutionTree;


	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ProcessInstanceModificationMultiInstanceTest : PluggableProcessEngineTestCase
	{

	  public const string PARALLEL_MULTI_INSTANCE_TASK_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationMultiInstanceTest.parallelTasks.bpmn20.xml";
	  public const string PARALLEL_MULTI_INSTANCE_SUBPROCESS_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationMultiInstanceTest.parallelSubprocess.bpmn20.xml";

	  public const string SEQUENTIAL_MULTI_INSTANCE_TASK_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationMultiInstanceTest.sequentialTasks.bpmn20.xml";
	  public const string SEQUENTIAL_MULTI_INSTANCE_SUBPROCESS_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationMultiInstanceTest.sequentialSubprocess.bpmn20.xml";

	  public const string PARALLEL_MULTI_INSTANCE_TASK_COMPLETION_CONDITION_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationMultiInstanceTest.parallelTasksCompletionCondition.bpmn20.xml";
	  public const string PARALLEL_MULTI_INSTANCE_SUBPROCESS_COMPLETION_CONDITION_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationMultiInstanceTest.parallelSubprocessCompletionCondition.bpmn20.xml";

	  public const string NESTED_PARALLEL_MULTI_INSTANCE_TASK_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationMultiInstanceTest.nestedParallelTasks.bpmn20.xml";

	  [Deployment(resources : PARALLEL_MULTI_INSTANCE_TASK_PROCESS)]
	  public virtual void testStartBeforeMultiInstanceBodyParallelTasks()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miParallelUserTasks");
		completeTasksInOrder("beforeTask");

		// when
		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("miTasks#multiInstanceBody").execute();

		// then
		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginMiBody("miTasks").activity("miTasks").activity("miTasks").activity("miTasks").endScope().beginMiBody("miTasks").activity("miTasks").activity("miTasks").activity("miTasks").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstance.Id, processEngine);
		assertThat(executionTree).matches(describeExecutionTree(null).scope().child(null).concurrent().noScope().child(null).scope().child("miTasks").concurrent().noScope().up().child("miTasks").concurrent().noScope().up().child("miTasks").concurrent().noScope().up().up().up().child(null).concurrent().noScope().child(null).scope().child("miTasks").concurrent().noScope().up().child("miTasks").concurrent().noScope().up().child("miTasks").concurrent().noScope().up().done());

		// and the process is able to complete successfully
		completeTasksInOrder("miTasks", "miTasks", "miTasks", "miTasks", "miTasks", "miTasks", "afterTask", "afterTask");
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : PARALLEL_MULTI_INSTANCE_SUBPROCESS_PROCESS)]
	  public virtual void testStartBeforeMultiInstanceBodyParallelSubprocess()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miParallelSubprocess");
		completeTasksInOrder("beforeTask");

		// when
		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("miSubProcess#multiInstanceBody").execute();

		// then
		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginMiBody("miSubProcess").beginScope("miSubProcess").activity("subProcessTask").endScope().beginScope("miSubProcess").activity("subProcessTask").endScope().beginScope("miSubProcess").activity("subProcessTask").endScope().endScope().beginMiBody("miSubProcess").beginScope("miSubProcess").activity("subProcessTask").endScope().beginScope("miSubProcess").activity("subProcessTask").endScope().beginScope("miSubProcess").activity("subProcessTask").endScope().done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstance.Id, processEngine);
		assertThat(executionTree).matches(describeExecutionTree(null).scope().child(null).concurrent().noScope().child(null).scope().child(null).concurrent().noScope().child("subProcessTask").scope().up().up().child(null).concurrent().noScope().child("subProcessTask").scope().up().up().child(null).concurrent().noScope().child("subProcessTask").scope().up().up().up().up().child(null).concurrent().noScope().child(null).scope().child(null).concurrent().noScope().child("subProcessTask").scope().up().up().child(null).concurrent().noScope().child("subProcessTask").scope().up().up().child(null).concurrent().noScope().child("subProcessTask").scope().done());

		// and the process is able to complete successfully
		completeTasksInOrder("subProcessTask", "subProcessTask", "subProcessTask", "subProcessTask", "subProcessTask", "afterTask", "subProcessTask", "afterTask");
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : PARALLEL_MULTI_INSTANCE_TASK_COMPLETION_CONDITION_PROCESS)]
	  public virtual void testStartInnerActivityParallelTasksWithCompletionCondition()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miParallelUserTasksCompletionCondition");

		// when
		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("miTasks").startBeforeActivity("miTasks").execute();

		// then the process is able to complete successfully and respects the completion condition
		completeTasksInOrder("miTasks", "miTasks", "miTasks", "miTasks");
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : PARALLEL_MULTI_INSTANCE_SUBPROCESS_COMPLETION_CONDITION_PROCESS)]
	  public virtual void testStartInnerActivityParallelSubprocessWithCompletionCondition()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miParallelSubprocessCompletionCondition");

		// when
		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("miSubProcess").startBeforeActivity("miSubProcess").execute();

		// then the process is able to complete successfully and respects the completion condition
		completeTasksInOrder("subProcessTask", "subProcessTask", "subProcessTask", "subProcessTask");
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : SEQUENTIAL_MULTI_INSTANCE_TASK_PROCESS)]
	  public virtual void testStartBeforeMultiInstanceBodySequentialTasks()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miSequentialUserTasks");
		completeTasksInOrder("beforeTask");

		// when
		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("miTasks#multiInstanceBody").execute();

		// then
		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginMiBody("miTasks").activity("miTasks").endScope().beginMiBody("miTasks").activity("miTasks").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstance.Id, processEngine);
		assertThat(executionTree).matches(describeExecutionTree(null).scope().child(null).concurrent().noScope().child("miTasks").scope().up().up().child(null).concurrent().noScope().child("miTasks").scope().done());

		// and the process is able to complete successfully
		completeTasksInOrder("miTasks", "miTasks", "miTasks", "miTasks", "miTasks", "miTasks", "afterTask", "afterTask");
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : SEQUENTIAL_MULTI_INSTANCE_SUBPROCESS_PROCESS)]
	  public virtual void testStartBeforeMultiInstanceBodySequentialSubprocess()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miSequentialSubprocess");
		completeTasksInOrder("beforeTask");

		// when
		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("miSubProcess#multiInstanceBody").execute();

		// then
		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginMiBody("miSubProcess").beginScope("miSubProcess").activity("subProcessTask").endScope().endScope().beginMiBody("miSubProcess").beginScope("miSubProcess").activity("subProcessTask").endScope().done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstance.Id, processEngine);
		assertThat(executionTree).matches(describeExecutionTree(null).scope().child(null).concurrent().noScope().child(null).scope().child("subProcessTask").scope().up().up().up().child(null).concurrent().noScope().child(null).scope().child("subProcessTask").scope().done());

		// and the process is able to complete successfully
		completeTasksInOrder("subProcessTask", "subProcessTask", "subProcessTask", "subProcessTask", "subProcessTask", "subProcessTask", "afterTask", "afterTask");
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : PARALLEL_MULTI_INSTANCE_TASK_PROCESS)]
	  public virtual void testStartBeforeInnerActivityParallelTasks()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miParallelUserTasks");
		completeTasksInOrder("beforeTask");

		// when
		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("miTasks", tree.getActivityInstances("miTasks#multiInstanceBody")[0].Id).execute();

		// then the mi variables should be correct
		IList<Execution> leafExecutions = runtimeService.createExecutionQuery().activityId("miTasks").list();
		assertEquals(4, leafExecutions.Count);
		assertVariableSet(leafExecutions, "loopCounter", Arrays.asList(0, 1, 2, 3));
		foreach (Execution leafExecution in leafExecutions)
		{
		  assertVariable(leafExecution, "nrOfInstances", 4);
		  assertVariable(leafExecution, "nrOfCompletedInstances", 0);
		  assertVariable(leafExecution, "nrOfActiveInstances", 4);
		}

		// and the trees should be correct
		tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginMiBody("miTasks").activity("miTasks").activity("miTasks").activity("miTasks").activity("miTasks").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstance.Id, processEngine);
		assertThat(executionTree).matches(describeExecutionTree(null).scope().child(null).scope().child("miTasks").concurrent().noScope().up().child("miTasks").concurrent().noScope().up().child("miTasks").concurrent().noScope().up().child("miTasks").concurrent().noScope().done());

		// and the process is able to complete successfully
		completeTasksInOrder("miTasks", "miTasks", "miTasks", "miTasks", "afterTask");
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : PARALLEL_MULTI_INSTANCE_SUBPROCESS_PROCESS)]
	  public virtual void testStartBeforeInnerActivityParallelSubprocess()
	  {
		// given the mi body is already instantiated
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miParallelSubprocess");
		completeTasksInOrder("beforeTask");

		// when
		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("miSubProcess", tree.getActivityInstances("miSubProcess#multiInstanceBody")[0].Id).execute();

		// then the mi variables should be correct
		IList<Execution> leafExecutions = runtimeService.createExecutionQuery().activityId("subProcessTask").list();
		assertEquals(4, leafExecutions.Count);
		assertVariableSet(leafExecutions, "loopCounter", Arrays.asList(0, 1, 2, 3));
		foreach (Execution leafExecution in leafExecutions)
		{
		  assertVariable(leafExecution, "nrOfInstances", 4);
		  assertVariable(leafExecution, "nrOfCompletedInstances", 0);
		  assertVariable(leafExecution, "nrOfActiveInstances", 4);
		}

		// and the trees are correct
		tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginMiBody("miSubProcess").beginScope("miSubProcess").activity("subProcessTask").endScope().beginScope("miSubProcess").activity("subProcessTask").endScope().beginScope("miSubProcess").activity("subProcessTask").endScope().beginScope("miSubProcess").activity("subProcessTask").endScope().done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstance.Id, processEngine);
		assertThat(executionTree).matches(describeExecutionTree(null).scope().child(null).scope().child(null).concurrent().noScope().child("subProcessTask").scope().up().up().child(null).concurrent().noScope().child("subProcessTask").scope().up().up().child(null).concurrent().noScope().child("subProcessTask").scope().up().up().child(null).concurrent().noScope().child("subProcessTask").scope().done());

		// and the process is able to complete successfully
		completeTasksInOrder("subProcessTask", "subProcessTask", "subProcessTask", "subProcessTask", "afterTask");
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : PARALLEL_MULTI_INSTANCE_TASK_PROCESS)]
	  public virtual void testStartBeforeInnerActivityWithMiBodyParallelTasks()
	  {
		// given the mi body is not yet instantiated
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miParallelUserTasks");

		// when
		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("miTasks").execute();

		// then the mi variables should be correct
		Execution leafExecution = runtimeService.createExecutionQuery().activityId("miTasks").singleResult();
		assertNotNull(leafExecution);
		assertVariable(leafExecution, "loopCounter", 0);
		assertVariable(leafExecution, "nrOfInstances", 1);
		assertVariable(leafExecution, "nrOfCompletedInstances", 0);
		assertVariable(leafExecution, "nrOfActiveInstances", 1);

		// and the tree should be correct
		tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("beforeTask").beginMiBody("miTasks").activity("miTasks").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstance.Id, processEngine);
		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("beforeTask").concurrent().noScope().up().child(null).concurrent().noScope().child(null).scope().child("miTasks").concurrent().noScope().done());

		// and the process is able to complete successfully
		completeTasksInOrder("miTasks", "afterTask", "beforeTask", "miTasks", "miTasks", "miTasks", "afterTask");
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : PARALLEL_MULTI_INSTANCE_TASK_PROCESS)]
	  public virtual void testStartBeforeInnerActivityWithMiBodyParallelTasksActivityStatistics()
	  {
		// given the mi body is not yet instantiated
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miParallelUserTasks");

		// when
		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("miTasks").execute();

		// then the activity instance statistics are correct
		IList<ActivityStatistics> statistics = managementService.createActivityStatisticsQuery(processInstance.ProcessDefinitionId).list();
		assertEquals(2, statistics.Count);

		ActivityStatistics miTasksStatistics = getStatisticsForActivity(statistics, "miTasks");
		assertNotNull(miTasksStatistics);
		assertEquals(1, miTasksStatistics.Instances);

		ActivityStatistics beforeTaskStatistics = getStatisticsForActivity(statistics, "beforeTask");
		assertNotNull(beforeTaskStatistics);
		assertEquals(1, beforeTaskStatistics.Instances);
	  }

	  [Deployment(resources : PARALLEL_MULTI_INSTANCE_SUBPROCESS_PROCESS)]
	  public virtual void testStartBeforeInnerActivityWithMiBodyParallelSubprocess()
	  {
		// given the mi body is not yet instantiated
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miParallelSubprocess");

		// when
		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("subProcessTask").execute();

		// then the mi variables should be correct
		Execution leafExecution = runtimeService.createExecutionQuery().activityId("subProcessTask").singleResult();
		assertNotNull(leafExecution);
		assertVariable(leafExecution, "loopCounter", 0);
		assertVariable(leafExecution, "nrOfInstances", 1);
		assertVariable(leafExecution, "nrOfCompletedInstances", 0);
		assertVariable(leafExecution, "nrOfActiveInstances", 1);

		// and the tree should be correct
		tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("beforeTask").beginMiBody("miSubProcess").beginScope("miSubProcess").activity("subProcessTask").endScope().done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstance.Id, processEngine);
		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("beforeTask").concurrent().noScope().up().child(null).concurrent().noScope().child(null).scope().child(null).concurrent().noScope().child("subProcessTask").scope().done());

		// and the process is able to complete successfully
		completeTasksInOrder("subProcessTask", "afterTask", "beforeTask", "subProcessTask", "subProcessTask", "subProcessTask", "afterTask");
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : PARALLEL_MULTI_INSTANCE_SUBPROCESS_PROCESS)]
	  public virtual void testStartBeforeInnerActivityWithMiBodyParallelSubprocessActivityStatistics()
	  {
		// given the mi body is not yet instantiated
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miParallelSubprocess");

		// when
		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("subProcessTask").execute();

		// then the activity instance statistics are correct
		IList<ActivityStatistics> statistics = managementService.createActivityStatisticsQuery(processInstance.ProcessDefinitionId).list();
		assertEquals(2, statistics.Count);

		ActivityStatistics miTasksStatistics = getStatisticsForActivity(statistics, "subProcessTask");
		assertNotNull(miTasksStatistics);
		assertEquals(1, miTasksStatistics.Instances);

		ActivityStatistics beforeTaskStatistics = getStatisticsForActivity(statistics, "beforeTask");
		assertNotNull(beforeTaskStatistics);
		assertEquals(1, beforeTaskStatistics.Instances);
	  }


	  [Deployment(resources : PARALLEL_MULTI_INSTANCE_SUBPROCESS_PROCESS)]
	  public virtual void testStartBeforeInnerActivityWithMiBodySetNrOfInstancesParallelSubprocess()
	  {
		// given the mi body is not yet instantiated
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miParallelSubprocess");

		// when
		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("subProcessTask").setVariable("nrOfInstances", 3).execute();

		// then the mi variables should be correct
		Execution leafExecution = runtimeService.createExecutionQuery().activityId("subProcessTask").singleResult();
		assertNotNull(leafExecution);
		assertVariable(leafExecution, "loopCounter", 0);
		assertVariable(leafExecution, "nrOfInstances", 3);
		assertVariable(leafExecution, "nrOfCompletedInstances", 0);
		assertVariable(leafExecution, "nrOfActiveInstances", 1);

		// and the trees should be correct
		tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("beforeTask").beginMiBody("miSubProcess").beginScope("miSubProcess").activity("subProcessTask").endScope().done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstance.Id, processEngine);
		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("beforeTask").concurrent().noScope().up().child(null).concurrent().noScope().child(null).scope().child(null).concurrent().noScope().child("subProcessTask").scope().done());

		// and completing the single active instance completes the mi body (even though nrOfInstances is 3;
		// joining is performed on the number of concurrent executions)
		completeTasksInOrder("subProcessTask");
		tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("beforeTask").activity("afterTask").done());

		// and the remainder of the process completes successfully
		completeTasksInOrder("beforeTask", "subProcessTask", "afterTask", "subProcessTask", "subProcessTask", "afterTask");
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : SEQUENTIAL_MULTI_INSTANCE_TASK_PROCESS)]
	  public virtual void testStartBeforeInnerActivitySequentialTasks()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miSequentialUserTasks");
		completeTasksInOrder("beforeTask");

		// then creating a second inner instance is not possible
		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		try
		{
		  runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("miTasks", tree.getActivityInstances("miTasks#multiInstanceBody")[0].Id).execute();
		  fail("expect exception");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent(e.Message, "Concurrent instantiation not possible for activities " + "in scope miTasks#multiInstanceBody");
		}

	  }

	  [Deployment(resources : SEQUENTIAL_MULTI_INSTANCE_SUBPROCESS_PROCESS)]
	  public virtual void testStartBeforeInnerActivitySequentialSubprocess()
	  {
		// given the mi body is already instantiated
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miSequentialSubprocess");
		completeTasksInOrder("beforeTask");

		// when
		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		try
		{
		  runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("miSubProcess", tree.getActivityInstances("miSubProcess#multiInstanceBody")[0].Id).execute();
		  fail("expect exception");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent(e.Message, "Concurrent instantiation not possible for activities " + "in scope miSubProcess#multiInstanceBody");
		}
	  }

	  [Deployment(resources : SEQUENTIAL_MULTI_INSTANCE_TASK_PROCESS)]
	  public virtual void testStartBeforeInnerActivityWithMiBodySequentialTasks()
	  {
		// given the mi body is not yet instantiated
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miSequentialUserTasks");

		// when
		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("miTasks").execute();

		// then the mi variables should be correct
		Execution leafExecution = runtimeService.createExecutionQuery().activityId("miTasks").singleResult();
		assertNotNull(leafExecution);
		assertVariable(leafExecution, "loopCounter", 0);
		assertVariable(leafExecution, "nrOfInstances", 1);
		assertVariable(leafExecution, "nrOfCompletedInstances", 0);
		assertVariable(leafExecution, "nrOfActiveInstances", 1);

		// and the trees should be correct
		tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("beforeTask").beginMiBody("miTasks").activity("miTasks").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstance.Id, processEngine);
		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("beforeTask").concurrent().noScope().up().child(null).concurrent().noScope().child("miTasks").scope().done());

		// and the process is able to complete successfully
		completeTasksInOrder("miTasks", "afterTask", "beforeTask", "miTasks", "miTasks", "miTasks", "afterTask");
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : SEQUENTIAL_MULTI_INSTANCE_TASK_PROCESS)]
	  public virtual void testStartBeforeInnerActivityWithMiBodySequentialTasksActivityStatistics()
	  {
		// given the mi body is not yet instantiated
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miSequentialUserTasks");

		// when
		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("miTasks").execute();

		// then the activity instance statistics are correct
		IList<ActivityStatistics> statistics = managementService.createActivityStatisticsQuery(processInstance.ProcessDefinitionId).list();
		assertEquals(2, statistics.Count);

		ActivityStatistics miTasksStatistics = getStatisticsForActivity(statistics, "miTasks");
		assertNotNull(miTasksStatistics);
		assertEquals(1, miTasksStatistics.Instances);

		ActivityStatistics beforeTaskStatistics = getStatisticsForActivity(statistics, "beforeTask");
		assertNotNull(beforeTaskStatistics);
		assertEquals(1, beforeTaskStatistics.Instances);
	  }

	  protected internal virtual ActivityStatistics getStatisticsForActivity(IList<ActivityStatistics> statistics, string activityId)
	  {
		foreach (ActivityStatistics statisticsInstance in statistics)
		{
		  if (statisticsInstance.Id.Equals(activityId))
		  {
			return statisticsInstance;
		  }
		}
		return null;
	  }

	  [Deployment(resources : SEQUENTIAL_MULTI_INSTANCE_SUBPROCESS_PROCESS)]
	  public virtual void testStartBeforeInnerActivityWithMiBodySequentialSubprocess()
	  {
		// given the mi body is not yet instantiated
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miSequentialSubprocess");

		// when
		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("subProcessTask").execute();

		// then the mi variables should be correct
		Execution leafExecution = runtimeService.createExecutionQuery().activityId("subProcessTask").singleResult();
		assertNotNull(leafExecution);
		assertVariable(leafExecution, "loopCounter", 0);
		assertVariable(leafExecution, "nrOfInstances", 1);
		assertVariable(leafExecution, "nrOfCompletedInstances", 0);
		assertVariable(leafExecution, "nrOfActiveInstances", 1);

		// and the trees should be correct
		tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("beforeTask").beginMiBody("miSubProcess").beginScope("miSubProcess").activity("subProcessTask").endScope().done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstance.Id, processEngine);
		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("beforeTask").concurrent().noScope().up().child(null).concurrent().noScope().child(null).scope().child("subProcessTask").scope().done());

		// and the process is able to complete successfully
		completeTasksInOrder("subProcessTask", "afterTask", "beforeTask", "subProcessTask", "subProcessTask", "subProcessTask", "afterTask");
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : SEQUENTIAL_MULTI_INSTANCE_SUBPROCESS_PROCESS)]
	  public virtual void testStartBeforeInnerActivityWithMiBodySequentialSubprocessActivityStatistics()
	  {
		// given the mi body is not yet instantiated
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miSequentialSubprocess");

		// when
		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("subProcessTask").execute();

		// then the activity instance statistics are correct
		IList<ActivityStatistics> statistics = managementService.createActivityStatisticsQuery(processInstance.ProcessDefinitionId).list();
		assertEquals(2, statistics.Count);

		ActivityStatistics miTasksStatistics = getStatisticsForActivity(statistics, "subProcessTask");
		assertNotNull(miTasksStatistics);
		assertEquals(1, miTasksStatistics.Instances);

		ActivityStatistics beforeTaskStatistics = getStatisticsForActivity(statistics, "beforeTask");
		assertNotNull(beforeTaskStatistics);
		assertEquals(1, beforeTaskStatistics.Instances);
	  }

	  [Deployment(resources : SEQUENTIAL_MULTI_INSTANCE_SUBPROCESS_PROCESS)]
	  public virtual void testStartBeforeInnerActivityWithMiBodySetNrOfInstancesSequentialSubprocess()
	  {
		// given the mi body is not yet instantiated
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miSequentialSubprocess");

		// when
		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("subProcessTask").setVariable("nrOfInstances", 3).execute();

		// then the mi variables should be correct
		Execution leafExecution = runtimeService.createExecutionQuery().activityId("subProcessTask").singleResult();
		assertNotNull(leafExecution);
		assertVariable(leafExecution, "loopCounter", 0);
		assertVariable(leafExecution, "nrOfInstances", 3);
		assertVariable(leafExecution, "nrOfCompletedInstances", 0);
		assertVariable(leafExecution, "nrOfActiveInstances", 1);

		// and the trees should be correct
		tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("beforeTask").beginMiBody("miSubProcess").beginScope("miSubProcess").activity("subProcessTask").endScope().done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstance.Id, processEngine);
		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("beforeTask").concurrent().noScope().up().child(null).concurrent().noScope().child(null).scope().child("subProcessTask").scope().done());

		// and two following sequential instances should be created
		completeTasksInOrder("subProcessTask");

		tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("beforeTask").beginMiBody("miSubProcess").beginScope("miSubProcess").activity("subProcessTask").done());

		leafExecution = runtimeService.createExecutionQuery().activityId("subProcessTask").singleResult();
		assertNotNull(leafExecution);
		assertVariable(leafExecution, "loopCounter", 1);
		assertVariable(leafExecution, "nrOfInstances", 3);
		assertVariable(leafExecution, "nrOfCompletedInstances", 1);
		assertVariable(leafExecution, "nrOfActiveInstances", 1);

		completeTasksInOrder("subProcessTask");

		// and the remainder of the process completes successfully
		completeTasksInOrder("subProcessTask", "beforeTask", "subProcessTask", "subProcessTask", "subProcessTask", "afterTask", "afterTask");
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : PARALLEL_MULTI_INSTANCE_TASK_PROCESS)]
	  public virtual void testCancelMultiInstanceBodyParallelTasks()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miParallelUserTasks");
		completeTasksInOrder("beforeTask");

		// when
		runtimeService.createProcessInstanceModification(processInstance.Id).cancelAllForActivity("miTasks#multiInstanceBody").execute();

		// then
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : PARALLEL_MULTI_INSTANCE_SUBPROCESS_PROCESS)]
	  public virtual void testCancelMultiInstanceBodyParallelSubprocess()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miParallelSubprocess");
		completeTasksInOrder("beforeTask");

		// when
		runtimeService.createProcessInstanceModification(processInstance.Id).cancelAllForActivity("miSubProcess#multiInstanceBody").execute();

		// then
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : SEQUENTIAL_MULTI_INSTANCE_TASK_PROCESS)]
	  public virtual void testCancelMultiInstanceBodySequentialTasks()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miSequentialUserTasks");
		completeTasksInOrder("beforeTask");

		// when
		runtimeService.createProcessInstanceModification(processInstance.Id).cancelAllForActivity("miTasks#multiInstanceBody").execute();

		// then
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : SEQUENTIAL_MULTI_INSTANCE_SUBPROCESS_PROCESS)]
	  public virtual void testCancelMultiInstanceBodySequentialSubprocess()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miSequentialSubprocess");
		completeTasksInOrder("beforeTask");

		// when
		runtimeService.createProcessInstanceModification(processInstance.Id).cancelAllForActivity("miSubProcess#multiInstanceBody").execute();

		// then
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : PARALLEL_MULTI_INSTANCE_TASK_PROCESS)]
	  public virtual void testCancelInnerActivityParallelTasks()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miParallelUserTasks");
		completeTasksInOrder("beforeTask");

		// when
		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(tree.getActivityInstances("miTasks")[0].Id).execute();

		// then
		tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginMiBody("miTasks").activity("miTasks").activity("miTasks").endScope().done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstance.Id, processEngine);
		assertThat(executionTree).matches(describeExecutionTree(null).scope().child(null).scope().child("miTasks").concurrent().noScope().up().child("miTasks").concurrent().noScope().up().done());

		// and the process is able to complete successfully
		completeTasksInOrder("miTasks", "miTasks", "afterTask");
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : PARALLEL_MULTI_INSTANCE_TASK_PROCESS)]
	  public virtual void testCancelAllInnerActivityParallelTasks()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miParallelUserTasks");
		completeTasksInOrder("beforeTask");

		// when
		runtimeService.createProcessInstanceModification(processInstance.Id).cancelAllForActivity("miTasks").execute();

		// then
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : NESTED_PARALLEL_MULTI_INSTANCE_TASK_PROCESS)]
	  public virtual void testCancelAllInnerActivityNestedParallelTasks()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nestedMiParallelUserTasks");
		completeTasksInOrder("beforeTask");

		// when
		runtimeService.createProcessInstanceModification(processInstance.Id).cancelAllForActivity("miTasks").execute();

		// then
		assertProcessEnded(processInstance.Id);
	  }

	  /// <summary>
	  /// Ensures that the modification cmd does not prune the last concurrent execution
	  /// because parallel MI requires this
	  /// </summary>
	  [Deployment(resources : PARALLEL_MULTI_INSTANCE_TASK_PROCESS)]
	  public virtual void testCancelInnerActivityParallelTasksAllButOne()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miParallelUserTasks");
		completeTasksInOrder("beforeTask");

		// when
		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(tree.getActivityInstances("miTasks")[0].Id).cancelActivityInstance(tree.getActivityInstances("miTasks")[1].Id).execute();

		// then
		tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginMiBody("miTasks").activity("miTasks").endScope().done());

		// the execution tree should still be in the expected shape
		ExecutionTree executionTree = ExecutionTree.forExecution(processInstance.Id, processEngine);
		assertThat(executionTree).matches(describeExecutionTree(null).scope().child(null).scope().child("miTasks").concurrent().noScope().done());

		// and the process is able to complete successfully
		completeTasksInOrder("miTasks", "afterTask");
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : PARALLEL_MULTI_INSTANCE_SUBPROCESS_PROCESS)]
	  public virtual void testCancelInnerActivityParallelSubprocess()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miParallelSubprocess");
		completeTasksInOrder("beforeTask");

		// when
		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(tree.getActivityInstances("miSubProcess")[0].Id).execute();

		// then
		tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginMiBody("miSubProcess").beginScope("miSubProcess").activity("subProcessTask").endScope().beginScope("miSubProcess").activity("subProcessTask").endScope().done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstance.Id, processEngine);
		assertThat(executionTree).matches(describeExecutionTree(null).scope().child(null).scope().child(null).concurrent().noScope().child("subProcessTask").scope().up().up().child(null).concurrent().noScope().child("subProcessTask").scope().up().up().done());

		// and the process is able to complete successfully
		completeTasksInOrder("subProcessTask", "subProcessTask", "afterTask");
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : SEQUENTIAL_MULTI_INSTANCE_TASK_PROCESS)]
	  public virtual void testCancelInnerActivitySequentialTasks()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miSequentialUserTasks");
		completeTasksInOrder("beforeTask");

		// when
		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(tree.getActivityInstances("miTasks")[0].Id).execute();

		// then
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : SEQUENTIAL_MULTI_INSTANCE_TASK_PROCESS)]
	  public virtual void testCancelAllInnerActivitySequentialTasks()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miSequentialUserTasks");
		completeTasksInOrder("beforeTask");

		// when
		runtimeService.createProcessInstanceModification(processInstance.Id).cancelAllForActivity("miTasks").execute();

		// then
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : SEQUENTIAL_MULTI_INSTANCE_SUBPROCESS_PROCESS)]
	  public virtual void testCancelInnerActivitySequentialSubprocess()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miSequentialSubprocess");
		completeTasksInOrder("beforeTask");

		// when
		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(tree.getActivityInstances("miSubProcess")[0].Id).execute();

		// then
		assertProcessEnded(processInstance.Id);
	  }

	  protected internal virtual void completeTasksInOrder(params string[] taskNames)
	  {
		foreach (string taskName in taskNames)
		{
		  // complete any task with that name
		  IList<Task> tasks = taskService.createTaskQuery().taskDefinitionKey(taskName).listPage(0, 1);
		  assertTrue("task for activity " + taskName + " does not exist", tasks.Count > 0);
		  taskService.complete(tasks[0].Id);
		}
	  }


	  protected internal virtual void assertVariable(Execution execution, string variableName, object expectedValue)
	  {
		object variableValue = runtimeService.getVariable(execution.Id, variableName);
		assertEquals("Value for variable '" + variableName + "' and " + execution + " " + "does not match.", expectedValue, variableValue);
	  }

	  protected internal virtual void assertVariableSet<T1>(IList<Execution> executions, string variableName, IList<T1> expectedValues)
	  {
		IList<object> actualValues = new List<object>();
		foreach (Execution execution in executions)
		{
		  actualValues.Add(runtimeService.getVariable(execution.Id, variableName));
		}

		foreach (object expectedValue in expectedValues)
		{
		  bool valueFound = actualValues.Remove(expectedValue);
		  assertTrue("Expected variable value '" + expectedValue + "' not contained in the list of actual values. " + "Unmatched actual values: " + actualValues, valueFound);
		}
		assertTrue("There are more actual than expected values.", actualValues.Count == 0);
	  }

	}

}