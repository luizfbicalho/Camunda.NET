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
namespace org.camunda.bpm.engine.test.api.multitenancy.tenantcheck
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;


	using StartFormData = org.camunda.bpm.engine.form.StartFormData;
	using TaskFormData = org.camunda.bpm.engine.form.TaskFormData;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	public class MultiTenancyFormServiceCmdsTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyFormServiceCmdsTenantCheckTest()
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

	 protected internal const string TENANT_ONE = "tenant1";

	  protected internal const string PROCESS_DEFINITION_KEY = "formKeyProcess";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

	  protected internal TaskService taskService;

	  protected internal FormService formService;

	  protected internal RuntimeService runtimeService;

	  protected internal IdentityService identityService;

	  protected internal RepositoryService repositoryService;

	  protected internal ProcessEngineConfiguration processEngineConfiguration;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown= org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {

		taskService = engineRule.TaskService;

		formService = engineRule.FormService;

		identityService = engineRule.IdentityService;

		runtimeService = engineRule.RuntimeService;

		repositoryService = engineRule.RepositoryService;

		processEngineConfiguration = engineRule.ProcessEngineConfiguration;

	  }

	  // GetStartForm test
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStartFormWithAuthenticatedTenant()
	  public virtual void testGetStartFormWithAuthenticatedTenant()
	  {

		testRule.deployForTenant(TENANT_ONE, "org/camunda/bpm/engine/test/api/authorization/formKeyProcess.bpmn20.xml");

		ProcessInstance instance = runtimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		StartFormData startFormData = formService.getStartFormData(instance.ProcessDefinitionId);

		// then
		assertNotNull(startFormData);
		assertEquals("aStartFormKey",startFormData.FormKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStartFormWithNoAuthenticatedTenant()
	  public virtual void testGetStartFormWithNoAuthenticatedTenant()
	  {

		testRule.deployForTenant(TENANT_ONE, "org/camunda/bpm/engine/test/api/authorization/formKeyProcess.bpmn20.xml");

		ProcessInstance instance = runtimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot get the process definition '" + instance.ProcessDefinitionId + "' because it belongs to no authenticated tenant.");

		// when
		formService.getStartFormData(instance.ProcessDefinitionId);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStartFormWithDisabledTenantCheck()
	  public virtual void testGetStartFormWithDisabledTenantCheck()
	  {

		testRule.deployForTenant(TENANT_ONE, "org/camunda/bpm/engine/test/api/authorization/formKeyProcess.bpmn20.xml");

		ProcessInstance instance = runtimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		identityService.setAuthentication("aUserId", null);
		processEngineConfiguration.TenantCheckEnabled = false;

		StartFormData startFormData = formService.getStartFormData(instance.ProcessDefinitionId);

		// then
		assertNotNull(startFormData);
		assertEquals("aStartFormKey",startFormData.FormKey);

	  }

	  // GetRenderedStartForm
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetRenderedStartFormWithAuthenticatedTenant()
	  public virtual void testGetRenderedStartFormWithAuthenticatedTenant()
	  {

		testRule.deployForTenant(TENANT_ONE, "org/camunda/bpm/engine/test/api/form/util/VacationRequest_deprecated_forms.bpmn20.xml", "org/camunda/bpm/engine/test/api/form/util/request.form");

		string processDefinitionId = repositoryService.createProcessDefinitionQuery().singleResult().Id;

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		assertNotNull(formService.getRenderedStartForm(processDefinitionId, "juel"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetRenderedStartFormWithNoAuthenticatedTenant()
	  public virtual void testGetRenderedStartFormWithNoAuthenticatedTenant()
	  {

		testRule.deployForTenant(TENANT_ONE, "org/camunda/bpm/engine/test/api/form/util/VacationRequest_deprecated_forms.bpmn20.xml", "org/camunda/bpm/engine/test/api/form/util/request.form");

		string processDefinitionId = repositoryService.createProcessDefinitionQuery().singleResult().Id;

		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot get the process definition '" + processDefinitionId + "' because it belongs to no authenticated tenant.");

		// when
		formService.getRenderedStartForm(processDefinitionId, "juel");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetRenderedStartFormWithDisabledTenantCheck()
	  public virtual void testGetRenderedStartFormWithDisabledTenantCheck()
	  {

		testRule.deployForTenant(TENANT_ONE, "org/camunda/bpm/engine/test/api/form/util/VacationRequest_deprecated_forms.bpmn20.xml", "org/camunda/bpm/engine/test/api/form/util/request.form");

		string processDefinitionId = repositoryService.createProcessDefinitionQuery().singleResult().Id;

		identityService.setAuthentication("aUserId", null);
		processEngineConfiguration.TenantCheckEnabled = false;

		assertNotNull(formService.getRenderedStartForm(processDefinitionId, "juel"));
	  }

	  // submitStartForm
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitStartFormWithAuthenticatedTenant()
	  public virtual void testSubmitStartFormWithAuthenticatedTenant()
	  {

		testRule.deployForTenant(TENANT_ONE, "org/camunda/bpm/engine/test/api/form/util/VacationRequest_deprecated_forms.bpmn20.xml", "org/camunda/bpm/engine/test/api/form/util/request.form");

		string processDefinitionId = repositoryService.createProcessDefinitionQuery().singleResult().Id;

		IDictionary<string, object> properties = new Dictionary<string, object>();
		properties["employeeName"] = "demo";

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		assertNotNull(formService.submitStartForm(processDefinitionId, properties));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitStartFormWithNoAuthenticatedTenant()
	  public virtual void testSubmitStartFormWithNoAuthenticatedTenant()
	  {

		testRule.deployForTenant(TENANT_ONE, "org/camunda/bpm/engine/test/api/form/util/VacationRequest_deprecated_forms.bpmn20.xml", "org/camunda/bpm/engine/test/api/form/util/request.form");

		string processDefinitionId = repositoryService.createProcessDefinitionQuery().singleResult().Id;

		IDictionary<string, object> properties = new Dictionary<string, object>();
		properties["employeeName"] = "demo";

		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot create an instance of the process definition '" + processDefinitionId + "' because it belongs to no authenticated tenant.");

		// when
		formService.submitStartForm(processDefinitionId, properties);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitStartFormWithDisabledTenantcheck()
	  public virtual void testSubmitStartFormWithDisabledTenantcheck()
	  {

		testRule.deployForTenant(TENANT_ONE, "org/camunda/bpm/engine/test/api/form/util/VacationRequest_deprecated_forms.bpmn20.xml", "org/camunda/bpm/engine/test/api/form/util/request.form");

		string processDefinitionId = repositoryService.createProcessDefinitionQuery().singleResult().Id;

		IDictionary<string, object> properties = new Dictionary<string, object>();
		properties["employeeName"] = "demo";

		identityService.setAuthentication("aUserId", null);
		processEngineConfiguration.TenantCheckEnabled = false;

		// when
		assertNotNull(formService.submitStartForm(processDefinitionId, properties));

	  }

	  // getStartFormKey
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStartFormKeyWithAuthenticatedTenant()
	  public virtual void testGetStartFormKeyWithAuthenticatedTenant()
	  {

		testRule.deployForTenant(TENANT_ONE, "org/camunda/bpm/engine/test/api/authorization/formKeyProcess.bpmn20.xml");

		string processDefinitionId = runtimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY).ProcessDefinitionId;

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));
		assertEquals("aStartFormKey", formService.getStartFormKey(processDefinitionId));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStartFormKeyWithNoAuthenticatedTenant()
	  public virtual void testGetStartFormKeyWithNoAuthenticatedTenant()
	  {

		testRule.deployForTenant(TENANT_ONE, "org/camunda/bpm/engine/test/api/authorization/formKeyProcess.bpmn20.xml");

		string processDefinitionId = runtimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY).ProcessDefinitionId;

		identityService.setAuthentication("aUserId", null);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot get the process definition '" + processDefinitionId + "' because it belongs to no authenticated tenant.");
		formService.getStartFormKey(processDefinitionId);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStartFormKeyWithDisabledTenantCheck()
	  public virtual void testGetStartFormKeyWithDisabledTenantCheck()
	  {

		testRule.deployForTenant(TENANT_ONE, "org/camunda/bpm/engine/test/api/authorization/formKeyProcess.bpmn20.xml");

		string processDefinitionId = runtimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY).ProcessDefinitionId;

		identityService.setAuthentication("aUserId", null);
		processEngineConfiguration.TenantCheckEnabled = false;

		// then
		assertEquals("aStartFormKey", formService.getStartFormKey(processDefinitionId));

	  }

	  // GetTaskForm test
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskFormWithAuthenticatedTenant()
	  public virtual void testGetTaskFormWithAuthenticatedTenant()
	  {

		testRule.deployForTenant(TENANT_ONE, "org/camunda/bpm/engine/test/api/authorization/formKeyProcess.bpmn20.xml");

		runtimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		string taskId = taskService.createTaskQuery().singleResult().Id;

		TaskFormData taskFormData = formService.getTaskFormData(taskId);

		// then
		assertNotNull(taskFormData);
		assertEquals("aTaskFormKey", taskFormData.FormKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskFormWithNoAuthenticatedTenant()
	  public virtual void testGetTaskFormWithNoAuthenticatedTenant()
	  {

		testRule.deployForTenant(TENANT_ONE, "org/camunda/bpm/engine/test/api/authorization/formKeyProcess.bpmn20.xml");

		runtimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		string taskId = taskService.createTaskQuery().singleResult().Id;

		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot read the task '" + taskId + "' because it belongs to no authenticated tenant.");

		// when
		formService.getTaskFormData(taskId);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskFormWithDisabledTenantCheck()
	  public virtual void testGetTaskFormWithDisabledTenantCheck()
	  {

		testRule.deployForTenant(TENANT_ONE, "org/camunda/bpm/engine/test/api/authorization/formKeyProcess.bpmn20.xml");

		runtimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		string taskId = taskService.createTaskQuery().singleResult().Id;

		identityService.setAuthentication("aUserId", null);
		processEngineConfiguration.TenantCheckEnabled = false;

		TaskFormData taskFormData = formService.getTaskFormData(taskId);

		// then
		assertNotNull(taskFormData);
		assertEquals("aTaskFormKey", taskFormData.FormKey);

	  }

	  // submitTaskForm
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitTaskFormWithAuthenticatedTenant()
	  public virtual void testSubmitTaskFormWithAuthenticatedTenant()
	  {

		testRule.deployForTenant(TENANT_ONE, "org/camunda/bpm/engine/test/api/authorization/formKeyProcess.bpmn20.xml");

		string processDefinitionId = repositoryService.createProcessDefinitionQuery().singleResult().Id;

		runtimeService.startProcessInstanceById(processDefinitionId);

		assertEquals(taskService.createTaskQuery().processDefinitionId(processDefinitionId).count(), 1);

		string taskId = taskService.createTaskQuery().processDefinitionId(processDefinitionId).singleResult().Id;

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		formService.submitTaskForm(taskId, null);

		// task gets completed on execution of submitTaskForm
		assertEquals(taskService.createTaskQuery().processDefinitionId(processDefinitionId).count(), 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitTaskFormWithNoAuthenticatedTenant()
	  public virtual void testSubmitTaskFormWithNoAuthenticatedTenant()
	  {

		testRule.deployForTenant(TENANT_ONE, "org/camunda/bpm/engine/test/api/authorization/formKeyProcess.bpmn20.xml");

		string processDefinitionId = repositoryService.createProcessDefinitionQuery().singleResult().Id;

		runtimeService.startProcessInstanceById(processDefinitionId);

		string taskId = taskService.createTaskQuery().processDefinitionId(processDefinitionId).singleResult().Id;

		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot work on task '" + taskId + "' because it belongs to no authenticated tenant.");

		// when
		formService.submitTaskForm(taskId, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitTaskFormWithDisabledTenantCheck()
	  public virtual void testSubmitTaskFormWithDisabledTenantCheck()
	  {

		testRule.deployForTenant(TENANT_ONE, "org/camunda/bpm/engine/test/api/authorization/formKeyProcess.bpmn20.xml");

		string processDefinitionId = repositoryService.createProcessDefinitionQuery().singleResult().Id;

		runtimeService.startProcessInstanceById(processDefinitionId);

		string taskId = taskService.createTaskQuery().processDefinitionId(processDefinitionId).singleResult().Id;

		identityService.setAuthentication("aUserId", null);
		processEngineConfiguration.TenantCheckEnabled = false;

		formService.submitTaskForm(taskId, null);

		// task gets completed on execution of submitTaskForm
		assertEquals(taskService.createTaskQuery().processDefinitionId(processDefinitionId).count(), 0);
	  }

	  // getRenderedTaskForm
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetRenderedTaskFormWithAuthenticatedTenant()
	  public virtual void testGetRenderedTaskFormWithAuthenticatedTenant()
	  {

		// deploy tenants
		testRule.deployForTenant(TENANT_ONE, "org/camunda/bpm/engine/test/api/form/FormsProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/form/task.form").Id;

		string procDefId = repositoryService.createProcessDefinitionQuery().singleResult().Id;

		IDictionary<string, object> properties = new Dictionary<string, object>();
		properties["room"] = "5b";
		properties["speaker"] = "Mike";
		formService.submitStartForm(procDefId, properties).Id;

		string taskId = taskService.createTaskQuery().singleResult().Id;
		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		// then
		assertEquals("Mike is speaking in room 5b", formService.getRenderedTaskForm(taskId, "juel"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetRenderedTaskFormWithNoAuthenticatedTenant()
	  public virtual void testGetRenderedTaskFormWithNoAuthenticatedTenant()
	  {

		// deploy tenants
		testRule.deployForTenant(TENANT_ONE, "org/camunda/bpm/engine/test/api/form/FormsProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/form/task.form").Id;

		string procDefId = repositoryService.createProcessDefinitionQuery().singleResult().Id;

		IDictionary<string, object> properties = new Dictionary<string, object>();
		properties["room"] = "5b";
		properties["speaker"] = "Mike";
		formService.submitStartForm(procDefId, properties);

		string taskId = taskService.createTaskQuery().singleResult().Id;
		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot read the task '" + taskId + "' because it belongs to no authenticated tenant.");

		// when
		formService.getRenderedTaskForm(taskId, "juel");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetRenderedTaskFormWithDisabledTenantCheck()
	  public virtual void testGetRenderedTaskFormWithDisabledTenantCheck()
	  {

		// deploy tenants
		testRule.deployForTenant(TENANT_ONE, "org/camunda/bpm/engine/test/api/form/FormsProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/form/task.form").Id;

		string procDefId = repositoryService.createProcessDefinitionQuery().singleResult().Id;

		IDictionary<string, object> properties = new Dictionary<string, object>();
		properties["room"] = "5b";
		properties["speaker"] = "Mike";
		formService.submitStartForm(procDefId, properties);

		string taskId = taskService.createTaskQuery().singleResult().Id;
		identityService.setAuthentication("aUserId", null);
		processEngineConfiguration.TenantCheckEnabled = false;

		// then
		assertEquals("Mike is speaking in room 5b", formService.getRenderedTaskForm(taskId, "juel"));
	  }

	  // getTaskFormKey
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskFormKeyWithAuthenticatedTenant()
	  public virtual void testGetTaskFormKeyWithAuthenticatedTenant()
	  {

		testRule.deployForTenant(TENANT_ONE, "org/camunda/bpm/engine/test/api/authorization/formKeyProcess.bpmn20.xml");

		runtimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		Task task = taskService.createTaskQuery().singleResult();

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));
		assertEquals("aTaskFormKey", formService.getTaskFormKey(task.ProcessDefinitionId, task.TaskDefinitionKey));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskFormKeyWithNoAuthenticatedTenant()
	  public virtual void testGetTaskFormKeyWithNoAuthenticatedTenant()
	  {

		testRule.deployForTenant(TENANT_ONE, "org/camunda/bpm/engine/test/api/authorization/formKeyProcess.bpmn20.xml");

		runtimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		Task task = taskService.createTaskQuery().singleResult();

		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot get the process definition '" + task.ProcessDefinitionId + "' because it belongs to no authenticated tenant.");

		// when
		formService.getTaskFormKey(task.ProcessDefinitionId, task.TaskDefinitionKey);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskFormKeyWithDisabledTenantCheck()
	  public virtual void testGetTaskFormKeyWithDisabledTenantCheck()
	  {

		testRule.deployForTenant(TENANT_ONE, "org/camunda/bpm/engine/test/api/authorization/formKeyProcess.bpmn20.xml");

		runtimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		Task task = taskService.createTaskQuery().singleResult();

		identityService.setAuthentication("aUserId", null);
		processEngineConfiguration.TenantCheckEnabled = false;

		formService.getTaskFormKey(task.ProcessDefinitionId, task.TaskDefinitionKey);
		// then
		assertEquals("aTaskFormKey", formService.getTaskFormKey(task.ProcessDefinitionId, task.TaskDefinitionKey));
	  }
	}

}