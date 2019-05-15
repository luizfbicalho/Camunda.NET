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
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;

	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ExecutionVariableSnapshotObserver = org.camunda.bpm.engine.impl.persistence.entity.ExecutionVariableSnapshotObserver;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ProcessInstanceWithVariablesImpl = org.camunda.bpm.engine.impl.persistence.entity.ProcessInstanceWithVariablesImpl;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using ProcessInstanceWithVariables = org.camunda.bpm.engine.runtime.ProcessInstanceWithVariables;

	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class StartProcessInstanceCmd : Command<ProcessInstanceWithVariables>
	{

	  private const long serialVersionUID = 1L;

	  protected internal readonly ProcessInstantiationBuilderImpl instantiationBuilder;

	  public StartProcessInstanceCmd(ProcessInstantiationBuilderImpl instantiationBuilder)
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

		// Start the process instance
		ExecutionEntity processInstance = processDefinition.createProcessInstance(instantiationBuilder.BusinessKey, instantiationBuilder.CaseInstanceId);

		if (!string.ReferenceEquals(instantiationBuilder.TenantId, null))
		{
		  processInstance.TenantId = instantiationBuilder.TenantId;
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.ExecutionVariableSnapshotObserver variablesListener = new org.camunda.bpm.engine.impl.persistence.entity.ExecutionVariableSnapshotObserver(processInstance);
		ExecutionVariableSnapshotObserver variablesListener = new ExecutionVariableSnapshotObserver(processInstance);

		processInstance.start(instantiationBuilder.Variables);

		commandContext.OperationLogManager.logProcessInstanceOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE, processInstance.Id, processInstance.ProcessDefinitionId, processInstance.getProcessDefinition().Key, Collections.singletonList(PropertyChange.EMPTY_CHANGE));

		return new ProcessInstanceWithVariablesImpl(processInstance, variablesListener.Variables);
	  }

	}

}