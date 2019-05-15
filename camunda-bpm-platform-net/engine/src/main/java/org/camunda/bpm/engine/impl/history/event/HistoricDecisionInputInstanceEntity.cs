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
namespace org.camunda.bpm.engine.impl.history.@event
{

	using HistoricDecisionInputInstance = org.camunda.bpm.engine.history.HistoricDecisionInputInstance;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ByteArrayField = org.camunda.bpm.engine.impl.persistence.entity.util.ByteArrayField;
	using TypedValueField = org.camunda.bpm.engine.impl.persistence.entity.util.TypedValueField;
	using ValueFields = org.camunda.bpm.engine.impl.variable.serializer.ValueFields;
	using ResourceTypes = org.camunda.bpm.engine.repository.ResourceTypes;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Philipp Ossler
	/// </summary>
	[Serializable]
	public class HistoricDecisionInputInstanceEntity : HistoryEvent, HistoricDecisionInputInstance, ValueFields
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			typedValueField = new TypedValueField(this, false);
		}


	  private const long serialVersionUID = 1L;

	  protected internal string decisionInstanceId;

	  protected internal string clauseId;
	  protected internal string clauseName;

	  protected internal long? longValue;
	  protected internal double? doubleValue;
	  protected internal string textValue;
	  protected internal string textValue2;

	  protected internal string tenantId;

	  protected internal ByteArrayField byteArrayField;
	  protected internal TypedValueField typedValueField;

	  protected internal DateTime createTime;

	  public HistoricDecisionInputInstanceEntity()
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
		byteArrayField = new ByteArrayField(this, ResourceTypes.HISTORY);
	  }

	  public HistoricDecisionInputInstanceEntity(string rootProcessInstanceId, DateTime removalTime)
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
		this.rootProcessInstanceId = rootProcessInstanceId;
		this.removalTime = removalTime;
		byteArrayField = new ByteArrayField(this, ResourceTypes.HISTORY, RootProcessInstanceId, RemovalTime);
	  }

	  public virtual string DecisionInstanceId
	  {
		  get
		  {
			return decisionInstanceId;
		  }
		  set
		  {
			this.decisionInstanceId = value;
		  }
	  }


	  public virtual string ClauseId
	  {
		  get
		  {
			return clauseId;
		  }
		  set
		  {
			this.clauseId = value;
		  }
	  }


	  public virtual string ClauseName
	  {
		  get
		  {
			return clauseName;
		  }
		  set
		  {
			this.clauseName = value;
		  }
	  }


	  public virtual string TypeName
	  {
		  get
		  {
			return typedValueField.TypeName;
		  }
		  set
		  {
			typedValueField.SerializerName = value;
		  }
	  }


	  public virtual object getValue()
	  {
		return typedValueField.getValue();
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

	  public virtual string ErrorMessage
	  {
		  get
		  {
			return typedValueField.ErrorMessage;
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			// used for save a byte value
			return clauseId;
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


	  public virtual string ByteArrayValueId
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


	  public virtual void setValue(TypedValue typedValue)
	  {
		typedValueField.setValue(typedValue);
	  }

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


	  public override string RootProcessInstanceId
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


	  public virtual void delete()
	  {
		byteArrayField.deleteByteArrayValue();

		Context.CommandContext.DbEntityManager.delete(this);
	  }

	}

}