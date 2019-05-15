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
namespace org.camunda.bpm.engine.test.api.runtime.util
{

	public class VariableSpec
	{

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string name_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal object value_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string variableTypeName_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string valueTypeName_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal object serializedValue_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool? storesCustomObjects_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal IDictionary<string, object> configuration_Renamed;

	  public virtual string Name
	  {
		  get
		  {
			return name_Renamed;
		  }
	  }
	  public virtual VariableSpec name(string name)
	  {
		this.name_Renamed = name;
		return this;
	  }
	  public virtual object Value
	  {
		  get
		  {
			return value_Renamed;
		  }
	  }
	  public virtual VariableSpec value(object value)
	  {
		this.value_Renamed = value;
		return this;
	  }
	  public virtual string VariableTypeName
	  {
		  get
		  {
			return variableTypeName_Renamed;
		  }
	  }
	  public virtual VariableSpec variableTypeName(string variableTypeName)
	  {
		this.variableTypeName_Renamed = variableTypeName;
		return this;
	  }
	  public virtual string ValueTypeName
	  {
		  get
		  {
			return valueTypeName_Renamed;
		  }
	  }
	  public virtual VariableSpec valueTypeName(string valueTypeName)
	  {
		this.valueTypeName_Renamed = valueTypeName;
		return this;
	  }
	  public virtual object SerializedValue
	  {
		  get
		  {
			return serializedValue_Renamed;
		  }
	  }
	  public virtual VariableSpec serializedValue(object serializedValue)
	  {
		this.serializedValue_Renamed = serializedValue;
		return this;
	  }
	  public virtual IDictionary<string, object> Configuration
	  {
		  get
		  {
			return configuration_Renamed;
		  }
	  }
	  public virtual VariableSpec configuration(IDictionary<string, object> configuration)
	  {
		this.configuration_Renamed = configuration;
		return this;
	  }
	  public virtual bool StoresCustomObjects
	  {
		  get
		  {
			return storesCustomObjects_Renamed.Value;
		  }
	  }
	  public virtual VariableSpec storesCustomObjects(bool storesCustomObjects)
	  {
		this.storesCustomObjects_Renamed = storesCustomObjects;
		return this;
	  }


	}

}