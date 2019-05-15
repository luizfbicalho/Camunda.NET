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
namespace org.camunda.bpm.engine.impl.history.@event
{
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using HistoryEventHandler = org.camunda.bpm.engine.impl.history.handler.HistoryEventHandler;
	using HistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.HistoryEventProducer;

	/// <summary>
	/// <para>The <seealso cref="HistoryEventProcessor"/> should be used to process an history event.</para>
	/// 
	/// <para>The <seealso cref="HistoryEvent"/> will be created with the help of the <seealso cref="HistoryEventProducer"/>
	/// from the <seealso cref="ProcessEngineConfiguration"/> and the given implementation of the
	/// <seealso cref="HistoryEventCreator"/> which uses the producer object to create an
	/// <seealso cref="HistoryEvent"/>. The <seealso cref="HistoryEvent"/> will be handled by the
	/// <seealso cref="HistoryEventHandler"/> from the <seealso cref="ProcessEngineConfiguration"/>.</para>
	/// 
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// @since 7.5
	/// </summary>
	public class HistoryEventProcessor
	{

	  /// <summary>
	  /// The <seealso cref="HistoryEventCreator"/> interface which is used to interchange the implementation
	  /// of the creation of different HistoryEvents.
	  /// </summary>
	  public class HistoryEventCreator
	  {
		/// <summary>
		/// Creates the <seealso cref="HistoryEvent"/> with the help off the given
		/// <seealso cref="HistoryEventProducer"/>.
		/// </summary>
		/// <param name="producer"> the producer which is used for the creation </param>
		/// <returns> the created <seealso cref="HistoryEvent"/> </returns>
		public virtual HistoryEvent createHistoryEvent(HistoryEventProducer producer)
		{
		  return null;
		}

		public virtual IList<HistoryEvent> createHistoryEvents(HistoryEventProducer producer)
		{
		  return Collections.emptyList();
		}
	  }


	  /// <summary>
	  /// Process an <seealso cref="HistoryEvent"/> and handle them directly after creation.
	  /// The <seealso cref="HistoryEvent"/> is created with the help of the given
	  /// <seealso cref="HistoryEventCreator"/> implementation.
	  /// </summary>
	  /// <param name="creator"> the creator is used to create the <seealso cref="HistoryEvent"/> which should be thrown </param>
	  public static void processHistoryEvents(HistoryEventCreator creator)
	  {
		HistoryEventProducer historyEventProducer = Context.ProcessEngineConfiguration.HistoryEventProducer;
		HistoryEventHandler historyEventHandler = Context.ProcessEngineConfiguration.HistoryEventHandler;

		HistoryEvent singleEvent = creator.createHistoryEvent(historyEventProducer);
		if (singleEvent != null)
		{
		  historyEventHandler.handleEvent(singleEvent);
		}

		IList<HistoryEvent> eventList = creator.createHistoryEvents(historyEventProducer);
		historyEventHandler.handleEvents(eventList);
	  }
	}

}