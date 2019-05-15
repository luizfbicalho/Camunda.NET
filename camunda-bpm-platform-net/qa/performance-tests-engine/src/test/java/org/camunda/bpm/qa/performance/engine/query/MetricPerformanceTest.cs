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
namespace org.camunda.bpm.qa.performance.engine.query
{
	using Metrics = org.camunda.bpm.engine.management.Metrics;
	using ProcessEnginePerformanceTestCase = org.camunda.bpm.qa.performance.engine.junit.ProcessEnginePerformanceTestCase;
	using GenerateMetricsTask = org.camunda.bpm.qa.performance.engine.loadgenerator.tasks.GenerateMetricsTask;
	using MetricIntervalStep = org.camunda.bpm.qa.performance.engine.steps.MetricIntervalStep;
	using MetricSumStep = org.camunda.bpm.qa.performance.engine.steps.MetricSumStep;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;
	using Parameter = org.junit.runners.Parameterized.Parameter;
	using Parameters = org.junit.runners.Parameterized.Parameters;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class MetricPerformanceTest extends org.camunda.bpm.qa.performance.engine.junit.ProcessEnginePerformanceTestCase
	public class MetricPerformanceTest : ProcessEnginePerformanceTestCase
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(0) public String name;
		public string name;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(1) public java.util.Date startDate;
	  public DateTime startDate;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(2) public java.util.Date endDate;
	  public DateTime endDate;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters(name="{index}") public static Iterable<Object[]> params()
	  public static IEnumerable<object[]> @params()
	  {
		return Arrays.asList(new object[][]
		{
			new object[] {null, null, null},
			new object[] {Metrics.ACTIVTY_INSTANCE_START, null, null},
			new object[] {Metrics.ACTIVTY_INSTANCE_START, new DateTime(), null},
			new object[] {null, new DateTime(), null},
			new object[] {null, null, new DateTime(GenerateMetricsTask.INTERVAL * 250)},
			new object[] {Metrics.ACTIVTY_INSTANCE_START, null, new DateTime(GenerateMetricsTask.INTERVAL * 250)},
			new object[] {Metrics.ACTIVTY_INSTANCE_START, new DateTime(), new DateTime(GenerateMetricsTask.INTERVAL * 250)},
			new object[] {null, new DateTime(), new DateTime(GenerateMetricsTask.INTERVAL * 250)}
		});
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void metricInterval()
	  public virtual void metricInterval()
	  {
		performanceTest().step(new MetricIntervalStep(name, startDate, endDate, engine)).run();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void metricSum()
	  public virtual void metricSum()
	  {
		performanceTest().step(new MetricSumStep(name, startDate, endDate, engine)).run();
	  }
	}

}