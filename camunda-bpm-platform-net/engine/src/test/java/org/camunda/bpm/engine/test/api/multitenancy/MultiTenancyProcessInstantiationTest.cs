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
//	import static org.hamcrest.CoreMatchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using BatchRestartHelper = org.camunda.bpm.engine.test.api.runtime.BatchRestartHelper;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using After = org.junit.After;


	public class MultiTenancyProcessInstantiationTest : PluggableProcessEngineTestCase
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyProcessInstantiationTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			batchHelper = new BatchRestartHelper(this);
		}


	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal static readonly BpmnModelInstance PROCESS = Bpmn.createExecutableProcess("testProcess").startEvent().userTask("userTask").endEvent().done();

	  public BatchRestartHelper batchHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void tearDown()
	  {
		base.tearDown();
		authorizationService.createAuthorizationQuery();
		batchHelper.removeAllRunningAndHistoricBatches();
	  }

	  public virtual void testStartProcessInstanceByKeyAndTenantId()
	  {
		deploymentForTenant(TENANT_ONE, PROCESS);
		deploymentForTenant(TENANT_TWO, PROCESS);

		runtimeService.createProcessInstanceByKey("testProcess").processDefinitionTenantId(TENANT_ONE).execute();

		assertThat(runtimeService.createProcessInstanceQuery().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

	  public virtual void testStartProcessInstanceByKeyForAnyTenant()
	  {
		deploymentForTenant(TENANT_ONE, PROCESS);

		runtimeService.createProcessInstanceByKey("testProcess").execute();

		assertThat(runtimeService.createProcessInstanceQuery().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

	  public virtual void testStartProcessInstanceByKeyWithoutTenantId()
	  {
		deployment(PROCESS);
		deploymentForTenant(TENANT_ONE, PROCESS);

		runtimeService.createProcessInstanceByKey("testProcess").processDefinitionWithoutTenantId().execute();

		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();
		assertThat(query.count(), @is(1L));
		assertThat(query.singleResult().TenantId, @is(nullValue()));
	  }

	  public virtual void testFailToStartProcessInstanceByKeyForOtherTenant()
	  {
		deploymentForTenant(TENANT_ONE, PROCESS);

		try
		{
		  runtimeService.createProcessInstanceByKey("testProcess").processDefinitionTenantId(TENANT_TWO).execute();

		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("no processes deployed"));
		}
	  }

	  public virtual void testFailToStartProcessInstanceByKeyForMultipleTenants()
	  {
		deploymentForTenant(TENANT_ONE, PROCESS);
		deploymentForTenant(TENANT_TWO, PROCESS);

		try
		{
		  runtimeService.createProcessInstanceByKey("testProcess").execute();

		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("multiple tenants"));
		}
	  }

	  public virtual void testFailToStartProcessInstanceByIdAndTenantId()
	  {
		deploymentForTenant(TENANT_ONE, PROCESS);

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		try
		{
		  runtimeService.createProcessInstanceById(processDefinition.Id).processDefinitionTenantId(TENANT_ONE).execute();

		  fail("expected exception");
		}
		catch (BadUserRequestException e)
		{
		  assertThat(e.Message, containsString("Cannot specify a tenant-id"));
		}
	  }

	  public virtual void testFailToStartProcessInstanceByIdWithoutTenantId()
	  {
		deploymentForTenant(TENANT_ONE, PROCESS);

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		try
		{
		  runtimeService.createProcessInstanceById(processDefinition.Id).processDefinitionWithoutTenantId().execute();

		  fail("expected exception");
		}
		catch (BadUserRequestException e)
		{
		  assertThat(e.Message, containsString("Cannot specify a tenant-id"));
		}
	  }

	  public virtual void testStartProcessInstanceAtActivityByKeyAndTenantId()
	  {
		deploymentForTenant(TENANT_ONE, PROCESS);
		deploymentForTenant(TENANT_TWO, PROCESS);

		runtimeService.createProcessInstanceByKey("testProcess").processDefinitionTenantId(TENANT_ONE).startBeforeActivity("userTask").execute();

		assertThat(runtimeService.createProcessInstanceQuery().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

	  public virtual void testStartProcessInstanceAtActivityByKeyForAnyTenant()
	  {
		deploymentForTenant(TENANT_ONE, PROCESS);

		runtimeService.createProcessInstanceByKey("testProcess").startBeforeActivity("userTask").execute();

		assertThat(runtimeService.createProcessInstanceQuery().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

	  public virtual void testStartProcessInstanceAtActivityByKeyWithoutTenantId()
	  {
		deployment(PROCESS);
		deploymentForTenant(TENANT_ONE, PROCESS);

		runtimeService.createProcessInstanceByKey("testProcess").processDefinitionWithoutTenantId().startBeforeActivity("userTask").execute();

		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();
		assertThat(query.count(), @is(1L));
		assertThat(query.singleResult().TenantId, @is(nullValue()));
	  }

	  public virtual void testFailToStartProcessInstanceAtActivityByKeyForOtherTenant()
	  {
		deploymentForTenant(TENANT_ONE, PROCESS);

		try
		{
		  runtimeService.createProcessInstanceByKey("testProcess").processDefinitionTenantId(TENANT_TWO).startBeforeActivity("userTask").execute();

		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("no processes deployed"));
		}
	  }

	  public virtual void testFailToStartProcessInstanceAtActivityByKeyForMultipleTenants()
	  {
		deploymentForTenant(TENANT_ONE, PROCESS);
		deploymentForTenant(TENANT_TWO, PROCESS);

		try
		{
		  runtimeService.createProcessInstanceByKey("testProcess").startBeforeActivity("userTask").execute();

		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("multiple tenants"));
		}
	  }

	  public virtual void testFailToStartProcessInstanceAtActivityByIdAndTenantId()
	  {
		deploymentForTenant(TENANT_ONE, PROCESS);

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		try
		{
		  runtimeService.createProcessInstanceById(processDefinition.Id).processDefinitionTenantId(TENANT_ONE).startBeforeActivity("userTask").execute();

		  fail("expected exception");
		}
		catch (BadUserRequestException e)
		{
		  assertThat(e.Message, containsString("Cannot specify a tenant-id"));
		}
	  }

	  public virtual void testFailToStartProcessInstanceAtActivityByIdWithoutTenantId()
	  {
		deployment(PROCESS);

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		try
		{
		  runtimeService.createProcessInstanceById(processDefinition.Id).processDefinitionWithoutTenantId().startBeforeActivity("userTask").execute();

		  fail("expected exception");
		}
		catch (BadUserRequestException e)
		{
		  assertThat(e.Message, containsString("Cannot specify a tenant-id"));
		}
	  }

	  public virtual void testStartProcessInstanceByKeyWithoutTenantIdNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		deployment(PROCESS);

		runtimeService.createProcessInstanceByKey("testProcess").processDefinitionWithoutTenantId().execute();

		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();
		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testFailToStartProcessInstanceByKeyNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		deploymentForTenant(TENANT_ONE, PROCESS);

		try
		{
		  runtimeService.createProcessInstanceByKey("testProcess").execute();

		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("no processes deployed with key 'testProcess'"));
		}
	  }

	  public virtual void testFailToStartProcessInstanceByKeyWithTenantIdNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		deploymentForTenant(TENANT_ONE, PROCESS);

		try
		{
		  runtimeService.createProcessInstanceByKey("testProcess").processDefinitionTenantId(TENANT_ONE).execute();

		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("Cannot create an instance of the process definition"));
		}
	  }

	  public virtual void testFailToStartProcessInstanceByIdNoAuthenticatedTenants()
	  {
		deploymentForTenant(TENANT_ONE, PROCESS);

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		identityService.setAuthentication("user", null, null);

		try
		{
		  runtimeService.createProcessInstanceById(processDefinition.Id).execute();

		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("Cannot create an instance of the process definition"));
		}
	  }

	  public virtual void testStartProcessInstanceByKeyWithTenantIdAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		deploymentForTenant(TENANT_ONE, PROCESS);
		deploymentForTenant(TENANT_TWO, PROCESS);

		runtimeService.createProcessInstanceByKey("testProcess").processDefinitionTenantId(TENANT_ONE).execute();

		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();
		assertThat(query.count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

	  public virtual void testStartProcessInstanceByIdAuthenticatedTenant()
	  {
		deploymentForTenant(TENANT_ONE, PROCESS);

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		runtimeService.createProcessInstanceById(processDefinition.Id).execute();

		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();
		assertThat(query.count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

	  public virtual void testStartProcessInstanceByKeyWithAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		deploymentForTenant(TENANT_ONE, PROCESS);
		deploymentForTenant(TENANT_TWO, PROCESS);

		runtimeService.createProcessInstanceByKey("testProcess").execute();

		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();
		assertThat(query.count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

	  public virtual void testStartProcessInstanceByKeyWithTenantIdDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		deploymentForTenant(TENANT_ONE, PROCESS);

		runtimeService.createProcessInstanceByKey("testProcess").processDefinitionTenantId(TENANT_ONE).execute();

		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();
		assertThat(query.count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testRestartProcessInstanceSyncWithTenantId()
	  {
		// given
		ProcessInstance processInstance = startAndDeleteProcessInstance(TENANT_ONE, PROCESS);

		identityService.setAuthentication("user", null, Collections.singletonList(TENANT_ONE));

		// when
		runtimeService.restartProcessInstances(processInstance.ProcessDefinitionId).startBeforeActivity("userTask").processInstanceIds(processInstance.Id).execute();

		// then
		ProcessInstance restartedInstance = runtimeService.createProcessInstanceQuery().active().processDefinitionId(processInstance.ProcessDefinitionId).singleResult();

		assertNotNull(restartedInstance);
		assertEquals(restartedInstance.TenantId, TENANT_ONE);
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testRestartProcessInstanceAsyncWithTenantId()
	  {
		// given
		ProcessInstance processInstance = startAndDeleteProcessInstance(TENANT_ONE, PROCESS);

		identityService.setAuthentication("user", null, Collections.singletonList(TENANT_ONE));

		// when
		Batch batch = runtimeService.restartProcessInstances(processInstance.ProcessDefinitionId).startBeforeActivity("userTask").processInstanceIds(processInstance.Id).executeAsync();

		batchHelper.completeBatch(batch);

		// then
		ProcessInstance restartedInstance = runtimeService.createProcessInstanceQuery().active().processDefinitionId(processInstance.ProcessDefinitionId).singleResult();

		assertNotNull(restartedInstance);
		assertEquals(restartedInstance.TenantId, TENANT_ONE);
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testFailToRestartProcessInstanceSyncWithOtherTenantId()
	  {
		// given
		ProcessInstance processInstance = startAndDeleteProcessInstance(TENANT_ONE, PROCESS);

		identityService.setAuthentication("user", null, Collections.singletonList(TENANT_TWO));

		try
		{
		  // when
		  runtimeService.restartProcessInstances(processInstance.ProcessDefinitionId).startBeforeActivity("userTask").processInstanceIds(processInstance.Id).execute();

		  fail("expected exception");
		}
		catch (BadUserRequestException e)
		{
		  // then
		  assertThat(e.Message, containsString("Historic process instance cannot be found: historicProcessInstanceId is null"));
		}
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testFailToRestartProcessInstanceAsyncWithOtherTenantId()
	  {
		// given
		ProcessInstance processInstance = startAndDeleteProcessInstance(TENANT_ONE, PROCESS);

		identityService.setAuthentication("user", null, Collections.singletonList(TENANT_TWO));

		try
		{
		  // when
		  runtimeService.restartProcessInstances(processInstance.ProcessDefinitionId).startBeforeActivity("userTask").processInstanceIds(processInstance.Id).executeAsync();
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("Cannot restart process instances of process definition '" + processInstance.ProcessDefinitionId + "' because it belongs to no authenticated tenant."));
		}

	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testRestartProcessInstanceSyncWithTenantIdByHistoricProcessInstanceQuery()
	  {
		// given
		ProcessInstance processInstance = startAndDeleteProcessInstance(TENANT_ONE, PROCESS);
		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().processDefinitionId(processInstance.ProcessDefinitionId);

		identityService.setAuthentication("user", null, Collections.singletonList(TENANT_ONE));

		// when
		runtimeService.restartProcessInstances(processInstance.ProcessDefinitionId).startBeforeActivity("userTask").historicProcessInstanceQuery(query).execute();

		// then
		ProcessInstance restartedInstance = runtimeService.createProcessInstanceQuery().active().processDefinitionId(processInstance.ProcessDefinitionId).singleResult();

		assertNotNull(restartedInstance);
		assertEquals(restartedInstance.TenantId, TENANT_ONE);
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testRestartProcessInstanceAsyncWithTenantIdByHistoricProcessInstanceQuery()
	  {
		// given
		ProcessInstance processInstance = startAndDeleteProcessInstance(TENANT_ONE, PROCESS);
		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().processDefinitionId(processInstance.ProcessDefinitionId);

		identityService.setAuthentication("user", null, Collections.singletonList(TENANT_ONE));

		// when
		Batch batch = runtimeService.restartProcessInstances(processInstance.ProcessDefinitionId).startBeforeActivity("userTask").historicProcessInstanceQuery(query).executeAsync();

		batchHelper.completeBatch(batch);

		// then
		ProcessInstance restartedInstance = runtimeService.createProcessInstanceQuery().active().processDefinitionId(processInstance.ProcessDefinitionId).singleResult();

		assertNotNull(restartedInstance);
		assertEquals(restartedInstance.TenantId, TENANT_ONE);
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testFailToRestartProcessInstanceSyncWithOtherTenantIdByHistoricProcessInstanceQuery()
	  {
		// given
		ProcessInstance processInstance = startAndDeleteProcessInstance(TENANT_ONE, PROCESS);
		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().processDefinitionId(processInstance.ProcessDefinitionId);

		identityService.setAuthentication("user", null, Collections.singletonList(TENANT_TWO));

		try
		{
		  // when
		  runtimeService.restartProcessInstances(processInstance.ProcessDefinitionId).startBeforeActivity("userTask").historicProcessInstanceQuery(query).execute();

		  fail("expected exception");
		}
		catch (BadUserRequestException e)
		{
		  // then
		  assertThat(e.Message, containsString("processInstanceIds is empty"));
		}
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testFailToRestartProcessInstanceAsyncWithOtherTenantIdByHistoricProcessInstanceQuery()
	  {
		// given
		ProcessInstance processInstance = startAndDeleteProcessInstance(TENANT_ONE, PROCESS);
		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().processDefinitionId(processInstance.ProcessDefinitionId);

		identityService.setAuthentication("user", null, Collections.singletonList(TENANT_TWO));

		try
		{
		  // when
		  runtimeService.restartProcessInstances(processInstance.ProcessDefinitionId).startBeforeActivity("userTask").historicProcessInstanceQuery(query).executeAsync();
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("processInstanceIds is empty"));
		}

	  }


	  public virtual ProcessInstance startAndDeleteProcessInstance(string tenantId, BpmnModelInstance modelInstance)
	  {
		string deploymentId = deploymentForTenant(TENANT_ONE, PROCESS);
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().deploymentId(deploymentId).singleResult();
		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);
		runtimeService.deleteProcessInstance(processInstance.Id, "test");

		return processInstance;
	  }

	}

}