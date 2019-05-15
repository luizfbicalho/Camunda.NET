using System.Collections.Generic;
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
//	import static org.camunda.bpm.engine.authorization.Permissions.ALL;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.CREATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.CREATE_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.ProcessDefinitionPermissions.SUSPEND_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_INSTANCE;


	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using ProcessDefinitionPermissions = org.camunda.bpm.engine.authorization.ProcessDefinitionPermissions;
	using ProcessInstancePermissions = org.camunda.bpm.engine.authorization.ProcessInstancePermissions;
	using AbstractQuery = org.camunda.bpm.engine.impl.AbstractQuery;
	using RepositoryServiceImpl = org.camunda.bpm.engine.impl.RepositoryServiceImpl;
	using ReadOnlyProcessDefinition = org.camunda.bpm.engine.impl.pvm.ReadOnlyProcessDefinition;
	using DiagramLayout = org.camunda.bpm.engine.repository.DiagramLayout;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessDefinitionQuery = org.camunda.bpm.engine.repository.ProcessDefinitionQuery;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class ProcessDefinitionAuthorizationTest : AuthorizationTest
	{

	  protected internal const string ONE_TASK_PROCESS_KEY = "oneTaskProcess";
	  protected internal const string TWO_TASKS_PROCESS_KEY = "twoTasksProcess";

	  protected internal new string deploymentId;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setUp() throws Exception
	  public override void setUp()
	  {
		deploymentId = createDeployment(null, "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/twoTasksProcess.bpmn20.xml").Id;
		base.setUp();
	  }

	  public override void tearDown()
	  {
		base.tearDown();
		deleteDeployment(deploymentId);
	  }

	  public virtual void testQueryWithoutAuthorization()
	  {
		// given
		// given user is not authorized to read any process definition

		// when
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testQueryWithReadPermissionOnAnyProcessDefinition()
	  {
		// given
		// given user gets read permission on any process definition
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ);

		// when
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		// then
		verifyQueryResults(query, 2);
	  }

	  public virtual void testQueryWithMultiple()
	  {
		// given
		// given user gets read permission on any process definition
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ);
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, READ);

		// when
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		// then
		verifyQueryResults(query, 2);
	  }

	  public virtual void testQueryWithReadPermissionOnOneTaskProcess()
	  {
		// given
		// given user gets read permission on "oneTaskProcess" process definition
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, READ);

		// when
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		// then
		verifyQueryResults(query, 1);

		ProcessDefinition definition = query.singleResult();
		assertNotNull(definition);
		assertEquals(ONE_TASK_PROCESS_KEY, definition.Key);
	  }

	  public virtual void testQueryWithRevokedReadPermission()
	  {
		// given
		// given user gets all permissions on any process definition
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, ALL);

		Authorization authorization = createRevokeAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY);
		authorization.UserId = userId;
		authorization.removePermission(READ);
		saveAuthorization(authorization);

		// when
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		// then
		verifyQueryResults(query, 1);

		ProcessDefinition definition = query.singleResult();
		assertNotNull(definition);
		assertEquals(TWO_TASKS_PROCESS_KEY, definition.Key);
	  }

	  public virtual void testQueryWithGroupAuthorizationRevokedReadPermission()
	  {
		// given
		// given user gets all permissions on any process definition
		Authorization authorization = createGrantAuthorization(PROCESS_DEFINITION, ANY);
		authorization.GroupId = groupId;
		authorization.addPermission(ALL);
		saveAuthorization(authorization);

		authorization = createRevokeAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY);
		authorization.GroupId = groupId;
		authorization.removePermission(READ);
		saveAuthorization(authorization);

		// when
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		// then
		verifyQueryResults(query, 1);

		ProcessDefinition definition = query.singleResult();
		assertNotNull(definition);
		assertEquals(TWO_TASKS_PROCESS_KEY, definition.Key);
	  }

	  // get process definition /////////////////////////////////////////////////////

	  public virtual void testGetProcessDefinitionWithoutAuthorizations()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;

		try
		{
		  // when
		  repositoryService.getProcessDefinition(processDefinitionId);
		  fail("Exception expected: It should not be possible to get the process definition");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(ONE_TASK_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testGetProcessDefinition()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, READ);

		// when
		ProcessDefinition definition = repositoryService.getProcessDefinition(processDefinitionId);

		// then
		assertNotNull(definition);
	  }

	  // get deployed process definition /////////////////////////////////////////////////////

	  public virtual void testGetDeployedProcessDefinitionWithoutAuthorizations()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;

		try
		{
		  // when
		  ((RepositoryServiceImpl)repositoryService).getDeployedProcessDefinition(processDefinitionId);
		  fail("Exception expected: It should not be possible to get the process definition");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(ONE_TASK_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testGetDeployedProcessDefinition()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, READ);

		// when
		ReadOnlyProcessDefinition definition = ((RepositoryServiceImpl)repositoryService).getDeployedProcessDefinition(processDefinitionId);

		// then
		assertNotNull(definition);
	  }

	  // get process diagram /////////////////////////////////////////////////////

	  public virtual void testGetProcessDiagramWithoutAuthorizations()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;

		try
		{
		  // when
		  repositoryService.getProcessDiagram(processDefinitionId);
		  fail("Exception expected: It should not be possible to get the process diagram");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(ONE_TASK_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testGetProcessDiagram()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, READ);

		// when
		Stream stream = repositoryService.getProcessDiagram(processDefinitionId);

		// then
		// no process diagram deployed
		assertNull(stream);
	  }

	  // get process model /////////////////////////////////////////////////////

	  public virtual void testGetProcessModelWithoutAuthorizations()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;

		try
		{
		  // when
		  repositoryService.getProcessModel(processDefinitionId);
		  fail("Exception expected: It should not be possible to get the process model");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(ONE_TASK_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testGetProcessModel()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, READ);

		// when
		Stream stream = repositoryService.getProcessModel(processDefinitionId);

		// then
		assertNotNull(stream);
	  }

	  // get bpmn model instance /////////////////////////////////////////////////////

	  public virtual void testGetBpmnModelInstanceWithoutAuthorizations()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;

		try
		{
		  // when
		  repositoryService.getBpmnModelInstance(processDefinitionId);
		  fail("Exception expected: It should not be possible to get the bpmn model instance");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(ONE_TASK_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testGetBpmnModelInstance()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, READ);

		// when
		BpmnModelInstance modelInstance = repositoryService.getBpmnModelInstance(processDefinitionId);

		// then
		assertNotNull(modelInstance);
	  }

	  // get process diagram layout /////////////////////////////////////////////////

	  public virtual void testGetProcessDiagramLayoutWithoutAuthorization()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;

		try
		{
		  // when
		  repositoryService.getProcessDiagramLayout(processDefinitionId);
		  fail("Exception expected: It should not be possible to get the process diagram layout");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(ONE_TASK_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testGetProcessDiagramLayout()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, READ);

		// when
		DiagramLayout diagramLayout = repositoryService.getProcessDiagramLayout(processDefinitionId);

		// then
		// no process diagram deployed
		assertNull(diagramLayout);
	  }

	  // suspend process definition by id ///////////////////////////////////////////

	  public virtual void testSuspendProcessDefinitionByIdWithoutAuthorization()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;

		try
		{
		  // when
		  repositoryService.suspendProcessDefinitionById(processDefinitionId);
		  fail("Exception expected: It should not be possible to suspend the process definition");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(ProcessDefinitionPermissions.SUSPEND.Name, message);
		  assertTextPresent(ONE_TASK_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testSuspendProcessDefinitionById()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, UPDATE);

		// when
		repositoryService.suspendProcessDefinitionById(processDefinitionId);

		// then
		ProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		assertTrue(definition.Suspended);
	  }

	  public virtual void testSuspendProcessDefinitionByIdWithSuspendPermission()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, ProcessDefinitionPermissions.SUSPEND);

		// when
		repositoryService.suspendProcessDefinitionById(processDefinitionId);

		// then
		ProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		assertTrue(definition.Suspended);
	  }

	  // activate process definition by id ///////////////////////////////////////////

	  public virtual void testActivateProcessDefinitionByIdWithoutAuthorization()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
		suspendProcessDefinitionById(processDefinitionId);

		try
		{
		  // when
		  repositoryService.activateProcessDefinitionById(processDefinitionId);
		  fail("Exception expected: It should not be possible to activate the process definition");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(ProcessDefinitionPermissions.SUSPEND.Name, message);
		  assertTextPresent(ONE_TASK_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testActivateProcessDefinitionById()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
		suspendProcessDefinitionById(processDefinitionId);
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, UPDATE);

		// when
		repositoryService.activateProcessDefinitionById(processDefinitionId);

		// then
		ProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		assertFalse(definition.Suspended);
	  }


	  public virtual void testActivateProcessDefinitionByIdWithSuspendPermission()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
		suspendProcessDefinitionById(processDefinitionId);
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, ProcessDefinitionPermissions.SUSPEND);

		// when
		repositoryService.activateProcessDefinitionById(processDefinitionId);

		// then
		ProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		assertFalse(definition.Suspended);
	  }

	  // suspend process definition by id including instances ///////////////////////////////////////////

	  public virtual void testSuspendProcessDefinitionByIdIncludingInstancesWithoutAuthorization()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, UPDATE);

		try
		{
		  // when
		  repositoryService.suspendProcessDefinitionById(processDefinitionId, true, null);
		  fail("Exception expected: It should not be possible to suspend the process definition");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(ProcessInstancePermissions.SUSPEND.Name, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(SUSPEND_INSTANCE.Name, message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(ONE_TASK_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testSuspendProcessDefinitionByIdIncludingInstancesWithUpdatePermissionOnProcessInstance()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, UPDATE, ProcessDefinitionPermissions.SUSPEND);

		string processInstanceId = startProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE, ProcessInstancePermissions.SUSPEND);

		try
		{
		  // when
		  repositoryService.suspendProcessDefinitionById(processDefinitionId, true, null);
		  fail("Exception expected: It should not be possible to suspend the process definition");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(ProcessInstancePermissions.SUSPEND.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(SUSPEND_INSTANCE.Name, message);
		  assertTextPresent(ONE_TASK_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testSuspendProcessDefinitionByIdIncludingInstancesWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, UPDATE);

		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		// when
		repositoryService.suspendProcessDefinitionById(processDefinitionId, true, null);

		// then
		ProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		assertTrue(definition.Suspended);

		ProcessInstance instance = selectSingleProcessInstance();
		assertTrue(instance.Suspended);
	  }

	  public virtual void testSuspendProcessDefinitionByIdIncludingInstancesWithSuspendPermissionOnAnyProcessInstance()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, ProcessDefinitionPermissions.SUSPEND);

		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, ProcessInstancePermissions.SUSPEND);

		// when
		repositoryService.suspendProcessDefinitionById(processDefinitionId, true, null);

		// then
		verifyProcessDefinitionSuspendedByKeyIncludingInstances();
	  }

	  public virtual void testSuspendProcessDefinitionByIdIncludingInstancesWithUpdateInstancePermissionOnProcessDefinition()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, UPDATE, UPDATE_INSTANCE);

		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

		// when
		repositoryService.suspendProcessDefinitionById(processDefinitionId, true, null);

		// then
		ProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		assertTrue(definition.Suspended);

		ProcessInstance instance = selectSingleProcessInstance();
		assertTrue(instance.Suspended);
	  }

	  public virtual void testSuspendProcessDefinitionByIdIncludingInstancesWithUpdateAndSuspendInstancePermissionOnProcessDefinition()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, UPDATE, SUSPEND_INSTANCE);

		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

		// when
		repositoryService.suspendProcessDefinitionById(processDefinitionId, true, null);

		// then
		verifyProcessDefinitionSuspendedByKeyIncludingInstances();
	  }

	  public virtual void testSuspendProcessDefinitionByIdIncludingInstancesWithSuspendAndSuspendInstancePermissionOnProcessDefinition()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, ProcessDefinitionPermissions.SUSPEND, SUSPEND_INSTANCE);

		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

		// when
		repositoryService.suspendProcessDefinitionById(processDefinitionId, true, null);

		// then
		verifyProcessDefinitionSuspendedByKeyIncludingInstances();
	  }

	  public virtual void testSuspendProcessDefinitionByIdIncludingInstancesWithSuspendAndUpdateInstancePermissionOnProcessDefinition()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, ProcessDefinitionPermissions.SUSPEND, UPDATE_INSTANCE);

		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

		// when
		repositoryService.suspendProcessDefinitionById(processDefinitionId, true, null);

		// then
		verifyProcessDefinitionSuspendedByKeyIncludingInstances();
	  }

	  // activate process definition by id including instances ///////////////////////////////////////////

	  public virtual void testActivateProcessDefinitionByIdIncludingInstancesWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
		suspendProcessDefinitionById(processDefinitionId);
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, UPDATE, ProcessDefinitionPermissions.SUSPEND);

		try
		{
		  // when
		  repositoryService.activateProcessDefinitionById(processDefinitionId, true, null);
		  fail("Exception expected: It should not be possible to activate the process definition");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(ProcessInstancePermissions.SUSPEND.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(SUSPEND_INSTANCE.Name, message);
		  assertTextPresent(ONE_TASK_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testActivateProcessDefinitionByIdIncludingInstancesWithUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
		suspendProcessDefinitionById(processDefinitionId);
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, UPDATE);

		try
		{
		  // when
		  repositoryService.activateProcessDefinitionById(processDefinitionId, true, null);
		  fail("Exception expected: It should not be possible to activate the process definition");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(ProcessInstancePermissions.SUSPEND.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(SUSPEND_INSTANCE.Name, message);
		  assertTextPresent(ONE_TASK_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testActivateProcessDefinitionByIdIncludingInstancesWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
		suspendProcessDefinitionById(processDefinitionId);
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, UPDATE);

		// when
		repositoryService.activateProcessDefinitionById(processDefinitionId, true, null);

		// then
		ProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		assertFalse(definition.Suspended);

		ProcessInstance instance = selectSingleProcessInstance();
		assertFalse(instance.Suspended);
	  }

	  public virtual void testActivateProcessDefinitionByIdIncludingInstancesWithSuspendPermissionOnAnyProcessInstance()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, ProcessInstancePermissions.SUSPEND);

		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
		suspendProcessDefinitionById(processDefinitionId);
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, ProcessDefinitionPermissions.SUSPEND);

		// when
		repositoryService.activateProcessDefinitionById(processDefinitionId, true, null);

		// then
		verifyProcessDefinitionActivatedByKeyIncludingInstances();
	  }

	  public virtual void testActivateProcessDefinitionByIdIncludingInstancesWithUpdateInstancePermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
		suspendProcessDefinitionById(processDefinitionId);
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, UPDATE, UPDATE_INSTANCE);

		// when
		repositoryService.activateProcessDefinitionById(processDefinitionId, true, null);

		// then
		ProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		assertFalse(definition.Suspended);

		ProcessInstance instance = selectSingleProcessInstance();
		assertFalse(instance.Suspended);
	  }

	  public virtual void testActivateProcessDefinitionByIdIncludingInstancesWithSuspendAndSuspendInstancePermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
		suspendProcessDefinitionById(processDefinitionId);
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, ProcessDefinitionPermissions.SUSPEND, SUSPEND_INSTANCE);

		// when
		repositoryService.activateProcessDefinitionById(processDefinitionId, true, null);

		// then
		verifyProcessDefinitionActivatedByKeyIncludingInstances();
	  }

	  public virtual void testActivateProcessDefinitionByIdIncludingInstancesWithSuspendAndUpdateInstancePermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
		suspendProcessDefinitionById(processDefinitionId);
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, ProcessDefinitionPermissions.SUSPEND, UPDATE_INSTANCE);

		// when
		repositoryService.activateProcessDefinitionById(processDefinitionId, true, null);

		// then
		verifyProcessDefinitionActivatedByKeyIncludingInstances();
	  }

	  public virtual void testActivateProcessDefinitionByIdIncludingInstancesWithUpdateAndSuspendInstancePermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

		string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
		suspendProcessDefinitionById(processDefinitionId);
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, UPDATE, SUSPEND_INSTANCE);

		// when
		repositoryService.activateProcessDefinitionById(processDefinitionId, true, null);

		// then
		verifyProcessDefinitionActivatedByKeyIncludingInstances();
	  }

	  // suspend process definition by key ///////////////////////////////////////////

	  public virtual void testSuspendProcessDefinitionByKeyWithoutAuthorization()
	  {
		// given

		try
		{
		  // when
		  repositoryService.suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		  fail("Exception expected: It should not be possible to suspend the process definition");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(ProcessDefinitionPermissions.SUSPEND.Name, message);
		  assertTextPresent(ONE_TASK_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testSuspendProcessDefinitionByKey()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, UPDATE);

		// when
		repositoryService.suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);

		// then
		ProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		assertTrue(definition.Suspended);
	  }

	  public virtual void testSuspendProcessDefinitionByKeyWithSuspendPermission()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, ProcessDefinitionPermissions.SUSPEND);

		// when
		repositoryService.suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);

		// then
		ProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		assertTrue(definition.Suspended);
	  }

	  // activate process definition by id ///////////////////////////////////////////

	  public virtual void testActivateProcessDefinitionByKeyWithoutAuthorization()
	  {
		// given
		suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);

		try
		{
		  // when
		  repositoryService.activateProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		  fail("Exception expected: It should not be possible to activate the process definition");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(ProcessDefinitionPermissions.SUSPEND.Name, message);
		  assertTextPresent(ONE_TASK_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testActivateProcessDefinitionByKey()
	  {
		// given
		suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, UPDATE);

		// when
		repositoryService.activateProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);

		// then
		ProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		assertFalse(definition.Suspended);
	  }

	  public virtual void testActivateProcessDefinitionByKeyWithSuspendPermission()
	  {
		// given
		suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, ProcessDefinitionPermissions.SUSPEND);

		// when
		repositoryService.activateProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);

		// then
		ProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		assertFalse(definition.Suspended);
	  }

	  // suspend process definition by key including instances ///////////////////////////////////////////

	  public virtual void testSuspendProcessDefinitionByKeyIncludingInstancesWithoutAuthorization()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, UPDATE, ProcessDefinitionPermissions.SUSPEND);

		try
		{
		  // when
		  repositoryService.suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY, true, null);
		  fail("Exception expected: It should not be possible to suspend the process definition");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(ProcessDefinitionPermissions.SUSPEND.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(SUSPEND_INSTANCE.Name, message);
		  assertTextPresent(ONE_TASK_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testSuspendProcessDefinitionByKeyIncludingInstancesWithUpdatePermissionOnProcessInstance()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, UPDATE, ProcessDefinitionPermissions.SUSPEND);

		string processInstanceId = startProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE, ProcessInstancePermissions.SUSPEND);

		try
		{
		  // when
		  repositoryService.suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY, true, null);
		  fail("Exception expected: It should not be possible to suspend the process definition");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(ProcessInstancePermissions.SUSPEND.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(SUSPEND_INSTANCE.Name, message);
		  assertTextPresent(ONE_TASK_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testSuspendProcessDefinitionByKeyIncludingInstancesWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, UPDATE);

		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		// when
		repositoryService.suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY, true, null);

		// then
		ProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		assertTrue(definition.Suspended);

		ProcessInstance instance = selectSingleProcessInstance();
		assertTrue(instance.Suspended);
	  }

	  public virtual void testSuspendProcessDefinitionByKeyIncludingInstancesWithSuspendPermissionOnAnyProcessInstance()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, ProcessDefinitionPermissions.SUSPEND);

		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, ProcessInstancePermissions.SUSPEND);

		// when
		repositoryService.suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY, true, null);

		// then
		verifyProcessDefinitionSuspendedByKeyIncludingInstances();
	  }

	  public virtual void testSuspendProcessDefinitionByKeyIncludingInstancesWithUpdateInstancePermissionOnProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, UPDATE, UPDATE_INSTANCE);

		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

		// when
		repositoryService.suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY, true, null);

		// then
		ProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		assertTrue(definition.Suspended);

		ProcessInstance instance = selectSingleProcessInstance();
		assertTrue(instance.Suspended);
	  }

	  public virtual void testSuspendProcessDefinitionByKeyIncludingInstancesWithSuspendAndSuspendInstancePermissionOnProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, ProcessDefinitionPermissions.SUSPEND, SUSPEND_INSTANCE);

		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

		// when
		repositoryService.suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY, true, null);

		// then
		verifyProcessDefinitionSuspendedByKeyIncludingInstances();
	  }


	  public virtual void testSuspendProcessDefinitionByKeyIncludingInstancesWithSuspendAndUpdateInstancePermissionOnProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, ProcessDefinitionPermissions.SUSPEND, UPDATE_INSTANCE);

		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

		// when
		repositoryService.suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY, true, null);

		// then
		verifyProcessDefinitionSuspendedByKeyIncludingInstances();
	  }


	  public virtual void testSuspendProcessDefinitionByKeyIncludingInstancesWithUpdateAndSuspendInstancePermissionOnProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, UPDATE, SUSPEND_INSTANCE);

		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

		// when
		repositoryService.suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY, true, null);

		// then
		verifyProcessDefinitionSuspendedByKeyIncludingInstances();
	  }

	  // activate process definition by key including instances ///////////////////////////////////////////

	  public virtual void testActivateProcessDefinitionByKeyIncludingInstancesWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

		suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, UPDATE);

		try
		{
		  // when
		  repositoryService.activateProcessDefinitionByKey(ONE_TASK_PROCESS_KEY, true, null);
		  fail("Exception expected: It should not be possible to activate the process definition");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(ProcessInstancePermissions.SUSPEND.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(SUSPEND_INSTANCE.Name, message);
		  assertTextPresent(ONE_TASK_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testActivateProcessDefinitionByKeyIncludingInstancesWithUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE, ProcessInstancePermissions.SUSPEND);

		suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, UPDATE, ProcessDefinitionPermissions.SUSPEND);

		try
		{
		  // when
		  repositoryService.activateProcessDefinitionByKey(ONE_TASK_PROCESS_KEY, true, null);
		  fail("Exception expected: It should not be possible to activate the process definition");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(ProcessInstancePermissions.SUSPEND.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(SUSPEND_INSTANCE.Name, message);
		  assertTextPresent(ONE_TASK_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testActivateProcessDefinitionByKeyIncludingInstancesWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, UPDATE);

		// when
		repositoryService.activateProcessDefinitionByKey(ONE_TASK_PROCESS_KEY, true, null);

		// then
		ProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		assertFalse(definition.Suspended);

		ProcessInstance instance = selectSingleProcessInstance();
		assertFalse(instance.Suspended);
	  }

	  public virtual void testActivateProcessDefinitionByKeyIncludingInstancesWithSuspendPermissionOnAnyProcessInstance()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, ProcessInstancePermissions.SUSPEND);

		suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, ProcessDefinitionPermissions.SUSPEND);

		// when
		repositoryService.activateProcessDefinitionByKey(ONE_TASK_PROCESS_KEY, true, null);

		// then
		verifyProcessDefinitionActivatedByKeyIncludingInstances();
	  }

	  public virtual void testActivateProcessDefinitionByKeyIncludingInstancesWithUpdateInstancePermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

		suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, UPDATE, UPDATE_INSTANCE);

		// when
		repositoryService.activateProcessDefinitionByKey(ONE_TASK_PROCESS_KEY, true, null);

		// then
		ProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		assertFalse(definition.Suspended);

		ProcessInstance instance = selectSingleProcessInstance();
		assertFalse(instance.Suspended);
	  }

	  public virtual void testActivateProcessDefinitionByKeyIncludingInstancesWithSuspendAndSuspendInstancePermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

		suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, ProcessDefinitionPermissions.SUSPEND, SUSPEND_INSTANCE);

		// when
		repositoryService.activateProcessDefinitionByKey(ONE_TASK_PROCESS_KEY, true, null);

		// then
		verifyProcessDefinitionActivatedByKeyIncludingInstances();
	  }


	  public virtual void testActivateProcessDefinitionByKeyIncludingInstancesWithSuspendAndUpdateInstancePermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

		suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, ProcessDefinitionPermissions.SUSPEND, UPDATE_INSTANCE);

		// when
		repositoryService.activateProcessDefinitionByKey(ONE_TASK_PROCESS_KEY, true, null);

		// then
		verifyProcessDefinitionActivatedByKeyIncludingInstances();
	  }


	  public virtual void testActivateProcessDefinitionByKeyIncludingInstancesWithUpdateAndSuspendInstancePermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

		suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, UPDATE, SUSPEND_INSTANCE);

		// when
		repositoryService.activateProcessDefinitionByKey(ONE_TASK_PROCESS_KEY, true, null);

		// then
		verifyProcessDefinitionActivatedByKeyIncludingInstances();
	  }


	  // update history time to live ///////////////////////////////////////////

	  public virtual void testProcessDefinitionUpdateTimeToLive()
	  {

		// given
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, UPDATE);
		ProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);

		// when
		repositoryService.updateProcessDefinitionHistoryTimeToLive(definition.Id, 6);

		// then
		definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		assertEquals(6, definition.HistoryTimeToLive.Value);

	  }

	  public virtual void testDecisionDefinitionUpdateTimeToLiveWithoutAuthorizations()
	  {
		//given
		ProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		try
		{
		  //when
		  repositoryService.updateProcessDefinitionHistoryTimeToLive(definition.Id, 6);
		  fail("Exception expected");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(ONE_TASK_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}

	  }

	  // startable in tasklist ///////////////////////////////////////////

	  public virtual void testStartableInTasklist()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, READ, CREATE_INSTANCE);
		createGrantAuthorization(PROCESS_INSTANCE, "*", userId, CREATE);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.repository.ProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		ProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);

		// when
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().startablePermissionCheck().startableInTasklist().list();
		// then
		assertNotNull(processDefinitions);
		assertEquals(1, repositoryService.createProcessDefinitionQuery().startablePermissionCheck().startableInTasklist().count());
		assertEquals(definition.Id, processDefinitions[0].Id);
		assertTrue(processDefinitions[0].StartableInTasklist);
	  }

	  public virtual void testStartableInTasklistReadAllProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, "*", userId, READ);
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, CREATE_INSTANCE);
		createGrantAuthorization(PROCESS_INSTANCE, "*", userId, CREATE);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.repository.ProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		ProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);

		// when
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().startablePermissionCheck().startableInTasklist().list();
		// then
		assertNotNull(processDefinitions);
		assertEquals(1, repositoryService.createProcessDefinitionQuery().startablePermissionCheck().startableInTasklist().count());
		assertEquals(definition.Id, processDefinitions[0].Id);
		assertTrue(processDefinitions[0].StartableInTasklist);
	  }

	  public virtual void testStartableInTasklistWithoutCreateInstancePerm()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, READ);
		createGrantAuthorization(PROCESS_INSTANCE, "*", userId, CREATE);
		selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);

		// when
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().startablePermissionCheck().startableInTasklist().list();
		// then
		assertNotNull(processDefinitions);
		assertEquals(0, processDefinitions.Count);
	  }

	  public virtual void testStartableInTasklistWithoutReadDefPerm()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, CREATE_INSTANCE);
		createGrantAuthorization(PROCESS_INSTANCE, "*", userId, CREATE);
		selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);

		// when
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().startablePermissionCheck().startableInTasklist().list();
		// then
		assertNotNull(processDefinitions);
		assertEquals(0, processDefinitions.Count);
	  }

	  public virtual void testStartableInTasklistWithoutCreatePerm()
	  {
		// given
		selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);

		// when
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().startablePermissionCheck().startableInTasklist().list();
		// then
		assertNotNull(processDefinitions);
		assertEquals(0, processDefinitions.Count);
	  }

	  // helper /////////////////////////////////////////////////////////////////////

	  protected internal virtual void verifyQueryResults(ProcessDefinitionQuery query, int countExpected)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: verifyQueryResults((org.camunda.bpm.engine.impl.AbstractQuery<?, ?>) query, countExpected);
		verifyQueryResults((AbstractQuery<object, ?>) query, countExpected);
	  }

	  protected internal virtual void verifyProcessDefinitionSuspendedByKeyIncludingInstances()
	  {
		ProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		assertTrue(definition.Suspended);

		ProcessInstance instance = selectSingleProcessInstance();
		assertTrue(instance.Suspended);
	  }

	  protected internal virtual void verifyProcessDefinitionActivatedByKeyIncludingInstances()
	  {
		ProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		assertFalse(definition.Suspended);

		ProcessInstance instance = selectSingleProcessInstance();
		assertFalse(instance.Suspended);
	  }

	}

}