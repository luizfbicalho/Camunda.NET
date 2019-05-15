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
namespace org.camunda.bpm.engine.impl.cmmn.model
{

	using VariableListener = org.camunda.bpm.engine.@delegate.VariableListener;
	using CmmnActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.CmmnActivityBehavior;
	using CoreActivity = org.camunda.bpm.engine.impl.core.model.CoreActivity;
	using CmmnElement = org.camunda.bpm.model.cmmn.instance.CmmnElement;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[Serializable]
	public class CmmnActivity : CoreActivity
	{

	  private const long serialVersionUID = 1L;

	  protected internal IList<CmmnActivity> activities = new List<CmmnActivity>();
	  protected internal IDictionary<string, CmmnActivity> namedActivities = new Dictionary<string, CmmnActivity>();

	  protected internal CmmnElement cmmnElement;

	  protected internal CmmnActivityBehavior activityBehavior;

	  protected internal CmmnCaseDefinition caseDefinition;

	  protected internal CmmnActivity parent;

	  protected internal IList<CmmnSentryDeclaration> sentries = new List<CmmnSentryDeclaration>();
	  protected internal IDictionary<string, CmmnSentryDeclaration> sentryMap = new Dictionary<string, CmmnSentryDeclaration>();

	  protected internal IList<CmmnSentryDeclaration> entryCriteria = new List<CmmnSentryDeclaration>();
	  protected internal IList<CmmnSentryDeclaration> exitCriteria = new List<CmmnSentryDeclaration>();

	  // eventName => activity id => variable listeners
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.Map<String, java.util.Map<String, java.util.List<org.camunda.bpm.engine.delegate.VariableListener<?>>>> resolvedVariableListeners;
	  protected internal IDictionary<string, IDictionary<string, IList<VariableListener<object>>>> resolvedVariableListeners;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.Map<String, java.util.Map<String, java.util.List<org.camunda.bpm.engine.delegate.VariableListener<?>>>> resolvedBuiltInVariableListeners;
	  protected internal IDictionary<string, IDictionary<string, IList<VariableListener<object>>>> resolvedBuiltInVariableListeners;

	  public CmmnActivity(string id, CmmnCaseDefinition caseDefinition) : base(id)
	  {
		this.caseDefinition = caseDefinition;
	  }

	  // create a new activity ///////////////////////////////////////

	  public override CmmnActivity createActivity(string activityId)
	  {
		CmmnActivity activity = new CmmnActivity(activityId, caseDefinition);
		if (!string.ReferenceEquals(activityId, null))
		{
		  namedActivities[activityId] = activity;
		}
		activity.Parent = this;
		activities.Add(activity);
		return activity;
	  }

	  // activities ////////////////////////////////////////////////

	  public override IList<CmmnActivity> Activities
	  {
		  get
		  {
			return activities;
		  }
	  }

	  public override CmmnActivity findActivity(string activityId)
	  {
		return (CmmnActivity) base.findActivity(activityId);
	  }

	  // child activity ////////////////////////////////////////////

	  public override CmmnActivity getChildActivity(string activityId)
	  {
		return namedActivities[activityId];
	  }

	  // behavior //////////////////////////////////////////////////

	  public override CmmnActivityBehavior ActivityBehavior
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


	  // parent ////////////////////////////////////////////////////

	  public virtual CmmnActivity Parent
	  {
		  get
		  {
			return this.parent;
		  }
		  set
		  {
			this.parent = value;
		  }
	  }


	  // case definition

	  public virtual CmmnCaseDefinition CaseDefinition
	  {
		  get
		  {
			return caseDefinition;
		  }
		  set
		  {
			this.caseDefinition = value;
		  }
	  }


	  // cmmn element

	  public virtual CmmnElement CmmnElement
	  {
		  get
		  {
			return cmmnElement;
		  }
		  set
		  {
			this.cmmnElement = value;
		  }
	  }


	  // sentry

	  public virtual IList<CmmnSentryDeclaration> Sentries
	  {
		  get
		  {
			return sentries;
		  }
	  }

	  public virtual CmmnSentryDeclaration getSentry(string sentryId)
	  {
		return sentryMap[sentryId];
	  }

	  public virtual void addSentry(CmmnSentryDeclaration sentry)
	  {
		sentryMap[sentry.Id] = sentry;
		sentries.Add(sentry);
	  }

	  // entryCriteria

	  public virtual IList<CmmnSentryDeclaration> EntryCriteria
	  {
		  get
		  {
			return entryCriteria;
		  }
		  set
		  {
			this.entryCriteria = value;
		  }
	  }


	  public virtual void addEntryCriteria(CmmnSentryDeclaration entryCriteria)
	  {
		this.entryCriteria.Add(entryCriteria);
	  }

	  // exitCriteria

	  public virtual IList<CmmnSentryDeclaration> ExitCriteria
	  {
		  get
		  {
			return exitCriteria;
		  }
		  set
		  {
			this.exitCriteria = value;
		  }
	  }


	  public virtual void addExitCriteria(CmmnSentryDeclaration exitCriteria)
	  {
		this.exitCriteria.Add(exitCriteria);
	  }

	  // variable listeners

	  /// <summary>
	  /// Returns a map of all variable listeners defined on this activity or any of
	  /// its parents activities. The map's key is the id of the respective activity
	  /// the listener is defined on.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public java.util.Map<String, java.util.List<org.camunda.bpm.engine.delegate.VariableListener<?>>> getVariableListeners(String eventName, boolean includeCustomListeners)
	  public virtual IDictionary<string, IList<VariableListener<object>>> getVariableListeners(string eventName, bool includeCustomListeners)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.Map<String, java.util.Map<String, java.util.List<org.camunda.bpm.engine.delegate.VariableListener<?>>>> listenerCache;
		IDictionary<string, IDictionary<string, IList<VariableListener<object>>>> listenerCache;
		if (includeCustomListeners)
		{
		  if (resolvedVariableListeners == null)
		  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: resolvedVariableListeners = new java.util.HashMap<String, java.util.Map<String,java.util.List<org.camunda.bpm.engine.delegate.VariableListener<?>>>>();
			resolvedVariableListeners = new Dictionary<string, IDictionary<string, IList<VariableListener<object>>>>();
		  }

		  listenerCache = resolvedVariableListeners;
		}
		else
		{
		  if (resolvedBuiltInVariableListeners == null)
		  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: resolvedBuiltInVariableListeners = new java.util.HashMap<String, java.util.Map<String,java.util.List<org.camunda.bpm.engine.delegate.VariableListener<?>>>>();
			resolvedBuiltInVariableListeners = new Dictionary<string, IDictionary<string, IList<VariableListener<object>>>>();
		  }
		  listenerCache = resolvedBuiltInVariableListeners;
		}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.Map<String, java.util.List<org.camunda.bpm.engine.delegate.VariableListener<?>>> resolvedListenersForEvent = listenerCache.get(eventName);
		IDictionary<string, IList<VariableListener<object>>> resolvedListenersForEvent = listenerCache[eventName];

		if (resolvedListenersForEvent == null)
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: resolvedListenersForEvent = new java.util.HashMap<String, java.util.List<org.camunda.bpm.engine.delegate.VariableListener<?>>>();
		  resolvedListenersForEvent = new Dictionary<string, IList<VariableListener<object>>>();
		  listenerCache[eventName] = resolvedListenersForEvent;

		  CmmnActivity currentActivity = this;

		  while (currentActivity != null)
		  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.engine.delegate.VariableListener<?>> localListeners = null;
			IList<VariableListener<object>> localListeners = null;
			if (includeCustomListeners)
			{
			  localListeners = currentActivity.getVariableListenersLocal(eventName);
			}
			else
			{
			  localListeners = currentActivity.getBuiltInVariableListenersLocal(eventName);
			}

			if (localListeners != null && localListeners.Count > 0)
			{
			  resolvedListenersForEvent[currentActivity.Id] = localListeners;
			}

			currentActivity = currentActivity.Parent;
		  }
		}

		return resolvedListenersForEvent;
	  }
	}

}