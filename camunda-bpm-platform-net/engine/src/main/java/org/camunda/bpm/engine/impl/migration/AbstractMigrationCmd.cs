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
namespace org.camunda.bpm.engine.impl.migration
{

	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using CompositePermissionCheck = org.camunda.bpm.engine.impl.db.CompositePermissionCheck;
	using PermissionCheckBuilder = org.camunda.bpm.engine.impl.db.PermissionCheckBuilder;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public abstract class AbstractMigrationCmd<T> : Command<T>
	{
		public abstract T execute(CommandContext commandContext);

	  protected internal MigrationPlanExecutionBuilderImpl executionBuilder;

	  public AbstractMigrationCmd(MigrationPlanExecutionBuilderImpl executionBuilder)
	  {
		this.executionBuilder = executionBuilder;
	  }

	  protected internal virtual void checkAuthorizations(CommandContext commandContext, ProcessDefinitionEntity sourceDefinition, ProcessDefinitionEntity targetDefinition, ICollection<string> processInstanceIds)
	  {

		CompositePermissionCheck migrateInstanceCheck = (new PermissionCheckBuilder()).conjunctive().atomicCheckForResourceId(Resources.PROCESS_DEFINITION, sourceDefinition.Key, Permissions.MIGRATE_INSTANCE).atomicCheckForResourceId(Resources.PROCESS_DEFINITION, targetDefinition.Key, Permissions.MIGRATE_INSTANCE).build();

		commandContext.AuthorizationManager.checkAuthorization(migrateInstanceCheck);
	  }

	  protected internal virtual ICollection<string> collectProcessInstanceIds(CommandContext commandContext)
	  {

		ISet<string> collectedProcessInstanceIds = new HashSet<string>();

		IList<string> processInstanceIds = executionBuilder.ProcessInstanceIds;
		if (processInstanceIds != null)
		{
		  collectedProcessInstanceIds.addAll(processInstanceIds);
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.ProcessInstanceQueryImpl processInstanceQuery = (org.camunda.bpm.engine.impl.ProcessInstanceQueryImpl) executionBuilder.getProcessInstanceQuery();
		ProcessInstanceQueryImpl processInstanceQuery = (ProcessInstanceQueryImpl) executionBuilder.ProcessInstanceQuery;
		if (processInstanceQuery != null)
		{
		  collectedProcessInstanceIds.addAll(processInstanceQuery.listIds());
		}

		return collectedProcessInstanceIds;
	  }

	  protected internal virtual void writeUserOperationLog(CommandContext commandContext, ProcessDefinitionEntity sourceProcessDefinition, ProcessDefinitionEntity targetProcessDefinition, int numInstances, bool async)
	  {

		IList<PropertyChange> propertyChanges = new List<PropertyChange>();
		propertyChanges.Add(new PropertyChange("processDefinitionId", sourceProcessDefinition.Id, targetProcessDefinition.Id));
		propertyChanges.Add(new PropertyChange("nrOfInstances", null, numInstances));
		propertyChanges.Add(new PropertyChange("async", null, async));

		commandContext.OperationLogManager.logProcessInstanceOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_MIGRATE, null, sourceProcessDefinition.Id, sourceProcessDefinition.Key, propertyChanges);
	  }

	  protected internal virtual ProcessDefinitionEntity resolveSourceProcessDefinition(CommandContext commandContext)
	  {

		string sourceProcessDefinitionId = executionBuilder.MigrationPlan.SourceProcessDefinitionId;

		ProcessDefinitionEntity sourceProcessDefinition = getProcessDefinition(commandContext, sourceProcessDefinitionId);
		EnsureUtil.ensureNotNull("sourceProcessDefinition", sourceProcessDefinition);

		return sourceProcessDefinition;
	  }

	  protected internal virtual ProcessDefinitionEntity resolveTargetProcessDefinition(CommandContext commandContext)
	  {
		string targetProcessDefinitionId = executionBuilder.MigrationPlan.TargetProcessDefinitionId;

		ProcessDefinitionEntity sourceProcessDefinition = getProcessDefinition(commandContext, targetProcessDefinitionId);
		EnsureUtil.ensureNotNull("sourceProcessDefinition", sourceProcessDefinition);

		return sourceProcessDefinition;
	  }

	  protected internal virtual ProcessDefinitionEntity getProcessDefinition(CommandContext commandContext, string processDefinitionId)
	  {

		return commandContext.ProcessEngineConfiguration.DeploymentCache.findDeployedProcessDefinitionById(processDefinitionId);
	  }
	}

}