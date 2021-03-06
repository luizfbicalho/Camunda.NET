﻿using System.Collections.Generic;

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
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;
	using Incident = org.camunda.bpm.engine.runtime.Incident;

	/// 
	/// <summary>
	/// @author Anna Pazola
	/// 
	/// </summary>
	public class CreateIncidentCmd : Command<Incident>
	{

	  protected internal string incidentType;
	  protected internal string executionId;
	  protected internal string configuration;
	  protected internal string message;

	  public CreateIncidentCmd(string incidentType, string executionId, string configuration, string message)
	  {
		this.incidentType = incidentType;
		this.executionId = executionId;
		this.configuration = configuration;
		this.message = message;
	  }

	  public virtual Incident execute(CommandContext commandContext)
	  {
		EnsureUtil.ensureNotNull(typeof(BadUserRequestException), "Execution id cannot be null", "executionId", executionId);
		EnsureUtil.ensureNotNull(typeof(BadUserRequestException), "incidentType", incidentType);

		ExecutionEntity execution = commandContext.ExecutionManager.findExecutionById(executionId);
		EnsureUtil.ensureNotNull(typeof(BadUserRequestException), "Cannot find an execution with executionId '" + executionId + "'", "execution", execution);
		EnsureUtil.ensureNotNull(typeof(BadUserRequestException), "Execution must be related to an activity", "activity", execution.getActivity());

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkUpdateProcessInstance(execution);
		}

		IList<PropertyChange> propertyChanges = new List<PropertyChange>();
		propertyChanges.Add(new PropertyChange("incidentType", null, incidentType));
		propertyChanges.Add(new PropertyChange("configuration", null, configuration));

		commandContext.OperationLogManager.logProcessInstanceOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE_INCIDENT, execution.ProcessInstanceId, execution.ProcessDefinitionId, null, propertyChanges);

		return execution.createIncident(incidentType, configuration, message);
	  }
	}

}