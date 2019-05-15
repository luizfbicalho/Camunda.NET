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
namespace org.camunda.bpm.engine.impl.history.parser
{
	using DmnDecision = org.camunda.bpm.dmn.engine.DmnDecision;
	using DmnDecisionEvaluationEvent = org.camunda.bpm.dmn.engine.@delegate.DmnDecisionEvaluationEvent;
	using DmnDecisionEvaluationListener = org.camunda.bpm.dmn.engine.@delegate.DmnDecisionEvaluationListener;
	using CaseExecutionEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CoreExecutionContext = org.camunda.bpm.engine.impl.context.CoreExecutionContext;
	using CoreExecution = org.camunda.bpm.engine.impl.core.instance.CoreExecution;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;
	using DmnHistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.DmnHistoryEventProducer;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;

	public class HistoryDecisionEvaluationListener : DmnDecisionEvaluationListener
	{

	  protected internal DmnHistoryEventProducer eventProducer;
	  protected internal HistoryLevel historyLevel;

	  public HistoryDecisionEvaluationListener(DmnHistoryEventProducer historyEventProducer, HistoryLevel historyLevel)
	  {
		this.eventProducer = historyEventProducer;
		this.historyLevel = historyLevel;
	  }

	  public virtual void notify(DmnDecisionEvaluationEvent evaluationEvent)
	  {
	   HistoryEvent historyEvent = createHistoryEvent(evaluationEvent);

		if (historyEvent != null)
		{
		  Context.ProcessEngineConfiguration.HistoryEventHandler.handleEvent(historyEvent);
		}
	  }

	  protected internal virtual HistoryEvent createHistoryEvent(DmnDecisionEvaluationEvent evaluationEvent)
	  {
		DmnDecision decisionTable = evaluationEvent.DecisionResult.Decision;
		if (isDeployedDecisionTable(decisionTable) && historyLevel.isHistoryEventProduced(HistoryEventTypes.DMN_DECISION_EVALUATE, decisionTable))
		{

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.bpm.engine.impl.context.CoreExecutionContext<? extends org.camunda.bpm.engine.impl.core.instance.CoreExecution> executionContext = org.camunda.bpm.engine.impl.context.Context.getCoreExecutionContext();
		  CoreExecutionContext<CoreExecution> executionContext = Context.CoreExecutionContext;
		  if (executionContext != null)
		  {
			CoreExecution coreExecution = executionContext.Execution;

			if (coreExecution is ExecutionEntity)
			{
			  ExecutionEntity execution = (ExecutionEntity) coreExecution;
			  return eventProducer.createDecisionEvaluatedEvt(execution, evaluationEvent);
			}
			else if (coreExecution is CaseExecutionEntity)
			{
			  CaseExecutionEntity caseExecution = (CaseExecutionEntity) coreExecution;
			  return eventProducer.createDecisionEvaluatedEvt(caseExecution, evaluationEvent);
			}

		  }

		  return eventProducer.createDecisionEvaluatedEvt(evaluationEvent);

		}
		else
		{
		  return null;
		}
	  }

	  protected internal virtual bool isDeployedDecisionTable(DmnDecision decision)
	  {
		if (decision is DecisionDefinition)
		{
		  return !string.ReferenceEquals(((DecisionDefinition) decision).Id, null);
		}
		else
		{
		  return false;
		}
	  }

	}

}