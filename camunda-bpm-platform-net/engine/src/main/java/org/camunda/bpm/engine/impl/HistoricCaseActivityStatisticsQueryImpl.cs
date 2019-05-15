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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using HistoricCaseActivityStatistics = org.camunda.bpm.engine.history.HistoricCaseActivityStatistics;
	using HistoricCaseActivityStatisticsQuery = org.camunda.bpm.engine.history.HistoricCaseActivityStatisticsQuery;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;

	/// <summary>
	/// @author smirnov
	/// 
	/// </summary>
	[Serializable]
	public class HistoricCaseActivityStatisticsQueryImpl : AbstractQuery<HistoricCaseActivityStatisticsQuery, HistoricCaseActivityStatistics>, HistoricCaseActivityStatisticsQuery
	{

	  private const long serialVersionUID = 1L;

	  protected internal string caseDefinitionId;

	  public HistoricCaseActivityStatisticsQueryImpl(string caseDefinitionId, CommandExecutor commandExecutor) : base(commandExecutor)
	  {
		this.caseDefinitionId = caseDefinitionId;
	  }

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.HistoricStatisticsManager.getHistoricStatisticsCountGroupedByCaseActivity(this);
	  }

	  public override IList<HistoricCaseActivityStatistics> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.HistoricStatisticsManager.getHistoricStatisticsGroupedByCaseActivity(this, page);
	  }

	  protected internal override void checkQueryOk()
	  {
		base.checkQueryOk();
		ensureNotNull("No valid case definition id supplied", "caseDefinitionId", caseDefinitionId);
	  }

	  public virtual string CaseDefinitionId
	  {
		  get
		  {
			return caseDefinitionId;
		  }
	  }

	}

}