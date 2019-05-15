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
namespace org.camunda.bpm.engine.test.api.@delegate
{
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using DelegateExecutionAsserter = org.camunda.bpm.engine.test.api.@delegate.AssertingJavaDelegate.DelegateExecutionAsserter;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;

	/// <summary>
	/// Tests for the execution hierarchy methods exposed in delegate execution
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class DelegateExecutionHierarchyTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		AssertingJavaDelegate.clear();
		base.tearDown();
	  }

	  public virtual void testSingleNonScopeActivity()
	  {

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		deployment(Bpmn.createExecutableProcess("testProcess").startEvent().serviceTask().camundaClass(typeof(AssertingJavaDelegate).FullName).endEvent().done());

		AssertingJavaDelegate.addAsserts(new DelegateExecutionAsserterAnonymousInnerClass(this));

		runtimeService.startProcessInstanceByKey("testProcess");

	  }

	  private class DelegateExecutionAsserterAnonymousInnerClass : DelegateExecutionAsserter
	  {
		  private readonly DelegateExecutionHierarchyTest outerInstance;

		  public DelegateExecutionAsserterAnonymousInnerClass(DelegateExecutionHierarchyTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public void doAssert(DelegateExecution execution)
		  {
			assertEquals(execution, execution.ProcessInstance);
			assertNull(execution.SuperExecution);
		  }
	  }

	  public virtual void testConcurrentServiceTasks()
	  {

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		deployment(Bpmn.createExecutableProcess("testProcess").startEvent().parallelGateway("fork").serviceTask().camundaClass(typeof(AssertingJavaDelegate).FullName).parallelGateway("join").endEvent().moveToNode("fork").serviceTask().camundaClass(typeof(AssertingJavaDelegate).FullName).connectTo("join").done());

		AssertingJavaDelegate.addAsserts(new DelegateExecutionAsserterAnonymousInnerClass2(this));

		runtimeService.startProcessInstanceByKey("testProcess");

	  }

	  private class DelegateExecutionAsserterAnonymousInnerClass2 : DelegateExecutionAsserter
	  {
		  private readonly DelegateExecutionHierarchyTest outerInstance;

		  public DelegateExecutionAsserterAnonymousInnerClass2(DelegateExecutionHierarchyTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public void doAssert(DelegateExecution execution)
		  {
			assertFalse(execution.Equals(execution.ProcessInstance));
			assertNull(execution.SuperExecution);
		  }
	  }

	  public virtual void testTaskInsideEmbeddedSubprocess()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		deployment(Bpmn.createExecutableProcess("testProcess").startEvent().subProcess().embeddedSubProcess().startEvent().serviceTask().camundaClass(typeof(AssertingJavaDelegate).FullName).endEvent().subProcessDone().endEvent().done());

		AssertingJavaDelegate.addAsserts(new DelegateExecutionAsserterAnonymousInnerClass3(this));

		runtimeService.startProcessInstanceByKey("testProcess");
	  }

	  private class DelegateExecutionAsserterAnonymousInnerClass3 : DelegateExecutionAsserter
	  {
		  private readonly DelegateExecutionHierarchyTest outerInstance;

		  public DelegateExecutionAsserterAnonymousInnerClass3(DelegateExecutionHierarchyTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public void doAssert(DelegateExecution execution)
		  {
			assertFalse(execution.Equals(execution.ProcessInstance));
			assertNull(execution.SuperExecution);
		  }
	  }

	  public virtual void testSubProcessInstance()
	  {

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		deployment(Bpmn.createExecutableProcess("testProcess").startEvent().callActivity().calledElement("testProcess2").endEvent().done(), Bpmn.createExecutableProcess("testProcess2").startEvent().serviceTask().camundaClass(typeof(AssertingJavaDelegate).FullName).endEvent().done());

		AssertingJavaDelegate.addAsserts(new DelegateExecutionAsserterAnonymousInnerClass4(this));

		runtimeService.startProcessInstanceByKey("testProcess");
	  }

	  private class DelegateExecutionAsserterAnonymousInnerClass4 : DelegateExecutionAsserter
	  {
		  private readonly DelegateExecutionHierarchyTest outerInstance;

		  public DelegateExecutionAsserterAnonymousInnerClass4(DelegateExecutionHierarchyTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public void doAssert(DelegateExecution execution)
		  {
			assertTrue(execution.Equals(execution.ProcessInstance));
			assertNotNull(execution.SuperExecution);
		  }
	  }
	}

}