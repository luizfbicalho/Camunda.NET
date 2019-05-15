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
	using ExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.ExecutionImpl;
	using PvmTestCase = org.camunda.bpm.engine.impl.test.PvmTestCase;
	using Automatic = org.camunda.bpm.engine.test.standalone.pvm.activities.Automatic;
	using EmbeddedSubProcess = org.camunda.bpm.engine.test.standalone.pvm.activities.EmbeddedSubProcess;
	using EventScopeCreatingSubprocess = org.camunda.bpm.engine.test.standalone.pvm.activities.EventScopeCreatingSubprocess;
	using WaitState = org.camunda.bpm.engine.test.standalone.pvm.activities.WaitState;


	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class PvmEventScopesTest : PvmTestCase
	{

	  /// 
	  /// <summary>
	  ///                       create evt scope --+
	  ///                                          |   
	  ///                                          v                                        
	  /// 
	  ///           +------------------------------+
	  ///           | embedded subprocess          |
	  /// +-----+   |  +-----------+   +---------+ |   +----+   +---+
	  /// |start|-->|  |startInside|-->|endInside| |-->|wait|-->|end|
	  /// +-----+   |  +-----------+   +---------+ |   +----+   +---+
	  ///           +------------------------------+
	  /// 
	  ///                                                           ^  
	  ///                                                           |
	  ///                                       destroy evt scope --+  
	  /// 
	  /// </summary>
	  public virtual void testActivityEndDestroysEventScopes()
	  {
		  PvmProcessDefinition processDefinition = (new ProcessDefinitionBuilder()).createActivity("start").initial().behavior(new Automatic()).transition("embeddedsubprocess").endActivity().createActivity("embeddedsubprocess").scope().behavior(new EventScopeCreatingSubprocess()).createActivity("startInside").behavior(new Automatic()).transition("endInside").endActivity().createActivity("endInside").behavior(new Automatic()).endActivity().transition("wait").endActivity().createActivity("wait").behavior(new WaitState()).transition("end").endActivity().createActivity("end").behavior(new Automatic()).endActivity().buildProcessDefinition();

		PvmProcessInstance processInstance = processDefinition.createProcessInstance();
		processInstance.start();

		bool eventScopeFound = false;
		IList<ExecutionImpl> executions = ((ExecutionImpl)processInstance).Executions;
		foreach (ExecutionImpl executionImpl in executions)
		{
		  if (executionImpl.EventScope)
		  {
			eventScopeFound = true;
			break;
		  }
		}

		assertTrue(eventScopeFound);

		processInstance.signal(null, null);

		assertTrue(processInstance.Ended);

	  }


	  /// <summary>
	  ///           +----------------------------------------------------------------------+
	  ///           | embedded subprocess                                                  |
	  ///           |                                                                      |
	  ///           |                                create evt scope --+                  |
	  ///           |                                                   |                  |
	  ///           |                                                   v                  |
	  ///           |                                                                      |
	  ///           |                  +--------------------------------+                  |
	  ///           |                  | nested embedded subprocess     |                  |
	  /// +-----+   | +-----------+    |  +-----------------+           |   +----+   +---+ |   +---+
	  /// |start|-->| |startInside|--> |  |startNestedInside|           |-->|wait|-->|end| |-->|end|
	  /// +-----+   | +-----------+    |  +-----------------+           |   +----+   +---+ |   +---+
	  ///           |                  +--------------------------------+                  |
	  ///           |                                                                      |
	  ///           +----------------------------------------------------------------------+
	  /// 
	  ///                                                                                  ^  
	  ///                                                                                  |
	  ///                                                              destroy evt scope --+  
	  /// </summary>
	  public virtual void testTransitionDestroysEventScope()
	  {
		PvmProcessDefinition processDefinition = (new ProcessDefinitionBuilder()).createActivity("start").initial().behavior(new Automatic()).transition("embeddedsubprocess").endActivity().createActivity("embeddedsubprocess").scope().behavior(new EmbeddedSubProcess()).createActivity("startInside").behavior(new Automatic()).transition("nestedSubProcess").endActivity().createActivity("nestedSubProcess").scope().behavior(new EventScopeCreatingSubprocess()).createActivity("startNestedInside").behavior(new Automatic()).endActivity().transition("wait").endActivity().createActivity("wait").behavior(new WaitState()).transition("endInside").endActivity().createActivity("endInside").behavior(new Automatic()).endActivity().transition("end").endActivity().createActivity("end").behavior(new Automatic()).endActivity().buildProcessDefinition();

		PvmProcessInstance processInstance = processDefinition.createProcessInstance();
		processInstance.start();

		IList<string> expectedActiveActivityIds = new List<string>();
		expectedActiveActivityIds.Add("wait");
		assertEquals(expectedActiveActivityIds, processInstance.findActiveActivityIds());


		PvmExecution execution = processInstance.findExecution("wait");
		execution.signal(null, null);

		assertTrue(processInstance.Ended);

	  }

	}

}