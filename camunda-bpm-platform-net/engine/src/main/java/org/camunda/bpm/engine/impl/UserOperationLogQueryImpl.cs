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
namespace org.camunda.bpm.engine.impl
{
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using UserOperationLogQuery = org.camunda.bpm.engine.history.UserOperationLogQuery;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using CompareUtil = org.camunda.bpm.engine.impl.util.CompareUtil;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	/// <summary>
	/// @author Danny Gräf
	/// </summary>
	[Serializable]
	public class UserOperationLogQueryImpl : AbstractQuery<UserOperationLogQuery, UserOperationLogEntry>, UserOperationLogQuery
	{

	  private const long serialVersionUID = 1L;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string deploymentId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionKey_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string executionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseExecutionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string taskId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string jobId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string jobDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string batchId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string userId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string operationId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string externalTaskId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string operationType_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string property_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string entityType_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string category_Conflict;
	  protected internal DateTime timestampAfter;
	  protected internal DateTime timestampBefore;

	  protected internal string[] entityTypes;
	  protected internal string[] categories;

	  public UserOperationLogQueryImpl()
	  {
	  }

	  public UserOperationLogQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual UserOperationLogQuery deploymentId(string deploymentId)
	  {
		ensureNotNull("deploymentId", deploymentId);
		this.deploymentId_Conflict = deploymentId;
		return this;
	  }

	  public virtual UserOperationLogQuery processDefinitionId(string processDefinitionId)
	  {
		ensureNotNull("processDefinitionId", processDefinitionId);
		this.processDefinitionId_Conflict = processDefinitionId;
		return this;
	  }

	  public virtual UserOperationLogQuery processDefinitionKey(string processDefinitionKey)
	  {
		ensureNotNull("processDefinitionKey", processDefinitionKey);
		this.processDefinitionKey_Conflict = processDefinitionKey;
		return this;
	  }

	  public virtual UserOperationLogQuery processInstanceId(string processInstanceId)
	  {
		ensureNotNull("processInstanceId", processInstanceId);
		this.processInstanceId_Conflict = processInstanceId;
		return this;
	  }

	  public virtual UserOperationLogQuery executionId(string executionId)
	  {
		ensureNotNull("executionId", executionId);
		this.executionId_Conflict = executionId;
		return this;
	  }

	  public virtual UserOperationLogQuery caseDefinitionId(string caseDefinitionId)
	  {
		ensureNotNull("caseDefinitionId", caseDefinitionId);
		this.caseDefinitionId_Conflict = caseDefinitionId;
		return this;
	  }

	  public virtual UserOperationLogQuery caseInstanceId(string caseInstanceId)
	  {
		ensureNotNull("caseInstanceId", caseInstanceId);
		this.caseInstanceId_Conflict = caseInstanceId;
		return this;
	  }

	  public virtual UserOperationLogQuery caseExecutionId(string caseExecutionId)
	  {
		ensureNotNull("caseExecutionId", caseExecutionId);
		this.caseExecutionId_Conflict = caseExecutionId;
		return this;
	  }


	  public virtual UserOperationLogQuery taskId(string taskId)
	  {
		ensureNotNull("taskId", taskId);
		this.taskId_Conflict = taskId;
		return this;
	  }

	  public virtual UserOperationLogQuery jobId(string jobId)
	  {
		ensureNotNull("jobId", jobId);
		this.jobId_Conflict = jobId;
		return this;
	  }

	  public virtual UserOperationLogQuery jobDefinitionId(string jobDefinitionId)
	  {
		ensureNotNull("jobDefinitionId", jobDefinitionId);
		this.jobDefinitionId_Conflict = jobDefinitionId;
		return this;
	  }

	  public virtual UserOperationLogQuery batchId(string batchId)
	  {
		ensureNotNull("batchId", batchId);
		this.batchId_Conflict = batchId;
		return this;
	  }

	  public virtual UserOperationLogQuery userId(string userId)
	  {
		ensureNotNull("userId", userId);
		this.userId_Conflict = userId;
		return this;
	  }

	  public virtual UserOperationLogQuery operationId(string operationId)
	  {
		ensureNotNull("operationId", operationId);
		this.operationId_Conflict = operationId;
		return this;
	  }

	  public virtual UserOperationLogQuery externalTaskId(string externalTaskId)
	  {
		ensureNotNull("externalTaskId", externalTaskId);
		this.externalTaskId_Conflict = externalTaskId;
		return this;
	  }

	  public virtual UserOperationLogQuery operationType(string operationType)
	  {
		ensureNotNull("operationType", operationType);
		this.operationType_Conflict = operationType;
		return this;
	  }

	  public virtual UserOperationLogQuery property(string property)
	  {
		ensureNotNull("property", property);
		this.property_Conflict = property;
		return this;
	  }

	  public virtual UserOperationLogQuery entityType(string entityType)
	  {
		ensureNotNull("entityType", entityType);
		this.entityType_Conflict = entityType;
		return this;
	  }

	  public virtual UserOperationLogQuery entityTypeIn(params string[] entityTypes)
	  {
		ensureNotNull("entity types", (object[]) entityTypes);
		this.entityTypes = entityTypes;
		return this;
	  }

	  public virtual UserOperationLogQuery category(string category)
	  {
		ensureNotNull("category", category);
		this.category_Conflict = category;
		return this;
	  }

	  public virtual UserOperationLogQuery categoryIn(params string[] categories)
	  {
		ensureNotNull("categories", (object[]) categories);
		this.categories = categories;
		return this;
	  }

	  public virtual UserOperationLogQuery afterTimestamp(DateTime after)
	  {
		this.timestampAfter = after;
		return this;
	  }

	  public virtual UserOperationLogQuery beforeTimestamp(DateTime before)
	  {
		this.timestampBefore = before;
		return this;
	  }

	  public virtual UserOperationLogQuery orderByTimestamp()
	  {
		return orderBy(OperationLogQueryProperty.TIMESTAMP);
	  }

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.OperationLogManager.findOperationLogEntryCountByQueryCriteria(this);
	  }

	  public override IList<UserOperationLogEntry> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.OperationLogManager.findOperationLogEntriesByQueryCriteria(this, page);
	  }

	  protected internal override bool hasExcludingConditions()
	  {
		return base.hasExcludingConditions() || CompareUtil.areNotInAscendingOrder(timestampAfter, timestampBefore);
	  }
	}

}