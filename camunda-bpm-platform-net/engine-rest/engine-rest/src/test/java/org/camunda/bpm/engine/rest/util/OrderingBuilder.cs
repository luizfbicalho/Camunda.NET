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

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class OrderingBuilder
	{

	  protected internal IList<IDictionary<string, object>> orderings = new List<IDictionary<string, object>>();
	  protected internal IDictionary<string, object> currentOrdering;

	  public static OrderingBuilder create()
	  {
		return new OrderingBuilder();
	  }

	  public virtual OrderingBuilder orderBy(string property)
	  {
		currentOrdering = new Dictionary<string, object>();
		orderings.Add(currentOrdering);
		currentOrdering["sortBy"] = property;
		return this;
	  }

	  public virtual OrderingBuilder desc()
	  {
		currentOrdering["sortOrder"] = "desc";
		return this;
	  }

	  public virtual OrderingBuilder asc()
	  {
		currentOrdering["sortOrder"] = "asc";
		return this;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public OrderingBuilder parameter(String key, Object value)
	  public virtual OrderingBuilder parameter(string key, object value)
	  {
		IDictionary<string, object> parameters = (IDictionary<string, object>) currentOrdering["parameters"];

		if (parameters == null)
		{
		  parameters = new Dictionary<string, object>();
		  currentOrdering["parameters"] = parameters;
		}

		parameters[key] = value;
		return this;
	  }

	  public virtual IList<IDictionary<string, object>> Json
	  {
		  get
		  {
			return orderings;
		  }
	  }
	}

}