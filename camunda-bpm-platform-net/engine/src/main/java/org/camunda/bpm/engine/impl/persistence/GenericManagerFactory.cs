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
namespace org.camunda.bpm.engine.impl.persistence
{
	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;
	using Session = org.camunda.bpm.engine.impl.interceptor.Session;
	using SessionFactory = org.camunda.bpm.engine.impl.interceptor.SessionFactory;
	using ReflectUtil = org.camunda.bpm.engine.impl.util.ReflectUtil;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class GenericManagerFactory : SessionFactory
	{

	  protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;

	  protected internal Type managerImplementation;

	  public GenericManagerFactory(Type managerImplementation)
	  {
		this.managerImplementation = managerImplementation;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public GenericManagerFactory(String classname)
	  public GenericManagerFactory(string classname)
	  {
		managerImplementation = (Type) ReflectUtil.loadClass(classname);
	  }

	  public virtual Type SessionType
	  {
		  get
		  {
			return managerImplementation;
		  }
	  }

	  public virtual Session openSession()
	  {
		try
		{
		  return System.Activator.CreateInstance(managerImplementation);
		}
		catch (Exception e)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw LOG.instantiateSessionException(managerImplementation.FullName, e);
		}
	  }
	}

}