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
namespace org.camunda.bpm.engine.impl.migration.instance.parser
{
	using CompensationUtil = org.camunda.bpm.engine.impl.bpmn.helper.CompensationUtil;

	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ReferenceWalker = org.camunda.bpm.engine.impl.tree.ReferenceWalker;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;

	/// <summary>
	/// Ensures that event subscriptions are visited in a top-down fashion, i.e.
	/// for a compensation handler in a scope that has an event scope execution, it is guaranteed
	/// that first the scope subscription is visited, and then the compensation handler
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class CompensationEventSubscriptionWalker : ReferenceWalker<EventSubscriptionEntity>
	{

	  public CompensationEventSubscriptionWalker(ICollection<MigratingActivityInstance> collection) : base(collectCompensationEventSubscriptions(collection))
	  {
	  }

	  protected internal static IList<EventSubscriptionEntity> collectCompensationEventSubscriptions(ICollection<MigratingActivityInstance> activityInstances)
	  {
		IList<EventSubscriptionEntity> eventSubscriptions = new List<EventSubscriptionEntity>();
		foreach (MigratingActivityInstance activityInstance in activityInstances)
		{
		  if (activityInstance.SourceScope.Scope)
		  {
			ExecutionEntity scopeExecution = activityInstance.resolveRepresentativeExecution();
			((IList<EventSubscriptionEntity>)eventSubscriptions).AddRange(scopeExecution.CompensateEventSubscriptions);
		  }
		}
		return eventSubscriptions;
	  }

	  protected internal override ICollection<EventSubscriptionEntity> nextElements()
	  {
		EventSubscriptionEntity eventSubscriptionEntity = CurrentElement;
		ExecutionEntity compensatingExecution = CompensationUtil.getCompensatingExecution(eventSubscriptionEntity);
		if (compensatingExecution != null)
		{
		  return compensatingExecution.CompensateEventSubscriptions;
		}
		else
		{
		  return Collections.emptyList();
		}
	  }

	}

}