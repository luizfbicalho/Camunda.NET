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
namespace org.camunda.bpm.engine.spring.test.expression.callactivity
{
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using ContextConfiguration = org.springframework.test.context.ContextConfiguration;

	/// <summary>
	/// The CallActivityBasedOnSpringBeansExpressionTest is used to test dynamically wiring in the calledElement 
	/// in the callActivity task. This test case helps verify that we do not have to hard code the sub process
	/// definition key within the process.
	/// 
	/// @author  Sang Venkatraman
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ContextConfiguration("classpath:org/camunda/bpm/engine/spring/test/expression/callactivity/testCallActivityByExpression-context.xml") public class CallActivityBasedOnSpringBeansExpressionTest extends org.camunda.bpm.engine.spring.test.SpringProcessEngineTestCase
	public class CallActivityBasedOnSpringBeansExpressionTest : SpringProcessEngineTestCase
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/spring/test/expression/callactivity/CallActivityBasedOnSpringBeansExpressionTest.testCallActivityByExpression.bpmn20.xml", "org/camunda/bpm/engine/spring/test/expression/callactivity/simpleSubProcess.bpmn20.xml" }) public void testCallActivityByExpression() throws Exception
		[Deployment(resources : { "org/camunda/bpm/engine/spring/test/expression/callactivity/CallActivityBasedOnSpringBeansExpressionTest.testCallActivityByExpression.bpmn20.xml", "org/camunda/bpm/engine/spring/test/expression/callactivity/simpleSubProcess.bpmn20.xml" })]
		public virtual void testCallActivityByExpression()
		{
			// Start process (main)
			ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testCallActivityByExpression");

			// one task in the subprocess should be active after starting the process instance
			TaskQuery taskQuery = taskService.createTaskQuery();
			Task taskBeforeSubProcess = taskQuery.singleResult();
			assertEquals("Task before subprocess", taskBeforeSubProcess.Name);

			// Completing the task continues the process which leads to calling the subprocess. The sub process we want to
			// call is passed in as a variable into this task
			taskService.complete(taskBeforeSubProcess.Id);
			Task taskInSubProcess = taskQuery.singleResult();
			assertEquals("Task in subprocess", taskInSubProcess.Name);

			// Completing the task in the subprocess, finishes the subprocess
			taskService.complete(taskInSubProcess.Id);
			Task taskAfterSubProcess = taskQuery.singleResult();
			assertEquals("Task after subprocess", taskAfterSubProcess.Name);

			// Completing this task end the process instance
			taskService.complete(taskAfterSubProcess.Id);
			assertProcessEnded(processInstance.Id);
		}

	}

}