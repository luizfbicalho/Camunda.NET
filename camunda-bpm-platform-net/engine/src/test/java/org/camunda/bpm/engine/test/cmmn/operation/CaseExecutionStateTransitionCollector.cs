using System.Collections.Generic;
using System.Text;

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
namespace org.camunda.bpm.engine.test.cmmn.operation
{

	using CaseExecutionListener = org.camunda.bpm.engine.@delegate.CaseExecutionListener;
	using DelegateCaseExecution = org.camunda.bpm.engine.@delegate.DelegateCaseExecution;
	using CaseExecutionState = org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState;
	using CmmnExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnExecution;
	using TestLogger = org.camunda.bpm.engine.impl.test.TestLogger;
	using Logger = org.slf4j.Logger;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseExecutionStateTransitionCollector : CaseExecutionListener
	{

	  private static readonly Logger LOG = TestLogger.TEST_LOGGER.Logger;

	  public IList<string> stateTransitions = new List<string>();

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.camunda.bpm.engine.delegate.DelegateCaseExecution planItem) throws Exception
	  public virtual void notify(DelegateCaseExecution planItem)
	  {
		CmmnExecution execution = (CmmnExecution) planItem;

		string activityId = execution.EventSource.Id;

		CaseExecutionState previousState = execution.PreviousState;
		string previousStateName = "()";
		if (!previousState.Equals(org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.NEW))
		{
		  previousStateName = previousState.ToString();
		}

		CaseExecutionState newState = execution.CurrentState;

		string stateTransition = previousStateName + " --" + execution.EventName + "(" + activityId + ")--> " + newState;

		LOG.debug("collecting state transition: " + stateTransition);

		stateTransitions.Add(stateTransition);
	  }

	  public override string ToString()
	  {
		StringBuilder text = new StringBuilder();
		foreach (string @event in stateTransitions)
		{
		  text.Append(@event);
		  text.Append("\n");
		}
		return text.ToString();

	  }

	}

}