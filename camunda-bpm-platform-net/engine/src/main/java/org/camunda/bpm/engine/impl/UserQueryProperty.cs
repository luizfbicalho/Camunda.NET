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
	using UserQuery = org.camunda.bpm.engine.identity.UserQuery;
	using QueryProperty = org.camunda.bpm.engine.query.QueryProperty;



	/// <summary>
	/// Contains the possible properties that can be used by the <seealso cref="UserQuery"/>.
	/// 
	/// @author Joram Barrez
	/// </summary>
	public interface UserQueryProperty
	{

	}

	public static class UserQueryProperty_Fields
	{
	  public static readonly QueryProperty USER_ID = new QueryPropertyImpl("ID_");
	  public static readonly QueryProperty FIRST_NAME = new QueryPropertyImpl("FIRST_");
	  public static readonly QueryProperty LAST_NAME = new QueryPropertyImpl("LAST_");
	  public static readonly QueryProperty EMAIL = new QueryPropertyImpl("EMAIL_");
	}

}