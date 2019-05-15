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
namespace org.camunda.spin.plugin.variable.type.impl
{

	using AbstractValueTypeImpl = org.camunda.bpm.engine.variable.impl.type.AbstractValueTypeImpl;
	using SerializableValue = org.camunda.bpm.engine.variable.value.SerializableValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;
	using SpinValue = org.camunda.spin.plugin.variable.value.SpinValue;
	using SpinValueBuilder = org.camunda.spin.plugin.variable.value.builder.SpinValueBuilder;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public abstract class SpinValueTypeImpl : AbstractValueTypeImpl, SpinValueType
	{

	  private const long serialVersionUID = 1L;

	  public SpinValueTypeImpl(string name) : base(name)
	  {
	  }

	  public virtual TypedValue createValue(object value, IDictionary<string, object> valueInfo)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.spin.plugin.variable.value.builder.SpinValueBuilder<?> builder = createValue((org.camunda.spin.plugin.variable.value.SpinValue) value);
		SpinValueBuilder<object> builder = createValue((SpinValue) value);
		applyValueInfo(builder, valueInfo);
		return builder.create();
	  }

	  public virtual SerializableValue createValueFromSerialized(string serializedValue, IDictionary<string, object> valueInfo)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.spin.plugin.variable.value.builder.SpinValueBuilder<?> builder = createValueFromSerialized(serializedValue);
		SpinValueBuilder<object> builder = createValueFromSerialized(serializedValue);
		applyValueInfo(builder, valueInfo);
		return builder.create();
	  }

	  public virtual bool PrimitiveValueType
	  {
		  get
		  {
			return false;
		  }
	  }

	  public virtual IDictionary<string, object> getValueInfo(TypedValue typedValue)
	  {
		if (!(typedValue is SpinValue))
		{
		  throw new System.ArgumentException("Value not of type Spin Value.");
		}
		SpinValue spinValue = (SpinValue) typedValue;

		IDictionary<string, object> valueInfo = new Dictionary<string, object>();

		if (spinValue.Transient)
		{
		  valueInfo[VALUE_INFO_TRANSIENT] = spinValue.Transient;
		}

		return valueInfo;
	  }

	  protected internal virtual void applyValueInfo<T1>(SpinValueBuilder<T1> builder, IDictionary<string, object> valueInfo)
	  {
		if (valueInfo != null)
		{
		  builder.Transient = isTransient(valueInfo);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected abstract org.camunda.spin.plugin.variable.value.builder.SpinValueBuilder<?> createValue(org.camunda.spin.plugin.variable.value.SpinValue value);
	  protected internal abstract SpinValueBuilder<object> createValue(SpinValue value);

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected abstract org.camunda.spin.plugin.variable.value.builder.SpinValueBuilder<?> createValueFromSerialized(String value);
	  protected internal abstract SpinValueBuilder<object> createValueFromSerialized(string value);

	}

}