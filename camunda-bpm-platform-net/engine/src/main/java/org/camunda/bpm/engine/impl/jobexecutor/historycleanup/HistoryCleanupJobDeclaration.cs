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
namespace org.camunda.bpm.engine.impl.jobexecutor.historycleanup
{
	using CommandLogger = org.camunda.bpm.engine.impl.cmd.CommandLogger;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using EverLivingJobEntity = org.camunda.bpm.engine.impl.persistence.entity.EverLivingJobEntity;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;

	/// <summary>
	/// Job declaration for history cleanup.
	/// @author Svetlana Dorokhova
	/// </summary>
	[Serializable]
	public class HistoryCleanupJobDeclaration : JobDeclaration<HistoryCleanupContext, EverLivingJobEntity>
	{

	  private static readonly CommandLogger LOG = ProcessEngineLogger.CMD_LOGGER;

	  public HistoryCleanupJobDeclaration() : base(HistoryCleanupJobHandler.TYPE)
	  {
	  }

	  protected internal override ExecutionEntity resolveExecution(HistoryCleanupContext context)
	  {
		return null;
	  }

	  protected internal override EverLivingJobEntity newJobInstance(HistoryCleanupContext context)
	  {
		return new EverLivingJobEntity();
	  }

	  protected internal override void postInitialize(HistoryCleanupContext context, EverLivingJobEntity job)
	  {
	  }


	  public override EverLivingJobEntity reconfigure(HistoryCleanupContext context, EverLivingJobEntity job)
	  {
		HistoryCleanupJobHandlerConfiguration configuration = resolveJobHandlerConfiguration(context);
		job.JobHandlerConfiguration = configuration;
		return job;
	  }

	  protected internal override HistoryCleanupJobHandlerConfiguration resolveJobHandlerConfiguration(HistoryCleanupContext context)
	  {
		HistoryCleanupJobHandlerConfiguration config = new HistoryCleanupJobHandlerConfiguration();
		config.ImmediatelyDue = context.ImmediatelyDue;
		config.MinuteFrom = context.MinuteFrom;
		config.MinuteTo = context.MinuteTo;
		return config;
	  }

	  public override DateTime resolveDueDate(HistoryCleanupContext context)
	  {
		return resolveDueDate(context.ImmediatelyDue);
	  }

	  private DateTime resolveDueDate(bool isImmediatelyDue)
	  {
		CommandContext commandContext = Context.CommandContext;
		if (isImmediatelyDue)
		{
		  return ClockUtil.CurrentTime;
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BatchWindow currentOrNextBatchWindow = commandContext.getProcessEngineConfiguration().getBatchWindowManager().getCurrentOrNextBatchWindow(org.camunda.bpm.engine.impl.util.ClockUtil.getCurrentTime(), commandContext.getProcessEngineConfiguration());
		  BatchWindow currentOrNextBatchWindow = commandContext.ProcessEngineConfiguration.BatchWindowManager.getCurrentOrNextBatchWindow(ClockUtil.CurrentTime, commandContext.ProcessEngineConfiguration);
		  if (currentOrNextBatchWindow != null)
		  {
			return currentOrNextBatchWindow.Start;
		  }
		  else
		  {
			return null;
		  }
		}
	  }
	}

}