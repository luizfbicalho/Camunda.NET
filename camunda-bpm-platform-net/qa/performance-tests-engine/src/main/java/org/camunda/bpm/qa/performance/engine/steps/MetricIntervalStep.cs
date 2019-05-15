﻿using System;

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
namespace org.camunda.bpm.qa.performance.engine.steps
{
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using MetricsQuery = org.camunda.bpm.engine.management.MetricsQuery;
	using PerfTestRunContext = org.camunda.bpm.qa.performance.engine.framework.PerfTestRunContext;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class MetricIntervalStep : ProcessEngineAwareStep
	{

	  protected internal string name;
	  protected internal DateTime startDate;
	  protected internal DateTime endDate;

	  public MetricIntervalStep(string name, DateTime startDate, DateTime endDate, ProcessEngine processEngine) : base(processEngine)
	  {
		this.name = name;
		this.startDate = startDate;
		this.endDate = endDate;
	  }

	  public override void execute(PerfTestRunContext context)
	  {
		MetricsQuery metricsQuery = this.processEngine.ManagementService.createMetricsQuery();

		if (!string.ReferenceEquals(name, null))
		{
		  metricsQuery.name(name);
		}

		if (startDate != null)
		{
		  metricsQuery.startDate(startDate);
		}

		if (endDate != null)
		{
		  metricsQuery.endDate(endDate);
		}

		metricsQuery.interval();
	  }
	}

}