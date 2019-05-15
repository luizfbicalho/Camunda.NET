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
namespace org.camunda.bpm.engine.test.api.runtime
{

	using EventSubscriptionQueryImpl = org.camunda.bpm.engine.impl.EventSubscriptionQueryImpl;
	using EventType = org.camunda.bpm.engine.impl.@event.EventType;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using EventSubscriptionQuery = org.camunda.bpm.engine.runtime.EventSubscriptionQuery;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Assert = org.junit.Assert;


	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class EventSubscriptionQueryTest : PluggableProcessEngineTestCase
	{

	  public virtual void testQueryByEventSubscriptionId()
	  {
		createExampleEventSubscriptions();

		IList<EventSubscription> list = runtimeService.createEventSubscriptionQuery().eventName("messageName2").list();
		assertEquals(1, list.Count);

		EventSubscription eventSubscription = list[0];

		EventSubscriptionQuery query = runtimeService.createEventSubscriptionQuery().eventSubscriptionId(eventSubscription.Id);

		assertEquals(1, query.count());
		assertEquals(1, query.list().size());
		assertNotNull(query.singleResult());

		try
		{
		  runtimeService.createEventSubscriptionQuery().eventSubscriptionId(null).list();
		  fail("Expected ProcessEngineException");
		}
		catch (ProcessEngineException)
		{
		}

		cleanDb();
	  }

	  public virtual void testQueryByEventName()
	  {

		createExampleEventSubscriptions();

		IList<EventSubscription> list = runtimeService.createEventSubscriptionQuery().eventName("messageName").list();
		assertEquals(2, list.Count);

		list = runtimeService.createEventSubscriptionQuery().eventName("messageName2").list();
		assertEquals(1, list.Count);

		try
		{
		  runtimeService.createEventSubscriptionQuery().eventName(null).list();
		  fail("Expected ProcessEngineException");
		}
		catch (ProcessEngineException)
		{
		}

		cleanDb();

	  }

	  public virtual void testQueryByEventType()
	  {

		createExampleEventSubscriptions();

		IList<EventSubscription> list = runtimeService.createEventSubscriptionQuery().eventType("signal").list();
		assertEquals(1, list.Count);

		list = runtimeService.createEventSubscriptionQuery().eventType("message").list();
		assertEquals(2, list.Count);

		try
		{
		  runtimeService.createEventSubscriptionQuery().eventType(null).list();
		  fail("Expected ProcessEngineException");
		}
		catch (ProcessEngineException)
		{
		}

		cleanDb();

	  }

	  public virtual void testQueryByActivityId()
	  {

		createExampleEventSubscriptions();

		IList<EventSubscription> list = runtimeService.createEventSubscriptionQuery().activityId("someOtherActivity").list();
		assertEquals(1, list.Count);

		list = runtimeService.createEventSubscriptionQuery().activityId("someActivity").eventType("message").list();
		assertEquals(2, list.Count);

		try
		{
		  runtimeService.createEventSubscriptionQuery().activityId(null).list();
		  fail("Expected ProcessEngineException");
		}
		catch (ProcessEngineException)
		{
		}

		cleanDb();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testQueryByExecutionId()
	  public virtual void testQueryByExecutionId()
	  {

		// starting two instances:
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("catchSignal");
		runtimeService.startProcessInstanceByKey("catchSignal");

		// test query by process instance id
		EventSubscription subscription = runtimeService.createEventSubscriptionQuery().processInstanceId(processInstance.Id).singleResult();
		assertNotNull(subscription);

		Execution executionWaitingForSignal = runtimeService.createExecutionQuery().activityId("signalEvent").processInstanceId(processInstance.Id).singleResult();

		// test query by execution id
		EventSubscription signalSubscription = runtimeService.createEventSubscriptionQuery().executionId(executionWaitingForSignal.Id).singleResult();
		assertNotNull(signalSubscription);

		assertEquals(signalSubscription, subscription);

		try
		{
		  runtimeService.createEventSubscriptionQuery().executionId(null).list();
		  fail("Expected ProcessEngineException");
		}
		catch (ProcessEngineException)
		{
		}

		cleanDb();

	  }

	  public virtual void testQuerySorting()
	  {
		createExampleEventSubscriptions();
		IList<EventSubscription> eventSubscriptions = runtimeService.createEventSubscriptionQuery().orderByCreated().asc().list();
		Assert.assertEquals(3, eventSubscriptions.Count);

		Assert.assertTrue(eventSubscriptions[0].Created.compareTo(eventSubscriptions[1].Created) < 0);
		Assert.assertTrue(eventSubscriptions[1].Created.compareTo(eventSubscriptions[2].Created) < 0);

		cleanDb();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMultipleEventSubscriptions()
	  public virtual void testMultipleEventSubscriptions()
	  {
		string message = "cancelation-requested";

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		assertTrue(areJobsAvailable());

		long eventSubscriptionCount = runtimeService.createEventSubscriptionQuery().count();
		assertEquals(2, eventSubscriptionCount);

		EventSubscription messageEvent = runtimeService.createEventSubscriptionQuery().eventType("message").singleResult();
		assertEquals(message, messageEvent.EventName);

		EventSubscription compensationEvent = runtimeService.createEventSubscriptionQuery().eventType("compensate").singleResult();
		assertNull(compensationEvent.EventName);

		runtimeService.createMessageCorrelation(message).processInstanceId(processInstance.Id).correlate();

		assertProcessEnded(processInstance.Id);
	  }


	  protected internal virtual void createExampleEventSubscriptions()
	  {
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly EventSubscriptionQueryTest outerInstance;

		  public CommandAnonymousInnerClass(EventSubscriptionQueryTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			DateTime calendar = new GregorianCalendar();


			EventSubscriptionEntity messageEventSubscriptionEntity1 = new EventSubscriptionEntity(EventType.MESSAGE);
			messageEventSubscriptionEntity1.EventName = "messageName";
			messageEventSubscriptionEntity1.ActivityId = "someActivity";
			calendar = new DateTime(2001, 1, 1);
			messageEventSubscriptionEntity1.Created = calendar;
			messageEventSubscriptionEntity1.insert();

			EventSubscriptionEntity messageEventSubscriptionEntity2 = new EventSubscriptionEntity(EventType.MESSAGE);
			messageEventSubscriptionEntity2.EventName = "messageName";
			messageEventSubscriptionEntity2.ActivityId = "someActivity";
			calendar = new DateTime(2000, 1, 1);
			messageEventSubscriptionEntity2.Created = calendar;
			messageEventSubscriptionEntity2.insert();

			EventSubscriptionEntity signalEventSubscriptionEntity3 = new EventSubscriptionEntity(EventType.SIGNAL);
			signalEventSubscriptionEntity3.EventName = "messageName2";
			signalEventSubscriptionEntity3.ActivityId = "someOtherActivity";
			calendar = new DateTime(2002, 1, 1);
			signalEventSubscriptionEntity3.Created = calendar;
			signalEventSubscriptionEntity3.insert();

			return null;
		  }
	  }

	  protected internal virtual void cleanDb()
	  {
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass2(this));

	  }

	  private class CommandAnonymousInnerClass2 : Command<Void>
	  {
		  private readonly EventSubscriptionQueryTest outerInstance;

		  public CommandAnonymousInnerClass2(EventSubscriptionQueryTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.runtime.EventSubscription> subscriptions = new org.camunda.bpm.engine.impl.EventSubscriptionQueryImpl().list();
			IList<EventSubscription> subscriptions = (new EventSubscriptionQueryImpl()).list();
			foreach (EventSubscription eventSubscriptionEntity in subscriptions)
			{
			  ((EventSubscriptionEntity) eventSubscriptionEntity).delete();
			}
			return null;
		  }
	  }


	}

}