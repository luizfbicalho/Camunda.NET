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
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using CaseInstanceQuery = org.camunda.bpm.engine.runtime.CaseInstanceQuery;

	public class MultiTenancyCreateCaseInstanceTest : PluggableProcessEngineTestCase
	{

	  protected internal const string CMMN_FILE = "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn";

	  protected internal const string CASE_DEFINITION_KEY = "oneTaskCase";

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  public virtual void testFailToCreateCaseInstanceByIdWithoutTenantId()
	  {
		deployment(CMMN_FILE);

		CaseDefinition caseDefinition = repositoryService.createCaseDefinitionQuery().singleResult();

		try
		{
		  caseService.withCaseDefinition(caseDefinition.Id).caseDefinitionWithoutTenantId().create();
		  fail("BadUserRequestException exception");
		}
		catch (BadUserRequestException e)
		{
		  assertThat(e.Message, containsString("Cannot specify a tenant-id"));
		}
	  }

	  public virtual void testFailToCreateCaseInstanceByIdWithTenantId()
	  {
		deploymentForTenant(TENANT_ONE, CMMN_FILE);

		CaseDefinition caseDefinition = repositoryService.createCaseDefinitionQuery().singleResult();

		try
		{
		  caseService.withCaseDefinition(caseDefinition.Id).caseDefinitionTenantId(TENANT_ONE).create();
		  fail("BadUserRequestException exception");
		}
		catch (BadUserRequestException e)
		{
		  assertThat(e.Message, containsString("Cannot specify a tenant-id"));
		}
	  }

	  public virtual void testFailToCreateCaseInstanceByKeyForNonExistingTenantID()
	  {
		deploymentForTenant(TENANT_ONE, CMMN_FILE);
		deploymentForTenant(TENANT_TWO, CMMN_FILE);

		try
		{
		  caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).caseDefinitionTenantId("nonExistingTenantId").create();
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("no case definition deployed with key 'oneTaskCase' and tenant-id 'nonExistingTenantId'"));
		}
	  }

	  public virtual void testFailToCreateCaseInstanceByKeyForMultipleTenants()
	  {
		deploymentForTenant(TENANT_ONE, CMMN_FILE);
		deploymentForTenant(TENANT_TWO, CMMN_FILE);

		try
		{
		  caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).create();
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("multiple tenants."));
		}
	  }

	  public virtual void testCreateCaseInstanceByKeyWithoutTenantId()
	  {
		deployment(CMMN_FILE);

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).caseDefinitionWithoutTenantId().create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();
		assertThat(query.count(), @is(1L));
		assertThat(query.singleResult().TenantId, @is(nullValue()));

	  }

	  public virtual void testCreateCaseInstanceByKeyForAnyTenants()
	  {
		deploymentForTenant(TENANT_ONE, CMMN_FILE);

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).create();

		assertThat(caseService.createCaseInstanceQuery().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

	  public virtual void testCreateCaseInstanceByKeyAndTenantId()
	  {
		deploymentForTenant(TENANT_ONE, CMMN_FILE);
		deploymentForTenant(TENANT_TWO, CMMN_FILE);

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).caseDefinitionTenantId(TENANT_ONE).create();

		assertThat(caseService.createCaseInstanceQuery().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

	  public virtual void testCreateCaseInstanceByKeyWithoutTenantIdNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		deployment(CMMN_FILE);

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).caseDefinitionWithoutTenantId().create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();
		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testFailToCreateCaseInstanceByKeyNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		deploymentForTenant(TENANT_ONE, CMMN_FILE);

		try
		{
		  caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).create();

		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("no case definition deployed with key 'oneTaskCase'"));
		}
	  }

	  public virtual void testFailToCreateCaseInstanceByKeyWithTenantIdNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		deploymentForTenant(TENANT_ONE, CMMN_FILE);

		try
		{
		  caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).caseDefinitionTenantId(TENANT_ONE).create();

		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("Cannot create an instance of the case definition"));
		}
	  }

	  public virtual void testFailToCreateCaseInstanceByIdNoAuthenticatedTenants()
	  {
		deploymentForTenant(TENANT_ONE, CMMN_FILE);

		CaseDefinition caseDefinition = repositoryService.createCaseDefinitionQuery().singleResult();

		identityService.setAuthentication("user", null, null);

		try
		{
		  caseService.withCaseDefinition(caseDefinition.Id).create();

		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("Cannot create an instance of the case definition"));
		}
	  }

	  public virtual void testCreateCaseInstanceByKeyWithTenantIdAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		deploymentForTenant(TENANT_ONE, CMMN_FILE);
		deploymentForTenant(TENANT_TWO, CMMN_FILE);

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).caseDefinitionTenantId(TENANT_ONE).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();
		assertThat(query.count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

	  public virtual void testCreateCaseInstanceByIdAuthenticatedTenant()
	  {
		deploymentForTenant(TENANT_ONE, CMMN_FILE);

		CaseDefinition caseDefinition = repositoryService.createCaseDefinitionQuery().singleResult();

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		caseService.withCaseDefinition(caseDefinition.Id).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();
		assertThat(query.count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

	  public virtual void testCreateCaseInstanceByKeyWithAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		deploymentForTenant(TENANT_ONE, CMMN_FILE);
		deploymentForTenant(TENANT_TWO, CMMN_FILE);

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();
		assertThat(query.count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

	  public virtual void testCreateCaseInstanceByKeyWithTenantIdDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		deploymentForTenant(TENANT_ONE, CMMN_FILE);

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).caseDefinitionTenantId(TENANT_ONE).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();
		assertThat(query.count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

	}

}