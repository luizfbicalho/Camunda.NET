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
namespace org.camunda.bpm.engine.test.api.runtime.migration
{

	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ExecutionTree = org.camunda.bpm.engine.test.util.ExecutionTree;

	/// <summary>
	/// Helper class to save the current state of a process instance.
	/// </summary>
	public class ProcessInstanceSnapshot
	{

	  protected internal string processInstanceId;
	  protected internal string processDefinitionId;
	  protected internal string deploymentId;
	  protected internal ActivityInstance activityTree;
	  protected internal ExecutionTree executionTree;
	  protected internal IList<EventSubscription> eventSubscriptions;
	  protected internal IList<Job> jobs;
	  protected internal IList<JobDefinition> jobDefinitions;
	  protected internal IList<Task> tasks;
	  protected internal IDictionary<string, VariableInstance> variables;

	  public ProcessInstanceSnapshot(string processInstanceId, string processDefinitionId)
	  {
		this.processInstanceId = processInstanceId;
		this.processDefinitionId = processDefinitionId;
	  }

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId;
		  }
		  set
		  {
			this.processInstanceId = value;
		  }
	  }


	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId;
		  }
		  set
		  {
			this.processDefinitionId = value;
		  }
	  }

	  public virtual string DeploymentId
	  {
		  set
		  {
			this.deploymentId = value;
		  }
		  get
		  {
			return deploymentId;
		  }
	  }



	  public virtual ActivityInstance ActivityTree
	  {
		  get
		  {
			ensurePropertySaved("activity tree", activityTree);
			return activityTree;
		  }
		  set
		  {
			this.activityTree = value;
		  }
	  }


	  public virtual ExecutionTree ExecutionTree
	  {
		  get
		  {
			ensurePropertySaved("execution tree", executionTree);
			return executionTree;
		  }
		  set
		  {
			this.executionTree = value;
		  }
	  }


	  public virtual IList<Task> Tasks
	  {
		  set
		  {
			this.tasks = value;
		  }
		  get
		  {
			ensurePropertySaved("tasks", tasks);
			return tasks;
		  }
	  }


	  public virtual Task getTaskForKey(string key)
	  {
		foreach (Task task in Tasks)
		{
		  if (key.Equals(task.TaskDefinitionKey))
		  {
			return task;
		  }
		}
		return null;
	  }

	  public virtual IList<EventSubscription> EventSubscriptions
	  {
		  get
		  {
			ensurePropertySaved("event subscriptions", eventSubscriptions);
			return eventSubscriptions;
		  }
		  set
		  {
			this.eventSubscriptions = value;
		  }
	  }

	  public virtual EventSubscription getEventSubscriptionById(string id)
	  {
		foreach (EventSubscription subscription in eventSubscriptions)
		{
		  if (subscription.Id.Equals(id))
		  {
			return subscription;
		  }
		}

		return null;
	  }

	  public virtual EventSubscription getEventSubscriptionForActivityIdAndEventName(string activityId, string eventName)
	  {

		IList<EventSubscription> collectedEventsubscriptions = getEventSubscriptionsForActivityIdAndEventName(activityId, eventName);

		if (collectedEventsubscriptions.Count == 0)
		{
		  return null;
		}
		else if (collectedEventsubscriptions.Count == 1)
		{
		  return collectedEventsubscriptions[0];
		}
		else
		{
		  throw new Exception("There is more than one event subscription for activity " + activityId + " and event " + eventName);
		}
	  }

	  public virtual IList<EventSubscription> getEventSubscriptionsForActivityIdAndEventName(string activityId, string eventName)
	  {

		IList<EventSubscription> collectedEventsubscriptions = new List<EventSubscription>();

		foreach (EventSubscription eventSubscription in EventSubscriptions)
		{
		  if (activityId.Equals(eventSubscription.ActivityId))
		  {
			if ((string.ReferenceEquals(eventName, null) && string.ReferenceEquals(eventSubscription.EventName, null)) || !string.ReferenceEquals(eventName, null) && eventName.Equals(eventSubscription.EventName))
			{
			  collectedEventsubscriptions.Add(eventSubscription);
			}
		  }
		}

		return collectedEventsubscriptions;
	  }


	  public virtual IList<Job> Jobs
	  {
		  get
		  {
			ensurePropertySaved("jobs", jobs);
			return jobs;
		  }
		  set
		  {
			this.jobs = value;
		  }
	  }

	  public virtual Job getJobForDefinitionId(string jobDefinitionId)
	  {
		IList<Job> collectedJobs = new List<Job>();

		foreach (Job job in Jobs)
		{
		  if (jobDefinitionId.Equals(job.JobDefinitionId))
		  {
			collectedJobs.Add(job);
		  }
		}

		if (collectedJobs.Count == 0)
		{
		  return null;
		}
		else if (collectedJobs.Count == 1)
		{
		  return collectedJobs[0];
		}
		else
		{
		  throw new Exception("There is more than one job for job definition " + jobDefinitionId);
		}
	  }

	  public virtual Job getJobById(string jobId)
	  {
		foreach (Job job in Jobs)
		{
		  if (jobId.Equals(job.Id))
		  {
			return job;
		  }
		}

		return null;
	  }


	  public virtual IList<JobDefinition> JobDefinitions
	  {
		  get
		  {
			ensurePropertySaved("job definitions", jobDefinitions);
			return jobDefinitions;
		  }
		  set
		  {
			this.jobDefinitions = value;
		  }
	  }

	  public virtual JobDefinition getJobDefinitionForActivityIdAndType(string activityId, string jobHandlerType)
	  {

		IList<JobDefinition> collectedDefinitions = new List<JobDefinition>();
		foreach (JobDefinition jobDefinition in JobDefinitions)
		{
		  if (activityId.Equals(jobDefinition.ActivityId) && jobHandlerType.Equals(jobDefinition.JobType))
		  {
			collectedDefinitions.Add(jobDefinition);
		  }
		}

		if (collectedDefinitions.Count == 0)
		{
		  return null;
		}
		else if (collectedDefinitions.Count == 1)
		{
		  return collectedDefinitions[0];
		}
		else
		{
		  throw new Exception("There is more than one job definition for activity " + activityId + " and job handler type " + jobHandlerType);
		}
	  }


	  public virtual ICollection<VariableInstance> getVariables()
	  {
		return variables.Values;
	  }

	  public virtual void setVariables(IList<VariableInstance> variables)
	  {
		this.variables = new Dictionary<string, VariableInstance>();

		foreach (VariableInstance variable in variables)
		{
		  this.variables[variable.Id] = variable;
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.runtime.VariableInstance getSingleVariable(final String variableName)
	  public virtual VariableInstance getSingleVariable(string variableName)
	  {
		return getSingleVariable(new ConditionAnonymousInnerClass(this, variableName));
	  }

	  private class ConditionAnonymousInnerClass : Condition<VariableInstance>
	  {
		  private readonly ProcessInstanceSnapshot outerInstance;

		  private string variableName;

		  public ConditionAnonymousInnerClass(ProcessInstanceSnapshot outerInstance, string variableName)
		  {
			  this.outerInstance = outerInstance;
			  this.variableName = variableName;
		  }


		  public bool matches(VariableInstance variable)
		  {
			return variableName.Equals(variable.Name);
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.runtime.VariableInstance getSingleVariable(final String executionId, final String variableName)
	  public virtual VariableInstance getSingleVariable(string executionId, string variableName)
	  {
		return getSingleVariable(new ConditionAnonymousInnerClass2(this, executionId, variableName));
	  }

	  private class ConditionAnonymousInnerClass2 : Condition<VariableInstance>
	  {
		  private readonly ProcessInstanceSnapshot outerInstance;

		  private string executionId;
		  private string variableName;

		  public ConditionAnonymousInnerClass2(ProcessInstanceSnapshot outerInstance, string executionId, string variableName)
		  {
			  this.outerInstance = outerInstance;
			  this.executionId = executionId;
			  this.variableName = variableName;
		  }


		  public bool matches(VariableInstance variable)
		  {
			return executionId.Equals(variable.ExecutionId) && variableName.Equals(variable.Name);
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.runtime.VariableInstance getSingleTaskVariable(final String taskId, final String variableName)
	  public virtual VariableInstance getSingleTaskVariable(string taskId, string variableName)
	  {
		return getSingleVariable(new ConditionAnonymousInnerClass3(this, taskId, variableName));
	  }

	  private class ConditionAnonymousInnerClass3 : Condition<VariableInstance>
	  {
		  private readonly ProcessInstanceSnapshot outerInstance;

		  private string taskId;
		  private string variableName;

		  public ConditionAnonymousInnerClass3(ProcessInstanceSnapshot outerInstance, string taskId, string variableName)
		  {
			  this.outerInstance = outerInstance;
			  this.taskId = taskId;
			  this.variableName = variableName;
		  }


		  public bool matches(VariableInstance variable)
		  {
			return variableName.Equals(variable.Name) && taskId.Equals(variable.TaskId);
		  }
	  }

	  protected internal virtual VariableInstance getSingleVariable(Condition<VariableInstance> condition)
	  {
		IList<VariableInstance> matchingVariables = new List<VariableInstance>();

		foreach (VariableInstance variable in variables.Values)
		{
		  if (condition.matches(variable))
		  {
			matchingVariables.Add(variable);
		  }
		}

		if (matchingVariables.Count == 1)
		{
		  return matchingVariables[0];
		}
		else if (matchingVariables.Count == 0)
		{
		  return null;
		}
		else
		{
		  throw new Exception("There is more than one variable that matches the given condition");
		}
	  }

	  public virtual VariableInstance getVariable(string id)
	  {
		return variables[id];
	  }

	  protected internal virtual void ensurePropertySaved(string name, object property)
	  {
		EnsureUtil.ensureNotNull(typeof(BadUserRequestException), "The snapshot has not saved the " + name + " of the process instance", name, property);
	  }

	  protected internal interface Condition<T>
	  {
		bool matches(T condition);
	  }

	}

}