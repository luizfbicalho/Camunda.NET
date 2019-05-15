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
namespace org.camunda.bpm.engine.impl.bpmn.behavior
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.DecisionEvaluationUtil.evaluateDecision;

	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using BaseCallableElement = org.camunda.bpm.engine.impl.core.model.BaseCallableElement;
	using AbstractVariableScope = org.camunda.bpm.engine.impl.core.variable.scope.AbstractVariableScope;
	using DecisionResultMapper = org.camunda.bpm.engine.impl.dmn.result.DecisionResultMapper;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;

	/// <summary>
	/// Implementation of a Bpmn BusinessRuleTask executing a DMN Decision.
	/// 
	/// The decision is resolved as a <seealso cref="BaseCallableElement"/>.
	/// 
	/// The decision is executed in the context of the current <seealso cref="VariableScope"/>.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class DmnBusinessRuleTaskActivityBehavior : AbstractBpmnActivityBehavior
	{

	  protected internal readonly BaseCallableElement callableElement;
	  protected internal readonly string resultVariable;
	  protected internal readonly DecisionResultMapper decisionResultMapper;

	  public DmnBusinessRuleTaskActivityBehavior(BaseCallableElement callableElement, string resultVariableName, DecisionResultMapper decisionResultMapper)
	  {
		this.callableElement = callableElement;
		this.resultVariable = resultVariableName;
		this.decisionResultMapper = decisionResultMapper;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(final org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual void execute(ActivityExecution execution)
	  {
		executeWithErrorPropagation(execution, new CallableAnonymousInnerClass(this, execution));
	  }

	  private class CallableAnonymousInnerClass : Callable<Void>
	  {
		  private readonly DmnBusinessRuleTaskActivityBehavior outerInstance;

		  private ActivityExecution execution;

		  public CallableAnonymousInnerClass(DmnBusinessRuleTaskActivityBehavior outerInstance, ActivityExecution execution)
		  {
			  this.outerInstance = outerInstance;
			  this.execution = execution;
		  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			evaluateDecision((AbstractVariableScope) execution, outerInstance.callableElement, outerInstance.resultVariable, outerInstance.decisionResultMapper);
			outerInstance.leave(execution);
			return null;
		  }

	  }

	}

}