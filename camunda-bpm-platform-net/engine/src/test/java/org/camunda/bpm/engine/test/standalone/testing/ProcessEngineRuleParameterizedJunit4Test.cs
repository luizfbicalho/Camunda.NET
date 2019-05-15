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
namespace org.camunda.bpm.engine.test.standalone.testing
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;


	using Task = org.camunda.bpm.engine.task.Task;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Ignore = org.junit.Ignore;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;
	using Parameters = org.junit.runners.Parameterized.Parameters;


	/// <summary>
	/// @author Thorben Lindhauer
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class ProcessEngineRuleParameterizedJunit4Test
	public class ProcessEngineRuleParameterizedJunit4Test
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters public static java.util.Collection<Object[]> data()
		public static ICollection<object[]> data()
		{
		return Arrays.asList(new object[][]
		{
			new object[] {1},
			new object[] {2}
		});
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule();
	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  public ProcessEngineRuleParameterizedJunit4Test(int parameter)
	  {

	  }

	  /// <summary>
	  /// Unnamed @Deployment annotations don't work with parameterized Unit tests
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Ignore @Test @Deployment public void ruleUsageExample()
	  public virtual void ruleUsageExample()
	  {
		RuntimeService runtimeService = engineRule.RuntimeService;
		runtimeService.startProcessInstanceByKey("ruleUsage");

		TaskService taskService = engineRule.TaskService;
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("My Task", task.Name);

		taskService.complete(task.Id);
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/standalone/testing/ProcessEngineRuleParameterizedJunit4Test.ruleUsageExample.bpmn20.xml") public void ruleUsageExampleWithNamedAnnotation()
	  [Deployment(resources : "org/camunda/bpm/engine/test/standalone/testing/ProcessEngineRuleParameterizedJunit4Test.ruleUsageExample.bpmn20.xml")]
	  public virtual void ruleUsageExampleWithNamedAnnotation()
	  {
		RuntimeService runtimeService = engineRule.RuntimeService;
		runtimeService.startProcessInstanceByKey("ruleUsage");

		TaskService taskService = engineRule.TaskService;
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("My Task", task.Name);

		taskService.complete(task.Id);
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }

	  /// <summary>
	  /// The rule should work with tests that have no deployment annotation
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWithoutDeploymentAnnotation()
	  public virtual void testWithoutDeploymentAnnotation()
	  {
		assertEquals("aString", "aString");
	  }

	}

}