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
namespace org.camunda.bpm.engine
{

	/// <summary>
	/// This exception is thrown when you try to claim a task that is already claimed
	/// by someone else.
	/// 
	/// @author Jorg Heymans
	/// @author Falko Menge 
	/// </summary>
	public class TaskAlreadyClaimedException : ProcessEngineException
	{

		private const long serialVersionUID = 1L;

		/// <summary>
		/// the id of the task that is already claimed </summary>
		private string taskId;

		/// <summary>
		/// the assignee of the task that is already claimed </summary>
		private string taskAssignee;

		public TaskAlreadyClaimedException(string taskId, string taskAssignee) : base("Task '" + taskId + "' is already claimed by someone else.")
		{
			this.taskId = taskId;
			this.taskAssignee = taskAssignee;
		}

		public virtual string TaskId
		{
			get
			{
				return this.taskId;
			}
		}

		public virtual string TaskAssignee
		{
			get
			{
				return this.taskAssignee;
			}
		}

	}

}