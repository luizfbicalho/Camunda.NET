﻿using System;
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

	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using NativeHistoricProcessInstanceQuery = org.camunda.bpm.engine.history.NativeHistoricProcessInstanceQuery;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;


	[Serializable]
	public class NativeHistoricProcessInstanceQueryImpl : AbstractNativeQuery<NativeHistoricProcessInstanceQuery, HistoricProcessInstance>, NativeHistoricProcessInstanceQuery
	{

	  private const long serialVersionUID = 1L;

	  public NativeHistoricProcessInstanceQueryImpl(CommandContext commandContext) : base(commandContext)
	  {
	  }

	  public NativeHistoricProcessInstanceQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }


	 //results ////////////////////////////////////////////////////////////////

	  public override IList<HistoricProcessInstance> executeList(CommandContext commandContext, IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return commandContext.HistoricProcessInstanceManager.findHistoricProcessInstancesByNativeQuery(parameterMap, firstResult, maxResults);
	  }

	  public override long executeCount(CommandContext commandContext, IDictionary<string, object> parameterMap)
	  {
		return commandContext.HistoricProcessInstanceManager.findHistoricProcessInstanceCountByNativeQuery(parameterMap);
	  }

	}

}