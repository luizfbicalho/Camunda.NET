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
	using PvmExecution = org.camunda.bpm.engine.impl.pvm.PvmExecution;
	using PvmProcessDefinition = org.camunda.bpm.engine.impl.pvm.PvmProcessDefinition;
	using PvmProcessInstance = org.camunda.bpm.engine.impl.pvm.PvmProcessInstance;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ProcessDefinitionImpl = org.camunda.bpm.engine.impl.pvm.process.ProcessDefinitionImpl;
	using PvmTestCase = org.camunda.bpm.engine.impl.test.PvmTestCase;
	using Automatic = org.camunda.bpm.engine.test.standalone.pvm.activities.Automatic;
	using EmbeddedSubProcess = org.camunda.bpm.engine.test.standalone.pvm.activities.EmbeddedSubProcess;
	using End = org.camunda.bpm.engine.test.standalone.pvm.activities.End;
	using ParallelGateway = org.camunda.bpm.engine.test.standalone.pvm.activities.ParallelGateway;
	using WaitState = org.camunda.bpm.engine.test.standalone.pvm.activities.WaitState;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class PvmEmbeddedSubProcessTest : PvmTestCase
	{

	  /// <summary>
	  ///           +------------------------------+
	  ///           | embedded subprocess          |
	  /// +-----+   |  +-----------+   +---------+ |   +---+
	  /// |start|-->|  |startInside|-->|endInside| |-->|end|
	  /// +-----+   |  +-----------+   +---------+ |   +---+
	  ///           +------------------------------+
	  /// </summary>
	  public virtual void testEmbeddedSubProcess()
	  {
		PvmProcessDefinition processDefinition = (new ProcessDefinitionBuilder()).createActivity("start").initial().behavior(new Automatic()).transition("embeddedsubprocess").endActivity().createActivity("embeddedsubprocess").scope().behavior(new EmbeddedSubProcess()).createActivity("startInside").behavior(new Automatic()).transition("endInside").endActivity().createActivity("endInside").behavior(new End()).endActivity().transition("end").endActivity().createActivity("end").behavior(new WaitState()).endActivity().buildProcessDefinition();

		PvmProcessInstance processInstance = processDefinition.createProcessInstance();
		processInstance.start();

		IList<string> expectedActiveActivityIds = new List<string>();
		expectedActiveActivityIds.Add("end");

		assertEquals(expectedActiveActivityIds, processInstance.findActiveActivityIds());
	  }

	  /// <summary>
	  ///           +----------------------------------------+
	  ///           | embeddedsubprocess        +----------+ |
	  ///           |                     +---->|endInside1| |
	  ///           |                     |     +----------+ |
	  ///           |                     |                  |
	  /// +-----+   |  +-----------+   +----+   +----------+ |   +---+
	  /// |start|-->|  |startInside|-->|fork|-->|endInside2| |-->|end|
	  /// +-----+   |  +-----------+   +----+   +----------+ |   +---+
	  ///           |                     |                  |
	  ///           |                     |     +----------+ |
	  ///           |                     +---->|endInside3| |
	  ///           |                           +----------+ |
	  ///           +----------------------------------------+
	  /// </summary>
	  public virtual void testMultipleConcurrentEndsInsideEmbeddedSubProcess()
	  {
		PvmProcessDefinition processDefinition = (new ProcessDefinitionBuilder()).createActivity("start").initial().behavior(new Automatic()).transition("embeddedsubprocess").endActivity().createActivity("embeddedsubprocess").scope().behavior(new EmbeddedSubProcess()).createActivity("startInside").behavior(new Automatic()).transition("fork").endActivity().createActivity("fork").behavior(new ParallelGateway()).transition("endInside1").transition("endInside2").transition("endInside3").endActivity().createActivity("endInside1").behavior(new End()).endActivity().createActivity("endInside2").behavior(new End()).endActivity().createActivity("endInside3").behavior(new End()).endActivity().transition("end").endActivity().createActivity("end").behavior(new End()).endActivity().buildProcessDefinition();

		PvmProcessInstance processInstance = processDefinition.createProcessInstance();
		processInstance.start();

		assertTrue(processInstance.Ended);
	  }

	  /// <summary>
	  ///           +-------------------------------------------------+
	  ///           | embeddedsubprocess        +----------+          |
	  ///           |                     +---->|endInside1|          |
	  ///           |                     |     +----------+          |
	  ///           |                     |                           |
	  /// +-----+   |  +-----------+   +----+   +----+   +----------+ |   +---+
	  /// |start|-->|  |startInside|-->|fork|-->|wait|-->|endInside2| |-->|end|
	  /// +-----+   |  +-----------+   +----+   +----+   +----------+ |   +---+
	  ///           |                     |                           |
	  ///           |                     |     +----------+          |
	  ///           |                     +---->|endInside3|          |
	  ///           |                           +----------+          |
	  ///           +-------------------------------------------------+
	  /// </summary>
	  public virtual void testMultipleConcurrentEndsInsideEmbeddedSubProcessWithWaitState()
	  {
		PvmProcessDefinition processDefinition = (new ProcessDefinitionBuilder()).createActivity("start").initial().behavior(new Automatic()).transition("embeddedsubprocess").endActivity().createActivity("embeddedsubprocess").scope().behavior(new EmbeddedSubProcess()).createActivity("startInside").behavior(new Automatic()).transition("fork").endActivity().createActivity("fork").behavior(new ParallelGateway()).transition("endInside1").transition("wait").transition("endInside3").endActivity().createActivity("endInside1").behavior(new End()).endActivity().createActivity("wait").behavior(new WaitState()).transition("endInside2").endActivity().createActivity("endInside2").behavior(new End()).endActivity().createActivity("endInside3").behavior(new End()).endActivity().transition("end").endActivity().createActivity("end").behavior(new End()).endActivity().buildProcessDefinition();

		PvmProcessInstance processInstance = processDefinition.createProcessInstance();
		processInstance.start();

		assertFalse(processInstance.Ended);
		PvmExecution execution = processInstance.findExecution("wait");
		execution.signal(null, null);

		assertTrue(processInstance.Ended);
	  }

	  /// <summary>
	  ///           +-------------------------------------------------------+
	  ///           | embedded subprocess                                   |
	  ///           |                  +--------------------------------+   |
	  ///           |                  | nested embedded subprocess     |   |
	  /// +-----+   | +-----------+    |  +-----------+   +---------+   |   |   +---+
	  /// |start|-->| |startInside|--> |  |startInside|-->|endInside|   |   |-->|end|
	  /// +-----+   | +-----------+    |  +-----------+   +---------+   |   |   +---+
	  ///           |                  +--------------------------------+   |
	  ///           |                                                       |
	  ///           +-------------------------------------------------------+
	  /// </summary>
	  public virtual void testNestedSubProcessNoEnd()
	  {
		PvmProcessDefinition processDefinition = (new ProcessDefinitionBuilder()).createActivity("start").initial().behavior(new Automatic()).transition("embeddedsubprocess").endActivity().createActivity("embeddedsubprocess").scope().behavior(new EmbeddedSubProcess()).createActivity("startInside").behavior(new Automatic()).transition("nestedSubProcess").endActivity().createActivity("nestedSubProcess").scope().behavior(new EmbeddedSubProcess()).createActivity("startNestedInside").behavior(new Automatic()).transition("endInside").endActivity().createActivity("endInside").behavior(new End()).endActivity().endActivity().transition("end").endActivity().createActivity("end").behavior(new WaitState()).endActivity().buildProcessDefinition();

		PvmProcessInstance processInstance = processDefinition.createProcessInstance();
		processInstance.start();

		IList<string> expectedActiveActivityIds = new List<string>();
		expectedActiveActivityIds.Add("end");

		assertEquals(expectedActiveActivityIds, processInstance.findActiveActivityIds());
	  }

	  /// <summary>
	  ///           +------------------------------+
	  ///           | embedded subprocess          |
	  /// +-----+   |  +-----------+               |
	  /// |start|-->|  |startInside|               |
	  /// +-----+   |  +-----------+               |
	  ///           +------------------------------+
	  /// </summary>
	  public virtual void testEmbeddedSubProcessWithoutEndEvents()
	  {
		PvmProcessDefinition processDefinition = (new ProcessDefinitionBuilder()).createActivity("start").initial().behavior(new Automatic()).transition("embeddedsubprocess").endActivity().createActivity("embeddedsubprocess").scope().behavior(new EmbeddedSubProcess()).createActivity("startInside").behavior(new Automatic()).endActivity().endActivity().buildProcessDefinition();

		PvmProcessInstance processInstance = processDefinition.createProcessInstance();
		processInstance.start();

		assertTrue(processInstance.Ended);
	  }

	  /// <summary>
	  ///           +-------------------------------------------------------+
	  ///           | embedded subprocess                                   |
	  ///           |                  +--------------------------------+   |
	  ///           |                  | nested embedded subprocess     |   |
	  /// +-----+   | +-----------+    |  +-----------+                 |   |
	  /// |start|-->| |startInside|--> |  |startInside|                 |   |
	  /// +-----+   | +-----------+    |  +-----------+                 |   |
	  ///           |                  +--------------------------------+   |
	  ///           |                                                       |
	  ///           +-------------------------------------------------------+
	  /// </summary>
	  public virtual void testNestedSubProcessBothNoEnd()
	  {
		PvmProcessDefinition processDefinition = (new ProcessDefinitionBuilder()).createActivity("start").initial().behavior(new Automatic()).transition("embeddedsubprocess").endActivity().createActivity("embeddedsubprocess").scope().behavior(new EmbeddedSubProcess()).createActivity("startInside").behavior(new Automatic()).transition("nestedSubProcess").endActivity().createActivity("nestedSubProcess").scope().behavior(new EmbeddedSubProcess()).createActivity("startNestedInside").behavior(new Automatic()).endActivity().endActivity().endActivity().buildProcessDefinition();

		PvmProcessInstance processInstance = processDefinition.createProcessInstance();
		processInstance.start();

		assertTrue(processInstance.Ended);
	  }


	  /// <summary>
	  ///           +------------------------------+
	  ///           | embedded subprocess          |
	  /// +-----+   |  +-----------+   +---------+ |
	  /// |start|-->|  |startInside|-->|endInside| |
	  /// +-----+   |  +-----------+   +---------+ |
	  ///           +------------------------------+
	  /// </summary>
	  public virtual void testEmbeddedSubProcessNoEnd()
	  {
		PvmProcessDefinition processDefinition = (new ProcessDefinitionBuilder()).createActivity("start").initial().behavior(new Automatic()).transition("embeddedsubprocess").endActivity().createActivity("embeddedsubprocess").scope().behavior(new EmbeddedSubProcess()).createActivity("startInside").behavior(new Automatic()).transition("endInside").endActivity().createActivity("endInside").behavior(new End()).endActivity().endActivity().buildProcessDefinition();

		PvmProcessInstance processInstance = processDefinition.createProcessInstance();
		processInstance.start();

		assertTrue(processInstance.Ended);
	  }

	  /// <summary>
	  ///           +------------------------------+
	  ///           | embedded subprocess          |
	  /// +-----+   |  +-----------+   +---------+ |   +---+
	  /// |start|-->|  |startInside|-->|endInside| |-->|end|
	  /// +-----+   |  +-----------+   +---------+ |   +---+
	  ///           +------------------------------+
	  /// </summary>
	  public virtual void testStartInScope()
	  {
		PvmProcessDefinition processDefinition = (new ProcessDefinitionBuilder()).createActivity("start").initial().behavior(new Automatic()).transition("embeddedsubprocess").endActivity().createActivity("embeddedsubprocess").scope().behavior(new EmbeddedSubProcess()).createActivity("startInside").behavior(new Automatic()).transition("endInside").endActivity().createActivity("endInside").behavior(new End()).endActivity().transition("end").endActivity().createActivity("end").behavior(new WaitState()).endActivity().buildProcessDefinition();

		PvmProcessInstance processInstance = ((ProcessDefinitionImpl) processDefinition).createProcessInstanceForInitial((ActivityImpl) processDefinition.findActivity("startInside"));
		processInstance.start();

		IList<string> expectedActiveActivityIds = new List<string>();
		expectedActiveActivityIds.Add("end");

		assertEquals(expectedActiveActivityIds, processInstance.findActiveActivityIds());
	  }

	}

}