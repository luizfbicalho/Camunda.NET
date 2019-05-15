using System;
using System.Collections;
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
namespace org.camunda.bpm.engine.impl.persistence.entity
{

	using BusinessCalendar = org.camunda.bpm.engine.impl.calendar.BusinessCalendar;
	using CycleBusinessCalendar = org.camunda.bpm.engine.impl.calendar.CycleBusinessCalendar;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using TransactionState = org.camunda.bpm.engine.impl.cfg.TransactionState;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using RepeatingFailedJobListener = org.camunda.bpm.engine.impl.jobexecutor.RepeatingFailedJobListener;
	using TimerDeclarationImpl = org.camunda.bpm.engine.impl.jobexecutor.TimerDeclarationImpl;
	using TimerEventJobHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerEventJobHandler;
	using TimerJobConfiguration = org.camunda.bpm.engine.impl.jobexecutor.TimerEventJobHandler.TimerJobConfiguration;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class TimerEntity : JobEntity
	{

	  public const string TYPE = "timer";

	  private const long serialVersionUID = 1L;

	  protected internal string repeat;

	  public TimerEntity()
	  {
	  }

	  public TimerEntity(TimerDeclarationImpl timerDeclaration)
	  {
		repeat = timerDeclaration.Repeat;
	  }

	  protected internal TimerEntity(TimerEntity te)
	  {
		jobHandlerConfiguration = te.jobHandlerConfiguration;
		jobHandlerType = te.jobHandlerType;
		isExclusive = te.isExclusive;
		repeat = te.repeat;
		retries = te.retries;
		executionId = te.executionId;
		processInstanceId = te.processInstanceId;
		jobDefinitionId = te.jobDefinitionId;
		suspensionState = te.suspensionState;
		deploymentId = te.deploymentId;
		processDefinitionId = te.processDefinitionId;
		processDefinitionKey = te.processDefinitionKey;
		tenantId = te.tenantId;
		priority = te.priority;
	  }

	  protected internal override void preExecute(CommandContext commandContext)
	  {
		if (JobHandler is TimerEventJobHandler)
		{
		  TimerEventJobHandler.TimerJobConfiguration configuration = (TimerEventJobHandler.TimerJobConfiguration) JobHandlerConfiguration;
		  if (!string.ReferenceEquals(repeat, null) && !configuration.FollowUpJobCreated)
		  {
			// this timer is a repeating timer and
			// a follow up timer job has not been scheduled yet

			DateTime newDueDate = calculateRepeat();

			if (newDueDate != null)
			{
			  // the listener is added to the transaction as SYNC on ROLLABCK,
			  // when it is necessary to schedule a new timer job invocation.
			  // If the transaction does not rollback, it is ignored.
			  ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
			  CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequiresNew;
			  RepeatingFailedJobListener listener = createRepeatingFailedJobListener(commandExecutor);

			  commandContext.TransactionContext.addTransactionListener(TransactionState.ROLLED_BACK, listener);

			  // create a new timer job
			  createNewTimerJob(newDueDate);
			}
		  }
		}
	  }

	  protected internal virtual RepeatingFailedJobListener createRepeatingFailedJobListener(CommandExecutor commandExecutor)
	  {
		return new RepeatingFailedJobListener(commandExecutor, Id);
	  }

	  public virtual void createNewTimerJob(DateTime dueDate)
	  {
		// create new timer job
		TimerEntity newTimer = new TimerEntity(this);
		newTimer.Duedate = dueDate;
		Context.CommandContext.JobManager.schedule(newTimer);
	  }

	  public virtual DateTime calculateRepeat()
	  {
		BusinessCalendar businessCalendar = Context.ProcessEngineConfiguration.BusinessCalendarManager.getBusinessCalendar(CycleBusinessCalendar.NAME);
		return businessCalendar.resolveDuedate(repeat);
	  }

	  public virtual string Repeat
	  {
		  get
		  {
			return repeat;
		  }
		  set
		  {
			this.repeat = value;
		  }
	  }


	  public override string Type
	  {
		  get
		  {
			return TYPE;
		  }
	  }

	  public override object PersistentState
	  {
		  get
		  {
			IDictionary<string, object> persistentState = (Hashtable) base.PersistentState;
			persistentState["repeat"] = repeat;
    
			return persistentState;
		  }
	  }

	  public override string ToString()
	  {
		return this.GetType().Name + "[repeat=" + repeat + ", id=" + id + ", revision=" + revision + ", duedate=" + duedate + ", lockOwner=" + lockOwner + ", lockExpirationTime=" + lockExpirationTime + ", executionId=" + executionId + ", processInstanceId=" + processInstanceId + ", isExclusive=" + isExclusive + ", retries=" + retries + ", jobHandlerType=" + jobHandlerType + ", jobHandlerConfiguration=" + jobHandlerConfiguration + ", exceptionByteArray=" + exceptionByteArray + ", exceptionByteArrayId=" + exceptionByteArrayId + ", exceptionMessage=" + exceptionMessage + ", deploymentId=" + deploymentId + "]";
	  }

	}

}