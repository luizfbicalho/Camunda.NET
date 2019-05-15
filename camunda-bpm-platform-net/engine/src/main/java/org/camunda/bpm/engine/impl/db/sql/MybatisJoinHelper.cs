using System.Collections.Generic;
using System.Text;

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
namespace org.camunda.bpm.engine.impl.db.sql
{

	using QueryProperty = org.camunda.bpm.engine.query.QueryProperty;

	/// <summary>
	/// @author Thorben Lindhauer
	/// </summary>
	public class MybatisJoinHelper
	{

	  protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;
	  protected internal const string DEFAULT_ORDER = "RES.ID_ asc";
	  public static IDictionary<string, MyBatisTableMapping> mappings = new Dictionary<string, MyBatisTableMapping>();

	  static MybatisJoinHelper()
	  {
		mappings[QueryOrderingProperty.RELATION_VARIABLE] = new VariableTableMapping();
		mappings[QueryOrderingProperty.RELATION_PROCESS_DEFINITION] = new ProcessDefinitionTableMapping();
		mappings[QueryOrderingProperty.RELATION_CASE_DEFINITION] = new CaseDefinitionTableMapping();
	  }

	  public static string tableAlias(string relation, int index)
	  {
		if (string.ReferenceEquals(relation, null))
		{
		  return "RES";
		}
		else
		{
		  MyBatisTableMapping mapping = getTableMapping(relation);

		  if (mapping.OneToOneRelation)
		  {
			return mapping.TableAlias;
		  }
		  else
		  {
			return mapping.TableAlias + index;
		  }
		}
	  }

	  public static string tableMapping(string relation)
	  {
		MyBatisTableMapping mapping = getTableMapping(relation);

		return mapping.TableName;
	  }

	  public static string orderBySelection(QueryOrderingProperty orderingProperty, int index)
	  {
		QueryProperty queryProperty = orderingProperty.QueryProperty;

		StringBuilder sb = new StringBuilder();

		if (!string.ReferenceEquals(queryProperty.Function, null))
		{
		  sb.Append(queryProperty.Function);
		  sb.Append("(");
		}

		sb.Append(tableAlias(orderingProperty.Relation, index));
		sb.Append(".");
		sb.Append(queryProperty.Name);

		if (!string.ReferenceEquals(queryProperty.Function, null))
		{
		  sb.Append(")");
		}

		return sb.ToString();
	  }

	  public static string orderBy(QueryOrderingProperty orderingProperty, int index)
	  {
		QueryProperty queryProperty = orderingProperty.QueryProperty;

		StringBuilder sb = new StringBuilder();

		sb.Append(tableAlias(orderingProperty.Relation, index));
		if (orderingProperty.ContainedProperty)
		{
		  sb.Append(".");
		}
		else
		{
		  sb.Append("_");
		}
		sb.Append(queryProperty.Name);

		sb.Append(" ");

		sb.Append(orderingProperty.Direction.Name);

		return sb.ToString();

	  }

	  protected internal static MyBatisTableMapping getTableMapping(string relation)
	  {
		MyBatisTableMapping mapping = mappings[relation];

		if (mapping == null)
		{
		  throw LOG.missingRelationMappingException(relation);
		}

		return mapping;
	  }
	}

}