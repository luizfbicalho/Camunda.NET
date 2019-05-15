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
namespace org.camunda.bpm.engine.test.api.multitenancy.query.history
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.*;

	using HistoricIdentityLinkLog = org.camunda.bpm.engine.history.HistoricIdentityLinkLog;
	using HistoricIdentityLinkLogQuery = org.camunda.bpm.engine.history.HistoricIdentityLinkLogQuery;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using IdentityLink = org.camunda.bpm.engine.task.IdentityLink;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// 
	/// <summary>
	/// @author Deivarayan Azhagappan
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class MultiTenancyHistoricIdentityLinkLogQueryTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyHistoricIdentityLinkLogQueryTest()
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


	  private const string GROUP_1 = "Group1";
	  private const string USER_1 = "User1";

	  private static string PROCESS_DEFINITION_KEY = "oneTaskProcess";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

	  protected internal HistoryService historyService;
	  protected internal RuntimeService runtimeService;
	  protected internal RepositoryService repositoryService;
	  protected internal TaskService taskService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal const string A_USER_ID = "aUserId";

	  protected internal const string TENANT_1 = "tenant1";
	  protected internal const string TENANT_2 = "tenant2";
	  protected internal const string TENANT_3 = "tenant3";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		taskService = engineRule.TaskService;
		repositoryService = engineRule.RepositoryService;
		historyService = engineRule.HistoryService;
		runtimeService = engineRule.RuntimeService;

		// create sample identity link
		BpmnModelInstance oneTaskProcess = Bpmn.createExecutableProcess("testProcess").startEvent().userTask("task").camundaCandidateUsers(A_USER_ID).endEvent().done();

		// deploy tenants
		testRule.deployForTenant(TENANT_1, oneTaskProcess);
		testRule.deployForTenant(TENANT_2, oneTaskProcess);
		testRule.deployForTenant(TENANT_3, oneTaskProcess);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void addandDeleteHistoricIdentityLinkForSingleTenant()
	  public virtual void addandDeleteHistoricIdentityLinkForSingleTenant()
	  {

		startProcessInstanceForTenant(TENANT_1);

		HistoricIdentityLinkLog historicIdentityLink = historyService.createHistoricIdentityLinkLogQuery().singleResult();

		taskService.deleteCandidateUser(historicIdentityLink.TaskId, A_USER_ID);

		HistoricIdentityLinkLogQuery query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.tenantIdIn(TENANT_1).count(), 2);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void historicIdentityLinkForMultipleTenant()
	  public virtual void historicIdentityLinkForMultipleTenant()
	  {
		startProcessInstanceForTenant(TENANT_1);

		// Query test
		HistoricIdentityLinkLog historicIdentityLink = historyService.createHistoricIdentityLinkLogQuery().singleResult();

		assertEquals(historicIdentityLink.TenantId, TENANT_1);

		// start process instance for another tenant
		startProcessInstanceForTenant(TENANT_2);

		// Query test
		IList<HistoricIdentityLinkLog> historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();

		assertEquals(historicIdentityLinks.Count, 2);

		HistoricIdentityLinkLogQuery query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.tenantIdIn(TENANT_1).count(), 1);

		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.tenantIdIn(TENANT_2).count(), 1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void addAndRemoveHistoricIdentityLinksForProcessDefinitionWithTenantId() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void addAndRemoveHistoricIdentityLinksForProcessDefinitionWithTenantId()
	  {
		string resourceName = "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml";
		testRule.deployForTenant(TENANT_1, resourceName);
		testRule.deployForTenant(TENANT_2, resourceName);

		ProcessDefinition processDefinition1 = repositoryService.createProcessDefinitionQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).list().get(0);
		ProcessDefinition processDefinition2 = repositoryService.createProcessDefinitionQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).list().get(1);

		assertNotNull(processDefinition1);
		assertNotNull(processDefinition2);

		testTenantsByProcessDefinition(processDefinition1.Id);
		testTenantsByProcessDefinition(processDefinition2.Id);

		IList<HistoricIdentityLinkLog> historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();

		assertEquals(historicIdentityLinks.Count, 8);

		// Query test
		HistoricIdentityLinkLogQuery query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.tenantIdIn(TENANT_1).count(), 4);
		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.tenantIdIn(TENANT_2).count(), 4);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") public void testTenantsByProcessDefinition(String processDefinitionId)
	  public virtual void testTenantsByProcessDefinition(string processDefinitionId)
	  {

		repositoryService.addCandidateStarterGroup(processDefinitionId, GROUP_1);

		repositoryService.addCandidateStarterUser(processDefinitionId, USER_1);

		repositoryService.deleteCandidateStarterGroup(processDefinitionId, GROUP_1);

		repositoryService.deleteCandidateStarterUser(processDefinitionId, USER_1);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") @Test public void identityLinksForProcessDefinitionWithTenantId() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void identityLinksForProcessDefinitionWithTenantId()
	  {
		string resourceName = "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml";
		testRule.deployForTenant(TENANT_1, resourceName);
		testRule.deployForTenant(TENANT_2, resourceName);

		ProcessDefinition processDefinition1 = repositoryService.createProcessDefinitionQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).list().get(0);

		assertNotNull(processDefinition1);

		// Add candidate group with process definition 1
		repositoryService.addCandidateStarterGroup(processDefinition1.Id, GROUP_1);

		// Add candidate user for process definition 2
		repositoryService.addCandidateStarterUser(processDefinition1.Id, USER_1);

		ProcessDefinition processDefinition2 = repositoryService.createProcessDefinitionQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).list().get(1);

		assertNotNull(processDefinition2);

		// Add candidate group with process definition 2
		repositoryService.addCandidateStarterGroup(processDefinition2.Id, GROUP_1);

		// Add candidate user for process definition 2
		repositoryService.addCandidateStarterUser(processDefinition2.Id, USER_1);

		// Identity link test
		IList<IdentityLink> identityLinks = repositoryService.getIdentityLinksForProcessDefinition(processDefinition1.Id);
		assertEquals(identityLinks.Count,2);
		assertEquals(identityLinks[0].TenantId, TENANT_1);
		assertEquals(identityLinks[1].TenantId, TENANT_1);

		identityLinks = repositoryService.getIdentityLinksForProcessDefinition(processDefinition2.Id);
		assertEquals(identityLinks.Count,2);
		assertEquals(identityLinks[0].TenantId, TENANT_2);
		assertEquals(identityLinks[1].TenantId, TENANT_2);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void singleQueryForMultipleTenant()
	  public virtual void singleQueryForMultipleTenant()
	  {
		startProcessInstanceForTenant(TENANT_1);
		startProcessInstanceForTenant(TENANT_2);
		startProcessInstanceForTenant(TENANT_3);

		HistoricIdentityLinkLogQuery query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.tenantIdIn(TENANT_1, TENANT_2).count(), 2);

		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.tenantIdIn(TENANT_2, TENANT_3).count(), 2);

		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.tenantIdIn(TENANT_1, TENANT_2, TENANT_3).count(), 3);

	  }

	  protected internal virtual ProcessInstance startProcessInstanceForTenant(string tenant)
	  {
		return runtimeService.createProcessInstanceByKey("testProcess").processDefinitionTenantId(tenant).execute();
	  }
	}

}