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
//	import static org.hamcrest.CoreMatchers.hasItems;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ActivityStatisticsQuery = org.camunda.bpm.engine.management.ActivityStatisticsQuery;
	using DeploymentStatistics = org.camunda.bpm.engine.management.DeploymentStatistics;
	using DeploymentStatisticsQuery = org.camunda.bpm.engine.management.DeploymentStatisticsQuery;
	using ProcessDefinitionStatistics = org.camunda.bpm.engine.management.ProcessDefinitionStatistics;
	using ProcessDefinitionStatisticsQuery = org.camunda.bpm.engine.management.ProcessDefinitionStatisticsQuery;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	public class MultiTenancyStatisticsQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal override void setUp()
	  {

		BpmnModelInstance process = Bpmn.createExecutableProcess("EmptyProcess").startEvent().done();

		BpmnModelInstance singleTaskProcess = Bpmn.createExecutableProcess("SingleTaskProcess").startEvent().userTask().done();

		deployment(process);
		deploymentForTenant(TENANT_ONE, singleTaskProcess);
		deploymentForTenant(TENANT_TWO, process);
	  }

	  public virtual void testDeploymentStatistics()
	  {
		IList<DeploymentStatistics> deploymentStatistics = managementService.createDeploymentStatisticsQuery().list();

		assertThat(deploymentStatistics.Count, @is(3));

		ISet<string> tenantIds = collectDeploymentTenantIds(deploymentStatistics);
		assertThat(tenantIds, hasItems(null, TENANT_ONE, TENANT_TWO));
	  }

	  public virtual void testProcessDefinitionStatistics()
	  {
		IList<ProcessDefinitionStatistics> processDefinitionStatistics = managementService.createProcessDefinitionStatisticsQuery().list();

		assertThat(processDefinitionStatistics.Count, @is(3));

		ISet<string> tenantIds = collectDefinitionTenantIds(processDefinitionStatistics);
		assertThat(tenantIds, hasItems(null, TENANT_ONE, TENANT_TWO));
	  }

	  public virtual void testQueryNoAuthenticatedTenantsForDeploymentStatistics()
	  {
		identityService.setAuthentication("user", null, null);

		DeploymentStatisticsQuery query = managementService.createDeploymentStatisticsQuery();
		assertThat(query.count(), @is(1L));

		ISet<string> tenantIds = collectDeploymentTenantIds(query.list());
		assertThat(tenantIds.Count, @is(1));
		assertThat(tenantIds.GetEnumerator().next(), @is(nullValue()));
	  }

	  public virtual void testQueryAuthenticatedTenantForDeploymentStatistics()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		DeploymentStatisticsQuery query = managementService.createDeploymentStatisticsQuery();

		assertThat(query.count(), @is(2L));

		ISet<string> tenantIds = collectDeploymentTenantIds(query.list());
		assertThat(tenantIds.Count, @is(2));
		assertThat(tenantIds, hasItems(null, TENANT_ONE));
	  }

	  public virtual void testQueryAuthenticatedTenantsForDeploymentStatistics()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE, TENANT_TWO));

		DeploymentStatisticsQuery query = managementService.createDeploymentStatisticsQuery();

		assertThat(query.count(), @is(3L));

		ISet<string> tenantIds = collectDeploymentTenantIds(query.list());
		assertThat(tenantIds.Count, @is(3));
		assertThat(tenantIds, hasItems(null, TENANT_ONE, TENANT_TWO));
	  }

	  public virtual void testQueryDisabledTenantCheckForDeploymentStatistics()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		DeploymentStatisticsQuery query = managementService.createDeploymentStatisticsQuery();

		assertThat(query.count(), @is(3L));

		ISet<string> tenantIds = collectDeploymentTenantIds(query.list());
		assertThat(tenantIds.Count, @is(3));
		assertThat(tenantIds, hasItems(null, TENANT_ONE, TENANT_TWO));
	  }

	  public virtual void testQueryNoAuthenticatedTenantsForProcessDefinitionStatistics()
	  {
		identityService.setAuthentication("user", null, null);

		ProcessDefinitionStatisticsQuery query = managementService.createProcessDefinitionStatisticsQuery();
		assertThat(query.count(), @is(1L));

		ISet<string> tenantIds = collectDefinitionTenantIds(query.list());
		assertThat(tenantIds.Count, @is(1));
		assertThat(tenantIds.GetEnumerator().next(), @is(nullValue()));
	  }

	  public virtual void testQueryAuthenticatedTenantForProcessDefinitionStatistics()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		ProcessDefinitionStatisticsQuery query = managementService.createProcessDefinitionStatisticsQuery();

		assertThat(query.count(), @is(2L));

		ISet<string> tenantIds = collectDefinitionTenantIds(query.list());
		assertThat(tenantIds.Count, @is(2));
		assertThat(tenantIds, hasItems(null, TENANT_ONE));
	  }

	  public virtual void testQueryAuthenticatedTenantsForProcessDefinitionStatistics()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE, TENANT_TWO));

		ProcessDefinitionStatisticsQuery query = managementService.createProcessDefinitionStatisticsQuery();

		assertThat(query.count(), @is(3L));

		ISet<string> tenantIds = collectDefinitionTenantIds(query.list());
		assertThat(tenantIds.Count, @is(3));
		assertThat(tenantIds, hasItems(null, TENANT_ONE, TENANT_TWO));
	  }

	  public virtual void testQueryDisabledTenantCheckForProcessDefinitionStatistics()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		ProcessDefinitionStatisticsQuery query = managementService.createProcessDefinitionStatisticsQuery();

		assertThat(query.count(), @is(3L));

		ISet<string> tenantIds = collectDefinitionTenantIds(query.list());
		assertThat(tenantIds.Count, @is(3));
		assertThat(tenantIds, hasItems(null, TENANT_ONE, TENANT_TWO));
	  }

	  public virtual void testActivityStatistics()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("SingleTaskProcess");

		ActivityStatisticsQuery query = managementService.createActivityStatisticsQuery(processInstance.ProcessDefinitionId);

		assertThat(query.count(), @is(1L));

	  }

	  public virtual void testQueryAuthenticatedTenantForActivityStatistics()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("SingleTaskProcess");

		ActivityStatisticsQuery query = managementService.createActivityStatisticsQuery(processInstance.ProcessDefinitionId);

		assertThat(query.count(), @is(1L));

	  }

	  public virtual void testQueryNoAuthenticatedTenantForActivityStatistics()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("SingleTaskProcess");

		identityService.setAuthentication("user", null);

		ActivityStatisticsQuery query = managementService.createActivityStatisticsQuery(processInstance.ProcessDefinitionId);

		assertThat(query.count(), @is(0L));

	  }

	  public virtual void testQueryDisabledTenantCheckForActivityStatistics()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("SingleTaskProcess");

		identityService.setAuthentication("user", null);
		processEngineConfiguration.TenantCheckEnabled = false;

		ActivityStatisticsQuery query = managementService.createActivityStatisticsQuery(processInstance.ProcessDefinitionId);

		assertThat(query.count(), @is(1L));

	  }

	  protected internal virtual ISet<string> collectDeploymentTenantIds(IList<DeploymentStatistics> deploymentStatistics)
	  {
		ISet<string> tenantIds = new HashSet<string>();

		foreach (DeploymentStatistics statistics in deploymentStatistics)
		{
		  tenantIds.Add(statistics.TenantId);
		}
		return tenantIds;
	  }

	  protected internal virtual ISet<string> collectDefinitionTenantIds(IList<ProcessDefinitionStatistics> processDefinitionStatistics)
	  {
		ISet<string> tenantIds = new HashSet<string>();

		foreach (ProcessDefinitionStatistics statistics in processDefinitionStatistics)
		{
		  tenantIds.Add(statistics.TenantId);
		}
		return tenantIds;
	  }

	}

}