using System;
using System.Collections.Generic;
using System.Threading;

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
namespace org.camunda.bpm.engine.impl.cmd
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotEmpty;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using BpmnProperties = org.camunda.bpm.engine.impl.bpmn.helper.BpmnProperties;
	using BpmnParse = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using ProcessApplicationContextUtil = org.camunda.bpm.engine.impl.context.ProcessApplicationContextUtil;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using TimerCatchIntermediateEventJobHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerCatchIntermediateEventJobHandler;
	using TimerDeclarationImpl = org.camunda.bpm.engine.impl.jobexecutor.TimerDeclarationImpl;
	using TimerExecuteNestedActivityJobHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerExecuteNestedActivityJobHandler;
	using TimerStartEventJobHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerStartEventJobHandler;
	using TimerStartEventSubprocessJobHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerStartEventSubprocessJobHandler;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using TimerEntity = org.camunda.bpm.engine.impl.persistence.entity.TimerEntity;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;


	/// <summary>
	/// @author Tobias Metzke
	/// </summary>
	[Serializable]
	public class RecalculateJobDuedateCmd : Command<Void>
	{

	  private const long serialVersionUID = 1L;

	  private readonly string jobId;
	  private bool creationDateBased;

	  public RecalculateJobDuedateCmd(string jobId, bool creationDateBased)
	  {
		ensureNotEmpty("The job id is mandatory", "jobId", jobId);
		this.jobId = jobId;
		this.creationDateBased = creationDateBased;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public Void execute(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  public virtual Void execute(CommandContext commandContext)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.JobEntity job = commandContext.getJobManager().findJobById(jobId);
		JobEntity job = commandContext.JobManager.findJobById(jobId);
		ensureNotNull(typeof(NotFoundException), "No job found with id '" + jobId + "'", "job", job);

		// allow timer jobs only
		checkJobType(job);

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkUpdateJob(job);
		}

		// prepare recalculation
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.jobexecutor.TimerDeclarationImpl timerDeclaration = findTimerDeclaration(commandContext, job);
		TimerDeclarationImpl timerDeclaration = findTimerDeclaration(commandContext, job);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.TimerEntity timer = (org.camunda.bpm.engine.impl.persistence.entity.TimerEntity) job;
		TimerEntity timer = (TimerEntity) job;
		DateTime oldDuedate = job.Duedate;
		ThreadStart runnable = () =>
		{
	timerDeclaration.resolveAndSetDuedate(timer.Execution, timer, creationDateBased);
		};

		// run recalculation in correct context
		ProcessDefinitionEntity contextDefinition = commandContext.ProcessEngineConfiguration.DeploymentCache.findDeployedProcessDefinitionById(job.ProcessDefinitionId);
		ProcessApplicationContextUtil.doContextSwitch(runnable, contextDefinition);

		// log operation
		IList<PropertyChange> propertyChanges = new List<PropertyChange>();
		propertyChanges.Add(new PropertyChange("duedate", oldDuedate, job.Duedate));
		propertyChanges.Add(new PropertyChange("creationDateBased", null, creationDateBased));
		commandContext.OperationLogManager.logJobOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_RECALC_DUEDATE, jobId, job.JobDefinitionId, job.ProcessInstanceId, job.ProcessDefinitionId, job.ProcessDefinitionKey, propertyChanges);

		return null;
	  }

	  protected internal virtual void checkJobType(JobEntity job)
	  {
		string type = job.JobHandlerType;
		if (!(TimerExecuteNestedActivityJobHandler.TYPE.Equals(type) || TimerCatchIntermediateEventJobHandler.TYPE.Equals(type) || TimerStartEventJobHandler.TYPE.Equals(type) || TimerStartEventSubprocessJobHandler.TYPE.Equals(type)) || !(job is TimerEntity))
		{
		  throw new ProcessEngineException("Only timer jobs can be recalculated, but the job with id '" + jobId + "' is of type '" + type + "'.");
		}
	  }

	  protected internal virtual TimerDeclarationImpl findTimerDeclaration(CommandContext commandContext, JobEntity job)
	  {
		TimerDeclarationImpl timerDeclaration = null;
		if (!string.ReferenceEquals(job.ExecutionId, null))
		{
		  // boundary or intermediate or subprocess start event
		  timerDeclaration = findTimerDeclarationForActivity(commandContext, job);
		}
		else
		{
		  // process instance start event
		  timerDeclaration = findTimerDeclarationForProcessStartEvent(commandContext, job);
		}

		if (timerDeclaration == null)
		{
		  throw new ProcessEngineException("No timer declaration found for job id '" + jobId + "'.");
		}
		return timerDeclaration;
	  }

	  protected internal virtual TimerDeclarationImpl findTimerDeclarationForActivity(CommandContext commandContext, JobEntity job)
	  {
		ExecutionEntity execution = commandContext.ExecutionManager.findExecutionById(job.ExecutionId);
		if (execution == null)
		{
		  throw new ProcessEngineException("No execution found with id '" + job.ExecutionId + "' for job id '" + jobId + "'.");
		}
		ActivityImpl activity = execution.getProcessDefinition().findActivity(job.ActivityId);
		IDictionary<string, TimerDeclarationImpl> timerDeclarations = activity.EventScope.Properties.get(BpmnProperties.TIMER_DECLARATIONS);
		return timerDeclarations[job.ActivityId];
	  }

	  protected internal virtual TimerDeclarationImpl findTimerDeclarationForProcessStartEvent(CommandContext commandContext, JobEntity job)
	  {
		ProcessDefinitionEntity processDefinition = commandContext.ProcessEngineConfiguration.DeploymentCache.findDeployedProcessDefinitionById(job.ProcessDefinitionId);
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.List<org.camunda.bpm.engine.impl.jobexecutor.TimerDeclarationImpl> timerDeclarations = (java.util.List<org.camunda.bpm.engine.impl.jobexecutor.TimerDeclarationImpl>) processDefinition.getProperty(org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse.PROPERTYNAME_START_TIMER);
		IList<TimerDeclarationImpl> timerDeclarations = (IList<TimerDeclarationImpl>) processDefinition.getProperty(BpmnParse.PROPERTYNAME_START_TIMER);
		foreach (TimerDeclarationImpl timerDeclarationCandidate in timerDeclarations)
		{
		  if (timerDeclarationCandidate.JobDefinitionId.Equals(job.JobDefinitionId))
		  {
			return timerDeclarationCandidate;
		  }
		}
		return null;
	  }
	}

}