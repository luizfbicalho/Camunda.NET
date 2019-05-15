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
namespace org.camunda.bpm.engine.test.bpmn.servicetask
{

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using CollectionUtil = org.camunda.bpm.engine.impl.util.CollectionUtil;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using GenderBean = org.camunda.bpm.engine.test.bpmn.servicetask.util.GenderBean;

	/// <summary>
	/// @author Joram Barrez
	/// @author Frederik Heremans
	/// </summary>
	public class JavaServiceTaskTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testJavaServiceDelegation()
	  public virtual void testJavaServiceDelegation()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("javaServiceDelegation", CollectionUtil.singletonMap("input", "Activiti BPM Engine"));
		Execution execution = runtimeService.createExecutionQuery().processInstanceId(pi.Id).activityId("waitState").singleResult();
		assertEquals("ACTIVITI BPM ENGINE", runtimeService.getVariable(execution.Id, "input"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testFieldInjection()
	  public virtual void testFieldInjection()
	  {
		// Process contains 2 service-tasks using field-injection. One should use the exposed setter,
		// the other is using the private field.
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("fieldInjection");
		Execution execution = runtimeService.createExecutionQuery().processInstanceId(pi.Id).activityId("waitState").singleResult();

		assertEquals("HELLO WORLD", runtimeService.getVariable(execution.Id, "var"));
		assertEquals("HELLO SETTER", runtimeService.getVariable(execution.Id, "setterVar"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExpressionFieldInjection()
	  public virtual void testExpressionFieldInjection()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["name"] = "kermit";
		vars["gender"] = "male";
		vars["genderBean"] = new GenderBean();

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("expressionFieldInjection", vars);
		Execution execution = runtimeService.createExecutionQuery().processInstanceId(pi.Id).activityId("waitState").singleResult();

		assertEquals("timrek .rM olleH", runtimeService.getVariable(execution.Id, "var2"));
		assertEquals("elam :si redneg ruoY", runtimeService.getVariable(execution.Id, "var1"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testUnexistingClassDelegation()
	  public virtual void testUnexistingClassDelegation()
	  {
		try
		{
		  runtimeService.startProcessInstanceByKey("unexistingClassDelegation");
		  fail();
		}
		catch (ProcessEngineException e)
		{
		  assertTrue(e.Message.contains("Exception while instantiating class 'org.camunda.bpm.engine.test.BogusClass'"));
		  assertNotNull(e.InnerException);
		  assertTrue(e.InnerException is ClassLoadingException);
		}
	  }

	  public virtual void testIllegalUseOfResultVariableName()
	  {
		try
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/servicetask/JavaServiceTaskTest.testIllegalUseOfResultVariableName.bpmn20.xml").deploy();
		  fail();
		}
		catch (ProcessEngineException e)
		{
		  assertTrue(e.Message.contains("resultVariable"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExceptionHandling()
	  public virtual void testExceptionHandling()
	  {

		// If variable value is != 'throw-exception', process goes
		// through service task and ends immidiately
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["var"] = "no-exception";
		runtimeService.startProcessInstanceByKey("exceptionHandling", vars);
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());

		// If variable value == 'throw-exception', process executes
		// service task, which generates and catches exception,
		// and takes sequence flow to user task
		vars["var"] = "throw-exception";
		runtimeService.startProcessInstanceByKey("exceptionHandling", vars);
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("Fix Exception", task.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testGetBusinessKeyFromDelegateExecution()
	  public virtual void testGetBusinessKeyFromDelegateExecution()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("businessKeyProcess", "1234567890");
		assertEquals(1, runtimeService.createProcessInstanceQuery().processDefinitionKey("businessKeyProcess").count());

		// Check if business-key was available from the process
		string key = (string) runtimeService.getVariable(processInstance.Id, "businessKeySetOnExecution");
		assertNotNull(key);
		assertEquals("1234567890", key);

		// check if BaseDelegateExecution#getBusinessKey() behaves like DelegateExecution#getProcessBusinessKey()
		string key2 = (string) runtimeService.getVariable(processInstance.Id, "businessKeyAsProcessBusinessKey");
		assertEquals(key2, key);
	  }

	}

}