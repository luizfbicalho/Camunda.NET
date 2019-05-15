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
//	import static org.junit.Assert.assertEquals;


	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using IncidentStatistics = org.camunda.bpm.engine.management.IncidentStatistics;
	using ProcessDefinitionStatistics = org.camunda.bpm.engine.management.ProcessDefinitionStatistics;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// 
	/// <summary>
	/// @author Deivarayan Azhagappan
	/// 
	/// </summary>
	public class MultiTenancySharedProcessDefinitionStatisticsQueryTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancySharedProcessDefinitionStatisticsQueryTest()
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
			tenantRuleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal const string ONE_TASK_PROCESS_DEFINITION_KEY = "oneTaskProcess";
	  protected internal const string FAILED_JOBS_PROCESS_DEFINITION_KEY = "ExampleProcess";

	  protected internal static StaticTenantIdTestProvider tenantIdProvider;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule bootstrapRule = new org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule()
	  public static ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {

		  tenantIdProvider = new StaticTenantIdTestProvider(TENANT_ONE);
		  configuration.TenantIdProvider = tenantIdProvider;

		  return configuration;
		  }
	  }

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);

	  protected internal ProcessEngineTestRule testRule;

	  protected internal RuntimeService runtimeService;

	  protected internal ManagementService managementService;

	  protected internal IdentityService identityService;

	  protected internal ProcessEngineConfiguration processEngineConfiguration;

	  protected internal static readonly BpmnModelInstance oneTaskProcess = Bpmn.createExecutableProcess(ONE_TASK_PROCESS_DEFINITION_KEY).startEvent().userTask().done();

	  protected internal static readonly BpmnModelInstance failingProcess = Bpmn.createExecutableProcess(FAILED_JOBS_PROCESS_DEFINITION_KEY).startEvent().serviceTask().camundaClass("org.camunda.bpm.engine.test.api.multitenancy.FailingDelegate").camundaAsyncBefore().done();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain tenantRuleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain tenantRuleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		runtimeService = engineRule.RuntimeService;
		identityService = engineRule.IdentityService;
		managementService = engineRule.ManagementService;
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void activeProcessInstancesCountWithNoAuthenticatedTenant()
	  public virtual void activeProcessInstancesCountWithNoAuthenticatedTenant()
	  {

		testRule.deploy(oneTaskProcess);

		startProcessInstances(ONE_TASK_PROCESS_DEFINITION_KEY);

		identityService.setAuthentication("user", null, null);

		IList<ProcessDefinitionStatistics> processDefinitionsStatistics = managementService.createProcessDefinitionStatisticsQuery().list();

		// then
		assertEquals(1, processDefinitionsStatistics.Count);
		// user must see only the process instances that belongs to no tenant
		assertEquals(1, processDefinitionsStatistics[0].Instances);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void activeProcessInstancesCountWithAuthenticatedTenant()
	  public virtual void activeProcessInstancesCountWithAuthenticatedTenant()
	  {

		testRule.deploy(oneTaskProcess);

		startProcessInstances(ONE_TASK_PROCESS_DEFINITION_KEY);

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		IList<ProcessDefinitionStatistics> processDefinitionsStatistics = managementService.createProcessDefinitionStatisticsQuery().list();

		// then
		assertEquals(1, processDefinitionsStatistics.Count);
		// user can see the process instances that belongs to tenant1 and instances that have no tenant  
		assertEquals(2, processDefinitionsStatistics[0].Instances);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void activeProcessInstancesCountWithDisabledTenantCheck()
	  public virtual void activeProcessInstancesCountWithDisabledTenantCheck()
	  {

		testRule.deploy(oneTaskProcess);

		startProcessInstances(ONE_TASK_PROCESS_DEFINITION_KEY);

		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		IList<ProcessDefinitionStatistics> processDefinitionsStatistics = managementService.createProcessDefinitionStatisticsQuery().list();

		// then
		assertEquals(1, processDefinitionsStatistics.Count);
		assertEquals(3, processDefinitionsStatistics[0].Instances);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void activeProcessInstancesCountWithMultipleAuthenticatedTenants()
	  public virtual void activeProcessInstancesCountWithMultipleAuthenticatedTenants()
	  {

		testRule.deploy(oneTaskProcess);

		startProcessInstances(ONE_TASK_PROCESS_DEFINITION_KEY);

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE, TENANT_TWO));

		IList<ProcessDefinitionStatistics> processDefinitionsStatistics = managementService.createProcessDefinitionStatisticsQuery().list();

		// then
		assertEquals(1, processDefinitionsStatistics.Count);
		// user can see all the active process instances 
		assertEquals(3, processDefinitionsStatistics[0].Instances);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failedJobsCountWithWithNoAuthenticatedTenant()
	  public virtual void failedJobsCountWithWithNoAuthenticatedTenant()
	  {

		testRule.deploy(failingProcess);

		startProcessInstances(FAILED_JOBS_PROCESS_DEFINITION_KEY);

		testRule.executeAvailableJobs();

		identityService.setAuthentication("user", null, null);

		IList<ProcessDefinitionStatistics> processDefinitionsStatistics = managementService.createProcessDefinitionStatisticsQuery().includeFailedJobs().list();

		// then
		assertEquals(1, processDefinitionsStatistics.Count);
		assertEquals(1, processDefinitionsStatistics[0].FailedJobs);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failedJobsCountWithWithDisabledTenantCheck()
	  public virtual void failedJobsCountWithWithDisabledTenantCheck()
	  {

		testRule.deploy(failingProcess);

		startProcessInstances(FAILED_JOBS_PROCESS_DEFINITION_KEY);

		testRule.executeAvailableJobs();

		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		IList<ProcessDefinitionStatistics> processDefinitionsStatistics = managementService.createProcessDefinitionStatisticsQuery().includeFailedJobs().list();

		// then
		assertEquals(1, processDefinitionsStatistics.Count);
		assertEquals(3, processDefinitionsStatistics[0].FailedJobs);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failedJobsCountWithAuthenticatedTenant()
	  public virtual void failedJobsCountWithAuthenticatedTenant()
	  {

		testRule.deploy(failingProcess);

		startProcessInstances(FAILED_JOBS_PROCESS_DEFINITION_KEY);

		testRule.executeAvailableJobs();

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		IList<ProcessDefinitionStatistics> processDefinitionsStatistics = managementService.createProcessDefinitionStatisticsQuery().includeFailedJobs().list();

		// then
		assertEquals(1, processDefinitionsStatistics.Count);
		assertEquals(2, processDefinitionsStatistics[0].FailedJobs);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failedJobsCountWithMultipleAuthenticatedTenants()
	  public virtual void failedJobsCountWithMultipleAuthenticatedTenants()
	  {

		testRule.deploy(failingProcess);

		startProcessInstances(FAILED_JOBS_PROCESS_DEFINITION_KEY);

		testRule.executeAvailableJobs();

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE, TENANT_TWO));

		IList<ProcessDefinitionStatistics> processDefinitionsStatistics = managementService.createProcessDefinitionStatisticsQuery().includeFailedJobs().list();

		// then
		assertEquals(1, processDefinitionsStatistics.Count);
		assertEquals(3, processDefinitionsStatistics[0].FailedJobs);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void incidentsCountWithNoAuthenticatedTenant()
	  public virtual void incidentsCountWithNoAuthenticatedTenant()
	  {

		testRule.deploy(failingProcess);

		startProcessInstances(FAILED_JOBS_PROCESS_DEFINITION_KEY);

		testRule.executeAvailableJobs();

		identityService.setAuthentication("user", null, null);

		IList<ProcessDefinitionStatistics> processDefinitionsStatistics = managementService.createProcessDefinitionStatisticsQuery().includeIncidents().list();

		// then
		assertEquals(1, processDefinitionsStatistics.Count);

		IList<IncidentStatistics> incidentStatistics = processDefinitionsStatistics[0].IncidentStatistics;
		assertEquals(1, incidentStatistics.Count);
		assertEquals(1, incidentStatistics[0].IncidentCount);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void incidentsCountWithDisabledTenantCheck()
	  public virtual void incidentsCountWithDisabledTenantCheck()
	  {

		testRule.deploy(failingProcess);

		startProcessInstances(FAILED_JOBS_PROCESS_DEFINITION_KEY);

		testRule.executeAvailableJobs();

		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		IList<ProcessDefinitionStatistics> processDefinitionsStatistics = managementService.createProcessDefinitionStatisticsQuery().includeIncidents().list();

		// then
		assertEquals(1, processDefinitionsStatistics.Count);

		IList<IncidentStatistics> incidentStatistics = processDefinitionsStatistics[0].IncidentStatistics;
		assertEquals(1, incidentStatistics.Count);
		assertEquals(3, incidentStatistics[0].IncidentCount);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void incidentsCountWithAuthenticatedTenant()
	  public virtual void incidentsCountWithAuthenticatedTenant()
	  {

		testRule.deploy(failingProcess);

		startProcessInstances(FAILED_JOBS_PROCESS_DEFINITION_KEY);

		testRule.executeAvailableJobs();

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		IList<ProcessDefinitionStatistics> processDefinitionsStatistics = managementService.createProcessDefinitionStatisticsQuery().includeIncidents().list();

		// then
		assertEquals(1, processDefinitionsStatistics.Count);

		IList<IncidentStatistics> incidentStatistics = processDefinitionsStatistics[0].IncidentStatistics;
		assertEquals(1, incidentStatistics.Count);
		assertEquals(2, incidentStatistics[0].IncidentCount);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void incidentsCountWithMultipleAuthenticatedTenants()
	  public virtual void incidentsCountWithMultipleAuthenticatedTenants()
	  {

		testRule.deploy(failingProcess);

		startProcessInstances(FAILED_JOBS_PROCESS_DEFINITION_KEY);

		testRule.executeAvailableJobs();

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE, TENANT_TWO));

		IList<ProcessDefinitionStatistics> processDefinitionsStatistics = managementService.createProcessDefinitionStatisticsQuery().includeIncidents().list();

		// then
		assertEquals(1, processDefinitionsStatistics.Count);
		IList<IncidentStatistics> incidentStatistics = processDefinitionsStatistics[0].IncidentStatistics;
		assertEquals(1, incidentStatistics.Count);
		assertEquals(3, incidentStatistics[0].IncidentCount);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void incidentsCountWithIncidentTypeAndAuthenticatedTenant()
	  public virtual void incidentsCountWithIncidentTypeAndAuthenticatedTenant()
	  {

		testRule.deploy(failingProcess);

		startProcessInstances(FAILED_JOBS_PROCESS_DEFINITION_KEY);

		testRule.executeAvailableJobs();

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		IList<ProcessDefinitionStatistics> processDefinitionsStatistics = managementService.createProcessDefinitionStatisticsQuery().includeIncidentsForType("failedJob").list();

		// then
		assertEquals(1, processDefinitionsStatistics.Count);

		IList<IncidentStatistics> incidentStatistics = processDefinitionsStatistics[0].IncidentStatistics;
		assertEquals(1, incidentStatistics.Count);
		assertEquals(2, incidentStatistics[0].IncidentCount);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void instancesFailedJobsAndIncidentsCountWithAuthenticatedTenant()
	  public virtual void instancesFailedJobsAndIncidentsCountWithAuthenticatedTenant()
	  {

		testRule.deploy(failingProcess);

		startProcessInstances(FAILED_JOBS_PROCESS_DEFINITION_KEY);

		testRule.executeAvailableJobs();

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		IList<ProcessDefinitionStatistics> processDefinitionsStatistics = managementService.createProcessDefinitionStatisticsQuery().includeFailedJobs().includeIncidents().list();

		// then
		assertEquals(1, processDefinitionsStatistics.Count);
		ProcessDefinitionStatistics processDefinitionStatistics = processDefinitionsStatistics[0];
		assertEquals(2, processDefinitionStatistics.Instances);
		assertEquals(2, processDefinitionStatistics.FailedJobs);

		IList<IncidentStatistics> incidentStatistics = processDefinitionStatistics.IncidentStatistics;
		assertEquals(1, incidentStatistics.Count);
		assertEquals(2, incidentStatistics[0].IncidentCount);
	  }

	  protected internal virtual void startProcessInstances(string key)
	  {
		TenantIdProvider = null;
		runtimeService.startProcessInstanceByKey(key);

		TenantIdProvider = TENANT_ONE;
		runtimeService.startProcessInstanceByKey(key);

		TenantIdProvider = TENANT_TWO;
		runtimeService.startProcessInstanceByKey(key);
	  }

	  protected internal virtual string TenantIdProvider
	  {
		  set
		  {
			tenantIdProvider.TenantIdProvider = value;
		  }
	  }
	}

}