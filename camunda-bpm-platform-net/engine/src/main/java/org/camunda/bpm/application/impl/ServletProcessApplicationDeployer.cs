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
namespace org.camunda.bpm.application.impl
{

	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;

	/// <summary>
	/// <para>This class is an implementation of <seealso cref="ServletContainerInitializer"/> and
	/// is notified whenever a subclass of <seealso cref="ServletProcessApplication"/> annotated
	/// with the <seealso cref="ProcessApplication"/> annotation is deployed. In such an event,
	/// we automatically add the class as <seealso cref="ServletContextListener"/> to the
	/// <seealso cref="ServletContext"/>.</para>
	/// 
	/// <para><strong>NOTE:</strong> Only works with Servlet 3.0 or better.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @HandlesTypes(ProcessApplication.class) public class ServletProcessApplicationDeployer implements javax.servlet.ServletContainerInitializer
	public class ServletProcessApplicationDeployer : ServletContainerInitializer
	{

	  private static ProcessApplicationLogger LOG = ProcessEngineLogger.PROCESS_APPLICATION_LOGGER;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void onStartup(java.util.Set<Class> c, javax.servlet.ServletContext ctx) throws javax.servlet.ServletException
	  public virtual void onStartup(ISet<Type> c, ServletContext ctx)
	  {
		if (c == null || c.Count == 0)
		{
		  // skip deployments that do not carry a PA
		  return;

		}

		if (c.Contains(typeof(ProcessApplication)))
		{
		  // this is a workaround for a bug in WebSphere-8.5 who
		  // ships the annotation itself as part of the discovered classes.

		  // copy into a fresh Set as we don't know if the original Set is mutable or immutable.
		  c = new HashSet<Type>(c);

		  // and now remove the annotation itself.
		  c.remove(typeof(ProcessApplication));
		}


		string contextPath = ctx.ContextPath;
		if (c.Count > 1)
		{
		  // a deployment must only contain a single PA
		  throw LOG.multiplePasException(c, contextPath);

		}
		else if (c.Count == 1)
		{
		  Type paClass = c.GetEnumerator().next();

		  // validate whether it is a legal Process Application
		  if (!paClass.IsAssignableFrom(typeof(AbstractProcessApplication)))
		  {
			throw LOG.paWrongTypeException(paClass);
		  }

		  // add it as listener if it's a ServletProcessApplication
		  if (paClass.IsAssignableFrom(typeof(ServletProcessApplication)))
		  {
			LOG.detectedPa(paClass);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			ctx.addListener(paClass.FullName);
		  }
		}
		else
		{
		  LOG.servletDeployerNoPaFound(contextPath);
		}

	  }

	}

}