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
namespace org.camunda.bpm.qa.performance.engine.dmn
{

	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using ProcessEnginePerformanceTestCase = org.camunda.bpm.qa.performance.engine.junit.ProcessEnginePerformanceTestCase;
	using StartProcessInstanceStep = org.camunda.bpm.qa.performance.engine.steps.StartProcessInstanceStep;
	using Test = org.junit.Test;

	/// <summary>
	/// Execute process definitions which contains a DMN business rule task.
	/// 
	/// @author Philipp Ossler
	/// 
	/// </summary>
	public class DmnBusinessRuleTaskTest : ProcessEnginePerformanceTestCase
	{

	  private const string BPMN = "org/camunda/bpm/qa/performance/engine/dmn/DmnBusinessRuleTaskTest.businessRuleTask.bpmn";
	  private const string DMN_DIR = "org/camunda/bpm/qa/performance/engine/dmn/";

	  private const string PROCESS_DEFINITION_KEY = "Process";

	  // decision definition keys
	  private const string TWO_RULES = "twoRules";
	  private const string FIVE_RULES = "fiveRules";
	  private const string TEN_RULES = "tenRules";
	  private const string ONE_HUNDRED_RULES = "oneHundredRules";

	  // 1.0 => 100% - all rules of the decision table will match
	  // 0.5 => 50% - half of the rules of the decision table will match
	  private const double NUMBER_OF_MATCHING_RULES = 1.0;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { BPMN, DMN_DIR + "DmnEnginePerformanceTest.twoRules.dmn" }) public void twoRules()
	  [Deployment(resources : { BPMN, DMN_DIR + "DmnEnginePerformanceTest.twoRules.dmn" })]
	  public virtual void twoRules()
	  {
		performanceTest().step(startProcessInstanceStep(TWO_RULES)).run();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { BPMN, DMN_DIR + "DmnEnginePerformanceTest.fiveRules.dmn" }) public void fiveRules()
	  [Deployment(resources : { BPMN, DMN_DIR + "DmnEnginePerformanceTest.fiveRules.dmn" })]
	  public virtual void fiveRules()
	  {
		performanceTest().step(startProcessInstanceStep(FIVE_RULES)).run();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { BPMN, DMN_DIR + "DmnEnginePerformanceTest.tenRules.dmn" }) public void tenRules()
	  [Deployment(resources : { BPMN, DMN_DIR + "DmnEnginePerformanceTest.tenRules.dmn" })]
	  public virtual void tenRules()
	  {
		performanceTest().step(startProcessInstanceStep(TEN_RULES)).run();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { BPMN, DMN_DIR + "DmnEnginePerformanceTest.oneHundredRules.dmn" }) public void onehundredRules()
	  [Deployment(resources : { BPMN, DMN_DIR + "DmnEnginePerformanceTest.oneHundredRules.dmn" })]
	  public virtual void onehundredRules()
	  {
		performanceTest().step(startProcessInstanceStep(ONE_HUNDRED_RULES)).run();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void noop()
	  public virtual void noop()
	  {
		performanceTest().step(new StartProcessInstanceStep(engine, PROCESS_DEFINITION_KEY)).run();
	  }

	  private StartProcessInstanceStep startProcessInstanceStep(string decisionDefinitionKey)
	  {
		IDictionary<string, object> variables = createVariables(decisionDefinitionKey);

		return new StartProcessInstanceStep(engine, PROCESS_DEFINITION_KEY, variables);
	  }

	  private IDictionary<string, object> createVariables(string decisionDefinitionKey)
	  {
		return Variables.createVariables().putValue("decisionKey", decisionDefinitionKey).putValue("input", NUMBER_OF_MATCHING_RULES);
	  }

	}

}