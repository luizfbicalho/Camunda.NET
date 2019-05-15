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
//	import static org.junit.Assert.assertThat;

	using Task = org.camunda.bpm.engine.task.Task;
	using CoreMatchers = org.hamcrest.CoreMatchers;


	/// <summary>
	/// @author Joram Barrez
	/// @author Falko Menge (camunda)
	/// </summary>
	public class ProcessEngineTestCaseTest : ProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSimpleProcess()
	  public virtual void testSimpleProcess()
	  {
		runtimeService.startProcessInstanceByKey("simpleProcess");

		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("My Task", task.Name);

		taskService.complete(task.Id);
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_AUDIT)]
	  public virtual void testRequiredHistoryLevelAudit()
	  {

		assertThat(currentHistoryLevel(), CoreMatchers.either<string>(@is(ProcessEngineConfiguration.HISTORY_AUDIT)).or(@is(ProcessEngineConfiguration.HISTORY_FULL)));
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_ACTIVITY)]
	  public virtual void testRequiredHistoryLevelActivity()
	  {

		assertThat(currentHistoryLevel(), CoreMatchers.either<string>(@is(ProcessEngineConfiguration.HISTORY_ACTIVITY)).or(@is(ProcessEngineConfiguration.HISTORY_AUDIT)).or(@is(ProcessEngineConfiguration.HISTORY_FULL)));
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testRequiredHistoryLevelFull()
	  {

		assertThat(currentHistoryLevel(), @is(ProcessEngineConfiguration.HISTORY_FULL));
	  }

	  protected internal virtual string currentHistoryLevel()
	  {
		return processEngine.ProcessEngineConfiguration.History;
	  }

	}

}