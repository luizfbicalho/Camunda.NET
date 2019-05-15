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
namespace org.camunda.bpm.identity.impl.ldap
{



	/// <summary>
	/// <para>Java Bean holding LDAP configuration</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class LdapConfiguration
	{

	  protected internal string initialContextFactory = "com.sun.jndi.ldap.LdapCtxFactory";
	  protected internal string securityAuthentication = "simple";

	  protected internal IDictionary<string, string> contextProperties = new Dictionary<string, string>();

	  protected internal string serverUrl;
	  protected internal string managerDn = "";
	  protected internal string managerPassword = "";

	  protected internal string baseDn = "";

	  protected internal string userDnPattern = "";

	  protected internal string userSearchBase = "";
	  protected internal string userSearchFilter = "(objectclass=person)";

	  protected internal string groupSearchBase = "";
	  protected internal string groupSearchFilter = "(objectclass=groupOfNames)";

	  protected internal string userIdAttribute = "uid";
	  protected internal string userFirstnameAttribute = "cn";
	  protected internal string userLastnameAttribute = "sn";
	  protected internal string userEmailAttribute = "email";
	  protected internal string userPasswordAttribute = "userpassword";

	  protected internal string groupIdAttribute = "ou";
	  protected internal string groupNameAttribute = "cn";
	  protected internal string groupTypeAttribute = "";
	  protected internal string groupMemberAttribute = "memberOf";

	  protected internal bool sortControlSupported = false;
	  protected internal bool useSsl = false;
	  protected internal bool usePosixGroups = false;
	  protected internal bool allowAnonymousLogin = false;

	  protected internal bool authorizationCheckEnabled = true;

	  // getters / setters //////////////////////////////////////

	  public virtual string InitialContextFactory
	  {
		  get
		  {
			return initialContextFactory;
		  }
		  set
		  {
			this.initialContextFactory = value;
		  }
	  }


	  public virtual string SecurityAuthentication
	  {
		  get
		  {
			return securityAuthentication;
		  }
		  set
		  {
			this.securityAuthentication = value;
		  }
	  }


	  public virtual IDictionary<string, string> ContextProperties
	  {
		  get
		  {
			return contextProperties;
		  }
		  set
		  {
			this.contextProperties = value;
		  }
	  }


	  public virtual string ServerUrl
	  {
		  get
		  {
			return serverUrl;
		  }
		  set
		  {
			this.serverUrl = value;
		  }
	  }


	  public virtual string ManagerDn
	  {
		  get
		  {
			return managerDn;
		  }
		  set
		  {
			this.managerDn = value;
		  }
	  }


	  public virtual string ManagerPassword
	  {
		  get
		  {
			return managerPassword;
		  }
		  set
		  {
			this.managerPassword = value;
		  }
	  }


	  public virtual string UserDnPattern
	  {
		  get
		  {
			return userDnPattern;
		  }
		  set
		  {
			this.userDnPattern = value;
		  }
	  }


	  public virtual string GroupSearchBase
	  {
		  get
		  {
			return groupSearchBase;
		  }
		  set
		  {
			this.groupSearchBase = value;
		  }
	  }


	  public virtual string GroupSearchFilter
	  {
		  get
		  {
			return groupSearchFilter;
		  }
		  set
		  {
			this.groupSearchFilter = value;
		  }
	  }


	  public virtual string GroupNameAttribute
	  {
		  get
		  {
			return groupNameAttribute;
		  }
		  set
		  {
			this.groupNameAttribute = value;
		  }
	  }


	  public virtual string BaseDn
	  {
		  get
		  {
			return baseDn;
		  }
		  set
		  {
			this.baseDn = value;
		  }
	  }


	  public virtual string UserSearchBase
	  {
		  get
		  {
			return userSearchBase;
		  }
		  set
		  {
			this.userSearchBase = value;
		  }
	  }


	  public virtual string UserSearchFilter
	  {
		  get
		  {
			return userSearchFilter;
		  }
		  set
		  {
			this.userSearchFilter = value;
		  }
	  }


	  public virtual string UserIdAttribute
	  {
		  get
		  {
			return userIdAttribute;
		  }
		  set
		  {
			this.userIdAttribute = value;
		  }
	  }


	  public virtual string UserFirstnameAttribute
	  {
		  get
		  {
			return userFirstnameAttribute;
		  }
		  set
		  {
			this.userFirstnameAttribute = value;
		  }
	  }


	  public virtual string UserLastnameAttribute
	  {
		  get
		  {
			return userLastnameAttribute;
		  }
		  set
		  {
			this.userLastnameAttribute = value;
		  }
	  }


	  public virtual string UserEmailAttribute
	  {
		  get
		  {
			return userEmailAttribute;
		  }
		  set
		  {
			this.userEmailAttribute = value;
		  }
	  }


	  public virtual string UserPasswordAttribute
	  {
		  get
		  {
			return userPasswordAttribute;
		  }
		  set
		  {
			this.userPasswordAttribute = value;
		  }
	  }


	  public virtual bool SortControlSupported
	  {
		  get
		  {
			return sortControlSupported;
		  }
		  set
		  {
			this.sortControlSupported = value;
		  }
	  }


	  public virtual string GroupIdAttribute
	  {
		  get
		  {
			return groupIdAttribute;
		  }
		  set
		  {
			this.groupIdAttribute = value;
		  }
	  }


	  public virtual string GroupMemberAttribute
	  {
		  get
		  {
			return groupMemberAttribute;
		  }
		  set
		  {
			this.groupMemberAttribute = value;
		  }
	  }


	  public virtual bool UseSsl
	  {
		  get
		  {
			return useSsl;
		  }
		  set
		  {
			this.useSsl = value;
		  }
	  }


	  public virtual bool UsePosixGroups
	  {
		  get
		  {
			return usePosixGroups;
		  }
		  set
		  {
			this.usePosixGroups = value;
		  }
	  }


	  public virtual SearchControls SearchControls
	  {
		  get
		  {
			SearchControls searchControls = new SearchControls();
			searchControls.SearchScope = SearchControls.SUBTREE_SCOPE;
			searchControls.TimeLimit = 30000;
			return searchControls;
		  }
	  }

	  public virtual string GroupTypeAttribute
	  {
		  get
		  {
			return groupTypeAttribute;
		  }
		  set
		  {
			this.groupTypeAttribute = value;
		  }
	  }


	  public virtual bool AllowAnonymousLogin
	  {
		  get
		  {
			return allowAnonymousLogin;
		  }
		  set
		  {
			this.allowAnonymousLogin = value;
		  }
	  }


	  public virtual bool AuthorizationCheckEnabled
	  {
		  get
		  {
			return authorizationCheckEnabled;
		  }
		  set
		  {
			this.authorizationCheckEnabled = value;
		  }
	  }


	}

}