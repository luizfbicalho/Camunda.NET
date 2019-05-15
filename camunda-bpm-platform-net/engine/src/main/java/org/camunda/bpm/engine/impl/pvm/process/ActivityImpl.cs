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

	using BpmnProperties = org.camunda.bpm.engine.impl.bpmn.helper.BpmnProperties;
	using BpmnParse = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse;
	using ActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityBehavior;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// @author Christopher Zell
	/// </summary>
	[Serializable]
	public class ActivityImpl : ScopeImpl, PvmActivity, HasDIBounds
	{

	  private const long serialVersionUID = 1L;

	  protected internal IList<TransitionImpl> outgoingTransitions = new List<TransitionImpl>();
	  protected internal IDictionary<string, TransitionImpl> namedOutgoingTransitions = new Dictionary<string, TransitionImpl>();
	  protected internal IList<TransitionImpl> incomingTransitions = new List<TransitionImpl>();

	  /// <summary>
	  /// the inner behavior of an activity. For activities which are flow scopes,
	  /// this must be a CompositeActivityBehavior. 
	  /// </summary>
	  protected internal ActivityBehavior activityBehavior;

	  /// <summary>
	  /// The start behavior for this activity. </summary>
	  protected internal ActivityStartBehavior activityStartBehavior = ActivityStartBehavior.DEFAULT;

	  protected internal ScopeImpl eventScope;
	  protected internal ScopeImpl flowScope;

	  protected internal bool isScope = false;

	  protected internal bool isAsyncBefore;
	  protected internal bool isAsyncAfter;

	  public ActivityImpl(string id, ProcessDefinitionImpl processDefinition) : base(id, processDefinition)
	  {
	  }

	  public virtual TransitionImpl createOutgoingTransition()
	  {
		return createOutgoingTransition(null);
	  }

	  public virtual TransitionImpl createOutgoingTransition(string transitionId)
	  {
		TransitionImpl transition = new TransitionImpl(transitionId, processDefinition);
		transition.Source = this;
		outgoingTransitions.Add(transition);

		if (!string.ReferenceEquals(transitionId, null))
		{
		  if (namedOutgoingTransitions.ContainsKey(transitionId))
		  {
			throw new PvmException("activity '" + id + " has duplicate transition '" + transitionId + "'");
		  }
		  namedOutgoingTransitions[transitionId] = transition;
		}

		return transition;
	  }

	  public virtual TransitionImpl findOutgoingTransition(string transitionId)
	  {
		return namedOutgoingTransitions[transitionId];
	  }

	  public override string ToString()
	  {
		return "Activity(" + id + ")";
	  }

	  // restricted setters ///////////////////////////////////////////////////////

	  protected internal virtual IList<TransitionImpl> OutgoingTransitions
	  {
		  set
		  {
			this.outgoingTransitions = value;
		  }
		  get
		  {
			return (System.Collections.IList) outgoingTransitions;
		  }
	  }

	  protected internal virtual IList<TransitionImpl> IncomingTransitions
	  {
		  set
		  {
			this.incomingTransitions = value;
		  }
		  get
		  {
			return (System.Collections.IList) incomingTransitions;
		  }
	  }

	  // getters and setters //////////////////////////////////////////////////////


	  public override ActivityBehavior ActivityBehavior
	  {
		  get
		  {
			return activityBehavior;
		  }
		  set
		  {
			this.activityBehavior = value;
		  }
	  }


	  public virtual ActivityStartBehavior ActivityStartBehavior
	  {
		  get
		  {
			return activityStartBehavior;
		  }
		  set
		  {
			this.activityStartBehavior = value;
		  }
	  }



	  public override bool Scope
	  {
		  get
		  {
			return isScope;
		  }
		  set
		  {
			this.isScope = value;
		  }
	  }


	  public virtual bool AsyncBefore
	  {
		  get
		  {
			return isAsyncBefore;
		  }
		  set
		  {
			setAsyncBefore(value, true);
		  }
	  }


	  public virtual void setAsyncBefore(bool isAsyncBefore, bool exclusive)
	  {
		if (delegateAsyncBeforeUpdate != null)
		{
		  delegateAsyncBeforeUpdate.updateAsyncBefore(isAsyncBefore, exclusive);
		}
		this.isAsyncBefore = isAsyncBefore;
	  }

	  public virtual bool AsyncAfter
	  {
		  get
		  {
			return isAsyncAfter;
		  }
		  set
		  {
			setAsyncAfter(value, true);
		  }
	  }


	  public virtual void setAsyncAfter(bool isAsyncAfter, bool exclusive)
	  {
		if (delegateAsyncAfterUpdate != null)
		{
		  delegateAsyncAfterUpdate.updateAsyncAfter(isAsyncAfter, exclusive);
		}
		this.isAsyncAfter = isAsyncAfter;
	  }

	  public virtual string ActivityId
	  {
		  get
		  {
			return base.Id;
		  }
	  }

	  public override ScopeImpl FlowScope
	  {
		  get
		  {
			return flowScope;
		  }
	  }

	  public override ScopeImpl EventScope
	  {
		  get
		  {
			return eventScope;
		  }
		  set
		  {
			if (this.eventScope != null)
			{
			  this.eventScope.eventActivities.remove(this);
			}
    
			this.eventScope = value;
    
			if (value != null)
			{
			  this.eventScope.eventActivities.Add(this);
			}
		  }
	  }


	  public override PvmScope LevelOfSubprocessScope
	  {
		  get
		  {
			ScopeImpl levelOfSubprocessScope = FlowScope;
			while (!levelOfSubprocessScope.isSubProcessScope)
			{
			  // cast always possible since process definition is always a sub process scope
			  levelOfSubprocessScope = ((PvmActivity)levelOfSubprocessScope).FlowScope;
			}
			return levelOfSubprocessScope;
		  }
	  }

	  // Graphical information ///////////////////////////////////////////

	  protected internal int x = -1;
	  protected internal int y = -1;
	  protected internal int width = -1;
	  protected internal int height = -1;

	  public virtual int X
	  {
		  get
		  {
			return x;
		  }
		  set
		  {
			this.x = value;
		  }
	  }


	  public virtual int Y
	  {
		  get
		  {
			return y;
		  }
		  set
		  {
			this.y = value;
		  }
	  }


	  public virtual int Width
	  {
		  get
		  {
			return width;
		  }
		  set
		  {
			this.width = value;
		  }
	  }


	  public virtual int Height
	  {
		  get
		  {
			return height;
		  }
		  set
		  {
			this.height = value;
		  }
	  }


	  public virtual ActivityImpl ParentFlowScopeActivity
	  {
		  get
		  {
			ScopeImpl flowScope = FlowScope;
			if (flowScope != ProcessDefinition)
			{
			  return (ActivityImpl) flowScope;
			}
			else
			{
			  return null;
			}
		  }
	  }

	  /// <summary>
	  /// Indicates whether activity is for compensation.
	  /// </summary>
	  /// <returns> true if this activity is for compensation. </returns>
	  public virtual bool CompensationHandler
	  {
		  get
		  {
			bool? isForCompensation = (bool?) getProperty(BpmnParse.PROPERTYNAME_IS_FOR_COMPENSATION);
			return true.Equals(isForCompensation);
		  }
	  }

	  /// <summary>
	  /// Find the compensation handler of this activity.
	  /// </summary>
	  /// <returns> the compensation handler or <code>null</code>, if this activity has no compensation handler. </returns>
	  public virtual ActivityImpl findCompensationHandler()
	  {
		string compensationHandlerId = (string) getProperty(BpmnParse.PROPERTYNAME_COMPENSATION_HANDLER_ID);
		if (!string.ReferenceEquals(compensationHandlerId, null))
		{
		  return ProcessDefinition.findActivity(compensationHandlerId);
		}
		else
		{
		  return null;
		}
	  }

	  /// <summary>
	  /// Indicates whether activity is a multi instance activity.
	  /// </summary>
	  /// <returns> true if this activity is a multi instance activity. </returns>
	  public virtual bool MultiInstance
	  {
		  get
		  {
			bool? isMultiInstance = (bool?) getProperty(BpmnParse.PROPERTYNAME_IS_MULTI_INSTANCE);
			return true.Equals(isMultiInstance);
		  }
	  }

	  public virtual bool TriggeredByEvent
	  {
		  get
		  {
			bool? isTriggeredByEvent = Properties.get(BpmnProperties.TRIGGERED_BY_EVENT);
			return true.Equals(isTriggeredByEvent);
		  }
	  }

	  //============================================================================
	  //===============================DELEGATES====================================
	  //============================================================================
	  /// <summary>
	  /// The delegate for the async before attribute update.
	  /// </summary>
	  protected internal AsyncBeforeUpdate delegateAsyncBeforeUpdate;
	  /// <summary>
	  /// The delegate for the async after attribute update.
	  /// </summary>
	  protected internal AsyncAfterUpdate delegateAsyncAfterUpdate;

	  public virtual AsyncBeforeUpdate DelegateAsyncBeforeUpdate
	  {
		  get
		  {
			return delegateAsyncBeforeUpdate;
		  }
		  set
		  {
			this.delegateAsyncBeforeUpdate = value;
		  }
	  }


	  public virtual AsyncAfterUpdate DelegateAsyncAfterUpdate
	  {
		  get
		  {
			return delegateAsyncAfterUpdate;
		  }
		  set
		  {
			this.delegateAsyncAfterUpdate = value;
		  }
	  }


	  /// <summary>
	  /// Delegate interface for the asyncBefore property update.
	  /// </summary>
	  public interface AsyncBeforeUpdate
	  {
		/// <summary>
		/// Method which is called if the asyncBefore property should be updated.
		/// </summary>
		/// <param name="asyncBefore"> the new value for the asyncBefore flag </param>
		/// <param name="exclusive"> the exclusive flag </param>
		void updateAsyncBefore(bool asyncBefore, bool exclusive);
	  }

	  /// <summary>
	  /// Delegate interface for the asyncAfter property update
	  /// </summary>
	  public interface AsyncAfterUpdate
	  {

		/// <summary>
		/// Method which is called if the asyncAfter property should be updated.
		/// </summary>
		/// <param name="asyncAfter"> the new value for the asyncBefore flag </param>
		/// <param name="exclusive"> the exclusive flag </param>
		void updateAsyncAfter(bool asyncAfter, bool exclusive);
	  }
	}

}