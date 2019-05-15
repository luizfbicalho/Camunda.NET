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
namespace org.camunda.bpm.engine.test.bpmn.@event.signal
{
	using EventType = org.camunda.bpm.engine.impl.@event.EventType;

	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;

	/// <summary>
	/// @author Philipp Ossler
	/// </summary>
	public class SignalEventDeploymentTest : PluggableProcessEngineTestCase
	{

	  private const string SIGNAL_START_EVENT_PROCESS = "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTest.signalStartEvent.bpmn20.xml";
	  private const string SIGNAL_START_EVENT_PROCESS_NEW_VERSION = "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTest.signalStartEvent_v2.bpmn20.xml";

	  public virtual void testCreateEventSubscriptionOnDeployment()
	  {
		deploymentId = repositoryService.createDeployment().addClasspathResource(SIGNAL_START_EVENT_PROCESS).deploy().Id;

		EventSubscription eventSubscription = runtimeService.createEventSubscriptionQuery().singleResult();
		assertNotNull(eventSubscription);

		assertEquals(EventType.SIGNAL.name(), eventSubscription.EventType);
		assertEquals("alert", eventSubscription.EventName);
		assertEquals("start", eventSubscription.ActivityId);
	  }

	  public virtual void testUpdateEventSubscriptionOnDeployment()
	  {
		deploymentId = repositoryService.createDeployment().addClasspathResource(SIGNAL_START_EVENT_PROCESS).deploy().Id;

		EventSubscription eventSubscription = runtimeService.createEventSubscriptionQuery().eventType("signal").singleResult();
		assertNotNull(eventSubscription);
		assertEquals("alert", eventSubscription.EventName);

		// deploy a new version of the process with different signal name
		string newDeploymentId = repositoryService.createDeployment().addClasspathResource(SIGNAL_START_EVENT_PROCESS_NEW_VERSION).deploy().Id;

		ProcessDefinition newProcessDefinition = repositoryService.createProcessDefinitionQuery().latestVersion().singleResult();
		assertEquals(2, newProcessDefinition.Version);

		IList<EventSubscription> newEventSubscriptions = runtimeService.createEventSubscriptionQuery().eventType("signal").list();
		// only one event subscription for the new version of the process definition
		assertEquals(1, newEventSubscriptions.Count);

		EventSubscriptionEntity newEventSubscription = (EventSubscriptionEntity) newEventSubscriptions.GetEnumerator().next();
		assertEquals(newProcessDefinition.Id, newEventSubscription.Configuration);
		assertEquals("abort", newEventSubscription.EventName);

		// clean db
		repositoryService.deleteDeployment(newDeploymentId);
	  }

	  public virtual void testAsyncSignalStartEventDeleteDeploymentWhileAsync()
	  {
		// given a deployment
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTest.signalStartEvent.bpmn20.xml").addClasspathResource("org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.throwAlertSignalAsync.bpmn20.xml").deploy();

		// and an active job for asynchronously triggering a signal start event
		runtimeService.startProcessInstanceByKey("throwSignalAsync");

		// then deleting the deployment succeeds
		repositoryService.deleteDeployment(deployment.Id, true);

		assertEquals(0, repositoryService.createDeploymentQuery().count());

		int historyLevel = processEngineConfiguration.HistoryLevel.Id;
		if (historyLevel >= org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_FULL.Id)
		{
		  // and there are no job logs left
		  assertEquals(0, historyService.createHistoricJobLogQuery().count());
		}

	  }

	}

}