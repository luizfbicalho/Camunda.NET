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
namespace org.camunda.bpm.engine.test.bpmn.@event.error
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;

	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Task = org.camunda.bpm.engine.task.Task;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	public class UnhandledBpmnErrorTest
	{
		private bool InstanceFieldsInitialized = false;

		public UnhandledBpmnErrorTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
		}


	  protected internal ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {
			configuration.EnableExceptionsAfterUnhandledBpmnError = true;
			return configuration;
		  }
	  }

	  protected internal ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);
	  public ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  private RuntimeService runtimeService;
	  private TaskService taskService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		runtimeService = engineRule.RuntimeService;
		taskService = engineRule.TaskService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testThrownInJavaDelegate()
	  public virtual void testThrownInJavaDelegate()
	  {
		// expect
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage(containsString("no error handler"));

		// given
		BpmnModelInstance instance = Bpmn.createExecutableProcess("process").startEvent().serviceTask().camundaClass(typeof(ThrowBpmnErrorDelegate)).endEvent().done();
		testRule.deploy(instance);

		// when
		runtimeService.startProcessInstanceByKey("process");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testUncaughtErrorSimpleProcess()
	  public virtual void testUncaughtErrorSimpleProcess()
	  {
		// expect
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage(containsString("no error handler"));

		// given simple process definition

		// when
		runtimeService.startProcessInstanceByKey("process");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testUnhandledErrorInEmbeddedSubprocess()
	  public virtual void testUnhandledErrorInEmbeddedSubprocess()
	  {
		// expect
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage(containsString("no error handler"));

		// given
		runtimeService.startProcessInstanceByKey("boundaryErrorOnEmbeddedSubprocess");

		// assume
		// After process start, usertask in subprocess should exist
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("subprocessTask", task.Name);

		// when
		// After task completion, error end event is reached which is never caught in the process
		taskService.complete(task.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/event/error/UnhandledBpmnErrorTest.testUncaughtErrorOnCallActivity.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/error/UnhandledBpmnErrorTest.subprocess.bpmn20.xml" }) public void testUncaughtErrorOnCallActivity()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/error/UnhandledBpmnErrorTest.testUncaughtErrorOnCallActivity.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/error/UnhandledBpmnErrorTest.subprocess.bpmn20.xml" })]
	  public virtual void testUncaughtErrorOnCallActivity()
	  {
		// expect
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage(containsString("no error handler"));

		// given
		runtimeService.startProcessInstanceByKey("uncaughtErrorOnCallActivity");

		// assume
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("Task in subprocess", task.Name);

		// when
		// Completing the task will reach the end error event,
		// which is never caught in the process
		taskService.complete(task.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testUncaughtErrorOnEventSubprocess()
	  public virtual void testUncaughtErrorOnEventSubprocess()
	  {
		// expect
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage(containsString("no error handler"));

		// given
		runtimeService.startProcessInstanceByKey("process").Id;

		// assume
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("subprocessTask", task.Name);

		// when
		// After task completion, error end event is reached which is never caught in the process
		taskService.complete(task.Id);
	  }
	}
}