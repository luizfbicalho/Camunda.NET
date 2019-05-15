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
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using DeploymentQuery = org.camunda.bpm.engine.repository.DeploymentQuery;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	public class MultiTenancyDeploymentQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setUp() throws Exception
	  public override void setUp()
	  {
		BpmnModelInstance emptyProcess = Bpmn.createExecutableProcess().done();

		deployment(emptyProcess);
		deploymentForTenant(TENANT_ONE, emptyProcess);
		deploymentForTenant(TENANT_TWO, emptyProcess);
	  }

	  public virtual void testQueryNoTenantIdSet()
	  {
		DeploymentQuery query = repositoryService.createDeploymentQuery();

	   assertThat(query.count(), @is(3L));
	  }

	  public virtual void testQueryByTenantId()
	  {
		DeploymentQuery query = repositoryService.createDeploymentQuery().tenantIdIn(TENANT_ONE);

		assertThat(query.count(), @is(1L));

		query = repositoryService.createDeploymentQuery().tenantIdIn(TENANT_TWO);

		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryByTenantIds()
	  {
		DeploymentQuery query = repositoryService.createDeploymentQuery().tenantIdIn(TENANT_ONE, TENANT_TWO);

		assertThat(query.count(), @is(2L));
	  }

	  public virtual void testQueryWithoutTenantId()
	  {
		DeploymentQuery query = repositoryService.createDeploymentQuery().withoutTenantId();

		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryByTenantIdsIncludeDeploymentsWithoutTenantId()
	  {
		DeploymentQuery query = repositoryService.createDeploymentQuery().tenantIdIn(TENANT_ONE).includeDeploymentsWithoutTenantId();

		assertThat(query.count(), @is(2L));

		query = repositoryService.createDeploymentQuery().tenantIdIn(TENANT_TWO).includeDeploymentsWithoutTenantId();

		assertThat(query.count(), @is(2L));

		query = repositoryService.createDeploymentQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).includeDeploymentsWithoutTenantId();

		assertThat(query.count(), @is(3L));
	  }

	  public virtual void testQueryByNonExistingTenantId()
	  {
		DeploymentQuery query = repositoryService.createDeploymentQuery().tenantIdIn("nonExisting");

		assertThat(query.count(), @is(0L));
	  }

	  public virtual void testFailQueryByTenantIdNull()
	  {
		try
		{
		  repositoryService.createDeploymentQuery().tenantIdIn((string) null);

		  fail("expected exception");
		}
		catch (NullValueException)
		{
		}
	  }

	  public virtual void testQuerySortingAsc()
	  {
		// exclude deployments without tenant id because of database-specific ordering
		IList<Deployment> deployments = repositoryService.createDeploymentQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).orderByTenantId().asc().list();

		assertThat(deployments.Count, @is(2));
		assertThat(deployments[0].TenantId, @is(TENANT_ONE));
		assertThat(deployments[1].TenantId, @is(TENANT_TWO));
	  }

	  public virtual void testQuerySortingDesc()
	  {
		// exclude deployments without tenant id because of database-specific ordering
		IList<Deployment> deployments = repositoryService.createDeploymentQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).orderByTenantId().desc().list();

		assertThat(deployments.Count, @is(2));
		assertThat(deployments[0].TenantId, @is(TENANT_TWO));
		assertThat(deployments[1].TenantId, @is(TENANT_ONE));
	  }

	  public virtual void testQueryNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		DeploymentQuery query = repositoryService.createDeploymentQuery();
		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		DeploymentQuery query = repositoryService.createDeploymentQuery();

		assertThat(query.count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
		assertThat(query.tenantIdIn(TENANT_ONE, TENANT_TWO).includeDeploymentsWithoutTenantId().count(), @is(2L));
	  }

	  public virtual void testQueryAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE, TENANT_TWO));

		DeploymentQuery query = repositoryService.createDeploymentQuery();

		assertThat(query.count(), @is(3L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(1L));
		assertThat(query.withoutTenantId().count(), @is(1L));
	  }

	  public virtual void testQueryDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		DeploymentQuery query = repositoryService.createDeploymentQuery();
		assertThat(query.count(), @is(3L));
	  }


	}

}