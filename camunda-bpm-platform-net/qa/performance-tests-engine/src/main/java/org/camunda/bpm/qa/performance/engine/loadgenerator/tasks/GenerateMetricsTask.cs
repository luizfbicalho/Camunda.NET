using System;
using System.Collections.Generic;
using System.Threading;

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
namespace org.camunda.bpm.qa.performance.engine.loadgenerator.tasks
{
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using MetricsRegistry = org.camunda.bpm.engine.impl.metrics.MetricsRegistry;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;

	/// <summary>
	/// Represents an task which generates metrics of an year.
	/// 
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class GenerateMetricsTask : ThreadStart
	{

	  /// <summary>
	  /// The iteration count which indicates how often the metric generation is
	  /// repeated per thread execution.
	  /// </summary>
	  public const int ITERATION_PER_EXECUTION = 2;

	  /// <summary>
	  /// The milliseconds per year.
	  /// </summary>
	  public static readonly long MS_COUNT_PER_YEAR = 365 * 24 * 60 * 60 * 1000L;

	  /// <summary>
	  /// Generator to generate the thread id's.
	  /// </summary>
	  public static readonly AtomicInteger THREAD_ID_GENERATOR = new AtomicInteger(0);

	  /// <summary>
	  /// The thread id which identifies the current thread.
	  /// </summary>
	  public static readonly ThreadLocal<int> THREAD_ID = new ThreadLocalAnonymousInnerClass();

	  private class ThreadLocalAnonymousInnerClass : ThreadLocal<int>
	  {

		  protected internal override int? initialValue()
		  {
			return THREAD_ID_GENERATOR.AndIncrement;
		  }
	  }

	  /// <summary>
	  /// The start time on which the thread should begin to generate metrics.
	  /// Each thread has his own start time, which is calculated with his id
	  /// and the milliseconds per year. That means each thread generated
	  /// data in a different year.
	  /// </summary>
	  public static readonly ThreadLocal<long> START_TIME = new ThreadLocalAnonymousInnerClass2();

	  private class ThreadLocalAnonymousInnerClass2 : ThreadLocal<long>
	  {

		  protected internal override long? initialValue()
		  {
			return MS_COUNT_PER_YEAR * THREAD_ID.get();
		  }
	  }

	  /// <summary>
	  /// The interval length in milliseconds.
	  /// </summary>
	  public const long INTERVAL = 15 * 60 * 1000;

	  /// <summary>
	  /// The process engine configuration, which is used for the metric reporting.
	  /// </summary>
	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;

	  public GenerateMetricsTask(ProcessEngine processEngine)
	  {
		this.processEngineConfiguration = (ProcessEngineConfigurationImpl) processEngine.ProcessEngineConfiguration;
	  }

	  public override void run()
	  {
		//set up
		long startTime = START_TIME.get();
		MetricsRegistry metricsRegistry = processEngineConfiguration.MetricsRegistry;
		ISet<string> metricNames = metricsRegistry.Meters.Keys;

		//generate metric
		for (int i = 0; i < ITERATION_PER_EXECUTION; i++)
		{
		  ClockUtil.CurrentTime = new DateTime(startTime);
		  foreach (string metricName in metricNames)
		  {
			//mark occurence
			metricsRegistry.markOccurrence(metricName, 1);
		  }
		  processEngineConfiguration.DbMetricsReporter.reportNow();
		  startTime += INTERVAL;
		}
		START_TIME.set(startTime);
	  }

	}

}