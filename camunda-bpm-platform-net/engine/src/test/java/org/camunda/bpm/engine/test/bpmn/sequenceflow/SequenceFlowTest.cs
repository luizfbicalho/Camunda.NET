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
namespace org.camunda.bpm.engine.test.bpmn.sequenceflow
{
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class SequenceFlowTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTakeAllOutgoingFlowsFromNonScopeTask()
	  public virtual void testTakeAllOutgoingFlowsFromNonScopeTask()
	  {
		// given
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("testProcess");

		// when
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

		// then
		assertEquals(2, taskService.createTaskQuery().count());
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey("task2").count());
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey("task3").count());

		foreach (Task followUpTask in taskService.createTaskQuery().list())
		{
		  taskService.complete(followUpTask.Id);
		}

		assertProcessEnded(instance.Id);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTakeAllOutgoingFlowsFromScopeTask()
	  public virtual void testTakeAllOutgoingFlowsFromScopeTask()
	  {
		// given
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("testProcess");

		// when
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

		// then
		assertEquals(2, taskService.createTaskQuery().count());
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey("task2").count());
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey("task3").count());

		foreach (Task followUpTask in taskService.createTaskQuery().list())
		{
		  taskService.complete(followUpTask.Id);
		}

		assertProcessEnded(instance.Id);
	  }
	}

}