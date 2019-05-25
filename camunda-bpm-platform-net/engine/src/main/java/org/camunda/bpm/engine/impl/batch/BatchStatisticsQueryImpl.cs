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
namespace org.camunda.bpm.engine.impl.batch
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using BatchStatistics = org.camunda.bpm.engine.batch.BatchStatistics;
	using BatchStatisticsQuery = org.camunda.bpm.engine.batch.BatchStatisticsQuery;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using SuspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState;

	[Serializable]
	public class BatchStatisticsQueryImpl : AbstractQuery<BatchStatisticsQuery, BatchStatistics>, BatchStatisticsQuery
	{

	  protected internal const long serialVersionUID = 1L;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string batchId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string type_Conflict;
	  protected internal bool isTenantIdSet = false;
	  protected internal string[] tenantIds;
	  protected internal SuspensionState suspensionState;

	  public BatchStatisticsQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual BatchStatisticsQuery batchId(string batchId)
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

	  public virtual BatchStatisticsQuery type(string type)
	  {
		ensureNotNull("Type", type);
		this.type_Conflict = type;
		return this;
	  }

	  public virtual string Type
	  {
		  get
		  {
			return type_Conflict;
		  }
	  }

	  public virtual BatchStatisticsQuery tenantIdIn(params string[] tenantIds)
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

	  public virtual BatchStatisticsQuery withoutTenantId()
	  {
		this.tenantIds = null;
		isTenantIdSet = true;
		return this;
	  }

	  public virtual BatchStatisticsQuery active()
	  {
		this.suspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.ACTIVE;
		return this;
	  }

	  public virtual BatchStatisticsQuery suspended()
	  {
		this.suspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.SUSPENDED;
		return this;
	  }

	  public virtual SuspensionState SuspensionState
	  {
		  get
		  {
			return suspensionState;
		  }
	  }

	  public virtual BatchStatisticsQuery orderById()
	  {
		return orderBy(org.camunda.bpm.engine.impl.BatchQueryProperty_Fields.ID);
	  }

	  public virtual BatchStatisticsQuery orderByTenantId()
	  {
		return orderBy(org.camunda.bpm.engine.impl.BatchQueryProperty_Fields.TENANT_ID);
	  }

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.StatisticsManager.getStatisticsCountGroupedByBatch(this);
	  }

	  public override IList<BatchStatistics> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.StatisticsManager.getStatisticsGroupedByBatch(this, page);
	  }

	}

}