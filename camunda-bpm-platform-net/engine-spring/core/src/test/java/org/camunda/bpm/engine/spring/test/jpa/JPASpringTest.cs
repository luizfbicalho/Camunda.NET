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
namespace org.camunda.bpm.engine.spring.test.jpa
{

	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using ContextConfiguration = org.springframework.test.context.ContextConfiguration;


	/// <summary>
	/// Test using spring-orm in spring-bean combined with JPA-variables in activiti.
	/// 
	/// @author Frederik Heremans
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ContextConfiguration("classpath:org/camunda/bpm/engine/spring/test/jpa/JPASpringTest-context.xml") public class JPASpringTest extends org.camunda.bpm.engine.spring.test.SpringProcessEngineTestCase
	public class JPASpringTest : SpringProcessEngineTestCase
	{
		[Deployment(resources : {"org/camunda/bpm/engine/spring/test/jpa/JPASpringTest.bpmn20.xml"})]
		public virtual void testJpaVariableHappyPath()
		{
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["customerName"] = "John Doe";
		variables["amount"] = 15000L;

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("LoanRequestProcess", variables);

		// Variable should be present containing the loanRequest created by the spring bean
		object value = runtimeService.getVariable(processInstance.Id, "loanRequest");
		assertNotNull(value);
		assertTrue(value is LoanRequest);
		LoanRequest request = (LoanRequest) value;
		assertEquals("John Doe", request.CustomerName);
		assertEquals(15000L, request.Amount.Value);
		assertFalse(request.Approved);

		// We will approve the request, which will update the entity
		variables = new Dictionary<string, object>();
		variables["approvedByManager"] = true;

		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		assertNotNull(task);
		taskService.complete(task.Id, variables);

		// If approved, the processsInstance should be finished, gateway based on loanRequest.approved value
		assertEquals(0, runtimeService.createProcessInstanceQuery().processInstanceId(processInstance.Id).count());

		}

	  [Deployment(resources : {"org/camunda/bpm/engine/spring/test/jpa/JPASpringTest.bpmn20.xml"})]
	  public virtual void testJpaVariableDisapprovalPath()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["customerName"] = "Jane Doe";
		variables["amount"] = 50000;

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("LoanRequestProcess", variables);

		// Variable should be present containing the loanRequest created by the spring bean
		object value = runtimeService.getVariable(processInstance.Id, "loanRequest");
		assertNotNull(value);
		assertTrue(value is LoanRequest);
		LoanRequest request = (LoanRequest) value;
		assertEquals("Jane Doe", request.CustomerName);
		assertEquals(50000L, request.Amount.Value);
		assertFalse(request.Approved);

		// We will disapprove the request, which will update the entity
		variables = new Dictionary<string, object>();
		variables["approvedByManager"] = false;

		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		assertNotNull(task);
		taskService.complete(task.Id, variables);

		runtimeService.getVariable(processInstance.Id, "loanRequest");
		request = (LoanRequest) value;
		assertFalse(request.Approved);

		// If disapproved, an extra task will be available instead of the process ending
		task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		assertNotNull(task);
		assertEquals("Send rejection letter", task.Name);
	  }


	}

}