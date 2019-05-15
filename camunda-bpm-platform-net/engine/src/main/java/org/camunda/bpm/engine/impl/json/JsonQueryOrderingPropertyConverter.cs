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
	using JsonArray = com.google.gson.JsonArray;
	using JsonObject = com.google.gson.JsonObject;
	using QueryProperty = org.camunda.bpm.engine.query.QueryProperty;


	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class JsonQueryOrderingPropertyConverter : JsonObjectConverter<QueryOrderingProperty>
	{


	  protected internal static JsonQueryOrderingPropertyConverter INSTANCE = new JsonQueryOrderingPropertyConverter();

	  protected internal static JsonArrayConverter<IList<QueryOrderingProperty>> ARRAY_CONVERTER = new JsonArrayOfObjectsConverter<QueryOrderingProperty>(INSTANCE);

	  public const string RELATION = "relation";
	  public const string QUERY_PROPERTY = "queryProperty";
	  public const string QUERY_PROPERTY_FUNCTION = "queryPropertyFunction";
	  public const string DIRECTION = "direction";
	  public const string RELATION_CONDITIONS = "relationProperties";


	  public virtual JsonObject toJsonObject(QueryOrderingProperty property)
	  {
		JsonObject jsonObject = JsonUtil.createObject();

		JsonUtil.addField(jsonObject, RELATION, property.Relation);

		QueryProperty queryProperty = property.QueryProperty;
		if (queryProperty != null)
		{
		  JsonUtil.addField(jsonObject, QUERY_PROPERTY, queryProperty.Name);
		  JsonUtil.addField(jsonObject, QUERY_PROPERTY_FUNCTION, queryProperty.Function);
		}

		Direction direction = property.Direction;
		if (direction != null)
		{
		  JsonUtil.addField(jsonObject, DIRECTION, direction.Name);
		}

		if (property.hasRelationConditions())
		{
		  JsonArray relationConditionsJson = JsonQueryFilteringPropertyConverter.ARRAY_CONVERTER.toJsonArray(property.RelationConditions);
		  JsonUtil.addField(jsonObject, RELATION_CONDITIONS, relationConditionsJson);
		}

		return jsonObject;
	  }

	  public override QueryOrderingProperty toObject(JsonObject jsonObject)
	  {
		string relation = null;
		if (jsonObject.has(RELATION))
		{
		  relation = JsonUtil.getString(jsonObject, RELATION);
		}

		QueryOrderingProperty property = null;
		if (QueryOrderingProperty.RELATION_VARIABLE.Equals(relation))
		{
		  property = new VariableOrderProperty();
		}
		else
		{
		  property = new QueryOrderingProperty();
		}

		property.Relation = relation;

		if (jsonObject.has(QUERY_PROPERTY))
		{
		  string propertyName = JsonUtil.getString(jsonObject, QUERY_PROPERTY);
		  string propertyFunction = null;
		  if (jsonObject.has(QUERY_PROPERTY_FUNCTION))
		  {
			propertyFunction = JsonUtil.getString(jsonObject, QUERY_PROPERTY_FUNCTION);
		  }

		  QueryProperty queryProperty = new QueryPropertyImpl(propertyName, propertyFunction);
		  property.QueryProperty = queryProperty;
		}

		if (jsonObject.has(DIRECTION))
		{
		  string direction = JsonUtil.getString(jsonObject, DIRECTION);
		  property.Direction = Direction.findByName(direction);
		}

		if (jsonObject.has(RELATION_CONDITIONS))
		{
		  IList<QueryEntityRelationCondition> relationConditions = JsonQueryFilteringPropertyConverter.ARRAY_CONVERTER.toObject(JsonUtil.getArray(jsonObject, RELATION_CONDITIONS));
		  property.RelationConditions = relationConditions;
		}

		return property;
	  }
	}

}