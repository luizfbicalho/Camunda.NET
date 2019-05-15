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
namespace org.camunda.bpm.engine.test.dmn.businessruletask
{
	using DmnDecisionResult = org.camunda.bpm.dmn.engine.DmnDecisionResult;
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using DecisionEvaluationUtil = org.camunda.bpm.engine.impl.util.DecisionEvaluationUtil;

	/// <summary>
	/// @author Philipp Ossler
	/// </summary>
	public class DecisionResultTestListener : ExecutionListener
	{

	  public static DmnDecisionResult decisionResult = null;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void notify(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
	  public virtual void notify(DelegateExecution execution)
	  {
		decisionResult = (DmnDecisionResult) execution.getVariable(DecisionEvaluationUtil.DECISION_RESULT_VARIABLE);
	  }

	  public static DmnDecisionResult DecisionResult
	  {
		  get
		  {
			return decisionResult;
		  }
	  }

	  public static void reset()
	  {
		decisionResult = null;
	  }

	}

}