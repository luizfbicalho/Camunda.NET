using System.Collections.Generic;
using System.IO;

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
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using DiagramLayout = org.camunda.bpm.engine.repository.DiagramLayout;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessDefinitionQuery = org.camunda.bpm.engine.repository.ProcessDefinitionQuery;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author kristin.polenz
	/// </summary>
	public class MultiTenancyProcessDefinitionCmdsTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyProcessDefinitionCmdsTenantCheckTest()
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

	  protected internal const string BPMN_PROCESS_MODEL = "org/camunda/bpm/engine/test/api/multitenancy/testProcess.bpmn";
	  protected internal const string BPMN_PROCESS_DIAGRAM = "org/camunda/bpm/engine/test/api/multitenancy/testProcess.png";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  protected internal RuntimeService runtimeService;
	  protected internal HistoryService historyService;
	  protected internal RepositoryService repositoryService;
	  protected internal IdentityService identityService;
	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;

	  protected internal string processDefinitionId;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		repositoryService = engineRule.RepositoryService;
		identityService = engineRule.IdentityService;
		runtimeService = engineRule.RuntimeService;
		historyService = engineRule.HistoryService;

		testRule.deployForTenant(TENANT_ONE, BPMN_PROCESS_MODEL, BPMN_PROCESS_DIAGRAM);

		processDefinitionId = repositoryService.createProcessDefinitionQuery().singleResult().Id;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToGetProcessModelNoAuthenticatedTenants()
	  public virtual void failToGetProcessModelNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		// declare expected exception
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot get the process definition");

		repositoryService.getProcessModel(processDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getProcessModelWithAuthenticatedTenant()
	  public virtual void getProcessModelWithAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		Stream inputStream = repositoryService.getProcessModel(processDefinitionId);

		assertThat(inputStream, notNullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getProcessModelDisabledTenantCheck()
	  public virtual void getProcessModelDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		Stream inputStream = repositoryService.getProcessModel(processDefinitionId);

		assertThat(inputStream, notNullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToGetProcessDiagramNoAuthenticatedTenants()
	  public virtual void failToGetProcessDiagramNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		// declare expected exception
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot get the process definition");

		repositoryService.getProcessDiagram(processDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getProcessDiagramWithAuthenticatedTenant()
	  public virtual void getProcessDiagramWithAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		Stream inputStream = repositoryService.getProcessDiagram(processDefinitionId);

		assertThat(inputStream, notNullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getProcessDiagramDisabledTenantCheck()
	  public virtual void getProcessDiagramDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		Stream inputStream = repositoryService.getProcessDiagram(processDefinitionId);

		assertThat(inputStream, notNullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToGetProcessDiagramLayoutNoAuthenticatedTenants()
	  public virtual void failToGetProcessDiagramLayoutNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		// declare expected exception
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot get the process definition");

		repositoryService.getProcessDiagramLayout(processDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getProcessDiagramLayoutWithAuthenticatedTenant()
	  public virtual void getProcessDiagramLayoutWithAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		DiagramLayout diagramLayout = repositoryService.getProcessDiagramLayout(processDefinitionId);

		assertThat(diagramLayout, notNullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getProcessDiagramLayoutDisabledTenantCheck()
	  public virtual void getProcessDiagramLayoutDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		DiagramLayout diagramLayout = repositoryService.getProcessDiagramLayout(processDefinitionId);

		assertThat(diagramLayout, notNullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToGetProcessDefinitionNoAuthenticatedTenants()
	  public virtual void failToGetProcessDefinitionNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		// declare expected exception
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot get the process definition");

		repositoryService.getProcessDefinition(processDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getProcessDefinitionWithAuthenticatedTenant()
	  public virtual void getProcessDefinitionWithAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		ProcessDefinition definition = repositoryService.getProcessDefinition(processDefinitionId);

		assertThat(definition.TenantId, @is(TENANT_ONE));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getProcessDefinitionDisabledTenantCheck()
	  public virtual void getProcessDefinitionDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		ProcessDefinition definition = repositoryService.getProcessDefinition(processDefinitionId);

		assertThat(definition.TenantId, @is(TENANT_ONE));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToGetBpmnModelInstanceNoAuthenticatedTenants()
	  public virtual void failToGetBpmnModelInstanceNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		// declare expected exception
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot get the process definition");

		repositoryService.getBpmnModelInstance(processDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getBpmnModelInstanceWithAuthenticatedTenant()
	  public virtual void getBpmnModelInstanceWithAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		BpmnModelInstance modelInstance = repositoryService.getBpmnModelInstance(processDefinitionId);

		assertThat(modelInstance, notNullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getBpmnModelInstanceDisabledTenantCheck()
	  public virtual void getBpmnModelInstanceDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		BpmnModelInstance modelInstance = repositoryService.getBpmnModelInstance(processDefinitionId);

		assertThat(modelInstance, notNullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToDeleteProcessDefinitionNoAuthenticatedTenant()
	  public virtual void failToDeleteProcessDefinitionNoAuthenticatedTenant()
	  {
		//given deployment with a process definition
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().list();
		//and user with no tenant authentication
		identityService.setAuthentication("user", null, null);

		// declare expected exception
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot delete the process definition");

		//deletion should end in exception, since tenant authorization is missing
		repositoryService.deleteProcessDefinition(processDefinitions[0].Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionWithAuthenticatedTenant()
	  public virtual void testDeleteProcessDefinitionWithAuthenticatedTenant()
	  {
		//given deployment with two process definitions
		Deployment deployment = testRule.deployForTenant(TENANT_ONE, "org/camunda/bpm/engine/test/repository/twoProcesses.bpmn20.xml");
		ProcessDefinitionQuery processDefinitionQuery = repositoryService.createProcessDefinitionQuery().deploymentId(deployment.Id);
		IList<ProcessDefinition> processDefinitions = processDefinitionQuery.list();
		//and user with tenant authentication
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		//when delete process definition with authenticated user
		repositoryService.deleteProcessDefinition(processDefinitions[0].Id);

		//then process definition should be deleted
		identityService.clearAuthentication();
		assertThat(processDefinitionQuery.count(), @is(1L));
		assertThat(processDefinitionQuery.tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteCascadeProcessDefinitionWithAuthenticatedTenant()
	  public virtual void testDeleteCascadeProcessDefinitionWithAuthenticatedTenant()
	  {
		//given deployment with a process definition and process instance
		BpmnModelInstance bpmnModel = Bpmn.createExecutableProcess("process").startEvent().userTask().endEvent().done();
		testRule.deployForTenant(TENANT_ONE, bpmnModel);
		ProcessDefinitionQuery processDefinitionQuery = repositoryService.createProcessDefinitionQuery();
		ProcessDefinition processDefinition = processDefinitionQuery.processDefinitionKey("process").singleResult();
		engineRule.RuntimeService.createProcessInstanceByKey("process").executeWithVariablesInReturn();

		//and user with tenant authentication
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		//when the corresponding process definition is cascading deleted from the deployment
		repositoryService.deleteProcessDefinition(processDefinition.Id, true);

		//then exist no process instance and one definition
		identityService.clearAuthentication();
		assertEquals(0, engineRule.RuntimeService.createProcessInstanceQuery().count());
		if (processEngineConfiguration.HistoryLevel.Id >= org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_ACTIVITY.Id)
		{
		  assertEquals(0, engineRule.HistoryService.createHistoricActivityInstanceQuery().count());
		}
		assertThat(repositoryService.createProcessDefinitionQuery().count(), @is(1L));
		assertThat(repositoryService.createProcessDefinitionQuery().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionDisabledTenantCheck()
	  public virtual void testDeleteProcessDefinitionDisabledTenantCheck()
	  {
		//given deployment with two process definitions
		Deployment deployment = testRule.deployForTenant(TENANT_ONE, "org/camunda/bpm/engine/test/repository/twoProcesses.bpmn20.xml");
		//tenant check disabled
		processEngineConfiguration.TenantCheckEnabled = false;
		ProcessDefinitionQuery processDefinitionQuery = repositoryService.createProcessDefinitionQuery().deploymentId(deployment.Id);
		IList<ProcessDefinition> processDefinitions = processDefinitionQuery.list();
		//user with no authentication
		identityService.setAuthentication("user", null, null);

		//when process definition should be deleted without tenant check
		repositoryService.deleteProcessDefinition(processDefinitions[0].Id);

		//then process definition is deleted
		identityService.clearAuthentication();
		assertThat(processDefinitionQuery.count(), @is(1L));
		assertThat(processDefinitionQuery.tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteCascadeProcessDefinitionDisabledTenantCheck()
	  public virtual void testDeleteCascadeProcessDefinitionDisabledTenantCheck()
	  {
		//given deployment with a process definition and process instances
		BpmnModelInstance bpmnModel = Bpmn.createExecutableProcess("process").startEvent().userTask().endEvent().done();
		testRule.deployForTenant(TENANT_ONE, bpmnModel);
		//tenant check disabled
		processEngineConfiguration.TenantCheckEnabled = false;
		ProcessDefinitionQuery processDefinitionQuery = repositoryService.createProcessDefinitionQuery();
		ProcessDefinition processDefinition = processDefinitionQuery.processDefinitionKey("process").singleResult();
		engineRule.RuntimeService.createProcessInstanceByKey("process").executeWithVariablesInReturn();
		//user with no authentication
		identityService.setAuthentication("user", null, null);

		//when the corresponding process definition is cascading deleted from the deployment
		repositoryService.deleteProcessDefinition(processDefinition.Id, true);

		//then exist no process instance and one definition, because test case deployes per default one definition
		identityService.clearAuthentication();
		assertEquals(0, engineRule.RuntimeService.createProcessInstanceQuery().count());
		if (processEngineConfiguration.HistoryLevel.Id >= org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_ACTIVITY.Id)
		{
		  assertEquals(0, engineRule.HistoryService.createHistoricActivityInstanceQuery().count());
		}
		assertThat(repositoryService.createProcessDefinitionQuery().count(), @is(1L));
		assertThat(repositoryService.createProcessDefinitionQuery().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToDeleteProcessDefinitionsByKeyNoAuthenticatedTenant()
	  public virtual void failToDeleteProcessDefinitionsByKeyNoAuthenticatedTenant()
	  {
		// given
		for (int i = 0; i < 3; i++)
		{
		  deployProcessDefinitionWithTenant();
		}

		identityService.setAuthentication("user", null, null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("No process definition found");

		// when
		repositoryService.deleteProcessDefinitions().byKey("process").withoutTenantId().delete();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionsByKeyForAllTenants()
	  public virtual void testDeleteProcessDefinitionsByKeyForAllTenants()
	  {
		// given
		for (int i = 0; i < 3; i++)
		{
		  deployProcessDefinitionWithTenant();
		  deployProcessDefinitionWithoutTenant();
		}

		// when
		repositoryService.deleteProcessDefinitions().byKey("process").delete();

		// then
		assertThat(repositoryService.createProcessDefinitionQuery().count(), @is(1L));
		assertThat(repositoryService.createProcessDefinitionQuery().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionsByKeyWithAuthenticatedTenant()
	  public virtual void testDeleteProcessDefinitionsByKeyWithAuthenticatedTenant()
	  {
		// given
		for (int i = 0; i < 3; i++)
		{
		  deployProcessDefinitionWithTenant();
		}

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		// when
		repositoryService.deleteProcessDefinitions().byKey("process").withTenantId(TENANT_ONE).delete();

		// then
		identityService.clearAuthentication();
		assertThat(repositoryService.createProcessDefinitionQuery().count(), @is(1L));
		assertThat(repositoryService.createProcessDefinitionQuery().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteCascadeProcessDefinitionsByKeyWithAuthenticatedTenant()
	  public virtual void testDeleteCascadeProcessDefinitionsByKeyWithAuthenticatedTenant()
	  {
		// given
		for (int i = 0; i < 3; i++)
		{
		  deployProcessDefinitionWithTenant();
		}

		runtimeService.startProcessInstanceByKey("process");

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		// when
		repositoryService.deleteProcessDefinitions().byKey("process").withTenantId(TENANT_ONE).cascade().delete();

		// then
		identityService.clearAuthentication();
		assertThat(historyService.createHistoricProcessInstanceQuery().count(), @is(0L));
		assertThat(repositoryService.createProcessDefinitionQuery().count(), @is(1L));
		assertThat(repositoryService.createProcessDefinitionQuery().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionsByKeyDisabledTenantCheck()
	  public virtual void testDeleteProcessDefinitionsByKeyDisabledTenantCheck()
	  {
		// given
		for (int i = 0; i < 3; i++)
		{
		  deployProcessDefinitionWithTenant();
		}

		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		// when
		repositoryService.deleteProcessDefinitions().byKey("process").withTenantId(TENANT_ONE).delete();

		// then
		identityService.clearAuthentication();
		assertThat(repositoryService.createProcessDefinitionQuery().count(), @is(1L));
		assertThat(repositoryService.createProcessDefinitionQuery().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteCascadeProcessDefinitionsByKeyDisabledTenantCheck()
	  public virtual void testDeleteCascadeProcessDefinitionsByKeyDisabledTenantCheck()
	  {
		// given
		for (int i = 0; i < 3; i++)
		{
		  deployProcessDefinitionWithTenant();
		}

		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);
		runtimeService.startProcessInstanceByKey("process");

		// when
		repositoryService.deleteProcessDefinitions().byKey("process").withTenantId(TENANT_ONE).cascade().delete();

		// then
		identityService.clearAuthentication();
		assertThat(historyService.createHistoricProcessInstanceQuery().count(), @is(0L));
		assertThat(repositoryService.createProcessDefinitionQuery().count(), @is(1L));
		assertThat(repositoryService.createProcessDefinitionQuery().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToDeleteProcessDefinitionsByIdsNoAuthenticatedTenant()
	  public virtual void failToDeleteProcessDefinitionsByIdsNoAuthenticatedTenant()
	  {
		// given
		for (int i = 0; i < 3; i++)
		{
		  deployProcessDefinitionWithTenant();
		}

		string[] processDefinitionIds = findProcessDefinitionIdsByKey("process");

		identityService.setAuthentication("user", null, null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot delete the process definition");

		// when
		repositoryService.deleteProcessDefinitions().byIds(processDefinitionIds).delete();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionsByIdsWithAuthenticatedTenant()
	  public virtual void testDeleteProcessDefinitionsByIdsWithAuthenticatedTenant()
	  {
		// given
		for (int i = 0; i < 3; i++)
		{
		  deployProcessDefinitionWithTenant();
		}

		string[] processDefinitionIds = findProcessDefinitionIdsByKey("process");

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		// when
		repositoryService.deleteProcessDefinitions().byIds(processDefinitionIds).delete();

		// then
		identityService.clearAuthentication();
		assertThat(repositoryService.createProcessDefinitionQuery().count(), @is(1L));
		assertThat(repositoryService.createProcessDefinitionQuery().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteCascadeProcessDefinitionsByIdsWithAuthenticatedTenant()
	  public virtual void testDeleteCascadeProcessDefinitionsByIdsWithAuthenticatedTenant()
	  {
		// given
		for (int i = 0; i < 3; i++)
		{
		  deployProcessDefinitionWithTenant();
		}

		string[] processDefinitionIds = findProcessDefinitionIdsByKey("process");

		runtimeService.startProcessInstanceByKey("process");

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		// when
		repositoryService.deleteProcessDefinitions().byIds(processDefinitionIds).cascade().delete();

		// then
		identityService.clearAuthentication();
		assertThat(historyService.createHistoricProcessInstanceQuery().count(), @is(0L));
		assertThat(repositoryService.createProcessDefinitionQuery().count(), @is(1L));
		assertThat(repositoryService.createProcessDefinitionQuery().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionsByIdsDisabledTenantCheck()
	  public virtual void testDeleteProcessDefinitionsByIdsDisabledTenantCheck()
	  {
		// given
		for (int i = 0; i < 3; i++)
		{
		  deployProcessDefinitionWithTenant();
		}

		string[] processDefinitionIds = findProcessDefinitionIdsByKey("process");

		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		// when
		repositoryService.deleteProcessDefinitions().byIds(processDefinitionIds).delete();

		// then
		identityService.clearAuthentication();
		assertThat(repositoryService.createProcessDefinitionQuery().count(), @is(1L));
		assertThat(repositoryService.createProcessDefinitionQuery().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteCascadeProcessDefinitionsByIdsDisabledTenantCheck()
	  public virtual void testDeleteCascadeProcessDefinitionsByIdsDisabledTenantCheck()
	  {
		// given
		for (int i = 0; i < 3; i++)
		{
		  deployProcessDefinitionWithTenant();
		}

		string[] processDefinitionIds = findProcessDefinitionIdsByKey("process");

		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);
		runtimeService.startProcessInstanceByKey("process");

		// when
		repositoryService.deleteProcessDefinitions().byIds(processDefinitionIds).cascade().delete();

		// then
		identityService.clearAuthentication();
		assertThat(historyService.createHistoricProcessInstanceQuery().count(), @is(0L));
		assertThat(repositoryService.createProcessDefinitionQuery().count(), @is(1L));
		assertThat(repositoryService.createProcessDefinitionQuery().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void updateHistoryTimeToLiveWithAuthenticatedTenant()
	  public virtual void updateHistoryTimeToLiveWithAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		repositoryService.updateProcessDefinitionHistoryTimeToLive(processDefinitionId, 6);

		ProcessDefinition definition = repositoryService.getProcessDefinition(processDefinitionId);

		assertThat(definition.TenantId, @is(TENANT_ONE));
		assertThat(definition.HistoryTimeToLive, @is(6));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void updateHistoryTimeToLiveDisabledTenantCheck()
	  public virtual void updateHistoryTimeToLiveDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		repositoryService.updateProcessDefinitionHistoryTimeToLive(processDefinitionId, 6);

		ProcessDefinition definition = repositoryService.getProcessDefinition(processDefinitionId);

		assertThat(definition.TenantId, @is(TENANT_ONE));
		assertThat(definition.HistoryTimeToLive, @is(6));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void updateHistoryTimeToLiveNoAuthenticatedTenants()
	  public virtual void updateHistoryTimeToLiveNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		// declare expected exception
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the process definition");

		repositoryService.updateProcessDefinitionHistoryTimeToLive(processDefinitionId, 6);
	  }

	  private string[] findProcessDefinitionIdsByKey(string processDefinitionKey)
	  {
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().processDefinitionKey(processDefinitionKey).list();
		IList<string> processDefinitionIds = new List<string>();
		foreach (ProcessDefinition processDefinition in processDefinitions)
		{
		  processDefinitionIds.Add(processDefinition.Id);
		}

		return processDefinitionIds.ToArray();
	  }

	  private void deployProcessDefinitionWithTenant()
	  {
		testRule.deployForTenant(TENANT_ONE, Bpmn.createExecutableProcess("process").startEvent().userTask().endEvent().done());
	  }

	  private void deployProcessDefinitionWithoutTenant()
	  {
		testRule.deploy(Bpmn.createExecutableProcess("process").startEvent().userTask().endEvent().done());
	  }

	}

}