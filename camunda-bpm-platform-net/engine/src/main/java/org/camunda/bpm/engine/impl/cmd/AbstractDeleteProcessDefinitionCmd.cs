using System;
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
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ProcessDefinitionManager = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionManager;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using UserOperationLogManager = org.camunda.bpm.engine.impl.persistence.entity.UserOperationLogManager;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	[Serializable]
	public abstract class AbstractDeleteProcessDefinitionCmd : Command<Void>
	{
		public abstract T execute(CommandContext commandContext);

	  protected internal bool cascade;
	  protected internal bool skipCustomListeners;
	  protected internal bool skipIoMappings;

	  protected internal virtual void deleteProcessDefinitionCmd(CommandContext commandContext, string processDefinitionId, bool cascade, bool skipCustomListeners, bool skipIoMappings)
	  {
		ensureNotNull("processDefinitionId", processDefinitionId);

		ProcessDefinition processDefinition = commandContext.ProcessDefinitionManager.findLatestProcessDefinitionById(processDefinitionId);
		ensureNotNull(typeof(NotFoundException), "No process definition found with id '" + processDefinitionId + "'", "processDefinition", processDefinition);

		IList<CommandChecker> commandCheckers = commandContext.ProcessEngineConfiguration.CommandCheckers;
		foreach (CommandChecker checker in commandCheckers)
		{
		  checker.checkDeleteProcessDefinitionById(processDefinitionId);
		}

		UserOperationLogManager userOperationLogManager = commandContext.OperationLogManager;
		userOperationLogManager.logProcessDefinitionOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE, processDefinitionId, processDefinition.Key, new PropertyChange("cascade", false, cascade));

		ProcessDefinitionManager definitionManager = commandContext.ProcessDefinitionManager;
		definitionManager.deleteProcessDefinition(processDefinition, processDefinitionId, cascade, cascade, skipCustomListeners, skipIoMappings);
	  }

	}

}