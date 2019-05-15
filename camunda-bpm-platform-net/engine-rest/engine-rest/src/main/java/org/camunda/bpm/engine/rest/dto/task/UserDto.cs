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
namespace org.camunda.bpm.engine.rest.dto.task
{
	/// <summary>
	/// @author: drobisch
	/// </summary>
	public class UserDto
	{
	  private string firstName;
	  private string lastName;
	  private string displayName;

	  private string id;

	  public UserDto(string id, string firstName, string lastName)
	  {
		this.id = id;
		this.firstName = firstName;
		this.lastName = lastName;

		if (string.ReferenceEquals(firstName, null) && string.ReferenceEquals(lastName, null))
		{
		  this.displayName = id;
		}
		else
		{
		  this.displayName = (!string.ReferenceEquals(lastName, null)) ? firstName + " " + lastName : firstName;
		}
	  }

	  public virtual string FirstName
	  {
		  get
		  {
			return firstName;
		  }
	  }

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual string LastName
	  {
		  get
		  {
			return lastName;
		  }
	  }

	  public virtual string DisplayName
	  {
		  get
		  {
			return displayName;
		  }
	  }

	  public override bool Equals(object o)
	  {
		if (this == o)
		{
			return true;
		}
		if (o == null || this.GetType() != o.GetType())
		{
			return false;
		}

		UserDto userDto = (UserDto) o;

		if (!string.ReferenceEquals(firstName, null) ?!firstName.Equals(userDto.firstName) :!string.ReferenceEquals(userDto.firstName, null))
		{
			return false;
		}
		if (!string.ReferenceEquals(id, null) ?!id.Equals(userDto.id) :!string.ReferenceEquals(userDto.id, null))
		{
			return false;
		}
		if (!string.ReferenceEquals(lastName, null) ?!lastName.Equals(userDto.lastName) :!string.ReferenceEquals(userDto.lastName, null))
		{
			return false;
		}

		return true;
	  }

	  public override int GetHashCode()
	  {
		int result = !string.ReferenceEquals(firstName, null) ? firstName.GetHashCode() : 0;
		result = 31 * result + (!string.ReferenceEquals(lastName, null) ? lastName.GetHashCode() : 0);
		result = 31 * result + (!string.ReferenceEquals(id, null) ? id.GetHashCode() : 0);
		return result;
	  }
	}

}