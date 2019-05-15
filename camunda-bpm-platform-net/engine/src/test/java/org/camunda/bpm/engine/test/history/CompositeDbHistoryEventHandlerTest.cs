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
	using CompositeDbHistoryEventHandler = org.camunda.bpm.engine.impl.history.handler.CompositeDbHistoryEventHandler;
	using HistoryEventHandler = org.camunda.bpm.engine.impl.history.handler.HistoryEventHandler;

	/// <summary>
	/// @author Alexander Tyatenkov
	/// 
	/// </summary>
	public class CompositeDbHistoryEventHandlerTest : AbstractCompositeHistoryEventHandlerTest
	{

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoryLevelTest.bpmn20.xml"})]
	  public virtual void testCompositeDbHistoryEventHandlerNonArgumentConstructor()
	  {
		processEngineConfiguration.HistoryEventHandler = new CompositeDbHistoryEventHandler();

		startProcessAndCompleteUserTask();

		assertEquals(0, countCustomHistoryEventHandler);
		assertEquals(2, historyService.createHistoricDetailQuery().count());
	  }

	  public virtual void testCompositeDbHistoryEventHandlerNonArgumentConstructorAddNullEvent()
	  {
		CompositeDbHistoryEventHandler compositeDbHistoryEventHandler = new CompositeDbHistoryEventHandler();
		try
		{
		  compositeDbHistoryEventHandler.add(null);
		  fail("NullValueException expected");
		}
		catch (NullValueException e)
		{
		  assertTextPresent("History event handler is null", e.Message);
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoryLevelTest.bpmn20.xml"})]
	  public virtual void testCompositeDbHistoryEventHandlerNonArgumentConstructorAddNotNullEvent()
	  {
		CompositeDbHistoryEventHandler compositeDbHistoryEventHandler = new CompositeDbHistoryEventHandler();
		compositeDbHistoryEventHandler.add(new CustomDbHistoryEventHandler(this));
		processEngineConfiguration.HistoryEventHandler = compositeDbHistoryEventHandler;

		startProcessAndCompleteUserTask();

		assertEquals(2, countCustomHistoryEventHandler);
		assertEquals(2, historyService.createHistoricDetailQuery().count());
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/history/HistoryLevelTest.bpmn20.xml" })]
	  public virtual void testCompositeDbHistoryEventHandlerNonArgumentConstructorAddTwoNotNullEvents()
	  {
		CompositeDbHistoryEventHandler compositeDbHistoryEventHandler = new CompositeDbHistoryEventHandler();
		compositeDbHistoryEventHandler.add(new CustomDbHistoryEventHandler(this));
		compositeDbHistoryEventHandler.add(new CustomDbHistoryEventHandler(this));
		processEngineConfiguration.HistoryEventHandler = compositeDbHistoryEventHandler;

		startProcessAndCompleteUserTask();

		assertEquals(4, countCustomHistoryEventHandler);
		assertEquals(2, historyService.createHistoricDetailQuery().count());
	  }

	  public virtual void testCompositeDbHistoryEventHandlerArgumentConstructorWithNullVarargs()
	  {
		HistoryEventHandler historyEventHandler = null;
		try
		{
		  new CompositeDbHistoryEventHandler(historyEventHandler);
		  fail("NullValueException expected");
		}
		catch (NullValueException e)
		{
		  assertTextPresent("History event handler is null", e.Message);
		}
	  }

	  public virtual void testCompositeDbHistoryEventHandlerArgumentConstructorWithNullTwoVarargs()
	  {
		try
		{
		  new CompositeDbHistoryEventHandler(null, null);
		  fail("NullValueException expected");
		}
		catch (NullValueException e)
		{
		  assertTextPresent("History event handler is null", e.Message);
		}
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/history/HistoryLevelTest.bpmn20.xml" })]
	  public virtual void testCompositeDbHistoryEventHandlerArgumentConstructorWithNotNullVarargsOneEvent()
	  {
		CompositeDbHistoryEventHandler compositeDbHistoryEventHandler = new CompositeDbHistoryEventHandler(new CustomDbHistoryEventHandler(this));
		processEngineConfiguration.HistoryEventHandler = compositeDbHistoryEventHandler;

		startProcessAndCompleteUserTask();

		assertEquals(2, countCustomHistoryEventHandler);
		assertEquals(2, historyService.createHistoricDetailQuery().count());
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/history/HistoryLevelTest.bpmn20.xml" })]
	  public virtual void testCompositeDbHistoryEventHandlerArgumentConstructorWithNotNullVarargsTwoEvents()
	  {
		CompositeDbHistoryEventHandler compositeDbHistoryEventHandler = new CompositeDbHistoryEventHandler(new CustomDbHistoryEventHandler(this), new CustomDbHistoryEventHandler(this));
		processEngineConfiguration.HistoryEventHandler = compositeDbHistoryEventHandler;

		startProcessAndCompleteUserTask();

		assertEquals(4, countCustomHistoryEventHandler);
		assertEquals(2, historyService.createHistoricDetailQuery().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoryLevelTest.bpmn20.xml"})]
	  public virtual void testCompositeDbHistoryEventHandlerArgumentConstructorWithEmptyList()
	  {
		CompositeDbHistoryEventHandler compositeDbHistoryEventHandler = new CompositeDbHistoryEventHandler(new List<HistoryEventHandler>());
		processEngineConfiguration.HistoryEventHandler = compositeDbHistoryEventHandler;

		startProcessAndCompleteUserTask();

		assertEquals(0, countCustomHistoryEventHandler);
		assertEquals(2, historyService.createHistoricDetailQuery().count());
	  }

	  public virtual void testCompositeDbHistoryEventHandlerArgumentConstructorWithNotEmptyListNullTwoEvents()
	  {
		// prepare the list with two null events
		IList<HistoryEventHandler> historyEventHandlers = new List<HistoryEventHandler>();
		historyEventHandlers.Add(null);
		historyEventHandlers.Add(null);

		try
		{
		  new CompositeDbHistoryEventHandler(historyEventHandlers);
		  fail("NullValueException expected");
		}
		catch (NullValueException e)
		{
		  assertTextPresent("History event handler is null", e.Message);
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoryLevelTest.bpmn20.xml"})]
	  public virtual void testCompositeDbHistoryEventHandlerArgumentConstructorWithNotEmptyListNotNullTwoEvents()
	  {
		// prepare the list with two events
		IList<HistoryEventHandler> historyEventHandlers = new List<HistoryEventHandler>();
		historyEventHandlers.Add(new CustomDbHistoryEventHandler(this));
		historyEventHandlers.Add(new CustomDbHistoryEventHandler(this));

		CompositeDbHistoryEventHandler compositeDbHistoryEventHandler = new CompositeDbHistoryEventHandler(historyEventHandlers);
		processEngineConfiguration.HistoryEventHandler = compositeDbHistoryEventHandler;

		startProcessAndCompleteUserTask();

		assertEquals(4, countCustomHistoryEventHandler);
		assertEquals(2, historyService.createHistoricDetailQuery().count());
	  }

	}

}