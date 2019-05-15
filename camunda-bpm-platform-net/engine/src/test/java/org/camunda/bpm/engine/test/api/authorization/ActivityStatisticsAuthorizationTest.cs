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
//	import static org.camunda.bpm.engine.authorization.Permissions.READ;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_INSTANCE;

	using AbstractQuery = org.camunda.bpm.engine.impl.AbstractQuery;
	using ActivityStatistics = org.camunda.bpm.engine.management.ActivityStatistics;
	using ActivityStatisticsQuery = org.camunda.bpm.engine.management.ActivityStatisticsQuery;
	using IncidentStatistics = org.camunda.bpm.engine.management.IncidentStatistics;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class ActivityStatisticsAuthorizationTest : AuthorizationTest
	{

	  protected internal const string ONE_INCIDENT_PROCESS_KEY = "process";

	  protected internal new string deploymentId;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setUp() throws Exception
	  public override void setUp()
	  {
		deploymentId = createDeployment(null, "org/camunda/bpm/engine/test/api/authorization/oneIncidentProcess.bpmn20.xml").Id;
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		base.setUp();
	  }

	  public override void tearDown()
	  {
		base.tearDown();
		deleteDeployment(deploymentId);
	  }

	  // without any authorization

	  public virtual void testQueryWithoutAuthorizations()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

		try
		{
		  // when
		  managementService.createActivityStatisticsQuery(processDefinitionId).list();
		  fail("Exception expected: It should not be possible to execute the activity statistics query");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(ONE_INCIDENT_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  // including instances //////////////////////////////////////////////////////////////

	  public virtual void testQueryIncludingInstancesWithoutAuthorizationOnProcessInstance()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, userId, READ);

		// when
		ActivityStatisticsQuery query = managementService.createActivityStatisticsQuery(processDefinitionId);

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testQueryIncludingInstancesWithReadPermissionOnOneProcessInstance()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

		disableAuthorization();
		string processInstanceId = runtimeService.createProcessInstanceQuery().list().get(0).Id;
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, userId, READ);
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		ActivityStatistics statistics = managementService.createActivityStatisticsQuery(processDefinitionId).singleResult();

		// then
		assertNotNull(statistics);
		assertEquals("scriptTask", statistics.Id);
		assertEquals(1, statistics.Instances);
		assertEquals(0, statistics.FailedJobs);
		assertTrue(statistics.IncidentStatistics.Count == 0);
	  }

	  public virtual void testQueryIncludingInstancesWithMany()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

		disableAuthorization();
		string processInstanceId = runtimeService.createProcessInstanceQuery().list().get(0).Id;
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, userId, READ);
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ);
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		ActivityStatistics statistics = managementService.createActivityStatisticsQuery(processDefinitionId).singleResult();

		// then
		assertNotNull(statistics);
		assertEquals("scriptTask", statistics.Id);
		assertEquals(1, statistics.Instances);
		assertEquals(0, statistics.FailedJobs);
		assertTrue(statistics.IncidentStatistics.Count == 0);
	  }

	  public virtual void testQueryIncludingInstancesWithReadPermissionOnAnyProcessInstance()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, userId, READ);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);

		// when
		ActivityStatistics statistics = managementService.createActivityStatisticsQuery(processDefinitionId).singleResult();

		// then
		assertNotNull(statistics);
		assertEquals("scriptTask", statistics.Id);
		assertEquals(3, statistics.Instances);
		assertEquals(0, statistics.FailedJobs);
		assertTrue(statistics.IncidentStatistics.Count == 0);
	  }

	  public virtual void testQueryIncludingInstancesWithReadInstancePermissionOnProcessDefinition()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, userId, READ, READ_INSTANCE);

		// when
		ActivityStatistics statistics = managementService.createActivityStatisticsQuery(processDefinitionId).singleResult();

		// then
		assertNotNull(statistics);
		assertEquals("scriptTask", statistics.Id);
		assertEquals(3, statistics.Instances);
		assertEquals(0, statistics.FailedJobs);
		assertTrue(statistics.IncidentStatistics.Count == 0);
	  }

	  // including failed jobs //////////////////////////////////////////////////////////////

	  public virtual void testQueryIncludingFailedJobsWithoutAuthorizationOnProcessInstance()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, userId, READ);

		// when
		ActivityStatistics statistics = managementService.createActivityStatisticsQuery(processDefinitionId).includeFailedJobs().singleResult();

		// then
		assertNull(statistics);
	  }

	  public virtual void testQueryIncludingFailedJobsWithReadPermissionOnOneProcessInstance()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

		disableAuthorization();
		string processInstanceId = runtimeService.createProcessInstanceQuery().list().get(0).Id;
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, userId, READ);
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		ActivityStatistics statistics = managementService.createActivityStatisticsQuery(processDefinitionId).includeFailedJobs().singleResult();

		// then
		assertNotNull(statistics);
		assertEquals("scriptTask", statistics.Id);
		assertEquals(1, statistics.Instances);
		assertEquals(1, statistics.FailedJobs);
		assertTrue(statistics.IncidentStatistics.Count == 0);
	  }

	  public virtual void testQueryIncludingFailedJobsWithReadPermissionOnAnyProcessInstance()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, userId, READ);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);

		// when
		ActivityStatistics statistics = managementService.createActivityStatisticsQuery(processDefinitionId).includeFailedJobs().singleResult();

		// then
		assertNotNull(statistics);
		assertEquals("scriptTask", statistics.Id);
		assertEquals(3, statistics.Instances);
		assertEquals(3, statistics.FailedJobs);
		assertTrue(statistics.IncidentStatistics.Count == 0);
	  }

	  public virtual void testQueryIncludingFailedJobsWithReadInstancePermissionOnProcessDefinition()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, userId, READ, READ_INSTANCE);

		// when
		ActivityStatistics statistics = managementService.createActivityStatisticsQuery(processDefinitionId).includeFailedJobs().singleResult();

		// then
		assertNotNull(statistics);
		assertEquals("scriptTask", statistics.Id);
		assertEquals(3, statistics.Instances);
		assertEquals(3, statistics.FailedJobs);
		assertTrue(statistics.IncidentStatistics.Count == 0);
	  }

	  // including incidents //////////////////////////////////////////////////////////////

	  public virtual void testQueryIncludingIncidentsWithoutAuthorizationOnProcessInstance()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, userId, READ);

		// when
		ActivityStatistics statistics = managementService.createActivityStatisticsQuery(processDefinitionId).includeIncidents().singleResult();

		// then
		assertNull(statistics);
	  }

	  public virtual void testQueryIncludingIncidentsWithReadPermissionOnOneProcessInstance()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

		disableAuthorization();
		string processInstanceId = runtimeService.createProcessInstanceQuery().list().get(0).Id;
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, userId, READ);
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		ActivityStatistics statistics = managementService.createActivityStatisticsQuery(processDefinitionId).includeIncidents().singleResult();

		// then
		assertNotNull(statistics);
		assertEquals("scriptTask", statistics.Id);
		assertEquals(1, statistics.Instances);
		assertEquals(0, statistics.FailedJobs);
		assertFalse(statistics.IncidentStatistics.Count == 0);
		IncidentStatistics incidentStatistics = statistics.IncidentStatistics[0];
		assertEquals(1, incidentStatistics.IncidentCount);
	  }

	  public virtual void testQueryIncludingIncidentsWithReadPermissionOnAnyProcessInstance()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, userId, READ);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);

		// when
		ActivityStatistics statistics = managementService.createActivityStatisticsQuery(processDefinitionId).includeIncidents().singleResult();

		// then
		assertNotNull(statistics);
		assertEquals("scriptTask", statistics.Id);
		assertEquals(3, statistics.Instances);
		assertEquals(0, statistics.FailedJobs);
		assertFalse(statistics.IncidentStatistics.Count == 0);
		IncidentStatistics incidentStatistics = statistics.IncidentStatistics[0];
		assertEquals(3, incidentStatistics.IncidentCount);
	  }

	  public virtual void testQueryIncludingIncidentsWithReadInstancePermissionOnProcessDefinition()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, userId, READ, READ_INSTANCE);

		// when
		ActivityStatistics statistics = managementService.createActivityStatisticsQuery(processDefinitionId).includeIncidents().singleResult();

		// then
		assertNotNull(statistics);
		assertEquals("scriptTask", statistics.Id);
		assertEquals(3, statistics.Instances);
		assertEquals(0, statistics.FailedJobs);
		assertFalse(statistics.IncidentStatistics.Count == 0);
		IncidentStatistics incidentStatistics = statistics.IncidentStatistics[0];
		assertEquals(3, incidentStatistics.IncidentCount);
	  }

	  // including incidents and failed jobs //////////////////////////////////////////////////////////

	  public virtual void testQueryIncludingIncidentsAndFailedJobsWithoutAuthorizationOnProcessInstance()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, userId, READ);

		// when
		ActivityStatistics statistics = managementService.createActivityStatisticsQuery(processDefinitionId).includeIncidents().includeFailedJobs().singleResult();

		// then
		assertNull(statistics);
	  }

	  public virtual void testQueryIncludingIncidentsAndFailedJobsWithReadPermissionOnOneProcessInstance()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

		disableAuthorization();
		string processInstanceId = runtimeService.createProcessInstanceQuery().list().get(0).Id;
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, userId, READ);
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		ActivityStatistics statistics = managementService.createActivityStatisticsQuery(processDefinitionId).includeIncidents().includeFailedJobs().singleResult();

		// then
		assertNotNull(statistics);
		assertEquals("scriptTask", statistics.Id);
		assertEquals(1, statistics.Instances);
		assertEquals(1, statistics.FailedJobs);
		assertFalse(statistics.IncidentStatistics.Count == 0);
		IncidentStatistics incidentStatistics = statistics.IncidentStatistics[0];
		assertEquals(1, incidentStatistics.IncidentCount);
	  }

	  public virtual void testQueryIncludingIncidentsAndFailedJobsWithReadPermissionOnAnyProcessInstance()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, userId, READ);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);

		// when
		ActivityStatistics statistics = managementService.createActivityStatisticsQuery(processDefinitionId).includeIncidents().includeFailedJobs().singleResult();

		// then
		assertNotNull(statistics);
		assertEquals("scriptTask", statistics.Id);
		assertEquals(3, statistics.Instances);
		assertEquals(3, statistics.FailedJobs);
		assertFalse(statistics.IncidentStatistics.Count == 0);
		IncidentStatistics incidentStatistics = statistics.IncidentStatistics[0];
		assertEquals(3, incidentStatistics.IncidentCount);
	  }

	  public virtual void testQueryIncludingIncidentsAndFailedJobsWithReadInstancePermissionOnProcessDefinition()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, userId, READ, READ_INSTANCE);

		// when
		ActivityStatistics statistics = managementService.createActivityStatisticsQuery(processDefinitionId).includeIncidents().includeFailedJobs().singleResult();

		// then
		assertNotNull(statistics);
		assertEquals("scriptTask", statistics.Id);
		assertEquals(3, statistics.Instances);
		assertEquals(3, statistics.FailedJobs);
		assertFalse(statistics.IncidentStatistics.Count == 0);
		IncidentStatistics incidentStatistics = statistics.IncidentStatistics[0];
		assertEquals(3, incidentStatistics.IncidentCount);
	  }

	  public virtual void testManyAuthorizationsActivityStatisticsQueryIncludingFailedJobsAndIncidents()
	  {
		string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, userId, READ, READ_INSTANCE);
		createGrantAuthorizationGroup(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, groupId, READ, READ_INSTANCE);

		IList<ActivityStatistics> statistics = managementService.createActivityStatisticsQuery(processDefinitionId).includeFailedJobs().includeIncidents().list();

		assertEquals(1, statistics.Count);

		ActivityStatistics activityResult = statistics[0];
		assertEquals(3, activityResult.Instances);
		assertEquals("scriptTask", activityResult.Id);
		assertEquals(3, activityResult.FailedJobs);
		assertFalse(activityResult.IncidentStatistics.Count == 0);
		IncidentStatistics incidentStatistics = activityResult.IncidentStatistics[0];
		assertEquals(3, incidentStatistics.IncidentCount);
	  }

	  public virtual void testManyAuthorizationsActivityStatisticsQuery()
	  {
		string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, userId, READ, READ_INSTANCE);
		createGrantAuthorizationGroup(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, groupId, READ, READ_INSTANCE);

		IList<ActivityStatistics> statistics = managementService.createActivityStatisticsQuery(processDefinitionId).list();

		assertEquals(1, statistics.Count);

		ActivityStatistics activityResult = statistics[0];
		assertEquals(3, activityResult.Instances);
		assertEquals("scriptTask", activityResult.Id);
		assertEquals(0, activityResult.FailedJobs);
		assertTrue(activityResult.IncidentStatistics.Count == 0);
	  }

	  // helper ///////////////////////////////////////////////////////////////////////////

	  protected internal virtual void verifyQueryResults(ActivityStatisticsQuery query, int countExpected)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: verifyQueryResults((org.camunda.bpm.engine.impl.AbstractQuery<?, ?>) query, countExpected);
		verifyQueryResults((AbstractQuery<object, ?>) query, countExpected);
	  }

	}

}