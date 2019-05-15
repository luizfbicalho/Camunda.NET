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
namespace org.camunda.bpm.application.impl.el
{
	using RuntimeContainerDelegate = org.camunda.bpm.container.RuntimeContainerDelegate;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ProcessApplicationElResolverTest : PluggableProcessEngineTestCase
	{

	  internal RuntimeContainerDelegate runtimeContainerDelegate = null;

	  internal CallingProcessApplication callingApp;
	  internal CalledProcessApplication calledApp;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void setUp() throws Exception
	  protected internal virtual void setUp()
	  {
		runtimeContainerDelegate = org.camunda.bpm.container.RuntimeContainerDelegate_Fields.INSTANCE.get();
		runtimeContainerDelegate.registerProcessEngine(processEngine);

		callingApp = new CallingProcessApplication();
		calledApp = new CalledProcessApplication();

		callingApp.deploy();
		calledApp.deploy();
	  }

	  public virtual void tearDown()
	  {

		callingApp.undeploy();
		calledApp.undeploy();

		if (runtimeContainerDelegate != null)
		{
		  runtimeContainerDelegate.unregisterProcessEngine(processEngine);
		}
	  }

	  /// <summary>
	  /// Tests that an expression for a call activity output parameter is resolved
	  /// in the context of the called process definition's application.
	  /// </summary>
	  public virtual void testCallActivityOutputExpression()
	  {
		// given an instance of the calling process that calls the called process
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("callingProcess");

		// when the called process is completed
		Task calledProcessTask = taskService.createTaskQuery().singleResult();
		taskService.complete(calledProcessTask.Id);

		// then the output mapping should have successfully resolved the expression
		string outVariable = (string) runtimeService.getVariable(instance.Id, "outVar");
		assertEquals(CalledProcessApplication.STRING_VARIABLE_VALUE, outVariable);
	  }

	  /// <summary>
	  /// Tests that an expression on an outgoing flow leaving a call activity
	  /// is resolved in the context of the calling process definition's application.
	  /// </summary>
	  public virtual void testCallActivityConditionalOutgoingFlow()
	  {
		// given an instance of the calling process that calls the called process
		runtimeService.startProcessInstanceByKey("callingProcessConditionalFlow");

		// when the called process is completed
		Task calledProcessTask = taskService.createTaskQuery().singleResult();
		taskService.complete(calledProcessTask.Id);

		// then the conditional flow expression was resolved in the context of the calling process application, so
		// the following task has been reached successfully
		Task afterCallActivityTask = taskService.createTaskQuery().singleResult();
		assertNotNull(afterCallActivityTask);
		assertEquals("afterCallActivityTask", afterCallActivityTask.TaskDefinitionKey);

	  }
	}

}