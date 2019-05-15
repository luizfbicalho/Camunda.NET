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
	using CmmnActivityExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnActivityExecution;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class EventListenerActivityBehavior : EventListenerOrMilestoneActivityBehavior
	{

	  protected internal new static readonly CmmnBehaviorLogger LOG = ProcessEngineLogger.CMNN_BEHAVIOR_LOGGER;

	  public virtual void created(CmmnActivityExecution execution)
	  {
		// TODO: implement this:

		// (1) in case of a UserEventListener there is nothing to do!

		// (2) in case of TimerEventListener we have to check
		// whether the timer must be triggered, when a transition
		// on another plan item or case file item happens!
	  }

	  protected internal override string TypeName
	  {
		  get
		  {
			return "event listener";
		  }
	  }

	  protected internal override bool isAtLeastOneEntryCriterionSatisfied(CmmnActivityExecution execution)
	  {
		return false;
	  }

	  public virtual void fireEntryCriteria(CmmnActivityExecution execution)
	  {
		throw LOG.criteriaNotAllowedForEventListenerException("entry", execution.Id);
	  }

	  public virtual void repeat(CmmnActivityExecution execution)
	  {
		// It is not possible to repeat a event listener
	  }

	  protected internal override bool evaluateRepetitionRule(CmmnActivityExecution execution)
	  {
		// It is not possible to define a repetition rule on an event listener
		return false;
	  }

	}

}