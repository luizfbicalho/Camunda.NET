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

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class DbMetricsReporter
	{

	  protected internal MetricsRegistry metricsRegistry;
	  protected internal CommandExecutor commandExecutor;
	  protected internal string reporterId;

	  // log every 15 minutes...
	  protected internal long reportingIntervalInSeconds = 60 * 15;

	  protected internal MetricsCollectionTask metricsCollectionTask;
	  private Timer timer;

	  public DbMetricsReporter(MetricsRegistry metricsRegistry, CommandExecutor commandExecutor)
	  {
		this.metricsRegistry = metricsRegistry;
		this.commandExecutor = commandExecutor;
		initMetricsCollectionTask();
	  }

	  protected internal virtual void initMetricsCollectionTask()
	  {
		metricsCollectionTask = new MetricsCollectionTask(metricsRegistry, commandExecutor);
	  }

	  public virtual void start()
	  {
		timer = new Timer("Camunda Metrics Reporter", true);
		long reportingIntervalInMillis = reportingIntervalInSeconds * 1000;

		timer.scheduleAtFixedRate(metricsCollectionTask, reportingIntervalInMillis, reportingIntervalInMillis);
	  }

	  public virtual void stop()
	  {
		if (timer != null)
		{
		  // cancel the timer
		  timer.cancel();
		  timer = null;
		  // collect and log manually for the last time
		  reportNow();
		}
	  }

	  public virtual void reportNow()
	  {
		if (metricsCollectionTask != null)
		{
		  metricsCollectionTask.run();
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void reportValueAtOnce(final String name, final long value)
	  public virtual void reportValueAtOnce(string name, long value)
	  {
		commandExecutor.execute(new CommandAnonymousInnerClass(this, name, value));
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly DbMetricsReporter outerInstance;

		  private string name;
		  private long value;

		  public CommandAnonymousInnerClass(DbMetricsReporter outerInstance, string name, long value)
		  {
			  this.outerInstance = outerInstance;
			  this.name = name;
			  this.value = value;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			commandContext.MeterLogManager.insert(new MeterLogEntity(name, outerInstance.reporterId, value, ClockUtil.CurrentTime));
			return null;
		  }
	  }

	  public virtual long ReportingIntervalInSeconds
	  {
		  get
		  {
			return reportingIntervalInSeconds;
		  }
		  set
		  {
			this.reportingIntervalInSeconds = value;
		  }
	  }


	  public virtual MetricsRegistry MetricsRegistry
	  {
		  get
		  {
			return metricsRegistry;
		  }
	  }

	  public virtual CommandExecutor CommandExecutor
	  {
		  get
		  {
			return commandExecutor;
		  }
	  }

	  public virtual MetricsCollectionTask MetricsCollectionTask
	  {
		  get
		  {
			return metricsCollectionTask;
		  }
		  set
		  {
			this.metricsCollectionTask = value;
		  }
	  }


	  public virtual string ReporterId
	  {
		  set
		  {
			this.reporterId = value;
			if (metricsCollectionTask != null)
			{
			  metricsCollectionTask.Reporter = value;
			}
		  }
	  }

	}

}