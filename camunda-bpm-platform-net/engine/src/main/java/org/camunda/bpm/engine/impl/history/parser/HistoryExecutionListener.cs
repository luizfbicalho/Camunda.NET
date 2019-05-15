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
namespace org.camunda.bpm.engine.impl.history.parser
{
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;
	using HistoryEventHandler = org.camunda.bpm.engine.impl.history.handler.HistoryEventHandler;
	using HistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.HistoryEventProducer;

	/// <summary>
	/// <para>An <seealso cref="ExecutionListener"/> implementation that delegates to a
	/// <seealso cref="HistoryEventProducer"/>.
	/// 
	/// </para>
	/// <para>This allows plugging the history as an execution listener into process
	/// execution and make sure history events are generated as we move through the
	/// process.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </para>
	/// </summary>
	public abstract class HistoryExecutionListener : ExecutionListener
	{

	  protected internal readonly HistoryEventProducer eventProducer;
	  protected internal HistoryLevel historyLevel;

	  public HistoryExecutionListener(HistoryEventProducer historyEventProducer, HistoryLevel historyLevel)
	  {
		this.eventProducer = historyEventProducer;
		this.historyLevel = historyLevel;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
	  public virtual void notify(DelegateExecution execution)
	  {

		// get the event handler
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.history.handler.HistoryEventHandler historyEventHandler = org.camunda.bpm.engine.impl.context.Context.getProcessEngineConfiguration().getHistoryEventHandler();
		HistoryEventHandler historyEventHandler = Context.ProcessEngineConfiguration.HistoryEventHandler;

		// delegate creation of the history event to the producer
		HistoryEvent historyEvent = createHistoryEvent(execution);

		if (historyEvent != null)
		{
		  // pass the event to the handler
		  historyEventHandler.handleEvent(historyEvent);
		}

	  }

	  protected internal abstract HistoryEvent createHistoryEvent(DelegateExecution execution);

	}

}