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
//	import static org.junit.Assert.assertThat;

	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using TenantIdProvider = org.camunda.bpm.engine.impl.cfg.multitenancy.TenantIdProvider;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	public class MultiTenancySharedDefinitionPropagationTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancySharedDefinitionPropagationTest()
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


	  protected internal const string PROCESS_DEFINITION_KEY = "testProcess";

	  protected internal const string TENANT_ID = "tenant1";


	  protected internal ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {

			TenantIdProvider tenantIdProvider = new StaticTenantIdTestProvider(TENANT_ID);
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
//ORIGINAL LINE: @Test public void propagateTenantIdToProcessInstance()
	  public virtual void propagateTenantIdToProcessInstance()
	  {
		testRule.deploy(Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().userTask().endEvent().done());

		engineRule.RuntimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		ProcessInstance processInstance = engineRule.RuntimeService.createProcessInstanceQuery().singleResult();
		assertThat(processInstance, @is(notNullValue()));
		// get the tenant id from the provider
		assertThat(processInstance.TenantId, @is(TENANT_ID));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void propagateTenantIdToIntermediateTimerJob()
	  public virtual void propagateTenantIdToIntermediateTimerJob()
	  {
		testRule.deploy(Bpmn.createExecutableProcess("process").startEvent().intermediateCatchEvent().timerWithDuration("PT1M").endEvent().done());

		engineRule.RuntimeService.startProcessInstanceByKey("process");

		// the job is created when the timer event is reached
		Job job = engineRule.ManagementService.createJobQuery().singleResult();
		assertThat(job, @is(notNullValue()));
		// inherit the tenant id from execution
		assertThat(job.TenantId, @is(TENANT_ID));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void propagateTenantIdToAsyncJob()
	  public virtual void propagateTenantIdToAsyncJob()
	  {
		testRule.deploy(Bpmn.createExecutableProcess("process").startEvent().userTask().camundaAsyncBefore().endEvent().done());

		engineRule.RuntimeService.startProcessInstanceByKey("process");

		// the job is created when the asynchronous activity is reached
		Job job = engineRule.ManagementService.createJobQuery().singleResult();
		assertThat(job, @is(notNullValue()));
		// inherit the tenant id from execution
		assertThat(job.TenantId, @is(TENANT_ID));
	  }

	}

}