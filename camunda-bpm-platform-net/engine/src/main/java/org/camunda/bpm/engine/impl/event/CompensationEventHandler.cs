using System;
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
namespace org.camunda.bpm.engine.impl.@event
{

	using CompensationUtil = org.camunda.bpm.engine.impl.bpmn.helper.CompensationUtil;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using CompositeActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.CompositeActivityBehavior;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using PvmAtomicOperation = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class CompensationEventHandler : EventHandler
	{

	  public virtual string EventHandlerType
	  {
		  get
		  {
			return EventType.COMPENSATE.name();
		  }
	  }

	  public virtual void handleEvent(EventSubscriptionEntity eventSubscription, object payload, object localPayload, string businessKey, CommandContext commandContext)
	  {
		eventSubscription.delete();

		string configuration = eventSubscription.Configuration;
		ensureNotNull("Compensating execution not set for compensate event subscription with id " + eventSubscription.Id, "configuration", configuration);

		ExecutionEntity compensatingExecution = commandContext.ExecutionManager.findExecutionById(configuration);

		ActivityImpl compensationHandler = eventSubscription.Activity;

		// activate execution
		compensatingExecution.Active = true;

		if (compensatingExecution.getActivity().ActivityBehavior is CompositeActivityBehavior)
		{
		  compensatingExecution.Parent.ActivityInstanceId = compensatingExecution.ActivityInstanceId;
		}

		if (compensationHandler.Scope && !compensationHandler.CompensationHandler)
		{
		  // descend into scope:
		  IList<EventSubscriptionEntity> eventsForThisScope = compensatingExecution.CompensateEventSubscriptions;
		  CompensationUtil.throwCompensationEvent(eventsForThisScope, compensatingExecution, false);

		}
		else
		{
		  try
		  {


			if (compensationHandler.SubProcessScope && compensationHandler.TriggeredByEvent)
			{
			  compensatingExecution.executeActivity(compensationHandler);
			}
			else
			{
			  // since we already have a scope execution, we don't need to create another one
			  // for a simple scoped compensation handler
			  compensatingExecution.setActivity(compensationHandler);
			  compensatingExecution.performOperation(org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.ACTIVITY_START);
			}


		  }
		  catch (Exception e)
		  {
			throw new ProcessEngineException("Error while handling compensation event " + eventSubscription, e);
		  }
		}
	  }

	}

}