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
namespace org.camunda.bpm.qa.performance.engine.steps
{

	using DecisionService = org.camunda.bpm.engine.DecisionService;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using PerfTestRunContext = org.camunda.bpm.qa.performance.engine.framework.PerfTestRunContext;
	using PerfTestStepBehavior = org.camunda.bpm.qa.performance.engine.framework.PerfTestStepBehavior;

	/// <summary>
	/// Evaluate a decision table using the DecisionService of the engine.
	/// 
	/// @author Philipp Ossler
	/// 
	/// </summary>
	public class EvaluateDecisionTableStep : ProcessEngineAwareStep, PerfTestStepBehavior
	{

	  protected internal readonly string decisionDefinitionKey;
	  protected internal readonly IDictionary<string, object> variables;

	  public EvaluateDecisionTableStep(ProcessEngine engine, string decisionDefinitionKey, IDictionary<string, object> variables) : base(engine)
	  {

		this.decisionDefinitionKey = decisionDefinitionKey;
		this.variables = variables;
	  }

	  public override void execute(PerfTestRunContext context)
	  {
		DecisionService decisionService = processEngine.DecisionService;

		decisionService.evaluateDecisionTableByKey(decisionDefinitionKey, variables);
	  }

	}

}