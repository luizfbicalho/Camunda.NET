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
namespace org.camunda.bpm.engine.test.standalone.db
{
	using PurgeDatabaseAndCacheCmd = org.camunda.bpm.engine.impl.cmd.PurgeDatabaseAndCacheCmd;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using DbEntityManager = org.camunda.bpm.engine.impl.db.entitymanager.DbEntityManager;
	using DbBulkOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbBulkOperation;
	using DbOperationType = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperationType;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DatabasePurgeReport = org.camunda.bpm.engine.impl.management.DatabasePurgeReport;
	using PurgeReport = org.camunda.bpm.engine.impl.management.PurgeReport;
	using CachePurgeReport = org.camunda.bpm.engine.impl.persistence.deploy.cache.CachePurgeReport;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;


	/// <summary>
	/// Copy of the current (7.11) <seealso cref="PurgeDatabaseAndCacheCmd"/>. Used by
	/// <seealso cref="TestIgnoreSchemaLogTableOnPurgePlugin"/> to ensure that the new database
	/// table ACT_SCHEMA_LOG is ignored in purge reports. This is necessary to run
	/// the test suite on a 7.10 engine with the 7.11 schema.
	/// TODO: Remove this after the 7.11 release.
	/// 
	/// @author Miklas Boskamp
	/// 
	/// </summary>
	[Serializable]
	public class TestIgnoreSchemaLogTablePurgeDatabaseAndCacheCmd : Command<PurgeReport>
	{

	  protected internal const string DELETE_TABLE_DATA = "deleteTableData";
	  protected internal const string SELECT_TABLE_COUNT = "selectTableCount";
	  protected internal const string TABLE_NAME = "tableName";
	  protected internal const string EMPTY_STRING = "";

	  public static readonly IList<string> TABLENAMES_EXCLUDED_FROM_DB_CLEAN_CHECK = Arrays.asList("ACT_GE_PROPERTY", "ACT_GE_SCHEMA_LOG");

	  public virtual PurgeReport execute(CommandContext commandContext)
	  {
		PurgeReport purgeReport = new PurgeReport();

		// purge the database
		DatabasePurgeReport databasePurgeReport = purgeDatabase(commandContext);
		purgeReport.DatabasePurgeReport = databasePurgeReport;

		// purge the deployment cache
		DeploymentCache deploymentCache = commandContext.ProcessEngineConfiguration.DeploymentCache;
		CachePurgeReport cachePurgeReport = deploymentCache.purgeCache();
		purgeReport.CachePurgeReport = cachePurgeReport;

		return purgeReport;
	  }

	  private DatabasePurgeReport purgeDatabase(CommandContext commandContext)
	  {
		DbEntityManager dbEntityManager = commandContext.DbEntityManager;
		// For MySQL and MariaDB we have to disable foreign key check,
		// to delete the table data as bulk operation (execution, incident etc.)
		// The flag will be reset by the DBEntityManager after flush.
		dbEntityManager.IgnoreForeignKeysForNextFlush = true;
		IList<string> tablesNames = dbEntityManager.TableNamesPresentInDatabase;
		string databaseTablePrefix = commandContext.ProcessEngineConfiguration.DatabaseTablePrefix.Trim();

		// for each table
		DatabasePurgeReport databasePurgeReport = new DatabasePurgeReport();
		foreach (string tableName in tablesNames)
		{
		  string tableNameWithoutPrefix = tableName.Replace(databaseTablePrefix, EMPTY_STRING);
		  if (!TABLENAMES_EXCLUDED_FROM_DB_CLEAN_CHECK.Contains(tableNameWithoutPrefix))
		  {

			// Check if table contains data
			IDictionary<string, string> param = new Dictionary<string, string>();
			param[TABLE_NAME] = tableName;
			long? count = (long?) dbEntityManager.selectOne(SELECT_TABLE_COUNT, param);

			if (count > 0)
			{
			  databasePurgeReport.addPurgeInformation(tableName, count);
			  // Get corresponding entity classes for the table, which contains data
			  IList<Type> entities = commandContext.TableDataManager.getEntities(tableName);

			  if (entities.Count == 0)
			  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new ProcessEngineException("No mapped implementation of " + typeof(DbEntity).FullName + " was found for: " + tableName);
			  }

			  // Delete the table data as bulk operation with the first entity
			  Type entity = entities[0];
			  DbBulkOperation deleteBulkOp = new DbBulkOperation(DbOperationType.DELETE_BULK, entity, DELETE_TABLE_DATA, param);
			  dbEntityManager.DbOperationManager.addOperation(deleteBulkOp);
			}
		  }
		}
		return databasePurgeReport;
	  }
	}

}