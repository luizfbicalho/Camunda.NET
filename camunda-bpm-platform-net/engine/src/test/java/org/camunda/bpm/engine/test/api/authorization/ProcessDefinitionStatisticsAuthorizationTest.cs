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
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;

	using AbstractQuery = org.camunda.bpm.engine.impl.AbstractQuery;
	using IncidentStatistics = org.camunda.bpm.engine.management.IncidentStatistics;
	using ProcessDefinitionStatistics = org.camunda.bpm.engine.management.ProcessDefinitionStatistics;
	using ProcessDefinitionStatisticsQuery = org.camunda.bpm.engine.management.ProcessDefinitionStatisticsQuery;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class ProcessDefinitionStatisticsAuthorizationTest : AuthorizationTest
	{

	  protected internal const string ONE_TASK_PROCESS_KEY = "oneTaskProcess";
	  protected internal const string ONE_INCIDENT_PROCESS_KEY = "process";

	  protected internal new string deploymentId;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setUp() throws Exception
	  public override void setUp()
	  {
		deploymentId = createDeployment(null, "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/authorization/oneIncidentProcess.bpmn20.xml").Id;
		base.setUp();
	  }

	  public override void tearDown()
	  {
		base.tearDown();
		deleteDeployment(deploymentId);
	  }

	  // without running instances //////////////////////////////////////////////////////////

	  public virtual void testQueryWithoutAuthorizations()
	  {
		// given

		// when
		ProcessDefinitionStatisticsQuery query = managementService.createProcessDefinitionStatisticsQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testQueryWithReadPermissionOnOneTaskProcess()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, READ);

		// when
		ProcessDefinitionStatisticsQuery query = managementService.createProcessDefinitionStatisticsQuery();

		// then
		verifyQueryResults(query, 1);

		ProcessDefinitionStatistics statistics = query.singleResult();
		assertEquals(ONE_TASK_PROCESS_KEY, statistics.Key);
		assertEquals(0, statistics.Instances);
		assertEquals(0, statistics.FailedJobs);
		assertTrue(statistics.IncidentStatistics.Count == 0);
	  }

	  public virtual void testQueryWithMultiple()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, READ);
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ);

		// when
		ProcessDefinitionStatisticsQuery query = managementService.createProcessDefinitionStatisticsQuery();

		// then
		verifyQueryResults(query, 2);
	  }

	  public virtual void testQueryWithReadPermissionOnAnyProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ);

		// when
		ProcessDefinitionStatisticsQuery query = managementService.createProcessDefinitionStatisticsQuery();

		// then
		verifyQueryResults(query, 2);

		IList<ProcessDefinitionStatistics> statistics = query.list();
		foreach (ProcessDefinitionStatistics result in statistics)
		{
		  verifyStatisticsResult(result, 0, 0, 0);
		}
	  }

	  // including instances //////////////////////////////////////////////////////////////

	  public virtual void testQueryIncludingInstancesWithoutProcessInstanceAuthorizations()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ);

		// when
		IList<ProcessDefinitionStatistics> statistics = managementService.createProcessDefinitionStatisticsQuery().list();

		// then
		assertEquals(2, statistics.Count);

		ProcessDefinitionStatistics oneTaskProcessStatistics = getStatisticsByKey(statistics, ONE_TASK_PROCESS_KEY);
		verifyStatisticsResult(oneTaskProcessStatistics, 2, 0, 0);

		ProcessDefinitionStatistics oneIncidentProcessStatistics = getStatisticsByKey(statistics, ONE_INCIDENT_PROCESS_KEY);
		verifyStatisticsResult(oneIncidentProcessStatistics, 3, 0, 0);
	  }

	  // including failed jobs ////////////////////////////////////////////////////////////

	  public virtual void testQueryIncludingFailedJobsWithoutProcessInstanceAuthorizations()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ);

		// when
		IList<ProcessDefinitionStatistics> statistics = managementService.createProcessDefinitionStatisticsQuery().includeFailedJobs().list();

		// then
		assertEquals(2, statistics.Count);

		ProcessDefinitionStatistics oneTaskProcessStatistics = getStatisticsByKey(statistics, ONE_TASK_PROCESS_KEY);
		verifyStatisticsResult(oneTaskProcessStatistics, 2, 0, 0);

		ProcessDefinitionStatistics oneIncidentProcessStatistics = getStatisticsByKey(statistics, ONE_INCIDENT_PROCESS_KEY);
		verifyStatisticsResult(oneIncidentProcessStatistics, 3, 3, 0);
	  }

	  // including incidents //////////////////////////////////////////////////////////////////////////

	  public virtual void testQueryIncludingIncidentsWithoutProcessInstanceAuthorizations()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ);

		// when
		IList<ProcessDefinitionStatistics> statistics = managementService.createProcessDefinitionStatisticsQuery().includeIncidents().list();

		// then
		assertEquals(2, statistics.Count);

		ProcessDefinitionStatistics oneTaskProcessStatistics = getStatisticsByKey(statistics, ONE_TASK_PROCESS_KEY);
		verifyStatisticsResult(oneTaskProcessStatistics, 2, 0, 0);

		ProcessDefinitionStatistics oneIncidentProcessStatistics = getStatisticsByKey(statistics, ONE_INCIDENT_PROCESS_KEY);
		verifyStatisticsResult(oneIncidentProcessStatistics, 3, 0, 3);
	  }

	  // including incidents and failed jobs ///////////////////////////////////////////////////////////////

	  public virtual void testQueryIncludingIncidentsAndFailedJobsWithoutProcessInstanceAuthorizations()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ);

		// when
		IList<ProcessDefinitionStatistics> statistics = managementService.createProcessDefinitionStatisticsQuery().includeIncidents().includeFailedJobs().list();

		// then
		assertEquals(2, statistics.Count);

		ProcessDefinitionStatistics oneTaskProcessStatistics = getStatisticsByKey(statistics, ONE_TASK_PROCESS_KEY);
		verifyStatisticsResult(oneTaskProcessStatistics, 2, 0, 0);

		ProcessDefinitionStatistics oneIncidentProcessStatistics = getStatisticsByKey(statistics, ONE_INCIDENT_PROCESS_KEY);
		verifyStatisticsResult(oneIncidentProcessStatistics, 3, 3, 3);
	  }

	  // helper ///////////////////////////////////////////////////////////////////////////

	  protected internal virtual void verifyQueryResults(ProcessDefinitionStatisticsQuery query, int countExpected)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: verifyQueryResults((org.camunda.bpm.engine.impl.AbstractQuery<?, ?>) query, countExpected);
		verifyQueryResults((AbstractQuery<object, ?>) query, countExpected);
	  }

	  protected internal virtual void verifyStatisticsResult(ProcessDefinitionStatistics statistics, int instances, int failedJobs, int incidents)
	  {
		assertEquals("Instances", instances, statistics.Instances);
		assertEquals("Failed Jobs", failedJobs, statistics.FailedJobs);

		IList<IncidentStatistics> incidentStatistics = statistics.IncidentStatistics;
		if (incidents == 0)
		{
		  assertTrue("Incidents supposed to be empty", incidentStatistics.Count == 0);
		}
		else
		{
		  // the test does have only one type of incidents
		  assertEquals("Incidents", incidents, incidentStatistics[0].IncidentCount);
		}
	  }

	  protected internal virtual ProcessDefinitionStatistics getStatisticsByKey(IList<ProcessDefinitionStatistics> statistics, string key)
	  {
		foreach (ProcessDefinitionStatistics result in statistics)
		{
		  if (key.Equals(result.Key))
		  {
			return result;
		  }
		}
		fail("No statistics found for key '" + key + "'.");
		return null;
	  }
	}

}