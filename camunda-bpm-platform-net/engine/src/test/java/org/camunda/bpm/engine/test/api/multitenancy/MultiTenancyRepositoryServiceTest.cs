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
namespace org.camunda.bpm.engine.test.api.multitenancy
{

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using CaseDefinitionEntity = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity;
	using DecisionDefinitionEntity = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionDefinitionEntity;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ResourceDefinitionEntity = org.camunda.bpm.engine.impl.repository.ResourceDefinitionEntity;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using DeploymentBuilder = org.camunda.bpm.engine.repository.DeploymentBuilder;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	public class MultiTenancyRepositoryServiceTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyRepositoryServiceTest()
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


	  protected internal const string TENANT_TWO = "tenant2";
	  protected internal const string TENANT_ONE = "tenant1";

	  protected internal static readonly BpmnModelInstance emptyProcess = Bpmn.createExecutableProcess().done();
	  protected internal const string CMMN = "org/camunda/bpm/engine/test/cmmn/deployment/CmmnDeploymentTest.testSimpleDeployment.cmmn";
	  protected internal const string DMN = "org/camunda/bpm/engine/test/api/multitenancy/simpleDecisionTable.dmn";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown= org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  protected internal RepositoryService repositoryService;
	  protected internal ProcessEngineConfiguration processEngineConfiguration;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		repositoryService = engineRule.RepositoryService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deploymentWithoutTenantId()
	  public virtual void deploymentWithoutTenantId()
	  {
		createDeploymentBuilder().deploy();

		Deployment deployment = repositoryService.createDeploymentQuery().singleResult();

		assertThat(deployment, @is(notNullValue()));
		assertThat(deployment.TenantId, @is(nullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deploymentWithTenantId()
	  public virtual void deploymentWithTenantId()
	  {
		createDeploymentBuilder().tenantId(TENANT_ONE).deploy();

		Deployment deployment = repositoryService.createDeploymentQuery().singleResult();

		assertThat(deployment, @is(notNullValue()));
		assertThat(deployment.TenantId, @is(TENANT_ONE));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void processDefinitionVersionWithTenantId()
	  public virtual void processDefinitionVersionWithTenantId()
	  {
		createDeploymentBuilder().tenantId(TENANT_ONE).deploy();

		createDeploymentBuilder().tenantId(TENANT_ONE).deploy();

		createDeploymentBuilder().tenantId(TENANT_TWO).deploy();

		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().orderByTenantId().asc().orderByProcessDefinitionVersion().asc().list();

		assertThat(processDefinitions.Count, @is(3));
		// process definition was deployed twice for tenant one
		assertThat(processDefinitions[0].Version, @is(1));
		assertThat(processDefinitions[1].Version, @is(2));
		// process definition version of tenant two have to be independent from tenant one
		assertThat(processDefinitions[2].Version, @is(1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deploymentWithDuplicateFilteringForSameTenant()
	  public virtual void deploymentWithDuplicateFilteringForSameTenant()
	  {
		// given: a deployment with tenant ID
		createDeploymentBuilder().enableDuplicateFiltering(false).name("twice").tenantId(TENANT_ONE).deploy();

		// if the same process is deployed with the same tenant ID again
		createDeploymentBuilder().enableDuplicateFiltering(false).name("twice").tenantId(TENANT_ONE).deploy();

		// then it does not create a new deployment
		assertThat(repositoryService.createDeploymentQuery().count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deploymentWithDuplicateFilteringForDifferentTenants()
	  public virtual void deploymentWithDuplicateFilteringForDifferentTenants()
	  {
		// given: a deployment with tenant ID
		createDeploymentBuilder().enableDuplicateFiltering(false).name("twice").tenantId(TENANT_ONE).deploy();

		// if the same process is deployed with the another tenant ID
		createDeploymentBuilder().enableDuplicateFiltering(false).name("twice").tenantId(TENANT_TWO).deploy();

		// then a new deployment is created
		assertThat(repositoryService.createDeploymentQuery().count(), @is(2L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deploymentWithDuplicateFilteringIgnoreDeploymentForNoTenant()
	  public virtual void deploymentWithDuplicateFilteringIgnoreDeploymentForNoTenant()
	  {
		// given: a deployment without tenant ID
		createDeploymentBuilder().enableDuplicateFiltering(false).name("twice").deploy();

		// if the same process is deployed with tenant ID
		createDeploymentBuilder().enableDuplicateFiltering(false).name("twice").tenantId(TENANT_ONE).deploy();

		// then a new deployment is created
		assertThat(repositoryService.createDeploymentQuery().count(), @is(2L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deploymentWithDuplicateFilteringIgnoreDeploymentForTenant()
	  public virtual void deploymentWithDuplicateFilteringIgnoreDeploymentForTenant()
	  {
		// given: a deployment with tenant ID
		createDeploymentBuilder().enableDuplicateFiltering(false).name("twice").tenantId(TENANT_ONE).deploy();

		// if the same process is deployed without tenant ID
		createDeploymentBuilder().enableDuplicateFiltering(false).name("twice").deploy();

		// then a new deployment is created
		assertThat(repositoryService.createDeploymentQuery().count(), @is(2L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getPreviousProcessDefinitionWithTenantId()
	  public virtual void getPreviousProcessDefinitionWithTenantId()
	  {
		testRule.deployForTenant(TENANT_ONE, emptyProcess);
		testRule.deployForTenant(TENANT_ONE, emptyProcess);
		testRule.deployForTenant(TENANT_ONE, emptyProcess);

		testRule.deployForTenant(TENANT_TWO, emptyProcess);
		testRule.deployForTenant(TENANT_TWO, emptyProcess);

		IList<ProcessDefinition> latestProcessDefinitions = repositoryService.createProcessDefinitionQuery().latestVersion().orderByTenantId().asc().list();

		ProcessDefinitionEntity previousDefinitionTenantOne = getPreviousDefinition((ProcessDefinitionEntity) latestProcessDefinitions[0]);
		ProcessDefinitionEntity previousDefinitionTenantTwo = getPreviousDefinition((ProcessDefinitionEntity) latestProcessDefinitions[1]);

		assertThat(previousDefinitionTenantOne.Version, @is(2));
		assertThat(previousDefinitionTenantOne.TenantId, @is(TENANT_ONE));

		assertThat(previousDefinitionTenantTwo.Version, @is(1));
		assertThat(previousDefinitionTenantTwo.TenantId, @is(TENANT_TWO));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getPreviousCaseDefinitionWithTenantId()
	  public virtual void getPreviousCaseDefinitionWithTenantId()
	  {
		testRule.deployForTenant(TENANT_ONE, CMMN);
		testRule.deployForTenant(TENANT_ONE, CMMN);
		testRule.deployForTenant(TENANT_ONE, CMMN);

		testRule.deployForTenant(TENANT_TWO, CMMN);
		testRule.deployForTenant(TENANT_TWO, CMMN);

		IList<CaseDefinition> latestCaseDefinitions = repositoryService.createCaseDefinitionQuery().latestVersion().orderByTenantId().asc().list();

		CaseDefinitionEntity previousDefinitionTenantOne = getPreviousDefinition((CaseDefinitionEntity) latestCaseDefinitions[0]);
		CaseDefinitionEntity previousDefinitionTenantTwo = getPreviousDefinition((CaseDefinitionEntity) latestCaseDefinitions[1]);

		assertThat(previousDefinitionTenantOne.Version, @is(2));
		assertThat(previousDefinitionTenantOne.TenantId, @is(TENANT_ONE));

		assertThat(previousDefinitionTenantTwo.Version, @is(1));
		assertThat(previousDefinitionTenantTwo.TenantId, @is(TENANT_TWO));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getPreviousDecisionDefinitionWithTenantId()
	  public virtual void getPreviousDecisionDefinitionWithTenantId()
	  {
		testRule.deployForTenant(TENANT_ONE, DMN);
		testRule.deployForTenant(TENANT_ONE, DMN);
		testRule.deployForTenant(TENANT_ONE, DMN);

		testRule.deployForTenant(TENANT_TWO, DMN);
		testRule.deployForTenant(TENANT_TWO, DMN);

		IList<DecisionDefinition> latestDefinitions = repositoryService.createDecisionDefinitionQuery().latestVersion().orderByTenantId().asc().list();

		DecisionDefinitionEntity previousDefinitionTenantOne = getPreviousDefinition((DecisionDefinitionEntity) latestDefinitions[0]);
		DecisionDefinitionEntity previousDefinitionTenantTwo = getPreviousDefinition((DecisionDefinitionEntity) latestDefinitions[1]);

		assertThat(previousDefinitionTenantOne.Version, @is(2));
		assertThat(previousDefinitionTenantOne.TenantId, @is(TENANT_ONE));

		assertThat(previousDefinitionTenantTwo.Version, @is(1));
		assertThat(previousDefinitionTenantTwo.TenantId, @is(TENANT_TWO));
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected <T extends org.camunda.bpm.engine.impl.repository.ResourceDefinitionEntity> T getPreviousDefinition(final T definitionEntity)
	  protected internal virtual T getPreviousDefinition<T>(T definitionEntity) where T : org.camunda.bpm.engine.impl.repository.ResourceDefinitionEntity
	  {
		return ((ProcessEngineConfigurationImpl) processEngineConfiguration).CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this, definitionEntity));
	  }

	  private class CommandAnonymousInnerClass : Command<T>
	  {
		  private readonly MultiTenancyRepositoryServiceTest outerInstance;

		  private ResourceDefinitionEntity definitionEntity;

		  public CommandAnonymousInnerClass(MultiTenancyRepositoryServiceTest outerInstance, ResourceDefinitionEntity definitionEntity)
		  {
			  this.outerInstance = outerInstance;
			  this.definitionEntity = definitionEntity;
		  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public T execute(org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
		  public override T execute(CommandContext commandContext)
		  {
			return (T) definitionEntity.PreviousDefinition;
		  }
	  }

	  protected internal virtual DeploymentBuilder createDeploymentBuilder()
	  {
		return repositoryService.createDeployment().addModelInstance("testProcess.bpmn", emptyProcess);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void tearDown()
	  {
		foreach (Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}
	  }

	}

}