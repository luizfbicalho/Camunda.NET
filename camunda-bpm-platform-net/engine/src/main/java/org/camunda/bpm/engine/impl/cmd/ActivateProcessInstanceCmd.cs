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
namespace org.camunda.bpm.engine.impl.cmd
{
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using UpdateJobSuspensionStateBuilderImpl = org.camunda.bpm.engine.impl.management.UpdateJobSuspensionStateBuilderImpl;
	using SuspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState;
	using UpdateProcessInstanceSuspensionStateBuilderImpl = org.camunda.bpm.engine.impl.runtime.UpdateProcessInstanceSuspensionStateBuilderImpl;

	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class ActivateProcessInstanceCmd : AbstractSetProcessInstanceStateCmd
	{

	  public ActivateProcessInstanceCmd(UpdateProcessInstanceSuspensionStateBuilderImpl builder) : base(builder)
	  {
	  }

	  protected internal override SuspensionState NewSuspensionState
	  {
		  get
		  {
			return org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.ACTIVE;
		  }
	  }

	  protected internal override ActivateJobCmd getNextCommand(UpdateJobSuspensionStateBuilderImpl jobCommandBuilder)
	  {
		return new ActivateJobCmd(jobCommandBuilder);
	  }

	  protected internal override string LogEntryOperation
	  {
		  get
		  {
			return org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_ACTIVATE;
		  }
	  }

	}

}