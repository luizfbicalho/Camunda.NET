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
namespace org.camunda.bpm.engine.test.api.task
{

	using HistoricTaskInstance = org.camunda.bpm.engine.history.HistoricTaskInstance;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Task = org.camunda.bpm.engine.task.Task;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class SubTaskTest : PluggableProcessEngineTestCase
	{

	  public virtual void testSubTask()
	  {
		Task gonzoTask = taskService.newTask();
		gonzoTask.Name = "gonzoTask";
		taskService.saveTask(gonzoTask);

		Task subTaskOne = taskService.newTask();
		subTaskOne.Name = "subtask one";
		string gonzoTaskId = gonzoTask.Id;
		subTaskOne.ParentTaskId = gonzoTaskId;
		taskService.saveTask(subTaskOne);

		Task subTaskTwo = taskService.newTask();
		subTaskTwo.Name = "subtask two";
		subTaskTwo.ParentTaskId = gonzoTaskId;
		taskService.saveTask(subTaskTwo);

		string subTaskId = subTaskOne.Id;
		assertTrue(taskService.getSubTasks(subTaskId).Count == 0);
		assertTrue(historyService.createHistoricTaskInstanceQuery().taskParentTaskId(subTaskId).list().Empty);

		IList<Task> subTasks = taskService.getSubTasks(gonzoTaskId);
		ISet<string> subTaskNames = new HashSet<string>();
		foreach (Task subTask in subTasks)
		{
		  subTaskNames.Add(subTask.Name);
		}

		if (processEngineConfiguration.HistoryLevel.Id >= org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_AUDIT.Id)
		{
		  ISet<string> expectedSubTaskNames = new HashSet<string>();
		  expectedSubTaskNames.Add("subtask one");
		  expectedSubTaskNames.Add("subtask two");

		  assertEquals(expectedSubTaskNames, subTaskNames);

		  IList<HistoricTaskInstance> historicSubTasks = historyService.createHistoricTaskInstanceQuery().taskParentTaskId(gonzoTaskId).list();

		  subTaskNames = new HashSet<string>();
		  foreach (HistoricTaskInstance historicSubTask in historicSubTasks)
		  {
			subTaskNames.Add(historicSubTask.Name);
		  }

		  assertEquals(expectedSubTaskNames, subTaskNames);
		}

		taskService.deleteTask(gonzoTaskId, true);
	  }
	}

}