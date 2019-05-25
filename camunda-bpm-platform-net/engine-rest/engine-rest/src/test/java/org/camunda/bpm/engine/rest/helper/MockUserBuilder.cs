﻿/*
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

	using User = org.camunda.bpm.engine.identity.User;

	public class MockUserBuilder
	{

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string id_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string firstName_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string lastName_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string email_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string password_Conflict;

	  public virtual MockUserBuilder id(string id)
	  {
		this.id_Conflict = id;
		return this;
	  }

	  public virtual MockUserBuilder firstName(string firstName)
	  {
		this.firstName_Conflict = firstName;
		return this;
	  }

	  public virtual MockUserBuilder lastName(string lastName)
	  {
		this.lastName_Conflict = lastName;
		return this;
	  }

	  public virtual MockUserBuilder email(string email)
	  {
		this.email_Conflict = email;
		return this;
	  }

	  public virtual MockUserBuilder password(string password)
	  {
		this.password_Conflict = password;
		return this;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public org.camunda.bpm.engine.identity.User build()
	  public virtual User build()
	  {
		User user = mock(typeof(User));
		when(user.Id).thenReturn(id_Conflict);
		when(user.FirstName).thenReturn(firstName_Conflict);
		when(user.LastName).thenReturn(lastName_Conflict);
		when(user.Email).thenReturn(email_Conflict);
		when(user.Password).thenReturn(password_Conflict);
		return user;
	  }

	}

}