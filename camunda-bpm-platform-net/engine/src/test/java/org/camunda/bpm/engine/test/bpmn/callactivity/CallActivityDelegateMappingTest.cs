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
namespace org.camunda.bpm.engine.test.bpmn.callactivity
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.fail;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Assert = org.junit.Assert;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class CallActivityDelegateMappingTest
	{
		private bool InstanceFieldsInitialized = false;

		public CallActivityDelegateMappingTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testHelper = new ProcessEngineTestRule(engineRule);
			chain = RuleChain.outerRule(engineRule).around(testHelper);
		}


	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  public ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain chain = org.junit.rules.RuleChain.outerRule(engineRule).around(testHelper);
	  public RuleChain chain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivityDelegateMappingTest.testCallSimpleSubProcessDelegateVarMapping.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml" }) public void testCallSubProcessWithDelegatedVariableMapping()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivityDelegateMappingTest.testCallSimpleSubProcessDelegateVarMapping.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
	  public virtual void testCallSubProcessWithDelegatedVariableMapping()
	  {
		//given
		engineRule.RuntimeService.startProcessInstanceByKey("callSimpleSubProcess");
		TaskQuery taskQuery = engineRule.TaskService.createTaskQuery();

		//when
		Task taskInSubProcess = taskQuery.singleResult();
		assertEquals("Task in subprocess", taskInSubProcess.Name);

		//then check value from input variable
		object inputVar = engineRule.RuntimeService.getVariable(taskInSubProcess.ProcessInstanceId, "TestInputVar");
		assertEquals("inValue", inputVar);

		//when completing the task in the subprocess, finishes the subprocess
		engineRule.TaskService.complete(taskInSubProcess.Id);
		Task taskAfterSubProcess = taskQuery.singleResult();
		assertEquals("Task after subprocess", taskAfterSubProcess.Name);

		//then check value from output variable
		ProcessInstance processInstance = engineRule.RuntimeService.createProcessInstanceQuery().singleResult();
		object outputVar = engineRule.RuntimeService.getVariable(processInstance.Id, "TestOutputVar");
		assertEquals("outValue", outputVar);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivityDelegateMappingTest.testCallSimpleSubProcessDelegateVarMappingExpression.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml" }) public void testCallSubProcessWithDelegatedVariableMappingeExpression()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivityDelegateMappingTest.testCallSimpleSubProcessDelegateVarMappingExpression.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
	  public virtual void testCallSubProcessWithDelegatedVariableMappingeExpression()
	  {
		//given

		IDictionary<object, object> vars = engineRule.ProcessEngineConfiguration.Beans;
		vars["expr"] = new DelegatedVarMapping();
		engineRule.ProcessEngineConfiguration.Beans = vars;
		engineRule.RuntimeService.startProcessInstanceByKey("callSimpleSubProcess");
		TaskQuery taskQuery = engineRule.TaskService.createTaskQuery();

		//when
		Task taskInSubProcess = taskQuery.singleResult();
		assertEquals("Task in subprocess", taskInSubProcess.Name);

		//then check if variable mapping was executed - check if input variable exist
		object inputVar = engineRule.RuntimeService.getVariable(taskInSubProcess.ProcessInstanceId, "TestInputVar");
		assertEquals("inValue", inputVar);

		//when completing the task in the subprocess, finishes the subprocess
		engineRule.TaskService.complete(taskInSubProcess.Id);
		Task taskAfterSubProcess = taskQuery.singleResult();
		assertEquals("Task after subprocess", taskAfterSubProcess.Name);

		//then check if variable output mapping was executed - check if output variable exist
		ProcessInstance processInstance = engineRule.RuntimeService.createProcessInstanceQuery().singleResult();
		object outputVar = engineRule.RuntimeService.getVariable(processInstance.Id, "TestOutputVar");
		assertEquals("outValue", outputVar);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivityDelegateMappingTest.testCallSimpleSubProcessDelegateVarMappingNotFound.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml" }) public void testCallSubProcessWithDelegatedVariableMappingNotFound()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivityDelegateMappingTest.testCallSimpleSubProcessDelegateVarMappingNotFound.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
	  public virtual void testCallSubProcessWithDelegatedVariableMappingNotFound()
	  {
		try
		{
		  engineRule.RuntimeService.startProcessInstanceByKey("callSimpleSubProcess");
		  fail("Execption expected!");
		}
		catch (ProcessEngineException e)
		{
		  //Exception while instantiating class 'org.camunda.bpm.engine.test.bpmn.callactivity.NotFoundMapping'
		  assertEquals("ENGINE-09008 Exception while instantiating class 'org.camunda.bpm.engine.test.bpmn.callactivity.NotFoundMapping': ENGINE-09017 Cannot load class 'org.camunda.bpm.engine.test.bpmn.callactivity.NotFoundMapping': org.camunda.bpm.engine.test.bpmn.callactivity.NotFoundMapping", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivityDelegateMappingTest.testCallSimpleSubProcessDelegateVarMappingExpressionNotFound.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml" }) public void testCallSubProcessWithDelegatedVariableMappingeExpressionNotFound()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivityDelegateMappingTest.testCallSimpleSubProcessDelegateVarMappingExpressionNotFound.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
	  public virtual void testCallSubProcessWithDelegatedVariableMappingeExpressionNotFound()
	  {
		try
		{
		  engineRule.RuntimeService.startProcessInstanceByKey("callSimpleSubProcess");
		  fail("Exception expected!");
		}
		catch (ProcessEngineException pee)
		{
		  assertEquals("Unknown property used in expression: ${notFound}. Cause: Cannot resolve identifier 'notFound'", pee.Message);
		}
	  }

	  private void delegateVariableMappingThrowException()
	  {
		//given
		engineRule.RuntimeService.startProcessInstanceByKey("callSimpleSubProcess");
		TaskQuery taskQuery = engineRule.TaskService.createTaskQuery();
		Task taskBeforeSubProcess = taskQuery.singleResult();
		assertEquals("Task before subprocess", taskBeforeSubProcess.Name);

		//when completing the task continues the process which leads to calling the subprocess
		//which throws an exception
		try
		{
		  engineRule.TaskService.complete(taskBeforeSubProcess.Id);
		  fail("Exeption expected!");
		}
		catch (ProcessEngineException pee)
		{ //then
		  Assert.assertTrue(pee.Message.equalsIgnoreCase("org.camunda.bpm.engine.ProcessEngineException: New process engine exception.") || pee.Message.contains("1234"));
		}

		//then process rollback to user task which is before sub process
		//not catched by boundary event
		taskBeforeSubProcess = taskQuery.singleResult();
		assertEquals("Task before subprocess", taskBeforeSubProcess.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivityDelegateMappingTest.testCallSimpleSubProcessDelegateVarMappingThrowException.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml" }) public void testCallSubProcessWithDelegatedVariableMappingThrowException()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivityDelegateMappingTest.testCallSimpleSubProcessDelegateVarMappingThrowException.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
	  public virtual void testCallSubProcessWithDelegatedVariableMappingThrowException()
	  {
		delegateVariableMappingThrowException();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivityDelegateMappingTest.testCallSimpleSubProcessDelegateVarMappingExpressionThrowException.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml" }) public void testCallSubProcessWithDelegatedVariableMappingeExpressionThrowException()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivityDelegateMappingTest.testCallSimpleSubProcessDelegateVarMappingExpressionThrowException.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
	  public virtual void testCallSubProcessWithDelegatedVariableMappingeExpressionThrowException()
	  {
		//given
		IDictionary<object, object> vars = engineRule.ProcessEngineConfiguration.Beans;
		vars["expr"] = new DelegateVarMappingThrowException();
		engineRule.ProcessEngineConfiguration.Beans = vars;
		delegateVariableMappingThrowException();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivityDelegateMappingTest.testCallSimpleSubProcessDelegateVarMappingThrowBpmnError.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml" }) public void testCallSubProcessWithDelegatedVariableMappingThrowBpmnError()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivityDelegateMappingTest.testCallSimpleSubProcessDelegateVarMappingThrowBpmnError.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
	  public virtual void testCallSubProcessWithDelegatedVariableMappingThrowBpmnError()
	  {
		delegateVariableMappingThrowException();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivityDelegateMappingTest.testCallSimpleSubProcessDelegateVarMappingExpressionThrowException.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml" }) public void testCallSubProcessWithDelegatedVariableMappingeExpressionThrowBpmnError()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivityDelegateMappingTest.testCallSimpleSubProcessDelegateVarMappingExpressionThrowException.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
	  public virtual void testCallSubProcessWithDelegatedVariableMappingeExpressionThrowBpmnError()
	  {
		//given
		IDictionary<object, object> vars = engineRule.ProcessEngineConfiguration.Beans;
		vars["expr"] = new DelegateVarMappingThrowBpmnError();
		engineRule.ProcessEngineConfiguration.Beans = vars;
		delegateVariableMappingThrowException();
	  }

	  private void delegateVariableMappingThrowExceptionOutput()
	  {
		//given
		engineRule.RuntimeService.startProcessInstanceByKey("callSimpleSubProcess");
		TaskQuery taskQuery = engineRule.TaskService.createTaskQuery();
		Task taskBeforeSubProcess = taskQuery.singleResult();
		assertEquals("Task before subprocess", taskBeforeSubProcess.Name);
		engineRule.TaskService.complete(taskBeforeSubProcess.Id);
		Task taskInSubProcess = taskQuery.singleResult();

		//when completing the task continues the process which leads to calling the output mapping
		//which throws an exception
		try
		{
		  engineRule.TaskService.complete(taskInSubProcess.Id);
		  fail("Exeption expected!");
		}
		catch (ProcessEngineException pee)
		{ //then
		  Assert.assertTrue(pee.Message.equalsIgnoreCase("org.camunda.bpm.engine.ProcessEngineException: New process engine exception.") || pee.Message.contains("1234"));
		}

		//then process rollback to user task which is in sub process
		//not catched by boundary event
		taskInSubProcess = taskQuery.singleResult();
		assertEquals("Task in subprocess", taskInSubProcess.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivityDelegateMappingTest.testCallSimpleSubProcessDelegateVarMappingThrowExceptionOutput.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml" }) public void testCallSubProcessWithDelegatedVariableMappingThrowExceptionOutput()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivityDelegateMappingTest.testCallSimpleSubProcessDelegateVarMappingThrowExceptionOutput.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
	  public virtual void testCallSubProcessWithDelegatedVariableMappingThrowExceptionOutput()
	  {
		delegateVariableMappingThrowExceptionOutput();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivityDelegateMappingTest.testCallSimpleSubProcessDelegateVarMappingExpressionThrowException.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml" }) public void testCallSubProcessWithDelegatedVariableMappingeExpressionThrowExceptionOutput()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivityDelegateMappingTest.testCallSimpleSubProcessDelegateVarMappingExpressionThrowException.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
	  public virtual void testCallSubProcessWithDelegatedVariableMappingeExpressionThrowExceptionOutput()
	  {
		//given
		IDictionary<object, object> vars = engineRule.ProcessEngineConfiguration.Beans;
		vars["expr"] = new DelegateVarMappingThrowExceptionOutput();
		engineRule.ProcessEngineConfiguration.Beans = vars;
		delegateVariableMappingThrowExceptionOutput();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivityDelegateMappingTest.testCallSimpleSubProcessDelegateVarMappingThrowBpmnErrorOutput.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml" }) public void testCallSubProcessWithDelegatedVariableMappingThrowBpmnErrorOutput()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivityDelegateMappingTest.testCallSimpleSubProcessDelegateVarMappingThrowBpmnErrorOutput.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
	  public virtual void testCallSubProcessWithDelegatedVariableMappingThrowBpmnErrorOutput()
	  {
		delegateVariableMappingThrowExceptionOutput();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivityDelegateMappingTest.testCallSimpleSubProcessDelegateVarMappingExpressionThrowException.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml" }) public void testCallSubProcessWithDelegatedVariableMappingeExpressionThrowBpmnErrorOutput()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivityDelegateMappingTest.testCallSimpleSubProcessDelegateVarMappingExpressionThrowException.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
	  public virtual void testCallSubProcessWithDelegatedVariableMappingeExpressionThrowBpmnErrorOutput()
	  {
		//given
		IDictionary<object, object> vars = engineRule.ProcessEngineConfiguration.Beans;
		vars["expr"] = new DelegateVarMappingThrowBpmnErrorOutput();
		engineRule.ProcessEngineConfiguration.Beans = vars;
		delegateVariableMappingThrowExceptionOutput();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivityDelegateMappingTest.testCallFailingSubProcessWithDelegatedVariableMapping.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/failingSubProcess.bpmn20.xml" }) public void testCallFailingSubProcessWithDelegatedVariableMapping()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivityDelegateMappingTest.testCallFailingSubProcessWithDelegatedVariableMapping.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/failingSubProcess.bpmn20.xml" })]
	  public virtual void testCallFailingSubProcessWithDelegatedVariableMapping()
	  {
		//given starting process instance with call activity
		//when call activity execution fails
		ProcessInstance procInst = engineRule.RuntimeService.startProcessInstanceByKey("callSimpleSubProcess");

		//then output mapping should be executed
		object outputVar = engineRule.RuntimeService.getVariable(procInst.Id, "TestOutputVar");
		assertEquals("outValue", outputVar);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivityDelegateMappingTest.testCallSubProcessWithDelegatedVariableMappingAndAsyncServiceTask.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcessWithAsyncService.bpmn20.xml" }) public void testCallSubProcessWithDelegatedVariableMappingAndAsyncServiceTask()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivityDelegateMappingTest.testCallSubProcessWithDelegatedVariableMappingAndAsyncServiceTask.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcessWithAsyncService.bpmn20.xml" })]
	  public virtual void testCallSubProcessWithDelegatedVariableMappingAndAsyncServiceTask()
	  {
		//given starting process instance with call activity which has asyn service task
		ProcessInstance superProcInst = engineRule.RuntimeService.startProcessInstanceByKey("callSimpleSubProcess");

		ProcessInstance subProcInst = engineRule.RuntimeService.createProcessInstanceQuery().processDefinitionKey("simpleSubProcessWithAsyncService").singleResult();

		//then delegation variable mapping class should also been resolved
		//input mapping should be executed
		object inVar = engineRule.RuntimeService.getVariable(subProcInst.Id, "TestInputVar");
		assertEquals("inValue", inVar);

		//and after finish call activity the ouput mapping is executed
		testHelper.executeAvailableJobs();

		object outputVar = engineRule.RuntimeService.getVariable(superProcInst.Id, "TestOutputVar");
		assertEquals("outValue", outputVar);
	  }

	}

}