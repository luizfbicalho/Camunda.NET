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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.containsString;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using IncidentStatistics = org.camunda.bpm.engine.management.IncidentStatistics;
	using ProcessDefinitionStatistics = org.camunda.bpm.engine.management.ProcessDefinitionStatistics;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;

	public class ProcessDefinitionStatisticsQueryTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml") public void testProcessDefinitionStatisticsQueryWithFailedJobs()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml")]
	  public virtual void testProcessDefinitionStatisticsQueryWithFailedJobs()
	  {
		runtimeService.startProcessInstanceByKey("ExampleProcess");

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["fail"] = true;
		runtimeService.startProcessInstanceByKey("ExampleProcess", parameters);

		executeAvailableJobs();

		IList<ProcessDefinitionStatistics> statistics = managementService.createProcessDefinitionStatisticsQuery().includeFailedJobs().list();

		Assert.assertEquals(1, statistics.Count);

		ProcessDefinitionStatistics definitionResult = statistics[0];
		Assert.assertEquals(2, definitionResult.Instances);
		Assert.assertEquals(1, definitionResult.FailedJobs);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml") public void testProcessDefinitionStatisticsQueryWithIncidents()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml")]
	  public virtual void testProcessDefinitionStatisticsQueryWithIncidents()
	  {
		runtimeService.startProcessInstanceByKey("ExampleProcess");

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["fail"] = true;
		runtimeService.startProcessInstanceByKey("ExampleProcess", parameters);

		executeAvailableJobs();

		IList<ProcessDefinitionStatistics> statistics = managementService.createProcessDefinitionStatisticsQuery().includeIncidents().list();

		Assert.assertEquals(1, statistics.Count);

		ProcessDefinitionStatistics definitionResult = statistics[0];
		Assert.assertEquals(2, definitionResult.Instances);

		assertFalse(definitionResult.IncidentStatistics.Count == 0);
		assertEquals(1, definitionResult.IncidentStatistics.Count);

		IncidentStatistics incidentStatistics = definitionResult.IncidentStatistics[0];
		Assert.assertEquals(org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE, incidentStatistics.IncidentType);
		Assert.assertEquals(1, incidentStatistics.IncidentCount);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml") public void testProcessDefinitionStatisticsQueryWithIncidentType()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml")]
	  public virtual void testProcessDefinitionStatisticsQueryWithIncidentType()
	  {
		runtimeService.startProcessInstanceByKey("ExampleProcess");

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["fail"] = true;
		runtimeService.startProcessInstanceByKey("ExampleProcess", parameters);

		executeAvailableJobs();

		IList<ProcessDefinitionStatistics> statistics = managementService.createProcessDefinitionStatisticsQuery().includeIncidentsForType("failedJob").list();

		Assert.assertEquals(1, statistics.Count);

		ProcessDefinitionStatistics definitionResult = statistics[0];
		Assert.assertEquals(2, definitionResult.Instances);

		assertFalse(definitionResult.IncidentStatistics.Count == 0);
		assertEquals(1, definitionResult.IncidentStatistics.Count);

		IncidentStatistics incidentStatistics = definitionResult.IncidentStatistics[0];
		Assert.assertEquals(org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE, incidentStatistics.IncidentType);
		Assert.assertEquals(1, incidentStatistics.IncidentCount);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml") public void testProcessDefinitionStatisticsQueryWithInvalidIncidentType()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml")]
	  public virtual void testProcessDefinitionStatisticsQueryWithInvalidIncidentType()
	  {
		runtimeService.startProcessInstanceByKey("ExampleProcess");

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["fail"] = true;
		runtimeService.startProcessInstanceByKey("ExampleProcess", parameters);

		executeAvailableJobs();

		IList<ProcessDefinitionStatistics> statistics = managementService.createProcessDefinitionStatisticsQuery().includeIncidentsForType("invalid").list();

		Assert.assertEquals(1, statistics.Count);

		ProcessDefinitionStatistics definitionResult = statistics[0];
		Assert.assertEquals(2, definitionResult.Instances);

		assertTrue(definitionResult.IncidentStatistics.Count == 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml") public void testProcessDefinitionStatisticsQueryWithIncidentsAndFailedJobs()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml")]
	  public virtual void testProcessDefinitionStatisticsQueryWithIncidentsAndFailedJobs()
	  {
		runtimeService.startProcessInstanceByKey("ExampleProcess");

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["fail"] = true;
		runtimeService.startProcessInstanceByKey("ExampleProcess", parameters);

		executeAvailableJobs();

		IList<ProcessDefinitionStatistics> statistics = managementService.createProcessDefinitionStatisticsQuery().includeIncidents().includeFailedJobs().list();

		Assert.assertEquals(1, statistics.Count);

		ProcessDefinitionStatistics definitionResult = statistics[0];
		Assert.assertEquals(2, definitionResult.Instances);
		Assert.assertEquals(1, definitionResult.FailedJobs);

		assertFalse(definitionResult.IncidentStatistics.Count == 0);
		assertEquals(1, definitionResult.IncidentStatistics.Count);

		IncidentStatistics incidentStatistics = definitionResult.IncidentStatistics[0];
		Assert.assertEquals(org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE, incidentStatistics.IncidentType);
		Assert.assertEquals(1, incidentStatistics.IncidentCount);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml") public void testProcessDefinitionStatisticsQueryWithoutRunningInstances()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml")]
	  public virtual void testProcessDefinitionStatisticsQueryWithoutRunningInstances()
	  {
		IList<ProcessDefinitionStatistics> statistics = managementService.createProcessDefinitionStatisticsQuery().includeFailedJobs().includeIncidents().list();

		Assert.assertEquals(1, statistics.Count);

		ProcessDefinitionStatistics definitionResult = statistics[0];
		Assert.assertEquals(0, definitionResult.Instances);
		Assert.assertEquals(0, definitionResult.FailedJobs);

		statistics = managementService.createProcessDefinitionStatisticsQuery().includeIncidents().list();

		assertTrue(definitionResult.IncidentStatistics.Count == 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml") public void testProcessDefinitionStatisticsQueryCount()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml")]
	  public virtual void testProcessDefinitionStatisticsQueryCount()
	  {
		runtimeService.startProcessInstanceByKey("ExampleProcess");

		executeAvailableJobs();

		long count = managementService.createProcessDefinitionStatisticsQuery().includeFailedJobs().includeIncidents().count();

		Assert.assertEquals(1, count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testMultiInstanceStatisticsQuery.bpmn20.xml") public void testMultiInstanceProcessDefinitionStatisticsQuery()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testMultiInstanceStatisticsQuery.bpmn20.xml")]
	  public virtual void testMultiInstanceProcessDefinitionStatisticsQuery()
	  {
		runtimeService.startProcessInstanceByKey("MIExampleProcess");

		IList<ProcessDefinitionStatistics> statistics = managementService.createProcessDefinitionStatisticsQuery().list();

		Assert.assertEquals(1, statistics.Count);

		ProcessDefinitionStatistics result = statistics[0];
		Assert.assertEquals(1, result.Instances);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testSubprocessStatisticsQuery.bpmn20.xml") public void testSubprocessProcessDefinitionStatisticsQuery()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testSubprocessStatisticsQuery.bpmn20.xml")]
	  public virtual void testSubprocessProcessDefinitionStatisticsQuery()
	  {
		runtimeService.startProcessInstanceByKey("ExampleProcess");

		IList<ProcessDefinitionStatistics> statistics = managementService.createProcessDefinitionStatisticsQuery().list();

		Assert.assertEquals(1, statistics.Count);

		ProcessDefinitionStatistics result = statistics[0];
		Assert.assertEquals(1, result.Instances);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testCallActivityWithIncidentsWithoutFailedJobs.bpmn20.xml") public void testCallActivityProcessDefinitionStatisticsQuery()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testCallActivityWithIncidentsWithoutFailedJobs.bpmn20.xml")]
	  public virtual void testCallActivityProcessDefinitionStatisticsQuery()
	  {
		runtimeService.startProcessInstanceByKey("callExampleSubProcess");

		executeAvailableJobs();

		IList<ProcessDefinitionStatistics> statistics = managementService.createProcessDefinitionStatisticsQuery().includeFailedJobs().list();

		Assert.assertEquals(2, statistics.Count);

		foreach (ProcessDefinitionStatistics result in statistics)
		{
		  if (result.Key.Equals("ExampleProcess"))
		  {
			Assert.assertEquals(1, result.Instances);
			Assert.assertEquals(1, result.FailedJobs);
		  }
		  else if (result.Key.Equals("callExampleSubProcess"))
		  {
			Assert.assertEquals(1, result.Instances);
			Assert.assertEquals(0, result.FailedJobs);
		  }
		  else
		  {
			fail(result + " was not expected.");
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml") public void testProcessDefinitionStatisticsQueryForMultipleVersions()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml")]
	  public virtual void testProcessDefinitionStatisticsQueryForMultipleVersions()
	  {
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml").deploy();

		IList<ProcessDefinition> definitions = repositoryService.createProcessDefinitionQuery().processDefinitionKey("ExampleProcess").list();

		foreach (ProcessDefinition definition in definitions)
		{
		  runtimeService.startProcessInstanceById(definition.Id);
		}

		executeAvailableJobs();

		IList<ProcessDefinitionStatistics> statistics = managementService.createProcessDefinitionStatisticsQuery().includeFailedJobs().includeIncidents().list();

		Assert.assertEquals(2, statistics.Count);

		ProcessDefinitionStatistics definitionResult = statistics[0];
		Assert.assertEquals(1, definitionResult.Instances);
		Assert.assertEquals(0, definitionResult.FailedJobs);

		assertTrue(definitionResult.IncidentStatistics.Count == 0);

		definitionResult = statistics[1];
		Assert.assertEquals(1, definitionResult.Instances);
		Assert.assertEquals(0, definitionResult.FailedJobs);

		assertTrue(definitionResult.IncidentStatistics.Count == 0);

		repositoryService.deleteDeployment(deployment.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml") public void testProcessDefinitionStatisticsQueryForMultipleVersionsWithFailedJobsAndIncidents()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml")]
	  public virtual void testProcessDefinitionStatisticsQueryForMultipleVersionsWithFailedJobsAndIncidents()
	  {
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml").deploy();

		IList<ProcessDefinition> definitions = repositoryService.createProcessDefinitionQuery().processDefinitionKey("ExampleProcess").list();

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["fail"] = true;

		foreach (ProcessDefinition definition in definitions)
		{
		  runtimeService.startProcessInstanceById(definition.Id, parameters);
		}

		executeAvailableJobs();

		IList<ProcessDefinitionStatistics> statistics = managementService.createProcessDefinitionStatisticsQuery().includeFailedJobs().includeIncidents().list();

		Assert.assertEquals(2, statistics.Count);

		ProcessDefinitionStatistics definitionResult = statistics[0];
		Assert.assertEquals(1, definitionResult.Instances);
		Assert.assertEquals(1, definitionResult.FailedJobs);

		IList<IncidentStatistics> incidentStatistics = definitionResult.IncidentStatistics;
		assertFalse(incidentStatistics.Count == 0);
		assertEquals(1, incidentStatistics.Count);

		IncidentStatistics incident = incidentStatistics[0];

		assertEquals(org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE, incident.IncidentType);
		assertEquals(1, incident.IncidentCount);

		definitionResult = statistics[1];
		Assert.assertEquals(1, definitionResult.Instances);
		Assert.assertEquals(1, definitionResult.FailedJobs);

		incidentStatistics = definitionResult.IncidentStatistics;
		assertFalse(incidentStatistics.Count == 0);
		assertEquals(1, incidentStatistics.Count);

		incident = incidentStatistics[0];

		assertEquals(org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE, incident.IncidentType);
		assertEquals(1, incident.IncidentCount);

		repositoryService.deleteDeployment(deployment.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml") public void testProcessDefinitionStatisticsQueryForMultipleVersionsWithIncidentType()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml")]
	  public virtual void testProcessDefinitionStatisticsQueryForMultipleVersionsWithIncidentType()
	  {
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml").deploy();

		IList<ProcessDefinition> definitions = repositoryService.createProcessDefinitionQuery().processDefinitionKey("ExampleProcess").list();

		foreach (ProcessDefinition definition in definitions)
		{
		  runtimeService.startProcessInstanceById(definition.Id);
		}

		executeAvailableJobs();

		IList<ProcessDefinitionStatistics> statistics = managementService.createProcessDefinitionStatisticsQuery().includeFailedJobs().includeIncidentsForType("failedJob").list();

		Assert.assertEquals(2, statistics.Count);

		ProcessDefinitionStatistics definitionResult = statistics[0];
		Assert.assertEquals(1, definitionResult.Instances);
		Assert.assertEquals(0, definitionResult.FailedJobs);

		assertTrue(definitionResult.IncidentStatistics.Count == 0);

		definitionResult = statistics[1];
		Assert.assertEquals(1, definitionResult.Instances);
		Assert.assertEquals(0, definitionResult.FailedJobs);

		assertTrue(definitionResult.IncidentStatistics.Count == 0);

		repositoryService.deleteDeployment(deployment.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml") public void testProcessDefinitionStatisticsQueryPagination()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml")]
	  public virtual void testProcessDefinitionStatisticsQueryPagination()
	  {
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQuery.bpmn20.xml").deploy();

		IList<ProcessDefinition> definitions = repositoryService.createProcessDefinitionQuery().processDefinitionKey("ExampleProcess").list();

		foreach (ProcessDefinition definition in definitions)
		{
		  runtimeService.startProcessInstanceById(definition.Id);
		}

		IList<ProcessDefinitionStatistics> statistics = managementService.createProcessDefinitionStatisticsQuery().includeFailedJobs().listPage(0, 1);

		Assert.assertEquals(1, statistics.Count);

		repositoryService.deleteDeployment(deployment.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testCallActivityWithIncidentsWithoutFailedJobs.bpmn20.xml") public void testProcessDefinitionStatisticsQueryWithIncidentsWithoutFailedJobs()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testCallActivityWithIncidentsWithoutFailedJobs.bpmn20.xml")]
	  public virtual void testProcessDefinitionStatisticsQueryWithIncidentsWithoutFailedJobs()
	  {
		runtimeService.startProcessInstanceByKey("callExampleSubProcess");

		executeAvailableJobs();

		IList<ProcessDefinitionStatistics> statistics = managementService.createProcessDefinitionStatisticsQuery().includeIncidents().includeFailedJobs().list();

		Assert.assertEquals(2, statistics.Count);

		ProcessDefinitionStatistics callExampleSubProcessStaticstics = null;
		ProcessDefinitionStatistics exampleSubProcessStaticstics = null;

		foreach (ProcessDefinitionStatistics current in statistics)
		{
		  if (current.Key.Equals("callExampleSubProcess"))
		  {
			callExampleSubProcessStaticstics = current;
		  }
		  else if (current.Key.Equals("ExampleProcess"))
		  {
			exampleSubProcessStaticstics = current;
		  }
		  else
		  {
			fail(current.Key + " was not expected.");
		  }
		}

		assertNotNull(callExampleSubProcessStaticstics);
		assertNotNull(exampleSubProcessStaticstics);

		// "super" process definition
		assertEquals(1, callExampleSubProcessStaticstics.Instances);
		assertEquals(0, callExampleSubProcessStaticstics.FailedJobs);

		assertFalse(callExampleSubProcessStaticstics.IncidentStatistics.Count == 0);
		assertEquals(1, callExampleSubProcessStaticstics.IncidentStatistics.Count);

		IncidentStatistics incidentStatistics = callExampleSubProcessStaticstics.IncidentStatistics[0];
		assertEquals(org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE, incidentStatistics.IncidentType);
		assertEquals(1, incidentStatistics.IncidentCount);

		// "called" process definition
		assertEquals(1, exampleSubProcessStaticstics.Instances);
		assertEquals(1, exampleSubProcessStaticstics.FailedJobs);

		assertFalse(exampleSubProcessStaticstics.IncidentStatistics.Count == 0);
		assertEquals(1, exampleSubProcessStaticstics.IncidentStatistics.Count);

		incidentStatistics = exampleSubProcessStaticstics.IncidentStatistics[0];
		assertEquals(org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE, incidentStatistics.IncidentType);
		assertEquals(1, incidentStatistics.IncidentCount);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testFailedTimerStartEvent.bpmn20.xml")]
	  public virtual void testQueryByIncidentsWithFailedTimerStartEvent()
	  {

		executeAvailableJobs();

		IList<ProcessDefinitionStatistics> statistics = managementService.createProcessDefinitionStatisticsQuery().includeIncidents().list();

		assertEquals(1, statistics.Count);

		ProcessDefinitionStatistics result = statistics[0];

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

		IList<ProcessDefinitionStatistics> statistics = managementService.createProcessDefinitionStatisticsQuery().includeIncidentsForType(org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE).list();

		assertEquals(1, statistics.Count);

		ProcessDefinitionStatistics result = statistics[0];

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

		IList<ProcessDefinitionStatistics> statistics = managementService.createProcessDefinitionStatisticsQuery().includeFailedJobs().list();

		assertEquals(1, statistics.Count);

		ProcessDefinitionStatistics result = statistics[0];

		// there is no running instance
		assertEquals(0, result.Instances);
		// but there is one failed timer job
		assertEquals(1, result.FailedJobs);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testFailedTimerStartEvent.bpmn20.xml")]
	  public virtual void testQueryByFailedJobsAndIncidentsWithFailedTimerStartEvent()
	  {

		executeAvailableJobs();

		IList<ProcessDefinitionStatistics> statistics = managementService.createProcessDefinitionStatisticsQuery().includeFailedJobs().includeIncidents().list();

		assertEquals(1, statistics.Count);

		ProcessDefinitionStatistics result = statistics[0];

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

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testCallActivityWithIncidentsWithoutFailedJobs.bpmn20.xml") public void testIncludeRootIncidentsOnly()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testCallActivityWithIncidentsWithoutFailedJobs.bpmn20.xml")]
	  public virtual void testIncludeRootIncidentsOnly()
	  {
		runtimeService.startProcessInstanceByKey("callExampleSubProcess");

		executeAvailableJobs();

		IList<ProcessDefinitionStatistics> statistics = managementService.createProcessDefinitionStatisticsQuery().includeRootIncidents().list();

		// two process definitions
		assertEquals(2, statistics.Count);

		foreach (ProcessDefinitionStatistics definitionResult in statistics)
		{

		  if (definitionResult.Key.Equals("callExampleSubProcess"))
		  {
			// there is no root incidents
			assertTrue(definitionResult.IncidentStatistics.Count == 0);

		  }
		  else if (definitionResult.Key.Equals("ExampleProcess"))
		  {
			// there is one root incident
			assertFalse(definitionResult.IncidentStatistics.Count == 0);
			assertEquals(1, definitionResult.IncidentStatistics.Count);
			assertEquals(1, definitionResult.IncidentStatistics[0].IncidentCount);

		  }
		  else
		  {
			// fail if the process definition key does not match
			fail();
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testCallActivityWithIncidentsWithoutFailedJobs.bpmn20.xml") public void testIncludeRootIncidentsFails()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testCallActivityWithIncidentsWithoutFailedJobs.bpmn20.xml")]
	  public virtual void testIncludeRootIncidentsFails()
	  {
		runtimeService.startProcessInstanceByKey("callExampleSubProcess");

		executeAvailableJobs();

		try
		{
			managementService.createProcessDefinitionStatisticsQuery().includeIncidents().includeRootIncidents().list();
		}
		catch (ProcessEngineException e)
		{
		  Assert.assertThat(e.Message, containsString("It is not possible to use includeIncident() and includeRootIncidents() to execute one query"));
		}
	  }

	  public virtual void testProcessDefinitionStatisticsProperties()
	  {
		string resourceName = "org/camunda/bpm/engine/test/api/mgmt/ProcessDefinitionStatisticsQueryTest.testProcessDefinitionStatisticsProperties.bpmn20.xml";
		string deploymentId = deploymentForTenant("tenant1", resourceName);

		ProcessDefinitionStatistics processDefinitionStatistics = managementService.createProcessDefinitionStatisticsQuery().singleResult();

		assertEquals("testProcess", processDefinitionStatistics.Key);
		assertEquals("process name", processDefinitionStatistics.Name);
		assertEquals("Examples", processDefinitionStatistics.Category);
		assertEquals(null, processDefinitionStatistics.Description); // it is not parsed for the statistics query
		assertEquals("tenant1", processDefinitionStatistics.TenantId);
		assertEquals("v0.1.0", processDefinitionStatistics.VersionTag);
		assertEquals(deploymentId, processDefinitionStatistics.DeploymentId);
		assertEquals(resourceName, processDefinitionStatistics.ResourceName);
		assertEquals(null, processDefinitionStatistics.DiagramResourceName);
		assertEquals(1, processDefinitionStatistics.Version);
		assertEquals(0, processDefinitionStatistics.Instances);
		assertEquals(0, processDefinitionStatistics.FailedJobs);
		assertTrue(processDefinitionStatistics.IncidentStatistics.Count == 0);
	  }

	}

}