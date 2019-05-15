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
namespace org.camunda.bpm.engine.test.api.authorization.history
{

	using DateUtils = org.apache.commons.lang3.time.DateUtils;
	using Groups = org.camunda.bpm.engine.authorization.Groups;
	using HistoricCaseInstance = org.camunda.bpm.engine.history.HistoricCaseInstance;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricIncident = org.camunda.bpm.engine.history.HistoricIncident;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using Meter = org.camunda.bpm.engine.impl.metrics.Meter;
	using HistoricIncidentEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricIncidentEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using TestPojo = org.camunda.bpm.engine.test.dmn.businessruletask.TestPojo;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Test = org.junit.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_CLEANUP_STRATEGY_END_TIME_BASED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_CLEANUP_STRATEGY_REMOVAL_TIME_BASED;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoryCleanupAuthorizationTest : AuthorizationTest
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setUp() throws Exception
	  public override void setUp()
	  {
		base.setUp();
		processEngineConfiguration.HistoryCleanupStrategy = HISTORY_CLEANUP_STRATEGY_END_TIME_BASED;
	  }

	  public override void tearDown()
	  {
		processEngineConfiguration.HistoryCleanupStrategy = HISTORY_CLEANUP_STRATEGY_REMOVAL_TIME_BASED;

		base.tearDown();
		clearDatabase();
		clearMetrics();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionRef.bpmn20.xml", "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml", "org/camunda/bpm/engine/test/api/authorization/oneTaskCase.cmmn" }) public void testHistoryCleanupWithAuthorization()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionRef.bpmn20.xml", "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml", "org/camunda/bpm/engine/test/api/authorization/oneTaskCase.cmmn" })]
	  public virtual void testHistoryCleanupWithAuthorization()
	  {
		// given
		prepareInstances(5, 5, 5);

		ClockUtil.CurrentTime = DateTime.Now;
		// when
		identityService.setAuthentication("user", Collections.singletonList(org.camunda.bpm.engine.authorization.Groups_Fields.CAMUNDA_ADMIN), null);

		string jobId = historyService.cleanUpHistoryAsync(true).Id;

		managementService.executeJob(jobId);

		// then
		assertResult(0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionRef.bpmn20.xml", "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml", "org/camunda/bpm/engine/test/api/authorization/oneTaskCase.cmmn" }) public void testHistoryCleanupWithoutAuthorization()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionRef.bpmn20.xml", "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml", "org/camunda/bpm/engine/test/api/authorization/oneTaskCase.cmmn" })]
	  public virtual void testHistoryCleanupWithoutAuthorization()
	  {
		// given
		prepareInstances(5, 5, 5);

		ClockUtil.CurrentTime = DateTime.Now;

		try
		{
		  // when
		  historyService.cleanUpHistoryAsync(true).Id;
		  fail("Exception expected: It should not be possible to execute the history cleanup");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent("ENGINE-03029 Required admin authenticated group or user.", message);
		}
	  }

	  protected internal virtual void prepareInstances(int? processInstanceTimeToLive, int? decisionTimeToLive, int? caseTimeToLive)
	  {
		// update time to live
		disableAuthorization();
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().processDefinitionKey("testProcess").list();
		assertEquals(1, processDefinitions.Count);
		repositoryService.updateProcessDefinitionHistoryTimeToLive(processDefinitions[0].Id, processInstanceTimeToLive);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.repository.DecisionDefinition> decisionDefinitions = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey("testDecision").list();
		IList<DecisionDefinition> decisionDefinitions = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey("testDecision").list();
		assertEquals(1, decisionDefinitions.Count);
		repositoryService.updateDecisionDefinitionHistoryTimeToLive(decisionDefinitions[0].Id, decisionTimeToLive);

		IList<CaseDefinition> caseDefinitions = repositoryService.createCaseDefinitionQuery().caseDefinitionKey("oneTaskCase").list();
		assertEquals(1, caseDefinitions.Count);
		repositoryService.updateCaseDefinitionHistoryTimeToLive(caseDefinitions[0].Id, caseTimeToLive);

		DateTime oldCurrentTime = ClockUtil.CurrentTime;
		ClockUtil.CurrentTime = DateUtils.addDays(oldCurrentTime, -6);

		// create 3 process instances
		IList<string> processInstanceIds = new List<string>();
		IDictionary<string, object> variables = Variables.createVariables().putValue("pojo", new TestPojo("okay", 13.37));
		for (int i = 0; i < 3; i++)
		{
		  ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess", variables);
		  processInstanceIds.Add(processInstance.Id);
		}
		runtimeService.deleteProcessInstances(processInstanceIds, null, true, true);

		// +10 standalone decisions
		for (int i = 0; i < 10; i++)
		{
		  decisionService.evaluateDecisionByKey("testDecision").variables(variables).evaluate();
		}

		// create 4 case instances
		for (int i = 0; i < 4; i++)
		{
		  CaseInstance caseInstance = caseService.createCaseInstanceByKey("oneTaskCase", Variables.createVariables().putValue("pojo", new TestPojo("okay", 13.37 + i)));
		  caseService.terminateCaseExecution(caseInstance.Id);
		  caseService.closeCaseInstance(caseInstance.Id);
		}

		ClockUtil.CurrentTime = oldCurrentTime;
		enableAuthorization();

	  }

	  protected internal virtual void assertResult(long expectedInstanceCount)
	  {
		long count = historyService.createHistoricProcessInstanceQuery().count() + historyService.createHistoricDecisionInstanceQuery().count() + historyService.createHistoricCaseInstanceQuery().count();
		assertEquals(expectedInstanceCount, count);
	  }

	  protected internal virtual void clearDatabase()
	  {
		// reset configuration changes
		string defaultStartTime = processEngineConfiguration.HistoryCleanupBatchWindowStartTime;
		string defaultEndTime = processEngineConfiguration.HistoryCleanupBatchWindowEndTime;
		int defaultBatchSize = processEngineConfiguration.HistoryCleanupBatchSize;

		processEngineConfiguration.HistoryCleanupBatchWindowStartTime = defaultStartTime;
		processEngineConfiguration.HistoryCleanupBatchWindowEndTime = defaultEndTime;
		processEngineConfiguration.HistoryCleanupBatchSize = defaultBatchSize;

		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));

		IList<HistoricProcessInstance> historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();
		foreach (HistoricProcessInstance historicProcessInstance in historicProcessInstances)
		{
		  historyService.deleteHistoricProcessInstance(historicProcessInstance.Id);
		}

		IList<HistoricDecisionInstance> historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().list();
		foreach (HistoricDecisionInstance historicDecisionInstance in historicDecisionInstances)
		{
		  historyService.deleteHistoricDecisionInstanceByInstanceId(historicDecisionInstance.Id);
		}

		IList<HistoricCaseInstance> historicCaseInstances = historyService.createHistoricCaseInstanceQuery().list();
		foreach (HistoricCaseInstance historicCaseInstance in historicCaseInstances)
		{
		  historyService.deleteHistoricCaseInstance(historicCaseInstance.Id);
		}
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly HistoryCleanupAuthorizationTest outerInstance;

		  public CommandAnonymousInnerClass(HistoryCleanupAuthorizationTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {

			IList<Job> jobs = outerInstance.managementService.createJobQuery().list();
			if (jobs.Count > 0)
			{
			  assertEquals(1, jobs.Count);
			  string jobId = jobs[0].Id;
			  commandContext.JobManager.deleteJob((JobEntity) jobs[0]);
			  commandContext.HistoricJobLogManager.deleteHistoricJobLogByJobId(jobId);
			}

			IList<HistoricIncident> historicIncidents = outerInstance.historyService.createHistoricIncidentQuery().list();
			foreach (HistoricIncident historicIncident in historicIncidents)
			{
			  commandContext.DbEntityManager.delete((HistoricIncidentEntity) historicIncident);
			}

			commandContext.MeterLogManager.deleteAll();

			return null;
		  }
	  }

	  protected internal virtual void clearMetrics()
	  {
		ICollection<Meter> meters = processEngineConfiguration.MetricsRegistry.Meters.Values;
		foreach (Meter meter in meters)
		{
		  meter.AndClear;
		}
		managementService.deleteMetrics(null);
	  }
	}

}