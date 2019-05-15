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

	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
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

	public class MultiTenancyMessageCorrelationCmdTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyMessageCorrelationCmdTenantCheckTest()
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

	  protected internal static readonly BpmnModelInstance MESSAGE_START_PROCESS = Bpmn.createExecutableProcess("messageStart").startEvent().message("message").userTask().endEvent().done();

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
//ORIGINAL LINE: @Test public void correlateMessageToStartEventNoAuthenticatedTenants()
	  public virtual void correlateMessageToStartEventNoAuthenticatedTenants()
	  {
		testRule.deployForTenant(TENANT_ONE, MESSAGE_START_PROCESS);
		testRule.deployForTenant(TENANT_TWO, MESSAGE_START_PROCESS);
		testRule.deploy(MESSAGE_START_PROCESS);

		engineRule.IdentityService.setAuthentication("user", null, null);

		engineRule.RuntimeService.createMessageCorrelation("message").correlateStartMessage();

		engineRule.IdentityService.clearAuthentication();

		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery();
		assertThat(query.count(), @is(1L));
		assertThat(query.withoutTenantId().count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void correlateMessageToStartEventWithAuthenticatedTenant()
	  public virtual void correlateMessageToStartEventWithAuthenticatedTenant()
	  {
		testRule.deployForTenant(TENANT_ONE, MESSAGE_START_PROCESS);
		testRule.deployForTenant(TENANT_TWO, MESSAGE_START_PROCESS);

		engineRule.IdentityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		engineRule.RuntimeService.createMessageCorrelation("message").correlateStartMessage();

		engineRule.IdentityService.clearAuthentication();

		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery();
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void correlateMessageToStartEventDisabledTenantCheck()
	  public virtual void correlateMessageToStartEventDisabledTenantCheck()
	  {
		testRule.deployForTenant(TENANT_ONE, MESSAGE_START_PROCESS);
		testRule.deployForTenant(TENANT_TWO, MESSAGE_START_PROCESS);

		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;
		engineRule.IdentityService.setAuthentication("user", null, null);

		engineRule.RuntimeService.createMessageCorrelation("message").tenantId(TENANT_ONE).correlateStartMessage();

		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery();
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void correlateMessageToIntermediateCatchEventNoAuthenticatedTenants()
	  public virtual void correlateMessageToIntermediateCatchEventNoAuthenticatedTenants()
	  {
		testRule.deployForTenant(TENANT_ONE, MESSAGE_CATCH_PROCESS);
		testRule.deployForTenant(TENANT_TWO, MESSAGE_CATCH_PROCESS);
		testRule.deploy(MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionTenantId(TENANT_ONE).execute();
		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionTenantId(TENANT_TWO).execute();
		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionWithoutTenantId().execute();

		engineRule.IdentityService.setAuthentication("user", null, null);

		engineRule.RuntimeService.createMessageCorrelation("message").correlate();

		engineRule.IdentityService.clearAuthentication();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.withoutTenantId().count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(0L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void correlateMessageToIntermediateCatchEventWithAuthenticatedTenant()
	  public virtual void correlateMessageToIntermediateCatchEventWithAuthenticatedTenant()
	  {
		testRule.deployForTenant(TENANT_ONE, MESSAGE_CATCH_PROCESS);
		testRule.deployForTenant(TENANT_TWO, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionTenantId(TENANT_ONE).execute();
		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionTenantId(TENANT_TWO).execute();

		engineRule.IdentityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		engineRule.RuntimeService.createMessageCorrelation("message").correlate();

		engineRule.IdentityService.clearAuthentication();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void correlateMessageToIntermediateCatchEventDisabledTenantCheck()
	  public virtual void correlateMessageToIntermediateCatchEventDisabledTenantCheck()
	  {
		testRule.deployForTenant(TENANT_ONE, MESSAGE_CATCH_PROCESS);
		testRule.deployForTenant(TENANT_TWO, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionTenantId(TENANT_ONE).execute();
		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionTenantId(TENANT_TWO).execute();

		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;
		engineRule.IdentityService.setAuthentication("user", null, null);

		engineRule.RuntimeService.createMessageCorrelation("message").tenantId(TENANT_ONE).correlate();

		engineRule.IdentityService.clearAuthentication();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void correlateMessageToStartAndIntermediateCatchEventWithNoAuthenticatedTenants()
	  public virtual void correlateMessageToStartAndIntermediateCatchEventWithNoAuthenticatedTenants()
	  {
		testRule.deploy(MESSAGE_START_PROCESS, MESSAGE_CATCH_PROCESS);
		testRule.deployForTenant(TENANT_ONE, MESSAGE_START_PROCESS, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionWithoutTenantId().execute();
		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionTenantId(TENANT_ONE).execute();

		engineRule.IdentityService.setAuthentication("user", null, null);

		engineRule.RuntimeService.createMessageCorrelation("message").correlateAll();

		engineRule.IdentityService.clearAuthentication();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.count(), @is(2L));
		assertThat(query.withoutTenantId().count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void correlateMessageToStartAndIntermediateCatchEventWithAuthenticatedTenant()
	  public virtual void correlateMessageToStartAndIntermediateCatchEventWithAuthenticatedTenant()
	  {
		testRule.deployForTenant(TENANT_TWO, MESSAGE_START_PROCESS, MESSAGE_CATCH_PROCESS);
		testRule.deployForTenant(TENANT_ONE, MESSAGE_START_PROCESS, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionTenantId(TENANT_ONE).execute();
		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionTenantId(TENANT_TWO).execute();

		engineRule.IdentityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		engineRule.RuntimeService.createMessageCorrelation("message").correlateAll();

		engineRule.IdentityService.clearAuthentication();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void correlateMessageToStartAndIntermediateCatchEventDisabledTenantCheck()
	  public virtual void correlateMessageToStartAndIntermediateCatchEventDisabledTenantCheck()
	  {
		testRule.deployForTenant(TENANT_TWO, MESSAGE_START_PROCESS, MESSAGE_CATCH_PROCESS);
		testRule.deployForTenant(TENANT_ONE, MESSAGE_START_PROCESS, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionTenantId(TENANT_ONE).execute();
		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionTenantId(TENANT_TWO).execute();

		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;
		engineRule.IdentityService.setAuthentication("user", null, null);

		engineRule.RuntimeService.createMessageCorrelation("message").correlateAll();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.count(), @is(4L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(2L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToCorrelateMessageByProcessInstanceIdNoAuthenticatedTenants()
	  public virtual void failToCorrelateMessageByProcessInstanceIdNoAuthenticatedTenants()
	  {
		testRule.deployForTenant(TENANT_ONE, MESSAGE_CATCH_PROCESS);

		ProcessInstance processInstance = engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionTenantId(TENANT_ONE).execute();

		// declared expected exception
		thrown.expect(typeof(MismatchingMessageCorrelationException));
		thrown.expectMessage("Cannot correlate message");

		engineRule.IdentityService.setAuthentication("user", null, null);

		engineRule.RuntimeService.createMessageCorrelation("message").processInstanceId(processInstance.Id).correlate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void correlateMessageByProcessInstanceIdWithAuthenticatedTenant()
	  public virtual void correlateMessageByProcessInstanceIdWithAuthenticatedTenant()
	  {
		testRule.deployForTenant(TENANT_ONE, MESSAGE_CATCH_PROCESS);

		ProcessInstance processInstance = engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").execute();

		engineRule.IdentityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		engineRule.RuntimeService.createMessageCorrelation("message").processInstanceId(processInstance.Id).correlate();

		engineRule.IdentityService.clearAuthentication();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToCorrelateMessageByProcessDefinitionIdNoAuthenticatedTenants()
	  public virtual void failToCorrelateMessageByProcessDefinitionIdNoAuthenticatedTenants()
	  {
		testRule.deployForTenant(TENANT_ONE, MESSAGE_START_PROCESS);

		ProcessDefinition processDefinition = engineRule.RepositoryService.createProcessDefinitionQuery().processDefinitionKey("messageStart").tenantIdIn(TENANT_ONE).singleResult();

		// declare expected exception
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot create an instance of the process definition");

		engineRule.IdentityService.setAuthentication("user", null, null);

		engineRule.RuntimeService.createMessageCorrelation("message").processDefinitionId(processDefinition.Id).correlateStartMessage();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void correlateMessageByProcessDefinitionIdWithAuthenticatedTenant()
	  public virtual void correlateMessageByProcessDefinitionIdWithAuthenticatedTenant()
	  {
		testRule.deployForTenant(TENANT_ONE, MESSAGE_START_PROCESS);

		ProcessDefinition processDefinition = engineRule.RepositoryService.createProcessDefinitionQuery().processDefinitionKey("messageStart").singleResult();

		engineRule.IdentityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		engineRule.RuntimeService.createMessageCorrelation("message").processDefinitionId(processDefinition.Id).correlateStartMessage();

		engineRule.IdentityService.clearAuthentication();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

	}

}