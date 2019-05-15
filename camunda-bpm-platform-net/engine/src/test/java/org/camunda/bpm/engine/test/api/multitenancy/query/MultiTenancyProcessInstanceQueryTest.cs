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
namespace org.camunda.bpm.engine.test.api.multitenancy.query
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	public class MultiTenancyProcessInstanceQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal override void setUp()
	  {
		BpmnModelInstance oneTaskProcess = Bpmn.createExecutableProcess("testProcess").startEvent().userTask().endEvent().done();

		deployment(oneTaskProcess);
		deploymentForTenant(TENANT_ONE, oneTaskProcess);
		deploymentForTenant(TENANT_TWO, oneTaskProcess);

		runtimeService.createProcessInstanceByKey("testProcess").processDefinitionWithoutTenantId().execute();
		runtimeService.createProcessInstanceByKey("testProcess").processDefinitionTenantId(TENANT_ONE).execute();
		runtimeService.createProcessInstanceByKey("testProcess").processDefinitionTenantId(TENANT_TWO).execute();
	  }

	  public virtual void testQueryNoTenantIdSet()
	  {
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();

		assertThat(query.count(), @is(3L));
	  }

	  public virtual void testQueryByTenantId()
	  {
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().tenantIdIn(TENANT_ONE);

		assertThat(query.count(), @is(1L));

		query = runtimeService.createProcessInstanceQuery().tenantIdIn(TENANT_TWO);

		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryByTenantIds()
	  {
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().tenantIdIn(TENANT_ONE, TENANT_TWO);

		assertThat(query.count(), @is(2L));
	  }

	  public virtual void testQueryByInstancesWithoutTenantId()
	  {
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().withoutTenantId();

		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryByNonExistingTenantId()
	  {
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().tenantIdIn("nonExisting");

		assertThat(query.count(), @is(0L));
	  }

	  public virtual void testFailQueryByTenantIdNull()
	  {
		try
		{
		  runtimeService.createProcessInstanceQuery().tenantIdIn((string) null);

		  fail("expected exception");
		}
		catch (NullValueException)
		{
		}
	  }

	  public virtual void testQuerySortingAsc()
	  {
		// exclude instances without tenant id because of database-specific ordering
		IList<ProcessInstance> processInstances = runtimeService.createProcessInstanceQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).orderByTenantId().asc().list();

		assertThat(processInstances.Count, @is(2));
		assertThat(processInstances[0].TenantId, @is(TENANT_ONE));
		assertThat(processInstances[1].TenantId, @is(TENANT_TWO));
	  }

	  public virtual void testQuerySortingDesc()
	  {
		// exclude instances without tenant id because of database-specific ordering
		IList<ProcessInstance> processInstances = runtimeService.createProcessInstanceQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).orderByTenantId().desc().list();

		assertThat(processInstances.Count, @is(2));
		assertThat(processInstances[0].TenantId, @is(TENANT_TWO));
		assertThat(processInstances[1].TenantId, @is(TENANT_ONE));
	  }

	  public virtual void testQueryNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();
		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();

		assertThat(query.count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
		assertThat(query.tenantIdIn(TENANT_ONE, TENANT_TWO).count(), @is(1L));
	  }

	  public virtual void testQueryAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE, TENANT_TWO));

		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();

		assertThat(query.count(), @is(3L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(1L));
		assertThat(query.withoutTenantId().count(), @is(1L));
	  }

	  public virtual void testQueryDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();
		assertThat(query.count(), @is(3L));
	  }

	  public virtual void testQueryByProcessDefinitionWithoutTenantId()
	  {
		// when
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().processDefinitionWithoutTenantId();

		// then
		assertThat(query.count(), @is(1L));
		assertThat(query.withoutTenantId().count(), @is(1L));
	  }

	  public virtual void testQueryByProcessDefinitionWithoutTenantId_VaryingProcessInstanceTenantId()
	  {
		// given
		StaticTenantIdTestProvider tenantIdProvider = new StaticTenantIdTestProvider(null);
		processEngineConfiguration.TenantIdProvider = tenantIdProvider;

		tenantIdProvider.TenantIdProvider = "anotherTenantId";

		runtimeService.createProcessInstanceByKey("testProcess").processDefinitionWithoutTenantId().execute();

		// when
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().processDefinitionWithoutTenantId();

		// then
		assertThat(query.count(), @is(2L));
		assertThat(query.withoutTenantId().count(), @is(1L));
		assertThat(query.tenantIdIn("anotherTenantId").count(), @is(1L));

		// cleanup
		processEngineConfiguration.TenantIdProvider = null;
	  }

	}

}