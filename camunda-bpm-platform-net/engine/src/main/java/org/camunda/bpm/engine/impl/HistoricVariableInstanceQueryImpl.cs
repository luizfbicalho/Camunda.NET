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
namespace org.camunda.bpm.engine.impl
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using HistoricVariableInstanceQuery = org.camunda.bpm.engine.history.HistoricVariableInstanceQuery;
	using CommandLogger = org.camunda.bpm.engine.impl.cmd.CommandLogger;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using HistoricVariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricVariableInstanceEntity;
	using AbstractTypedValueSerializer = org.camunda.bpm.engine.impl.variable.serializer.AbstractTypedValueSerializer;
	using VariableSerializers = org.camunda.bpm.engine.impl.variable.serializer.VariableSerializers;

	/// <summary>
	/// @author Christian Lipphardt (camunda)
	/// </summary>
	[Serializable]
	public class HistoricVariableInstanceQueryImpl : AbstractQuery<HistoricVariableInstanceQuery, HistoricVariableInstance>, HistoricVariableInstanceQuery
	{

	  private static readonly CommandLogger LOG = ProcessEngineLogger.CMD_LOGGER;

	  private const long serialVersionUID = 1L;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string variableId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionKey_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string variableName_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string variableNameLike_Conflict;
	  protected internal QueryVariableValue queryVariableValue;
	  protected internal string[] variableTypes;
	  protected internal string[] taskIds;
	  protected internal string[] executionIds;
	  protected internal string[] caseExecutionIds;
	  protected internal string[] caseActivityIds;
	  protected internal string[] activityInstanceIds;
	  protected internal string[] tenantIds;
	  protected internal string[] processInstanceIds;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool includeDeleted_Conflict = false;

	  protected internal bool isByteArrayFetchingEnabled = true;
	  protected internal bool isCustomObjectDeserializationEnabled = true;

	  public HistoricVariableInstanceQueryImpl()
	  {
	  }

	  public HistoricVariableInstanceQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual HistoricVariableInstanceQuery variableId(string id)
	  {
		ensureNotNull("variableId", id);
		this.variableId_Conflict = id;
		return this;
	  }

	  public virtual HistoricVariableInstanceQueryImpl processInstanceId(string processInstanceId)
	  {
		ensureNotNull("processInstanceId", processInstanceId);
		this.processInstanceId_Conflict = processInstanceId;
		return this;
	  }

	  public virtual HistoricVariableInstanceQuery processDefinitionId(string processDefinitionId)
	  {
		ensureNotNull("processDefinitionId", processDefinitionId);
		this.processDefinitionId_Conflict = processDefinitionId;
		return this;
	  }

	  public virtual HistoricVariableInstanceQuery processDefinitionKey(string processDefinitionKey)
	  {
		this.processDefinitionKey_Conflict = processDefinitionKey;
		return this;
	  }

	  public virtual HistoricVariableInstanceQuery caseInstanceId(string caseInstanceId)
	  {
		ensureNotNull("caseInstanceId", caseInstanceId);
		this.caseInstanceId_Conflict = caseInstanceId;
		return this;
	  }

	  public virtual HistoricVariableInstanceQuery variableTypeIn(params string[] variableTypes)
	  {
		ensureNotNull("Variable types", (object[]) variableTypes);
		this.variableTypes = lowerCase(variableTypes);
		return this;
	  }

	  private string[] lowerCase(params string[] variableTypes)
	  {
		for (int i = 0; i < variableTypes.Length; i++)
		{
		  variableTypes[i] = variableTypes[i].ToLower();
		}
		return variableTypes;
	  }

	  /// <summary>
	  /// Only select historic process variables with the given process instance ids. </summary>
	  public virtual HistoricVariableInstanceQuery processInstanceIdIn(params string[] processInstanceIds)
	  {
		ensureNotNull("Process Instance Ids", (object[]) processInstanceIds);
		this.processInstanceIds = processInstanceIds;
		return this;
	  }

	  public virtual HistoricVariableInstanceQuery taskIdIn(params string[] taskIds)
	  {
		ensureNotNull("Task Ids", (object[]) taskIds);
		this.taskIds = taskIds;
		return this;
	  }

	  public virtual HistoricVariableInstanceQuery executionIdIn(params string[] executionIds)
	  {
		ensureNotNull("Execution Ids", (object[]) executionIds);
		this.executionIds = executionIds;
		return this;
	  }

	  public virtual HistoricVariableInstanceQuery caseExecutionIdIn(params string[] caseExecutionIds)
	  {
		ensureNotNull("Case execution ids", (object[]) caseExecutionIds);
		this.caseExecutionIds = caseExecutionIds;
		return this;
	  }

	  public virtual HistoricVariableInstanceQuery caseActivityIdIn(params string[] caseActivityIds)
	  {
		ensureNotNull("Case activity ids", (object[]) caseActivityIds);
		this.caseActivityIds = caseActivityIds;
		return this;
	  }

	  public virtual HistoricVariableInstanceQuery activityInstanceIdIn(params string[] activityInstanceIds)
	  {
		ensureNotNull("Activity Instance Ids", (object[]) activityInstanceIds);
		this.activityInstanceIds = activityInstanceIds;
		return this;
	  }

	  public virtual HistoricVariableInstanceQuery variableName(string variableName)
	  {
		ensureNotNull("variableName", variableName);
		this.variableName_Conflict = variableName;
		return this;
	  }

	  public virtual HistoricVariableInstanceQuery variableValueEquals(string variableName, object variableValue)
	  {
		ensureNotNull("variableName", variableName);
		ensureNotNull("variableValue", variableValue);
		this.variableName_Conflict = variableName;
		queryVariableValue = new QueryVariableValue(variableName, variableValue, QueryOperator.EQUALS, true);
		return this;
	  }

	  public virtual HistoricVariableInstanceQuery variableNameLike(string variableNameLike)
	  {
		ensureNotNull("variableNameLike", variableNameLike);
		this.variableNameLike_Conflict = variableNameLike;
		return this;
	  }

	  protected internal virtual void ensureVariablesInitialized()
	  {
		if (this.queryVariableValue != null)
		{
		  VariableSerializers variableSerializers = Context.ProcessEngineConfiguration.VariableSerializers;
		  queryVariableValue.initialize(variableSerializers);
		}
	  }

	  public virtual HistoricVariableInstanceQuery disableBinaryFetching()
	  {
		isByteArrayFetchingEnabled = false;
		return this;
	  }

	  public virtual HistoricVariableInstanceQuery disableCustomObjectDeserialization()
	  {
		this.isCustomObjectDeserializationEnabled = false;
		return this;
	  }

	  public virtual HistoricVariableInstanceQuery tenantIdIn(params string[] tenantIds)
	  {
		ensureNotNull("tenantIds", (object[]) tenantIds);
		this.tenantIds = tenantIds;
		return this;
	  }

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		ensureVariablesInitialized();
		return commandContext.HistoricVariableInstanceManager.findHistoricVariableInstanceCountByQueryCriteria(this);
	  }

	  public override IList<HistoricVariableInstance> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		ensureVariablesInitialized();
		IList<HistoricVariableInstance> historicVariableInstances = commandContext.HistoricVariableInstanceManager.findHistoricVariableInstancesByQueryCriteria(this, page);

		if (historicVariableInstances != null)
		{
		  foreach (HistoricVariableInstance historicVariableInstance in historicVariableInstances)
		  {

			HistoricVariableInstanceEntity variableInstanceEntity = (HistoricVariableInstanceEntity) historicVariableInstance;
			if (shouldFetchValue(variableInstanceEntity))
			{
			  try
			  {
				variableInstanceEntity.getTypedValue(isCustomObjectDeserializationEnabled);

			  }
			  catch (Exception t)
			  {
				// do not fail if one of the variables fails to load
				LOG.exceptionWhileGettingValueForVariable(t);
			  }
			}

		  }
		}
		return historicVariableInstances;
	  }

	  protected internal virtual bool shouldFetchValue(HistoricVariableInstanceEntity entity)
	  {
		// do not fetch values for byte arrays eagerly (unless requested by the user)
		return isByteArrayFetchingEnabled || !AbstractTypedValueSerializer.BINARY_VALUE_TYPES.Contains(entity.Serializer.Type.Name);
	  }

	  // order by /////////////////////////////////////////////////////////////////

	  public virtual HistoricVariableInstanceQuery orderByProcessInstanceId()
	  {
		orderBy(HistoricVariableInstanceQueryProperty_Fields.PROCESS_INSTANCE_ID);
		return this;
	  }

	  public virtual HistoricVariableInstanceQuery orderByVariableName()
	  {
		orderBy(HistoricVariableInstanceQueryProperty_Fields.VARIABLE_NAME);
		return this;
	  }

	  public virtual HistoricVariableInstanceQuery orderByTenantId()
	  {
		orderBy(HistoricVariableInstanceQueryProperty_Fields.TENANT_ID);
		return this;
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId_Conflict;
		  }
	  }

	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId_Conflict;
		  }
	  }

	  public virtual string[] ActivityInstanceIds
	  {
		  get
		  {
			return activityInstanceIds;
		  }
	  }

	  public virtual string[] ProcessInstanceIds
	  {
		  get
		  {
			return processInstanceIds;
		  }
	  }

	  public virtual string[] TaskIds
	  {
		  get
		  {
			return taskIds;
		  }
	  }

	  public virtual string[] ExecutionIds
	  {
		  get
		  {
			return executionIds;
		  }
	  }

	  public virtual string[] CaseExecutionIds
	  {
		  get
		  {
			return caseExecutionIds;
		  }
	  }

	  public virtual string[] CaseActivityIds
	  {
		  get
		  {
			return caseActivityIds;
		  }
	  }

	  public virtual string VariableName
	  {
		  get
		  {
			return variableName_Conflict;
		  }
	  }

	  public virtual string VariableNameLike
	  {
		  get
		  {
			return variableNameLike_Conflict;
		  }
	  }

	  public virtual QueryVariableValue QueryVariableValue
	  {
		  get
		  {
			return queryVariableValue;
		  }
	  }

	  public virtual HistoricVariableInstanceQuery includeDeleted()
	  {
		includeDeleted_Conflict = true;
		return this;
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId_Conflict;
		  }
	  }

	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey_Conflict;
		  }
	  }
	}

}