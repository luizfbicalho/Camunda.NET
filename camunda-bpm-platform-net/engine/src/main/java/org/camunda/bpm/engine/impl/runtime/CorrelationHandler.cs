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
namespace org.camunda.bpm.engine.impl.runtime
{

	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;

	/// <summary>
	/// @author Thorben Lindhauer
	/// @author Daniel Meyer
	/// @author Michael Scholz
	/// </summary>
	public interface CorrelationHandler
	{

	  /// <summary>
	  /// Correlate the given message to either a waiting execution or a process
	  /// definition with a message start event.
	  /// </summary>
	  /// <param name="correlationSet">
	  ///          any of its members may be <code>null</code>
	  /// </param>
	  /// <returns> the matched correlation target or <code>null</code> if the message
	  ///         could not be correlated. </returns>
	  CorrelationHandlerResult correlateMessage(CommandContext commandContext, string messageName, CorrelationSet correlationSet);

	  /// <summary>
	  /// Correlate the given message to all waiting executions and all process
	  /// definitions which have a message start event.
	  /// </summary>
	  /// <param name="correlationSet">
	  ///          any of its members may be <code>null</code>
	  /// </param>
	  /// <returns> all matched correlation targets or an empty List if the message
	  ///         could not be correlated. </returns>
	  IList<CorrelationHandlerResult> correlateMessages(CommandContext commandContext, string messageName, CorrelationSet correlationSet);

	  /// <summary>
	  /// Correlate the given message to process definitions with a message start
	  /// event.
	  /// </summary>
	  /// <param name="correlationSet">
	  ///          any of its members may be <code>null</code>
	  /// </param>
	  /// <returns> the matched correlation targets or an empty list if the message
	  ///         could not be correlated. </returns>
	  IList<CorrelationHandlerResult> correlateStartMessages(CommandContext commandContext, string messageName, CorrelationSet correlationSet);

	}

}