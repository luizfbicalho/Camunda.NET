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
namespace org.camunda.bpm.engine.spring.application
{

	using ProcessApplication = org.camunda.bpm.application.ProcessApplication;
	using ProcessApplicationInfo = org.camunda.bpm.application.ProcessApplicationInfo;
	using ServletContextAware = org.springframework.web.context.ServletContextAware;

	/// <summary>
	/// <para>Process Application to be used in a Spring Web Application.</para>
	/// 
	/// <para>Requires the <em>spring-web</em> module to be on the classpath</para>
	/// 
	/// <para>In addition to the services provided by the <seealso cref="SpringProcessApplication"/>,
	/// this <seealso cref="ProcessApplication"/> exposes the servlet context path of the web application
	/// which it is a part of (see <seealso cref="ProcessApplicationInfo#PROP_SERVLET_CONTEXT_PATH"/>).</para>
	/// 
	/// <para>This implementation should be used with Spring Web Applications.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class SpringServletProcessApplication : SpringProcessApplication, ServletContextAware
	{

	  protected internal ServletContext servletContext;

	  public virtual ServletContext ServletContext
	  {
		  set
		  {
			this.servletContext = value;
		  }
	  }

	  public override void start()
	  {
		properties[org.camunda.bpm.application.ProcessApplicationInfo_Fields.PROP_SERVLET_CONTEXT_PATH] = servletContext.ContextPath;
		base.start();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void afterPropertiesSet() throws Exception
	  public override void afterPropertiesSet()
	  {
		// for backwards compatibility
		start();
	  }

	}

}