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
namespace org.camunda.bpm.engine.impl.cmmn.behavior
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.DecisionEvaluationUtil.evaluateDecision;

	using CmmnActivityExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnActivityExecution;
	using AbstractVariableScope = org.camunda.bpm.engine.impl.core.variable.scope.AbstractVariableScope;
	using DecisionResultMapper = org.camunda.bpm.engine.impl.dmn.result.DecisionResultMapper;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class DmnDecisionTaskActivityBehavior : DecisionTaskActivityBehavior
	{

	  protected internal DecisionResultMapper decisionResultMapper;

	  protected internal override void performStart(CmmnActivityExecution execution)
	  {
		try
		{
		  evaluateDecision((AbstractVariableScope) execution, callableElement, resultVariable, decisionResultMapper);

		  if (execution.Active)
		  {
			execution.complete();
		  }
		}
		catch (Exception e)
		{
		  throw e;
		}
		catch (Exception e)
		{
		  throw LOG.decisionDefinitionEvaluationFailed(execution, e);
		}
	  }

	  public virtual DecisionResultMapper DecisionTableResultMapper
	  {
		  get
		  {
			return decisionResultMapper;
		  }
		  set
		  {
			this.decisionResultMapper = value;
		  }
	  }


	}

}