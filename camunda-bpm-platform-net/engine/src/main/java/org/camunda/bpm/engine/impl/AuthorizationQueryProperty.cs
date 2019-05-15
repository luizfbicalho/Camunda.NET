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
	using AuthorizationQuery = org.camunda.bpm.engine.authorization.AuthorizationQuery;
	using QueryProperty = org.camunda.bpm.engine.query.QueryProperty;

	/// <summary>
	/// Contains the possible properties that can be used in an <seealso cref="AuthorizationQuery"/>.
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public interface AuthorizationQueryProperty
	{
	}

	public static class AuthorizationQueryProperty_Fields
	{
	  public static readonly QueryProperty RESOURCE_TYPE = new QueryPropertyImpl("RESOURCE_TYPE_");
	  public static readonly QueryProperty RESOURCE_ID = new QueryPropertyImpl("RESOURCE_ID_");
	}

}