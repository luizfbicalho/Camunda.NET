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
	public class HalUser : HalResource<HalUser>, HalIdResource
	{

	  public static readonly HalRelation REL_SELF = HalRelation.build("self", typeof(UserRestService), UriBuilder.fromPath(org.camunda.bpm.engine.rest.UserRestService_Fields.PATH).path("{id}"));

	  protected internal string id;
	  protected internal string firstName;
	  protected internal string lastName;
	  protected internal string email;


	  public static HalUser fromUser(User user)
	  {

		HalUser halUser = new HalUser();

		halUser.id = user.Id;
		halUser.firstName = user.FirstName;
		halUser.lastName = user.LastName;
		halUser.email = user.Email;

		halUser.linker.createLink(REL_SELF, user.Id);

		return halUser;

	  }

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual string FirstName
	  {
		  get
		  {
			return firstName;
		  }
	  }

	  public virtual string LastName
	  {
		  get
		  {
			return lastName;
		  }
	  }

	  public virtual string Email
	  {
		  get
		  {
			return email;
		  }
	  }

	}

}