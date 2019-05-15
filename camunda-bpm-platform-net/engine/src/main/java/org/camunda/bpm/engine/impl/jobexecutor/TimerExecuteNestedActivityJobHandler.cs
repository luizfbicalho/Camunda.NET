using System;

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
namespace org.camunda.bpm.engine.impl.jobexecutor
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public class TimerExecuteNestedActivityJobHandler : TimerEventJobHandler
	{

	  public const string TYPE = "timer-transition";

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
		ActivityImpl activity = execution.getProcessDefinition().findActivity(activityId);

		ensureNotNull("Error while firing timer: boundary event activity " + configuration + " not found", "boundary event activity", activity);

		try
		{

		  execution.executeEventHandlerActivity(activity);

		}
		catch (Exception e)
		{
		  throw e;

		}
		catch (Exception e)
		{
		  throw new ProcessEngineException("exception during timer execution: " + e.Message, e);
		}
	  }
	}

}