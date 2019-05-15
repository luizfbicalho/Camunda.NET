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
namespace org.camunda.bpm.engine.impl.cmd
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ProcessDefinitionImpl = org.camunda.bpm.engine.impl.pvm.process.ProcessDefinitionImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using TransitionInstance = org.camunda.bpm.engine.runtime.TransitionInstance;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public abstract class AbstractProcessInstanceModificationCommand : Command<Void>
	{
		public abstract T execute(org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext);

	  protected internal string processInstanceId;
	  protected internal bool skipCustomListeners;
	  protected internal bool skipIoMappings;

	  public AbstractProcessInstanceModificationCommand(string processInstanceId)
	  {
		this.processInstanceId = processInstanceId;
	  }

	  public virtual bool SkipCustomListeners
	  {
		  set
		  {
			this.skipCustomListeners = value;
		  }
	  }

	  public virtual bool SkipIoMappings
	  {
		  set
		  {
			this.skipIoMappings = value;
		  }
	  }

	  public virtual string ProcessInstanceId
	  {
		  set
		  {
			this.processInstanceId = value;
		  }
		  get
		  {
			return processInstanceId;
		  }
	  }


	  protected internal virtual ActivityInstance findActivityInstance(ActivityInstance tree, string activityInstanceId)
	  {
		if (activityInstanceId.Equals(tree.Id))
		{
		  return tree;
		}
		else
		{
		  foreach (ActivityInstance child in tree.ChildActivityInstances)
		  {
			ActivityInstance matchingChildInstance = findActivityInstance(child, activityInstanceId);
			if (matchingChildInstance != null)
			{
			  return matchingChildInstance;
			}
		  }
		}

		return null;
	  }

	  protected internal virtual TransitionInstance findTransitionInstance(ActivityInstance tree, string transitionInstanceId)
	  {
		foreach (TransitionInstance childTransitionInstance in tree.ChildTransitionInstances)
		{
		  if (matchesRequestedTransitionInstance(childTransitionInstance, transitionInstanceId))
		  {
			return childTransitionInstance;
		  }
		}

		foreach (ActivityInstance child in tree.ChildActivityInstances)
		{
		  TransitionInstance matchingChildInstance = findTransitionInstance(child, transitionInstanceId);
		  if (matchingChildInstance != null)
		  {
			return matchingChildInstance;
		  }
		}

		return null;
	  }

	  protected internal virtual bool matchesRequestedTransitionInstance(TransitionInstance instance, string queryInstanceId)
	  {
		bool match = instance.Id.Equals(queryInstanceId);

		// check if the execution queried for has been replaced by the given instance
		// => if yes, given instance is matched
		// this is a fix for CAM-4090 to tolerate inconsistent transition instance ids as described in CAM-4143
		if (!match)
		{
		  // note: execution id = transition instance id
		  ExecutionEntity cachedExecution = Context.CommandContext.DbEntityManager.getCachedEntity(typeof(ExecutionEntity), queryInstanceId);

		  // follow the links of execution replacement;
		  // note: this can be at most two hops:
		  // case 1:
		  //   the query execution is the scope execution
		  //     => tree may have expanded meanwhile
		  //     => scope execution references replacing execution directly (one hop)
		  //
		  // case 2:
		  //   the query execution is a concurrent execution
		  //     => tree may have compacted meanwhile
		  //     => concurrent execution references scope execution directly (one hop)
		  //
		  // case 3:
		  //   the query execution is a concurrent execution
		  //     => tree may have compacted/expanded/compacted/../expanded any number of times
		  //     => the concurrent execution has been removed and therefore references the scope execution (first hop)
		  //     => the scope execution may have been replaced itself again with another concurrent execution (second hop)
		  //   note that the scope execution may have a long "history" of replacements, but only the last replacement is relevant here
		  if (cachedExecution != null)
		  {
			ExecutionEntity replacingExecution = cachedExecution.resolveReplacedBy();

			if (replacingExecution != null)
			{
			  match = replacingExecution.Id.Equals(instance.Id);
			}
		  }
		}

		return match;
	  }

	  protected internal virtual ScopeImpl getScopeForActivityInstance(ProcessDefinitionImpl processDefinition, ActivityInstance activityInstance)
	  {
		string scopeId = activityInstance.ActivityId;

		if (processDefinition.Id.Equals(scopeId))
		{
		  return processDefinition;
		}
		else
		{
		  return processDefinition.findActivity(scopeId);
		}
	  }

	  protected internal virtual ExecutionEntity getScopeExecutionForActivityInstance(ExecutionEntity processInstance, ActivityExecutionTreeMapping mapping, ActivityInstance activityInstance)
	  {
		ensureNotNull("activityInstance", activityInstance);

		ProcessDefinitionImpl processDefinition = processInstance.getProcessDefinition();
		ScopeImpl scope = getScopeForActivityInstance(processDefinition, activityInstance);

		ISet<ExecutionEntity> executions = mapping.getExecutions(scope);
		ISet<string> activityInstanceExecutions = new HashSet<string>(Arrays.asList(activityInstance.ExecutionIds));

		// TODO: this is a hack around the activity instance tree
		// remove with fix of CAM-3574
		foreach (string activityInstanceExecutionId in activityInstance.ExecutionIds)
		{
		  ExecutionEntity execution = Context.CommandContext.ExecutionManager.findExecutionById(activityInstanceExecutionId);
		  if (execution.Concurrent && execution.hasChildren())
		  {
			// concurrent executions have at most one child
			ExecutionEntity child = execution.Executions[0];
			activityInstanceExecutions.Add(child.Id);
		  }
		}

		// find the scope execution for the given activity instance
		ISet<ExecutionEntity> retainedExecutionsForInstance = new HashSet<ExecutionEntity>();
		foreach (ExecutionEntity execution in executions)
		{
		  if (activityInstanceExecutions.Contains(execution.Id))
		  {
			retainedExecutionsForInstance.Add(execution);
		  }
		}

		if (retainedExecutionsForInstance.Count != 1)
		{
		  throw new ProcessEngineException("There are " + retainedExecutionsForInstance.Count + " (!= 1) executions for activity instance " + activityInstance.Id);
		}

		return retainedExecutionsForInstance.GetEnumerator().next();
	  }

	  protected internal virtual string describeFailure(string detailMessage)
	  {
		return "Cannot perform instruction: " + describe() + "; " + detailMessage;
	  }

	  protected internal abstract string describe();

	  public override string ToString()
	  {
		return describe();
	  }
	}

}