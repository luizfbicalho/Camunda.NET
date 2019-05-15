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
namespace org.camunda.bpm.engine.impl.persistence.entity
{

	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using DbEntityLifecycleAware = org.camunda.bpm.engine.impl.db.DbEntityLifecycleAware;
	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;
	using HasDbRevision = org.camunda.bpm.engine.impl.db.HasDbRevision;
	using HistoricEntity = org.camunda.bpm.engine.impl.db.HistoricEntity;
	using HistoricVariableUpdateEventEntity = org.camunda.bpm.engine.impl.history.@event.HistoricVariableUpdateEventEntity;
	using ByteArrayField = org.camunda.bpm.engine.impl.persistence.entity.util.ByteArrayField;
	using TypedValueField = org.camunda.bpm.engine.impl.persistence.entity.util.TypedValueField;
	using TypedValueSerializer = org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer;
	using ValueFields = org.camunda.bpm.engine.impl.variable.serializer.ValueFields;
	using ResourceTypes = org.camunda.bpm.engine.repository.ResourceTypes;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Christian Lipphardt (camunda)
	/// </summary>
	[Serializable]
	public class HistoricVariableInstanceEntity : ValueFields, HistoricVariableInstance, DbEntity, HasDbRevision, HistoricEntity, DbEntityLifecycleAware
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			byteArrayField = new ByteArrayField(this, ResourceTypes.HISTORY);
			typedValueField = new TypedValueField(this, false);
		}


	  private const long serialVersionUID = 1L;
	  protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;

	  protected internal string id;

	  protected internal string processDefinitionKey;
	  protected internal string processDefinitionId;
	  protected internal string rootProcessInstanceId;
	  protected internal string processInstanceId;

	  protected internal string taskId;
	  protected internal string executionId;
	  protected internal string activityInstanceId;
	  protected internal string tenantId;

	  protected internal string caseDefinitionKey;
	  protected internal string caseDefinitionId;
	  protected internal string caseInstanceId;
	  protected internal string caseExecutionId;

	  protected internal string name;
	  protected internal int revision;
	  protected internal DateTime createTime;

	  protected internal long? longValue;
	  protected internal double? doubleValue;
	  protected internal string textValue;
	  protected internal string textValue2;

	  protected internal string state = "CREATED";

	  protected internal DateTime removalTime;

	  protected internal ByteArrayField byteArrayField;

	  protected internal TypedValueField typedValueField;

	  public HistoricVariableInstanceEntity()
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
	  }

	  public HistoricVariableInstanceEntity(HistoricVariableUpdateEventEntity historyEvent)
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
		updateFromEvent(historyEvent);
	  }

	  public virtual void updateFromEvent(HistoricVariableUpdateEventEntity historyEvent)
	  {
		this.id = historyEvent.VariableInstanceId;
		this.processDefinitionKey = historyEvent.ProcessDefinitionKey;
		this.processDefinitionId = historyEvent.ProcessDefinitionId;
		this.processInstanceId = historyEvent.ProcessInstanceId;
		this.taskId = historyEvent.TaskId;
		this.executionId = historyEvent.ExecutionId;
		this.activityInstanceId = historyEvent.ScopeActivityInstanceId;
		this.tenantId = historyEvent.TenantId;
		this.caseDefinitionKey = historyEvent.CaseDefinitionKey;
		this.caseDefinitionId = historyEvent.CaseDefinitionId;
		this.caseInstanceId = historyEvent.CaseInstanceId;
		this.caseExecutionId = historyEvent.CaseExecutionId;
		this.name = historyEvent.VariableName;
		this.longValue = historyEvent.LongValue;
		this.doubleValue = historyEvent.DoubleValue;
		this.textValue = historyEvent.TextValue;
		this.textValue2 = historyEvent.TextValue2;
		this.createTime = historyEvent.Timestamp;
		this.rootProcessInstanceId = historyEvent.RootProcessInstanceId;
		this.removalTime = historyEvent.RemovalTime;

		SerializerName = historyEvent.SerializerName;

		byteArrayField.deleteByteArrayValue();

		if (historyEvent.ByteValue != null)
		{
		  byteArrayField.RootProcessInstanceId = rootProcessInstanceId;
		  byteArrayField.RemovalTime = removalTime;
		  setByteArrayValue(historyEvent.ByteValue);
		}

	  }

	  public virtual void delete()
	  {
		byteArrayField.deleteByteArrayValue();

		Context.CommandContext.DbEntityManager.delete(this);
	  }

	  public virtual object PersistentState
	  {
		  get
		  {
			IList<object> state = new List<object>(8);
			state.Add(SerializerName);
			state.Add(textValue);
			state.Add(textValue2);
			state.Add(this.state);
			state.Add(doubleValue);
			state.Add(longValue);
			state.Add(processDefinitionId);
			state.Add(processDefinitionKey);
			state.Add(ByteArrayId);
			return state;
		  }
	  }

	  public virtual int RevisionNext
	  {
		  get
		  {
			return revision + 1;
		  }
	  }

	  public virtual object Value
	  {
		  get
		  {
			return typedValueField.getValue();
		  }
	  }

	  public virtual TypedValue TypedValue
	  {
		  get
		  {
			return typedValueField.TypedValue;
		  }
	  }

	  public virtual TypedValue getTypedValue(bool deserializeValue)
	  {
		return typedValueField.getTypedValue(deserializeValue);
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer<?> getSerializer()
	  public virtual TypedValueSerializer<object> Serializer
	  {
		  get
		  {
			return typedValueField.Serializer;
		  }
	  }

	  public virtual string ByteArrayValueId
	  {
		  get
		  {
			return byteArrayField.ByteArrayId;
		  }
	  }

	  public virtual string ByteArrayId
	  {
		  get
		  {
			return byteArrayField.ByteArrayId;
		  }
		  set
		  {
			byteArrayField.ByteArrayId = value;
		  }
	  }


	  public virtual sbyte[] getByteArrayValue()
	  {
		return byteArrayField.getByteArrayValue();
	  }

	  public virtual void setByteArrayValue(sbyte[] bytes)
	  {
		byteArrayField.setByteArrayValue(bytes);
	  }

	  // entity lifecycle /////////////////////////////////////////////////////////

	  public virtual void postLoad()
	  {
		// make sure the serializer is initialized
		typedValueField.postLoad();
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual string SerializerName
	  {
		  get
		  {
			return typedValueField.SerializerName;
		  }
		  set
		  {
			typedValueField.SerializerName = value;
		  }
	  }


	  public virtual string TypeName
	  {
		  get
		  {
			return typedValueField.TypeName;
		  }
	  }

	  public virtual string VariableTypeName
	  {
		  get
		  {
			return TypeName;
		  }
	  }

	  public virtual string VariableName
	  {
		  get
		  {
			return name;
		  }
	  }

	  public virtual int Revision
	  {
		  get
		  {
			return revision;
		  }
		  set
		  {
			this.revision = value;
		  }
	  }


	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
		  set
		  {
			this.name = value;
		  }
	  }


	  public virtual long? LongValue
	  {
		  get
		  {
			return longValue;
		  }
		  set
		  {
			this.longValue = value;
		  }
	  }


	  public virtual double? DoubleValue
	  {
		  get
		  {
			return doubleValue;
		  }
		  set
		  {
			this.doubleValue = value;
		  }
	  }


	  public virtual string TextValue
	  {
		  get
		  {
			return textValue;
		  }
		  set
		  {
			this.textValue = value;
		  }
	  }


	  public virtual string TextValue2
	  {
		  get
		  {
			return textValue2;
		  }
		  set
		  {
			this.textValue2 = value;
		  }
	  }


	  public virtual void setByteArrayValue(ByteArrayEntity byteArrayValue)
	  {
		byteArrayField.setByteArrayValue(byteArrayValue);
	  }

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
		  set
		  {
			this.id = value;
		  }
	  }


	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey;
		  }
		  set
		  {
			this.processDefinitionKey = value;
		  }
	  }


	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId;
		  }
		  set
		  {
			this.processDefinitionId = value;
		  }
	  }


	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId;
		  }
		  set
		  {
			this.processInstanceId = value;
		  }
	  }


	  public virtual string TaskId
	  {
		  get
		  {
			return taskId;
		  }
		  set
		  {
			this.taskId = value;
		  }
	  }


	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId;
		  }
		  set
		  {
			this.executionId = value;
		  }
	  }


	  [Obsolete]
	  public virtual string ActivtyInstanceId
	  {
		  get
		  {
			return activityInstanceId;
		  }
	  }

	  public virtual string ActivityInstanceId
	  {
		  get
		  {
			return activityInstanceId;
		  }
		  set
		  {
			this.activityInstanceId = value;
		  }
	  }


	  public virtual string CaseDefinitionKey
	  {
		  get
		  {
			return caseDefinitionKey;
		  }
		  set
		  {
			this.caseDefinitionKey = value;
		  }
	  }


	  public virtual string CaseDefinitionId
	  {
		  get
		  {
			return caseDefinitionId;
		  }
		  set
		  {
			this.caseDefinitionId = value;
		  }
	  }


	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId;
		  }
		  set
		  {
			this.caseInstanceId = value;
		  }
	  }


	  public virtual string CaseExecutionId
	  {
		  get
		  {
			return caseExecutionId;
		  }
		  set
		  {
			this.caseExecutionId = value;
		  }
	  }


	  public virtual string ErrorMessage
	  {
		  get
		  {
			return typedValueField.ErrorMessage;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
		  set
		  {
			this.tenantId = value;
		  }
	  }


	  public virtual string State
	  {
		  get
		  {
			return state;
		  }
		  set
		  {
			this.state = value;
		  }
	  }


	  public virtual DateTime CreateTime
	  {
		  get
		  {
			return createTime;
		  }
		  set
		  {
			this.createTime = value;
		  }
	  }


	  public virtual string RootProcessInstanceId
	  {
		  get
		  {
			return rootProcessInstanceId;
		  }
		  set
		  {
			this.rootProcessInstanceId = value;
		  }
	  }


	  public virtual DateTime RemovalTime
	  {
		  get
		  {
			return removalTime;
		  }
		  set
		  {
			this.removalTime = value;
		  }
	  }


	  public override string ToString()
	  {
		return this.GetType().Name + "[id=" + id + ", processDefinitionKey=" + processDefinitionKey + ", processDefinitionId=" + processDefinitionId + ", rootProcessInstanceId=" + rootProcessInstanceId + ", removalTime=" + removalTime + ", processInstanceId=" + processInstanceId + ", taskId=" + taskId + ", executionId=" + executionId + ", tenantId=" + tenantId + ", activityInstanceId=" + activityInstanceId + ", caseDefinitionKey=" + caseDefinitionKey + ", caseDefinitionId=" + caseDefinitionId + ", caseInstanceId=" + caseInstanceId + ", caseExecutionId=" + caseExecutionId + ", name=" + name + ", createTime=" + createTime + ", revision=" + revision + ", serializerName=" + SerializerName + ", longValue=" + longValue + ", doubleValue=" + doubleValue + ", textValue=" + textValue + ", textValue2=" + textValue2 + ", state=" + state + ", byteArrayId=" + ByteArrayId + "]";
	  }

	}

}