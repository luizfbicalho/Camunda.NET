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
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessDefinitionQuery = org.camunda.bpm.engine.repository.ProcessDefinitionQuery;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	public class MultiTenancyProcessDefinitionQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal const string PROCESS_DEFINITION_KEY = "process";
	  protected internal static readonly BpmnModelInstance emptyProcess = Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).done();

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal override void setUp()
	  {
		deployment(emptyProcess);
		deploymentForTenant(TENANT_ONE, emptyProcess);
		deploymentForTenant(TENANT_TWO, emptyProcess);
	  }

	  public virtual void testQueryNoTenantIdSet()
	  {
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		assertThat(query.count(), @is(3L));
	  }

	  public virtual void testQueryByTenantId()
	  {
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().tenantIdIn(TENANT_ONE);

		assertThat(query.count(), @is(1L));

		query = repositoryService.createProcessDefinitionQuery().tenantIdIn(TENANT_TWO);

		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryByTenantIds()
	  {
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().tenantIdIn(TENANT_ONE, TENANT_TWO);

		assertThat(query.count(), @is(2L));
	  }

	  public virtual void testQueryByDefinitionsWithoutTenantId()
	  {
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().withoutTenantId();

		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryByTenantIdsIncludeDefinitionsWithoutTenantId()
	  {
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().tenantIdIn(TENANT_ONE).includeProcessDefinitionsWithoutTenantId();

		assertThat(query.count(), @is(2L));

		query = repositoryService.createProcessDefinitionQuery().tenantIdIn(TENANT_TWO).includeProcessDefinitionsWithoutTenantId();

		assertThat(query.count(), @is(2L));

		query = repositoryService.createProcessDefinitionQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).includeProcessDefinitionsWithoutTenantId();

		assertThat(query.count(), @is(3L));
	  }

	  public virtual void testQueryByKey()
	  {
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().processDefinitionKey(PROCESS_DEFINITION_KEY);
		// one definition for each tenant
		assertThat(query.count(), @is(3L));

		query = repositoryService.createProcessDefinitionQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).withoutTenantId();
		// one definition without tenant id
		assertThat(query.count(), @is(1L));

		query = repositoryService.createProcessDefinitionQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).tenantIdIn(TENANT_ONE);
		// one definition for tenant one
		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryByLatestNoTenantIdSet()
	  {
		// deploy a second version for tenant one
		deploymentForTenant(TENANT_ONE, emptyProcess);

		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).latestVersion();
		// one definition for each tenant
		assertThat(query.count(), @is(3L));

		IDictionary<string, ProcessDefinition> processDefinitionsForTenant = getProcessDefinitionsForTenant(query.list());
		assertThat(processDefinitionsForTenant[TENANT_ONE].Version, @is(2));
		assertThat(processDefinitionsForTenant[TENANT_TWO].Version, @is(1));
		assertThat(processDefinitionsForTenant[null].Version, @is(1));
	  }

	  public virtual void testQueryByLatestWithTenantId()
	  {
		// deploy a second version for tenant one
		deploymentForTenant(TENANT_ONE, emptyProcess);

		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).latestVersion().tenantIdIn(TENANT_ONE);

		assertThat(query.count(), @is(1L));

		ProcessDefinition processDefinition = query.singleResult();
		assertThat(processDefinition.TenantId, @is(TENANT_ONE));
		assertThat(processDefinition.Version, @is(2));

		query = repositoryService.createProcessDefinitionQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).latestVersion().tenantIdIn(TENANT_TWO);

		assertThat(query.count(), @is(1L));

		processDefinition = query.singleResult();
		assertThat(processDefinition.TenantId, @is(TENANT_TWO));
		assertThat(processDefinition.Version, @is(1));
	  }

	  public virtual void testQueryByLatestWithTenantIds()
	  {
		// deploy a second version for tenant one
		deploymentForTenant(TENANT_ONE, emptyProcess);

		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).latestVersion().tenantIdIn(TENANT_ONE, TENANT_TWO);
		// one definition for each tenant
		assertThat(query.count(), @is(2L));

		IDictionary<string, ProcessDefinition> processDefinitionsForTenant = getProcessDefinitionsForTenant(query.list());
		assertThat(processDefinitionsForTenant[TENANT_ONE].Version, @is(2));
		assertThat(processDefinitionsForTenant[TENANT_TWO].Version, @is(1));
	  }

	  public virtual void testQueryByLatestWithoutTenantId()
	  {
		// deploy a second version without tenant id
		deployment(emptyProcess);

		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).latestVersion().withoutTenantId();

		assertThat(query.count(), @is(1L));

		ProcessDefinition processDefinition = query.singleResult();
		assertThat(processDefinition.TenantId, @is(nullValue()));
		assertThat(processDefinition.Version, @is(2));
	  }

	  public virtual void testQueryByLatestWithTenantIdsIncludeDefinitionsWithoutTenantId()
	  {
		// deploy a second version without tenant id
		deployment(emptyProcess);
		// deploy a third version for tenant one
		deploymentForTenant(TENANT_ONE, emptyProcess);
		deploymentForTenant(TENANT_ONE, emptyProcess);

		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).latestVersion().tenantIdIn(TENANT_ONE, TENANT_TWO).includeProcessDefinitionsWithoutTenantId();

		assertThat(query.count(), @is(3L));

		IDictionary<string, ProcessDefinition> processDefinitionsForTenant = getProcessDefinitionsForTenant(query.list());
		assertThat(processDefinitionsForTenant[TENANT_ONE].Version, @is(3));
		assertThat(processDefinitionsForTenant[TENANT_TWO].Version, @is(1));
		assertThat(processDefinitionsForTenant[null].Version, @is(2));
	  }

	  public virtual void testQueryByNonExistingTenantId()
	  {
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().tenantIdIn("nonExisting");

		assertThat(query.count(), @is(0L));
	  }

	  public virtual void testFailQueryByTenantIdNull()
	  {
		try
		{
		  repositoryService.createProcessDefinitionQuery().tenantIdIn((string) null);

		  fail("expected exception");
		}
		catch (NullValueException)
		{
		}
	  }

	  public virtual void testQuerySortingAsc()
	  {
		// exclude definitions without tenant id because of database-specific ordering
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).orderByTenantId().asc().list();

		assertThat(processDefinitions.Count, @is(2));
		assertThat(processDefinitions[0].TenantId, @is(TENANT_ONE));
		assertThat(processDefinitions[1].TenantId, @is(TENANT_TWO));
	  }

	  public virtual void testQuerySortingDesc()
	  {
		// exclude definitions without tenant id because of database-specific ordering
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).orderByTenantId().desc().list();

		assertThat(processDefinitions.Count, @is(2));
		assertThat(processDefinitions[0].TenantId, @is(TENANT_TWO));
		assertThat(processDefinitions[1].TenantId, @is(TENANT_ONE));
	  }

	  protected internal virtual IDictionary<string, ProcessDefinition> getProcessDefinitionsForTenant(IList<ProcessDefinition> processDefinitions)
	  {
		IDictionary<string, ProcessDefinition> definitionsForTenant = new Dictionary<string, ProcessDefinition>();

		foreach (ProcessDefinition definition in processDefinitions)
		{
		  definitionsForTenant[definition.TenantId] = definition;
		}
		return definitionsForTenant;
	  }

	  public virtual void testQueryNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();
		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		assertThat(query.count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
		assertThat(query.tenantIdIn(TENANT_ONE, TENANT_TWO).includeProcessDefinitionsWithoutTenantId().count(), @is(2L));
	  }

	  public virtual void testQueryAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE, TENANT_TWO));

		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		assertThat(query.count(), @is(3L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(1L));
		assertThat(query.withoutTenantId().count(), @is(1L));
	  }

	  public virtual void testQueryDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();
		assertThat(query.count(), @is(3L));
	  }

	}

}