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
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;



	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class DeleteProcessInstancesCmd : AbstractDeleteProcessInstanceCmd, Command<Void>
	{

	  private const long serialVersionUID = 1L;
	  protected internal IList<string> processInstanceIds;

	  public DeleteProcessInstancesCmd(IList<string> processInstanceIds, string deleteReason, bool skipCustomListeners, bool externallyTerminated, bool skipSubprocesses, bool failIfNotExists)
	  {

		this.processInstanceIds = processInstanceIds;
		this.deleteReason = deleteReason;
		this.skipCustomListeners = skipCustomListeners;
		this.externallyTerminated = externallyTerminated;
		this.skipSubprocesses = skipSubprocesses;
		this.failIfNotExists = failIfNotExists;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		foreach (string processInstanceId in this.processInstanceIds)
		{
		  deleteProcessInstance(commandContext, processInstanceId, deleteReason, skipCustomListeners, externallyTerminated, false, skipSubprocesses);
		}
		return null;
	  }

	}

}