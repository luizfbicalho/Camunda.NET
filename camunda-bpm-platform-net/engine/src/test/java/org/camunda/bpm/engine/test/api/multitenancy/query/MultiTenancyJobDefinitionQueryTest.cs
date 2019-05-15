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
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using JobDefinitionQuery = org.camunda.bpm.engine.management.JobDefinitionQuery;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	public class MultiTenancyJobDefinitionQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal override void setUp()
	  {
		BpmnModelInstance process = Bpmn.createExecutableProcess("testProcess").startEvent().timerWithDuration("PT1M").userTask().endEvent().done();

		deployment(process);
		deploymentForTenant(TENANT_ONE, process);
		deploymentForTenant(TENANT_TWO, process);

		// the deployed process definition contains a timer start event
		// - so a job definition is created on deployment.
	  }

	  public virtual void testQueryNoTenantIdSet()
	  {
		JobDefinitionQuery query = managementService.createJobDefinitionQuery();

		assertThat(query.count(), @is(3L));
	  }

	  public virtual void testQueryByTenantId()
	  {
		JobDefinitionQuery query = managementService.createJobDefinitionQuery().tenantIdIn(TENANT_ONE);

		assertThat(query.count(), @is(1L));

		query = managementService.createJobDefinitionQuery().tenantIdIn(TENANT_TWO);

		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryByTenantIds()
	  {
		JobDefinitionQuery query = managementService.createJobDefinitionQuery().tenantIdIn(TENANT_ONE, TENANT_TWO);

		assertThat(query.count(), @is(2L));
	  }

	  public virtual void testQueryByDefinitionsWithoutTenantIds()
	  {
		JobDefinitionQuery query = managementService.createJobDefinitionQuery().withoutTenantId();

		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryByTenantIdsIncludeDefinitionsWithoutTenantId()
	  {
		JobDefinitionQuery query = managementService.createJobDefinitionQuery().tenantIdIn(TENANT_ONE).includeJobDefinitionsWithoutTenantId();

		assertThat(query.count(), @is(2L));

		query = managementService.createJobDefinitionQuery().tenantIdIn(TENANT_TWO).includeJobDefinitionsWithoutTenantId();

		assertThat(query.count(), @is(2L));

		query = managementService.createJobDefinitionQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).includeJobDefinitionsWithoutTenantId();

		assertThat(query.count(), @is(3L));
	  }

	  public virtual void testQueryByNonExistingTenantId()
	  {
		JobDefinitionQuery query = managementService.createJobDefinitionQuery().tenantIdIn("nonExisting");

		assertThat(query.count(), @is(0L));
	  }

	  public virtual void testFailQueryByTenantIdNull()
	  {
		try
		{
		  managementService.createJobDefinitionQuery().tenantIdIn((string) null);

		  fail("expected exception");
		}
		catch (NullValueException)
		{
		}
	  }

	  public virtual void testQuerySortingAsc()
	  {
		// exclude job definitions without tenant id because of database-specific ordering
		IList<JobDefinition> jobDefinitions = managementService.createJobDefinitionQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).orderByTenantId().asc().list();

		assertThat(jobDefinitions.Count, @is(2));
		assertThat(jobDefinitions[0].TenantId, @is(TENANT_ONE));
		assertThat(jobDefinitions[1].TenantId, @is(TENANT_TWO));
	  }

	  public virtual void testQuerySortingDesc()
	  {
		// exclude job definitions without tenant id because of database-specific ordering
		IList<JobDefinition> jobDefinitions = managementService.createJobDefinitionQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).orderByTenantId().desc().list();

		assertThat(jobDefinitions.Count, @is(2));
		assertThat(jobDefinitions[0].TenantId, @is(TENANT_TWO));
		assertThat(jobDefinitions[1].TenantId, @is(TENANT_ONE));
	  }

	  public virtual void testQueryNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		JobDefinitionQuery query = managementService.createJobDefinitionQuery();
		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		JobDefinitionQuery query = managementService.createJobDefinitionQuery();

		assertThat(query.count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
		assertThat(query.tenantIdIn(TENANT_ONE, TENANT_TWO).includeJobDefinitionsWithoutTenantId().count(), @is(2L));
	  }

	  public virtual void testQueryAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE, TENANT_TWO));

		JobDefinitionQuery query = managementService.createJobDefinitionQuery();

		assertThat(query.count(), @is(3L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(1L));
		assertThat(query.withoutTenantId().count(), @is(1L));
	  }

	  public virtual void testQueryDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		JobDefinitionQuery query = managementService.createJobDefinitionQuery();
		assertThat(query.count(), @is(3L));
	  }

	}

}