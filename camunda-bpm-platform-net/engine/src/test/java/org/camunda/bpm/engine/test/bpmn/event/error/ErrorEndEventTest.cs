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
namespace org.camunda.bpm.engine.test.bpmn.@event.error
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;


	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	public class ErrorEndEventTest
	{
		private bool InstanceFieldsInitialized = false;

		public ErrorEndEventTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal RuntimeService runtimeService;
	  protected internal TaskService taskService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		runtimeService = engineRule.RuntimeService;
		taskService = engineRule.TaskService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/event/error/testPropagateOutputVariablesWhileThrowError.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/error/ErrorEventTest.errorParent.bpmn20.xml" }) public void testPropagateOutputVariablesWhileThrowError()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/error/testPropagateOutputVariablesWhileThrowError.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/error/ErrorEventTest.errorParent.bpmn20.xml" })]
	  public virtual void testPropagateOutputVariablesWhileThrowError()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["input"] = 42;
		string processInstanceId = runtimeService.startProcessInstanceByKey("ErrorParentProcess", variables).Id;

		// when
		string id = taskService.createTaskQuery().taskName("ut2").singleResult().Id;
		taskService.complete(id);

		// then
		assertEquals(1, taskService.createTaskQuery().taskName("task after catched error").count());
		// and set the output variable of the called process to the process
		assertNotNull(runtimeService.getVariable(processInstanceId, "cancelReason"));
		assertEquals(42, runtimeService.getVariable(processInstanceId, "output"));
	  }

	}

}