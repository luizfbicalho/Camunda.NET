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

	using Cache = org.camunda.bpm.engine.rest.cache.Cache;

	using JsonNode = com.fasterxml.jackson.databind.JsonNode;
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class HalRelationCacheConfiguration
	{

	  public const string CONFIG_CACHE_IMPLEMENTATION = "cacheImplementation";
	  public const string CONFIG_CACHES = "caches";

	  protected internal ObjectMapper objectMapper = new ObjectMapper();
	  protected internal Type cacheImplementationClass;
	  protected internal IDictionary<Type, IDictionary<string, object>> cacheConfigurations;

	  public HalRelationCacheConfiguration()
	  {
		cacheConfigurations = new Dictionary<Type, IDictionary<string, object>>();
	  }

	  public HalRelationCacheConfiguration(string configuration) : this()
	  {
		parseConfiguration(configuration);
	  }

	  public virtual Type CacheImplementationClass
	  {
		  get
		  {
			return cacheImplementationClass;
		  }
		  set
		  {
			if (value.IsAssignableFrom(typeof(Cache)))
			{
			  this.cacheImplementationClass = (Type) value;
			}
			else
			{
	//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			  throw new HalRelationCacheConfigurationException("Cache implementation class " + value.FullName + " does not implement the interface " + typeof(Cache).FullName);
			}
		  }
	  }


	  public virtual IDictionary<Type, IDictionary<string, object>> CacheConfigurations
	  {
		  get
		  {
			return cacheConfigurations;
		  }
		  set
		  {
			this.cacheConfigurations = value;
		  }
	  }


	  public virtual void addCacheConfiguration(Type halResourceClass, IDictionary<string, object> cacheConfiguration)
	  {
		this.cacheConfigurations[halResourceClass] = cacheConfiguration;
	  }

	  protected internal virtual void parseConfiguration(string configuration)
	  {
		try
		{
		  JsonNode jsonConfiguration = objectMapper.readTree(configuration);
		  parseConfiguration(jsonConfiguration);
		}
		catch (IOException e)
		{
		  throw new HalRelationCacheConfigurationException("Unable to parse cache configuration", e);
		}
	  }

	  protected internal virtual void parseConfiguration(JsonNode jsonConfiguration)
	  {
		parseCacheImplementationClass(jsonConfiguration);
		parseCacheConfigurations(jsonConfiguration);
	  }

	  protected internal virtual void parseCacheImplementationClass(JsonNode jsonConfiguration)
	  {
		JsonNode jsonNode = jsonConfiguration.get(CONFIG_CACHE_IMPLEMENTATION);
		if (jsonNode != null)
		{
		  string cacheImplementationClassName = jsonNode.textValue();
		  Type cacheImplementationClass = loadClass(cacheImplementationClassName);
		  CacheImplementationClass = cacheImplementationClass;
		}
		else
		{
		  throw new HalRelationCacheConfigurationException("Unable to find the " + CONFIG_CACHE_IMPLEMENTATION + " parameter");
		}
	  }

	  protected internal virtual void parseCacheConfigurations(JsonNode jsonConfiguration)
	  {
		JsonNode jsonNode = jsonConfiguration.get(CONFIG_CACHES);
		if (jsonNode != null)
		{
		  IEnumerator<KeyValuePair<string, JsonNode>> cacheConfigurations = jsonNode.fields();
		  while (cacheConfigurations.MoveNext())
		  {
			KeyValuePair<string, JsonNode> cacheConfiguration = cacheConfigurations.Current;
			parseCacheConfiguration(cacheConfiguration.Key, cacheConfiguration.Value);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected void parseCacheConfiguration(String halResourceClassName, com.fasterxml.jackson.databind.JsonNode jsonConfiguration)
	  protected internal virtual void parseCacheConfiguration(string halResourceClassName, JsonNode jsonConfiguration)
	  {
		try
		{
		  Type halResourceClass = loadClass(halResourceClassName);
		  IDictionary<string, object> configuration = objectMapper.treeToValue(jsonConfiguration, typeof(System.Collections.IDictionary));
		  addCacheConfiguration(halResourceClass, configuration);
		}
		catch (IOException)
		{
		  throw new HalRelationCacheConfigurationException("Unable to parse cache configuration for HAL resource " + halResourceClassName);
		}
	  }

	  protected internal virtual Type loadClass(string className)
	  {
		try
		{
		  // use classloader which loaded the REST api classes
		  return Type.GetType(className, true, typeof(HalRelationCacheConfiguration).ClassLoader);
		}
		catch (ClassNotFoundException e)
		{
		  throw new HalRelationCacheConfigurationException("Unable to load class of cache configuration " + className, e);
		}
	  }

	}

}