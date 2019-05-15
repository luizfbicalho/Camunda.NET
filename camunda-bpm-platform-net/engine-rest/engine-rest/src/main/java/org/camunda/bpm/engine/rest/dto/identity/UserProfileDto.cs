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
namespace org.camunda.bpm.engine.rest.dto.identity
{
	using User = org.camunda.bpm.engine.identity.User;


	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class UserProfileDto
	{

	  protected internal string id;
	  protected internal string firstName;
	  protected internal string lastName;
	  protected internal string email;

	  // transformers ////////////////////////////////////////

	  public static UserProfileDto fromUser(User user)
	  {
		UserProfileDto result = new UserProfileDto();
		result.id = user.Id;
		result.firstName = user.FirstName;
		result.lastName = user.LastName;
		result.email = user.Email;
		return result;
	  }

	  public static IList<UserProfileDto> fromUserList(IList<User> sourceList)
	  {
		IList<UserProfileDto> resultList = new List<UserProfileDto>();
		foreach (User user in sourceList)
		{
		  resultList.Add(fromUser(user));
		}
		return resultList;
	  }

	  public virtual void update(User dbUser)
	  {
		dbUser.Id = Id;
		dbUser.FirstName = FirstName;
		dbUser.LastName = LastName;
		dbUser.Email = Email;
	  }

	  // getter / setters ////////////////////////////////////

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
		  set
		  {
			this.id = value;
		  }
	  }


	  public virtual string FirstName
	  {
		  get
		  {
			return firstName;
		  }
		  set
		  {
			this.firstName = value;
		  }
	  }


	  public virtual string LastName
	  {
		  get
		  {
			return lastName;
		  }
		  set
		  {
			this.lastName = value;
		  }
	  }


	  public virtual string Email
	  {
		  get
		  {
			return email;
		  }
		  set
		  {
			this.email = value;
		  }
	  }

	}

}