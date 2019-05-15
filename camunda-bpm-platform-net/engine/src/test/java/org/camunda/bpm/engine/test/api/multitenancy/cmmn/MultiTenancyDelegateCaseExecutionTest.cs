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
namespace org.camunda.bpm.engine.test.api.multitenancy.cmmn
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using DelegateCaseExecution = org.camunda.bpm.engine.@delegate.DelegateCaseExecution;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using AssertingCaseExecutionListener = org.camunda.bpm.engine.test.api.multitenancy.listener.AssertingCaseExecutionListener;
	using DelegateCaseExecutionAsserter = org.camunda.bpm.engine.test.api.multitenancy.listener.AssertingCaseExecutionListener.DelegateCaseExecutionAsserter;

	/// <summary>
	/// Tests if a <seealso cref="DelegateCaseExecution"/> has the correct tenant-id.
	/// </summary>
	public class MultiTenancyDelegateCaseExecutionTest : PluggableProcessEngineTestCase
	{

	  protected internal const string HUMAN_TASK_CMMN_FILE = "org/camunda/bpm/engine/test/api/multitenancy/HumanTaskCaseExecutionListener.cmmn";
	  protected internal const string CASE_TASK_CMMN_FILE = "org/camunda/bpm/engine/test/api/multitenancy/CaseTaskCaseExecutionListener.cmmn";
	  protected internal const string CMMN_FILE = "org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCaseWithManualActivation.cmmn";

	  protected internal const string TENANT_ID = "tenant1";

	  public virtual void testSingleExecution()
	  {
		deploymentForTenant(TENANT_ID, HUMAN_TASK_CMMN_FILE);

		AssertingCaseExecutionListener.addAsserts(hasTenantId("tenant1"));

		createCaseInstance("case");
	  }

	  public virtual void testCallCaseTask()
	  {
		deploymentForTenant(TENANT_ID, CMMN_FILE);
		deployment(CASE_TASK_CMMN_FILE);

		AssertingCaseExecutionListener.addAsserts(hasTenantId("tenant1"));

		createCaseInstance("oneCaseTaskCase");
	  }

	  protected internal virtual void createCaseInstance(string caseDefinitionKey)
	  {
		CaseDefinition caseDefinition = repositoryService.createCaseDefinitionQuery().caseDefinitionKey(caseDefinitionKey).latestVersion().singleResult();

		caseService.createCaseInstanceById(caseDefinition.Id);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		AssertingCaseExecutionListener.clear();
		base.tearDown();
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected static org.camunda.bpm.engine.test.api.multitenancy.listener.AssertingCaseExecutionListener.DelegateCaseExecutionAsserter hasTenantId(final String expectedTenantId)
	  protected internal static AssertingCaseExecutionListener.DelegateCaseExecutionAsserter hasTenantId(string expectedTenantId)
	  {
		return new DelegateCaseExecutionAsserterAnonymousInnerClass(expectedTenantId);
	  }

	  private class DelegateCaseExecutionAsserterAnonymousInnerClass : AssertingCaseExecutionListener.DelegateCaseExecutionAsserter
	  {
		  private string expectedTenantId;

		  public DelegateCaseExecutionAsserterAnonymousInnerClass(string expectedTenantId)
		  {
			  this.expectedTenantId = expectedTenantId;
		  }


		  public void doAssert(DelegateCaseExecution execution)
		  {
			assertThat(execution.TenantId, @is(expectedTenantId));
		  }
	  }

	}

}