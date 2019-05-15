using System;
using System.Collections.Generic;
using System.Text;

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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.GROUP;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.USER;



	using BadUserRequestException = org.camunda.bpm.engine.BadUserRequestException;
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using Group = org.camunda.bpm.engine.identity.Group;
	using GroupQuery = org.camunda.bpm.engine.identity.GroupQuery;
	using NativeUserQuery = org.camunda.bpm.engine.identity.NativeUserQuery;
	using Tenant = org.camunda.bpm.engine.identity.Tenant;
	using TenantQuery = org.camunda.bpm.engine.identity.TenantQuery;
	using User = org.camunda.bpm.engine.identity.User;
	using UserQuery = org.camunda.bpm.engine.identity.UserQuery;
	using AbstractQuery = org.camunda.bpm.engine.impl.AbstractQuery;
	using QueryOrderingProperty = org.camunda.bpm.engine.impl.QueryOrderingProperty;
	using UserQueryImpl = org.camunda.bpm.engine.impl.UserQueryImpl;
	using UserQueryProperty = org.camunda.bpm.engine.impl.UserQueryProperty;
	using IdentityProviderException = org.camunda.bpm.engine.impl.identity.IdentityProviderException;
	using ReadOnlyIdentityProvider = org.camunda.bpm.engine.impl.identity.ReadOnlyIdentityProvider;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using GroupEntity = org.camunda.bpm.engine.impl.persistence.entity.GroupEntity;
	using UserEntity = org.camunda.bpm.engine.impl.persistence.entity.UserEntity;
	using LdapPluginLogger = org.camunda.bpm.identity.impl.ldap.util.LdapPluginLogger;

	/// <summary>
	/// <para>LDAP <seealso cref="ReadOnlyIdentityProvider"/>.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class LdapIdentityProviderSession : ReadOnlyIdentityProvider
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  private static readonly Logger LOG = Logger.getLogger(typeof(LdapIdentityProviderSession).FullName);

	  protected internal LdapConfiguration ldapConfiguration;
	  protected internal LdapContext initialContext;

	  public LdapIdentityProviderSession(LdapConfiguration ldapConfiguration)
	  {
		this.ldapConfiguration = ldapConfiguration;
	  }

	  // Session Lifecycle //////////////////////////////////

	  public virtual void flush()
	  {
		// nothing to do
	  }

	  public virtual void close()
	  {
		if (initialContext != null)
		{
		  try
		  {
			initialContext.close();
		  }
		  catch (Exception e)
		  {
			// ignore
			LdapPluginLogger.INSTANCE.exceptionWhenClosingLdapCOntext(e);
		  }
		}
	  }

	  protected internal virtual InitialLdapContext openContext(string userDn, string password)
	  {
		Dictionary<string, string> env = new Dictionary<string, string>();
		env[Context.INITIAL_CONTEXT_FACTORY] = ldapConfiguration.InitialContextFactory;
		env[Context.SECURITY_AUTHENTICATION] = ldapConfiguration.SecurityAuthentication;
		env[Context.PROVIDER_URL] = ldapConfiguration.ServerUrl;
		env[Context.SECURITY_PRINCIPAL] = userDn;
		env[Context.SECURITY_CREDENTIALS] = password;

		// for anonymous login
		if (ldapConfiguration.AllowAnonymousLogin && password.Length == 0)
		{
		  env[Context.SECURITY_AUTHENTICATION] = "none";
		}

		if (ldapConfiguration.UseSsl)
		{
		  env[Context.SECURITY_PROTOCOL] = "ssl";
		}

		// add additional properties
		IDictionary<string, string> contextProperties = ldapConfiguration.ContextProperties;
		if (contextProperties != null)
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
		  env.putAll(contextProperties);
		}

		try
		{
		  return new InitialLdapContext(env, null);

		}
		catch (AuthenticationException e)
		{
		  throw new LdapAuthenticationException("Could not authenticate with LDAP server", e);

		}
		catch (NamingException e)
		{
		  throw new IdentityProviderException("Could not connect to LDAP server", e);

		}
	  }

	  protected internal virtual void ensureContextInitialized()
	  {
		if (initialContext == null)
		{
		  initialContext = openContext(ldapConfiguration.ManagerDn, ldapConfiguration.ManagerPassword);
		}
	  }

	  // Users /////////////////////////////////////////////////

	  public virtual User findUserById(string userId)
	  {
		return createUserQuery(org.camunda.bpm.engine.impl.context.Context.CommandContext).userId(userId).singleResult();
	  }

	  public virtual UserQuery createUserQuery()
	  {
		return new LdapUserQueryImpl(org.camunda.bpm.engine.impl.context.Context.ProcessEngineConfiguration.CommandExecutorTxRequired);
	  }

	  public virtual UserQueryImpl createUserQuery(CommandContext commandContext)
	  {
		return new LdapUserQueryImpl();
	  }

	  public virtual NativeUserQuery createNativeUserQuery()
	  {
		throw new BadUserRequestException("Native user queries are not supported for LDAP identity service provider.");
	  }

	  public virtual long findUserCountByQueryCriteria(LdapUserQueryImpl query)
	  {
		ensureContextInitialized();
		return findUserByQueryCriteria(query).Count;
	  }

	  public virtual IList<User> findUserByQueryCriteria(LdapUserQueryImpl query)
	  {
		ensureContextInitialized();
		if (!string.ReferenceEquals(query.GroupId, null))
		{
		  // if restriction on groupId is provided, we need to search in group tree first, look for the group and then further restrict on the members
		  return findUsersByGroupId(query);
		}
		else
		{
		  string userBaseDn = composeDn(ldapConfiguration.UserSearchBase, ldapConfiguration.BaseDn);
		  return findUsersWithoutGroupId(query, userBaseDn, false);
		}
	  }

	  protected internal virtual IList<User> findUsersByGroupId(LdapUserQueryImpl query)
	  {
		string baseDn = getDnForGroup(query.GroupId);

		// compose group search filter
		string groupSearchFilter = "(& " + ldapConfiguration.GroupSearchFilter + ")";

		NamingEnumeration<SearchResult> enumeration = null;
		try
		{
		  enumeration = initialContext.search(baseDn, groupSearchFilter, ldapConfiguration.SearchControls);

		  IList<string> groupMemberList = new List<string>();

		  // first find group
		  while (enumeration.hasMoreElements())
		  {
			SearchResult result = enumeration.nextElement();
			Attribute memberAttribute = result.Attributes.get(ldapConfiguration.GroupMemberAttribute);
			if (null != memberAttribute)
			{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: javax.naming.NamingEnumeration<?> allMembers = memberAttribute.getAll();
			  NamingEnumeration<object> allMembers = memberAttribute.All;

			  // iterate group members
			  while (allMembers.hasMoreElements())
			  {
				groupMemberList.Add((string) allMembers.nextElement());
			  }
			}
		  }

		  IList<User> userList = new List<User>();
		  string userBaseDn = composeDn(ldapConfiguration.UserSearchBase, ldapConfiguration.BaseDn);
		  int memberCount = 0;
		  foreach (string memberId in groupMemberList)
		  {
			if (userList.Count < query.MaxResults && memberCount >= query.FirstResult)
			{
			  if (ldapConfiguration.UsePosixGroups)
			  {
				query.userId(memberId);
			  }
			  IList<User> users = ldapConfiguration.UsePosixGroups ? findUsersWithoutGroupId(query, userBaseDn, true) : findUsersWithoutGroupId(query, memberId, true);
			  if (users.Count > 0)
			  {
				userList.Add(users[0]);
			  }
			}
			memberCount++;
		  }

		  return userList;

		}
		catch (NamingException e)
		{
		  throw new IdentityProviderException("Could not query for users", e);

		}
		finally
		{
		  try
		  {
			if (enumeration != null)
			{
			  enumeration.close();
			}
		  }
		  catch (Exception)
		  {
			// ignore silently
		  }
		}
	  }

	  public virtual IList<User> findUsersWithoutGroupId(LdapUserQueryImpl query, string userBaseDn, bool ignorePagination)
	  {

		if (ldapConfiguration.SortControlSupported)
		{
		  applyRequestControls(query);
		}

		NamingEnumeration<SearchResult> enumeration = null;
		try
		{

		  string filter = getUserSearchFilter(query);
		  enumeration = initialContext.search(userBaseDn, filter, ldapConfiguration.SearchControls);

		  // perform client-side paging
		  int resultCount = 0;
		  IList<User> userList = new List<User>();

		  StringBuilder resultLogger = new StringBuilder();
		  if (LdapPluginLogger.INSTANCE.DebugEnabled)
		  {
			resultLogger.Append("LDAP user query results: [");
		  }

		  while (enumeration.hasMoreElements() && (userList.Count < query.MaxResults || ignorePagination))
		  {
			SearchResult result = enumeration.nextElement();

			UserEntity user = transformUser(result);

			string userId = user.Id;

			if (string.ReferenceEquals(userId, null))
			{
			  LdapPluginLogger.INSTANCE.invalidLdapUserReturned(user, result);
			}
			else
			{
			  if (isAuthenticatedUser(user) || isAuthorized(READ, USER, userId))
			  {

				if (resultCount >= query.FirstResult || ignorePagination)
				{
				  if (LdapPluginLogger.INSTANCE.DebugEnabled)
				  {
					resultLogger.Append(user);
					resultLogger.Append(" based on ");
					resultLogger.Append(result);
					resultLogger.Append(", ");
				  }

				  userList.Add(user);
				}

				resultCount++;
			  }
			}
		  }

		  if (LdapPluginLogger.INSTANCE.DebugEnabled)
		  {
			resultLogger.Append("]");
			LdapPluginLogger.INSTANCE.userQueryResult(resultLogger.ToString());
		  }

		  return userList;

		}
		catch (NamingException e)
		{
		  throw new IdentityProviderException("Could not query for users", e);

		}
		finally
		{
		  try
		  {
			if (enumeration != null)
			{
			  enumeration.close();
			}
		  }
		  catch (Exception)
		  {
			// ignore silently
		  }
		}
	  }

	  public virtual bool checkPassword(string userId, string password)
	  {

		// prevent a null password
		if (string.ReferenceEquals(password, null))
		{
		  return false;
		}

		// engine can't work without users
		if (string.ReferenceEquals(userId, null) || userId.Length == 0)
		{
		  return false;
		}

		/*
		* We only allow login with no password if anonymous login is set.
		* RFC allows such a behavior but discourages the usage so we provide it for
		* user which have an ldap with anonymous login.
		*/
		if (!ldapConfiguration.AllowAnonymousLogin && password.Equals(""))
		{
		  return false;
		}

		// first search for user using manager DN
		LdapUserEntity user = (LdapUserEntity) findUserById(userId);
		close();

		if (user == null)
		{
		  return false;
		}
		else
		{

		  try
		  {
			// bind authenticate for user + supplied password
			openContext(user.Dn, password);
			return true;

		  }
		  catch (LdapAuthenticationException)
		  {
			return false;

		  }

		}

	  }

	  protected internal virtual string getUserSearchFilter(LdapUserQueryImpl query)
	  {

		StringWriter search = new StringWriter();
		search.write("(&");

		// restrict to users
		search.write(ldapConfiguration.UserSearchFilter);

		// add additional filters from query
		if (!string.ReferenceEquals(query.Id, null))
		{
		  addFilter(ldapConfiguration.UserIdAttribute, escapeLDAPSearchFilter(query.Id), search);
		}
		if (query.Ids != null && query.Ids.Length > 0)
		{
		  // wrap ids in OR statement
		  search.write("(|");
		  foreach (string userId in query.Ids)
		  {
			addFilter(ldapConfiguration.UserIdAttribute, escapeLDAPSearchFilter(userId), search);
		  }
		  search.write(")");
		}
		if (!string.ReferenceEquals(query.Email, null))
		{
		  addFilter(ldapConfiguration.UserEmailAttribute, query.Email, search);
		}
		if (!string.ReferenceEquals(query.EmailLike, null))
		{
		  addFilter(ldapConfiguration.UserEmailAttribute, query.EmailLike, search);
		}
		if (!string.ReferenceEquals(query.FirstName, null))
		{
		  addFilter(ldapConfiguration.UserFirstnameAttribute, query.FirstName, search);
		}
		if (!string.ReferenceEquals(query.FirstNameLike, null))
		{
		  addFilter(ldapConfiguration.UserFirstnameAttribute, query.FirstNameLike, search);
		}
		if (!string.ReferenceEquals(query.LastName, null))
		{
		  addFilter(ldapConfiguration.UserLastnameAttribute, query.LastName, search);
		}
		if (!string.ReferenceEquals(query.LastNameLike, null))
		{
		  addFilter(ldapConfiguration.UserLastnameAttribute, query.LastNameLike, search);
		}

		search.write(")");

		return search.ToString();
	  }

	  // Groups ///////////////////////////////////////////////

	  public virtual Group findGroupById(string groupId)
	  {
		return createGroupQuery(org.camunda.bpm.engine.impl.context.Context.CommandContext).groupId(groupId).singleResult();
	  }

	  public virtual GroupQuery createGroupQuery()
	  {
		return new LdapGroupQuery(org.camunda.bpm.engine.impl.context.Context.ProcessEngineConfiguration.CommandExecutorTxRequired);
	  }

	  public virtual GroupQuery createGroupQuery(CommandContext commandContext)
	  {
		return new LdapGroupQuery();
	  }

	  public virtual long findGroupCountByQueryCriteria(LdapGroupQuery ldapGroupQuery)
	  {
		ensureContextInitialized();
		return findGroupByQueryCriteria(ldapGroupQuery).Count;
	  }

	  public virtual IList<Group> findGroupByQueryCriteria(LdapGroupQuery query)
	  {
		ensureContextInitialized();

		string groupBaseDn = composeDn(ldapConfiguration.GroupSearchBase, ldapConfiguration.BaseDn);

		if (ldapConfiguration.SortControlSupported)
		{
		  applyRequestControls(query);
		}

		NamingEnumeration<SearchResult> enumeration = null;
		try
		{

		  string filter = getGroupSearchFilter(query);
		  enumeration = initialContext.search(groupBaseDn, filter, ldapConfiguration.SearchControls);

		  // perform client-side paging
		  int resultCount = 0;
		  IList<Group> groupList = new List<Group>();

		  StringBuilder resultLogger = new StringBuilder();
		  if (LdapPluginLogger.INSTANCE.DebugEnabled)
		  {
			resultLogger.Append("LDAP group query results: [");
		  }

		  while (enumeration.hasMoreElements() && groupList.Count < query.MaxResults)
		  {
			SearchResult result = enumeration.nextElement();

			GroupEntity group = transformGroup(result);

			string groupId = group.Id;

			if (string.ReferenceEquals(groupId, null))
			{
			  LdapPluginLogger.INSTANCE.invalidLdapGroupReturned(group, result);
			}
			else
			{
			  if (isAuthorized(READ, GROUP, groupId))
			  {

				if (resultCount >= query.FirstResult)
				{
				  if (LdapPluginLogger.INSTANCE.DebugEnabled)
				  {
					resultLogger.Append(group);
					resultLogger.Append(" based on ");
					resultLogger.Append(result);
					resultLogger.Append(", ");
				  }

				  groupList.Add(group);
				}

				resultCount++;
			  }
			}
		  }

		  if (LdapPluginLogger.INSTANCE.DebugEnabled)
		  {
			resultLogger.Append("]");
			LdapPluginLogger.INSTANCE.groupQueryResult(resultLogger.ToString());
		  }

		  return groupList;

		}
		catch (NamingException e)
		{
		  throw new IdentityProviderException("Could not query for users", e);

		}
		finally
		{
		  try
		  {
			if (enumeration != null)
			{
			  enumeration.close();
			}
		  }
		  catch (Exception)
		  {
			// ignore silently
		  }
		}
	  }

	  protected internal virtual string getGroupSearchFilter(LdapGroupQuery query)
	  {

		StringWriter search = new StringWriter();
		search.write("(&");

		// restrict to groups
		search.write(ldapConfiguration.GroupSearchFilter);

		// add additional filters from query
		if (!string.ReferenceEquals(query.Id, null))
		{
		  addFilter(ldapConfiguration.GroupIdAttribute, query.Id, search);
		}
		if (query.Ids != null && query.Ids.Length > 0)
		{
		  search.write("(|");
		  foreach (string id in query.Ids)
		  {
			addFilter(ldapConfiguration.GroupIdAttribute, id, search);
		  }
		  search.write(")");
		}
		if (!string.ReferenceEquals(query.Name, null))
		{
		  addFilter(ldapConfiguration.GroupNameAttribute, query.Name, search);
		}
		if (!string.ReferenceEquals(query.NameLike, null))
		{
		  addFilter(ldapConfiguration.GroupNameAttribute, query.NameLike, search);
		}
		if (!string.ReferenceEquals(query.UserId, null))
		{
		  string userDn = null;
		  if (ldapConfiguration.UsePosixGroups)
		  {
			userDn = query.UserId;
		  }
		  else
		  {
			userDn = getDnForUser(query.UserId);
		  }
		  addFilter(ldapConfiguration.GroupMemberAttribute, escapeLDAPSearchFilter(userDn), search);
		}
		search.write(")");

		return search.ToString();
	  }

	  // Utils ////////////////////////////////////////////

	  protected internal virtual string getDnForUser(string userId)
	  {
		LdapUserEntity user = (LdapUserEntity) createUserQuery(org.camunda.bpm.engine.impl.context.Context.CommandContext).userId(userId).singleResult();
		if (user == null)
		{
		  return "";
		}
		else
		{
		  return user.Dn;
		}
	  }

	  protected internal virtual string getDnForGroup(string groupId)
	  {
		LdapGroupEntity group = (LdapGroupEntity) createGroupQuery(org.camunda.bpm.engine.impl.context.Context.CommandContext).groupId(groupId).singleResult();
		if (group == null)
		{
		  return "";
		}
		else
		{
		  return group.Dn;
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected String getStringAttributeValue(String attrName, javax.naming.directory.Attributes attributes) throws javax.naming.NamingException
	  protected internal virtual string getStringAttributeValue(string attrName, Attributes attributes)
	  {
		Attribute attribute = attributes.get(attrName);
		if (attribute != null)
		{
		  return (string) attribute.get();
		}
		else
		{
		  return null;
		}
	  }

	  protected internal virtual void addFilter(string attributeName, string attributeValue, StringWriter writer)
	  {
		writer.write("(");
		writer.write(attributeName);
		writer.write("=");
		writer.write(attributeValue);
		writer.write(")");
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected LdapUserEntity transformUser(javax.naming.directory.SearchResult result) throws javax.naming.NamingException
	  protected internal virtual LdapUserEntity transformUser(SearchResult result)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.naming.directory.Attributes attributes = result.getAttributes();
		Attributes attributes = result.Attributes;
		LdapUserEntity user = new LdapUserEntity();
		user.Dn = result.NameInNamespace;
		user.Id = getStringAttributeValue(ldapConfiguration.UserIdAttribute, attributes);
		user.FirstName = getStringAttributeValue(ldapConfiguration.UserFirstnameAttribute, attributes);
		user.LastName = getStringAttributeValue(ldapConfiguration.UserLastnameAttribute, attributes);
		user.Email = getStringAttributeValue(ldapConfiguration.UserEmailAttribute, attributes);
		return user;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected org.camunda.bpm.engine.impl.persistence.entity.GroupEntity transformGroup(javax.naming.directory.SearchResult result) throws javax.naming.NamingException
	  protected internal virtual GroupEntity transformGroup(SearchResult result)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.naming.directory.Attributes attributes = result.getAttributes();
		Attributes attributes = result.Attributes;
		LdapGroupEntity group = new LdapGroupEntity();
		group.Dn = result.NameInNamespace;
		group.Id = getStringAttributeValue(ldapConfiguration.GroupIdAttribute, attributes);
		group.Name = getStringAttributeValue(ldapConfiguration.GroupNameAttribute, attributes);
		group.Type = getStringAttributeValue(ldapConfiguration.GroupTypeAttribute, attributes);
		return group;
	  }

	  protected internal virtual void applyRequestControls<T1>(AbstractQuery<T1> query)
	  {

		try
		{
		  IList<Control> controls = new List<Control>();

		  IList<QueryOrderingProperty> orderBy = query.OrderingProperties;
		  if (orderBy != null)
		  {
			foreach (QueryOrderingProperty orderingProperty in orderBy)
			{
			  string propertyName = orderingProperty.QueryProperty.Name;
			  if (org.camunda.bpm.engine.impl.UserQueryProperty_Fields.USER_ID.Name.Equals(propertyName))
			  {
				controls.Add(new SortControl(ldapConfiguration.UserIdAttribute, Control.CRITICAL));

			  }
			  else if (org.camunda.bpm.engine.impl.UserQueryProperty_Fields.EMAIL.Name.Equals(propertyName))
			  {
				controls.Add(new SortControl(ldapConfiguration.UserEmailAttribute, Control.CRITICAL));

			  }
			  else if (org.camunda.bpm.engine.impl.UserQueryProperty_Fields.FIRST_NAME.Name.Equals(propertyName))
			  {
				controls.Add(new SortControl(ldapConfiguration.UserFirstnameAttribute, Control.CRITICAL));

			  }
			  else if (org.camunda.bpm.engine.impl.UserQueryProperty_Fields.LAST_NAME.Name.Equals(propertyName))
			  {
				controls.Add(new SortControl(ldapConfiguration.UserLastnameAttribute, Control.CRITICAL));
			  }
			}
		  }

		  initialContext.RequestControls = controls.ToArray();

		}
		catch (Exception e)
		{
		  throw new IdentityProviderException("Exception while setting paging settings", e);
		}
	  }

	  protected internal virtual string composeDn(params string[] parts)
	  {
		StringWriter resultDn = new StringWriter();
		for (int i = 0; i < parts.Length; i++)
		{
		  string part = parts[i];
		  if (string.ReferenceEquals(part, null) || part.Length == 0)
		  {
			continue;
		  }
		  if (part.EndsWith(",", StringComparison.Ordinal))
		  {
			part = StringHelper.SubstringSpecial(part, part.Length - 2, part.Length - 1);
		  }
		  if (part.StartsWith(",", StringComparison.Ordinal))
		  {
			part = part.Substring(1);
		  }
		  string currentDn = resultDn.ToString();
		  if (!currentDn.EndsWith(",", StringComparison.Ordinal) && currentDn.Length > 0)
		  {
			resultDn.write(",");
		  }
		  resultDn.write(part);
		}
		return resultDn.ToString();
	  }


	  /// <returns> true if the passed-in user is currently authenticated </returns>
	  protected internal virtual bool isAuthenticatedUser(UserEntity user)
	  {
		if (string.ReferenceEquals(user.Id, null))
		{
		  return false;
		}
		return user.Id.Equals(org.camunda.bpm.engine.impl.context.Context.CommandContext.AuthenticatedUserId, StringComparison.OrdinalIgnoreCase);
	  }

	  protected internal virtual bool isAuthorized(Permission permission, Resource resource, string resourceId)
	  {
		return !ldapConfiguration.AuthorizationCheckEnabled || org.camunda.bpm.engine.impl.context.Context.CommandContext.AuthorizationManager.isAuthorized(permission, resource, resourceId);
	  }

	  // Based on https://www.owasp.org/index.php/Preventing_LDAP_Injection_in_Java
	  protected internal string escapeLDAPSearchFilter(string filter)
	  {
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < filter.Length; i++)
		{
		  char curChar = filter[i];
			switch (curChar)
			{
			  case '\\':
				sb.Append("\\x0005c");
				break;
			  case '*':
				sb.Append("\\x0002a");
				break;
			  case '(':
				sb.Append("\\x00028");
				break;
			  case ')':
				sb.Append("\\x00029");
				break;
			  case '\u0000':
				sb.Append("\\x0000");
				break;
			  default:
				sb.Append(curChar);
			break;
			}
		}
		return sb.ToString();
	  }

	  public virtual TenantQuery createTenantQuery()
	  {
		return new LdapTenantQuery(org.camunda.bpm.engine.impl.context.Context.ProcessEngineConfiguration.CommandExecutorTxRequired);
	  }

	  public virtual TenantQuery createTenantQuery(CommandContext commandContext)
	  {
		return new LdapTenantQuery();
	  }

	  public virtual Tenant findTenantById(string id)
	  {
		// since multi-tenancy is not supported for the LDAP plugin, always return null
		return null;
	  }
	}

}