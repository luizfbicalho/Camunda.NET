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

	/// <summary>
	/// Specifies a condition by which two entity types can be related.
	/// <code>comparisonProperty</code> and <code>scalarValue</code>
	/// are exclusive, i.e. one of the should be <code>null</code>.
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class QueryEntityRelationCondition
	{

	  protected internal QueryProperty property;
	  protected internal QueryProperty comparisonProperty;
	  protected internal object scalarValue;

	  public QueryEntityRelationCondition(QueryProperty queryProperty, object scalarValue) : this(queryProperty, null, scalarValue)
	  {
	  }

	  public QueryEntityRelationCondition(QueryProperty queryProperty, QueryProperty comparisonProperty) : this(queryProperty, comparisonProperty, null)
	  {
	  }

	  public QueryEntityRelationCondition(QueryProperty queryProperty, QueryProperty comparisonProperty, object scalarValue)
	  {
		this.property = queryProperty;
		this.comparisonProperty = comparisonProperty;
		this.scalarValue = scalarValue;
	  }

	  public virtual QueryProperty Property
	  {
		  get
		  {
			return property;
		  }
	  }

	  public virtual QueryProperty ComparisonProperty
	  {
		  get
		  {
			return comparisonProperty;
		  }
	  }

	  public virtual object ScalarValue
	  {
		  get
		  {
			return scalarValue;
		  }
	  }

	  /// <summary>
	  /// This assumes that scalarValue and comparisonProperty are mutually exclusive.
	  /// Either a condition is expressed is by a scalar value, or with a property of another entity.
	  /// </summary>
	  public virtual bool PropertyComparison
	  {
		  get
		  {
			return comparisonProperty != null;
		  }
	  }

	  public override string ToString()
	  {
		return "QueryEntityRelationCondition["
		  + "property=" + property + ", comparisonProperty=" + comparisonProperty + ", scalarValue=" + scalarValue + "]";
	  }
	}

}