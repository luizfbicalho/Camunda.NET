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
namespace org.camunda.bpm.engine.rest.filter
{

	/// <summary>
	/// <para>Cache control filter setting "Cache-Control: no-cache" on all GET requests.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </para>
	/// </summary>
	public class CacheControlFilter : Filter
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void init(FilterConfig filterConfig) throws ServletException
	  public virtual void init(FilterConfig filterConfig)
	  {

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void doFilter(ServletRequest req, ServletResponse resp, FilterChain chain) throws java.io.IOException, ServletException
	  public virtual void doFilter(ServletRequest req, ServletResponse resp, FilterChain chain)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.servlet.http.HttpServletRequest request = (javax.servlet.http.HttpServletRequest) req;
		HttpServletRequest request = (HttpServletRequest) req;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.servlet.http.HttpServletResponse response = (javax.servlet.http.HttpServletResponse) resp;
		HttpServletResponse response = (HttpServletResponse) resp;

		if ("GET".Equals(request.Method) && !request.RequestURI.EndsWith("xml"))
		{
		  response.setHeader("Cache-Control", "no-cache");
		}

		chain.doFilter(req, resp);
	  }

	  public virtual void destroy()
	  {

	  }

	}

}