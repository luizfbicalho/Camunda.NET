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

	using JsonUtil = org.camunda.bpm.engine.impl.util.JsonUtil;
	using JsonObject = com.google.gson.JsonObject;
	using QueryProperty = org.camunda.bpm.engine.query.QueryProperty;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class JsonQueryFilteringPropertyConverter : JsonObjectConverter<QueryEntityRelationCondition>
	{

	  protected internal static JsonQueryFilteringPropertyConverter INSTANCE = new JsonQueryFilteringPropertyConverter();

	  protected internal static JsonArrayConverter<IList<QueryEntityRelationCondition>> ARRAY_CONVERTER = new JsonArrayOfObjectsConverter<IList<QueryEntityRelationCondition>>(INSTANCE);

	  public const string BASE_PROPERTY = "baseField";
	  public const string COMPARISON_PROPERTY = "comparisonField";
	  public const string SCALAR_VALUE = "value";

	  public virtual JsonObject toJsonObject(QueryEntityRelationCondition filteringProperty)
	  {
		JsonObject jsonObject = JsonUtil.createObject();

		JsonUtil.addField(jsonObject, BASE_PROPERTY, filteringProperty.Property.Name);

		QueryProperty comparisonProperty = filteringProperty.ComparisonProperty;
		if (comparisonProperty != null)
		{
		  JsonUtil.addField(jsonObject, COMPARISON_PROPERTY, comparisonProperty.Name);
		}

		object scalarValue = filteringProperty.ScalarValue;
		if (scalarValue != null)
		{
		  JsonUtil.addFieldRawValue(jsonObject, SCALAR_VALUE, scalarValue);
		}

		return jsonObject;
	  }

	  public override QueryEntityRelationCondition toObject(JsonObject jsonObject)
	  {
		// this is limited in that it allows only String values;
		// that is sufficient for current use case with task filters
		// but could be extended by a data type in the future
		string scalarValue = null;
		if (jsonObject.has(SCALAR_VALUE))
		{
		  scalarValue = JsonUtil.getString(jsonObject, SCALAR_VALUE);
		}

		QueryProperty baseProperty = null;
		if (jsonObject.has(BASE_PROPERTY))
		{
		  baseProperty = new QueryPropertyImpl(JsonUtil.getString(jsonObject, BASE_PROPERTY));
		}

		QueryProperty comparisonProperty = null;
		if (jsonObject.has(COMPARISON_PROPERTY))
		{
		  comparisonProperty = new QueryPropertyImpl(JsonUtil.getString(jsonObject, COMPARISON_PROPERTY));
		}

		return new QueryEntityRelationCondition(baseProperty, comparisonProperty, scalarValue);
	  }

	}

}