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
	using ActivityStatistics = org.camunda.bpm.engine.management.ActivityStatistics;
	using IncidentStatistics = org.camunda.bpm.engine.management.IncidentStatistics;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;

	public class ActivityStatisticsQueryTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml") public void testActivityStatisticsQueryWithoutFailedJobs()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml")]
	  public virtual void testActivityStatisticsQueryWithoutFailedJobs()
	  {

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["fail"] = true;

		runtimeService.startProcessInstanceByKey("ExampleProcess", parameters);

		executeAvailableJobs();

		ProcessDefinition definition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("ExampleProcess").singleResult();

		IList<ActivityStatistics> statistics = managementService.createActivityStatisticsQuery(definition.Id).list();

		Assert.assertEquals(1, statistics.Count);

		ActivityStatistics activityResult = statistics[0];
		Assert.assertEquals(1, activityResult.Instances);
		Assert.assertEquals("theServiceTask", activityResult.Id);
		Assert.assertEquals(0, activityResult.FailedJobs);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml") public void testActivityStatisticsQueryWithIncidents()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml")]
	  public virtual void testActivityStatisticsQueryWithIncidents()
	  {

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["fail"] = true;

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("ExampleProcess", parameters);

		executeAvailableJobs();

		IList<ActivityStatistics> statistics = managementService.createActivityStatisticsQuery(processInstance.ProcessDefinitionId).includeIncidents().list();

		Assert.assertEquals(1, statistics.Count);

		ActivityStatistics activityResult = statistics[0];

		IList<IncidentStatistics> incidentStatistics = activityResult.IncidentStatistics;
		assertFalse(incidentStatistics.Count == 0);
		assertEquals(1, incidentStatistics.Count);

		IncidentStatistics incident = incidentStatistics[0];
		assertEquals(org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE, incident.IncidentType);
		assertEquals(1, incident.IncidentCount);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml") public void testActivityStatisticsQueryWithIncidentType()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml")]
	  public virtual void testActivityStatisticsQueryWithIncidentType()
	  {

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["fail"] = true;

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("ExampleProcess", parameters);

		executeAvailableJobs();

		IList<ActivityStatistics> statistics = managementService.createActivityStatisticsQuery(processInstance.ProcessDefinitionId).includeIncidentsForType("failedJob").list();

		Assert.assertEquals(1, statistics.Count);

		ActivityStatistics activityResult = statistics[0];

		IList<IncidentStatistics> incidentStatistics = activityResult.IncidentStatistics;
		assertFalse(incidentStatistics.Count == 0);
		assertEquals(1, incidentStatistics.Count);

		IncidentStatistics incident = incidentStatistics[0];
		assertEquals(org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE, incident.IncidentType);
		assertEquals(1, incident.IncidentCount);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml") public void testActivityStatisticsQueryWithInvalidIncidentType()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml")]
	  public virtual void testActivityStatisticsQueryWithInvalidIncidentType()
	  {

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["fail"] = true;

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("ExampleProcess", parameters);

		executeAvailableJobs();

		IList<ActivityStatistics> statistics = managementService.createActivityStatisticsQuery(processInstance.ProcessDefinitionId).includeIncidentsForType("invalid").list();

		Assert.assertEquals(1, statistics.Count);

		ActivityStatistics activityResult = statistics[0];

		IList<IncidentStatistics> incidentStatistics = activityResult.IncidentStatistics;
		assertTrue(incidentStatistics.Count == 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testCallActivityWithIncidentsWithoutFailedJobs.bpmn20.xml") public void testActivityStatisticsQueryWithIncidentsWithoutFailedJobs()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testCallActivityWithIncidentsWithoutFailedJobs.bpmn20.xml")]
	  public virtual void testActivityStatisticsQueryWithIncidentsWithoutFailedJobs()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("callExampleSubProcess");

		executeAvailableJobs();

		IList<ActivityStatistics> statistics = managementService.createActivityStatisticsQuery(processInstance.ProcessDefinitionId).includeIncidents().includeFailedJobs().list();

		Assert.assertEquals(1, statistics.Count);

		ActivityStatistics activityResult = statistics[0];

		Assert.assertEquals("callSubProcess", activityResult.Id);
		Assert.assertEquals(0, activityResult.FailedJobs); // has no failed jobs

		IList<IncidentStatistics> incidentStatistics = activityResult.IncidentStatistics;
		assertFalse(incidentStatistics.Count == 0);
		assertEquals(1, incidentStatistics.Count);

		IncidentStatistics incident = incidentStatistics[0];
		assertEquals(org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE, incident.IncidentType);
		assertEquals(1, incident.IncidentCount); //... but has one incident
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQuery.bpmn20.xml") public void testActivityStatisticsQuery()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQuery.bpmn20.xml")]
	  public virtual void testActivityStatisticsQuery()
	  {
		runtimeService.startProcessInstanceByKey("ExampleProcess");
		ProcessDefinition definition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("ExampleProcess").singleResult();

		IList<ActivityStatistics> statistics = managementService.createActivityStatisticsQuery(definition.Id).includeFailedJobs().includeIncidents().list();

		Assert.assertEquals(1, statistics.Count);

		ActivityStatistics activityResult = statistics[0];
		Assert.assertEquals(1, activityResult.Instances);
		Assert.assertEquals("theTask", activityResult.Id);
		Assert.assertEquals(0, activityResult.FailedJobs);
		assertTrue(activityResult.IncidentStatistics.Count == 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQuery.bpmn20.xml") public void testActivityStatisticsQueryCount()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQuery.bpmn20.xml")]
	  public virtual void testActivityStatisticsQueryCount()
	  {
		runtimeService.startProcessInstanceByKey("ExampleProcess");
		ProcessDefinition definition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("ExampleProcess").singleResult();

		long count = managementService.createActivityStatisticsQuery(definition.Id).includeFailedJobs().includeIncidents().count();

		Assert.assertEquals(1, count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQuery.bpmn20.xml") public void testManyInstancesActivityStatisticsQuery()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQuery.bpmn20.xml")]
	  public virtual void testManyInstancesActivityStatisticsQuery()
	  {
		runtimeService.startProcessInstanceByKey("ExampleProcess");
		runtimeService.startProcessInstanceByKey("ExampleProcess");
		runtimeService.startProcessInstanceByKey("ExampleProcess");

		ProcessDefinition definition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("ExampleProcess").singleResult();

		IList<ActivityStatistics> statistics = managementService.createActivityStatisticsQuery(definition.Id).includeFailedJobs().includeIncidents().list();

		Assert.assertEquals(1, statistics.Count);

		ActivityStatistics activityResult = statistics[0];
		Assert.assertEquals(3, activityResult.Instances);
		Assert.assertEquals("theTask", activityResult.Id);
		Assert.assertEquals(0, activityResult.FailedJobs);
		assertTrue(activityResult.IncidentStatistics.Count == 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testMultiInstanceStatisticsQuery.bpmn20.xml") public void testParallelMultiInstanceActivityStatisticsQueryIncludingFailedJobIncidents()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testMultiInstanceStatisticsQuery.bpmn20.xml")]
	  public virtual void testParallelMultiInstanceActivityStatisticsQueryIncludingFailedJobIncidents()
	  {
		runtimeService.startProcessInstanceByKey("MIExampleProcess");
		ProcessDefinition definition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("MIExampleProcess").singleResult();

		IList<ActivityStatistics> statistics = managementService.createActivityStatisticsQuery(definition.Id).includeFailedJobs().includeIncidents().list();

		Assert.assertEquals(1, statistics.Count);

		ActivityStatistics activityResult = statistics[0];
		Assert.assertEquals(3, activityResult.Instances);
		Assert.assertEquals("theTask", activityResult.Id);
		Assert.assertEquals(0, activityResult.FailedJobs);
		assertTrue(activityResult.IncidentStatistics.Count == 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testMultiInstanceStatisticsQuery.bpmn20.xml") public void testParallelMultiInstanceActivityStatisticsQuery()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testMultiInstanceStatisticsQuery.bpmn20.xml")]
	  public virtual void testParallelMultiInstanceActivityStatisticsQuery()
	  {
		runtimeService.startProcessInstanceByKey("MIExampleProcess");
		ProcessDefinition definition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("MIExampleProcess").singleResult();

		IList<ActivityStatistics> statistics = managementService.createActivityStatisticsQuery(definition.Id).list();

		Assert.assertEquals(1, statistics.Count);

		ActivityStatistics activityResult = statistics[0];
		Assert.assertEquals(3, activityResult.Instances);
		Assert.assertEquals("theTask", activityResult.Id);
		Assert.assertEquals(0, activityResult.FailedJobs);
		assertTrue(activityResult.IncidentStatistics.Count == 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testSubprocessStatisticsQuery.bpmn20.xml") public void testSubprocessActivityStatisticsQuery()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testSubprocessStatisticsQuery.bpmn20.xml")]
	  public virtual void testSubprocessActivityStatisticsQuery()
	  {
		runtimeService.startProcessInstanceByKey("ExampleProcess");

		ProcessDefinition definition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("ExampleProcess").singleResult();

		IList<ActivityStatistics> statistics = managementService.createActivityStatisticsQuery(definition.Id).list();

		Assert.assertEquals(1, statistics.Count);

		ActivityStatistics result = statistics[0];
		Assert.assertEquals(1, result.Instances);
		Assert.assertEquals("subProcessTask", result.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testCallActivityStatisticsQuery.bpmn20.xml", "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml"}) public void testCallActivityActivityStatisticsQuery()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testCallActivityStatisticsQuery.bpmn20.xml", "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQueryWithFailedJobs.bpmn20.xml"})]
	  public virtual void testCallActivityActivityStatisticsQuery()
	  {
		runtimeService.startProcessInstanceByKey("callExampleSubProcess");

		executeAvailableJobs();

		ProcessDefinition definition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("ExampleProcess").singleResult();

		IList<ActivityStatistics> statistics = managementService.createActivityStatisticsQuery(definition.Id).includeFailedJobs().includeIncidents().list();

		Assert.assertEquals(1, statistics.Count);

		ActivityStatistics result = statistics[0];
		Assert.assertEquals(1, result.Instances);
		Assert.assertEquals(0, result.FailedJobs);
		assertTrue(result.IncidentStatistics.Count == 0);

		ProcessDefinition callSubProcessDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("callExampleSubProcess").singleResult();

		IList<ActivityStatistics> callSubProcessStatistics = managementService.createActivityStatisticsQuery(callSubProcessDefinition.Id).includeFailedJobs().includeIncidents().list();

		Assert.assertEquals(1, callSubProcessStatistics.Count);

		result = callSubProcessStatistics[0];
		Assert.assertEquals(1, result.Instances);
		Assert.assertEquals(0, result.FailedJobs);
		assertTrue(result.IncidentStatistics.Count == 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testActivityStatisticsQueryWithIntermediateTimer.bpmn20.xml") public void testActivityStatisticsQueryWithIntermediateTimer()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testActivityStatisticsQueryWithIntermediateTimer.bpmn20.xml")]
	  public virtual void testActivityStatisticsQueryWithIntermediateTimer()
	  {
		runtimeService.startProcessInstanceByKey("ExampleProcess");
		ProcessDefinition definition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("ExampleProcess").singleResult();

		IList<ActivityStatistics> statistics = managementService.createActivityStatisticsQuery(definition.Id).includeFailedJobs().includeIncidents().list();

		Assert.assertEquals(1, statistics.Count);

		ActivityStatistics activityResult = statistics[0];
		Assert.assertEquals(1, activityResult.Instances);
		Assert.assertEquals("theTimer", activityResult.Id);
		Assert.assertEquals(0, activityResult.FailedJobs);
		assertTrue(activityResult.IncidentStatistics.Count == 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNullProcessDefinitionParameter()
	  public virtual void testNullProcessDefinitionParameter()
	  {
		try
		{
		  managementService.createActivityStatisticsQuery(null).list();
		  Assert.fail();
		}
		catch (ProcessEngineException)
		{
		  // expected
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testParallelGatewayStatisticsQuery.bpmn20.xml") public void testActivityStatisticsQueryPagination()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testParallelGatewayStatisticsQuery.bpmn20.xml")]
	  public virtual void testActivityStatisticsQueryPagination()
	  {

		ProcessDefinition definition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("ParGatewayExampleProcess").singleResult();

		runtimeService.startProcessInstanceById(definition.Id);

		IList<ActivityStatistics> statistics = managementService.createActivityStatisticsQuery(definition.Id).includeFailedJobs().includeIncidents().listPage(0, 1);

		Assert.assertEquals(1, statistics.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testParallelGatewayStatisticsQuery.bpmn20.xml") public void testParallelGatewayActivityStatisticsQuery()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testParallelGatewayStatisticsQuery.bpmn20.xml")]
	  public virtual void testParallelGatewayActivityStatisticsQuery()
	  {

		ProcessDefinition definition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("ParGatewayExampleProcess").singleResult();

		runtimeService.startProcessInstanceById(definition.Id);

		IList<ActivityStatistics> statistics = managementService.createActivityStatisticsQuery(definition.Id).list();

		Assert.assertEquals(2, statistics.Count);

		foreach (ActivityStatistics result in statistics)
		{
		  Assert.assertEquals(1, result.Instances);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testNonInterruptingBoundaryEventStatisticsQuery.bpmn20.xml")]
	  public virtual void testNonInterruptingBoundaryEventActivityStatisticsQuery()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		Job boundaryJob = managementService.createJobQuery().singleResult();
		managementService.executeJob(boundaryJob.Id);

		// when
		IList<ActivityStatistics> activityStatistics = managementService.createActivityStatisticsQuery(processInstance.ProcessDefinitionId).list();

		// then
		assertEquals(2, activityStatistics.Count);

		ActivityStatistics userTaskStatistics = getStatistics(activityStatistics, "task");
		assertNotNull(userTaskStatistics);
		assertEquals("task", userTaskStatistics.Id);
		assertEquals(1, userTaskStatistics.Instances);

		ActivityStatistics afterBoundaryStatistics = getStatistics(activityStatistics, "afterBoundaryTask");
		assertNotNull(afterBoundaryStatistics);
		assertEquals("afterBoundaryTask", afterBoundaryStatistics.Id);
		assertEquals(1, afterBoundaryStatistics.Instances);

	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testAsyncInterruptingEventSubProcessStatisticsQuery.bpmn20.xml")]
	  public virtual void testAsyncInterruptingEventSubProcessActivityStatisticsQuery()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		runtimeService.correlateMessage("Message");

		// when
		ActivityStatistics activityStatistics = managementService.createActivityStatisticsQuery(processInstance.ProcessDefinitionId).singleResult();

		  // then
		  assertNotNull(activityStatistics);
		  assertEquals("eventSubprocess", activityStatistics.Id);
		  assertEquals(1, activityStatistics.Instances);
	  }

	  protected internal virtual ActivityStatistics getStatistics(IList<ActivityStatistics> activityStatistics, string activityId)
	  {
		foreach (ActivityStatistics statistics in activityStatistics)
		{
		  if (activityId.Equals(statistics.Id))
		  {
			return statistics;
		  }
		}

		return null;
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testFailedTimerStartEvent.bpmn20.xml")]
	  public virtual void testQueryByIncidentsWithFailedTimerStartEvent()
	  {

		ProcessDefinition definition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("process").singleResult();

		executeAvailableJobs();

		IList<ActivityStatistics> statistics = managementService.createActivityStatisticsQuery(definition.Id).includeIncidents().list();

		assertEquals(1, statistics.Count);

		ActivityStatistics result = statistics[0];

		assertEquals("theStart", result.Id);

		// there is no running activity instance
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

		ProcessDefinition definition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("process").singleResult();

		executeAvailableJobs();

		IList<ActivityStatistics> statistics = managementService.createActivityStatisticsQuery(definition.Id).includeIncidentsForType(org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE).list();

		assertEquals(1, statistics.Count);

		ActivityStatistics result = statistics[0];

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

		ProcessDefinition definition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("process").singleResult();

		executeAvailableJobs();

		IList<ActivityStatistics> statistics = managementService.createActivityStatisticsQuery(definition.Id).includeFailedJobs().list();

		assertEquals(1, statistics.Count);

		ActivityStatistics result = statistics[0];

		// there is no running instance
		assertEquals(0, result.Instances);
		// but there is one failed timer job
		assertEquals(1, result.FailedJobs);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testFailedTimerStartEvent.bpmn20.xml")]
	  public virtual void testQueryByFailedJobsAndIncidentsWithFailedTimerStartEvent()
	  {

		ProcessDefinition definition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("process").singleResult();

		executeAvailableJobs();

		IList<ActivityStatistics> statistics = managementService.createActivityStatisticsQuery(definition.Id).includeFailedJobs().includeIncidents().list();

		assertEquals(1, statistics.Count);

		ActivityStatistics result = statistics[0];

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
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQuery.bpmn20.xml") public void FAILING_testActivityStatisticsQueryWithNoInstances()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/StatisticsTest.testStatisticsQuery.bpmn20.xml")]
	  public virtual void FAILING_testActivityStatisticsQueryWithNoInstances()
	  {

		ProcessDefinition definition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("ExampleProcess").singleResult();

		IList<ActivityStatistics> statistics = managementService.createActivityStatisticsQuery(definition.Id).list();

		Assert.assertEquals(1, statistics.Count);
		ActivityStatistics result = statistics[0];
		Assert.assertEquals("theTask", result.Id);
		Assert.assertEquals(0, result.Instances);
		Assert.assertEquals(0, result.FailedJobs);

	  }
	}

}