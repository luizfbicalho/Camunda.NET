﻿/*
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
	using Automatic = org.camunda.bpm.engine.test.standalone.pvm.activities.Automatic;
	using Decision = org.camunda.bpm.engine.test.standalone.pvm.activities.Decision;
	using WaitState = org.camunda.bpm.engine.test.standalone.pvm.activities.WaitState;

	using TestCase = junit.framework.TestCase;

	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class PvmTest : TestCase
	{

	  public virtual void testPvmWaitState()
	  {
		PvmProcessDefinition processDefinition = (new ProcessDefinitionBuilder()).createActivity("a").initial().behavior(new WaitState()).transition("b").endActivity().createActivity("b").behavior(new WaitState()).transition("c").endActivity().createActivity("c").behavior(new WaitState()).endActivity().buildProcessDefinition();

		PvmProcessInstance processInstance = processDefinition.createProcessInstance();
		processInstance.start();

		PvmExecution activityInstance = processInstance.findExecution("a");
		assertNotNull(activityInstance);

		activityInstance.signal(null, null);

		activityInstance = processInstance.findExecution("b");
		assertNotNull(activityInstance);

		activityInstance.signal(null, null);

		activityInstance = processInstance.findExecution("c");
		assertNotNull(activityInstance);
	  }

	  public virtual void testPvmAutomatic()
	  {
		PvmProcessDefinition processDefinition = (new ProcessDefinitionBuilder()).createActivity("a").initial().behavior(new Automatic()).transition("b").endActivity().createActivity("b").behavior(new Automatic()).transition("c").endActivity().createActivity("c").behavior(new WaitState()).endActivity().buildProcessDefinition();

		PvmProcessInstance processInstance = processDefinition.createProcessInstance();
		processInstance.start();

		assertNotNull(processInstance.findExecution("c"));
	  }

	  public virtual void testPvmDecision()
	  {
		PvmProcessDefinition processDefinition = (new ProcessDefinitionBuilder()).createActivity("start").initial().behavior(new Automatic()).transition("checkCredit").endActivity().createActivity("checkCredit").behavior(new Decision()).transition("askDaughterOut", "wow").transition("takeToGolf", "nice").transition("ignore", "default").endActivity().createActivity("takeToGolf").behavior(new WaitState()).endActivity().createActivity("askDaughterOut").behavior(new WaitState()).endActivity().createActivity("ignore").behavior(new WaitState()).endActivity().buildProcessDefinition();

		PvmProcessInstance processInstance = processDefinition.createProcessInstance();
		processInstance.setVariable("creditRating", "Aaa-");
		processInstance.start();
		assertNotNull(processInstance.findExecution("takeToGolf"));

		processInstance = processDefinition.createProcessInstance();
		processInstance.setVariable("creditRating", "AAA+");
		processInstance.start();
		assertNotNull(processInstance.findExecution("askDaughterOut"));

		processInstance = processDefinition.createProcessInstance();
		processInstance.setVariable("creditRating", "bb-");
		processInstance.start();
		assertNotNull(processInstance.findExecution("ignore"));
	  }
	}

}