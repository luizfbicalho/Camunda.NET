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
namespace org.camunda.bpm.engine.impl.core.model
{

	using BaseDelegateExecution = org.camunda.bpm.engine.@delegate.BaseDelegateExecution;
	using CoreActivityBehavior = org.camunda.bpm.engine.impl.core.@delegate.CoreActivityBehavior;
	using IoMapping = org.camunda.bpm.engine.impl.core.variable.mapping.IoMapping;

	/// <summary>
	/// @author Daniel Meyer
	/// @author Roman Smirnov
	/// @author Sebastian Menski
	/// 
	/// </summary>
	[Serializable]
	public abstract class CoreActivity : CoreModelElement
	{

	  private const long serialVersionUID = 1L;

	  protected internal IoMapping ioMapping;

	  public CoreActivity(string id) : base(id)
	  {
	  }

	  /// <summary>
	  /// searches for the activity recursively </summary>
	  public virtual CoreActivity findActivity(string activityId)
	  {
		CoreActivity localActivity = getChildActivity(activityId);
		if (localActivity != null)
		{
		  return localActivity;
		}
		foreach (CoreActivity activity in Activities)
		{
		  CoreActivity nestedActivity = activity.findActivity(activityId);
		  if (nestedActivity != null)
		  {
			return nestedActivity;
		  }
		}
		return null;
	  }

	  public virtual CoreActivity createActivity()
	  {
		return createActivity(null);
	  }

	  /// <summary>
	  /// searches for the activity locally </summary>
	  public abstract CoreActivity getChildActivity(string activityId);

	  public abstract CoreActivity createActivity(string activityId);

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public abstract java.util.List<? extends CoreActivity> getActivities();
	  public abstract IList<CoreActivity> Activities {get;}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public abstract org.camunda.bpm.engine.impl.core.delegate.CoreActivityBehavior<? extends org.camunda.bpm.engine.delegate.BaseDelegateExecution> getActivityBehavior();
	  public abstract CoreActivityBehavior<BaseDelegateExecution> ActivityBehavior {get;}

	  public virtual IoMapping IoMapping
	  {
		  get
		  {
			return ioMapping;
		  }
		  set
		  {
			this.ioMapping = value;
		  }
	  }


	  public override string ToString()
	  {
		return "Activity(" + id + ")";
	  }

	}

}