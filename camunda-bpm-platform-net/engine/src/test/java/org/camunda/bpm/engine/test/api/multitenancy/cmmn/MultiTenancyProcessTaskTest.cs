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
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	public class MultiTenancyProcessTaskTest : PluggableProcessEngineTestCase
	{

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal const string CMMN_LATEST = "org/camunda/bpm/engine/test/api/multitenancy/CaseWithProcessTask.cmmn";
	  protected internal const string CMMN_LATEST_WITH_MANUAL_ACTIVATION = "org/camunda/bpm/engine/test/api/multitenancy/CaseWithProcessTaskWithManualActivation.cmmn";
	  protected internal const string CMMN_DEPLOYMENT = "org/camunda/bpm/engine/test/api/multitenancy/CaseWithProcessTaskDeploymentBinding.cmmn";
	  protected internal const string CMMN_VERSION = "org/camunda/bpm/engine/test/api/multitenancy/CaseWithProcessTaskVersionBinding.cmmn";
	  protected internal const string CMMN_VERSION_2 = "org/camunda/bpm/engine/test/api/multitenancy/CaseWithProcessTaskVersionBinding_v2.cmmn";

	  protected internal const string CMMN_TENANT_CONST = "org/camunda/bpm/engine/test/api/multitenancy/CaseWithProcessTaskTenantIdConst.cmmn";
	  protected internal const string CMMN_TENANT_EXPR = "org/camunda/bpm/engine/test/api/multitenancy/CaseWithProcessTaskTenantIdExpr.cmmn";

	  protected internal const string PROCESS_TASK_ID = "PI_ProcessTask_1";

	  protected internal static readonly BpmnModelInstance PROCESS = Bpmn.createExecutableProcess("testProcess").startEvent().userTask().endEvent().done();

	  public virtual void testStartProcessInstanceWithDeploymentBinding()
	  {

		deploymentForTenant(TENANT_ONE, CMMN_DEPLOYMENT, PROCESS);
		deploymentForTenant(TENANT_TWO, CMMN_DEPLOYMENT, PROCESS);

		createCaseInstance("testCaseDeployment", TENANT_ONE);
		createCaseInstance("testCaseDeployment", TENANT_TWO);

		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().processDefinitionKey("testProcess");
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(1L));
	  }

	  public virtual void testStartProcessInstanceWithLatestBindingSameVersion()
	  {

		deploymentForTenant(TENANT_ONE, CMMN_LATEST_WITH_MANUAL_ACTIVATION, PROCESS);
		deploymentForTenant(TENANT_TWO, CMMN_LATEST_WITH_MANUAL_ACTIVATION, PROCESS);

		createCaseInstance("testCase", TENANT_ONE);
		createCaseInstance("testCase", TENANT_TWO);

		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().processDefinitionKey("testProcess");
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(1L));
	  }

	  public virtual void testStartProcessInstanceWithLatestBindingDifferentVersion()
	  {

		deploymentForTenant(TENANT_ONE, CMMN_LATEST_WITH_MANUAL_ACTIVATION, PROCESS);

		deploymentForTenant(TENANT_TWO, CMMN_LATEST_WITH_MANUAL_ACTIVATION, PROCESS);
		deploymentForTenant(TENANT_TWO, PROCESS);

		createCaseInstance("testCase", TENANT_ONE);
		createCaseInstance("testCase", TENANT_TWO);

		ProcessDefinition latestProcessTenantTwo = repositoryService.createProcessDefinitionQuery().tenantIdIn(TENANT_TWO).processDefinitionKey("testProcess").latestVersion().singleResult();

		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().processDefinitionKey("testProcess");
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).processDefinitionId(latestProcessTenantTwo.Id).count(), @is(1L));
	  }

	  public virtual void testStartProcessInstanceWithVersionBinding()
	  {

		deploymentForTenant(TENANT_ONE, CMMN_VERSION, PROCESS);
		deploymentForTenant(TENANT_TWO, CMMN_VERSION, PROCESS);

		createCaseInstance("testCaseVersion", TENANT_ONE);
		createCaseInstance("testCaseVersion", TENANT_TWO);

		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().processDefinitionKey("testProcess");
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(1L));
	  }

	  public virtual void testFailStartProcessInstanceFromOtherTenantWithDeploymentBinding()
	  {

		deploymentForTenant(TENANT_ONE, CMMN_DEPLOYMENT);
		deploymentForTenant(TENANT_TWO, PROCESS);

		try
		{
		  createCaseInstance("testCaseDeployment", TENANT_ONE);

		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("no processes deployed with key = 'testProcess'"));
		}
	  }

	  public virtual void testFailStartProcessInstanceFromOtherTenantWithLatestBinding()
	  {

		deploymentForTenant(TENANT_ONE, CMMN_LATEST);
		deploymentForTenant(TENANT_TWO, PROCESS);

		try
		{
		  createCaseInstance("testCase", TENANT_ONE);

		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("no processes deployed with key 'testProcess'"));
		}
	  }

	  public virtual void testFailStartProcessInstanceFromOtherTenantWithVersionBinding()
	  {

		deploymentForTenant(TENANT_ONE, CMMN_VERSION_2, PROCESS);

		deploymentForTenant(TENANT_TWO, PROCESS);
		deploymentForTenant(TENANT_TWO, PROCESS);

		try
		{
		  createCaseInstance("testCaseVersion", TENANT_ONE);

		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("no processes deployed with key = 'testProcess'"));
		}
	  }

	  public virtual void testProcessRefTenantIdConstant()
	  {
		deployment(CMMN_TENANT_CONST);
		deploymentForTenant(TENANT_ONE, PROCESS);

		caseService.withCaseDefinitionByKey("testCase").create();

		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().processDefinitionKey("testProcess");
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

	  public virtual void testProcessRefTenantIdExpression()
	  {
		deployment(CMMN_TENANT_EXPR);
		deploymentForTenant(TENANT_ONE, PROCESS);

		caseService.withCaseDefinitionByKey("testCase").create();

		CaseExecution caseExecution = caseService.createCaseExecutionQuery().activityId(PROCESS_TASK_ID).singleResult();
		caseService.withCaseExecution(caseExecution.Id).manualStart();

		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().processDefinitionKey("testProcess");
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

	  protected internal virtual void createCaseInstance(string caseDefinitionKey, string tenantId)
	  {
		caseService.withCaseDefinitionByKey(caseDefinitionKey).caseDefinitionTenantId(tenantId).create();

		CaseExecution caseExecution = caseService.createCaseExecutionQuery().activityId(PROCESS_TASK_ID).tenantIdIn(tenantId).singleResult();
		caseService.withCaseExecution(caseExecution.Id).manualStart();
	  }

	}

}