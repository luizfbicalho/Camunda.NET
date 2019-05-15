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
namespace org.camunda.bpm.container.impl.jboss.util
{

	/// <summary>
	/// Utility methods to manipulate the Current Thread Context Classloader
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public class Tccl
	{

	  public interface Operation<T>
	  {
		T run();
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static <T> T runUnderClassloader(final Operation<T> operation, final ClassLoader classLoader)
	  public static T runUnderClassloader<T>(Operation<T> operation, ClassLoader classLoader)
	  {
		SecurityManager sm = System.SecurityManager;
		if (sm != null)
		{
		  return AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClass(operation, classLoader));
		}
		else
		{
		  return runWithTccl(operation, classLoader);
		}
	  }

	  private class PrivilegedActionAnonymousInnerClass : PrivilegedAction<T>
	  {
		  private org.camunda.bpm.container.impl.jboss.util.Tccl.Operation<T> operation;
		  private ClassLoader classLoader;

		  public PrivilegedActionAnonymousInnerClass(org.camunda.bpm.container.impl.jboss.util.Tccl.Operation<T> operation, ClassLoader classLoader)
		  {
			  this.operation = operation;
			  this.classLoader = classLoader;
		  }

		  public T run()
		  {
			try
			{
			  return runWithTccl(operation, classLoader);
			}
			catch (Exception e)
			{
			  throw new Exception(e);
			}
		  }
	  }

	  private static T runWithTccl<T>(Operation<T> operation, ClassLoader classLoader)
	  {
		ClassLoader cl = Thread.CurrentThread.ContextClassLoader;
		Thread.CurrentThread.ContextClassLoader = classLoader;
		try
		{
		  return operation.run();
		}
		finally
		{
		  Thread.CurrentThread.ContextClassLoader = cl;
		}
	  }

	}
}