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
namespace org.camunda.bpm.engine.test.standalone.pvm
{

	using ProcessDefinitionBuilder = org.camunda.bpm.engine.impl.pvm.ProcessDefinitionBuilder;
	using PvmProcessDefinition = org.camunda.bpm.engine.impl.pvm.PvmProcessDefinition;
	using PvmProcessInstance = org.camunda.bpm.engine.impl.pvm.PvmProcessInstance;
	using PvmTestCase = org.camunda.bpm.engine.impl.test.PvmTestCase;
	using Automatic = org.camunda.bpm.engine.test.standalone.pvm.activities.Automatic;
	using EmbeddedSubProcess = org.camunda.bpm.engine.test.standalone.pvm.activities.EmbeddedSubProcess;
	using End = org.camunda.bpm.engine.test.standalone.pvm.activities.End;
	using ParallelGateway = org.camunda.bpm.engine.test.standalone.pvm.activities.ParallelGateway;
	using WaitState = org.camunda.bpm.engine.test.standalone.pvm.activities.WaitState;



	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class PvmEventTest : PvmTestCase
	{

	  /// <summary>
	  /// +-------+   +-----+
	  /// | start |-->| end |
	  /// +-------+   +-----+
	  /// </summary>
	  public virtual void testStartEndEvents()
	  {
		EventCollector eventCollector = new EventCollector();

		PvmProcessDefinition processDefinition = (new ProcessDefinitionBuilder("events")).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_START, eventCollector).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_END, eventCollector).createActivity("start").initial().behavior(new Automatic()).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_START, eventCollector).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_END, eventCollector).startTransition("end").executionListener(eventCollector).endTransition().endActivity().createActivity("end").behavior(new End()).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_START, eventCollector).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_END, eventCollector).endActivity().buildProcessDefinition();

		PvmProcessInstance processInstance = processDefinition.createProcessInstance();
		processInstance.start();

		IList<string> expectedEvents = new List<string>();
		expectedEvents.Add("start on ProcessDefinition(events)");
		expectedEvents.Add("start on Activity(start)");
		expectedEvents.Add("end on Activity(start)");
		expectedEvents.Add("take on (start)-->(end)");
		expectedEvents.Add("start on Activity(end)");
		expectedEvents.Add("end on Activity(end)");
		expectedEvents.Add("end on ProcessDefinition(events)");

		assertEquals("expected " + expectedEvents + ", but was \n" + eventCollector + "\n", expectedEvents, eventCollector.events);
	  }

	  /// <summary>
	  ///           +------------------------------+
	  /// +-----+   | +-----------+   +----------+ |   +---+
	  /// |start|-->| |startInside|-->|endInsdide| |-->|end|
	  /// +-----+   | +-----------+   +----------+ |   +---+
	  ///           +------------------------------+
	  /// </summary>
	  public virtual void testEmbeddedSubProcessEvents()
	  {
		EventCollector eventCollector = new EventCollector();

		PvmProcessDefinition processDefinition = (new ProcessDefinitionBuilder("events")).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_START, eventCollector).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_END, eventCollector).createActivity("start").initial().behavior(new Automatic()).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_START, eventCollector).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_END, eventCollector).transition("embeddedsubprocess").endActivity().createActivity("embeddedsubprocess").scope().behavior(new EmbeddedSubProcess()).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_START, eventCollector).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_END, eventCollector).createActivity("startInside").behavior(new Automatic()).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_START, eventCollector).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_END, eventCollector).transition("endInside").endActivity().createActivity("endInside").behavior(new End()).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_START, eventCollector).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_END, eventCollector).endActivity().transition("end").endActivity().createActivity("end").behavior(new End()).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_START, eventCollector).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_END, eventCollector).endActivity().buildProcessDefinition();

		PvmProcessInstance processInstance = processDefinition.createProcessInstance();
		processInstance.start();

		IList<string> expectedEvents = new List<string>();
		expectedEvents.Add("start on ProcessDefinition(events)");
		expectedEvents.Add("start on Activity(start)");
		expectedEvents.Add("end on Activity(start)");
		expectedEvents.Add("start on Activity(embeddedsubprocess)");
		expectedEvents.Add("start on Activity(startInside)");
		expectedEvents.Add("end on Activity(startInside)");
		expectedEvents.Add("start on Activity(endInside)");
		expectedEvents.Add("end on Activity(endInside)");
		expectedEvents.Add("end on Activity(embeddedsubprocess)");
		expectedEvents.Add("start on Activity(end)");
		expectedEvents.Add("end on Activity(end)");
		expectedEvents.Add("end on ProcessDefinition(events)");

		assertEquals("expected " + expectedEvents + ", but was \n" + eventCollector + "\n", expectedEvents, eventCollector.events);
	  }


	  /// <summary>
	  ///                   +--+
	  ///              +--->|c1|---+
	  ///              |    +--+   |
	  ///              |           v
	  /// +-----+   +----+       +----+   +---+
	  /// |start|-->|fork|       |join|-->|end|
	  /// +-----+   +----+       +----+   +---+
	  ///              |           ^
	  ///              |    +--+   |
	  ///              +--->|c2|---+
	  ///                   +--+
	  /// </summary>
	  public virtual void testSimpleAutmaticConcurrencyEvents()
	  {
		EventCollector eventCollector = new EventCollector();

		PvmProcessDefinition processDefinition = (new ProcessDefinitionBuilder("events")).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_START, eventCollector).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_END, eventCollector).createActivity("start").initial().behavior(new Automatic()).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_START, eventCollector).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_END, eventCollector).transition("fork").endActivity().createActivity("fork").behavior(new ParallelGateway()).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_START, eventCollector).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_END, eventCollector).transition("c1").transition("c2").endActivity().createActivity("c1").behavior(new Automatic()).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_START, eventCollector).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_END, eventCollector).transition("join").endActivity().createActivity("c2").behavior(new Automatic()).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_START, eventCollector).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_END, eventCollector).transition("join").endActivity().createActivity("join").behavior(new ParallelGateway()).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_START, eventCollector).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_END, eventCollector).transition("end").endActivity().createActivity("end").behavior(new End()).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_START, eventCollector).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_END, eventCollector).endActivity().buildProcessDefinition();

		PvmProcessInstance processInstance = processDefinition.createProcessInstance();
		processInstance.start();

		IList<string> expectedEvents = new List<string>();
		expectedEvents.Add("start on ProcessDefinition(events)");
		expectedEvents.Add("start on Activity(start)");
		expectedEvents.Add("end on Activity(start)");
		expectedEvents.Add("start on Activity(fork)");
		expectedEvents.Add("end on Activity(fork)");
		expectedEvents.Add("start on Activity(c2)");
		expectedEvents.Add("end on Activity(c2)");
		expectedEvents.Add("start on Activity(join)");
		expectedEvents.Add("start on Activity(c1)");
		expectedEvents.Add("end on Activity(c1)");
		expectedEvents.Add("start on Activity(join)");
		expectedEvents.Add("end on Activity(join)");
		expectedEvents.Add("end on Activity(join)");
		expectedEvents.Add("start on Activity(end)");
		expectedEvents.Add("end on Activity(end)");
		expectedEvents.Add("end on ProcessDefinition(events)");

		assertEquals("expected " + expectedEvents + ", but was \n" + eventCollector + "\n", expectedEvents, eventCollector.events);
	  }

	  /// <summary>
	  ///           +-----------------------------------------------+
	  /// +-----+   | +-----------+   +------------+   +----------+ |   +---+
	  /// |start|-->| |startInside|-->| taskInside |-->|endInsdide| |-->|end|
	  /// +-----+   | +-----------+   +------------+   +----------+ |   +---+
	  ///           +-----------------------------------------------+
	  /// </summary>
	  public virtual void testEmbeddedSubProcessEventsDelete()
	  {
		EventCollector eventCollector = new EventCollector();

		PvmProcessDefinition processDefinition = (new ProcessDefinitionBuilder("events")).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_START, eventCollector).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_END, eventCollector).createActivity("start").initial().behavior(new Automatic()).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_START, eventCollector).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_END, eventCollector).transition("embeddedsubprocess").endActivity().createActivity("embeddedsubprocess").scope().behavior(new EmbeddedSubProcess()).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_START, eventCollector).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_END, eventCollector).createActivity("startInside").behavior(new Automatic()).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_START, eventCollector).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_END, eventCollector).transition("taskInside").endActivity().createActivity("taskInside").behavior(new WaitState()).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_START, eventCollector).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_END, eventCollector).transition("endInside").endActivity().createActivity("endInside").behavior(new End()).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_START, eventCollector).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_END, eventCollector).endActivity().transition("end").endActivity().createActivity("end").behavior(new End()).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_START, eventCollector).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_END, eventCollector).endActivity().buildProcessDefinition();

		PvmProcessInstance processInstance = processDefinition.createProcessInstance();
		processInstance.start();

		processInstance.deleteCascade("");

		IList<string> expectedEvents = new List<string>();
		expectedEvents.Add("start on ProcessDefinition(events)");
		expectedEvents.Add("start on Activity(start)");
		expectedEvents.Add("end on Activity(start)");
		expectedEvents.Add("start on Activity(embeddedsubprocess)");
		expectedEvents.Add("start on Activity(startInside)");
		expectedEvents.Add("end on Activity(startInside)");
		expectedEvents.Add("start on Activity(taskInside)");
		expectedEvents.Add("end on Activity(taskInside)");
		expectedEvents.Add("end on Activity(embeddedsubprocess)");
		expectedEvents.Add("end on ProcessDefinition(events)");

		assertEquals("expected " + expectedEvents + ", but was \n" + eventCollector + "\n", expectedEvents, eventCollector.events);
	  }
	}

}