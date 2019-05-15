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
namespace org.camunda.bpm.engine.test.api.multitenancy.tenantcheck
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;


	using EventType = org.camunda.bpm.engine.impl.@event.EventType;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	public class MultiTenancyStartProcessInstanceByConditionCmdTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyStartProcessInstanceByConditionCmdTenantCheckTest()
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

	  protected internal static readonly BpmnModelInstance PROCESS = Bpmn.createExecutableProcess("conditionStart").startEvent().conditionalEventDefinition().condition("${true}").conditionalEventDefinitionDone().userTask().endEvent().done();

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown= org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  public IdentityService identityService;
	  public RepositoryService repositoryService;
	  public RuntimeService runtimeService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		identityService = engineRule.IdentityService;
		repositoryService = engineRule.RepositoryService;
		runtimeService = engineRule.RuntimeService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoAuthenticatedTenants() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testNoAuthenticatedTenants()
	  {
		// given
		testRule.deployForTenant(TENANT_ONE, PROCESS);
		testRule.deployForTenant(TENANT_TWO, PROCESS);
		testRule.deploy(PROCESS);

		ensureEventSubscriptions(3);

		identityService.setAuthentication("user", null, null);

		IDictionary<string, object> variableMap = new Dictionary<string, object>();
		variableMap["foo"] = "bar";

		// when
		IList<ProcessInstance> instances = engineRule.RuntimeService.createConditionEvaluation().setVariables(variableMap).evaluateStartConditions();

		// then
		assertNotNull(instances);
		assertEquals(1, instances.Count);

		identityService.clearAuthentication();

		ProcessInstanceQuery processInstanceQuery = engineRule.RuntimeService.createProcessInstanceQuery();
		assertEquals(1, processInstanceQuery.count());
		assertEquals(1, processInstanceQuery.withoutTenantId().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWithAuthenticatedTenant() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testWithAuthenticatedTenant()
	  {
		// given
		testRule.deployForTenant(TENANT_ONE, PROCESS);
		testRule.deployForTenant(TENANT_TWO, PROCESS);

		ensureEventSubscriptions(2);

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		IDictionary<string, object> variableMap = new Dictionary<string, object>();
		variableMap["foo"] = "bar";

		// when
		IList<ProcessInstance> processInstances = engineRule.RuntimeService.createConditionEvaluation().setVariables(variableMap).tenantId(TENANT_ONE).evaluateStartConditions();

		// then
		assertNotNull(processInstances);
		assertEquals(1, processInstances.Count);

		identityService.clearAuthentication();

		ProcessInstanceQuery processInstanceQuery = engineRule.RuntimeService.createProcessInstanceQuery();
		assertEquals(1, processInstanceQuery.tenantIdIn(TENANT_ONE).count());
		assertEquals(0, processInstanceQuery.tenantIdIn(TENANT_TWO).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWithAuthenticatedTenant2() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testWithAuthenticatedTenant2()
	  {
		// given
		testRule.deployForTenant(TENANT_ONE, PROCESS);
		testRule.deployForTenant(TENANT_TWO, PROCESS);

		ensureEventSubscriptions(2);

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		IDictionary<string, object> variableMap = new Dictionary<string, object>();
		variableMap["foo"] = "bar";

		// when
		IList<ProcessInstance> processInstances = engineRule.RuntimeService.createConditionEvaluation().setVariables(variableMap).evaluateStartConditions();

		// then
		assertNotNull(processInstances);
		assertEquals(1, processInstances.Count);

		identityService.clearAuthentication();

		ProcessInstanceQuery processInstanceQuery = engineRule.RuntimeService.createProcessInstanceQuery();
		assertEquals(1, processInstanceQuery.tenantIdIn(TENANT_ONE).count());
		assertEquals(0, processInstanceQuery.tenantIdIn(TENANT_TWO).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDisabledTenantCheck() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testDisabledTenantCheck()
	  {
		// given
		testRule.deployForTenant(TENANT_ONE, PROCESS);
		testRule.deployForTenant(TENANT_TWO, PROCESS);

		ensureEventSubscriptions(2);

		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		IDictionary<string, object> variableMap = new Dictionary<string, object>();
		variableMap["foo"] = "bar";

		// when
		IList<ProcessInstance> evaluateStartConditions = engineRule.RuntimeService.createConditionEvaluation().setVariables(variableMap).evaluateStartConditions();
		assertEquals(2, evaluateStartConditions.Count);

		identityService.clearAuthentication();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailToEvaluateConditionByProcessDefinitionIdNoAuthenticatedTenants()
	  public virtual void testFailToEvaluateConditionByProcessDefinitionIdNoAuthenticatedTenants()
	  {
		// given
		testRule.deployForTenant(TENANT_ONE, PROCESS);

		ensureEventSubscriptions(1);

		ProcessDefinition processDefinition = engineRule.RepositoryService.createProcessDefinitionQuery().processDefinitionKey("conditionStart").singleResult();

		// expected
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot create an instance of the process definition");

		identityService.setAuthentication("user", null, null);

		// when
		engineRule.RuntimeService.createConditionEvaluation().setVariable("foo", "bar").processDefinitionId(processDefinition.Id).evaluateStartConditions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEvaluateConditionByProcessDefinitionIdWithAuthenticatedTenants()
	  public virtual void testEvaluateConditionByProcessDefinitionIdWithAuthenticatedTenants()
	  {
		// given
		testRule.deployForTenant(TENANT_ONE, PROCESS);

		ensureEventSubscriptions(1);

		ProcessDefinition processDefinition = engineRule.RepositoryService.createProcessDefinitionQuery().processDefinitionKey("conditionStart").singleResult();

		identityService = engineRule.IdentityService;
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		// when
		IList<ProcessInstance> instances = engineRule.RuntimeService.createConditionEvaluation().setVariable("foo", "bar").tenantId(TENANT_ONE).processDefinitionId(processDefinition.Id).evaluateStartConditions();

		// then
		assertNotNull(instances);
		assertEquals(1, instances.Count);
		assertEquals(TENANT_ONE, instances[0].TenantId);

		identityService.clearAuthentication();

		ProcessInstanceQuery processInstanceQuery = engineRule.RuntimeService.createProcessInstanceQuery();
		assertEquals(1, processInstanceQuery.tenantIdIn(TENANT_ONE).count());

		EventSubscription eventSubscription = engineRule.RuntimeService.createEventSubscriptionQuery().singleResult();
		assertEquals(EventType.CONDITONAL.name(), eventSubscription.EventType);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubscriptionsWhenDeletingGroupsProcessDefinitionsByIds()
	  public virtual void testSubscriptionsWhenDeletingGroupsProcessDefinitionsByIds()
	  {
		// given
		string processDefId1 = testRule.deployForTenantAndGetDefinition(TENANT_ONE, PROCESS).Id;
		string processDefId2 = testRule.deployForTenantAndGetDefinition(TENANT_ONE, PROCESS).Id;
		string processDefId3 = testRule.deployForTenantAndGetDefinition(TENANT_ONE, PROCESS).Id;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unused") String processDefId4 = testRule.deployAndGetDefinition(PROCESS).getId();
		string processDefId4 = testRule.deployAndGetDefinition(PROCESS).Id;
		string processDefId5 = testRule.deployAndGetDefinition(PROCESS).Id;
		string processDefId6 = testRule.deployAndGetDefinition(PROCESS).Id;

		BpmnModelInstance processAnotherKey = Bpmn.createExecutableProcess("anotherKey").startEvent().conditionalEventDefinition().condition("${true}").conditionalEventDefinitionDone().userTask().endEvent().done();

		string processDefId7 = testRule.deployForTenantAndGetDefinition(TENANT_ONE, processAnotherKey).Id;
		string processDefId8 = testRule.deployForTenantAndGetDefinition(TENANT_ONE, processAnotherKey).Id;
		string processDefId9 = testRule.deployForTenantAndGetDefinition(TENANT_ONE, processAnotherKey).Id;

		// assume
		assertEquals(3, runtimeService.createEventSubscriptionQuery().count());

		// when
		repositoryService.deleteProcessDefinitions().byIds(processDefId8, processDefId5, processDefId3, processDefId9, processDefId1).delete();

		// then
		IList<EventSubscription> list = runtimeService.createEventSubscriptionQuery().list();
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

	  protected internal virtual void ensureEventSubscriptions(int count)
	  {
		IList<EventSubscription> eventSubscriptions = engineRule.RuntimeService.createEventSubscriptionQuery().list();
		assertEquals(count, eventSubscriptions.Count);
		foreach (EventSubscription eventSubscription in eventSubscriptions)
		{
		  assertEquals(EventType.CONDITONAL.name(), eventSubscription.EventType);
		}
	  }
	}

}