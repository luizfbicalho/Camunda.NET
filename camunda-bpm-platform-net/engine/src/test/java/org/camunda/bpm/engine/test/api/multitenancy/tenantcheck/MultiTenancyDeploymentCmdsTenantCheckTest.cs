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
//	import static org.hamcrest.Matchers.hasSize;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using DeploymentQuery = org.camunda.bpm.engine.repository.DeploymentQuery;
	using Resource = org.camunda.bpm.engine.repository.Resource;
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

	/// <summary>
	/// @author kristin.polenz
	/// </summary>
	public class MultiTenancyDeploymentCmdsTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyDeploymentCmdsTenantCheckTest()
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
	  protected internal static readonly BpmnModelInstance startEndProcess = Bpmn.createExecutableProcess().startEvent().endEvent().done();

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown= org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  protected internal RepositoryService repositoryService;
	  protected internal IdentityService identityService;
	  protected internal ProcessEngineConfiguration processEngineConfiguration;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		repositoryService = engineRule.RepositoryService;
		identityService = engineRule.IdentityService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createDeploymentForAnotherTenant()
	  public virtual void createDeploymentForAnotherTenant()
	  {
		identityService.setAuthentication("user", null, null);

		repositoryService.createDeployment().addModelInstance("emptyProcess.bpmn", emptyProcess).tenantId(TENANT_ONE).deploy();

		identityService.clearAuthentication();

		DeploymentQuery query = repositoryService.createDeploymentQuery();
		assertThat(query.count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createDeploymentWithAuthenticatedTenant()
	  public virtual void createDeploymentWithAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		repositoryService.createDeployment().addModelInstance("emptyProcess.bpmn", emptyProcess).tenantId(TENANT_ONE).deploy();

		identityService.clearAuthentication();

		DeploymentQuery query = repositoryService.createDeploymentQuery();
		assertThat(query.count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createDeploymentDisabledTenantCheck()
	  public virtual void createDeploymentDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		repositoryService.createDeployment().addModelInstance("emptyProcessOne", emptyProcess).tenantId(TENANT_ONE).deploy();
		repositoryService.createDeployment().addModelInstance("emptyProcessTwo", startEndProcess).tenantId(TENANT_TWO).deploy();

		DeploymentQuery query = repositoryService.createDeploymentQuery();
		assertThat(query.count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToDeleteDeploymentNoAuthenticatedTenant()
	  public virtual void failToDeleteDeploymentNoAuthenticatedTenant()
	  {
		Deployment deployment = testRule.deployForTenant(TENANT_ONE, emptyProcess);

		identityService.setAuthentication("user", null, null);

		// declare expected exception
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot delete the deployment");

		repositoryService.deleteDeployment(deployment.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteDeploymentWithAuthenticatedTenant()
	  public virtual void deleteDeploymentWithAuthenticatedTenant()
	  {
		Deployment deployment = testRule.deployForTenant(TENANT_ONE, emptyProcess);

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		repositoryService.deleteDeployment(deployment.Id);

		identityService.clearAuthentication();

		DeploymentQuery query = repositoryService.createDeploymentQuery();
		assertThat(query.count(), @is(0L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteDeploymentDisabledTenantCheck()
	  public virtual void deleteDeploymentDisabledTenantCheck()
	  {
		Deployment deploymentOne = testRule.deployForTenant(TENANT_ONE, emptyProcess);
		Deployment deploymentTwo = testRule.deployForTenant(TENANT_TWO, startEndProcess);

		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		repositoryService.deleteDeployment(deploymentOne.Id);
		repositoryService.deleteDeployment(deploymentTwo.Id);

		DeploymentQuery query = repositoryService.createDeploymentQuery();
		assertThat(query.count(), @is(0L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(0L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToGetDeploymentResourceNamesNoAuthenticatedTenant()
	  public virtual void failToGetDeploymentResourceNamesNoAuthenticatedTenant()
	  {
		Deployment deployment = testRule.deployForTenant(TENANT_ONE, emptyProcess);

		identityService.setAuthentication("user", null, null);

		// declare expected exception
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot get the deployment");

		repositoryService.getDeploymentResourceNames(deployment.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getDeploymentResourceNamesWithAuthenticatedTenant()
	  public virtual void getDeploymentResourceNamesWithAuthenticatedTenant()
	  {
		Deployment deployment = testRule.deployForTenant(TENANT_ONE, emptyProcess);

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		IList<string> deploymentResourceNames = repositoryService.getDeploymentResourceNames(deployment.Id);
		assertThat(deploymentResourceNames, hasSize(1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getDeploymentResourceNamesDisabledTenantCheck()
	  public virtual void getDeploymentResourceNamesDisabledTenantCheck()
	  {
		Deployment deploymentOne = testRule.deployForTenant(TENANT_ONE, emptyProcess);
		Deployment deploymentTwo = testRule.deployForTenant(TENANT_TWO, startEndProcess);

		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		IList<string> deploymentResourceNames = repositoryService.getDeploymentResourceNames(deploymentOne.Id);
		assertThat(deploymentResourceNames, hasSize(1));

		deploymentResourceNames = repositoryService.getDeploymentResourceNames(deploymentTwo.Id);
		assertThat(deploymentResourceNames, hasSize(1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToGetDeploymentResourcesNoAuthenticatedTenant()
	  public virtual void failToGetDeploymentResourcesNoAuthenticatedTenant()
	  {
		Deployment deployment = testRule.deployForTenant(TENANT_ONE, emptyProcess);

		identityService.setAuthentication("user", null, null);

		// declare expected exception
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot get the deployment");

		repositoryService.getDeploymentResources(deployment.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getDeploymentResourcesWithAuthenticatedTenant()
	  public virtual void getDeploymentResourcesWithAuthenticatedTenant()
	  {
		Deployment deployment = testRule.deployForTenant(TENANT_ONE, emptyProcess);

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		IList<Resource> deploymentResources = repositoryService.getDeploymentResources(deployment.Id);
		assertThat(deploymentResources, hasSize(1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getDeploymentResourcesDisabledTenantCheck()
	  public virtual void getDeploymentResourcesDisabledTenantCheck()
	  {
		Deployment deploymentOne = testRule.deployForTenant(TENANT_ONE, emptyProcess);
		Deployment deploymentTwo = testRule.deployForTenant(TENANT_TWO, startEndProcess);

		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		IList<Resource> deploymentResources = repositoryService.getDeploymentResources(deploymentOne.Id);
		assertThat(deploymentResources, hasSize(1));

		deploymentResources = repositoryService.getDeploymentResources(deploymentTwo.Id);
		assertThat(deploymentResources, hasSize(1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToGetResourceAsStreamNoAuthenticatedTenant()
	  public virtual void failToGetResourceAsStreamNoAuthenticatedTenant()
	  {
		Deployment deployment = testRule.deployForTenant(TENANT_ONE, emptyProcess);

		Resource resource = repositoryService.getDeploymentResources(deployment.Id)[0];

		identityService.setAuthentication("user", null, null);

		// declare expected exception
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot get the deployment");

		repositoryService.getResourceAsStream(deployment.Id, resource.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getResourceAsStreamWithAuthenticatedTenant()
	  public virtual void getResourceAsStreamWithAuthenticatedTenant()
	  {
		Deployment deployment = testRule.deployForTenant(TENANT_ONE, emptyProcess);

		Resource resource = repositoryService.getDeploymentResources(deployment.Id)[0];

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		Stream inputStream = repositoryService.getResourceAsStream(deployment.Id, resource.Name);
		assertThat(inputStream, notNullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getResourceAsStreamDisabledTenantCheck()
	  public virtual void getResourceAsStreamDisabledTenantCheck()
	  {
		Deployment deploymentOne = testRule.deployForTenant(TENANT_ONE, emptyProcess);
		Deployment deploymentTwo = testRule.deployForTenant(TENANT_TWO, startEndProcess);

		Resource resourceOne = repositoryService.getDeploymentResources(deploymentOne.Id)[0];
		Resource resourceTwo = repositoryService.getDeploymentResources(deploymentTwo.Id)[0];

		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		Stream inputStream = repositoryService.getResourceAsStream(deploymentOne.Id, resourceOne.Name);
		assertThat(inputStream, notNullValue());

		inputStream = repositoryService.getResourceAsStream(deploymentTwo.Id, resourceTwo.Name);
		assertThat(inputStream, notNullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToGetResourceAsStreamByIdNoAuthenticatedTenant()
	  public virtual void failToGetResourceAsStreamByIdNoAuthenticatedTenant()
	  {
		Deployment deployment = testRule.deployForTenant(TENANT_ONE, emptyProcess);

		Resource resource = repositoryService.getDeploymentResources(deployment.Id)[0];

		identityService.setAuthentication("user", null, null);

		// declare expected exception
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot get the deployment");

		repositoryService.getResourceAsStreamById(deployment.Id, resource.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getResourceAsStreamByIdWithAuthenticatedTenant()
	  public virtual void getResourceAsStreamByIdWithAuthenticatedTenant()
	  {
		Deployment deployment = testRule.deployForTenant(TENANT_ONE, emptyProcess);

		Resource resource = repositoryService.getDeploymentResources(deployment.Id)[0];

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		Stream inputStream = repositoryService.getResourceAsStreamById(deployment.Id, resource.Id);
		assertThat(inputStream, notNullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getResourceAsStreamByIdDisabledTenantCheck()
	  public virtual void getResourceAsStreamByIdDisabledTenantCheck()
	  {
		Deployment deploymentOne = testRule.deployForTenant(TENANT_ONE, emptyProcess);
		Deployment deploymentTwo = testRule.deployForTenant(TENANT_TWO, startEndProcess);

		Resource resourceOne = repositoryService.getDeploymentResources(deploymentOne.Id)[0];
		Resource resourceTwo = repositoryService.getDeploymentResources(deploymentTwo.Id)[0];

		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		Stream inputStream = repositoryService.getResourceAsStreamById(deploymentOne.Id, resourceOne.Id);
		assertThat(inputStream, notNullValue());

		inputStream = repositoryService.getResourceAsStreamById(deploymentTwo.Id, resourceTwo.Id);
		assertThat(inputStream, notNullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void redeployForDifferentAuthenticatedTenants()
	  public virtual void redeployForDifferentAuthenticatedTenants()
	  {
		Deployment deploymentOne = repositoryService.createDeployment().addModelInstance("emptyProcess.bpmn", emptyProcess).addModelInstance("startEndProcess.bpmn", startEndProcess).tenantId(TENANT_ONE).deploy();

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_TWO));

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot get the deployment");

		repositoryService.createDeployment().addDeploymentResources(deploymentOne.Id).tenantId(TENANT_TWO).deploy();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void redeployForTheSameAuthenticatedTenant()
	  public virtual void redeployForTheSameAuthenticatedTenant()
	  {
		Deployment deploymentOne = repositoryService.createDeployment().addModelInstance("emptyProcess.bpmn", emptyProcess).addModelInstance("startEndProcess.bpmn", startEndProcess).tenantId(TENANT_ONE).deploy();

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		repositoryService.createDeployment().addDeploymentResources(deploymentOne.Id).tenantId(TENANT_ONE).deploy();

		DeploymentQuery query = repositoryService.createDeploymentQuery();
		assertThat(query.count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(2L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void redeployForDifferentAuthenticatedTenantsDisabledTenantCheck()
	  public virtual void redeployForDifferentAuthenticatedTenantsDisabledTenantCheck()
	  {
		Deployment deploymentOne = repositoryService.createDeployment().addModelInstance("emptyProcess.bpmn", emptyProcess).addModelInstance("startEndProcess.bpmn", startEndProcess).tenantId(TENANT_ONE).deploy();

		identityService.setAuthentication("user", null, null);
		processEngineConfiguration.TenantCheckEnabled = false;

		repositoryService.createDeployment().addDeploymentResources(deploymentOne.Id).tenantId(TENANT_TWO).deploy();

		DeploymentQuery query = repositoryService.createDeploymentQuery();
		assertThat(query.count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void tearDown()
	  {
		identityService.clearAuthentication();
		foreach (Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}
	  }
	}

}