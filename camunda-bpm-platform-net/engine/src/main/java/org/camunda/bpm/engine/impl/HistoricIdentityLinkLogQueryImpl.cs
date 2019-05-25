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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using HistoricIdentityLinkLog = org.camunda.bpm.engine.history.HistoricIdentityLinkLog;
	using HistoricIdentityLinkLogQuery = org.camunda.bpm.engine.history.HistoricIdentityLinkLogQuery;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;

	 /// <summary>
	 /// @author Deivarayan Azhagappan
	 /// 
	 /// </summary>
	public class HistoricIdentityLinkLogQueryImpl : AbstractVariableQueryImpl<HistoricIdentityLinkLogQuery, HistoricIdentityLinkLog>, HistoricIdentityLinkLogQuery
	{

	  private const long serialVersionUID = 1L;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime dateBefore_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime dateAfter_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string type_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string userId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string groupId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string taskId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionKey_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string operationType_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string assignerId_Conflict;
	  protected internal string[] tenantIds;

	  public HistoricIdentityLinkLogQueryImpl()
	  {
	  }

	  public HistoricIdentityLinkLogQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual string Type
	  {
		  get
		  {
			return type_Conflict;
		  }
	  }

	  public virtual string UserId
	  {
		  get
		  {
			return userId_Conflict;
		  }
	  }

	  public virtual string GroupId
	  {
		  get
		  {
			return groupId_Conflict;
		  }
	  }

	  public virtual string TaskId
	  {
		  get
		  {
			return taskId_Conflict;
		  }
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId_Conflict;
		  }
	  }

	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey_Conflict;
		  }
	  }

	  public virtual string OperationType
	  {
		  get
		  {
			return operationType_Conflict;
		  }
	  }

	  public virtual string AssignerId
	  {
		  get
		  {
			return assignerId_Conflict;
		  }
	  }

	  public virtual HistoricIdentityLinkLogQuery tenantIdIn(params string[] tenantIds)
	  {
		ensureNotNull("tenantIds", (object[]) tenantIds);
		this.tenantIds = tenantIds;
		return this;
	  }

	  public virtual DateTime DateBefore
	  {
		  get
		  {
			return dateBefore_Conflict;
		  }
	  }

	  public virtual DateTime DateAfter
	  {
		  get
		  {
			return dateAfter_Conflict;
		  }
	  }

	  public virtual HistoricIdentityLinkLogQuery type(string type)
	  {
		ensureNotNull("type", type);
		this.type_Conflict = type;
		return this;
	  }

	  public virtual HistoricIdentityLinkLogQuery dateBefore(DateTime dateBefore)
	  {
		ensureNotNull("dateBefore", dateBefore);
		this.dateBefore_Conflict = dateBefore;
		return this;
	  }

	  public virtual HistoricIdentityLinkLogQuery dateAfter(DateTime dateAfter)
	  {
		ensureNotNull("dateAfter", dateAfter);
		this.dateAfter_Conflict = dateAfter;
		return this;
	  }

	  public virtual HistoricIdentityLinkLogQuery userId(string userId)
	  {
		ensureNotNull("userId", userId);
		this.userId_Conflict = userId;
		return this;
	  }

	  public virtual HistoricIdentityLinkLogQuery groupId(string groupId)
	  {
		ensureNotNull("groupId", groupId);
		this.groupId_Conflict = groupId;
		return this;
	  }

	  public virtual HistoricIdentityLinkLogQuery taskId(string taskId)
	  {
		ensureNotNull("taskId", taskId);
		this.taskId_Conflict = taskId;
		return this;
	  }

	  public virtual HistoricIdentityLinkLogQuery processDefinitionId(string processDefinitionId)
	  {
		ensureNotNull("processDefinitionId", processDefinitionId);
		this.processDefinitionId_Conflict = processDefinitionId;
		return this;
	  }

	  public virtual HistoricIdentityLinkLogQuery processDefinitionKey(string processDefinitionKey)
	  {
		ensureNotNull("processDefinitionKey", processDefinitionKey);
		this.processDefinitionKey_Conflict = processDefinitionKey;
		return this;
	  }

	  public virtual HistoricIdentityLinkLogQuery operationType(string operationType)
	  {
		ensureNotNull("operationType", operationType);
		this.operationType_Conflict = operationType;
		return this;
	  }

	  public virtual HistoricIdentityLinkLogQuery assignerId(string assignerId)
	  {
		ensureNotNull("assignerId", assignerId);
		this.assignerId_Conflict = assignerId;
		return this;
	  }

	  public virtual HistoricIdentityLinkLogQuery orderByTime()
	  {
		orderBy(HistoricIdentityLinkLogQueryProperty_Fields.TIME);
		return this;
	  }

	  public virtual HistoricIdentityLinkLogQuery orderByType()
	  {
		orderBy(HistoricIdentityLinkLogQueryProperty_Fields.TYPE);
		return this;
	  }

	  public virtual HistoricIdentityLinkLogQuery orderByUserId()
	  {
		orderBy(HistoricIdentityLinkLogQueryProperty_Fields.USER_ID);
		return this;
	  }

	  public virtual HistoricIdentityLinkLogQuery orderByGroupId()
	  {
		orderBy(HistoricIdentityLinkLogQueryProperty_Fields.GROUP_ID);
		return this;
	  }

	  public virtual HistoricIdentityLinkLogQuery orderByTaskId()
	  {
		orderBy(HistoricIdentityLinkLogQueryProperty_Fields.TASK_ID);
		return this;
	  }

	  public virtual HistoricIdentityLinkLogQuery orderByProcessDefinitionId()
	  {
		orderBy(HistoricIdentityLinkLogQueryProperty_Fields.PROC_DEFINITION_ID);
		return this;
	  }

	  public virtual HistoricIdentityLinkLogQuery orderByProcessDefinitionKey()
	  {
		orderBy(HistoricIdentityLinkLogQueryProperty_Fields.PROC_DEFINITION_KEY);
		return this;
	  }

	  public virtual HistoricIdentityLinkLogQuery orderByOperationType()
	  {
		orderBy(HistoricIdentityLinkLogQueryProperty_Fields.OPERATION_TYPE);
		return this;
	  }

	  public virtual HistoricIdentityLinkLogQuery orderByAssignerId()
	  {
		orderBy(HistoricIdentityLinkLogQueryProperty_Fields.ASSIGNER_ID);
		return this;
	  }

	  public virtual HistoricIdentityLinkLogQuery orderByTenantId()
	  {
		orderBy(HistoricIdentityLinkLogQueryProperty_Fields.TENANT_ID);
		return this;
	  }

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.HistoricIdentityLinkManager.findHistoricIdentityLinkLogCountByQueryCriteria(this);
	  }

	  public override IList<HistoricIdentityLinkLog> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.HistoricIdentityLinkManager.findHistoricIdentityLinkLogByQueryCriteria(this, page);
	  }
	}

}