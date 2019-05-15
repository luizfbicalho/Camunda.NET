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
	using AbstractSetProcessDefinitionStateCmd = org.camunda.bpm.engine.impl.cmd.AbstractSetProcessDefinitionStateCmd;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ProcessDefinitionSuspensionStateConfiguration = org.camunda.bpm.engine.impl.jobexecutor.TimerChangeProcessDefinitionSuspensionStateJobHandler.ProcessDefinitionSuspensionStateConfiguration;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using UpdateProcessDefinitionSuspensionStateBuilderImpl = org.camunda.bpm.engine.impl.repository.UpdateProcessDefinitionSuspensionStateBuilderImpl;
	using JsonUtil = org.camunda.bpm.engine.impl.util.JsonUtil;
	using JsonObject = com.google.gson.JsonObject;

	/// <summary>
	/// @author Joram Barrez
	/// @author roman.smirnov
	/// </summary>
	public abstract class TimerChangeProcessDefinitionSuspensionStateJobHandler : JobHandler<ProcessDefinitionSuspensionStateConfiguration>
	{
		public abstract void onDelete(T configuration, JobEntity jobEntity);
		public abstract void execute(T configuration, ExecutionEntity execution, CommandContext commandContext, string tenantId);
		public abstract string Type {get;}

	  protected internal const string JOB_HANDLER_CFG_BY = "by";
	  protected internal const string JOB_HANDLER_CFG_PROCESS_DEFINITION_ID = "processDefinitionId";
	  protected internal const string JOB_HANDLER_CFG_PROCESS_DEFINITION_KEY = "processDefinitionKey";
	  protected internal const string JOB_HANDLER_CFG_PROCESS_DEFINITION_TENANT_ID = "processDefinitionTenantId";

	  protected internal const string JOB_HANDLER_CFG_INCLUDE_PROCESS_INSTANCES = "includeProcessInstances";

	  public virtual void execute(ProcessDefinitionSuspensionStateConfiguration configuration, ExecutionEntity execution, CommandContext commandContext, string tenantId)
	  {
		AbstractSetProcessDefinitionStateCmd cmd = getCommand(configuration);
		cmd.disableLogUserOperation();
		cmd.execute(commandContext);
	  }

	  protected internal abstract AbstractSetProcessDefinitionStateCmd getCommand(ProcessDefinitionSuspensionStateConfiguration configuration);

	  public virtual ProcessDefinitionSuspensionStateConfiguration newConfiguration(string canonicalString)
	  {
		JsonObject jsonObject = JsonUtil.asObject(canonicalString);

		return ProcessDefinitionSuspensionStateConfiguration.fromJson(jsonObject);
	  }

	  public class ProcessDefinitionSuspensionStateConfiguration : JobHandlerConfiguration
	  {

		protected internal string processDefinitionKey;
		protected internal string processDefinitionId;
		protected internal bool includeProcessInstances;
		protected internal string tenantId;
		protected internal bool isTenantIdSet;
		protected internal string by;

		public virtual string toCanonicalString()
		{
		  JsonObject json = JsonUtil.createObject();

		  JsonUtil.addField(json, JOB_HANDLER_CFG_BY, by);
		  JsonUtil.addField(json, JOB_HANDLER_CFG_PROCESS_DEFINITION_KEY, processDefinitionKey);
		  JsonUtil.addField(json, JOB_HANDLER_CFG_INCLUDE_PROCESS_INSTANCES, includeProcessInstances);
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

		public virtual UpdateProcessDefinitionSuspensionStateBuilderImpl createBuilder()
		{
		  UpdateProcessDefinitionSuspensionStateBuilderImpl builder = new UpdateProcessDefinitionSuspensionStateBuilderImpl();

		  if (by.Equals(JOB_HANDLER_CFG_PROCESS_DEFINITION_ID))
		  {
			builder.byProcessDefinitionId(processDefinitionId);

		  }
		  else if (by.Equals(JOB_HANDLER_CFG_PROCESS_DEFINITION_KEY))
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

		  builder.includeProcessInstances(includeProcessInstances);

		  return builder;
		}

		public static ProcessDefinitionSuspensionStateConfiguration fromJson(JsonObject jsonObject)
		{
		  ProcessDefinitionSuspensionStateConfiguration config = new ProcessDefinitionSuspensionStateConfiguration();

		  config.by = JsonUtil.getString(jsonObject, JOB_HANDLER_CFG_BY);
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
		  if (jsonObject.has(JOB_HANDLER_CFG_INCLUDE_PROCESS_INSTANCES))
		  {
			config.includeProcessInstances = JsonUtil.getBoolean(jsonObject, JOB_HANDLER_CFG_INCLUDE_PROCESS_INSTANCES);
		  }

		  return config;
		}

		public static ProcessDefinitionSuspensionStateConfiguration byProcessDefinitionId(string processDefinitionId, bool includeProcessInstances)
		{
		  ProcessDefinitionSuspensionStateConfiguration configuration = new ProcessDefinitionSuspensionStateConfiguration();

		  configuration.by = JOB_HANDLER_CFG_PROCESS_DEFINITION_ID;
		  configuration.processDefinitionId = processDefinitionId;
		  configuration.includeProcessInstances = includeProcessInstances;

		  return configuration;
		}

		public static ProcessDefinitionSuspensionStateConfiguration byProcessDefinitionKey(string processDefinitionKey, bool includeProcessInstances)
		{
		  ProcessDefinitionSuspensionStateConfiguration configuration = new ProcessDefinitionSuspensionStateConfiguration();

		  configuration.by = JOB_HANDLER_CFG_PROCESS_DEFINITION_KEY;
		  configuration.processDefinitionKey = processDefinitionKey;
		  configuration.includeProcessInstances = includeProcessInstances;

		  return configuration;
		}

		public static ProcessDefinitionSuspensionStateConfiguration byProcessDefinitionKeyAndTenantId(string processDefinitionKey, string tenantId, bool includeProcessInstances)
		{
		  ProcessDefinitionSuspensionStateConfiguration configuration = byProcessDefinitionKey(processDefinitionKey, includeProcessInstances);

		  configuration.isTenantIdSet = true;
		  configuration.tenantId = tenantId;

		  return configuration;

		}


	  }

	  public virtual void onDelete(ProcessDefinitionSuspensionStateConfiguration configuration, JobEntity jobEntity)
	  {
		// do nothing
	  }

	}

}