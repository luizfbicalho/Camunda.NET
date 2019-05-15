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
//	import static org.camunda.bpm.engine.authorization.Permissions.READ;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_INSTANCE;

	using AbstractQuery = org.camunda.bpm.engine.impl.AbstractQuery;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using JobDefinitionQuery = org.camunda.bpm.engine.management.JobDefinitionQuery;
	using Job = org.camunda.bpm.engine.runtime.Job;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class JobDefinitionAuthorizationTest : AuthorizationTest
	{

	  protected internal const string TIMER_START_PROCESS_KEY = "timerStartProcess";
	  protected internal const string TIMER_BOUNDARY_PROCESS_KEY = "timerBoundaryProcess";

	  protected internal new string deploymentId;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setUp() throws Exception
	  public override void setUp()
	  {
		deploymentId = createDeployment(null, "org/camunda/bpm/engine/test/api/authorization/timerStartEventProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/authorization/timerBoundaryEventProcess.bpmn20.xml").Id;
		base.setUp();
	  }

	  public override void tearDown()
	  {
		base.tearDown();
		deleteDeployment(deploymentId);
	  }

	  // job definition query ///////////////////////////////////////

	  public virtual void testQueryWithoutAuthorization()
	  {
		// given

		// when
		JobDefinitionQuery query = managementService.createJobDefinitionQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testQueryWithReadPermissionOnProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, TIMER_START_PROCESS_KEY, userId, READ);

		// when
		JobDefinitionQuery query = managementService.createJobDefinitionQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryWithReadPermissionOnAnyProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ);

		// when
		JobDefinitionQuery query = managementService.createJobDefinitionQuery();

		// then
		verifyQueryResults(query, 2);
	  }

	  public virtual void testQueryWithMultiple()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ);
		createGrantAuthorization(PROCESS_DEFINITION, TIMER_START_PROCESS_KEY, userId, READ);

		// when
		JobDefinitionQuery query = managementService.createJobDefinitionQuery();

		// then
		verifyQueryResults(query, 2);
	  }

	  // suspend job definition by id ///////////////////////////////

	  public virtual void testSuspendByIdWithoutAuthorization()
	  {
		// given
		string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		try
		{
		  // when
		  managementService.suspendJobDefinitionById(jobDefinitionId);
		  fail("Exception expected: It should not be possible to suspend a job definition");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testSuspendByIdWithUpdatePermissionOnProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE);
		string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		// when
		managementService.suspendJobDefinitionById(jobDefinitionId);

		// then
		JobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		assertNotNull(jobDefinition);
		assertTrue(jobDefinition.Suspended);
	  }

	  public virtual void testSuspendByIdWithUpdatePermissionOnAnyProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE);
		string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		// when
		managementService.suspendJobDefinitionById(jobDefinitionId);

		// then
		JobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		assertNotNull(jobDefinition);
		assertTrue(jobDefinition.Suspended);
	  }

	  // activate job definition by id ///////////////////////////////

	  public virtual void testActivateByIdWithoutAuthorization()
	  {
		// given
		string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobDefinitionById(jobDefinitionId);

		try
		{
		  // when
		  managementService.activateJobDefinitionById(jobDefinitionId);
		  fail("Exception expected: It should not be possible to activate a job definition");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testActivateByIdWithUpdatePermissionOnProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE);
		string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobDefinitionById(jobDefinitionId);

		// when
		managementService.activateJobDefinitionById(jobDefinitionId);

		// then
		JobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		assertNotNull(jobDefinition);
		assertFalse(jobDefinition.Suspended);
	  }

	  public virtual void testActivateByIdWithUpdatePermissionOnAnyProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE);
		string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobDefinitionById(jobDefinitionId);

		// when
		managementService.activateJobDefinitionById(jobDefinitionId);

		// then
		JobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		assertNotNull(jobDefinition);
		assertFalse(jobDefinition.Suspended);
	  }

	  // suspend job definition by id (including jobs) ///////////////////////////////

	  public virtual void testSuspendIncludingJobsByIdWithoutAuthorization()
	  {
		// given
		string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE);

		try
		{
		  // when
		  managementService.suspendJobDefinitionById(jobDefinitionId, true);
		  fail("Exception expected: It should not be possible to suspend a job definition");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testSuspendIncludingJobsByIdWithUpdatePermissionOnProcessInstance()
	  {
		// given
		string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE);
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		try
		{
		  // when
		  managementService.suspendJobDefinitionById(jobDefinitionId, true);
		  fail("Exception expected: It should not be possible to suspend a job definition");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testSuspendIncludingJobsByIdWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		// when
		managementService.suspendJobDefinitionById(jobDefinitionId, true);

		// then
		JobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		assertNotNull(jobDefinition);
		assertTrue(jobDefinition.Suspended);

		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertTrue(job.Suspended);
	  }

	  public virtual void testSuspendIncludingJobsByIdWithUpdateInstancePermissionOnProcessDefinition()
	  {
		// given
		string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE, UPDATE_INSTANCE);

		// when
		managementService.suspendJobDefinitionById(jobDefinitionId, true);

		// then
		JobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		assertNotNull(jobDefinition);
		assertTrue(jobDefinition.Suspended);

		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertTrue(job.Suspended);
	  }

	  public virtual void testSuspendIncludingJobsByIdWithUpdateInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE);
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE);

		// when
		managementService.suspendJobDefinitionById(jobDefinitionId, true);

		// then
		JobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		assertNotNull(jobDefinition);
		assertTrue(jobDefinition.Suspended);

		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertTrue(job.Suspended);
	  }

	  // activate job definition by id (including jobs) ///////////////////////////////

	  public virtual void testActivateIncludingJobsByIdWithoutAuthorization()
	  {
		// given
		string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		suspendJobDefinitionIncludingJobsById(jobDefinitionId);

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE);

		try
		{
		  // when
		  managementService.activateJobDefinitionById(jobDefinitionId, true);
		  fail("Exception expected: It should not be possible to suspend a job definition");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testActivateIncludingJobsByIdWithUpdatePermissionOnProcessInstance()
	  {
		// given
		string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobDefinitionIncludingJobsById(jobDefinitionId);

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE);
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		try
		{
		  // when
		  managementService.activateJobDefinitionById(jobDefinitionId, true);
		  fail("Exception expected: It should not be possible to suspend a job definition");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testActivateIncludingJobsByIdWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobDefinitionIncludingJobsById(jobDefinitionId);

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		// when
		managementService.activateJobDefinitionById(jobDefinitionId, true);

		// then
		JobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		assertNotNull(jobDefinition);
		assertFalse(jobDefinition.Suspended);

		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertFalse(job.Suspended);
	  }

	  public virtual void testActivateIncludingJobsByIdWithUpdateInstancePermissionOnProcessDefinition()
	  {
		// given
		string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobDefinitionIncludingJobsById(jobDefinitionId);

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE, UPDATE_INSTANCE);

		// when
		managementService.activateJobDefinitionById(jobDefinitionId, true);

		// then
		JobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		assertNotNull(jobDefinition);
		assertFalse(jobDefinition.Suspended);

		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertFalse(job.Suspended);
	  }

	  public virtual void testActivateIncludingJobsByIdWithUpdateInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobDefinitionIncludingJobsById(jobDefinitionId);

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE);
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE);

		// when
		managementService.activateJobDefinitionById(jobDefinitionId, true);

		// then
		JobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		assertNotNull(jobDefinition);
		assertFalse(jobDefinition.Suspended);

		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertFalse(job.Suspended);
	  }

	  // suspend job definition by process definition id ///////////////////////////////

	  public virtual void testSuspendByProcessDefinitionIdWithoutAuthorization()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		try
		{
		  // when
		  managementService.suspendJobDefinitionByProcessDefinitionId(processDefinitionId);
		  fail("Exception expected: It should not be possible to suspend a job definition");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testSuspendByProcessDefinitionIdWithUpdatePermissionOnProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE);
		string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		// when
		managementService.suspendJobDefinitionByProcessDefinitionId(processDefinitionId);

		// then
		JobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		assertNotNull(jobDefinition);
		assertTrue(jobDefinition.Suspended);
	  }

	  public virtual void testSuspendByProcessDefinitionIdWithUpdatePermissionOnAnyProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE);
		string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		// when
		managementService.suspendJobDefinitionByProcessDefinitionId(processDefinitionId);

		// then
		JobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		assertNotNull(jobDefinition);
		assertTrue(jobDefinition.Suspended);
	  }

	  // activate job definition by process definition id ///////////////////////////////

	  public virtual void testActivateByProcessDefinitionIdWithoutAuthorization()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobDefinitionByProcessDefinitionId(processDefinitionId);

		try
		{
		  // when
		  managementService.activateJobDefinitionByProcessDefinitionId(processDefinitionId);
		  fail("Exception expected: It should not be possible to activate a job definition");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testActivateByProcessDefinitionIdWithUpdatePermissionOnProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE);
		string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobDefinitionByProcessDefinitionId(processDefinitionId);

		// when
		managementService.activateJobDefinitionByProcessDefinitionId(processDefinitionId);

		// then
		JobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		assertNotNull(jobDefinition);
		assertFalse(jobDefinition.Suspended);
	  }

	  public virtual void testActivateByProcessDefinitionIdWithUpdatePermissionOnAnyProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE);
		string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobDefinitionByProcessDefinitionId(processDefinitionId);

		// when
		managementService.activateJobDefinitionByProcessDefinitionId(processDefinitionId);

		// then
		JobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		assertNotNull(jobDefinition);
		assertFalse(jobDefinition.Suspended);
	  }

	  // suspend job definition by process definition id (including jobs) ///////////////////////////////

	  public virtual void testSuspendIncludingJobsByProcessDefinitionIdWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE);

		try
		{
		  // when
		  managementService.suspendJobDefinitionByProcessDefinitionId(processDefinitionId, true);
		  fail("Exception expected: It should not be possible to suspend a job definition");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testSuspendIncludingJobsByProcessDefinitionIdWithUpdatePermissionOnProcessInstance()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE);
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		try
		{
		  // when
		  managementService.suspendJobDefinitionByProcessDefinitionId(processDefinitionId, true);
		  fail("Exception expected: It should not be possible to suspend a job definition");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testSuspendIncludingJobsByProcessDefinitionIdWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		// when
		managementService.suspendJobDefinitionByProcessDefinitionId(processDefinitionId, true);

		// then
		JobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		assertNotNull(jobDefinition);
		assertTrue(jobDefinition.Suspended);

		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertTrue(job.Suspended);
	  }

	  public virtual void testSuspendIncludingJobsByProcessDefinitionIdWithUpdateInstancePermissionOnProcessDefinition()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE, UPDATE_INSTANCE);

		// when
		managementService.suspendJobDefinitionByProcessDefinitionId(processDefinitionId, true);

		// then
		JobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		assertNotNull(jobDefinition);
		assertTrue(jobDefinition.Suspended);

		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertTrue(job.Suspended);
	  }

	  public virtual void testSuspendIncludingJobsByProcessDefinitionIdWithUpdateInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE);
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE);

		// when
		managementService.suspendJobDefinitionByProcessDefinitionId(processDefinitionId, true);

		// then
		JobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		assertNotNull(jobDefinition);
		assertTrue(jobDefinition.Suspended);

		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertTrue(job.Suspended);
	  }

	  // activate job definition by id (including jobs) ///////////////////////////////

	  public virtual void testActivateIncludingJobsByProcessDefinitionIdWithoutAuthorization()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		suspendJobDefinitionIncludingJobsByProcessDefinitionId(processDefinitionId);

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE);

		try
		{
		  // when
		  managementService.activateJobDefinitionByProcessDefinitionId(processDefinitionId, true);
		  fail("Exception expected: It should not be possible to suspend a job definition");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testActivateIncludingJobsByProcessDefinitionIdWithUpdatePermissionOnProcessInstance()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobDefinitionIncludingJobsByProcessDefinitionId(processDefinitionId);

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE);
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		try
		{
		  // when
		  managementService.activateJobDefinitionByProcessDefinitionId(processDefinitionId, true);
		  fail("Exception expected: It should not be possible to suspend a job definition");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testActivateIncludingJobsByProcessDefinitionIdWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobDefinitionIncludingJobsByProcessDefinitionId(processDefinitionId);

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		// when
		managementService.activateJobDefinitionByProcessDefinitionId(processDefinitionId, true);

		// then
		JobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		assertNotNull(jobDefinition);
		assertFalse(jobDefinition.Suspended);

		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertFalse(job.Suspended);
	  }

	  public virtual void testActivateIncludingJobsByProcessDefinitionIdWithUpdateInstancePermissionOnProcessDefinition()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobDefinitionIncludingJobsByProcessDefinitionId(processDefinitionId);

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE, UPDATE_INSTANCE);

		// when
		managementService.activateJobDefinitionByProcessDefinitionId(processDefinitionId, true);

		// then
		JobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		assertNotNull(jobDefinition);
		assertFalse(jobDefinition.Suspended);

		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertFalse(job.Suspended);
	  }

	  public virtual void testActivateIncludingJobsByProcessDefinitionIdWithUpdateInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobDefinitionIncludingJobsByProcessDefinitionId(processDefinitionId);

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE);
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE);

		// when
		managementService.activateJobDefinitionByProcessDefinitionId(processDefinitionId, true);

		// then
		JobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		assertNotNull(jobDefinition);
		assertFalse(jobDefinition.Suspended);

		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertFalse(job.Suspended);
	  }

	  // suspend job definition by process definition key ///////////////////////////////

	  public virtual void testSuspendByProcessDefinitionKeyWithoutAuthorization()
	  {
		// given

		try
		{
		  // when
		  managementService.suspendJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		  fail("Exception expected: It should not be possible to suspend a job definition");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testSuspendByProcessDefinitionKeyWithUpdatePermissionOnProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE);

		// when
		managementService.suspendJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

		// then
		JobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		assertNotNull(jobDefinition);
		assertTrue(jobDefinition.Suspended);
	  }

	  public virtual void testSuspendByProcessDefinitionKeyWithUpdatePermissionOnAnyProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE);

		// when
		managementService.suspendJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

		// then
		JobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		assertNotNull(jobDefinition);
		assertTrue(jobDefinition.Suspended);
	  }

	  // activate job definition by process definition key ///////////////////////////////

	  public virtual void testActivateByProcessDefinitionKeyWithoutAuthorization()
	  {
		// given
		suspendJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

		try
		{
		  // when
		  managementService.activateJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		  fail("Exception expected: It should not be possible to suspend a job definition");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testActivateByProcessDefinitionKeyWithUpdatePermissionOnProcessDefinition()
	  {
		// given
		suspendJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE);

		// when
		managementService.activateJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

		// then
		JobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		assertNotNull(jobDefinition);
		assertFalse(jobDefinition.Suspended);
	  }

	  public virtual void testActivateByProcessDefinitionKeyWithUpdatePermissionOnAnyProcessDefinition()
	  {
		// given
		suspendJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE);

		// when
		managementService.activateJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

		// then
		JobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		assertNotNull(jobDefinition);
		assertFalse(jobDefinition.Suspended);
	  }

	  // suspend job definition by process definition key (including jobs) ///////////////////////////////

	  public virtual void testSuspendIncludingJobsByProcessDefinitionKeyWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE);

		try
		{
		  // when
		  managementService.suspendJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY, true);
		  fail("Exception expected: It should not be possible to suspend a job definition");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testSuspendIncludingJobsByProcessDefinitionKeyWithUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE);
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		try
		{
		  // when
		  managementService.suspendJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY, true);
		  fail("Exception expected: It should not be possible to suspend a job definition");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testSuspendIncludingJobsByProcessDefinitionKeyWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		// when
		managementService.suspendJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY, true);

		// then
		JobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		assertNotNull(jobDefinition);
		assertTrue(jobDefinition.Suspended);

		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertTrue(job.Suspended);
	  }

	  public virtual void testSuspendIncludingJobsByProcessDefinitionKeyWithUpdateInstancePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE, UPDATE_INSTANCE);

		// when
		managementService.suspendJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY, true);

		// then
		JobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		assertNotNull(jobDefinition);
		assertTrue(jobDefinition.Suspended);

		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertTrue(job.Suspended);
	  }

	  public virtual void testSuspendIncludingJobsByProcessDefinitionKeyWithUpdateInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE);
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE);

		// when
		managementService.suspendJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY, true);

		// then
		JobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		assertNotNull(jobDefinition);
		assertTrue(jobDefinition.Suspended);

		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertTrue(job.Suspended);
	  }

	  // activate job definition by id (including jobs) ///////////////////////////////

	  public virtual void testActivateIncludingJobsByProcessDefinitionKeyWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		suspendJobDefinitionIncludingJobsByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE);

		try
		{
		  // when
		  managementService.activateJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY, true);
		  fail("Exception expected: It should not be possible to suspend a job definition");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testActivateIncludingJobsByProcessDefinitionKeyWithUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobDefinitionIncludingJobsByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE);
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		try
		{
		  // when
		  managementService.activateJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY, true);
		  fail("Exception expected: It should not be possible to suspend a job definition");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testActivateIncludingJobsByProcessDefinitionKeyWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobDefinitionIncludingJobsByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		// when
		managementService.activateJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY, true);

		// then
		JobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		assertNotNull(jobDefinition);
		assertFalse(jobDefinition.Suspended);

		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertFalse(job.Suspended);
	  }

	  public virtual void testActivateIncludingJobsByProcessDefinitionKeyWithUpdateInstancePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobDefinitionIncludingJobsByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE, UPDATE_INSTANCE);

		// when
		managementService.activateJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY, true);

		// then
		JobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		assertNotNull(jobDefinition);
		assertFalse(jobDefinition.Suspended);

		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertFalse(job.Suspended);
	  }

	  public virtual void testActivateIncludingJobsByProcessDefinitionKeyWithUpdateInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobDefinitionIncludingJobsByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE);
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE);

		// when
		managementService.activateJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY, true);

		// then
		JobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		assertNotNull(jobDefinition);
		assertFalse(jobDefinition.Suspended);

		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertFalse(job.Suspended);
	  }

	  // helper /////////////////////////////////////////////////////

	  protected internal virtual void verifyQueryResults(JobDefinitionQuery query, int countExpected)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: verifyQueryResults((org.camunda.bpm.engine.impl.AbstractQuery<?, ?>) query, countExpected);
		verifyQueryResults((AbstractQuery<object, ?>) query, countExpected);
	  }

	  protected internal virtual JobDefinition selectJobDefinitionByProcessDefinitionKey(string processDefinitionKey)
	  {
		disableAuthorization();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().processDefinitionKey(processDefinitionKey).singleResult();
		enableAuthorization();
		return jobDefinition;
	  }

	  protected internal virtual Job selectJobByProcessInstanceId(string processInstanceId)
	  {
		disableAuthorization();
		Job job = managementService.createJobQuery().processInstanceId(processInstanceId).singleResult();
		enableAuthorization();
		return job;
	  }

	}

}