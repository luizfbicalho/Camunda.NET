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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotContainsNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotEmpty;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;

	public class ProcessInstanceModificationCmd : AbstractModificationCmd<Void>
	{

	  private static readonly CommandLogger LOG = ProcessEngineLogger.CMD_LOGGER;
	  protected internal bool writeUserOperationLog;

	  public ProcessInstanceModificationCmd(ModificationBuilderImpl modificationBuilder, bool writeUserOperationLog) : base(modificationBuilder)
	  {
		this.writeUserOperationLog = writeUserOperationLog;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public Void execute(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  public override Void execute(CommandContext commandContext)
	  {
		IList<AbstractProcessInstanceModificationCommand> instructions = builder.Instructions;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Collection<String> processInstanceIds = collectProcessInstanceIds(commandContext);
		ICollection<string> processInstanceIds = collectProcessInstanceIds(commandContext);

		ensureNotEmpty(typeof(BadUserRequestException), "Modification instructions cannot be empty", instructions);
		ensureNotEmpty(typeof(BadUserRequestException), "Process instance ids cannot be empty", "Process instance ids", processInstanceIds);
		ensureNotContainsNull(typeof(BadUserRequestException), "Process instance ids cannot be null", "Process instance ids", processInstanceIds);

		ProcessDefinitionEntity processDefinition = getProcessDefinition(commandContext, builder.ProcessDefinitionId);
		ensureNotNull(typeof(BadUserRequestException), "Process definition id cannot be null", processDefinition);

		if (writeUserOperationLog)
		{
		  writeUserOperationLog(commandContext, processDefinition, processInstanceIds.Count, false);
		}

		bool skipCustomListeners = builder.SkipCustomListeners;
		bool skipIoMappings = builder.SkipIoMappings;

		foreach (string processInstanceId in processInstanceIds)
		{
		  ExecutionEntity processInstance = commandContext.ExecutionManager.findExecutionById(processInstanceId);

		  ensureProcessInstanceExist(processInstanceId, processInstance);
		  ensureSameProcessDefinition(processInstance, processDefinition.Id);

		  ProcessInstanceModificationBuilderImpl builder = createProcessInstanceModificationBuilder(processInstanceId, commandContext);
		  builder.execute(false, skipCustomListeners, skipIoMappings);
		}

		return null;
	  }

	  protected internal virtual void ensureSameProcessDefinition(ExecutionEntity processInstance, string processDefinitionId)
	  {
		if (!processDefinitionId.Equals(processInstance.ProcessDefinitionId))
		{
		  throw LOG.processDefinitionOfInstanceDoesNotMatchModification(processInstance, processDefinitionId);
		}
	  }

	  protected internal virtual void ensureProcessInstanceExist(string processInstanceId, ExecutionEntity processInstance)
	  {
		if (processInstance == null)
		{
		  throw LOG.processInstanceDoesNotExist(processInstanceId);
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected org.camunda.bpm.engine.impl.ProcessInstanceModificationBuilderImpl createProcessInstanceModificationBuilder(final String processInstanceId, final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  protected internal virtual ProcessInstanceModificationBuilderImpl createProcessInstanceModificationBuilder(string processInstanceId, CommandContext commandContext)
	  {
		ProcessInstanceModificationBuilderImpl processInstanceModificationBuilder = new ProcessInstanceModificationBuilderImpl(commandContext, processInstanceId);
		IList<AbstractProcessInstanceModificationCommand> operations = processInstanceModificationBuilder.ModificationOperations;

		ActivityInstance activityInstanceTree = null;

		foreach (AbstractProcessInstanceModificationCommand instruction in builder.Instructions)
		{

		  instruction.ProcessInstanceId = processInstanceId;

		  if (!(instruction is ActivityCancellationCmd) || !((ActivityCancellationCmd)instruction).CancelCurrentActiveActivityInstances)
		  {
			operations.Add(instruction);
		  }
		  else
		  {

			if (activityInstanceTree == null)
			{
			  activityInstanceTree = commandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this, processInstanceId, commandContext));
			}

			ActivityCancellationCmd cancellationInstruction = (ActivityCancellationCmd) instruction;
			IList<AbstractInstanceCancellationCmd> cmds = cancellationInstruction.createActivityInstanceCancellations(activityInstanceTree, commandContext);
			((IList<AbstractProcessInstanceModificationCommand>)operations).AddRange(cmds);
		  }

		}

		return processInstanceModificationBuilder;
	  }

	  private class CallableAnonymousInnerClass : Callable<ActivityInstance>
	  {
		  private readonly ProcessInstanceModificationCmd outerInstance;

		  private string processInstanceId;
		  private CommandContext commandContext;

		  public CallableAnonymousInnerClass(ProcessInstanceModificationCmd outerInstance, string processInstanceId, CommandContext commandContext)
		  {
			  this.outerInstance = outerInstance;
			  this.processInstanceId = processInstanceId;
			  this.commandContext = commandContext;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public org.camunda.bpm.engine.runtime.ActivityInstance call() throws Exception
		  public override ActivityInstance call()
		  {
			return (new GetActivityInstanceCmd(processInstanceId)).execute(commandContext);
		  }
	  }

	}

}