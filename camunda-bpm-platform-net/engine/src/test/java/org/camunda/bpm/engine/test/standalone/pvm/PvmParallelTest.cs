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
	using PvmTestCase = org.camunda.bpm.engine.impl.test.PvmTestCase;
	using Automatic = org.camunda.bpm.engine.test.standalone.pvm.activities.Automatic;
	using End = org.camunda.bpm.engine.test.standalone.pvm.activities.End;
	using ParallelGateway = org.camunda.bpm.engine.test.standalone.pvm.activities.ParallelGateway;
	using WaitState = org.camunda.bpm.engine.test.standalone.pvm.activities.WaitState;



	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class PvmParallelTest : PvmTestCase
	{

	  public virtual void testSimpleAutmaticConcurrency()
	  {
		PvmProcessDefinition processDefinition = (new ProcessDefinitionBuilder()).createActivity("start").initial().behavior(new Automatic()).transition("fork").endActivity().createActivity("fork").behavior(new ParallelGateway()).transition("c1").transition("c2").endActivity().createActivity("c1").behavior(new Automatic()).transition("join").endActivity().createActivity("c2").behavior(new Automatic()).transition("join").endActivity().createActivity("join").behavior(new ParallelGateway()).transition("end").endActivity().createActivity("end").behavior(new End()).endActivity().buildProcessDefinition();

		PvmProcessInstance processInstance = processDefinition.createProcessInstance();
		processInstance.start();

		assertTrue(processInstance.Ended);
	  }

	  public virtual void testSimpleWaitStateConcurrency()
	  {
		PvmProcessDefinition processDefinition = (new ProcessDefinitionBuilder()).createActivity("start").initial().behavior(new Automatic()).transition("fork").endActivity().createActivity("fork").behavior(new ParallelGateway()).transition("c1").transition("c2").endActivity().createActivity("c1").behavior(new WaitState()).transition("join").endActivity().createActivity("c2").behavior(new WaitState()).transition("join").endActivity().createActivity("join").behavior(new ParallelGateway()).transition("end").endActivity().createActivity("end").behavior(new WaitState()).endActivity().buildProcessDefinition();

		PvmProcessInstance processInstance = processDefinition.createProcessInstance();
		processInstance.start();

		PvmExecution activityInstanceC1 = processInstance.findExecution("c1");
		assertNotNull(activityInstanceC1);

		PvmExecution activityInstanceC2 = processInstance.findExecution("c2");
		assertNotNull(activityInstanceC2);

		activityInstanceC1.signal(null, null);
		activityInstanceC2.signal(null, null);

		IList<string> activityNames = processInstance.findActiveActivityIds();
		IList<string> expectedActivityNames = new List<string>();
		expectedActivityNames.Add("end");

		assertEquals(expectedActivityNames, activityNames);
	  }

	  public virtual void testUnstructuredConcurrencyTwoJoins()
	  {
		PvmProcessDefinition processDefinition = (new ProcessDefinitionBuilder()).createActivity("start").initial().behavior(new Automatic()).transition("fork").endActivity().createActivity("fork").behavior(new ParallelGateway()).transition("c1").transition("c2").transition("c3").endActivity().createActivity("c1").behavior(new Automatic()).transition("join1").endActivity().createActivity("c2").behavior(new Automatic()).transition("join1").endActivity().createActivity("c3").behavior(new Automatic()).transition("join2").endActivity().createActivity("join1").behavior(new ParallelGateway()).transition("c4").endActivity().createActivity("c4").behavior(new Automatic()).transition("join2").endActivity().createActivity("join2").behavior(new ParallelGateway()).transition("end").endActivity().createActivity("end").behavior(new WaitState()).endActivity().buildProcessDefinition();

		PvmProcessInstance processInstance = processDefinition.createProcessInstance();
		processInstance.start();

		assertNotNull(processInstance.findExecution("end"));
	  }

	  public virtual void testUnstructuredConcurrencyTwoForks()
	  {
		PvmProcessDefinition processDefinition = (new ProcessDefinitionBuilder()).createActivity("start").initial().behavior(new Automatic()).transition("fork1").endActivity().createActivity("fork1").behavior(new ParallelGateway()).transition("c1").transition("c2").transition("fork2").endActivity().createActivity("c1").behavior(new Automatic()).transition("join").endActivity().createActivity("c2").behavior(new Automatic()).transition("join").endActivity().createActivity("fork2").behavior(new ParallelGateway()).transition("c3").transition("c4").endActivity().createActivity("c3").behavior(new Automatic()).transition("join").endActivity().createActivity("c4").behavior(new Automatic()).transition("join").endActivity().createActivity("join").behavior(new ParallelGateway()).transition("end").endActivity().createActivity("end").behavior(new WaitState()).endActivity().buildProcessDefinition();

		PvmProcessInstance processInstance = processDefinition.createProcessInstance();
		processInstance.start();

		assertNotNull(processInstance.findExecution("end"));
	  }

	  public virtual void testJoinForkCombinedInOneParallelGateway()
	  {
		PvmProcessDefinition processDefinition = (new ProcessDefinitionBuilder()).createActivity("start").initial().behavior(new Automatic()).transition("fork").endActivity().createActivity("fork").behavior(new ParallelGateway()).transition("c1").transition("c2").transition("c3").endActivity().createActivity("c1").behavior(new Automatic()).transition("join1").endActivity().createActivity("c2").behavior(new Automatic()).transition("join1").endActivity().createActivity("c3").behavior(new Automatic()).transition("join2").endActivity().createActivity("join1").behavior(new ParallelGateway()).transition("c4").transition("c5").transition("c6").endActivity().createActivity("c4").behavior(new Automatic()).transition("join2").endActivity().createActivity("c5").behavior(new Automatic()).transition("join2").endActivity().createActivity("c6").behavior(new Automatic()).transition("join2").endActivity().createActivity("join2").behavior(new ParallelGateway()).transition("end").endActivity().createActivity("end").behavior(new WaitState()).endActivity().buildProcessDefinition();

		PvmProcessInstance processInstance = processDefinition.createProcessInstance();
		processInstance.start();

		assertNotNull(processInstance.findExecution("end"));
	  }
	}

}