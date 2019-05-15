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
namespace org.camunda.bpm.engine.impl.pvm
{

	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using CoreModelElement = org.camunda.bpm.engine.impl.core.model.CoreModelElement;
	using ActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityBehavior;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ActivityStartBehavior = org.camunda.bpm.engine.impl.pvm.process.ActivityStartBehavior;
	using ProcessDefinitionImpl = org.camunda.bpm.engine.impl.pvm.process.ProcessDefinitionImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using TransitionImpl = org.camunda.bpm.engine.impl.pvm.process.TransitionImpl;



	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") public class ProcessDefinitionBuilder
	public class ProcessDefinitionBuilder
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			processElement = processDefinition;
		}


	  protected internal ProcessDefinitionImpl processDefinition;
	  protected internal Stack<ScopeImpl> scopeStack = new Stack<ScopeImpl>();
	  protected internal CoreModelElement processElement;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal TransitionImpl transition_Renamed;
	  protected internal IList<object[]> unresolvedTransitions = new List<object[]>();

	  public ProcessDefinitionBuilder() : this(null)
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
	  }

	  public ProcessDefinitionBuilder(string processDefinitionId)
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
		processDefinition = new ProcessDefinitionImpl(processDefinitionId);
		scopeStack.Push(processDefinition);
	  }

	  public virtual ProcessDefinitionBuilder createActivity(string id)
	  {
		ActivityImpl activity = scopeStack.Peek().createActivity(id);
		scopeStack.Push(activity);
		processElement = activity;

		transition_Renamed = null;

		return this;
	  }

	  public virtual ProcessDefinitionBuilder attachedTo(string id, bool isInterrupting)
	  {
		ActivityImpl activity = Activity;
		activity.EventScope = processDefinition.findActivity(id);

		if (isInterrupting)
		{
		  activity.ActivityStartBehavior = ActivityStartBehavior.INTERRUPT_EVENT_SCOPE;
		}
		else
		{
		  activity.ActivityStartBehavior = ActivityStartBehavior.CONCURRENT_IN_FLOW_SCOPE;
		}

		return this;
	  }


	  public virtual ProcessDefinitionBuilder endActivity()
	  {
		scopeStack.Pop();
		processElement = scopeStack.Peek();

		transition_Renamed = null;

		return this;
	  }

	  public virtual ProcessDefinitionBuilder initial()
	  {
		processDefinition.Initial = Activity;
		return this;
	  }

	  public virtual ProcessDefinitionBuilder startTransition(string destinationActivityId)
	  {
		return startTransition(destinationActivityId, null);
	  }

	  public virtual ProcessDefinitionBuilder startTransition(string destinationActivityId, string transitionId)
	  {
		if (string.ReferenceEquals(destinationActivityId, null))
		{
		  throw new PvmException("destinationActivityId is null");
		}
		ActivityImpl activity = Activity;
		transition_Renamed = activity.createOutgoingTransition(transitionId);
		unresolvedTransitions.Add(new object[]{transition_Renamed, destinationActivityId});
		processElement = transition_Renamed;
		return this;
	  }

	  public virtual ProcessDefinitionBuilder endTransition()
	  {
		processElement = scopeStack.Peek();
		transition_Renamed = null;
		return this;
	  }

	  public virtual ProcessDefinitionBuilder transition(string destinationActivityId)
	  {
		return transition(destinationActivityId, null);
	  }

	  public virtual ProcessDefinitionBuilder transition(string destinationActivityId, string transitionId)
	  {
		startTransition(destinationActivityId, transitionId);
		endTransition();
		return this;
	  }

	  public virtual ProcessDefinitionBuilder behavior(ActivityBehavior activityBehaviour)
	  {
		Activity.ActivityBehavior = activityBehaviour;
		return this;
	  }

	  public virtual ProcessDefinitionBuilder property(string name, object value)
	  {
		processElement.setProperty(name, value);
		return this;
	  }

	  public virtual PvmProcessDefinition buildProcessDefinition()
	  {
		foreach (object[] unresolvedTransition in unresolvedTransitions)
		{
		  TransitionImpl transition = (TransitionImpl) unresolvedTransition[0];
		  string destinationActivityName = (string) unresolvedTransition[1];
		  ActivityImpl destination = processDefinition.findActivity(destinationActivityName);
		  if (destination == null)
		  {
			throw new Exception("destination '" + destinationActivityName + "' not found.  (referenced from transition in '" + transition.Source.Id + "')");
		  }
		  transition.setDestination(destination);
		}
		return processDefinition;
	  }

	  protected internal virtual ActivityImpl Activity
	  {
		  get
		  {
			return (ActivityImpl) scopeStack.Peek();
		  }
	  }

	  public virtual ProcessDefinitionBuilder scope()
	  {
		Activity.Scope = true;
		return this;
	  }

	  public virtual ProcessDefinitionBuilder executionListener(ExecutionListener executionListener)
	  {
		if (transition_Renamed != null)
		{
		  transition_Renamed.addExecutionListener(executionListener);
		}
		else
		{
		  throw new PvmException("not in a transition scope");
		}
		return this;
	  }

	  public virtual ProcessDefinitionBuilder executionListener(string eventName, ExecutionListener executionListener)
	  {
		if (transition_Renamed == null)
		{
		  scopeStack.Peek().addExecutionListener(eventName, executionListener);
		}
		else
		{
		  transition_Renamed.addExecutionListener(executionListener);
		}
		return this;
	  }
	}

}