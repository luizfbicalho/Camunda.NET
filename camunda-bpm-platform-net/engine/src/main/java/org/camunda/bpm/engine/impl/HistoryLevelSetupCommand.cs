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
namespace org.camunda.bpm.engine.impl
{
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using DetermineHistoryLevelCmd = org.camunda.bpm.engine.impl.cmd.DetermineHistoryLevelCmd;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;
	using DbEntityManager = org.camunda.bpm.engine.impl.db.entitymanager.DbEntityManager;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using PropertyEntity = org.camunda.bpm.engine.impl.persistence.entity.PropertyEntity;

	public sealed class HistoryLevelSetupCommand : Command<Void>
	{

	  private static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;

	  public Void execute(CommandContext commandContext)
	  {

		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;

		checkStartupLockExists(commandContext);

		HistoryLevel databaseHistoryLevel = (new DetermineHistoryLevelCmd(processEngineConfiguration.HistoryLevels)).execute(commandContext);
		determineAutoHistoryLevel(processEngineConfiguration, databaseHistoryLevel);

		HistoryLevel configuredHistoryLevel = processEngineConfiguration.HistoryLevel;

		if (databaseHistoryLevel == null)
		{

		  commandContext.PropertyManager.acquireExclusiveLockForStartup();
		  databaseHistoryLevel = (new DetermineHistoryLevelCmd(processEngineConfiguration.HistoryLevels)).execute(commandContext);

		  if (databaseHistoryLevel == null)
		  {
			LOG.noHistoryLevelPropertyFound();
			dbCreateHistoryLevel(commandContext);
		  }
		}
		else
		{
		  if (configuredHistoryLevel.Id != databaseHistoryLevel.Id)
		  {
			throw new ProcessEngineException("historyLevel mismatch: configuration says " + configuredHistoryLevel + " and database says " + databaseHistoryLevel);
		  }
		}

		return null;
	  }

	  public static void dbCreateHistoryLevel(CommandContext commandContext)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
		HistoryLevel configuredHistoryLevel = processEngineConfiguration.HistoryLevel;
		PropertyEntity property = new PropertyEntity("historyLevel", Convert.ToString(configuredHistoryLevel.Id));
		commandContext.getSession(typeof(DbEntityManager)).insert(property);
		LOG.creatingHistoryLevelPropertyInDatabase(configuredHistoryLevel);
	  }

	  /// 
	  /// <returns> Integer value representing the history level or <code>null</code> if none found </returns>
	  public static int? databaseHistoryLevel(CommandContext commandContext)
	  {

		try
		{
		  PropertyEntity historyLevelProperty = commandContext.PropertyManager.findPropertyById("historyLevel");
		  return historyLevelProperty != null ? new int?(historyLevelProperty.Value) : null;
		}
		catch (Exception e)
		{
		  LOG.couldNotSelectHistoryLevel(e.Message);
		  return null;
		}

	  }

	  protected internal void determineAutoHistoryLevel(ProcessEngineConfigurationImpl engineConfiguration, HistoryLevel databaseHistoryLevel)
	  {
		HistoryLevel configuredHistoryLevel = engineConfiguration.HistoryLevel;

		if (configuredHistoryLevel == null && ProcessEngineConfiguration.HISTORY_AUTO.Equals(engineConfiguration.History))
		{

		  // automatically determine history level or use default AUDIT
		  if (databaseHistoryLevel != null)
		  {
			engineConfiguration.HistoryLevel = databaseHistoryLevel;
		  }
		  else
		  {
			engineConfiguration.HistoryLevel = engineConfiguration.DefaultHistoryLevel;
		  }
		}
	  }

	  protected internal void checkStartupLockExists(CommandContext commandContext)
	  {
		PropertyEntity historyStartupProperty = commandContext.PropertyManager.findPropertyById("startup.lock");
		if (historyStartupProperty == null)
		{
		  LOG.noStartupLockPropertyFound();
		}
	  }

	}

}