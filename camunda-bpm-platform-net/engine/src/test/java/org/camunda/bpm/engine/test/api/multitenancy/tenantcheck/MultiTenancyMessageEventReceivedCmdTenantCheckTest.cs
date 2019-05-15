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
namespace org.camunda.bpm.engine.test.api.multitenancy.tenantcheck
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	public class MultiTenancyMessageEventReceivedCmdTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyMessageEventReceivedCmdTenantCheckTest()
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


	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal static readonly BpmnModelInstance MESSAGE_CATCH_PROCESS = Bpmn.createExecutableProcess("messageCatch").startEvent().intermediateCatchEvent().message("message").userTask().endEvent().done();

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown= org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void correlateReceivedMessageToIntermediateCatchEventNoAuthenticatedTenants()
	  public virtual void correlateReceivedMessageToIntermediateCatchEventNoAuthenticatedTenants()
	  {
		testRule.deploy(MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").execute();

		Execution execution = engineRule.RuntimeService.createExecutionQuery().processDefinitionKey("messageCatch").messageEventSubscriptionName("message").singleResult();

		engineRule.IdentityService.setAuthentication("user", null, null);

		engineRule.RuntimeService.messageEventReceived("message", execution.Id);

		engineRule.IdentityService.clearAuthentication();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.count(), @is(1L));
		assertThat(query.withoutTenantId().count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void correlateReceivedMessageToIntermediateCatchEventWithAuthenticatedTenant()
	  public virtual void correlateReceivedMessageToIntermediateCatchEventWithAuthenticatedTenant()
	  {
		testRule.deployForTenant(TENANT_ONE, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").execute();

		Execution execution = engineRule.RuntimeService.createExecutionQuery().processDefinitionKey("messageCatch").messageEventSubscriptionName("message").singleResult();

		engineRule.IdentityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		engineRule.RuntimeService.messageEventReceived("message", execution.Id);

		engineRule.IdentityService.clearAuthentication();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void correlateReceivedMessageToIntermediateCatchEventDisabledTenantCheck()
	  public virtual void correlateReceivedMessageToIntermediateCatchEventDisabledTenantCheck()
	  {
		testRule.deployForTenant(TENANT_ONE, MESSAGE_CATCH_PROCESS);
		testRule.deployForTenant(TENANT_TWO, MESSAGE_CATCH_PROCESS);

		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;
		engineRule.IdentityService.setAuthentication("user", null, null);

		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionTenantId(TENANT_ONE).execute();
		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionTenantId(TENANT_TWO).execute();

		Execution execution = engineRule.RuntimeService.createExecutionQuery().processDefinitionKey("messageCatch").messageEventSubscriptionName("message").tenantIdIn(TENANT_ONE).singleResult();

		engineRule.RuntimeService.messageEventReceived("message", execution.Id);

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToCorrelateReceivedMessageToIntermediateCatchEventNoAuthenticatedTenants()
	  public virtual void failToCorrelateReceivedMessageToIntermediateCatchEventNoAuthenticatedTenants()
	  {
		testRule.deployForTenant(TENANT_ONE, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionTenantId(TENANT_ONE).execute();

		Execution execution = engineRule.RuntimeService.createExecutionQuery().processDefinitionKey("messageCatch").messageEventSubscriptionName("message").tenantIdIn(TENANT_ONE).singleResult();

		// declare expected exception
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the process instance");

		engineRule.IdentityService.setAuthentication("user", null, null);

		engineRule.RuntimeService.messageEventReceived("message", execution.Id);
	  }

	}

}