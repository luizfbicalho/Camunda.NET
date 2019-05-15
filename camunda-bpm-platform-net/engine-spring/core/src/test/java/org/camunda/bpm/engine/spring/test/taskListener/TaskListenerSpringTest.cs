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
namespace org.camunda.bpm.engine.spring.test.taskListener
{
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using ContextConfiguration = org.springframework.test.context.ContextConfiguration;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ContextConfiguration("classpath:org/camunda/bpm/engine/spring/test/taskListener/TaskListenerDelegateExpressionTest-context.xml") public class TaskListenerSpringTest extends org.camunda.bpm.engine.spring.test.SpringProcessEngineTestCase
	public class TaskListenerSpringTest : SpringProcessEngineTestCase
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTaskListenerDelegateExpression()
		public virtual void testTaskListenerDelegateExpression()
		{
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("taskListenerDelegateExpression");

		// Completing first task will set variable on process instance
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);
		assertEquals("task1-complete", runtimeService.getVariable(processInstance.Id, "calledInExpression"));

		// Completing second task will set variable on process instance
		task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);
		assertEquals("task2-notify", runtimeService.getVariable(processInstance.Id, "calledThroughNotify"));
		}

	}

}