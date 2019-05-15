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
namespace org.camunda.bpm.engine.test.api.authorization
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.ALL;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.CREATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.CREATE_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.DELETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.DELETE_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.ProcessDefinitionPermissions.SUSPEND_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.ProcessDefinitionPermissions.READ_INSTANCE_VARIABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.ProcessDefinitionPermissions.UPDATE_INSTANCE_VARIABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.ProcessInstancePermissions.SUSPEND;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.ProcessInstancePermissions.UPDATE_VARIABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.TASK;


	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using AbstractQuery = org.camunda.bpm.engine.impl.AbstractQuery;
	using RuntimeServiceImpl = org.camunda.bpm.engine.impl.RuntimeServiceImpl;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using VariableInstanceQuery = org.camunda.bpm.engine.runtime.VariableInstanceQuery;
	using Task = org.camunda.bpm.engine.task.Task;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class ProcessInstanceAuthorizationTest : AuthorizationTest
	{

	  protected internal const string PROCESS_KEY = "oneTaskProcess";
	  protected internal const string MESSAGE_START_PROCESS_KEY = "messageStartProcess";
	  protected internal const string MESSAGE_BOUNDARY_PROCESS_KEY = "messageBoundaryProcess";
	  protected internal const string SIGNAL_BOUNDARY_PROCESS_KEY = "signalBoundaryProcess";
	  protected internal const string SIGNAL_START_PROCESS_KEY = "signalStartProcess";
	  protected internal const string THROW_WARNING_SIGNAL_PROCESS_KEY = "throwWarningSignalProcess";
	  protected internal const string THROW_ALERT_SIGNAL_PROCESS_KEY = "throwAlertSignalProcess";

	  protected internal new string deploymentId;
	  protected internal bool ensureSpecificVariablePermission;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setUp() throws Exception
	  public override void setUp()
	  {
		deploymentId = createDeployment(null, "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/authorization/messageStartEventProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/authorization/messageBoundaryEventProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/authorization/signalBoundaryEventProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/authorization/signalStartEventProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/authorization/throwWarningSignalEventProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/authorization/throwAlertSignalEventProcess.bpmn20.xml").Id;
		ensureSpecificVariablePermission = processEngineConfiguration.EnforceSpecificVariablePermission;
		base.setUp();
	  }

	  public override void tearDown()
	  {
		base.tearDown();
		deleteDeployment(deploymentId);
		processEngineConfiguration.EnforceSpecificVariablePermission = ensureSpecificVariablePermission;
	  }

	  // process instance query //////////////////////////////////////////////////////////

	  public virtual void testSimpleQueryWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);

		// when
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testSimpleQueryWithReadPermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();

		// then
		verifyQueryResults(query, 1);

		ProcessInstance instance = query.singleResult();
		assertNotNull(instance);
		assertEquals(processInstanceId, instance.Id);
	  }

	  public virtual void testSimpleQueryWithMultiple()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);

		// when
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testSimpleQueryWithReadPermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);

		// when
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();

		// then
		verifyQueryResults(query, 1);

		ProcessInstance instance = query.singleResult();
		assertNotNull(instance);
		assertEquals(processInstanceId, instance.Id);
	  }

	  public virtual void testSimpleQueryWithReadInstancesPermissionOnOneTaskProcess()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE);

		// when
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();

		// then
		verifyQueryResults(query, 1);

		ProcessInstance instance = query.singleResult();
		assertNotNull(instance);
		assertEquals(processInstanceId, instance.Id);
	  }

	  public virtual void testSimpleQueryWithReadInstancesPermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE);

		// when
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();

		// then
		verifyQueryResults(query, 1);

		ProcessInstance instance = query.singleResult();
		assertNotNull(instance);
		assertEquals(processInstanceId, instance.Id);
	  }

	  // process instance query (multiple process instances) ////////////////////////

	  public virtual void testQueryWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);

		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);

		// when
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testQueryWithReadPermissionOnProcessInstance()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;

		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);

		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();

		// then
		verifyQueryResults(query, 1);

		ProcessInstance instance = query.singleResult();
		assertNotNull(instance);
		assertEquals(processInstanceId, instance.Id);
	  }

	  public virtual void testQueryWithReadPermissionOnAnyProcessInstance()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);

		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);

		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);

		// when
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();

		// then
		verifyQueryResults(query, 7);
	  }

	  public virtual void testQueryWithReadInstancesPermissionOnOneTaskProcess()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);

		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE);

		// when
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();

		// then
		verifyQueryResults(query, 3);
	  }

	  public virtual void testQueryWithReadInstancesPermissionOnAnyProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);

		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE);

		// when
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();

		// then
		verifyQueryResults(query, 7);
	  }

	  // start process instance by key //////////////////////////////////////////////

	  public virtual void testStartProcessInstanceByKeyWithoutAuthorization()
	  {
		// given
		// no authorization to start a process instance

		try
		{
		  // when
		  runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		  fail("Exception expected: It should not be possible to start a process instance");
		}
		catch (AuthorizationException e)
		{

		  // then
		  assertTextPresent("The user with id 'test' does not have 'CREATE' permission on resource 'ProcessInstance'", e.Message);
		}
	  }

	  public virtual void testStartProcessInstanceByKeyWithCreatePermissionOnProcessInstance()
	  {
		// given
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);

		try
		{
		  // when
		  runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		  fail("Exception expected: It should not be possible to start a process instance");
		}
		catch (AuthorizationException e)
		{

		  // then
		  assertTextPresent("The user with id 'test' does not have 'CREATE_INSTANCE' permission on resource 'oneTaskProcess' of type 'ProcessDefinition'", e.Message);
		}
	  }

	  public virtual void testStartProcessInstanceByKeyWithCreateInstancesPermissionOnProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, CREATE_INSTANCE);

		try
		{
		  // when
		  runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		  fail("Exception expected: It should not be possible to start a process instance");
		}
		catch (AuthorizationException e)
		{

		  // then
		  assertTextPresent("The user with id 'test' does not have 'CREATE' permission on resource 'ProcessInstance'", e.Message);
		}
	  }

	  public virtual void testStartProcessInstanceByKey()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, CREATE_INSTANCE);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);

		// when
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);

		// then
		disableAuthorization();
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();
		verifyQueryResults(query, 1);
		enableAuthorization();
	  }

	  // start process instance by id //////////////////////////////////////////////

	  public virtual void testStartProcessInstanceByIdWithoutAuthorization()
	  {
		// given
		// no authorization to start a process instance

		string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

		try
		{
		  // when
		  runtimeService.startProcessInstanceById(processDefinitionId);
		  fail("Exception expected: It should not be possible to start a process instance");
		}
		catch (AuthorizationException e)
		{

		  // then
		  assertTextPresent("The user with id 'test' does not have 'CREATE' permission on resource 'ProcessInstance'", e.Message);
		}
	  }

	  public virtual void testStartProcessInstanceByIdWithCreatePermissionOnProcessInstance()
	  {
		// given
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);

		string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

		try
		{
		  // when
		  runtimeService.startProcessInstanceById(processDefinitionId);
		  fail("Exception expected: It should not be possible to start a process instance");
		}
		catch (AuthorizationException e)
		{

		  // then
		  assertTextPresent("The user with id 'test' does not have 'CREATE_INSTANCE' permission on resource 'oneTaskProcess' of type 'ProcessDefinition'", e.Message);
		}
	  }

	  public virtual void testStartProcessInstanceByIdWithCreateInstancesPermissionOnProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, CREATE_INSTANCE);

		string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

		try
		{
		  // when
		  runtimeService.startProcessInstanceById(processDefinitionId);
		  fail("Exception expected: It should not be possible to start a process instance");
		}
		catch (AuthorizationException e)
		{

		  // then
		  assertTextPresent("The user with id 'test' does not have 'CREATE' permission on resource 'ProcessInstance'", e.Message);
		}
	  }

	  public virtual void testStartProcessInstanceById()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, CREATE_INSTANCE);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);

		string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

		// when
		runtimeService.startProcessInstanceById(processDefinitionId);

		// then
		disableAuthorization();
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();
		verifyQueryResults(query, 1);
		enableAuthorization();
	  }

	  public virtual void testStartProcessInstanceAtActivitiesByKey()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, CREATE_INSTANCE);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);

		// when
		runtimeService.createProcessInstanceByKey(PROCESS_KEY).startBeforeActivity("theTask").execute();

		// then
		disableAuthorization();
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();
		verifyQueryResults(query, 1);
		enableAuthorization();
	  }

	  public virtual void testStartProcessInstanceAtActivitiesByKeyWithoutAuthorization()
	  {
		// given
		// no authorization to start a process instance

		try
		{
		  // when
		  runtimeService.createProcessInstanceByKey(PROCESS_KEY).startBeforeActivity("theTask").execute();
		  fail("Exception expected: It should not be possible to start a process instance");
		}
		catch (AuthorizationException e)
		{

		  // then
		  assertTextPresent("The user with id 'test' does not have 'CREATE' permission on resource 'ProcessInstance'", e.Message);
		}
	  }

	  public virtual void testStartProcessInstanceAtActivitiesByKeyWithCreatePermissionOnProcessInstance()
	  {
		// given
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);

		try
		{
		  // when
		  runtimeService.createProcessInstanceByKey(PROCESS_KEY).startBeforeActivity("theTask").execute();
		  fail("Exception expected: It should not be possible to start a process instance");
		}
		catch (AuthorizationException e)
		{

		  // then
		  assertTextPresent("The user with id 'test' does not have 'CREATE_INSTANCE' permission on resource 'oneTaskProcess' of type 'ProcessDefinition'", e.Message);
		}
	  }

	  public virtual void testStartProcessInstanceAtActivitiesByKeyWithCreateInstancesPermissionOnProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, CREATE_INSTANCE);

		try
		{
		  // when
		  runtimeService.createProcessInstanceByKey(PROCESS_KEY).startBeforeActivity("theTask").execute();
		  fail("Exception expected: It should not be possible to start a process instance");
		}
		catch (AuthorizationException e)
		{

		  // then
		  assertTextPresent("The user with id 'test' does not have 'CREATE' permission on resource 'ProcessInstance'", e.Message);
		}
	  }

	  public virtual void testStartProcessInstanceAtActivitiesById()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, CREATE_INSTANCE);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);

		string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

		// when
		runtimeService.createProcessInstanceById(processDefinitionId).startBeforeActivity("theTask").execute();

		// then
		disableAuthorization();
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();
		verifyQueryResults(query, 1);
		enableAuthorization();
	  }

	  public virtual void testStartProcessInstanceAtActivitiesByIdWithoutAuthorization()
	  {
		// given
		// no authorization to start a process instance

		string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

		try
		{
		  // when
		  runtimeService.createProcessInstanceById(processDefinitionId).startBeforeActivity("theTask").execute();
		  fail("Exception expected: It should not be possible to start a process instance");
		}
		catch (AuthorizationException e)
		{

		  // then
		  assertTextPresent("The user with id 'test' does not have 'CREATE' permission on resource 'ProcessInstance'", e.Message);
		}
	  }

	  public virtual void testStartProcessInstanceAtActivitiesByIdWithCreatePermissionOnProcessInstance()
	  {
		// given
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);

		string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

		try
		{
		  // when
		  runtimeService.createProcessInstanceById(processDefinitionId).startBeforeActivity("theTask").execute();
		  fail("Exception expected: It should not be possible to start a process instance");
		}
		catch (AuthorizationException e)
		{

		  // then
		  assertTextPresent("The user with id 'test' does not have 'CREATE_INSTANCE' permission on resource 'oneTaskProcess' of type 'ProcessDefinition'", e.Message);
		}
	  }

	  public virtual void testStartProcessInstanceAtActivitiesByIdWithCreateInstancesPermissionOnProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, CREATE_INSTANCE);

		string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

		try
		{
		  // when
		  runtimeService.createProcessInstanceById(processDefinitionId).startBeforeActivity("theTask").execute();
		  fail("Exception expected: It should not be possible to start a process instance");
		}
		catch (AuthorizationException e)
		{

		  // then
		  assertTextPresent("The user with id 'test' does not have 'CREATE' permission on resource 'ProcessInstance'", e.Message);
		}
	  }

	  // start process instance by message //////////////////////////////////////////////

	  public virtual void testStartProcessInstanceByMessageWithoutAuthorization()
	  {
		// given
		// no authorization to start a process instance

		try
		{
		  // when
		  runtimeService.startProcessInstanceByMessage("startInvoiceMessage");
		  fail("Exception expected: It should not be possible to start a process instance");
		}
		catch (AuthorizationException e)
		{

		  // then
		  assertTextPresent("The user with id 'test' does not have 'CREATE' permission on resource 'ProcessInstance'", e.Message);
		}
	  }

	  public virtual void testStartProcessInstanceByMessageWithCreatePermissionOnProcessInstance()
	  {
		// given
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);

		try
		{
		  // when
		  runtimeService.startProcessInstanceByMessage("startInvoiceMessage");
		  fail("Exception expected: It should not be possible to start a process instance");
		}
		catch (AuthorizationException e)
		{

		  // then
		  assertTextPresent("The user with id 'test' does not have 'CREATE_INSTANCE' permission on resource 'messageStartProcess' of type 'ProcessDefinition'", e.Message);
		}
	  }

	  public virtual void testStartProcessInstanceByMessageWithCreateInstancesPermissionOnProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, MESSAGE_START_PROCESS_KEY, userId, CREATE_INSTANCE);

		try
		{
		  // when
		  runtimeService.startProcessInstanceByMessage("startInvoiceMessage");
		  fail("Exception expected: It should not be possible to start a process instance");
		}
		catch (AuthorizationException e)
		{

		  // then
		  assertTextPresent("The user with id 'test' does not have 'CREATE' permission on resource 'ProcessInstance'", e.Message);
		}
	  }

	  public virtual void testStartProcessInstanceByMessage()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, MESSAGE_START_PROCESS_KEY, userId, CREATE_INSTANCE);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);

		// when
		runtimeService.startProcessInstanceByMessage("startInvoiceMessage");

		// then
		disableAuthorization();
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();
		verifyQueryResults(query, 1);
		enableAuthorization();
	  }

	  // start process instance by message and process definition id /////////////////////////////

	  public virtual void testStartProcessInstanceByMessageAndProcDefIdWithoutAuthorization()
	  {
		// given
		// no authorization to start a process instance

		string processDefinitionId = selectProcessDefinitionByKey(MESSAGE_START_PROCESS_KEY).Id;

		try
		{
		  // when
		  runtimeService.startProcessInstanceByMessageAndProcessDefinitionId("startInvoiceMessage", processDefinitionId);
		  fail("Exception expected: It should not be possible to start a process instance");
		}
		catch (AuthorizationException e)
		{

		  // then
		  assertTextPresent("The user with id 'test' does not have 'CREATE' permission on resource 'ProcessInstance'", e.Message);
		}
	  }

	  public virtual void testStartProcessInstanceByMessageAndProcDefIdWithCreatePermissionOnProcessInstance()
	  {
		// given
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);

		string processDefinitionId = selectProcessDefinitionByKey(MESSAGE_START_PROCESS_KEY).Id;

		try
		{
		  // when
		  runtimeService.startProcessInstanceByMessageAndProcessDefinitionId("startInvoiceMessage", processDefinitionId);
		  fail("Exception expected: It should not be possible to start a process instance");
		}
		catch (AuthorizationException e)
		{

		  // then
		  assertTextPresent("The user with id 'test' does not have 'CREATE_INSTANCE' permission on resource 'messageStartProcess' of type 'ProcessDefinition'", e.Message);
		}
	  }

	  public virtual void testStartProcessInstanceByMessageAndProcDefIdWithCreateInstancesPermissionOnProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, MESSAGE_START_PROCESS_KEY, userId, CREATE_INSTANCE);

		string processDefinitionId = selectProcessDefinitionByKey(MESSAGE_START_PROCESS_KEY).Id;

		try
		{
		  // when
		  runtimeService.startProcessInstanceByMessageAndProcessDefinitionId("startInvoiceMessage", processDefinitionId);
		  fail("Exception expected: It should not be possible to start a process instance");
		}
		catch (AuthorizationException e)
		{

		  // then
		  assertTextPresent("The user with id 'test' does not have 'CREATE' permission on resource 'ProcessInstance'", e.Message);
		}
	  }

	  public virtual void testStartProcessInstanceByMessageAndProcDefId()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, MESSAGE_START_PROCESS_KEY, userId, CREATE_INSTANCE);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);

		string processDefinitionId = selectProcessDefinitionByKey(MESSAGE_START_PROCESS_KEY).Id;

		// when
		runtimeService.startProcessInstanceByMessageAndProcessDefinitionId("startInvoiceMessage", processDefinitionId);

		// then
		disableAuthorization();
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();
		verifyQueryResults(query, 1);
		enableAuthorization();
	  }

	  // delete process instance /////////////////////////////

	  public virtual void testDeleteProcessInstanceWithoutAuthorization()
	  {
		// given
		// no authorization to delete a process instance

		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;

		try
		{
		  // when
		  runtimeService.deleteProcessInstance(processInstanceId, null);
		  fail("Exception expected: It should not be possible to delete a process instance");
		}
		catch (AuthorizationException e)
		{

		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(DELETE.Name, message);
		  assertTextPresent(processInstanceId, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(DELETE_INSTANCE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testDeleteProcessInstanceWithDeletePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, DELETE);

		// when
		runtimeService.deleteProcessInstance(processInstanceId, null);

		// then
		disableAuthorization();
		assertProcessEnded(processInstanceId);
		enableAuthorization();
	  }

	  public virtual void testDeleteProcessInstanceWithDeletePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, DELETE);

		// when
		runtimeService.deleteProcessInstance(processInstanceId, null);

		// then
		disableAuthorization();
		assertProcessEnded(processInstanceId);
		enableAuthorization();
	  }

	  public virtual void testDeleteProcessInstanceWithDeleteInstancesPermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, DELETE_INSTANCE);

		// when
		runtimeService.deleteProcessInstance(processInstanceId, null);

		// then
		disableAuthorization();
		assertProcessEnded(processInstanceId);
		enableAuthorization();
	  }

	  public virtual void testDeleteProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, DELETE);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, DELETE_INSTANCE);

		// when
		runtimeService.deleteProcessInstance(processInstanceId, null);

		// then
		disableAuthorization();
		assertProcessEnded(processInstanceId);
		enableAuthorization();
	  }

	  // get active activity ids ///////////////////////////////////

	  public virtual void testGetActiveActivityIdsWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;

		try
		{
		  // when
		  runtimeService.getActiveActivityIds(processInstanceId);
		  fail("Exception expected: It should not be possible to retrieve active ativity ids");
		}
		catch (AuthorizationException)
		{

		  // then
	//      String message = e.getMessage();
	//      assertTextPresent(userId, message);
	//      assertTextPresent(READ.getName(), message);
	//      assertTextPresent(processInstanceId, message);
	//      assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		}
	  }

	  public virtual void testGetActiveActivityIdsWithReadPermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		IList<string> activityIds = runtimeService.getActiveActivityIds(processInstanceId);

		// then
		assertNotNull(activityIds);
		assertFalse(activityIds.Count == 0);
	  }

	  public virtual void testGetActiveActivityIdsWithReadPermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);

		// when
		IList<string> activityIds = runtimeService.getActiveActivityIds(processInstanceId);

		// then
		assertNotNull(activityIds);
		assertFalse(activityIds.Count == 0);
	  }

	  public virtual void testGetActiveActivityIdsWithReadInstancesPermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE);

		// when
		IList<string> activityIds = runtimeService.getActiveActivityIds(processInstanceId);

		// then
		assertNotNull(activityIds);
		assertFalse(activityIds.Count == 0);
	  }

	  public virtual void testGetActiveActivityIds()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE);

		// when
		IList<string> activityIds = runtimeService.getActiveActivityIds(processInstanceId);

		// then
		assertNotNull(activityIds);
		assertFalse(activityIds.Count == 0);
	  }

	  // get activity instance ///////////////////////////////////////////

	  public virtual void testGetActivityInstanceWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;

		try
		{
		  // when
		  runtimeService.getActivityInstance(processInstanceId);
		  fail("Exception expected: It should not be possible to retrieve ativity instances");
		}
		catch (AuthorizationException e)
		{

		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(processInstanceId, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(READ_INSTANCE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testGetActivityInstanceWithReadPermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		ActivityInstance activityInstance = runtimeService.getActivityInstance(processInstanceId);

		// then
		assertNotNull(activityInstance);
	  }

	  public virtual void testGetActivityInstanceWithReadPermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);

		// when
		ActivityInstance activityInstance = runtimeService.getActivityInstance(processInstanceId);

		// then
		assertNotNull(activityInstance);
	  }

	  public virtual void testGetActivityInstanceWithReadInstancesPermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE);

		// when
		ActivityInstance activityInstance = runtimeService.getActivityInstance(processInstanceId);

		// then
		assertNotNull(activityInstance);
	  }

	  public virtual void testGetActivityInstanceIds()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE);

		// when
		ActivityInstance activityInstance = runtimeService.getActivityInstance(processInstanceId);

		// then
		assertNotNull(activityInstance);
	  }

	  // signal execution ///////////////////////////////////////////

	  public virtual void testSignalWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;

		try
		{
		  // when
		  runtimeService.signal(processInstanceId);
		  fail("Exception expected: It should not be possible to signal an execution");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(processInstanceId, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testSignalWithUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		// when
		runtimeService.signal(processInstanceId);

		// then
		assertProcessEnded(processInstanceId);
	  }

	  public virtual void testSignalWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		// when
		runtimeService.signal(processInstanceId);

		// then
		assertProcessEnded(processInstanceId);
	  }

	  public virtual void testSignalWithUpdateInstancesPermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE);

		// when
		runtimeService.signal(processInstanceId);

		// then
		assertProcessEnded(processInstanceId);
	  }

	  public virtual void testSignal()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE);

		// when
		ActivityInstance activityInstance = runtimeService.getActivityInstance(processInstanceId);

		// then
		assertNotNull(activityInstance);
	  }

	  // signal event received //////////////////////////////////////

	  public virtual void testSignalEventReceivedWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY).Id;

		try
		{
		  // when
		  runtimeService.signalEventReceived("alert");
		  fail("Exception expected: It should not be possible to trigger a signal event");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(processInstanceId, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(SIGNAL_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testSignalEventReceivedWithUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		// when
		runtimeService.signalEventReceived("alert");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("taskAfterBoundaryEvent", task.TaskDefinitionKey);
	  }

	  public virtual void testSignalEventReceivedWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		// when
		runtimeService.signalEventReceived("alert");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("taskAfterBoundaryEvent", task.TaskDefinitionKey);
	  }

	  public virtual void testSignalEventReceivedWithUpdateInstancesPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, SIGNAL_BOUNDARY_PROCESS_KEY, userId, UPDATE_INSTANCE);

		// when
		runtimeService.signalEventReceived("alert");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("taskAfterBoundaryEvent", task.TaskDefinitionKey);
	  }

	  public virtual void testSignalEventReceived()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);
		createGrantAuthorization(PROCESS_DEFINITION, SIGNAL_BOUNDARY_PROCESS_KEY, userId, UPDATE_INSTANCE);

		// when
		runtimeService.signalEventReceived("alert");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("taskAfterBoundaryEvent", task.TaskDefinitionKey);
	  }

	  public virtual void testSignalEventReceivedTwoExecutionsShouldFail()
	  {
		// given
		string firstProcessInstanceId = startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY).Id;
		string secondProcessInstanceId = startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_INSTANCE, firstProcessInstanceId, userId, UPDATE);

		try
		{
		  // when
		  runtimeService.signalEventReceived("alert");
		  fail("Exception expected: It should not be possible to trigger a signal event");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(secondProcessInstanceId, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(SIGNAL_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testSignalEventReceivedTwoExecutionsShouldSuccess()
	  {
		// given
		string firstProcessInstanceId = startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY).Id;
		string secondProcessInstanceId = startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_INSTANCE, firstProcessInstanceId, userId, UPDATE);
		createGrantAuthorization(PROCESS_INSTANCE, secondProcessInstanceId, userId, UPDATE);

		// when
		runtimeService.signalEventReceived("alert");

		// then
		disableAuthorization();
		IList<Task> tasks = taskService.createTaskQuery().list();
		assertFalse(tasks.Count == 0);
		foreach (Task task in tasks)
		{
		  assertEquals("taskAfterBoundaryEvent", task.TaskDefinitionKey);
		}
		enableAuthorization();
	  }

	  // signal event received by execution id //////////////////////////////////////

	  public virtual void testSignalEventReceivedByExecutionIdWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY).Id;
		string executionId = selectSingleTask().ExecutionId;

		try
		{
		  // when
		  runtimeService.signalEventReceived("alert", executionId);
		  fail("Exception expected: It should not be possible to trigger a signal event");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(processInstanceId, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(SIGNAL_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testSignalEventReceivedByExecutionIdWithUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		string executionId = selectSingleTask().ExecutionId;

		// when
		runtimeService.signalEventReceived("alert", executionId);

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("taskAfterBoundaryEvent", task.TaskDefinitionKey);
	  }

	  public virtual void testSignalEventReceivedByExecutionIdWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		string executionId = selectSingleTask().ExecutionId;

		// when
		runtimeService.signalEventReceived("alert", executionId);

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("taskAfterBoundaryEvent", task.TaskDefinitionKey);
	  }

	  public virtual void testSignalEventReceivedByExecutionIdWithUpdateInstancesPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, SIGNAL_BOUNDARY_PROCESS_KEY, userId, UPDATE_INSTANCE);

		string executionId = selectSingleTask().ExecutionId;

		// when
		runtimeService.signalEventReceived("alert", executionId);

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("taskAfterBoundaryEvent", task.TaskDefinitionKey);
	  }

	  public virtual void testSignalEventReceivedByExecutionId()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);
		createGrantAuthorization(PROCESS_DEFINITION, SIGNAL_BOUNDARY_PROCESS_KEY, userId, UPDATE_INSTANCE);

		string executionId = selectSingleTask().ExecutionId;

		// when
		runtimeService.signalEventReceived("alert", executionId);

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("taskAfterBoundaryEvent", task.TaskDefinitionKey);
	  }

	  public virtual void testStartProcessInstanceBySignalEventReceivedWithoutAuthorization()
	  {
		// given
		// no authorization to start a process instance

		try
		{
		  // when
		  runtimeService.signalEventReceived("warning");
		  fail("Exception expected");
		}
		catch (AuthorizationException e)
		{
		  // then
		  assertTextPresent("The user with id 'test' does not have 'CREATE' permission on resource 'ProcessInstance'", e.Message);
		}
	  }

	  public virtual void testStartProcessInstanceBySignalEventReceivedWithCreatePermissionOnProcessInstance()
	  {
		// given
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);

		try
		{
		  // when
		  runtimeService.signalEventReceived("warning");
		  fail("Exception expected");
		}
		catch (AuthorizationException e)
		{
		  // then
		  assertTextPresent("The user with id 'test' does not have 'CREATE_INSTANCE' permission on resource 'signalStartProcess' of type 'ProcessDefinition'", e.Message);
		}
	  }

	  public virtual void testStartProcessInstanceBySignalEventReceived()
	  {
		// given
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);
		createGrantAuthorization(PROCESS_DEFINITION, SIGNAL_START_PROCESS_KEY, userId, CREATE_INSTANCE);

		// when
		runtimeService.signalEventReceived("warning");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("task", task.TaskDefinitionKey);
	  }

	  /// <summary>
	  /// currently the ThrowSignalEventActivityBehavior does not check authorization
	  /// </summary>
	  public virtual void FAILING_testStartProcessInstanceByThrowSignalEventWithCreatePermissionOnProcessInstance()
	  {
		// given
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);
		createGrantAuthorization(PROCESS_DEFINITION, THROW_WARNING_SIGNAL_PROCESS_KEY, userId, CREATE_INSTANCE);

		try
		{
		  // when
		  runtimeService.startProcessInstanceByKey(THROW_WARNING_SIGNAL_PROCESS_KEY);
		  fail("Exception expected");
		}
		catch (AuthorizationException e)
		{
		  // then
		  assertTextPresent("The user with id 'test' does not have 'CREATE_INSTANCE' permission on resource 'signalStartProcess' of type 'ProcessDefinition'", e.Message);
		}
	  }

	  public virtual void testStartProcessInstanceByThrowSignalEvent()
	  {
		// given
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);
		createGrantAuthorization(PROCESS_DEFINITION, SIGNAL_START_PROCESS_KEY, userId, CREATE_INSTANCE);
		createGrantAuthorization(PROCESS_DEFINITION, THROW_WARNING_SIGNAL_PROCESS_KEY, userId, CREATE_INSTANCE);

		// when
		runtimeService.startProcessInstanceByKey(THROW_WARNING_SIGNAL_PROCESS_KEY);

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("task", task.TaskDefinitionKey);
	  }

	  /// <summary>
	  /// currently the ThrowSignalEventActivityBehavior does not check authorization
	  /// </summary>
	  public virtual void FAILING_testThrowSignalEventWithoutAuthorization()
	  {
		// given
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);
		createGrantAuthorization(PROCESS_DEFINITION, THROW_ALERT_SIGNAL_PROCESS_KEY, userId, CREATE_INSTANCE);

		string processInstanceId = startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY).Id;

		try
		{
		  // when
		  runtimeService.startProcessInstanceByKey(THROW_ALERT_SIGNAL_PROCESS_KEY);

		  fail("Exception expected");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(processInstanceId, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(SIGNAL_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testThrowSignalEvent()
	  {
		// given
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);
		createGrantAuthorization(PROCESS_DEFINITION, THROW_ALERT_SIGNAL_PROCESS_KEY, userId, CREATE_INSTANCE);

		string processInstanceId = startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);
		createGrantAuthorization(PROCESS_DEFINITION, SIGNAL_BOUNDARY_PROCESS_KEY, userId, UPDATE_INSTANCE);

		// when
		runtimeService.startProcessInstanceByKey(THROW_ALERT_SIGNAL_PROCESS_KEY);

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("taskAfterBoundaryEvent", task.TaskDefinitionKey);
	  }

	  // message event received /////////////////////////////////////

	  public virtual void testMessageEventReceivedWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
		string executionId = selectSingleTask().ExecutionId;

		try
		{
		  // when
		  runtimeService.messageEventReceived("boundaryInvoiceMessage", executionId);
		  fail("Exception expected: It should not be possible to trigger a message event");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(processInstanceId, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(MESSAGE_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testMessageEventReceivedByExecutionIdWithUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		string executionId = selectSingleTask().ExecutionId;

		// when
		runtimeService.messageEventReceived("boundaryInvoiceMessage", executionId);

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("taskAfterBoundaryEvent", task.TaskDefinitionKey);
	  }

	  public virtual void testMessageEventReceivedByExecutionIdWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		startProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		string executionId = selectSingleTask().ExecutionId;

		// when
		runtimeService.messageEventReceived("boundaryInvoiceMessage", executionId);

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("taskAfterBoundaryEvent", task.TaskDefinitionKey);
	  }

	  public virtual void testMessageEventReceivedByExecutionIdWithUpdateInstancesPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, MESSAGE_BOUNDARY_PROCESS_KEY, userId, UPDATE_INSTANCE);

		string executionId = selectSingleTask().ExecutionId;

		// when
		runtimeService.messageEventReceived("boundaryInvoiceMessage", executionId);

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("taskAfterBoundaryEvent", task.TaskDefinitionKey);
	  }

	  public virtual void testMessageEventReceivedByExecutionId()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);
		createGrantAuthorization(PROCESS_DEFINITION, MESSAGE_BOUNDARY_PROCESS_KEY, userId, UPDATE_INSTANCE);

		string executionId = selectSingleTask().ExecutionId;

		// when
		runtimeService.messageEventReceived("boundaryInvoiceMessage", executionId);

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("taskAfterBoundaryEvent", task.TaskDefinitionKey);
	  }

	  // correlate message (correlates to an execution) /////////////

	  public virtual void testCorrelateMessageExecutionWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;

		try
		{
		  // when
		  runtimeService.correlateMessage("boundaryInvoiceMessage");
		  fail("Exception expected: It should not be possible to correlate a message.");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(processInstanceId, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(MESSAGE_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testCorrelateMessageExecutionWithUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		// when
		runtimeService.correlateMessage("boundaryInvoiceMessage");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("taskAfterBoundaryEvent", task.TaskDefinitionKey);
	  }

	  public virtual void testCorrelateMessageExecutionWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		startProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		// when
		runtimeService.correlateMessage("boundaryInvoiceMessage");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("taskAfterBoundaryEvent", task.TaskDefinitionKey);
	  }

	  public virtual void testCorrelateMessageExecutionWithUpdateInstancesPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, MESSAGE_BOUNDARY_PROCESS_KEY, userId, UPDATE_INSTANCE);

		// when
		runtimeService.correlateMessage("boundaryInvoiceMessage");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("taskAfterBoundaryEvent", task.TaskDefinitionKey);
	  }

	  public virtual void testCorrelateMessageExecution()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);
		createGrantAuthorization(PROCESS_DEFINITION, MESSAGE_BOUNDARY_PROCESS_KEY, userId, UPDATE_INSTANCE);

		// when
		runtimeService.correlateMessage("boundaryInvoiceMessage");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("taskAfterBoundaryEvent", task.TaskDefinitionKey);
	  }

	  // correlate message (correlates to a process definition) /////////////

	  public virtual void testCorrelateMessageProcessDefinitionWithoutAuthorization()
	  {
		// given

		try
		{
		  // when
		  runtimeService.correlateMessage("startInvoiceMessage");
		  fail("Exception expected: It should not be possible to correlate a message.");
		}
		catch (AuthorizationException e)
		{
		  // then
		  assertTextPresent("The user with id 'test' does not have 'CREATE' permission on resource 'ProcessInstance'", e.Message);
		}
	  }

	  public virtual void testCorrelateMessageProcessDefinitionWithCreatePermissionOnProcessInstance()
	  {
		// given
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);

		try
		{
		  // when
		  runtimeService.correlateMessage("startInvoiceMessage");
		  fail("Exception expected: It should not be possible to correlate a message.");
		}
		catch (AuthorizationException e)
		{

		  // then
		  assertTextPresent("The user with id 'test' does not have 'CREATE_INSTANCE' permission on resource 'messageStartProcess' of type 'ProcessDefinition'", e.Message);
		}
	  }

	  public virtual void testCorrelateMessageProcessDefinitionWithCreateInstancesPermissionOnProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, MESSAGE_START_PROCESS_KEY, userId, CREATE_INSTANCE);

		try
		{
		  // when
		  runtimeService.correlateMessage("startInvoiceMessage");
		  fail("Exception expected: It should not be possible to correlate a message.");
		}
		catch (AuthorizationException e)
		{

		  // then
		  assertTextPresent("The user with id 'test' does not have 'CREATE' permission on resource 'ProcessInstance'", e.Message);
		}
	  }

	  public virtual void testCorrelateMessageProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);
		createGrantAuthorization(PROCESS_DEFINITION, MESSAGE_START_PROCESS_KEY, userId, CREATE_INSTANCE);

		// when
		runtimeService.correlateMessage("startInvoiceMessage");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("task", task.TaskDefinitionKey);
	  }

	  // correlate all (correlates to executions) ///////////////////

	  public virtual void testCorrelateAllExecutionWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;

		try
		{
		  // when
		  runtimeService.createMessageCorrelation("boundaryInvoiceMessage").correlateAll();
		  fail("Exception expected: It should not be possible to correlate a message.");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(processInstanceId, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(MESSAGE_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testCorrelateAllExecutionWithUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		// when
		runtimeService.createMessageCorrelation("boundaryInvoiceMessage").correlateAll();

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("taskAfterBoundaryEvent", task.TaskDefinitionKey);
	  }

	  public virtual void testCorrelateAllExecutionWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		startProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		// when
		runtimeService.createMessageCorrelation("boundaryInvoiceMessage").correlateAll();

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("taskAfterBoundaryEvent", task.TaskDefinitionKey);
	  }

	  public virtual void testCorrelateAllExecutionWithUpdateInstancesPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, MESSAGE_BOUNDARY_PROCESS_KEY, userId, UPDATE_INSTANCE);

		// when
		runtimeService.createMessageCorrelation("boundaryInvoiceMessage").correlateAll();

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("taskAfterBoundaryEvent", task.TaskDefinitionKey);
	  }

	  public virtual void testCorrelateAllExecution()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);
		createGrantAuthorization(PROCESS_DEFINITION, MESSAGE_BOUNDARY_PROCESS_KEY, userId, UPDATE_INSTANCE);

		// when
		runtimeService.createMessageCorrelation("boundaryInvoiceMessage").correlateAll();

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("taskAfterBoundaryEvent", task.TaskDefinitionKey);
	  }

	  public virtual void testCorrelateAllTwoExecutionsShouldFail()
	  {
		// given
		string firstProcessInstanceId = startProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
		string secondProcessInstanceId = startProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_INSTANCE, firstProcessInstanceId, userId, UPDATE);

		try
		{
		  // when
		  runtimeService.createMessageCorrelation("boundaryInvoiceMessage").correlateAll();
		  fail("Exception expected: It should not be possible to trigger a signal event");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(secondProcessInstanceId, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(MESSAGE_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testCorrelateAllTwoExecutionsShouldSuccess()
	  {
		// given
		string firstProcessInstanceId = startProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
		string secondProcessInstanceId = startProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_INSTANCE, firstProcessInstanceId, userId, UPDATE);
		createGrantAuthorization(PROCESS_INSTANCE, secondProcessInstanceId, userId, UPDATE);

		// when
		runtimeService.createMessageCorrelation("boundaryInvoiceMessage").correlateAll();

		// then
		disableAuthorization();
		IList<Task> tasks = taskService.createTaskQuery().list();
		assertFalse(tasks.Count == 0);
		foreach (Task task in tasks)
		{
		  assertEquals("taskAfterBoundaryEvent", task.TaskDefinitionKey);
		}
		enableAuthorization();
	  }

	  // correlate all (correlates to a process definition) /////////////

	  public virtual void testCorrelateAllProcessDefinitionWithoutAuthorization()
	  {
		// given

		try
		{
		  // when
		  runtimeService.createMessageCorrelation("startInvoiceMessage").correlateAll();
		  fail("Exception expected: It should not be possible to correlate a message.");
		}
		catch (AuthorizationException e)
		{
		  // then
		  assertTextPresent("The user with id 'test' does not have 'CREATE' permission on resource 'ProcessInstance'", e.Message);
		}
	  }

	  public virtual void testCorrelateAllProcessDefinitionWithCreatePermissionOnProcessInstance()
	  {
		// given
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);

		try
		{
		  // when
		  runtimeService.createMessageCorrelation("startInvoiceMessage").correlateAll();
		  fail("Exception expected: It should not be possible to correlate a message.");
		}
		catch (AuthorizationException e)
		{

		  // then
		  assertTextPresent("The user with id 'test' does not have 'CREATE_INSTANCE' permission on resource 'messageStartProcess' of type 'ProcessDefinition'", e.Message);
		}
	  }

	  public virtual void testCorrelateAllProcessDefinitionWithCreateInstancesPermissionOnProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, MESSAGE_START_PROCESS_KEY, userId, CREATE_INSTANCE);

		try
		{
		  // when
		  runtimeService.createMessageCorrelation("startInvoiceMessage").correlateAll();
		  fail("Exception expected: It should not be possible to correlate a message.");
		}
		catch (AuthorizationException e)
		{

		  // then
		  assertTextPresent("The user with id 'test' does not have 'CREATE' permission on resource 'ProcessInstance'", e.Message);
		}
	  }

	  public virtual void testCorrelateAllProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);
		createGrantAuthorization(PROCESS_DEFINITION, MESSAGE_START_PROCESS_KEY, userId, CREATE_INSTANCE);

		// when
		runtimeService.createMessageCorrelation("startInvoiceMessage").correlateAll();

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("task", task.TaskDefinitionKey);
	  }

	  // suspend process instance by id /////////////////////////////

	  public virtual void testSuspendProcessInstanceByIdWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;

		try
		{
		  // when
		  runtimeService.suspendProcessInstanceById(processInstanceId);
		  fail("Exception expected: It should not be posssible to suspend a process instance.");
		}
		catch (AuthorizationException e)
		{

		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(processInstanceId, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		  assertTextPresent(SUSPEND.Name, message);
		  assertTextPresent(SUSPEND_INSTANCE.Name, message);
		}
	  }

	  public virtual void testSuspendProcessInstanceByIdWithUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		// when
		runtimeService.suspendProcessInstanceById(processInstanceId);

		// then
		ProcessInstance instance = selectSingleProcessInstance();
		assertTrue(instance.Suspended);
	  }

	  public virtual void testSuspendProcessInstanceByIdWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		// when
		runtimeService.suspendProcessInstanceById(processInstanceId);

		// then
		ProcessInstance instance = selectSingleProcessInstance();
		assertTrue(instance.Suspended);
	  }

	  public virtual void testSuspendProcessInstanceByIdWithUpdateInstancesPermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE);

		// when
		runtimeService.suspendProcessInstanceById(processInstanceId);

		// then
		ProcessInstance instance = selectSingleProcessInstance();
		assertTrue(instance.Suspended);
	  }

	  public virtual void testSuspendProcessInstanceById()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE, SUSPEND);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE, SUSPEND_INSTANCE);

		// when
		runtimeService.suspendProcessInstanceById(processInstanceId);

		// then
		ProcessInstance instance = selectSingleProcessInstance();
		assertTrue(instance.Suspended);
	  }

	  public virtual void testSuspendProcessInstanceByIdWithSuspendPermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, SUSPEND);

		// when
		runtimeService.suspendProcessInstanceById(processInstanceId);

		// then
		ProcessInstance instance = selectSingleProcessInstance();
		assertTrue(instance.Suspended);
	  }

	  public virtual void testSuspendProcessInstanceByIdWithSuspendPermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, SUSPEND);

		// when
		runtimeService.suspendProcessInstanceById(processInstanceId);

		// then
		ProcessInstance instance = selectSingleProcessInstance();
		assertTrue(instance.Suspended);
	  }

	  public virtual void testSuspendProcessInstanceByIdWithSuspendInstancesPermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, SUSPEND_INSTANCE);

		// when
		runtimeService.suspendProcessInstanceById(processInstanceId);

		// then
		ProcessInstance instance = selectSingleProcessInstance();
		assertTrue(instance.Suspended);
	  }

	  // activate process instance by id /////////////////////////////

	  public virtual void testActivateProcessInstanceByIdWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		suspendProcessInstanceById(processInstanceId);

		try
		{
		  // when
		  runtimeService.activateProcessInstanceById(processInstanceId);
		  fail("Exception expected: It should not be posssible to activate a process instance.");
		}
		catch (AuthorizationException e)
		{

		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(processInstanceId, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		  assertTextPresent(SUSPEND.Name, message);
		  assertTextPresent(SUSPEND_INSTANCE.Name, message);
		}
	  }

	  public virtual void testActivateProcessInstanceByIdWithUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		suspendProcessInstanceById(processInstanceId);
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		// when
		runtimeService.activateProcessInstanceById(processInstanceId);

		// then
		ProcessInstance instance = selectSingleProcessInstance();
		assertFalse(instance.Suspended);
	  }

	  public virtual void testActivateProcessInstanceByIdWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		suspendProcessInstanceById(processInstanceId);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		// when
		runtimeService.activateProcessInstanceById(processInstanceId);

		// then
		ProcessInstance instance = selectSingleProcessInstance();
		assertFalse(instance.Suspended);
	  }

	  public virtual void testActivateProcessInstanceByIdWithUpdateInstancesPermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		suspendProcessInstanceById(processInstanceId);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE);

		// when
		runtimeService.activateProcessInstanceById(processInstanceId);

		// then
		ProcessInstance instance = selectSingleProcessInstance();
		assertFalse(instance.Suspended);
	  }

	  public virtual void testActivateProcessInstanceById()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		suspendProcessInstanceById(processInstanceId);
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE, SUSPEND);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE, SUSPEND_INSTANCE);

		// when
		runtimeService.activateProcessInstanceById(processInstanceId);

		// then
		ProcessInstance instance = selectSingleProcessInstance();
		assertFalse(instance.Suspended);
	  }

	  public virtual void testActivateProcessInstanceByIdWithSuspendPermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		suspendProcessInstanceById(processInstanceId);
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, SUSPEND);

		// when
		runtimeService.activateProcessInstanceById(processInstanceId);

		// then
		ProcessInstance instance = selectSingleProcessInstance();
		assertFalse(instance.Suspended);
	  }

	  public virtual void testActivateProcessInstanceByIdWithSuspendPermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		suspendProcessInstanceById(processInstanceId);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, SUSPEND);

		// when
		runtimeService.activateProcessInstanceById(processInstanceId);

		// then
		ProcessInstance instance = selectSingleProcessInstance();
		assertFalse(instance.Suspended);
	  }

	  public virtual void testActivateProcessInstanceByIdWithSuspendInstancesPermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		suspendProcessInstanceById(processInstanceId);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, SUSPEND_INSTANCE);

		// when
		runtimeService.activateProcessInstanceById(processInstanceId);

		// then
		ProcessInstance instance = selectSingleProcessInstance();
		assertFalse(instance.Suspended);
	  }

	  // suspend process instance by process definition id /////////////////////////////

	  public virtual void testSuspendProcessInstanceByProcessDefinitionIdWithoutAuthorization()
	  {
		// given
		string processDefinitionId = startProcessInstanceByKey(PROCESS_KEY).ProcessDefinitionId;

		try
		{
		  // when
		  runtimeService.suspendProcessInstanceByProcessDefinitionId(processDefinitionId);
		  fail("Exception expected: It should not be posssible to suspend a process instance.");
		}
		catch (AuthorizationException e)
		{

		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		  assertTextPresent(SUSPEND.Name, message);
		  assertTextPresent(SUSPEND_INSTANCE.Name, message);
		}
	  }

	  public virtual void testSuspendProcessInstanceByProcessDefinitionIdWithUpdatePermissionOnProcessInstance()
	  {
		// given
		ProcessInstance instance = startProcessInstanceByKey(PROCESS_KEY);
		string processInstanceId = instance.Id;
		string processDefinitionId = instance.ProcessDefinitionId;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		try
		{
		  // when
		  runtimeService.suspendProcessInstanceByProcessDefinitionId(processDefinitionId);
		  fail("Exception expected: It should not be posssible to suspend a process instance.");
		}
		catch (AuthorizationException e)
		{

		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(SUSPEND_INSTANCE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		  assertTextPresent(SUSPEND.Name, message);
		  assertTextPresent(UPDATE.Name, message);
		}
	  }

	  public virtual void testSuspendProcessInstanceByProcessDefinitionIdWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		ProcessInstance instance = startProcessInstanceByKey(PROCESS_KEY);
		string processDefinitionId = instance.ProcessDefinitionId;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		// when
		runtimeService.suspendProcessInstanceByProcessDefinitionId(processDefinitionId);

		// then
		instance = selectSingleProcessInstance();
		assertTrue(instance.Suspended);
	  }

	  public virtual void testSuspendProcessInstanceByProcessDefinitionIdWithUpdateInstancesPermissionOnProcessDefinition()
	  {
		// given
		ProcessInstance instance = startProcessInstanceByKey(PROCESS_KEY);
		string processDefinitionId = instance.ProcessDefinitionId;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE);

		// when
		runtimeService.suspendProcessInstanceByProcessDefinitionId(processDefinitionId);

		// then
		instance = selectSingleProcessInstance();
		assertTrue(instance.Suspended);
	  }

	  public virtual void testSuspendProcessInstanceByProcessDefinitionId()
	  {
		// given
		ProcessInstance instance = startProcessInstanceByKey(PROCESS_KEY);
		string processInstanceId = instance.Id;
		string processDefinitionId = instance.ProcessDefinitionId;

		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE, SUSPEND);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE, SUSPEND_INSTANCE);

		// when
		runtimeService.suspendProcessInstanceByProcessDefinitionId(processDefinitionId);

		// then
		instance = selectSingleProcessInstance();
		assertTrue(instance.Suspended);
	  }

	  public virtual void testSuspendProcessInstanceByProcessDefinitionIdWithSuspendPermissionOnProcessInstance()
	  {
		// given
		ProcessInstance instance = startProcessInstanceByKey(PROCESS_KEY);
		string processInstanceId = instance.Id;
		string processDefinitionId = instance.ProcessDefinitionId;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, SUSPEND);

		try
		{
		  // when
		  runtimeService.suspendProcessInstanceByProcessDefinitionId(processDefinitionId);
		  fail("Exception expected: It should not be posssible to suspend a process instance.");
		}
		catch (AuthorizationException e)
		{

		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(SUSPEND_INSTANCE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		  assertTextPresent(SUSPEND.Name, message);
		  assertTextPresent(UPDATE.Name, message);
		}
	  }

	  public virtual void testSuspendProcessInstanceByProcessDefinitionIdWithSuspendPermissionOnAnyProcessInstance()
	  {
		// given
		ProcessInstance instance = startProcessInstanceByKey(PROCESS_KEY);
		string processDefinitionId = instance.ProcessDefinitionId;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, SUSPEND);

		// when
		runtimeService.suspendProcessInstanceByProcessDefinitionId(processDefinitionId);

		// then
		instance = selectSingleProcessInstance();
		assertTrue(instance.Suspended);
	  }

	  public virtual void testSuspendProcessInstanceByProcessDefinitionIdWithSuspendInstancesPermissionOnProcessDefinition()
	  {
		// given
		ProcessInstance instance = startProcessInstanceByKey(PROCESS_KEY);
		string processDefinitionId = instance.ProcessDefinitionId;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, SUSPEND_INSTANCE);

		// when
		runtimeService.suspendProcessInstanceByProcessDefinitionId(processDefinitionId);

		// then
		instance = selectSingleProcessInstance();
		assertTrue(instance.Suspended);
	  }


	  // activate process instance by process definition id /////////////////////////////

	  public virtual void testActivateProcessInstanceByProcessDefinitionIdWithoutAuthorization()
	  {
		// given
		ProcessInstance instance = startProcessInstanceByKey(PROCESS_KEY);
		string processInstanceId = instance.Id;
		string processDefinitionId = instance.ProcessDefinitionId;
		suspendProcessInstanceById(processInstanceId);

		try
		{
		  // when
		  runtimeService.activateProcessInstanceByProcessDefinitionId(processDefinitionId);
		  fail("Exception expected: It should not be posssible to suspend a process instance.");
		}
		catch (AuthorizationException e)
		{

		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		  assertTextPresent(SUSPEND.Name, message);
		  assertTextPresent(SUSPEND_INSTANCE.Name, message);
		}
	  }

	  public virtual void testActivateProcessInstanceByProcessDefinitionIdWithUpdatePermissionOnProcessInstance()
	  {
		// given
		ProcessInstance instance = startProcessInstanceByKey(PROCESS_KEY);
		string processInstanceId = instance.Id;
		string processDefinitionId = instance.ProcessDefinitionId;
		suspendProcessInstanceById(processInstanceId);

		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		try
		{
		  // when
		  runtimeService.activateProcessInstanceByProcessDefinitionId(processDefinitionId);
		  fail("Exception expected: It should not be posssible to suspend a process instance.");
		}
		catch (AuthorizationException e)
		{

		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(SUSPEND_INSTANCE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		  assertTextPresent(SUSPEND.Name, message);
		  assertTextPresent(UPDATE.Name, message);
		}
	  }

	  public virtual void testActivateProcessInstanceByProcessDefinitionIdWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		ProcessInstance instance = startProcessInstanceByKey(PROCESS_KEY);
		string processInstanceId = instance.Id;
		string processDefinitionId = instance.ProcessDefinitionId;
		suspendProcessInstanceById(processInstanceId);

		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		// when
		runtimeService.activateProcessInstanceByProcessDefinitionId(processDefinitionId);

		// then
		instance = selectSingleProcessInstance();
		assertFalse(instance.Suspended);
	  }

	  public virtual void testActivateProcessInstanceByProcessDefinitionIdWithUpdateInstancesPermissionOnProcessDefinition()
	  {
		// given
		ProcessInstance instance = startProcessInstanceByKey(PROCESS_KEY);
		string processInstanceId = instance.Id;
		string processDefinitionId = instance.ProcessDefinitionId;
		suspendProcessInstanceById(processInstanceId);

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE);

		// when
		runtimeService.activateProcessInstanceByProcessDefinitionId(processDefinitionId);

		// then
		instance = selectSingleProcessInstance();
		assertFalse(instance.Suspended);
	  }

	  public virtual void testActivateProcessInstanceByProcessDefinitionId()
	  {
		// given
		ProcessInstance instance = startProcessInstanceByKey(PROCESS_KEY);
		string processInstanceId = instance.Id;
		string processDefinitionId = instance.ProcessDefinitionId;
		suspendProcessInstanceById(processInstanceId);

		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE, SUSPEND);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE, SUSPEND_INSTANCE);

		// when
		runtimeService.activateProcessInstanceByProcessDefinitionId(processDefinitionId);

		// then
		instance = selectSingleProcessInstance();
		assertFalse(instance.Suspended);
	  }

	  public virtual void testActivateProcessInstanceByProcessDefinitionIdWithSuspendPermissionOnProcessInstance()
	  {
		// given
		ProcessInstance instance = startProcessInstanceByKey(PROCESS_KEY);
		string processInstanceId = instance.Id;
		string processDefinitionId = instance.ProcessDefinitionId;
		suspendProcessInstanceById(processInstanceId);

		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, SUSPEND);

		try
		{
		  // when
		  runtimeService.activateProcessInstanceByProcessDefinitionId(processDefinitionId);
		  fail("Exception expected: It should not be posssible to suspend a process instance.");
		}
		catch (AuthorizationException e)
		{

		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(SUSPEND_INSTANCE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		  assertTextPresent(SUSPEND.Name, message);
		  assertTextPresent(UPDATE.Name, message);
		}
	  }

	  public virtual void testActivateProcessInstanceByProcessDefinitionIdWithSuspendPermissionOnAnyProcessInstance()
	  {
		// given
		ProcessInstance instance = startProcessInstanceByKey(PROCESS_KEY);
		string processInstanceId = instance.Id;
		string processDefinitionId = instance.ProcessDefinitionId;
		suspendProcessInstanceById(processInstanceId);

		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, SUSPEND);

		// when
		runtimeService.activateProcessInstanceByProcessDefinitionId(processDefinitionId);

		// then
		instance = selectSingleProcessInstance();
		assertFalse(instance.Suspended);
	  }

	  public virtual void testActivateProcessInstanceByProcessDefinitionIdWithSuspendInstancesPermissionOnProcessDefinition()
	  {
		// given
		ProcessInstance instance = startProcessInstanceByKey(PROCESS_KEY);
		string processInstanceId = instance.Id;
		string processDefinitionId = instance.ProcessDefinitionId;
		suspendProcessInstanceById(processInstanceId);

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, SUSPEND_INSTANCE);

		// when
		runtimeService.activateProcessInstanceByProcessDefinitionId(processDefinitionId);

		// then
		instance = selectSingleProcessInstance();
		assertFalse(instance.Suspended);
	  }

	  // suspend process instance by process definition key /////////////////////////////

	  public virtual void testSuspendProcessInstanceByProcessDefinitionKeyWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);

		try
		{
		  // when
		  runtimeService.suspendProcessInstanceByProcessDefinitionKey(PROCESS_KEY);
		  fail("Exception expected: It should not be posssible to suspend a process instance.");
		}
		catch (AuthorizationException e)
		{

		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		  assertTextPresent(SUSPEND.Name, message);
		  assertTextPresent(SUSPEND_INSTANCE.Name, message);
		}
	  }

	  public virtual void testSuspendProcessInstanceByProcessDefinitionKeyWithUpdatePermissionOnProcessInstance()
	  {
		// given
		ProcessInstance instance = startProcessInstanceByKey(PROCESS_KEY);
		string processInstanceId = instance.Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		try
		{
		  // when
		  runtimeService.suspendProcessInstanceByProcessDefinitionKey(PROCESS_KEY);
		  fail("Exception expected: It should not be posssible to suspend a process instance.");
		}
		catch (AuthorizationException e)
		{

		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(SUSPEND_INSTANCE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		  assertTextPresent(SUSPEND.Name, message);
		  assertTextPresent(UPDATE.Name, message);
		}
	  }

	  public virtual void testSuspendProcessInstanceByProcessDefinitionKeyWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		// when
		runtimeService.suspendProcessInstanceByProcessDefinitionKey(PROCESS_KEY);

		// then
		ProcessInstance instance = selectSingleProcessInstance();
		assertTrue(instance.Suspended);
	  }

	  public virtual void testSuspendProcessInstanceByProcessDefinitionKeyWithUpdateInstancesPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE);

		// when
		runtimeService.suspendProcessInstanceByProcessDefinitionKey(PROCESS_KEY);

		// then
		ProcessInstance instance = selectSingleProcessInstance();
		assertTrue(instance.Suspended);
	  }

	  public virtual void testSuspendProcessInstanceByProcessDefinitionKey()
	  {
		// given
		ProcessInstance instance = startProcessInstanceByKey(PROCESS_KEY);
		string processInstanceId = instance.Id;

		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE);

		// when
		runtimeService.suspendProcessInstanceByProcessDefinitionKey(PROCESS_KEY);

		// then
		instance = selectSingleProcessInstance();
		assertTrue(instance.Suspended);
	  }

	  public virtual void testSuspendProcessInstanceByProcessDefinitionKeyWithSuspendPermissionOnProcessInstance()
	  {
		// given
		ProcessInstance instance = startProcessInstanceByKey(PROCESS_KEY);
		string processInstanceId = instance.Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, SUSPEND);

		try
		{
		  // when
		  runtimeService.suspendProcessInstanceByProcessDefinitionKey(PROCESS_KEY);
		  fail("Exception expected: It should not be posssible to suspend a process instance.");
		}
		catch (AuthorizationException e)
		{

		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(SUSPEND_INSTANCE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		  assertTextPresent(SUSPEND.Name, message);
		  assertTextPresent(UPDATE.Name, message);
		}
	  }

	  public virtual void testSuspendProcessInstanceByProcessDefinitionKeyWithSuspendPermissionOnAnyProcessInstance()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, SUSPEND);

		// when
		runtimeService.suspendProcessInstanceByProcessDefinitionKey(PROCESS_KEY);

		// then
		ProcessInstance instance = selectSingleProcessInstance();
		assertTrue(instance.Suspended);
	  }

	  public virtual void testSuspendProcessInstanceByProcessDefinitionKeyWithSuspendInstancesPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, SUSPEND_INSTANCE);

		// when
		runtimeService.suspendProcessInstanceByProcessDefinitionKey(PROCESS_KEY);

		// then
		ProcessInstance instance = selectSingleProcessInstance();
		assertTrue(instance.Suspended);
	  }

	  // activate process instance by process definition key /////////////////////////////

	  public virtual void testActivateProcessInstanceByProcessDefinitionKeyWithoutAuthorization()
	  {
		// given
		ProcessInstance instance = startProcessInstanceByKey(PROCESS_KEY);
		string processInstanceId = instance.Id;
		suspendProcessInstanceById(processInstanceId);

		try
		{
		  // when
		  runtimeService.activateProcessInstanceByProcessDefinitionKey(PROCESS_KEY);
		  fail("Exception expected: It should not be posssible to suspend a process instance.");
		}
		catch (AuthorizationException e)
		{

		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		  assertTextPresent(SUSPEND.Name, message);
		  assertTextPresent(SUSPEND_INSTANCE.Name, message);
		}
	  }

	  public virtual void testActivateProcessInstanceByProcessDefinitionKeyWithUpdatePermissionOnProcessInstance()
	  {
		// given
		ProcessInstance instance = startProcessInstanceByKey(PROCESS_KEY);
		string processInstanceId = instance.Id;
		suspendProcessInstanceById(processInstanceId);

		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		try
		{
		  // when
		  runtimeService.activateProcessInstanceByProcessDefinitionKey(PROCESS_KEY);
		  fail("Exception expected: It should not be posssible to suspend a process instance.");
		}
		catch (AuthorizationException e)
		{

		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(SUSPEND_INSTANCE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		  assertTextPresent(SUSPEND.Name, message);
		  assertTextPresent(UPDATE.Name, message);
		}
	  }

	  public virtual void testActivateProcessInstanceByProcessDefinitionKeyWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		ProcessInstance instance = startProcessInstanceByKey(PROCESS_KEY);
		string processInstanceId = instance.Id;
		suspendProcessInstanceById(processInstanceId);

		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		// when
		runtimeService.activateProcessInstanceByProcessDefinitionKey(PROCESS_KEY);

		// then
		instance = selectSingleProcessInstance();
		assertFalse(instance.Suspended);
	  }

	  public virtual void testActivateProcessInstanceByProcessDefinitionKeyWithUpdateInstancesPermissionOnProcessDefinition()
	  {
		// given
		ProcessInstance instance = startProcessInstanceByKey(PROCESS_KEY);
		string processInstanceId = instance.Id;
		suspendProcessInstanceById(processInstanceId);

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE);

		// when
		runtimeService.activateProcessInstanceByProcessDefinitionKey(PROCESS_KEY);

		// then
		instance = selectSingleProcessInstance();
		assertFalse(instance.Suspended);
	  }

	  public virtual void testActivateProcessInstanceByProcessDefinitionKey()
	  {
		// given
		ProcessInstance instance = startProcessInstanceByKey(PROCESS_KEY);
		string processInstanceId = instance.Id;
		suspendProcessInstanceById(processInstanceId);

		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE, SUSPEND);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE, SUSPEND_INSTANCE);

		// when
		runtimeService.activateProcessInstanceByProcessDefinitionKey(PROCESS_KEY);

		// then
		instance = selectSingleProcessInstance();
		assertFalse(instance.Suspended);
	  }

	  public virtual void testActivateProcessInstanceByProcessDefinitionKeyWithSuspendPermissionOnProcessInstance()
	  {
		// given
		ProcessInstance instance = startProcessInstanceByKey(PROCESS_KEY);
		string processInstanceId = instance.Id;
		suspendProcessInstanceById(processInstanceId);

		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, SUSPEND);

		try
		{
		  // when
		  runtimeService.activateProcessInstanceByProcessDefinitionKey(PROCESS_KEY);
		  fail("Exception expected: It should not be posssible to suspend a process instance.");
		}
		catch (AuthorizationException e)
		{

		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(SUSPEND_INSTANCE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		  assertTextPresent(SUSPEND.Name, message);
		  assertTextPresent(UPDATE.Name, message);
		}
	  }

	  public virtual void testActivateProcessInstanceByProcessDefinitionKeyWithSuspendPermissionOnAnyProcessInstance()
	  {
		// given
		ProcessInstance instance = startProcessInstanceByKey(PROCESS_KEY);
		string processInstanceId = instance.Id;
		suspendProcessInstanceById(processInstanceId);

		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, SUSPEND);

		// when
		runtimeService.activateProcessInstanceByProcessDefinitionKey(PROCESS_KEY);

		// then
		instance = selectSingleProcessInstance();
		assertFalse(instance.Suspended);
	  }

	  public virtual void testActivateProcessInstanceByProcessDefinitionKeyWithSuspendInstancesPermissionOnProcessDefinition()
	  {
		// given
		ProcessInstance instance = startProcessInstanceByKey(PROCESS_KEY);
		string processInstanceId = instance.Id;
		suspendProcessInstanceById(processInstanceId);

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, SUSPEND_INSTANCE);

		// when
		runtimeService.activateProcessInstanceByProcessDefinitionKey(PROCESS_KEY);

		// then
		instance = selectSingleProcessInstance();
		assertFalse(instance.Suspended);
	  }

	  // modify process instance /////////////////////////////////////

	  public virtual void testModifyProcessInstanceWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;

		try
		{
		  // when
		  runtimeService.createProcessInstanceModification(processInstanceId).startBeforeActivity("taskAfterBoundaryEvent").execute();
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(processInstanceId, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(MESSAGE_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testModifyProcessInstanceWithUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		// when
		runtimeService.createProcessInstanceModification(processInstanceId).startBeforeActivity("taskAfterBoundaryEvent").execute();

		// then
		disableAuthorization();
		IList<Task> tasks = taskService.createTaskQuery().list();
		enableAuthorization();

		assertFalse(tasks.Count == 0);
		assertEquals(2, tasks.Count);
	  }

	  public virtual void testModifyProcessInstanceWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		// when
		runtimeService.createProcessInstanceModification(processInstanceId).startBeforeActivity("taskAfterBoundaryEvent").execute();

		// then
		disableAuthorization();
		IList<Task> tasks = taskService.createTaskQuery().list();
		enableAuthorization();

		assertFalse(tasks.Count == 0);
		assertEquals(2, tasks.Count);
	  }

	  public virtual void testModifyProcessInstanceWithUpdateInstancePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, MESSAGE_BOUNDARY_PROCESS_KEY, userId, UPDATE_INSTANCE);

		// when
		runtimeService.createProcessInstanceModification(processInstanceId).startBeforeActivity("taskAfterBoundaryEvent").execute();

		// then
		disableAuthorization();
		IList<Task> tasks = taskService.createTaskQuery().list();
		enableAuthorization();

		assertFalse(tasks.Count == 0);
		assertEquals(2, tasks.Count);
	  }

	  public virtual void testModifyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);
		createGrantAuthorization(PROCESS_DEFINITION, MESSAGE_BOUNDARY_PROCESS_KEY, userId, UPDATE_INSTANCE);

		// when
		runtimeService.createProcessInstanceModification(processInstanceId).startBeforeActivity("taskAfterBoundaryEvent").execute();

		// then
		disableAuthorization();
		IList<Task> tasks = taskService.createTaskQuery().list();
		enableAuthorization();

		assertFalse(tasks.Count == 0);
		assertEquals(2, tasks.Count);
	  }

	  public virtual void testDeleteProcessInstanceByModifyingWithoutDeleteAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, MESSAGE_BOUNDARY_PROCESS_KEY, userId, UPDATE_INSTANCE);

		try
		{
		  // when
		  runtimeService.createProcessInstanceModification(processInstanceId).cancelAllForActivity("task").execute();
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(DELETE.Name, message);
		  assertTextPresent(processInstanceId, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(DELETE_INSTANCE.Name, message);
		  assertTextPresent(MESSAGE_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testDeleteProcessInstanceByModifyingWithoutDeletePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, MESSAGE_BOUNDARY_PROCESS_KEY, userId, UPDATE_INSTANCE);
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, DELETE);

		// when
		runtimeService.createProcessInstanceModification(processInstanceId).cancelAllForActivity("task").execute();

		// then
		assertProcessEnded(processInstanceId);
	  }

	  public virtual void testDeleteProcessInstanceByModifyingWithoutDeletePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, MESSAGE_BOUNDARY_PROCESS_KEY, userId, UPDATE_INSTANCE);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, DELETE);

		// when
		runtimeService.createProcessInstanceModification(processInstanceId).cancelAllForActivity("task").execute();

		// then
		assertProcessEnded(processInstanceId);
	  }

	  public virtual void testDeleteProcessInstanceByModifyingWithoutDeleteInstancePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
		Authorization authorization = createGrantAuthorization(PROCESS_DEFINITION, MESSAGE_BOUNDARY_PROCESS_KEY);
		authorization.UserId = userId;
		authorization.addPermission(UPDATE_INSTANCE);
		authorization.addPermission(DELETE_INSTANCE);
		saveAuthorization(authorization);

		// when
		runtimeService.createProcessInstanceModification(processInstanceId).cancelAllForActivity("task").execute();

		// then
		assertProcessEnded(processInstanceId);
	  }

	  // clear process instance authorization ////////////////////////

	  public virtual void testClearProcessInstanceAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, ALL);
		createGrantAuthorization(TASK, ANY, userId, ALL);

		disableAuthorization();
		Authorization authorization = authorizationService.createAuthorizationQuery().resourceId(processInstanceId).singleResult();
		enableAuthorization();
		assertNotNull(authorization);

		string taskId = selectSingleTask().Id;

		// when
		taskService.complete(taskId);

		// then
		disableAuthorization();
		authorization = authorizationService.createAuthorizationQuery().resourceId(processInstanceId).singleResult();
		enableAuthorization();

		assertNull(authorization);
	  }

	  public virtual void testDeleteProcessInstanceClearAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, ALL);

		disableAuthorization();
		Authorization authorization = authorizationService.createAuthorizationQuery().resourceId(processInstanceId).singleResult();
		enableAuthorization();
		assertNotNull(authorization);

		// when
		runtimeService.deleteProcessInstance(processInstanceId, null);

		// then
		disableAuthorization();
		authorization = authorizationService.createAuthorizationQuery().resourceId(processInstanceId).singleResult();
		enableAuthorization();

		assertNull(authorization);
	  }

	  // RuntimeService#getVariable() ////////////////////////////////////////////

	  public virtual void testGetVariableWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;

		try
		{
		  // when
		  runtimeService.getVariable(processInstanceId, VARIABLE_NAME);
		  fail("Exception expected: It should not be to retrieve the variable instance");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(processInstanceId, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(READ_INSTANCE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}

		// given (2)
		setReadVariableAsDefaultReadVariablePermission();

		try
		{
		  // when (2)
		  runtimeService.getVariable(processInstanceId, VARIABLE_NAME);
		  fail("Exception expected: It should not be to retrieve the variable instance");
		}
		catch (AuthorizationException e)
		{
		  // then (2)
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ_INSTANCE_VARIABLE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testGetVariableWithReadPermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		object variable = runtimeService.getVariable(processInstanceId, VARIABLE_NAME);

		// then
		assertEquals(VARIABLE_VALUE, variable);
	  }

	  public virtual void testGetVariableWithReadPermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);

		// when
		object variable = runtimeService.getVariable(processInstanceId, VARIABLE_NAME);

		// then
		assertEquals(VARIABLE_VALUE, variable);
	  }

	  public virtual void testGetVariableWithReadInstancePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE);

		// when
		object variable = runtimeService.getVariable(processInstanceId, VARIABLE_NAME);

		// then
		assertEquals(VARIABLE_VALUE, variable);
	  }

	  public virtual void testGetVariableWithReadInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE);

		// when
		object variable = runtimeService.getVariable(processInstanceId, VARIABLE_NAME);

		// then
		assertEquals(VARIABLE_VALUE, variable);
	  }

	  public virtual void testGetVariableWithReadInstanceVariablePermissionOnProcessDefinition()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE_VARIABLE);

		// when
		object variable = runtimeService.getVariable(processInstanceId, VARIABLE_NAME);

		// then
		assertEquals(VARIABLE_VALUE, variable);
	  }

	  public virtual void testGetVariableWithReadInstanceVariablePermissionOnAnyProcessDefinition()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE_VARIABLE);

		// when
		object variable = runtimeService.getVariable(processInstanceId, VARIABLE_NAME);

		// then
		assertEquals(VARIABLE_VALUE, variable);
	  }

	  // RuntimeService#getVariableLocal() ////////////////////////////////////////////

	  public virtual void testGetVariableLocalWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;

		try
		{
		  // when
		  runtimeService.getVariableLocal(processInstanceId, VARIABLE_NAME);
		  fail("Exception expected: It should not be to retrieve the variable instance");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(processInstanceId, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(READ_INSTANCE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}

		// given (2)
		setReadVariableAsDefaultReadVariablePermission();

		try
		{
		  // when (2)
		  runtimeService.getVariableLocal(processInstanceId, VARIABLE_NAME);
		  fail("Exception expected: It should not be to retrieve the variable instance");
		}
		catch (AuthorizationException e)
		{
		  // then (2)
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ_INSTANCE_VARIABLE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testGetVariableLocalWithReadPermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		object variable = runtimeService.getVariableLocal(processInstanceId, VARIABLE_NAME);

		// then
		assertEquals(VARIABLE_VALUE, variable);
	  }

	  public virtual void testGetVariableLocalWithReadPermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);

		// when
		object variable = runtimeService.getVariableLocal(processInstanceId, VARIABLE_NAME);

		// then
		assertEquals(VARIABLE_VALUE, variable);
	  }

	  public virtual void testGetVariableLocalWithReadInstancePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE);

		// when
		object variable = runtimeService.getVariableLocal(processInstanceId, VARIABLE_NAME);

		// then
		assertEquals(VARIABLE_VALUE, variable);
	  }

	  public virtual void testGetVariableLocalWithReadInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE);

		// when
		object variable = runtimeService.getVariableLocal(processInstanceId, VARIABLE_NAME);

		// then
		assertEquals(VARIABLE_VALUE, variable);
	  }

	  public virtual void testGetVariableLocalWithReadInstanceVariablePermissionOnProcessDefinition()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE_VARIABLE);

		// when
		object variable = runtimeService.getVariableLocal(processInstanceId, VARIABLE_NAME);

		// then
		assertEquals(VARIABLE_VALUE, variable);
	  }

	  public virtual void testGetVariableLocalWithReadInstanceVariablePermissionOnAnyProcessDefinition()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE_VARIABLE);

		// when
		object variable = runtimeService.getVariableLocal(processInstanceId, VARIABLE_NAME);

		// then
		assertEquals(VARIABLE_VALUE, variable);
	  }

	  // RuntimeService#getVariableTyped() ////////////////////////////////////////////

	  public virtual void testGetVariableTypedWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;

		try
		{
		  // when
		  runtimeService.getVariableTyped(processInstanceId, VARIABLE_NAME);
		  fail("Exception expected: It should not be to retrieve the variable instance");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(processInstanceId, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(READ_INSTANCE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}

		// given (2)
		setReadVariableAsDefaultReadVariablePermission();

		try
		{
		  // when (2)
		  runtimeService.getVariableTyped(processInstanceId, VARIABLE_NAME);
		  fail("Exception expected: It should not be to retrieve the variable instance");
		}
		catch (AuthorizationException e)
		{
		  // then (2)
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ_INSTANCE_VARIABLE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testGetVariableTypedWithReadPermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		TypedValue typedValue = runtimeService.getVariableTyped(processInstanceId, VARIABLE_NAME);

		// then
		assertNotNull(typedValue);
		assertEquals(VARIABLE_VALUE, typedValue.Value);
	  }

	  public virtual void testGetVariableTypedWithReadPermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);

		// when
		TypedValue typedValue = runtimeService.getVariableTyped(processInstanceId, VARIABLE_NAME);

		// then
		assertNotNull(typedValue);
		assertEquals(VARIABLE_VALUE, typedValue.Value);
	  }

	  public virtual void testGetVariableTypedWithReadInstancePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE);

		// when
		TypedValue typedValue = runtimeService.getVariableTyped(processInstanceId, VARIABLE_NAME);

		// then
		assertNotNull(typedValue);
		assertEquals(VARIABLE_VALUE, typedValue.Value);
	  }

	  public virtual void testGetVariableTypedWithReadInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE);

		// when
		TypedValue typedValue = runtimeService.getVariableTyped(processInstanceId, VARIABLE_NAME);

		// then
		assertNotNull(typedValue);
		assertEquals(VARIABLE_VALUE, typedValue.Value);
	  }

	  public virtual void testGetVariableTypedWithReadInstanceVariablePermissionOnProcessDefinition()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE_VARIABLE);

		// when
		TypedValue typedValue = runtimeService.getVariableTyped(processInstanceId, VARIABLE_NAME);

		// then
		assertNotNull(typedValue);
		assertEquals(VARIABLE_VALUE, typedValue.Value);
	  }

	  public virtual void testGetVariableTypedWithReadInstanceVariablePermissionOnAnyProcessDefinition()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE_VARIABLE);

		// when
		TypedValue typedValue = runtimeService.getVariableTyped(processInstanceId, VARIABLE_NAME);

		// then
		assertNotNull(typedValue);
		assertEquals(VARIABLE_VALUE, typedValue.Value);
	  }

	  // RuntimeService#getVariableLocalTyped() ////////////////////////////////////////////

	  public virtual void testGetVariableLocalTypedWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;

		try
		{
		  // when
		  runtimeService.getVariableLocalTyped(processInstanceId, VARIABLE_NAME);
		  fail("Exception expected: It should not be to retrieve the variable instance");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(processInstanceId, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(READ_INSTANCE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}

		// given (2)
		setReadVariableAsDefaultReadVariablePermission();

		try
		{
		  // when (2)
		  runtimeService.getVariableLocalTyped(processInstanceId, VARIABLE_NAME);
		  fail("Exception expected: It should not be to retrieve the variable instance");
		}
		catch (AuthorizationException e)
		{
		  // then (2)
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ_INSTANCE_VARIABLE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testGetVariableLocalTypedWithReadPermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		TypedValue typedValue = runtimeService.getVariableLocalTyped(processInstanceId, VARIABLE_NAME);

		// then
		assertNotNull(typedValue);
		assertEquals(VARIABLE_VALUE, typedValue.Value);
	  }

	  public virtual void testGetVariableLocalTypedWithReadPermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);

		// when
		TypedValue typedValue = runtimeService.getVariableLocalTyped(processInstanceId, VARIABLE_NAME);

		// then
		assertNotNull(typedValue);
		assertEquals(VARIABLE_VALUE, typedValue.Value);
	  }

	  public virtual void testGetVariableLocalTypedWithReadInstancePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE);

		// when
		TypedValue typedValue = runtimeService.getVariableLocalTyped(processInstanceId, VARIABLE_NAME);

		// then
		assertNotNull(typedValue);
		assertEquals(VARIABLE_VALUE, typedValue.Value);
	  }

	  public virtual void testGetVariableLocalTypedWithReadInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE);

		// when
		TypedValue typedValue = runtimeService.getVariableLocalTyped(processInstanceId, VARIABLE_NAME);

		// then
		assertNotNull(typedValue);
		assertEquals(VARIABLE_VALUE, typedValue.Value);
	  }

	  public virtual void testGetVariableLocalTypedWithReadInstanceVariablePermissionOnProcessDefinition()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE_VARIABLE);

		// when
		TypedValue typedValue = runtimeService.getVariableLocalTyped(processInstanceId, VARIABLE_NAME);

		// then
		assertNotNull(typedValue);
		assertEquals(VARIABLE_VALUE, typedValue.Value);
	  }

	  public virtual void testGetVariableLocalTypedWithReadInstanceVariablePermissionOnAnyProcessDefinition()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE_VARIABLE);

		// when
		TypedValue typedValue = runtimeService.getVariableLocalTyped(processInstanceId, VARIABLE_NAME);

		// then
		assertNotNull(typedValue);
		assertEquals(VARIABLE_VALUE, typedValue.Value);
	  }

	  // RuntimeService#getVariables() ////////////////////////////////////////////

	  public virtual void testGetVariablesWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;

		try
		{
		  // when
		  runtimeService.getVariables(processInstanceId);
		  fail("Exception expected: It should not be to retrieve the variable instances");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(processInstanceId, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(READ_INSTANCE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}

		// given (2)
		setReadVariableAsDefaultReadVariablePermission();

		try
		{
		  // when (2)
		  runtimeService.getVariables(processInstanceId);
		  fail("Exception expected: It should not be to retrieve the variable instances");
		}
		catch (AuthorizationException e)
		{
		  // then (2)
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ_INSTANCE_VARIABLE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testGetVariablesWithReadPermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		IDictionary<string, object> variables = runtimeService.getVariables(processInstanceId);

		// then
		assertNotNull(variables);
		assertFalse(variables.Count == 0);
		assertEquals(1, variables.Count);

		assertEquals(VARIABLE_VALUE, variables[VARIABLE_NAME]);
	  }

	  public virtual void testGetVariablesWithReadPermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);

		// when
		IDictionary<string, object> variables = runtimeService.getVariables(processInstanceId);

		// then
		assertNotNull(variables);
		assertFalse(variables.Count == 0);
		assertEquals(1, variables.Count);

		assertEquals(VARIABLE_VALUE, variables[VARIABLE_NAME]);
	  }

	  public virtual void testGetVariablesWithReadInstancePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE);

		// when
		IDictionary<string, object> variables = runtimeService.getVariables(processInstanceId);

		// then
		assertNotNull(variables);
		assertFalse(variables.Count == 0);
		assertEquals(1, variables.Count);

		assertEquals(VARIABLE_VALUE, variables[VARIABLE_NAME]);
	  }

	  public virtual void testGetVariablesWithReadInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE);

		// when
		IDictionary<string, object> variables = runtimeService.getVariables(processInstanceId);

		// then
		assertNotNull(variables);
		assertFalse(variables.Count == 0);
		assertEquals(1, variables.Count);

		assertEquals(VARIABLE_VALUE, variables[VARIABLE_NAME]);
	  }

	  public virtual void testGetVariablesWithReadInstanceVariablePermissionOnProcessDefinition()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE_VARIABLE);

		// when
		IDictionary<string, object> variables = runtimeService.getVariables(processInstanceId);

		// then
		verifyGetVariables(variables);
	  }

	  public virtual void testGetVariablesWithReadInstanceVariablePermissionOnAnyProcessDefinition()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE_VARIABLE);

		// when
		IDictionary<string, object> variables = runtimeService.getVariables(processInstanceId);

		// then
		verifyGetVariables(variables);
	  }

	  // RuntimeService#getVariablesLocal() ////////////////////////////////////////////

	  // RuntimeService#getVariablesLocal() ////////////////////////////////////////////

	  public virtual void testGetVariablesLocalWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;

		try
		{
		  // when
		  runtimeService.getVariablesLocal(processInstanceId);
		  fail("Exception expected: It should not be to retrieve the variable instances");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(processInstanceId, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(READ_INSTANCE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}

		// given (2)
		setReadVariableAsDefaultReadVariablePermission();

		try
		{
		  // when (2)
		  runtimeService.getVariablesLocal(processInstanceId);
		  fail("Exception expected: It should not be to retrieve the variable instances");
		}
		catch (AuthorizationException e)
		{
		  // then (2)
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ_INSTANCE_VARIABLE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testGetVariablesLocalWithReadPermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		IDictionary<string, object> variables = runtimeService.getVariablesLocal(processInstanceId);

		// then
		assertNotNull(variables);
		assertFalse(variables.Count == 0);
		assertEquals(1, variables.Count);

		assertEquals(VARIABLE_VALUE, variables[VARIABLE_NAME]);
	  }

	  public virtual void testGetVariablesLocalWithReadPermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);

		// when
		IDictionary<string, object> variables = runtimeService.getVariablesLocal(processInstanceId);

		// then
		assertNotNull(variables);
		assertFalse(variables.Count == 0);
		assertEquals(1, variables.Count);

		assertEquals(VARIABLE_VALUE, variables[VARIABLE_NAME]);
	  }

	  public virtual void testGetVariablesLocalWithReadInstancePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE);

		// when
		IDictionary<string, object> variables = runtimeService.getVariablesLocal(processInstanceId);

		// then
		assertNotNull(variables);
		assertFalse(variables.Count == 0);
		assertEquals(1, variables.Count);

		assertEquals(VARIABLE_VALUE, variables[VARIABLE_NAME]);
	  }

	  public virtual void testGetVariablesLocalWithReadInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE);

		// when
		IDictionary<string, object> variables = runtimeService.getVariablesLocal(processInstanceId);

		// then
		assertNotNull(variables);
		assertFalse(variables.Count == 0);
		assertEquals(1, variables.Count);

		assertEquals(VARIABLE_VALUE, variables[VARIABLE_NAME]);
	  }

	  public virtual void testGetVariablesLocalWithReadInstanceVariablePermissionOnProcessDefinition()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE_VARIABLE);

		// when
		IDictionary<string, object> variables = runtimeService.getVariablesLocal(processInstanceId);

		// then
		verifyGetVariables(variables);
	  }

	  public virtual void testGetVariablesLocalWithReadInstanceVariablePermissionOnAnyProcessDefinition()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE_VARIABLE);

		// when
		IDictionary<string, object> variables = runtimeService.getVariablesLocal(processInstanceId);

		// then
		verifyGetVariables(variables);
	  }

	  // RuntimeService#getVariablesTyped() ////////////////////////////////////////////

	  // RuntimeService#getVariablesTyped() ////////////////////////////////////////////

	  public virtual void testGetVariablesTypedWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;

		try
		{
		  // when
		  runtimeService.getVariablesTyped(processInstanceId);
		  fail("Exception expected: It should not be to retrieve the variable instances");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(processInstanceId, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(READ_INSTANCE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}

		// given (2)
		setReadVariableAsDefaultReadVariablePermission();

		try
		{
		  // when (2)
		  runtimeService.getVariablesTyped(processInstanceId);
		  fail("Exception expected: It should not be to retrieve the variable instances");
		}
		catch (AuthorizationException e)
		{
		  // then (2)
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ_INSTANCE_VARIABLE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testGetVariablesTypedWithReadPermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		VariableMap variables = runtimeService.getVariablesTyped(processInstanceId);

		// then
		assertNotNull(variables);
		assertFalse(variables.Empty);
		assertEquals(1, variables.size());

		assertEquals(VARIABLE_VALUE, variables.get(VARIABLE_NAME));
	  }

	  public virtual void testGetVariablesTypedWithReadPermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);

		// when
		VariableMap variables = runtimeService.getVariablesTyped(processInstanceId);

		// then
		assertNotNull(variables);
		assertFalse(variables.Empty);
		assertEquals(1, variables.size());

		assertEquals(VARIABLE_VALUE, variables.get(VARIABLE_NAME));
	  }

	  public virtual void testGetVariablesTypedWithReadInstancePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE);

		// when
		VariableMap variables = runtimeService.getVariablesTyped(processInstanceId);

		// then
		assertNotNull(variables);
		assertFalse(variables.Empty);
		assertEquals(1, variables.size());

		assertEquals(VARIABLE_VALUE, variables.get(VARIABLE_NAME));
	  }

	  public virtual void testGetVariablesTypedWithReadInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE);

		// when
		VariableMap variables = runtimeService.getVariablesTyped(processInstanceId);

		// then
		assertNotNull(variables);
		assertFalse(variables.Empty);
		assertEquals(1, variables.size());

		assertEquals(VARIABLE_VALUE, variables.get(VARIABLE_NAME));
	  }

	  public virtual void testGetVariablesTypedWithReadInstanceVariablePermissionOnProcessDefinition()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE_VARIABLE);

		// when
		VariableMap variables = runtimeService.getVariablesTyped(processInstanceId);

		// then
		verifyGetVariables(variables);
	  }

	  public virtual void testGetVariablesTypedWithReadInstanceVariablePermissionOnAnyProcessDefinition()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE_VARIABLE);

		// when
		VariableMap variables = runtimeService.getVariablesTyped(processInstanceId);

		// then
		verifyGetVariables(variables);
	  }

	  // RuntimeService#getVariablesLocalTyped() ////////////////////////////////////////////

	  // RuntimeService#getVariablesLocalTyped() ////////////////////////////////////////////

	  public virtual void testGetVariablesLocalTypedWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;

		try
		{
		  // when
		  runtimeService.getVariablesLocalTyped(processInstanceId);
		  fail("Exception expected: It should not be to retrieve the variable instances");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(processInstanceId, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(READ_INSTANCE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}

		// given (2)
		setReadVariableAsDefaultReadVariablePermission();

		try
		{
		  // when (2)
		  runtimeService.getVariablesLocalTyped(processInstanceId);
		  fail("Exception expected: It should not be to retrieve the variable instances");
		}
		catch (AuthorizationException e)
		{
		  // then (2)
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ_INSTANCE_VARIABLE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testGetVariablesLocalTypedWithReadPermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		VariableMap variables = runtimeService.getVariablesLocalTyped(processInstanceId);

		// then
		assertNotNull(variables);
		assertFalse(variables.Empty);
		assertEquals(1, variables.size());

		assertEquals(VARIABLE_VALUE, variables.get(VARIABLE_NAME));
	  }

	  public virtual void testGetVariablesLocalTypedWithReadPermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);

		// when
		VariableMap variables = runtimeService.getVariablesLocalTyped(processInstanceId);

		// then
		assertNotNull(variables);
		assertFalse(variables.Empty);
		assertEquals(1, variables.size());

		assertEquals(VARIABLE_VALUE, variables.get(VARIABLE_NAME));
	  }

	  public virtual void testGetVariablesLocalTypedWithReadInstancePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE);

		// when
		VariableMap variables = runtimeService.getVariablesLocalTyped(processInstanceId);

		// then
		assertNotNull(variables);
		assertFalse(variables.Empty);
		assertEquals(1, variables.size());

		assertEquals(VARIABLE_VALUE, variables.get(VARIABLE_NAME));
	  }

	  public virtual void testGetVariablesLocalTypedWithReadInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE);

		// when
		VariableMap variables = runtimeService.getVariablesLocalTyped(processInstanceId);

		// then
		assertNotNull(variables);
		assertFalse(variables.Empty);
		assertEquals(1, variables.size());

		assertEquals(VARIABLE_VALUE, variables.get(VARIABLE_NAME));
	  }

	  public virtual void testGetVariablesLocalTypedWithReadInstanceVariablePermissionOnProcessDefinition()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE_VARIABLE);

		// when
		VariableMap variables = runtimeService.getVariablesLocalTyped(processInstanceId);

		// then
		verifyGetVariables(variables);
	  }

	  public virtual void testGetVariablesLocalTypedWithReadInstanceVariablePermissionOnAnyProcessDefinition()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE_VARIABLE);

		// when
		VariableMap variables = runtimeService.getVariablesLocalTyped(processInstanceId);

		// then
		verifyGetVariables(variables);
	  }

	  // RuntimeService#getVariables() ////////////////////////////////////////////

	  public virtual void testGetVariablesByNameWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;

		try
		{
		  // when
		  runtimeService.getVariables(processInstanceId, Arrays.asList(VARIABLE_NAME));
		  fail("Exception expected: It should not be to retrieve the variable instances");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(processInstanceId, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(READ_INSTANCE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}

		// given (2)
		setReadVariableAsDefaultReadVariablePermission();

		try
		{
		  // when (2)
		  runtimeService.getVariables(processInstanceId, Arrays.asList(VARIABLE_NAME));
		  fail("Exception expected: It should not be to retrieve the variable instances");
		}
		catch (AuthorizationException e)
		{
		  // then (2)
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ_INSTANCE_VARIABLE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testGetVariablesByNameWithReadPermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		IDictionary<string, object> variables = runtimeService.getVariables(processInstanceId, Arrays.asList(VARIABLE_NAME));

		// then
		assertNotNull(variables);
		assertFalse(variables.Count == 0);
		assertEquals(1, variables.Count);

		assertEquals(VARIABLE_VALUE, variables[VARIABLE_NAME]);
	  }

	  public virtual void testGetVariablesByNameWithReadPermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);

		// when
		IDictionary<string, object> variables = runtimeService.getVariables(processInstanceId, Arrays.asList(VARIABLE_NAME));

		// then
		assertNotNull(variables);
		assertFalse(variables.Count == 0);
		assertEquals(1, variables.Count);

		assertEquals(VARIABLE_VALUE, variables[VARIABLE_NAME]);
	  }

	  public virtual void testGetVariablesByNameWithReadInstancePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE);

		// when
		IDictionary<string, object> variables = runtimeService.getVariables(processInstanceId, Arrays.asList(VARIABLE_NAME));

		// then
		assertNotNull(variables);
		assertFalse(variables.Count == 0);
		assertEquals(1, variables.Count);

		assertEquals(VARIABLE_VALUE, variables[VARIABLE_NAME]);
	  }

	  public virtual void testGetVariablesByNameWithReadInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE);

		// when
		IDictionary<string, object> variables = runtimeService.getVariables(processInstanceId, Arrays.asList(VARIABLE_NAME));

		// then
		assertNotNull(variables);
		assertFalse(variables.Count == 0);
		assertEquals(1, variables.Count);

		assertEquals(VARIABLE_VALUE, variables[VARIABLE_NAME]);
	  }

	  public virtual void testGetVariablesByNameWithReadInstanceVariablePermissionOnProcessDefinition()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE_VARIABLE);

		// when
		IDictionary<string, object> variables = runtimeService.getVariables(processInstanceId, Arrays.asList(VARIABLE_NAME));

		// then
		verifyGetVariables(variables);
	  }

	  public virtual void testGetVariablesByNameWithReadInstanceVariablePermissionOnAnyProcessDefinition()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE_VARIABLE);

		// when
		IDictionary<string, object> variables = runtimeService.getVariables(processInstanceId, Arrays.asList(VARIABLE_NAME));

		// then
		verifyGetVariables(variables);
	  }

	  // RuntimeService#getVariablesLocal() ////////////////////////////////////////////

	  public virtual void testGetVariablesLocalByNameWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;

		try
		{
		  // when
		  runtimeService.getVariablesLocal(processInstanceId, Arrays.asList(VARIABLE_NAME));
		  fail("Exception expected: It should not be to retrieve the variable instances");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(processInstanceId, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(READ_INSTANCE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}

		// given (2)
		setReadVariableAsDefaultReadVariablePermission();

		try
		{
		  // when (2)
		  runtimeService.getVariablesLocal(processInstanceId, Arrays.asList(VARIABLE_NAME));
		  fail("Exception expected: It should not be to retrieve the variable instances");
		}
		catch (AuthorizationException e)
		{
		  // then (2)
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ_INSTANCE_VARIABLE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testGetVariablesLocalByNameWithReadPermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		IDictionary<string, object> variables = runtimeService.getVariablesLocal(processInstanceId, Arrays.asList(VARIABLE_NAME));

		// then
		assertNotNull(variables);
		assertFalse(variables.Count == 0);
		assertEquals(1, variables.Count);

		assertEquals(VARIABLE_VALUE, variables[VARIABLE_NAME]);
	  }

	  public virtual void testGetVariablesLocalByNameWithReadPermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);

		// when
		IDictionary<string, object> variables = runtimeService.getVariablesLocal(processInstanceId, Arrays.asList(VARIABLE_NAME));

		// then
		assertNotNull(variables);
		assertFalse(variables.Count == 0);
		assertEquals(1, variables.Count);

		assertEquals(VARIABLE_VALUE, variables[VARIABLE_NAME]);
	  }

	  public virtual void testGetVariablesLocalByNameWithReadInstancePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE);

		// when
		IDictionary<string, object> variables = runtimeService.getVariablesLocal(processInstanceId, Arrays.asList(VARIABLE_NAME));

		// then
		assertNotNull(variables);
		assertFalse(variables.Count == 0);
		assertEquals(1, variables.Count);

		assertEquals(VARIABLE_VALUE, variables[VARIABLE_NAME]);
	  }

	  public virtual void testGetVariablesLocalByNameWithReadInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE);

		// when
		IDictionary<string, object> variables = runtimeService.getVariablesLocal(processInstanceId, Arrays.asList(VARIABLE_NAME));

		// then
		assertNotNull(variables);
		assertFalse(variables.Count == 0);
		assertEquals(1, variables.Count);

		assertEquals(VARIABLE_VALUE, variables[VARIABLE_NAME]);
	  }

	  public virtual void testGetVariablesLocalByNameWithReadInstanceVariablePermissionOnProcessDefinition()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE_VARIABLE);

		// when
		IDictionary<string, object> variables = runtimeService.getVariablesLocal(processInstanceId, Arrays.asList(VARIABLE_NAME));

		// then
		verifyGetVariables(variables);
	  }

	  public virtual void testGetVariablesLocalByNameWithReadInstanceVariablePermissionOnAnyProcessDefinition()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE_VARIABLE);

		// when
		IDictionary<string, object> variables = runtimeService.getVariablesLocal(processInstanceId, Arrays.asList(VARIABLE_NAME));

		// then
		verifyGetVariables(variables);
	  }

	  // RuntimeService#getVariablesTyped() ////////////////////////////////////////////

	  public virtual void testGetVariablesTypedByNameWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;

		try
		{
		  // when
		  runtimeService.getVariablesTyped(processInstanceId, Arrays.asList(VARIABLE_NAME), false);
		  fail("Exception expected: It should not be to retrieve the variable instances");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(processInstanceId, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(READ_INSTANCE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}

		// given (2)
		setReadVariableAsDefaultReadVariablePermission();

		try
		{
		  // when (2)
		  runtimeService.getVariablesTyped(processInstanceId, Arrays.asList(VARIABLE_NAME), false);
		  fail("Exception expected: It should not be to retrieve the variable instances");
		}
		catch (AuthorizationException e)
		{
		  // then (2)
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ_INSTANCE_VARIABLE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testGetVariablesTypedByNameWithReadPermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		VariableMap variables = runtimeService.getVariablesTyped(processInstanceId, Arrays.asList(VARIABLE_NAME), false);

		// then
		assertNotNull(variables);
		assertFalse(variables.Empty);
		assertEquals(1, variables.size());

		assertEquals(VARIABLE_VALUE, variables.get(VARIABLE_NAME));
	  }

	  public virtual void testGetVariablesTypedByNameWithReadPermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);

		// when
		VariableMap variables = runtimeService.getVariablesTyped(processInstanceId, Arrays.asList(VARIABLE_NAME), false);

		// then
		assertNotNull(variables);
		assertFalse(variables.Empty);
		assertEquals(1, variables.size());

		assertEquals(VARIABLE_VALUE, variables.get(VARIABLE_NAME));
	  }

	  public virtual void testGetVariablesTypedByNameWithReadInstancePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE);

		// when
		VariableMap variables = runtimeService.getVariablesTyped(processInstanceId, Arrays.asList(VARIABLE_NAME), false);

		// then
		assertNotNull(variables);
		assertFalse(variables.Empty);
		assertEquals(1, variables.size());

		assertEquals(VARIABLE_VALUE, variables.get(VARIABLE_NAME));
	  }

	  public virtual void testGetVariablesTypedByNameWithReadInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE);

		// when
		VariableMap variables = runtimeService.getVariablesTyped(processInstanceId, Arrays.asList(VARIABLE_NAME), false);

		// then
		assertNotNull(variables);
		assertFalse(variables.Empty);
		assertEquals(1, variables.size());

		assertEquals(VARIABLE_VALUE, variables.get(VARIABLE_NAME));
	  }

	  public virtual void testGetVariablesTypedByNameWithReadInstanceVariablePermissionOnProcessDefinition()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE_VARIABLE);

		// when
		VariableMap variables = runtimeService.getVariablesTyped(processInstanceId, Arrays.asList(VARIABLE_NAME), false);

		// then
		verifyGetVariables(variables);
	  }

	  public virtual void testGetVariablesTypedByNameWithReadInstanceVariablePermissionOnAnyProcessDefinition()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE_VARIABLE);

		// when
		VariableMap variables = runtimeService.getVariablesTyped(processInstanceId, Arrays.asList(VARIABLE_NAME), false);

		// then
		verifyGetVariables(variables);
	  }

	  // RuntimeService#getVariablesLocalTyped() ////////////////////////////////////////////

	  public virtual void testGetVariablesLocalTypedByNameWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;

		try
		{
		  // when
		  runtimeService.getVariablesLocalTyped(processInstanceId, Arrays.asList(VARIABLE_NAME), false);
		  fail("Exception expected: It should not be to retrieve the variable instances");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(processInstanceId, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(READ_INSTANCE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}

		// given (2)
		setReadVariableAsDefaultReadVariablePermission();

		try
		{
		  // when (2)
		  runtimeService.getVariablesLocalTyped(processInstanceId, Arrays.asList(VARIABLE_NAME), false);
		  fail("Exception expected: It should not be to retrieve the variable instances");
		}
		catch (AuthorizationException e)
		{
		  // then (2)
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ_INSTANCE_VARIABLE.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testGetVariablesLocalTypedByNameWithReadPermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		VariableMap variables = runtimeService.getVariablesLocalTyped(processInstanceId, Arrays.asList(VARIABLE_NAME), false);

		// then
		assertNotNull(variables);
		assertFalse(variables.Empty);
		assertEquals(1, variables.size());

		assertEquals(VARIABLE_VALUE, variables.get(VARIABLE_NAME));
	  }

	  public virtual void testGetVariablesLocalTypedByNameWithReadPermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);

		// when
		VariableMap variables = runtimeService.getVariablesLocalTyped(processInstanceId, Arrays.asList(VARIABLE_NAME), false);

		// then
		assertNotNull(variables);
		assertFalse(variables.Empty);
		assertEquals(1, variables.size());

		assertEquals(VARIABLE_VALUE, variables.get(VARIABLE_NAME));
	  }

	  public virtual void testGetVariablesLocalTypedByNameWithReadInstancePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE);

		// when
		VariableMap variables = runtimeService.getVariablesLocalTyped(processInstanceId, Arrays.asList(VARIABLE_NAME), false);

		// then
		assertNotNull(variables);
		assertFalse(variables.Empty);
		assertEquals(1, variables.size());

		assertEquals(VARIABLE_VALUE, variables.get(VARIABLE_NAME));
	  }

	  public virtual void testGetVariablesLocalTypedByNameWithReadInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE);

		// when
		VariableMap variables = runtimeService.getVariablesLocalTyped(processInstanceId, Arrays.asList(VARIABLE_NAME), false);

		// then
		assertNotNull(variables);
		assertFalse(variables.Empty);
		assertEquals(1, variables.size());

		assertEquals(VARIABLE_VALUE, variables.get(VARIABLE_NAME));
	  }

	  public virtual void testGetVariablesLocalTypedByNameWithReadInstanceVariablePermissionOnProcessDefinition()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE_VARIABLE);

		// when
		VariableMap variables = runtimeService.getVariablesLocalTyped(processInstanceId, Arrays.asList(VARIABLE_NAME), false);

		// then
		verifyGetVariables(variables);
	  }

	  public virtual void testGetVariablesLocalTypedByNameWithReadInstanceVariablePermissionOnAnyProcessDefinition()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE_VARIABLE);

		// when
		VariableMap variables = runtimeService.getVariablesLocalTyped(processInstanceId, Arrays.asList(VARIABLE_NAME), false);

		// then
		verifyGetVariables(variables);
	  }

	  // RuntimeService#setVariable() ////////////////////////////////////////////

	  public virtual void testSetVariableWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;

		try
		{
		  // when
		  runtimeService.setVariable(processInstanceId, VARIABLE_NAME, VARIABLE_VALUE);
		  fail("Exception expected: It should not be to set a variable");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  verifyMessageIsValid(processInstanceId, message);
		}
	  }

	  public virtual void testSetVariableWithUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		verifySetVariable(processInstanceId);
	  }

	  public virtual void testSetVariableWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		verifySetVariable(processInstanceId);
	  }

	  public virtual void testSetVariableWithUpdateInstanceInstancePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE);

		// when
		runtimeService.setVariable(processInstanceId, VARIABLE_NAME, VARIABLE_VALUE);

		// then
		disableAuthorization();
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();
		verifyQueryResults(query, 1);
		enableAuthorization();
	  }

	  public virtual void testSetVariableWithUpdateInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE);

		verifySetVariable(processInstanceId);
	  }

	  public virtual void testSetVariableWithUpdateVariablePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE_VARIABLE);

		verifySetVariable(processInstanceId);
	  }

	  public virtual void testSetVariableWithUpdateVariablePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE_VARIABLE);

		verifySetVariable(processInstanceId);
	  }

	  public virtual void testSetVariableWithUpdateInstanceVariablePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE_VARIABLE);

		verifySetVariable(processInstanceId);
	  }

	  public virtual void testSetVariableWithUpdateInstanceVariablePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE_VARIABLE);

		verifySetVariable(processInstanceId);
	  }

	  // RuntimeService#setVariableLocal() ////////////////////////////////////////////

	  public virtual void testSetVariableLocalWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;

		try
		{
		  // when
		  runtimeService.setVariableLocal(processInstanceId, VARIABLE_NAME, VARIABLE_VALUE);
		  fail("Exception expected: It should not be to set a variable");
		}
		catch (AuthorizationException e)
		{
		  // then
		  verifyMessageIsValid(processInstanceId, e.Message);
		}
	  }

	  public virtual void testSetVariableLocalWithUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		verifySetVariableLocal(processInstanceId);
	  }

	  public virtual void testSetVariableLocalWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		verifySetVariableLocal(processInstanceId);
	  }

	  public virtual void testSetVariableLocalWithUpdateInstancePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE);

		verifySetVariableLocal(processInstanceId);
	  }

	  public virtual void testSetVariableLocalWithUpdateInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE);

		// when
		runtimeService.setVariableLocal(processInstanceId, VARIABLE_NAME, VARIABLE_VALUE);

		// then
		disableAuthorization();
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();
		verifyQueryResults(query, 1);
		enableAuthorization();
	  }

	  public virtual void testSetVariableLocalWithUpdateVariablePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE_VARIABLE);

		verifySetVariableLocal(processInstanceId);
	  }

	  public virtual void testSetVariableLocalWithUpdateVariablePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE_VARIABLE);

		verifySetVariableLocal(processInstanceId);
	  }

	  public virtual void testSetVariableLocalWithUpdateInstanceVariablePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE_VARIABLE);

		verifySetVariableLocal(processInstanceId);
	  }

	  public virtual void testSetVariableLocalWithUpdateInstanceVariablePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE_VARIABLE);

		verifySetVariableLocal(processInstanceId);
	  }

	  // RuntimeService#setVariables() ////////////////////////////////////////////

	  public virtual void testSetVariablesWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;

		try
		{
		  // when
		  runtimeService.setVariables(processInstanceId, Variables);
		  fail("Exception expected: It should not be to set a variable");
		}
		catch (AuthorizationException e)
		{
		  // then
		  verifyMessageIsValid(processInstanceId, e.Message);
		}
	  }

	  public virtual void testSetVariablesWithUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		verifySetVariables(processInstanceId);
	  }

	  public virtual void testSetVariablesWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		verifySetVariables(processInstanceId);
	  }

	  public virtual void testSetVariablesWithUpdateInstancePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE);

		verifySetVariables(processInstanceId);
	  }

	  public virtual void testSetVariablesWithUpdateInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE);

		verifySetVariables(processInstanceId);
	  }

	  public virtual void testSetVariablesWithUpdateVariablePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		verifySetVariables(processInstanceId);
	  }

	  public virtual void testSetVariablesWithUpdateVariablePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE_VARIABLE);

		verifySetVariables(processInstanceId);
	  }

	  public virtual void testSetVariablesWithUpdateInstanceVariablePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE_VARIABLE);

		verifySetVariables(processInstanceId);
	  }

	  public virtual void testSetVariablesWithUpdateInstanceVariablePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE_VARIABLE);

		verifySetVariables(processInstanceId);
	  }

	  // RuntimeService#setVariablesLocal() ////////////////////////////////////////////

	  public virtual void testSetVariablesLocalWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;

		try
		{
		  // when
		  runtimeService.setVariablesLocal(processInstanceId, Variables);
		  fail("Exception expected: It should not be to set a variable");
		}
		catch (AuthorizationException e)
		{
		  // then
		  verifyMessageIsValid(processInstanceId, e.Message);
		}
	  }

	  public virtual void testSetVariablesLocalWithUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		verifySetVariablesLocal(processInstanceId);
	  }

	  public virtual void testSetVariablesLocalWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		verifySetVariablesLocal(processInstanceId);
	  }

	  public virtual void testSetVariablesLocalWithUpdateInstancePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE);

		verifySetVariablesLocal(processInstanceId);
	  }

	  public virtual void testSetVariablesLocalWithUpdateInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE);

		verifySetVariablesLocal(processInstanceId);
	  }

	  public virtual void testSetVariablesLocalWithUpdateVariablePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE_VARIABLE);

		verifySetVariablesLocal(processInstanceId);
	  }

	  public virtual void testSetVariablesLocalWithUpdateVariablePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE_VARIABLE);

		verifySetVariablesLocal(processInstanceId);
	  }

	  public virtual void testSetVariablesLocalWithUpdateInstanceVariablePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE_VARIABLE);

		verifySetVariablesLocal(processInstanceId);
	  }

	  public virtual void testSetVariablesLocalWithUpdateInstanceVariablePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE_VARIABLE);

		verifySetVariablesLocal(processInstanceId);
	  }

	  // RuntimeService#removeVariable() ////////////////////////////////////////////

	  public virtual void testRemoveVariableWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;

		try
		{
		  // when
		  runtimeService.removeVariable(processInstanceId, VARIABLE_NAME);
		  fail("Exception expected: It should not be to set a variable");
		}
		catch (AuthorizationException e)
		{
		  // then
		  verifyMessageIsValid(processInstanceId, e.Message);
		}
	  }

	  public virtual void testRemoveVariableWithUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		verifyRemoveVariable(processInstanceId);
	  }

	  public virtual void testRemoveVariableWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		verifyRemoveVariable(processInstanceId);
	  }

	  public virtual void testRemoveVariableWithUpdateInstancePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE);

		verifyRemoveVariable(processInstanceId);
	  }

	  public virtual void testRemoveVariableWithUpdateInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE);

		verifyRemoveVariable(processInstanceId);
	  }

	  public virtual void testRemoveVariableWithUpdateVariablePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE_VARIABLE);

		verifyRemoveVariable(processInstanceId);
	  }

	  public virtual void testRemoveVariableWithUpdateVariablePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE_VARIABLE);

		verifyRemoveVariable(processInstanceId);
	  }

	  public virtual void testRemoveVariableWithUpdateInstanceVariablePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE_VARIABLE);

		verifyRemoveVariable(processInstanceId);
	  }

	  public virtual void testRemoveVariableWithUpdateInstanceVariablePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE_VARIABLE);

		verifyRemoveVariable(processInstanceId);
	  }

	  // RuntimeService#removeVariableLocal() ////////////////////////////////////////////

	  public virtual void testRemoveVariableLocalWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;

		try
		{
		  // when
		  runtimeService.removeVariableLocal(processInstanceId, VARIABLE_NAME);
		  fail("Exception expected: It should not be to set a variable");
		}
		catch (AuthorizationException e)
		{
		  // then
		  verifyMessageIsValid(processInstanceId, e.Message);
		}
	  }

	  public virtual void testRemoveVariableLocalWithUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		verifyRemoveVariableLocal(processInstanceId);
	  }

	  public virtual void testRemoveVariableLocalWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		verifyRemoveVariableLocal(processInstanceId);
	  }

	  public virtual void testRemoveVariableLocalWithUpdateInstancePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE);

		verifyRemoveVariableLocal(processInstanceId);
	  }

	  public virtual void testRemoveVariableLocalWithUpdateInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE);

		verifyRemoveVariableLocal(processInstanceId);
	  }

	  public virtual void testRemoveVariableLocalWithUpdateVariablePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE_VARIABLE);

		verifyRemoveVariableLocal(processInstanceId);
	  }

	  public virtual void testRemoveVariableLocalWithUpdateVariablePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE_VARIABLE);

		verifyRemoveVariableLocal(processInstanceId);
	  }

	  public virtual void testRemoveVariableLocalWithUpdateInstanceVariablePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE_VARIABLE);

		verifyRemoveVariableLocal(processInstanceId);
	  }

	  public virtual void testRemoveVariableLocalWithUpdateInstanceVariablePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE_VARIABLE);

		verifyRemoveVariableLocal(processInstanceId);
	  }

	  // RuntimeService#removeVariables() ////////////////////////////////////////////

	  public virtual void testRemoveVariablesWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;

		try
		{
		  // when
		  runtimeService.removeVariables(processInstanceId, Arrays.asList(VARIABLE_NAME));
		  fail("Exception expected: It should not be to set a variable");
		}
		catch (AuthorizationException e)
		{
		  // then
		  verifyMessageIsValid(processInstanceId, e.Message);
		}
	  }

	  public virtual void testRemoveVariablesWithUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		verifyRemoveVariables(processInstanceId);
	  }

	  public virtual void testRemoveVariablesWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		verifyRemoveVariables(processInstanceId);
	  }

	  public virtual void testRemoveVariablesWithUpdateInstancePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE);

		verifyRemoveVariables(processInstanceId);
	  }

	  public virtual void testRemoveVariablesWithUpdateInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE);

		verifyRemoveVariables(processInstanceId);
	  }

	  public virtual void testRemoveVariablesWithUpdateVariablePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE_VARIABLE);

		verifyRemoveVariables(processInstanceId);
	  }

	  public virtual void testRemoveVariablesWithUpdateVariablePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE_VARIABLE);

		verifyRemoveVariables(processInstanceId);
	  }

	  public virtual void testRemoveVariablesWithUpdateInstanceVariablePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE_VARIABLE);

		verifyRemoveVariables(processInstanceId);
	  }

	  public virtual void testRemoveVariablesWithUpdateInstanceVariablePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE_VARIABLE);

		verifyRemoveVariables(processInstanceId);
	  }

	  // RuntimeService#removeVariablesLocal() ////////////////////////////////////////////

	  public virtual void testRemoveVariablesLocalWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;

		try
		{
		  // when
		  runtimeService.removeVariablesLocal(processInstanceId, Arrays.asList(VARIABLE_NAME));
		  fail("Exception expected: It should not be to set a variable");
		}
		catch (AuthorizationException e)
		{
		  // then
		  verifyMessageIsValid(processInstanceId, e.Message);
		}
	  }

	  public virtual void testRemoveVariablesLocalWithUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		verifyRemoveVariablesLocal(processInstanceId);
	  }

	  public virtual void testRemoveVariablesLocalWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		verifyRemoveVariablesLocal(processInstanceId);
	  }

	  public virtual void testRemoveVariablesLocalWithUpdateInstancePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE);

		verifyRemoveVariablesLocal(processInstanceId);
	  }

	  public virtual void testRemoveVariablesLocalWithUpdateInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE);

		verifyRemoveVariablesLocal(processInstanceId);
	  }

	  public virtual void testRemoveVariablesLocalWithUpdateVariablePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE_VARIABLE);

		verifyRemoveVariablesLocal(processInstanceId);
	  }

	  public virtual void testRemoveVariablesLocalWithUpdateVariablePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE_VARIABLE);

		verifyRemoveVariablesLocal(processInstanceId);
	  }

	  public virtual void testRemoveVariablesLocalWithUpdateInstanceVariablePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE_VARIABLE);

		verifyRemoveVariablesLocal(processInstanceId);
	  }

	  public virtual void testRemoveVariablesLocalWithUpdateInstanceVariablePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE_VARIABLE);

		verifyRemoveVariablesLocal(processInstanceId);
	  }

	  // RuntimeServiceImpl#updateVariables() ////////////////////////////////////////////

	  public virtual void testUpdateVariablesWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;

		try
		{
		  // when (1)
		  ((RuntimeServiceImpl)runtimeService).updateVariables(processInstanceId, Variables, null);
		  fail("Exception expected: It should not be to set a variable");
		}
		catch (AuthorizationException e)
		{
		  // then (1)
		  verifyMessageIsValid(processInstanceId, e.Message);
		}

		try
		{
		  // when (2)
		  ((RuntimeServiceImpl)runtimeService).updateVariables(processInstanceId, null, Arrays.asList(VARIABLE_NAME));
		  fail("Exception expected: It should not be to set a variable");
		}
		catch (AuthorizationException e)
		{
		  // then (2)
		  verifyMessageIsValid(processInstanceId, e.Message);
		}

		try
		{
		  // when (3)
		  ((RuntimeServiceImpl)runtimeService).updateVariables(processInstanceId, Variables, Arrays.asList(VARIABLE_NAME));
		  fail("Exception expected: It should not be to set a variable");
		}
		catch (AuthorizationException e)
		{
		  // then (3)
		  verifyMessageIsValid(processInstanceId, e.Message);
		}
	  }

	  public virtual void testUpdateVariablesWithUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		verifyUpdateVariables(processInstanceId);
	  }

	  public virtual void testUpdateVariablesWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		verifyUpdateVariables(processInstanceId);
	  }

	  public virtual void testUpdateVariablesWithUpdateInstancePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE);

		verifyUpdateVariables(processInstanceId);
	  }

	  public virtual void testUpdateVariablesWithUpdateInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE);

		verifyUpdateVariables(processInstanceId);
	  }

	  public virtual void testUpdateVariablesWithUpdateVariablePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE_VARIABLE);

		verifyUpdateVariables(processInstanceId);
	  }

	  public virtual void testUpdateVariablesWithUpdateVariablePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE_VARIABLE);

		verifyUpdateVariables(processInstanceId);
	  }

	  public virtual void testUpdateVariablesWithUpdateInstanceVariablePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE_VARIABLE);

		verifyUpdateVariables(processInstanceId);
	  }

	  public virtual void testUpdateVariablesWithUpdateInstanceVariablePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE_VARIABLE);

		verifyUpdateVariables(processInstanceId);
	  }

	  // RuntimeServiceImpl#updateVariablesLocal() ////////////////////////////////////////////

	  public virtual void testUpdateVariablesLocalWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;

		try
		{
		  // when (1)
		  ((RuntimeServiceImpl)runtimeService).updateVariablesLocal(processInstanceId, Variables, null);
		  fail("Exception expected: It should not be to set a variable");
		}
		catch (AuthorizationException e)
		{
		  // then (1)
		  verifyMessageIsValid(processInstanceId, e.Message);
		}

		try
		{
		  // when (2)
		  ((RuntimeServiceImpl)runtimeService).updateVariablesLocal(processInstanceId, null, Arrays.asList(VARIABLE_NAME));
		  fail("Exception expected: It should not be to set a variable");
		}
		catch (AuthorizationException e)
		{
		  // then (2)
		  verifyMessageIsValid(processInstanceId, e.Message);
		}

		try
		{
		  // when (3)
		  ((RuntimeServiceImpl)runtimeService).updateVariablesLocal(processInstanceId, Variables, Arrays.asList(VARIABLE_NAME));
		  fail("Exception expected: It should not be to set a variable");
		}
		catch (AuthorizationException e)
		{
		  // then (3)
		  verifyMessageIsValid(processInstanceId, e.Message);
		}
	  }

	  public virtual void testUpdateVariablesLocalWithUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		verifyUpdateVariablesLocal(processInstanceId);
	  }

	  public virtual void testUpdateVariablesLocalWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		verifyUpdateVariablesLocal(processInstanceId);
	  }

	  public virtual void testUpdateVariablesLocalWithUpdateInstancePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE);

		verifyUpdateVariablesLocal(processInstanceId);
	  }

	  public virtual void testUpdateVariablesLocalWithUpdateInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE);

		verifyUpdateVariablesLocal(processInstanceId);
	  }

	  public virtual void testUpdateVariablesLocalWithUpdateVariablePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE_VARIABLE);

		verifyUpdateVariablesLocal(processInstanceId);
	  }

	  public virtual void testUpdateVariablesLocalWithUpdateVariablePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE_VARIABLE);

		verifyUpdateVariablesLocal(processInstanceId);
	  }

	  public virtual void testUpdateVariablesLocalWithUpdateInstanceVariablePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_INSTANCE_VARIABLE);

		verifyUpdateVariablesLocal(processInstanceId);
	  }

	  public virtual void testUpdateVariablesLocalWithUpdateInstanceVariablePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE_VARIABLE);

		verifyUpdateVariablesLocal(processInstanceId);
	  }

	  // helper /////////////////////////////////////////////////////

	  protected internal virtual void verifyQueryResults(ProcessInstanceQuery query, int countExpected)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: verifyQueryResults((org.camunda.bpm.engine.impl.AbstractQuery<?, ?>) query, countExpected);
		verifyQueryResults((AbstractQuery<object, ?>) query, countExpected);
	  }

	  protected internal virtual void verifyQueryResults(VariableInstanceQuery query, int countExpected)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: verifyQueryResults((org.camunda.bpm.engine.impl.AbstractQuery<?, ?>) query, countExpected);
		verifyQueryResults((AbstractQuery<object, ?>) query, countExpected);
	  }

	  protected internal virtual void verifyMessageIsValid(string processInstanceId, string message)
	  {
		assertTextPresent(userId, message);
		assertTextPresent(UPDATE.Name, message);
		assertTextPresent(UPDATE_VARIABLE.Name, message);
		assertTextPresent(processInstanceId, message);
		assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		assertTextPresent(UPDATE_INSTANCE.Name, message);
		assertTextPresent(UPDATE_INSTANCE_VARIABLE.Name, message);
		assertTextPresent(PROCESS_KEY, message);
		assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
	  }

	  protected internal virtual void verifyVariableInstanceCountDisabledAuthorization(int count)
	  {
		disableAuthorization();
		verifyQueryResults(runtimeService.createVariableInstanceQuery(), count);
		enableAuthorization();
	  }

	  protected internal virtual void verifySetVariable(string processInstanceId)
	  {
		// when
		runtimeService.setVariable(processInstanceId, VARIABLE_NAME, VARIABLE_VALUE);

		// then
		verifyVariableInstanceCountDisabledAuthorization(1);
	  }

	  protected internal virtual void verifySetVariableLocal(string processInstanceId)
	  {
		// when
		runtimeService.setVariableLocal(processInstanceId, VARIABLE_NAME, VARIABLE_VALUE);

		// then
		verifyVariableInstanceCountDisabledAuthorization(1);
	  }

	  protected internal virtual void verifySetVariables(string processInstanceId)
	  {
		// when
		runtimeService.setVariables(processInstanceId, Variables);

		// then
		verifyVariableInstanceCountDisabledAuthorization(1);
	  }

	  protected internal virtual void verifySetVariablesLocal(string processInstanceId)
	  {
		// when
		runtimeService.setVariablesLocal(processInstanceId, Variables);

		// then
		verifyVariableInstanceCountDisabledAuthorization(1);
	  }

	  protected internal virtual void verifyRemoveVariable(string processInstanceId)
	  {
		// when
		runtimeService.removeVariable(processInstanceId, VARIABLE_NAME);

		// then
		verifyVariableInstanceCountDisabledAuthorization(0);
	  }

	  protected internal virtual void verifyRemoveVariableLocal(string processInstanceId)
	  {
		// when
		runtimeService.removeVariableLocal(processInstanceId, VARIABLE_NAME);

		// then
		verifyVariableInstanceCountDisabledAuthorization(0);
	  }

	  protected internal virtual void verifyRemoveVariables(string processInstanceId)
	  {
		// when
		runtimeService.removeVariables(processInstanceId, Arrays.asList(VARIABLE_NAME));

		// then
		verifyVariableInstanceCountDisabledAuthorization(0);
	  }

	  protected internal virtual void verifyRemoveVariablesLocal(string processInstanceId)
	  {
		// when
		runtimeService.removeVariablesLocal(processInstanceId, Arrays.asList(VARIABLE_NAME));

		// then
		verifyVariableInstanceCountDisabledAuthorization(0);
	  }

	  protected internal virtual void verifyUpdateVariables(string processInstanceId)
	  {
		// when (1)
		((RuntimeServiceImpl)runtimeService).updateVariables(processInstanceId, Variables, null);

		// then (1)
		verifyVariableInstanceCountDisabledAuthorization(1);

		// when (2)
		((RuntimeServiceImpl)runtimeService).updateVariables(processInstanceId, null, Arrays.asList(VARIABLE_NAME));

		// then (2)
		verifyVariableInstanceCountDisabledAuthorization(0);

		// when (3)
		((RuntimeServiceImpl)runtimeService).updateVariables(processInstanceId, Variables, Arrays.asList(VARIABLE_NAME));

		// then (3)
		verifyVariableInstanceCountDisabledAuthorization(0);
	  }

	  protected internal virtual void verifyUpdateVariablesLocal(string processInstanceId)
	  {
		// when (1)
		((RuntimeServiceImpl)runtimeService).updateVariablesLocal(processInstanceId, Variables, null);

		// then (1)
		verifyVariableInstanceCountDisabledAuthorization(1);

		// when (2)
		((RuntimeServiceImpl)runtimeService).updateVariablesLocal(processInstanceId, null, Arrays.asList(VARIABLE_NAME));

		// then (2)
		verifyVariableInstanceCountDisabledAuthorization(0);

		// when (3)
		((RuntimeServiceImpl)runtimeService).updateVariablesLocal(processInstanceId, Variables, Arrays.asList(VARIABLE_NAME));

		// then (3)
		verifyVariableInstanceCountDisabledAuthorization(0);
	  }

	  protected internal virtual void setReadVariableAsDefaultReadVariablePermission()
	  {
		processEngineConfiguration.EnforceSpecificVariablePermission = true;
	  }

	  protected internal virtual void verifyGetVariables(IDictionary<string, object> variables)
	  {
		assertNotNull(variables);
		assertFalse(variables.Count == 0);
		assertEquals(1, variables.Count);

		assertEquals(VARIABLE_VALUE, variables[VARIABLE_NAME]);
	  }

	}

}