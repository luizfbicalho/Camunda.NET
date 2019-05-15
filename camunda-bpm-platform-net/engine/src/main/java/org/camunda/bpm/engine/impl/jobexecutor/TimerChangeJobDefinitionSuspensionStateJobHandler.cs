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
	using AbstractSetJobDefinitionStateCmd = org.camunda.bpm.engine.impl.cmd.AbstractSetJobDefinitionStateCmd;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobDefinitionSuspensionStateConfiguration = org.camunda.bpm.engine.impl.jobexecutor.TimerChangeJobDefinitionSuspensionStateJobHandler.JobDefinitionSuspensionStateConfiguration;
	using UpdateJobDefinitionSuspensionStateBuilderImpl = org.camunda.bpm.engine.impl.management.UpdateJobDefinitionSuspensionStateBuilderImpl;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using JsonUtil = org.camunda.bpm.engine.impl.util.JsonUtil;
	using JsonObject = com.google.gson.JsonObject;

	/// <summary>
	/// @author roman.smirnov
	/// </summary>
	public abstract class TimerChangeJobDefinitionSuspensionStateJobHandler : JobHandler<JobDefinitionSuspensionStateConfiguration>
	{
		public abstract void onDelete(T configuration, JobEntity jobEntity);
		public abstract void execute(T configuration, ExecutionEntity execution, CommandContext commandContext, string tenantId);
		public abstract string Type {get;}

	  protected internal const string JOB_HANDLER_CFG_BY = "by";
	  protected internal const string JOB_HANDLER_CFG_JOB_DEFINITION_ID = "jobDefinitionId";
	  protected internal const string JOB_HANDLER_CFG_PROCESS_DEFINITION_ID = "processDefinitionId";
	  protected internal const string JOB_HANDLER_CFG_PROCESS_DEFINITION_KEY = "processDefinitionKey";
	  protected internal const string JOB_HANDLER_CFG_PROCESS_DEFINITION_TENANT_ID = "processDefinitionTenantId";

	  protected internal const string JOB_HANDLER_CFG_INCLUDE_JOBS = "includeJobs";

	  public virtual void execute(JobDefinitionSuspensionStateConfiguration configuration, ExecutionEntity execution, CommandContext commandContext, string tenantId)
	  {
		AbstractSetJobDefinitionStateCmd cmd = getCommand(configuration);
		cmd.disableLogUserOperation();
		cmd.execute(commandContext);
	  }

	  protected internal abstract AbstractSetJobDefinitionStateCmd getCommand(JobDefinitionSuspensionStateConfiguration configuration);

	  public virtual JobDefinitionSuspensionStateConfiguration newConfiguration(string canonicalString)
	  {
		JsonObject jsonObject = JsonUtil.asObject(canonicalString);

		return JobDefinitionSuspensionStateConfiguration.fromJson(jsonObject);
	  }

	  public class JobDefinitionSuspensionStateConfiguration : JobHandlerConfiguration
	  {

		protected internal string jobDefinitionId;
		protected internal string processDefinitionKey;
		protected internal string processDefinitionId;
		protected internal bool includeJobs;
		protected internal string tenantId;
		protected internal bool isTenantIdSet;
		protected internal string by;

		public virtual string toCanonicalString()
		{
		  JsonObject json = JsonUtil.createObject();

		  JsonUtil.addField(json, JOB_HANDLER_CFG_BY, by);
		  JsonUtil.addField(json, JOB_HANDLER_CFG_JOB_DEFINITION_ID, jobDefinitionId);
		  JsonUtil.addField(json, JOB_HANDLER_CFG_PROCESS_DEFINITION_KEY, processDefinitionKey);
		  JsonUtil.addField(json, JOB_HANDLER_CFG_INCLUDE_JOBS, includeJobs);
		  JsonUtil.addField(json, JOB_HANDLER_CFG_PROCESS_DEFINITION_ID, processDefinitionId);

		  if (isTenantIdSet)
		  {
			if (!string.ReferenceEquals(tenantId, null))
			{
			  JsonUtil.addField(json, JOB_HANDLER_CFG_PROCESS_DEFINITION_TENANT_ID, tenantId);
			}
			else
			{
			  JsonUtil.addNullField(json, JOB_HANDLER_CFG_PROCESS_DEFINITION_TENANT_ID);
			}
		  }

		  return json.ToString();
		}

		public virtual UpdateJobDefinitionSuspensionStateBuilderImpl createBuilder()
		{
		  UpdateJobDefinitionSuspensionStateBuilderImpl builder = new UpdateJobDefinitionSuspensionStateBuilderImpl();

		  if (JOB_HANDLER_CFG_PROCESS_DEFINITION_ID.Equals(by))
		  {
			builder.byProcessDefinitionId(processDefinitionId);

		  }
		  else if (JOB_HANDLER_CFG_JOB_DEFINITION_ID.Equals(by))
		  {
			builder.byJobDefinitionId(jobDefinitionId);
		  }
		  else if (JOB_HANDLER_CFG_PROCESS_DEFINITION_KEY.Equals(by))
		  {
			builder.byProcessDefinitionKey(processDefinitionKey);

			if (isTenantIdSet)
			{

			  if (!string.ReferenceEquals(tenantId, null))
			  {
				builder.processDefinitionTenantId(tenantId);

			  }
			  else
			  {
				builder.processDefinitionWithoutTenantId();
			  }
			}

		  }
		  else
		  {
			throw new ProcessEngineException("Unexpected job handler configuration for property '" + JOB_HANDLER_CFG_BY + "': " + by);
		  }

		  builder.includeJobs(includeJobs);

		  return builder;
		}

		public static JobDefinitionSuspensionStateConfiguration fromJson(JsonObject jsonObject)
		{
		  JobDefinitionSuspensionStateConfiguration config = new JobDefinitionSuspensionStateConfiguration();

		  config.by = JsonUtil.getString(jsonObject, JOB_HANDLER_CFG_BY);
		  if (jsonObject.has(JOB_HANDLER_CFG_JOB_DEFINITION_ID))
		  {
			config.jobDefinitionId = JsonUtil.getString(jsonObject, JOB_HANDLER_CFG_JOB_DEFINITION_ID);
		  }
		  if (jsonObject.has(JOB_HANDLER_CFG_PROCESS_DEFINITION_ID))
		  {
			config.processDefinitionId = JsonUtil.getString(jsonObject, JOB_HANDLER_CFG_PROCESS_DEFINITION_ID);
		  }
		  if (jsonObject.has(JOB_HANDLER_CFG_PROCESS_DEFINITION_KEY))
		  {
			config.processDefinitionKey = JsonUtil.getString(jsonObject, JOB_HANDLER_CFG_PROCESS_DEFINITION_KEY);
		  }
		  if (jsonObject.has(JOB_HANDLER_CFG_PROCESS_DEFINITION_TENANT_ID))
		  {
			config.isTenantIdSet = true;
			if (!JsonUtil.isNull(jsonObject, JOB_HANDLER_CFG_PROCESS_DEFINITION_TENANT_ID))
			{
			  config.tenantId = JsonUtil.getString(jsonObject, JOB_HANDLER_CFG_PROCESS_DEFINITION_TENANT_ID);
			}
		  }
		  if (jsonObject.has(JOB_HANDLER_CFG_INCLUDE_JOBS))
		  {
			config.includeJobs = JsonUtil.getBoolean(jsonObject, JOB_HANDLER_CFG_INCLUDE_JOBS);
		  }

		  return config;
		}

		public static JobDefinitionSuspensionStateConfiguration byJobDefinitionId(string jobDefinitionId, bool includeJobs)
		{
		  JobDefinitionSuspensionStateConfiguration configuration = new JobDefinitionSuspensionStateConfiguration();
		  configuration.by = JOB_HANDLER_CFG_JOB_DEFINITION_ID;
		  configuration.jobDefinitionId = jobDefinitionId;
		  configuration.includeJobs = includeJobs;

		  return configuration;
		}

		public static JobDefinitionSuspensionStateConfiguration byProcessDefinitionId(string processDefinitionId, bool includeJobs)
		{
		  JobDefinitionSuspensionStateConfiguration configuration = new JobDefinitionSuspensionStateConfiguration();

		  configuration.by = JOB_HANDLER_CFG_PROCESS_DEFINITION_ID;
		  configuration.processDefinitionId = processDefinitionId;
		  configuration.includeJobs = includeJobs;

		  return configuration;
		}

		public static JobDefinitionSuspensionStateConfiguration byProcessDefinitionKey(string processDefinitionKey, bool includeJobs)
		{
		  JobDefinitionSuspensionStateConfiguration configuration = new JobDefinitionSuspensionStateConfiguration();

		  configuration.by = JOB_HANDLER_CFG_PROCESS_DEFINITION_KEY;
		  configuration.processDefinitionKey = processDefinitionKey;
		  configuration.includeJobs = includeJobs;

		  return configuration;
		}

		public static JobDefinitionSuspensionStateConfiguration ByProcessDefinitionKeyAndTenantId(string processDefinitionKey, string tenantId, bool includeProcessInstances)
		{
		  JobDefinitionSuspensionStateConfiguration configuration = byProcessDefinitionKey(processDefinitionKey, includeProcessInstances);

		  configuration.isTenantIdSet = true;
		  configuration.tenantId = tenantId;

		  return configuration;

		}


	  }

	  public virtual void onDelete(JobDefinitionSuspensionStateConfiguration configuration, JobEntity jobEntity)
	  {
		// do nothing
	  }

	}

}