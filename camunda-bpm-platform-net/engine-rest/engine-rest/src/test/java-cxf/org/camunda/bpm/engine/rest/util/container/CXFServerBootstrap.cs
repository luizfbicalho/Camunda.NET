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
namespace org.camunda.bpm.engine.rest.util.container
{
	using Server = org.apache.cxf.endpoint.Server;
	using JAXRSServerFactoryBean = org.apache.cxf.jaxrs.JAXRSServerFactoryBean;


	/// <summary>
	/// @author Christopher Zell
	/// </summary>
	public class CXFServerBootstrap : EmbeddedServerBootstrap
	{

	  private Server server;
	  private string propertyPort;
	  private JAXRSServerFactoryBean jaxrsServerFactoryBean;

	  public CXFServerBootstrap(Application application)
	  {
		setupServer(application);
	  }

	  public override void start()
	  {
		try
		{
		  server.start();
		}
		catch (Exception e)
		{
		  throw new ServerBootstrapException(e);
		}
	  }

	  private void separateProvidersAndResources(Application application, IList<Type> resourceClasses, IList<object> providerInstances)
	  {
		ISet<Type> classes = application.Classes;

		foreach (Type clazz in classes)
		{
		  Annotation[] annotations = clazz.GetCustomAttributes(true);
		  foreach (Annotation annotation in annotations)
		  {
			if (annotation.annotationType().Equals(typeof(Provider)))
			{
			  // for providers we have to create an instance
			  try
			  {
				providerInstances.Add(System.Activator.CreateInstance(clazz));
				break;
			  }
			  catch (Exception e)
			  {
				throw new Exception(e);
			  }
			}
			else if (annotation.annotationType().Equals(typeof(Path)))
			{
			  resourceClasses.Add(clazz);
			  break;
			}
		  }
		}
	  }


	  private void setupServer(Application application)
	  {
		jaxrsServerFactoryBean = new JAXRSServerFactoryBean();
		IList<Type> resourceClasses = new List<Type>();
		IList<object> providerInstances = new List<object>();

		// separate the providers and resources from the application returned classes
		separateProvidersAndResources(application, resourceClasses, providerInstances);
		jaxrsServerFactoryBean.ResourceClasses = resourceClasses;
		jaxrsServerFactoryBean.Providers = providerInstances;

		// set up address
		Properties serverProperties = readProperties();
		propertyPort = serverProperties.getProperty(PORT_PROPERTY);
		jaxrsServerFactoryBean.Address = "http://localhost:" + propertyPort + ROOT_RESOURCE_PATH;

		// set start to false so create call does not start server
		jaxrsServerFactoryBean.Start = false;
		server = jaxrsServerFactoryBean.create();
	  }

	  public override void stop()
	  {
		try
		{
		  server.stop();
		  server.destroy();
		  // DO NOT DELETE LINE BELOW
		  // KILL'S all jetty threads, otherwise it will block the port and tomcat can't be bind to the address
		  jaxrsServerFactoryBean.Bus.shutdown(true);
		}
		catch (Exception e)
		{
		  throw new ServerBootstrapException(e);
		}
	  }
	}

}