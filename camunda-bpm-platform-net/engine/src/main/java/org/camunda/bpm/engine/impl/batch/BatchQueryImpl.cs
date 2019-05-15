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

	using Batch = org.camunda.bpm.engine.batch.Batch;
	using BatchQuery = org.camunda.bpm.engine.batch.BatchQuery;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using SuspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState;

	[Serializable]
	public class BatchQueryImpl : AbstractQuery<BatchQuery, Batch>, BatchQuery
	{

	  private const long serialVersionUID = 1L;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string batchId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string type_Renamed;
	  protected internal bool isTenantIdSet = false;
	  protected internal string[] tenantIds;
	  protected internal SuspensionState suspensionState;

	  public BatchQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual BatchQuery batchId(string batchId)
	  {
		ensureNotNull("Batch id", batchId);
		this.batchId_Renamed = batchId;
		return this;
	  }

	  public virtual string BatchId
	  {
		  get
		  {
			return batchId_Renamed;
		  }
	  }

	  public virtual BatchQuery type(string type)
	  {
		ensureNotNull("Type", type);
		this.type_Renamed = type;
		return this;
	  }

	  public virtual string Type
	  {
		  get
		  {
			return type_Renamed;
		  }
	  }

	  public virtual BatchQuery tenantIdIn(params string[] tenantIds)
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

	  public virtual BatchQuery withoutTenantId()
	  {
		this.tenantIds = null;
		isTenantIdSet = true;
		return this;
	  }

	  public virtual BatchQuery active()
	  {
		this.suspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.ACTIVE;
		return this;
	  }

	  public virtual BatchQuery suspended()
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

	  public virtual BatchQuery orderById()
	  {
		return orderBy(org.camunda.bpm.engine.impl.BatchQueryProperty_Fields.ID);
	  }

	  public virtual BatchQuery orderByTenantId()
	  {
		return orderBy(org.camunda.bpm.engine.impl.BatchQueryProperty_Fields.TENANT_ID);
	  }

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.BatchManager.findBatchCountByQueryCriteria(this);
	  }

	  public override IList<Batch> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.BatchManager.findBatchesByQueryCriteria(this, page);
	  }

	}

}