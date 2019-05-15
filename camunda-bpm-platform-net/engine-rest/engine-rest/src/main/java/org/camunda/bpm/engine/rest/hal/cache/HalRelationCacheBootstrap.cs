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
namespace org.camunda.bpm.engine.rest.hal.cache
{


	using ReflectUtil = org.camunda.bpm.engine.impl.util.ReflectUtil;
	using Cache = org.camunda.bpm.engine.rest.cache.Cache;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class HalRelationCacheBootstrap : ServletContextListener
	{

	  public const string CONTEXT_PARAM_NAME = "org.camunda.bpm.engine.rest.hal.cache.config";

	  protected internal ObjectMapper objectMapper = new ObjectMapper();

	  public virtual void contextInitialized(ServletContextEvent sce)
	  {
		string contextParameter = sce.ServletContext.getInitParameter(CONTEXT_PARAM_NAME);
		if (!string.ReferenceEquals(contextParameter, null))
		{
		  configureCaches(contextParameter);
		}
	  }

	  public virtual void contextDestroyed(ServletContextEvent sce)
	  {
		Hal.Instance.destroyHalRelationCaches();
	  }

	  public virtual void configureCaches(string contextParameter)
	  {
		HalRelationCacheConfiguration configuration = new HalRelationCacheConfiguration(contextParameter);
		configureCaches(configuration);
	  }

	  public virtual void configureCaches(HalRelationCacheConfiguration configuration)
	  {
		Type cacheClass = configuration.CacheImplementationClass;
		foreach (KeyValuePair<Type, IDictionary<string, object>> cacheConfiguration in configuration.CacheConfigurations.SetOfKeyValuePairs())
		{
		  Cache cache = createCache(cacheClass, cacheConfiguration.Value);
		  registerCache(cacheConfiguration.Key, cache);
		}
	  }

	  protected internal virtual Cache createCache(Type cacheClass, IDictionary<string, object> cacheConfiguration)
	  {
		Cache cache = createCacheInstance(cacheClass);
		configureCache(cache, cacheConfiguration);
		return cache;
	  }

	  protected internal virtual void configureCache(Cache cache, IDictionary<string, object> cacheConfiguration)
	  {
		foreach (KeyValuePair<string, object> configuration in cacheConfiguration.SetOfKeyValuePairs())
		{
		  configureCache(cache, configuration.Key, configuration.Value);
		}
	  }

	  protected internal virtual Cache createCacheInstance(Type cacheClass)
	  {
		try
		{
		  return ReflectUtil.instantiate(cacheClass);
		}
		catch (ProcessEngineException e)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new HalRelationCacheConfigurationException("Unable to instantiate cache class " + cacheClass.FullName, e);
		}
	  }

	  protected internal virtual void configureCache(Cache cache, string property, object value)
	  {
		System.Reflection.MethodInfo setter;
		try
		{
		  setter = ReflectUtil.getSingleSetter(property, cache.GetType());
		}
		catch (ProcessEngineException e)
		{
		  throw new HalRelationCacheConfigurationException("Unable to find setter for property " + property, e);
		}

		if (setter == null)
		{
		  throw new HalRelationCacheConfigurationException("Unable to find setter for property " + property);
		}

		try
		{
		  setter.invoke(cache, value);
		}
		catch (IllegalAccessException)
		{
		  throw new HalRelationCacheConfigurationException("Unable to access setter for property " + property);
		}
		catch (InvocationTargetException)
		{
		  throw new HalRelationCacheConfigurationException("Unable to invoke setter for property " + property);
		}
	  }

	  protected internal virtual void registerCache(Type halResourceClass, Cache cache)
	  {
		Hal.Instance.registerHalRelationCache(halResourceClass, cache);
	  }

	}

}