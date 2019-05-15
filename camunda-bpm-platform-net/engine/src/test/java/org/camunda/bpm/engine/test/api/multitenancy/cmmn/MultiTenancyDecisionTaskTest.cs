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
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;

	public class MultiTenancyDecisionTaskTest : PluggableProcessEngineTestCase
	{

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal const string CMMN_LATEST = "org/camunda/bpm/engine/test/api/multitenancy/CaseWithDecisionTask.cmmn";
	  protected internal const string CMMN_DEPLOYMENT = "org/camunda/bpm/engine/test/api/multitenancy/CaseWithDecisionTaskDeploymentBinding.cmmn";
	  protected internal const string CMMN_VERSION = "org/camunda/bpm/engine/test/api/multitenancy/CaseWithDecisionTaskVersionBinding.cmmn";
	  protected internal const string CMMN_VERSION_2 = "org/camunda/bpm/engine/test/api/multitenancy/CaseWithDecisionTaskVersionBinding_v2.cmmn";
	  protected internal const string CMMN_CONST = "org/camunda/bpm/engine/test/api/multitenancy/CaseWithDecisionTaskTenantIdConst.cmmn";
	  protected internal const string CMMN_WITHOUT_TENANT = "org/camunda/bpm/engine/test/api/multitenancy/CaseWithDecisionTaskWithoutTenantId.cmmn";
	  protected internal const string CMMN_EXPR = "org/camunda/bpm/engine/test/api/multitenancy/CaseWithDecisionTaskTenantIdExpr.cmmn";

	  protected internal const string DMN_FILE = "org/camunda/bpm/engine/test/api/multitenancy/simpleDecisionTable.dmn";
	  protected internal const string DMN_FILE_VERSION_TWO = "org/camunda/bpm/engine/test/api/multitenancy/simpleDecisionTable_v2.dmn";

	  protected internal const string CASE_DEFINITION_KEY = "caseDecisionTask";
	  protected internal const string DECISION_TASK_ID = "PI_DecisionTask_1";

	  protected internal const string RESULT_OF_VERSION_ONE = "A";
	  protected internal const string RESULT_OF_VERSION_TWO = "C";

	  public virtual void testEvaluateDecisionWithDeploymentBinding()
	  {
		deploymentForTenant(TENANT_ONE, CMMN_DEPLOYMENT, DMN_FILE);
		deploymentForTenant(TENANT_TWO, CMMN_DEPLOYMENT, DMN_FILE_VERSION_TWO);

		CaseInstance caseInstanceOne = createCaseInstance(CASE_DEFINITION_KEY, TENANT_ONE);
		CaseInstance caseInstanceTwo = createCaseInstance(CASE_DEFINITION_KEY, TENANT_TWO);

		assertThat((string)caseService.getVariable(caseInstanceOne.Id, "decisionVar"), @is(RESULT_OF_VERSION_ONE));
		assertThat((string)caseService.getVariable(caseInstanceTwo.Id, "decisionVar"), @is(RESULT_OF_VERSION_TWO));
	  }

	  public virtual void testEvaluateDecisionWithLatestBindingSameVersion()
	  {
		deploymentForTenant(TENANT_ONE, CMMN_LATEST, DMN_FILE);
		deploymentForTenant(TENANT_TWO, CMMN_LATEST, DMN_FILE_VERSION_TWO);

		CaseInstance caseInstanceOne = createCaseInstance(CASE_DEFINITION_KEY, TENANT_ONE);
		CaseInstance caseInstanceTwo = createCaseInstance(CASE_DEFINITION_KEY, TENANT_TWO);

		assertThat((string)caseService.getVariable(caseInstanceOne.Id, "decisionVar"), @is(RESULT_OF_VERSION_ONE));
		assertThat((string)caseService.getVariable(caseInstanceTwo.Id, "decisionVar"), @is(RESULT_OF_VERSION_TWO));
	  }

	  public virtual void testEvaluateDecisionWithLatestBindingDifferentVersions()
	  {
		deploymentForTenant(TENANT_ONE, CMMN_LATEST, DMN_FILE);

		deploymentForTenant(TENANT_TWO, CMMN_LATEST, DMN_FILE);
		deploymentForTenant(TENANT_TWO, CMMN_LATEST, DMN_FILE_VERSION_TWO);

		CaseInstance caseInstanceOne = createCaseInstance(CASE_DEFINITION_KEY, TENANT_ONE);
		CaseInstance caseInstanceTwo = createCaseInstance(CASE_DEFINITION_KEY, TENANT_TWO);

		assertThat((string)caseService.getVariable(caseInstanceOne.Id, "decisionVar"), @is(RESULT_OF_VERSION_ONE));
		assertThat((string)caseService.getVariable(caseInstanceTwo.Id, "decisionVar"), @is(RESULT_OF_VERSION_TWO));
	  }

	  public virtual void testEvaluateDecisionWithVersionBinding()
	  {
		deploymentForTenant(TENANT_ONE, CMMN_VERSION, DMN_FILE);
		deploymentForTenant(TENANT_ONE, DMN_FILE_VERSION_TWO);

		deploymentForTenant(TENANT_TWO, CMMN_VERSION, DMN_FILE_VERSION_TWO);
		deploymentForTenant(TENANT_TWO, DMN_FILE);

		CaseInstance caseInstanceOne = createCaseInstance(CASE_DEFINITION_KEY, TENANT_ONE);
		CaseInstance caseInstanceTwo = createCaseInstance(CASE_DEFINITION_KEY, TENANT_TWO);

		assertThat((string)caseService.getVariable(caseInstanceOne.Id, "decisionVar"), @is(RESULT_OF_VERSION_ONE));
		assertThat((string)caseService.getVariable(caseInstanceTwo.Id, "decisionVar"), @is(RESULT_OF_VERSION_TWO));
	  }

	  public virtual void testFailEvaluateDecisionFromOtherTenantWithDeploymentBinding()
	  {
		deploymentForTenant(TENANT_ONE, CMMN_DEPLOYMENT);
		deploymentForTenant(TENANT_TWO, DMN_FILE);

		try
		{
		  createCaseInstance(CASE_DEFINITION_KEY, TENANT_ONE);

		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("no decision definition deployed with key = 'decision'"));
		}
	  }

	  public virtual void testFailEvaluateDecisionFromOtherTenantWithLatestBinding()
	  {
		deploymentForTenant(TENANT_ONE, CMMN_LATEST);
		deploymentForTenant(TENANT_TWO, DMN_FILE);

		try
		{
		  createCaseInstance(CASE_DEFINITION_KEY, TENANT_ONE);

		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("no decision definition deployed with key 'decision'"));
		}
	  }

	  public virtual void testFailEvaluateDecisionFromOtherTenantWithVersionBinding()
	  {
		deploymentForTenant(TENANT_ONE, CMMN_VERSION_2, DMN_FILE);

		deploymentForTenant(TENANT_TWO, DMN_FILE);
		deploymentForTenant(TENANT_TWO, DMN_FILE);

		try
		{
		  createCaseInstance(CASE_DEFINITION_KEY, TENANT_ONE);

		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("no decision definition deployed with key = 'decision', version = '2' and tenant-id 'tenant1'"));
		}
	  }

	  public virtual void testEvaluateDecisionRefTenantIdConstant()
	  {
		deployment(CMMN_CONST);
		deploymentForTenant(TENANT_ONE, DMN_FILE);
		deploymentForTenant(TENANT_TWO, DMN_FILE_VERSION_TWO);

		CaseInstance caseInstance = createCaseInstance(CASE_DEFINITION_KEY);

		assertThat((string)caseService.getVariable(caseInstance.Id, "decisionVar"), @is(RESULT_OF_VERSION_ONE));
	  }

	  public virtual void testEvaluateDecisionRefWithoutTenantIdConstant()
	  {
		deploymentForTenant(TENANT_ONE, CMMN_WITHOUT_TENANT);
		deployment(DMN_FILE);
		deploymentForTenant(TENANT_TWO, DMN_FILE_VERSION_TWO);

		CaseInstance caseInstance = createCaseInstance(CASE_DEFINITION_KEY);

		assertThat((string)caseService.getVariable(caseInstance.Id, "decisionVar"), @is(RESULT_OF_VERSION_ONE));
	  }

	  public virtual void testEvaluateDecisionRefTenantIdExpression()
	  {
		deployment(CMMN_EXPR);
		deploymentForTenant(TENANT_ONE, DMN_FILE);
		deploymentForTenant(TENANT_TWO, DMN_FILE_VERSION_TWO);

		CaseInstance caseInstance = createCaseInstance(CASE_DEFINITION_KEY);

		assertThat((string)caseService.getVariable(caseInstance.Id, "decisionVar"), @is(RESULT_OF_VERSION_ONE));
	  }

	  protected internal virtual CaseInstance createCaseInstance(string caseDefinitionKey, string tenantId)
	  {
		CaseInstance caseInstance = caseService.withCaseDefinitionByKey(caseDefinitionKey).caseDefinitionTenantId(tenantId).create();

		CaseExecution caseExecution = caseService.createCaseExecutionQuery().activityId(DECISION_TASK_ID).tenantIdIn(tenantId).singleResult();
		caseService.withCaseExecution(caseExecution.Id).setVariable("status", "gold").manualStart();
		return caseInstance;
	  }

	  protected internal virtual CaseInstance createCaseInstance(string caseDefinitionKey)
	  {
		CaseInstance caseInstance = caseService.withCaseDefinitionByKey(caseDefinitionKey).create();

		CaseExecution caseExecution = caseService.createCaseExecutionQuery().activityId(DECISION_TASK_ID).singleResult();
		caseService.withCaseExecution(caseExecution.Id).setVariable("status", "gold").manualStart();
		return caseInstance;
	  }

	}

}