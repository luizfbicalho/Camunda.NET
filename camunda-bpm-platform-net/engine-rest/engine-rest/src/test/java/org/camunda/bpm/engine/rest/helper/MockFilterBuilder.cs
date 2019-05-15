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
namespace org.camunda.bpm.engine.rest.helper
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;

	using Filter = org.camunda.bpm.engine.filter.Filter;
	using Query = org.camunda.bpm.engine.query.Query;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class MockFilterBuilder
	{

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string id_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string resourceType_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string name_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string owner_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal Query query_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal IDictionary<string, object> properties_Renamed;

	  public virtual MockFilterBuilder id(string id)
	  {
		this.id_Renamed = id;
		return this;
	  }

	  public virtual MockFilterBuilder resourceType(string resourceType)
	  {
		this.resourceType_Renamed = resourceType;
		return this;
	  }

	  public virtual MockFilterBuilder name(string name)
	  {
		this.name_Renamed = name;
		return this;
	  }

	  public virtual MockFilterBuilder owner(string owner)
	  {
		this.owner_Renamed = owner;
		return this;
	  }

	  public virtual MockFilterBuilder query<T1>(Query<T1> query)
	  {
		this.query_Renamed = query;
		return this;
	  }

	  public virtual MockFilterBuilder properties(IDictionary<string, object> properties)
	  {
		this.properties_Renamed = properties;
		return this;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public org.camunda.bpm.engine.filter.Filter build()
	  public virtual Filter build()
	  {
		Filter filter = mock(typeof(Filter));
		when(filter.Id).thenReturn(id_Renamed);
		when(filter.ResourceType).thenReturn(resourceType_Renamed);
		when(filter.Name).thenReturn(name_Renamed);
		when(filter.Owner).thenReturn(owner_Renamed);
		when(filter.Query).thenReturn(query_Renamed);
		when(filter.Properties).thenReturn(properties_Renamed);
		return filter;
	  }

	}

}