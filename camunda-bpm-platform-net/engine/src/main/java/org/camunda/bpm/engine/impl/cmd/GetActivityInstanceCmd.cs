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


	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ActivityInstanceImpl = org.camunda.bpm.engine.impl.persistence.entity.ActivityInstanceImpl;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using TransitionInstanceImpl = org.camunda.bpm.engine.impl.persistence.entity.TransitionInstanceImpl;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using CompensationBehavior = org.camunda.bpm.engine.impl.pvm.runtime.CompensationBehavior;
	using LegacyBehavior = org.camunda.bpm.engine.impl.pvm.runtime.LegacyBehavior;
	using PvmExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;

	/// <summary>
	/// <para>Creates an activity instance tree according to the following strategy:
	/// 
	/// <ul>
	///   <li> Event scope executions are not considered at all
	///   <li> For every leaf execution, generate an activity/transition instance;
	///   the activity instance id is set in the leaf execution and the parent instance id is set in the parent execution
	///   <li> For every non-leaf scope execution, generate an activity instance;
	///   the activity instance id is always set in the parent execution and the parent activity
	///   instance id is always set in the parent's parent (because of tree compactation, we ensure
	///   that an activity instance id for a scope activity is always stored in the corresponding scope execution's parent,
	///   unless the execution is a leaf)
	///   <li> Compensation is an exception to the above procedure: A compensation throw event is not a scope, however the compensating executions
	///   are added as child executions of the (probably non-scope) execution executing the throw event. Logically, the compensating executions
	///   are children of the scope execution the throwing event is executed in. Due to this oddity, the activity instance id are stored on different
	///   executions
	/// </ul>
	/// 
	/// @author Thorben Lindhauer
	/// 
	/// </para>
	/// </summary>
	public class GetActivityInstanceCmd : Command<ActivityInstance>
	{

	  protected internal string processInstanceId;

	  public GetActivityInstanceCmd(string processInstanceId)
	  {
		this.processInstanceId = processInstanceId;
	  }

	  public virtual ActivityInstance execute(CommandContext commandContext)
	  {

		ensureNotNull("processInstanceId", processInstanceId);
		IList<ExecutionEntity> executionList = loadProcessInstance(processInstanceId, commandContext);

		if (executionList.Count == 0)
		{
		  return null;
		}

		checkGetActivityInstance(processInstanceId, commandContext);

		IList<ExecutionEntity> nonEventScopeExecutions = filterNonEventScopeExecutions(executionList);
		IList<ExecutionEntity> leaves = filterLeaves(nonEventScopeExecutions);
		// Leaves must be ordered in a predictable way (e.g. by ID)
		// in order to return a stable execution tree with every repeated invocation of this command.
		// For legacy process instances, there may miss scope executions for activities that are now a scope.
		// In this situation, there may be multiple scope candidates for the same instance id; which one
		// can depend on the order the leaves are iterated.
		orderById(leaves);

		ExecutionEntity processInstance = filterProcessInstance(executionList);

		if (processInstance.Ended)
		{
		  return null;
		}

		// create act instance for process instance
		ActivityInstanceImpl processActInst = createActivityInstance(processInstance, processInstance.getProcessDefinition(), processInstanceId, null);
		IDictionary<string, ActivityInstanceImpl> activityInstances = new Dictionary<string, ActivityInstanceImpl>();
		activityInstances[processInstanceId] = processActInst;

		IDictionary<string, TransitionInstanceImpl> transitionInstances = new Dictionary<string, TransitionInstanceImpl>();

		foreach (ExecutionEntity leaf in leaves)
		{
		  // skip leafs without activity, e.g. if only the process instance exists after cancellation
		  // it will not have an activity set
		  if (leaf.getActivity() == null)
		  {
			continue;
		  }

		  IDictionary<ScopeImpl, PvmExecutionImpl> activityExecutionMapping = leaf.createActivityExecutionMapping();
		  IDictionary<ScopeImpl, PvmExecutionImpl> scopeInstancesToCreate = new Dictionary<ScopeImpl, PvmExecutionImpl>(activityExecutionMapping);

		  // create an activity/transition instance for each leaf that executes a non-scope activity
		  // and does not throw compensation
		  if (!string.ReferenceEquals(leaf.ActivityInstanceId, null))
		  {

			if (!CompensationBehavior.isCompensationThrowing(leaf) || LegacyBehavior.isCompensationThrowing(leaf, activityExecutionMapping))
			{
			  string parentActivityInstanceId = null;

			  parentActivityInstanceId = activityExecutionMapping[leaf.getActivity().FlowScope].ParentActivityInstanceId;

			  ActivityInstanceImpl leafInstance = createActivityInstance(leaf, leaf.getActivity(), leaf.ActivityInstanceId, parentActivityInstanceId);
			  activityInstances[leafInstance.Id] = leafInstance;

			  scopeInstancesToCreate.Remove(leaf.getActivity());
			}
		  }
		  else
		  {
			TransitionInstanceImpl transitionInstance = createTransitionInstance(leaf);
			transitionInstances[transitionInstance.Id] = transitionInstance;

			scopeInstancesToCreate.Remove(leaf.getActivity());
		  }

		  LegacyBehavior.removeLegacyNonScopesFromMapping(scopeInstancesToCreate);
		  scopeInstancesToCreate.Remove(leaf.getProcessDefinition());

		  // create an activity instance for each scope (including compensation throwing executions)
		  foreach (KeyValuePair<ScopeImpl, PvmExecutionImpl> scopeExecutionEntry in scopeInstancesToCreate.SetOfKeyValuePairs())
		  {
			ScopeImpl scope = scopeExecutionEntry.Key;
			PvmExecutionImpl scopeExecution = scopeExecutionEntry.Value;

			string activityInstanceId = null;
			string parentActivityInstanceId = null;

			activityInstanceId = scopeExecution.ParentActivityInstanceId;
			parentActivityInstanceId = activityExecutionMapping[scope.FlowScope].ParentActivityInstanceId;

			if (activityInstances.ContainsKey(activityInstanceId))
			{
			  continue;
			}
			else
			{
			  // regardless of the tree structure (compacted or not), the scope's activity instance id
			  // is the activity instance id of the parent execution and the parent activity instance id
			  // of that is the actual parent activity instance id
			  ActivityInstanceImpl scopeInstance = createActivityInstance(scopeExecution, scope, activityInstanceId, parentActivityInstanceId);
			  activityInstances[activityInstanceId] = scopeInstance;
			}
		  }
		}

		LegacyBehavior.repairParentRelationships(activityInstances.Values, processInstanceId);
		populateChildInstances(activityInstances, transitionInstances);

		return processActInst;
	  }

	  protected internal virtual void checkGetActivityInstance(string processInstanceId, CommandContext commandContext)
	  {
		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkReadProcessInstance(processInstanceId);
		}
	  }

	  protected internal virtual void orderById(IList<ExecutionEntity> leaves)
	  {
		leaves.Sort(ExecutionIdComparator.INSTANCE);
	  }

	  protected internal virtual ActivityInstanceImpl createActivityInstance(PvmExecutionImpl scopeExecution, ScopeImpl scope, string activityInstanceId, string parentActivityInstanceId)
	  {
		ActivityInstanceImpl actInst = new ActivityInstanceImpl();

		actInst.Id = activityInstanceId;
		actInst.ParentActivityInstanceId = parentActivityInstanceId;
		actInst.ProcessInstanceId = scopeExecution.ProcessInstanceId;
		actInst.ProcessDefinitionId = scopeExecution.ProcessDefinitionId;
		actInst.BusinessKey = scopeExecution.BusinessKey;
		actInst.ActivityId = scope.Id;

		string name = scope.Name;
		if (string.ReferenceEquals(name, null))
		{
		  name = (string) scope.getProperty("name");
		}
		actInst.ActivityName = name;

		if (scope.Id.Equals(scopeExecution.ProcessDefinition.Id))
		{
		  actInst.ActivityType = "processDefinition";
		}
		else
		{
		  actInst.ActivityType = (string) scope.getProperty("type");
		}

		IList<string> executionIds = new List<string>();
		executionIds.Add(scopeExecution.Id);

		foreach (PvmExecutionImpl childExecution in scopeExecution.NonEventScopeExecutions)
		{
		  // add all concurrent children that are not in an activity
		  if (childExecution.Concurrent && (string.ReferenceEquals(childExecution.ActivityId, null)))
		  {
			executionIds.Add(childExecution.Id);
		  }
		}
		actInst.ExecutionIds = executionIds.ToArray();

		return actInst;
	  }

	  protected internal virtual TransitionInstanceImpl createTransitionInstance(PvmExecutionImpl execution)
	  {
		TransitionInstanceImpl transitionInstance = new TransitionInstanceImpl();

		// can use execution id as persistent ID for transition as an execution
		// can execute as most one transition at a time.
		transitionInstance.Id = execution.Id;
		transitionInstance.ParentActivityInstanceId = execution.ParentActivityInstanceId;
		transitionInstance.ProcessInstanceId = execution.ProcessInstanceId;
		transitionInstance.ProcessDefinitionId = execution.ProcessDefinitionId;
		transitionInstance.ExecutionId = execution.Id;
		transitionInstance.ActivityId = execution.ActivityId;

		ActivityImpl activity = execution.getActivity();
		if (activity != null)
		{
		  string name = activity.Name;
		  if (string.ReferenceEquals(name, null))
		  {
			name = (string) activity.getProperty("name");
		  }
		  transitionInstance.ActivityName = name;
		  transitionInstance.ActivityType = (string) activity.getProperty("type");
		}

		return transitionInstance;
	  }

	  protected internal virtual void populateChildInstances(IDictionary<string, ActivityInstanceImpl> activityInstances, IDictionary<string, TransitionInstanceImpl> transitionInstances)
	  {
		IDictionary<ActivityInstanceImpl, IList<ActivityInstanceImpl>> childActivityInstances = new Dictionary<ActivityInstanceImpl, IList<ActivityInstanceImpl>>();
		IDictionary<ActivityInstanceImpl, IList<TransitionInstanceImpl>> childTransitionInstances = new Dictionary<ActivityInstanceImpl, IList<TransitionInstanceImpl>>();

		foreach (ActivityInstanceImpl instance in activityInstances.Values)
		{
		  if (!string.ReferenceEquals(instance.ParentActivityInstanceId, null))
		  {
			ActivityInstanceImpl parentInstance = activityInstances[instance.ParentActivityInstanceId];
			if (parentInstance == null)
			{
			  throw new ProcessEngineException("No parent activity instance with id " + instance.ParentActivityInstanceId + " generated");
			}
			putListElement(childActivityInstances, parentInstance, instance);
		  }
		}

		foreach (TransitionInstanceImpl instance in transitionInstances.Values)
		{
		  if (!string.ReferenceEquals(instance.ParentActivityInstanceId, null))
		  {
			ActivityInstanceImpl parentInstance = activityInstances[instance.ParentActivityInstanceId];
			if (parentInstance == null)
			{
			  throw new ProcessEngineException("No parent activity instance with id " + instance.ParentActivityInstanceId + " generated");
			}
			putListElement(childTransitionInstances, parentInstance, instance);
		  }
		}

		foreach (KeyValuePair<ActivityInstanceImpl, IList<ActivityInstanceImpl>> entry in childActivityInstances.SetOfKeyValuePairs())
		{
		  ActivityInstanceImpl instance = entry.Key;
		  IList<ActivityInstanceImpl> childInstances = entry.Value;
		  if (childInstances != null)
		  {
			instance.ChildActivityInstances = childInstances.ToArray();
		  }
		}

		foreach (KeyValuePair<ActivityInstanceImpl, IList<TransitionInstanceImpl>> entry in childTransitionInstances.SetOfKeyValuePairs())
		{
		ActivityInstanceImpl instance = entry.Key;
		IList<TransitionInstanceImpl> childInstances = entry.Value;
		if (childTransitionInstances != null)
		{
		  instance.ChildTransitionInstances = childInstances.ToArray();
		}
		}

	  }

	  protected internal virtual void putListElement<S, T>(IDictionary<S, IList<T>> mapOfLists, S key, T listElement)
	  {
		IList<T> list = mapOfLists[key];
		if (list == null)
		{
		  list = new List<T>();
		  mapOfLists[key] = list;
		}
		list.Add(listElement);
	  }

	  protected internal virtual ExecutionEntity filterProcessInstance(IList<ExecutionEntity> executionList)
	  {
		foreach (ExecutionEntity execution in executionList)
		{
		  if (execution.ProcessInstanceExecution)
		  {
			return execution;
		  }
		}

		throw new ProcessEngineException("Could not determine process instance execution");
	  }

	  protected internal virtual IList<ExecutionEntity> filterLeaves(IList<ExecutionEntity> executionList)
	  {
		IList<ExecutionEntity> leaves = new List<ExecutionEntity>();
		foreach (ExecutionEntity execution in executionList)
		{
		  // although executions executing throwing compensation events are not leaves in the tree,
		  // they are treated as leaves since their child executions are logical children of their parent scope execution
		  if (execution.NonEventScopeExecutions.Count == 0 || CompensationBehavior.isCompensationThrowing(execution))
		  {
			leaves.Add(execution);
		  }
		}
		return leaves;
	  }

	  protected internal virtual IList<ExecutionEntity> filterNonEventScopeExecutions(IList<ExecutionEntity> executionList)
	  {
		IList<ExecutionEntity> nonEventScopeExecutions = new List<ExecutionEntity>();
		foreach (ExecutionEntity execution in executionList)
		{
		  if (!execution.EventScope)
		  {
			nonEventScopeExecutions.Add(execution);
		  }
		}
		return nonEventScopeExecutions;
	  }

	  protected internal virtual IList<ExecutionEntity> loadProcessInstance(string processInstanceId, CommandContext commandContext)
	  {

		IList<ExecutionEntity> result = null;

		// first try to load from cache
		// check whether the process instance is already (partially) loaded in command context
		IList<ExecutionEntity> cachedExecutions = commandContext.DbEntityManager.getCachedEntitiesByType(typeof(ExecutionEntity));
		foreach (ExecutionEntity executionEntity in cachedExecutions)
		{
		  if (processInstanceId.Equals(executionEntity.ProcessInstanceId))
		  {
			// found one execution from process instance
			result = new List<ExecutionEntity>();
			ExecutionEntity processInstance = executionEntity.getProcessInstance();
			// add process instance
			result.Add(processInstance);
			loadChildExecutionsFromCache(processInstance, result);
			break;
		  }
		}

		if (result == null)
		{
		  // if the process instance could not be found in cache, load from database
		  result = loadFromDb(processInstanceId, commandContext);
		}

		return result;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected java.util.List<org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity> loadFromDb(final String processInstanceId, final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  protected internal virtual IList<ExecutionEntity> loadFromDb(string processInstanceId, CommandContext commandContext)
	  {

		IList<ExecutionEntity> executions = commandContext.ExecutionManager.findExecutionsByProcessInstanceId(processInstanceId);
		ExecutionEntity processInstance = commandContext.ExecutionManager.findExecutionById(processInstanceId);

		// initialize parent/child sets
		if (processInstance != null)
		{
		  processInstance.restoreProcessInstance(executions, null, null, null, null, null, null);
		}

		return executions;
	  }

	  /// <summary>
	  /// Loads all executions that are part of this process instance tree from the dbSqlSession cache.
	  /// (optionally querying the db if a child is not already loaded.
	  /// </summary>
	  /// <param name="execution"> the current root execution (already contained in childExecutions) </param>
	  /// <param name="childExecutions"> the list in which all child executions should be collected </param>
	  protected internal virtual void loadChildExecutionsFromCache(ExecutionEntity execution, IList<ExecutionEntity> childExecutions)
	  {
		IList<ExecutionEntity> childrenOfThisExecution = execution.Executions;
		if (childrenOfThisExecution != null)
		{
		  ((IList<ExecutionEntity>)childExecutions).AddRange(childrenOfThisExecution);
		  foreach (ExecutionEntity child in childrenOfThisExecution)
		  {
			loadChildExecutionsFromCache(child, childExecutions);
		  }
		}
	  }

	  public class ExecutionIdComparator : IComparer<ExecutionEntity>
	  {

		public static readonly ExecutionIdComparator INSTANCE = new ExecutionIdComparator();

		public virtual int Compare(ExecutionEntity o1, ExecutionEntity o2)
		{
		  return o1.Id.CompareTo(o2.Id);
		}

	  }



	}

}