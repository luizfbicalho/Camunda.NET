using System;
using System.Threading;

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
namespace org.camunda.bpm.engine.impl.util
{

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ClassLoaderUtil
	{

	  public static ClassLoader ContextClassloader
	  {
		  get
		  {
			if (System.SecurityManager != null)
			{
			  return AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClass());
			}
			else
			{
			  return Thread.CurrentThread.ContextClassLoader;
			}
		  }
		  set
		  {
			if (System.SecurityManager != null)
			{
			  AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClass3(value));
			}
			else
			{
			  Thread.CurrentThread.ContextClassLoader = value;
			}
		  }
	  }

	  private class PrivilegedActionAnonymousInnerClass : PrivilegedAction<ClassLoader>
	  {
		  public ClassLoader run()
		  {
			return Thread.CurrentThread.ContextClassLoader;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static ClassLoader getClassloader(final Class clazz)
	  public static ClassLoader getClassloader(Type clazz)
	  {
		if (System.SecurityManager != null)
		{
		  return AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClass2(clazz));
		}
		else
		{
		  return clazz.ClassLoader;
		}
	  }

	  private class PrivilegedActionAnonymousInnerClass2 : PrivilegedAction<ClassLoader>
	  {
		  private Type clazz;

		  public PrivilegedActionAnonymousInnerClass2(Type clazz)
		  {
			  this.clazz = clazz;
		  }

		  public ClassLoader run()
		  {
			return clazz.ClassLoader;
		  }
	  }


	  private class PrivilegedActionAnonymousInnerClass3 : PrivilegedAction<Void>
	  {
		  private ClassLoader classLoader;

		  public PrivilegedActionAnonymousInnerClass3(ClassLoader classLoader)
		  {
			  this.classLoader = classLoader;
		  }

		  public Void run()
		  {
			Thread.CurrentThread.ContextClassLoader = classLoader;
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static ClassLoader getServletContextClassloader(final javax.servlet.ServletContextEvent sce)
	  public static ClassLoader getServletContextClassloader(ServletContextEvent sce)
	  {
		if (System.SecurityManager != null)
		{
		  return AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClass4(sce));
		}
		else
		{
		  return sce.ServletContext.ClassLoader;
		}
	  }

	  private class PrivilegedActionAnonymousInnerClass4 : PrivilegedAction<ClassLoader>
	  {
		  private ServletContextEvent sce;

		  public PrivilegedActionAnonymousInnerClass4(ServletContextEvent sce)
		  {
			  this.sce = sce;
		  }

		  public ClassLoader run()
		  {
			return sce.ServletContext.ClassLoader;
		  }
	  }

	}

}