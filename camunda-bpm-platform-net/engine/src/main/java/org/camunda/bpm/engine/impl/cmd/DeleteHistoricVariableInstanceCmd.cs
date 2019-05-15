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
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using HistoricVariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricVariableInstanceEntity;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using ResourceDefinitionEntity = org.camunda.bpm.engine.impl.repository.ResourceDefinitionEntity;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotEmpty;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	/// <summary>
	/// @author Tobias Metzke
	/// 
	/// </summary>
	[Serializable]
	public class DeleteHistoricVariableInstanceCmd : Command<Void>
	{

	  private const long serialVersionUID = 1L;
	  private string variableInstanceId;

	  public DeleteHistoricVariableInstanceCmd(string variableInstanceId)
	  {
		this.variableInstanceId = variableInstanceId;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		ensureNotEmpty(typeof(BadUserRequestException),"variableInstanceId", variableInstanceId);

		HistoricVariableInstanceEntity variable = commandContext.HistoricVariableInstanceManager.findHistoricVariableInstanceByVariableInstanceId(variableInstanceId);
		ensureNotNull(typeof(NotFoundException), "No historic variable instance found with id: " + variableInstanceId, "variable", variable);

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkDeleteHistoricVariableInstance(variable);
		}

		commandContext.HistoricDetailManager.deleteHistoricDetailsByVariableInstanceId(variableInstanceId);

		commandContext.HistoricVariableInstanceManager.deleteHistoricVariableInstanceByVariableInstanceId(variableInstanceId);

		// create user operation log
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.bpm.engine.impl.repository.ResourceDefinitionEntity<?> definition = null;
		ResourceDefinitionEntity<object> definition = null;
		try
		{
		  if (!string.ReferenceEquals(variable.ProcessDefinitionId, null))
		  {
			definition = commandContext.ProcessEngineConfiguration.DeploymentCache.findDeployedProcessDefinitionById(variable.ProcessDefinitionId);
		  }
		  else if (!string.ReferenceEquals(variable.CaseDefinitionId, null))
		  {
			definition = commandContext.ProcessEngineConfiguration.DeploymentCache.findDeployedCaseDefinitionById(variable.CaseDefinitionId);
		  }
		}
		catch (NullValueException)
		{
		  // definition has been deleted already
		}
		commandContext.OperationLogManager.logHistoricVariableOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY, variable, definition, new PropertyChange("name", null, variable.Name));

		return null;
	  }
	}

}