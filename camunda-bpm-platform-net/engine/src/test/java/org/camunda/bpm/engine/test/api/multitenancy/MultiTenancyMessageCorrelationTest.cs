using System.Collections.Generic;

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
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	public class MultiTenancyMessageCorrelationTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyMessageCorrelationTest()
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
//ORIGINAL LINE: @Test public void correlateMessageToStartEventNoTenantIdSetForNonTenant()
	  public virtual void correlateMessageToStartEventNoTenantIdSetForNonTenant()
	  {
		testRule.deploy(MESSAGE_START_PROCESS);

		engineRule.RuntimeService.createMessageCorrelation("message").correlate();

		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery();
		assertThat(query.count(), @is(1L));
		assertThat(query.singleResult().TenantId, @is(nullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void correlateMessageToStartEventNoTenantIdSetForTenant()
	  public virtual void correlateMessageToStartEventNoTenantIdSetForTenant()
	  {
		testRule.deployForTenant(TENANT_ONE, MESSAGE_START_PROCESS);

		engineRule.RuntimeService.createMessageCorrelation("message").correlate();

		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery();
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void correlateMessageToStartEventWithoutTenantId()
	  public virtual void correlateMessageToStartEventWithoutTenantId()
	  {
		testRule.deploy(MESSAGE_START_PROCESS);
		testRule.deployForTenant(TENANT_ONE, MESSAGE_START_PROCESS);

		engineRule.RuntimeService.createMessageCorrelation("message").withoutTenantId().correlate();

		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery();
		assertThat(query.count(), @is(1L));
		assertThat(query.singleResult().TenantId, @is(nullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void correlateMessageToStartEventWithTenantId()
	  public virtual void correlateMessageToStartEventWithTenantId()
	  {
		testRule.deployForTenant(TENANT_ONE, MESSAGE_START_PROCESS);
		testRule.deployForTenant(TENANT_TWO, MESSAGE_START_PROCESS);

		engineRule.RuntimeService.createMessageCorrelation("message").tenantId(TENANT_ONE).correlate();

		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery();
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void correlateMessageToIntermediateCatchEventNoTenantIdSetForNonTenant()
	  public virtual void correlateMessageToIntermediateCatchEventNoTenantIdSetForNonTenant()
	  {
		testRule.deploy(MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.startProcessInstanceByKey("messageCatch");

		engineRule.RuntimeService.createMessageCorrelation("message").correlate();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void correlateMessageToIntermediateCatchEventNoTenantIdSetForTenant()
	  public virtual void correlateMessageToIntermediateCatchEventNoTenantIdSetForTenant()
	  {
		testRule.deployForTenant(TENANT_ONE, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.startProcessInstanceByKey("messageCatch");

		engineRule.RuntimeService.createMessageCorrelation("message").correlate();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void correlateMessageToIntermediateCatchEventWithoutTenantId()
	  public virtual void correlateMessageToIntermediateCatchEventWithoutTenantId()
	  {
		testRule.deploy(MESSAGE_CATCH_PROCESS);
		testRule.deployForTenant(TENANT_ONE, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionWithoutTenantId().execute();
		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionTenantId(TENANT_ONE).execute();

		engineRule.RuntimeService.createMessageCorrelation("message").withoutTenantId().correlate();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.count(), @is(1L));
		assertThat(query.singleResult().TenantId, @is(nullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void correlateMessageToIntermediateCatchEventWithTenantId()
	  public virtual void correlateMessageToIntermediateCatchEventWithTenantId()
	  {
		testRule.deployForTenant(TENANT_ONE, MESSAGE_CATCH_PROCESS);
		testRule.deployForTenant(TENANT_TWO, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionTenantId(TENANT_ONE).execute();
		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionTenantId(TENANT_TWO).execute();

		engineRule.RuntimeService.createMessageCorrelation("message").tenantId(TENANT_ONE).correlate();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void correlateMessageToStartAndIntermediateCatchEventWithoutTenantId()
	  public virtual void correlateMessageToStartAndIntermediateCatchEventWithoutTenantId()
	  {
		testRule.deploy(MESSAGE_START_PROCESS, MESSAGE_CATCH_PROCESS);
		testRule.deployForTenant(TENANT_ONE, MESSAGE_START_PROCESS, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionWithoutTenantId().execute();

		engineRule.RuntimeService.createMessageCorrelation("message").withoutTenantId().correlateAll();

		IList<Task> tasks = engineRule.TaskService.createTaskQuery().list();
		assertThat(tasks.Count, @is(2));
		assertThat(tasks[0].TenantId, @is(nullValue()));
		assertThat(tasks[1].TenantId, @is(nullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void correlateMessageToStartAndIntermediateCatchEventWithTenantId()
	  public virtual void correlateMessageToStartAndIntermediateCatchEventWithTenantId()
	  {
		testRule.deployForTenant(TENANT_ONE, MESSAGE_START_PROCESS, MESSAGE_CATCH_PROCESS);
		testRule.deployForTenant(TENANT_TWO, MESSAGE_START_PROCESS, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionTenantId(TENANT_ONE).execute();
		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionTenantId(TENANT_TWO).execute();

		engineRule.RuntimeService.createMessageCorrelation("message").tenantId(TENANT_ONE).correlateAll();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void correlateMessageToMultipleIntermediateCatchEventsWithoutTenantId()
	  public virtual void correlateMessageToMultipleIntermediateCatchEventsWithoutTenantId()
	  {
		testRule.deploy(MESSAGE_CATCH_PROCESS);
		testRule.deployForTenant(TENANT_ONE, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionWithoutTenantId().execute();
		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionWithoutTenantId().execute();

		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionTenantId(TENANT_ONE).execute();
		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionTenantId(TENANT_ONE).execute();

		engineRule.RuntimeService.createMessageCorrelation("message").withoutTenantId().correlateAll();

		IList<Task> tasks = engineRule.TaskService.createTaskQuery().list();
		assertThat(tasks.Count, @is(2));
		assertThat(tasks[0].TenantId, @is(nullValue()));
		assertThat(tasks[1].TenantId, @is(nullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void correlateMessageToMultipleIntermediateCatchEventsWithTenantId()
	  public virtual void correlateMessageToMultipleIntermediateCatchEventsWithTenantId()
	  {
		testRule.deployForTenant(TENANT_ONE, MESSAGE_CATCH_PROCESS);
		testRule.deployForTenant(TENANT_TWO, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionTenantId(TENANT_ONE).execute();
		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionTenantId(TENANT_ONE).execute();

		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionTenantId(TENANT_TWO).execute();
		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionTenantId(TENANT_TWO).execute();

		engineRule.RuntimeService.createMessageCorrelation("message").tenantId(TENANT_ONE).correlateAll();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void correlateStartMessageWithoutTenantId()
	  public virtual void correlateStartMessageWithoutTenantId()
	  {
		testRule.deploy(MESSAGE_START_PROCESS);
		testRule.deployForTenant(TENANT_ONE, MESSAGE_START_PROCESS);

		engineRule.RuntimeService.createMessageCorrelation("message").withoutTenantId().correlateStartMessage();

		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery();
		assertThat(query.count(), @is(1L));
		assertThat(query.singleResult().TenantId, @is(nullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void correlateStartMessageWithTenantId()
	  public virtual void correlateStartMessageWithTenantId()
	  {
		testRule.deployForTenant(TENANT_ONE, MESSAGE_START_PROCESS);
		testRule.deployForTenant(TENANT_TWO, MESSAGE_START_PROCESS);

		engineRule.RuntimeService.createMessageCorrelation("message").tenantId(TENANT_ONE).correlateStartMessage();

		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery();
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void correlateMessagesToStartEventsForMultipleTenants()
	  public virtual void correlateMessagesToStartEventsForMultipleTenants()
	  {
		testRule.deployForTenant(TENANT_ONE, MESSAGE_START_PROCESS);
		testRule.deployForTenant(TENANT_TWO, MESSAGE_START_PROCESS);

		engineRule.RuntimeService.createMessageCorrelation("message").correlateAll();

		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery();
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void correlateMessagesToIntermediateCatchEventsForMultipleTenants()
	  public virtual void correlateMessagesToIntermediateCatchEventsForMultipleTenants()
	  {
		testRule.deployForTenant(TENANT_ONE, MESSAGE_CATCH_PROCESS);
		testRule.deployForTenant(TENANT_TWO, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionTenantId(TENANT_ONE).execute();
		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionTenantId(TENANT_TWO).execute();

		engineRule.RuntimeService.createMessageCorrelation("message").correlateAll();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void correlateMessagesToStartAndIntermediateCatchEventForMultipleTenants()
	  public virtual void correlateMessagesToStartAndIntermediateCatchEventForMultipleTenants()
	  {
		testRule.deployForTenant(TENANT_ONE, MESSAGE_START_PROCESS);
		testRule.deployForTenant(TENANT_TWO, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionTenantId(TENANT_TWO).execute();

		engineRule.RuntimeService.createMessageCorrelation("message").correlateAll();

		assertThat(engineRule.RuntimeService.createProcessInstanceQuery().tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(engineRule.TaskService.createTaskQuery().tenantIdIn(TENANT_TWO).count(), @is(1L));
	  }

	  public virtual void failToCorrelateMessageToIntermediateCatchEventsForMultipleTenants()
	  {
		testRule.deployForTenant(TENANT_ONE, MESSAGE_CATCH_PROCESS);
		testRule.deployForTenant(TENANT_TWO, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionTenantId(TENANT_ONE).execute();
		engineRule.RuntimeService.createProcessInstanceByKey("messageCatch").processDefinitionTenantId(TENANT_TWO).execute();

		// declare expected exception
		thrown.expect(typeof(MismatchingMessageCorrelationException));
		thrown.expectMessage("Cannot correlate a message with name 'message' to a single execution");

		engineRule.RuntimeService.createMessageCorrelation("message").correlate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubscriptionsWhenDeletingGroupsProcessDefinitionsByIds()
	  public virtual void testSubscriptionsWhenDeletingGroupsProcessDefinitionsByIds()
	  {
		// given
		string processDefId1 = testRule.deployForTenantAndGetDefinition(TENANT_ONE, MESSAGE_START_PROCESS).Id;
		string processDefId2 = testRule.deployForTenantAndGetDefinition(TENANT_ONE, MESSAGE_START_PROCESS).Id;
		string processDefId3 = testRule.deployForTenantAndGetDefinition(TENANT_ONE, MESSAGE_START_PROCESS).Id;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unused") String processDefId4 = testRule.deployAndGetDefinition(MESSAGE_START_PROCESS).getId();
		string processDefId4 = testRule.deployAndGetDefinition(MESSAGE_START_PROCESS).Id;
		string processDefId5 = testRule.deployAndGetDefinition(MESSAGE_START_PROCESS).Id;
		string processDefId6 = testRule.deployAndGetDefinition(MESSAGE_START_PROCESS).Id;

		BpmnModelInstance processAnotherKey = Bpmn.createExecutableProcess("anotherKey").startEvent().message("sophisticated message").userTask().endEvent().done();

		string processDefId7 = testRule.deployForTenantAndGetDefinition(TENANT_ONE, processAnotherKey).Id;
		string processDefId8 = testRule.deployForTenantAndGetDefinition(TENANT_ONE, processAnotherKey).Id;
		string processDefId9 = testRule.deployForTenantAndGetDefinition(TENANT_ONE, processAnotherKey).Id;

		// assume
		assertEquals(3, engineRule.RuntimeService.createEventSubscriptionQuery().count());

		// when
		engineRule.RepositoryService.deleteProcessDefinitions().byIds(processDefId8, processDefId5, processDefId3, processDefId9, processDefId1).delete();

		// then
		IList<EventSubscription> list = engineRule.RuntimeService.createEventSubscriptionQuery().list();
		assertEquals(3, list.Count);
		foreach (EventSubscription eventSubscription in list)
		{
		  EventSubscriptionEntity eventSubscriptionEntity = (EventSubscriptionEntity) eventSubscription;
		  if (eventSubscriptionEntity.Configuration.Equals(processDefId2))
		  {
			assertEquals(TENANT_ONE, eventSubscription.TenantId);
		  }
		  else if (eventSubscriptionEntity.Configuration.Equals(processDefId6))
		  {
			assertEquals(null, eventSubscription.TenantId);
		  }
		  else if (eventSubscriptionEntity.Configuration.Equals(processDefId7))
		  {
			assertEquals(TENANT_ONE, eventSubscription.TenantId);
		  }
		  else
		  {
			fail("This process definition '" + eventSubscriptionEntity.Configuration + "' and the respective event subscription should not exist.");
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToCorrelateMessageToStartEventsForMultipleTenants()
	  public virtual void failToCorrelateMessageToStartEventsForMultipleTenants()
	  {
		testRule.deployForTenant(TENANT_ONE, MESSAGE_START_PROCESS);
		testRule.deployForTenant(TENANT_TWO, MESSAGE_START_PROCESS);

		// declare expected exception
		thrown.expect(typeof(MismatchingMessageCorrelationException));
		thrown.expectMessage("Cannot correlate a message with name 'message' to a single process definition");

		engineRule.RuntimeService.createMessageCorrelation("message").correlate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToCorrelateStartMessageForMultipleTenants()
	  public virtual void failToCorrelateStartMessageForMultipleTenants()
	  {
		testRule.deployForTenant(TENANT_ONE, MESSAGE_START_PROCESS);
		testRule.deployForTenant(TENANT_TWO, MESSAGE_START_PROCESS);

		// declare expected exception
		thrown.expect(typeof(MismatchingMessageCorrelationException));
		thrown.expectMessage("Cannot correlate a message with name 'message' to a single process definition");

		engineRule.RuntimeService.createMessageCorrelation("message").correlateStartMessage();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToCorrelateMessageByProcessInstanceIdWithoutTenantId()
	  public virtual void failToCorrelateMessageByProcessInstanceIdWithoutTenantId()
	  {
		// declare expected exception
		thrown.expect(typeof(BadUserRequestException));
		thrown.expectMessage("Cannot specify a tenant-id");

		engineRule.RuntimeService.createMessageCorrelation("message").processInstanceId("id").withoutTenantId().correlate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToCorrelateMessageByProcessInstanceIdAndTenantId()
	  public virtual void failToCorrelateMessageByProcessInstanceIdAndTenantId()
	  {
		// declare expected exception
		thrown.expect(typeof(BadUserRequestException));
		thrown.expectMessage("Cannot specify a tenant-id");

		engineRule.RuntimeService.createMessageCorrelation("message").processInstanceId("id").tenantId(TENANT_ONE).correlate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToCorrelateMessageByProcessDefinitionIdWithoutTenantId()
	  public virtual void failToCorrelateMessageByProcessDefinitionIdWithoutTenantId()
	  {
		// declare expected exception
		thrown.expect(typeof(BadUserRequestException));
		thrown.expectMessage("Cannot specify a tenant-id");

		engineRule.RuntimeService.createMessageCorrelation("message").processDefinitionId("id").withoutTenantId().correlateStartMessage();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToCorrelateMessageByProcessDefinitionIdAndTenantId()
	  public virtual void failToCorrelateMessageByProcessDefinitionIdAndTenantId()
	  {
		// declare expected exception
		thrown.expect(typeof(BadUserRequestException));
		thrown.expectMessage("Cannot specify a tenant-id");

		engineRule.RuntimeService.createMessageCorrelation("message").processDefinitionId("id").tenantId(TENANT_ONE).correlateStartMessage();
	  }

	}

}