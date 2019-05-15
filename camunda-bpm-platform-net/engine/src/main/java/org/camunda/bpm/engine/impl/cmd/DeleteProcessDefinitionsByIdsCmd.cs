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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotEmpty;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using BpmnDeployer = org.camunda.bpm.engine.impl.bpmn.deployer.BpmnDeployer;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using Deployer = org.camunda.bpm.engine.impl.persistence.deploy.Deployer;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ProcessDefinitionManager = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionManager;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using UserOperationLogManager = org.camunda.bpm.engine.impl.persistence.entity.UserOperationLogManager;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;

	/// <summary>
	/// Command to delete process definitions by ids.
	/// 
	/// @author Tassilo Weidner
	/// </summary>
	[Serializable]
	public class DeleteProcessDefinitionsByIdsCmd : Command<Void>
	{

	  private const long serialVersionUID = 1L;

	  protected internal readonly ISet<string> processDefinitionIds;
	  protected internal bool cascadeToHistory;
	  protected internal bool cascadeToInstances;
	  protected internal bool skipCustomListeners;
	  protected internal bool writeUserOperationLog;
	  protected internal bool skipIoMappings;

	  public DeleteProcessDefinitionsByIdsCmd(IList<string> processDefinitionIds, bool cascade, bool skipCustomListeners, bool skipIoMappings) : this(processDefinitionIds, cascade, cascade, skipCustomListeners, skipIoMappings, true)
	  {
	  }

	  public DeleteProcessDefinitionsByIdsCmd(IList<string> processDefinitionIds, bool cascadeToHistory, bool cascadeToInstances, bool skipCustomListeners, bool writeUserOperationLog) : this(processDefinitionIds, cascadeToHistory, cascadeToInstances, skipCustomListeners, false, writeUserOperationLog)
	  {
	  }

	  public DeleteProcessDefinitionsByIdsCmd(IList<string> processDefinitionIds, bool cascadeToHistory, bool cascadeToInstances, bool skipCustomListeners, bool skipIoMappings, bool writeUserOperationLog)
	  {
		this.processDefinitionIds = new HashSet<string>(processDefinitionIds);
		this.cascadeToHistory = cascadeToHistory;
		this.cascadeToInstances = cascadeToInstances;
		this.skipCustomListeners = skipCustomListeners;
		this.skipIoMappings = skipIoMappings;
		this.writeUserOperationLog = writeUserOperationLog;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		ensureNotNull("processDefinitionIds", processDefinitionIds);

		IList<ProcessDefinition> processDefinitions;
		if (processDefinitionIds.Count == 1)
		{
		  ProcessDefinition processDefinition = getSingleProcessDefinition(commandContext);
		  processDefinitions = new List<ProcessDefinition>();
		  processDefinitions.Add(processDefinition);
		}
		else
		{
		  ProcessDefinitionManager processDefinitionManager = commandContext.ProcessDefinitionManager;
		  processDefinitions = processDefinitionManager.findDefinitionsByIds(processDefinitionIds);
		  ensureNotEmpty(typeof(NotFoundException), "No process definition found", "processDefinitions", processDefinitions);
		}

		ISet<ProcessDefinitionGroup> groups = groupByKeyAndTenant(processDefinitions);

		foreach (ProcessDefinitionGroup group in groups)
		{
		  checkAuthorization(group);
		}

		foreach (ProcessDefinitionGroup group in groups)
		{
		  deleteProcessDefinitions(group);
		}

		return null;
	  }

	  protected internal virtual ProcessDefinition getSingleProcessDefinition(CommandContext commandContext)
	  {
		string processDefinitionId = processDefinitionIds.GetEnumerator().next();
		ensureNotNull("processDefinitionId", processDefinitionId);
		ProcessDefinition processDefinition = commandContext.ProcessDefinitionManager.findLatestProcessDefinitionById(processDefinitionId);
		ensureNotNull(typeof(NotFoundException), "No process definition found with id '" + processDefinitionId + "'", "processDefinition", processDefinition);

		return processDefinition;
	  }

	  protected internal virtual ISet<ProcessDefinitionGroup> groupByKeyAndTenant(IList<ProcessDefinition> processDefinitions)
	  {
		ISet<ProcessDefinitionGroup> groups = new HashSet<ProcessDefinitionGroup>();
		IDictionary<ProcessDefinitionGroup, IList<ProcessDefinitionEntity>> map = new Dictionary<ProcessDefinitionGroup, IList<ProcessDefinitionEntity>>();

		foreach (ProcessDefinition current in processDefinitions)
		{
		  ProcessDefinitionEntity processDefinition = (ProcessDefinitionEntity) current;

		  ProcessDefinitionGroup group = new ProcessDefinitionGroup();
		  group.key = processDefinition.Key;
		  group.tenant = processDefinition.TenantId;

		  IList<ProcessDefinitionEntity> definitions = group.processDefinitions;
		  if (map.ContainsKey(group))
		  {
			definitions = map[group];
		  }
		  else
		  {
			groups.Add(group);
			map[group] = definitions;
		  }

		  definitions.Add(processDefinition);
		}

		return groups;
	  }

	  protected internal virtual ProcessDefinitionEntity findNewLatestProcessDefinition(ProcessDefinitionGroup group)
	  {
		ProcessDefinitionEntity newLatestProcessDefinition = null;

		IList<ProcessDefinitionEntity> processDefinitions = group.processDefinitions;
		ProcessDefinitionEntity firstProcessDefinition = processDefinitions[0];

		if (isLatestProcessDefinition(firstProcessDefinition))
		{
		  foreach (ProcessDefinitionEntity processDefinition in processDefinitions)
		  {
			string previousProcessDefinitionId = processDefinition.PreviousProcessDefinitionId;
			if (!string.ReferenceEquals(previousProcessDefinitionId, null) && !this.processDefinitionIds.Contains(previousProcessDefinitionId))
			{
			  CommandContext commandContext = Context.CommandContext;
			  ProcessDefinitionManager processDefinitionManager = commandContext.ProcessDefinitionManager;
			  newLatestProcessDefinition = processDefinitionManager.findLatestDefinitionById(previousProcessDefinitionId);
			  break;
			}
		  }
		}

		return newLatestProcessDefinition;
	  }

	  protected internal virtual bool isLatestProcessDefinition(ProcessDefinitionEntity processDefinition)
	  {
		ProcessDefinitionManager processDefinitionManager = Context.CommandContext.ProcessDefinitionManager;
		string key = processDefinition.Key;
		string tenantId = processDefinition.TenantId;
		ProcessDefinitionEntity latestProcessDefinition = processDefinitionManager.findLatestDefinitionByKeyAndTenantId(key, tenantId);
		return processDefinition.Id.Equals(latestProcessDefinition.Id);
	  }

	  protected internal virtual void checkAuthorization(ProcessDefinitionGroup group)
	  {
		IList<CommandChecker> commandCheckers = Context.CommandContext.ProcessEngineConfiguration.CommandCheckers;
		IList<ProcessDefinitionEntity> processDefinitions = group.processDefinitions;
		foreach (ProcessDefinitionEntity processDefinition in processDefinitions)
		{
		  foreach (CommandChecker commandChecker in commandCheckers)
		  {
			commandChecker.checkDeleteProcessDefinitionById(processDefinition.Id);
		  }
		}
	  }

	  protected internal virtual void deleteProcessDefinitions(ProcessDefinitionGroup group)
	  {
		ProcessDefinitionEntity newLatestProcessDefinition = findNewLatestProcessDefinition(group);

		CommandContext commandContext = Context.CommandContext;
		UserOperationLogManager userOperationLogManager = commandContext.OperationLogManager;
		ProcessDefinitionManager definitionManager = commandContext.ProcessDefinitionManager;

		IList<ProcessDefinitionEntity> processDefinitions = group.processDefinitions;
		foreach (ProcessDefinitionEntity processDefinition in processDefinitions)
		{
		  string processDefinitionId = processDefinition.Id;

		  if (writeUserOperationLog)
		  {
			userOperationLogManager.logProcessDefinitionOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE, processDefinitionId, processDefinition.Key, new PropertyChange("cascade", false, cascadeToHistory));
		  }

		  definitionManager.deleteProcessDefinition(processDefinition, processDefinitionId, cascadeToHistory, cascadeToInstances, skipCustomListeners, skipIoMappings);
		}

		if (newLatestProcessDefinition != null)
		{
		  ProcessEngineConfigurationImpl configuration = Context.ProcessEngineConfiguration;
		  DeploymentCache deploymentCache = configuration.DeploymentCache;
		  newLatestProcessDefinition = deploymentCache.resolveProcessDefinition(newLatestProcessDefinition);

		  IList<Deployer> deployers = configuration.Deployers;
		  foreach (Deployer deployer in deployers)
		  {
			if (deployer is BpmnDeployer)
			{
			  ((BpmnDeployer) deployer).addEventSubscriptions(newLatestProcessDefinition);
			}
		  }
		}
	  }

	  private class ProcessDefinitionGroup
	  {

		internal string key;
		internal string tenant;
		internal IList<ProcessDefinitionEntity> processDefinitions = new List<ProcessDefinitionEntity>();

		public override int GetHashCode()
		{
		  const int prime = 31;
		  int result = 1;
		  result = prime * result + ((string.ReferenceEquals(key, null)) ? 0 : key.GetHashCode());
		  result = prime * result + ((string.ReferenceEquals(tenant, null)) ? 0 : tenant.GetHashCode());
		  return result;
		}

		public override bool Equals(object obj)
		{
		  if (this == obj)
		  {
			return true;
		  }
		  if (obj == null)
		  {
			return false;
		  }
		  if (this.GetType() != obj.GetType())
		  {
			return false;
		  }
		  ProcessDefinitionGroup other = (ProcessDefinitionGroup) obj;
		  if (string.ReferenceEquals(key, null))
		  {
			if (!string.ReferenceEquals(other.key, null))
			{
			  return false;
			}
		  }
		  else if (!key.Equals(other.key))
		  {
			return false;
		  }
		  if (string.ReferenceEquals(tenant, null))
		  {
			if (!string.ReferenceEquals(other.tenant, null))
			{
			  return false;
			}
		  }
		  else if (!tenant.Equals(other.tenant))
		  {
			return false;
		  }
		  return true;
		}

	  }

	}

}