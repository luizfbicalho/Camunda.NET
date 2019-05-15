﻿/*
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
namespace org.camunda.bpm.engine.impl.jobexecutor
{
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;

	/// <summary>
	/// <seealso cref="JobHandler"/> implementation for timer start events which are embedded into an event subprocess.
	/// 
	/// The configuration is the id of the start event activity.
	/// 
	/// @author Daniel Meyer
	/// @author Kristin Polenz
	/// 
	/// </summary>
	public class TimerStartEventSubprocessJobHandler : TimerEventJobHandler
	{

	  public const string TYPE = "timer-start-event-subprocess";

	  public override string Type
	  {
		  get
		  {
			return TYPE;
		  }
	  }

	  public virtual void execute(TimerJobConfiguration configuration, ExecutionEntity execution, CommandContext commandContext, string tenantId)
	  {
		string activityId = configuration.TimerElementKey;
		ActivityImpl eventSubprocessActivity = execution.getProcessDefinition().findActivity(activityId);

		if (eventSubprocessActivity != null)
		{
		  execution.executeEventHandlerActivity(eventSubprocessActivity);

		}
		else
		{
		  throw new ProcessEngineException("Error while triggering event subprocess using timer start event: cannot find activity with id '" + configuration + "'.");
		}

	  }

	}

}