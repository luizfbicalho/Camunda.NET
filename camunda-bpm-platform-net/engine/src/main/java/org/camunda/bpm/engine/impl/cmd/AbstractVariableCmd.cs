using System;

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
	using AbstractVariableScope = org.camunda.bpm.engine.impl.core.variable.scope.AbstractVariableScope;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using Callback = org.camunda.bpm.engine.impl.pvm.runtime.Callback;
	using PvmExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl;

	/// <summary>
	/// @author Stefan Hentschel.
	/// </summary>
	[Serializable]
	public abstract class AbstractVariableCmd : Command<Void>
	{

	  private const long serialVersionUID = 1L;

	  protected internal CommandContext commandContext;
	  protected internal string entityId;
	  protected internal bool isLocal;
	  protected internal bool preventLogUserOperation = false;

	  public AbstractVariableCmd(string entityId, bool isLocal)
	  {
		this.entityId = entityId;
		this.isLocal = isLocal;
	  }

	  public virtual AbstractVariableCmd disableLogUserOperation()
	  {
		this.preventLogUserOperation = true;
		return this;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		this.commandContext = commandContext;

		AbstractVariableScope scope = Entity;

		executeOperation(scope);

		ExecutionEntity contextExecution = ContextExecution;
		if (contextExecution != null)
		{
		  contextExecution.dispatchDelayedEventsAndPerformOperation((Callback<PvmExecutionImpl, Void>) null);
		}

		if (!preventLogUserOperation)
		{
		  logVariableOperation(scope);
		}

		return null;
	  };

	  protected internal abstract AbstractVariableScope Entity {get;}

	  protected internal abstract ExecutionEntity ContextExecution {get;}

	  protected internal abstract void logVariableOperation(AbstractVariableScope scope);

	  protected internal abstract void executeOperation(AbstractVariableScope scope);

	  protected internal abstract string LogEntryOperation {get;}
	}

}