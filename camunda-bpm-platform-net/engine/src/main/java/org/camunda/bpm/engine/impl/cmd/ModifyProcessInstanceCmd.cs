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


	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ExecutionManager = org.camunda.bpm.engine.impl.persistence.entity.ExecutionManager;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using ModificationUtil = org.camunda.bpm.engine.impl.util.ModificationUtil;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ModifyProcessInstanceCmd : Command<Void>
	{

	  private static readonly CommandLogger LOG = ProcessEngineLogger.CMD_LOGGER;

	  protected internal ProcessInstanceModificationBuilderImpl builder;
	  protected internal bool writeOperationLog;

	  public ModifyProcessInstanceCmd(ProcessInstanceModificationBuilderImpl processInstanceModificationBuilder) : this(processInstanceModificationBuilder, true)
	  {
	  }

	  public ModifyProcessInstanceCmd(ProcessInstanceModificationBuilderImpl processInstanceModificationBuilder, bool writeOperationLog)
	  {
		this.builder = processInstanceModificationBuilder;
		this.writeOperationLog = writeOperationLog;
	  }


	  public virtual Void execute(CommandContext commandContext)
	  {
		string processInstanceId = builder.ProcessInstanceId;

		ExecutionManager executionManager = commandContext.ExecutionManager;
		ExecutionEntity processInstance = executionManager.findExecutionById(processInstanceId);

		ensureProcessInstanceExist(processInstanceId, processInstance);

		checkUpdateProcessInstance(processInstance, commandContext);

		processInstance.PreserveScope = true;

		IList<AbstractProcessInstanceModificationCommand> instructions = builder.ModificationOperations;

		checkCancellation(commandContext);
		for (int i = 0; i < instructions.Count; i++)
		{

		  AbstractProcessInstanceModificationCommand instruction = instructions[i];
		  LOG.debugModificationInstruction(processInstanceId, i + 1, instruction.describe());

		  instruction.SkipCustomListeners = builder.SkipCustomListeners;
		  instruction.SkipIoMappings = builder.SkipIoMappings;
		  instruction.execute(commandContext);
		}

		processInstance = executionManager.findExecutionById(processInstanceId);

		if (!processInstance.hasChildren())
		{
		  if (processInstance.getActivity() == null)
		  {
			// process instance was cancelled
			checkDeleteProcessInstance(processInstance, commandContext);
			deletePropagate(processInstance, builder.ModificationReason, builder.SkipCustomListeners, builder.SkipIoMappings);
		  }
		  else if (processInstance.Ended)
		  {
			// process instance has ended regularly
			processInstance.propagateEnd();
		  }
		}

		if (writeOperationLog)
		{
		  commandContext.OperationLogManager.logProcessInstanceOperation(LogEntryOperation, processInstanceId, null, null, Collections.singletonList(PropertyChange.EMPTY_CHANGE));
		}

		return null;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void checkCancellation(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  private void checkCancellation(CommandContext commandContext)
	  {
		foreach (AbstractProcessInstanceModificationCommand instruction in builder.ModificationOperations)
		{
		  if (instruction is ActivityCancellationCmd && ((ActivityCancellationCmd) instruction).cancelCurrentActiveActivityInstances)
		  {
			ActivityInstance activityInstanceTree = commandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this, commandContext));
			((ActivityCancellationCmd) instruction).ActivityInstanceTreeToCancel = activityInstanceTree;
		  }
		}
	  }

	  private class CallableAnonymousInnerClass : Callable<ActivityInstance>
	  {
		  private readonly ModifyProcessInstanceCmd outerInstance;

		  private CommandContext commandContext;

		  public CallableAnonymousInnerClass(ModifyProcessInstanceCmd outerInstance, CommandContext commandContext)
		  {
			  this.outerInstance = outerInstance;
			  this.commandContext = commandContext;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public org.camunda.bpm.engine.runtime.ActivityInstance call() throws Exception
		  public override ActivityInstance call()
		  {
			return (new GetActivityInstanceCmd(((ActivityCancellationCmd) instruction).processInstanceId)).execute(commandContext);
		  }
	  }

	  protected internal virtual void ensureProcessInstanceExist(string processInstanceId, ExecutionEntity processInstance)
	  {
		if (processInstance == null)
		{
		  throw LOG.processInstanceDoesNotExist(processInstanceId);
		}
	  }

	  protected internal virtual string LogEntryOperation
	  {
		  get
		  {
			return org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_MODIFY_PROCESS_INSTANCE;
		  }
	  }

	  protected internal virtual void checkUpdateProcessInstance(ExecutionEntity execution, CommandContext commandContext)
	  {
		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkUpdateProcessInstance(execution);
		}
	  }

	  protected internal virtual void checkDeleteProcessInstance(ExecutionEntity execution, CommandContext commandContext)
	  {
		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkDeleteProcessInstance(execution);
		}
	  }

	  protected internal virtual void deletePropagate(ExecutionEntity processInstance, string deleteReason, bool skipCustomListeners, bool skipIoMappings)
	  {
		ExecutionEntity topmostDeletableExecution = processInstance;
		ExecutionEntity parentScopeExecution = (ExecutionEntity) topmostDeletableExecution.getParentScopeExecution(true);

		while (parentScopeExecution != null && (parentScopeExecution.NonEventScopeExecutions.Count <= 1))
		{
			topmostDeletableExecution = parentScopeExecution;
			parentScopeExecution = (ExecutionEntity) topmostDeletableExecution.getParentScopeExecution(true);
		}

		topmostDeletableExecution.deleteCascade(deleteReason, skipCustomListeners, skipIoMappings);
		ModificationUtil.handleChildRemovalInScope(topmostDeletableExecution);
	  }

	}

}