using System;
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
namespace org.camunda.bpm.engine.impl
{

	using QueryProperty = org.camunda.bpm.engine.query.QueryProperty;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;

	/// <summary>
	/// <para>A QueryOrderingProperty specifies a condition by which the results of a query should be
	/// sorted. It can either specify a sorting by a property of the entities to be selected or
	/// a sorting by a property of a related entity. For example in a <seealso cref="TaskQuery"/>,
	/// the entity to be selected is <seealso cref="Task"/> while a related entity could be a
	/// <seealso cref="VariableInstance"/>.</para>
	/// 
	/// <para>It is made up of the following:</para>
	/// 
	/// <para>
	/// <dl>
	///   <dt>relation</dt>
	///     <dd>A symbolic name that identifies a related entity. <code>null</code> if
	///     an ordering over a property of the entity to be selected is expressed.</dd>
	///   <dt>queryProperty</dt>
	///     <dd>The property to be sorted on. An instance of <seealso cref="QueryProperty"/>.</dd>
	///   <dt>direction</dt>
	///     <dd>The ordering direction, refer to <seealso cref="Direction"/></dd>
	///   <dt>relationConditions</dt>
	///     <dd>A list of constraints that describe the nature of the relation to another entity
	///     (or in SQL terms, the joining conditions). Is <code>null</code> if relation
	///     is <code>null</code>. Contains instances of <seealso cref="QueryEntityRelationCondition"/>.</dd>
	/// <dl>
	/// </para>
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	[Serializable]
	public class QueryOrderingProperty
	{

	  public const string RELATION_VARIABLE = "variable";
	  public const string RELATION_PROCESS_DEFINITION = "process-definition";
	  public const string RELATION_CASE_DEFINITION = "case-definition";

	  protected internal const long serialVersionUID = 1L;

	  protected internal string relation;
	  protected internal QueryProperty queryProperty;
	  protected internal Direction direction;
	  protected internal IList<QueryEntityRelationCondition> relationConditions;

	  public QueryOrderingProperty()
	  {
	  }

	  public QueryOrderingProperty(QueryProperty queryProperty, Direction direction)
	  {
		this.queryProperty = queryProperty;
		this.direction = direction;
	  }

	  public QueryOrderingProperty(string relation, QueryProperty queryProperty)
	  {
		this.relation = relation;
		this.queryProperty = queryProperty;
	  }

	  public virtual QueryProperty QueryProperty
	  {
		  get
		  {
			return queryProperty;
		  }
		  set
		  {
			this.queryProperty = value;
		  }
	  }


	  public virtual Direction Direction
	  {
		  set
		  {
			this.direction = value;
		  }
		  get
		  {
			return direction;
		  }
	  }


	  public virtual IList<QueryEntityRelationCondition> RelationConditions
	  {
		  get
		  {
			return relationConditions;
		  }
		  set
		  {
			this.relationConditions = value;
		  }
	  }


	  public virtual bool hasRelationConditions()
	  {
		return relationConditions != null && relationConditions.Count > 0;
	  }

	  public virtual string Relation
	  {
		  get
		  {
			return relation;
		  }
		  set
		  {
			this.relation = value;
		  }
	  }


	  /// <returns> whether this ordering property is contained in the default fields
	  /// of the base entity (e.g. task.NAME_ is a contained property; LOWER(task.NAME_) or
	  /// variable.TEXT_ (given a task query) is not contained) </returns>
	  public virtual bool ContainedProperty
	  {
		  get
		  {
			return string.ReferenceEquals(relation, null) && string.ReferenceEquals(queryProperty.Function, null);
		  }
	  }

	  public override string ToString()
	  {

		return "QueryOrderingProperty["
		  + "relation=" + relation + ", queryProperty=" + queryProperty + ", direction=" + direction + ", relationConditions=" + RelationConditionsString + "]";
	  }

	  public virtual string RelationConditionsString
	  {
		  get
		  {
			StringBuilder builder = new StringBuilder();
			builder.Append("[");
			if (relationConditions != null)
			{
			  for (int i = 0; i < relationConditions.Count; i++)
			  {
				if (i > 0)
				{
				  builder.Append(",");
				}
				builder.Append(relationConditions[i]);
			  }
			}
			builder.Append("]");
			return builder.ToString();
		  }
	  }

	}

}