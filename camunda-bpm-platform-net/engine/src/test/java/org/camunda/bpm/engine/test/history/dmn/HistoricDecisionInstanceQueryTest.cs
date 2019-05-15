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
namespace org.camunda.bpm.engine.test.history.dmn
{
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricDecisionInstanceQuery = org.camunda.bpm.engine.history.HistoricDecisionInstanceQuery;
	using NativeHistoricDecisionInstanceQuery = org.camunda.bpm.engine.history.NativeHistoricDecisionInstanceQuery;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using DecisionDefinitionQuery = org.camunda.bpm.engine.repository.DecisionDefinitionQuery;
	using DecisionRequirementsDefinition = org.camunda.bpm.engine.repository.DecisionRequirementsDefinition;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using DateTime = org.joda.time.DateTime;
	using Test = org.junit.Test;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

	/// <summary>
	/// @author Philipp Ossler
	/// @author Ingo Richtsmeier
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoricDecisionInstanceQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal const string DECISION_CASE = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.caseWithDecisionTask.cmmn";
	  protected internal const string DECISION_PROCESS = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.processWithBusinessRuleTask.bpmn20.xml";
	  protected internal const string DECISION_PROCESS_WITH_UNDERSCORE = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.processWithBusinessRuleTask_.bpmn20.xml";

	  protected internal const string DECISION_SINGLE_OUTPUT_DMN = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.decisionSingleOutput.dmn11.xml";
	  protected internal const string DECISION_SINGLE_OUTPUT_DMN_WITH_UNDERSCORE = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.decisionSingleOutput_.dmn11.xml";
	  protected internal const string DECISION_NO_INPUT_DMN = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.noInput.dmn11.xml";

	  protected internal const string DRG_DMN = "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml";

	  protected internal const string DECISION_DEFINITION_KEY = "testDecision";
	  protected internal const string DISH_DECISION = "dish-decision";

	  [Deployment(resources : { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testQueryIncludeInputsForNonExistingDecision()
	  {
		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().includeInputs();
		assertThat(query.singleResult(), @is(nullValue()));

		startProcessInstanceAndEvaluateDecision();

		assertThat(query.decisionInstanceId("nonExisting").singleResult(), @is(nullValue()));
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testQueryIncludeOutputs()
	  {

		startProcessInstanceAndEvaluateDecision();

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		try
		{
		  query.singleResult().Outputs;
		  fail("expected exception: output not fetched");
		}
		catch (ProcessEngineException)
		{
		  // should throw exception if output is not fetched
		}

		assertThat(query.includeOutputs().singleResult().Outputs.size(), @is(1));
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testQueryIncludeOutputsForNonExistingDecision()
	  {
		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().includeOutputs();
		assertThat(query.singleResult(), @is(nullValue()));

		startProcessInstanceAndEvaluateDecision();

		assertThat(query.decisionInstanceId("nonExisting").singleResult(), @is(nullValue()));
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_NO_INPUT_DMN })]
	  public virtual void testQueryIncludeInputsNoInput()
	  {

		startProcessInstanceAndEvaluateDecision();

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		assertThat(query.includeInputs().singleResult().Inputs.size(), @is(0));
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_NO_INPUT_DMN })]
	  public virtual void testQueryIncludeOutputsNoInput()
	  {

		startProcessInstanceAndEvaluateDecision();

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		assertThat(query.includeOutputs().singleResult().Outputs.size(), @is(0));
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testQueryPaging()
	  {

		startProcessInstanceAndEvaluateDecision();
		startProcessInstanceAndEvaluateDecision();

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		assertThat(query.listPage(0, 2).size(), @is(2));
		assertThat(query.listPage(1, 1).size(), @is(1));
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testQuerySortByEvaluationTime()
	  {

		startProcessInstanceAndEvaluateDecision();
		waitASignificantAmountOfTime();
		startProcessInstanceAndEvaluateDecision();

		IList<HistoricDecisionInstance> orderAsc = historyService.createHistoricDecisionInstanceQuery().orderByEvaluationTime().asc().list();
		assertThat(orderAsc[0].EvaluationTime < orderAsc[1].EvaluationTime, @is(true));

		IList<HistoricDecisionInstance> orderDesc = historyService.createHistoricDecisionInstanceQuery().orderByEvaluationTime().desc().list();
		assertThat(orderDesc[0].EvaluationTime > orderDesc[1].EvaluationTime, @is(true));
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testQueryByDecisionInstanceId()
	  {
		ProcessInstance pi1 = startProcessInstanceAndEvaluateDecision();
		ProcessInstance pi2 = startProcessInstanceAndEvaluateDecision();

		string decisionInstanceId1 = historyService.createHistoricDecisionInstanceQuery().processInstanceId(pi1.Id).singleResult().Id;
		string decisionInstanceId2 = historyService.createHistoricDecisionInstanceQuery().processInstanceId(pi2.Id).singleResult().Id;

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		assertThat(query.decisionInstanceId(decisionInstanceId1).count(), @is(1L));
		assertThat(query.decisionInstanceId(decisionInstanceId2).count(), @is(1L));
		assertThat(query.decisionInstanceId("unknown").count(), @is(0L));
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testQueryByDecisionInstanceIds()
	  {
		ProcessInstance pi1 = startProcessInstanceAndEvaluateDecision();
		ProcessInstance pi2 = startProcessInstanceAndEvaluateDecision();

		string decisionInstanceId1 = historyService.createHistoricDecisionInstanceQuery().processInstanceId(pi1.Id).singleResult().Id;
		string decisionInstanceId2 = historyService.createHistoricDecisionInstanceQuery().processInstanceId(pi2.Id).singleResult().Id;

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		assertThat(query.decisionInstanceIdIn(decisionInstanceId1).count(), @is(1L));
		assertThat(query.decisionInstanceIdIn(decisionInstanceId2).count(), @is(1L));
		assertThat(query.decisionInstanceIdIn(decisionInstanceId1, decisionInstanceId2).count(), @is(2L));
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testQueryByDecisionDefinitionId()
	  {
		string decisionDefinitionId = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(DECISION_DEFINITION_KEY).singleResult().Id;

		startProcessInstanceAndEvaluateDecision();

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		assertThat(query.decisionDefinitionId(decisionDefinitionId).count(), @is(1L));
		assertThat(query.decisionDefinitionId("other id").count(), @is(0L));
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN, DRG_DMN })]
	  public virtual void testQueryByDecisionDefinitionIdIn()
	  {
		//given
		string decisionDefinitionId = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(DECISION_DEFINITION_KEY).singleResult().Id;
		string decisionDefinitionId2 = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(DISH_DECISION).singleResult().Id;

		//when
		startProcessInstanceAndEvaluateDecision();
		decisionService.evaluateDecisionTableByKey(DISH_DECISION).variables(Variables.createVariables().putValue("temperature", 21).putValue("dayType", "Weekend")).evaluate();

		//then
		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		assertThat(query.decisionDefinitionIdIn(decisionDefinitionId, decisionDefinitionId2).count(), @is(2L));
		assertThat(query.decisionDefinitionIdIn("other id", "anotherFake").count(), @is(0L));
	  }

	  [Deployment(resources : {DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN, DRG_DMN})]
	  public virtual void testQueryByInvalidDecisionDefinitionIdIn()
	  {
		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();
		try
		{
		  query.decisionDefinitionIdIn("aFake", null).count();
		  fail("exception expected");
		}
		catch (ProcessEngineException)
		{
		  //expected
		}
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN, DRG_DMN })]
	  public virtual void testQueryByDecisionDefinitionKeyIn()
	  {

		//when
		startProcessInstanceAndEvaluateDecision();
		decisionService.evaluateDecisionTableByKey(DISH_DECISION).variables(Variables.createVariables().putValue("temperature", 21).putValue("dayType", "Weekend")).evaluate();

		//then
		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		assertThat(query.decisionDefinitionKeyIn(DISH_DECISION, DECISION_DEFINITION_KEY).count(), @is(2L));
		assertThat(query.decisionDefinitionKeyIn("other id", "anotherFake").count(), @is(0L));
	  }

	  [Deployment(resources : {DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN, DRG_DMN})]
	  public virtual void testQueryByInvalidDecisionDefinitionKeyIn()
	  {
		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();
		try
		{
		  query.decisionDefinitionKeyIn("aFake", null).count();
		  fail("exception expected");
		}
		catch (ProcessEngineException)
		{
		  //expected
		}
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testQueryByDecisionDefinitionKey()
	  {

		startProcessInstanceAndEvaluateDecision();

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		assertThat(query.decisionDefinitionKey(DECISION_DEFINITION_KEY).count(), @is(1L));
		assertThat(query.decisionDefinitionKey("other key").count(), @is(0L));
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testQueryByDecisionDefinitionName()
	  {

		startProcessInstanceAndEvaluateDecision();

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		assertThat(query.decisionDefinitionName("sample decision").count(), @is(1L));
		assertThat(query.decisionDefinitionName("other name").count(), @is(0L));
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_PROCESS_WITH_UNDERSCORE, DECISION_SINGLE_OUTPUT_DMN, DECISION_SINGLE_OUTPUT_DMN_WITH_UNDERSCORE })]
	  public virtual void testQueryByDecisionDefinitionNameLike()
	  {

		startProcessInstanceAndEvaluateDecision();
		startProcessInstanceAndEvaluateDecisionWithUnderscore();

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		assertThat(query.decisionDefinitionNameLike("%ample dec%").count(), @is(1L));
		assertThat(query.decisionDefinitionNameLike("%ample\\_%").count(), @is(1L));

	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testQueryByInvalidDecisionDefinitionNameLike()
	  {

		startProcessInstanceAndEvaluateDecision();

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		assertThat(query.decisionDefinitionNameLike("%invalid%").count(), @is(0L));

		try
		{
		  query.decisionDefinitionNameLike(null);
		  fail();
		}
		catch (NotValidException)
		{
		  // Expected exception
		}
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testQueryByProcessDefinitionKey()
	  {
		string processDefinitionKey = repositoryService.createProcessDefinitionQuery().singleResult().Key;

		startProcessInstanceAndEvaluateDecision();

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		assertThat(query.processDefinitionKey(processDefinitionKey).count(), @is(1L));
		assertThat(query.processDefinitionKey("other process").count(), @is(0L));
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testQueryByProcessDefinitionId()
	  {
		string processDefinitionId = repositoryService.createProcessDefinitionQuery().singleResult().Id;

		startProcessInstanceAndEvaluateDecision();

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		assertThat(query.processDefinitionId(processDefinitionId).count(), @is(1L));
		assertThat(query.processDefinitionId("other process").count(), @is(0L));
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testQueryByProcessInstanceId()
	  {

		startProcessInstanceAndEvaluateDecision();

		string processInstanceId = runtimeService.createProcessInstanceQuery().singleResult().Id;

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		assertThat(query.processInstanceId(processInstanceId).count(), @is(1L));
		assertThat(query.processInstanceId("other process").count(), @is(0L));
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testQueryByActivityId()
	  {

		startProcessInstanceAndEvaluateDecision();

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		assertThat(query.activityIdIn("task").count(), @is(1L));
		assertThat(query.activityIdIn("other activity").count(), @is(0L));
		assertThat(query.activityIdIn("task", "other activity").count(), @is(1L));
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testQueryByActivityInstanceId()
	  {

		startProcessInstanceAndEvaluateDecision();

		string activityInstanceId = historyService.createHistoricActivityInstanceQuery().activityId("task").singleResult().Id;

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();
		assertThat(query.activityInstanceIdIn(activityInstanceId).count(), @is(1L));
		assertThat(query.activityInstanceIdIn("other activity").count(), @is(0L));
		assertThat(query.activityInstanceIdIn(activityInstanceId, "other activity").count(), @is(1L));
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testQueryByEvaluatedBefore()
	  {
		DateTime beforeEvaluated = new DateTime(1441612000);
		DateTime evaluated = new DateTime(1441613000);
		DateTime afterEvaluated = new DateTime(1441614000);

		ClockUtil.CurrentTime = evaluated;
		startProcessInstanceAndEvaluateDecision();

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();
		assertThat(query.evaluatedBefore(afterEvaluated).count(), @is(1L));
		assertThat(query.evaluatedBefore(evaluated).count(), @is(1L));
		assertThat(query.evaluatedBefore(beforeEvaluated).count(), @is(0L));

		ClockUtil.reset();
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testQueryByEvaluatedAfter()
	  {
		DateTime beforeEvaluated = new DateTime(1441612000);
		DateTime evaluated = new DateTime(1441613000);
		DateTime afterEvaluated = new DateTime(1441614000);

		ClockUtil.CurrentTime = evaluated;
		startProcessInstanceAndEvaluateDecision();

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();
		assertThat(query.evaluatedAfter(beforeEvaluated).count(), @is(1L));
		assertThat(query.evaluatedAfter(evaluated).count(), @is(1L));
		assertThat(query.evaluatedAfter(afterEvaluated).count(), @is(0L));

		ClockUtil.reset();
	  }

	  [Deployment(resources : { DECISION_CASE, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testQueryByCaseDefinitionKey()
	  {
		createCaseInstanceAndEvaluateDecision();

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		assertThat(query.caseDefinitionKey("case").count(), @is(1L));
	  }

	  public virtual void testQueryByInvalidCaseDefinitionKey()
	  {
		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		assertThat(query.caseDefinitionKey("invalid").count(), @is(0L));

		try
		{
		  query.caseDefinitionKey(null);
		  fail("exception expected");
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  [Deployment(resources : { DECISION_CASE, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testQueryByCaseDefinitionId()
	  {
		CaseInstance caseInstance = createCaseInstanceAndEvaluateDecision();

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		assertThat(query.caseDefinitionId(caseInstance.CaseDefinitionId).count(), @is(1L));
	  }

	  public virtual void testQueryByInvalidCaseDefinitionId()
	  {
		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		assertThat(query.caseDefinitionId("invalid").count(), @is(0L));

		try
		{
		  query.caseDefinitionId(null);
		  fail("exception expected");
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  [Deployment(resources : { DECISION_CASE, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testQueryByCaseInstanceId()
	  {
		CaseInstance caseInstance = createCaseInstanceAndEvaluateDecision();

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		assertThat(query.caseInstanceId(caseInstance.Id).count(), @is(1L));
	  }

	  public virtual void testQueryByInvalidCaseInstanceId()
	  {
		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		assertThat(query.caseInstanceId("invalid").count(), @is(0L));

		try
		{
		  query.caseInstanceId(null);
		  fail("exception expected");
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  [Deployment(resources : { DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testQueryByUserId()
	  {
		evaluateDecisionWithAuthenticatedUser("demo");

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		assertThat(query.userId("demo").count(), @is(1L));
	  }

	  [Deployment(resources : { DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testQueryByInvalidUserId()
	  {
		evaluateDecisionWithAuthenticatedUser("demo");

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		assertThat(query.userId("dem1").count(), @is(0L));

		try
		{
		  query.userId(null);
		  fail("exception expected");
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  [Deployment(resources : { DRG_DMN })]
	  public virtual void testQueryByRootDecisionInstanceId()
	  {
		decisionService.evaluateDecisionTableByKey(DISH_DECISION).variables(Variables.createVariables().putValue("temperature", 21).putValue("dayType", "Weekend")).evaluate();

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();
		assertThat(query.count(), @is(3L));

		string rootDecisionInstanceId = query.decisionDefinitionKey(DISH_DECISION).singleResult().Id;
		string requiredDecisionInstanceId1 = query.decisionDefinitionKey("season").singleResult().Id;
		string requiredDecisionInstanceId2 = query.decisionDefinitionKey("guestCount").singleResult().Id;

		query = historyService.createHistoricDecisionInstanceQuery();
		assertThat(query.rootDecisionInstanceId(rootDecisionInstanceId).count(), @is(3L));
		assertThat(query.rootDecisionInstanceId(requiredDecisionInstanceId1).count(), @is(0L));
		assertThat(query.rootDecisionInstanceId(requiredDecisionInstanceId2).count(), @is(0L));
	  }

	  [Deployment(resources : { DRG_DMN })]
	  public virtual void testQueryByRootDecisionInstancesOnly()
	  {
		decisionService.evaluateDecisionTableByKey(DISH_DECISION).variables(Variables.createVariables().putValue("temperature", 21).putValue("dayType", "Weekend")).evaluate();

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		assertThat(query.count(), @is(3L));
		assertThat(query.rootDecisionInstancesOnly().count(), @is(1L));
		assertThat(query.rootDecisionInstancesOnly().singleResult().DecisionDefinitionKey, @is(DISH_DECISION));
	  }

	  [Deployment(resources : { DRG_DMN })]
	  public virtual void testQueryByDecisionRequirementsDefinitionId()
	  {
		decisionService.evaluateDecisionTableByKey(DISH_DECISION).variables(Variables.createVariables().putValue("temperature", 21).putValue("dayType", "Weekend")).evaluate();

		DecisionRequirementsDefinition decisionRequirementsDefinition = repositoryService.createDecisionRequirementsDefinitionQuery().singleResult();

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		assertThat(query.decisionRequirementsDefinitionId("notExisting").count(), @is(0L));
		assertThat(query.decisionRequirementsDefinitionId(decisionRequirementsDefinition.Id).count(), @is(3L));
	  }

	  [Deployment(resources : { DRG_DMN })]
	  public virtual void testQueryByDecisionRequirementsDefinitionKey()
	  {
		decisionService.evaluateDecisionTableByKey(DISH_DECISION).variables(Variables.createVariables().putValue("temperature", 21).putValue("dayType", "Weekend")).evaluate();

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		assertThat(query.decisionRequirementsDefinitionKey("notExisting").count(), @is(0L));
		assertThat(query.decisionRequirementsDefinitionKey("dish").count(), @is(3L));
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testNativeQuery()
	  {

		startProcessInstanceAndEvaluateDecision();

		string tablePrefix = processEngineConfiguration.DatabaseTablePrefix;

		NativeHistoricDecisionInstanceQuery nativeQuery = historyService.createNativeHistoricDecisionInstanceQuery().sql("SELECT * FROM " + tablePrefix + "ACT_HI_DECINST");

		assertThat(nativeQuery.list().size(), @is(1));

		NativeHistoricDecisionInstanceQuery nativeQueryWithParameter = historyService.createNativeHistoricDecisionInstanceQuery().sql("SELECT * FROM " + tablePrefix + "ACT_HI_DECINST H WHERE H.DEC_DEF_KEY_ = #{decisionDefinitionKey}");

		assertThat(nativeQueryWithParameter.parameter("decisionDefinitionKey", DECISION_DEFINITION_KEY).list().size(), @is(1));
		assertThat(nativeQueryWithParameter.parameter("decisionDefinitionKey", "other decision").list().size(), @is(0));
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testNativeCountQuery()
	  {

		startProcessInstanceAndEvaluateDecision();

		string tablePrefix = processEngineConfiguration.DatabaseTablePrefix;

		NativeHistoricDecisionInstanceQuery nativeQuery = historyService.createNativeHistoricDecisionInstanceQuery().sql("SELECT count(*) FROM " + tablePrefix + "ACT_HI_DECINST");

		assertThat(nativeQuery.count(), @is(1L));
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testNativeQueryPaging()
	  {

		startProcessInstanceAndEvaluateDecision();
		startProcessInstanceAndEvaluateDecision();

		string tablePrefix = processEngineConfiguration.DatabaseTablePrefix;

		NativeHistoricDecisionInstanceQuery nativeQuery = historyService.createNativeHistoricDecisionInstanceQuery().sql("SELECT * FROM " + tablePrefix + "ACT_HI_DECINST");

		assertThat(nativeQuery.listPage(0, 2).size(), @is(2));
		assertThat(nativeQuery.listPage(1, 1).size(), @is(1));
	  }

	  protected internal virtual ProcessInstance startProcessInstanceAndEvaluateDecision()
	  {
		return runtimeService.startProcessInstanceByKey("testProcess", Variables);
	  }

	  protected internal virtual ProcessInstance startProcessInstanceAndEvaluateDecisionWithUnderscore()
	  {
		return runtimeService.startProcessInstanceByKey("testProcess_", Variables);
	  }

	  protected internal virtual CaseInstance createCaseInstanceAndEvaluateDecision()
	  {
		return caseService.withCaseDefinitionByKey("case").setVariables(Variables).create();
	  }

	  protected internal virtual void evaluateDecisionWithAuthenticatedUser(string userId)
	  {
		identityService.AuthenticatedUserId = userId;
		VariableMap variables = Variables.putValue("input1", "test");
		decisionService.evaluateDecisionTableByKey(DECISION_DEFINITION_KEY, variables);
	  }

	  protected internal virtual VariableMap Variables
	  {
		  get
		  {
			VariableMap variables = Variables.createVariables();
			variables.put("input1", "test");
			return variables;
		  }
	  }

	  /// <summary>
	  /// Use between two rule evaluations to ensure the expected order by evaluation time.
	  /// </summary>
	  protected internal virtual void waitASignificantAmountOfTime()
	  {
		DateTime now = new DateTime(ClockUtil.CurrentTime);
		ClockUtil.CurrentTime = now.plusSeconds(10).toDate();
	  }

	}

}