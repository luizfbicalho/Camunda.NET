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
namespace org.camunda.bpm.engine.test.api.multitenancy
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using AssertingJavaDelegate = org.camunda.bpm.engine.test.api.@delegate.AssertingJavaDelegate;
	using DelegateExecutionAsserter = org.camunda.bpm.engine.test.api.@delegate.AssertingJavaDelegate.DelegateExecutionAsserter;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;

	/// <summary>
	/// Tests if a <seealso cref="DelegateExecution"/> has the correct tenant-id. The
	/// assertions are checked inside the service tasks.
	/// </summary>
	public class MultiTenancyDelegateExecutionTest : PluggableProcessEngineTestCase
	{

	  protected internal const string PROCESS_DEFINITION_KEY = "testProcess";

	  public virtual void testSingleExecution()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		deploymentForTenant("tenant1", Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().serviceTask().camundaClass(typeof(AssertingJavaDelegate).FullName).endEvent().done());

		AssertingJavaDelegate.addAsserts(hasTenantId("tenant1"));

		startProcessInstance(PROCESS_DEFINITION_KEY);
	  }

	  public virtual void testConcurrentExecution()
	  {

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		deploymentForTenant("tenant1", Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().parallelGateway("fork").serviceTask().camundaClass(typeof(AssertingJavaDelegate).FullName).parallelGateway("join").endEvent().moveToNode("fork").serviceTask().camundaClass(typeof(AssertingJavaDelegate).FullName).connectTo("join").done());

		AssertingJavaDelegate.addAsserts(hasTenantId("tenant1"));

		startProcessInstance(PROCESS_DEFINITION_KEY);
	  }

	  public virtual void testEmbeddedSubprocess()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		deploymentForTenant("tenant1", Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().subProcess().embeddedSubProcess().startEvent().serviceTask().camundaClass(typeof(AssertingJavaDelegate).FullName).endEvent().subProcessDone().endEvent().done());

		AssertingJavaDelegate.addAsserts(hasTenantId("tenant1"));

		startProcessInstance(PROCESS_DEFINITION_KEY);
	  }

	  protected internal virtual void startProcessInstance(string processDefinitionKey)
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionKey(processDefinitionKey).latestVersion().singleResult();

		runtimeService.startProcessInstanceById(processDefinition.Id);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		AssertingJavaDelegate.clear();
		base.tearDown();
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected static org.camunda.bpm.engine.test.api.delegate.AssertingJavaDelegate.DelegateExecutionAsserter hasTenantId(final String expectedTenantId)
	  protected internal static AssertingJavaDelegate.DelegateExecutionAsserter hasTenantId(string expectedTenantId)
	  {
		return new DelegateExecutionAsserterAnonymousInnerClass(expectedTenantId);
	  }

	  private class DelegateExecutionAsserterAnonymousInnerClass : AssertingJavaDelegate.DelegateExecutionAsserter
	  {
		  private string expectedTenantId;

		  public DelegateExecutionAsserterAnonymousInnerClass(string expectedTenantId)
		  {
			  this.expectedTenantId = expectedTenantId;
		  }


		  public void doAssert(DelegateExecution execution)
		  {
			assertThat(execution.TenantId, @is(expectedTenantId));
		  }
	  }

	}

}