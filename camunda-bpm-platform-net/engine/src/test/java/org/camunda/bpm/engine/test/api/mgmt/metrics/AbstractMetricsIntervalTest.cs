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
namespace org.camunda.bpm.engine.test.api.mgmt.metrics
{
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Meter = org.camunda.bpm.engine.impl.metrics.Meter;
	using MetricsRegistry = org.camunda.bpm.engine.impl.metrics.MetricsRegistry;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using DateTime = org.joda.time.DateTime;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;


	/// <summary>
	/// Represents the abstract metrics interval test class, which contains methods
	/// for generating metrics and clean up afterwards.
	/// 
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public abstract class AbstractMetricsIntervalTest
	{
		private bool InstanceFieldsInitialized = false;

		public AbstractMetricsIntervalTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			TEST_RULE = new ProcessEngineTestRule(ENGINE_RULE);
			RULE_CHAIN = RuleChain.outerRule(ENGINE_RULE).around(TEST_RULE);
		}


	  protected internal readonly ProcessEngineRule ENGINE_RULE = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule TEST_RULE;
	  protected internal readonly string REPORTER_ID = "REPORTER_ID";
	  protected internal const int DEFAULT_INTERVAL = 15;
	  protected internal const int DEFAULT_INTERVAL_MILLIS = 15 * 60 * 1000;
	  protected internal const int MIN_OCCURENCE = 1;
	  protected internal const int MAX_OCCURENCE = 250;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain RULE_CHAIN = org.junit.rules.RuleChain.outerRule(ENGINE_RULE).around(TEST_RULE);
	  public RuleChain RULE_CHAIN;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public final org.junit.rules.ExpectedException exception = org.junit.rules.ExpectedException.none();
	  public readonly ExpectedException exception = ExpectedException.none();

	  protected internal RuntimeService runtimeService;
	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;
	  protected internal ManagementService managementService;
	  protected internal string lastReporterId;
	  protected internal DateTime firstInterval;
	  protected internal int metricsCount;
	  protected internal MetricsRegistry metricsRegistry;
	  protected internal Random rand;

	  protected internal virtual void generateMeterData(long dataCount, long interval)
	  {
		//set up for randomnes
		ISet<string> metricNames = metricsRegistry.Meters.Keys;
		metricsCount = metricNames.Count;

		//start date is the default interval since mariadb can't set 0 as timestamp
		long startDate = DEFAULT_INTERVAL_MILLIS;
		firstInterval = new DateTime(startDate);
		//we will have 5 metric reports in an interval
		int dataPerInterval = 5;

		//generate data
		for (int i = 0; i < dataCount; i++)
		{
		  //calulate diff so timer can be set correctly
		  long diff = interval / dataPerInterval;
		  for (int j = 0; j < dataPerInterval; j++)
		  {
			ClockUtil.CurrentTime = new DateTime(startDate);
			//generate random count of data per interv
			//for each metric
			reportMetrics();
			startDate += diff;
		  }
		}
	  }

	  protected internal virtual void reportMetrics()
	  {
		foreach (string metricName in metricsRegistry.Meters.Keys)
		{
		  //mark random occurence
		  long occurence = (long)(rand.Next((MAX_OCCURENCE - MIN_OCCURENCE) + 1) + MIN_OCCURENCE);
		  metricsRegistry.markOccurrence(metricName, occurence);
		}
		//report logged metrics
		processEngineConfiguration.DbMetricsReporter.reportNow();
	  }

	  protected internal virtual void clearMetrics()
	  {
		clearLocalMetrics();
		managementService.deleteMetrics(null);
	  }

	  protected internal virtual void clearLocalMetrics()
	  {
		ICollection<Meter> meters = processEngineConfiguration.MetricsRegistry.Meters.Values;
		foreach (Meter meter in meters)
		{
		  meter.AndClear;
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initMetrics() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void initMetrics()
	  {
		runtimeService = ENGINE_RULE.RuntimeService;
		processEngineConfiguration = ENGINE_RULE.ProcessEngineConfiguration;
		managementService = ENGINE_RULE.ManagementService;

		//clean up before start
		clearMetrics();

		//init metrics
		processEngineConfiguration.DbMetricsReporterActivate = true;
		lastReporterId = processEngineConfiguration.DbMetricsReporter.MetricsCollectionTask.Reporter;
		processEngineConfiguration.DbMetricsReporter.ReporterId = REPORTER_ID;
		metricsRegistry = processEngineConfiguration.MetricsRegistry;
		rand = new Random((DateTime.Now).Ticks);
		generateMeterData(3, DEFAULT_INTERVAL_MILLIS);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanUp()
	  public virtual void cleanUp()
	  {
		ClockUtil.reset();
		processEngineConfiguration.DbMetricsReporterActivate = false;
		processEngineConfiguration.DbMetricsReporter.ReporterId = lastReporterId;
		clearMetrics();
	  }
	}

}