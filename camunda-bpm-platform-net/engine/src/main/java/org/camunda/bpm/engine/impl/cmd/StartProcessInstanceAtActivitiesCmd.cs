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
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotEmpty;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using CoreModelElement = org.camunda.bpm.engine.impl.core.model.CoreModelElement;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ExecutionVariableSnapshotObserver = org.camunda.bpm.engine.impl.persistence.entity.ExecutionVariableSnapshotObserver;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ProcessInstanceWithVariablesImpl = org.camunda.bpm.engine.impl.persistence.entity.ProcessInstanceWithVariablesImpl;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ProcessDefinitionImpl = org.camunda.bpm.engine.impl.pvm.process.ProcessDefinitionImpl;
	using TransitionImpl = org.camunda.bpm.engine.impl.pvm.process.TransitionImpl;
	using ProcessInstanceWithVariables = org.camunda.bpm.engine.runtime.ProcessInstanceWithVariables;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class StartProcessInstanceAtActivitiesCmd : Command<ProcessInstanceWithVariables>
	{

	  private static readonly CommandLogger LOG = ProcessEngineLogger.CMD_LOGGER;

	  protected internal ProcessInstantiationBuilderImpl instantiationBuilder;

	  public StartProcessInstanceAtActivitiesCmd(ProcessInstantiationBuilderImpl instantiationBuilder)
	  {
		this.instantiationBuilder = instantiationBuilder;
	  }

	  public virtual ProcessInstanceWithVariables execute(CommandContext commandContext)
	  {

		ProcessDefinitionEntity processDefinition = (new GetDeployedProcessDefinitionCmd(instantiationBuilder, false)).execute(commandContext);

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkCreateProcessInstance(processDefinition);
		}

		ProcessInstanceModificationBuilderImpl modificationBuilder = instantiationBuilder.ModificationBuilder;
		ensureNotEmpty("At least one instantiation instruction required (e.g. by invoking startBefore(..), startAfter(..) or startTransition(..))", "instructions", modificationBuilder.ModificationOperations);

		// instantiate the process
		ActivityImpl initialActivity = determineFirstActivity(processDefinition, modificationBuilder);

		ExecutionEntity processInstance = processDefinition.createProcessInstance(instantiationBuilder.BusinessKey, instantiationBuilder.CaseInstanceId, initialActivity);

		if (!string.ReferenceEquals(instantiationBuilder.TenantId, null))
		{
		  processInstance.TenantId = instantiationBuilder.TenantId;
		}

		processInstance.SkipCustomListeners = modificationBuilder.SkipCustomListeners;
		VariableMap variables = modificationBuilder.ProcessVariables;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.ExecutionVariableSnapshotObserver variablesListener = new org.camunda.bpm.engine.impl.persistence.entity.ExecutionVariableSnapshotObserver(processInstance);
		ExecutionVariableSnapshotObserver variablesListener = new ExecutionVariableSnapshotObserver(processInstance);

		processInstance.startWithoutExecuting(variables);

		// prevent ending of the process instance between instructions
		processInstance.PreserveScope = true;

		// apply modifications
		IList<AbstractProcessInstanceModificationCommand> instructions = modificationBuilder.ModificationOperations;

		for (int i = 0; i < instructions.Count; i++)
		{
		  AbstractProcessInstanceModificationCommand instruction = instructions[i];
		  LOG.debugStartingInstruction(processInstance.Id, i, instruction.describe());

		  instruction.ProcessInstanceId = processInstance.Id;
		  instruction.SkipCustomListeners = modificationBuilder.SkipCustomListeners;
		  instruction.SkipIoMappings = modificationBuilder.SkipIoMappings;
		  instruction.execute(commandContext);
		}

		if (!processInstance.hasChildren() && processInstance.Ended)
		{
		  // process instance has ended regularly but this has not been propagated yet
		  // due to preserveScope setting
		  processInstance.propagateEnd();
		}

		commandContext.OperationLogManager.logProcessInstanceOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE, processInstance.Id, processInstance.ProcessDefinitionId, processInstance.getProcessDefinition().Key, Collections.singletonList(PropertyChange.EMPTY_CHANGE));

		return new ProcessInstanceWithVariablesImpl(processInstance, variablesListener.Variables);
	  }


	  /// <summary>
	  /// get the activity that is started by the first instruction, if exists;
	  /// return null if the first instruction is a start-transition instruction
	  /// </summary>
	  protected internal virtual ActivityImpl determineFirstActivity(ProcessDefinitionImpl processDefinition, ProcessInstanceModificationBuilderImpl modificationBuilder)
	  {
		AbstractProcessInstanceModificationCommand firstInstruction = modificationBuilder.ModificationOperations[0];

		if (firstInstruction is AbstractInstantiationCmd)
		{
		  AbstractInstantiationCmd instantiationInstruction = (AbstractInstantiationCmd) firstInstruction;
		  CoreModelElement targetElement = instantiationInstruction.getTargetElement(processDefinition);

		  ensureNotNull(typeof(NotValidException), "Element '" + instantiationInstruction.TargetElementId + "' does not exist in process " + processDefinition.Id, "targetElement", targetElement);

		  if (targetElement is ActivityImpl)
		  {
			return (ActivityImpl) targetElement;
		  }
		  else if (targetElement is TransitionImpl)
		  {
			return (ActivityImpl)((TransitionImpl) targetElement).Destination;
		  }

		}

		return null;
	  }

	}

}