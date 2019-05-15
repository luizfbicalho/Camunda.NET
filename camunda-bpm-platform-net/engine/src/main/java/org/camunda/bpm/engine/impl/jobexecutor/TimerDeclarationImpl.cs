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
namespace org.camunda.bpm.engine.impl.jobexecutor
{

	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using BpmnProperties = org.camunda.bpm.engine.impl.bpmn.helper.BpmnProperties;
	using BusinessCalendar = org.camunda.bpm.engine.impl.calendar.BusinessCalendar;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using StartProcessVariableScope = org.camunda.bpm.engine.impl.el.StartProcessVariableScope;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using TimerEntity = org.camunda.bpm.engine.impl.persistence.entity.TimerEntity;
	using PvmScope = org.camunda.bpm.engine.impl.pvm.PvmScope;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;

	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// </summary>
	[Serializable]
	public class TimerDeclarationImpl : JobDeclaration<ExecutionEntity, TimerEntity>
	{

	  private const long serialVersionUID = 1L;

	  protected internal Expression description;
	  protected internal TimerDeclarationType type;

	  protected internal string repeat;
	  protected internal bool isInterruptingTimer; // For boundary timers
	  protected internal string eventScopeActivityId = null;
	  protected internal bool? isParallelMultiInstance;

	  protected internal string rawJobHandlerConfiguration;

	  public TimerDeclarationImpl(Expression expression, TimerDeclarationType type, string jobHandlerType) : base(jobHandlerType)
	  {
		this.description = expression;
		this.type = type;
	  }

	  public virtual bool InterruptingTimer
	  {
		  get
		  {
			return isInterruptingTimer;
		  }
		  set
		  {
			this.isInterruptingTimer = value;
		  }
	  }


	  public virtual string Repeat
	  {
		  get
		  {
			return repeat;
		  }
	  }

	  public virtual string EventScopeActivityId
	  {
		  set
		  {
			this.eventScopeActivityId = value;
		  }
		  get
		  {
			return eventScopeActivityId;
		  }
	  }


	  protected internal virtual TimerEntity newJobInstance(ExecutionEntity execution)
	  {

		TimerEntity timer = new TimerEntity(this);
		if (execution != null)
		{
		  timer.Execution = execution;
		}

		return timer;
	  }

	  public virtual string RawJobHandlerConfiguration
	  {
		  set
		  {
			this.rawJobHandlerConfiguration = value;
		  }
	  }

	  public virtual void updateJob(TimerEntity timer)
	  {
		initializeConfiguration(timer.Execution, timer);
	  }

	  protected internal virtual void initializeConfiguration(ExecutionEntity context, TimerEntity job)
	  {
		string dueDateString = resolveAndSetDuedate(context, job, false);

		if (type == TimerDeclarationType.CYCLE && !string.ReferenceEquals(jobHandlerType, TimerCatchIntermediateEventJobHandler.TYPE))
		{

		  // See ACT-1427: A boundary timer with a cancelActivity='true', doesn't need to repeat itself
		  if (!isInterruptingTimer)
		  {
			string prepared = prepareRepeat(dueDateString);
			job.Repeat = prepared;
		  }
		}
	  }

	  public virtual string resolveAndSetDuedate(ExecutionEntity context, TimerEntity job, bool creationDateBased)
	  {
		BusinessCalendar businessCalendar = Context.ProcessEngineConfiguration.BusinessCalendarManager.getBusinessCalendar(type.calendarName);

		if (description == null)
		{
		  throw new ProcessEngineException("Timer '" + context.ActivityId + "' was not configured with a valid duration/time");
		}

		string dueDateString = null;
		DateTime duedate = null;

		// ACT-1415: timer-declaration on start-event may contain expressions NOT
		// evaluating variables but other context, evaluating should happen nevertheless
		VariableScope scopeForExpression = context;
		if (scopeForExpression == null)
		{
		  scopeForExpression = StartProcessVariableScope.SharedInstance;
		}

		object dueDateValue = description.getValue(scopeForExpression);
		if (dueDateValue is string)
		{
		  dueDateString = (string)dueDateValue;
		}
		else if (dueDateValue is DateTime)
		{
		  duedate = (DateTime)dueDateValue;
		}
		else
		{
		  throw new ProcessEngineException("Timer '" + context.ActivityId + "' was not configured with a valid duration/time, either hand in a java.util.Date or a String in format 'yyyy-MM-dd'T'hh:mm:ss'");
		}

		if (duedate == null)
		{
		  if (creationDateBased)
		  {
			if (job.CreateTime == null)
			{
			  throw new ProcessEngineException("Timer '" + context.ActivityId + "' has no creation time and cannot be recalculated based on creation date. Either recalculate on your own or trigger recalculation with creationDateBased set to false.");
			}
			duedate = businessCalendar.resolveDuedate(dueDateString, job.CreateTime);
		  }
		  else
		  {
			duedate = businessCalendar.resolveDuedate(dueDateString);
		  }
		}

		job.Duedate = duedate;
		return dueDateString;
	  }

	  protected internal virtual void postInitialize(ExecutionEntity execution, TimerEntity timer)
	  {
		initializeConfiguration(execution, timer);
	  }

	  protected internal virtual string prepareRepeat(string dueDate)
	  {
		if (dueDate.StartsWith("R", StringComparison.Ordinal) && dueDate.Split("/", true).length == 2)
		{
		  SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
		  return dueDate.Replace("/","/" + sdf.format(ClockUtil.CurrentTime) + "/");
		}
		return dueDate;
	  }

	  public virtual TimerEntity createTimerInstance(ExecutionEntity execution)
	  {
		return createTimer(execution);
	  }

	  public virtual TimerEntity createStartTimerInstance(string deploymentId)
	  {
		return createTimer(deploymentId);
	  }

	  public virtual TimerEntity createTimer(string deploymentId)
	  {
		TimerEntity timer = base.createJobInstance((ExecutionEntity) null);
		timer.DeploymentId = deploymentId;
		scheduleTimer(timer);
		return timer;
	  }

	  public virtual TimerEntity createTimer(ExecutionEntity execution)
	  {
		TimerEntity timer = base.createJobInstance(execution);
		scheduleTimer(timer);
		return timer;
	  }

	  protected internal virtual void scheduleTimer(TimerEntity timer)
	  {
		Context.CommandContext.JobManager.schedule(timer);
	  }

	  protected internal virtual ExecutionEntity resolveExecution(ExecutionEntity context)
	  {
		return context;
	  }

	  protected internal override JobHandlerConfiguration resolveJobHandlerConfiguration(ExecutionEntity context)
	  {
		return resolveJobHandler().newConfiguration(rawJobHandlerConfiguration);
	  }

	  public static IDictionary<string, TimerDeclarationImpl> getDeclarationsForScope(PvmScope scope)
	  {
		if (scope == null)
		{
		  return Collections.emptyMap();
		}

		IDictionary<string, TimerDeclarationImpl> result = scope.Properties.get(BpmnProperties.TIMER_DECLARATIONS);
		if (result != null)
		{
		  return result;
		}
		else
		{
		  return Collections.emptyMap();
		}
	  }

	}

}