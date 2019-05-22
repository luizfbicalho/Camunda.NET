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
namespace org.camunda.bpm.engine.cdi.impl.util
{


	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;

	/// <summary>
	/// Utility class for performing programmatic bean lookups.
	/// 
	/// @author Daniel Meyer
	/// @author Mark Struberg
	/// </summary>
	public class ProgrammaticBeanLookup
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  public static readonly Logger LOG = Logger.getLogger(typeof(ProgrammaticBeanLookup).FullName);

	  public static T lookup<T>(Type clazz, BeanManager bm)
	  {
			  clazz = typeof(T);
		return lookup(clazz, bm, true);
	  }

	  public static T lookup<T>(Type clazz, BeanManager bm, bool optional)
	  {
			  clazz = typeof(T);
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.Set<javax.enterprise.inject.spi.Bean<?>> beans = bm.getBeans(clazz);
		ISet<Bean<object>> beans = bm.getBeans(clazz);
		T instance = getContextualReference(bm, beans, clazz);
		if (!optional && instance == default(T))
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new System.InvalidOperationException("CDI BeanManager cannot find an instance of requested type '" + clazz.FullName + "'");
		}
		return instance;
	  }

	  public static object lookup(string name, BeanManager bm)
	  {
		return lookup(name, bm, true);
	  }

	  public static object lookup(string name, BeanManager bm, bool optional)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.Set<javax.enterprise.inject.spi.Bean<?>> beans = bm.getBeans(name);
		ISet<Bean<object>> beans = bm.getBeans(name);

		// NOTE: we use Object.class as BeanType of the ContextualReference to resolve.
		// Mark says this is not strictly spec compliant but should work on all implementations.
		// A strictly compliant implementation would
		// - collect all bean types of the bean
		// - calculate the type such that it has the most types in the set of bean types which are assignable from this type.
		object instance = getContextualReference(bm, beans, typeof(object));
		if (!optional && instance == null)
		{
		  throw new System.InvalidOperationException("CDI BeanManager cannot find an instance of requested type '" + name + "'");
		}
		return instance;
	  }

	  /// <returns> a ContextualInstance of the given type </returns>
	  /// <exception cref="javax.enterprise.inject.AmbiguousResolutionException"> if the given type is satisfied by more than one Bean </exception>
	  /// <seealso cref= #lookup(Class, boolean) </seealso>
	  public static T lookup<T>(Type clazz)
	  {
			  clazz = typeof(T);
		return lookup(clazz, true);
	  }

	  /// <param name="optional"> if <code>false</code> then the bean must exist. </param>
	  /// <returns> a ContextualInstance of the given type if optional is <code>false</code>. If optional is <code>true</code> null might be returned if no bean got found. </returns>
	  /// <exception cref="IllegalStateException"> if there is no bean of the given class, but only if optional is <code>false</code> </exception>
	  /// <exception cref="javax.enterprise.inject.AmbiguousResolutionException"> if the given type is satisfied by more than one Bean </exception>
	  /// <seealso cref= #lookup(Class, boolean) </seealso>
	  public static T lookup<T>(Type clazz, bool optional)
	  {
			  clazz = typeof(T);
		BeanManager bm = BeanManagerLookup.BeanManager;
		return lookup(clazz, bm, optional);
	  }

	  public static object lookup(string name)
	  {
		BeanManager bm = BeanManagerLookup.BeanManager;
		return lookup(name, bm);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private static <T> T getContextualReference(javax.enterprise.inject.spi.BeanManager bm, java.util.Set<javax.enterprise.inject.spi.Bean<?>> beans, Class type)
	  private static T getContextualReference<T, T1>(BeanManager bm, ISet<T1> beans, Type type)
	  {
		if (beans == null || beans.Count == 0)
		{
		  return null;
		}

		// if we would resolve to multiple beans then BeanManager#resolve would throw an AmbiguousResolutionException
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: javax.enterprise.inject.spi.Bean<?> bean = bm.resolve(beans);
		Bean<object> bean = bm.resolve(beans);
		if (bean == null)
		{
		  return null;

		}
		else
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: javax.enterprise.context.spi.CreationalContext<?> creationalContext = bm.createCreationalContext(bean);
		  CreationalContext<object> creationalContext = bm.createCreationalContext(bean);

		  // if we obtain a contextual reference to a @Dependent scope bean, make sure it is released
		  if (isDependentScoped(bean))
		  {
			releaseOnContextClose(creationalContext, bean);
		  }

		  return (T) bm.getReference(bean, type, creationalContext);

		}
	  }

	  private static bool isDependentScoped<T1>(Bean<T1> bean)
	  {
		return typeof(Dependent).Equals(bean.Scope);
	  }

	  private static void releaseOnContextClose<T1, T2>(CreationalContext<T1> creationalContext, Bean<T2> bean)
	  {
		CommandContext commandContext = Context.CommandContext;
		if (commandContext != null)
		{
		  commandContext.registerCommandContextListener(new CreationalContextReleaseListener(creationalContext));

		}
		else
		{
		  LOG.warning("Obtained instance of @Dependent scoped bean " + bean + " outside of process engine command context. " + "Bean instance will not be destroyed. This is likely to create a memory leak. Please use a normal scope like @ApplicationScoped for this bean.");

		}
	  }

	}

}