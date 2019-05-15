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

	using EventType = org.camunda.bpm.engine.impl.@event.EventType;

	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using SuspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ExecutionQuery = org.camunda.bpm.engine.runtime.ExecutionQuery;


	/// <summary>
	/// @author Joram Barrez
	/// @author Frederik Heremans
	/// @author Daniel Meyer
	/// </summary>
	public class ExecutionQueryImpl : AbstractVariableQueryImpl<ExecutionQuery, Execution>, ExecutionQuery
	{

	  private const long serialVersionUID = 1L;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionKey_Renamed;
	  protected internal string businessKey;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string activityId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string executionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processInstanceId_Renamed;
	  protected internal IList<EventSubscriptionQueryValue> eventSubscriptions;
	  protected internal SuspensionState suspensionState;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string incidentType_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string incidentId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string incidentMessage_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string incidentMessageLike_Renamed;

	  protected internal bool isTenantIdSet = false;
	  protected internal string[] tenantIds;

	  public ExecutionQueryImpl()
	  {
	  }

	  public ExecutionQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual ExecutionQueryImpl processDefinitionId(string processDefinitionId)
	  {
		ensureNotNull("Process definition id", processDefinitionId);
		this.processDefinitionId_Renamed = processDefinitionId;
		return this;
	  }

	  public virtual ExecutionQueryImpl processDefinitionKey(string processDefinitionKey)
	  {
		ensureNotNull("Process definition key", processDefinitionKey);
		this.processDefinitionKey_Renamed = processDefinitionKey;
		return this;
	  }

	  public virtual ExecutionQueryImpl processInstanceId(string processInstanceId)
	  {
		ensureNotNull("Process instance id", processInstanceId);
		this.processInstanceId_Renamed = processInstanceId;
		return this;
	  }

	  public virtual ExecutionQuery processInstanceBusinessKey(string businessKey)
	  {
		ensureNotNull("Business key", businessKey);
		this.businessKey = businessKey;
		return this;
	  }

	  public virtual ExecutionQueryImpl executionId(string executionId)
	  {
		ensureNotNull("Execution id", executionId);
		this.executionId_Renamed = executionId;
		return this;
	  }

	  public virtual ExecutionQueryImpl activityId(string activityId)
	  {
		this.activityId_Renamed = activityId;
		return this;
	  }

	  public virtual ExecutionQuery signalEventSubscription(string signalName)
	  {
		return eventSubscription(EventType.SIGNAL, signalName);
	  }

	  public virtual ExecutionQuery signalEventSubscriptionName(string signalName)
	  {
		return eventSubscription(EventType.SIGNAL, signalName);
	  }

	  public virtual ExecutionQuery messageEventSubscriptionName(string messageName)
	  {
		return eventSubscription(EventType.MESSAGE, messageName);
	  }

	  public virtual ExecutionQuery messageEventSubscription()
	  {
		return eventSubscription(EventType.MESSAGE, null);
	  }

	  public virtual ExecutionQuery eventSubscription(EventType eventType, string eventName)
	  {
		ensureNotNull("event type", eventType);
		if (!EventType.MESSAGE.Equals(eventType))
		{
		  // event name is optional for message events
		  ensureNotNull("event name", eventName);
		}
		if (eventSubscriptions == null)
		{
		  eventSubscriptions = new List<EventSubscriptionQueryValue>();
		}
		eventSubscriptions.Add(new EventSubscriptionQueryValue(eventName, eventType.name()));
		return this;
	  }

	  public virtual ExecutionQuery suspended()
	  {
		this.suspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.SUSPENDED;
		return this;
	  }

	  public virtual ExecutionQuery active()
	  {
		this.suspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.ACTIVE;
		return this;
	  }

	  public virtual ExecutionQuery processVariableValueEquals(string variableName, object variableValue)
	  {
		addVariable(variableName, variableValue, QueryOperator.EQUALS, false);
		return this;
	  }

	  public virtual ExecutionQuery processVariableValueNotEquals(string variableName, object variableValue)
	  {
		addVariable(variableName, variableValue, QueryOperator.NOT_EQUALS, false);
		return this;
	  }

	  public virtual ExecutionQuery incidentType(string incidentType)
	  {
		ensureNotNull("incident type", incidentType);
		this.incidentType_Renamed = incidentType;
		return this;
	  }

	  public virtual ExecutionQuery incidentId(string incidentId)
	  {
		ensureNotNull("incident id", incidentId);
		this.incidentId_Renamed = incidentId;
		return this;
	  }

	  public virtual ExecutionQuery incidentMessage(string incidentMessage)
	  {
		ensureNotNull("incident message", incidentMessage);
		this.incidentMessage_Renamed = incidentMessage;
		return this;
	  }

	  public virtual ExecutionQuery incidentMessageLike(string incidentMessageLike)
	  {
		ensureNotNull("incident messageLike", incidentMessageLike);
		this.incidentMessageLike_Renamed = incidentMessageLike;
		return this;
	  }

	  public virtual ExecutionQuery tenantIdIn(params string[] tenantIds)
	  {
		ensureNotNull("tenantIds", (object[]) tenantIds);
		this.tenantIds = tenantIds;
		isTenantIdSet = true;
		return this;
	  }

	  public virtual ExecutionQuery withoutTenantId()
	  {
		this.tenantIds = null;
		isTenantIdSet = true;
		return this;
	  }

	  //ordering ////////////////////////////////////////////////////

	  public virtual ExecutionQueryImpl orderByProcessInstanceId()
	  {
		orderBy(ExecutionQueryProperty_Fields.PROCESS_INSTANCE_ID);
		return this;
	  }

	  public virtual ExecutionQueryImpl orderByProcessDefinitionId()
	  {
		orderBy(new QueryOrderingProperty(QueryOrderingProperty.RELATION_PROCESS_DEFINITION, ExecutionQueryProperty_Fields.PROCESS_DEFINITION_ID));
		return this;
	  }

	  public virtual ExecutionQueryImpl orderByProcessDefinitionKey()
	  {
		orderBy(new QueryOrderingProperty(QueryOrderingProperty.RELATION_PROCESS_DEFINITION, ExecutionQueryProperty_Fields.PROCESS_DEFINITION_KEY));
		return this;
	  }

	  public virtual ExecutionQuery orderByTenantId()
	  {
		orderBy(ExecutionQueryProperty_Fields.TENANT_ID);
		return this;
	  }

	  //results ////////////////////////////////////////////////////

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		ensureVariablesInitialized();
		return commandContext.ExecutionManager.findExecutionCountByQueryCriteria(this);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.runtime.Execution> executeList(org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext, Page page)
	  public override IList<Execution> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		ensureVariablesInitialized();
		return (System.Collections.IList) commandContext.ExecutionManager.findExecutionsByQueryCriteria(this, page);
	  }

	  //getters ////////////////////////////////////////////////////

	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey_Renamed;
		  }
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId_Renamed;
		  }
	  }

	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId_Renamed;
		  }
	  }

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId_Renamed;
		  }
	  }

	  public virtual string ProcessInstanceIds
	  {
		  get
		  {
			return null;
		  }
	  }

	  public virtual string BusinessKey
	  {
		  get
		  {
			return businessKey;
		  }
	  }

	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId_Renamed;
		  }
	  }

	  public virtual SuspensionState SuspensionState
	  {
		  get
		  {
			return suspensionState;
		  }
		  set
		  {
			this.suspensionState = value;
		  }
	  }


	  public virtual IList<EventSubscriptionQueryValue> EventSubscriptions
	  {
		  get
		  {
			return eventSubscriptions;
		  }
		  set
		  {
			this.eventSubscriptions = value;
		  }
	  }


	  public virtual string IncidentId
	  {
		  get
		  {
			return incidentId_Renamed;
		  }
	  }

	  public virtual string IncidentType
	  {
		  get
		  {
			return incidentType_Renamed;
		  }
	  }

	  public virtual string IncidentMessage
	  {
		  get
		  {
			return incidentMessage_Renamed;
		  }
	  }

	  public virtual string IncidentMessageLike
	  {
		  get
		  {
			return incidentMessageLike_Renamed;
		  }
	  }

	}

}