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
//	import static org.junit.Assert.assertThat;

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	public class MultiTenancyBusinessRuleTaskTest : PluggableProcessEngineTestCase
	{

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal const string DMN_FILE = "org/camunda/bpm/engine/test/api/multitenancy/simpleDecisionTable.dmn";
	  protected internal const string DMN_FILE_VERSION_TWO = "org/camunda/bpm/engine/test/api/multitenancy/simpleDecisionTable_v2.dmn";

	  protected internal const string RESULT_OF_VERSION_ONE = "A";
	  protected internal const string RESULT_OF_VERSION_TWO = "C";

	  public const string DMN_FILE_VERSION_TAG = "org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionVersionTagOkay.dmn11.xml";
	  public const string DMN_FILE_VERSION_TAG_TWO = "org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionVersionTagOkay_v2.dmn11.xml";

	  protected internal const string RESULT_OF_VERSION_TAG_ONE = "A";
	  protected internal const string RESULT_OF_VERSION_TAG_TWO = "C";

	  public virtual void testEvaluateDecisionWithDeploymentBinding()
	  {

		BpmnModelInstance process = Bpmn.createExecutableProcess("process").startEvent().businessRuleTask().camundaDecisionRef("decision").camundaDecisionRefBinding("deployment").camundaMapDecisionResult("singleEntry").camundaResultVariable("decisionVar").camundaAsyncAfter().endEvent().done();

		deploymentForTenant(TENANT_ONE, DMN_FILE, process);
		deploymentForTenant(TENANT_TWO, DMN_FILE_VERSION_TWO, process);

		ProcessInstance processInstanceOne = runtimeService.createProcessInstanceByKey("process").setVariable("status", "gold").processDefinitionTenantId(TENANT_ONE).execute();

		ProcessInstance processInstanceTwo = runtimeService.createProcessInstanceByKey("process").setVariable("status", "gold").processDefinitionTenantId(TENANT_TWO).execute();

		assertThat((string)runtimeService.getVariable(processInstanceOne.Id, "decisionVar"), @is(RESULT_OF_VERSION_ONE));
		assertThat((string)runtimeService.getVariable(processInstanceTwo.Id, "decisionVar"), @is(RESULT_OF_VERSION_TWO));
	  }

	  public virtual void testEvaluateDecisionWithLatestBindingSameVersion()
	  {

		BpmnModelInstance process = Bpmn.createExecutableProcess("process").startEvent().businessRuleTask().camundaDecisionRef("decision").camundaDecisionRefBinding("latest").camundaMapDecisionResult("singleEntry").camundaResultVariable("decisionVar").camundaAsyncAfter().endEvent().done();

		deploymentForTenant(TENANT_ONE, DMN_FILE, process);
		deploymentForTenant(TENANT_TWO, DMN_FILE_VERSION_TWO, process);

		ProcessInstance processInstanceOne = runtimeService.createProcessInstanceByKey("process").setVariable("status", "gold").processDefinitionTenantId(TENANT_ONE).execute();

		ProcessInstance processInstanceTwo = runtimeService.createProcessInstanceByKey("process").setVariable("status", "gold").processDefinitionTenantId(TENANT_TWO).execute();

		assertThat((string)runtimeService.getVariable(processInstanceOne.Id, "decisionVar"), @is(RESULT_OF_VERSION_ONE));
		assertThat((string)runtimeService.getVariable(processInstanceTwo.Id, "decisionVar"), @is(RESULT_OF_VERSION_TWO));
	  }

	  public virtual void testEvaluateDecisionWithLatestBindingDifferentVersions()
	  {

		BpmnModelInstance process = Bpmn.createExecutableProcess("process").startEvent().businessRuleTask().camundaDecisionRef("decision").camundaDecisionRefBinding("latest").camundaMapDecisionResult("singleEntry").camundaResultVariable("decisionVar").camundaAsyncAfter().endEvent().done();

		deploymentForTenant(TENANT_ONE, DMN_FILE, process);

		deploymentForTenant(TENANT_TWO, DMN_FILE, process);
		deploymentForTenant(TENANT_TWO, DMN_FILE_VERSION_TWO);

		ProcessInstance processInstanceOne = runtimeService.createProcessInstanceByKey("process").setVariable("status", "gold").processDefinitionTenantId(TENANT_ONE).execute();

		ProcessInstance processInstanceTwo = runtimeService.createProcessInstanceByKey("process").setVariable("status", "gold").processDefinitionTenantId(TENANT_TWO).execute();

		assertThat((string)runtimeService.getVariable(processInstanceOne.Id, "decisionVar"), @is(RESULT_OF_VERSION_ONE));
		assertThat((string)runtimeService.getVariable(processInstanceTwo.Id, "decisionVar"), @is(RESULT_OF_VERSION_TWO));
	  }

	  public virtual void testEvaluateDecisionWithVersionBinding()
	  {

		BpmnModelInstance process = Bpmn.createExecutableProcess("process").startEvent().businessRuleTask().camundaDecisionRef("decision").camundaDecisionRefBinding("version").camundaDecisionRefVersion("1").camundaMapDecisionResult("singleEntry").camundaResultVariable("decisionVar").camundaAsyncAfter().endEvent().done();

		deploymentForTenant(TENANT_ONE, DMN_FILE, process);
		deploymentForTenant(TENANT_ONE, DMN_FILE_VERSION_TWO);

		deploymentForTenant(TENANT_TWO, DMN_FILE_VERSION_TWO, process);
		deploymentForTenant(TENANT_TWO, DMN_FILE);

		ProcessInstance processInstanceOne = runtimeService.createProcessInstanceByKey("process").setVariable("status", "gold").processDefinitionTenantId(TENANT_ONE).execute();

		ProcessInstance processInstanceTwo = runtimeService.createProcessInstanceByKey("process").setVariable("status", "gold").processDefinitionTenantId(TENANT_TWO).execute();

		assertThat((string)runtimeService.getVariable(processInstanceOne.Id, "decisionVar"), @is(RESULT_OF_VERSION_ONE));
		assertThat((string)runtimeService.getVariable(processInstanceTwo.Id, "decisionVar"), @is(RESULT_OF_VERSION_TWO));
	  }

	  public virtual void testEvaluateDecisionWithVersionTagBinding()
	  {
		// given
		deploymentForTenant(TENANT_ONE, DMN_FILE_VERSION_TAG);
		deployment(Bpmn.createExecutableProcess("process").startEvent().businessRuleTask().camundaDecisionRef("decision").camundaDecisionRefTenantId(TENANT_ONE).camundaDecisionRefBinding("versionTag").camundaDecisionRefVersionTag("0.0.2").camundaMapDecisionResult("singleEntry").camundaResultVariable("decisionVar").endEvent().camundaAsyncBefore().done());

		// when
		ProcessInstance processInstance = runtimeService.createProcessInstanceByKey("process").setVariable("status", "gold").execute();

		// then
		assertThat((string)runtimeService.getVariable(processInstance.Id, "decisionVar"), @is(RESULT_OF_VERSION_TAG_ONE));
	  }

	  public virtual void testEvaluateDecisionWithVersionTagBinding_ResolveTenantFromDefinition()
	  {
		// given
		BpmnModelInstance process = Bpmn.createExecutableProcess("process").startEvent().businessRuleTask().camundaDecisionRef("decision").camundaDecisionRefBinding("versionTag").camundaDecisionRefVersionTag("0.0.2").camundaMapDecisionResult("singleEntry").camundaResultVariable("decisionVar").endEvent().camundaAsyncBefore().done();

		deploymentForTenant(TENANT_ONE, DMN_FILE_VERSION_TAG, process);
		deploymentForTenant(TENANT_TWO, DMN_FILE_VERSION_TAG_TWO, process);

		ProcessInstance processInstanceOne = runtimeService.createProcessInstanceByKey("process").setVariable("status", "gold").processDefinitionTenantId(TENANT_ONE).execute();

		ProcessInstance processInstanceTwo = runtimeService.createProcessInstanceByKey("process").setVariable("status", "gold").processDefinitionTenantId(TENANT_TWO).execute();

		assertThat((string)runtimeService.getVariable(processInstanceOne.Id, "decisionVar"), @is(RESULT_OF_VERSION_TAG_ONE));
		assertThat((string)runtimeService.getVariable(processInstanceTwo.Id, "decisionVar"), @is(RESULT_OF_VERSION_TAG_TWO));
	  }

	  public virtual void testFailEvaluateDecisionFromOtherTenantWithDeploymentBinding()
	  {

		BpmnModelInstance process = Bpmn.createExecutableProcess("process").startEvent().businessRuleTask().camundaDecisionRef("decision").camundaDecisionRefBinding("deployment").camundaAsyncAfter().endEvent().done();

		deploymentForTenant(TENANT_ONE, process);
		deploymentForTenant(TENANT_TWO, DMN_FILE);

		try
		{
		  runtimeService.createProcessInstanceByKey("process").processDefinitionTenantId(TENANT_ONE).execute();

		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("no decision definition deployed with key = 'decision'"));
		}
	  }

	  public virtual void testFailEvaluateDecisionFromOtherTenantWithLatestBinding()
	  {

		BpmnModelInstance process = Bpmn.createExecutableProcess("process").startEvent().businessRuleTask().camundaDecisionRef("decision").camundaDecisionRefBinding("latest").camundaAsyncAfter().endEvent().done();

		deploymentForTenant(TENANT_ONE, process);
		deploymentForTenant(TENANT_TWO, DMN_FILE);

		try
		{
		  runtimeService.createProcessInstanceByKey("process").processDefinitionTenantId(TENANT_ONE).execute();

		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("no decision definition deployed with key 'decision'"));
		}
	  }

	  public virtual void testFailEvaluateDecisionFromOtherTenantWithVersionBinding()
	  {

		BpmnModelInstance process = Bpmn.createExecutableProcess("process").startEvent().businessRuleTask().camundaDecisionRef("decision").camundaDecisionRefBinding("version").camundaDecisionRefVersion("2").camundaAsyncAfter().endEvent().done();

		deploymentForTenant(TENANT_ONE, DMN_FILE, process);

		deploymentForTenant(TENANT_TWO, DMN_FILE);
		deploymentForTenant(TENANT_TWO, DMN_FILE);

		try
		{
		  runtimeService.createProcessInstanceByKey("process").processDefinitionTenantId(TENANT_ONE).execute();

		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("no decision definition deployed with key = 'decision', version = '2' and tenant-id 'tenant1'"));
		}
	  }

	  public virtual void testFailEvaluateDecisionFromOtherTenantWithVersionTagBinding()
	  {
		// given
		BpmnModelInstance process = Bpmn.createExecutableProcess("process").startEvent().businessRuleTask().camundaDecisionRef("decision").camundaDecisionRefBinding("versionTag").camundaDecisionRefVersionTag("0.0.2").camundaMapDecisionResult("singleEntry").camundaResultVariable("result").camundaAsyncAfter().endEvent().done();

		deploymentForTenant(TENANT_ONE, process);

		deploymentForTenant(TENANT_TWO, DMN_FILE_VERSION_TAG);

		try
		{
		  // when
		  runtimeService.createProcessInstanceByKey("process").processDefinitionTenantId(TENANT_ONE).execute();

		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  // then
		  assertThat(e.Message, containsString("no decision definition deployed with key = 'decision', versionTag = '0.0.2' and tenant-id 'tenant1': decisionDefinition is null"));
		}
	  }

	  public virtual void testEvaluateDecisionTenantIdConstant()
	  {

		BpmnModelInstance process = Bpmn.createExecutableProcess("process").startEvent().businessRuleTask().camundaDecisionRef("decision").camundaDecisionRefBinding("latest").camundaDecisionRefTenantId(TENANT_ONE).camundaMapDecisionResult("singleEntry").camundaResultVariable("decisionVar").camundaAsyncAfter().endEvent().done();

		deploymentForTenant(TENANT_ONE, DMN_FILE);
		deploymentForTenant(TENANT_TWO, DMN_FILE_VERSION_TWO);
		deployment(process);

		ProcessInstance processInstanceOne = runtimeService.createProcessInstanceByKey("process").setVariable("status", "gold").execute();

		assertThat((string)runtimeService.getVariable(processInstanceOne.Id, "decisionVar"), @is(RESULT_OF_VERSION_ONE));
	  }

	  public virtual void testEvaluateDecisionWithoutTenantIdConstant()
	  {

		BpmnModelInstance process = Bpmn.createExecutableProcess("process").startEvent().businessRuleTask().camundaDecisionRef("decision").camundaDecisionRefBinding("latest").camundaDecisionRefTenantId("${null}").camundaMapDecisionResult("singleEntry").camundaResultVariable("decisionVar").camundaAsyncAfter().endEvent().done();

		deployment(DMN_FILE);
		deploymentForTenant(TENANT_ONE, process);
		deploymentForTenant(TENANT_TWO, DMN_FILE_VERSION_TWO);

		ProcessInstance processInstanceOne = runtimeService.createProcessInstanceByKey("process").setVariable("status", "gold").execute();

		assertThat((string)runtimeService.getVariable(processInstanceOne.Id, "decisionVar"), @is(RESULT_OF_VERSION_ONE));
	  }

	  public virtual void testEvaluateDecisionTenantIdExpression()
	  {

		BpmnModelInstance process = Bpmn.createExecutableProcess("process").startEvent().businessRuleTask().camundaDecisionRef("decision").camundaDecisionRefBinding("latest").camundaDecisionRefTenantId("${'" + TENANT_ONE + "'}").camundaMapDecisionResult("singleEntry").camundaResultVariable("decisionVar").camundaAsyncAfter().endEvent().done();

		deploymentForTenant(TENANT_ONE, DMN_FILE);
		deploymentForTenant(TENANT_TWO, DMN_FILE_VERSION_TWO);
		deployment(process);

		ProcessInstance processInstanceOne = runtimeService.createProcessInstanceByKey("process").setVariable("status", "gold").execute();

		assertThat((string)runtimeService.getVariable(processInstanceOne.Id, "decisionVar"), @is(RESULT_OF_VERSION_ONE));
	  }

	  public virtual void testEvaluateDecisionTenantIdCompositeExpression()
	  {
		// given
		BpmnModelInstance process = Bpmn.createExecutableProcess("process").startEvent().businessRuleTask().camundaDecisionRef("decision").camundaDecisionRefBinding("latest").camundaDecisionRefTenantId("tenant${'1'}").camundaMapDecisionResult("singleEntry").camundaResultVariable("decisionVar").camundaAsyncAfter().endEvent().done();
		deploymentForTenant(TENANT_ONE, DMN_FILE);
		deploymentForTenant(TENANT_TWO, DMN_FILE_VERSION_TWO);
		deployment(process);

		// when
		ProcessInstance processInstanceOne = runtimeService.createProcessInstanceByKey("process").setVariable("status", "gold").execute();

		// then
		assertThat((string)runtimeService.getVariable(processInstanceOne.Id, "decisionVar"), @is(RESULT_OF_VERSION_ONE));
	  }

	}

}