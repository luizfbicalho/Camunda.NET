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
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;

	public abstract class AbstractUpdateProcessInstancesSuspendStateCmd<T> : Command<T>
	{
		public abstract T execute(CommandContext commandContext);

	  protected internal UpdateProcessInstancesSuspensionStateBuilderImpl builder;
	  protected internal CommandExecutor commandExecutor;
	  protected internal bool suspending;

	  public AbstractUpdateProcessInstancesSuspendStateCmd(CommandExecutor commandExecutor, UpdateProcessInstancesSuspensionStateBuilderImpl builder, bool suspending)
	  {
		this.commandExecutor = commandExecutor;
		this.builder = builder;
		this.suspending = suspending;
	  }

	  protected internal virtual ICollection<string> collectProcessInstanceIds()
	  {
		HashSet<string> allProcessInstanceIds = new HashSet<string>();

		IList<string> processInstanceIds = builder.ProcessInstanceIds;
		if (processInstanceIds != null)
		{
		  allProcessInstanceIds.addAll(processInstanceIds);
		}

		ProcessInstanceQueryImpl processInstanceQuery = (ProcessInstanceQueryImpl) builder.ProcessInstanceQuery;
		if (processInstanceQuery != null)
		{
		  allProcessInstanceIds.addAll(processInstanceQuery.listIds());
		}

		HistoricProcessInstanceQueryImpl historicProcessInstanceQuery = (HistoricProcessInstanceQueryImpl) builder.HistoricProcessInstanceQuery;
		if (historicProcessInstanceQuery != null)
		{
		  allProcessInstanceIds.addAll(historicProcessInstanceQuery.listIds());
		}

		return allProcessInstanceIds;
	  }

	  protected internal virtual void writeUserOperationLog(CommandContext commandContext, int numInstances, bool async)
	  {

		IList<PropertyChange> propertyChanges = new List<PropertyChange>();
		propertyChanges.Add(new PropertyChange("nrOfInstances", null, numInstances));
		propertyChanges.Add(new PropertyChange("async", null, async));

		string operationType;
		if (suspending)
		{
		  operationType = org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SUSPEND_JOB;

		}
		else
		{
		  operationType = org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_ACTIVATE_JOB;
		}
		commandContext.OperationLogManager.logProcessInstanceOperation(operationType, null, null, null, propertyChanges);
	  }
	}

}