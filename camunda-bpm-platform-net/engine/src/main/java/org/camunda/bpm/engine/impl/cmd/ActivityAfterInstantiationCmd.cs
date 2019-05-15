﻿using System.Text;

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
	using CoreModelElement = org.camunda.bpm.engine.impl.core.model.CoreModelElement;
	using PvmActivity = org.camunda.bpm.engine.impl.pvm.PvmActivity;
	using ProcessDefinitionImpl = org.camunda.bpm.engine.impl.pvm.process.ProcessDefinitionImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using TransitionImpl = org.camunda.bpm.engine.impl.pvm.process.TransitionImpl;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ActivityAfterInstantiationCmd : AbstractInstantiationCmd
	{

	  protected internal string activityId;

	  public ActivityAfterInstantiationCmd(string activityId) : this(null, activityId)
	  {
	  }

	  public ActivityAfterInstantiationCmd(string processInstanceId, string activityId) : this(processInstanceId, activityId, null)
	  {
	  }

	  public ActivityAfterInstantiationCmd(string processInstanceId, string activityId, string ancestorActivityInstanceId) : base(processInstanceId, ancestorActivityInstanceId)
	  {
		this.activityId = activityId;
	  }

	  protected internal override ScopeImpl getTargetFlowScope(ProcessDefinitionImpl processDefinition)
	  {
		TransitionImpl transition = findTransition(processDefinition);

		return transition.getDestination().FlowScope;
	  }

	  protected internal override CoreModelElement getTargetElement(ProcessDefinitionImpl processDefinition)
	  {
		return findTransition(processDefinition);
	  }

	  protected internal virtual TransitionImpl findTransition(ProcessDefinitionImpl processDefinition)
	  {
		PvmActivity activity = processDefinition.findActivity(activityId);

		EnsureUtil.ensureNotNull(typeof(NotValidException), describeFailure("Activity '" + activityId + "' does not exist"), "activity", activity);

		if (activity.OutgoingTransitions.Count == 0)
		{
		  throw new ProcessEngineException("Cannot start after activity " + activityId + "; activity " + "has no outgoing sequence flow to take");
		}
		else if (activity.OutgoingTransitions.Count > 1)
		{
		  throw new ProcessEngineException("Cannot start after activity " + activityId + "; " + "activity has more than one outgoing sequence flow");
		}

		return (TransitionImpl) activity.OutgoingTransitions[0];
	  }

	  public override string TargetElementId
	  {
		  get
		  {
			return activityId;
		  }
	  }

	  protected internal override string describe()
	  {
		StringBuilder sb = new StringBuilder();
		sb.Append("Start after activity '");
		sb.Append(activityId);
		sb.Append("'");
		if (!string.ReferenceEquals(ancestorActivityInstanceId, null))
		{
		  sb.Append(" with ancestor activity instance '");
		  sb.Append(ancestorActivityInstanceId);
		  sb.Append("'");
		}

		return sb.ToString();
	  }
	}

}