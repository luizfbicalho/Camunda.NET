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
namespace org.camunda.bpm.engine.impl.dmn.invocation
{
	using DmnDecision = org.camunda.bpm.dmn.engine.DmnDecision;
	using DmnDecisionResult = org.camunda.bpm.dmn.engine.DmnDecisionResult;
	using DmnEngine = org.camunda.bpm.dmn.engine.DmnEngine;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DelegateInvocation = org.camunda.bpm.engine.impl.@delegate.DelegateInvocation;
	using DecisionDefinitionEntity = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionDefinitionEntity;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using VariableContext = org.camunda.bpm.engine.variable.context.VariableContext;

	/// <summary>
	/// <seealso cref="DelegateInvocation"/> invoking a <seealso cref="DecisionDefinition"/>
	/// in a given <seealso cref="VariableContext"/>.
	/// 
	/// The DmnEngine instance is resolved from the Context.
	/// 
	/// The invocation result is a <seealso cref="DmnDecisionResult"/>.
	/// 
	/// The target of the invocation is the <seealso cref="DecisionDefinition"/>.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class DecisionInvocation : DelegateInvocation
	{

	  protected internal DecisionDefinition decisionDefinition;
	  protected internal VariableContext variableContext;

	  public DecisionInvocation(DecisionDefinition decisionDefinition, VariableContext variableContext) : base(null, (DecisionDefinitionEntity) decisionDefinition)
	  {
		this.decisionDefinition = decisionDefinition;
		this.variableContext = variableContext;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void invoke() throws Exception
	  protected internal override void invoke()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.dmn.engine.DmnEngine dmnEngine = org.camunda.bpm.engine.impl.context.Context.getProcessEngineConfiguration().getDmnEngine();
		DmnEngine dmnEngine = Context.ProcessEngineConfiguration.DmnEngine;

		invocationResult = dmnEngine.evaluateDecision((DmnDecision) decisionDefinition, variableContext);
	  }

	  public override DmnDecisionResult InvocationResult
	  {
		  get
		  {
			return (DmnDecisionResult) base.InvocationResult;
		  }
	  }

	  public virtual DecisionDefinition DecisionDefinition
	  {
		  get
		  {
			return decisionDefinition;
		  }
	  }

	}

}