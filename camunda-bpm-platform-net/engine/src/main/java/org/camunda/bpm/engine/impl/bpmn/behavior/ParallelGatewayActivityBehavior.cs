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
namespace org.camunda.bpm.engine.impl.bpmn.behavior
{

	using PvmActivity = org.camunda.bpm.engine.impl.pvm.PvmActivity;
	using PvmTransition = org.camunda.bpm.engine.impl.pvm.PvmTransition;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;

	/// <summary>
	/// Implementation of the Parallel Gateway/AND gateway as definined in the BPMN
	/// 2.0 specification.
	/// 
	/// The Parallel Gateway can be used for splitting a path of execution into
	/// multiple paths of executions (AND-split/fork behavior), one for every
	/// outgoing sequence flow.
	/// 
	/// The Parallel Gateway can also be used for merging or joining paths of
	/// execution (AND-join). In this case, on every incoming sequence flow an
	/// execution needs to arrive, before leaving the Parallel Gateway (and
	/// potentially then doing the fork behavior in case of multiple outgoing
	/// sequence flow).
	/// 
	/// Note that there is a slight difference to spec (p. 436): "The parallel
	/// gateway is activated if there is at least one Token on each incoming sequence
	/// flow." We only check the number of incoming tokens to the number of sequenceflow.
	/// So if two tokens would arrive through the same sequence flow, our implementation
	/// would activate the gateway.
	/// 
	/// Note that a Parallel Gateway having one incoming and multiple ougoing
	/// sequence flow, is the same as having multiple outgoing sequence flow on a
	/// given activity. However, a parallel gateway does NOT check conditions on the
	/// outgoing sequence flow.
	/// 
	/// @author Joram Barrez
	/// @author Tom Baeyens
	/// </summary>
	public class ParallelGatewayActivityBehavior : GatewayActivityBehavior
	{

	  protected internal new static readonly BpmnBehaviorLogger LOG = ProcessEngineLogger.BPMN_BEHAVIOR_LOGGER;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public virtual void execute(ActivityExecution execution)
	  {

		// Join
		PvmActivity activity = execution.Activity;
		IList<PvmTransition> outgoingTransitions = execution.Activity.OutgoingTransitions;

		execution.inactivate();
		lockConcurrentRoot(execution);

		IList<ActivityExecution> joinedExecutions = execution.findInactiveConcurrentExecutions(activity);
		int nbrOfExecutionsToJoin = execution.Activity.IncomingTransitions.Count;
		int nbrOfExecutionsJoined = joinedExecutions.Count;

		if (nbrOfExecutionsJoined == nbrOfExecutionsToJoin)
		{

		  // Fork
		  LOG.activityActivation(activity.Id, nbrOfExecutionsJoined, nbrOfExecutionsToJoin);
		  execution.leaveActivityViaTransitions(outgoingTransitions, joinedExecutions);

		}
		else
		{
		  LOG.noActivityActivation(activity.Id, nbrOfExecutionsJoined, nbrOfExecutionsToJoin);
		}
	  }

	}

}