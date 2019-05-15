using System;
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


	using Group = org.camunda.bpm.engine.identity.Group;
	using Tenant = org.camunda.bpm.engine.identity.Tenant;
	using ExceptionDto = org.camunda.bpm.engine.rest.dto.ExceptionDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using NamedProcessEngineRestServiceImpl = org.camunda.bpm.engine.rest.impl.NamedProcessEngineRestServiceImpl;
	using EngineUtil = org.camunda.bpm.engine.rest.util.EngineUtil;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// <summary>
	/// <para>
	/// Servlet filter to plug in authentication.
	/// </para>
	/// 
	/// <para>Valid init-params:</para>
	/// <table>
	/// <thead>
	///   <tr><th>Parameter</th><th>Required</th><th>Expected value</th></tr>
	/// <thead>
	/// <tbody>
	///    <tr><td>{@value #AUTHENTICATION_PROVIDER_PARAM}</td><td>yes</td><td>An implementation of <seealso cref="AuthenticationProvider"/></td></tr>
	///    <tr>
	///      <td>{@value #SERVLET_PATH_PREFIX}</td>
	///      <td>no</td>
	///      <td>The expected servlet path. Should only be set, if the underlying JAX-RS application is not deployed as a servlet (e.g. Resteasy allows deployments
	///      as a servlet filter). Value has to match what would be the <seealso cref="HttpServletRequest#getServletPath()"/> if it was deployed as a servlet.</td></tr>
	/// </tbody>
	/// </table>
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class ProcessEngineAuthenticationFilter : Filter
	{

	  // regexes for urls that may be accessed unauthorized
	  protected internal static readonly Pattern[] WHITE_LISTED_URL_PATTERNS = new Pattern[] {Pattern.compile("^" + NamedProcessEngineRestServiceImpl.PATH + "/?")};

	  protected internal static readonly Pattern ENGINE_REQUEST_URL_PATTERN = Pattern.compile("^" + NamedProcessEngineRestServiceImpl.PATH + "/(.*?)(/|$)");
	  protected internal const string DEFAULT_ENGINE_NAME = "default";

	  // init params
	  public const string AUTHENTICATION_PROVIDER_PARAM = "authentication-provider";
	  public const string SERVLET_PATH_PREFIX = "rest-url-pattern-prefix";

	  protected internal AuthenticationProvider authenticationProvider;
	  protected internal string servletPathPrefix;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void init(javax.servlet.FilterConfig filterConfig) throws javax.servlet.ServletException
	  public override void init(FilterConfig filterConfig)
	  {
		string authenticationProviderClassName = filterConfig.getInitParameter(AUTHENTICATION_PROVIDER_PARAM);

		if (string.ReferenceEquals(authenticationProviderClassName, null))
		{
		  throw new ServletException("Cannot instantiate authentication filter: no authentication provider set. init-param " + AUTHENTICATION_PROVIDER_PARAM + " missing");
		}

		try
		{
		  Type authenticationProviderClass = Type.GetType(authenticationProviderClassName);
		  authenticationProvider = (AuthenticationProvider) System.Activator.CreateInstance(authenticationProviderClass);
		}
		catch (ClassNotFoundException e)
		{
		  throw new ServletException("Cannot instantiate authentication filter: authentication provider not found", e);
		}
		catch (InstantiationException e)
		{
		  throw new ServletException("Cannot instantiate authentication filter: cannot instantiate authentication provider", e);
		}
		catch (IllegalAccessException e)
		{
		  throw new ServletException("Cannot instantiate authentication filter: constructor not accessible", e);
		}
		catch (System.InvalidCastException e)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new ServletException("Cannot instantiate authentication filter: authentication provider does not implement interface " + typeof(AuthenticationProvider).FullName, e);
		}

		servletPathPrefix = filterConfig.getInitParameter(SERVLET_PATH_PREFIX);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void doFilter(javax.servlet.ServletRequest request, javax.servlet.ServletResponse response, javax.servlet.FilterChain chain) throws java.io.IOException, javax.servlet.ServletException
	  public override void doFilter(ServletRequest request, ServletResponse response, FilterChain chain)
	  {

		HttpServletRequest req = (HttpServletRequest) request;
		HttpServletResponse resp = (HttpServletResponse) response;

		string servletPath = servletPathPrefix;
		if (string.ReferenceEquals(servletPath, null))
		{
		  servletPath = req.ServletPath;
		}
		string requestUrl = req.RequestURI.substring(req.ContextPath.length() + servletPath.Length);

		bool requiresEngineAuthentication = requiresEngineAuthentication(requestUrl);

		if (!requiresEngineAuthentication)
		{
		  chain.doFilter(request, response);
		  return;
		}

		string engineName = extractEngineName(requestUrl);
		ProcessEngine engine = getAddressedEngine(engineName);

		if (engine == null)
		{
		  resp.Status = Status.NOT_FOUND.StatusCode;
		  ExceptionDto exceptionDto = new ExceptionDto();
		  exceptionDto.Type = typeof(InvalidRequestException).Name;
		  exceptionDto.Message = "Process engine " + engineName + " not available";
		  ObjectMapper objectMapper = new ObjectMapper();

		  resp.ContentType = MediaType.APPLICATION_JSON;
		  objectMapper.writer().writeValue(resp.Writer, exceptionDto);
		  resp.Writer.flush();

		  return;
		}

		AuthenticationResult authenticationResult = authenticationProvider.extractAuthenticatedUser(req, engine);

		if (authenticationResult.Authenticated)
		{
		  try
		  {
			string authenticatedUser = authenticationResult.AuthenticatedUser;
			IList<string> groups = authenticationResult.Groups;
			IList<string> tenants = authenticationResult.Tenants;
			setAuthenticatedUser(engine, authenticatedUser, groups, tenants);
			chain.doFilter(request, response);
		  }
		  finally
		  {
			clearAuthentication(engine);
		  }
		}
		else
		{
		  resp.Status = Status.UNAUTHORIZED.StatusCode;
		  authenticationProvider.augmentResponseByAuthenticationChallenge(resp, engine);
		}

	  }

	  public override void destroy()
	  {

	  }

	  protected internal virtual void setAuthenticatedUser(ProcessEngine engine, string userId, IList<string> groupIds, IList<string> tenantIds)
	  {
		if (groupIds == null)
		{
		  groupIds = getGroupsOfUser(engine, userId);
		}

		if (tenantIds == null)
		{
		  tenantIds = getTenantsOfUser(engine, userId);
		}

		engine.IdentityService.setAuthentication(userId, groupIds, tenantIds);
	  }

	  protected internal virtual IList<string> getGroupsOfUser(ProcessEngine engine, string userId)
	  {
		IList<Group> groups = engine.IdentityService.createGroupQuery().groupMember(userId).list();

		IList<string> groupIds = new List<string>();
		foreach (Group group in groups)
		{
		  groupIds.Add(group.Id);
		}
		return groupIds;
	  }

	  protected internal virtual IList<string> getTenantsOfUser(ProcessEngine engine, string userId)
	  {
		IList<Tenant> tenants = engine.IdentityService.createTenantQuery().userMember(userId).includingGroupsOfUser(true).list();

		IList<string> tenantIds = new List<string>();
		foreach (Tenant tenant in tenants)
		{
		  tenantIds.Add(tenant.Id);
		}
		return tenantIds;
	  }

	  protected internal virtual void clearAuthentication(ProcessEngine engine)
	  {
		engine.IdentityService.clearAuthentication();
	  }

	  protected internal virtual bool requiresEngineAuthentication(string requestUrl)
	  {
		foreach (Pattern whiteListedUrlPattern in WHITE_LISTED_URL_PATTERNS)
		{
		  Matcher matcher = whiteListedUrlPattern.matcher(requestUrl);
		  if (matcher.matches())
		  {
			return false;
		  }
		}

		return true;
	  }

	  /// <summary>
	  /// May not return null
	  /// </summary>
	  protected internal virtual string extractEngineName(string requestUrl)
	  {

		Matcher matcher = ENGINE_REQUEST_URL_PATTERN.matcher(requestUrl);

		if (matcher.find())
		{
		  return matcher.group(1);
		}
		else
		{
		  // any request that does not match a specific engine and is not an /engine request
		  // is mapped to the default engine
		  return DEFAULT_ENGINE_NAME;
		}
	  }

	  protected internal virtual ProcessEngine getAddressedEngine(string engineName)
	  {
		return EngineUtil.lookupProcessEngine(engineName);
	  }

	}

}