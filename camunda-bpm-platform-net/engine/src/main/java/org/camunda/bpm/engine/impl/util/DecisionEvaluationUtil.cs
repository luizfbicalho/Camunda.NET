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
namespace org.camunda.bpm.engine.impl.util
{
	using DmnDecisionResult = org.camunda.bpm.dmn.engine.DmnDecisionResult;
	using DmnDecisionTableResult = org.camunda.bpm.dmn.engine.DmnDecisionTableResult;
	using DmnDecisionTableResultImpl = org.camunda.bpm.dmn.engine.impl.DmnDecisionTableResultImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using BaseCallableElement = org.camunda.bpm.engine.impl.core.model.BaseCallableElement;
	using AbstractVariableScope = org.camunda.bpm.engine.impl.core.variable.scope.AbstractVariableScope;
	using DecisionInvocation = org.camunda.bpm.engine.impl.dmn.invocation.DecisionInvocation;
	using VariableScopeContext = org.camunda.bpm.engine.impl.dmn.invocation.VariableScopeContext;
	using CollectEntriesDecisionResultMapper = org.camunda.bpm.engine.impl.dmn.result.CollectEntriesDecisionResultMapper;
	using DecisionResultMapper = org.camunda.bpm.engine.impl.dmn.result.DecisionResultMapper;
	using ResultListDecisionTableResultMapper = org.camunda.bpm.engine.impl.dmn.result.ResultListDecisionTableResultMapper;
	using SingleEntryDecisionResultMapper = org.camunda.bpm.engine.impl.dmn.result.SingleEntryDecisionResultMapper;
	using SingleResultDecisionResultMapper = org.camunda.bpm.engine.impl.dmn.result.SingleResultDecisionResultMapper;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using VariableContext = org.camunda.bpm.engine.variable.context.VariableContext;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class DecisionEvaluationUtil
	{

	  public const string DECISION_RESULT_VARIABLE = "decisionResult";

	  public static DecisionResultMapper getDecisionResultMapperForName(string mapDecisionResult)
	  {
		if ("singleEntry".Equals(mapDecisionResult))
		{
		  return new SingleEntryDecisionResultMapper();

		}
		else if ("singleResult".Equals(mapDecisionResult))
		{
		  return new SingleResultDecisionResultMapper();

		}
		else if ("collectEntries".Equals(mapDecisionResult))
		{
		  return new CollectEntriesDecisionResultMapper();

		}
		else if ("resultList".Equals(mapDecisionResult) || string.ReferenceEquals(mapDecisionResult, null))
		{
		  return new ResultListDecisionTableResultMapper();

		}
		else
		{
		  return null;
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void evaluateDecision(org.camunda.bpm.engine.impl.core.variable.scope.AbstractVariableScope execution, org.camunda.bpm.engine.impl.core.model.BaseCallableElement callableElement, String resultVariable, org.camunda.bpm.engine.impl.dmn.result.DecisionResultMapper decisionResultMapper) throws Exception
	  public static void evaluateDecision(AbstractVariableScope execution, BaseCallableElement callableElement, string resultVariable, DecisionResultMapper decisionResultMapper)
	  {

		DecisionDefinition decisionDefinition = resolveDecisionDefinition(callableElement, execution);
		DecisionInvocation invocation = createInvocation(decisionDefinition, execution);

		invoke(invocation);

		DmnDecisionResult result = invocation.InvocationResult;
		if (result != null)
		{
		  execution.setVariableLocalTransient(DECISION_RESULT_VARIABLE, result);

		  if (!string.ReferenceEquals(resultVariable, null) && decisionResultMapper != null)
		  {
			object mappedDecisionResult = decisionResultMapper.mapDecisionResult(result);
			execution.setVariable(resultVariable, mappedDecisionResult);
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.camunda.bpm.dmn.engine.DmnDecisionResult evaluateDecision(org.camunda.bpm.engine.repository.DecisionDefinition decisionDefinition, org.camunda.bpm.engine.variable.VariableMap variables) throws Exception
	  public static DmnDecisionResult evaluateDecision(DecisionDefinition decisionDefinition, VariableMap variables)
	  {
		DecisionInvocation invocation = createInvocation(decisionDefinition, variables);
		invoke(invocation);
		return invocation.InvocationResult;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.camunda.bpm.dmn.engine.DmnDecisionTableResult evaluateDecisionTable(org.camunda.bpm.engine.repository.DecisionDefinition decisionDefinition, org.camunda.bpm.engine.variable.VariableMap variables) throws Exception
	  public static DmnDecisionTableResult evaluateDecisionTable(DecisionDefinition decisionDefinition, VariableMap variables)
	  {
		// doesn't throw an exception if the decision definition is not implemented as decision table
		DmnDecisionResult decisionResult = evaluateDecision(decisionDefinition, variables);
		return DmnDecisionTableResultImpl.wrap(decisionResult);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected static void invoke(org.camunda.bpm.engine.impl.dmn.invocation.DecisionInvocation invocation) throws Exception
	  protected internal static void invoke(DecisionInvocation invocation)
	  {
		Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(invocation);
	  }

	  protected internal static DecisionInvocation createInvocation(DecisionDefinition decisionDefinition, VariableMap variables)
	  {
		return createInvocation(decisionDefinition, variables.asVariableContext());
	  }

	  protected internal static DecisionInvocation createInvocation(DecisionDefinition decisionDefinition, AbstractVariableScope variableScope)
	  {
		return createInvocation(decisionDefinition, VariableScopeContext.wrap(variableScope));
	  }

	  protected internal static DecisionInvocation createInvocation(DecisionDefinition decisionDefinition, VariableContext variableContext)
	  {
		return new DecisionInvocation(decisionDefinition, variableContext);
	  }

	  protected internal static DecisionDefinition resolveDecisionDefinition(BaseCallableElement callableElement, AbstractVariableScope execution)
	  {
		return CallableElementUtil.getDecisionDefinitionToCall(execution, callableElement);
	  }

	}

}