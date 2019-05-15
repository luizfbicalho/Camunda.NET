using System;

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
namespace org.camunda.bpm.engine.impl.history.producer
{
	using CaseExecutionEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using HistoricCaseActivityInstanceEventEntity = org.camunda.bpm.engine.impl.history.@event.HistoricCaseActivityInstanceEventEntity;
	using HistoricCaseInstanceEventEntity = org.camunda.bpm.engine.impl.history.@event.HistoricCaseInstanceEventEntity;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class CacheAwareCmmnHistoryEventProducer : DefaultCmmnHistoryEventProducer
	{

	  protected internal override HistoricCaseInstanceEventEntity loadCaseInstanceEventEntity(CaseExecutionEntity caseExecutionEntity)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String caseInstanceId = caseExecutionEntity.getCaseInstanceId();
		string caseInstanceId = caseExecutionEntity.CaseInstanceId;

		HistoricCaseInstanceEventEntity cachedEntity = findInCache(typeof(HistoricCaseInstanceEventEntity), caseInstanceId);

		if (cachedEntity != null)
		{
		  return cachedEntity;
		}
		else
		{
		  return newCaseInstanceEventEntity(caseExecutionEntity);
		}

	  }

	  protected internal override HistoricCaseActivityInstanceEventEntity loadCaseActivityInstanceEventEntity(CaseExecutionEntity caseExecutionEntity)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String caseActivityInstanceId = caseExecutionEntity.getId();
		string caseActivityInstanceId = caseExecutionEntity.Id;

		HistoricCaseActivityInstanceEventEntity cachedEntity = findInCache(typeof(HistoricCaseActivityInstanceEventEntity), caseActivityInstanceId);

		if (cachedEntity != null)
		{
		  return cachedEntity;
		}
		else
		{
		  return newCaseActivityInstanceEventEntity(caseExecutionEntity);
		}

	  }

	  /// <summary>
	  /// find a cached entity by primary key </summary>
	  protected internal virtual T findInCache<T>(Type<T> type, string id) where T : org.camunda.bpm.engine.impl.history.@event.HistoryEvent
	  {
		return Context.CommandContext.DbEntityManager.getCachedEntity(type, id);
	  }

	}

}