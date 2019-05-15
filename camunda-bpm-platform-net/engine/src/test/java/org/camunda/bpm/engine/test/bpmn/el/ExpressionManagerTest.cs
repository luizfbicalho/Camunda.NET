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
namespace org.camunda.bpm.engine.test.bpmn.el
{

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Frederik Heremans
	/// </summary>
	public class ExpressionManagerTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMethodExpressions()
	  public virtual void testMethodExpressions()
	  {
		// Process contains 2 service tasks. one containing a method with no params, the other
		// contains a method with 2 params. When the process completes without exception,
		// test passed.
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["aString"] = "abcdefgh";
		runtimeService.startProcessInstanceByKey("methodExpressionProcess", vars);

		assertEquals(0, runtimeService.createProcessInstanceQuery().processDefinitionKey("methodExpressionProcess").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExecutionAvailable()
	  public virtual void testExecutionAvailable()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();

		vars["myVar"] = new ExecutionTestVariable();
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testExecutionAvailableProcess", vars);

		// Check of the testMethod has been called with the current execution
		string value = (string) runtimeService.getVariable(processInstance.Id, "testVar");
		assertNotNull(value);
		assertEquals("myValue", value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAuthenticatedUserIdAvailable()
	  public virtual void testAuthenticatedUserIdAvailable()
	  {
		try
		{
		  // Setup authentication
		  identityService.AuthenticatedUserId = "frederik";
		  ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testAuthenticatedUserIdAvailableProcess");

		  // Check if the variable that has been set in service-task is the authenticated user
		  string value = (string) runtimeService.getVariable(processInstance.Id, "theUser");
		  assertNotNull(value);
		  assertEquals("frederik", value);
		}
		finally
		{
		  // Cleanup
		  identityService.clearAuthentication();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testResolvesVariablesFromDifferentScopes()
	  public virtual void testResolvesVariablesFromDifferentScopes()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["assignee"] = "michael";

		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("michael", task.Assignee);

		variables["assignee"] = "johnny";
		ProcessInstance secondInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);
		task = taskService.createTaskQuery().processInstanceId(secondInstance.Id).singleResult();
		assertEquals("johnny", task.Assignee);
	  }
	}

}