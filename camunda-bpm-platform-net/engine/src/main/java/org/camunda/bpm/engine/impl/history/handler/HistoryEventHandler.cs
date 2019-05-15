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
namespace org.camunda.bpm.engine.impl.history.handler
{

	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;

	/// <summary>
	/// <para>The interface for implementing an history event handler.</para>
	/// 
	/// <para>The <seealso cref="HistoryEventHandler"/> is responsible for consuming the event. Many different
	/// implementations of this interface can be imagined. Some implementations might persist the
	/// event to a database, others might persist the event to a message queue and handle it
	/// asynchronously.</para>
	/// 
	/// <para>The default implementation of this interface is <seealso cref="DbHistoryEventHandler"/> which
	/// persists events to a database.</para>
	/// 
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface HistoryEventHandler
	{

	  /// <summary>
	  /// Called by the process engine when an history event is fired.
	  /// </summary>
	  /// <param name="historyEvent"> the <seealso cref="HistoryEvent"/> that is about to be fired. </param>
	  void handleEvent(HistoryEvent historyEvent);

	  /// <summary>
	  /// Called by the process engine when an history event is fired.
	  /// </summary>
	  /// <param name="historyEvents"> the <seealso cref="HistoryEvent"/> that is about to be fired. </param>
	  void handleEvents(IList<HistoryEvent> historyEvents);

	}

}