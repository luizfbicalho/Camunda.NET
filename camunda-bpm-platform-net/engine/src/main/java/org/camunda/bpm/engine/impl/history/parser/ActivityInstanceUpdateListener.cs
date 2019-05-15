﻿/*
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
namespace org.camunda.bpm.engine.impl.history.parser
{
	using DelegateTask = org.camunda.bpm.engine.@delegate.DelegateTask;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;
	using HistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.HistoryEventProducer;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ActivityInstanceUpdateListener : HistoryTaskListener
	{

	  public ActivityInstanceUpdateListener(HistoryEventProducer historyEventProducer, HistoryLevel historyLevel) : base(historyEventProducer, historyLevel)
	  {
	  }

	  protected internal override HistoryEvent createHistoryEvent(DelegateTask task, ExecutionEntity execution)
	  {
		if (historyLevel.isHistoryEventProduced(HistoryEventTypes.ACTIVITY_INSTANCE_UPDATE, execution))
		{
		  return eventProducer.createActivityInstanceUpdateEvt(execution, task);
		}
		else
		{
		  return null;
		}
	  }


	}

}