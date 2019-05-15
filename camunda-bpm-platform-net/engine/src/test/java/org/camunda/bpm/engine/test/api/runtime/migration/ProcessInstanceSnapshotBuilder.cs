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

	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ExecutionTree = org.camunda.bpm.engine.test.util.ExecutionTree;

	public class ProcessInstanceSnapshotBuilder
	{

	  protected internal ProcessEngine processEngine;
	  protected internal string processInstanceId;
	  protected internal ProcessInstanceSnapshot snapshot;

	  public ProcessInstanceSnapshotBuilder(ProcessInstance processInstance, ProcessEngine processEngine)
	  {
		this.processEngine = processEngine;
		this.processInstanceId = processInstance.Id;
		this.snapshot = new ProcessInstanceSnapshot(processInstance.Id, processInstance.ProcessDefinitionId);
	  }

	  public virtual ProcessInstanceSnapshotBuilder deploymentId()
	  {
		string deploymentId = processEngine.RepositoryService.getProcessDefinition(snapshot.ProcessDefinitionId).DeploymentId;
		snapshot.DeploymentId = deploymentId;

		return this;
	  }

	  public virtual ProcessInstanceSnapshotBuilder activityTree()
	  {
		ActivityInstance activityInstance = processEngine.RuntimeService.getActivityInstance(processInstanceId);
		snapshot.ActivityTree = activityInstance;

		return this;
	  }

	  public virtual ProcessInstanceSnapshotBuilder executionTree()
	  {
		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);
		snapshot.ExecutionTree = executionTree;

		return this;
	  }

	  public virtual ProcessInstanceSnapshotBuilder tasks()
	  {
		IList<Task> tasks = processEngine.TaskService.createTaskQuery().processInstanceId(processInstanceId).list();
		snapshot.Tasks = tasks;

		return this;
	  }

	  public virtual ProcessInstanceSnapshotBuilder eventSubscriptions()
	  {
		IList<EventSubscription> eventSubscriptions = processEngine.RuntimeService.createEventSubscriptionQuery().processInstanceId(processInstanceId).list();
		snapshot.EventSubscriptions = eventSubscriptions;

		return this;
	  }

	  public virtual ProcessInstanceSnapshotBuilder jobs()
	  {
		IList<Job> jobs = processEngine.ManagementService.createJobQuery().processInstanceId(processInstanceId).list();
		snapshot.Jobs = jobs;

		string processDefinitionId = processEngine.RuntimeService.createProcessInstanceQuery().processInstanceId(processInstanceId).singleResult().ProcessDefinitionId;
		IList<JobDefinition> jobDefinitions = processEngine.ManagementService.createJobDefinitionQuery().processDefinitionId(processDefinitionId).list();
		snapshot.JobDefinitions = jobDefinitions;

		return this;
	  }

	  public virtual ProcessInstanceSnapshotBuilder variables()
	  {
		IList<VariableInstance> variables = processEngine.RuntimeService.createVariableInstanceQuery().processInstanceIdIn(processInstanceId).list();
		snapshot.setVariables(variables);

		return this;
	  }

	  public virtual ProcessInstanceSnapshot build()
	  {
		return snapshot;
	  }

	  public virtual ProcessInstanceSnapshot full()
	  {
		deploymentId();
		activityTree();
		executionTree();
		tasks();
		eventSubscriptions();
		jobs();
		variables();

		return build();
	  }

	}

}