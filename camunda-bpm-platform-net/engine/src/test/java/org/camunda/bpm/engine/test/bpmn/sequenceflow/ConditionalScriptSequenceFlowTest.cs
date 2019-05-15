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
namespace org.camunda.bpm.engine.test.bpmn.sequenceflow
{

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class ConditionalScriptSequenceFlowTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testScriptExpression()
	  public virtual void testScriptExpression()
	  {
		string[] directions = new string[] {"left", "right"};
		IDictionary<string, object> variables = new Dictionary<string, object>();

		foreach (string direction in directions)
		{
		  variables["foo"] = direction;
		  runtimeService.startProcessInstanceByKey("process", variables);

		  Task task = taskService.createTaskQuery().singleResult();
		  assertEquals(direction, task.TaskDefinitionKey);
		  taskService.complete(task.Id);
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testScriptExpressionWithNonBooleanResult()
	  public virtual void testScriptExpressionWithNonBooleanResult()
	  {
		try
		{
		  runtimeService.startProcessInstanceByKey("process");
		  fail("expected exception: invalid return value in script");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("condition script returns non-Boolean", e.Message);
		}
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/sequenceflow/ConditionalScriptSequenceFlowTest.testScriptResourceExpression.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/sequenceflow/condition-left.groovy" })]
	  public virtual void testScriptResourceExpression()
	  {
		string[] directions = new string[] {"left", "right"};
		IDictionary<string, object> variables = new Dictionary<string, object>();

		foreach (string direction in directions)
		{
		  variables["foo"] = direction;
		  runtimeService.startProcessInstanceByKey("process", variables);

		  Task task = taskService.createTaskQuery().singleResult();
		  assertEquals(direction, task.TaskDefinitionKey);
		  taskService.complete(task.Id);
		}

	  }

	}

}