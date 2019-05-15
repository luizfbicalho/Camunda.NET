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
namespace org.camunda.bpm.engine.rest.util
{

	using SerializableValueType = org.camunda.bpm.engine.variable.type.SerializableValueType;

	/// <summary>
	/// <para>Builds maps that fulfill the camunda variable json format.</para>
	/// <para>
	/// For example, if VariablesBuilder.variable("aKey", "aValue").variable("anotherKey", "anotherValue", "String").getVariables()
	/// a map is returned that is supposed to be mapped to JSON by rest-assured as follows:
	/// </para>
	/// <code>
	/// {
	///    "aKey" : {"value" : "aValue"},
	///    "anotherKey" : {"value" : "anotherValue", "type" : "String"}
	/// }
	/// </code>
	/// 
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class VariablesBuilder
	{

	  private IDictionary<string, object> variables;

	  private VariablesBuilder()
	  {
		variables = new Dictionary<string, object>();
	  }

	  public static VariablesBuilder create()
	  {
		VariablesBuilder builder = new VariablesBuilder();
		return builder;
	  }

	  public virtual VariablesBuilder variable(string name, object value, string type)
	  {
		IDictionary<string, object> variableValue = getVariableValueMap(value, type);
		variables[name] = variableValue;
		return this;
	  }

	  public virtual VariablesBuilder variable(string name, object value, string type, bool local)
	  {
		IDictionary<string, object> variableValue = getVariableValueMap(value, type, local);
		variables[name] = variableValue;
		return this;
	  }

	  public virtual VariablesBuilder variable(string name, object value)
	  {
		return variable(name, value, null);
	  }

	  public virtual VariablesBuilder variable(string name, object value, bool local)
	  {
		return variable(name, value, null, local);
	  }

	  public virtual VariablesBuilder variable(string name, string type, object value, string serializationFormat, string objectType)
	  {
		IDictionary<string, object> variableValue = getObjectValueMap(value, type, serializationFormat, objectType);
		variables[name] = variableValue;
		return this;
	  }

	  public virtual IDictionary<string, object> Variables
	  {
		  get
		  {
			return variables;
		  }
	  }

	  public static IDictionary<string, object> getVariableValueMap(object value)
	  {
		return getVariableValueMap(value, null);
	  }

	  public static IDictionary<string, object> getVariableValueMap(object value, bool local)
	  {
		return getVariableValueMap(value, null, local);
	  }

	  public static IDictionary<string, object> getVariableValueMap(object value, string type)
	  {
		return getVariableValueMap(value, type, false);
	  }

	  public static IDictionary<string, object> getVariableValueMap(object value, string type, bool local)
	  {
		IDictionary<string, object> variable = new Dictionary<string, object>();
		if (value != null)
		{
		  variable["value"] = value;
		}
		if (!string.ReferenceEquals(type, null))
		{
		  variable["type"] = type;
		}

		variable["local"] = local;

		return variable;
	  }

	  public static IDictionary<string, object> getObjectValueMap(object value, string variableType, string serializationFormat, string objectTypeName)
	  {
		IDictionary<string, object> serializedVariable = new Dictionary<string, object>();

		if (value != null)
		{
		  serializedVariable["value"] = value;
		}

		if (!string.ReferenceEquals(variableType, null))
		{
		  serializedVariable["type"] = variableType;
		}

		IDictionary<string, object> typeInfo = new Dictionary<string, object>();
		typeInfo[SerializableValueType.VALUE_INFO_SERIALIZATION_DATA_FORMAT] = serializationFormat;
		typeInfo[SerializableValueType.VALUE_INFO_OBJECT_TYPE_NAME] = objectTypeName;

		serializedVariable["valueInfo"] = typeInfo;

		return serializedVariable;
	  }

	  public virtual VariablesBuilder variableTransient(string name, string value, string type)
	  {
		IDictionary<string, object> valueMap = getVariableValueMap(value, type);
		IDictionary<string, object> valueInfo = new Dictionary<string, object>();
		valueInfo["transient"] = true;
		valueMap["valueInfo"] = valueInfo;
		variables[name] = valueMap;
		return this;
	  }

	}

}