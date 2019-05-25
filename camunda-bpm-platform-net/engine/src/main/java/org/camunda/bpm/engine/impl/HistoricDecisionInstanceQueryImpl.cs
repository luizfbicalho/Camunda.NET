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


	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricDecisionInstanceQuery = org.camunda.bpm.engine.history.HistoricDecisionInstanceQuery;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;

	/// <summary>
	///  @author Philipp Ossler
	/// </summary>
	[Serializable]
	public class HistoricDecisionInstanceQueryImpl : AbstractQuery<HistoricDecisionInstanceQuery, HistoricDecisionInstance>, HistoricDecisionInstanceQuery
	{

	  private const long serialVersionUID = 1L;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string decisionInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string[] decisionInstanceIdIn_Conflict;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string decisionDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string[] decisionDefinitionIdIn_Conflict;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string decisionDefinitionKey_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string[] decisionDefinitionKeyIn_Conflict;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string decisionDefinitionName_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string decisionDefinitionNameLike_Conflict;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionKey_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processInstanceId_Conflict;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseDefinitionKey_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseInstanceId_Conflict;

	  protected internal string[] activityInstanceIds;
	  protected internal string[] activityIds;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime evaluatedBefore_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime evaluatedAfter_Conflict;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string userId_Conflict;

	  protected internal bool includeInput = false;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool includeOutputs_Conflict = false;

	  protected internal bool isByteArrayFetchingEnabled = true;
	  protected internal bool isCustomObjectDeserializationEnabled = true;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string rootDecisionInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool rootDecisionInstancesOnly_Conflict = false;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string decisionRequirementsDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string decisionRequirementsDefinitionKey_Conflict;

	  protected internal string[] tenantIds;

	  public HistoricDecisionInstanceQueryImpl()
	  {
	  }

	  public HistoricDecisionInstanceQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual HistoricDecisionInstanceQuery decisionInstanceId(string decisionInstanceId)
	  {
		ensureNotNull(typeof(NotValidException), "decisionInstanceId", decisionInstanceId);
		this.decisionInstanceId_Conflict = decisionInstanceId;
		return this;
	  }

	  public virtual HistoricDecisionInstanceQuery decisionInstanceIdIn(params string[] decisionInstanceIdIn)
	  {
		ensureNotNull("decisionInstanceIdIn", (object[]) decisionInstanceIdIn);
		this.decisionInstanceIdIn_Conflict = decisionInstanceIdIn;
		return this;
	  }

	  public virtual HistoricDecisionInstanceQuery decisionDefinitionId(string decisionDefinitionId)
	  {
		ensureNotNull(typeof(NotValidException), "decisionDefinitionId", decisionDefinitionId);
		this.decisionDefinitionId_Conflict = decisionDefinitionId;
		return this;
	  }

	  public virtual HistoricDecisionInstanceQuery decisionDefinitionIdIn(params string[] decisionDefinitionIdIn)
	  {
		ensureNotNull(typeof(NotValidException), "decisionDefinitionIdIn", decisionDefinitionIdIn);
		this.decisionDefinitionIdIn_Conflict = decisionDefinitionIdIn;
		return this;
	  }

	  public virtual HistoricDecisionInstanceQuery decisionDefinitionKey(string decisionDefinitionKey)
	  {
		ensureNotNull(typeof(NotValidException), "decisionDefinitionKey", decisionDefinitionKey);
		this.decisionDefinitionKey_Conflict = decisionDefinitionKey;
		return this;
	  }

	  public virtual HistoricDecisionInstanceQuery decisionDefinitionKeyIn(params string[] decisionDefinitionKeyIn)
	  {
		ensureNotNull(typeof(NotValidException), "decisionDefinitionKeyIn", decisionDefinitionKeyIn);
		this.decisionDefinitionKeyIn_Conflict = decisionDefinitionKeyIn;
		return this;
	  }

	  public virtual HistoricDecisionInstanceQuery decisionDefinitionName(string decisionDefinitionName)
	  {
		ensureNotNull(typeof(NotValidException), "decisionDefinitionName", decisionDefinitionName);
		this.decisionDefinitionName_Conflict = decisionDefinitionName;
		return this;
	  }

	  public virtual HistoricDecisionInstanceQuery decisionDefinitionNameLike(string decisionDefinitionNameLike)
	  {
		ensureNotNull(typeof(NotValidException), "decisionDefinitionNameLike", decisionDefinitionNameLike);
		this.decisionDefinitionNameLike_Conflict = decisionDefinitionNameLike;
		return this;
	  }

	  public virtual HistoricDecisionInstanceQuery processDefinitionKey(string processDefinitionKey)
	  {
		ensureNotNull(typeof(NotValidException), "processDefinitionKey", processDefinitionKey);
		this.processDefinitionKey_Conflict = processDefinitionKey;
		return this;
	  }

	  public virtual HistoricDecisionInstanceQuery processDefinitionId(string processDefinitionId)
	  {
		ensureNotNull(typeof(NotValidException), "processDefinitionId", processDefinitionId);
		this.processDefinitionId_Conflict = processDefinitionId;
		return this;
	  }

	  public virtual HistoricDecisionInstanceQuery processInstanceId(string processInstanceId)
	  {
		ensureNotNull(typeof(NotValidException), "processInstanceId", processInstanceId);
		this.processInstanceId_Conflict = processInstanceId;
		return this;
	  }

	  public virtual HistoricDecisionInstanceQuery caseDefinitionKey(string caseDefinitionKey)
	  {
		ensureNotNull(typeof(NotValidException), "caseDefinitionKey", caseDefinitionKey);
		this.caseDefinitionKey_Conflict = caseDefinitionKey;
		return this;
	  }

	  public virtual HistoricDecisionInstanceQuery caseDefinitionId(string caseDefinitionId)
	  {
		ensureNotNull(typeof(NotValidException), "caseDefinitionId", caseDefinitionId);
		this.caseDefinitionId_Conflict = caseDefinitionId;
		return this;
	  }

	  public virtual HistoricDecisionInstanceQuery caseInstanceId(string caseInstanceId)
	  {
		ensureNotNull(typeof(NotValidException), "caseInstanceId", caseInstanceId);
		this.caseInstanceId_Conflict = caseInstanceId;
		return this;
	  }

	  public virtual HistoricDecisionInstanceQuery activityIdIn(params string[] activityIds)
	  {
		ensureNotNull("activityIds", (object[]) activityIds);
		this.activityIds = activityIds;
		return this;
	  }

	  public virtual HistoricDecisionInstanceQuery activityInstanceIdIn(params string[] activityInstanceIds)
	  {
		ensureNotNull("activityInstanceIds", (object[]) activityInstanceIds);
		this.activityInstanceIds = activityInstanceIds;
		return this;
	  }

	  public virtual HistoricDecisionInstanceQuery evaluatedBefore(DateTime evaluatedBefore)
	  {
		ensureNotNull(typeof(NotValidException), "evaluatedBefore", evaluatedBefore);
		this.evaluatedBefore_Conflict = evaluatedBefore;
		return this;
	  }

	  public virtual HistoricDecisionInstanceQuery evaluatedAfter(DateTime evaluatedAfter)
	  {
		ensureNotNull(typeof(NotValidException), "evaluatedAfter", evaluatedAfter);
		this.evaluatedAfter_Conflict = evaluatedAfter;
		return this;
	  }

	  public virtual HistoricDecisionInstanceQuery tenantIdIn(params string[] tenantIds)
	  {
		ensureNotNull("tenantIds", (object[]) tenantIds);
		this.tenantIds = tenantIds;
		return this;
	  }

	  public virtual HistoricDecisionInstanceQuery orderByTenantId()
	  {
		return orderBy(HistoricDecisionInstanceQueryProperty_Fields.TENANT_ID);
	  }

	  public virtual HistoricDecisionInstanceQuery userId(string userId)
	  {
		ensureNotNull(typeof(NotValidException), "userId", userId);
		this.userId_Conflict = userId;
		return this;
	  }

	  public virtual HistoricDecisionInstanceQuery orderByEvaluationTime()
	  {
		orderBy(HistoricDecisionInstanceQueryProperty_Fields.EVALUATION_TIME);
		return this;
	  }

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.HistoricDecisionInstanceManager.findHistoricDecisionInstanceCountByQueryCriteria(this);
	  }

	  public override IList<HistoricDecisionInstance> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.HistoricDecisionInstanceManager.findHistoricDecisionInstancesByQueryCriteria(this, page);
	  }

	  public virtual string DecisionDefinitionId
	  {
		  get
		  {
			return decisionDefinitionId_Conflict;
		  }
	  }

	  public virtual string DecisionDefinitionKey
	  {
		  get
		  {
			return decisionDefinitionKey_Conflict;
		  }
	  }

	  public virtual string DecisionDefinitionName
	  {
		  get
		  {
			return decisionDefinitionName_Conflict;
		  }
	  }

	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey_Conflict;
		  }
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId_Conflict;
		  }
	  }

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId_Conflict;
		  }
	  }

	  public virtual string CaseDefinitionKey
	  {
		  get
		  {
			return caseDefinitionKey_Conflict;
		  }
	  }

	  public virtual string CaseDefinitionId
	  {
		  get
		  {
			return caseDefinitionId_Conflict;
		  }
	  }

	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId_Conflict;
		  }
	  }

	  public virtual string[] ActivityInstanceIds
	  {
		  get
		  {
			return activityInstanceIds;
		  }
	  }

	  public virtual string[] ActivityIds
	  {
		  get
		  {
			return activityIds;
		  }
	  }

	  public virtual string[] TenantIds
	  {
		  get
		  {
			return tenantIds;
		  }
	  }

	  public virtual HistoricDecisionInstanceQuery includeInputs()
	  {
		includeInput = true;
		return this;
	  }

	  public virtual HistoricDecisionInstanceQuery includeOutputs()
	  {
		includeOutputs_Conflict = true;
		return this;
	  }

	  public virtual bool IncludeInput
	  {
		  get
		  {
			return includeInput;
		  }
	  }

	  public virtual bool IncludeOutputs
	  {
		  get
		  {
			return includeOutputs_Conflict;
		  }
	  }

	  public virtual HistoricDecisionInstanceQuery disableBinaryFetching()
	  {
		isByteArrayFetchingEnabled = false;
		return this;
	  }

	  public virtual HistoricDecisionInstanceQuery disableCustomObjectDeserialization()
	  {
		isCustomObjectDeserializationEnabled = false;
		return this;
	  }

	  public virtual bool ByteArrayFetchingEnabled
	  {
		  get
		  {
			return isByteArrayFetchingEnabled;
		  }
	  }

	  public virtual bool CustomObjectDeserializationEnabled
	  {
		  get
		  {
			return isCustomObjectDeserializationEnabled;
		  }
	  }

	  public virtual string RootDecisionInstanceId
	  {
		  get
		  {
			return rootDecisionInstanceId_Conflict;
		  }
	  }

	  public virtual HistoricDecisionInstanceQuery rootDecisionInstanceId(string rootDecisionInstanceId)
	  {
		ensureNotNull(typeof(NotValidException), "rootDecisionInstanceId", rootDecisionInstanceId);
		this.rootDecisionInstanceId_Conflict = rootDecisionInstanceId;
		return this;
	  }

	  public virtual bool RootDecisionInstancesOnly
	  {
		  get
		  {
			return rootDecisionInstancesOnly_Conflict;
		  }
	  }

	  public virtual HistoricDecisionInstanceQuery rootDecisionInstancesOnly()
	  {
		this.rootDecisionInstancesOnly_Conflict = true;
		return this;
	  }

	  public virtual HistoricDecisionInstanceQuery decisionRequirementsDefinitionId(string decisionRequirementsDefinitionId)
	  {
		ensureNotNull(typeof(NotValidException), "decisionRequirementsDefinitionId", decisionRequirementsDefinitionId);
		this.decisionRequirementsDefinitionId_Conflict = decisionRequirementsDefinitionId;
		return this;
	  }

	  public virtual HistoricDecisionInstanceQuery decisionRequirementsDefinitionKey(string decisionRequirementsDefinitionKey)
	  {
		ensureNotNull(typeof(NotValidException), "decisionRequirementsDefinitionKey", decisionRequirementsDefinitionKey);
		this.decisionRequirementsDefinitionKey_Conflict = decisionRequirementsDefinitionKey;
		return this;
	  }

	  public virtual string DecisionRequirementsDefinitionId
	  {
		  get
		  {
			return decisionRequirementsDefinitionId_Conflict;
		  }
	  }

	  public virtual string DecisionRequirementsDefinitionKey
	  {
		  get
		  {
			return decisionRequirementsDefinitionKey_Conflict;
		  }
	  }
	}

}