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
//	import static org.hamcrest.CoreMatchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using DmnDecisionTableResult = org.camunda.bpm.dmn.engine.DmnDecisionTableResult;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;

	public class MultiTenancyDecisionTableEvaluationTest : PluggableProcessEngineTestCase
	{

	  protected internal const string DMN_FILE = "org/camunda/bpm/engine/test/api/dmn/Example.dmn";
	  protected internal const string DMN_FILE_SECOND_VERSION = "org/camunda/bpm/engine/test/api/dmn/Example_v2.dmn";

	  protected internal const string DECISION_DEFINITION_KEY = "decision";

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal const string RESULT_OF_FIRST_VERSION = "ok";
	  protected internal const string RESULT_OF_SECOND_VERSION = "notok";

	  public virtual void testFailToEvaluateDecisionByIdWithoutTenantId()
	  {
		deployment(DMN_FILE);

		DecisionDefinition decisionDefinition = repositoryService.createDecisionDefinitionQuery().singleResult();

		try
		{
		  decisionService.evaluateDecisionTableById(decisionDefinition.Id).variables(createVariables()).decisionDefinitionWithoutTenantId().evaluate();
		  fail("BadUserRequestException exception");
		}
		catch (BadUserRequestException e)
		{
		  assertThat(e.Message, containsString("Cannot specify a tenant-id"));
		}
	  }

	  public virtual void testFailToEvaluateDecisionByIdWithTenantId()
	  {
		deploymentForTenant(TENANT_ONE, DMN_FILE);

		DecisionDefinition decisionDefinition = repositoryService.createDecisionDefinitionQuery().singleResult();

		try
		{
		  decisionService.evaluateDecisionTableById(decisionDefinition.Id).variables(createVariables()).decisionDefinitionTenantId(TENANT_ONE).evaluate();
		  fail("BadUserRequestException exception");
		}
		catch (BadUserRequestException e)
		{
		  assertThat(e.Message, containsString("Cannot specify a tenant-id"));
		}
	  }

	  public virtual void testFailToEvaluateDecisionByKeyForNonExistingTenantID()
	  {
		deploymentForTenant(TENANT_ONE, DMN_FILE);
		deploymentForTenant(TENANT_TWO, DMN_FILE);

		try
		{
		  decisionService.evaluateDecisionTableByKey(DECISION_DEFINITION_KEY).variables(createVariables()).decisionDefinitionTenantId("nonExistingTenantId").evaluate();
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("no decision definition deployed with key 'decision' and tenant-id 'nonExistingTenantId'"));
		}
	  }

	  public virtual void testFailToEvaluateDecisionByKeyForMultipleTenants()
	  {
		deploymentForTenant(TENANT_ONE, DMN_FILE);
		deploymentForTenant(TENANT_TWO, DMN_FILE);

		try
		{
		  decisionService.evaluateDecisionTableByKey(DECISION_DEFINITION_KEY).variables(createVariables()).evaluate();
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("multiple tenants."));
		}
	  }

	  public virtual void testEvaluateDecisionByKeyWithoutTenantId()
	  {
		deployment(DMN_FILE);

		DmnDecisionTableResult decisionResult = decisionService.evaluateDecisionTableByKey(DECISION_DEFINITION_KEY).variables(createVariables()).decisionDefinitionWithoutTenantId().evaluate();

		assertThatDecisionHasResult(decisionResult, RESULT_OF_FIRST_VERSION);
	  }

	  public virtual void testEvaluateDecisionByKeyForAnyTenants()
	  {
		deploymentForTenant(TENANT_ONE, DMN_FILE);

		DmnDecisionTableResult decisionResult = decisionService.evaluateDecisionTableByKey(DECISION_DEFINITION_KEY).variables(createVariables()).evaluate();

		assertThatDecisionHasResult(decisionResult, RESULT_OF_FIRST_VERSION);
	  }


	  public virtual void testEvaluateDecisionByKeyAndTenantId()
	  {
		deploymentForTenant(TENANT_ONE, DMN_FILE);
		deploymentForTenant(TENANT_TWO, DMN_FILE_SECOND_VERSION);

		DmnDecisionTableResult decisionResult = decisionService.evaluateDecisionTableByKey(DECISION_DEFINITION_KEY).variables(createVariables()).decisionDefinitionTenantId(TENANT_ONE).evaluate();

		assertThatDecisionHasResult(decisionResult, RESULT_OF_FIRST_VERSION);
	  }

	  public virtual void testEvaluateDecisionByKeyLatestVersionAndTenantId()
	  {
		deploymentForTenant(TENANT_ONE, DMN_FILE);
		deploymentForTenant(TENANT_ONE, DMN_FILE_SECOND_VERSION);

		DmnDecisionTableResult decisionResult = decisionService.evaluateDecisionTableByKey(DECISION_DEFINITION_KEY).variables(createVariables()).decisionDefinitionTenantId(TENANT_ONE).evaluate();

		assertThatDecisionHasResult(decisionResult, RESULT_OF_SECOND_VERSION);
	  }

	  public virtual void testEvaluateDecisionByKeyVersionAndTenantId()
	  {
		deploymentForTenant(TENANT_ONE, DMN_FILE);

		deploymentForTenant(TENANT_TWO, DMN_FILE);
		deploymentForTenant(TENANT_TWO, DMN_FILE_SECOND_VERSION);

		DmnDecisionTableResult decisionResult = decisionService.evaluateDecisionTableByKey(DECISION_DEFINITION_KEY).variables(createVariables()).version(1).decisionDefinitionTenantId(TENANT_TWO).evaluate();

		assertThatDecisionHasResult(decisionResult, RESULT_OF_FIRST_VERSION);
	  }

	  public virtual void testEvaluateDecisionByKeyWithoutTenantIdNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		deployment(DMN_FILE);

		DmnDecisionTableResult decisionResult = decisionService.evaluateDecisionTableByKey(DECISION_DEFINITION_KEY).decisionDefinitionWithoutTenantId().variables(createVariables()).evaluate();

		assertThatDecisionHasResult(decisionResult, RESULT_OF_FIRST_VERSION);
	  }

	  public virtual void testFailToEvaluateDecisionByKeyNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		deploymentForTenant(TENANT_ONE, DMN_FILE);

		try
		{
		  decisionService.evaluateDecisionTableByKey(DECISION_DEFINITION_KEY).variables(createVariables()).evaluate();

		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("no decision definition deployed with key 'decision'"));
		}
	  }

	  public virtual void testFailToEvaluateDecisionByKeyWithTenantIdNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		deploymentForTenant(TENANT_ONE, DMN_FILE);

		try
		{
		  decisionService.evaluateDecisionTableByKey(DECISION_DEFINITION_KEY).decisionDefinitionTenantId(TENANT_ONE).variables(createVariables()).evaluate();

		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("Cannot evaluate the decision"));
		}
	  }

	  public virtual void testFailToEvaluateDecisionByIdNoAuthenticatedTenants()
	  {
		deploymentForTenant(TENANT_ONE, DMN_FILE);

		DecisionDefinition decisionDefinition = repositoryService.createDecisionDefinitionQuery().singleResult();

		identityService.setAuthentication("user", null, null);

		try
		{
		  decisionService.evaluateDecisionTableById(decisionDefinition.Id).variables(createVariables()).evaluate();

		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("Cannot evaluate the decision"));
		}
	  }

	  public virtual void testEvaluateDecisionByKeyWithTenantIdAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		deploymentForTenant(TENANT_ONE, DMN_FILE);
		deploymentForTenant(TENANT_TWO, DMN_FILE);

		DmnDecisionTableResult decisionResult = decisionService.evaluateDecisionTableByKey(DECISION_DEFINITION_KEY).decisionDefinitionTenantId(TENANT_ONE).variables(createVariables()).evaluate();

		assertThatDecisionHasResult(decisionResult, RESULT_OF_FIRST_VERSION);
	  }

	  public virtual void testEvaluateDecisionByIdAuthenticatedTenant()
	  {
		deploymentForTenant(TENANT_ONE, DMN_FILE);

		DecisionDefinition decisionDefinition = repositoryService.createDecisionDefinitionQuery().singleResult();

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		DmnDecisionTableResult decisionResult = decisionService.evaluateDecisionTableById(decisionDefinition.Id).variables(createVariables()).evaluate();

		assertThatDecisionHasResult(decisionResult, RESULT_OF_FIRST_VERSION);
	  }

	  public virtual void testEvaluateDecisionByKeyWithAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		deploymentForTenant(TENANT_ONE, DMN_FILE);
		deploymentForTenant(TENANT_TWO, DMN_FILE_SECOND_VERSION);

		DmnDecisionTableResult decisionResult = decisionService.evaluateDecisionTableByKey(DECISION_DEFINITION_KEY).variables(createVariables()).evaluate();

		assertThatDecisionHasResult(decisionResult, RESULT_OF_FIRST_VERSION);
	  }

	  public virtual void testEvaluateDecisionByKeyWithTenantIdDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		deploymentForTenant(TENANT_ONE, DMN_FILE);

		DmnDecisionTableResult decisionResult = decisionService.evaluateDecisionTableByKey(DECISION_DEFINITION_KEY).decisionDefinitionTenantId(TENANT_ONE).variables(createVariables()).evaluate();

		assertThatDecisionHasResult(decisionResult, RESULT_OF_FIRST_VERSION);
	  }

	  protected internal virtual VariableMap createVariables()
	  {
		return Variables.createVariables().putValue("status", "silver").putValue("sum", 723);
	  }

	  protected internal virtual void assertThatDecisionHasResult(DmnDecisionTableResult decisionResult, object expectedValue)
	  {
		assertThat(decisionResult, @is(notNullValue()));
		assertThat(decisionResult.size(), @is(1));
		string value = decisionResult.SingleResult.FirstEntry;
		assertThat(value, @is(expectedValue));
	  }

	}

}