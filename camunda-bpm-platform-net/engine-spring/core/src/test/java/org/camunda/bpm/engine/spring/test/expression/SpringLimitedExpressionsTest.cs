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
namespace org.camunda.bpm.engine.spring.test.expression
{
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using ContextConfiguration = org.springframework.test.context.ContextConfiguration;


	/// <summary>
	/// Test limiting the exposed beans in expressions.
	/// 
	/// @author Frederik Heremans
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ContextConfiguration("classpath:org/camunda/bpm/engine/spring/test/expression/expressionLimitedBeans-context.xml") public class SpringLimitedExpressionsTest extends org.camunda.bpm.engine.spring.test.SpringProcessEngineTestCase
	public class SpringLimitedExpressionsTest : SpringProcessEngineTestCase
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testLimitedBeansExposed() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public virtual void testLimitedBeansExposed()
		{
		// Start process, which has a service-task which calls 'bean1', which is exposed
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("limitedExpressionProcess");

		string beanOutput = (string) runtimeService.getVariable(processInstance.Id, "beanOutput");
		assertNotNull(beanOutput);
		assertEquals("Activiti BPMN 2.0 process engine", beanOutput);

		// Finish the task, should continue to serviceTask which uses a bean that is present
		// in application-context, but not exposed explicitly in "beans", should throw error!
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		assertNotNull(task);

		try
		{
		  taskService.complete(task.Id);
		  fail("Exception should have been thrown");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("Unknown property used in expression", ae.Message);
		}
		}
	}

}