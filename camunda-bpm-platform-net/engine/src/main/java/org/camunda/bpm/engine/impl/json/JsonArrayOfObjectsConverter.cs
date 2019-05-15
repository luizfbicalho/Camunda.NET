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
namespace org.camunda.bpm.engine.impl.json
{

	using JsonElement = com.google.gson.JsonElement;
	using JsonArray = com.google.gson.JsonArray;
	using JsonUtil = org.camunda.bpm.engine.impl.util.JsonUtil;

	/// <summary>
	/// @author Thorben Lindhauer
	/// </summary>
	public class JsonArrayOfObjectsConverter<T> : JsonArrayConverter<IList<T>>
	{

	  protected internal JsonObjectConverter<T> objectConverter;

	  public JsonArrayOfObjectsConverter(JsonObjectConverter<T> objectConverter)
	  {
		this.objectConverter = objectConverter;
	  }

	  public virtual JsonArray toJsonArray(IList<T> objects)
	  {
		JsonArray jsonArray = JsonUtil.createArray();

		foreach (T @object in objects)
		{
		  JsonElement jsonObject = objectConverter.toJsonObject(@object);
		  jsonArray.add(jsonObject);
		}

		return jsonArray;
	  }

	  public override IList<T> toObject(JsonArray jsonArray)
	  {
		IList<T> result = new List<T>();
		foreach (JsonElement jsonElement in jsonArray)
		{
		  T @object = objectConverter.toObject(JsonUtil.getObject(jsonElement));
		  result.Add(@object);
		}

		return result;
	  }
	}

}