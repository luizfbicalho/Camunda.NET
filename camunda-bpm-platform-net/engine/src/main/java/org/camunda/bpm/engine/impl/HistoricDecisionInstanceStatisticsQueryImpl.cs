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
namespace org.camunda.bpm.engine.impl
{
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;
	using HistoricDecisionInstanceStatistics = org.camunda.bpm.engine.history.HistoricDecisionInstanceStatistics;
	using HistoricDecisionInstanceStatisticsQuery = org.camunda.bpm.engine.history.HistoricDecisionInstanceStatisticsQuery;


	/// <summary>
	/// @author Askar Akhmerov
	/// </summary>
	[Serializable]
	public class HistoricDecisionInstanceStatisticsQueryImpl : AbstractQuery<HistoricDecisionInstanceStatisticsQuery, HistoricDecisionInstanceStatistics>, HistoricDecisionInstanceStatisticsQuery
	{

	  protected internal readonly string decisionRequirementsDefinitionId;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string decisionInstanceId_Renamed;

	  public HistoricDecisionInstanceStatisticsQueryImpl(string decisionRequirementsDefinitionId, CommandExecutor commandExecutor) : base(commandExecutor)
	  {
		this.decisionRequirementsDefinitionId = decisionRequirementsDefinitionId;
	  }

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();

		long count = commandContext.StatisticsManager.getStatisticsCountGroupedByDecisionRequirementsDefinition(this);

		return count;
	  }

	  public override IList<HistoricDecisionInstanceStatistics> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();

		IList<HistoricDecisionInstanceStatistics> statisticsList = commandContext.StatisticsManager.getStatisticsGroupedByDecisionRequirementsDefinition(this, page);

		return statisticsList;
	  }

	  protected internal override void checkQueryOk()
	  {
		base.checkQueryOk();
		EnsureUtil.ensureNotNull("decisionRequirementsDefinitionId", decisionRequirementsDefinitionId);
	  }

	  public virtual string DecisionRequirementsDefinitionId
	  {
		  get
		  {
			return decisionRequirementsDefinitionId;
		  }
	  }

	  public virtual HistoricDecisionInstanceStatisticsQuery decisionInstanceId(string decisionInstanceId)
	  {
		this.decisionInstanceId_Renamed = decisionInstanceId;
		return this;
	  }

	  public virtual string DecisionInstanceId
	  {
		  get
		  {
			return decisionInstanceId_Renamed;
		  }
		  set
		  {
			this.decisionInstanceId_Renamed = value;
		  }
	  }


	}

}