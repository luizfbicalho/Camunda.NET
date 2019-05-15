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
namespace org.camunda.bpm.engine.impl.variable.serializer
{
	using PrimitiveValueType = org.camunda.bpm.engine.variable.type.PrimitiveValueType;
	using PrimitiveValue = org.camunda.bpm.engine.variable.value.PrimitiveValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public abstract class PrimitiveValueSerializer<T> : AbstractTypedValueSerializer<T>
	{

	  public PrimitiveValueSerializer(PrimitiveValueType variableType) : base(variableType)
	  {
	  }

	  public override string Name
	  {
		  get
		  {
			// default implementation returns the name of the type. This is OK since we assume that
			// there is only a single serializer for a primitive variable type.
			// If multiple serializers exist for the same type, they must override
			// this method and return distinct values.
			return valueType.Name;
		  }
	  }

	  public override T readValue(ValueFields valueFields, bool deserializeObjectValue)
	  {
		// primitive values are always deserialized
		return readValue(valueFields);
	  }

	  public abstract T readValue(ValueFields valueFields);

	  public override PrimitiveValueType Type
	  {
		  get
		  {
			return (PrimitiveValueType) base.Type;
		  }
	  }

	  protected internal override bool canWriteValue(TypedValue typedValue)
	  {
		object value = typedValue.Value;
		Type javaType = Type.JavaType;

		return value == null || javaType.IsAssignableFrom(value.GetType());
	  }

	}

}