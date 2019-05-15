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
namespace org.camunda.bpm.engine.rest.hal.task
{
	using Task = org.camunda.bpm.engine.task.Task;


	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class HalTaskList : HalCollectionResource<HalTaskList>
	{

	  public static HalTaskList generate(IList<Task> tasks, long count, ProcessEngine engine)
	  {
		return fromTaskList(tasks, count).embed(HalTask.REL_ASSIGNEE, engine).embed(HalTask.REL_OWNER, engine).embed(HalTask.REL_PROCESS_DEFINITION, engine).embed(HalTask.REL_CASE_DEFINITION, engine);
	  }

	  public static HalTaskList fromTaskList(IList<Task> tasks, long count)
	  {

		HalTaskList taskList = new HalTaskList();

		// embed tasks
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.engine.rest.hal.HalResource<?>> embeddedTasks = new java.util.ArrayList<org.camunda.bpm.engine.rest.hal.HalResource<?>>();
		IList<HalResource<object>> embeddedTasks = new List<HalResource<object>>();
		foreach (Task task in tasks)
		{
		  embeddedTasks.Add(HalTask.fromTask(task));
		}

		taskList.addEmbedded("task", embeddedTasks);

		// links
		taskList.addLink("self", fromPath(org.camunda.bpm.engine.rest.TaskRestService_Fields.PATH).build());

		taskList.count = count;

		return taskList;
	  }

	}

}