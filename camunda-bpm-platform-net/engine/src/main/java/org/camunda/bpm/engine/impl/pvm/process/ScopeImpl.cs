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
namespace org.camunda.bpm.engine.impl.pvm.process
{

	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using CoreActivity = org.camunda.bpm.engine.impl.core.model.CoreActivity;


	/// <summary>
	/// A Bpmn scope. The scope has references to two lists of activities:
	/// - the flow activities (activities for which the <seealso cref="ActivityImpl.getFlowScope() flow scope"/> is this scope
	/// - event listener activities (activities for which the <seealso cref="ActivityImpl.getEventScope() event scope"/> is this scope.
	/// 
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// </summary>
	[Serializable]
	public abstract class ScopeImpl : CoreActivity, PvmScope
	{
		public abstract PvmScope LevelOfSubprocessScope {get;}
		public abstract org.camunda.bpm.engine.impl.pvm.process.ScopeImpl FlowScope {get;}
		public abstract PvmScope EventScope {get;}
		public abstract bool Scope {get;}

	  private const long serialVersionUID = 1L;

	  protected internal bool isSubProcessScope = false;

	  /// <summary>
	  /// The activities for which the flow scope is this scope </summary>
	  protected internal IList<ActivityImpl> flowActivities = new List<ActivityImpl>();
	  protected internal IDictionary<string, ActivityImpl> namedFlowActivities = new Dictionary<string, ActivityImpl>();

	  /// <summary>
	  /// activities for which this is the event scope * </summary>
	  protected internal ISet<ActivityImpl> eventActivities = new HashSet<ActivityImpl>();

	  protected internal ProcessDefinitionImpl processDefinition;

	  public ScopeImpl(string id, ProcessDefinitionImpl processDefinition) : base(id)
	  {
		this.processDefinition = processDefinition;
	  }

	  public override ActivityImpl findActivity(string activityId)
	  {
		return (ActivityImpl) base.findActivity(activityId);
	  }

	  public virtual TransitionImpl findTransition(string transitionId)
	  {
		foreach (PvmActivity childActivity in flowActivities)
		{
		  foreach (PvmTransition transition in childActivity.OutgoingTransitions)
		  {
			if (transitionId.Equals(transition.Id))
			{
			  return (TransitionImpl) transition;
			}
		  }
		}

		foreach (ActivityImpl childActivity in flowActivities)
		{
		  TransitionImpl nestedTransition = childActivity.findTransition(transitionId);
		  if (nestedTransition != null)
		  {
			return nestedTransition;
		  }
		}

		return null;
	  }

	  public virtual ActivityImpl findActivityAtLevelOfSubprocess(string activityId)
	  {
		if (!SubProcessScope)
		{
		  throw new ProcessEngineException("This is not a sub process scope.");
		}
		ActivityImpl activity = findActivity(activityId);
		if (activity == null || activity.LevelOfSubprocessScope != this)
		{
		  return null;
		}
		else
		{
		  return activity;
		}
	  }

	  /// <summary>
	  /// searches for the activity locally </summary>
	  public override ActivityImpl getChildActivity(string activityId)
	  {
		return namedFlowActivities[activityId];
	  }


	  /// <summary>
	  /// Represents the backlog error callback interface.
	  /// Contains a callback method, which is called if the activity in the backlog
	  /// is not read till the end of parsing.
	  /// </summary>
	  public interface BacklogErrorCallback
	  {
		/// <summary>
		/// In error case the callback will called.
		/// </summary>
		void callback();
	  }

	  /// <summary>
	  /// The key identifies the activity which is referenced but not read yet.
	  /// The value is the error callback, which is called if the activity is not
	  /// read till the end of parsing.
	  /// </summary>
	  protected internal readonly IDictionary<string, BacklogErrorCallback> BACKLOG = new Dictionary<string, BacklogErrorCallback>();

	  /// <summary>
	  /// Returns the backlog error callback's.
	  /// </summary>
	  /// <returns> the callback's </returns>
	  public virtual ICollection<BacklogErrorCallback> BacklogErrorCallbacks
	  {
		  get
		  {
			return BACKLOG.Values;
		  }
	  }

	  /// <summary>
	  /// Returns true if the backlog is empty.
	  /// </summary>
	  /// <returns> true if empty, false otherwise </returns>
	  public virtual bool BacklogEmpty
	  {
		  get
		  {
			return BACKLOG.Count == 0;
		  }
	  }

	  /// <summary>
	  /// Add's the given activity reference and the error callback to the backlog.
	  /// </summary>
	  /// <param name="activityRef"> the activity reference which is not read until now </param>
	  /// <param name="callback"> the error callback which should called if activity will not be read </param>
	  public virtual void addToBacklog(string activityRef, BacklogErrorCallback callback)
	  {
		BACKLOG[activityRef] = callback;
	  }

	  public override ActivityImpl createActivity(string activityId)
	  {
		ActivityImpl activity = new ActivityImpl(activityId, processDefinition);
		if (!string.ReferenceEquals(activityId, null))
		{
		  if (processDefinition.findActivity(activityId) != null)
		  {
			throw new PvmException("duplicate activity id '" + activityId + "'");
		  }
		  if (BACKLOG.ContainsKey(activityId))
		  {
			BACKLOG.Remove(activityId);
		  }
		  namedFlowActivities[activityId] = activity;
		}
		activity.flowScope = this;
		flowActivities.Add(activity);

		return activity;
	  }

	  public virtual bool isAncestorFlowScopeOf(ScopeImpl other)
	  {
		ScopeImpl otherAncestor = other.FlowScope;
		while (otherAncestor != null)
		{
		  if (this == otherAncestor)
		  {
			return true;
		  }
		  else
		  {
			otherAncestor = otherAncestor.FlowScope;
		  }
		}

		return false;
	  }

	  public virtual bool contains(ActivityImpl activity)
	  {
		if (namedFlowActivities.ContainsKey(activity.Id))
		{
		  return true;
		}
		foreach (ActivityImpl nestedActivity in flowActivities)
		{
		  if (nestedActivity.contains(activity))
		  {
			return true;
		  }
		}
		return false;
	  }

	  // event listeners //////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Deprecated public java.util.List<org.camunda.bpm.engine.delegate.ExecutionListener> getExecutionListeners(String eventName)
	  [Obsolete]
	  public virtual IList<ExecutionListener> getExecutionListeners(string eventName)
	  {
		return (System.Collections.IList) base.getListeners(eventName);
	  }

	  [Obsolete]
	  public virtual void addExecutionListener(string eventName, ExecutionListener executionListener)
	  {
		base.addListener(eventName, executionListener);
	  }

	  [Obsolete]
	  public virtual void addExecutionListener(string eventName, ExecutionListener executionListener, int index)
	  {
		base.addListener(eventName, executionListener, index);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) @Deprecated public java.util.Map<String, java.util.List<org.camunda.bpm.engine.delegate.ExecutionListener>> getExecutionListeners()
	  [Obsolete]
	  public virtual IDictionary<string, IList<ExecutionListener>> ExecutionListeners
	  {
		  get
		  {
			return (System.Collections.IDictionary) base.Listeners;
		  }
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public override IList<ActivityImpl> Activities
	  {
		  get
		  {
			return flowActivities;
		  }
	  }

	  public virtual ISet<ActivityImpl> EventActivities
	  {
		  get
		  {
			return eventActivities;
		  }
	  }

	  public virtual bool SubProcessScope
	  {
		  get
		  {
			return isSubProcessScope;
		  }
		  set
		  {
			this.isSubProcessScope = value;
		  }
	  }


	  public virtual ProcessDefinitionImpl ProcessDefinition
	  {
		  get
		  {
			return processDefinition;
		  }
	  }

	}

}