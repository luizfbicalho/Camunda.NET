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
namespace org.camunda.bpm.engine.test.api.multitenancy.query.history
{
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	/// <summary>
	/// @author Askar Akhmerov
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_AUDIT)]
	public class MultiTenancyHistoricProcessInstanceStateTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyHistoricProcessInstanceStateTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			processEngineTestRule = new ProcessEngineTestRule(processEngineRule);
			ruleChain = RuleChain.outerRule(processEngineTestRule).around(processEngineRule);
		}

	  public const string PROCESS_ID = "process1";
	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  public ProcessEngineRule processEngineRule = new ProvidedProcessEngineRule();
	  public ProcessEngineTestRule processEngineTestRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(processEngineTestRule).around(processEngineRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspensionWithTenancy() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSuspensionWithTenancy()
	  {
		BpmnModelInstance instance = Bpmn.createExecutableProcess(PROCESS_ID).startEvent().userTask().endEvent().done();
		ProcessDefinition processDefinition = processEngineTestRule.deployAndGetDefinition(instance);
		ProcessDefinition processDefinition1 = processEngineTestRule.deployForTenantAndGetDefinition(TENANT_ONE, instance);
		ProcessDefinition processDefinition2 = processEngineTestRule.deployForTenantAndGetDefinition(TENANT_TWO, instance);

		ProcessInstance processInstance = processEngineRule.RuntimeService.startProcessInstanceById(processDefinition.Id);
		ProcessInstance processInstance1 = processEngineRule.RuntimeService.startProcessInstanceById(processDefinition1.Id);
		ProcessInstance processInstance2 = processEngineRule.RuntimeService.startProcessInstanceById(processDefinition2.Id);

		//suspend Tenant one
		processEngineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(processDefinition1.Key).processDefinitionTenantId(processDefinition1.TenantId).suspend();

		string[] processInstances = new string[] {processInstance1.Id, processInstance2.Id, processInstance.Id};

		verifyStates(processInstances, new string[]{org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_SUSPENDED, org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_ACTIVE, org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_ACTIVE});


		//suspend without tenant
		processEngineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(processDefinition.Key).processDefinitionWithoutTenantId().suspend();

		verifyStates(processInstances, new string[]{org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_SUSPENDED, org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_ACTIVE, org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_SUSPENDED});

		//reactivate without tenant
		processEngineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(processDefinition.Key).processDefinitionWithoutTenantId().activate();


		verifyStates(processInstances, new string[]{org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_SUSPENDED, org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_ACTIVE, org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_ACTIVE});
	  }

	  protected internal virtual void verifyStates(string[] processInstances, string[] states)
	  {
		for (int i = 0; i < processInstances.Length; i++)
		{
		  assertThat(processEngineRule.HistoryService.createHistoricProcessInstanceQuery().processInstanceId(processInstances[i]).singleResult().State, @is(states[i]));
		}
	  }
	}

}