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
//	import static org.camunda.bpm.engine.authorization.Resources.DEPLOYMENT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_INSTANCE;

	using AbstractQuery = org.camunda.bpm.engine.impl.AbstractQuery;
	using DeploymentStatistics = org.camunda.bpm.engine.management.DeploymentStatistics;
	using DeploymentStatisticsQuery = org.camunda.bpm.engine.management.DeploymentStatisticsQuery;
	using IncidentStatistics = org.camunda.bpm.engine.management.IncidentStatistics;


	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class DeploymentStatisticsAuthorizationTest : AuthorizationTest
	{

	  protected internal const string ONE_INCIDENT_PROCESS_KEY = "process";
	  protected internal const string TIMER_START_PROCESS_KEY = "timerStartProcess";
	  protected internal const string TIMER_BOUNDARY_PROCESS_KEY = "timerBoundaryProcess";

	  protected internal string firstDeploymentId;
	  protected internal string secondDeploymentId;
	  protected internal string thirdDeploymentId;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setUp() throws Exception
	  public override void setUp()
	  {
		firstDeploymentId = createDeployment("first", "org/camunda/bpm/engine/test/api/authorization/oneIncidentProcess.bpmn20.xml").Id;
		secondDeploymentId = createDeployment("second", "org/camunda/bpm/engine/test/api/authorization/timerStartEventProcess.bpmn20.xml").Id;
		thirdDeploymentId = createDeployment("third", "org/camunda/bpm/engine/test/api/authorization/timerBoundaryEventProcess.bpmn20.xml").Id;
		base.setUp();
	  }

	  public override void tearDown()
	  {
		base.tearDown();
		deleteDeployment(firstDeploymentId);
		deleteDeployment(secondDeploymentId);
		deleteDeployment(thirdDeploymentId);
	  }

	  // deployment statistics query without process instance authorizations /////////////////////////////////////////////

	  public virtual void testQueryWithoutAuthorization()
	  {
		// given

		// when
		DeploymentStatisticsQuery query = managementService.createDeploymentStatisticsQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testQueryWithReadPermissionOnDeployment()
	  {
		// given
		createGrantAuthorization(DEPLOYMENT, firstDeploymentId, userId, READ);

		// when
		DeploymentStatisticsQuery query = managementService.createDeploymentStatisticsQuery();

		// then
		verifyQueryResults(query, 1);

		DeploymentStatistics statistics = query.singleResult();
		verifyStatisticsResult(statistics, 0, 0, 0);
	  }

	  public virtual void testQueryWithMultiple()
	  {
		// given
		createGrantAuthorization(DEPLOYMENT, firstDeploymentId, userId, READ);
		createGrantAuthorization(DEPLOYMENT, ANY, userId, READ);

		// when
		DeploymentStatisticsQuery query = managementService.createDeploymentStatisticsQuery();

		// then
		verifyQueryResults(query, 3);
	  }

	  public virtual void testQueryWithReadPermissionOnAnyDeployment()
	  {
		// given
		createGrantAuthorization(DEPLOYMENT, ANY, userId, READ);

		// when
		DeploymentStatisticsQuery query = managementService.createDeploymentStatisticsQuery();

		// then
		verifyQueryResults(query, 3);

		IList<DeploymentStatistics> result = query.list();
		foreach (DeploymentStatistics statistics in result)
		{
		  verifyStatisticsResult(statistics, 0, 0, 0);
		}
	  }

	  // deployment statistics query (including process instances) /////////////////////////////////////////////

	  public virtual void testQueryWithReadPermissionOnProcessInstance()
	  {
		// given
		createGrantAuthorization(DEPLOYMENT, ANY, userId, READ);

		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);

		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		DeploymentStatisticsQuery query = managementService.createDeploymentStatisticsQuery();

		// then
		IList<DeploymentStatistics> statistics = query.list();

		foreach (DeploymentStatistics deploymentStatistics in statistics)
		{
		  string id = deploymentStatistics.Id;
		  if (id.Equals(firstDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 0, 0, 0);
		  }
		  else if (id.Equals(secondDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 0, 0, 0);
		  }
		  else if (id.Equals(thirdDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 1, 0, 0);
		  }
		  else
		  {
			fail("Unexpected deployment");
		  }
		}
	  }

	  public virtual void testQueryWithReadPermissionOnAnyProcessInstance()
	  {
		// given
		createGrantAuthorization(DEPLOYMENT, ANY, userId, READ);

		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);

		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);

		// when
		DeploymentStatisticsQuery query = managementService.createDeploymentStatisticsQuery();

		// then
		IList<DeploymentStatistics> statistics = query.list();

		foreach (DeploymentStatistics deploymentStatistics in statistics)
		{
		  string id = deploymentStatistics.Id;
		  if (id.Equals(firstDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
		  }
		  else if (id.Equals(secondDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
		  }
		  else if (id.Equals(thirdDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
		  }
		  else
		  {
			fail("Unexpected deployment");
		  }
		}
	  }

	  public virtual void testQueryWithReadInstancePermissionOnProcessDefinition()
	  {
		// given
		createGrantAuthorization(DEPLOYMENT, ANY, userId, READ);

		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);

		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, READ_INSTANCE);

		// when
		DeploymentStatisticsQuery query = managementService.createDeploymentStatisticsQuery();

		// then
		IList<DeploymentStatistics> statistics = query.list();

		foreach (DeploymentStatistics deploymentStatistics in statistics)
		{
		  string id = deploymentStatistics.Id;
		  if (id.Equals(firstDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 0, 0, 0);
		  }
		  else if (id.Equals(secondDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 0, 0, 0);
		  }
		  else if (id.Equals(thirdDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
		  }
		  else
		  {
			fail("Unexpected deployment");
		  }
		}
	  }

	  public virtual void testQueryWithReadInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		createGrantAuthorization(DEPLOYMENT, ANY, userId, READ);

		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);

		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE);

		// when
		DeploymentStatisticsQuery query = managementService.createDeploymentStatisticsQuery();

		// then
		IList<DeploymentStatistics> statistics = query.list();

		foreach (DeploymentStatistics deploymentStatistics in statistics)
		{
		  string id = deploymentStatistics.Id;
		  if (id.Equals(firstDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
		  }
		  else if (id.Equals(secondDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
		  }
		  else if (id.Equals(thirdDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
		  }
		  else
		  {
			fail("Unexpected deployment");
		  }
		}
	  }

	  // deployment statistics query (including failed jobs) /////////////////////////////////////////////

	  public virtual void testQueryIncludingFailedJobsWithReadPermissionOnProcessInstance()
	  {
		// given
		createGrantAuthorization(DEPLOYMENT, ANY, userId, READ);

		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		string processInstanceId = startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);

		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

		// when
		DeploymentStatisticsQuery query = managementService.createDeploymentStatisticsQuery().includeFailedJobs();

		// then
		IList<DeploymentStatistics> statistics = query.list();

		foreach (DeploymentStatistics deploymentStatistics in statistics)
		{
		  string id = deploymentStatistics.Id;
		  if (id.Equals(firstDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 1, 1, 0);
		  }
		  else if (id.Equals(secondDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 0, 0, 0);
		  }
		  else if (id.Equals(thirdDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 0, 0, 0);
		  }
		  else
		  {
			fail("Unexpected deployment");
		  }
		}
	  }

	  public virtual void testQueryIncludingFailedJobsWithReadPermissionOnAnyProcessInstance()
	  {
		// given
		createGrantAuthorization(DEPLOYMENT, ANY, userId, READ);

		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);

		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);

		// when
		DeploymentStatisticsQuery query = managementService.createDeploymentStatisticsQuery().includeFailedJobs();

		// then
		IList<DeploymentStatistics> statistics = query.list();

		foreach (DeploymentStatistics deploymentStatistics in statistics)
		{
		  string id = deploymentStatistics.Id;
		  if (id.Equals(firstDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 3, 3, 0);
		  }
		  else if (id.Equals(secondDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
		  }
		  else if (id.Equals(thirdDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
		  }
		  else
		  {
			fail("Unexpected deployment");
		  }
		}
	  }

	  public virtual void testQueryIncludingFailedJobsWithReadInstancePermissionOnProcessDefinition()
	  {
		// given
		createGrantAuthorization(DEPLOYMENT, ANY, userId, READ);

		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);

		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, userId, READ_INSTANCE);

		// when
		DeploymentStatisticsQuery query = managementService.createDeploymentStatisticsQuery().includeFailedJobs();

		// then
		IList<DeploymentStatistics> statistics = query.list();

		foreach (DeploymentStatistics deploymentStatistics in statistics)
		{
		  string id = deploymentStatistics.Id;
		  if (id.Equals(firstDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 3, 3, 0);
		  }
		  else if (id.Equals(secondDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 0, 0, 0);
		  }
		  else if (id.Equals(thirdDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 0, 0, 0);
		  }
		  else
		  {
			fail("Unexpected deployment");
		  }
		}
	  }

	  public virtual void testQueryIncludingFailedJobsWithReadInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		createGrantAuthorization(DEPLOYMENT, ANY, userId, READ);

		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);

		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE);

		// when
		DeploymentStatisticsQuery query = managementService.createDeploymentStatisticsQuery().includeFailedJobs();

		// then
		IList<DeploymentStatistics> statistics = query.list();

		foreach (DeploymentStatistics deploymentStatistics in statistics)
		{
		  string id = deploymentStatistics.Id;
		  if (id.Equals(firstDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 3, 3, 0);
		  }
		  else if (id.Equals(secondDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
		  }
		  else if (id.Equals(thirdDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
		  }
		  else
		  {
			fail("Unexpected deployment");
		  }
		}
	  }

	  // deployment statistics query (including incidents) /////////////////////////////////////////////

	  public virtual void testQueryIncludingIncidentsWithReadPermissionOnProcessInstance()
	  {
		// given
		createGrantAuthorization(DEPLOYMENT, ANY, userId, READ);

		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		string processInstanceId = startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);

		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

		// when
		DeploymentStatisticsQuery query = managementService.createDeploymentStatisticsQuery().includeIncidents();

		// then
		IList<DeploymentStatistics> statistics = query.list();

		foreach (DeploymentStatistics deploymentStatistics in statistics)
		{
		  string id = deploymentStatistics.Id;
		  if (id.Equals(firstDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 1, 0, 1);
		  }
		  else if (id.Equals(secondDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 0, 0, 0);
		  }
		  else if (id.Equals(thirdDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 0, 0, 0);
		  }
		  else
		  {
			fail("Unexpected deployment");
		  }
		}
	  }

	  public virtual void testQueryIncludingIncidentsWithReadPermissionOnAnyProcessInstance()
	  {
		// given
		createGrantAuthorization(DEPLOYMENT, ANY, userId, READ);

		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);

		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);

		// when
		DeploymentStatisticsQuery query = managementService.createDeploymentStatisticsQuery().includeIncidents();

		// then
		IList<DeploymentStatistics> statistics = query.list();

		foreach (DeploymentStatistics deploymentStatistics in statistics)
		{
		  string id = deploymentStatistics.Id;
		  if (id.Equals(firstDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 3, 0, 3);
		  }
		  else if (id.Equals(secondDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
		  }
		  else if (id.Equals(thirdDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
		  }
		  else
		  {
			fail("Unexpected deployment");
		  }
		}
	  }

	  public virtual void testQueryIncludingIncidentsWithReadInstancePermissionOnProcessDefinition()
	  {
		// given
		createGrantAuthorization(DEPLOYMENT, ANY, userId, READ);

		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);

		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, userId, READ_INSTANCE);

		// when
		DeploymentStatisticsQuery query = managementService.createDeploymentStatisticsQuery().includeIncidents();

		// then
		IList<DeploymentStatistics> statistics = query.list();

		foreach (DeploymentStatistics deploymentStatistics in statistics)
		{
		  string id = deploymentStatistics.Id;
		  if (id.Equals(firstDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 3, 0, 3);
		  }
		  else if (id.Equals(secondDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 0, 0, 0);
		  }
		  else if (id.Equals(thirdDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 0, 0, 0);
		  }
		  else
		  {
			fail("Unexpected deployment");
		  }
		}
	  }

	  public virtual void testQueryIncludingIncidentsWithReadInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		createGrantAuthorization(DEPLOYMENT, ANY, userId, READ);

		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);

		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE);

		// when
		DeploymentStatisticsQuery query = managementService.createDeploymentStatisticsQuery().includeIncidents();

		// then
		IList<DeploymentStatistics> statistics = query.list();

		foreach (DeploymentStatistics deploymentStatistics in statistics)
		{
		  string id = deploymentStatistics.Id;
		  if (id.Equals(firstDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 3, 0, 3);
		  }
		  else if (id.Equals(secondDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
		  }
		  else if (id.Equals(thirdDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
		  }
		  else
		  {
			fail("Unexpected deployment");
		  }
		}
	  }

	  // deployment statistics query (including failed jobs and incidents) /////////////////////////////////////////////

	  public virtual void testQueryIncludingFailedJobsAndIncidentsWithReadPermissionOnProcessInstance()
	  {
		// given
		createGrantAuthorization(DEPLOYMENT, ANY, userId, READ);

		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		string processInstanceId = startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);

		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

		// when
		DeploymentStatisticsQuery query = managementService.createDeploymentStatisticsQuery().includeFailedJobs().includeIncidents();

		// then
		IList<DeploymentStatistics> statistics = query.list();

		foreach (DeploymentStatistics deploymentStatistics in statistics)
		{
		  string id = deploymentStatistics.Id;
		  if (id.Equals(firstDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 1, 1, 1);
		  }
		  else if (id.Equals(secondDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 0, 0, 0);
		  }
		  else if (id.Equals(thirdDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 0, 0, 0);
		  }
		  else
		  {
			fail("Unexpected deployment");
		  }
		}
	  }

	  public virtual void testQueryIncludingFailedJobsAndIncidentsWithReadPermissionOnAnyProcessInstance()
	  {
		// given
		createGrantAuthorization(DEPLOYMENT, ANY, userId, READ);

		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);

		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);

		// when
		DeploymentStatisticsQuery query = managementService.createDeploymentStatisticsQuery().includeFailedJobs().includeIncidents();

		// then
		IList<DeploymentStatistics> statistics = query.list();

		foreach (DeploymentStatistics deploymentStatistics in statistics)
		{
		  string id = deploymentStatistics.Id;
		  if (id.Equals(firstDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 3, 3, 3);
		  }
		  else if (id.Equals(secondDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
		  }
		  else if (id.Equals(thirdDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
		  }
		  else
		  {
			fail("Unexpected deployment");
		  }
		}
	  }

	  public virtual void testQueryIncludingFailedJobsAndIncidentsWithReadInstancePermissionOnProcessDefinition()
	  {
		// given
		createGrantAuthorization(DEPLOYMENT, ANY, userId, READ);

		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);

		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, userId, READ_INSTANCE);

		// when
		DeploymentStatisticsQuery query = managementService.createDeploymentStatisticsQuery().includeFailedJobs().includeIncidents();

		// then
		IList<DeploymentStatistics> statistics = query.list();

		foreach (DeploymentStatistics deploymentStatistics in statistics)
		{
		  string id = deploymentStatistics.Id;
		  if (id.Equals(firstDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 3, 3, 3);
		  }
		  else if (id.Equals(secondDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 0, 0, 0);
		  }
		  else if (id.Equals(thirdDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 0, 0, 0);
		  }
		  else
		  {
			fail("Unexpected deployment");
		  }
		}
	  }

	  public virtual void testQueryIncludingFailedJobsAndIncidentsWithReadInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		createGrantAuthorization(DEPLOYMENT, ANY, userId, READ);

		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_START_PROCESS_KEY);

		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE);

		// when
		DeploymentStatisticsQuery query = managementService.createDeploymentStatisticsQuery().includeFailedJobs().includeIncidents();

		// then
		IList<DeploymentStatistics> statistics = query.list();

		foreach (DeploymentStatistics deploymentStatistics in statistics)
		{
		  string id = deploymentStatistics.Id;
		  if (id.Equals(firstDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 3, 3, 3);
		  }
		  else if (id.Equals(secondDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
		  }
		  else if (id.Equals(thirdDeploymentId))
		  {
			verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
		  }
		  else
		  {
			fail("Unexpected deployment");
		  }
		}
	  }

	  // helper ///////////////////////////////////////////////////////////////////////////

	  protected internal virtual void verifyQueryResults(DeploymentStatisticsQuery query, int countExpected)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: verifyQueryResults((org.camunda.bpm.engine.impl.AbstractQuery<?, ?>) query, countExpected);
		verifyQueryResults((AbstractQuery<object, ?>) query, countExpected);
	  }

	  protected internal virtual void verifyStatisticsResult(DeploymentStatistics statistics, int instances, int failedJobs, int incidents)
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

	}

}