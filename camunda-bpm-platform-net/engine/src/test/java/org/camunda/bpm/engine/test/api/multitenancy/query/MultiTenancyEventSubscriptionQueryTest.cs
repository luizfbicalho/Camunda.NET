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
namespace org.camunda.bpm.engine.test.api.multitenancy.query
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using EventSubscriptionQuery = org.camunda.bpm.engine.runtime.EventSubscriptionQuery;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	public class MultiTenancyEventSubscriptionQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal override void setUp()
	  {
		BpmnModelInstance process = Bpmn.createExecutableProcess("testProcess").startEvent().message("start").userTask().endEvent().done();

		deployment(process);
		deploymentForTenant(TENANT_ONE, process);
		deploymentForTenant(TENANT_TWO, process);

		// the deployed process definition contains a message start event
		// - so a message event subscription is created on deployment.
	  }

	  public virtual void testQueryNoTenantIdSet()
	  {
		EventSubscriptionQuery query = runtimeService.createEventSubscriptionQuery();

		assertThat(query.count(), @is(3L));
	  }

	  public virtual void testQueryByTenantId()
	  {
		EventSubscriptionQuery query = runtimeService.createEventSubscriptionQuery().tenantIdIn(TENANT_ONE);

		assertThat(query.count(), @is(1L));

		query = runtimeService.createEventSubscriptionQuery().tenantIdIn(TENANT_TWO);

		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryByTenantIds()
	  {
		EventSubscriptionQuery query = runtimeService.createEventSubscriptionQuery().tenantIdIn(TENANT_ONE, TENANT_TWO);

		assertThat(query.count(), @is(2L));
	  }

	  public virtual void testQueryBySubscriptionsWithoutTenantId()
	  {
		EventSubscriptionQuery query = runtimeService.createEventSubscriptionQuery().withoutTenantId();

		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryByTenantIdsIncludeSubscriptionsWithoutTenantId()
	  {
		EventSubscriptionQuery query = runtimeService.createEventSubscriptionQuery().tenantIdIn(TENANT_ONE).includeEventSubscriptionsWithoutTenantId();

		assertThat(query.count(), @is(2L));

		query = runtimeService.createEventSubscriptionQuery().tenantIdIn(TENANT_TWO).includeEventSubscriptionsWithoutTenantId();

		assertThat(query.count(), @is(2L));

		query = runtimeService.createEventSubscriptionQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).includeEventSubscriptionsWithoutTenantId();

		assertThat(query.count(), @is(3L));
	  }

	  public virtual void testQueryByNonExistingTenantId()
	  {
		EventSubscriptionQuery query = runtimeService.createEventSubscriptionQuery().tenantIdIn("nonExisting");

		assertThat(query.count(), @is(0L));
	  }

	  public virtual void testFailQueryByTenantIdNull()
	  {
		try
		{
		  runtimeService.createEventSubscriptionQuery().tenantIdIn((string) null);

		  fail("expected exception");
		}
		catch (NullValueException)
		{
		}
	  }

	  public virtual void testQuerySortingAsc()
	  {
		// exclude subscriptions without tenant id because of database-specific ordering
		IList<EventSubscription> eventSubscriptions = runtimeService.createEventSubscriptionQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).orderByTenantId().asc().list();

		assertThat(eventSubscriptions.Count, @is(2));
		assertThat(eventSubscriptions[0].TenantId, @is(TENANT_ONE));
		assertThat(eventSubscriptions[1].TenantId, @is(TENANT_TWO));
	  }

	  public virtual void testQuerySortingDesc()
	  {
		// exclude subscriptions without tenant id because of database-specific ordering
		IList<EventSubscription> eventSubscriptions = runtimeService.createEventSubscriptionQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).orderByTenantId().desc().list();

		assertThat(eventSubscriptions.Count, @is(2));
		assertThat(eventSubscriptions[0].TenantId, @is(TENANT_TWO));
		assertThat(eventSubscriptions[1].TenantId, @is(TENANT_ONE));
	  }

	  public virtual void testQueryNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		EventSubscriptionQuery query = runtimeService.createEventSubscriptionQuery();
		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		EventSubscriptionQuery query = runtimeService.createEventSubscriptionQuery();

		assertThat(query.count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
		assertThat(query.tenantIdIn(TENANT_ONE, TENANT_TWO).includeEventSubscriptionsWithoutTenantId().count(), @is(2L));
	  }

	  public virtual void testQueryAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE, TENANT_TWO));

		EventSubscriptionQuery query = runtimeService.createEventSubscriptionQuery();

		assertThat(query.count(), @is(3L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(1L));
	  }

	  public virtual void testQueryDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		EventSubscriptionQuery query = runtimeService.createEventSubscriptionQuery();
		assertThat(query.count(), @is(3L));
	  }

	}

}