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
namespace org.camunda.bpm.application.impl.web
{

	using DefaultEjbProcessApplication = org.camunda.bpm.application.impl.ejb.DefaultEjbProcessApplication;


	/// <summary>
	/// <para>Sets the ProcessApplicationInfo.PROP_SERVLET_CONTEXT_PATH property if this is 
	/// deployed as part of a WebApplication.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ProcessArchiveServletContextListener : ServletContextListener
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @EJB private org.camunda.bpm.application.ProcessApplicationInterface defaultEjbProcessApplication;
	  private ProcessApplicationInterface defaultEjbProcessApplication;

	  public virtual void contextInitialized(ServletContextEvent contextEvent)
	  {

		string contextPath = contextEvent.ServletContext.ContextPath;

		defaultEjbProcessApplication.Properties[org.camunda.bpm.application.ProcessApplicationInfo_Fields.PROP_SERVLET_CONTEXT_PATH] = contextPath;

	  }

	  public virtual void contextDestroyed(ServletContextEvent arg0)
	  {
	  }

	}

}