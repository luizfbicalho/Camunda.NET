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
namespace org.camunda.bpm.engine.impl.cfg
{


	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ConfigurationLogger : ProcessEngineLogger
	{

	  public virtual ProcessEngineException invalidConfigTransactionManagerIsNull()
	  {
		return new ProcessEngineException(exceptionMessage("001", "Property 'transactionManager' is null and 'transactionManagerJndiName' is not set. " + "Please set either the 'transactionManager' property or the 'transactionManagerJndiName' property."));
	  }

	  public virtual ProcessEngineException invalidConfigCannotFindTransactionManger(string jndiName, NamingException e)
	  {
		return new ProcessEngineException(exceptionMessage("002", "Cannot lookup instance of Jta Transaction manager in JNDI using name '{}'", jndiName), e);
	  }

	  public virtual void pluginActivated(string pluginName, string processEngineName)
	  {
		logInfo("003", "Plugin '{}' activated on process engine '{}'", pluginName, processEngineName);
	  }

	  public virtual void debugDatabaseproductName(string databaseProductName)
	  {
		logDebug("004", "Database product name {}", databaseProductName);
	  }

	  public virtual void debugDatabaseType(string databaseType)
	  {
		logDebug("005", "Database type {}", databaseType);
	  }

	  public virtual void usingDeprecatedHistoryLevelVariable()
	  {
		logWarn("006", "Using deprecated history level 'variable'. " + "This history level is deprecated and replaced by 'activity'. " + "Consider using 'ACTIVITY' instead.");
	  }

	  public virtual ProcessEngineException invalidConfigDefaultUserPermissionNameForTask(string defaultUserPermissionNameForTask, string[] validPermissionNames)
	  {
		return new ProcessEngineException(exceptionMessage("007", "Invalid value '{}' for configuration property 'defaultUserPermissionNameForTask'. Valid values are: '{}'", defaultUserPermissionNameForTask, validPermissionNames));
	  }

	  public virtual ProcessEngineException invalidPropertyValue(string propertyName, string propertyValue)
	  {
		return new ProcessEngineException(exceptionMessage("008", "Invalid value '{}' for configuration property '{}'.", propertyValue, propertyName));
	  }

	  public virtual ProcessEngineException invalidPropertyValue(string propertyName, string propertyValue, string reason)
	  {
		return new ProcessEngineException(exceptionMessage("009", "Invalid value '{}' for configuration property '{}': {}.", propertyValue, propertyName, reason));
	  }

	  public virtual void invalidBatchOperation(string operation, string historyTimeToLive)
	  {
		logWarn("010", "Invalid batch operation name '{}' with history time to live set to'{}'", operation, historyTimeToLive);
	  }

	  public virtual ProcessEngineException invalidPropertyValue(string propertyName, string propertyValue, Exception e)
	  {
		return new ProcessEngineException(exceptionMessage("011", "Invalid value '{}' for configuration property '{}'.", propertyValue, propertyName), e);
	  }


	}

}