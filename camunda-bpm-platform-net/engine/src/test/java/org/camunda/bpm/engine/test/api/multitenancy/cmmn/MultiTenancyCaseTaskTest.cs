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
namespace org.camunda.bpm.engine.test.api.multitenancy.cmmn
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseInstanceQuery = org.camunda.bpm.engine.runtime.CaseInstanceQuery;

	public class MultiTenancyCaseTaskTest : PluggableProcessEngineTestCase
	{

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal const string CMMN_LATEST = "org/camunda/bpm/engine/test/api/multitenancy/CaseWithCaseTask.cmmn";
	  protected internal const string CMMN_LATEST_WITH_MANUAL_ACTIVATION = "org/camunda/bpm/engine/test/api/multitenancy/CaseWithCaseTaskWithManualActivation.cmmn";
	  protected internal const string CMMN_DEPLOYMENT = "org/camunda/bpm/engine/test/api/multitenancy/CaseWithCaseTaskDeploymentBinding.cmmn";
	  protected internal const string CMMN_VERSION = "org/camunda/bpm/engine/test/api/multitenancy/CaseWithCaseTaskVersionBinding.cmmn";
	  protected internal const string CMMN_VERSION_2 = "org/camunda/bpm/engine/test/api/multitenancy/CaseWithCaseTaskVersionBinding_v2.cmmn";

	  protected internal const string CMMN_TENANT_CONST = "org/camunda/bpm/engine/test/api/multitenancy/CaseWithCaseTaskTenantIdConst.cmmn";
	  protected internal const string CMMN_TENANT_EXPR = "org/camunda/bpm/engine/test/api/multitenancy/CaseWithCaseTaskTenantIdExpr.cmmn";

	  protected internal const string CMMN_CASE = "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn";

	  protected internal const string CASE_TASK_ID = "PI_CaseTask_1";

	  public virtual void testStartCaseInstanceWithDeploymentBinding()
	  {

		deploymentForTenant(TENANT_ONE, CMMN_DEPLOYMENT, CMMN_CASE);
		deploymentForTenant(TENANT_TWO, CMMN_DEPLOYMENT, CMMN_CASE);

		createCaseInstance("caseTaskCaseDeployment", TENANT_ONE);
		createCaseInstance("caseTaskCaseDeployment", TENANT_TWO);

		CaseInstanceQuery query = caseService.createCaseInstanceQuery().caseDefinitionKey("oneTaskCase");
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(1L));
	  }

	  public virtual void testStartCaseInstanceWithLatestBindingSameVersion()
	  {

		deploymentForTenant(TENANT_ONE, CMMN_LATEST_WITH_MANUAL_ACTIVATION, CMMN_CASE);
		deploymentForTenant(TENANT_TWO, CMMN_LATEST_WITH_MANUAL_ACTIVATION, CMMN_CASE);

		createCaseInstance("caseTaskCase", TENANT_ONE);
		createCaseInstance("caseTaskCase", TENANT_TWO);

		CaseInstanceQuery query = caseService.createCaseInstanceQuery().caseDefinitionKey("oneTaskCase");
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(1L));
	  }

	  public virtual void testStartCaseInstanceWithLatestBindingDifferentVersion()
	  {

		deploymentForTenant(TENANT_ONE, CMMN_LATEST_WITH_MANUAL_ACTIVATION, CMMN_CASE);

		deploymentForTenant(TENANT_TWO, CMMN_LATEST_WITH_MANUAL_ACTIVATION, CMMN_CASE);
		deploymentForTenant(TENANT_TWO, CMMN_CASE);

		createCaseInstance("caseTaskCase", TENANT_ONE);
		createCaseInstance("caseTaskCase", TENANT_TWO);

		CaseInstanceQuery query = caseService.createCaseInstanceQuery().caseDefinitionKey("oneTaskCase");
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));

		CaseDefinition latestCaseDefinitionTenantTwo = repositoryService.createCaseDefinitionQuery().caseDefinitionKey("oneTaskCase").tenantIdIn(TENANT_TWO).latestVersion().singleResult();
		query = caseService.createCaseInstanceQuery().caseDefinitionId(latestCaseDefinitionTenantTwo.Id);
		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testStartCaseInstanceWithVersionBinding()
	  {

		deploymentForTenant(TENANT_ONE, CMMN_VERSION, CMMN_CASE);
		deploymentForTenant(TENANT_TWO, CMMN_VERSION, CMMN_CASE);

		createCaseInstance("caseTaskCaseVersion", TENANT_ONE);
		createCaseInstance("caseTaskCaseVersion", TENANT_TWO);

		CaseInstanceQuery query = caseService.createCaseInstanceQuery().caseDefinitionKey("oneTaskCase");
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(1L));
	  }

	  public virtual void testFailStartCaseInstanceFromOtherTenantWithDeploymentBinding()
	  {

		deploymentForTenant(TENANT_ONE, CMMN_DEPLOYMENT);
		deploymentForTenant(TENANT_TWO, CMMN_CASE);

		try
		{
		  createCaseInstance("caseTaskCaseDeployment", TENANT_ONE);

		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("no case definition deployed with key = 'oneTaskCase'"));
		}
	  }

	  public virtual void testFailStartCaseInstanceFromOtherTenantWithLatestBinding()
	  {

		deploymentForTenant(TENANT_ONE, CMMN_LATEST);
		deploymentForTenant(TENANT_TWO, CMMN_CASE);

		try
		{
		  createCaseInstance("caseTaskCase", TENANT_ONE);

		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("no case definition deployed with key 'oneTaskCase'"));
		}
	  }

	  public virtual void testFailStartCaseInstanceFromOtherTenantWithVersionBinding()
	  {

		deploymentForTenant(TENANT_ONE, CMMN_VERSION_2, CMMN_CASE);

		deploymentForTenant(TENANT_TWO, CMMN_CASE);
		deploymentForTenant(TENANT_TWO, CMMN_CASE);

		try
		{
		  createCaseInstance("caseTaskCaseVersion", TENANT_ONE);

		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("no case definition deployed with key = 'oneTaskCase'"));
		}
	  }

	  public virtual void testCaseRefTenantIdConstant()
	  {
		deployment(CMMN_TENANT_CONST);
		deploymentForTenant(TENANT_ONE, CMMN_CASE);

		caseService.withCaseDefinitionByKey("caseTaskCase").create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery().caseDefinitionKey("oneTaskCase");
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

	  public virtual void testCaseRefTenantIdExpression()
	  {
		deployment(CMMN_TENANT_EXPR);
		deploymentForTenant(TENANT_ONE, CMMN_CASE);

		caseService.withCaseDefinitionByKey("caseTaskCase").create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery().caseDefinitionKey("oneTaskCase");
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

	  protected internal virtual void createCaseInstance(string caseDefinitionKey, string tenantId)
	  {
		caseService.withCaseDefinitionByKey(caseDefinitionKey).caseDefinitionTenantId(tenantId).create();

		CaseExecution caseExecution = caseService.createCaseExecutionQuery().activityId(CASE_TASK_ID).tenantIdIn(tenantId).singleResult();
		caseService.withCaseExecution(caseExecution.Id).manualStart();
	  }

	}

}