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
	using DelegateTask = org.camunda.bpm.engine.@delegate.DelegateTask;
	using TaskListener = org.camunda.bpm.engine.@delegate.TaskListener;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;
	using HistoryEventHandler = org.camunda.bpm.engine.impl.history.handler.HistoryEventHandler;
	using HistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.HistoryEventProducer;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;

	/// <summary>
	/// <para>A <seealso cref="TaskListener"/> implementation that delegates to a
	/// <seealso cref="HistoryEventProducer"/>.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </para>
	/// </summary>
	public abstract class HistoryTaskListener : TaskListener
	{

	  protected internal readonly HistoryEventProducer eventProducer;
	  protected internal HistoryLevel historyLevel;

	  public HistoryTaskListener(HistoryEventProducer historyEventProducer, HistoryLevel historyLevel)
	  {
		this.eventProducer = historyEventProducer;
		this.historyLevel = historyLevel;
	  }

	  public virtual void notify(DelegateTask task)
	  {

		// get the event handler
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.history.handler.HistoryEventHandler historyEventHandler = org.camunda.bpm.engine.impl.context.Context.getProcessEngineConfiguration().getHistoryEventHandler();
		HistoryEventHandler historyEventHandler = Context.ProcessEngineConfiguration.HistoryEventHandler;

		ExecutionEntity execution = ((TaskEntity) task).getExecution();

		if (execution != null)
		{

		  // delegate creation of the history event to the producer
		  HistoryEvent historyEvent = createHistoryEvent(task, execution);

		  if (historyEvent != null)
		  {
			// pass the event to the handler
			historyEventHandler.handleEvent(historyEvent);
		  }

		}

	  }

	  protected internal abstract HistoryEvent createHistoryEvent(DelegateTask task, ExecutionEntity execution);

	}

}