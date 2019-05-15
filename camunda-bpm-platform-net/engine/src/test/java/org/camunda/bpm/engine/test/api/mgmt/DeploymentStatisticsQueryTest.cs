using System;
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
namespace org.camunda.bpm.engine.test.api.mgmt
{

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using DeploymentStatistics = org.camunda.bpm.engine.management.DeploymentStatistics;
	using IncidentStatistics = org.camunda.bpm.engine.management.IncidentStatistics;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;

	public class DeploymentStatisticsQueryTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeploymentStatisticsQuery()
	  public virtual void testDeploymentStatisticsQuery()
	  {
		string deploymentName = "my deployment";

		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testMultiInstanceStatisticsQuery.bpmn20.xml").addClasspathResource("org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testParallelGatewayStatisticsQuery.bpmn20.xml").name(deploymentName).deploy();
		runtimeService.startProcessInstanceByKey("MIExampleProcess");
		runtimeService.startProcessInstanceByKey("ParGatewayExampleProcess");

		IList<DeploymentStatistics> statistics = managementService.createDeploymentStatisticsQuery().includeFailedJobs().list();

		Assert.assertEquals(1, statistics.Count);

		DeploymentStatistics result = statistics[0];
		Assert.assertEquals(2, result.Instances);
		Assert.assertEquals(0, result.FailedJobs);

		Assert.assertEquals(deployment.Id, result.Id);
		Assert.assertEquals(deploymentName, result.Name);

		// only compare time on second level (i.e. drop milliseconds)
		DateTime cal1 = new DateTime();
		cal1 = new DateTime(deployment.DeploymentTime);
		cal1.set(DateTime.MILLISECOND, 0);

		DateTime cal2 = new DateTime();
		cal2 = new DateTime(result.DeploymentTime);
		cal2.set(DateTime.MILLISECOND, 0);

		Assert.assertTrue(cal1.Equals(cal2));

		repositoryService.deleteDeployment(deployment.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeploymentStatisticsQueryCountAndPaging()
	  public virtual void testDeploymentStatisticsQueryCountAndPaging()
	  {
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testMultiInstanceStatisticsQuery.bpmn20.xml").addClasspathResource("org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testParallelGatewayStatisticsQuery.bpmn20.xml").deploy();

		runtimeService.startProcessInstanceByKey("MIExampleProcess");
		runtimeService.startProcessInstanceByKey("ParGatewayExampleProcess");

		org.camunda.bpm.engine.repository.Deployment anotherDeployment = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testMultiInstanceStatisticsQuery.bpmn20.xml").addClasspathResource("org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testParallelGatewayStatisticsQuery.bpmn20.xml").deploy();

		runtimeService.startProcessInstanceByKey("MIExampleProcess");
		runtimeService.startProcessInstanceByKey("ParGatewayExampleProcess");

		long count = managementService.createDeploymentStatisticsQuery().includeFailedJobs().count();

		Assert.assertEquals(2, count);

		IList<DeploymentStatistics> statistics = managementService.createDeploymentStatisticsQuery().includeFailedJobs().listPage(0, 1);
		Assert.assertEquals(1, statistics.Count);

		repositoryService.deleteDeployment(deployment.Id, true);
		repositoryService.deleteDeployment(anotherDeployment.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testMultiInstanceStatisticsQuery.bpmn20.xml", "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml"}) public void testDeploymentStatisticsQueryWithFailedJobs()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testMultiInstanceStatisticsQuery.bpmn20.xml", "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml"})]
	  public virtual void testDeploymentStatisticsQueryWithFailedJobs()
	  {

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["fail"] = true;

		runtimeService.startProcessInstanceByKey("MIExampleProcess");
		runtimeService.startProcessInstanceByKey("ExampleProcess", parameters);

		executeAvailableJobs();

		IList<DeploymentStatistics> statistics = managementService.createDeploymentStatisticsQuery().includeFailedJobs().list();

		DeploymentStatistics result = statistics[0];
		Assert.assertEquals(1, result.FailedJobs);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testMultiInstanceStatisticsQuery.bpmn20.xml", "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml"}) public void testDeploymentStatisticsQueryWithIncidents()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testMultiInstanceStatisticsQuery.bpmn20.xml", "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml"})]
	  public virtual void testDeploymentStatisticsQueryWithIncidents()
	  {

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["fail"] = true;

		runtimeService.startProcessInstanceByKey("MIExampleProcess");
		runtimeService.startProcessInstanceByKey("ExampleProcess", parameters);

		executeAvailableJobs();

		IList<DeploymentStatistics> statistics = managementService.createDeploymentStatisticsQuery().includeIncidents().list();

		assertFalse(statistics.Count == 0);
		assertEquals(1, statistics.Count);

		DeploymentStatistics result = statistics[0];

		IList<IncidentStatistics> incidentStatistics = result.IncidentStatistics;
		assertFalse(incidentStatistics.Count == 0);
		assertEquals(1, incidentStatistics.Count);

		IncidentStatistics incident = incidentStatistics[0];
		assertEquals(org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE, incident.IncidentType);
		assertEquals(1, incident.IncidentCount);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testMultiInstanceStatisticsQuery.bpmn20.xml", "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml"}) public void testDeploymentStatisticsQueryWithIncidentType()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testMultiInstanceStatisticsQuery.bpmn20.xml", "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml"})]
	  public virtual void testDeploymentStatisticsQueryWithIncidentType()
	  {

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["fail"] = true;

		runtimeService.startProcessInstanceByKey("MIExampleProcess");
		runtimeService.startProcessInstanceByKey("ExampleProcess", parameters);

		executeAvailableJobs();

		IList<DeploymentStatistics> statistics = managementService.createDeploymentStatisticsQuery().includeIncidentsForType("failedJob").list();

		assertFalse(statistics.Count == 0);
		assertEquals(1, statistics.Count);

		DeploymentStatistics result = statistics[0];

		IList<IncidentStatistics> incidentStatistics = result.IncidentStatistics;
		assertFalse(incidentStatistics.Count == 0);
		assertEquals(1, incidentStatistics.Count);

		IncidentStatistics incident = incidentStatistics[0];
		assertEquals(org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE, incident.IncidentType);
		assertEquals(1, incident.IncidentCount);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testMultiInstanceStatisticsQuery.bpmn20.xml", "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml"}) public void testDeploymentStatisticsQueryWithInvalidIncidentType()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testMultiInstanceStatisticsQuery.bpmn20.xml", "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml"})]
	  public virtual void testDeploymentStatisticsQueryWithInvalidIncidentType()
	  {

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["fail"] = true;

		runtimeService.startProcessInstanceByKey("MIExampleProcess");
		runtimeService.startProcessInstanceByKey("ExampleProcess", parameters);

		executeAvailableJobs();

		IList<DeploymentStatistics> statistics = managementService.createDeploymentStatisticsQuery().includeIncidentsForType("invalid").list();

		assertFalse(statistics.Count == 0);
		assertEquals(1, statistics.Count);

		DeploymentStatistics result = statistics[0];

		IList<IncidentStatistics> incidentStatistics = result.IncidentStatistics;
		assertTrue(incidentStatistics.Count == 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testMultiInstanceStatisticsQuery.bpmn20.xml", "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml"}) public void testDeploymentStatisticsQueryWithIncidentsAndFailedJobs()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testMultiInstanceStatisticsQuery.bpmn20.xml", "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml"})]
	  public virtual void testDeploymentStatisticsQueryWithIncidentsAndFailedJobs()
	  {

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["fail"] = true;

		runtimeService.startProcessInstanceByKey("MIExampleProcess");
		runtimeService.startProcessInstanceByKey("ExampleProcess", parameters);

		executeAvailableJobs();

		IList<DeploymentStatistics> statistics = managementService.createDeploymentStatisticsQuery().includeIncidents().includeFailedJobs().list();

		assertFalse(statistics.Count == 0);
		assertEquals(1, statistics.Count);

		DeploymentStatistics result = statistics[0];

		Assert.assertEquals(1, result.FailedJobs);

		IList<IncidentStatistics> incidentStatistics = result.IncidentStatistics;
		assertFalse(incidentStatistics.Count == 0);
		assertEquals(1, incidentStatistics.Count);

		IncidentStatistics incident = incidentStatistics[0];
		assertEquals(org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE, incident.IncidentType);
		assertEquals(1, incident.IncidentCount);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testCallActivityWithIncidentsWithoutFailedJobs.bpmn20.xml") public void testDeploymentStatisticsQueryWithTwoIncidentsAndOneFailedJobs()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testCallActivityWithIncidentsWithoutFailedJobs.bpmn20.xml")]
	  public virtual void testDeploymentStatisticsQueryWithTwoIncidentsAndOneFailedJobs()
	  {
		runtimeService.startProcessInstanceByKey("callExampleSubProcess");

		executeAvailableJobs();

		IList<DeploymentStatistics> statistics = managementService.createDeploymentStatisticsQuery().includeIncidents().includeFailedJobs().list();

		assertFalse(statistics.Count == 0);
		assertEquals(1, statistics.Count);

		DeploymentStatistics result = statistics[0];

		// has one failed job
		Assert.assertEquals(1, result.FailedJobs);

		IList<IncidentStatistics> incidentStatistics = result.IncidentStatistics;
		assertFalse(incidentStatistics.Count == 0);
		assertEquals(1, incidentStatistics.Count);

		IncidentStatistics incident = incidentStatistics[0];
		assertEquals(org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE, incident.IncidentType);
		assertEquals(2, incident.IncidentCount); // ...but two incidents
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testMultiInstanceStatisticsQuery.bpmn20.xml", "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testParallelGatewayStatisticsQuery.bpmn20.xml"}) public void testDeploymentStatisticsQueryWithoutRunningInstances()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testMultiInstanceStatisticsQuery.bpmn20.xml", "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testParallelGatewayStatisticsQuery.bpmn20.xml"})]
	  public virtual void testDeploymentStatisticsQueryWithoutRunningInstances()
	  {
		IList<DeploymentStatistics> statistics = managementService.createDeploymentStatisticsQuery().includeFailedJobs().list();

		Assert.assertEquals(1, statistics.Count);

		DeploymentStatistics result = statistics[0];
		Assert.assertEquals(0, result.Instances);
		Assert.assertEquals(0, result.FailedJobs);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testFailedTimerStartEvent.bpmn20.xml")]
	  public virtual void testQueryByIncidentsWithFailedTimerStartEvent()
	  {

		executeAvailableJobs();

		IList<DeploymentStatistics> statistics = managementService.createDeploymentStatisticsQuery().includeIncidents().list();

		assertEquals(1, statistics.Count);

		DeploymentStatistics result = statistics[0];

		// there is no running instance
		assertEquals(0, result.Instances);

		IList<IncidentStatistics> incidentStatistics = result.IncidentStatistics;

		// but there is one incident for the failed timer job
		assertEquals(1, incidentStatistics.Count);

		IncidentStatistics incidentStatistic = incidentStatistics[0];
		assertEquals(1, incidentStatistic.IncidentCount);
		assertEquals(org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE, incidentStatistic.IncidentType);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testFailedTimerStartEvent.bpmn20.xml")]
	  public virtual void testQueryByIncidentTypeWithFailedTimerStartEvent()
	  {

		executeAvailableJobs();

		IList<DeploymentStatistics> statistics = managementService.createDeploymentStatisticsQuery().includeIncidentsForType(org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE).list();

		assertEquals(1, statistics.Count);

		DeploymentStatistics result = statistics[0];

		// there is no running instance
		assertEquals(0, result.Instances);

		IList<IncidentStatistics> incidentStatistics = result.IncidentStatistics;

		// but there is one incident for the failed timer job
		assertEquals(1, incidentStatistics.Count);

		IncidentStatistics incidentStatistic = incidentStatistics[0];
		assertEquals(1, incidentStatistic.IncidentCount);
		assertEquals(org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE, incidentStatistic.IncidentType);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testFailedTimerStartEvent.bpmn20.xml")]
	  public virtual void testQueryByFailedJobsWithFailedTimerStartEvent()
	  {

		executeAvailableJobs();

		IList<DeploymentStatistics> statistics = managementService.createDeploymentStatisticsQuery().includeFailedJobs().list();

		assertEquals(1, statistics.Count);

		DeploymentStatistics result = statistics[0];

		// there is no running instance
		assertEquals(0, result.Instances);
		// but there is one failed timer job
		assertEquals(1, result.FailedJobs);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testFailedTimerStartEvent.bpmn20.xml")]
	  public virtual void testQueryByFailedJobsAndIncidentsWithFailedTimerStartEvent()
	  {

		executeAvailableJobs();

		IList<DeploymentStatistics> statistics = managementService.createDeploymentStatisticsQuery().includeFailedJobs().includeIncidents().list();

		assertEquals(1, statistics.Count);

		DeploymentStatistics result = statistics[0];

		// there is no running instance
		assertEquals(0, result.Instances);
		// but there is one failed timer job
		assertEquals(1, result.FailedJobs);

		IList<IncidentStatistics> incidentStatistics = result.IncidentStatistics;

		// and there is one incident for the failed timer job
		assertEquals(1, incidentStatistics.Count);

		IncidentStatistics incidentStatistic = incidentStatistics[0];
		assertEquals(1, incidentStatistic.IncidentCount);
		assertEquals(org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE, incidentStatistic.IncidentType);
	  }
	}

}