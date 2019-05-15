using System;
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
namespace org.camunda.bpm.engine.impl.cmd
{

	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;

	/// <summary>
	/// Patches task variables: First, applies modifications to existing variables and then deletes
	/// specified variables.
	/// 
	/// @author kristin.polenz@camunda.com
	/// 
	/// </summary>
	[Serializable]
	public class PatchTaskVariablesCmd : AbstractPatchVariablesCmd
	{

	  private const long serialVersionUID = 1L;

	  public PatchTaskVariablesCmd<T1>(string taskId, IDictionary<T1> modifications, ICollection<string> deletions, bool isLocal) where T1 : object : base(taskId, modifications, deletions, isLocal)
	  {
	  }

	  protected internal override AbstractSetVariableCmd SetVariableCmd
	  {
		  get
		  {
			return new SetTaskVariablesCmd(entityId, variables, isLocal);
		  }
	  }

	  protected internal override AbstractRemoveVariableCmd RemoveVariableCmd
	  {
		  get
		  {
			return new RemoveTaskVariablesCmd(entityId, deletions, isLocal);
		  }
	  }

	  public override void logVariableOperation(CommandContext commandContext)
	  {
		commandContext.OperationLogManager.logVariableOperation(LogEntryOperation, null, entityId, PropertyChange.EMPTY_CHANGE);
	  }
	}

}