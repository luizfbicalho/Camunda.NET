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
namespace org.camunda.bpm.engine.impl.history.transformer
{

	using CaseExecutionListener = org.camunda.bpm.engine.@delegate.CaseExecutionListener;
	using ItemHandler = org.camunda.bpm.engine.impl.cmmn.handler.ItemHandler;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using CmmnCaseDefinition = org.camunda.bpm.engine.impl.cmmn.model.CmmnCaseDefinition;
	using CmmnSentryDeclaration = org.camunda.bpm.engine.impl.cmmn.model.CmmnSentryDeclaration;
	using CmmnTransformListener = org.camunda.bpm.engine.impl.cmmn.transformer.CmmnTransformListener;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;
	using CmmnHistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.CmmnHistoryEventProducer;
	using Case = org.camunda.bpm.model.cmmn.instance.Case;
	using CasePlanModel = org.camunda.bpm.model.cmmn.instance.CasePlanModel;
	using CaseTask = org.camunda.bpm.model.cmmn.instance.CaseTask;
	using DecisionTask = org.camunda.bpm.model.cmmn.instance.DecisionTask;
	using Definitions = org.camunda.bpm.model.cmmn.instance.Definitions;
	using EventListener = org.camunda.bpm.model.cmmn.instance.EventListener;
	using HumanTask = org.camunda.bpm.model.cmmn.instance.HumanTask;
	using Milestone = org.camunda.bpm.model.cmmn.instance.Milestone;
	using PlanItem = org.camunda.bpm.model.cmmn.instance.PlanItem;
	using ProcessTask = org.camunda.bpm.model.cmmn.instance.ProcessTask;
	using Sentry = org.camunda.bpm.model.cmmn.instance.Sentry;
	using Stage = org.camunda.bpm.model.cmmn.instance.Stage;
	using Task = org.camunda.bpm.model.cmmn.instance.Task;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class CmmnHistoryTransformListener : CmmnTransformListener
	{

	  // Cached listeners
	  // listeners can be reused for a given process engine instance but cannot be cached in static fields since
	  // different process engine instances on the same Classloader may have different HistoryEventProducer
	  // configurations wired
	  protected internal CaseExecutionListener CASE_INSTANCE_CREATE_LISTENER;
	  protected internal CaseExecutionListener CASE_INSTANCE_UPDATE_LISTENER;
	  protected internal CaseExecutionListener CASE_INSTANCE_CLOSE_LISTENER;

	  protected internal CaseExecutionListener CASE_ACTIVITY_INSTANCE_CREATE_LISTENER;
	  protected internal CaseExecutionListener CASE_ACTIVITY_INSTANCE_UPDATE_LISTENER;
	  protected internal CaseExecutionListener CASE_ACTIVITY_INSTANCE_END_LISTENER;

	  // The history level set in the process engine configuration
	  protected internal HistoryLevel historyLevel;

	  public CmmnHistoryTransformListener(HistoryLevel historyLevel, CmmnHistoryEventProducer historyEventProducer)
	  {
		this.historyLevel = historyLevel;
		initCaseExecutionListeners(historyEventProducer, historyLevel);
	  }

	  protected internal virtual void initCaseExecutionListeners(CmmnHistoryEventProducer historyEventProducer, HistoryLevel historyLevel)
	  {
		CASE_INSTANCE_CREATE_LISTENER = new CaseInstanceCreateListener(historyEventProducer, historyLevel);
		CASE_INSTANCE_UPDATE_LISTENER = new CaseInstanceUpdateListener(historyEventProducer, historyLevel);
		CASE_INSTANCE_CLOSE_LISTENER = new CaseInstanceCloseListener(historyEventProducer, historyLevel);

		CASE_ACTIVITY_INSTANCE_CREATE_LISTENER = new CaseActivityInstanceCreateListener(historyEventProducer, historyLevel);
		CASE_ACTIVITY_INSTANCE_UPDATE_LISTENER = new CaseActivityInstanceUpdateListener(historyEventProducer, historyLevel);
		CASE_ACTIVITY_INSTANCE_END_LISTENER = new CaseActivityInstanceEndListener(historyEventProducer, historyLevel);
	  }

	  public virtual void transformRootElement<T1>(Definitions definitions, IList<T1> caseDefinitions) where T1 : org.camunda.bpm.engine.impl.cmmn.model.CmmnCaseDefinition
	  {
	  }

	  public virtual void transformCase(Case element, CmmnCaseDefinition caseDefinition)
	  {
	  }

	  public virtual void transformCasePlanModel(org.camunda.bpm.model.cmmn.impl.instance.CasePlanModel casePlanModel, CmmnActivity caseActivity)
	  {
		transformCasePlanModel((CasePlanModel) casePlanModel, caseActivity);
	  }

	  public virtual void transformCasePlanModel(CasePlanModel casePlanModel, CmmnActivity caseActivity)
	  {
		addCasePlanModelHandlers(caseActivity);
	  }

	  public virtual void transformHumanTask(PlanItem planItem, HumanTask humanTask, CmmnActivity caseActivity)
	  {
		addTaskOrStageHandlers(caseActivity);
	  }

	  public virtual void transformProcessTask(PlanItem planItem, ProcessTask processTask, CmmnActivity caseActivity)
	  {
		addTaskOrStageHandlers(caseActivity);
	  }

	  public virtual void transformCaseTask(PlanItem planItem, CaseTask caseTask, CmmnActivity caseActivity)
	  {
		addTaskOrStageHandlers(caseActivity);
	  }

	  public virtual void transformDecisionTask(PlanItem planItem, DecisionTask decisionTask, CmmnActivity caseActivity)
	  {
		addTaskOrStageHandlers(caseActivity);
	  }

	  public virtual void transformTask(PlanItem planItem, Task task, CmmnActivity caseActivity)
	  {
		addTaskOrStageHandlers(caseActivity);
	  }

	  public virtual void transformStage(PlanItem planItem, Stage stage, CmmnActivity caseActivity)
	  {
		addTaskOrStageHandlers(caseActivity);
	  }

	  public virtual void transformMilestone(PlanItem planItem, Milestone milestone, CmmnActivity caseActivity)
	  {
		addEventListenerOrMilestoneHandlers(caseActivity);
	  }

	  public virtual void transformEventListener(PlanItem planItem, EventListener eventListener, CmmnActivity caseActivity)
	  {
		addEventListenerOrMilestoneHandlers(caseActivity);
	  }

	  public virtual void transformSentry(Sentry sentry, CmmnSentryDeclaration sentryDeclaration)
	  {
	  }

	  protected internal virtual void addCasePlanModelHandlers(CmmnActivity caseActivity)
	  {
		if (caseActivity != null)
		{
		  if (historyLevel.isHistoryEventProduced(HistoryEventTypes.CASE_INSTANCE_CREATE, null))
		  {
			foreach (string @event in ItemHandler.CASE_PLAN_MODEL_CREATE_EVENTS)
			{
			  caseActivity.addBuiltInListener(@event, CASE_INSTANCE_CREATE_LISTENER);
			}
		  }
		  if (historyLevel.isHistoryEventProduced(HistoryEventTypes.CASE_INSTANCE_UPDATE, null))
		  {
			foreach (string @event in ItemHandler.CASE_PLAN_MODEL_UPDATE_EVENTS)
			{
			  caseActivity.addBuiltInListener(@event, CASE_INSTANCE_UPDATE_LISTENER);
			}
		  }
		  if (historyLevel.isHistoryEventProduced(HistoryEventTypes.CASE_INSTANCE_CLOSE, null))
		  {
			foreach (string @event in ItemHandler.CASE_PLAN_MODEL_CLOSE_EVENTS)
			{
			  caseActivity.addBuiltInListener(@event, CASE_INSTANCE_CLOSE_LISTENER);
			}
		  }
		}
	  }

	  protected internal virtual void addTaskOrStageHandlers(CmmnActivity caseActivity)
	  {
		if (caseActivity != null)
		{
		  if (historyLevel.isHistoryEventProduced(HistoryEventTypes.CASE_ACTIVITY_INSTANCE_CREATE, null))
		  {
			foreach (string @event in ItemHandler.TASK_OR_STAGE_CREATE_EVENTS)
			{
			  caseActivity.addBuiltInListener(@event, CASE_ACTIVITY_INSTANCE_CREATE_LISTENER);
			}
		  }
		  if (historyLevel.isHistoryEventProduced(HistoryEventTypes.CASE_ACTIVITY_INSTANCE_UPDATE, null))
		  {
			foreach (string @event in ItemHandler.TASK_OR_STAGE_UPDATE_EVENTS)
			{
			  caseActivity.addBuiltInListener(@event, CASE_ACTIVITY_INSTANCE_UPDATE_LISTENER);
			}
		  }
		  if (historyLevel.isHistoryEventProduced(HistoryEventTypes.CASE_ACTIVITY_INSTANCE_END, null))
		  {
			foreach (string @event in ItemHandler.TASK_OR_STAGE_END_EVENTS)
			{
			  caseActivity.addBuiltInListener(@event, CASE_ACTIVITY_INSTANCE_END_LISTENER);
			}
		  }
		}
	  }

	  protected internal virtual void addEventListenerOrMilestoneHandlers(CmmnActivity caseActivity)
	  {
		if (caseActivity != null)
		{
		  if (historyLevel.isHistoryEventProduced(HistoryEventTypes.CASE_ACTIVITY_INSTANCE_CREATE, null))
		  {
			foreach (string @event in ItemHandler.EVENT_LISTENER_OR_MILESTONE_CREATE_EVENTS)
			{
			  caseActivity.addBuiltInListener(@event, CASE_ACTIVITY_INSTANCE_CREATE_LISTENER);
			}
		  }
		  if (historyLevel.isHistoryEventProduced(HistoryEventTypes.CASE_ACTIVITY_INSTANCE_UPDATE, null))
		  {
			foreach (string @event in ItemHandler.EVENT_LISTENER_OR_MILESTONE_UPDATE_EVENTS)
			{
			  caseActivity.addBuiltInListener(@event, CASE_ACTIVITY_INSTANCE_UPDATE_LISTENER);
			}
		  }
		  if (historyLevel.isHistoryEventProduced(HistoryEventTypes.CASE_ACTIVITY_INSTANCE_END, null))
		  {
			foreach (string @event in ItemHandler.EVENT_LISTENER_OR_MILESTONE_END_EVENTS)
			{
			  caseActivity.addBuiltInListener(@event, CASE_ACTIVITY_INSTANCE_END_LISTENER);
			}
		  }
		}
	  }

	}

}