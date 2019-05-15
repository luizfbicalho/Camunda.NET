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

	using EventSubscriptionDeclaration = org.camunda.bpm.engine.impl.bpmn.parser.EventSubscriptionDeclaration;
	using EventType = org.camunda.bpm.engine.impl.@event.EventType;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using MigrationInstruction = org.camunda.bpm.engine.migration.MigrationInstruction;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class EventSubscriptionInstanceHandler : MigratingDependentInstanceParseHandler<MigratingActivityInstance, IList<EventSubscriptionEntity>>
	{

	  public static readonly ISet<string> SUPPORTED_EVENT_TYPES = new HashSet<string>(Arrays.asList(EventType.MESSAGE.name(), EventType.SIGNAL.name(), EventType.CONDITONAL.name()));

	  public virtual void handle(MigratingInstanceParseContext parseContext, MigratingActivityInstance owningInstance, IList<EventSubscriptionEntity> elements)
	  {

		IDictionary<string, EventSubscriptionDeclaration> targetDeclarations = getDeclarationsByTriggeringActivity(owningInstance.TargetScope);

		foreach (EventSubscriptionEntity eventSubscription in elements)
		{
		  if (!SupportedEventTypes.Contains(eventSubscription.EventType))
		  {
			// ignore unsupported event subscriptions
			continue;
		  }

		  MigrationInstruction migrationInstruction = parseContext.findSingleMigrationInstruction(eventSubscription.ActivityId);
		  ActivityImpl targetActivity = parseContext.getTargetActivity(migrationInstruction);

		  if (targetActivity != null && owningInstance.migratesTo(targetActivity.EventScope))
		  {
			// the event subscription is migrated
			EventSubscriptionDeclaration targetDeclaration = targetDeclarations.Remove(targetActivity.Id);

			owningInstance.addMigratingDependentInstance(new MigratingEventSubscriptionInstance(eventSubscription, targetActivity, migrationInstruction.UpdateEventTrigger, targetDeclaration));

		  }
		  else
		  {
			// the event subscription will be removed
			owningInstance.addRemovingDependentInstance(new MigratingEventSubscriptionInstance(eventSubscription));

		  }

		  parseContext.consume(eventSubscription);
		}

		if (owningInstance.migrates())
		{
		  addEmergingEventSubscriptions(owningInstance, targetDeclarations);
		}
	  }

	  protected internal virtual ISet<string> SupportedEventTypes
	  {
		  get
		  {
			return SUPPORTED_EVENT_TYPES;
		  }
	  }

	  protected internal virtual IDictionary<string, EventSubscriptionDeclaration> getDeclarationsByTriggeringActivity(ScopeImpl eventScope)
	  {
		IDictionary<string, EventSubscriptionDeclaration> declarations = EventSubscriptionDeclaration.getDeclarationsForScope(eventScope);

		return new Dictionary<string, EventSubscriptionDeclaration>(declarations);
	  }

	  protected internal virtual void addEmergingEventSubscriptions(MigratingActivityInstance owningInstance, IDictionary<string, EventSubscriptionDeclaration> targetDeclarations)
	  {
		foreach (string key in targetDeclarations.Keys)
		{
		  // the event subscription will be created
		  EventSubscriptionDeclaration declaration = targetDeclarations[key];
		  if (!declaration.StartEvent)
		  {
			owningInstance.addEmergingDependentInstance(new MigratingEventSubscriptionInstance(declaration));
		  }
		}
	  }

	}

}