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

	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using SequentialMultiInstanceActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.SequentialMultiInstanceActivityBehavior;
	using BpmnProperties = org.camunda.bpm.engine.impl.bpmn.helper.BpmnProperties;
	using CoreActivityBehavior = org.camunda.bpm.engine.impl.core.@delegate.CoreActivityBehavior;
	using CoreModelElement = org.camunda.bpm.engine.impl.core.model.CoreModelElement;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using PvmActivity = org.camunda.bpm.engine.impl.pvm.PvmActivity;
	using PvmScope = org.camunda.bpm.engine.impl.pvm.PvmScope;
	using PvmTransition = org.camunda.bpm.engine.impl.pvm.PvmTransition;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ActivityStartBehavior = org.camunda.bpm.engine.impl.pvm.process.ActivityStartBehavior;
	using ProcessDefinitionImpl = org.camunda.bpm.engine.impl.pvm.process.ProcessDefinitionImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using TransitionImpl = org.camunda.bpm.engine.impl.pvm.process.TransitionImpl;
	using ActivityStackCollector = org.camunda.bpm.engine.impl.tree.ActivityStackCollector;
	using FlowScopeWalker = org.camunda.bpm.engine.impl.tree.FlowScopeWalker;
	using ReferenceWalker = org.camunda.bpm.engine.impl.tree.ReferenceWalker;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using VariableMapImpl = org.camunda.bpm.engine.variable.impl.VariableMapImpl;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public abstract class AbstractInstantiationCmd : AbstractProcessInstanceModificationCommand
	{

	  protected internal VariableMap variables;
	  protected internal VariableMap variablesLocal;
	  protected internal string ancestorActivityInstanceId;

	  public AbstractInstantiationCmd(string processInstanceId, string ancestorActivityInstanceId) : base(processInstanceId)
	  {
		this.ancestorActivityInstanceId = ancestorActivityInstanceId;
		this.variables = new VariableMapImpl();
		this.variablesLocal = new VariableMapImpl();
	  }

	  public virtual void addVariable(string name, object value)
	  {
		this.variables.put(name, value);
	  }

	  public virtual void addVariableLocal(string name, object value)
	  {
		this.variablesLocal.put(name, value);
	  }

	  public virtual void addVariables(IDictionary<string, object> variables)
	  {
		this.variables.putAll(variables);
	  }

	  public virtual void addVariablesLocal(IDictionary<string, object> variables)
	  {
		this.variablesLocal.putAll(variables);
	  }

	  public virtual VariableMap Variables
	  {
		  get
		  {
			return variables;
		  }
	  }

	  public virtual VariableMap VariablesLocal
	  {
		  get
		  {
			return variablesLocal;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public Void execute(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  public override Void execute(CommandContext commandContext)
	  {
		ExecutionEntity processInstance = commandContext.ExecutionManager.findExecutionById(processInstanceId);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.pvm.process.ProcessDefinitionImpl processDefinition = processInstance.getProcessDefinition();
		ProcessDefinitionImpl processDefinition = processInstance.getProcessDefinition();

		CoreModelElement elementToInstantiate = getTargetElement(processDefinition);

		EnsureUtil.ensureNotNull(typeof(NotValidException), describeFailure("Element '" + TargetElementId + "' does not exist in process '" + processDefinition.Id + "'"), "element", elementToInstantiate);

		// rebuild the mapping because the execution tree changes with every iteration
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.ActivityExecutionTreeMapping mapping = new org.camunda.bpm.engine.impl.ActivityExecutionTreeMapping(commandContext, processInstanceId);
		ActivityExecutionTreeMapping mapping = new ActivityExecutionTreeMapping(commandContext, processInstanceId);

		// before instantiating an activity, two things have to be determined:
		//
		// activityStack:
		// For the activity to instantiate, we build a stack of parent flow scopes
		// for which no executions exist yet and that have to be instantiated
		//
		// scopeExecution:
		// This is typically the execution under which a new sub tree has to be created.
		// if an explicit ancestor activity instance is set:
		//   - this is the scope execution for that ancestor activity instance
		//   - throws exception if that scope execution is not in the parent hierarchy
		//     of the activity to be started
		// if no explicit ancestor activity instance is set:
		//   - this is the execution of the first parent/ancestor flow scope that has an execution
		//   - throws an exception if there is more than one such execution

		ScopeImpl targetFlowScope = getTargetFlowScope(processDefinition);

		// prepare to walk up the flow scope hierarchy and collect the flow scope activities
		ActivityStackCollector stackCollector = new ActivityStackCollector();
		FlowScopeWalker walker = new FlowScopeWalker(targetFlowScope);
		walker.addPreVisitor(stackCollector);

		ExecutionEntity scopeExecution = null;

		// if no explicit ancestor activity instance is set
		if (string.ReferenceEquals(ancestorActivityInstanceId, null))
		{
		  // walk until a scope is reached for which executions exist
		  walker.walkWhile(new WalkConditionAnonymousInnerClass(this, processDefinition, mapping));

		  ISet<ExecutionEntity> flowScopeExecutions = mapping.getExecutions(walker.CurrentElement);

		  if (flowScopeExecutions.Count > 1)
		  {
			throw new ProcessEngineException("Ancestor activity execution is ambiguous for activity " + targetFlowScope);
		  }

		  scopeExecution = flowScopeExecutions.GetEnumerator().next();
		}
		else
		{
		  ActivityInstance tree = commandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this, commandContext));

		  ActivityInstance ancestorInstance = findActivityInstance(tree, ancestorActivityInstanceId);
		  EnsureUtil.ensureNotNull(typeof(NotValidException), describeFailure("Ancestor activity instance '" + ancestorActivityInstanceId + "' does not exist"), "ancestorInstance", ancestorInstance);

		  // determine ancestor activity scope execution and activity
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity ancestorScopeExecution = getScopeExecutionForActivityInstance(processInstance, mapping, ancestorInstance);
		  ExecutionEntity ancestorScopeExecution = getScopeExecutionForActivityInstance(processInstance, mapping, ancestorInstance);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.pvm.PvmScope ancestorScope = getScopeForActivityInstance(processDefinition, ancestorInstance);
		  PvmScope ancestorScope = getScopeForActivityInstance(processDefinition, ancestorInstance);

		  // walk until the scope of the ancestor scope execution is reached
		  walker.walkWhile(new WalkConditionAnonymousInnerClass2(this, processDefinition, mapping, ancestorScopeExecution, ancestorScope));

		  ISet<ExecutionEntity> flowScopeExecutions = mapping.getExecutions(walker.CurrentElement);

		  if (!flowScopeExecutions.Contains(ancestorScopeExecution))
		  {
			throw new NotValidException(describeFailure("Scope execution for '" + ancestorActivityInstanceId + "' cannot be found in parent hierarchy of flow element '" + elementToInstantiate.Id + "'"));
		  }

		  scopeExecution = ancestorScopeExecution;
		}

		IList<PvmActivity> activitiesToInstantiate = stackCollector.ActivityStack;
		activitiesToInstantiate.Reverse();

		// We have to make a distinction between
		// - "regular" activities for which the activity stack can be instantiated and started
		//   right away
		// - interrupting or cancelling activities for which we have to ensure that
		//   the interruption and cancellation takes place before we instantiate the activity stack
		ActivityImpl topMostActivity = null;
		ScopeImpl flowScope = null;
		if (activitiesToInstantiate.Count > 0)
		{
		  topMostActivity = (ActivityImpl) activitiesToInstantiate[0];
		  flowScope = topMostActivity.FlowScope;
		}
		else if (elementToInstantiate.GetType().IsAssignableFrom(typeof(ActivityImpl)))
		{
		  topMostActivity = (ActivityImpl) elementToInstantiate;
		  flowScope = topMostActivity.FlowScope;
		}
		else if (elementToInstantiate.GetType().IsAssignableFrom(typeof(TransitionImpl)))
		{
		  TransitionImpl transitionToInstantiate = (TransitionImpl) elementToInstantiate;
		  flowScope = transitionToInstantiate.Source.FlowScope;
		}

		if (!supportsConcurrentChildInstantiation(flowScope))
		{
		  throw new ProcessEngineException("Concurrent instantiation not possible for " + "activities in scope " + flowScope.Id);
		}

		ActivityStartBehavior startBehavior = ActivityStartBehavior.CONCURRENT_IN_FLOW_SCOPE;
		if (topMostActivity != null)
		{
		  startBehavior = topMostActivity.ActivityStartBehavior;

		  if (activitiesToInstantiate.Count > 0)
		  {
			// this is in BPMN relevant if there is an interrupting event sub process.
			// we have to distinguish between instantiation of the start event and any other activity.
			// instantiation of the start event means interrupting behavior; instantiation
			// of any other task means no interruption.
			PvmActivity initialActivity = topMostActivity.Properties.get(BpmnProperties.INITIAL_ACTIVITY);
			PvmActivity secondTopMostActivity = null;
			if (activitiesToInstantiate.Count > 1)
			{
			  secondTopMostActivity = activitiesToInstantiate[1];
			}
			else if (elementToInstantiate.GetType().IsAssignableFrom(typeof(ActivityImpl)))
			{
			  secondTopMostActivity = (PvmActivity) elementToInstantiate;
			}

			if (initialActivity != secondTopMostActivity)
			{
			  startBehavior = ActivityStartBehavior.CONCURRENT_IN_FLOW_SCOPE;
			}
		  }
		}

		switch (startBehavior)
		{
		  case ActivityStartBehavior.CANCEL_EVENT_SCOPE:
		  {
			  ScopeImpl scopeToCancel = topMostActivity.EventScope;
			  ExecutionEntity executionToCancel = getSingleExecutionForScope(mapping, scopeToCancel);
			  if (executionToCancel != null)
			  {
				executionToCancel.deleteCascade("Cancelling activity " + topMostActivity + " executed.", skipCustomListeners, skipIoMappings);
				instantiate(executionToCancel.Parent, activitiesToInstantiate, elementToInstantiate);
			  }
			  else
			  {
				ExecutionEntity flowScopeExecution = getSingleExecutionForScope(mapping, topMostActivity.FlowScope);
				instantiateConcurrent(flowScopeExecution, activitiesToInstantiate, elementToInstantiate);
			  }
			  break;
		  }
		  case ActivityStartBehavior.INTERRUPT_EVENT_SCOPE:
		  {
			  ScopeImpl scopeToCancel = topMostActivity.EventScope;
			  ExecutionEntity executionToCancel = getSingleExecutionForScope(mapping, scopeToCancel);
			  executionToCancel.interrupt("Interrupting activity " + topMostActivity + " executed.", skipCustomListeners, skipIoMappings);
			  executionToCancel.setActivity(null);
			  executionToCancel.leaveActivityInstance();
			  instantiate(executionToCancel, activitiesToInstantiate, elementToInstantiate);
			  break;
		  }
		  case ActivityStartBehavior.INTERRUPT_FLOW_SCOPE:
		  {
			  ScopeImpl scopeToCancel = topMostActivity.FlowScope;
			  ExecutionEntity executionToCancel = getSingleExecutionForScope(mapping, scopeToCancel);
			  executionToCancel.interrupt("Interrupting activity " + topMostActivity + " executed.", skipCustomListeners, skipIoMappings);
			  executionToCancel.setActivity(null);
			  executionToCancel.leaveActivityInstance();
			  instantiate(executionToCancel, activitiesToInstantiate, elementToInstantiate);
			  break;
		  }
		  default:
		  {
			  // if all child executions have been cancelled
			  // or this execution has ended executing its scope, it can be reused
			  if (!scopeExecution.hasChildren() && (scopeExecution.getActivity() == null || scopeExecution.Ended))
			  {
				// reuse the scope execution
				instantiate(scopeExecution, activitiesToInstantiate, elementToInstantiate);
			  }
			  else
			  {
				// if the activity is not cancelling/interrupting, it can simply be instantiated as
				// a concurrent child of the scopeExecution
				instantiateConcurrent(scopeExecution, activitiesToInstantiate, elementToInstantiate);
			  }
			  break;
		  }
		}

		return null;
	  }

	  private class WalkConditionAnonymousInnerClass : ReferenceWalker.WalkCondition<ScopeImpl>
	  {
		  private readonly AbstractInstantiationCmd outerInstance;

		  private ProcessDefinitionImpl processDefinition;
		  private ActivityExecutionTreeMapping mapping;

		  public WalkConditionAnonymousInnerClass(AbstractInstantiationCmd outerInstance, ProcessDefinitionImpl processDefinition, ActivityExecutionTreeMapping mapping)
		  {
			  this.outerInstance = outerInstance;
			  this.processDefinition = processDefinition;
			  this.mapping = mapping;
		  }

		  public bool isFulfilled(ScopeImpl element)
		  {
			return mapping.getExecutions(element).Count > 0 || element == processDefinition;
		  }
	  }

	  private class CallableAnonymousInnerClass : Callable<ActivityInstance>
	  {
		  private readonly AbstractInstantiationCmd outerInstance;

		  private CommandContext commandContext;

		  public CallableAnonymousInnerClass(AbstractInstantiationCmd outerInstance, CommandContext commandContext)
		  {
			  this.outerInstance = outerInstance;
			  this.commandContext = commandContext;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.runtime.ActivityInstance call() throws Exception
		  public ActivityInstance call()
		  {
			return (new GetActivityInstanceCmd(outerInstance.processInstanceId)).execute(commandContext);
		  }
	  }

	  private class WalkConditionAnonymousInnerClass2 : ReferenceWalker.WalkCondition<ScopeImpl>
	  {
		  private readonly AbstractInstantiationCmd outerInstance;

		  private ProcessDefinitionImpl processDefinition;
		  private ActivityExecutionTreeMapping mapping;
		  private ExecutionEntity ancestorScopeExecution;
		  private PvmScope ancestorScope;

		  public WalkConditionAnonymousInnerClass2(AbstractInstantiationCmd outerInstance, ProcessDefinitionImpl processDefinition, ActivityExecutionTreeMapping mapping, ExecutionEntity ancestorScopeExecution, PvmScope ancestorScope)
		  {
			  this.outerInstance = outerInstance;
			  this.processDefinition = processDefinition;
			  this.mapping = mapping;
			  this.ancestorScopeExecution = ancestorScopeExecution;
			  this.ancestorScope = ancestorScope;
		  }

		  public bool isFulfilled(ScopeImpl element)
		  {
			return (mapping.getExecutions(element).Contains(ancestorScopeExecution) && element == ancestorScope) || element == processDefinition;
		  }
	  }

	  /// <summary>
	  /// Cannot create more than inner instance in a sequential MI construct
	  /// </summary>
	  protected internal virtual bool supportsConcurrentChildInstantiation(ScopeImpl flowScope)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.bpm.engine.impl.core.delegate.CoreActivityBehavior<?> behavior = flowScope.getActivityBehavior();
		CoreActivityBehavior<object> behavior = flowScope.ActivityBehavior;
		return behavior == null || !(behavior is SequentialMultiInstanceActivityBehavior);
	  }

	  protected internal virtual ExecutionEntity getSingleExecutionForScope(ActivityExecutionTreeMapping mapping, ScopeImpl scope)
	  {
		ISet<ExecutionEntity> executions = mapping.getExecutions(scope);

		if (executions.Count > 0)
		{
		  if (executions.Count > 1)
		  {
			throw new ProcessEngineException("Executions for activity " + scope + " ambiguous");
		  }

		  return executions.GetEnumerator().next();
		}
		else
		{
		  return null;
		}
	  }

	  protected internal virtual bool isConcurrentStart(ActivityStartBehavior startBehavior)
	  {
		return startBehavior == ActivityStartBehavior.DEFAULT || startBehavior == ActivityStartBehavior.CONCURRENT_IN_FLOW_SCOPE;
	  }

	  protected internal virtual void instantiate(ExecutionEntity ancestorScopeExecution, IList<PvmActivity> parentFlowScopes, CoreModelElement targetElement)
	  {
		if (targetElement.GetType().IsAssignableFrom(typeof(PvmTransition)))
		{
		  ancestorScopeExecution.executeActivities(parentFlowScopes, null, (PvmTransition) targetElement, variables, variablesLocal, skipCustomListeners, skipIoMappings);
		}
		else if (targetElement.GetType().IsAssignableFrom(typeof(PvmActivity)))
		{
		  ancestorScopeExecution.executeActivities(parentFlowScopes, (PvmActivity) targetElement, null, variables, variablesLocal, skipCustomListeners, skipIoMappings);

		}
		else
		{
		  throw new ProcessEngineException("Cannot instantiate element " + targetElement);
		}
	  }


	  protected internal virtual void instantiateConcurrent(ExecutionEntity ancestorScopeExecution, IList<PvmActivity> parentFlowScopes, CoreModelElement targetElement)
	  {
		if (targetElement.GetType().IsAssignableFrom(typeof(PvmTransition)))
		{
		  ancestorScopeExecution.executeActivitiesConcurrent(parentFlowScopes, null, (PvmTransition) targetElement, variables, variablesLocal, skipCustomListeners, skipIoMappings);
		}
		else if (targetElement.GetType().IsAssignableFrom(typeof(PvmActivity)))
		{
		  ancestorScopeExecution.executeActivitiesConcurrent(parentFlowScopes, (PvmActivity) targetElement, null, variables, variablesLocal, skipCustomListeners, skipIoMappings);

		}
		else
		{
		  throw new ProcessEngineException("Cannot instantiate element " + targetElement);
		}
	  }

	  protected internal abstract ScopeImpl getTargetFlowScope(ProcessDefinitionImpl processDefinition);

	  protected internal abstract CoreModelElement getTargetElement(ProcessDefinitionImpl processDefinition);

	  protected internal abstract string TargetElementId {get;}

	}

}