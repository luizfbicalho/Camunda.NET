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
namespace org.camunda.bpm.engine.test.bpmn.usertask
{

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;


	/// <summary>
	/// Simple process test to validate the current implementation protoype.
	/// 
	/// @author Joram Barrez 
	/// </summary>
	public class TaskAssigneeTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTaskAssignee()
	  public virtual void testTaskAssignee()
	  {

		// Start process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("taskAssigneeExampleProcess");

		// Get task list
		IList<Task> tasks = taskService.createTaskQuery().taskAssignee("kermit").list();
		assertEquals(1, tasks.Count);
		Task myTask = tasks[0];
		assertEquals("Schedule meeting", myTask.Name);
		assertEquals("Schedule an engineering meeting for next week with the new hire.", myTask.Description);

		// Complete task. Process is now finished
		taskService.complete(myTask.Id);
		// assert if the process instance completed
		assertProcessEnded(processInstance.Id);
	  }

	}

}