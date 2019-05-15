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
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;


	public class TimerStartEventJobHandler : TimerEventJobHandler
	{

	  private static readonly JobExecutorLogger LOG = ProcessEngineLogger.JOB_EXECUTOR_LOGGER;

	  public const string TYPE = "timer-start-event";

	  public override string Type
	  {
		  get
		  {
			return TYPE;
		  }
	  }

	  public virtual void execute(TimerJobConfiguration configuration, ExecutionEntity execution, CommandContext commandContext, string tenantId)
	  {
		DeploymentCache deploymentCache = Context.ProcessEngineConfiguration.DeploymentCache;

		string definitionKey = configuration.TimerElementKey;
		ProcessDefinition processDefinition = deploymentCache.findDeployedLatestProcessDefinitionByKeyAndTenantId(definitionKey, tenantId);

		try
		{
		  startProcessInstance(commandContext, tenantId, processDefinition);
		}
		catch (Exception e)
		{
		  throw e;
		}
	  }

	  protected internal virtual void startProcessInstance(CommandContext commandContext, string tenantId, ProcessDefinition processDefinition)
	  {
		if (!processDefinition.Suspended)
		{

		  RuntimeService runtimeService = commandContext.ProcessEngineConfiguration.RuntimeService;
		  runtimeService.createProcessInstanceByKey(processDefinition.Key).processDefinitionTenantId(tenantId).execute();

		}
		else
		{
		  LOG.ignoringSuspendedJob(processDefinition);
		}
	  }
	}

}