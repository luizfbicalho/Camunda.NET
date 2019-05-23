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
namespace org.camunda.bpm.engine.impl.identity
{


	/// <summary>
	/// <para>Allows to expose the id of the currently authenticated user,
	/// his groups and his tenants to the process engine.</para>
	/// 
	/// <para>The current authentication is managed using a Thread Local. The value can
	/// be set using <seealso cref="setCurrentAuthentication(string, System.Collections.IList)"/>,
	/// retrieved using <seealso cref="getCurrentAuthentication()"/> and cleared
	/// using <seealso cref="clearCurrentAuthentication()"/>.</para>
	/// 
	/// <para>Users typically do not use this class directly but rather use
	/// the corresponding Service API methods:
	/// <ul>
	/// <li></li>
	/// </ul>
	/// </para>
	/// 
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// </summary>
	public class Authentication
	{

	  protected internal string authenticatedUserId;
	  protected internal IList<string> authenticatedGroupIds;
	  protected internal IList<string> authenticatedTenantIds;

	  public Authentication()
	  {
	  }

	  public Authentication(string authenticatedUserId, IList<string> groupIds) : this(authenticatedUserId, groupIds, null)
	  {
	  }

	  public Authentication(string authenticatedUserId, IList<string> authenticatedGroupIds, IList<string> authenticatedTenantIds)
	  {
		this.authenticatedUserId = authenticatedUserId;

		if (authenticatedGroupIds != null)
		{
		  this.authenticatedGroupIds = new List<string>(authenticatedGroupIds);
		}

		if (authenticatedTenantIds != null)
		{
		  this.authenticatedTenantIds = new List<string>(authenticatedTenantIds);
		}
	  }

	  public virtual IList<string> GroupIds
	  {
		  get
		  {
			return authenticatedGroupIds;
		  }
	  }

	  public virtual string UserId
	  {
		  get
		  {
			return authenticatedUserId;
		  }
	  }

	  public virtual IList<string> TenantIds
	  {
		  get
		  {
			return authenticatedTenantIds;
		  }
	  }

	}

}