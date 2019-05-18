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
	/// @author Daniel Meyer
	/// 
	/// </summary>
	[Serializable]
	public class AbstractTypedValue<T> : TypedValue
	{

	  private const long serialVersionUID = 1L;

	  protected internal T value;

	  protected internal ValueType type;

	  protected internal bool isTransient;

	  public AbstractTypedValue(T value, ValueType type)
	  {
		this.value = value;
		this.type = type;
	  }

	  public virtual T Value
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
			return type;
		  }
	  }

	  public override string ToString()
	  {
		return "Value '" + value + "' of type '" + type + "', isTransient=" + isTransient;
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