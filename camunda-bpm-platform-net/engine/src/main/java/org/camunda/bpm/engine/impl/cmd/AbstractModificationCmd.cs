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

	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;

	public abstract class AbstractModificationCmd<T> : Command<T>
	{
		public abstract T execute(CommandContext commandContext);

	  protected internal ModificationBuilderImpl builder;

	  public AbstractModificationCmd(ModificationBuilderImpl modificationBuilderImpl)
	  {
		this.builder = modificationBuilderImpl;
	  }

	  protected internal virtual ICollection<string> collectProcessInstanceIds(CommandContext commandContext)
	  {

		ISet<string> collectedProcessInstanceIds = new HashSet<string>();

		IList<string> processInstanceIds = builder.ProcessInstanceIds;
		if (processInstanceIds != null)
		{
		  collectedProcessInstanceIds.addAll(processInstanceIds);
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.ProcessInstanceQueryImpl processInstanceQuery = (org.camunda.bpm.engine.impl.ProcessInstanceQueryImpl) builder.getProcessInstanceQuery();
		ProcessInstanceQueryImpl processInstanceQuery = (ProcessInstanceQueryImpl) builder.ProcessInstanceQuery;
		if (processInstanceQuery != null)
		{
		  collectedProcessInstanceIds.addAll(processInstanceQuery.listIds());
		}

		return collectedProcessInstanceIds;
	  }

	  protected internal virtual void writeUserOperationLog(CommandContext commandContext, ProcessDefinition processDefinition, int numInstances, bool async)
	  {

		IList<PropertyChange> propertyChanges = new List<PropertyChange>();
		propertyChanges.Add(new PropertyChange("nrOfInstances", null, numInstances));
		propertyChanges.Add(new PropertyChange("async", null, async));

		commandContext.OperationLogManager.logProcessInstanceOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_MODIFY_PROCESS_INSTANCE, null, processDefinition.Id, processDefinition.Key, propertyChanges);
	  }

	  protected internal virtual ProcessDefinitionEntity getProcessDefinition(CommandContext commandContext, string processDefinitionId)
	  {

		return commandContext.ProcessEngineConfiguration.DeploymentCache.findDeployedProcessDefinitionById(processDefinitionId);
	  }

	}

}