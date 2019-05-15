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
namespace org.camunda.bpm.engine.rest.hal.user
{
	using User = org.camunda.bpm.engine.identity.User;


	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class HalUserList : HalResource<HalUserList>
	{

	  public static HalUserList fromUserList(IList<User> users)
	  {
		HalUserList result = new HalUserList();

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.engine.rest.hal.HalResource<?>> halUsers = new java.util.ArrayList<org.camunda.bpm.engine.rest.hal.HalResource<?>>();
		IList<HalResource<object>> halUsers = new List<HalResource<object>>();
		foreach (User user in users)
		{
		  halUsers.Add(HalUser.fromUser(user));
		}

		// embedd the user list
		result.addEmbedded("users", halUsers);

		// self link
		result.addLink("self", UriBuilder.fromResource(typeof(UserRestService)).build());

		return result;
	  }

	}

}