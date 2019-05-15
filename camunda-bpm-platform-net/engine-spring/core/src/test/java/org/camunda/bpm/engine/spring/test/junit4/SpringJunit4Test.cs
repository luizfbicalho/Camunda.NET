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
namespace org.camunda.bpm.engine.spring.test.junit4
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;

	using Task = org.camunda.bpm.engine.task.Task;
	using ProcessEngineRule = org.camunda.bpm.engine.test.ProcessEngineRule;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using After = org.junit.After;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;
	using Autowired = org.springframework.beans.factory.annotation.Autowired;
	using ContextConfiguration = org.springframework.test.context.ContextConfiguration;
	using SpringJUnit4ClassRunner = org.springframework.test.context.junit4.SpringJUnit4ClassRunner;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(SpringJUnit4ClassRunner.class) @ContextConfiguration("classpath:org/camunda/bpm/engine/spring/test/junit4/springTypicalUsageTest-context.xml") public class SpringJunit4Test
	public class SpringJunit4Test
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Autowired private org.camunda.bpm.engine.ProcessEngine processEngine;
		private ProcessEngine processEngine;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Autowired private org.camunda.bpm.engine.RuntimeService runtimeService;
	  private RuntimeService runtimeService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Autowired private org.camunda.bpm.engine.TaskService taskService;
	  private TaskService taskService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Autowired @Rule public org.camunda.bpm.engine.test.ProcessEngineRule activitiSpringRule;
	  public ProcessEngineRule activitiSpringRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void closeProcessEngine()
	  public virtual void closeProcessEngine()
	  {
		// Required, since all the other tests seem to do a specific drop on the end
		processEngine.close();
		processEngine = null;
		runtimeService = null;
		taskService = null;
		activitiSpringRule = null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void simpleProcessTest()
	  public virtual void simpleProcessTest()
	  {
		runtimeService.startProcessInstanceByKey("simpleProcess");
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("My Task", task.Name);

		taskService.complete(task.Id);
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());

	  }

	}

}