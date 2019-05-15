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
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author kristin.polenz
	/// </summary>
	public class MultiTenancySignalReceiveCmdTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancySignalReceiveCmdTenantCheckTest()
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

	  protected internal static readonly BpmnModelInstance SIGNAL_START_PROCESS = Bpmn.createExecutableProcess("signalStart").startEvent().signal("signal").userTask().endEvent().done();

	  protected internal static readonly BpmnModelInstance SIGNAL_CATCH_PROCESS = Bpmn.createExecutableProcess("signalCatch").startEvent().intermediateCatchEvent().signal("signal").userTask().endEvent().done();

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown= org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sendSignalToStartEventNoAuthenticatedTenants()
	  public virtual void sendSignalToStartEventNoAuthenticatedTenants()
	  {
		testRule.deploy(SIGNAL_START_PROCESS);
		testRule.deployForTenant(TENANT_ONE, SIGNAL_START_PROCESS);

		engineRule.IdentityService.setAuthentication("user", null, null);

		engineRule.RuntimeService.createSignalEvent("signal").send();

		engineRule.IdentityService.clearAuthentication();

		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery();
		assertThat(query.count(), @is(1L));
		assertThat(query.withoutTenantId().count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sendSignalToStartEventWithAuthenticatedTenant()
	  public virtual void sendSignalToStartEventWithAuthenticatedTenant()
	  {
		testRule.deployForTenant(TENANT_ONE, SIGNAL_START_PROCESS);
		testRule.deployForTenant(TENANT_TWO, SIGNAL_START_PROCESS);

		engineRule.IdentityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		engineRule.RuntimeService.createSignalEvent("signal").send();

		engineRule.IdentityService.clearAuthentication();

		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery();
		assertThat(query.count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sendSignalToStartEventDisabledTenantCheck()
	  public virtual void sendSignalToStartEventDisabledTenantCheck()
	  {
		testRule.deployForTenant(TENANT_ONE, SIGNAL_START_PROCESS);
		testRule.deployForTenant(TENANT_TWO, SIGNAL_START_PROCESS);

		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;
		engineRule.IdentityService.setAuthentication("user", null, null);

		engineRule.RuntimeService.createSignalEvent("signal").send();

		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery();
		assertThat(query.count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sendSignalToIntermediateCatchEventNoAuthenticatedTenants()
	  public virtual void sendSignalToIntermediateCatchEventNoAuthenticatedTenants()
	  {
		testRule.deploy(SIGNAL_CATCH_PROCESS);
		testRule.deployForTenant(TENANT_ONE, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionWithoutTenantId().execute();
		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionTenantId(TENANT_ONE).execute();

		engineRule.IdentityService.setAuthentication("user", null, null);

		engineRule.RuntimeService.createSignalEvent("signal").send();

		engineRule.IdentityService.clearAuthentication();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.count(), @is(1L));
		assertThat(query.withoutTenantId().count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sendSignalToIntermediateCatchEventWithAuthenticatedTenant()
	  public virtual void sendSignalToIntermediateCatchEventWithAuthenticatedTenant()
	  {
		testRule.deployForTenant(TENANT_ONE, SIGNAL_CATCH_PROCESS);
		testRule.deployForTenant(TENANT_TWO, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionTenantId(TENANT_ONE).execute();
		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionTenantId(TENANT_TWO).execute();

		engineRule.IdentityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		engineRule.RuntimeService.createSignalEvent("signal").send();

		engineRule.IdentityService.clearAuthentication();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sendSignalToIntermediateCatchEventDisabledTenantCheck()
	  public virtual void sendSignalToIntermediateCatchEventDisabledTenantCheck()
	  {
		testRule.deployForTenant(TENANT_ONE, SIGNAL_CATCH_PROCESS);
		testRule.deployForTenant(TENANT_TWO, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionTenantId(TENANT_ONE).execute();
		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionTenantId(TENANT_TWO).execute();

		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;
		engineRule.IdentityService.setAuthentication("user", null, null);

		engineRule.RuntimeService.createSignalEvent("signal").send();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sendSignalToStartAndIntermediateCatchEventNoAuthenticatedTenants()
	  public virtual void sendSignalToStartAndIntermediateCatchEventNoAuthenticatedTenants()
	  {
		testRule.deploy(SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS);
		testRule.deployForTenant(TENANT_ONE, SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionWithoutTenantId().execute();
		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionTenantId(TENANT_ONE).execute();

		engineRule.IdentityService.setAuthentication("user", null, null);

		engineRule.RuntimeService.createSignalEvent("signal").send();

		engineRule.IdentityService.clearAuthentication();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.count(), @is(2L));
		assertThat(query.withoutTenantId().count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sendSignalToStartAndIntermediateCatchEventWithAuthenticatedTenant()
	  public virtual void sendSignalToStartAndIntermediateCatchEventWithAuthenticatedTenant()
	  {
		testRule.deployForTenant(TENANT_ONE, SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS);
		testRule.deployForTenant(TENANT_TWO, SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionTenantId(TENANT_ONE).execute();
		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionTenantId(TENANT_TWO).execute();

		engineRule.IdentityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		engineRule.RuntimeService.createSignalEvent("signal").send();

		engineRule.IdentityService.clearAuthentication();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sendSignalToStartAndIntermediateCatchEventDisabledTenantCheck()
	  public virtual void sendSignalToStartAndIntermediateCatchEventDisabledTenantCheck()
	  {
		testRule.deployForTenant(TENANT_ONE, SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS);
		testRule.deployForTenant(TENANT_TWO, SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionTenantId(TENANT_ONE).execute();
		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionTenantId(TENANT_TWO).execute();

		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;
		engineRule.IdentityService.setAuthentication("user", null, null);

		engineRule.RuntimeService.createSignalEvent("signal").send();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.count(), @is(4L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(2L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sendSignalToIntermediateCatchEventWithExecutionIdAndAuthenticatedTenant()
	  public virtual void sendSignalToIntermediateCatchEventWithExecutionIdAndAuthenticatedTenant()
	  {
		testRule.deployForTenant(TENANT_ONE, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionTenantId(TENANT_ONE).execute();

		Execution execution = engineRule.RuntimeService.createExecutionQuery().processDefinitionKey("signalCatch").signalEventSubscriptionName("signal").singleResult();

		engineRule.IdentityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		engineRule.RuntimeService.createSignalEvent("signal").executionId(execution.Id).send();

		engineRule.IdentityService.clearAuthentication();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToSendSignalToIntermediateCatchEventWithExecutionIdAndNoAuthenticatedTenants()
	  public virtual void failToSendSignalToIntermediateCatchEventWithExecutionIdAndNoAuthenticatedTenants()
	  {
		testRule.deployForTenant(TENANT_ONE, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionTenantId(TENANT_ONE).execute();

		Execution execution = engineRule.RuntimeService.createExecutionQuery().processDefinitionKey("signalCatch").signalEventSubscriptionName("signal").singleResult();

		// declare expected exception
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the process instance");

		engineRule.IdentityService.setAuthentication("user", null, null);

		engineRule.RuntimeService.createSignalEvent("signal").executionId(execution.Id).send();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void signalIntermediateCatchEventNoAuthenticatedTenants()
	  public virtual void signalIntermediateCatchEventNoAuthenticatedTenants()
	  {
		testRule.deploy(SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").execute();

		Execution execution = engineRule.RuntimeService.createExecutionQuery().processDefinitionKey("signalCatch").signalEventSubscriptionName("signal").singleResult();

		engineRule.IdentityService.setAuthentication("user", null, null);

		engineRule.RuntimeService.signal(execution.Id, "signal", null, null);

		engineRule.IdentityService.clearAuthentication();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.count(), @is(1L));
		assertThat(query.withoutTenantId().count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void signalIntermediateCatchEventWithAuthenticatedTenant()
	  public virtual void signalIntermediateCatchEventWithAuthenticatedTenant()
	  {
		testRule.deployForTenant(TENANT_ONE, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionTenantId(TENANT_ONE).execute();

		Execution execution = engineRule.RuntimeService.createExecutionQuery().processDefinitionKey("signalCatch").signalEventSubscriptionName("signal").singleResult();

		engineRule.IdentityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		engineRule.RuntimeService.signal(execution.Id, "signal", null, null);

		engineRule.IdentityService.clearAuthentication();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void signalIntermediateCatchEventDisabledTenantCheck()
	  public virtual void signalIntermediateCatchEventDisabledTenantCheck()
	  {
		testRule.deployForTenant(TENANT_ONE, SIGNAL_CATCH_PROCESS);
		testRule.deployForTenant(TENANT_TWO, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionTenantId(TENANT_ONE).execute();
		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionTenantId(TENANT_TWO).execute();

		Execution execution = engineRule.RuntimeService.createExecutionQuery().processDefinitionKey("signalCatch").signalEventSubscriptionName("signal").tenantIdIn(TENANT_ONE).singleResult();

		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;
		engineRule.IdentityService.setAuthentication("user", null, null);

		engineRule.RuntimeService.signal(execution.Id, "signal", null, null);

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToSignalIntermediateCatchEventNoAuthenticatedTenants()
	  public virtual void failToSignalIntermediateCatchEventNoAuthenticatedTenants()
	  {
		testRule.deployForTenant(TENANT_ONE, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").execute();

		Execution execution = engineRule.RuntimeService.createExecutionQuery().processDefinitionKey("signalCatch").signalEventSubscriptionName("signal").singleResult();

		// declared expected exception
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the process instance");

		engineRule.IdentityService.setAuthentication("user", null, null);

		engineRule.RuntimeService.signal(execution.Id, "signal", null, null);
	  }
	}

}