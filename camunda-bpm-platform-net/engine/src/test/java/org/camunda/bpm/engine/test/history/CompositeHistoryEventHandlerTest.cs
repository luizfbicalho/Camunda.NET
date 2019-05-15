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

	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using CompositeHistoryEventHandler = org.camunda.bpm.engine.impl.history.handler.CompositeHistoryEventHandler;
	using DbHistoryEventHandler = org.camunda.bpm.engine.impl.history.handler.DbHistoryEventHandler;
	using HistoryEventHandler = org.camunda.bpm.engine.impl.history.handler.HistoryEventHandler;

	/// <summary>
	/// @author Alexander Tyatenkov
	/// 
	/// </summary>
	public class CompositeHistoryEventHandlerTest : AbstractCompositeHistoryEventHandlerTest
	{

	  [Deployment(resources : { "org/camunda/bpm/engine/test/history/HistoryLevelTest.bpmn20.xml" })]
	  public virtual void testDefaultHistoryEventHandler()
	  {
		// use default DbHistoryEventHandler
		processEngineConfiguration.HistoryEventHandler = new DbHistoryEventHandler();

		startProcessAndCompleteUserTask();

		assertEquals(0, countCustomHistoryEventHandler);
		assertEquals(2, historyService.createHistoricDetailQuery().count());
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/history/HistoryLevelTest.bpmn20.xml" })]
	  public virtual void testCompositeHistoryEventHandlerNonArgumentConstructor()
	  {
		processEngineConfiguration.HistoryEventHandler = new CompositeHistoryEventHandler();

		startProcessAndCompleteUserTask();

		assertEquals(0, countCustomHistoryEventHandler);
		assertEquals(0, historyService.createHistoricDetailQuery().count());
	  }

	  public virtual void testCompositeHistoryEventHandlerNonArgumentConstructorAddNullEvent()
	  {
		CompositeHistoryEventHandler compositeHistoryEventHandler = new CompositeHistoryEventHandler();
		try
		{
		  compositeHistoryEventHandler.add(null);
		  fail("NullValueException expected");
		}
		catch (NullValueException e)
		{
		  assertTextPresent("History event handler is null", e.Message);
		}
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/history/HistoryLevelTest.bpmn20.xml" })]
	  public virtual void testCompositeHistoryEventHandlerNonArgumentConstructorAddNotNullEvent()
	  {
		CompositeHistoryEventHandler compositeHistoryEventHandler = new CompositeHistoryEventHandler();
		compositeHistoryEventHandler.add(new CustomDbHistoryEventHandler(this));
		processEngineConfiguration.HistoryEventHandler = compositeHistoryEventHandler;

		startProcessAndCompleteUserTask();

		assertEquals(2, countCustomHistoryEventHandler);
		assertEquals(0, historyService.createHistoricDetailQuery().count());
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/history/HistoryLevelTest.bpmn20.xml" })]
	  public virtual void testCompositeHistoryEventHandlerNonArgumentConstructorAddNotNullTwoEvents()
	  {
		CompositeHistoryEventHandler compositeHistoryEventHandler = new CompositeHistoryEventHandler();
		compositeHistoryEventHandler.add(new CustomDbHistoryEventHandler(this));
		compositeHistoryEventHandler.add(new DbHistoryEventHandler());
		processEngineConfiguration.HistoryEventHandler = compositeHistoryEventHandler;

		startProcessAndCompleteUserTask();

		assertEquals(2, countCustomHistoryEventHandler);
		assertEquals(2, historyService.createHistoricDetailQuery().count());
	  }

	  public virtual void testCompositeHistoryEventHandlerArgumentConstructorWithNullVarargs()
	  {
		HistoryEventHandler historyEventHandler = null;
		try
		{
		  new CompositeHistoryEventHandler(historyEventHandler);
		  fail("NullValueException expected");
		}
		catch (NullValueException e)
		{
		  assertTextPresent("History event handler is null", e.Message);
		}
	  }

	  public virtual void testCompositeHistoryEventHandlerArgumentConstructorWithNullTwoVarargs()
	  {
		try
		{
		  new CompositeHistoryEventHandler(null, null);
		  fail("NullValueException expected");
		}
		catch (NullValueException e)
		{
		  assertTextPresent("History event handler is null", e.Message);
		}
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/history/HistoryLevelTest.bpmn20.xml" })]
	  public virtual void testCompositeHistoryEventHandlerArgumentConstructorWithNotNullVarargsOneEvent()
	  {
		CompositeHistoryEventHandler compositeHistoryEventHandler = new CompositeHistoryEventHandler(new CustomDbHistoryEventHandler(this));
		processEngineConfiguration.HistoryEventHandler = compositeHistoryEventHandler;

		startProcessAndCompleteUserTask();

		assertEquals(2, countCustomHistoryEventHandler);
		assertEquals(0, historyService.createHistoricDetailQuery().count());
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/history/HistoryLevelTest.bpmn20.xml" })]
	  public virtual void testCompositeHistoryEventHandlerArgumentConstructorWithNotNullVarargsTwoEvents()
	  {
		CompositeHistoryEventHandler compositeHistoryEventHandler = new CompositeHistoryEventHandler(new CustomDbHistoryEventHandler(this), new DbHistoryEventHandler());
		processEngineConfiguration.HistoryEventHandler = compositeHistoryEventHandler;

		startProcessAndCompleteUserTask();

		assertEquals(2, countCustomHistoryEventHandler);
		assertEquals(2, historyService.createHistoricDetailQuery().count());
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/history/HistoryLevelTest.bpmn20.xml" })]
	  public virtual void testCompositeHistoryEventHandlerArgumentConstructorWithEmptyList()
	  {
		CompositeHistoryEventHandler compositeHistoryEventHandler = new CompositeHistoryEventHandler(new List<HistoryEventHandler>());
		processEngineConfiguration.HistoryEventHandler = compositeHistoryEventHandler;

		startProcessAndCompleteUserTask();

		assertEquals(0, countCustomHistoryEventHandler);
		assertEquals(0, historyService.createHistoricDetailQuery().count());
	  }

	  public virtual void testCompositeHistoryEventHandlerArgumentConstructorWithNotEmptyListNullTwoEvents()
	  {
		// prepare the list with two null events
		IList<HistoryEventHandler> historyEventHandlers = new List<HistoryEventHandler>();
		historyEventHandlers.Add(null);
		historyEventHandlers.Add(null);

		try
		{
		  new CompositeHistoryEventHandler(historyEventHandlers);
		  fail("NullValueException expected");
		}
		catch (NullValueException e)
		{
		  assertTextPresent("History event handler is null", e.Message);
		}
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/history/HistoryLevelTest.bpmn20.xml" })]
	  public virtual void testCompositeHistoryEventHandlerArgumentConstructorWithNotEmptyListNotNullTwoEvents()
	  {
		// prepare the list with two events
		IList<HistoryEventHandler> historyEventHandlers = new List<HistoryEventHandler>();
		historyEventHandlers.Add(new CustomDbHistoryEventHandler(this));
		historyEventHandlers.Add(new DbHistoryEventHandler());

		CompositeHistoryEventHandler compositeHistoryEventHandler = new CompositeHistoryEventHandler(historyEventHandlers);
		processEngineConfiguration.HistoryEventHandler = compositeHistoryEventHandler;

		startProcessAndCompleteUserTask();

		assertEquals(2, countCustomHistoryEventHandler);
		assertEquals(2, historyService.createHistoricDetailQuery().count());
	  }

	}

}