using System;

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
//	import static org.hamcrest.CoreMatchers.hasItem;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using Authentication = org.camunda.bpm.engine.impl.identity.Authentication;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using AssertingJavaDelegate = org.camunda.bpm.engine.test.api.@delegate.AssertingJavaDelegate;
	using DelegateExecutionAsserter = org.camunda.bpm.engine.test.api.@delegate.AssertingJavaDelegate.DelegateExecutionAsserter;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using After = org.junit.After;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	public class MultiTenancyJobExecutorTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyJobExecutorTest()
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
			ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  protected internal const string TENANT_ID = "tenant1";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setAuthenticatedTenantForTimerStartEvent()
	  public virtual void setAuthenticatedTenantForTimerStartEvent()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		testRule.deployForTenant(TENANT_ID, Bpmn.createExecutableProcess("process").startEvent().timerWithDuration("PT1M").serviceTask().camundaClass(typeof(AssertingJavaDelegate).FullName).userTask().endEvent().done());

		AssertingJavaDelegate.addAsserts(hasAuthenticatedTenantId(TENANT_ID));

		ClockUtil.CurrentTime = tomorrow();
		testRule.waitForJobExecutorToProcessAllJobs();

		assertThat(engineRule.TaskService.createTaskQuery().count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setAuthenticatedTenantForIntermediateTimerEvent()
	  public virtual void setAuthenticatedTenantForIntermediateTimerEvent()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		testRule.deployForTenant(TENANT_ID, Bpmn.createExecutableProcess("process").startEvent().intermediateCatchEvent().timerWithDuration("PT1M").serviceTask().camundaClass(typeof(AssertingJavaDelegate).FullName).endEvent().done());

		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceByKey("process");

		AssertingJavaDelegate.addAsserts(hasAuthenticatedTenantId(TENANT_ID));

		ClockUtil.CurrentTime = tomorrow();
		testRule.waitForJobExecutorToProcessAllJobs();
		testRule.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setAuthenticatedTenantForAsyncJob()
	  public virtual void setAuthenticatedTenantForAsyncJob()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		testRule.deployForTenant(TENANT_ID, Bpmn.createExecutableProcess("process").startEvent().serviceTask().camundaAsyncBefore().camundaClass(typeof(AssertingJavaDelegate).FullName).endEvent().done());

		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceByKey("process");

		AssertingJavaDelegate.addAsserts(hasAuthenticatedTenantId(TENANT_ID));

		testRule.waitForJobExecutorToProcessAllJobs();
		testRule.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void dontSetAuthenticatedTenantForJobWithoutTenant()
	  public virtual void dontSetAuthenticatedTenantForJobWithoutTenant()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		testRule.deploy(Bpmn.createExecutableProcess("process").startEvent().serviceTask().camundaAsyncBefore().camundaClass(typeof(AssertingJavaDelegate).FullName).endEvent().done());

		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceByKey("process");

		AssertingJavaDelegate.addAsserts(hasNoAuthenticatedTenantId());

		testRule.waitForJobExecutorToProcessAllJobs();
		testRule.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void dontSetAuthenticatedTenantWhileManualJobExecution()
	  public virtual void dontSetAuthenticatedTenantWhileManualJobExecution()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		testRule.deployForTenant(TENANT_ID, Bpmn.createExecutableProcess("process").startEvent().serviceTask().camundaAsyncBefore().camundaClass(typeof(AssertingJavaDelegate).FullName).endEvent().done());

		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceByKey("process");

		AssertingJavaDelegate.addAsserts(hasNoAuthenticatedTenantId());

		testRule.executeAvailableJobs();
		testRule.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected static org.camunda.bpm.engine.test.api.delegate.AssertingJavaDelegate.DelegateExecutionAsserter hasAuthenticatedTenantId(final String expectedTenantId)
	  protected internal static AssertingJavaDelegate.DelegateExecutionAsserter hasAuthenticatedTenantId(string expectedTenantId)
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
			IdentityService identityService = execution.ProcessEngineServices.IdentityService;

			Authentication currentAuthentication = identityService.CurrentAuthentication;
			assertThat(currentAuthentication, @is(notNullValue()));
			assertThat(currentAuthentication.TenantIds, hasItem(expectedTenantId));
		  }
	  }

	  protected internal static AssertingJavaDelegate.DelegateExecutionAsserter hasNoAuthenticatedTenantId()
	  {
		return new DelegateExecutionAsserterAnonymousInnerClass2();
	  }

	  private class DelegateExecutionAsserterAnonymousInnerClass2 : AssertingJavaDelegate.DelegateExecutionAsserter
	  {

		  public void doAssert(DelegateExecution execution)
		  {
			IdentityService identityService = execution.ProcessEngineServices.IdentityService;

			Authentication currentAuthentication = identityService.CurrentAuthentication;
			assertThat(currentAuthentication, @is(nullValue()));
		  }
	  }

	  protected internal virtual DateTime tomorrow()
	  {
		DateTime calendar = new DateTime();
		calendar.AddDays(1);

		return calendar;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void tearDown()
	  {
		AssertingJavaDelegate.clear();
	  }

	}

}