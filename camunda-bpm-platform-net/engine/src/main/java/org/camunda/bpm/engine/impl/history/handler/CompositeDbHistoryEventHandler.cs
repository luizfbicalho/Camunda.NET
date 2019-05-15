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

	/// <summary>
	/// A <seealso cref="CompositeHistoryEventHandler"/> implementation which additionally adds
	/// to the list of <seealso cref="HistoryEventHandler"/> the <seealso cref="DbHistoryEventHandler"/>
	/// which persists events to a database.
	/// 
	/// @author Alexander Tyatenkov
	/// 
	/// </summary>
	public class CompositeDbHistoryEventHandler : CompositeHistoryEventHandler
	{

	  /// <summary>
	  /// Non-argument constructor that adds <seealso cref="DbHistoryEventHandler"/> to the
	  /// list of <seealso cref="HistoryEventHandler"/>.
	  /// </summary>
	  public CompositeDbHistoryEventHandler() : base()
	  {
		addDefaultDbHistoryEventHandler();
	  }

	  /// <summary>
	  /// Constructor that takes a varargs parameter <seealso cref="HistoryEventHandler"/> that
	  /// consume the event and adds <seealso cref="DbHistoryEventHandler"/> to the list of
	  /// <seealso cref="HistoryEventHandler"/>.
	  /// </summary>
	  /// <param name="historyEventHandlers">
	  ///          the list of <seealso cref="HistoryEventHandler"/> that consume the event. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public CompositeDbHistoryEventHandler(final HistoryEventHandler... historyEventHandlers)
	  public CompositeDbHistoryEventHandler(params HistoryEventHandler[] historyEventHandlers) : base(historyEventHandlers)
	  {
		addDefaultDbHistoryEventHandler();
	  }

	  /// <summary>
	  /// Constructor that takes a list of <seealso cref="HistoryEventHandler"/> that consume
	  /// the event and adds <seealso cref="DbHistoryEventHandler"/> to the list of
	  /// <seealso cref="HistoryEventHandler"/>.
	  /// </summary>
	  /// <param name="historyEventHandlers">
	  ///          the list of <seealso cref="HistoryEventHandler"/> that consume the event. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public CompositeDbHistoryEventHandler(final java.util.List<HistoryEventHandler> historyEventHandlers)
	  public CompositeDbHistoryEventHandler(IList<HistoryEventHandler> historyEventHandlers) : base(historyEventHandlers)
	  {
		addDefaultDbHistoryEventHandler();
	  }

	  /// <summary>
	  /// Add <seealso cref="DbHistoryEventHandler"/> to the list of
	  /// <seealso cref="HistoryEventHandler"/>.
	  /// </summary>
	  private void addDefaultDbHistoryEventHandler()
	  {
		historyEventHandlers.Add(new DbHistoryEventHandler());
	  }

	}

}