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
	using DelegateCaseExecution = org.camunda.bpm.engine.@delegate.DelegateCaseExecution;
	using CaseExecutionEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity;
	using CmmnExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnExecution;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using HistoricCaseActivityInstanceEventEntity = org.camunda.bpm.engine.impl.history.@event.HistoricCaseActivityInstanceEventEntity;
	using HistoricCaseInstanceEventEntity = org.camunda.bpm.engine.impl.history.@event.HistoricCaseInstanceEventEntity;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class DefaultCmmnHistoryEventProducer : CmmnHistoryEventProducer
	{

	  public virtual HistoryEvent createCaseInstanceCreateEvt(DelegateCaseExecution caseExecution)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity caseExecutionEntity = (org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity) caseExecution;
		CaseExecutionEntity caseExecutionEntity = (CaseExecutionEntity) caseExecution;

		// create event instance
		HistoricCaseInstanceEventEntity evt = newCaseInstanceEventEntity(caseExecutionEntity);

		// initialize event
		initCaseInstanceEvent(evt, caseExecutionEntity, HistoryEventTypes.CASE_INSTANCE_CREATE);

		// set create time
		evt.CreateTime = ClockUtil.CurrentTime;

		// set create user id
		evt.CreateUserId = Context.CommandContext.AuthenticatedUserId;

		// set super case instance id
		CmmnExecution superCaseExecution = caseExecutionEntity.SuperCaseExecution;
		if (superCaseExecution != null)
		{
		  evt.SuperCaseInstanceId = superCaseExecution.CaseInstanceId;
		}

		// set super process instance id
		ExecutionEntity superExecution = caseExecutionEntity.getSuperExecution();
		if (superExecution != null)
		{
		  evt.SuperProcessInstanceId = superExecution.ProcessInstanceId;
		}

		return evt;
	  }

	  public virtual HistoryEvent createCaseInstanceUpdateEvt(DelegateCaseExecution caseExecution)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity caseExecutionEntity = (org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity) caseExecution;
		CaseExecutionEntity caseExecutionEntity = (CaseExecutionEntity) caseExecution;

		// create event instance
		HistoricCaseInstanceEventEntity evt = loadCaseInstanceEventEntity(caseExecutionEntity);

		// initialize event
		initCaseInstanceEvent(evt, caseExecutionEntity, HistoryEventTypes.CASE_INSTANCE_UPDATE);

		return evt;
	  }

	  public virtual HistoryEvent createCaseInstanceCloseEvt(DelegateCaseExecution caseExecution)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity caseExecutionEntity = (org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity) caseExecution;
		CaseExecutionEntity caseExecutionEntity = (CaseExecutionEntity) caseExecution;

		// create event instance
		HistoricCaseInstanceEventEntity evt = loadCaseInstanceEventEntity(caseExecutionEntity);

		// initialize event
		initCaseInstanceEvent(evt, caseExecutionEntity, HistoryEventTypes.CASE_INSTANCE_CLOSE);

		// set end time
		evt.EndTime = ClockUtil.CurrentTime;

		if (evt.StartTime != null)
		{
		  evt.DurationInMillis = evt.EndTime.Ticks - evt.StartTime.Ticks;
		}

		return evt;
	  }

	  public virtual HistoryEvent createCaseActivityInstanceCreateEvt(DelegateCaseExecution caseExecution)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity caseExecutionEntity = (org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity) caseExecution;
		CaseExecutionEntity caseExecutionEntity = (CaseExecutionEntity) caseExecution;

		// create event instance
		HistoricCaseActivityInstanceEventEntity evt = newCaseActivityInstanceEventEntity(caseExecutionEntity);

		// initialize event
		initCaseActivityInstanceEvent(evt, caseExecutionEntity, HistoryEventTypes.CASE_ACTIVITY_INSTANCE_CREATE);

		// set start time
		evt.CreateTime = ClockUtil.CurrentTime;

		return evt;
	  }

	  public virtual HistoryEvent createCaseActivityInstanceUpdateEvt(DelegateCaseExecution caseExecution)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity caseExecutionEntity = (org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity) caseExecution;
		CaseExecutionEntity caseExecutionEntity = (CaseExecutionEntity) caseExecution;

		// create event instance
		HistoricCaseActivityInstanceEventEntity evt = loadCaseActivityInstanceEventEntity(caseExecutionEntity);

		// initialize event
		initCaseActivityInstanceEvent(evt, caseExecutionEntity, HistoryEventTypes.CASE_ACTIVITY_INSTANCE_UPDATE);

		if (caseExecutionEntity.Task != null)
		{
		  evt.TaskId = caseExecutionEntity.Task.Id;
		}

		if (caseExecutionEntity.getSubProcessInstance() != null)
		{
		  evt.CalledProcessInstanceId = caseExecutionEntity.getSubProcessInstance().Id;
		}

		if (caseExecutionEntity.getSubCaseInstance() != null)
		{
		  evt.CalledCaseInstanceId = caseExecutionEntity.getSubCaseInstance().Id;
		}

		return evt;
	  }

	  public virtual HistoryEvent createCaseActivityInstanceEndEvt(DelegateCaseExecution caseExecution)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity caseExecutionEntity = (org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity) caseExecution;
		CaseExecutionEntity caseExecutionEntity = (CaseExecutionEntity) caseExecution;

		// create event instance
		HistoricCaseActivityInstanceEventEntity evt = loadCaseActivityInstanceEventEntity(caseExecutionEntity);

		// initialize event
		initCaseActivityInstanceEvent(evt, caseExecutionEntity, HistoryEventTypes.CASE_ACTIVITY_INSTANCE_END);

		// set end time
		evt.EndTime = ClockUtil.CurrentTime;

		// calculate duration
		if (evt.StartTime != null)
		{
		  evt.DurationInMillis = evt.EndTime.Ticks - evt.StartTime.Ticks;
		}

		return evt;
	  }

	  protected internal virtual HistoricCaseInstanceEventEntity newCaseInstanceEventEntity(CaseExecutionEntity caseExecutionEntity)
	  {
		return new HistoricCaseInstanceEventEntity();
	  }

	  protected internal virtual HistoricCaseInstanceEventEntity loadCaseInstanceEventEntity(CaseExecutionEntity caseExecutionEntity)
	  {
		return newCaseInstanceEventEntity(caseExecutionEntity);
	  }

	  protected internal virtual void initCaseInstanceEvent(HistoricCaseInstanceEventEntity evt, CaseExecutionEntity caseExecutionEntity, HistoryEventTypes eventType)
	  {
		evt.Id = caseExecutionEntity.CaseInstanceId;
		evt.EventType = eventType.EventName;
		evt.CaseDefinitionId = caseExecutionEntity.CaseDefinitionId;
		evt.CaseInstanceId = caseExecutionEntity.CaseInstanceId;
		evt.CaseExecutionId = caseExecutionEntity.Id;
		evt.BusinessKey = caseExecutionEntity.BusinessKey;
		evt.State = caseExecutionEntity.State;
		evt.TenantId = caseExecutionEntity.TenantId;
	  }

	  protected internal virtual HistoricCaseActivityInstanceEventEntity newCaseActivityInstanceEventEntity(CaseExecutionEntity caseExecutionEntity)
	  {
		return new HistoricCaseActivityInstanceEventEntity();
	  }

	  protected internal virtual HistoricCaseActivityInstanceEventEntity loadCaseActivityInstanceEventEntity(CaseExecutionEntity caseExecutionEntity)
	  {
		return newCaseActivityInstanceEventEntity(caseExecutionEntity);
	  }

	  protected internal virtual void initCaseActivityInstanceEvent(HistoricCaseActivityInstanceEventEntity evt, CaseExecutionEntity caseExecutionEntity, HistoryEventTypes eventType)
	  {
		evt.Id = caseExecutionEntity.Id;
		evt.ParentCaseActivityInstanceId = caseExecutionEntity.ParentId;
		evt.EventType = eventType.EventName;
		evt.CaseDefinitionId = caseExecutionEntity.CaseDefinitionId;
		evt.CaseInstanceId = caseExecutionEntity.CaseInstanceId;
		evt.CaseExecutionId = caseExecutionEntity.Id;
		evt.CaseActivityInstanceState = caseExecutionEntity.State;

		evt.Required = caseExecutionEntity.Required;

		evt.CaseActivityId = caseExecutionEntity.ActivityId;
		evt.CaseActivityName = caseExecutionEntity.ActivityName;
		evt.CaseActivityType = caseExecutionEntity.ActivityType;

		evt.TenantId = caseExecutionEntity.TenantId;
	  }

	}

}