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
namespace org.camunda.bpm.engine.test.api.task
{
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class SubTaskDataTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule rule = new org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule();
	  public ProcessEngineRule rule = new ProvidedProcessEngineRule();

	  protected internal RepositoryService repositoryService;
	  protected internal RuntimeService runtimeService;
	  protected internal TaskService taskService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		repositoryService = rule.RepositoryService;
		runtimeService = rule.RuntimeService;
		taskService = rule.TaskService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testSubTaskData()
	  public virtual void testSubTaskData()
	  {
		//given simple process with user task
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("subTaskTest");
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();

		// when set variable to user task
		taskService.setVariable(task.Id, "testVariable", "testValue");

		// then variable is set in the scope of execution
		Assert.assertEquals("testValue", runtimeService.getVariable(task.ExecutionId, "testVariable"));

		// when sub task is created create subtask for user task
		Task subTask = taskService.newTask("123456789");
		subTask.ParentTaskId = task.Id;
		subTask.Name = "Test Subtask";
		taskService.saveTask(subTask);

		// and variable is update
		taskService.setVariable(subTask.Id, "testVariable", "newTestValue");

		//then variable is also updated in the scope execution
		Assert.assertEquals("newTestValue", runtimeService.getVariable(task.ExecutionId, "testVariable"));
	  }
	}

}