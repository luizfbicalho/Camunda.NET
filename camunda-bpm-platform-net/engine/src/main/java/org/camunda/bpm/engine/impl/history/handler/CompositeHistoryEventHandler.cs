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
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;

	/// <summary>
	/// A <seealso cref="HistoryEventHandler"/> implementation which delegates to a list of
	/// <seealso cref="HistoryEventHandler"/>.
	/// 
	/// @author Alexander Tyatenkov
	/// 
	/// </summary>
	public class CompositeHistoryEventHandler : HistoryEventHandler
	{

	  /// <summary>
	  /// The list of <seealso cref="HistoryEventHandler"/> which consume the event.
	  /// </summary>
	  protected internal readonly IList<HistoryEventHandler> historyEventHandlers = new List<HistoryEventHandler>();

	  /// <summary>
	  /// Non-argument constructor for default initialization.
	  /// </summary>
	  public CompositeHistoryEventHandler()
	  {
	  }

	  /// <summary>
	  /// Constructor that takes a varargs parameter <seealso cref="HistoryEventHandler"/> that
	  /// consume the event.
	  /// </summary>
	  /// <param name="historyEventHandlers">
	  ///          the list of <seealso cref="HistoryEventHandler"/> that consume the event. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public CompositeHistoryEventHandler(final HistoryEventHandler... historyEventHandlers)
	  public CompositeHistoryEventHandler(params HistoryEventHandler[] historyEventHandlers)
	  {
		initializeHistoryEventHandlers(Arrays.asList(historyEventHandlers));
	  }

	  /// <summary>
	  /// Constructor that takes a list of <seealso cref="HistoryEventHandler"/> that consume
	  /// the event.
	  /// </summary>
	  /// <param name="historyEventHandlers">
	  ///          the list of <seealso cref="HistoryEventHandler"/> that consume the event. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public CompositeHistoryEventHandler(final java.util.List<HistoryEventHandler> historyEventHandlers)
	  public CompositeHistoryEventHandler(IList<HistoryEventHandler> historyEventHandlers)
	  {
		initializeHistoryEventHandlers(historyEventHandlers);
	  }

	  /// <summary>
	  /// Initialize <seealso cref="historyEventHandlers"/> with data transfered from constructor
	  /// </summary>
	  /// <param name="historyEventHandlers"> </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void initializeHistoryEventHandlers(final java.util.List<HistoryEventHandler> historyEventHandlers)
	  private void initializeHistoryEventHandlers(IList<HistoryEventHandler> historyEventHandlers)
	  {
		EnsureUtil.ensureNotNull("History event handler", historyEventHandlers);
		foreach (HistoryEventHandler historyEventHandler in historyEventHandlers)
		{
		  EnsureUtil.ensureNotNull("History event handler", historyEventHandler);
		  this.historyEventHandlers.Add(historyEventHandler);
		}
	  }

	  /// <summary>
	  /// Adds the <seealso cref="HistoryEventHandler"/> to the list of
	  /// <seealso cref="HistoryEventHandler"/> that consume the event.
	  /// </summary>
	  /// <param name="historyEventHandler">
	  ///          the <seealso cref="HistoryEventHandler"/> that consume the event. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void add(final HistoryEventHandler historyEventHandler)
	  public virtual void add(HistoryEventHandler historyEventHandler)
	  {
		EnsureUtil.ensureNotNull("History event handler", historyEventHandler);
		historyEventHandlers.Add(historyEventHandler);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public void handleEvent(final org.camunda.bpm.engine.impl.history.event.HistoryEvent historyEvent)
	  public virtual void handleEvent(HistoryEvent historyEvent)
	  {
		foreach (HistoryEventHandler historyEventHandler in historyEventHandlers)
		{
		  historyEventHandler.handleEvent(historyEvent);
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public void handleEvents(final java.util.List<org.camunda.bpm.engine.impl.history.event.HistoryEvent> historyEvents)
	  public virtual void handleEvents(IList<HistoryEvent> historyEvents)
	  {
		foreach (HistoryEvent historyEvent in historyEvents)
		{
		  handleEvent(historyEvent);
		}
	  }

	}

}