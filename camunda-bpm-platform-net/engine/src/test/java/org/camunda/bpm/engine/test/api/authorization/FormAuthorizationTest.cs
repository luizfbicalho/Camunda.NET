using System.IO;

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
namespace org.camunda.bpm.engine.test.api.authorization
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.CREATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.CREATE_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ_TASK;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE_TASK;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.ProcessDefinitionPermissions.READ_TASK_VARIABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.TaskPermissions.READ_VARIABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.TASK;

	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using StartFormData = org.camunda.bpm.engine.form.StartFormData;
	using TaskFormData = org.camunda.bpm.engine.form.TaskFormData;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class FormAuthorizationTest : AuthorizationTest
	{

	  protected internal const string FORM_PROCESS_KEY = "FormsProcess";
	  protected internal const string RENDERED_FORM_PROCESS_KEY = "renderedFormProcess";
	  protected internal const string CASE_KEY = "oneTaskCase";

	  protected internal new string deploymentId;
	  protected internal bool ensureSpecificVariablePermission;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setUp() throws Exception
	  public override void setUp()
	  {
		deploymentId = createDeployment(null, "org/camunda/bpm/engine/test/api/form/DeployedFormsProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/form/start.form", "org/camunda/bpm/engine/test/api/form/task.form", "org/camunda/bpm/engine/test/api/authorization/renderedFormProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/authorization/oneTaskCase.cmmn").Id;
		ensureSpecificVariablePermission = processEngineConfiguration.EnforceSpecificVariablePermission;
		base.setUp();
	  }

	  public override void tearDown()
	  {
		base.tearDown();
		deleteDeployment(deploymentId);
		processEngineConfiguration.EnforceSpecificVariablePermission = ensureSpecificVariablePermission;
	  }

	  // get start form data ///////////////////////////////////////////

	  public virtual void testGetStartFormDataWithoutAuthorizations()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(FORM_PROCESS_KEY).Id;

		try
		{
		  // when
		  formService.getStartFormData(processDefinitionId);
		  fail("Exception expected: It should not be possible to get start form data");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(FORM_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testGetStartFormData()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(FORM_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, FORM_PROCESS_KEY, userId, READ);

		// when
		StartFormData startFormData = formService.getStartFormData(processDefinitionId);

		// then
		assertNotNull(startFormData);
		assertEquals("deployment:org/camunda/bpm/engine/test/api/form/start.form", startFormData.FormKey);
	  }

	  // get rendered start form /////////////////////////////////////

	  public virtual void testGetRenderedStartFormWithoutAuthorization()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(RENDERED_FORM_PROCESS_KEY).Id;

		try
		{
		  // when
		  formService.getRenderedStartForm(processDefinitionId);
		  fail("Exception expected: It should not be possible to get start form data");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(RENDERED_FORM_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testGetRenderedStartForm()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(RENDERED_FORM_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, RENDERED_FORM_PROCESS_KEY, userId, READ);

		// when
		object renderedStartForm = formService.getRenderedStartForm(processDefinitionId);

		// then
		assertNotNull(renderedStartForm);
	  }

	  // get start form variables //////////////////////////////////

	  public virtual void testGetStartFormVariablesWithoutAuthorization()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(RENDERED_FORM_PROCESS_KEY).Id;

		try
		{
		  // when
		  formService.getStartFormVariables(processDefinitionId);
		  fail("Exception expected: It should not be possible to get start form data");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(RENDERED_FORM_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testGetStartFormVariables()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(RENDERED_FORM_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, RENDERED_FORM_PROCESS_KEY, userId, READ);

		// when
		VariableMap variables = formService.getStartFormVariables(processDefinitionId);

		// then
		assertNotNull(variables);
		assertEquals(1, variables.size());
	  }

	  // submit start form /////////////////////////////////////////

	  public virtual void testSubmitStartFormWithoutAuthorization()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(FORM_PROCESS_KEY).Id;

		try
		{
		  // when
		  formService.submitStartForm(processDefinitionId, null);
		  fail("Exception expected: It should not possible to submit a start form");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(CREATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		}
	  }

	  public virtual void testSubmitStartFormWithCreatePermissionOnProcessInstance()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(FORM_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);

		try
		{
		  // when
		  formService.submitStartForm(processDefinitionId, null);
		  fail("Exception expected: It should not possible to submit a start form");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(CREATE_INSTANCE.Name, message);
		  assertTextPresent(FORM_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testSubmitStartFormWithCreateInstancePermissionOnProcessDefinition()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(FORM_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, FORM_PROCESS_KEY, userId, CREATE_INSTANCE);

		try
		{
		  // when
		  formService.submitStartForm(processDefinitionId, null);
		  fail("Exception expected: It should not possible to submit a start form");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(CREATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		}
	  }

	  public virtual void testSubmitStartForm()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(FORM_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, FORM_PROCESS_KEY, userId, CREATE_INSTANCE);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);

		// when
		ProcessInstance instance = formService.submitStartForm(processDefinitionId, null);

		// then
		assertNotNull(instance);
	  }

	  // get task form data (standalone task) /////////////////////////////////

	  public virtual void testStandaloneTaskGetTaskFormDataWithoutAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		try
		{
		  // when
		  formService.getTaskFormData(taskId);
		  fail("Exception expected: It should not possible to get task form data");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(taskId, message);
		  assertTextPresent(TASK.resourceName(), message);
		}

		// given (2)
		setReadVariableAsDefaultReadVariablePermission();

		try
		{
		  // when (2)
		  formService.getTaskFormData(taskId);
		  fail("Exception expected: It should not possible to get task form data");
		}
		catch (AuthorizationException e)
		{
		  // then (2)
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ_VARIABLE.Name, message);
		  assertTextPresent(taskId, message);
		  assertTextPresent(TASK.resourceName(), message);
		}

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskGetTaskFormData()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		createGrantAuthorization(TASK, taskId, userId, READ);

		// when
		TaskFormData taskFormData = formService.getTaskFormData(taskId);

		// then
		// Standalone task, no TaskFormData available
		assertNull(taskFormData);

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskGetTaskFormDataWithReadVariablePermission()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string taskId = "myTask";
		createTask(taskId);
		createGrantAuthorization(TASK, taskId, userId, READ_VARIABLE);

		// when
		TaskFormData taskFormData = formService.getTaskFormData(taskId);

		// then
		// Standalone task, no TaskFormData available
		assertNull(taskFormData);

		deleteTask(taskId, true);
	  }

	  // get task form data (process task) /////////////////////////////////

	  public virtual void testProcessTaskGetTaskFormDataWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(FORM_PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		try
		{
		  // when
		  formService.getTaskFormData(taskId);
		  fail("Exception expected: It should not possible to get task form data");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(taskId, message);
		  assertTextPresent(TASK.resourceName(), message);
		  assertTextPresent(READ_TASK.Name, message);
		  assertTextPresent(FORM_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}

		// given (2)
		setReadVariableAsDefaultReadVariablePermission();

		try
		{
		  // when (2)
		  formService.getTaskFormData(taskId);
		  fail("Exception expected: It should not possible to get task form data");
		}
		catch (AuthorizationException e)
		{
		  // then (2)
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ_VARIABLE.Name, message);
		  assertTextPresent(taskId, message);
		  assertTextPresent(TASK.resourceName(), message);
		  assertTextPresent(READ_TASK_VARIABLE.Name, message);
		  assertTextPresent(FORM_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testProcessTaskGetTaskFormDataWithReadPermissionOnTask()
	  {
		// given
		startProcessInstanceByKey(FORM_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, READ);

		// when
		TaskFormData taskFormData = formService.getTaskFormData(taskId);

		// then
		assertNotNull(taskFormData);
	  }

	  public virtual void testProcessTaskGetTaskFormDataWithReadTaskPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(FORM_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(PROCESS_DEFINITION, FORM_PROCESS_KEY, userId, READ_TASK);

		// when
		TaskFormData taskFormData = formService.getTaskFormData(taskId);

		// then
		assertNotNull(taskFormData);
	  }

	  public virtual void testProcessTaskGetTaskFormDataWithReadVariablePermissionOnTask()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		startProcessInstanceByKey(FORM_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, READ_VARIABLE);

		// when
		TaskFormData taskFormData = formService.getTaskFormData(taskId);

		// then
		assertNotNull(taskFormData);
	  }

	  public virtual void testProcessTaskGetTaskFormDataWithReadTaskVariablePermissionOnProcessDefinition()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		startProcessInstanceByKey(FORM_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(PROCESS_DEFINITION, FORM_PROCESS_KEY, userId, READ_TASK_VARIABLE);

		// when
		TaskFormData taskFormData = formService.getTaskFormData(taskId);

		// then
		assertNotNull(taskFormData);
	  }

	  public virtual void testProcessTaskGetTaskFormData()
	  {
		// given
		startProcessInstanceByKey(FORM_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, READ);
		createGrantAuthorization(PROCESS_DEFINITION, FORM_PROCESS_KEY, userId, READ_TASK);

		// when
		TaskFormData taskFormData = formService.getTaskFormData(taskId);

		// then
		assertNotNull(taskFormData);
	  }

	  // get task form data (case task) /////////////////////////////////

	  public virtual void testCaseTaskGetTaskFormData()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;

		// when
		TaskFormData taskFormData = formService.getTaskFormData(taskId);

		// then
		assertNotNull(taskFormData);
	  }

	  // get rendered task form (standalone task) //////////////////

	  public virtual void testStandaloneTaskGetTaskRenderedFormWithoutAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		try
		{
		  // when
		  formService.getRenderedTaskForm(taskId);
		  fail("Exception expected: It should not possible to get rendered task form");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(taskId, message);
		  assertTextPresent(TASK.resourceName(), message);
		}

		// given (2)
		setReadVariableAsDefaultReadVariablePermission();

		try
		{
		  // when (2)
		  formService.getRenderedTaskForm(taskId);
		  fail("Exception expected: It should not possible to get rendered task form");
		}
		catch (AuthorizationException e)
		{
		  // then (2)
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ_VARIABLE.Name, message);
		  assertTextPresent(taskId, message);
		  assertTextPresent(TASK.resourceName(), message);
		}


		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskGetTaskRenderedForm()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		createGrantAuthorization(TASK, taskId, userId, READ);

		try
		{
		  // when
		  // Standalone task, no TaskFormData available
		  formService.getRenderedTaskForm(taskId);
		}
		catch (NullValueException)
		{
		}

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskGetTaskRenderedFormWithReadVariablePermission()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string taskId = "myTask";
		createTask(taskId);
		createGrantAuthorization(TASK, taskId, userId, READ_VARIABLE);

		try
		{
		  // when
		  // Standalone task, no TaskFormData available
		  formService.getRenderedTaskForm(taskId);
		}
		catch (NullValueException)
		{
		}

		deleteTask(taskId, true);
	  }

	  // get rendered task form (process task) /////////////////////////////////

	  public virtual void testProcessTaskGetRenderedTaskFormWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(RENDERED_FORM_PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		try
		{
		  // when
		  formService.getRenderedTaskForm(taskId);
		  fail("Exception expected: It should not possible to get rendered task form");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(taskId, message);
		  assertTextPresent(TASK.resourceName(), message);
		  assertTextPresent(READ_TASK.Name, message);
		  assertTextPresent(RENDERED_FORM_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}

		// given (2)
		setReadVariableAsDefaultReadVariablePermission();

		try
		{
		  // when (2)
		  formService.getRenderedTaskForm(taskId);
		  fail("Exception expected: It should not possible to get rendered task form");
		}
		catch (AuthorizationException e)
		{
		  // then (2)
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ_VARIABLE.Name, message);
		  assertTextPresent(taskId, message);
		  assertTextPresent(TASK.resourceName(), message);
		  assertTextPresent(READ_TASK_VARIABLE.Name, message);
		  assertTextPresent(RENDERED_FORM_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testProcessTaskGetRenderedTaskFormWithReadPermissionOnTask()
	  {
		// given
		startProcessInstanceByKey(RENDERED_FORM_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, READ);

		// when
		object taskForm = formService.getRenderedTaskForm(taskId);

		// then
		assertNotNull(taskForm);
	  }

	  public virtual void testProcessTaskGetRenderedTaskFormWithReadTaskPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(RENDERED_FORM_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(PROCESS_DEFINITION, RENDERED_FORM_PROCESS_KEY, userId, READ_TASK);

		// when
		object taskForm = formService.getRenderedTaskForm(taskId);

		// then
		assertNotNull(taskForm);
	  }

	  public virtual void testProcessTaskGetRenderedTaskFormWithReadTaskVariablesPermissionOnProcessDefinition()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		startProcessInstanceByKey(RENDERED_FORM_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(PROCESS_DEFINITION, RENDERED_FORM_PROCESS_KEY, userId, READ_TASK_VARIABLE);

		// when
		object taskForm = formService.getRenderedTaskForm(taskId);

		// then
		assertNotNull(taskForm);
	  }

	  public virtual void testProcessTaskGetRenderedTaskFormWithReadVariablePermissionOnTask()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		startProcessInstanceByKey(RENDERED_FORM_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, READ_VARIABLE);

		// when
		object taskForm = formService.getRenderedTaskForm(taskId);

		// then
		assertNotNull(taskForm);
	  }

	  public virtual void testProcessTaskGetRenderedTaskForm()
	  {
		// given
		startProcessInstanceByKey(RENDERED_FORM_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, READ);
		createGrantAuthorization(PROCESS_DEFINITION, RENDERED_FORM_PROCESS_KEY, userId, READ_TASK);

		// when
		object taskForm = formService.getRenderedTaskForm(taskId);

		// then
		assertNotNull(taskForm);
	  }

	  // get rendered task form (case task) /////////////////////////////////

	  public virtual void testCaseTaskGetRenderedTaskForm()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;

		// when
		object taskForm = formService.getRenderedTaskForm(taskId);

		// then
		assertNull(taskForm);
	  }

	  // get task form variables (standalone task) ////////////////////////

	  public virtual void testStandaloneTaskGetTaskFormVariablesWithoutAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		try
		{
		  // when
		  formService.getTaskFormVariables(taskId);
		  fail("Exception expected: It should not possible to get task form variables");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(taskId, message);
		  assertTextPresent(TASK.resourceName(), message);
		}

		// given (2)
		setReadVariableAsDefaultReadVariablePermission();

		try
		{
		  // when (2)
		  formService.getTaskFormVariables(taskId);
		  fail("Exception expected: It should not possible to get task form variables");
		}
		catch (AuthorizationException e)
		{
		  // then (2)
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ_VARIABLE.Name, message);
		  assertTextPresent(taskId, message);
		  assertTextPresent(TASK.resourceName(), message);
		}

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskGetTaskFormVariables()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		createGrantAuthorization(TASK, taskId, userId, READ);

		// when
		VariableMap variables = formService.getTaskFormVariables(taskId);

		// then
		assertNotNull(variables);

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskGetTaskFormVariablesWithReadVariablePermission()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string taskId = "myTask";
		createTask(taskId);
		createGrantAuthorization(TASK, taskId, userId, READ_VARIABLE);

		// when
		VariableMap variables = formService.getTaskFormVariables(taskId);

		// then
		assertNotNull(variables);

		deleteTask(taskId, true);
	  }

	  // get task form variables (process task) /////////////////////////////////

	  public virtual void testProcessTaskGetTaskFormVariablesWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(RENDERED_FORM_PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		try
		{
		  // when
		  formService.getTaskFormVariables(taskId);
		  fail("Exception expected: It should not possible to get task form variables");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(taskId, message);
		  assertTextPresent(TASK.resourceName(), message);
		  assertTextPresent(READ_TASK.Name, message);
		  assertTextPresent(RENDERED_FORM_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}

		// given (2)
		processEngineConfiguration.EnforceSpecificVariablePermission = true;

		try
		{
		  // when (2)
		  formService.getTaskFormVariables(taskId);
		  fail("Exception expected: It should not possible to get task form variables");
		}
		catch (AuthorizationException e)
		{
		  // then (2)
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ_VARIABLE.Name, message);
		  assertTextPresent(taskId, message);
		  assertTextPresent(TASK.resourceName(), message);
		  assertTextPresent(READ_TASK_VARIABLE.Name, message);
		  assertTextPresent(RENDERED_FORM_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testProcessTaskGetTaskFormVariablesWithReadPermissionOnTask()
	  {
		// given
		startProcessInstanceByKey(RENDERED_FORM_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, READ);

		// when
		VariableMap variables = formService.getTaskFormVariables(taskId);

		// then
		assertNotNull(variables);
		assertEquals(1, variables.size());
	  }

	  public virtual void testProcessTaskGetTaskFormVariablesWithReadTaskPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(RENDERED_FORM_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(PROCESS_DEFINITION, RENDERED_FORM_PROCESS_KEY, userId, READ_TASK);

		// when
		VariableMap variables = formService.getTaskFormVariables(taskId);

		// then
		assertNotNull(variables);
		assertEquals(1, variables.size());
	  }

	  public virtual void testProcessTaskGetTaskFormVariables()
	  {
		// given
		startProcessInstanceByKey(RENDERED_FORM_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, READ);
		createGrantAuthorization(PROCESS_DEFINITION, RENDERED_FORM_PROCESS_KEY, userId, READ_TASK);

		// when
		VariableMap variables = formService.getTaskFormVariables(taskId);

		// then
		assertNotNull(variables);
		assertEquals(1, variables.size());
	  }

	  public virtual void testProcessTaskGetTaskFormVariablesWithReadVariablePermissionOnTask()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		startProcessInstanceByKey(RENDERED_FORM_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, READ_VARIABLE);

		// when
		VariableMap variables = formService.getTaskFormVariables(taskId);

		// then
		assertNotNull(variables);
		assertEquals(1, variables.size());
	  }

	  public virtual void testProcessTaskGetTaskFormVariablesWithReadTaskVariablePermissionOnProcessDefinition()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		startProcessInstanceByKey(RENDERED_FORM_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(PROCESS_DEFINITION, RENDERED_FORM_PROCESS_KEY, userId, READ_TASK_VARIABLE);

		// when
		VariableMap variables = formService.getTaskFormVariables(taskId);

		// then
		assertNotNull(variables);
		assertEquals(1, variables.size());
	  }

	  // get task form variables (case task) /////////////////////////////////

	  public virtual void testCaseTaskGetTaskFormVariables()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;

		// when
		VariableMap variables = formService.getTaskFormVariables(taskId);

		// then
		assertNotNull(variables);
		assertEquals(0, variables.size());
	  }

	  // submit task form (standalone task) ////////////////////////////////

	  public virtual void testStandaloneTaskSubmitTaskFormWithoutAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		try
		{
		  // when
		  formService.submitTaskForm(taskId, null);
		  fail("Exception expected: It should not possible to submit a task form");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(taskId, message);
		  assertTextPresent(TASK.resourceName(), message);
		}

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskSubmitTaskForm()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		formService.submitTaskForm(taskId, null);

		// then
		Task task = selectSingleTask();
		assertNull(task);

		deleteTask(taskId, true);
	  }

	  // submit task form (process task) ////////////////////////////////

	  public virtual void testProcessTaskSubmitTaskFormWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(FORM_PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		try
		{
		  // when
		  formService.submitTaskForm(taskId, null);
		  fail("Exception expected: It should not possible to submit a task form");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(taskId, message);
		  assertTextPresent(TASK.resourceName(), message);
		  assertTextPresent(UPDATE_TASK.Name, message);
		  assertTextPresent(FORM_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testProcessTaskSubmitTaskFormWithUpdatePermissionOnTask()
	  {
		// given
		startProcessInstanceByKey(FORM_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);
		createGrantAuthorization(PROCESS_DEFINITION, FORM_PROCESS_KEY, userId, UPDATE_TASK);

		// when
		formService.submitTaskForm(taskId, null);

		// then
		Task task = selectSingleTask();
		assertNull(task);
	  }

	  public virtual void testProcessTaskSubmitTaskFormWithUpdateTaskPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(FORM_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);
		createGrantAuthorization(PROCESS_DEFINITION, FORM_PROCESS_KEY, userId, UPDATE_TASK);

		// when
		formService.submitTaskForm(taskId, null);

		// then
		Task task = selectSingleTask();
		assertNull(task);
	  }

	  public virtual void testProcessTaskSubmitTaskForm()
	  {
		// given
		startProcessInstanceByKey(FORM_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);
		createGrantAuthorization(PROCESS_DEFINITION, FORM_PROCESS_KEY, userId, UPDATE_TASK);

		// when
		formService.submitTaskForm(taskId, null);

		// then
		Task task = selectSingleTask();
		assertNull(task);
	  }

	  // submit task form (case task) ////////////////////////////////

	  public virtual void testCaseTaskSubmitTaskForm()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;

		// when
		formService.submitTaskForm(taskId, null);

		// then
		Task task = selectSingleTask();
		assertNull(task);
	  }

	  // get start form key ////////////////////////////////////////

	  public virtual void testGetStartFormKeyWithoutAuthorizations()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(FORM_PROCESS_KEY).Id;

		try
		{
		  // when
		  formService.getStartFormKey(processDefinitionId);
		  fail("Exception expected: It should not possible to get a start form key");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(FORM_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testGetStartFormKey()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(FORM_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, FORM_PROCESS_KEY, userId, READ);

		// when
		string formKey = formService.getStartFormKey(processDefinitionId);

		// then
		assertEquals("deployment:org/camunda/bpm/engine/test/api/form/start.form", formKey);
	  }

	  // get task form key ////////////////////////////////////////

	  public virtual void testGetTaskFormKeyWithoutAuthorizations()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(FORM_PROCESS_KEY).Id;

		try
		{
		  // when
		  formService.getTaskFormKey(processDefinitionId, "task");
		  fail("Exception expected: It should not possible to get a task form key");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(FORM_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testGetTaskFormKey()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(FORM_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, FORM_PROCESS_KEY, userId, READ);

		// when
		string formKey = formService.getTaskFormKey(processDefinitionId, "task");

		// then
		assertEquals("deployment:org/camunda/bpm/engine/test/api/form/task.form", formKey);
	  }

	  // get deployed start form////////////////////////////////////////

	  public virtual void testGetDeployedStartForm()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(FORM_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, FORM_PROCESS_KEY, userId, READ);

		// when
		Stream inputStream = formService.getDeployedStartForm(processDefinitionId);
		assertNotNull(inputStream);
	  }

	  public virtual void testGetDeployedStartFormWithoutAuthorization()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(FORM_PROCESS_KEY).Id;

		try
		{
		  // when
		  formService.getDeployedStartForm(processDefinitionId);
		  fail("Exception expected: It should not possible to get a deployed start form");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(FORM_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  // get deployed task form////////////////////////////////////////

	  public virtual void testGetDeployedTaskForm()
	  {
		// given
		startProcessInstanceByKey(FORM_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, READ);

		// when
		Stream inputStream = formService.getDeployedTaskForm(taskId);
		assertNotNull(inputStream);
	  }

	  public virtual void testGetDeployedTaskFormWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(FORM_PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		try
		{
		  // when
		  formService.getDeployedTaskForm(taskId);
		  fail("Exception expected: It should not possible to get a deployed task form");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(FORM_PROCESS_KEY, message);
		  assertTextPresent(TASK.resourceName(), message);
		}
	  }

	  // helper ////////////////////////////////////////////////////////////////////////////////

	  protected internal virtual void setReadVariableAsDefaultReadVariablePermission()
	  {
		processEngineConfiguration.EnforceSpecificVariablePermission = true;
	  }
	}

}