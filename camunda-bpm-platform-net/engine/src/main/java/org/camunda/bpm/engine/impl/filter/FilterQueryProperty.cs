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
namespace org.camunda.bpm.engine.impl.filter
{
	using QueryProperty = org.camunda.bpm.engine.query.QueryProperty;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public interface FilterQueryProperty
	{

	}

	public static class FilterQueryProperty_Fields
	{
	  public static readonly QueryProperty FILTER_ID = new QueryPropertyImpl("ID_");
	  public static readonly QueryProperty RESOURCE_TYPE = new QueryPropertyImpl("RESOURCE_TYPE_");
	  public static readonly QueryProperty NAME = new QueryPropertyImpl("NAME_");
	  public static readonly QueryProperty OWNER = new QueryPropertyImpl("OWNER_");
	  public static readonly QueryProperty QUERY = new QueryPropertyImpl("QUERY_");
	  public static readonly QueryProperty PROPERTIES = new QueryPropertyImpl("PROPERTIES_");
	}

}