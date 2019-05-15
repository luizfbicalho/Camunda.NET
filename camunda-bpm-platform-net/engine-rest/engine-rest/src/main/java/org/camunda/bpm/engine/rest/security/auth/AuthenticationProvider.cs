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


	/// <summary>
	/// A provider to handle the authentication of <seealso cref="HttpServletRequest"/>s.
	/// May implement a specific authentication scheme.
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public interface AuthenticationProvider
	{

	  /// <summary>
	  /// Checks the request for authentication. May not return null, but always an <seealso cref="AuthenticationResult"/> that indicates, whether
	  /// authentication was successful, and, if true, always provides the authenticated user.
	  /// </summary>
	  /// <param name="request"> the request to authenticate </param>
	  /// <param name="engine"> the process engine the request addresses. May be used to authenticate against the engine's identity service. </param>
	  AuthenticationResult extractAuthenticatedUser(HttpServletRequest request, ProcessEngine engine);

	  /// <summary>
	  /// <para>
	  /// Callback to add an authentication challenge to the response to the client. Called in case of unsuccessful authentication.
	  /// </para>
	  /// 
	  /// <para>
	  /// For example, a Http Basic auth implementation may set the WWW-Authenticate header to <code>Basic realm="engine name"</code>.
	  /// </para>
	  /// </summary>
	  /// <param name="request"> the response to augment </param>
	  /// <param name="engine"> the process engine the request addressed. May be considered as an authentication realm to create a specific authentication
	  /// challenge </param>
	  void augmentResponseByAuthenticationChallenge(HttpServletResponse response, ProcessEngine engine);
	}

}