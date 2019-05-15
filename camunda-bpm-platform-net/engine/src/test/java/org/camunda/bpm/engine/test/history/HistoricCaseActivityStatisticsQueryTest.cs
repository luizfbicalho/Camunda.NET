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
namespace org.camunda.bpm.engine.test.history
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using HistoricCaseActivityStatistics = org.camunda.bpm.engine.history.HistoricCaseActivityStatistics;
	using HistoricCaseActivityStatisticsQuery = org.camunda.bpm.engine.history.HistoricCaseActivityStatisticsQuery;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_ACTIVITY)]
	public class HistoricCaseActivityStatisticsQueryTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule();
		public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal HistoryService historyService;
	  protected internal CaseService caseService;
	  protected internal RepositoryService repositoryService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		historyService = engineRule.HistoryService;
		caseService = engineRule.CaseService;
		repositoryService = engineRule.RepositoryService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseDefinitionNull()
	  public virtual void testCaseDefinitionNull()
	  {
		// given

		// when
		try
		{
		  historyService.createHistoricCaseActivityStatisticsQuery(null).list();
		  fail("It should not be possible to query for statistics by null.");
		}
		catch (NullValueException)
		{

		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn") public void testNoCaseActivityInstances()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn")]
	  public virtual void testNoCaseActivityInstances()
	  {
		// given
		string caseDefinitionId = CaseDefinition.Id;

		// when
		HistoricCaseActivityStatisticsQuery query = historyService.createHistoricCaseActivityStatisticsQuery(caseDefinitionId);

		// then
		assertEquals(0, query.count());
		assertThat(query.list().size(), @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn") public void testSingleTask()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn")]
	  public virtual void testSingleTask()
	  {
		// given
		string caseDefinitionId = CaseDefinition.Id;

		createCaseByKey(5, "oneTaskCase");

		// when
		HistoricCaseActivityStatisticsQuery query = historyService.createHistoricCaseActivityStatisticsQuery(caseDefinitionId);

		// then
		IList<HistoricCaseActivityStatistics> statistics = query.list();

		assertEquals(1, query.count());
		assertThat(statistics.Count, @is(1));
		assertStatisitcs(statistics[0], "PI_HumanTask_1", 5, 0, 0, 0, 0, 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testMultipleTasks()
	  public virtual void testMultipleTasks()
	  {

		// given
		string caseDefinitionId = CaseDefinition.Id;

		createCaseByKey(5, "case");

		disableByActivity("DISABLED");
		completeByActivity("COMPLETED");
		terminateByActivity("TERMINATED");

		// when
		HistoricCaseActivityStatisticsQuery query = historyService.createHistoricCaseActivityStatisticsQuery(caseDefinitionId);

		// then
		IList<HistoricCaseActivityStatistics> statistics = query.list();
		assertThat(statistics.Count, @is(6));
		assertEquals(query.count(), 6);

		assertStatisitcs(statistics[0], "ACTIVE", 5, 0, 0, 0, 0, 0);
		assertStatisitcs(statistics[1], "AVAILABLE", 0, 5, 0, 0, 0, 0);
		assertStatisitcs(statistics[2], "COMPLETED", 0, 0, 5, 0, 0, 0);
		assertStatisitcs(statistics[3], "DISABLED", 0, 0, 0, 5, 0, 0);
		assertStatisitcs(statistics[4], "ENABLED", 0, 0, 0, 0, 5, 0);
		assertStatisitcs(statistics[5], "TERMINATED", 0, 0, 0, 0, 0, 5);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/history/HistoricCaseActivityStatisticsQueryTest.testMultipleTasks.cmmn" }) public void testStateCount()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/history/HistoricCaseActivityStatisticsQueryTest.testMultipleTasks.cmmn" })]
	  public virtual void testStateCount()
	  {

		// given
		string caseDefinitionId = CaseDefinition.Id;

		createCaseByKey(3, "case");
		completeByActivity("ACTIVE");
		manuallyStartByActivity("AVAILABLE");
		completeByActivity("AVAILABLE");

		createCaseByKey(5, "case");
		completeByActivity("ACTIVE");
		disableByActivity("AVAILABLE");
		reenableByActivity("AVAILABLE");
		manuallyStartByActivity("AVAILABLE");
		terminateByActivity("AVAILABLE");

		createCaseByKey(5, "case");
		terminateByActivity("ACTIVE");

		manuallyStartByActivity("ENABLED");
		completeByActivity("ENABLED");

		manuallyStartByActivity("DISABLED");
		terminateByActivity("DISABLED");

		createCaseByKey(2, "case");
		disableByActivity("DISABLED");

		// when
		HistoricCaseActivityStatisticsQuery query = historyService.createHistoricCaseActivityStatisticsQuery(caseDefinitionId);

		// then
		IList<HistoricCaseActivityStatistics> statistics = query.list();
		assertThat(statistics.Count, @is(6));
		assertEquals(query.count(), 6);

		assertStatisitcs(statistics[0], "ACTIVE", 2, 0, 8, 0, 0, 5);
		assertStatisitcs(statistics[1], "AVAILABLE", 0, 7, 3, 0, 0, 5);
		assertStatisitcs(statistics[2], "COMPLETED", 15, 0, 0, 0, 0, 0);
		assertStatisitcs(statistics[3], "DISABLED", 0, 0, 0, 2, 0, 13);
		assertStatisitcs(statistics[4], "ENABLED", 0, 0, 13, 0, 2, 0);
		assertStatisitcs(statistics[5], "TERMINATED", 15, 0, 0, 0, 0, 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn", "org/camunda/bpm/engine/test/history/HistoricCaseActivityStatisticsQueryTest.testMultipleTasks.cmmn" }) public void testMultipleCaseDefinitions()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn", "org/camunda/bpm/engine/test/history/HistoricCaseActivityStatisticsQueryTest.testMultipleTasks.cmmn" })]
	  public virtual void testMultipleCaseDefinitions()
	  {

		// given
		string caseDefinitionId1 = getCaseDefinition("oneTaskCase").Id;
		string caseDefinitionId2 = getCaseDefinition("case").Id;

		createCaseByKey(5, "oneTaskCase");
		createCaseByKey(10, "case");

		// when
		HistoricCaseActivityStatisticsQuery query1 = historyService.createHistoricCaseActivityStatisticsQuery(caseDefinitionId1);
		HistoricCaseActivityStatisticsQuery query2 = historyService.createHistoricCaseActivityStatisticsQuery(caseDefinitionId2);

		// then
		assertThat(query1.list().size(), @is(1));
		assertThat(query2.list().size(), @is(6));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/history/HistoricCaseActivityStatisticsQueryTest.testMultipleTasks.cmmn" }) public void testPagination()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/history/HistoricCaseActivityStatisticsQueryTest.testMultipleTasks.cmmn" })]
	  public virtual void testPagination()
	  {
		// given
		string caseDefinitionId = CaseDefinition.Id;

		createCaseByKey(5, "case");

		// when
		IList<HistoricCaseActivityStatistics> statistics = historyService.createHistoricCaseActivityStatisticsQuery(caseDefinitionId).listPage(2, 1);

		// then
		assertThat(statistics.Count, @is(1));
		assertThat(statistics[0].Id, @is("COMPLETED"));
	  }

	  protected internal virtual void assertStatisitcs(HistoricCaseActivityStatistics statistics, string id, long active, long availabe, long completed, long disabled, long enabled, long terminated)
	  {
		assertThat(statistics.Id, @is(id));
		assertEquals(active, statistics.Active);
		assertEquals(availabe, statistics.Available);
		assertEquals(completed, statistics.Completed);
		assertEquals(disabled, statistics.Disabled);
		assertEquals(enabled, statistics.Enabled);
		assertEquals(terminated, statistics.Terminated);
	  }

	  protected internal virtual void createCaseByKey(int numberOfInstances, string key)
	  {
		for (int i = 0; i < numberOfInstances; i++)
		{
		  caseService.createCaseInstanceByKey(key);
		}
	  }

	  protected internal virtual CaseDefinition CaseDefinition
	  {
		  get
		  {
			return repositoryService.createCaseDefinitionQuery().singleResult();
		  }
	  }

	  protected internal virtual CaseDefinition getCaseDefinition(string key)
	  {
		return repositoryService.createCaseDefinitionQuery().caseDefinitionKey(key).singleResult();
	  }

	  protected internal virtual IList<CaseExecution> getCaseExecutionsByActivity(string activityId)
	  {
		return caseService.createCaseExecutionQuery().activityId(activityId).list();
	  }

	  protected internal virtual void disableByActivity(string activityId)
	  {
		IList<CaseExecution> executions = getCaseExecutionsByActivity(activityId);
		foreach (CaseExecution caseExecution in executions)
		{
		  caseService.disableCaseExecution(caseExecution.Id);
		}
	  }

	  protected internal virtual void reenableByActivity(string activityId)
	  {
		IList<CaseExecution> executions = getCaseExecutionsByActivity(activityId);
		foreach (CaseExecution caseExecution in executions)
		{
		  caseService.reenableCaseExecution(caseExecution.Id);
		}
	  }

	  protected internal virtual void manuallyStartByActivity(string activityId)
	  {
		IList<CaseExecution> executions = getCaseExecutionsByActivity(activityId);
		foreach (CaseExecution caseExecution in executions)
		{
		  caseService.manuallyStartCaseExecution(caseExecution.Id);
		}
	  }

	  protected internal virtual void completeByActivity(string activityId)
	  {
		IList<CaseExecution> executions = getCaseExecutionsByActivity(activityId);
		foreach (CaseExecution caseExecution in executions)
		{
		  caseService.completeCaseExecution(caseExecution.Id);
		}
	  }

	  protected internal virtual void terminateByActivity(string activityId)
	  {
		IList<CaseExecution> executions = getCaseExecutionsByActivity(activityId);
		foreach (CaseExecution caseExecution in executions)
		{
		  caseService.terminateCaseExecution(caseExecution.Id);
		}
	  }

	}

}