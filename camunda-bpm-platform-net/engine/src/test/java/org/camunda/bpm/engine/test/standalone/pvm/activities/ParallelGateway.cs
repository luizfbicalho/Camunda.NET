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
namespace org.camunda.bpm.engine.test.standalone.pvm.activities
{

	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;
	using PvmActivity = org.camunda.bpm.engine.impl.pvm.PvmActivity;
	using PvmTransition = org.camunda.bpm.engine.impl.pvm.PvmTransition;
	using ActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityBehavior;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using Logger = org.slf4j.Logger;

	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class ParallelGateway : ActivityBehavior
	{

	private static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;

	  public virtual void execute(ActivityExecution execution)
	  {
		PvmActivity activity = execution.Activity;

		IList<PvmTransition> outgoingTransitions = execution.Activity.OutgoingTransitions;

		execution.inactivate();

		IList<ActivityExecution> joinedExecutions = execution.findInactiveConcurrentExecutions(activity);

		int nbrOfExecutionsToJoin = execution.Activity.IncomingTransitions.Count;
		int nbrOfExecutionsJoined = joinedExecutions.Count;

		if (nbrOfExecutionsJoined == nbrOfExecutionsToJoin)
		{
		  LOG.debug("parallel gateway '" + activity.Id + "' activates: " + nbrOfExecutionsJoined + " of " + nbrOfExecutionsToJoin + " joined");
		  execution.leaveActivityViaTransitions(outgoingTransitions, joinedExecutions);

		}
		else
		{
		  LOG.debug("parallel gateway '" + activity.Id + "' does not activate: " + nbrOfExecutionsJoined + " of " + nbrOfExecutionsToJoin + " joined");
		}
	  }
	}

}