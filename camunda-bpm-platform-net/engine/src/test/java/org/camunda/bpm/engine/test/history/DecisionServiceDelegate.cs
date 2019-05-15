using System;

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

	using DmnDecisionRuleResult = org.camunda.bpm.dmn.engine.DmnDecisionRuleResult;
	using DmnDecisionTableResult = org.camunda.bpm.dmn.engine.DmnDecisionTableResult;
	using CaseExecutionListener = org.camunda.bpm.engine.@delegate.CaseExecutionListener;
	using DelegateCaseExecution = org.camunda.bpm.engine.@delegate.DelegateCaseExecution;
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;

	[Serializable]
	public class DecisionServiceDelegate : JavaDelegate, CaseExecutionListener
	{

	  private const long serialVersionUID = 1L;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
	  public virtual void execute(DelegateExecution execution)
	  {
		DecisionService decisionService = execution.ProcessEngineServices.DecisionService;
		evaluateDecision(decisionService, execution);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.camunda.bpm.engine.delegate.DelegateCaseExecution caseExecution) throws Exception
	  public virtual void notify(DelegateCaseExecution caseExecution)
	  {
		DecisionService decisionService = caseExecution.ProcessEngineServices.DecisionService;
		evaluateDecision(decisionService, caseExecution);
	  }

	  public virtual bool evaluate(DelegateCaseExecution caseExecution)
	  {
		DecisionService decisionService = caseExecution.ProcessEngineServices.DecisionService;
		DmnDecisionTableResult result = evaluateDecision(decisionService, caseExecution);
		DmnDecisionRuleResult singleResult = result.SingleResult;
		return (bool?) singleResult.SingleEntry.Value;
	  }

	  protected internal virtual DmnDecisionTableResult evaluateDecision(DecisionService decisionService, VariableScope variableScope)
	  {
		return decisionService.evaluateDecisionTableByKey("testDecision", variableScope.Variables);
	  }

	}

}