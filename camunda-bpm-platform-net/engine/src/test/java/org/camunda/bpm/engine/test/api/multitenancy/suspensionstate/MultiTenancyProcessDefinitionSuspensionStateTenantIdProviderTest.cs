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
namespace org.camunda.bpm.engine.test.api.multitenancy.suspensionstate
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using TenantIdProvider = org.camunda.bpm.engine.impl.cfg.multitenancy.TenantIdProvider;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	public class MultiTenancyProcessDefinitionSuspensionStateTenantIdProviderTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyProcessDefinitionSuspensionStateTenantIdProviderTest()
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
			ruleChain = RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
		}


	  protected internal const string TENANT_ONE = "tenant1";

	  protected internal const string PROCESS_DEFINITION_KEY = "testProcess";

	  protected internal static readonly BpmnModelInstance PROCESS = Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().userTask().camundaAsyncBefore().endEvent().done();


	  protected internal ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {

			TenantIdProvider tenantIdProvider = new StaticTenantIdTestProvider(TENANT_ONE);
			configuration.TenantIdProvider = tenantIdProvider;

			return configuration;
		  }
	  }
	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);

	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown= org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void setUp()
	  {

		testRule.deploy(PROCESS);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendProcessDefinitionByIdIncludeInstancesFromAllTenants()
	  public virtual void suspendProcessDefinitionByIdIncludeInstancesFromAllTenants()
	  {
		// given active process instances with tenant id of process definition without tenant id
		engineRule.RuntimeService.createProcessInstanceByKey(PROCESS_DEFINITION_KEY).processDefinitionWithoutTenantId().execute();

		ProcessDefinition processDefinition = engineRule.RepositoryService.createProcessDefinitionQuery().withoutTenantId().singleResult();

		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery().processDefinitionId(processDefinition.Id);
		assertThat(query.active().count(), @is(1L));
		assertThat(query.active().tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.suspended().count(), @is(0L));

		// suspend all instances of process definition
		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionId(processDefinition.Id).includeProcessInstances(true).suspend();

		assertThat(query.active().count(), @is(0L));
		assertThat(query.suspended().count(), @is(1L));
		assertThat(query.suspended().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void activateProcessDefinitionByIdIncludeInstancesFromAllTenants()
	  public virtual void activateProcessDefinitionByIdIncludeInstancesFromAllTenants()
	  {
		// given suspended process instances with tenant id of process definition without tenant id
		engineRule.RuntimeService.createProcessInstanceByKey(PROCESS_DEFINITION_KEY).processDefinitionWithoutTenantId().execute();

		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).includeProcessInstances(true).suspend();

		ProcessDefinition processDefinition = engineRule.RepositoryService.createProcessDefinitionQuery().withoutTenantId().singleResult();

		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery().processDefinitionId(processDefinition.Id);
		assertThat(query.suspended().count(), @is(1L));
		assertThat(query.suspended().tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.active().count(), @is(0L));

		// activate all instance of process definition
		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionId(processDefinition.Id).includeProcessInstances(true).activate();

		assertThat(query.suspended().count(), @is(0L));
		assertThat(query.active().count(), @is(1L));
		assertThat(query.active().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }
	}

}