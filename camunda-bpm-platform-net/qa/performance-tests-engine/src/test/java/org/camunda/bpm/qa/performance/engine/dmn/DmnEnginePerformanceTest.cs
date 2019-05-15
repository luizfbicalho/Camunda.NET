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
	using EvaluateDecisionTableStep = org.camunda.bpm.qa.performance.engine.steps.EvaluateDecisionTableStep;
	using Test = org.junit.Test;

	/// <summary>
	/// Evaluate DMN decision tables via decision service.
	/// 
	/// @author Philipp Ossler
	/// </summary>
	public class DmnEnginePerformanceTest : ProcessEnginePerformanceTestCase
	{

	  // 1.0 => 100% - all rules of the decision table will match
	  // 0.5 => 50% - half of the rules of the decision table will match
	  private const double NUMBER_OF_MATCHING_RULES = 1.0;

	  // decision ids
	  private const string TWO_RULES = "twoRules";
	  private const string FIVE_RULES = "fiveRules";
	  private const string TEN_RULES = "tenRules";
	  private const string ONE_HUNDRED_RULES = "oneHundredRules";

	  private const string TWO_RULES_TWO_INPUTS = "twoRulesTwoInputs";
	  private const string FIVE_RULES_TWO_INPUTS = "fiveRulesTwoInputs";
	  private const string TEN_RULES_TWO_INPUTS = "tenRulesTwoInputs";
	  private const string ONE_HUNDRED_RULES_TWO_INPUTS = "oneHundredRulesTwoInputs";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void twoRules()
	  public virtual void twoRules()
	  {
		performanceTest().step(evaluateDecisionTableStep(TWO_RULES)).run();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void fiveRules()
	  public virtual void fiveRules()
	  {
		performanceTest().step(evaluateDecisionTableStep(FIVE_RULES)).run();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void tenRules()
	  public virtual void tenRules()
	  {
		performanceTest().step(evaluateDecisionTableStep(TEN_RULES)).run();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void oneHundredRules()
	  public virtual void oneHundredRules()
	  {
		performanceTest().step(evaluateDecisionTableStep(ONE_HUNDRED_RULES)).run();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void twoRulesTwoInputs()
	  public virtual void twoRulesTwoInputs()
	  {
		performanceTest().step(evaluateDecisionTableStep(TWO_RULES_TWO_INPUTS)).run();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void fiveRulesTwoInputs()
	  public virtual void fiveRulesTwoInputs()
	  {
	   performanceTest().step(evaluateDecisionTableStep(FIVE_RULES_TWO_INPUTS)).run();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void tenRulesTwoInputs()
	  public virtual void tenRulesTwoInputs()
	  {
		performanceTest().step(evaluateDecisionTableStep(TEN_RULES_TWO_INPUTS)).run();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void oneHundredRulesTwoInputs()
	  public virtual void oneHundredRulesTwoInputs()
	  {
		performanceTest().step(evaluateDecisionTableStep(ONE_HUNDRED_RULES_TWO_INPUTS)).run();
	  }

	  private EvaluateDecisionTableStep evaluateDecisionTableStep(string decisionKey)
	  {
		IDictionary<string, object> variables = createVariables();

		return new EvaluateDecisionTableStep(engine, decisionKey, variables);
	  }

	  private IDictionary<string, object> createVariables()
	  {
		return Variables.createVariables().putValue("input", NUMBER_OF_MATCHING_RULES);
	  }

	}

}