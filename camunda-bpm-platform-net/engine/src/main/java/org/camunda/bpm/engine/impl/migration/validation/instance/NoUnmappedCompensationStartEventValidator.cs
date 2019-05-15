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
namespace org.camunda.bpm.engine.impl.migration.validation.instance
{
	using MigratingCompensationEventSubscriptionInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingCompensationEventSubscriptionInstance;
	using MigratingEventScopeInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingEventScopeInstance;
	using MigratingProcessInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingProcessInstance;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;

	/// <summary>
	/// Subscriptions for compensation start events must be migrated, similar to compensation boundary events.
	/// However, this is not validated by <seealso cref="NoUnmappedLeafInstanceValidator"/> because
	/// the corresponding event scope instance need not be a leaf in the instance tree (the scope itself may contain
	/// event subscriptions).
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class NoUnmappedCompensationStartEventValidator : MigratingCompensationInstanceValidator
	{

	  public virtual void validate(MigratingEventScopeInstance migratingInstance, MigratingProcessInstance migratingProcessInstance, MigratingActivityInstanceValidationReportImpl ancestorInstanceReport)
	  {
		MigratingCompensationEventSubscriptionInstance eventSubscription = migratingInstance.EventSubscription;

		ActivityImpl eventHandlerActivity = (ActivityImpl) eventSubscription.SourceScope;

		// note: compensation event scopes without children are already handled by NoUnmappedLeafInstanceValidator
		if (eventHandlerActivity.TriggeredByEvent && eventSubscription.TargetScope == null && migratingInstance.Children.Count > 0)
		{
		  ancestorInstanceReport.addFailure("Cannot migrate subscription for compensation handler '" + eventSubscription.SourceScope.Id + "'. " + "There is no migration instruction for the compensation start event");
		}
	  }

	  public virtual void validate(MigratingCompensationEventSubscriptionInstance migratingInstance, MigratingProcessInstance migratingProcessInstance, MigratingActivityInstanceValidationReportImpl ancestorInstanceReport)
	  {
		// Compensation start event subscriptions are MigratingEventScopeInstances
		// because they reference an event scope execution
	  }

	}

}