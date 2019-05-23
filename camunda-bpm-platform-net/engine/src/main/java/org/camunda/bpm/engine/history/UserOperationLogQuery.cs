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
namespace org.camunda.bpm.engine.history
{

	using Query = org.camunda.bpm.engine.query.Query;


	/// <summary>
	/// Programmatic querying for <seealso cref="UserOperationLogEntry"/> instances.
	/// 
	/// @author Danny Gräf
	/// </summary>
	public interface UserOperationLogQuery : Query<UserOperationLogQuery, UserOperationLogEntry>
	{

	  /// <summary>
	  /// Query for operations on entities of a given type only. This allows you to restrict the
	  /// result set to all operations which were performed on the same Entity (ie. all Task Operations,
	  /// All IdentityLink Operations ...)
	  /// </summary>
	  /// <seealso cref= EntityTypes#TASK </seealso>
	  /// <seealso cref= EntityTypes#IDENTITY_LINK </seealso>
	  /// <seealso cref= EntityTypes#ATTACHMENT </seealso>
	  UserOperationLogQuery entityType(string entityType);

	  /// <summary>
	  /// Query for operations on entities of a given type only. This allows you to restrict the
	  /// result set to all operations which were performed on the same Entity (ie. all Task Operations,
	  /// All IdentityLink Operations ...)
	  /// </summary>
	  /// <seealso cref= EntityTypes#TASK </seealso>
	  /// <seealso cref= EntityTypes#IDENTITY_LINK </seealso>
	  /// <seealso cref= EntityTypes#ATTACHMENT </seealso>
	  UserOperationLogQuery entityTypeIn(params string[] entityTypes);

	  /// <summary>
	  /// Query for operations of a given type only. Types of operations depend on the entity on which the operation
	  /// was performed. For Instance: Tasks may be delegated, claimed, completed ...
	  /// Check the <seealso cref="UserOperationLogEntry"/> class for a list of constants of supported operations.
	  /// </summary>
	  UserOperationLogQuery operationType(string operationType);

	  /// <summary>
	  /// Query entries which are existing for the given deployment id. </summary>
	  UserOperationLogQuery deploymentId(string deploymentId);

	  /// <summary>
	  /// Query entries which are existing for the given process definition id. </summary>
	  UserOperationLogQuery processDefinitionId(string processDefinitionId);

	  /// <summary>
	  /// Query entries which are operate on all process definitions of the given key. </summary>
	  UserOperationLogQuery processDefinitionKey(string processDefinitionKey);

	  /// <summary>
	  /// Query entries which are existing for the given process instance. </summary>
	  UserOperationLogQuery processInstanceId(string processInstanceId);

	  /// <summary>
	  /// Query entries which are existing for the given execution. </summary>
	  UserOperationLogQuery executionId(string executionId);

	  /// <summary>
	  /// Query entries which are existing for the given case definition id. </summary>
	  UserOperationLogQuery caseDefinitionId(string caseDefinitionId);

	  /// <summary>
	  /// Query entries which are existing for the given case instance. </summary>
	  UserOperationLogQuery caseInstanceId(string caseInstanceId);

	  /// <summary>
	  /// Query entries which are existing for the given case execution. </summary>
	  UserOperationLogQuery caseExecutionId(string caseExecutionId);

	  /// <summary>
	  /// Query entries which are existing for the task. </summary>
	  UserOperationLogQuery taskId(string taskId);

	  /// <summary>
	  /// Query entries which are existing for the job. </summary>
	  UserOperationLogQuery jobId(string jobId);

	  /// <summary>
	  /// Query entries which are existing for the job definition. </summary>
	  UserOperationLogQuery jobDefinitionId(string jobDefinitionId);

	  /// <summary>
	  /// Query entries which are existing for the batch. </summary>
	  UserOperationLogQuery batchId(string batchId);

	  /// <summary>
	  /// Query entries which are existing for the user. </summary>
	  UserOperationLogQuery userId(string userId);

	  /// <summary>
	  /// Query entries of a composite operation.
	  /// This allows grouping multiple updates which are part of the same operation:
	  /// for instance, a User may update multiple fields of a UserTask when calling <seealso cref="TaskService.saveTask(org.camunda.bpm.engine.task.Task)"/>
	  /// which will be logged as separate <seealso cref="UserOperationLogEntry OperationLogEntries"/> with the same 'operationId'
	  /// 
	  /// </summary>
	  UserOperationLogQuery operationId(string operationId);

	  /// <summary>
	  /// Query entries which are existing for the external task. </summary>
	  UserOperationLogQuery externalTaskId(string externalTaskId);

	  /// <summary>
	  /// Query entries that changed a property. </summary>
	  UserOperationLogQuery property(string property);

	  /// <summary>
	  /// Query for operations of the given category only. This allows you to restrict the
	  /// result set to all operations which were performed in the same domain (ie. all Task Worker Operations,
	  /// All Admin Operations ...)
	  /// </summary>
	  /// <seealso cref= UserOperationLogEntry#CATEGORY_ADMIN </seealso>
	  /// <seealso cref= UserOperationLogEntry#CATEGORY_OPERATOR </seealso>
	  /// <seealso cref= UserOperationLogEntry#CATEGORY_TASK_WORKER </seealso>
	  UserOperationLogQuery category(string category);

	  /// <summary>
	  /// Query for operations of given categories only. This allows you to restrict the
	  /// result set to all operations which were performed in the same domain (ie. all Task Worker Operations,
	  /// All Admin Operations ...)
	  /// </summary>
	  /// <seealso cref= UserOperationLogEntry#CATEGORY_ADMIN </seealso>
	  /// <seealso cref= UserOperationLogEntry#CATEGORY_OPERATOR </seealso>
	  /// <seealso cref= UserOperationLogEntry#CATEGORY_TASK_WORKER </seealso>
	  UserOperationLogQuery categoryIn(params string[] categories);

	  /// <summary>
	  /// Query entries after the time stamp. </summary>
	  UserOperationLogQuery afterTimestamp(DateTime after);

	  /// <summary>
	  /// Query entries before the time stamp. </summary>
	  UserOperationLogQuery beforeTimestamp(DateTime before);

	  /// <summary>
	  /// Order by time stamp (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  UserOperationLogQuery orderByTimestamp();
	}

}