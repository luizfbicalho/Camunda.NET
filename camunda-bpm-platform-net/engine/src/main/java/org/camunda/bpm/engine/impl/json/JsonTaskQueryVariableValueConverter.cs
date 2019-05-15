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
namespace org.camunda.bpm.engine.impl.json
{
	using JsonObject = com.google.gson.JsonObject;
	using JsonUtil = org.camunda.bpm.engine.impl.util.JsonUtil;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class JsonTaskQueryVariableValueConverter : JsonObjectConverter<TaskQueryVariableValue>
	{

	  public virtual JsonObject toJsonObject(TaskQueryVariableValue variable)
	  {
		JsonObject jsonObject = JsonUtil.createObject();
		JsonUtil.addField(jsonObject, "name", variable.Name);
		JsonUtil.addFieldRawValue(jsonObject, "value", variable.Value);
		JsonUtil.addField(jsonObject, "operator", variable.Operator.name());
		return jsonObject;
	  }

	  public override TaskQueryVariableValue toObject(JsonObject json)
	  {
		string name = JsonUtil.getString(json, "name");
		object value = JsonUtil.getRawObject(json, "value");
		QueryOperator @operator = Enum.Parse(typeof(QueryOperator), JsonUtil.getString(json, "operator"));
		bool isTaskVariable = JsonUtil.getBoolean(json, "taskVariable");
		bool isProcessVariable = JsonUtil.getBoolean(json, "processVariable");
		return new TaskQueryVariableValue(name, value, @operator, isTaskVariable, isProcessVariable);
	  }
	}

}