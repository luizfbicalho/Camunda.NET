using System;

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

	using HistoricVariableUpdate = org.camunda.bpm.engine.history.HistoricVariableUpdate;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DbEntityLifecycleAware = org.camunda.bpm.engine.impl.db.DbEntityLifecycleAware;
	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;
	using DbEntityManager = org.camunda.bpm.engine.impl.db.entitymanager.DbEntityManager;
	using HistoricVariableUpdateEventEntity = org.camunda.bpm.engine.impl.history.@event.HistoricVariableUpdateEventEntity;
	using ByteArrayField = org.camunda.bpm.engine.impl.persistence.entity.util.ByteArrayField;
	using TypedValueField = org.camunda.bpm.engine.impl.persistence.entity.util.TypedValueField;
	using TypedValueSerializer = org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer;
	using ValueFields = org.camunda.bpm.engine.impl.variable.serializer.ValueFields;
	using ResourceTypes = org.camunda.bpm.engine.repository.ResourceTypes;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class HistoricDetailVariableInstanceUpdateEntity : HistoricVariableUpdateEventEntity, ValueFields, HistoricVariableUpdate, DbEntityLifecycleAware
	{
		private bool InstanceFieldsInitialized = false;

		public HistoricDetailVariableInstanceUpdateEntity()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			typedValueField = new TypedValueField(this, false);
			byteArrayField = new ByteArrayField(this, ResourceTypes.HISTORY);
		}


	  private const long serialVersionUID = 1L;
	  protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;

	  protected internal TypedValueField typedValueField;

	  protected internal ByteArrayField byteArrayField;

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

	  public override void delete()
	  {

		DbEntityManager dbEntityManger = Context.CommandContext.DbEntityManager;

		dbEntityManger.delete(this);

		byteArrayField.deleteByteArrayValue();
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

	  public virtual string ErrorMessage
	  {
		  get
		  {
			return typedValueField.ErrorMessage;
		  }
	  }

	  public override string ByteArrayId
	  {
		  set
		  {
			byteArrayField.ByteArrayId = value;
		  }
	  }

	  public override string SerializerName
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

	  public virtual string ByteArrayValueId
	  {
		  get
		  {
			return byteArrayField.ByteArrayId;
		  }
	  }

	  public virtual sbyte[] ByteArrayValue
	  {
		  get
		  {
			return byteArrayField.getByteArrayValue();
		  }
		  set
		  {
			byteArrayField.setByteArrayValue(value);
		  }
	  }


	  public virtual string Name
	  {
		  get
		  {
			return VariableName;
		  }
	  }

	  // entity lifecycle /////////////////////////////////////////////////////////

	  public virtual void postLoad()
	  {
		// make sure the serializer is initialized
		typedValueField.postLoad();
	  }

	  // getters and setters //////////////////////////////////////////////////////

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

	  public virtual DateTime Time
	  {
		  get
		  {
			return timestamp;
		  }
	  }

	  public override string ToString()
	  {
		return this.GetType().Name + "[variableName=" + variableName + ", variableInstanceId=" + variableInstanceId + ", revision=" + revision + ", serializerName=" + serializerName + ", longValue=" + longValue + ", doubleValue=" + doubleValue + ", textValue=" + textValue + ", textValue2=" + textValue2 + ", byteArrayId=" + byteArrayId + ", activityInstanceId=" + activityInstanceId + ", eventType=" + eventType + ", executionId=" + executionId + ", id=" + id + ", processDefinitionId=" + processInstanceId + ", processInstanceId=" + processInstanceId + ", taskId=" + taskId + ", timestamp=" + timestamp + "]";
	  }

	}

}