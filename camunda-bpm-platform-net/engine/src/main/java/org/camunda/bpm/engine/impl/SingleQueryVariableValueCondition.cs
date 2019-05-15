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

	using TypedValueSerializer = org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer;
	using ValueFields = org.camunda.bpm.engine.impl.variable.serializer.ValueFields;
	using VariableSerializers = org.camunda.bpm.engine.impl.variable.serializer.VariableSerializers;
	using JPAVariableSerializer = org.camunda.bpm.engine.impl.variable.serializer.jpa.JPAVariableSerializer;
	using UntypedValueImpl = org.camunda.bpm.engine.variable.impl.value.UntypedValueImpl;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class SingleQueryVariableValueCondition : AbstractQueryVariableValueCondition, ValueFields
	{

	  protected internal string textValue;
	  protected internal string textValue2;
	  protected internal long? longValue;
	  protected internal double? doubleValue;
	  protected internal string type;

	  public SingleQueryVariableValueCondition(QueryVariableValue variableValue) : base(variableValue)
	  {
	  }

	  public override void initializeValue(VariableSerializers serializers)
	  {
		TypedValue typedValue = wrappedQueryValue.TypedValue;
		initializeValue(serializers, typedValue);
	  }

	  public virtual void initializeValue(VariableSerializers serializers, TypedValue typedValue)
	  {
		TypedValueSerializer serializer = determineSerializer(serializers, typedValue);

		if (typedValue is UntypedValueImpl)
		{
		  // type has been detected
		  typedValue = serializer.convertToTypedValue((UntypedValueImpl) typedValue);
		}
		serializer.writeValue(typedValue, this);
		this.type = serializer.Name;
	  }

	  protected internal virtual TypedValueSerializer determineSerializer(VariableSerializers serializers, TypedValue value)
	  {
		TypedValueSerializer serializer = serializers.findSerializerForValue(value);

		if (serializer.Type == ValueType.BYTES)
		{
		  throw new ProcessEngineException("Variables of type ByteArray cannot be used to query");
		}
		else if (serializer.Type == ValueType.FILE)
		{
		  throw new ProcessEngineException("Variables of type File cannot be used to query");
		}
		else if (serializer is JPAVariableSerializer)
		{
		  if (wrappedQueryValue.Operator != QueryOperator.EQUALS)
		  {
			throw new ProcessEngineException("JPA entity variables can only be used in 'variableValueEquals'");
		  }

		}
		else
		{
		  if (!serializer.Type.PrimitiveValueType)
		  {
			throw new ProcessEngineException("Object values cannot be used to query");
		  }

		}

		return serializer;
	  }

	  public override IList<SingleQueryVariableValueCondition> DisjunctiveConditions
	  {
		  get
		  {
			return Collections.singletonList(this);
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			return wrappedQueryValue.Name;
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

	  public virtual sbyte[] ByteArrayValue
	  {
		  get
		  {
			return null;
		  }
		  set
		  {
		  }
	  }


	  public virtual string Type
	  {
		  get
		  {
			return type;
		  }
	  }


	}

}