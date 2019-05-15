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
namespace org.camunda.bpm.engine.test.api.context
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.assertNull;
	using DelegateExecutionContext = org.camunda.bpm.engine.context.DelegateExecutionContext;
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// Represents test class to test the delegate execution context.
	/// 
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class DelegateExecutionContextTest
	{
		private bool InstanceFieldsInitialized = false;

		public DelegateExecutionContextTest()
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
			ruleChain = RuleChain.outerRule(engineRule).around(testHelper);
		}


//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  protected internal static readonly BpmnModelInstance DELEGATION_PROCESS = Bpmn.createExecutableProcess("process1").startEvent().serviceTask("serviceTask1").camundaClass(typeof(DelegateClass).FullName).endEvent().done();


//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  protected internal static readonly BpmnModelInstance EXEUCTION_LISTENER_PROCESS = Bpmn.createExecutableProcess("process2").startEvent().camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, typeof(ExecutionListenerImpl).FullName).endEvent().done();

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testHelper);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDelegateExecutionContext()
	  public virtual void testDelegateExecutionContext()
	  {
		// given
		ProcessDefinition definition = testHelper.deployAndGetDefinition(DELEGATION_PROCESS);
		// a process instance with a service task and a java delegate
		ProcessInstance instance = engineRule.RuntimeService.startProcessInstanceById(definition.Id);

		//then delegation execution context is no more available
		DelegateExecution execution = DelegateExecutionContext.CurrentDelegationExecution;
		assertNull(execution);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDelegateExecutionContextWithExecutionListener()
	  public virtual void testDelegateExecutionContextWithExecutionListener()
	  {
		//given
		ProcessDefinition definition = testHelper.deployAndGetDefinition(EXEUCTION_LISTENER_PROCESS);
		// a process instance with a service task and an execution listener
		engineRule.RuntimeService.startProcessInstanceById(definition.Id);

		//then delegation execution context is no more available
		DelegateExecution execution = DelegateExecutionContext.CurrentDelegationExecution;
		assertNull(execution);
	  }

	  public class ExecutionListenerImpl : ExecutionListener
	  {

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void notify(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void notify(DelegateExecution execution)
		{
		  checkDelegationContext(execution);
		}
	  }

	  public class DelegateClass : JavaDelegate
	  {

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{
		  checkDelegationContext(execution);
		}
	  }

	  protected internal static void checkDelegationContext(DelegateExecution execution)
	  {
		//then delegation execution context is available
		assertNotNull(DelegateExecutionContext.CurrentDelegationExecution);
		assertEquals(DelegateExecutionContext.CurrentDelegationExecution, execution);
	  }
	}

}