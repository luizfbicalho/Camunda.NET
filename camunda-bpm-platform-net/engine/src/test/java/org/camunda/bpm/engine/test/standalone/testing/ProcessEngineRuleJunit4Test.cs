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
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using Task = org.camunda.bpm.engine.task.Task;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using CoreMatchers = org.hamcrest.CoreMatchers;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;


	/// <summary>
	/// Test runners follow the this rule:
	///   - if the class extends Testcase, run as Junit 3
	///   - otherwise use Junit 4
	/// 
	/// So this test can be included in the regular test suite without problems.
	/// 
	/// @author Joram Barrez
	/// </summary>
	public class ProcessEngineRuleJunit4Test
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule();
	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void ruleUsageExample()
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

	  /// <summary>
	  /// The rule should work with tests that have no deployment annotation
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWithoutDeploymentAnnotation()
	  public virtual void testWithoutDeploymentAnnotation()
	  {
		assertEquals("aString", "aString");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_AUDIT) public void requiredHistoryLevelAudit()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_AUDIT)]
	  public virtual void requiredHistoryLevelAudit()
	  {

		assertThat(currentHistoryLevel(), CoreMatchers.either<string>(@is(ProcessEngineConfiguration.HISTORY_AUDIT)).or(@is(ProcessEngineConfiguration.HISTORY_FULL)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_ACTIVITY) public void requiredHistoryLevelActivity()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_ACTIVITY)]
	  public virtual void requiredHistoryLevelActivity()
	  {

		assertThat(currentHistoryLevel(), CoreMatchers.either<string>(@is(ProcessEngineConfiguration.HISTORY_ACTIVITY)).or(@is(ProcessEngineConfiguration.HISTORY_AUDIT)).or(@is(ProcessEngineConfiguration.HISTORY_FULL)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void requiredHistoryLevelFull()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void requiredHistoryLevelFull()
	  {

		assertThat(currentHistoryLevel(), @is(ProcessEngineConfiguration.HISTORY_FULL));
	  }

	  protected internal virtual string currentHistoryLevel()
	  {
		return engineRule.ProcessEngine.ProcessEngineConfiguration.History;
	  }

	}

}