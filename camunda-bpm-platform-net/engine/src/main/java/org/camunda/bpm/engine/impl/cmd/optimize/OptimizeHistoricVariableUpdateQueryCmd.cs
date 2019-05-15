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

	using HistoricVariableUpdate = org.camunda.bpm.engine.history.HistoricVariableUpdate;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using HistoricDetailVariableInstanceUpdateEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricDetailVariableInstanceUpdateEntity;
	using AbstractTypedValueSerializer = org.camunda.bpm.engine.impl.variable.serializer.AbstractTypedValueSerializer;

	public class OptimizeHistoricVariableUpdateQueryCmd : Command<IList<HistoricVariableUpdate>>
	{

	  private static readonly CommandLogger LOG = ProcessEngineLogger.CMD_LOGGER;

	  protected internal DateTime occurredAfter;
	  protected internal DateTime occurredAt;
	  protected internal int maxResults;

	  public OptimizeHistoricVariableUpdateQueryCmd(DateTime occurredAfter, DateTime occurredAt, int maxResults)
	  {
		this.occurredAfter = occurredAfter;
		this.occurredAt = occurredAt;
		this.maxResults = maxResults;
	  }

	  public virtual IList<HistoricVariableUpdate> execute(CommandContext commandContext)
	  {
		IList<HistoricVariableUpdate> historicVariableUpdates = commandContext.OptimizeManager.getHistoricVariableUpdates(occurredAfter, occurredAt, maxResults);
		fetchVariableValues(historicVariableUpdates, commandContext);
		return historicVariableUpdates;
	  }

	  private void fetchVariableValues(IList<HistoricVariableUpdate> historicVariableUpdates, CommandContext commandContext)
	  {
		if (historicVariableUpdates != null)
		{

		  IList<string> byteArrayIds = getByteArrayIds(historicVariableUpdates);
		  if (byteArrayIds.Count > 0)
		  {
			// pre-fetch all byte arrays into dbEntityCache to avoid (n+1) number of queries
			commandContext.OptimizeManager.getHistoricVariableUpdateByteArrays(byteArrayIds);
		  }

		  resolveTypedValues(historicVariableUpdates);
		}
	  }

	  protected internal virtual bool isNotByteArrayVariableType(HistoricDetailVariableInstanceUpdateEntity entity)
	  {
		// do not fetch values for byte arrays/ blob variables (e.g. files or bytes)
		return !AbstractTypedValueSerializer.BINARY_VALUE_TYPES.Contains(entity.Serializer.Type.Name);
	  }

	  protected internal virtual bool isHistoricDetailVariableInstanceUpdateEntity(HistoricVariableUpdate variableUpdate)
	  {
		return variableUpdate is HistoricDetailVariableInstanceUpdateEntity;
	  }

	  protected internal virtual IList<string> getByteArrayIds(IList<HistoricVariableUpdate> variableUpdates)
	  {
		IList<string> byteArrayIds = new List<string>();

		foreach (HistoricVariableUpdate variableUpdate in variableUpdates)
		{
		  if (isHistoricDetailVariableInstanceUpdateEntity(variableUpdate))
		  {
			HistoricDetailVariableInstanceUpdateEntity entity = (HistoricDetailVariableInstanceUpdateEntity) variableUpdate;

			if (isNotByteArrayVariableType(entity))
			{
			  string byteArrayId = entity.ByteArrayValueId;
			  if (!string.ReferenceEquals(byteArrayId, null))
			  {
				byteArrayIds.Add(byteArrayId);
			  }
			}

		  }
		}

		return byteArrayIds;
	  }

	  protected internal virtual void resolveTypedValues(IList<HistoricVariableUpdate> variableUpdates)
	  {
		foreach (HistoricVariableUpdate variableUpdate in variableUpdates)
		{
		  if (isHistoricDetailVariableInstanceUpdateEntity(variableUpdate))
		  {
			HistoricDetailVariableInstanceUpdateEntity entity = (HistoricDetailVariableInstanceUpdateEntity) variableUpdate;

			if (isNotByteArrayVariableType(entity))
			{
			  try
			  {
				entity.getTypedValue(false);
			  }
			  catch (Exception t)
			  {
				// do not fail if one of the variables fails to load
				LOG.exceptionWhileGettingValueForVariable(t);
			  }
			}

		  }
		}
	  }

	}

}