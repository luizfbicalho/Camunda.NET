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
namespace org.camunda.bpm.qa.upgrade.scenarios.sentry
{
	using CaseService = org.camunda.bpm.engine.CaseService;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using CaseExecutionQuery = org.camunda.bpm.engine.runtime.CaseExecutionQuery;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using Deployment = org.camunda.bpm.engine.test.Deployment;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class SentryScenario
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static String deployOneTaskProcess()
	  public static string deployOneTaskProcess()
	  {
		return "org/camunda/bpm/qa/upgrade/sentry/sentry.cmmn";
	  }

	  [DescribesScenario("triggerTaskEntryCriterion")]
	  public static ScenarioSetup triggerEntryCriterion()
	  {
		return new ScenarioSetupAnonymousInnerClass();
	  }

	  private class ScenarioSetupAnonymousInnerClass : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {
			CaseService caseService = engine.CaseService;
			caseService.createCaseInstanceByKey("case", scenarioName);
		  }
	  }

	  [DescribesScenario("triggerStageEntryCriterion")]
	  public static ScenarioSetup completeStage()
	  {
		return new ScenarioSetupAnonymousInnerClass2();
	  }

	  private class ScenarioSetupAnonymousInnerClass2 : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {
			CaseService caseService = engine.CaseService;
			CaseInstance caseInstance = caseService.createCaseInstanceByKey("case", scenarioName);
			string caseInstanceId = caseInstance.Id;

			CaseExecutionQuery query = caseService.createCaseExecutionQuery().caseInstanceId(caseInstanceId);

			string firstHumanTaskId = query.activityId("PI_HumanTask_1").singleResult().Id;
			caseService.manuallyStartCaseExecution(firstHumanTaskId);
			caseService.completeCaseExecution(firstHumanTaskId);

			string secondHumanTaskId = query.activityId("PI_HumanTask_2").singleResult().Id;
			caseService.manuallyStartCaseExecution(secondHumanTaskId);
		  }
	  }

	  [DescribesScenario("newSentryInstance")]
	  public static ScenarioSetup newSentryInstance()
	  {
		return new ScenarioSetupAnonymousInnerClass3();
	  }

	  private class ScenarioSetupAnonymousInnerClass3 : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {
			CaseService caseService = engine.CaseService;
			CaseInstance caseInstance = caseService.createCaseInstanceByKey("case", scenarioName);
			string caseInstanceId = caseInstance.Id;

			CaseExecutionQuery query = caseService.createCaseExecutionQuery().caseInstanceId(caseInstanceId);

			string firstHumanTaskId = query.activityId("PI_HumanTask_1").singleResult().Id;
			caseService.manuallyStartCaseExecution(firstHumanTaskId);
			caseService.completeCaseExecution(firstHumanTaskId);

			string secondHumanTaskId = query.activityId("PI_HumanTask_2").singleResult().Id;
			caseService.manuallyStartCaseExecution(secondHumanTaskId);
			caseService.completeCaseExecution(secondHumanTaskId);
		  }
	  }

	  [DescribesScenario("completeInstance")]
	  public static ScenarioSetup completeInstance()
	  {
		return new ScenarioSetupAnonymousInnerClass4();
	  }

	  private class ScenarioSetupAnonymousInnerClass4 : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {
			CaseService caseService = engine.CaseService;
			caseService.createCaseInstanceByKey("case", scenarioName);
		  }
	  }

	}

}