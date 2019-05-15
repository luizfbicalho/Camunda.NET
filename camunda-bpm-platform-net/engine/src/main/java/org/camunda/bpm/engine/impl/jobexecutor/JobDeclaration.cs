using System;

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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ParameterValueProvider = org.camunda.bpm.engine.impl.core.variable.mapping.value.ParameterValueProvider;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using JobDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.JobDefinitionEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ProcessDefinitionImpl = org.camunda.bpm.engine.impl.pvm.process.ProcessDefinitionImpl;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;

	/// <summary>
	/// <para>A job declaration is associated with an activity in the process definition graph.
	/// It provides data about jobs which are to be created when executing this activity.
	/// It also acts as a factory for new Job Instances.</para>
	/// 
	/// <para>Jobs are of a type T and are created in the context of type S (e.g. an execution or an event subscription).
	/// An instance of the context class is handed in when a job is created.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	[Serializable]
	public abstract class JobDeclaration<S, T> where T : org.camunda.bpm.engine.impl.persistence.entity.JobEntity
	{

	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// the id of the associated persistent jobDefinitionId </summary>
	  protected internal string jobDefinitionId;

	  protected internal string jobHandlerType;
	  protected internal JobHandlerConfiguration jobHandlerConfiguration;
	  protected internal string jobConfiguration;

	  protected internal bool exclusive = JobEntity.DEFAULT_EXCLUSIVE;

	  protected internal ActivityImpl activity;

	  protected internal ParameterValueProvider jobPriorityProvider;

	  public JobDeclaration(string jobHandlerType)
	  {
		this.jobHandlerType = jobHandlerType;
	  }

	  // Job instance factory //////////////////////////////////////////

	  /// 
	  /// <returns> the created Job instances </returns>
	  public virtual T createJobInstance(S context)
	  {

		T job = newJobInstance(context);

		// set job definition id
		string jobDefinitionId = resolveJobDefinitionId(context);
		job.JobDefinitionId = jobDefinitionId;

		if (!string.ReferenceEquals(jobDefinitionId, null))
		{

		  JobDefinitionEntity jobDefinition = Context.CommandContext.JobDefinitionManager.findById(jobDefinitionId);

		  if (jobDefinition != null)
		  {
			// if job definition is suspended while creating a job instance,
			// suspend the job instance right away:
			job.SuspensionState = jobDefinition.SuspensionState;
			job.ProcessDefinitionKey = jobDefinition.ProcessDefinitionKey;
			job.ProcessDefinitionId = jobDefinition.ProcessDefinitionId;
			job.TenantId = jobDefinition.TenantId;
		  }

		}

		job.JobHandlerConfiguration = resolveJobHandlerConfiguration(context);
		job.JobHandlerType = resolveJobHandlerType(context);
		job.Exclusive = resolveExclusive(context);
		job.Retries = resolveRetries(context);
		job.Duedate = resolveDueDate(context);


		// contentExecution can be null in case of a timer start event or
		// and batch jobs unrelated to executions
		ExecutionEntity contextExecution = resolveExecution(context);

		if (Context.ProcessEngineConfiguration.ProducePrioritizedJobs)
		{
		  long priority = Context.ProcessEngineConfiguration.JobPriorityProvider.determinePriority(contextExecution, this, jobDefinitionId);

		  job.Priority = priority;
		}

		if (contextExecution != null)
		{
		  // in case of shared process definitions, the job definitions have no tenant id.
		  // To distinguish jobs between tenants and enable the tenant check for the job executor,
		  // use the tenant id from the execution.
		  job.TenantId = contextExecution.TenantId;
		}

		postInitialize(context, job);

		return job;
	  }

	  /// <summary>
	  /// Re-initialize configuration part.
	  /// </summary>
	  public virtual T reconfigure(S context, T job)
	  {
		return job;
	  }

	  /// <summary>
	  /// general callback to override any configuration after the defaults have been applied
	  /// </summary>
	  protected internal virtual void postInitialize(S context, T job)
	  {
	  }

	  /// <summary>
	  /// Returns the execution in which context the job is created. The execution
	  /// is used to determine the job's priority based on a BPMN activity
	  /// the execution is currently executing. May be null.
	  /// </summary>
	  protected internal abstract ExecutionEntity resolveExecution(S context);

	  protected internal abstract T newJobInstance(S context);

	  // Getter / Setters //////////////////////////////////////////

	  public virtual string JobDefinitionId
	  {
		  get
		  {
			return jobDefinitionId;
		  }
		  set
		  {
			this.jobDefinitionId = value;
		  }
	  }

	  protected internal virtual string resolveJobDefinitionId(S context)
	  {
		return jobDefinitionId;
	  }


	  public virtual string JobHandlerType
	  {
		  get
		  {
			return jobHandlerType;
		  }
	  }

	  protected internal virtual JobHandler resolveJobHandler()
	  {
		 JobHandler jobHandler = Context.ProcessEngineConfiguration.JobHandlers[jobHandlerType];
		 ensureNotNull("Cannot find job handler '" + jobHandlerType + "' from job '" + this + "'", "jobHandler", jobHandler);

		 return jobHandler;
	  }

	  protected internal virtual string resolveJobHandlerType(S context)
	  {
		return jobHandlerType;
	  }

	  protected internal abstract JobHandlerConfiguration resolveJobHandlerConfiguration(S context);

	  protected internal virtual bool resolveExclusive(S context)
	  {
		return exclusive;
	  }

	  protected internal virtual int resolveRetries(S context)
	  {
		return Context.ProcessEngineConfiguration.DefaultNumberOfRetries;
	  }

	  public virtual DateTime resolveDueDate(S context)
	  {
		ProcessEngineConfiguration processEngineConfiguration = Context.ProcessEngineConfiguration;
		if (processEngineConfiguration != null && (processEngineConfiguration.JobExecutorAcquireByDueDate || processEngineConfiguration.EnsureJobDueDateNotNull))
		{
		  return ClockUtil.CurrentTime;
		}
		else
		{
		  return null;
		}
	  }

	  public virtual bool Exclusive
	  {
		  get
		  {
			return exclusive;
		  }
		  set
		  {
			this.exclusive = value;
		  }
	  }


	  public virtual string ActivityId
	  {
		  get
		  {
			if (activity != null)
			{
			  return activity.Id;
			}
			else
			{
			  return null;
			}
		  }
	  }

	  public virtual ActivityImpl Activity
	  {
		  get
		  {
			return activity;
		  }
		  set
		  {
			this.activity = value;
		  }
	  }


	  public virtual ProcessDefinitionImpl ProcessDefinition
	  {
		  get
		  {
			if (activity != null)
			{
			  return activity.ProcessDefinition;
			}
			else
			{
			  return null;
			}
		  }
	  }

	  public virtual string JobConfiguration
	  {
		  get
		  {
			return jobConfiguration;
		  }
		  set
		  {
			this.jobConfiguration = value;
		  }
	  }


	  public virtual ParameterValueProvider JobPriorityProvider
	  {
		  get
		  {
			return jobPriorityProvider;
		  }
		  set
		  {
			this.jobPriorityProvider = value;
		  }
	  }

	}

}