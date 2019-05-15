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
namespace org.camunda.bpm.engine.impl.db.entitymanager
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.db.entitymanager.cache.DbEntityState.DELETED_MERGED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.db.entitymanager.cache.DbEntityState.DELETED_PERSISTENT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.db.entitymanager.cache.DbEntityState.DELETED_TRANSIENT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.db.entitymanager.cache.DbEntityState.MERGED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.db.entitymanager.cache.DbEntityState.PERSISTENT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.db.entitymanager.cache.DbEntityState.TRANSIENT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperationType.DELETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperationType.DELETE_BULK;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperationType.INSERT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperationType.UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperationType.UPDATE_BULK;


	using BatchExecutorException = org.apache.ibatis.executor.BatchExecutorException;
	using BatchResult = org.apache.ibatis.executor.BatchResult;
	using IdGenerator = org.camunda.bpm.engine.impl.cfg.IdGenerator;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using CaseDefinitionQueryImpl = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionQueryImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CachedDbEntity = org.camunda.bpm.engine.impl.db.entitymanager.cache.CachedDbEntity;
	using DbEntityCache = org.camunda.bpm.engine.impl.db.entitymanager.cache.DbEntityCache;
	using DbEntityState = org.camunda.bpm.engine.impl.db.entitymanager.cache.DbEntityState;
	using DbBulkOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbBulkOperation;
	using DbEntityOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbEntityOperation;
	using DbOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperation;
	using DbOperationManager = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperationManager;
	using DbOperationType = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperationType;
	using DbGroupQueryImpl = org.camunda.bpm.engine.impl.identity.db.DbGroupQueryImpl;
	using DbUserQueryImpl = org.camunda.bpm.engine.impl.identity.db.DbUserQueryImpl;
	using Session = org.camunda.bpm.engine.impl.interceptor.Session;
	using JobExecutorContext = org.camunda.bpm.engine.impl.jobexecutor.JobExecutorContext;
	using ByteArrayEntity = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayEntity;
	using CollectionUtil = org.camunda.bpm.engine.impl.util.CollectionUtil;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;
	using ExceptionUtil = org.camunda.bpm.engine.impl.util.ExceptionUtil;
	using ResourceTypes = org.camunda.bpm.engine.repository.ResourceTypes;

	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "rawtypes" }) public class DbEntityManager implements org.camunda.bpm.engine.impl.interceptor.Session, org.camunda.bpm.engine.impl.db.EntityLoadListener
	public class DbEntityManager : Session, EntityLoadListener
	{

	  protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;
	  protected internal const string TOGGLE_FOREIGN_KEY_STMT = "toggleForeignKey";
	  public const int BATCH_SIZE = 50;

	  protected internal IList<OptimisticLockingListener> optimisticLockingListeners;

	  protected internal IdGenerator idGenerator;

	  protected internal DbEntityCache dbEntityCache;

	  protected internal DbOperationManager dbOperationManager;

	  protected internal PersistenceSession persistenceSession;
	  protected internal bool isIgnoreForeignKeysForNextFlush;

	  public DbEntityManager(IdGenerator idGenerator, PersistenceSession persistenceSession)
	  {
		this.idGenerator = idGenerator;
		this.persistenceSession = persistenceSession;
		if (persistenceSession != null)
		{
		  this.persistenceSession.addEntityLoadListener(this);
		}
		initializeEntityCache();
		initializeOperationManager();
	  }

	  protected internal virtual void initializeOperationManager()
	  {
		dbOperationManager = new DbOperationManager();
	  }

	  protected internal virtual void initializeEntityCache()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.jobexecutor.JobExecutorContext jobExecutorContext = org.camunda.bpm.engine.impl.context.Context.getJobExecutorContext();
		JobExecutorContext jobExecutorContext = Context.JobExecutorContext;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl processEngineConfiguration = org.camunda.bpm.engine.impl.context.Context.getProcessEngineConfiguration();
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;

		if (processEngineConfiguration != null && processEngineConfiguration.DbEntityCacheReuseEnabled && jobExecutorContext != null)
		{

		  dbEntityCache = jobExecutorContext.EntityCache;
		  if (dbEntityCache == null)
		  {
			dbEntityCache = new DbEntityCache(processEngineConfiguration.DbEntityCacheKeyMapping);
			jobExecutorContext.EntityCache = dbEntityCache;
		  }

		}
		else
		{

		  if (processEngineConfiguration != null)
		  {
			dbEntityCache = new DbEntityCache(processEngineConfiguration.DbEntityCacheKeyMapping);
		  }
		  else
		  {
			dbEntityCache = new DbEntityCache();
		  }
		}

	  }

	  // selects /////////////////////////////////////////////////

	  public virtual System.Collections.IList selectList(string statement)
	  {
		return selectList(statement, null, 0, int.MaxValue);
	  }

	  public virtual System.Collections.IList selectList(string statement, object parameter)
	  {
		return selectList(statement, parameter, 0, int.MaxValue);
	  }

	  public virtual System.Collections.IList selectList(string statement, object parameter, Page page)
	  {
		if (page != null)
		{
		  return selectList(statement, parameter, page.FirstResult, page.MaxResults);
		}
		else
		{
		  return selectList(statement, parameter, 0, int.MaxValue);
		}
	  }

	  public virtual System.Collections.IList selectList(string statement, ListQueryParameterObject parameter, Page page)
	  {
		return selectList(statement, parameter);
	  }

	  public virtual System.Collections.IList selectList(string statement, object parameter, int firstResult, int maxResults)
	  {
		return selectList(statement, new ListQueryParameterObject(parameter, firstResult, maxResults));
	  }

	  public virtual System.Collections.IList selectList(string statement, ListQueryParameterObject parameter)
	  {
		return selectListWithRawParameter(statement, parameter, parameter.FirstResult, parameter.MaxResults);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List selectListWithRawParameter(String statement, Object parameter, int firstResult, int maxResults)
	  public virtual System.Collections.IList selectListWithRawParameter(string statement, object parameter, int firstResult, int maxResults)
	  {
		if (firstResult == -1 || maxResults == -1)
		{
		  return Collections.EMPTY_LIST;
		}
		System.Collections.IList loadedObjects = persistenceSession.selectList(statement, parameter);
		return filterLoadedObjects(loadedObjects);
	  }

	  public virtual object selectOne(string statement, object parameter)
	  {
		object result = persistenceSession.selectOne(statement, parameter);
		if (result is DbEntity)
		{
		  DbEntity loadedObject = (DbEntity) result;
		  result = cacheFilter(loadedObject);
		}
		return result;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public boolean selectBoolean(String statement, Object parameter)
	  public virtual bool selectBoolean(string statement, object parameter)
	  {
		IList<string> result = (IList<string>) persistenceSession.selectList(statement, parameter);
		if (result != null)
		{
		  return result.Contains(1);
		}
		return false;

	  }

	  public virtual T selectById<T>(Type<T> entityClass, string id) where T : org.camunda.bpm.engine.impl.db.DbEntity
	  {
		T persistentObject = dbEntityCache.get(entityClass, id);
		if (persistentObject != null)
		{
		  return persistentObject;
		}

		persistentObject = persistenceSession.selectById(entityClass, id);

		if (persistentObject == null)
		{
		  return default(T);
		}
		// don't have to put object into the cache now. See onEntityLoaded() callback
		return persistentObject;
	  }

	  public virtual T getCachedEntity<T>(Type<T> type, string id) where T : org.camunda.bpm.engine.impl.db.DbEntity
	  {
		return dbEntityCache.get(type, id);
	  }

	  public virtual IList<T> getCachedEntitiesByType<T>(Type<T> type) where T : org.camunda.bpm.engine.impl.db.DbEntity
	  {
		return dbEntityCache.getEntitiesByType(type);
	  }

	  protected internal virtual System.Collections.IList filterLoadedObjects(IList<object> loadedObjects)
	  {
		if (loadedObjects.Count == 0 || loadedObjects[0] == null)
		{
		  return loadedObjects;
		}
		if (!(loadedObjects[0].GetType().IsAssignableFrom(typeof(DbEntity))))
		{
		  return loadedObjects;
		}
		IList<DbEntity> filteredObjects = new List<DbEntity>(loadedObjects.Count);
		foreach (object loadedObject in loadedObjects)
		{
		  DbEntity cachedPersistentObject = cacheFilter((DbEntity) loadedObject);
		  filteredObjects.Add(cachedPersistentObject);
		}
		return filteredObjects;
	  }

	  /// <summary>
	  /// returns the object in the cache.  if this object was loaded before,
	  /// then the original object is returned. 
	  /// </summary>
	  protected internal virtual DbEntity cacheFilter(DbEntity persistentObject)
	  {
		DbEntity cachedPersistentObject = dbEntityCache.get(persistentObject.GetType(), persistentObject.Id);
		if (cachedPersistentObject != null)
		{
		  return cachedPersistentObject;
		}
		else
		{
		  return persistentObject;
		}

	  }

	  public virtual void onEntityLoaded(DbEntity entity)
	  {
		// we get a callback when the persistence session loads an object from the database
		DbEntity cachedPersistentObject = dbEntityCache.get(entity.GetType(), entity.Id);
		if (cachedPersistentObject == null)
		{
		  // only put into the cache if not already present
		  dbEntityCache.putPersistent(entity);

		  // invoke postLoad() lifecycle method
		  if (entity is DbEntityLifecycleAware)
		  {
			DbEntityLifecycleAware lifecycleAware = (DbEntityLifecycleAware) entity;
			lifecycleAware.postLoad();
		  }
		}

	  }

	  public virtual void @lock(string statement)
	  {
		@lock(statement, null);
	  }

	  public virtual void @lock(string statement, object parameter)
	  {
		persistenceSession.@lock(statement, parameter);
	  }

	  public virtual bool isDirty(DbEntity dbEntity)
	  {
		CachedDbEntity cachedEntity = dbEntityCache.getCachedEntity(dbEntity);
		if (cachedEntity == null)
		{
		  return false;
		}
		else
		{
		  return cachedEntity.Dirty || cachedEntity.EntityState == DbEntityState.MERGED;
		}
	  }

	  public virtual void flush()
	  {

		// flush the entity cache which inserts operations to the db operation manager
		flushEntityCache();

		// flush the db operation manager
		flushDbOperationManager();
	  }

	  public virtual bool IgnoreForeignKeysForNextFlush
	  {
		  set
		  {
			isIgnoreForeignKeysForNextFlush = value;
		  }
	  }

	  protected internal virtual void flushDbOperationManager()
	  {

		// obtain totally ordered operation list from operation manager
		IList<DbOperation> operationsToFlush = dbOperationManager.calculateFlush();
		if (operationsToFlush == null || operationsToFlush.Count == 0)
		{
		  return;
		}

		LOG.databaseFlushSummary(operationsToFlush);

		// If we want to delete all table data as bulk operation, on tables which have self references,
		// We need to turn the foreign key check off on MySQL and MariaDB.
		// On other databases we have to do nothing, the mapped statement will be empty.
		if (isIgnoreForeignKeysForNextFlush)
		{
		  persistenceSession.executeNonEmptyUpdateStmt(TOGGLE_FOREIGN_KEY_STMT, false);
		  persistenceSession.flushOperations();
		}

		try
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<java.util.List<org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperation>> batches = org.camunda.bpm.engine.impl.util.CollectionUtil.partition(operationsToFlush, BATCH_SIZE);
		  IList<IList<DbOperation>> batches = CollectionUtil.partition(operationsToFlush, BATCH_SIZE);
		  foreach (IList<DbOperation> batch in batches)
		  {
			flushDbOperations(batch);
		  }
		}
		finally
		{
		  if (isIgnoreForeignKeysForNextFlush)
		  {
			persistenceSession.executeNonEmptyUpdateStmt(TOGGLE_FOREIGN_KEY_STMT, true);
			persistenceSession.flushOperations();
			isIgnoreForeignKeysForNextFlush = false;
		  }
		}
	  }

	  protected internal virtual void flushDbOperations(IList<DbOperation> operationsToFlush)
	  {
		// execute the flush
		foreach (DbOperation dbOperation in operationsToFlush)
		{
		  bool doOptimisticLockingException = false;
		  try
		  {
			persistenceSession.executeDbOperation(dbOperation);
		  }
		  catch (Exception e)
		  {
			//some of the exceptions are considered to be optimistic locking exception
			doOptimisticLockingException = isOptimisticLockingException(dbOperation, e);
			if (!doOptimisticLockingException)
			{
			  throw LOG.flushDbOperationException(operationsToFlush, dbOperation, e);
			}
		  }
		  if (dbOperation.Failed || doOptimisticLockingException)
		  {
			handleOptimisticLockingException(dbOperation);
		  }
		}

		if (Context.ProcessEngineConfiguration.JdbcBatchProcessing)
		{
		  IList<BatchResult> flushResult = new List<BatchResult>();
		  try
		  {
			flushResult = persistenceSession.flushOperations();
		  }
		  catch (Exception e)
		  {
			//some of the exceptions are considered to be optimistic locking exception
			DbOperation failedOperation = hasOptimisticLockingException(operationsToFlush, e);
			if (failedOperation == null)
			{
			  throw LOG.flushDbOperationsException(operationsToFlush, e);
			}
			else
			{
			  handleOptimisticLockingException(failedOperation);
			}
		  }
		  checkFlushResults(operationsToFlush, flushResult);
		}
	  }

	  /// <summary>
	  /// An OptimisticLockingException check for batch processing
	  /// </summary>
	  /// <param name="operationsToFlush"> The list of DB operations in which the Exception occurred </param>
	  /// <param name="cause"> the Exception object </param>
	  /// <returns> The DbOperation where the OptimisticLockingException has occurred
	  /// or null if no OptimisticLockingException occurred </returns>
	  private DbOperation hasOptimisticLockingException(IList<DbOperation> operationsToFlush, Exception cause)
	  {

		BatchExecutorException batchExecutorException = ExceptionUtil.findBatchExecutorException(cause);

		if (batchExecutorException != null)
		{

		  int failedOperationIndex = batchExecutorException.SuccessfulBatchResults.size();
		  if (failedOperationIndex < operationsToFlush.Count)
		  {
			DbOperation failedOperation = operationsToFlush[failedOperationIndex];
			if (isOptimisticLockingException(failedOperation, cause))
			{
			  return failedOperation;
			}
		  }
		}

		return null;
	  }

	  /// <summary>
	  /// Checks if the reason for a persistence exception was the foreign-key referencing of a (currently)
	  /// non-existing entity. This might happen with concurrent transactions, leading to an
	  /// OptimisticLockingException.
	  /// </summary>
	  /// <param name="failedOperation">
	  /// @return </param>
	  private bool isOptimisticLockingException(DbOperation failedOperation, Exception cause)
	  {

		bool isConstraintViolation = ExceptionUtil.checkForeignKeyConstraintViolation(cause);
		bool isVariableIntegrityViolation = ExceptionUtil.checkVariableIntegrityViolation(cause);

		if (isVariableIntegrityViolation)
		{

		  return true;
		}
		else if (isConstraintViolation && failedOperation is DbEntityOperation && ((DbEntityOperation) failedOperation).Entity is HasDbReferences && (failedOperation.OperationType.Equals(DbOperationType.INSERT) || failedOperation.OperationType.Equals(DbOperationType.UPDATE)))
		{

		  DbEntity entity = ((DbEntityOperation) failedOperation).Entity;
		  foreach (KeyValuePair<string, Type> reference in ((HasDbReferences)entity).ReferencedEntitiesIdAndClass.SetOfKeyValuePairs())
		  {
			DbEntity referencedEntity = this.persistenceSession.selectById(reference.Value, reference.Key);
			if (referencedEntity == null)
			{

			  return true;
			}
		  }
		}

		return false;
	  }

	  protected internal virtual void checkFlushResults(IList<DbOperation> operationsToFlush, IList<BatchResult> flushResult)
	  {
		int flushResultSize = 0;

		if (flushResult != null && flushResult.Count > 0)
		{
		  LOG.printBatchResults(flushResult);
		  //process the batch results to handle Optimistic Lock Exceptions
		  IEnumerator<DbOperation> operationIt = operationsToFlush.GetEnumerator();
		  foreach (BatchResult batchResult in flushResult)
		  {
			foreach (int statementResult in batchResult.UpdateCounts)
			{
			  flushResultSize++;
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			  DbOperation thisOperation = operationIt.next();
			  thisOperation.RowsAffected = statementResult;
			  if (thisOperation is DbEntityOperation && ((DbEntityOperation) thisOperation).Entity is HasDbRevision && !thisOperation.OperationType.Equals(DbOperationType.INSERT))
			  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.db.DbEntity dbEntity = ((org.camunda.bpm.engine.impl.db.entitymanager.operation.DbEntityOperation) thisOperation).getEntity();
				DbEntity dbEntity = ((DbEntityOperation) thisOperation).Entity;
				if (statementResult != 1)
				{
				  ((DbEntityOperation) thisOperation).Failed = true;
				  handleOptimisticLockingException(thisOperation);
				}
				else
				{
				  //update revision number in cache
				  if (thisOperation.OperationType.Equals(DbOperationType.UPDATE))
				  {
					HasDbRevision versionedObject = (HasDbRevision) dbEntity;
					versionedObject.Revision = versionedObject.RevisionNext;
				  }
				}
			  }
			}
		  }
		  //this must not happen, but worth checking
		  if (operationsToFlush.Count != flushResultSize)
		  {
			LOG.wrongBatchResultsSizeException(operationsToFlush);
		  }
		}
	  }

	  public virtual void flushEntity(DbEntity entity)
	  {
		CachedDbEntity cachedEntity = dbEntityCache.getCachedEntity(entity);
		if (cachedEntity != null)
		{
		  flushCachedEntity(cachedEntity);
		}

		flushDbOperationManager();
	  }

	  protected internal virtual void handleOptimisticLockingException(DbOperation dbOperation)
	  {
		bool isHandled = false;

		if (optimisticLockingListeners != null)
		{
		  foreach (OptimisticLockingListener optimisticLockingListener in optimisticLockingListeners)
		  {
			if (optimisticLockingListener.EntityType == null || optimisticLockingListener.EntityType.IsAssignableFrom(dbOperation.EntityType))
			{
			  optimisticLockingListener.failedOperation(dbOperation);
			  isHandled = true;
			}
		  }
		}

		if (!isHandled && Context.ProcessEngineConfiguration.SkipHistoryOptimisticLockingExceptions)
		{
		  DbEntity dbEntity = ((DbEntityOperation) dbOperation).Entity;
		  if (dbEntity is HistoricEntity || isHistoricByteArray(dbEntity))
		  {
			isHandled = true;
		  }
		}

		if (!isHandled)
		{
		  throw LOG.concurrentUpdateDbEntityException(dbOperation);
		}
	  }

	  protected internal virtual bool isHistoricByteArray(DbEntity dbEntity)
	  {
		if (dbEntity is ByteArrayEntity)
		{
		  ByteArrayEntity byteArrayEntity = (ByteArrayEntity) dbEntity;
		  return byteArrayEntity.Type.Equals(ResourceTypes.HISTORY.Value);
		}
		else
		{
		  return false;
		}
	  }

	  /// <summary>
	  /// Flushes the entity cache:
	  /// Depending on the entity state, the required <seealso cref="DbOperation"/> is performed and the cache is updated.
	  /// </summary>
	  protected internal virtual void flushEntityCache()
	  {
		IList<CachedDbEntity> cachedEntities = dbEntityCache.CachedEntities;
		foreach (CachedDbEntity cachedDbEntity in cachedEntities)
		{
		  flushCachedEntity(cachedDbEntity);
		}

		// log cache state after flush
		LOG.flushedCacheState(dbEntityCache.CachedEntities);
	  }

	  protected internal virtual void flushCachedEntity(CachedDbEntity cachedDbEntity)
	  {

		if (cachedDbEntity.EntityState == TRANSIENT)
		{
		  // latest state of references in cache is relevant when determining insertion order
		  cachedDbEntity.determineEntityReferences();
		  // perform INSERT
		  performEntityOperation(cachedDbEntity, INSERT);
		  // mark PERSISTENT
		  cachedDbEntity.EntityState = PERSISTENT;

		}
		else if (cachedDbEntity.EntityState == PERSISTENT && cachedDbEntity.Dirty)
		{
		  // object is dirty -> perform UPDATE
		  performEntityOperation(cachedDbEntity, UPDATE);

		}
		else if (cachedDbEntity.EntityState == MERGED)
		{
		  // perform UPDATE
		  performEntityOperation(cachedDbEntity, UPDATE);
		  // mark PERSISTENT
		  cachedDbEntity.EntityState = PERSISTENT;

		}
		else if (cachedDbEntity.EntityState == DELETED_TRANSIENT)
		{
		  // remove from cache
		  dbEntityCache.remove(cachedDbEntity);

		}
		else if (cachedDbEntity.EntityState == DELETED_PERSISTENT || cachedDbEntity.EntityState == DELETED_MERGED)
		{
		  // perform DELETE
		  performEntityOperation(cachedDbEntity, DELETE);
		  // remove from cache
		  dbEntityCache.remove(cachedDbEntity);

		}

		// if object is PERSISTENT after flush
		if (cachedDbEntity.EntityState == PERSISTENT)
		{
		  // make a new copy
		  cachedDbEntity.makeCopy();
		  // update cached references
		  cachedDbEntity.determineEntityReferences();
		}
	  }

	  public virtual void insert(DbEntity dbEntity)
	  {
		// generate Id if not present
		ensureHasId(dbEntity);

		validateId(dbEntity);

		// put into cache
		dbEntityCache.putTransient(dbEntity);

	  }

	  public virtual void merge(DbEntity dbEntity)
	  {

		if (string.ReferenceEquals(dbEntity.Id, null))
		{
		  throw LOG.mergeDbEntityException(dbEntity);
		}

		// NOTE: a proper implementation of merge() would fetch the entity from the database
		// and merge the state changes. For now, we simply always perform an update.
		// Supposedly, the "proper" implementation would reduce the number of situations where
		// optimistic locking results in a conflict.

		dbEntityCache.putMerged(dbEntity);
	  }

	  public virtual void forceUpdate(DbEntity entity)
	  {
		CachedDbEntity cachedEntity = dbEntityCache.getCachedEntity(entity);
		if (cachedEntity != null && cachedEntity.EntityState == PERSISTENT)
		{
		  cachedEntity.forceSetDirty();
		}
	  }

	  public virtual void delete(DbEntity dbEntity)
	  {
		dbEntityCache.Deleted = dbEntity;
	  }

	  public virtual void undoDelete(DbEntity entity)
	  {
		dbEntityCache.undoDelete(entity);
	  }

	  public virtual void update(Type entityType, string statement, object parameter)
	  {
		performBulkOperation(entityType, statement, parameter, UPDATE_BULK);
	  }

	  /// <summary>
	  /// Several update operations added by this method will be executed preserving the order of method calls, no matter what entity type they refer to.
	  /// They will though be executed after all "not-bulk" operations (e.g. <seealso cref="DbEntityManager#insert(DbEntity)"/> or <seealso cref="DbEntityManager#merge(DbEntity)"/>)
	  /// and after those updates added by <seealso cref="DbEntityManager#update(Class, String, Object)"/>. </summary>
	  /// <param name="entityType"> </param>
	  /// <param name="statement"> </param>
	  /// <param name="parameter"> </param>
	  public virtual void updatePreserveOrder(Type entityType, string statement, object parameter)
	  {
		performBulkOperationPreserveOrder(entityType, statement, parameter, UPDATE_BULK);
	  }

	  public virtual void delete(Type entityType, string statement, object parameter)
	  {
		performBulkOperation(entityType, statement, parameter, DELETE_BULK);
	  }

	  /// <summary>
	  /// Several delete operations added by this method will be executed preserving the order of method calls, no matter what entity type they refer to.
	  /// They will though be executed after all "not-bulk" operations (e.g. <seealso cref="DbEntityManager#insert(DbEntity)"/> or <seealso cref="DbEntityManager#merge(DbEntity)"/>)
	  /// and after those deletes added by <seealso cref="DbEntityManager#delete(Class, String, Object)"/>. </summary>
	  /// <param name="entityType"> </param>
	  /// <param name="statement"> </param>
	  /// <param name="parameter"> </param>
	  /// <returns> delete operation </returns>
	  public virtual DbBulkOperation deletePreserveOrder(Type entityType, string statement, object parameter)
	  {
		return performBulkOperationPreserveOrder(entityType, statement, parameter, DELETE_BULK);
	  }

	  protected internal virtual DbBulkOperation performBulkOperation(Type entityType, string statement, object parameter, DbOperationType operationType)
	  {
		// create operation
		DbBulkOperation bulkOperation = createDbBulkOperation(entityType, statement, parameter, operationType);

		// schedule operation
		dbOperationManager.addOperation(bulkOperation);
		return bulkOperation;
	  }

	  protected internal virtual DbBulkOperation performBulkOperationPreserveOrder(Type entityType, string statement, object parameter, DbOperationType operationType)
	  {
		DbBulkOperation bulkOperation = createDbBulkOperation(entityType, statement, parameter, operationType);

		// schedule operation
		dbOperationManager.addOperationPreserveOrder(bulkOperation);
		return bulkOperation;
	  }

	  private DbBulkOperation createDbBulkOperation(Type entityType, string statement, object parameter, DbOperationType operationType)
	  {
		// create operation
		DbBulkOperation bulkOperation = new DbBulkOperation();

		// configure operation
		bulkOperation.OperationType = operationType;
		bulkOperation.EntityType = entityType;
		bulkOperation.Statement = statement;
		bulkOperation.Parameter = parameter;
		return bulkOperation;
	  }

	  protected internal virtual void performEntityOperation(CachedDbEntity cachedDbEntity, DbOperationType type)
	  {
		DbEntityOperation dbOperation = new DbEntityOperation();
		dbOperation.Entity = cachedDbEntity.Entity;
		dbOperation.FlushRelevantEntityReferences = cachedDbEntity.FlushRelevantEntityReferences;
		dbOperation.OperationType = type;
		dbOperationManager.addOperation(dbOperation);
	  }

	  public virtual void close()
	  {

	  }

	  public virtual bool isDeleted(DbEntity @object)
	  {
		return dbEntityCache.isDeleted(@object);
	  }

	  protected internal virtual void ensureHasId(DbEntity dbEntity)
	  {
		if (string.ReferenceEquals(dbEntity.Id, null))
		{
		  string nextId = idGenerator.NextId;
		  dbEntity.Id = nextId;
		}
	  }

	  protected internal virtual void validateId(DbEntity dbEntity)
	  {
		EnsureUtil.ensureValidIndividualResourceId("Entity " + dbEntity + " has an invalid id", dbEntity.Id);
	  }

	  public virtual IList<T> pruneDeletedEntities<T>(IList<T> listToPrune) where T : org.camunda.bpm.engine.impl.db.DbEntity
	  {
		List<T> prunedList = new List<T>();
		foreach (T potentiallyDeleted in listToPrune)
		{
		  if (!isDeleted(potentiallyDeleted))
		  {
			prunedList.Add(potentiallyDeleted);
		  }
		}
		return prunedList;
	  }

	  public virtual bool contains(DbEntity dbEntity)
	  {
		return dbEntityCache.contains(dbEntity);
	  }

	  // getters / setters /////////////////////////////////

	  public virtual DbOperationManager DbOperationManager
	  {
		  get
		  {
			return dbOperationManager;
		  }
		  set
		  {
			this.dbOperationManager = value;
		  }
	  }


	  public virtual DbEntityCache DbEntityCache
	  {
		  get
		  {
			return dbEntityCache;
		  }
		  set
		  {
			this.dbEntityCache = value;
		  }
	  }


	  // query factory methods ////////////////////////////////////////////////////

	  public virtual DeploymentQueryImpl createDeploymentQuery()
	  {
		return new DeploymentQueryImpl();
	  }

	  public virtual ProcessDefinitionQueryImpl createProcessDefinitionQuery()
	  {
		return new ProcessDefinitionQueryImpl();
	  }

	  public virtual CaseDefinitionQueryImpl createCaseDefinitionQuery()
	  {
		return new CaseDefinitionQueryImpl();
	  }

	  public virtual ProcessInstanceQueryImpl createProcessInstanceQuery()
	  {
		return new ProcessInstanceQueryImpl();
	  }

	  public virtual ExecutionQueryImpl createExecutionQuery()
	  {
		return new ExecutionQueryImpl();
	  }

	  public virtual TaskQueryImpl createTaskQuery()
	  {
		return new TaskQueryImpl();
	  }

	  public virtual JobQueryImpl createJobQuery()
	  {
		return new JobQueryImpl();
	  }

	  public virtual HistoricProcessInstanceQueryImpl createHistoricProcessInstanceQuery()
	  {
		return new HistoricProcessInstanceQueryImpl();
	  }

	  public virtual HistoricActivityInstanceQueryImpl createHistoricActivityInstanceQuery()
	  {
		return new HistoricActivityInstanceQueryImpl();
	  }

	  public virtual HistoricTaskInstanceQueryImpl createHistoricTaskInstanceQuery()
	  {
		return new HistoricTaskInstanceQueryImpl();
	  }

	  public virtual HistoricDetailQueryImpl createHistoricDetailQuery()
	  {
		return new HistoricDetailQueryImpl();
	  }

	  public virtual HistoricVariableInstanceQueryImpl createHistoricVariableInstanceQuery()
	  {
		return new HistoricVariableInstanceQueryImpl();
	  }

	  public virtual HistoricJobLogQueryImpl createHistoricJobLogQuery()
	  {
		return new HistoricJobLogQueryImpl();
	  }

	  public virtual UserQueryImpl createUserQuery()
	  {
		return new DbUserQueryImpl();
	  }

	  public virtual GroupQueryImpl createGroupQuery()
	  {
		return new DbGroupQueryImpl();
	  }

	  public virtual void registerOptimisticLockingListener(OptimisticLockingListener optimisticLockingListener)
	  {
		if (optimisticLockingListeners == null)
		{
		  optimisticLockingListeners = new List<OptimisticLockingListener>();
		}
		optimisticLockingListeners.Add(optimisticLockingListener);
	  }

	  public virtual IList<string> TableNamesPresentInDatabase
	  {
		  get
		  {
			return persistenceSession.TableNamesPresent;
		  }
	  }



	}

}