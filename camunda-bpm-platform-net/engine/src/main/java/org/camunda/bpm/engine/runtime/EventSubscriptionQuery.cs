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
namespace org.camunda.bpm.engine.runtime
{
	using Query = org.camunda.bpm.engine.query.Query;

	/// <summary>
	/// Allows querying of event subscriptions.
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public interface EventSubscriptionQuery : Query<EventSubscriptionQuery, EventSubscription>
	{

	  /// <summary>
	  /// Only select subscriptions with the given id. * </summary>
	  EventSubscriptionQuery eventSubscriptionId(string id);

	  /// <summary>
	  /// Only select subscriptions for events with the given name. * </summary>
	  EventSubscriptionQuery eventName(string eventName);

	  /// <summary>
	  /// Only select subscriptions for events with the given type. "message" selects message event subscriptions,
	  /// "signal" selects signal event subscriptions, "compensation" selects compensation event subscriptions,
	  /// "conditional" selects conditional event subscriptions.*
	  /// </summary>
	  EventSubscriptionQuery eventType(string eventType);

	  /// <summary>
	  /// Only select subscriptions that belong to an execution with the given id. * </summary>
	  EventSubscriptionQuery executionId(string executionId);

	  /// <summary>
	  /// Only select subscriptions that belong to a process instance with the given id. * </summary>
	  EventSubscriptionQuery processInstanceId(string processInstanceId);

	  /// <summary>
	  /// Only select subscriptions that belong to an activity with the given id. * </summary>
	  EventSubscriptionQuery activityId(string activityId);

	  /// <summary>
	  /// Only select subscriptions that belong to one of the given tenant ids. </summary>
	  EventSubscriptionQuery tenantIdIn(params string[] tenantIds);

	  /// <summary>
	  /// Only select subscriptions which have no tenant id. </summary>
	  EventSubscriptionQuery withoutTenantId();

	  /// <summary>
	  /// Select subscriptions which have no tenant id. Can be used in combination
	  /// with <seealso cref="#tenantIdIn(String...)"/>.
	  /// </summary>
	  EventSubscriptionQuery includeEventSubscriptionsWithoutTenantId();

	  /// <summary>
	  /// Order by event subscription creation date (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  EventSubscriptionQuery orderByCreated();

	  /// <summary>
	  /// Order by tenant id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// Note that the ordering of subscriptions without tenant id is database-specific.
	  /// </summary>
	  EventSubscriptionQuery orderByTenantId();

	}
}