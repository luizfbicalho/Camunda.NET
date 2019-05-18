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
namespace org.camunda.bpm.engine.variable.impl.value
{
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// Used when the type of an object has not been specified by the user and
	/// needs to be autodetected.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	[Serializable]
	public class UntypedValueImpl : TypedValue
	{

	  private const long serialVersionUID = 1L;

	  protected internal object value;

	  protected internal bool isTransient;

	  public UntypedValueImpl(object @object) : this(@object, false)
	  {
	  }

	  public UntypedValueImpl(object @object, bool isTransient)
	  {
		this.value = @object;
		this.isTransient = isTransient;
	  }

	  public virtual object Value
	  {
		  get
		  {
			return value;
		  }
	  }

	  public virtual ValueType Type
	  {
		  get
		  {
			// no type
			return null;
		  }
	  }

	  public override string ToString()
	  {
		return "Untyped value '" + value + "', isTransient = " + isTransient;
	  }

	  public override int GetHashCode()
	  {
		const int prime = 31;
		int result = 1;
		result = prime * result + ((value == null) ? 0 : value.GetHashCode());
		result = prime * result + (isTransient ? 1 : 0);
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
		UntypedValueImpl other = (UntypedValueImpl) obj;
		if (value == null)
		{
		  if (other.value != null)
		  {
			return false;
		  }
		}
		else if (!value.Equals(other.value))
		{
		  return false;
		}
		if (isTransient != other.Transient)
		{
		  return false;
		}
		return true;
	  }

	  public virtual bool Transient
	  {
		  get
		  {
			return isTransient;
		  }
		  set
		  {
			this.isTransient = value;
		  }
	  }


	}

}