using System;

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
namespace org.camunda.bpm.engine.rest.security.auth.impl
{
	using Base64 = org.camunda.bpm.engine.impl.digest._apacheCommonsCodec.Base64;


	/// <summary>
	/// <para>
	/// Authenticates a request against the provided process engine's identity service by applying http basic authentication.
	/// </para>
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class HttpBasicAuthenticationProvider : AuthenticationProvider
	{

	  protected internal const string BASIC_AUTH_HEADER_PREFIX = "Basic ";

	  public virtual AuthenticationResult extractAuthenticatedUser(HttpServletRequest request, ProcessEngine engine)
	  {
		string authorizationHeader = request.getHeader(HttpHeaders.AUTHORIZATION);

		if (!string.ReferenceEquals(authorizationHeader, null) && authorizationHeader.StartsWith(BASIC_AUTH_HEADER_PREFIX, StringComparison.Ordinal))
		{
		  string encodedCredentials = authorizationHeader.Substring(BASIC_AUTH_HEADER_PREFIX.Length);
		  string decodedCredentials = StringHelper.NewString(Base64.decodeBase64(encodedCredentials));
		  int firstColonIndex = decodedCredentials.IndexOf(":", StringComparison.Ordinal);

		  if (firstColonIndex == -1)
		  {
			return AuthenticationResult.unsuccessful();
		  }
		  else
		  {
			string userName = decodedCredentials.Substring(0, firstColonIndex);
			string password = decodedCredentials.Substring(firstColonIndex + 1);
			if (isAuthenticated(engine, userName, password))
			{
			  return AuthenticationResult.successful(userName);
			}
			else
			{
			  return AuthenticationResult.unsuccessful(userName);
			}
		  }
		}
		else
		{
		  return AuthenticationResult.unsuccessful();
		}
	  }

	  protected internal virtual bool isAuthenticated(ProcessEngine engine, string userName, string password)
	  {
		return engine.IdentityService.checkPassword(userName, password);
	  }

	  public virtual void augmentResponseByAuthenticationChallenge(HttpServletResponse response, ProcessEngine engine)
	  {
		response.setHeader(HttpHeaders.WWW_AUTHENTICATE, BASIC_AUTH_HEADER_PREFIX + "realm=\"" + engine.Name + "\"");
	  }
	}

}