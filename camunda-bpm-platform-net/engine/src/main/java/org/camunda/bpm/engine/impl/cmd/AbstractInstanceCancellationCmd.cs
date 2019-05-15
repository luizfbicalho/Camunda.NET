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
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ModificationUtil = org.camunda.bpm.engine.impl.util.ModificationUtil;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public abstract class AbstractInstanceCancellationCmd : AbstractProcessInstanceModificationCommand
	{

	  protected internal string cancellationReason;

	  public AbstractInstanceCancellationCmd(string processInstanceId) : base(processInstanceId)
	  {
		this.cancellationReason = "Cancellation due to process instance modifcation";
	  }

	  public AbstractInstanceCancellationCmd(string processInstanceId, string cancellationReason) : base(processInstanceId)
	  {
		this.cancellationReason = cancellationReason;
	  }

	  public override Void execute(CommandContext commandContext)
	  {
		ExecutionEntity sourceInstanceExecution = determineSourceInstanceExecution(commandContext);

		// Outline:
		// 1. find topmost scope execution beginning at scopeExecution that has exactly
		//    one child (this is the topmost scope we can cancel)
		// 2. cancel all children of the topmost execution
		// 3. cancel the activity of the topmost execution itself (if applicable)
		// 4. remove topmost execution (and concurrent parent) if topmostExecution is not the process instance

		ExecutionEntity topmostCancellableExecution = sourceInstanceExecution;
		ExecutionEntity parentScopeExecution = (ExecutionEntity) topmostCancellableExecution.getParentScopeExecution(false);

		// if topmostCancellableExecution's scope execution has no other non-event-scope children,
		// we have reached the correct execution
		while (parentScopeExecution != null && (parentScopeExecution.NonEventScopeExecutions.Count <= 1))
		{
			topmostCancellableExecution = parentScopeExecution;
			parentScopeExecution = (ExecutionEntity) topmostCancellableExecution.getParentScopeExecution(false);
		}

		if (topmostCancellableExecution.PreserveScope)
		{
		  topmostCancellableExecution.interrupt(cancellationReason, skipCustomListeners, skipIoMappings);
		  topmostCancellableExecution.leaveActivityInstance();
		  topmostCancellableExecution.setActivity(null);
		}
		else
		{
		  topmostCancellableExecution.deleteCascade(cancellationReason, skipCustomListeners, skipIoMappings);
		  ModificationUtil.handleChildRemovalInScope(topmostCancellableExecution);
		}

		return null;
	  }

	  protected internal abstract ExecutionEntity determineSourceInstanceExecution(CommandContext commandContext);

	  protected internal virtual ExecutionEntity findSuperExecution(ExecutionEntity parentScopeExecution, ExecutionEntity topmostCancellableExecution)
	  {
		ExecutionEntity superExecution = null;
		if (parentScopeExecution == null)
		{
		  superExecution = topmostCancellableExecution.getSuperExecution();

		}
		return superExecution;
	  }

	}

}