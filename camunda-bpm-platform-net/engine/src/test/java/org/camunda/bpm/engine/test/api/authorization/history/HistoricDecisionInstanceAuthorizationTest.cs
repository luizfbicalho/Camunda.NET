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
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricDecisionInstanceQuery = org.camunda.bpm.engine.history.HistoricDecisionInstanceQuery;
	using CleanableHistoricDecisionInstanceReportResult = org.camunda.bpm.engine.history.CleanableHistoricDecisionInstanceReportResult;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using Variables = org.camunda.bpm.engine.variable.Variables;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.DELETE_HISTORY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ_HISTORY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.DECISION_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	/// <summary>
	/// @author Philipp Ossler
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoricDecisionInstanceAuthorizationTest : AuthorizationTest
	{

	  protected internal const string PROCESS_KEY = "testProcess";
	  protected internal const string DECISION_DEFINITION_KEY = "testDecision";

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setUp() throws Exception
	  public override void setUp()
	  {
		deploymentId = createDeployment(null, "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.processWithBusinessRuleTask.bpmn20.xml", "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.decisionSingleOutput.dmn11.xml").Id;
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
		startProcessInstanceAndEvaluateDecision();

		// when
		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testQueryWithReadPermissionOnDecisionDefinition()
	  {
		// given
		startProcessInstanceAndEvaluateDecision();
		createGrantAuthorization(DECISION_DEFINITION, DECISION_DEFINITION_KEY, userId, READ_HISTORY);

		// when
		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryWithReadPermissionOnAnyDecisionDefinition()
	  {
		// given
		startProcessInstanceAndEvaluateDecision();
		createGrantAuthorization(DECISION_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryWithMultiple()
	  {
		// given
		startProcessInstanceAndEvaluateDecision();
		createGrantAuthorization(DECISION_DEFINITION, ANY, userId, READ_HISTORY);
		createGrantAuthorization(DECISION_DEFINITION, DECISION_DEFINITION_KEY, userId, READ_HISTORY);

		// when
		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testDeleteHistoricDecisionInstanceWithoutAuthorization()
	  {
		// given
		startProcessInstanceAndEvaluateDecision();
		string decisionDefinitionId = selectDecisionDefinitionByKey(DECISION_DEFINITION_KEY).Id;

		try
		{
		  // when
		  historyService.deleteHistoricDecisionInstanceByDefinitionId(decisionDefinitionId);
		  fail("expect authorization exception");
		}
		catch (AuthorizationException e)
		{
		  // then
		  assertThat(e.Message, @is("The user with id 'test' does not have 'DELETE_HISTORY' permission on resource 'testDecision' of type 'DecisionDefinition'."));
		}
	  }

	  public virtual void testDeleteHistoricDecisionInstanceWithDeleteHistoryPermissionOnDecisionDefinition()
	  {
		// given
		startProcessInstanceAndEvaluateDecision();
		createGrantAuthorization(DECISION_DEFINITION, ANY, userId, DELETE_HISTORY);
		string decisionDefinitionId = selectDecisionDefinitionByKey(DECISION_DEFINITION_KEY).Id;


		// when
		historyService.deleteHistoricDecisionInstanceByDefinitionId(decisionDefinitionId);

		// then
		disableAuthorization();
		assertThat(historyService.createHistoricDecisionInstanceQuery().count(), @is(0L));
		enableAuthorization();
	  }

	  public virtual void testDeleteHistoricDecisionInstanceWithDeleteHistoryPermissionOnAnyDecisionDefinition()
	  {
		// given
		startProcessInstanceAndEvaluateDecision();
		createGrantAuthorization(DECISION_DEFINITION, DECISION_DEFINITION_KEY, userId, DELETE_HISTORY);
		string decisionDefinitionId = selectDecisionDefinitionByKey(DECISION_DEFINITION_KEY).Id;

		// when
		historyService.deleteHistoricDecisionInstanceByDefinitionId(decisionDefinitionId);

		// then
		disableAuthorization();
		assertThat(historyService.createHistoricDecisionInstanceQuery().count(), @is(0L));
		enableAuthorization();
	  }

	  public virtual void testDeleteHistoricDecisionInstanceByInstanceIdWithoutAuthorization()
	  {

		// given
		createGrantAuthorization(DECISION_DEFINITION, DECISION_DEFINITION_KEY, userId, READ_HISTORY);
		startProcessInstanceAndEvaluateDecision();

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();
		HistoricDecisionInstance historicDecisionInstance = query.includeInputs().includeOutputs().singleResult();

		try
		{
		  // when
		  historyService.deleteHistoricDecisionInstanceByInstanceId(historicDecisionInstance.Id);
		  fail("expect authorization exception");
		}
		catch (AuthorizationException e)
		{
		  // then
		  assertThat(e.Message, @is("The user with id 'test' does not have 'DELETE_HISTORY' permission on resource 'testDecision' of type 'DecisionDefinition'."));
		}
	  }

	  public virtual void testDeleteHistoricDecisionInstanceByInstanceIdWithDeleteHistoryPermissionOnDecisionDefinition()
	  {

		// given
		createGrantAuthorization(DECISION_DEFINITION, DECISION_DEFINITION_KEY, userId, DELETE_HISTORY, READ_HISTORY);
		startProcessInstanceAndEvaluateDecision();

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();
		verifyQueryResults(query, 1);
		HistoricDecisionInstance historicDecisionInstance = query.includeInputs().includeOutputs().singleResult();

		// when
		historyService.deleteHistoricDecisionInstanceByInstanceId(historicDecisionInstance.Id);

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testHistoryCleanupReportWithoutAuthorization()
	  {
		// given
		prepareDecisionInstances(DECISION_DEFINITION_KEY, -6, 5, 10);

		// when
		IList<CleanableHistoricDecisionInstanceReportResult> reportResults = historyService.createCleanableHistoricDecisionInstanceReport().list();

		// then
		assertEquals(0, reportResults.Count);
	  }

	  public virtual void testHistoryCleanupReportWithAuthorization()
	  {
		// given
		prepareDecisionInstances(DECISION_DEFINITION_KEY, -6, 5, 10);

		createGrantAuthorization(DECISION_DEFINITION, DECISION_DEFINITION_KEY, userId, Permissions.READ, Permissions.READ_HISTORY);
		createGrantAuthorizationGroup(DECISION_DEFINITION, DECISION_DEFINITION_KEY, groupId, Permissions.READ, Permissions.READ_HISTORY);

		// when
		IList<CleanableHistoricDecisionInstanceReportResult> reportResults = historyService.createCleanableHistoricDecisionInstanceReport().list();

		// then
		assertEquals(1, reportResults.Count);
		assertEquals(10, reportResults[0].CleanableDecisionInstanceCount);
		assertEquals(10, reportResults[0].FinishedDecisionInstanceCount);
	  }

	  public virtual void testHistoryCleanupReportWithReadPermissionOnly()
	  {
		// given
		prepareDecisionInstances(DECISION_DEFINITION_KEY, -6, 5, 10);

		createGrantAuthorization(DECISION_DEFINITION, DECISION_DEFINITION_KEY, userId, Permissions.READ);

		// when
		IList<CleanableHistoricDecisionInstanceReportResult> reportResults = historyService.createCleanableHistoricDecisionInstanceReport().list();

		// then
		assertEquals(0, reportResults.Count);
	  }

	  public virtual void testHistoryCleanupReportWithReadHistoryPermissionOnly()
	  {
		// given
		prepareDecisionInstances(DECISION_DEFINITION_KEY, -6, 5, 10);

		createGrantAuthorization(DECISION_DEFINITION, DECISION_DEFINITION_KEY, userId, Permissions.READ_HISTORY);

		// when
		IList<CleanableHistoricDecisionInstanceReportResult> reportResults = historyService.createCleanableHistoricDecisionInstanceReport().list();

		// then
		assertEquals(0, reportResults.Count);
	  }

	  protected internal virtual void startProcessInstanceAndEvaluateDecision()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["input1"] = null;
		startProcessInstanceByKey(PROCESS_KEY, variables);
	  }

	  protected internal virtual void prepareDecisionInstances(string key, int daysInThePast, int? historyTimeToLive, int instanceCount)
	  {
		DecisionDefinition decisionDefinition = selectDecisionDefinitionByKey(key);
		disableAuthorization();
		repositoryService.updateDecisionDefinitionHistoryTimeToLive(decisionDefinition.Id, historyTimeToLive);
		enableAuthorization();

		DateTime oldCurrentTime = ClockUtil.CurrentTime;
		ClockUtil.CurrentTime = DateUtils.addDays(oldCurrentTime, daysInThePast);

		IDictionary<string, object> variables = Variables.createVariables().putValue("input1", null);
		for (int i = 0; i < instanceCount; i++)
		{
		  disableAuthorization();
		  decisionService.evaluateDecisionByKey(key).variables(variables).evaluate();
		  enableAuthorization();
		}

		ClockUtil.CurrentTime = oldCurrentTime;
	  }

	}

}