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
namespace org.camunda.bpm.engine.impl
{

	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ProcessDefinitionImpl = org.camunda.bpm.engine.impl.pvm.process.ProcessDefinitionImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using CompensationBehavior = org.camunda.bpm.engine.impl.pvm.runtime.CompensationBehavior;
	using PvmExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;

	/// <summary>
	/// Maps an activity (plain activities + their containing flow scopes) to the scope executions
	/// that are executing them. For every instance of a scope, there is one such execution.
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class ActivityExecutionTreeMapping
	{

	  protected internal IDictionary<ScopeImpl, ISet<ExecutionEntity>> activityExecutionMapping;
	  protected internal CommandContext commandContext;
	  protected internal string processInstanceId;
	  protected internal ProcessDefinitionImpl processDefinition;

	  public ActivityExecutionTreeMapping(CommandContext commandContext, string processInstanceId)
	  {
		this.activityExecutionMapping = new Dictionary<ScopeImpl, ISet<ExecutionEntity>>();
		this.commandContext = commandContext;
		this.processInstanceId = processInstanceId;

		initialize();
	  }

	  protected internal virtual void submitExecution(ExecutionEntity execution, ScopeImpl scope)
	  {
		getExecutions(scope).Add(execution);
	  }

	  public virtual ISet<ExecutionEntity> getExecutions(ScopeImpl activity)
	  {
		ISet<ExecutionEntity> executionsForActivity = activityExecutionMapping[activity];
		if (executionsForActivity == null)
		{
		  executionsForActivity = new HashSet<ExecutionEntity>();
		  activityExecutionMapping[activity] = executionsForActivity;
		}

		return executionsForActivity;
	  }

	  public virtual ExecutionEntity getExecution(ActivityInstance activityInstance)
	  {
		ScopeImpl scope = null;

		if (activityInstance.Id.Equals(activityInstance.ProcessInstanceId))
		{
		  scope = processDefinition;
		}
		else
		{
		  scope = processDefinition.findActivity(activityInstance.ActivityId);
		}

		return intersect(getExecutions(scope), activityInstance.ExecutionIds);
	  }

	  protected internal virtual ExecutionEntity intersect(ISet<ExecutionEntity> executions, string[] executionIds)
	  {
		ISet<string> executionIdSet = new HashSet<string>();
		foreach (string executionId in executionIds)
		{
		  executionIdSet.Add(executionId);
		}

		foreach (ExecutionEntity execution in executions)
		{
		  if (executionIdSet.Contains(execution.Id))
		  {
			return execution;
		  }
		}
		throw new ProcessEngineException("Could not determine execution");
	  }

	  protected internal virtual void initialize()
	  {
		ExecutionEntity processInstance = commandContext.ExecutionManager.findExecutionById(processInstanceId);
		this.processDefinition = processInstance.getProcessDefinition();

		IList<ExecutionEntity> executions = fetchExecutionsForProcessInstance(processInstance);
		executions.Add(processInstance);

		IList<ExecutionEntity> leaves = findLeaves(executions);

		assignExecutionsToActivities(leaves);
	  }

	  protected internal virtual void assignExecutionsToActivities(IList<ExecutionEntity> leaves)
	  {
		foreach (ExecutionEntity leaf in leaves)
		{
		  ScopeImpl activity = leaf.getActivity();

		  if (activity != null)
		  {
			if (!string.ReferenceEquals(leaf.ActivityInstanceId, null))
			{
			  EnsureUtil.ensureNotNull("activity", activity);
			  submitExecution(leaf, activity);
			}
			mergeScopeExecutions(leaf);


		  }
		  else if (leaf.ProcessInstanceExecution)
		  {
			submitExecution(leaf, leaf.getProcessDefinition());
		  }
		}
	  }

	  protected internal virtual void mergeScopeExecutions(ExecutionEntity leaf)
	  {
		IDictionary<ScopeImpl, PvmExecutionImpl> mapping = leaf.createActivityExecutionMapping();

		foreach (KeyValuePair<ScopeImpl, PvmExecutionImpl> mappingEntry in mapping.SetOfKeyValuePairs())
		{
		  ScopeImpl scope = mappingEntry.Key;
		  ExecutionEntity scopeExecution = (ExecutionEntity) mappingEntry.Value;

		  submitExecution(scopeExecution, scope);
		}


	  }

	  protected internal virtual IList<ExecutionEntity> fetchExecutionsForProcessInstance(ExecutionEntity execution)
	  {
		IList<ExecutionEntity> executions = new List<ExecutionEntity>();
		((IList<ExecutionEntity>)executions).AddRange(execution.Executions);
		foreach (ExecutionEntity child in execution.Executions)
		{
		  ((IList<ExecutionEntity>)executions).AddRange(fetchExecutionsForProcessInstance(child));
		}

		return executions;
	  }

	  protected internal virtual IList<ExecutionEntity> findLeaves(IList<ExecutionEntity> executions)
	  {
		IList<ExecutionEntity> leaves = new List<ExecutionEntity>();

		foreach (ExecutionEntity execution in executions)
		{
		  if (isLeaf(execution))
		  {
			leaves.Add(execution);
		  }
		}

		return leaves;
	  }

	  /// <summary>
	  /// event-scope executions are not considered in this mapping and must be ignored
	  /// </summary>
	  protected internal virtual bool isLeaf(ExecutionEntity execution)
	  {
		if (CompensationBehavior.isCompensationThrowing(execution))
		{
		  return true;
		}
		else
		{
		  return !execution.EventScope && execution.NonEventScopeExecutions.Count == 0;
		}
	  }
	}

}