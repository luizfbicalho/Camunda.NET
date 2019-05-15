﻿using System;
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
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using PvmExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	[Serializable]
	public class FoxDeleteProcessInstanceCmd : Command<Void>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string processInstanceId;
	  protected internal string deleteReason;

	  public FoxDeleteProcessInstanceCmd(string processInstanceId, string deleteReason)
	  {
		this.processInstanceId = processInstanceId;
		this.deleteReason = deleteReason;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		ensureNotNull("processInstanceId", processInstanceId);

		ExecutionEntity execution = commandContext.ExecutionManager.findExecutionById(processInstanceId);

		ensureNotNull("No process instance found for id '" + processInstanceId + "'", "execution", execution);

		commandContext.TaskManager.deleteTasksByProcessInstanceId(processInstanceId, deleteReason, false, false);

		foreach (PvmExecutionImpl currentExecution in this.collectExecutionToDelete(execution))
		{
		  currentExecution.deleteCascade2(deleteReason);
		}
		return null;
	  }

	  public virtual IList<PvmExecutionImpl> collectExecutionToDelete(PvmExecutionImpl execution)
	  {
		IList<PvmExecutionImpl> result = new List<PvmExecutionImpl>();
		foreach (PvmExecutionImpl currentExecution in execution.Executions)
		{
		  ((IList<PvmExecutionImpl>)result).AddRange(this.collectExecutionToDelete(currentExecution));
		}
		if (execution.SubProcessInstance != null)
		{
		  ((IList<PvmExecutionImpl>)result).AddRange(this.collectExecutionToDelete(execution.SubProcessInstance));
		}
		result.Add(execution);
		return result;
	  }

	}

}