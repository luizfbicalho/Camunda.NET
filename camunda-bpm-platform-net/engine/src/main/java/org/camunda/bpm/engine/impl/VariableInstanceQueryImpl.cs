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


	using CommandLogger = org.camunda.bpm.engine.impl.cmd.CommandLogger;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using VariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity;
	using CompareUtil = org.camunda.bpm.engine.impl.util.CompareUtil;
	using AbstractTypedValueSerializer = org.camunda.bpm.engine.impl.variable.serializer.AbstractTypedValueSerializer;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using VariableInstanceQuery = org.camunda.bpm.engine.runtime.VariableInstanceQuery;

	/// <summary>
	/// @author roman.smirnov
	/// </summary>
	[Serializable]
	public class VariableInstanceQueryImpl : AbstractVariableQueryImpl<VariableInstanceQuery, VariableInstance>, VariableInstanceQuery
	{

	  private static readonly CommandLogger LOG = ProcessEngineLogger.CMD_LOGGER;

	  private const long serialVersionUID = 1L;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string variableId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string variableName_Renamed;
	  protected internal string[] variableNames;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string variableNameLike_Renamed;
	  protected internal string[] executionIds;
	  protected internal string[] processInstanceIds;
	  protected internal string[] caseExecutionIds;
	  protected internal string[] caseInstanceIds;
	  protected internal string[] taskIds;
	  protected internal string[] variableScopeIds;
	  protected internal string[] activityInstanceIds;
	  protected internal string[] tenantIds;

	  protected internal bool isByteArrayFetchingEnabled = true;
	  protected internal bool isCustomObjectDeserializationEnabled = true;

	  public VariableInstanceQueryImpl()
	  {
	  }

	  public VariableInstanceQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual VariableInstanceQuery variableId(string id)
	  {
		ensureNotNull("id", id);
		this.variableId_Renamed = id;
		return this;
	  }

	  public virtual VariableInstanceQuery variableName(string variableName)
	  {
		this.variableName_Renamed = variableName;
		return this;
	  }

	  public virtual VariableInstanceQuery variableNameIn(params string[] variableNames)
	  {
		this.variableNames = variableNames;
		return this;
	  }

	  public virtual VariableInstanceQuery variableNameLike(string variableNameLike)
	  {
		this.variableNameLike_Renamed = variableNameLike;
		return this;
	  }

	  public virtual VariableInstanceQuery executionIdIn(params string[] executionIds)
	  {
		this.executionIds = executionIds;
		return this;
	  }

	  public virtual VariableInstanceQuery processInstanceIdIn(params string[] processInstanceIds)
	  {
		this.processInstanceIds = processInstanceIds;
		return this;
	  }

	  public virtual VariableInstanceQuery caseExecutionIdIn(params string[] caseExecutionIds)
	  {
		this.caseExecutionIds = caseExecutionIds;
		return this;
	  }

	  public virtual VariableInstanceQuery caseInstanceIdIn(params string[] caseInstanceIds)
	  {
		this.caseInstanceIds = caseInstanceIds;
		return this;
	  }

	  public virtual VariableInstanceQuery taskIdIn(params string[] taskIds)
	  {
		this.taskIds = taskIds;
		return this;
	  }

	  public virtual VariableInstanceQuery variableScopeIdIn(params string[] variableScopeIds)
	  {
		this.variableScopeIds = variableScopeIds;
		return this;
	  }

	  public virtual VariableInstanceQuery activityInstanceIdIn(params string[] activityInstanceIds)
	  {
		this.activityInstanceIds = activityInstanceIds;
		return this;
	  }

	  public virtual VariableInstanceQuery disableBinaryFetching()
	  {
		this.isByteArrayFetchingEnabled = false;
		return this;
	  }

	  public virtual VariableInstanceQuery disableCustomObjectDeserialization()
	  {
		this.isCustomObjectDeserializationEnabled = false;
		return this;
	  }

	  public virtual VariableInstanceQuery tenantIdIn(params string[] tenantIds)
	  {
		ensureNotNull("tenantIds", (object[]) tenantIds);
		this.tenantIds = tenantIds;
		return this;
	  }

	  // ordering ////////////////////////////////////////////////////

	  public virtual VariableInstanceQuery orderByVariableName()
	  {
		orderBy(VariableInstanceQueryProperty_Fields.VARIABLE_NAME);
		return this;
	  }

	  public virtual VariableInstanceQuery orderByVariableType()
	  {
		orderBy(VariableInstanceQueryProperty_Fields.VARIABLE_TYPE);
		return this;
	  }

	  public virtual VariableInstanceQuery orderByActivityInstanceId()
	  {
		orderBy(VariableInstanceQueryProperty_Fields.ACTIVITY_INSTANCE_ID);
		return this;
	  }

	  public virtual VariableInstanceQuery orderByTenantId()
	  {
		orderBy(VariableInstanceQueryProperty_Fields.TENANT_ID);
		return this;
	  }

	  protected internal override bool hasExcludingConditions()
	  {
		return base.hasExcludingConditions() || CompareUtil.elementIsNotContainedInArray(variableName_Renamed, variableNames);
	  }

	  // results ////////////////////////////////////////////////////

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		ensureVariablesInitialized();
		return commandContext.VariableInstanceManager.findVariableInstanceCountByQueryCriteria(this);
	  }

	  public override IList<VariableInstance> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		ensureVariablesInitialized();
		IList<VariableInstance> result = commandContext.VariableInstanceManager.findVariableInstanceByQueryCriteria(this, page);

		if (result == null)
		{
		  return result;
		}

		// iterate over the result array to initialize the value and serialized value of the variable
		foreach (VariableInstance variableInstance in result)
		{
		  VariableInstanceEntity variableInstanceEntity = (VariableInstanceEntity) variableInstance;

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

		return result;
	  }

	  protected internal virtual bool shouldFetchValue(VariableInstanceEntity entity)
	  {
		// do not fetch values for byte arrays eagerly (unless requested by the user)
		return isByteArrayFetchingEnabled || !AbstractTypedValueSerializer.BINARY_VALUE_TYPES.Contains(entity.Serializer.Type.Name);
	  }

	  // getters ////////////////////////////////////////////////////

	  public virtual string VariableId
	  {
		  get
		  {
			return variableId_Renamed;
		  }
	  }

	  public virtual string VariableName
	  {
		  get
		  {
			return variableName_Renamed;
		  }
	  }

	  public virtual string[] VariableNames
	  {
		  get
		  {
			return variableNames;
		  }
	  }

	  public virtual string VariableNameLike
	  {
		  get
		  {
			return variableNameLike_Renamed;
		  }
	  }

	  public virtual string[] ExecutionIds
	  {
		  get
		  {
			return executionIds;
		  }
	  }

	  public virtual string[] ProcessInstanceIds
	  {
		  get
		  {
			return processInstanceIds;
		  }
	  }

	  public virtual string[] CaseExecutionIds
	  {
		  get
		  {
			return caseExecutionIds;
		  }
	  }

	  public virtual string[] CaseInstanceIds
	  {
		  get
		  {
			return caseInstanceIds;
		  }
	  }

	  public virtual string[] TaskIds
	  {
		  get
		  {
			return taskIds;
		  }
	  }

	  public virtual string[] VariableScopeIds
	  {
		  get
		  {
			return variableScopeIds;
		  }
	  }

	  public virtual string[] ActivityInstanceIds
	  {
		  get
		  {
			return activityInstanceIds;
		  }
	  }
	}

}