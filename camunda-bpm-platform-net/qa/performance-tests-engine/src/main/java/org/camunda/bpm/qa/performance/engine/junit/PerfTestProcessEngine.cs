using System;
using System.Collections.Generic;
using System.IO;

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
namespace org.camunda.bpm.qa.performance.engine.junit
{

	using DataSource = org.apache.tomcat.jdbc.pool.DataSource;
	using PoolProperties = org.apache.tomcat.jdbc.pool.PoolProperties;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using ProcessEngineConfiguration = org.camunda.bpm.engine.ProcessEngineConfiguration;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ProcessEnginePlugin = org.camunda.bpm.engine.impl.cfg.ProcessEnginePlugin;
	using StandaloneProcessEngineConfiguration = org.camunda.bpm.engine.impl.cfg.StandaloneProcessEngineConfiguration;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using ReflectUtil = org.camunda.bpm.engine.impl.util.ReflectUtil;
	using PerfTestException = org.camunda.bpm.qa.performance.engine.framework.PerfTestException;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class PerfTestProcessEngine
	{

	  public const string PROPERTIES_FILE_NAME = "perf-test-config.properties";

	  protected internal static ProcessEngine processEngine;

	  public static ProcessEngine Instance
	  {
		  get
		  {
			if (processEngine == null)
			{
    
			  // load properties
			  Properties properties = loadProperties();
			  javax.sql.DataSource datasource = createDatasource(properties);
			  processEngine = createProcessEngine(datasource, properties);
    
			}
			return processEngine;
		  }
	  }

	  protected internal static ProcessEngine createProcessEngine(javax.sql.DataSource datasource, Properties properties)
	  {

		ProcessEngineConfigurationImpl processEngineConfiguration = new StandaloneProcessEngineConfiguration();
		processEngineConfiguration.DataSource = datasource;
		processEngineConfiguration.DatabaseSchemaUpdate = ProcessEngineConfiguration.DB_SCHEMA_UPDATE_TRUE;

		processEngineConfiguration.History = properties.getProperty("historyLevel");

		processEngineConfiguration.JdbcBatchProcessing = Convert.ToBoolean(properties.getProperty("jdbcBatchProcessing"));

		// load plugins
		string processEnginePlugins = properties.getProperty("processEnginePlugins", "");
		foreach (string pluginName in processEnginePlugins.Split(",", true))
		{
		  if (pluginName.Length > 1)
		  {
			object pluginInstance = ReflectUtil.instantiate(pluginName);
			if (!(pluginInstance is ProcessEnginePlugin))
			{
			  throw new PerfTestException("Plugin " + pluginName + " is not an instance of ProcessEnginePlugin");

			}
			else
			{
			  IList<ProcessEnginePlugin> plugins = processEngineConfiguration.ProcessEnginePlugins;
			  if (plugins == null)
			  {
				plugins = new List<ProcessEnginePlugin>();
				processEngineConfiguration.ProcessEnginePlugins = plugins;
			  }
			  plugins.Add((ProcessEnginePlugin) pluginInstance);

			}
		  }
		}

		return processEngineConfiguration.buildProcessEngine();
	  }

	  public static Properties loadProperties()
	  {
		Stream propertyInputStream = null;
		try
		{
		  propertyInputStream = typeof(PerfTestProcessEngine).ClassLoader.getResourceAsStream(PROPERTIES_FILE_NAME);
		  Properties properties = new Properties();
		  properties.load(propertyInputStream);
		  return properties;

		}
		catch (Exception e)
		{
		  throw new PerfTestException("Cannot load properties from file " + PROPERTIES_FILE_NAME + ": " + e);

		}
		finally
		{
		  IoUtil.closeSilently(propertyInputStream);
		}
	  }

	  protected internal static javax.sql.DataSource createDatasource(Properties properties)
	  {

		PoolProperties p = new PoolProperties();
		p.Url = properties.getProperty("databaseUrl");
		p.DriverClassName = properties.getProperty("databaseDriver");
		p.Username = properties.getProperty("databaseUser");
		p.Password = properties.getProperty("databasePassword");

		p.JmxEnabled = false;

		p.MaxActive = 100;
		p.InitialSize = 10;

		DataSource datasource = new DataSource();
		datasource.PoolProperties = p;

		return datasource;
	  }

	}

}