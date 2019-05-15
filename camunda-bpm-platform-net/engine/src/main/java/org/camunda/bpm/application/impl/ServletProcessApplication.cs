﻿using System;
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
namespace org.camunda.bpm.application.impl
{


	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using ClassLoaderUtil = org.camunda.bpm.engine.impl.util.ClassLoaderUtil;

	/// <summary>
	/// <para>A <seealso cref="AbstractProcessApplication"/> Implementation to be used in a Servlet container environment.</para>
	/// 
	/// <para>This class implements the <seealso cref="ServletContextListener"/> interface and can thus participate in the
	/// deployment lifecycle of your web application.</para>
	/// 
	/// <h2>Usage</h2>
	/// <para>In a <strong>Servlet 3.0</strong> container it is sufficient adding a custom subclass of
	/// <seealso cref="ServletProcessApplication"/> annotated with <code>{@literal @}ProcessApplication</code> to your
	/// application:
	/// <pre>
	/// {@literal @}ProcessApplication("Loan Approval App")
	/// public class LoanApprovalApplication extends ServletProcessApplication {
	/// // empty implementation
	/// }
	/// </pre>
	/// This, in combination with a <code>META-INF/processes.xml</code> file is sufficient for making sure
	/// that the process application class is picked up at runtime.</para>
	/// <para>In a <strong>Servlet 2.5</strong> container, the process application can be added as a web listener
	/// to your project's <code>web.xml</code></para>
	/// <pre>
	/// {@literal <}?xml version="1.0" encoding="UTF-8"?{@literal >}
	/// {@literal <}web-app version="2.5" xmlns="http://java.sun.com/xml/ns/javaee"
	///       xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
	///       xsi:schemaLocation="http://java.sun.com/xml/ns/javaee    http://java.sun.com/xml/ns/javaee/web-app_2_5.xsd"{@literal >}
	/// 
	/// {@literal <}listener{@literal >}
	///   {@literal <}listener-class{@literal >}org.my.project.MyProcessApplication{@literal <}/listener-class{@literal >}
	/// {@literal <}/listener{@literal >}
	/// {@literal <}/web-app{@literal >}
	/// </pre>
	/// </p>
	/// 
	/// <h2>Invocation Semantics</h2>
	/// <para>When the <seealso cref="#execute(java.util.concurrent.Callable)"/> method is invoked, the servlet process
	/// application modifies the context classloader of the current Thread to the classloader that loaded
	/// the application-provided subclass of this class. This allows
	/// <ul>
	/// <li>the process engine to resolve <seealso cref="JavaDelegate"/> implementations using the classloader
	/// of the process application</li>
	/// <li>In apache tomcat this allows you to resolve Naming Resources (JNDI) form the naming
	/// context of the process application. JNDI name resolution is based on the TCCL in Apache Tomcat.</li>
	/// </ul>
	/// </para>
	/// 
	/// 
	/// <pre>
	///                        Set TCCL of Process Application
	///                                     |
	///                                     |  +--------------------+
	///                                     |  |Process Application |
	///                       invoke        v  |                    |
	///      ProcessEngine -----------------O--|--> Java Delegate   |
	///                                        |                    |
	///                                        |                    |
	///                                        +--------------------+
	/// 
	/// </pre>
	/// 
	/// <h2>Process Application Reference</h2>
	/// <para>The process engine holds a <seealso cref="WeakReference"/> to the <seealso cref="ServletProcessApplication"/> and does not cache any
	/// classes loaded using the Process Application classloader.</para>
	/// 
	/// 
	/// @author Daniel Meyer
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ServletProcessApplication : AbstractProcessApplication, ServletContextListener
	{

	  protected internal string servletContextName;
	  protected internal string servletContextPath;

	  protected internal ProcessApplicationReferenceImpl reference;

	  protected internal ClassLoader processApplicationClassloader;
	  protected internal ServletContext servletContext;


	  protected internal override string autodetectProcessApplicationName()
	  {
		string name = (!string.ReferenceEquals(servletContextName, null) && servletContextName.Length > 0) ? servletContextName : servletContextPath;
		if (name.StartsWith("/", StringComparison.Ordinal))
		{
		  name = name.Substring(1);
		}
		return name;
	  }

	  public override ProcessApplicationReference Reference
	  {
		  get
		  {
			 if (reference == null)
			 {
			   reference = new ProcessApplicationReferenceImpl(this);
			 }
			 return reference;
		  }
	  }

	  public virtual void contextInitialized(ServletContextEvent sce)
	  {
		servletContext = sce.ServletContext;
		servletContextPath = servletContext.ContextPath;
		servletContextName = sce.ServletContext.ServletContextName;

		processApplicationClassloader = initProcessApplicationClassloader(sce);

		// perform lifecycle start
		deploy();
	  }

	  protected internal virtual ClassLoader initProcessApplicationClassloader(ServletContextEvent sce)
	  {

		if (isServlet30ApiPresent(sce) && this.GetType().Equals(typeof(ServletProcessApplication)))
		{
		  return ClassLoaderUtil.getServletContextClassloader(sce);

		}
		else
		{
		  return ClassLoaderUtil.getClassloader(this.GetType());

		}

	  }

	  private bool isServlet30ApiPresent(ServletContextEvent sce)
	  {
		return sce.ServletContext.MajorVersion >= 3;
	  }

	  public override ClassLoader ProcessApplicationClassloader
	  {
		  get
		  {
			return processApplicationClassloader;
		  }
	  }

	  public virtual void contextDestroyed(ServletContextEvent sce)
	  {

		// perform lifecycle stop
		undeploy();

		// clear the reference
		if (reference != null)
		{
		  reference.clear();
		}
		reference = null;
	  }

	  public override IDictionary<string, string> Properties
	  {
		  get
		  {
			IDictionary<string, string> properties = new Dictionary<string, string>();
    
			// set the servlet context path as property
			properties[org.camunda.bpm.application.ProcessApplicationInfo_Fields.PROP_SERVLET_CONTEXT_PATH] = servletContextPath;
    
			return properties;
		  }
	  }

	  public virtual ServletContext ServletContext
	  {
		  get
		  {
			return servletContext;
		  }
	  }
	}
}