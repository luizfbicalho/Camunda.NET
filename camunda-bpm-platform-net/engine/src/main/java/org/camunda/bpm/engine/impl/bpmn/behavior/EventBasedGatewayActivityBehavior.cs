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
	using ConditionalEventDefinition = org.camunda.bpm.engine.impl.bpmn.parser.ConditionalEventDefinition;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityBehavior;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;

	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class EventBasedGatewayActivityBehavior : FlowNodeActivityBehavior
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public virtual void execute(ActivityExecution execution)
	  {
		// If conditional events exist after the event based gateway they should be evaluated.
		// If a condition is satisfied the event based gateway should be left,
		// otherwise the event based gateway is a wait state
		ActivityImpl eventBasedGateway = (ActivityImpl) execution.Activity;
		foreach (ActivityImpl act in eventBasedGateway.EventActivities)
		{
		  ActivityBehavior activityBehavior = act.ActivityBehavior;
		  if (activityBehavior is ConditionalEventBehavior)
		  {
			ConditionalEventBehavior conditionalEventBehavior = (ConditionalEventBehavior) activityBehavior;
			ConditionalEventDefinition conditionalEventDefinition = conditionalEventBehavior.ConditionalEventDefinition;
			if (conditionalEventDefinition.tryEvaluate(execution))
			{
			  ((ExecutionEntity) execution).executeEventHandlerActivity(conditionalEventDefinition.ConditionalActivity);
			  return;
			}
		  }
		}
	  }
	}

}