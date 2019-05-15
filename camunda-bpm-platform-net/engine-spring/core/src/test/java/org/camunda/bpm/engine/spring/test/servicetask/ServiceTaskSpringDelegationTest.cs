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
namespace org.camunda.bpm.engine.spring.test.servicetask
{
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using ContextConfiguration = org.springframework.test.context.ContextConfiguration;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ContextConfiguration("classpath:org/camunda/bpm/engine/spring/test/servicetask/servicetaskSpringTest-context.xml") public class ServiceTaskSpringDelegationTest extends org.camunda.bpm.engine.spring.test.SpringProcessEngineTestCase
	public class ServiceTaskSpringDelegationTest : SpringProcessEngineTestCase
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDelegateExpression()
		public virtual void testDelegateExpression()
		{
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey("delegateExpressionToSpringBean");
		assertEquals("Activiti BPMN 2.0 process engine", runtimeService.getVariable(procInst.Id, "myVar"));
		assertEquals("fieldInjectionWorking", runtimeService.getVariable(procInst.Id, "fieldInjection"));
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDelegateClass()
	  public virtual void testDelegateClass()
	  {
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey("delegateClassToSpringBean");
		assertEquals("Activiti BPMN 2.0 process engine", runtimeService.getVariable(procInst.Id, "myVar"));
		assertEquals("fieldInjectionWorking", runtimeService.getVariable(procInst.Id, "fieldInjection"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDelegateClassNotABean()
	  public virtual void testDelegateClassNotABean()
	  {
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey("delegateClassToSpringBean");
		assertEquals("DelegateClassNotABean was called", runtimeService.getVariable(procInst.Id, "message"));
		assertTrue((bool?)runtimeService.getVariable(procInst.Id, "injectedFieldIsNull"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMethodExpressionOnSpringBean()
	  public virtual void testMethodExpressionOnSpringBean()
	  {
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey("methodExpressionOnSpringBean");
		assertEquals("ACTIVITI BPMN 2.0 PROCESS ENGINE", runtimeService.getVariable(procInst.Id, "myVar"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExecutionAndTaskListenerDelegationExpression()
	  public virtual void testExecutionAndTaskListenerDelegationExpression()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("executionAndTaskListenerDelegation");
		assertEquals("working", runtimeService.getVariable(processInstance.Id, "executionListenerVar"));
		assertEquals("working", runtimeService.getVariable(processInstance.Id, "taskListenerVar"));

		assertEquals("executionListenerInjection", runtimeService.getVariable(processInstance.Id, "executionListenerField"));
		assertEquals("taskListenerInjection", runtimeService.getVariable(processInstance.Id, "taskListenerField"));
	  }

	}

}