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
namespace org.camunda.bpm.engine.impl.cmd
{

	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DbMetricsReporter = org.camunda.bpm.engine.impl.metrics.reporter.DbMetricsReporter;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	[Serializable]
	public class ReportDbMetricsCmd : Command<Void>
	{

	  private const long serialVersionUID = 1L;

	  public virtual Void execute(CommandContext commandContext)
	  {
		ProcessEngineConfigurationImpl engineConfiguration = Context.ProcessEngineConfiguration;

		if (!engineConfiguration.MetricsEnabled)
		{
		  throw new ProcessEngineException("Metrics reporting is disabled");
		}

		if (!engineConfiguration.DbMetricsReporterActivate)
		{
		  throw new ProcessEngineException("Metrics reporting to database is disabled");
		}

		DbMetricsReporter dbMetricsReporter = engineConfiguration.DbMetricsReporter;
		dbMetricsReporter.reportNow();
		return null;
	  }

	}

}