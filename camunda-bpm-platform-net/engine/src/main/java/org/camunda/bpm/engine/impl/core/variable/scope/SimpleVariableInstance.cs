﻿/*
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
namespace org.camunda.bpm.engine.impl.core.variable.scope
{
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Daniel Meyer
	/// @author Roman Smirnov
	/// @author Sebastian Menski
	/// 
	/// </summary>
	public class SimpleVariableInstance : CoreVariableInstance
	{

	  protected internal string name;
	  protected internal TypedValue value;

	  public SimpleVariableInstance(string name, TypedValue value)
	  {
		this.name = name;
		this.value = value;
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
	  public virtual TypedValue getTypedValue(bool deserialize)
	  {
		return value;
	  }
	  public virtual TypedValue Value
	  {
		  set
		  {
			this.value = value;
		  }
	  }

	  public class SimpleVariableInstanceFactory : VariableInstanceFactory<SimpleVariableInstance>
	  {

		public static readonly SimpleVariableInstanceFactory INSTANCE = new SimpleVariableInstanceFactory();

		public virtual SimpleVariableInstance build(string name, TypedValue value, bool isTransient)
		{
		  return new SimpleVariableInstance(name, value);
		}

	  }

	}

}