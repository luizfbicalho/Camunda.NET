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
namespace org.camunda.bpm.engine.impl.cmd.optimize
{
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;


	public class OptimizeCompletedHistoricProcessInstanceQueryCmd : Command<IList<HistoricProcessInstance>>
	{

	  protected internal DateTime finishedAfter;
	  protected internal DateTime finishedAt;
	  protected internal int maxResults;

	  public OptimizeCompletedHistoricProcessInstanceQueryCmd(DateTime finishedAfter, DateTime finishedAt, int maxResults)
	  {
		this.finishedAfter = finishedAfter;
		this.finishedAt = finishedAt;
		this.maxResults = maxResults;
	  }

	  public virtual IList<HistoricProcessInstance> execute(CommandContext commandContext)
	  {
		return commandContext.OptimizeManager.getCompletedHistoricProcessInstances(finishedAfter, finishedAt, maxResults);
	  }

	}

}