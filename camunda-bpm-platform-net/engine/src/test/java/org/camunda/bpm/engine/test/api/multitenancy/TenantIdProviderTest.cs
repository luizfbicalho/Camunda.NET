using System.Collections.Generic;

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
	using DelegateCaseExecution = org.camunda.bpm.engine.@delegate.DelegateCaseExecution;
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using TenantIdProvider = org.camunda.bpm.engine.impl.cfg.multitenancy.TenantIdProvider;
	using TenantIdProviderCaseInstanceContext = org.camunda.bpm.engine.impl.cfg.multitenancy.TenantIdProviderCaseInstanceContext;
	using TenantIdProviderHistoricDecisionInstanceContext = org.camunda.bpm.engine.impl.cfg.multitenancy.TenantIdProviderHistoricDecisionInstanceContext;
	using TenantIdProviderProcessInstanceContext = org.camunda.bpm.engine.impl.cfg.multitenancy.TenantIdProviderProcessInstanceContext;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.nullValue;
	using After = org.junit.After;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using ClassRule = org.junit.ClassRule;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class TenantIdProviderTest
	{
		private bool InstanceFieldsInitialized = false;

		public TenantIdProviderTest()
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


	  protected internal const string CONFIGURATION_RESOURCE = "org/camunda/bpm/engine/test/api/multitenancy/TenantIdProviderTest.camunda.cfg.xml";

	  protected internal const string PROCESS_DEFINITION_KEY = "testProcess";
	  protected internal const string DECISION_DEFINITION_KEY = "decision";
	  protected internal const string CASE_DEFINITION_KEY = "caseTaskCase";

	  protected internal const string DMN_FILE = "org/camunda/bpm/engine/test/api/multitenancy/simpleDecisionTable.dmn";

	  protected internal const string CMMN_FILE = "org/camunda/bpm/engine/test/api/multitenancy/CaseWithCaseTask.cmmn";
	  protected internal const string CMMN_FILE_WITH_MANUAL_ACTIVATION = "org/camunda/bpm/engine/test/api/multitenancy/CaseWithCaseTaskWithManualActivation.cmmn";
	  protected internal const string CMMN_VARIABLE_FILE = "org/camunda/bpm/engine/test/api/multitenancy/CaseWithCaseTaskVariables.cmmn";
	  protected internal const string CMMN_SUBPROCESS_FILE = "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn";

	  protected internal const string TENANT_ID = "tenant1";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule bootstrapRule = new org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule(CONFIGURATION_RESOURCE);
	  public static ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRule(CONFIGURATION_RESOURCE);
	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);

	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		TestTenantIdProvider.reset();
	  }

	  // root process instance //////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerCalledForProcessDefinitionWithoutTenantId()
	  public virtual void providerCalledForProcessDefinitionWithoutTenantId()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		// given a deployment without tenant id
		testRule.deploy(Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().done());

		// if a process instance is started
		engineRule.RuntimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		// then the tenant id provider is invoked
		assertThat(tenantIdProvider.parameters.Count, @is(1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerNotCalledForProcessDefinitionWithTenantId()
	  public virtual void providerNotCalledForProcessDefinitionWithTenantId()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		// given a deployment with a tenant id
		testRule.deployForTenant(TENANT_ID, Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().done());

		// if a process instance is started
		engineRule.RuntimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		// then the tenant id provider is not invoked
		assertThat(tenantIdProvider.parameters.Count, @is(0));
	  }



//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerCalledForStartedProcessInstanceByStartFormWithoutTenantId()
	  public virtual void providerCalledForStartedProcessInstanceByStartFormWithoutTenantId()
	  {
		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		// given a deployment without a tenant id
		testRule.deploy(Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().done(), "org/camunda/bpm/engine/test/api/form/util/request.form");

		// when a process instance is started with a start form
		string processDefinitionId = engineRule.RepositoryService.createProcessDefinitionQuery().singleResult().Id;

		IDictionary<string, object> properties = new Dictionary<string, object>();
		properties["employeeName"] = "demo";

		ProcessInstance procInstance = engineRule.FormService.submitStartForm(processDefinitionId, properties);
		assertNotNull(procInstance);

		// then the tenant id provider is invoked
		assertThat(tenantIdProvider.parameters.Count, @is(1));
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerNotCalledForStartedProcessInstanceByStartFormWithTenantId()
	  public virtual void providerNotCalledForStartedProcessInstanceByStartFormWithTenantId()
	  {
		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		// given a deployment with a tenant id
		testRule.deployForTenant(TENANT_ID, Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().done(), "org/camunda/bpm/engine/test/api/form/util/request.form");

		// when a process instance is started with a start form
		string processDefinitionId = engineRule.RepositoryService.createProcessDefinitionQuery().singleResult().Id;

		IDictionary<string, object> properties = new Dictionary<string, object>();
		properties["employeeName"] = "demo";

		ProcessInstance procInstance = engineRule.FormService.submitStartForm(processDefinitionId, properties);
		assertNotNull(procInstance);

		// then the tenant id provider is not invoked
		assertThat(tenantIdProvider.parameters.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerCalledForStartedProcessInstanceByModificationWithoutTenantId()
	  public virtual void providerCalledForStartedProcessInstanceByModificationWithoutTenantId()
	  {
		// given a deployment without a tenant id
		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;
		testRule.deploy(Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().userTask("task").endEvent().done(), "org/camunda/bpm/engine/test/api/form/util/request.form");

		// when a process instance is created and the instance is set to a starting point
		string processInstanceId = engineRule.RuntimeService.createProcessInstanceByKey(PROCESS_DEFINITION_KEY).startBeforeActivity("task").execute().ProcessInstanceId;

		//then provider is called
		assertNotNull(engineRule.RuntimeService.getActivityInstance(processInstanceId));
		assertThat(tenantIdProvider.parameters.Count, @is(1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerNotCalledForStartedProcessInstanceByModificationWithTenantId()
	  public virtual void providerNotCalledForStartedProcessInstanceByModificationWithTenantId()
	  {
		// given a deployment with a tenant id
		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;
		testRule.deployForTenant(TENANT_ID, Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().userTask("task").endEvent().done(), "org/camunda/bpm/engine/test/api/form/util/request.form");

		// when a process instance is created and the instance is set to a starting point
		string processInstanceId = engineRule.RuntimeService.createProcessInstanceByKey(PROCESS_DEFINITION_KEY).startBeforeActivity("task").execute().ProcessInstanceId;

		//then provider should not be called
		assertNotNull(engineRule.RuntimeService.getActivityInstance(processInstanceId));
		assertThat(tenantIdProvider.parameters.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerCalledWithVariables()
	  public virtual void providerCalledWithVariables()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.deploy(Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().done());

		// if a process instance is started
		engineRule.RuntimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY, Variables.createVariables().putValue("varName", true));

		// then the tenant id provider is passed in the variable
		assertThat(tenantIdProvider.parameters.Count, @is(1));
		assertThat((bool?) tenantIdProvider.parameters[0].Variables.get("varName"), @is(true));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerCalledWithProcessDefinition()
	  public virtual void providerCalledWithProcessDefinition()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.deploy(Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().done());
		ProcessDefinition deployedProcessDefinition = engineRule.RepositoryService.createProcessDefinitionQuery().singleResult();

		// if a process instance is started
		engineRule.RuntimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		// then the tenant id provider is passed in the process definition
		ProcessDefinition passedProcessDefinition = tenantIdProvider.parameters[0].ProcessDefinition;
		assertThat(passedProcessDefinition, @is(notNullValue()));
		assertThat(passedProcessDefinition.Id, @is(deployedProcessDefinition.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setsTenantId()
	  public virtual void setsTenantId()
	  {

		string tenantId = TENANT_ID;
		StaticTenantIdTestProvider tenantIdProvider = new StaticTenantIdTestProvider(tenantId);
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.deploy(Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().userTask().done());

		// if a process instance is started
		engineRule.RuntimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		// then the tenant id provider can set the tenant id to a value
		ProcessInstance processInstance = engineRule.RuntimeService.createProcessInstanceQuery().singleResult();
		assertThat(processInstance.TenantId, @is(tenantId));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setNullTenantId()
	  public virtual void setNullTenantId()
	  {

		string tenantId = null;
		StaticTenantIdTestProvider tenantIdProvider = new StaticTenantIdTestProvider(tenantId);
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.deploy(Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().userTask().done());

		// if a process instance is started
		engineRule.RuntimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		// then the tenant id provider can set the tenant id to null
		ProcessInstance processInstance = engineRule.RuntimeService.createProcessInstanceQuery().singleResult();
		assertThat(processInstance.TenantId, @is(nullValue()));
	  }

	  // sub process instance //////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerCalledForProcessDefinitionWithoutTenantId_SubProcessInstance()
	  public virtual void providerCalledForProcessDefinitionWithoutTenantId_SubProcessInstance()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		// given a deployment without tenant id
		testRule.deploy(Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().done(), Bpmn.createExecutableProcess("superProcess").startEvent().callActivity().calledElement(PROCESS_DEFINITION_KEY).done());

		// if a process instance is started
		engineRule.RuntimeService.startProcessInstanceByKey("superProcess");

		// then the tenant id provider is invoked twice
		assertThat(tenantIdProvider.parameters.Count, @is(2));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerNotCalledForProcessDefinitionWithTenantId_SubProcessInstance()
	  public virtual void providerNotCalledForProcessDefinitionWithTenantId_SubProcessInstance()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		// given a deployment with a tenant id
		testRule.deployForTenant(TENANT_ID, Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().done(), Bpmn.createExecutableProcess("superProcess").startEvent().callActivity().calledElement(PROCESS_DEFINITION_KEY).done());

		// if a process instance is started
		engineRule.RuntimeService.startProcessInstanceByKey("superProcess");

		// then the tenant id provider is not invoked
		assertThat(tenantIdProvider.parameters.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerCalledWithVariables_SubProcessInstance()
	  public virtual void providerCalledWithVariables_SubProcessInstance()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.deploy(Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().done(), Bpmn.createExecutableProcess("superProcess").startEvent().callActivity().calledElement(PROCESS_DEFINITION_KEY).camundaIn("varName", "varName").done());

		// if a process instance is started
		engineRule.RuntimeService.startProcessInstanceByKey("superProcess", Variables.createVariables().putValue("varName", true));

		// then the tenant id provider is passed in the variable
		assertThat(tenantIdProvider.parameters[1].Variables.size(), @is(1));
		assertThat((bool?) tenantIdProvider.parameters[1].Variables.get("varName"), @is(true));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerCalledWithProcessDefinition_SubProcessInstance()
	  public virtual void providerCalledWithProcessDefinition_SubProcessInstance()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.deploy(Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().done(), Bpmn.createExecutableProcess("superProcess").startEvent().callActivity().calledElement(PROCESS_DEFINITION_KEY).done());
		ProcessDefinition deployedProcessDefinition = engineRule.RepositoryService.createProcessDefinitionQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).singleResult();

		// if a process instance is started
		engineRule.RuntimeService.startProcessInstanceByKey("superProcess");

		// then the tenant id provider is passed in the process definition
		ProcessDefinition passedProcessDefinition = tenantIdProvider.parameters[1].ProcessDefinition;
		assertThat(passedProcessDefinition, @is(notNullValue()));
		assertThat(passedProcessDefinition.Id, @is(deployedProcessDefinition.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerCalledWithSuperProcessInstance()
	  public virtual void providerCalledWithSuperProcessInstance()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.deploy(Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().done(), Bpmn.createExecutableProcess("superProcess").startEvent().callActivity().calledElement(PROCESS_DEFINITION_KEY).done());
		ProcessDefinition superProcessDefinition = engineRule.RepositoryService.createProcessDefinitionQuery().processDefinitionKey("superProcess").singleResult();


		// if a process instance is started
		engineRule.RuntimeService.startProcessInstanceByKey("superProcess");

		// then the tenant id provider is passed in the process definition
		DelegateExecution superExecution = tenantIdProvider.parameters[1].SuperExecution;
		assertThat(superExecution, @is(notNullValue()));
		assertThat(superExecution.ProcessDefinitionId, @is(superProcessDefinition.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setsTenantId_SubProcessInstance()
	  public virtual void setsTenantId_SubProcessInstance()
	  {

		string tenantId = TENANT_ID;
		SetValueOnSubProcessInstanceTenantIdProvider tenantIdProvider = new SetValueOnSubProcessInstanceTenantIdProvider(tenantId);
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.deploy(Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().userTask().done(), Bpmn.createExecutableProcess("superProcess").startEvent().callActivity().calledElement(PROCESS_DEFINITION_KEY).done());

		// if a process instance is started
		engineRule.RuntimeService.startProcessInstanceByKey("superProcess");

		// then the tenant id provider can set the tenant id to a value
		ProcessInstance subProcessInstance = engineRule.RuntimeService.createProcessInstanceQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).singleResult();
		assertThat(subProcessInstance.TenantId, @is(tenantId));

		// and the super process instance is not assigned a tenant id
		ProcessInstance superProcessInstance = engineRule.RuntimeService.createProcessInstanceQuery().processDefinitionKey("superProcess").singleResult();
		assertThat(superProcessInstance.TenantId, @is(nullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setNullTenantId_SubProcessInstance()
	  public virtual void setNullTenantId_SubProcessInstance()
	  {

		string tenantId = null;
		SetValueOnSubProcessInstanceTenantIdProvider tenantIdProvider = new SetValueOnSubProcessInstanceTenantIdProvider(tenantId);
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.deploy(Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().userTask().done(), Bpmn.createExecutableProcess("superProcess").startEvent().callActivity().calledElement(PROCESS_DEFINITION_KEY).done());

		// if a process instance is started
		engineRule.RuntimeService.startProcessInstanceByKey("superProcess");

		// then the tenant id provider can set the tenant id to null
		ProcessInstance processInstance = engineRule.RuntimeService.createProcessInstanceQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).singleResult();
		assertThat(processInstance.TenantId, @is(nullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void tenantIdInheritedFromSuperProcessInstance()
	  public virtual void tenantIdInheritedFromSuperProcessInstance()
	  {

		string tenantId = TENANT_ID;
		SetValueOnRootProcessInstanceTenantIdProvider tenantIdProvider = new SetValueOnRootProcessInstanceTenantIdProvider(tenantId);
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.deploy(Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().userTask().done(), Bpmn.createExecutableProcess("superProcess").startEvent().callActivity().calledElement(PROCESS_DEFINITION_KEY).done());

		// if a process instance is started
		engineRule.RuntimeService.startProcessInstanceByKey("superProcess");

		// then the tenant id is inherited to the sub process instance even tough it is not set by the provider
		ProcessInstance processInstance = engineRule.RuntimeService.createProcessInstanceQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).singleResult();
		assertThat(processInstance.TenantId, @is(tenantId));
	  }

	  // process task in case //////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerCalledForProcessDefinitionWithoutTenantId_ProcessTask()
	  public virtual void providerCalledForProcessDefinitionWithoutTenantId_ProcessTask()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.deploy(Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().userTask().done(), "org/camunda/bpm/engine/test/api/multitenancy/CaseWithProcessTask.cmmn");

		// if the case is started
		engineRule.CaseService.createCaseInstanceByKey("testCase");
		CaseExecution caseExecution = engineRule.CaseService.createCaseExecutionQuery().activityId("PI_ProcessTask_1").singleResult();


		// then the tenant id provider is invoked once for the process instance
		assertThat(tenantIdProvider.parameters.Count, @is(1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerNotCalledForProcessDefinitionWithTenantId_ProcessTask()
	  public virtual void providerNotCalledForProcessDefinitionWithTenantId_ProcessTask()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.deployForTenant(TENANT_ID, Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().userTask().done(), "org/camunda/bpm/engine/test/api/multitenancy/CaseWithProcessTask.cmmn");

		// if the case is started
		engineRule.CaseService.createCaseInstanceByKey("testCase");
		CaseExecution caseExecution = engineRule.CaseService.createCaseExecutionQuery().activityId("PI_ProcessTask_1").singleResult();

		// then the tenant id provider is not invoked
		assertThat(tenantIdProvider.parameters.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerCalledWithVariables_ProcessTask()
	  public virtual void providerCalledWithVariables_ProcessTask()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.deploy(Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().userTask().done(), "org/camunda/bpm/engine/test/api/multitenancy/CaseWithProcessTask.cmmn");

		// if the case is started
		engineRule.CaseService.createCaseInstanceByKey("testCase", Variables.createVariables().putValue("varName", true));
		CaseExecution caseExecution = engineRule.CaseService.createCaseExecutionQuery().activityId("PI_ProcessTask_1").singleResult();

		// then the tenant id provider is passed in the variable
		assertThat(tenantIdProvider.parameters.Count, @is(1));

		VariableMap variables = tenantIdProvider.parameters[0].Variables;
		assertThat(variables.size(), @is(1));
		assertThat((bool?) variables.get("varName"), @is(true));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerCalledWithProcessDefinition_ProcessTask()
	  public virtual void providerCalledWithProcessDefinition_ProcessTask()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.deploy(Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().userTask().done(), "org/camunda/bpm/engine/test/api/multitenancy/CaseWithProcessTask.cmmn");

		// if the case is started
		engineRule.CaseService.createCaseInstanceByKey("testCase");
		CaseExecution caseExecution = engineRule.CaseService.createCaseExecutionQuery().activityId("PI_ProcessTask_1").singleResult();

		// then the tenant id provider is passed in the process definition
		assertThat(tenantIdProvider.parameters.Count, @is(1));
		assertThat(tenantIdProvider.parameters[0].ProcessDefinition, @is(notNullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerCalledWithSuperCaseExecution()
	  public virtual void providerCalledWithSuperCaseExecution()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.deploy(Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().userTask().done(), "org/camunda/bpm/engine/test/api/multitenancy/CaseWithProcessTask.cmmn");

		// if the case is started
		engineRule.CaseService.createCaseInstanceByKey("testCase");
		CaseExecution caseExecution = engineRule.CaseService.createCaseExecutionQuery().activityId("PI_ProcessTask_1").singleResult();

		// then the tenant id provider is handed in the super case execution
		assertThat(tenantIdProvider.parameters.Count, @is(1));
		assertThat(tenantIdProvider.parameters[0].SuperCaseExecution, @is(notNullValue()));
	  }

	  // historic decision instance //////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerCalledForDecisionDefinitionWithoutTenantId()
	  public virtual void providerCalledForDecisionDefinitionWithoutTenantId()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		// given a deployment without tenant id
		testRule.deploy(DMN_FILE);

		// if a decision definition is evaluated
		engineRule.DecisionService.evaluateDecisionTableByKey(DECISION_DEFINITION_KEY).variables(createVariables()).evaluate();

		// then the tenant id provider is invoked
		assertThat(tenantIdProvider.dmnParameters.Count, @is(1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerNotCalledForDecisionDefinitionWithTenantId()
	  public virtual void providerNotCalledForDecisionDefinitionWithTenantId()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		// given a deployment with a tenant id
		testRule.deployForTenant(TENANT_ID, DMN_FILE);

		// if a decision definition is evaluated
		engineRule.DecisionService.evaluateDecisionTableByKey(DECISION_DEFINITION_KEY).variables(createVariables()).evaluate();

		// then the tenant id provider is not invoked
		assertThat(tenantIdProvider.dmnParameters.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerCalledWithDecisionDefinition()
	  public virtual void providerCalledWithDecisionDefinition()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.deploy(DMN_FILE);
		DecisionDefinition deployedDecisionDefinition = engineRule.RepositoryService.createDecisionDefinitionQuery().singleResult();

		// if a decision definition is evaluated
		engineRule.DecisionService.evaluateDecisionTableByKey(DECISION_DEFINITION_KEY).variables(createVariables()).evaluate();

		// then the tenant id provider is passed in the decision definition
		DecisionDefinition passedDecisionDefinition = tenantIdProvider.dmnParameters[0].DecisionDefinition;
		assertThat(passedDecisionDefinition, @is(notNullValue()));
		assertThat(passedDecisionDefinition.Id, @is(deployedDecisionDefinition.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setsTenantIdForHistoricDecisionInstance()
	  public virtual void setsTenantIdForHistoricDecisionInstance()
	  {

		string tenantId = TENANT_ID;
		StaticTenantIdTestProvider tenantIdProvider = new StaticTenantIdTestProvider(tenantId);
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.deploy(DMN_FILE);

		// if a decision definition is evaluated
		engineRule.DecisionService.evaluateDecisionTableByKey(DECISION_DEFINITION_KEY).variables(createVariables()).evaluate();

		// then the tenant id provider can set the tenant id to a value
		HistoricDecisionInstance historicDecisionInstance = engineRule.HistoryService.createHistoricDecisionInstanceQuery().singleResult();
		assertThat(historicDecisionInstance.TenantId, @is(tenantId));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setNullTenantIdForHistoricDecisionInstance()
	  public virtual void setNullTenantIdForHistoricDecisionInstance()
	  {

		string tenantId = null;
		StaticTenantIdTestProvider tenantIdProvider = new StaticTenantIdTestProvider(tenantId);
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.deploy(DMN_FILE);

		// if a decision definition is evaluated
		engineRule.DecisionService.evaluateDecisionTableByKey(DECISION_DEFINITION_KEY).variables(createVariables()).evaluate();

		// then the tenant id provider can set the tenant id to null
		HistoricDecisionInstance historicDecisionInstance = engineRule.HistoryService.createHistoricDecisionInstanceQuery().singleResult();
		assertThat(historicDecisionInstance.TenantId, @is(nullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerCalledForHistoricDecisionDefinitionWithoutTenantId_BusinessRuleTask()
	  public virtual void providerCalledForHistoricDecisionDefinitionWithoutTenantId_BusinessRuleTask()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		BpmnModelInstance process = Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().businessRuleTask().camundaDecisionRef(DECISION_DEFINITION_KEY).endEvent().done();

		// given a deployment without tenant id
		testRule.deploy(process, DMN_FILE);

		// if a decision definition is evaluated
		engineRule.RuntimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY, createVariables());

		// then the tenant id provider is invoked
		assertThat(tenantIdProvider.dmnParameters.Count, @is(1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerNotCalledForHistoricDecisionDefinitionWithTenantId_BusinessRuleTask()
	  public virtual void providerNotCalledForHistoricDecisionDefinitionWithTenantId_BusinessRuleTask()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		// given a deployment with a tenant id
		testRule.deployForTenant(TENANT_ID, Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().businessRuleTask().camundaDecisionRef(DECISION_DEFINITION_KEY).endEvent().done(), DMN_FILE);

		// if a process instance is started
		engineRule.RuntimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY, createVariables());

		// then the tenant id providers are not invoked
		assertThat(tenantIdProvider.dmnParameters.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerCalledWithExecution_BusinessRuleTasks()
	  public virtual void providerCalledWithExecution_BusinessRuleTasks()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		BpmnModelInstance process = Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().businessRuleTask().camundaDecisionRef(DECISION_DEFINITION_KEY).camundaAsyncAfter().endEvent().done();

		testRule.deploy(process, DMN_FILE);

		// if a process instance is started
		engineRule.RuntimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY, createVariables());
		Execution execution = engineRule.RuntimeService.createExecutionQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).singleResult();

		// then the tenant id provider is invoked
		assertThat(tenantIdProvider.dmnParameters.Count, @is(1));
		ExecutionEntity passedExecution = (ExecutionEntity) tenantIdProvider.dmnParameters[0].Execution;
		assertThat(passedExecution, @is(notNullValue()));
		assertThat(passedExecution.Parent.Id, @is(execution.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setsTenantIdForHistoricDecisionInstance_BusinessRuleTask()
	  public virtual void setsTenantIdForHistoricDecisionInstance_BusinessRuleTask()
	  {

		string tenantId = TENANT_ID;
		SetValueOnHistoricDecisionInstanceTenantIdProvider tenantIdProvider = new SetValueOnHistoricDecisionInstanceTenantIdProvider(tenantId);
		TestTenantIdProvider.@delegate = tenantIdProvider;

		BpmnModelInstance process = Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().businessRuleTask().camundaDecisionRef(DECISION_DEFINITION_KEY).camundaAsyncAfter().endEvent().done();

		testRule.deploy(process, DMN_FILE);

		// if a process instance is started
		engineRule.RuntimeService.createProcessInstanceByKey(PROCESS_DEFINITION_KEY).setVariables(createVariables()).execute();

		// then the tenant id provider can set the tenant id to a value
		HistoricDecisionInstance historicDecisionInstance = engineRule.HistoryService.createHistoricDecisionInstanceQuery().decisionDefinitionKey(DECISION_DEFINITION_KEY).singleResult();
		assertThat(historicDecisionInstance.TenantId, @is(tenantId));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setNullTenantIdForHistoricDecisionInstance_BusinessRuleTask()
	  public virtual void setNullTenantIdForHistoricDecisionInstance_BusinessRuleTask()
	  {

		string tenantId = null;
		SetValueOnHistoricDecisionInstanceTenantIdProvider tenantIdProvider = new SetValueOnHistoricDecisionInstanceTenantIdProvider(tenantId);
		TestTenantIdProvider.@delegate = tenantIdProvider;

		BpmnModelInstance process = Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().businessRuleTask().camundaDecisionRef(DECISION_DEFINITION_KEY).camundaAsyncAfter().endEvent().done();

		testRule.deploy(process, DMN_FILE);

		// if a process instance is started
		engineRule.RuntimeService.createProcessInstanceByKey(PROCESS_DEFINITION_KEY).setVariables(createVariables()).execute();

		// then the tenant id provider can set the tenant id to a value
		HistoricDecisionInstance historicDecisionInstance = engineRule.HistoryService.createHistoricDecisionInstanceQuery().decisionDefinitionKey(DECISION_DEFINITION_KEY).singleResult();
		assertThat(historicDecisionInstance.TenantId, @is(nullValue()));
	  }

	  protected internal virtual VariableMap createVariables()
	  {
		return Variables.createVariables().putValue("status", "gold");
	  }

	  // root case instance //////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerCalledForCaseDefinitionWithoutTenantId()
	  public virtual void providerCalledForCaseDefinitionWithoutTenantId()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		// given a deployment without tenant id
		testRule.deploy(CMMN_FILE_WITH_MANUAL_ACTIVATION);

		// if a case instance is created
		engineRule.CaseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).create();

		// then the tenant id provider is invoked
		assertThat(tenantIdProvider.caseParameters.Count, @is(1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerNotCalledForCaseInstanceWithTenantId()
	  public virtual void providerNotCalledForCaseInstanceWithTenantId()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		// given a deployment with a tenant id
		testRule.deployForTenant(TENANT_ID, CMMN_FILE_WITH_MANUAL_ACTIVATION);

		// if a case instance is created
		engineRule.CaseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).create();

		// then the tenant id provider is not invoked
		assertThat(tenantIdProvider.caseParameters.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerCalledForCaseInstanceWithVariables()
	  public virtual void providerCalledForCaseInstanceWithVariables()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.deploy(CMMN_FILE_WITH_MANUAL_ACTIVATION);

		// if a case instance is created
		engineRule.CaseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariables(Variables.createVariables().putValue("varName", true)).create();

		// then the tenant id provider is passed in the variable
		assertThat(tenantIdProvider.caseParameters.Count, @is(1));
		assertThat((bool?) tenantIdProvider.caseParameters[0].Variables.get("varName"), @is(true));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerCalledWithCaseDefinition()
	  public virtual void providerCalledWithCaseDefinition()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.deploy(CMMN_FILE_WITH_MANUAL_ACTIVATION);
		CaseDefinition deployedCaseDefinition = engineRule.RepositoryService.createCaseDefinitionQuery().singleResult();

		// if a case instance is created
		engineRule.CaseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).create();

		// then the tenant id provider is passed in the case definition
		CaseDefinition passedCaseDefinition = tenantIdProvider.caseParameters[0].CaseDefinition;
		assertThat(passedCaseDefinition, @is(notNullValue()));
		assertThat(passedCaseDefinition.Id, @is(deployedCaseDefinition.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setsTenantIdForCaseInstance()
	  public virtual void setsTenantIdForCaseInstance()
	  {

		string tenantId = TENANT_ID;
		StaticTenantIdTestProvider tenantIdProvider = new StaticTenantIdTestProvider(tenantId);
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.deploy(CMMN_FILE_WITH_MANUAL_ACTIVATION);

		// if a case instance is created
		engineRule.CaseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).create();

		// then the tenant id provider can set the tenant id to a value
		CaseInstance caseInstance = engineRule.CaseService.createCaseInstanceQuery().singleResult();
		assertThat(caseInstance.TenantId, @is(tenantId));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setNullTenantIdForCaseInstance()
	  public virtual void setNullTenantIdForCaseInstance()
	  {

		string tenantId = null;
		StaticTenantIdTestProvider tenantIdProvider = new StaticTenantIdTestProvider(tenantId);
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.deploy(CMMN_FILE_WITH_MANUAL_ACTIVATION);

		// if a case instance is created
		engineRule.CaseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).create();

		// then the tenant id provider can set the tenant id to null
		CaseInstance caseInstance = engineRule.CaseService.createCaseInstanceQuery().singleResult();
		assertThat(caseInstance.TenantId, @is(nullValue()));
	  }

	  // sub case instance //////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerCalledForCaseDefinitionWithoutTenantId_SubCaseInstance()
	  public virtual void providerCalledForCaseDefinitionWithoutTenantId_SubCaseInstance()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		// given a deployment without tenant id
		testRule.deploy(CMMN_SUBPROCESS_FILE,CMMN_FILE);

		// if a case instance is created
		engineRule.CaseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).create();

		// then the tenant id provider is invoked twice
		assertThat(tenantIdProvider.caseParameters.Count, @is(2));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerNotCalledForCaseDefinitionWithTenantId_SubCaseInstance()
	  public virtual void providerNotCalledForCaseDefinitionWithTenantId_SubCaseInstance()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		// given a deployment with a tenant id
		testRule.deployForTenant(TENANT_ID, CMMN_SUBPROCESS_FILE, CMMN_FILE);

		// if a case instance is created
		engineRule.CaseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).create();

		// then the tenant id provider is not invoked
		assertThat(tenantIdProvider.caseParameters.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerCalledWithVariables_SubCaseInstance()
	  public virtual void providerCalledWithVariables_SubCaseInstance()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.deploy(CMMN_SUBPROCESS_FILE, CMMN_VARIABLE_FILE);

		// if a case instance is created
		engineRule.CaseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariables(Variables.createVariables().putValue("varName", true)).create();

		// then the tenant id provider is passed in the variable
		assertThat(tenantIdProvider.caseParameters[1].Variables.size(), @is(1));
		assertThat((bool?) tenantIdProvider.caseParameters[1].Variables.get("varName"), @is(true));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerCalledWithCaseDefinition_SubCaseInstance()
	  public virtual void providerCalledWithCaseDefinition_SubCaseInstance()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.deploy(CMMN_SUBPROCESS_FILE, CMMN_FILE);
		CaseDefinition deployedCaseDefinition = engineRule.RepositoryService.createCaseDefinitionQuery().caseDefinitionKey("oneTaskCase").singleResult();

		// if a case instance is created
		engineRule.CaseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).create();

		// then the tenant id provider is passed in the case definition
		CaseDefinition passedCaseDefinition = tenantIdProvider.caseParameters[1].CaseDefinition;
		assertThat(passedCaseDefinition, @is(notNullValue()));
		assertThat(passedCaseDefinition.Id, @is(deployedCaseDefinition.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerCalledWithSuperCaseInstance()
	  public virtual void providerCalledWithSuperCaseInstance()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.deploy(CMMN_SUBPROCESS_FILE, CMMN_FILE_WITH_MANUAL_ACTIVATION);
		CaseDefinition superCaseDefinition = engineRule.RepositoryService.createCaseDefinitionQuery().caseDefinitionKey(CASE_DEFINITION_KEY).singleResult();


		// if a case instance is created
		engineRule.CaseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).create();
		startCaseTask();

		// then the tenant id provider is passed in the case definition
		DelegateCaseExecution superCaseExecution = tenantIdProvider.caseParameters[1].SuperCaseExecution;
		assertThat(superCaseExecution, @is(notNullValue()));
		assertThat(superCaseExecution.CaseDefinitionId, @is(superCaseDefinition.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setsTenantId_SubCaseInstance()
	  public virtual void setsTenantId_SubCaseInstance()
	  {

		string tenantId = TENANT_ID;
		SetValueOnSubCaseInstanceTenantIdProvider tenantIdProvider = new SetValueOnSubCaseInstanceTenantIdProvider(tenantId);
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.deploy(CMMN_SUBPROCESS_FILE, CMMN_FILE);

		// if a case instance is created
		engineRule.CaseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).create();

		// then the tenant id provider can set the tenant id to a value
		CaseInstance subCaseInstance = engineRule.CaseService.createCaseInstanceQuery().caseDefinitionKey("oneTaskCase").singleResult();
		assertThat(subCaseInstance.TenantId, @is(tenantId));

		// and the super case instance is not assigned a tenant id
		CaseInstance superCaseInstance = engineRule.CaseService.createCaseInstanceQuery().caseDefinitionKey(CASE_DEFINITION_KEY).singleResult();
		assertThat(superCaseInstance.TenantId, @is(nullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setNullTenantId_SubCaseInstance()
	  public virtual void setNullTenantId_SubCaseInstance()
	  {

		string tenantId = null;
		SetValueOnSubCaseInstanceTenantIdProvider tenantIdProvider = new SetValueOnSubCaseInstanceTenantIdProvider(tenantId);
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.deploy(CMMN_SUBPROCESS_FILE, CMMN_FILE);

		// if a case instance is created
		engineRule.CaseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).create();

		// then the tenant id provider can set the tenant id to null
		CaseInstance caseInstance = engineRule.CaseService.createCaseInstanceQuery().caseDefinitionKey("oneTaskCase").singleResult();
		assertThat(caseInstance.TenantId, @is(nullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void tenantIdInheritedFromSuperCaseInstance()
	  public virtual void tenantIdInheritedFromSuperCaseInstance()
	  {

		string tenantId = TENANT_ID;
		SetValueOnRootCaseInstanceTenantIdProvider tenantIdProvider = new SetValueOnRootCaseInstanceTenantIdProvider(tenantId);
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.deploy(CMMN_SUBPROCESS_FILE, CMMN_FILE);

		// if a case instance is created
		engineRule.CaseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).create();

		// then the tenant id is inherited to the sub case instance even tough it is not set by the provider
		CaseInstance caseInstance = engineRule.CaseService.createCaseInstanceQuery().caseDefinitionKey("oneTaskCase").singleResult();
		assertThat(caseInstance.TenantId, @is(tenantId));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void providerCalledForCaseInstanceWithSuperCaseExecution()
	  public virtual void providerCalledForCaseInstanceWithSuperCaseExecution()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.deploy(CMMN_SUBPROCESS_FILE, CMMN_FILE);

		// if the case is started
		engineRule.CaseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).create();

		// then the tenant id provider is handed in the super case execution
		assertThat(tenantIdProvider.caseParameters.Count, @is(2));
		assertThat(tenantIdProvider.caseParameters[1].SuperCaseExecution, @is(notNullValue()));
	  }

	  protected internal virtual void startCaseTask()
	  {
		CaseExecution caseExecution = engineRule.CaseService.createCaseExecutionQuery().activityId("PI_CaseTask_1").singleResult();
		engineRule.CaseService.withCaseExecution(caseExecution.Id).manualStart();
	  }

	  // helpers //////////////////////////////////////////

	  public class TestTenantIdProvider : TenantIdProvider
	  {

		protected internal static TenantIdProvider @delegate;

		public virtual string provideTenantIdForProcessInstance(TenantIdProviderProcessInstanceContext ctx)
		{
		  if (@delegate != null)
		  {
			return @delegate.provideTenantIdForProcessInstance(ctx);
		  }
		  else
		  {
			return null;
		  }
		}

		public static void reset()
		{
		  @delegate = null;
		}

		public virtual string provideTenantIdForHistoricDecisionInstance(TenantIdProviderHistoricDecisionInstanceContext ctx)
		{
		  if (@delegate != null)
		  {
			return @delegate.provideTenantIdForHistoricDecisionInstance(ctx);
		  }
		  else
		  {
			return null;
		  }
		}

		public virtual string provideTenantIdForCaseInstance(TenantIdProviderCaseInstanceContext ctx)
		{
		  if (@delegate != null)
		  {
			return @delegate.provideTenantIdForCaseInstance(ctx);
		  }
		  else
		  {
			return null;
		  }
		}
	  }

	  public class ContextLoggingTenantIdProvider : TenantIdProvider
	  {

		protected internal IList<TenantIdProviderProcessInstanceContext> parameters = new List<TenantIdProviderProcessInstanceContext>();
		protected internal IList<TenantIdProviderHistoricDecisionInstanceContext> dmnParameters = new List<TenantIdProviderHistoricDecisionInstanceContext>();
		protected internal IList<TenantIdProviderCaseInstanceContext> caseParameters = new List<TenantIdProviderCaseInstanceContext>();

		public virtual string provideTenantIdForProcessInstance(TenantIdProviderProcessInstanceContext ctx)
		{
		  parameters.Add(ctx);
		  return null;
		}

		public virtual string provideTenantIdForHistoricDecisionInstance(TenantIdProviderHistoricDecisionInstanceContext ctx)
		{
		  dmnParameters.Add(ctx);
		  return null;
		}

		public virtual string provideTenantIdForCaseInstance(TenantIdProviderCaseInstanceContext ctx)
		{
		  caseParameters.Add(ctx);
		  return null;
		}

	  }

	  //only sets tenant ids on sub process instances
	  public class SetValueOnSubProcessInstanceTenantIdProvider : TenantIdProvider
	  {

		internal readonly string tenantIdToSet;

		public SetValueOnSubProcessInstanceTenantIdProvider(string tenantIdToSet)
		{
		  this.tenantIdToSet = tenantIdToSet;
		}

		public virtual string provideTenantIdForProcessInstance(TenantIdProviderProcessInstanceContext ctx)
		{
		  return ctx.SuperExecution != null ? tenantIdToSet : null;
		}

		public virtual string provideTenantIdForHistoricDecisionInstance(TenantIdProviderHistoricDecisionInstanceContext ctx)
		{
		  return null;
		}

		public virtual string provideTenantIdForCaseInstance(TenantIdProviderCaseInstanceContext ctx)
		{
		  return null;
		}

	  }

	  // only sets tenant ids on root process instances
	  public class SetValueOnRootProcessInstanceTenantIdProvider : TenantIdProvider
	  {

		internal readonly string tenantIdToSet;

		public SetValueOnRootProcessInstanceTenantIdProvider(string tenantIdToSet)
		{
		  this.tenantIdToSet = tenantIdToSet;
		}

		public virtual string provideTenantIdForProcessInstance(TenantIdProviderProcessInstanceContext ctx)
		{
		  return ctx.SuperExecution == null ? tenantIdToSet : null;
		}

		public virtual string provideTenantIdForHistoricDecisionInstance(TenantIdProviderHistoricDecisionInstanceContext ctx)
		{
		  return null;
		}

		public virtual string provideTenantIdForCaseInstance(TenantIdProviderCaseInstanceContext ctx)
		{
		  return null;
		}
	  }

	  //only sets tenant ids on historic decision instances when an execution exists
	  public class SetValueOnHistoricDecisionInstanceTenantIdProvider : TenantIdProvider
	  {

		internal readonly string tenantIdToSet;

		public SetValueOnHistoricDecisionInstanceTenantIdProvider(string tenantIdToSet)
		{
		  this.tenantIdToSet = tenantIdToSet;
		}

		public virtual string provideTenantIdForProcessInstance(TenantIdProviderProcessInstanceContext ctx)
		{
		  return null;
		}

		public virtual string provideTenantIdForHistoricDecisionInstance(TenantIdProviderHistoricDecisionInstanceContext ctx)
		{
		  return ctx.Execution != null ? tenantIdToSet : null;
		}

		public virtual string provideTenantIdForCaseInstance(TenantIdProviderCaseInstanceContext ctx)
		{
		  return null;
		}
	  }

	  //only sets tenant ids on sub case instances
	  public class SetValueOnSubCaseInstanceTenantIdProvider : TenantIdProvider
	  {

		internal readonly string tenantIdToSet;

		public SetValueOnSubCaseInstanceTenantIdProvider(string tenantIdToSet)
		{
		  this.tenantIdToSet = tenantIdToSet;
		}

		public virtual string provideTenantIdForProcessInstance(TenantIdProviderProcessInstanceContext ctx)
		{
		  return null;
		}

		public virtual string provideTenantIdForHistoricDecisionInstance(TenantIdProviderHistoricDecisionInstanceContext ctx)
		{
		  return null;
		}

		public virtual string provideTenantIdForCaseInstance(TenantIdProviderCaseInstanceContext ctx)
		{
		  return ctx.SuperCaseExecution != null ? tenantIdToSet : null;
		}
	  }

	  // only sets tenant ids on root case instances
	  public class SetValueOnRootCaseInstanceTenantIdProvider : TenantIdProvider
	  {

		internal readonly string tenantIdToSet;

		public SetValueOnRootCaseInstanceTenantIdProvider(string tenantIdToSet)
		{
		  this.tenantIdToSet = tenantIdToSet;
		}

		public virtual string provideTenantIdForProcessInstance(TenantIdProviderProcessInstanceContext ctx)
		{
		  return null;
		}

		public virtual string provideTenantIdForHistoricDecisionInstance(TenantIdProviderHistoricDecisionInstanceContext ctx)
		{
		  return null;
		}

		public virtual string provideTenantIdForCaseInstance(TenantIdProviderCaseInstanceContext ctx)
		{
		  return ctx.SuperCaseExecution == null ? tenantIdToSet : null;
		}
	  }

	}

}