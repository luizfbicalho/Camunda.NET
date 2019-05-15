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
namespace org.camunda.bpm.engine.test.bpmn.tasklistener.util
{

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using DelegateTask = org.camunda.bpm.engine.@delegate.DelegateTask;
	using TaskListener = org.camunda.bpm.engine.@delegate.TaskListener;


	/// <summary>
	/// @author Falko Menge <falko.menge@camunda.com>
	/// </summary>
	public class AssigneeOverwriteFromVariable : TaskListener
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public void notify(org.camunda.bpm.engine.delegate.DelegateTask delegateTask)
	  public virtual void notify(DelegateTask delegateTask)
	  {
		// get mapping table from variable
		DelegateExecution execution = delegateTask.Execution;
		IDictionary<string, string> assigneeMappingTable = (IDictionary<string, string>) execution.getVariable("assigneeMappingTable");

		// get assignee from process
		string assigneeFromProcessDefinition = delegateTask.Assignee;

		// overwrite assignee if there is an entry in the mapping table
		if (assigneeMappingTable.ContainsKey(assigneeFromProcessDefinition))
		{
		  string assigneeFromMappingTable = assigneeMappingTable[assigneeFromProcessDefinition];
		  delegateTask.Assignee = assigneeFromMappingTable;
		}
	  }

	}

}