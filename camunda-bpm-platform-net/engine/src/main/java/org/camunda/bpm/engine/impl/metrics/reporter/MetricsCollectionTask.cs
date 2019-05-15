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
namespace org.camunda.bpm.engine.impl.metrics.reporter
{

	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using MeterLogEntity = org.camunda.bpm.engine.impl.persistence.entity.MeterLogEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;

	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class MetricsCollectionTask : TimerTask
	{

	  private static readonly MetricsLogger LOG = ProcessEngineLogger.METRICS_LOGGER;

	  protected internal MetricsRegistry metricsRegistry;
	  protected internal CommandExecutor commandExecutor;
	  protected internal string reporterId = null;

	  public MetricsCollectionTask(MetricsRegistry metricsRegistry, CommandExecutor commandExecutor)
	  {
		this.metricsRegistry = metricsRegistry;
		this.commandExecutor = commandExecutor;
	  }

	  public virtual void run()
	  {
		try
		{
		  collectMetrics();
		}
		catch (Exception e)
		{
		  try
		  {
			LOG.couldNotCollectAndLogMetrics(e);
		  }
		  catch (Exception)
		  {
			// ignore if log can't be written
		  }
		}
	  }

	  protected internal virtual void collectMetrics()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.impl.persistence.entity.MeterLogEntity> logs = new java.util.ArrayList<org.camunda.bpm.engine.impl.persistence.entity.MeterLogEntity>();
		IList<MeterLogEntity> logs = new List<MeterLogEntity>();
		foreach (Meter meter in metricsRegistry.Meters.Values)
		{
		  logs.Add(new MeterLogEntity(meter.Name, reporterId, meter.AndClear, ClockUtil.CurrentTime));

		}

		commandExecutor.execute(new CommandAnonymousInnerClass(this, logs));
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly MetricsCollectionTask outerInstance;

		  private IList<MeterLogEntity> logs;

		  public CommandAnonymousInnerClass(MetricsCollectionTask outerInstance, IList<MeterLogEntity> logs)
		  {
			  this.outerInstance = outerInstance;
			  this.logs = logs;
		  }


		  public Void execute(CommandContext commandContext)
		  {
			foreach (MeterLogEntity meterLogEntity in logs)
			{
			  commandContext.MeterLogManager.insert(meterLogEntity);
			}
			return null;
		  }
	  }

	  public virtual string Reporter
	  {
		  get
		  {
			return reporterId;
		  }
		  set
		  {
			this.reporterId = value;
		  }
	  }




	}

}