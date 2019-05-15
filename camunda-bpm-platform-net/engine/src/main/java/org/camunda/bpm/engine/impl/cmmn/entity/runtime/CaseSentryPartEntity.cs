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
namespace org.camunda.bpm.engine.impl.cmmn.entity.runtime
{

	using CmmnExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnExecution;
	using CmmnSentryPart = org.camunda.bpm.engine.impl.cmmn.execution.CmmnSentryPart;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using HasDbReferences = org.camunda.bpm.engine.impl.db.HasDbReferences;
	using HasDbRevision = org.camunda.bpm.engine.impl.db.HasDbRevision;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[Serializable]
	public class CaseSentryPartEntity : CmmnSentryPart, DbEntity, HasDbRevision, HasDbReferences
	{

	  private const long serialVersionUID = 1L;

	  // references

	  protected internal CaseExecutionEntity caseInstance;
	  protected internal CaseExecutionEntity caseExecution;
	  protected internal CaseExecutionEntity sourceCaseExecution;

	  // persistence

	  protected internal string id;
	  protected internal int revision = 1;
	  protected internal string caseInstanceId;
	  protected internal string caseExecutionId;
	  protected internal string sourceCaseExecutionId;
	  protected internal string tenantId;
	  private bool forcedUpdate;

	  // id ///////////////////////////////////////////////////////////////////

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
		  set
		  {
			this.id = value;
		  }
	  }


	  // case instance id /////////////////////////////////////////////////////

	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId;
		  }
	  }

	  public override CaseExecutionEntity getCaseInstance()
	  {
		ensureCaseInstanceInitialized();
		return caseInstance;
	  }

	  protected internal virtual void ensureCaseInstanceInitialized()
	  {
		if ((caseInstance == null) && (!string.ReferenceEquals(caseInstanceId, null)))
		{
		  caseInstance = findCaseExecutionById(caseInstanceId);
		}
	  }

	  public override void setCaseInstance(CmmnExecution caseInstance)
	  {
		this.caseInstance = (CaseExecutionEntity) caseInstance;

		if (caseInstance != null)
		{
		  caseInstanceId = caseInstance.Id;
		}
		else
		{
		  caseInstanceId = null;
		}
	  }

	  // case execution id //////////////////////////////////////////////////////

	  public virtual string CaseExecutionId
	  {
		  get
		  {
			return caseExecutionId;
		  }
	  }

	  public override CaseExecutionEntity getCaseExecution()
	  {
		ensureCaseExecutionInitialized();
		return caseExecution;
	  }

	  protected internal virtual void ensureCaseExecutionInitialized()
	  {
		if ((caseExecution == null) && (!string.ReferenceEquals(caseExecutionId, null)))
		{
		  caseExecution = findCaseExecutionById(caseExecutionId);
		}
	  }

	  public override void setCaseExecution(CmmnExecution caseExecution)
	  {
		this.caseExecution = (CaseExecutionEntity) caseExecution;

		if (caseExecution != null)
		{
		  caseExecutionId = caseExecution.Id;
		}
		else
		{
		  caseExecutionId = null;
		}
	  }

	  // source case execution id //////////////////////////////////////////////////

	  public override string SourceCaseExecutionId
	  {
		  get
		  {
			return sourceCaseExecutionId;
		  }
	  }

	  public override CmmnExecution SourceCaseExecution
	  {
		  get
		  {
			ensureSourceCaseExecutionInitialized();
			return sourceCaseExecution;
		  }
		  set
		  {
			this.sourceCaseExecution = (CaseExecutionEntity) value;
    
			if (value != null)
			{
			  sourceCaseExecutionId = value.Id;
			}
			else
			{
			  sourceCaseExecutionId = null;
			}
		  }
	  }

	  protected internal virtual void ensureSourceCaseExecutionInitialized()
	  {
		if ((sourceCaseExecution == null) && (!string.ReferenceEquals(sourceCaseExecutionId, null)))
		{
		  sourceCaseExecution = findCaseExecutionById(sourceCaseExecutionId);
		}
	  }


	  // persistence /////////////////////////////////////////////////////////

	  public virtual int Revision
	  {
		  get
		  {
			return revision;
		  }
		  set
		  {
			this.revision = value;
		  }
	  }


	  public virtual int RevisionNext
	  {
		  get
		  {
			return revision + 1;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
		  set
		  {
			this.tenantId = value;
		  }
	  }


	  public virtual void forceUpdate()
	  {
		this.forcedUpdate = true;
	  }

	  public virtual object PersistentState
	  {
		  get
		  {
			IDictionary<string, object> persistentState = new Dictionary<string, object>();
			persistentState["satisfied"] = Satisfied;
    
			if (forcedUpdate)
			{
			  persistentState["forcedUpdate"] = true;
			}
    
			return persistentState;
		  }
	  }

	  // helper ////////////////////////////////////////////////////////////////////

	  protected internal virtual CaseExecutionEntity findCaseExecutionById(string caseExecutionId)
	  {
		return Context.CommandContext.CaseExecutionManager.findCaseExecutionById(caseExecutionId);
	  }

	  public virtual ISet<string> ReferencedEntityIds
	  {
		  get
		  {
			ISet<string> referencedEntityIds = new HashSet<string>();
			return referencedEntityIds;
		  }
	  }

	  public virtual IDictionary<string, Type> ReferencedEntitiesIdAndClass
	  {
		  get
		  {
			IDictionary<string, Type> referenceIdAndClass = new Dictionary<string, Type>();
    
			if (!string.ReferenceEquals(caseExecutionId, null))
			{
			  referenceIdAndClass[caseExecutionId] = typeof(CaseExecutionEntity);
			}
			if (!string.ReferenceEquals(caseInstanceId, null))
			{
			  referenceIdAndClass[caseInstanceId] = typeof(CaseExecutionEntity);
			}
    
			return referenceIdAndClass;
		  }
	  }
	}

}