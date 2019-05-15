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
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;
	using Incident = org.camunda.bpm.engine.runtime.Incident;

	/// 
	/// <summary>
	/// @author Anna Pazola
	/// 
	/// </summary>
	public class ResolveIncidentCmd : Command<Void>
	{

	  protected internal string incidentId;

	  public ResolveIncidentCmd(string incidentId)
	  {
		EnsureUtil.ensureNotNull(typeof(BadUserRequestException), "", "incidentId", incidentId);
		this.incidentId = incidentId;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.runtime.Incident incident = commandContext.getIncidentManager().findIncidentById(incidentId);
		Incident incident = commandContext.IncidentManager.findIncidentById(incidentId);

		EnsureUtil.ensureNotNull(typeof(NotFoundException), "Cannot find an incident with id '" + incidentId + "'", "incident", incident);

		if (incident.IncidentType.Equals("failedJob") || incident.IncidentType.Equals("failedExternalTask"))
		{
		  throw new BadUserRequestException("Cannot resolve an incident of type " + incident.IncidentType);
		}

		EnsureUtil.ensureNotNull(typeof(BadUserRequestException), "", "executionId", incident.ExecutionId);
		ExecutionEntity execution = commandContext.ExecutionManager.findExecutionById(incident.ExecutionId);

		EnsureUtil.ensureNotNull(typeof(BadUserRequestException), "Cannot find an execution for an incident with id '" + incidentId + "'", "execution", execution);

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkUpdateProcessInstance(execution);
		}

		commandContext.OperationLogManager.logProcessInstanceOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_RESOLVE, execution.ProcessInstanceId, execution.ProcessDefinitionId, null, Collections.singletonList(new PropertyChange("incidentId", null, incidentId)));

		execution.resolveIncident(incidentId);
		return null;
	  }
	}

}