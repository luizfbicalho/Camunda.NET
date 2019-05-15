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
	using TimerJobConfiguration = org.camunda.bpm.engine.impl.jobexecutor.TimerEventJobHandler.TimerJobConfiguration;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;


	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public abstract class TimerEventJobHandler : JobHandler<TimerJobConfiguration>
	{
		public abstract void onDelete(T configuration, JobEntity jobEntity);
		public abstract void execute(T configuration, org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity execution, org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext, string tenantId);
		public abstract string Type {get;}

	  public const string JOB_HANDLER_CONFIG_PROPERTY_DELIMITER = "$";
	  public const string JOB_HANDLER_CONFIG_PROPERTY_FOLLOW_UP_JOB_CREATED = "followUpJobCreated";

	  public virtual TimerJobConfiguration newConfiguration(string canonicalString)
	  {
		string[] configParts = canonicalString.Split("\\" + JOB_HANDLER_CONFIG_PROPERTY_DELIMITER, true);

		if (configParts.Length > 2)
		{
		  throw new ProcessEngineException("Illegal timer job handler configuration: '" + canonicalString + "': exprecting a one or two part configuration seperated by '" + JOB_HANDLER_CONFIG_PROPERTY_DELIMITER + "'.");
		}

		TimerJobConfiguration configuration = new TimerJobConfiguration();
		configuration.timerElementKey = configParts[0];

		if (configParts.Length == 2)
		{
		  configuration.followUpJobCreated = JOB_HANDLER_CONFIG_PROPERTY_FOLLOW_UP_JOB_CREATED.Equals(configParts[1]);
		}

		return configuration;
	  }

	  public class TimerJobConfiguration : JobHandlerConfiguration
	  {

		protected internal string timerElementKey;
		protected internal bool followUpJobCreated;

		public virtual string TimerElementKey
		{
			get
			{
			  return timerElementKey;
			}
			set
			{
			  this.timerElementKey = value;
			}
		}


		public virtual bool FollowUpJobCreated
		{
			get
			{
			  return followUpJobCreated;
			}
			set
			{
			  this.followUpJobCreated = value;
			}
		}


		public virtual string toCanonicalString()
		{
		  string canonicalString = timerElementKey;

		  if (followUpJobCreated)
		  {
			canonicalString += JOB_HANDLER_CONFIG_PROPERTY_DELIMITER + JOB_HANDLER_CONFIG_PROPERTY_FOLLOW_UP_JOB_CREATED;
		  }

		  return canonicalString;
		}

	  }

	  public virtual void onDelete(TimerJobConfiguration configuration, JobEntity jobEntity)
	  {
		// do nothing
	  }

	}

}