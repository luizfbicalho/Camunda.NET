using System.Collections.Generic;
using System.Text;

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
namespace org.camunda.bpm.container.impl.jboss.config
{

	using ProcessEnginePluginXml = org.camunda.bpm.container.impl.metadata.spi.ProcessEnginePluginXml;
	using ProcessEngineException = org.camunda.bpm.engine.ProcessEngineException;

	/// <summary>
	/// @author Daniel Meyer
	/// @author Thorben Lindhauer
	/// </summary>
	public class ManagedProcessEngineMetadata
	{

	  /// <summary>
	  /// indicates whether the process engine should automatically create / 
	  /// update the database schema upon startup 
	  /// </summary>
	  public static string PROP_IS_AUTO_SCHEMA_UPDATE = "isAutoSchemaUpdate";

	  /// <summary>
	  /// indicates whether the identity module is used and if this tables are
	  ///  required 
	  /// </summary>
	  public static string PROP_IS_IDENTITY_USED = "isIdentityUsed";

	  /// <summary>
	  /// indicates whether the job executor should be automatically activated </summary>
	  public static string PROP_IS_ACTIVATE_JOB_EXECUTOR = "isActivateJobExecutor";

	  /// <summary>
	  /// the prefix to be used for all process engine database tables </summary>
	  public static string PROP_DB_TABLE_PREFIX = "dbTablePrefix";

	  /// <summary>
	  /// the name of the platform job executor acquisition to use </summary>
	  public static string PROP_JOB_EXECUTOR_ACQUISITION_NAME = "jobExecutorAcquisitionName";

	  private bool isDefault;
	  private string engineName;
	  private string datasourceJndiName;
	  private string historyLevel;
	  protected internal string configuration;
	  private IDictionary<string, string> configurationProperties;
	  private IDictionary<string, string> foxLegacyProperties;
	  private IList<ProcessEnginePluginXml> pluginConfigurations;

	  /// <param name="isDefault"> </param>
	  /// <param name="engineName"> </param>
	  /// <param name="datasourceJndiName"> </param>
	  /// <param name="historyLevel"> </param>
	  /// <param name="configuration"> </param>
	  /// <param name="properties"> </param>
	  /// <param name="pluginConfigurations"> </param>
	  public ManagedProcessEngineMetadata(bool isDefault, string engineName, string datasourceJndiName, string historyLevel, string configuration, IDictionary<string, string> properties, IList<ProcessEnginePluginXml> pluginConfigurations)
	  {
		this.isDefault = isDefault;
		this.engineName = engineName;
		this.datasourceJndiName = datasourceJndiName;
		this.historyLevel = historyLevel;
		this.configuration = configuration;
		this.configurationProperties = selectProperties(properties, false);
		this.foxLegacyProperties = selectProperties(properties, true);
		this.pluginConfigurations = pluginConfigurations;
	  }

	  public virtual bool Default
	  {
		  get
		  {
			return isDefault;
		  }
		  set
		  {
			this.isDefault = value;
		  }
	  }


	  public virtual string EngineName
	  {
		  get
		  {
			return engineName;
		  }
		  set
		  {
			this.engineName = value;
		  }
	  }


	  public virtual string DatasourceJndiName
	  {
		  get
		  {
			return datasourceJndiName;
		  }
		  set
		  {
			this.datasourceJndiName = value;
		  }
	  }


	  public virtual string HistoryLevel
	  {
		  get
		  {
			return historyLevel;
		  }
		  set
		  {
			this.historyLevel = value;
		  }
	  }


	  public virtual string Configuration
	  {
		  get
		  {
			return configuration;
		  }
		  set
		  {
			this.configuration = value;
		  }
	  }


	  public virtual IDictionary<string, string> ConfigurationProperties
	  {
		  get
		  {
			return configurationProperties;
		  }
		  set
		  {
			this.configurationProperties = value;
		  }
	  }


	  public virtual IDictionary<string, string> FoxLegacyProperties
	  {
		  get
		  {
			return foxLegacyProperties;
		  }
		  set
		  {
			this.foxLegacyProperties = value;
		  }
	  }


	  public virtual IList<ProcessEnginePluginXml> PluginConfigurations
	  {
		  get
		  {
			return pluginConfigurations;
		  }
		  set
		  {
			this.pluginConfigurations = value;
		  }
	  }


	  public virtual bool IdentityUsed
	  {
		  get
		  {
			object @object = FoxLegacyProperties[PROP_IS_IDENTITY_USED];
			if (@object == null)
			{
			  return true;
			}
			else
			{
			  return bool.Parse((string) @object);
			}
		  }
	  }

	  public virtual bool AutoSchemaUpdate
	  {
		  get
		  {
			object @object = FoxLegacyProperties[PROP_IS_AUTO_SCHEMA_UPDATE];
			if (@object == null)
			{
			  return true;
			}
			else
			{
			  return bool.Parse((string) @object);
			}
		  }
	  }

	  public virtual bool ActivateJobExecutor
	  {
		  get
		  {
			object @object = FoxLegacyProperties[PROP_IS_ACTIVATE_JOB_EXECUTOR];
			if (@object == null)
			{
			  return true;
			}
			else
			{
			  return bool.Parse((string) @object);
			}
		  }
	  }

	  public virtual string DbTablePrefix
	  {
		  get
		  {
			object @object = FoxLegacyProperties[PROP_DB_TABLE_PREFIX];
			if (@object == null)
			{
			  return null;
			}
			else
			{
			  return (string) @object;
			}
		  }
	  }

	  public virtual string JobExecutorAcquisitionName
	  {
		  get
		  {
			object @object = FoxLegacyProperties[PROP_JOB_EXECUTOR_ACQUISITION_NAME];
			if (@object == null)
			{
			  return "default";
			}
			else
			{
			  return (string) @object;
			}
		  }
	  }

	  /// <summary>
	  /// validates the configuration and throws <seealso cref="ProcessEngineException"/> 
	  /// if the configuration is invalid.
	  /// </summary>
	  public virtual void validate()
	  {
		StringBuilder validationErrorBuilder = new StringBuilder("Process engine configuration is invalid: \n");
		bool isValid = true;

		if (string.ReferenceEquals(datasourceJndiName, null) || datasourceJndiName.Length == 0)
		{
		  isValid = false;
		  validationErrorBuilder.Append(" property 'datasource' cannot be null \n");
		}
		if (string.ReferenceEquals(engineName, null) || engineName.Length == 0)
		{
		  isValid = false;
		  validationErrorBuilder.Append(" property 'engineName' cannot be null \n");
		}

		for (int i = 0; i < pluginConfigurations.Count; i++)
		{
		  ProcessEnginePluginXml pluginConfiguration = pluginConfigurations[i];
		  if (string.ReferenceEquals(pluginConfiguration.PluginClass, null) || pluginConfiguration.PluginClass.Length == 0)
		  {
			isValid = false;
			validationErrorBuilder.Append(" property 'class' in plugin[" + i + "] cannot be null \n");
		  }
		}

		if (!isValid)
		{
		  throw new ProcessEngineException(validationErrorBuilder.ToString());
		}
	  }

	  private IDictionary<string, string> selectProperties(IDictionary<string, string> allProperties, bool selectFoxProperties)
	  {
		IDictionary<string, string> result = null;
		if (selectFoxProperties)
		{
		  result = new Dictionary<string, string>();
		  string isAutoSchemaUpdate = allProperties[PROP_IS_AUTO_SCHEMA_UPDATE];
		  string isActivateJobExecutor = allProperties[PROP_IS_ACTIVATE_JOB_EXECUTOR];
		  string isIdentityUsed = allProperties[PROP_IS_IDENTITY_USED];
		  string dbTablePrefix = allProperties[PROP_DB_TABLE_PREFIX];
		  string jobExecutorAcquisitionName = allProperties[PROP_JOB_EXECUTOR_ACQUISITION_NAME];

		  if (!string.ReferenceEquals(isAutoSchemaUpdate, null))
		  {
			result[PROP_IS_AUTO_SCHEMA_UPDATE] = isAutoSchemaUpdate;
		  }
		  if (!string.ReferenceEquals(isActivateJobExecutor, null))
		  {
			result[PROP_IS_ACTIVATE_JOB_EXECUTOR] = isActivateJobExecutor;
		  }
		  if (!string.ReferenceEquals(isIdentityUsed, null))
		  {
			result[PROP_IS_IDENTITY_USED] = isIdentityUsed;
		  }
		  if (!string.ReferenceEquals(dbTablePrefix, null))
		  {
			result[PROP_DB_TABLE_PREFIX] = dbTablePrefix;
		  }
		  if (!string.ReferenceEquals(jobExecutorAcquisitionName, null))
		  {
			result[PROP_JOB_EXECUTOR_ACQUISITION_NAME] = jobExecutorAcquisitionName;
		  }
		}
		else
		{
		  result = new Dictionary<string, string>(allProperties);
		  result.Remove(PROP_IS_AUTO_SCHEMA_UPDATE);
		  result.Remove(PROP_IS_ACTIVATE_JOB_EXECUTOR);
		  result.Remove(PROP_IS_IDENTITY_USED);
		  result.Remove(PROP_DB_TABLE_PREFIX);
		  result.Remove(PROP_JOB_EXECUTOR_ACQUISITION_NAME);
		}
		return result;
	  }

	}

}