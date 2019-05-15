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
namespace org.camunda.bpm.engine.test.api.variables
{

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using RemoveAndUpdateValueDelegate = org.camunda.bpm.engine.test.history.RemoveAndUpdateValueDelegate;
	using ReplaceAndUpdateValueDelegate = org.camunda.bpm.engine.test.history.ReplaceAndUpdateValueDelegate;
	using UpdateValueDelegate = org.camunda.bpm.engine.test.history.UpdateValueDelegate;
	using Variables = org.camunda.bpm.engine.variable.Variables;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ImplicitVariableUpdateTest : PluggableProcessEngineTestCase
	{

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/variables/ImplicitVariableUpdateTest.sequence.bpmn20.xml")]
	  public virtual void testUpdate()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("serviceTaskProcess", Variables.createVariables().putValue("listVar", new List<string>()).putValue("delegate", new UpdateValueDelegate()));

		IList<string> list = (IList<string>) runtimeService.getVariable(instance.Id, "listVar");
		assertNotNull(list);
		assertEquals(1, list.Count);
		assertEquals(UpdateValueDelegate.NEW_ELEMENT, list[0]);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/variables/ImplicitVariableUpdateTest.parallel.bpmn20.xml")]
	  public virtual void testUpdateParallelFlow()
	  {
		// should also work when execution tree is expanded between the implicit update
		// and when the engine notices it

		ProcessInstance instance = runtimeService.startProcessInstanceByKey("serviceTaskProcess", Variables.createVariables().putValue("listVar", new List<string>()).putValue("delegate", new UpdateValueDelegate()));

		IList<string> list = (IList<string>) runtimeService.getVariable(instance.Id, "listVar");
		assertNotNull(list);
		assertEquals(1, list.Count);
		assertEquals(UpdateValueDelegate.NEW_ELEMENT, list[0]);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/variables/ImplicitVariableUpdateTest.sequence.bpmn20.xml")]
	  public virtual void testUpdatePreviousValue()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("serviceTaskProcess", Variables.createVariables().putValue("listVar", new List<string>()).putValue("delegate", new ReplaceAndUpdateValueDelegate()));

		IList<string> list = (IList<string>) runtimeService.getVariable(instance.Id, "listVar");
		assertNotNull(list);
		assertTrue(list.Count == 0);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/variables/ImplicitVariableUpdateTest.sequence.bpmn20.xml")]
	  public virtual void testRemoveAndUpdateValue()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("serviceTaskProcess", Variables.createVariables().putValue("listVar", new List<string>()).putValue("delegate", new RemoveAndUpdateValueDelegate()));

		object variableValue = runtimeService.getVariable(instance.Id, "listVar");
		assertNull(variableValue);
	  }
	}

}