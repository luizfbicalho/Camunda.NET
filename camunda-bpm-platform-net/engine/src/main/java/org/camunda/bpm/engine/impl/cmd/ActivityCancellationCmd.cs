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

	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ProcessDefinitionImpl = org.camunda.bpm.engine.impl.pvm.process.ProcessDefinitionImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using TransitionInstance = org.camunda.bpm.engine.runtime.TransitionInstance;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ActivityCancellationCmd : AbstractProcessInstanceModificationCommand
	{

	  protected internal string activityId;
	  protected internal bool cancelCurrentActiveActivityInstances;
	  protected internal ActivityInstance activityInstanceTree;

	  public ActivityCancellationCmd(string activityId) : this(null, activityId)
	  {
	  }

	  public ActivityCancellationCmd(string processInstanceId, string activityId) : base(processInstanceId)
	  {
		this.activityId = activityId;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public Void execute(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  public override Void execute(CommandContext commandContext)
	  {
		ActivityInstance activityInstanceTree = getActivityInstanceTree(commandContext);
		IList<AbstractInstanceCancellationCmd> commands = createActivityInstanceCancellations(activityInstanceTree, commandContext);

		foreach (AbstractInstanceCancellationCmd cmd in commands)
		{
		  cmd.SkipCustomListeners = skipCustomListeners;
		  cmd.SkipIoMappings = skipIoMappings;
		  cmd.execute(commandContext);
		}

		return null;
	  }

	  protected internal virtual ISet<string> collectParentScopeIdsForActivity(ProcessDefinitionImpl processDefinition, string activityId)
	  {
		ISet<string> parentScopeIds = new HashSet<string>();
		ScopeImpl scope = processDefinition.findActivity(activityId);

		while (scope != null)
		{
		  parentScopeIds.Add(scope.Id);
		  scope = scope.FlowScope;
		}

		return parentScopeIds;
	  }

	  protected internal virtual IList<TransitionInstance> getTransitionInstancesForActivity(ActivityInstance tree, ISet<string> parentScopeIds)
	  {
		// prune all search paths that are not in the scope hierarchy of the activity in question
		if (!parentScopeIds.Contains(tree.ActivityId))
		{
		  return Collections.emptyList();
		}

		IList<TransitionInstance> instances = new List<TransitionInstance>();
		TransitionInstance[] transitionInstances = tree.ChildTransitionInstances;

		foreach (TransitionInstance transitionInstance in transitionInstances)
		{
		  if (activityId.Equals(transitionInstance.ActivityId))
		  {
			instances.Add(transitionInstance);
		  }
		}

		foreach (ActivityInstance child in tree.ChildActivityInstances)
		{
		  ((IList<TransitionInstance>)instances).AddRange(getTransitionInstancesForActivity(child, parentScopeIds));
		}

		return instances;
	  }

	  protected internal virtual IList<ActivityInstance> getActivityInstancesForActivity(ActivityInstance tree, ISet<string> parentScopeIds)
	  {
		// prune all search paths that are not in the scope hierarchy of the activity in question
		if (!parentScopeIds.Contains(tree.ActivityId))
		{
		  return Collections.emptyList();
		}

		IList<ActivityInstance> instances = new List<ActivityInstance>();

		if (activityId.Equals(tree.ActivityId))
		{
		  instances.Add(tree);
		}

		foreach (ActivityInstance child in tree.ChildActivityInstances)
		{
		  ((IList<ActivityInstance>)instances).AddRange(getActivityInstancesForActivity(child, parentScopeIds));
		}

		return instances;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.runtime.ActivityInstance getActivityInstanceTree(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  public virtual ActivityInstance getActivityInstanceTree(CommandContext commandContext)
	  {
		return commandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this, commandContext));
	  }

	  private class CallableAnonymousInnerClass : Callable<ActivityInstance>
	  {
		  private readonly ActivityCancellationCmd outerInstance;

		  private CommandContext commandContext;

		  public CallableAnonymousInnerClass(ActivityCancellationCmd outerInstance, CommandContext commandContext)
		  {
			  this.outerInstance = outerInstance;
			  this.commandContext = commandContext;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public org.camunda.bpm.engine.runtime.ActivityInstance call() throws Exception
		  public override ActivityInstance call()
		  {
			return (new GetActivityInstanceCmd(outerInstance.processInstanceId)).execute(commandContext);
		  }
	  }

	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId;
		  }
	  }

	  public virtual ActivityInstance ActivityInstanceTreeToCancel
	  {
		  set
		  {
			this.activityInstanceTree = value;
		  }
	  }

	  protected internal override string describe()
	  {
		return "Cancel all instances of activity '" + activityId + "'";
	  }

	  public virtual IList<AbstractInstanceCancellationCmd> createActivityInstanceCancellations(ActivityInstance activityInstanceTree, CommandContext commandContext)
	  {
		IList<AbstractInstanceCancellationCmd> commands = new List<AbstractInstanceCancellationCmd>();

		ExecutionEntity processInstance = commandContext.ExecutionManager.findExecutionById(processInstanceId);
		ProcessDefinitionImpl processDefinition = processInstance.getProcessDefinition();
		ISet<string> parentScopeIds = collectParentScopeIdsForActivity(processDefinition, activityId);

		IList<ActivityInstance> childrenForActivity = getActivityInstancesForActivity(activityInstanceTree, parentScopeIds);
		foreach (ActivityInstance instance in childrenForActivity)
		{
		  commands.Add(new ActivityInstanceCancellationCmd(processInstanceId, instance.Id));
		}

		IList<TransitionInstance> transitionInstancesForActivity = getTransitionInstancesForActivity(activityInstanceTree, parentScopeIds);
		foreach (TransitionInstance instance in transitionInstancesForActivity)
		{
		  commands.Add(new TransitionInstanceCancellationCmd(processInstanceId, instance.Id));
		}
		return commands;

	  }

	  public virtual bool CancelCurrentActiveActivityInstances
	  {
		  get
		  {
			return cancelCurrentActiveActivityInstances;
		  }
		  set
		  {
			this.cancelCurrentActiveActivityInstances = value;
		  }
	  }

	}

}