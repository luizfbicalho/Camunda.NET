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
namespace org.camunda.bpm.engine.impl.persistence.entity
{

	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using TransitionInstance = org.camunda.bpm.engine.runtime.TransitionInstance;

	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ActivityInstanceImpl : ProcessElementInstanceImpl, ActivityInstance
	{

	  protected internal string businessKey;
	  protected internal string activityId;
	  protected internal string activityName;
	  protected internal string activityType;

	  protected internal ActivityInstance[] childActivityInstances = new ActivityInstance[0];
	  protected internal TransitionInstance[] childTransitionInstances = new TransitionInstance[0];

	  protected internal string[] executionIds = new string[0];

	  public virtual ActivityInstance[] ChildActivityInstances
	  {
		  get
		  {
			return childActivityInstances;
		  }
		  set
		  {
			this.childActivityInstances = value;
		  }
	  }


	  public virtual string BusinessKey
	  {
		  get
		  {
			return businessKey;
		  }
		  set
		  {
			this.businessKey = value;
		  }
	  }


	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId;
		  }
		  set
		  {
			this.activityId = value;
		  }
	  }


	  public virtual string[] ExecutionIds
	  {
		  get
		  {
			return executionIds;
		  }
		  set
		  {
			this.executionIds = value;
		  }
	  }


	  public virtual TransitionInstance[] ChildTransitionInstances
	  {
		  get
		  {
			return childTransitionInstances;
		  }
		  set
		  {
			this.childTransitionInstances = value;
		  }
	  }


	  public virtual string ActivityType
	  {
		  get
		  {
			return activityType;
		  }
		  set
		  {
			this.activityType = value;
		  }
	  }


	  public virtual string ActivityName
	  {
		  get
		  {
			return activityName;
		  }
		  set
		  {
			this.activityName = value;
		  }
	  }


	  protected internal virtual void writeTree(StringWriter writer, string prefix, bool isTail)
	  {
		writer.append(prefix);
		if (isTail)
		{
		  writer.append("└── ");
		}
		else
		{
		  writer.append("├── ");
		}

		writer.append(ActivityId + "=>" + Id + "\n");

		for (int i = 0; i < childTransitionInstances.Length; i++)
		{
		  TransitionInstance transitionInstance = childTransitionInstances[i];
		  bool transitionIsTail = (i == (childTransitionInstances.Length - 1)) && (childActivityInstances.Length == 0);
		  writeTransition(transitionInstance, writer, prefix + (isTail ? "    " : "│   "), transitionIsTail);
		}

		for (int i = 0; i < childActivityInstances.Length; i++)
		{
		  ActivityInstanceImpl child = (ActivityInstanceImpl) childActivityInstances[i];
		  child.writeTree(writer, prefix + (isTail ? "    " : "│   "), (i == (childActivityInstances.Length - 1)));
		}
	  }

	  protected internal virtual void writeTransition(TransitionInstance transition, StringWriter writer, string prefix, bool isTail)
	  {
		writer.append(prefix);
		if (isTail)
		{
		  writer.append("└── ");
		}
		else
		{
		  writer.append("├── ");
		}

		writer.append("transition to/from " + transition.ActivityId + ":" + transition.Id + "\n");
	  }

	  public override string ToString()
	  {
		StringWriter writer = new StringWriter();
		writeTree(writer, "", true);
		return writer.ToString();
	  }

	  public virtual ActivityInstance[] getActivityInstances(string activityId)
	  {
		EnsureUtil.ensureNotNull("activityId", activityId);

		IList<ActivityInstance> instances = new List<ActivityInstance>();
		collectActivityInstances(activityId, instances);

		return instances.ToArray();
	  }

	  protected internal virtual void collectActivityInstances(string activityId, IList<ActivityInstance> instances)
	  {
		if (this.activityId.Equals(activityId))
		{
		  instances.Add(this);
		}
		else
		{
		  foreach (ActivityInstance childInstance in childActivityInstances)
		  {
			((ActivityInstanceImpl) childInstance).collectActivityInstances(activityId, instances);
		  }
		}
	  }

	  public virtual TransitionInstance[] getTransitionInstances(string activityId)
	  {
		EnsureUtil.ensureNotNull("activityId", activityId);

		IList<TransitionInstance> instances = new List<TransitionInstance>();
		collectTransitionInstances(activityId, instances);

		return instances.ToArray();
	  }

	  protected internal virtual void collectTransitionInstances(string activityId, IList<TransitionInstance> instances)
	  {
		bool instanceFound = false;

		foreach (TransitionInstance childTransitionInstance in childTransitionInstances)
		{
		  if (activityId.Equals(childTransitionInstance.ActivityId))
		  {
			instances.Add(childTransitionInstance);
			instanceFound = true;
		  }
		}

		if (!instanceFound)
		{
		  foreach (ActivityInstance childActivityInstance in childActivityInstances)
		  {
			((ActivityInstanceImpl) childActivityInstance).collectTransitionInstances(activityId, instances);
		  }
		}
	  }

	}

}