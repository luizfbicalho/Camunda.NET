using System;
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
namespace org.camunda.bpm.engine.impl.db
{

	using BatchExecutorException = org.apache.ibatis.executor.BatchExecutorException;
	using BatchResult = org.apache.ibatis.executor.BatchResult;
	using ProcessApplicationUnavailableException = org.camunda.bpm.application.ProcessApplicationUnavailableException;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using CachedDbEntity = org.camunda.bpm.engine.impl.db.entitymanager.cache.CachedDbEntity;
	using DbEntityState = org.camunda.bpm.engine.impl.db.entitymanager.cache.DbEntityState;
	using DbOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperation;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ClassNameUtil = org.camunda.bpm.engine.impl.util.ClassNameUtil;
	using ExceptionUtil = org.camunda.bpm.engine.impl.util.ExceptionUtil;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;

	/// <summary>
	/// @author Stefan Hentschel.
	/// </summary>
	public class EnginePersistenceLogger : ProcessEngineLogger
	{

	  protected internal static readonly string HINT_TEXT = "Hint: Set <property name=\"databaseSchemaUpdate\" to value=\"true\" or " +
												"value=\"create-drop\" (use create-drop for testing only!) in bean " +
												"processEngineConfiguration in camunda.cfg.xml for automatic schema creation";

	  protected internal virtual string buildStringFromList<T1>(ICollection<T1> list)
	  {
		StringBuilder message = new StringBuilder();
		message.Append("[");
		message.Append("\n");
		foreach (object @object in list)
		{
		  message.Append("  ");
		  message.Append(@object.ToString());
		  message.Append("\n");
		}
		message.Append("]");

		return message.ToString();
	  }

	  private string buildStringFromMap<T1>(IDictionary<T1> map)
	  {
		StringBuilder message = new StringBuilder();
		message.Append("[");
		message.Append("\n");
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for(java.util.Map.Entry<String, ?> entry : map.entrySet())
		foreach (KeyValuePair<string, ?> entry in map.SetOfKeyValuePairs())
		{
		  message.Append("  ");
		  message.Append(entry.Key);
		  message.Append(": ");
		  message.Append(entry.Value.ToString());
		  message.Append("\n");
		}
		message.Append("]");
		return message.ToString();
	  }

	  public virtual ProcessEngineException entityCacheLookupException<T>(Type<T> type, string id, Type entity, Exception cause) where T : DbEntity
	  {
		return new ProcessEngineException(exceptionMessage("001", "Could not lookup entity of type '{}' and id '{}': found entity of type '{}'.", type, id, entity), cause);
	  }

	  public virtual ProcessEngineException entityCacheDuplicateEntryException(string currentState, string id, Type entityClass, DbEntityState foundState)
	  {
		return new ProcessEngineException(exceptionMessage("002", "Cannot add {} entity with id '{}' and type '{}' into cache. An entity with the same id and type is already in state '{}'", currentState, id, entityClass, foundState));
	  }

	  public virtual ProcessEngineException alreadyMarkedEntityInEntityCacheException(string id, Type entityClass, DbEntityState state)
	  {

		return new ProcessEngineException(exceptionMessage("003", "Inserting an entity with Id '{}' and type '{}' which is already marked with state '{}'", id, entityClass, state));
	  }

	  public virtual ProcessEngineException flushDbOperationException(IList<DbOperation> operationsToFlush, DbOperation operation, Exception cause)
	  {

		string exceptionMessage = exceptionMessage("004", "Exception while executing Database Operation '{}' with message '{}'. Flush summary: \n {}", operation.ToString(), cause.Message, buildStringFromList(operationsToFlush));

		return new ProcessEngineException(exceptionMessage, cause);
	  }

	  public virtual OptimisticLockingException concurrentUpdateDbEntityException(DbOperation operation)
	  {
		return new OptimisticLockingException(exceptionMessage("005", "Execution of '{}' failed. Entity was updated by another transaction concurrently.", operation));
	  }

	  public virtual void flushedCacheState(IList<CachedDbEntity> cachedEntities)
	  {
		if (DebugEnabled)
		{
		  logDebug("006", "Cache state after flush: {}", buildStringFromList(cachedEntities));
		}

	  }

	  public virtual ProcessEngineException mergeDbEntityException(DbEntity entity)
	  {
		return new ProcessEngineException(exceptionMessage("007", "Cannot merge DbEntity '{}' without id", entity));
	  }

	  public virtual void databaseFlushSummary(ICollection<DbOperation> operations)
	  {
	   if (DebugEnabled)
	   {
		 logDebug("008", "Flush Summary: {}", buildStringFromList(operations));
	   }
	  }

	  public virtual void executeDatabaseOperation(string operationType, object parameter)
	  {
		if (DebugEnabled)
		{

		  string message;
		  if (parameter != null)
		  {
			message = parameter.ToString();
		  }
		  else
		  {
			message = "null";
		  }

		  if (parameter is DbEntity)
		  {
			DbEntity dbEntity = (DbEntity) parameter;
			message = ClassNameUtil.getClassNameWithoutPackage(dbEntity) + "[id=" + dbEntity.Id + "]";
		  }

		  logDebug("009", "SQL operation: '{}'; Entity: '{}'", operationType, message);
		}
	  }

	  public virtual void executeDatabaseBulkOperation(string operationType, string statement, object parameter)
	  {
		logDebug("010", "SQL bulk operation: '{}'; Statement: '{}'; Parameter: '{}'", operationType, statement, parameter);
	  }

	  public virtual void fetchDatabaseTables(string source, IList<string> tableNames)
	  {
		if (DebugEnabled)
		{
		  logDebug("011", "Retrieving process engine tables from: '{}'. Retrieved tables: {}", source, buildStringFromList(tableNames));
		}
	  }

	  public virtual void missingSchemaResource(string resourceName, string operation)
	  {
		logDebug("012", "There is no schema resource '{}' for operation '{}'.", resourceName, operation);
	  }

	  public virtual ProcessEngineException missingSchemaResourceException(string resourceName, string operation)
	  {
		return new ProcessEngineException(exceptionMessage("013", "There is no schema resource '{}' for operation '{}'.", resourceName, operation));
	  }

	  public virtual ProcessEngineException missingSchemaResourceFileException(string fileName, Exception cause)
	  {
		return new ProcessEngineException(exceptionMessage("014", "Cannot find schema resource file with name '{}'",fileName), cause);
	  }

	  public virtual void failedDatabaseOperation(string operation, string statement, Exception cause)
	  {
		logError("015", "Problem during schema operation '{}' with statement '{}'. Cause: '{}'", operation, statement, cause.Message);
	  }

	  public virtual void performingDatabaseOperation(string operation, string component, string resourceName)
	  {
		logInfo("016", "Performing database operation '{}' on component '{}' with resource '{}'", operation, component, resourceName);
	  }

	  public virtual void successfulDatabaseOperation(string operation, string component)
	  {
		logDebug("Database schema operation '{}' for component '{}' was successful.", operation, component);
	  }

	  public virtual ProcessEngineException performDatabaseOperationException(string operation, string sql, Exception cause)
	  {
		return new ProcessEngineException(exceptionMessage("017", "Could not perform operation '{}' on database schema for SQL Statement: '{}'.", operation, sql), cause);
	  }

	  public virtual ProcessEngineException checkDatabaseTableException(Exception cause)
	  {
		return new ProcessEngineException(exceptionMessage("018", "Could not check if tables are already present using metadata."), cause);
	  }

	  public virtual ProcessEngineException getDatabaseTableNameException(Exception cause)
	  {
		return new ProcessEngineException(exceptionMessage("019", "Unable to fetch process engine table names."), cause);
	  }

	  public virtual ProcessEngineException missingRelationMappingException(string relation)
	  {
		return new ProcessEngineException(exceptionMessage("020", "There is no mapping for the relation '{}' registered.", relation));
	  }

	  public virtual ProcessEngineException databaseHistoryLevelException(string level)
	  {
		return new ProcessEngineException(exceptionMessage("021", "historyLevel '{}' is higher then 'none' and dbHistoryUsed is set to false.", level));
	  }

	  public virtual ProcessEngineException invokeSchemaResourceToolException(int length)
	  {
		return new ProcessEngineException(exceptionMessage("022", "Schema resource tool was invoked with '{}' parameters." + "Schema resource tool must be invoked with exactly 2 parameters:" + "\n - 1st parameter is the process engine configuration file," + "\n - 2nd parameter is the schema resource file name", length));
	  }

	  public virtual ProcessEngineException loadModelException(string type, string modelName, string id, Exception cause)
	  {
		return new ProcessEngineException(exceptionMessage("023", "Could not load {} Model for {} definition with id '{}'.", type, modelName, id), cause);
	  }

	  public virtual void removeEntryFromDeploymentCacheFailure(string modelName, string id, Exception cause)
	  {
		logWarn("024", "Could not remove {} definition with id '{}' from the cache. Reason: '{}'", modelName, id, cause.Message, cause);
	  }


	  public virtual ProcessEngineException engineAuthorizationTypeException(int usedType, int global, int grant, int revoke)
	  {
		return new ProcessEngineException(exceptionMessage("025", "Unrecognized authorization type '{}'. Must be one of ['{}', '{}', '{}']", usedType, global, grant, revoke));
	  }

	  public virtual System.InvalidOperationException permissionStateException(string methodName, string type)
	  {
		return new System.InvalidOperationException(exceptionMessage("026", "Method '{}' cannot be used for authorization with type '{}'.", methodName, type));
	  }

	  public virtual ProcessEngineException notUsableGroupIdForGlobalAuthorizationException()
	  {
		return new ProcessEngineException(exceptionMessage("027", "Cannot use 'groupId' for GLOBAL authorization"));
	  }

	  public virtual ProcessEngineException illegalValueForUserIdException(string id, string expected)
	  {
		return new ProcessEngineException(exceptionMessage("028", "Illegal value '{}' for userId for GLOBAL authorization. Must be '{}'", id, expected));
	  }

	  public virtual AuthorizationException requiredCamundaAdminException()
	  {
		return new AuthorizationException(exceptionMessage("029", "Required admin authenticated group or user."));
	  }

	  public virtual void createChildExecution(ExecutionEntity child, ExecutionEntity parent)
	  {
		if (DebugEnabled)
		{
		  logDebug("030", "Child execution '{}' created with parent '{}'.", child.ToString(), parent.ToString());
		}
	  }

	  public virtual void initializeExecution(ExecutionEntity entity)
	  {
		logDebug("031", "Initializing execution '{}'", entity.ToString());
	  }

	  public virtual void initializeTimerDeclaration(ExecutionEntity entity)
	  {
		logDebug("032", "Initializing timer declaration '{}'", entity.ToString());
	  }

	  public virtual ProcessEngineException requiredAsyncContinuationException(string id)
	  {
		return new ProcessEngineException(exceptionMessage("033", "Asynchronous Continuation for activity with id '{}' requires a message job declaration", id));
	  }

	  public virtual ProcessEngineException restoreProcessInstanceException(ExecutionEntity entity)
	  {
		return new ProcessEngineException(exceptionMessage("034", "Can only restore process instances. This method must be called on a process instance execution but was called on '{}'", entity.ToString()));

	  }

	  public virtual ProcessEngineException executionNotFoundException(string id)
	  {
		return new ProcessEngineException(exceptionMessage("035", "Unable to find execution for id '{}'", id));
	  }

	  public virtual ProcessEngineException castModelInstanceException(ModelElementInstance instance, string toElement, string type, string @namespace, Exception cause)
	  {

		return new ProcessEngineException(exceptionMessage("036", "Cannot cast '{}' to '{}'. Element is of type '{}' with namespace '{}'.", instance, toElement, type, @namespace), cause);
	  }

	  public virtual BadUserRequestException requestedProcessInstanceNotFoundException(string id)
	  {
		return new BadUserRequestException(exceptionMessage("037", "No process instance found for id '{}'", id));
	  }

	  public virtual NotValidException queryExtensionException(string extendedClassName, string extendingClassName)
	  {
		return new NotValidException(exceptionMessage("038", "Unable to extend a query of class '{}' by a query of class '{}'.", extendedClassName, extendingClassName));
	  }

	  public virtual ProcessEngineException unsupportedResourceTypeException(string type)
	  {
		return new ProcessEngineException(exceptionMessage("039", "Unsupported resource type '{}'", type));
	  }

	  public virtual ProcessEngineException serializerNotDefinedException(object entity)
	  {
		return new ProcessEngineException(exceptionMessage("040", "No serializer defined for variable instance '{}'", entity));
	  }

	  public virtual ProcessEngineException serializerOutOfContextException()
	  {
		return new ProcessEngineException(exceptionMessage("041", "Cannot work with serializers outside of command context."));
	  }

	  public virtual ProcessEngineException taskIsAlreadyAssignedException(string usedId, string foundId)
	  {
		return new ProcessEngineException(exceptionMessage("042", "Cannot assign '{}' to a task assignment that has already '{}' set.", usedId, foundId));
	  }

	  public virtual SuspendedEntityInteractionException suspendedEntityException(string type, string id)
	  {
		return new SuspendedEntityInteractionException(exceptionMessage("043", "{} with id '{}' is suspended.", type, id));
	  }

	  public virtual void logUpdateUnrelatedProcessDefinitionEntity(string thisKey, string thatKey, string thisDeploymentId, string thatDeploymentId)
	  {
		logDebug("044", "Cannot update entity from an unrelated process definition: this key '{}', that key '{}', this deploymentId '{}', that deploymentId '{}'", thisKey, thatKey, thisDeploymentId, thatDeploymentId);
	  }

	  public virtual ProcessEngineException toManyProcessDefinitionsException(int count, string key, string versionAttribute, string versionValue, string tenantId)
	  {
		return new ProcessEngineException(exceptionMessage("045", "There are '{}' results for a process definition with key '{}', {} '{}' and tenant-id '{}'.", count, key, versionAttribute, versionValue));
	  }

	  public virtual ProcessEngineException notAllowedIdException(string id)
	  {
		return new ProcessEngineException(exceptionMessage("046", "Cannot set id '{}'. Only the provided id generation is allowed for properties.", id));
	  }

	  public virtual void countRowsPerProcessEngineTable(IDictionary<string, long> map)
	  {
		if (DebugEnabled)
		{
		  logDebug("047", "Number of rows per process engine table: {}", buildStringFromMap(map));
		}
	  }

	  public virtual ProcessEngineException countTableRowsException(Exception cause)
	  {
		return new ProcessEngineException(exceptionMessage("048", "Could not fetch table counts."), cause);
	  }

	  public virtual void selectTableCountForTable(string name)
	  {
		logDebug("049", "Selecting table count for table with name '{}'", name);
	  }

	  public virtual ProcessEngineException retrieveMetadataException(Exception cause)
	  {
		return new ProcessEngineException(exceptionMessage("050", "Could not retrieve database metadata. Reason: '{}'", cause.Message), cause);
	  }

	  public virtual ProcessEngineException invokeTaskListenerException(Exception cause)
	  {
		return new ProcessEngineException(exceptionMessage("051", "There was an exception while invoking the TaskListener. Message: '{}'", cause.Message), cause);
	  }

	  public virtual BadUserRequestException uninitializedFormKeyException()
	  {
		return new BadUserRequestException(exceptionMessage("052", "The form key is not initialized. You must call initializeFormKeys() on the task query before you can " + "retrieve the form key."));
	  }

	  public virtual ProcessEngineException disabledHistoryException()
	  {
		return new ProcessEngineException(exceptionMessage("053", "History is not enabled."));
	  }

	  public virtual ProcessEngineException instantiateSessionException(string name, Exception cause)
	  {
		return new ProcessEngineException(exceptionMessage("054", "Could not instantiate class '{}'. Message: '{}'", name, cause.Message), cause);
	  }

	  public virtual WrongDbException wrongDbVersionException(string version, string dbVersion)
	  {
		return new WrongDbException(exceptionMessage("055", "Version mismatch: Camunda library version is '{}' and db version is '{}'. " + HINT_TEXT, version, dbVersion), version, dbVersion);
	  }

	  public virtual ProcessEngineException missingTableException(IList<string> components)
	  {
		return new ProcessEngineException(exceptionMessage("056", "Tables are missing for the following components: {}", buildStringFromList(components)));
	  }

	  public virtual ProcessEngineException missingActivitiTablesException()
	  {
		return new ProcessEngineException(exceptionMessage("057", "There are no Camunda tables in the database. " + HINT_TEXT));
	  }

	  public virtual ProcessEngineException unableToFetchDbSchemaVersion(Exception cause)
	  {
		return new ProcessEngineException(exceptionMessage("058", "Could not fetch the database schema version."), cause);
	  }

	  public virtual void failedTofetchVariableValue(Exception cause)
	  {
		logDebug("059", "Could not fetch value for variable.", cause);
	  }

	  public virtual ProcessEngineException historicDecisionInputInstancesNotFetchedException()
	  {
		return new ProcessEngineException(exceptionMessage("060", "The input instances for the historic decision instance are not fetched. You must call 'includeInputs()' on the query to enable fetching."));
	  }

	  public virtual ProcessEngineException historicDecisionOutputInstancesNotFetchedException()
	  {
		return new ProcessEngineException(exceptionMessage("061", "The output instances for the historic decision instance are not fetched. You must call 'includeOutputs()' on the query to enable fetching."));
	  }

	  public virtual void executingDDL(IList<string> logLines)
	  {
		if (DebugEnabled)
		{
		  logDebug("062", "Executing Schmema DDL {}", buildStringFromList(logLines));
		}
	  }

	  public virtual ProcessEngineException collectResultValueOfUnsupportedTypeException(TypedValue collectResultValue)
	  {
		return new ProcessEngineException(exceptionMessage("063", "The collect result value '{}' of the decision table result is not of type integer, long or double.", collectResultValue));
	  }

	  public virtual void creatingHistoryLevelPropertyInDatabase(HistoryLevel historyLevel)
	  {
		logInfo("065", "Creating historyLevel property in database for level: {}", historyLevel);
	  }

	  public virtual void couldNotSelectHistoryLevel(string message)
	  {
		logWarn("066", "Could not select history level property: {}", message);
	  }

	  public virtual void noHistoryLevelPropertyFound()
	  {
		logInfo("067", "No history level property found in database");
	  }

	  public virtual void noDeploymentLockPropertyFound()
	  {
		logError("068", "No deployment lock property found in databse");
	  }

	  public virtual void debugJobExecuted(JobEntity jobEntity)
	  {
		logDebug("069", "Job executed, deleting it", jobEntity);
	  }

	  public virtual ProcessEngineException multipleTenantsForProcessDefinitionKeyException(string processDefinitionKey)
	  {
		return new ProcessEngineException(exceptionMessage("070", "Cannot resolve a unique process definition for key '{}' because it exists for multiple tenants.", processDefinitionKey));
	  }

	  public virtual ProcessEngineException cannotDeterminePaDataformats(ProcessApplicationUnavailableException e)
	  {
		return new ProcessEngineException(exceptionMessage("071","Cannot determine process application variable serializers. Context Process Application is unavailable."), e);
	  }

	  public virtual ProcessEngineException cannotChangeTenantIdOfTask(string taskId, string currentTenantId, string tenantIdToSet)
	  {
		return new ProcessEngineException(exceptionMessage("072", "Cannot change tenantId of Task '{}'. Current tenant id '{}', Tenant id to set '{}'", taskId, currentTenantId, tenantIdToSet));
	  }

	  public virtual ProcessEngineException cannotSetDifferentTenantIdOnSubtask(string parentTaskId, string tenantId, string tenantIdToSet)
	  {
		return new ProcessEngineException(exceptionMessage("073", "Cannot set different tenantId on subtask than on parent Task. Parent taskId: '{}', tenantId: '{}', tenant id to set '{}'", parentTaskId, tenantId, tenantIdToSet));
	  }

	  public virtual ProcessEngineException multipleTenantsForDecisionDefinitionKeyException(string decisionDefinitionKey)
	  {
		return new ProcessEngineException(exceptionMessage("074", "Cannot resolve a unique decision definition for key '{}' because it exists for multiple tenants.", decisionDefinitionKey));
	  }

	  public virtual ProcessEngineException multipleTenantsForCaseDefinitionKeyException(string caseDefinitionKey)
	  {
		return new ProcessEngineException(exceptionMessage("075", "Cannot resolve a unique case definition for key '{}' because it exists for multiple tenants.", caseDefinitionKey));
	  }

	  public virtual ProcessEngineException deleteProcessDefinitionWithProcessInstancesException(string processDefinitionId, long? processInstanceCount)
	  {
		return new ProcessEngineException(exceptionMessage("076", "Deletion of process definition without cascading failed. Process definition with id: {} can't be deleted, since there exists {} dependening process instances.", processDefinitionId, processInstanceCount));
	  }

	  public virtual ProcessEngineException resolveParentOfExecutionFailedException(string parentId, string executionId)
	  {
		return new ProcessEngineException(exceptionMessage("077", "Cannot resolve parent with id '{}' of execution '{}', perhaps it was deleted in the meantime", parentId, executionId));
	  }

	  public virtual void noHistoryCleanupLockPropertyFound()
	  {
		logError("078", "No history cleanup lock property found in databse");
	  }

	  public virtual void logUpdateUnrelatedCaseDefinitionEntity(string thisKey, string thatKey, string thisDeploymentId, string thatDeploymentId)
	  {
		logDebug("079", "Cannot update entity from an unrelated case definition: this key '{}', that key '{}', this deploymentId '{}', that deploymentId '{}'", thisKey, thatKey, thisDeploymentId, thatDeploymentId);
	  }

	  public virtual void logUpdateUnrelatedDecisionDefinitionEntity(string thisKey, string thatKey, string thisDeploymentId, string thatDeploymentId)
	  {
		logDebug("080", "Cannot update entity from an unrelated decision definition: this key '{}', that key '{}', this deploymentId '{}', that deploymentId '{}'", thisKey, thatKey, thisDeploymentId, thatDeploymentId);
	  }

	  public virtual void noStartupLockPropertyFound()
	  {
		logError("081", "No startup lock property found in database");
	  }

	  public virtual void printBatchResults(IList<BatchResult> results)
	  {
		if (results.Count > 0)
		{
		  StringBuilder sb = new StringBuilder();
		  sb.Append("Batch summary:\n");
		  for (int i = 0; i < results.Count; i++)
		  {
			BatchResult result = results[i];
			sb.Append("Result ").Append(i).Append(":\t");
			sb.Append(result.Sql.replaceAll("\n", "").replaceAll("\\s+", " ")).Append("\t");
			sb.Append("Update counts: ").Append(Arrays.ToString(result.UpdateCounts)).Append("\n");
		  }
		  logDebug("082", sb.ToString());
		}
	  }

	  public virtual ProcessEngineException flushDbOperationsException(IList<DbOperation> operationsToFlush, Exception cause)
	  {
		string message = cause.Message;

		//collect real SQL exception messages in case of batch processing
		Exception exCause = cause;
		do
		{
		  if (exCause is BatchExecutorException)
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<java.sql.SQLException> relatedSqlExceptions = org.camunda.bpm.engine.impl.util.ExceptionUtil.findRelatedSqlExceptions(exCause);
			IList<SQLException> relatedSqlExceptions = ExceptionUtil.findRelatedSqlExceptions(exCause);
			StringBuilder sb = new StringBuilder();
			foreach (SQLException sqlException in relatedSqlExceptions)
			{
			  sb.Append(sqlException).Append("\n");
			}
			message = message + "\n" + sb.ToString();
		  }
		  exCause = exCause.InnerException;
		} while (exCause != null);

		string exceptionMessage = exceptionMessage("083", "Exception while executing Batch Database Operations with message '{}'. Flush summary: \n {}", message, buildStringFromList(operationsToFlush));

		return new ProcessEngineException(exceptionMessage, cause);
	  }

	  public virtual ProcessEngineException wrongBatchResultsSizeException(IList<DbOperation> operationsToFlush)
	  {
		return new ProcessEngineException(exceptionMessage("084", "Exception while executing Batch Database Operations: the size of Batch Result does not correspond to the number of flushed operations. Flush summary: \n {}", buildStringFromList(operationsToFlush)));
	  }

	  public virtual ProcessEngineException multipleDefinitionsForVersionTagException(string decisionDefinitionKey, string decisionDefinitionVersionTag)
	  {
		return new ProcessEngineException(exceptionMessage("085", "Found more than one decision definition for key '{}' and versionTag '{}'", decisionDefinitionKey, decisionDefinitionVersionTag));
	  }

	  public virtual BadUserRequestException invalidResourceForPermission(string resourceType, string permission)
	  {
		return new BadUserRequestException(exceptionMessage("086", "The resource type '{}' is not valid for '{}' permission.", resourceType, permission));
	  }

	  public virtual BadUserRequestException invalidResourceForAuthorization(int resourceType, string permission)
	  {
		return new BadUserRequestException(exceptionMessage("087", "The resource type with id:'{}' is not valid for '{}' permission.", resourceType, permission));
	  }


	  public virtual BadUserRequestException disabledPermissionException(string permission)
	  {
		return new BadUserRequestException(exceptionMessage("088", "The '{}' permission is disabled, please check your process engine configuration.", permission));
	  }

	}

}