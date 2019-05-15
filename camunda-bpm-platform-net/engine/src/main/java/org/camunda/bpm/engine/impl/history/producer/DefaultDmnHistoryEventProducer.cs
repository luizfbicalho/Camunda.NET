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
namespace org.camunda.bpm.engine.impl.history.producer
{

	using DmnDecisionEvaluationEvent = org.camunda.bpm.dmn.engine.@delegate.DmnDecisionEvaluationEvent;
	using DmnDecisionLiteralExpressionEvaluationEvent = org.camunda.bpm.dmn.engine.@delegate.DmnDecisionLiteralExpressionEvaluationEvent;
	using DmnDecisionLogicEvaluationEvent = org.camunda.bpm.dmn.engine.@delegate.DmnDecisionLogicEvaluationEvent;
	using DmnDecisionTableEvaluationEvent = org.camunda.bpm.dmn.engine.@delegate.DmnDecisionTableEvaluationEvent;
	using DmnEvaluatedDecisionRule = org.camunda.bpm.dmn.engine.@delegate.DmnEvaluatedDecisionRule;
	using DmnEvaluatedInput = org.camunda.bpm.dmn.engine.@delegate.DmnEvaluatedInput;
	using DmnEvaluatedOutput = org.camunda.bpm.dmn.engine.@delegate.DmnEvaluatedOutput;
	using DelegateCaseExecution = org.camunda.bpm.engine.@delegate.DelegateCaseExecution;
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using HistoricDecisionInputInstance = org.camunda.bpm.engine.history.HistoricDecisionInputInstance;
	using HistoricDecisionOutputInstance = org.camunda.bpm.engine.history.HistoricDecisionOutputInstance;
	using TenantIdProvider = org.camunda.bpm.engine.impl.cfg.multitenancy.TenantIdProvider;
	using TenantIdProviderHistoricDecisionInstanceContext = org.camunda.bpm.engine.impl.cfg.multitenancy.TenantIdProviderHistoricDecisionInstanceContext;
	using CaseDefinitionEntity = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity;
	using CaseExecutionEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;
	using HistoricDecisionEvaluationEvent = org.camunda.bpm.engine.impl.history.@event.HistoricDecisionEvaluationEvent;
	using HistoricDecisionInputInstanceEntity = org.camunda.bpm.engine.impl.history.@event.HistoricDecisionInputInstanceEntity;
	using HistoricDecisionInstanceEntity = org.camunda.bpm.engine.impl.history.@event.HistoricDecisionInstanceEntity;
	using HistoricDecisionOutputInstanceEntity = org.camunda.bpm.engine.impl.history.@event.HistoricDecisionOutputInstanceEntity;
	using HistoricProcessInstanceEventEntity = org.camunda.bpm.engine.impl.history.@event.HistoricProcessInstanceEventEntity;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using DoubleValue = org.camunda.bpm.engine.variable.value.DoubleValue;
	using IntegerValue = org.camunda.bpm.engine.variable.value.IntegerValue;
	using LongValue = org.camunda.bpm.engine.variable.value.LongValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_REMOVAL_TIME_STRATEGY_START;

	/// <summary>
	/// @author Philipp Ossler
	/// @author Ingo Richtsmeier
	/// </summary>
	public class DefaultDmnHistoryEventProducer : DmnHistoryEventProducer
	{

	  protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.impl.history.event.HistoryEvent createDecisionEvaluatedEvt(final org.camunda.bpm.engine.delegate.DelegateExecution execution, final org.camunda.bpm.dmn.engine.delegate.DmnDecisionEvaluationEvent evaluationEvent)
	  public virtual HistoryEvent createDecisionEvaluatedEvt(DelegateExecution execution, DmnDecisionEvaluationEvent evaluationEvent)
	  {
		return createHistoryEvent(evaluationEvent, new HistoricDecisionInstanceSupplierAnonymousInnerClass(this, execution, evaluationEvent));
	  }

	  private class HistoricDecisionInstanceSupplierAnonymousInnerClass : HistoricDecisionInstanceSupplier
	  {
		  private readonly DefaultDmnHistoryEventProducer outerInstance;

		  private DelegateExecution execution;
		  private DmnDecisionEvaluationEvent evaluationEvent;

		  public HistoricDecisionInstanceSupplierAnonymousInnerClass(DefaultDmnHistoryEventProducer outerInstance, DelegateExecution execution, DmnDecisionEvaluationEvent evaluationEvent)
		  {
			  this.outerInstance = outerInstance;
			  this.execution = execution;
			  this.evaluationEvent = evaluationEvent;
		  }


		  public HistoricDecisionInstanceEntity createHistoricDecisionInstance(DmnDecisionLogicEvaluationEvent evaluationEvent, HistoricDecisionInstanceEntity rootDecisionInstance)
		  {
			return outerInstance.createDecisionEvaluatedEvt(evaluationEvent, (ExecutionEntity) execution);
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.impl.history.event.HistoryEvent createDecisionEvaluatedEvt(final org.camunda.bpm.engine.delegate.DelegateCaseExecution execution, final org.camunda.bpm.dmn.engine.delegate.DmnDecisionEvaluationEvent evaluationEvent)
	  public virtual HistoryEvent createDecisionEvaluatedEvt(DelegateCaseExecution execution, DmnDecisionEvaluationEvent evaluationEvent)
	  {
		return createHistoryEvent(evaluationEvent, new HistoricDecisionInstanceSupplierAnonymousInnerClass2(this, execution, evaluationEvent));
	  }

	  private class HistoricDecisionInstanceSupplierAnonymousInnerClass2 : HistoricDecisionInstanceSupplier
	  {
		  private readonly DefaultDmnHistoryEventProducer outerInstance;

		  private DelegateCaseExecution execution;
		  private DmnDecisionEvaluationEvent evaluationEvent;

		  public HistoricDecisionInstanceSupplierAnonymousInnerClass2(DefaultDmnHistoryEventProducer outerInstance, DelegateCaseExecution execution, DmnDecisionEvaluationEvent evaluationEvent)
		  {
			  this.outerInstance = outerInstance;
			  this.execution = execution;
			  this.evaluationEvent = evaluationEvent;
		  }


		  public HistoricDecisionInstanceEntity createHistoricDecisionInstance(DmnDecisionLogicEvaluationEvent evaluationEvent, HistoricDecisionInstanceEntity rootDecisionInstance)
		  {
			return outerInstance.createDecisionEvaluatedEvt(evaluationEvent, (CaseExecutionEntity) execution);
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.impl.history.event.HistoryEvent createDecisionEvaluatedEvt(final org.camunda.bpm.dmn.engine.delegate.DmnDecisionEvaluationEvent evaluationEvent)
	  public virtual HistoryEvent createDecisionEvaluatedEvt(DmnDecisionEvaluationEvent evaluationEvent)
	  {
		return createHistoryEvent(evaluationEvent, new HistoricDecisionInstanceSupplierAnonymousInnerClass3(this, evaluationEvent));
	  }

	  private class HistoricDecisionInstanceSupplierAnonymousInnerClass3 : HistoricDecisionInstanceSupplier
	  {
		  private readonly DefaultDmnHistoryEventProducer outerInstance;

		  private DmnDecisionEvaluationEvent evaluationEvent;

		  public HistoricDecisionInstanceSupplierAnonymousInnerClass3(DefaultDmnHistoryEventProducer outerInstance, DmnDecisionEvaluationEvent evaluationEvent)
		  {
			  this.outerInstance = outerInstance;
			  this.evaluationEvent = evaluationEvent;
		  }


		  public HistoricDecisionInstanceEntity createHistoricDecisionInstance(DmnDecisionLogicEvaluationEvent evaluationEvent, HistoricDecisionInstanceEntity rootDecisionInstance)
		  {
			return outerInstance.createDecisionEvaluatedEvt(evaluationEvent, rootDecisionInstance);
		  }
	  }

	  protected internal interface HistoricDecisionInstanceSupplier
	  {
		HistoricDecisionInstanceEntity createHistoricDecisionInstance(DmnDecisionLogicEvaluationEvent evaluationEvent, HistoricDecisionInstanceEntity rootDecisionInstance);
	  }

	  protected internal virtual HistoryEvent createHistoryEvent(DmnDecisionEvaluationEvent evaluationEvent, HistoricDecisionInstanceSupplier supplier)
	  {
		HistoricDecisionEvaluationEvent @event = newDecisionEvaluationEvent(evaluationEvent);

		HistoricDecisionInstanceEntity rootDecisionEvent = supplier.createHistoricDecisionInstance(evaluationEvent.DecisionResult, null);
		@event.RootHistoricDecisionInstance = rootDecisionEvent;

		IList<HistoricDecisionInstanceEntity> requiredDecisionEvents = new List<HistoricDecisionInstanceEntity>();
		foreach (DmnDecisionLogicEvaluationEvent requiredDecisionResult in evaluationEvent.RequiredDecisionResults)
		{
		  HistoricDecisionInstanceEntity requiredDecisionEvent = supplier.createHistoricDecisionInstance(requiredDecisionResult, rootDecisionEvent);
		  requiredDecisionEvents.Add(requiredDecisionEvent);
		}
		@event.RequiredHistoricDecisionInstances = requiredDecisionEvents;

		return @event;
	  }

	  protected internal virtual HistoricDecisionInstanceEntity createDecisionEvaluatedEvt(DmnDecisionLogicEvaluationEvent evaluationEvent, ExecutionEntity execution)
	  {
		// create event instance
		HistoricDecisionInstanceEntity @event = newDecisionInstanceEventEntity(execution, evaluationEvent);
		setReferenceToProcessInstance(@event, execution);

		if (HistoryRemovalTimeStrategyStart)
		{
		  provideRemovalTime(@event);
		}

		// initialize event
		initDecisionInstanceEvent(@event, evaluationEvent, HistoryEventTypes.DMN_DECISION_EVALUATE);

		DecisionDefinition decisionDefinition = (DecisionDefinition) evaluationEvent.Decision;
		string tenantId = execution.TenantId;
		if (string.ReferenceEquals(tenantId, null))
		{
		  tenantId = provideTenantId(decisionDefinition, @event);
		}
		@event.TenantId = tenantId;
		return @event;
	  }

	  protected internal virtual HistoricDecisionInstanceEntity createDecisionEvaluatedEvt(DmnDecisionLogicEvaluationEvent evaluationEvent, CaseExecutionEntity execution)
	  {
		// create event instance
		HistoricDecisionInstanceEntity @event = newDecisionInstanceEventEntity(execution, evaluationEvent);
		// initialize event
		initDecisionInstanceEvent(@event, evaluationEvent, HistoryEventTypes.DMN_DECISION_EVALUATE);
		setReferenceToCaseInstance(@event, execution);

		DecisionDefinition decisionDefinition = (DecisionDefinition) evaluationEvent.Decision;
		string tenantId = execution.TenantId;
		if (string.ReferenceEquals(tenantId, null))
		{
		  tenantId = provideTenantId(decisionDefinition, @event);
		}
		@event.TenantId = tenantId;
		return @event;
	  }

	  protected internal virtual HistoricDecisionInstanceEntity createDecisionEvaluatedEvt(DmnDecisionLogicEvaluationEvent evaluationEvent, HistoricDecisionInstanceEntity rootDecisionInstance)
	  {
		// create event instance
		HistoricDecisionInstanceEntity @event = newDecisionInstanceEventEntity(evaluationEvent);
		// initialize event
		initDecisionInstanceEvent(@event, evaluationEvent, HistoryEventTypes.DMN_DECISION_EVALUATE, rootDecisionInstance);

		// set the user id if there is an authenticated user and no process instance
		UserId = @event;

		DecisionDefinition decisionDefinition = (DecisionDefinition) evaluationEvent.Decision;
		string tenantId = decisionDefinition.TenantId;
		if (string.ReferenceEquals(tenantId, null))
		{
		  tenantId = provideTenantId(decisionDefinition, @event);
		}
		@event.TenantId = tenantId;
		return @event;
	  }

	  protected internal virtual HistoricDecisionEvaluationEvent newDecisionEvaluationEvent(DmnDecisionEvaluationEvent evaluationEvent)
	  {
		return new HistoricDecisionEvaluationEvent();
	  }

	  protected internal virtual HistoricDecisionInstanceEntity newDecisionInstanceEventEntity(ExecutionEntity executionEntity, DmnDecisionLogicEvaluationEvent evaluationEvent)
	  {
		return new HistoricDecisionInstanceEntity();
	  }

	  protected internal virtual HistoricDecisionInstanceEntity newDecisionInstanceEventEntity(CaseExecutionEntity executionEntity, DmnDecisionLogicEvaluationEvent evaluationEvent)
	  {
		return new HistoricDecisionInstanceEntity();
	  }

	  protected internal virtual HistoricDecisionInstanceEntity newDecisionInstanceEventEntity(DmnDecisionLogicEvaluationEvent evaluationEvent)
	  {
		return new HistoricDecisionInstanceEntity();
	  }

	  protected internal virtual void initDecisionInstanceEvent(HistoricDecisionInstanceEntity @event, DmnDecisionLogicEvaluationEvent evaluationEvent, HistoryEventTypes eventType)
	  {
		initDecisionInstanceEvent(@event, evaluationEvent, eventType, null);
	  }

	  protected internal virtual void initDecisionInstanceEvent(HistoricDecisionInstanceEntity @event, DmnDecisionLogicEvaluationEvent evaluationEvent, HistoryEventTypes eventType, HistoricDecisionInstanceEntity rootDecisionInstance)
	  {
		@event.EventType = eventType.EventName;

		DecisionDefinition decision = (DecisionDefinition) evaluationEvent.Decision;
		@event.DecisionDefinitionId = decision.Id;
		@event.DecisionDefinitionKey = decision.Key;
		@event.DecisionDefinitionName = decision.Name;

		if (!string.ReferenceEquals(decision.DecisionRequirementsDefinitionId, null))
		{
		  @event.DecisionRequirementsDefinitionId = decision.DecisionRequirementsDefinitionId;
		  @event.DecisionRequirementsDefinitionKey = decision.DecisionRequirementsDefinitionKey;
		}

		// set current time as evaluation time
		@event.EvaluationTime = ClockUtil.CurrentTime;

		if (string.ReferenceEquals(@event.RootProcessInstanceId, null) && string.ReferenceEquals(@event.CaseInstanceId, null))
		{

		  if (rootDecisionInstance != null)
		  {
			@event.RemovalTime = rootDecisionInstance.RemovalTime;
		  }
		  else
		  {
			DateTime removalTime = calculateRemovalTime(@event, decision);
			@event.RemovalTime = removalTime;
		  }
		}

		if (evaluationEvent is DmnDecisionTableEvaluationEvent)
		{
		  initDecisionInstanceEventForDecisionTable(@event, (DmnDecisionTableEvaluationEvent) evaluationEvent);

		}
		else if (evaluationEvent is DmnDecisionLiteralExpressionEvaluationEvent)
		{
		  initDecisionInstanceEventForDecisionLiteralExpression(@event, (DmnDecisionLiteralExpressionEvaluationEvent) evaluationEvent);

		}
		else
		{
		  @event.Inputs = System.Linq.Enumerable.Empty<HistoricDecisionInputInstance> ();
		  @event.Outputs = System.Linq.Enumerable.Empty<HistoricDecisionOutputInstance> ();
		}
	  }

	  protected internal virtual void initDecisionInstanceEventForDecisionTable(HistoricDecisionInstanceEntity @event, DmnDecisionTableEvaluationEvent evaluationEvent)
	  {
		if (evaluationEvent.CollectResultValue != null)
		{
		  double? collectResultValue = getCollectResultValue(evaluationEvent.CollectResultValue);
		  @event.CollectResultValue = collectResultValue;
		}

		IList<HistoricDecisionInputInstance> historicDecisionInputInstances = createHistoricDecisionInputInstances(evaluationEvent, @event.RootProcessInstanceId, @event.RemovalTime);
		@event.Inputs = historicDecisionInputInstances;

		IList<HistoricDecisionOutputInstance> historicDecisionOutputInstances = createHistoricDecisionOutputInstances(evaluationEvent, @event.RootProcessInstanceId, @event.RemovalTime);
		@event.Outputs = historicDecisionOutputInstances;
	  }

	  protected internal virtual double? getCollectResultValue(TypedValue collectResultValue)
	  {
		// the built-in collect aggregators return only numbers
		if (collectResultValue is IntegerValue)
		{
		  return ((IntegerValue) collectResultValue).Value.doubleValue();

		}
		else if (collectResultValue is LongValue)
		{
		  return ((LongValue) collectResultValue).Value.doubleValue();

		}
		else if (collectResultValue is DoubleValue)
		{
		  return ((DoubleValue) collectResultValue).Value;

		}
		else
		{
		  throw LOG.collectResultValueOfUnsupportedTypeException(collectResultValue);
		}
	  }

	  protected internal virtual IList<HistoricDecisionInputInstance> createHistoricDecisionInputInstances(DmnDecisionTableEvaluationEvent evaluationEvent, string rootProcessInstanceId, DateTime removalTime)
	  {
		IList<HistoricDecisionInputInstance> inputInstances = new List<HistoricDecisionInputInstance>();

		foreach (DmnEvaluatedInput inputClause in evaluationEvent.Inputs)
		{

		  HistoricDecisionInputInstanceEntity inputInstance = new HistoricDecisionInputInstanceEntity(rootProcessInstanceId, removalTime);
		  inputInstance.ClauseId = inputClause.Id;
		  inputInstance.ClauseName = inputClause.Name;
		  inputInstance.CreateTime = ClockUtil.CurrentTime;

		  TypedValue typedValue = Variables.untypedValue(inputClause.Value);
		  inputInstance.setValue(typedValue);

		  inputInstances.Add(inputInstance);
		}

		return inputInstances;
	  }

	  protected internal virtual IList<HistoricDecisionOutputInstance> createHistoricDecisionOutputInstances(DmnDecisionTableEvaluationEvent evaluationEvent, string rootProcessInstanceId, DateTime removalTime)
	  {
		IList<HistoricDecisionOutputInstance> outputInstances = new List<HistoricDecisionOutputInstance>();

		IList<DmnEvaluatedDecisionRule> matchingRules = evaluationEvent.MatchingRules;
		for (int index = 0; index < matchingRules.Count; index++)
		{
		  DmnEvaluatedDecisionRule rule = matchingRules[index];

		  string ruleId = rule.Id;
		  int? ruleOrder = index + 1;

		  foreach (DmnEvaluatedOutput outputClause in rule.OutputEntries.values())
		  {

			HistoricDecisionOutputInstanceEntity outputInstance = new HistoricDecisionOutputInstanceEntity(rootProcessInstanceId, removalTime);
			outputInstance.ClauseId = outputClause.Id;
			outputInstance.ClauseName = outputClause.Name;
			outputInstance.CreateTime = ClockUtil.CurrentTime;

			outputInstance.RuleId = ruleId;
			outputInstance.RuleOrder = ruleOrder;

			outputInstance.VariableName = outputClause.OutputName;
			outputInstance.setValue(outputClause.Value);

			outputInstances.Add(outputInstance);
		  }
		}

		return outputInstances;
	  }

	  protected internal virtual void initDecisionInstanceEventForDecisionLiteralExpression(HistoricDecisionInstanceEntity @event, DmnDecisionLiteralExpressionEvaluationEvent evaluationEvent)
	  {
		// no inputs for expression
		@event.Inputs = System.Linq.Enumerable.Empty<HistoricDecisionInputInstance> ();

		HistoricDecisionOutputInstanceEntity outputInstance = new HistoricDecisionOutputInstanceEntity(@event.RootProcessInstanceId, @event.RemovalTime);
		outputInstance.VariableName = evaluationEvent.OutputName;
		outputInstance.setValue(evaluationEvent.OutputValue);

		@event.Outputs = Collections.singletonList<HistoricDecisionOutputInstance> (outputInstance);
	  }

	  protected internal virtual void setReferenceToProcessInstance(HistoricDecisionInstanceEntity @event, ExecutionEntity execution)
	  {
		@event.ProcessDefinitionKey = getProcessDefinitionKey(execution);
		@event.ProcessDefinitionId = execution.ProcessDefinitionId;

		@event.RootProcessInstanceId = execution.RootProcessInstanceId;
		@event.ProcessInstanceId = execution.ProcessInstanceId;
		@event.ExecutionId = execution.Id;

		@event.ActivityId = execution.ActivityId;
		@event.ActivityInstanceId = execution.ActivityInstanceId;
	  }

	  protected internal virtual string getProcessDefinitionKey(ExecutionEntity execution)
	  {
		ProcessDefinitionEntity definition = execution.getProcessDefinition();
		if (definition != null)
		{
		  return definition.Key;
		}
		else
		{
		  return null;
		}
	  }

	  protected internal virtual void setReferenceToCaseInstance(HistoricDecisionInstanceEntity @event, CaseExecutionEntity execution)
	  {
		@event.CaseDefinitionKey = getCaseDefinitionKey(execution);
		@event.CaseDefinitionId = execution.CaseDefinitionId;

		@event.CaseInstanceId = execution.CaseInstanceId;
		@event.ExecutionId = execution.Id;

		@event.ActivityId = execution.ActivityId;
		@event.ActivityInstanceId = execution.Id;
	  }

	  protected internal virtual string getCaseDefinitionKey(CaseExecutionEntity execution)
	  {
		CaseDefinitionEntity definition = (CaseDefinitionEntity) execution.CaseDefinition;
		if (definition != null)
		{
		  return definition.Key;
		}
		else
		{
		  return null;
		}
	  }

	  protected internal virtual HistoricDecisionInstanceEntity UserId
	  {
		  set
		  {
			value.UserId = Context.CommandContext.AuthenticatedUserId;
		  }
	  }

	  protected internal virtual string provideTenantId(DecisionDefinition decisionDefinition, HistoricDecisionInstanceEntity @event)
	  {
		TenantIdProvider tenantIdProvider = Context.ProcessEngineConfiguration.TenantIdProvider;
		string tenantId = null;

		if (tenantIdProvider != null)
		{
		  TenantIdProviderHistoricDecisionInstanceContext ctx = null;

		  if (!string.ReferenceEquals(@event.ExecutionId, null))
		  {
			ctx = new TenantIdProviderHistoricDecisionInstanceContext(decisionDefinition, getExecution(@event));
		  }
		  else if (!string.ReferenceEquals(@event.CaseExecutionId, null))
		  {
			ctx = new TenantIdProviderHistoricDecisionInstanceContext(decisionDefinition, getCaseExecution(@event));
		  }
		  else
		  {
			ctx = new TenantIdProviderHistoricDecisionInstanceContext(decisionDefinition);
		  }

		  tenantId = tenantIdProvider.provideTenantIdForHistoricDecisionInstance(ctx);
		}

		return tenantId;
	  }

	  protected internal virtual DelegateExecution getExecution(HistoricDecisionInstanceEntity @event)
	  {
		return Context.CommandContext.ExecutionManager.findExecutionById(@event.ExecutionId);
	  }

	  protected internal virtual DelegateCaseExecution getCaseExecution(HistoricDecisionInstanceEntity @event)
	  {
		  return Context.CommandContext.CaseExecutionManager.findCaseExecutionById(@event.CaseExecutionId);
	  }

	  protected internal virtual DateTime calculateRemovalTime(HistoricDecisionInstanceEntity historicDecisionInstance, DecisionDefinition decisionDefinition)
	  {
		return Context.ProcessEngineConfiguration.HistoryRemovalTimeProvider.calculateRemovalTime(historicDecisionInstance, decisionDefinition);
	  }

	  protected internal virtual void provideRemovalTime(HistoryEvent historyEvent)
	  {
		string rootProcessInstanceId = historyEvent.RootProcessInstanceId;
		if (!string.ReferenceEquals(rootProcessInstanceId, null))
		{
		  HistoricProcessInstanceEventEntity historicRootProcessInstance = getHistoricRootProcessInstance(rootProcessInstanceId);

		  if (historicRootProcessInstance != null)
		  {
			DateTime removalTime = historicRootProcessInstance.RemovalTime;
			historyEvent.RemovalTime = removalTime;
		  }
		}
	  }

	  protected internal virtual bool HistoryRemovalTimeStrategyStart
	  {
		  get
		  {
			return HISTORY_REMOVAL_TIME_STRATEGY_START.Equals(HistoryRemovalTimeStrategy);
		  }
	  }

	  protected internal virtual string HistoryRemovalTimeStrategy
	  {
		  get
		  {
			return Context.ProcessEngineConfiguration.HistoryRemovalTimeStrategy;
		  }
	  }

	  protected internal virtual HistoricProcessInstanceEventEntity getHistoricRootProcessInstance(string rootProcessInstanceId)
	  {
		return Context.CommandContext.DbEntityManager.selectById(typeof(HistoricProcessInstanceEventEntity), rootProcessInstanceId);
	  }

	}

}