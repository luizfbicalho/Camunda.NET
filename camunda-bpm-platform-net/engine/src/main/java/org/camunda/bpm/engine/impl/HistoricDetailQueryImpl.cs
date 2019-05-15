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


	using HistoricDetail = org.camunda.bpm.engine.history.HistoricDetail;
	using HistoricDetailQuery = org.camunda.bpm.engine.history.HistoricDetailQuery;
	using CommandLogger = org.camunda.bpm.engine.impl.cmd.CommandLogger;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using HistoricDetailVariableInstanceUpdateEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricDetailVariableInstanceUpdateEntity;
	using AbstractTypedValueSerializer = org.camunda.bpm.engine.impl.variable.serializer.AbstractTypedValueSerializer;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class HistoricDetailQueryImpl : AbstractQuery<HistoricDetailQuery, HistoricDetail>, HistoricDetailQuery
	{

	  private static readonly CommandLogger LOG = ProcessEngineLogger.CMD_LOGGER;

	  private const long serialVersionUID = 1L;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string detailId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string taskId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processInstanceId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseInstanceId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string executionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseExecutionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string activityId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string activityInstanceId_Renamed;
	  protected internal string type;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string variableInstanceId_Renamed;
	  protected internal string[] variableTypes;
	  protected internal string[] tenantIds;
	  protected internal string[] processInstanceIds;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string userOperationId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal long? sequenceCounter_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime occurredBefore_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime occurredAfter_Renamed;

	  protected internal bool excludeTaskRelated = false;
	  protected internal bool isByteArrayFetchingEnabled = true;
	  protected internal bool isCustomObjectDeserializationEnabled = true;

	  public HistoricDetailQueryImpl()
	  {
	  }

	  public HistoricDetailQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual HistoricDetailQuery detailId(string id)
	  {
		ensureNotNull("detailId", id);
		this.detailId_Renamed = id;
		return this;
	  }

	  public virtual HistoricDetailQuery variableInstanceId(string variableInstanceId)
	  {
		ensureNotNull("variableInstanceId", variableInstanceId);
		this.variableInstanceId_Renamed = variableInstanceId;
		return this;
	  }

	  public virtual HistoricDetailQuery variableTypeIn(params string[] variableTypes)
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

	  public virtual HistoricDetailQuery processInstanceId(string processInstanceId)
	  {
		this.processInstanceId_Renamed = processInstanceId;
		return this;
	  }

	  public virtual HistoricDetailQuery caseInstanceId(string caseInstanceId)
	  {
		ensureNotNull("Case instance id", caseInstanceId);
		this.caseInstanceId_Renamed = caseInstanceId;
		return this;
	  }

	  public virtual HistoricDetailQuery executionId(string executionId)
	  {
		this.executionId_Renamed = executionId;
		return this;
	  }

	  public virtual HistoricDetailQuery caseExecutionId(string caseExecutionId)
	  {
		ensureNotNull("Case execution id", caseExecutionId);
		this.caseExecutionId_Renamed = caseExecutionId;
		return this;
	  }

	  public virtual HistoricDetailQuery activityId(string activityId)
	  {
		this.activityId_Renamed = activityId;
		return this;
	  }

	  public virtual HistoricDetailQuery activityInstanceId(string activityInstanceId)
	  {
		this.activityInstanceId_Renamed = activityInstanceId;
		return this;
	  }

	  public virtual HistoricDetailQuery taskId(string taskId)
	  {
		this.taskId_Renamed = taskId;
		return this;
	  }

	  public virtual HistoricDetailQuery formProperties()
	  {
		this.type = "FormProperty";
		return this;
	  }

	  public virtual HistoricDetailQuery formFields()
	  {
		this.type = "FormProperty";
		return this;
	  }

	  public virtual HistoricDetailQuery variableUpdates()
	  {
		this.type = "VariableUpdate";
		return this;
	  }

	  public virtual HistoricDetailQuery tenantIdIn(params string[] tenantIds)
	  {
		ensureNotNull("tenantIds", (object[]) tenantIds);
		this.tenantIds = tenantIds;
		return this;
	  }

	  public virtual HistoricDetailQuery processInstanceIdIn(params string[] processInstanceIds)
	  {
		ensureNotNull("Process Instance Ids", (object[]) processInstanceIds);
		this.processInstanceIds = processInstanceIds;
		return this;
	  }

	  public virtual HistoricDetailQuery userOperationId(string userOperationId)
	  {
		ensureNotNull("userOperationId", userOperationId);
		this.userOperationId_Renamed = userOperationId;
		return this;
	  }

	  public virtual HistoricDetailQueryImpl sequenceCounter(long sequenceCounter)
	  {
		this.sequenceCounter_Renamed = sequenceCounter;
		return this;
	  }

	  public virtual HistoricDetailQuery excludeTaskDetails()
	  {
		this.excludeTaskRelated = true;
		return this;
	  }

	  public virtual HistoricDetailQuery occurredBefore(DateTime date)
	  {
		ensureNotNull("occurred before", date);
		occurredBefore_Renamed = date;
		return this;
	  }

	  public virtual HistoricDetailQuery occurredAfter(DateTime date)
	  {
		ensureNotNull("occurred after", date);
		occurredAfter_Renamed = date;
		return this;
	  }

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.HistoricDetailManager.findHistoricDetailCountByQueryCriteria(this);
	  }

	  public virtual HistoricDetailQuery disableBinaryFetching()
	  {
		this.isByteArrayFetchingEnabled = false;
		return this;
	  }

	  public virtual HistoricDetailQuery disableCustomObjectDeserialization()
	  {
		this.isCustomObjectDeserializationEnabled = false;
		return this;
	  }

	  public override IList<HistoricDetail> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		IList<HistoricDetail> historicDetails = commandContext.HistoricDetailManager.findHistoricDetailsByQueryCriteria(this, page);
		if (historicDetails != null)
		{
		  foreach (HistoricDetail historicDetail in historicDetails)
		  {
			if (historicDetail is HistoricDetailVariableInstanceUpdateEntity)
			{
			  HistoricDetailVariableInstanceUpdateEntity entity = (HistoricDetailVariableInstanceUpdateEntity) historicDetail;
			  if (shouldFetchValue(entity))
			  {
				try
				{
				  entity.getTypedValue(isCustomObjectDeserializationEnabled);

				}
				catch (Exception t)
				{
				  // do not fail if one of the variables fails to load
				  LOG.exceptionWhileGettingValueForVariable(t);
				}
			  }

			}
		  }
		}
		return historicDetails;
	  }

	  protected internal virtual bool shouldFetchValue(HistoricDetailVariableInstanceUpdateEntity entity)
	  {
		// do not fetch values for byte arrays eagerly (unless requested by the user)
		return isByteArrayFetchingEnabled || !AbstractTypedValueSerializer.BINARY_VALUE_TYPES.Contains(entity.Serializer.Type.Name);
	  }

	  // order by /////////////////////////////////////////////////////////////////

	  public virtual HistoricDetailQuery orderByProcessInstanceId()
	  {
		orderBy(HistoricDetailQueryProperty_Fields.PROCESS_INSTANCE_ID);
		return this;
	  }

	  public virtual HistoricDetailQuery orderByTime()
	  {
		orderBy(HistoricDetailQueryProperty_Fields.TIME);
		return this;
	  }

	  public virtual HistoricDetailQuery orderByVariableName()
	  {
		orderBy(HistoricDetailQueryProperty_Fields.VARIABLE_NAME);
		return this;
	  }

	  public virtual HistoricDetailQuery orderByFormPropertyId()
	  {
		orderBy(HistoricDetailQueryProperty_Fields.VARIABLE_NAME);
		return this;
	  }

	  public virtual HistoricDetailQuery orderByVariableRevision()
	  {
		orderBy(HistoricDetailQueryProperty_Fields.VARIABLE_REVISION);
		return this;
	  }

	  public virtual HistoricDetailQuery orderByVariableType()
	  {
		orderBy(HistoricDetailQueryProperty_Fields.VARIABLE_TYPE);
		return this;
	  }

	  public virtual HistoricDetailQuery orderPartiallyByOccurrence()
	  {
		orderBy(HistoricDetailQueryProperty_Fields.SEQUENCE_COUNTER);
		return this;
	  }

	  public virtual HistoricDetailQuery orderByTenantId()
	  {
		return orderBy(HistoricDetailQueryProperty_Fields.TENANT_ID);
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId_Renamed;
		  }
	  }

	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId_Renamed;
		  }
	  }

	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId_Renamed;
		  }
	  }

	  public virtual string CaseExecutionId
	  {
		  get
		  {
			return caseExecutionId_Renamed;
		  }
	  }

	  public virtual string TaskId
	  {
		  get
		  {
			return taskId_Renamed;
		  }
	  }

	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId_Renamed;
		  }
	  }

	  public virtual string Type
	  {
		  get
		  {
			return type;
		  }
	  }

	  public virtual bool ExcludeTaskRelated
	  {
		  get
		  {
			return excludeTaskRelated;
		  }
	  }

	  public virtual string DetailId
	  {
		  get
		  {
			return detailId_Renamed;
		  }
	  }

	  public virtual string[] ProcessInstanceIds
	  {
		  get
		  {
			return processInstanceIds;
		  }
	  }

	  public virtual DateTime OccurredBefore
	  {
		  get
		  {
			return occurredBefore_Renamed;
		  }
	  }

	  public virtual DateTime OccurredAfter
	  {
		  get
		  {
			return occurredAfter_Renamed;
		  }
	  }
	}

}