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
namespace org.camunda.bpm.engine.test.bpmn.@event.message
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using DeploymentWithDefinitions = org.camunda.bpm.engine.repository.DeploymentWithDefinitions;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	using AssertionFailedError = junit.framework.AssertionFailedError;

	public class MessageStartEventSubscriptionTest
	{
		private bool InstanceFieldsInitialized = false;

		public MessageStartEventSubscriptionTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  private const string SINGLE_MESSAGE_START_EVENT_XML = "org/camunda/bpm/engine/test/bpmn/event/message/MessageStartEventTest.testSingleMessageStartEvent.bpmn20.xml";
	  private const string ONE_TASK_PROCESS = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml";
	  private const string MESSAGE_EVENT_PROCESS = "singleMessageStartEvent";

	  private static readonly BpmnModelInstance MODEL_WITHOUT_MESSAGE = Bpmn.createExecutableProcess(MESSAGE_EVENT_PROCESS).startEvent().userTask().endEvent().done();

	  private static readonly BpmnModelInstance MODEL = Bpmn.createExecutableProcess("another").startEvent().message("anotherMessage").userTask().endEvent().done();

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown= org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  protected internal RepositoryService repositoryService;
	  protected internal RuntimeService runtimeService;
	  protected internal TaskService taskService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void setUp()
	  {
		repositoryService = engineRule.RepositoryService;
		runtimeService = engineRule.RuntimeService;
		taskService = engineRule.TaskService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateProcessVersionCancelsSubscriptions()
	  public virtual void testUpdateProcessVersionCancelsSubscriptions()
	  {
		testRule.deploy(SINGLE_MESSAGE_START_EVENT_XML);
		IList<EventSubscription> eventSubscriptions = runtimeService.createEventSubscriptionQuery().list();
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().list();

		assertEquals(1, eventSubscriptions.Count);
		assertEquals(1, processDefinitions.Count);

		// when
		testRule.deploy(SINGLE_MESSAGE_START_EVENT_XML);

		// then
		IList<EventSubscription> newEventSubscriptions = runtimeService.createEventSubscriptionQuery().list();
		IList<ProcessDefinition> newProcessDefinitions = repositoryService.createProcessDefinitionQuery().list();

		assertEquals(1, newEventSubscriptions.Count);
		assertEquals(2, newProcessDefinitions.Count);
		foreach (ProcessDefinition processDefinition in newProcessDefinitions)
		{
		  if (processDefinition.Version == 1)
		  {
			foreach (EventSubscription subscription in newEventSubscriptions)
			{
			  EventSubscriptionEntity subscriptionEntity = (EventSubscriptionEntity) subscription;
			  assertFalse(subscriptionEntity.Configuration.Equals(processDefinition.Id));
			}
		  }
		  else
		  {
			foreach (EventSubscription subscription in newEventSubscriptions)
			{
			  EventSubscriptionEntity subscriptionEntity = (EventSubscriptionEntity) subscription;
			  assertTrue(subscriptionEntity.Configuration.Equals(processDefinition.Id));
			}
		  }
		}
//JAVA TO C# CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
//ORIGINAL LINE: assertFalse(eventSubscriptions.equals(newEventSubscriptions));
		assertFalse(eventSubscriptions.SequenceEqual(newEventSubscriptions));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEventSubscriptionAfterDeleteLatestProcessVersion()
	  public virtual void testEventSubscriptionAfterDeleteLatestProcessVersion()
	  {
		// given a deployed process
		testRule.deploy(SINGLE_MESSAGE_START_EVENT_XML);
		ProcessDefinition processDefinitionV1 = repositoryService.createProcessDefinitionQuery().singleResult();
		assertNotNull(processDefinitionV1);

		// deploy second version of the process
		string deploymentId = testRule.deploy(SINGLE_MESSAGE_START_EVENT_XML).Id;

		// when
		repositoryService.deleteDeployment(deploymentId, true);

		// then
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionKey(MESSAGE_EVENT_PROCESS).singleResult();
		assertEquals(processDefinitionV1.Id, processDefinition.Id);

		EventSubscriptionEntity eventSubscription = (EventSubscriptionEntity) runtimeService.createEventSubscriptionQuery().singleResult();
		assertNotNull(eventSubscription);
		assertEquals(processDefinitionV1.Id, eventSubscription.Configuration);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStartInstanceAfterDeleteLatestProcessVersionByIds()
	  public virtual void testStartInstanceAfterDeleteLatestProcessVersionByIds()
	  {
		// given a deployed process
		testRule.deploy(SINGLE_MESSAGE_START_EVENT_XML);
		// deploy second version of the process
		DeploymentWithDefinitions deployment = testRule.deploy(SINGLE_MESSAGE_START_EVENT_XML);
		ProcessDefinition processDefinition = deployment.DeployedProcessDefinitions[0];

		// delete it
		repositoryService.deleteProcessDefinitions().byIds(processDefinition.Id).delete();

		// when
		ProcessInstance processInstance = runtimeService.startProcessInstanceByMessage("newInvoiceMessage");

		// then
		assertFalse(processInstance.Ended);
		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		taskService.complete(task.Id);

		ProcessInstance completedInstance = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance.Id).singleResult();

		if (completedInstance != null)
		{
		  throw new AssertionFailedError("Expected finished process instance '" + completedInstance + "' but it was still in the db");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStartInstanceAfterDeleteLatestProcessVersion()
	  public virtual void testStartInstanceAfterDeleteLatestProcessVersion()
	  {
		// given a deployed process
		testRule.deploy(SINGLE_MESSAGE_START_EVENT_XML);
		// deploy second version of the process
		string deploymentId = testRule.deploy(SINGLE_MESSAGE_START_EVENT_XML).Id;
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeploymentQuery().deploymentId(deploymentId).singleResult();

		// delete it
		repositoryService.deleteDeployment(deployment.Id, true);

		// when
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("singleMessageStartEvent");

		assertFalse(processInstance.Ended);

		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		taskService.complete(task.Id);

		ProcessInstance completedInstance = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance.Id).singleResult();

		if (completedInstance != null)
		{
		  throw new AssertionFailedError("Expected finished process instance '" + completedInstance + "' but it was still in the db");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVersionWithoutConditionAfterDeleteLatestProcessVersionWithCondition()
	  public virtual void testVersionWithoutConditionAfterDeleteLatestProcessVersionWithCondition()
	  {
		// given a process
		testRule.deploy(MODEL_WITHOUT_MESSAGE);

		// deploy second version of the process
		string deploymentId = testRule.deploy(SINGLE_MESSAGE_START_EVENT_XML).Id;
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeploymentQuery().deploymentId(deploymentId).singleResult();

		// delete it
		repositoryService.deleteDeployment(deployment.Id, true);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("No subscriptions were found during evaluation of the conditional start events.");

		// when
		runtimeService.createConditionEvaluation().setVariable("foo", 1).evaluateStartConditions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubscriptionsWhenDeletingProcessDefinitionsInOneTransactionByKeys()
	  public virtual void testSubscriptionsWhenDeletingProcessDefinitionsInOneTransactionByKeys()
	  {
		// given three versions of the process
		testRule.deploy(SINGLE_MESSAGE_START_EVENT_XML);
		testRule.deploy(SINGLE_MESSAGE_START_EVENT_XML);
		testRule.deploy(SINGLE_MESSAGE_START_EVENT_XML);

		// when
		repositoryService.deleteProcessDefinitions().byKey(MESSAGE_EVENT_PROCESS).delete();

		// then
		assertEquals(0, runtimeService.createEventSubscriptionQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubscriptionsWhenDeletingGroupsProcessDefinitionsByIds()
	  public virtual void testSubscriptionsWhenDeletingGroupsProcessDefinitionsByIds()
	  {
		// given
		string processDefId11 = deployProcess(SINGLE_MESSAGE_START_EVENT_XML);
		string processDefId12 = deployProcess(SINGLE_MESSAGE_START_EVENT_XML);
		string processDefId13 = testRule.deployAndGetDefinition(MODEL_WITHOUT_MESSAGE).Id;

		string processDefId21 = deployModel(MODEL);
		string processDefId22 = deployModel(MODEL);
		string processDefId23 = deployModel(MODEL);

		string processDefId31 = deployProcess(ONE_TASK_PROCESS);
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unused") String processDefId32 = deployProcess(ONE_TASK_PROCESS);
		string processDefId32 = deployProcess(ONE_TASK_PROCESS);

		// assume
		assertEquals(1, runtimeService.createEventSubscriptionQuery().count());

		// when
		repositoryService.deleteProcessDefinitions().byIds(processDefId21,processDefId23,processDefId13, processDefId12,processDefId31).delete();

		// then
		IList<EventSubscription> list = runtimeService.createEventSubscriptionQuery().list();
		assertEquals(2, list.Count);
		foreach (EventSubscription eventSubscription in list)
		{
		  EventSubscriptionEntity eventSubscriptionEntity = (EventSubscriptionEntity) eventSubscription;
		  if (!eventSubscriptionEntity.Configuration.Equals(processDefId11) && !eventSubscriptionEntity.Configuration.Equals(processDefId22))
		  {
			fail("This process definition '" + eventSubscriptionEntity.Configuration + "' and the respective event subscription should not exist.");
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubscriptionsWhenDeletingProcessDefinitionsInOneTransactionByIdOrdered()
	  public virtual void testSubscriptionsWhenDeletingProcessDefinitionsInOneTransactionByIdOrdered()
	  {
		// given
		string definitionId1 = deployProcess(SINGLE_MESSAGE_START_EVENT_XML);
		string definitionId2 = deployProcess(SINGLE_MESSAGE_START_EVENT_XML);
		string definitionId3 = deployProcess(SINGLE_MESSAGE_START_EVENT_XML);

		// when
		repositoryService.deleteProcessDefinitions().byIds(definitionId1, definitionId2, definitionId3).delete();

		// then
		assertEquals(0, runtimeService.createEventSubscriptionQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubscriptionsWhenDeletingProcessDefinitionsInOneTransactionByIdReverseOrder()
	  public virtual void testSubscriptionsWhenDeletingProcessDefinitionsInOneTransactionByIdReverseOrder()
	  {
		// given
		string definitionId1 = deployProcess(SINGLE_MESSAGE_START_EVENT_XML);
		string definitionId2 = deployProcess(SINGLE_MESSAGE_START_EVENT_XML);
		string definitionId3 = deployProcess(SINGLE_MESSAGE_START_EVENT_XML);

		// when
		repositoryService.deleteProcessDefinitions().byIds(definitionId3, definitionId2, definitionId1).delete();

		// then
		assertEquals(0, runtimeService.createEventSubscriptionQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMixedSubscriptionsWhenDeletingProcessDefinitionsInOneTransactionById1()
	  public virtual void testMixedSubscriptionsWhenDeletingProcessDefinitionsInOneTransactionById1()
	  {
		// given first version without condition
		string definitionId1 = deployModel(MODEL_WITHOUT_MESSAGE);
		string definitionId2 = deployProcess(SINGLE_MESSAGE_START_EVENT_XML);
		string definitionId3 = deployProcess(SINGLE_MESSAGE_START_EVENT_XML);

		// when
		repositoryService.deleteProcessDefinitions().byIds(definitionId1, definitionId2, definitionId3).delete();

		// then
		assertEquals(0, runtimeService.createEventSubscriptionQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMixedSubscriptionsWhenDeletingProcessDefinitionsInOneTransactionById2()
	  public virtual void testMixedSubscriptionsWhenDeletingProcessDefinitionsInOneTransactionById2()
	  {
		// given second version without condition
		string definitionId1 = deployProcess(SINGLE_MESSAGE_START_EVENT_XML);
		string definitionId2 = deployModel(MODEL_WITHOUT_MESSAGE);
		string definitionId3 = deployProcess(SINGLE_MESSAGE_START_EVENT_XML);

		// when
		repositoryService.deleteProcessDefinitions().byIds(definitionId1, definitionId2, definitionId3).delete();

		// then
		assertEquals(0, runtimeService.createEventSubscriptionQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMixedSubscriptionsWhenDeletingProcessDefinitionsInOneTransactionById3()
	  public virtual void testMixedSubscriptionsWhenDeletingProcessDefinitionsInOneTransactionById3()
	  {
		// given third version without condition
		string definitionId1 = deployProcess(SINGLE_MESSAGE_START_EVENT_XML);
		string definitionId2 = deployProcess(SINGLE_MESSAGE_START_EVENT_XML);
		string definitionId3 = deployModel(MODEL_WITHOUT_MESSAGE);

		// when
		repositoryService.deleteProcessDefinitions().byIds(definitionId1, definitionId2, definitionId3).delete();

		// then
		assertEquals(0, runtimeService.createEventSubscriptionQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMixedSubscriptionsWhenDeletingTwoProcessDefinitionsInOneTransaction1()
	  public virtual void testMixedSubscriptionsWhenDeletingTwoProcessDefinitionsInOneTransaction1()
	  {
		// given first version without condition
		string definitionId1 = deployModel(MODEL_WITHOUT_MESSAGE);
		string definitionId2 = deployProcess(SINGLE_MESSAGE_START_EVENT_XML);
		string definitionId3 = deployProcess(SINGLE_MESSAGE_START_EVENT_XML);

		// when
		repositoryService.deleteProcessDefinitions().byIds(definitionId2, definitionId3).delete();

		// then
		assertEquals(0, runtimeService.createEventSubscriptionQuery().count());
		assertEquals(definitionId1, repositoryService.createProcessDefinitionQuery().singleResult().Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMixedSubscriptionsWhenDeletingTwoProcessDefinitionsInOneTransaction2()
	  public virtual void testMixedSubscriptionsWhenDeletingTwoProcessDefinitionsInOneTransaction2()
	  {
		// given second version without condition
		string definitionId1 = deployProcess(SINGLE_MESSAGE_START_EVENT_XML);
		string definitionId2 = deployModel(MODEL_WITHOUT_MESSAGE);
		string definitionId3 = deployProcess(SINGLE_MESSAGE_START_EVENT_XML);

		// when
		repositoryService.deleteProcessDefinitions().byIds(definitionId2, definitionId3).delete();

		// then
		assertEquals(1, runtimeService.createEventSubscriptionQuery().count());
		assertEquals(definitionId1, ((EventSubscriptionEntity) runtimeService.createEventSubscriptionQuery().singleResult()).Configuration);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMixedSubscriptionsWhenDeletingTwoProcessDefinitionsInOneTransaction3()
	  public virtual void testMixedSubscriptionsWhenDeletingTwoProcessDefinitionsInOneTransaction3()
	  {
		// given third version without condition
		string definitionId1 = deployProcess(SINGLE_MESSAGE_START_EVENT_XML);
		string definitionId2 = deployProcess(SINGLE_MESSAGE_START_EVENT_XML);
		string definitionId3 = deployModel(MODEL_WITHOUT_MESSAGE);

		// when
		repositoryService.deleteProcessDefinitions().byIds(definitionId2, definitionId3).delete();

		// then
		assertEquals(1, runtimeService.createEventSubscriptionQuery().count());
		assertEquals(definitionId1, ((EventSubscriptionEntity) runtimeService.createEventSubscriptionQuery().singleResult()).Configuration);
	  }

	  /// <summary>
	  /// Tests the case, when no new subscription is needed, as it is not the latest version, that is being deleted.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteNotLatestVersion()
	  public virtual void testDeleteNotLatestVersion()
	  {
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unused") String definitionId1 = deployProcess(SINGLE_MESSAGE_START_EVENT_XML);
		  string definitionId1 = deployProcess(SINGLE_MESSAGE_START_EVENT_XML);
		string definitionId2 = deployProcess(SINGLE_MESSAGE_START_EVENT_XML);
		string definitionId3 = deployProcess(SINGLE_MESSAGE_START_EVENT_XML);

		// when
		repositoryService.deleteProcessDefinitions().byIds(definitionId2).delete();

		// then
		assertEquals(1, runtimeService.createEventSubscriptionQuery().count());
		assertEquals(definitionId3, ((EventSubscriptionEntity) runtimeService.createEventSubscriptionQuery().singleResult()).Configuration);
	  }

	  /// <summary>
	  /// Tests the case when the previous of the previous version will be needed.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubscribePreviousPreviousVersion()
	  public virtual void testSubscribePreviousPreviousVersion()
	  {

		string definitionId1 = deployProcess(SINGLE_MESSAGE_START_EVENT_XML);
		string definitionId2 = deployProcess(SINGLE_MESSAGE_START_EVENT_XML);
		string definitionId3 = deployProcess(SINGLE_MESSAGE_START_EVENT_XML); //we're deleting version 3, but as version 2 is already deleted, we must subscribe version 1

		// when
		repositoryService.deleteProcessDefinitions().byIds(definitionId2, definitionId3).delete();

		// then
		assertEquals(1, runtimeService.createEventSubscriptionQuery().count());
		assertEquals(definitionId1, ((EventSubscriptionEntity) runtimeService.createEventSubscriptionQuery().singleResult()).Configuration);
	  }

	  protected internal virtual string deployProcess(string resourcePath)
	  {
		IList<ProcessDefinition> deployedProcessDefinitions = testRule.deploy(resourcePath).DeployedProcessDefinitions;
		assertEquals(1, deployedProcessDefinitions.Count);
		return deployedProcessDefinitions[0].Id;
	  }

	  protected internal virtual string deployModel(BpmnModelInstance model)
	  {
		IList<ProcessDefinition> deployedProcessDefinitions = testRule.deploy(model).DeployedProcessDefinitions;
		assertEquals(1, deployedProcessDefinitions.Count);
		string definitionId2 = deployedProcessDefinitions[0].Id;
		return definitionId2;
	  }
	}

}