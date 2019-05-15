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
namespace org.camunda.bpm.engine.rest.security.auth
{

	public class AuthenticationResult
	{

	  protected internal bool isAuthenticated;

	  protected internal string authenticatedUser;
	  protected internal IList<string> groups;
	  protected internal IList<string> tenants;

	  public AuthenticationResult(string authenticatedUser, bool isAuthenticated)
	  {
		this.authenticatedUser = authenticatedUser;
		this.isAuthenticated = isAuthenticated;
	  }

	  public virtual string AuthenticatedUser
	  {
		  get
		  {
			return authenticatedUser;
		  }
		  set
		  {
			this.authenticatedUser = value;
		  }
	  }


	  public virtual bool Authenticated
	  {
		  get
		  {
			return isAuthenticated;
		  }
		  set
		  {
			this.isAuthenticated = value;
		  }
	  }


	  public virtual IList<string> Groups
	  {
		  get
		  {
			return groups;
		  }
		  set
		  {
			this.groups = value;
		  }
	  }


	  public virtual IList<string> Tenants
	  {
		  get
		  {
			return tenants;
		  }
		  set
		  {
			this.tenants = value;
		  }
	  }


	  public static AuthenticationResult successful(string userId)
	  {
		return new AuthenticationResult(userId, true);
	  }

	  public static AuthenticationResult unsuccessful()
	  {
		return new AuthenticationResult(null, false);
	  }

	  public static AuthenticationResult unsuccessful(string userId)
	  {
		return new AuthenticationResult(userId, false);
	  }
	}

}