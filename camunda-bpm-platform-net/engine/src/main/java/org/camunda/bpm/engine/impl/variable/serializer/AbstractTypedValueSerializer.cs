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
namespace org.camunda.bpm.engine.impl.variable.serializer
{

	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public abstract class AbstractTypedValueSerializer<T> : TypedValueSerializer<T> where T : org.camunda.bpm.engine.variable.value.TypedValue
	{
		public abstract TypedValue convertToTypedValue(org.camunda.bpm.engine.variable.impl.value.UntypedValueImpl untypedValue);
		public abstract TypedValue readValue(ValueFields valueFields, bool deserializeValue);
		public abstract void writeValue(T value, ValueFields valueFields);
		public abstract string Name {get;}

	  public static readonly ISet<string> BINARY_VALUE_TYPES = new HashSet<string>();
	  static AbstractTypedValueSerializer()
	  {
		BINARY_VALUE_TYPES.Add(ValueType.BYTES.Name);
		BINARY_VALUE_TYPES.Add(ValueType.FILE.Name);
	  }

	  protected internal ValueType valueType;

	  public AbstractTypedValueSerializer(ValueType type)
	  {
		valueType = type;
	  }

	  public virtual ValueType Type
	  {
		  get
		  {
			return valueType;
		  }
	  }

	  public virtual string SerializationDataformat
	  {
		  get
		  {
			// default implementation returns null
			return null;
		  }
	  }

	  public virtual bool canHandle(TypedValue value)
	  {
		if (value.Type != null && !valueType.GetType().IsAssignableFrom(value.Type.GetType()))
		{
		  return false;
		}
		else
		{
		  return canWriteValue(value);
		}
	  }

	  protected internal abstract bool canWriteValue(TypedValue value);

	  public virtual bool isMutableValue(T typedValue)
	  {
		// default
		return false;
	  }

	}

}