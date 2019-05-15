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
namespace org.camunda.bpm.engine.test.cmmn.listener
{

	using DelegateCaseExecution = org.camunda.bpm.engine.@delegate.DelegateCaseExecution;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[Serializable]
	public class MyCaseExecutionListener
	{

	  private const long serialVersionUID = 1L;

	  public virtual void notify(DelegateCaseExecution caseExecution, string @event)
	  {
		string eventCounterName = @event + "EventCounter";

		int? eventCounter = (int?) caseExecution.getVariable(eventCounterName);

		if (eventCounter == null)
		{
		  eventCounter = 0;
		}

		int? counter = (int?) caseExecution.getVariable("eventCounter");

		if (counter == null)
		{
		  counter = 0;
		}

		caseExecution.setVariable(@event, true);
		caseExecution.setVariable(eventCounterName, eventCounter + 1);
		caseExecution.setVariable("eventCounter", counter + 1);
		caseExecution.setVariable(@event + "OnCaseExecutionId", caseExecution.Id);

	  }

	}

}