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

	using AbstractProcessApplication = org.camunda.bpm.application.AbstractProcessApplication;
	using ProcessApplicationElResolver = org.camunda.bpm.application.ProcessApplicationElResolver;
	using EjbProcessApplication = org.camunda.bpm.application.impl.EjbProcessApplication;
	using ServletProcessApplication = org.camunda.bpm.application.impl.ServletProcessApplication;
	using ELResolver = org.camunda.bpm.engine.impl.javax.el.ELResolver;
	using ClassUtils = org.springframework.util.ClassUtils;
	using WebApplicationContext = org.springframework.web.context.WebApplicationContext;
	using WebApplicationContextUtils = org.springframework.web.context.support.WebApplicationContextUtils;

	/// <summary>
	/// <para>ProcessApplicationElResolver implementation providing support for the Spring Framework.</para>
	/// 
	/// <para>This implementation supports the following environments:
	///  <ul>
	///    <li>Bootstrapping through <seealso cref="SpringProcessApplication"/>. In this case the spring application context 
	///        is retrieved from the <seealso cref="SpringProcessApplication"/> class.</li>
	///    <li>Bootstrapping through <seealso cref="ServletProcessApplication"/>. In this case we have access to the <seealso cref="ServletContext"/>
	///        which allows accessing the web application's application context through the WebApplicationContextUtils class.</li>    
	///    </li>
	///  </ul>
	/// </para>
	/// 
	/// <para><strong>Limitation</strong>: The <seealso cref="EjbProcessApplication"/> is currently unsupported.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class SpringProcessApplicationElResolver : ProcessApplicationElResolver
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  private static readonly Logger LOGGER = Logger.getLogger(typeof(SpringProcessApplicationElResolver).FullName);

	  public virtual int? Precedence
	  {
		  get
		  {
			return org.camunda.bpm.application.ProcessApplicationElResolver_Fields.SPRING_RESOLVER;
		  }
	  }

	  public virtual ELResolver getElResolver(AbstractProcessApplication processApplication)
	  {

		if (processApplication is SpringProcessApplication)
		{
		  SpringProcessApplication springProcessApplication = (SpringProcessApplication) processApplication;
		  return new ApplicationContextElResolver(springProcessApplication.ApplicationContext);

		}
		else if (processApplication is ServletProcessApplication)
		{
		  ServletProcessApplication servletProcessApplication = (ServletProcessApplication) processApplication;

		  if (!ClassUtils.isPresent("org.springframework.web.context.support.WebApplicationContextUtils", processApplication.ProcessApplicationClassloader))
		  {
			LOGGER.log(Level.FINE, "WebApplicationContextUtils must be present for SpringProcessApplicationElResolver to work");
			return null;
		  }

		  ServletContext servletContext = servletProcessApplication.ServletContext;
		  WebApplicationContext applicationContext = WebApplicationContextUtils.getWebApplicationContext(servletContext);
		  if (applicationContext != null)
		  {
			return new ApplicationContextElResolver(applicationContext);
		  }

		}

		LOGGER.log(Level.FINE, "Process application class {0} unsupported by SpringProcessApplicationElResolver", processApplication);
		return null;
	  }

	}

}