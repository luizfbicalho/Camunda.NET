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
	public class UserDto
	{

	  protected internal UserProfileDto profile;

	  protected internal UserCredentialsDto credentials;

	  // transformers //////////////////////////////////

	  public static UserDto fromUser(User user, bool isIncludeCredentials)
	  {
		UserDto userDto = new UserDto();
		userDto.Profile = UserProfileDto.fromUser(user);
		if (isIncludeCredentials)
		{
		  userDto.Credentials = UserCredentialsDto.fromUser(user);
		}
		return userDto;
	  }

	  // getters / setters /////////////////////////////

	  public virtual UserProfileDto Profile
	  {
		  get
		  {
			return profile;
		  }
		  set
		  {
			this.profile = value;
		  }
	  }


	  public virtual UserCredentialsDto Credentials
	  {
		  get
		  {
			return credentials;
		  }
		  set
		  {
			this.credentials = value;
		  }
	  }


	}

}