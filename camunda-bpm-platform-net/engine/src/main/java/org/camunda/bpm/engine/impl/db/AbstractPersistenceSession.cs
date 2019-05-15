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
namespace org.camunda.bpm.engine.impl.db
{

	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DbBulkOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbBulkOperation;
	using DbEntityOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbEntityOperation;
	using DbOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperation;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public abstract class AbstractPersistenceSession : PersistenceSession
	{
		public abstract void close();
		public abstract void flush();
		public abstract void dbSchemaCheckVersion();
		public abstract void rollback();
		public abstract void commit();
		public abstract IList<org.apache.ibatis.executor.BatchResult> flushOperations();
		public abstract int executeNonEmptyUpdateStmt(string updateStmt, object parameter);
		public abstract int executeUpdate(string updateStatement, object parameter);
		public abstract void @lock(string statement, object parameter);
		public abstract object selectOne(string statement, object parameter);
		public abstract T selectById(Type<T> type, string id);
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public abstract java.util.List<JavaToDotNetGenericWildcard> selectList(String statement, Object parameter);
		public abstract IList<object> selectList(string statement, object parameter);

	  protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;
	  protected internal IList<EntityLoadListener> listeners = new List<EntityLoadListener>(1);

	  public virtual void executeDbOperation(DbOperation operation)
	  {
		switch (operation.OperationType)
		{

		  case INSERT:
			insertEntity((DbEntityOperation) operation);
			break;

		  case DELETE:
			deleteEntity((DbEntityOperation) operation);
			break;
		  case DELETE_BULK:
			deleteBulk((DbBulkOperation) operation);
			break;

		  case UPDATE:
			updateEntity((DbEntityOperation) operation);
			break;
		  case UPDATE_BULK:
			updateBulk((DbBulkOperation) operation);
			break;

		}
	  }

	  protected internal abstract void insertEntity(DbEntityOperation operation);

	  protected internal abstract void deleteEntity(DbEntityOperation operation);

	  protected internal abstract void deleteBulk(DbBulkOperation operation);

	  protected internal abstract void updateEntity(DbEntityOperation operation);

	  protected internal abstract void updateBulk(DbBulkOperation operation);

	  protected internal abstract string DbVersion {get;}

	  public virtual void dbSchemaCreate()
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;

		HistoryLevel configuredHistoryLevel = processEngineConfiguration.HistoryLevel;
		if ((!processEngineConfiguration.DbHistoryUsed) && (!configuredHistoryLevel.Equals(org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_NONE)))
		{
		  throw LOG.databaseHistoryLevelException(configuredHistoryLevel.Name);
		}

		if (EngineTablePresent)
		{
		  string dbVersion = DbVersion;
		  if (!org.camunda.bpm.engine.ProcessEngine_Fields.VERSION.Equals(dbVersion))
		  {
			throw LOG.wrongDbVersionException(org.camunda.bpm.engine.ProcessEngine_Fields.VERSION, dbVersion);
		  }
		}
		else
		{
		  dbSchemaCreateEngine();
		}

		if (processEngineConfiguration.DbHistoryUsed)
		{
		  dbSchemaCreateHistory();
		}

		if (processEngineConfiguration.DbIdentityUsed)
		{
		  dbSchemaCreateIdentity();
		}

		if (processEngineConfiguration.CmmnEnabled)
		{
		  dbSchemaCreateCmmn();
		}

		if (processEngineConfiguration.CmmnEnabled && processEngineConfiguration.DbHistoryUsed)
		{
		  dbSchemaCreateCmmnHistory();
		}

		if (processEngineConfiguration.DmnEnabled)
		{
		  dbSchemaCreateDmn();

		  if (processEngineConfiguration.DbHistoryUsed)
		  {
			dbSchemaCreateDmnHistory();
		  }
		}
	  }

	  protected internal abstract void dbSchemaCreateIdentity();

	  protected internal abstract void dbSchemaCreateHistory();

	  protected internal abstract void dbSchemaCreateEngine();

	  protected internal abstract void dbSchemaCreateCmmn();

	  protected internal abstract void dbSchemaCreateCmmnHistory();

	  protected internal abstract void dbSchemaCreateDmn();

	  protected internal abstract void dbSchemaCreateDmnHistory();


	  public virtual void dbSchemaDrop()
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;

		if (processEngineConfiguration.DmnEnabled)
		{
		  dbSchemaDropDmn();

		  if (processEngineConfiguration.DbHistoryUsed)
		  {
			dbSchemaDropDmnHistory();
		  }
		}

		if (processEngineConfiguration.CmmnEnabled)
		{
		  dbSchemaDropCmmn();
		}

		dbSchemaDropEngine();

		if (processEngineConfiguration.CmmnEnabled && processEngineConfiguration.DbHistoryUsed)
		{
		  dbSchemaDropCmmnHistory();
		}

		if (processEngineConfiguration.DbHistoryUsed)
		{
		  dbSchemaDropHistory();
		}

		if (processEngineConfiguration.DbIdentityUsed)
		{
		  dbSchemaDropIdentity();
		}
	  }

	  protected internal abstract void dbSchemaDropIdentity();

	  protected internal abstract void dbSchemaDropHistory();

	  protected internal abstract void dbSchemaDropEngine();

	  protected internal abstract void dbSchemaDropCmmn();

	  protected internal abstract void dbSchemaDropCmmnHistory();

	  protected internal abstract void dbSchemaDropDmn();

	  protected internal abstract void dbSchemaDropDmnHistory();

	  public virtual void dbSchemaPrune()
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
		if (HistoryTablePresent && !processEngineConfiguration.DbHistoryUsed)
		{
		  dbSchemaDropHistory();
		}
		if (IdentityTablePresent && !processEngineConfiguration.DbIdentityUsed)
		{
		  dbSchemaDropIdentity();
		}
		if (CmmnTablePresent && !processEngineConfiguration.CmmnEnabled)
		{
		  dbSchemaDropCmmn();
		}
		if (CmmnHistoryTablePresent && (!processEngineConfiguration.CmmnEnabled || !processEngineConfiguration.DbHistoryUsed))
		{
		  dbSchemaDropCmmnHistory();
		}
		if (DmnTablePresent && !processEngineConfiguration.DmnEnabled)
		{
		  dbSchemaDropDmn();
		}
		if (DmnHistoryTablePresent && (!processEngineConfiguration.DmnEnabled || !processEngineConfiguration.DbHistoryUsed))
		{
		  dbSchemaDropDmnHistory();
		}
	  }

	  public abstract bool EngineTablePresent {get;}

	  public abstract bool HistoryTablePresent {get;}

	  public abstract bool IdentityTablePresent {get;}

	  public abstract bool CmmnTablePresent {get;}

	  public abstract bool CmmnHistoryTablePresent {get;}

	  public abstract bool DmnTablePresent {get;}

	  public abstract bool DmnHistoryTablePresent {get;}

	  public virtual void dbSchemaUpdate()
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;

		if (!EngineTablePresent)
		{
		  dbSchemaCreateEngine();
		}

		if (!HistoryTablePresent && processEngineConfiguration.DbHistoryUsed)
		{
		  dbSchemaCreateHistory();
		}

		if (!IdentityTablePresent && processEngineConfiguration.DbIdentityUsed)
		{
		  dbSchemaCreateIdentity();
		}

		if (!CmmnTablePresent && processEngineConfiguration.CmmnEnabled)
		{
		  dbSchemaCreateCmmn();
		}

		if (!CmmnHistoryTablePresent && processEngineConfiguration.CmmnEnabled && processEngineConfiguration.DbHistoryUsed)
		{
		  dbSchemaCreateCmmnHistory();
		}

		if (!DmnTablePresent && processEngineConfiguration.DmnEnabled)
		{
		  dbSchemaCreateDmn();
		}

		if (!DmnHistoryTablePresent && processEngineConfiguration.DmnEnabled && processEngineConfiguration.DbHistoryUsed)
		{
		  dbSchemaCreateDmnHistory();
		}

	  }

	  public virtual IList<string> TableNamesPresent
	  {
		  get
		  {
			return Collections.emptyList();
		  }
	  }

	  public virtual void addEntityLoadListener(EntityLoadListener listener)
	  {
		this.listeners.Add(listener);
	  }


	  protected internal virtual void fireEntityLoaded(object result)
	  {
		if (result != null && result is DbEntity)
		{
		  DbEntity entity = (DbEntity) result;
		  foreach (EntityLoadListener entityLoadListener in listeners)
		  {
			entityLoadListener.onEntityLoaded(entity);
		  }
		}
	  }
	}

}