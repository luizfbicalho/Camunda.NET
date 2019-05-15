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
namespace org.camunda.bpm.engine.test.history
{

	using HistoricVariableUpdateEventEntity = org.camunda.bpm.engine.impl.history.@event.HistoricVariableUpdateEventEntity;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;
	using HistoryEventHandler = org.camunda.bpm.engine.impl.history.handler.HistoryEventHandler;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Task = org.camunda.bpm.engine.task.Task;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public abstract class AbstractCompositeHistoryEventHandlerTest : PluggableProcessEngineTestCase
	{

	  protected internal HistoryEventHandler originalHistoryEventHandler;

	  /// <summary>
	  /// The counter used to check the amount of triggered events.
	  /// </summary>
	  protected internal int countCustomHistoryEventHandler;

	  /// <summary>
	  /// Perform common setup.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void setUp() throws Exception
	  protected internal override void setUp()
	  {
		base.setUp();
		// save current history event handler
		originalHistoryEventHandler = processEngineConfiguration.HistoryEventHandler;
		// clear the event counter
		countCustomHistoryEventHandler = 0;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		base.tearDown();
		// reset original history event handler
		processEngineConfiguration.HistoryEventHandler = originalHistoryEventHandler;
	  }

	  /// <summary>
	  /// The helper method to execute the test task.
	  /// </summary>
	  protected internal virtual void startProcessAndCompleteUserTask()
	  {
		runtimeService.startProcessInstanceByKey("HistoryLevelTest");
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);
	  }

	  /// <summary>
	  /// A <seealso cref="HistoryEventHandler"/> implementation to count the history events.
	  /// </summary>
	  protected internal class CustomDbHistoryEventHandler : HistoryEventHandler
	  {
		  private readonly AbstractCompositeHistoryEventHandlerTest outerInstance;

		  public CustomDbHistoryEventHandler(AbstractCompositeHistoryEventHandlerTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		public virtual void handleEvent(HistoryEvent historyEvent)
		{
		  // take into account only variable related events
		  if (historyEvent is HistoricVariableUpdateEventEntity)
		  {
			// emulate the history event processing and persisting
			outerInstance.countCustomHistoryEventHandler++;
		  }
		}

		public virtual void handleEvents(IList<HistoryEvent> historyEvents)
		{
		  foreach (HistoryEvent historyEvent in historyEvents)
		  {
			handleEvent(historyEvent);
		  }
		}

	  }

	}

}