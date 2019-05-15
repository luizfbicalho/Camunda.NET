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
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using NativeHistoricVariableInstanceQuery = org.camunda.bpm.engine.history.NativeHistoricVariableInstanceQuery;
	using CommandLogger = org.camunda.bpm.engine.impl.cmd.CommandLogger;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using HistoricVariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricVariableInstanceEntity;



	[Serializable]
	public class NativeHistoricVariableInstanceQueryImpl : AbstractNativeQuery<NativeHistoricVariableInstanceQuery, HistoricVariableInstance>, NativeHistoricVariableInstanceQuery
	{

	  private static readonly CommandLogger LOG = ProcessEngineLogger.CMD_LOGGER;

	  private const long serialVersionUID = 1L;

	  protected internal bool isCustomObjectDeserializationEnabled = true;

	  public NativeHistoricVariableInstanceQueryImpl(CommandContext commandContext) : base(commandContext)
	  {
	  }

	  public NativeHistoricVariableInstanceQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }


	  //results ////////////////////////////////////////////////////////////////

	  public virtual NativeHistoricVariableInstanceQuery disableCustomObjectDeserialization()
	  {
			this.isCustomObjectDeserializationEnabled = false;
			return this;
	  }

	  public override IList<HistoricVariableInstance> executeList(CommandContext commandContext, IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		IList<HistoricVariableInstance> historicVariableInstances = commandContext.HistoricVariableInstanceManager.findHistoricVariableInstancesByNativeQuery(parameterMap, firstResult, maxResults);

		if (historicVariableInstances != null)
		{
		  foreach (HistoricVariableInstance historicVariableInstance in historicVariableInstances)
		  {

			HistoricVariableInstanceEntity variableInstanceEntity = (HistoricVariableInstanceEntity) historicVariableInstance;
			  try
			  {
				variableInstanceEntity.getTypedValue(isCustomObjectDeserializationEnabled);
			  }
			  catch (Exception t)
			  {
				// do not fail if one of the variables fails to load
				LOG.exceptionWhileGettingValueForVariable(t);
			  }
		  }
		}
		return historicVariableInstances;
	  }

	  public override long executeCount(CommandContext commandContext, IDictionary<string, object> parameterMap)
	  {
		return commandContext.HistoricVariableInstanceManager.findHistoricVariableInstanceCountByNativeQuery(parameterMap);
	  }

	}

}