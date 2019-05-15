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

	using DelegateTask = org.camunda.bpm.engine.@delegate.DelegateTask;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using AssertingTaskListener = org.camunda.bpm.engine.test.api.@delegate.AssertingTaskListener;
	using DelegateTaskAsserter = org.camunda.bpm.engine.test.api.@delegate.AssertingTaskListener.DelegateTaskAsserter;

	/// <summary>
	/// Tests if a <seealso cref="DelegateTask"/> has the correct tenant-id. The
	/// assertions are checked inside the task listener.
	/// </summary>
	public class MultiTenancyDelegateTaskTest : PluggableProcessEngineTestCase
	{

	  protected internal const string BPMN = "org/camunda/bpm/engine/test/api/multitenancy/taskListener.bpmn";

	  public virtual void testSingleExecutionWithUserTask()
	  {
		deploymentForTenant("tenant1", BPMN);

		AssertingTaskListener.addAsserts(hasTenantId("tenant1"));

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		runtimeService.startProcessInstanceById(processDefinition.Id);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected static org.camunda.bpm.engine.test.api.delegate.AssertingTaskListener.DelegateTaskAsserter hasTenantId(final String expectedTenantId)
	  protected internal static AssertingTaskListener.DelegateTaskAsserter hasTenantId(string expectedTenantId)
	  {
		return new DelegateTaskAsserterAnonymousInnerClass(expectedTenantId);
	  }

	  private class DelegateTaskAsserterAnonymousInnerClass : AssertingTaskListener.DelegateTaskAsserter
	  {
		  private string expectedTenantId;

		  public DelegateTaskAsserterAnonymousInnerClass(string expectedTenantId)
		  {
			  this.expectedTenantId = expectedTenantId;
		  }


		  public void doAssert(DelegateTask task)
		  {
			assertThat(task.TenantId, @is(expectedTenantId));
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		AssertingTaskListener.clear();
		base.tearDown();
	  }

	}

}