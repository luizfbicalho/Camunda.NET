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
namespace org.camunda.bpm.engine.variable.impl.type
{

	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	[Serializable]
	public abstract class AbstractValueTypeImpl : ValueType
	{
		public abstract TypedValue createValue(object value, IDictionary<string, object> valueInfo);
		public abstract IDictionary<string, object> getValueInfo(TypedValue typedValue);
		public abstract bool PrimitiveValueType {get;}

	  private const long serialVersionUID = 1L;

	  protected internal string name;

	  public AbstractValueTypeImpl(string name)
	  {
		this.name = name;
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public override string ToString()
	  {
		return name;
	  }

	  public virtual bool Abstract
	  {
		  get
		  {
			return false;
		  }
	  }

	  public virtual ValueType Parent
	  {
		  get
		  {
			return null;
		  }
	  }

	  public virtual bool canConvertFromTypedValue(TypedValue typedValue)
	  {
		return false;
	  }

	  public virtual TypedValue convertFromTypedValue(TypedValue typedValue)
	  {
		throw unsupportedConversion(typedValue.Type);
	  }

	  protected internal virtual System.ArgumentException unsupportedConversion(ValueType typeToConvertTo)
	  {
		return new System.ArgumentException("The type " + Name + " supports no conversion from type: " + typeToConvertTo.Name);
	  }

	  public override int GetHashCode()
	  {
		const int prime = 31;
		int result = 1;
		result = prime * result + ((string.ReferenceEquals(name, null)) ? 0 : name.GetHashCode());
		return result;
	  }

	  public override bool Equals(object obj)
	  {
		if (this == obj)
		{
		  return true;
		}
		if (obj == null)
		{
		  return false;
		}
		if (this.GetType() != obj.GetType())
		{
		  return false;
		}
		AbstractValueTypeImpl other = (AbstractValueTypeImpl) obj;
		if (string.ReferenceEquals(name, null))
		{
		  if (!string.ReferenceEquals(other.name, null))
		  {
			return false;
		  }
		}
		else if (!name.Equals(other.name))
		{
		  return false;
		}
		return true;
	  }

	  protected internal virtual bool? isTransient(IDictionary<string, object> valueInfo)
	  {
		if (valueInfo != null && valueInfo.ContainsKey(org.camunda.bpm.engine.variable.type.ValueType_Fields.VALUE_INFO_TRANSIENT))
		{
		  object isTransient = valueInfo[org.camunda.bpm.engine.variable.type.ValueType_Fields.VALUE_INFO_TRANSIENT];
		  if (isTransient is bool?)
		  {
			return (bool?) isTransient;
		  }
		  else
		  {
			throw new System.ArgumentException("The property 'transient' should have a value of type 'boolean'.");
		  }
		}
		return false;
	  }

	}

}