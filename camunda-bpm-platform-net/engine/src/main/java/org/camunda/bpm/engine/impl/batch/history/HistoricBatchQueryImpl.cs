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
namespace org.camunda.bpm.engine.impl.batch.history
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using HistoricBatchQuery = org.camunda.bpm.engine.batch.history.HistoricBatchQuery;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;

	[Serializable]
	public class HistoricBatchQueryImpl : AbstractQuery<HistoricBatchQuery, HistoricBatch>, HistoricBatchQuery
	{

	  private const long serialVersionUID = 1L;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string batchId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string type_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool? completed_Conflict;
	  protected internal bool isTenantIdSet = false;
	  protected internal string[] tenantIds;

	  public HistoricBatchQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual HistoricBatchQuery batchId(string batchId)
	  {
		ensureNotNull("Batch id", batchId);
		this.batchId_Conflict = batchId;
		return this;
	  }

	  public virtual string BatchId
	  {
		  get
		  {
			return batchId_Conflict;
		  }
	  }

	  public virtual HistoricBatchQuery type(string type)
	  {
		ensureNotNull("Type", type);
		this.type_Conflict = type;
		return this;
	  }

	  public virtual HistoricBatchQuery completed(bool completed)
	  {
		this.completed_Conflict = completed;
		return this;
	  }

	  public virtual HistoricBatchQuery tenantIdIn(params string[] tenantIds)
	  {
		ensureNotNull("tenantIds", (object[]) tenantIds);
		this.tenantIds = tenantIds;
		isTenantIdSet = true;
		return this;
	  }

	  public virtual string[] TenantIds
	  {
		  get
		  {
			return tenantIds;
		  }
	  }

	  public virtual bool TenantIdSet
	  {
		  get
		  {
			return isTenantIdSet;
		  }
	  }

	  public virtual HistoricBatchQuery withoutTenantId()
	  {
		this.tenantIds = null;
		isTenantIdSet = true;
		return this;
	  }

	  public virtual string Type
	  {
		  get
		  {
			return type_Conflict;
		  }
	  }

	  public virtual HistoricBatchQuery orderById()
	  {
		return orderBy(org.camunda.bpm.engine.impl.HistoricBatchQueryProperty_Fields.ID);
	  }

	  public virtual HistoricBatchQuery orderByStartTime()
	  {
		return orderBy(org.camunda.bpm.engine.impl.HistoricBatchQueryProperty_Fields.START_TIME);
	  }

	  public virtual HistoricBatchQuery orderByEndTime()
	  {
		return orderBy(org.camunda.bpm.engine.impl.HistoricBatchQueryProperty_Fields.END_TIME);
	  }

	  public virtual HistoricBatchQuery orderByTenantId()
	  {
		return orderBy(org.camunda.bpm.engine.impl.HistoricBatchQueryProperty_Fields.TENANT_ID);
	  }

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.HistoricBatchManager.findBatchCountByQueryCriteria(this);
	  }


	  public override IList<HistoricBatch> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.HistoricBatchManager.findBatchesByQueryCriteria(this, page);
	  }
	}

}