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

	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using NativeHistoricDecisionInstanceQuery = org.camunda.bpm.engine.history.NativeHistoricDecisionInstanceQuery;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;

	/// <summary>
	/// @author Philipp Ossler
	/// </summary>
	[Serializable]
	public class NativeHistoryDecisionInstanceQueryImpl : AbstractNativeQuery<NativeHistoricDecisionInstanceQuery, HistoricDecisionInstance>, NativeHistoricDecisionInstanceQuery
	{

	  private const long serialVersionUID = 1L;

	  public NativeHistoryDecisionInstanceQueryImpl(CommandContext commandContext) : base(commandContext)
	  {
	  }

	  public NativeHistoryDecisionInstanceQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public override long executeCount(CommandContext commandContext, IDictionary<string, object> parameterMap)
	  {
		return commandContext.HistoricDecisionInstanceManager.findHistoricDecisionInstanceCountByNativeQuery(parameterMap);
	  }

	  public override IList<HistoricDecisionInstance> executeList(CommandContext commandContext, IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return commandContext.HistoricDecisionInstanceManager.findHistoricDecisionInstancesByNativeQuery(parameterMap, firstResult, maxResults);
	  }

	}

}