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
namespace org.camunda.bpm.engine.impl.json
{

	using QueryProperty = org.camunda.bpm.engine.query.QueryProperty;


	/// <summary>
	/// Deserializes query ordering properties from the deprecated 7.2 format in which
	/// the SQL-like orderBy parameter was used.
	/// 
	/// Is able to deserialize strings like:
	/// 
	/// <ul>
	///   <li>RES.ID_ asc</li>
	///   <li>LOWER(RES.NAME_) desc</li>
	///   <li>RES.ID_ asc, RES.NAME_ desc</li>
	/// </ul>
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class JsonLegacyQueryOrderingPropertyConverter
	{

	  public const string ORDER_BY_DELIMITER = ",";

	  public static JsonLegacyQueryOrderingPropertyConverter INSTANCE = new JsonLegacyQueryOrderingPropertyConverter();

	  public virtual IList<QueryOrderingProperty> fromOrderByString(string orderByString)
	  {
		IList<QueryOrderingProperty> properties = new List<QueryOrderingProperty>();

		string[] orderByClauses = orderByString.Split(ORDER_BY_DELIMITER, true);

		foreach (string orderByClause in orderByClauses)
		{
		  orderByClause = orderByClause.Trim();
		  string[] clauseParts = orderByClause.Split(" ", true);

		  if (clauseParts.Length == 0)
		  {
			continue;
		  }
		  else if (clauseParts.Length > 2)
		  {
			throw new ProcessEngineException("Invalid order by clause: " + orderByClause);
		  }

		  string function = null;

		  string propertyPart = clauseParts[0];

		  int functionArgumentBegin = propertyPart.IndexOf("(", StringComparison.Ordinal);
		  if (functionArgumentBegin >= 0)
		  {
			function = propertyPart.Substring(0, functionArgumentBegin);
			int functionArgumentEnd = propertyPart.IndexOf(")", StringComparison.Ordinal);

			propertyPart = propertyPart.Substring(functionArgumentBegin + 1, functionArgumentEnd - (functionArgumentBegin + 1));
		  }

		  string[] propertyParts = propertyPart.Split("\\.", true);

		  string property = null;
		  if (propertyParts.Length == 1)
		  {
			property = propertyParts[0];
		  }
		  else if (propertyParts.Length == 2)
		  {
			property = propertyParts[1];
		  }
		  else
		  {
			throw new ProcessEngineException("Invalid order by property part: " + clauseParts[0]);
		  }

		  QueryProperty queryProperty = new QueryPropertyImpl(property, function);

		  Direction direction = null;
		  if (clauseParts.Length == 2)
		  {
			string directionPart = clauseParts[1];
			direction = Direction.findByName(directionPart);
		  }

		  QueryOrderingProperty orderingProperty = new QueryOrderingProperty(null, queryProperty);
		  orderingProperty.Direction = direction;
		  properties.Add(orderingProperty);
		}

		return properties;
	  }

	}

}