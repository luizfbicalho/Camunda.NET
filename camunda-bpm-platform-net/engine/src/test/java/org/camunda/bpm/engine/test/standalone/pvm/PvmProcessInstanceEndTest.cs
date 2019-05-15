﻿using System;

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
	using WaitState = org.camunda.bpm.engine.test.standalone.pvm.activities.WaitState;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class PvmProcessInstanceEndTest : PvmTestCase
	{

	  public virtual void testSimpleProcessInstanceEnd()
	  {
		EventCollector eventCollector = new EventCollector();

		PvmProcessDefinition processDefinition = (new ProcessDefinitionBuilder()).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_START, eventCollector).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_END, eventCollector).createActivity("start").initial().behavior(new Automatic()).transition("wait").endActivity().createActivity("wait").behavior(new WaitState()).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_START, eventCollector).executionListener(org.camunda.bpm.engine.impl.pvm.PvmEvent.EVENTNAME_END, eventCollector).endActivity().buildProcessDefinition();

		PvmProcessInstance processInstance = processDefinition.createProcessInstance();
		processInstance.start();

		Console.Error.WriteLine(eventCollector);

		processInstance.deleteCascade("test");

		Console.Error.WriteLine();
		Console.Error.WriteLine(eventCollector);
	  }
	}

}