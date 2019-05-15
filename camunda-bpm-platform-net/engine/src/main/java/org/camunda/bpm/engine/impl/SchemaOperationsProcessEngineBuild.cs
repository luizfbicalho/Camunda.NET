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
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;
	using PersistenceSession = org.camunda.bpm.engine.impl.db.PersistenceSession;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using PropertyEntity = org.camunda.bpm.engine.impl.persistence.entity.PropertyEntity;

	/// <summary>
	/// @author Tom Baeyens
	/// @author Roman Smirnov
	/// @author Sebastian Menski
	/// @author Daniel Meyer
	/// </summary>
	public class SchemaOperationsProcessEngineBuild : SchemaOperationsCommand
	{

	  private static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;

	  public virtual Void execute(CommandContext commandContext)
	  {
		string databaseSchemaUpdate = Context.ProcessEngineConfiguration.DatabaseSchemaUpdate;
		PersistenceSession persistenceSession = commandContext.getSession(typeof(PersistenceSession));
		if (ProcessEngineConfigurationImpl.DB_SCHEMA_UPDATE_DROP_CREATE.Equals(databaseSchemaUpdate))
		{
		  try
		  {
			persistenceSession.dbSchemaDrop();
		  }
		  catch (Exception)
		  {
			// ignore
		  }
		}
		if (ProcessEngineConfiguration.DB_SCHEMA_UPDATE_CREATE_DROP.Equals(databaseSchemaUpdate) || ProcessEngineConfigurationImpl.DB_SCHEMA_UPDATE_DROP_CREATE.Equals(databaseSchemaUpdate) || ProcessEngineConfigurationImpl.DB_SCHEMA_UPDATE_CREATE.Equals(databaseSchemaUpdate))
		{
		  persistenceSession.dbSchemaCreate();
		}
		else if (ProcessEngineConfiguration.DB_SCHEMA_UPDATE_FALSE.Equals(databaseSchemaUpdate))
		{
		  persistenceSession.dbSchemaCheckVersion();
		}
		else if (ProcessEngineConfiguration.DB_SCHEMA_UPDATE_TRUE.Equals(databaseSchemaUpdate))
		{
		  persistenceSession.dbSchemaUpdate();
		}

		return null;
	  }
	}

}