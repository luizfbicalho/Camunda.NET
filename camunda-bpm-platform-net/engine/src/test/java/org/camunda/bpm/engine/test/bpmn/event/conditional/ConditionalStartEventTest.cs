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
namespace org.camunda.bpm.engine.test.bpmn.@event.conditional
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;


	using EventType = org.camunda.bpm.engine.impl.@event.EventType;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using DeploymentWithDefinitions = org.camunda.bpm.engine.repository.DeploymentWithDefinitions;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	public class ConditionalStartEventTest
	{
		private bool InstanceFieldsInitialized = false;

		private class ConditionalProcessVarSpecificationAnonymousInnerClass2 : ConditionalProcessVarSpecification
		{
			private readonly ConditionalEventWithSpecificVariableEventTest outerInstance;

			public ConditionalProcessVarSpecificationAnonymousInnerClass2(ConditionalEventWithSpecificVariableEventTest outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public BpmnModelInstance getProcessWithVarName(bool interrupting, string condition)
			{
			  return modify(TASK_MODEL).addSubProcessTo(CONDITIONAL_EVENT_PROCESS_KEY).triggerByEvent().embeddedSubProcess().startEvent().interrupting(interrupting).conditionalEventDefinition(CONDITIONAL_EVENT).condition(condition).camundaVariableName(VARIABLE_NAME).conditionalEventDefinitionDone().userTask().name(TASK_AFTER_CONDITION).endEvent().done();
			}

			public BpmnModelInstance getProcessWithVarNameAndEvents(bool interrupting, string varEvent)
			{
			  return modify(TASK_MODEL).addSubProcessTo(CONDITIONAL_EVENT_PROCESS_KEY).triggerByEvent().embeddedSubProcess().startEvent().interrupting(interrupting).conditionalEventDefinition(CONDITIONAL_EVENT).condition(CONDITION_EXPR).camundaVariableName(VARIABLE_NAME).camundaVariableEvents(varEvent).conditionalEventDefinitionDone().userTask().name(TASK_AFTER_CONDITION).endEvent().done();
			}

			public BpmnModelInstance getProcessWithVarEvents(bool interrupting, string varEvent)
			{
			  return modify(TASK_MODEL).addSubProcessTo(CONDITIONAL_EVENT_PROCESS_KEY).triggerByEvent().embeddedSubProcess().startEvent().interrupting(interrupting).conditionalEventDefinition(CONDITIONAL_EVENT).condition(CONDITION_EXPR).camundaVariableEvents(varEvent).conditionalEventDefinitionDone().userTask().name(TASK_AFTER_CONDITION).endEvent().done();
			}

			public override string ToString()
			{
			  return "ConditionalStartEventWithVarEvents";
			}
		}

		public ConditionalStartEventTest()
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


	  private const string SINGLE_CONDITIONAL_START_EVENT_XML = "org/camunda/bpm/engine/test/bpmn/event/conditional/ConditionalStartEventTest.testSingleConditionalStartEvent.bpmn20.xml";
	  private const string SINGLE_CONDITIONAL_XML = "org/camunda/bpm/engine/test/bpmn/event/conditional/ConditionalStartEventTest.testSingleConditionalStartEvent1.bpmn20.xml";
	  private const string TRUE_CONDITION_START_XML = "org/camunda/bpm/engine/test/bpmn/event/conditional/ConditionalStartEventTest.testStartInstanceWithTrueConditionalStartEvent.bpmn20.xml";
	  private const string TWO_EQUAL_CONDITIONAL_START_EVENT_XML = "org/camunda/bpm/engine/test/bpmn/event/conditional/ConditionalStartEventTest.testTwoEqualConditionalStartEvent.bpmn20.xml";
	  private const string MULTIPLE_CONDITION_XML = "org/camunda/bpm/engine/test/bpmn/event/conditional/ConditionalStartEventTest.testMultipleCondition.bpmn20.xml";
	  private const string START_INSTANCE_WITH_VARIABLE_NAME_XML = "org/camunda/bpm/engine/test/bpmn/event/conditional/ConditionalStartEventTest.testStartInstanceWithVariableName.bpmn20.xml";
	  private const string ONE_TASK_PROCESS = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml";

	  private const string MULTIPLE_CONDITIONS = "multipleConditions";
	  private const string TRUE_CONDITION_PROCESS = "trueConditionProcess";
	  private const string CONDITIONAL_EVENT_PROCESS = "conditionalEventProcess";

	  private static readonly BpmnModelInstance MODEL_WITHOUT_CONDITION = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS).startEvent().userTask().endEvent().done();

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

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void setUp()
	  {
		repositoryService = engineRule.RepositoryService;
		runtimeService = engineRule.RuntimeService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = SINGLE_CONDITIONAL_START_EVENT_XML) public void testDeploymentCreatesSubscriptions()
	  [Deployment(resources : SINGLE_CONDITIONAL_START_EVENT_XML)]
	  public virtual void testDeploymentCreatesSubscriptions()
	  {
		// given a deployed process
		string processDefinitionId = repositoryService.createProcessDefinitionQuery().processDefinitionKey(CONDITIONAL_EVENT_PROCESS).singleResult().Id;

		// when
		IList<EventSubscription> eventSubscriptions = runtimeService.createEventSubscriptionQuery().list();

		// then
		assertEquals(1, eventSubscriptions.Count);
		EventSubscriptionEntity conditionalEventSubscription = (EventSubscriptionEntity) eventSubscriptions[0];
		assertEquals(EventType.CONDITONAL.name(), conditionalEventSubscription.EventType);
		assertEquals(processDefinitionId, conditionalEventSubscription.Configuration);
		assertNull(conditionalEventSubscription.EventName);
		assertNull(conditionalEventSubscription.ExecutionId);
		assertNull(conditionalEventSubscription.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = SINGLE_CONDITIONAL_START_EVENT_XML) public void testUpdateProcessVersionCancelsSubscriptions()
	  [Deployment(resources : SINGLE_CONDITIONAL_START_EVENT_XML)]
	  public virtual void testUpdateProcessVersionCancelsSubscriptions()
	  {
		// given a deployed process
		IList<EventSubscription> eventSubscriptions = runtimeService.createEventSubscriptionQuery().list();
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().list();

		assertEquals(1, eventSubscriptions.Count);
		assertEquals(1, processDefinitions.Count);

		// when
		testRule.deploy(SINGLE_CONDITIONAL_START_EVENT_XML);

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
//ORIGINAL LINE: @Test @Deployment(resources = SINGLE_CONDITIONAL_START_EVENT_XML) public void testEventSubscriptionAfterDeleteLatestProcessVersion()
	  [Deployment(resources : SINGLE_CONDITIONAL_START_EVENT_XML)]
	  public virtual void testEventSubscriptionAfterDeleteLatestProcessVersion()
	  {
		// given a deployed process
		ProcessDefinition processDefinitionV1 = repositoryService.createProcessDefinitionQuery().singleResult();
		assertNotNull(processDefinitionV1);

		// deploy second version of the process
		string deploymentId = testRule.deploy(SINGLE_CONDITIONAL_XML).Id;

		// when
		repositoryService.deleteDeployment(deploymentId, true);

		// then
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionKey(CONDITIONAL_EVENT_PROCESS).singleResult();
		assertEquals(processDefinitionV1.Id, processDefinition.Id);

		EventSubscriptionEntity eventSubscription = (EventSubscriptionEntity) runtimeService.createEventSubscriptionQuery().singleResult();
		assertNotNull(eventSubscription);
		assertEquals(processDefinitionV1.Id, eventSubscription.Configuration);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = SINGLE_CONDITIONAL_START_EVENT_XML) public void testStartInstanceAfterDeleteLatestProcessVersionByIds()
	  [Deployment(resources : SINGLE_CONDITIONAL_START_EVENT_XML)]
	  public virtual void testStartInstanceAfterDeleteLatestProcessVersionByIds()
	  {
		// given a deployed process

		// deploy second version of the process
		DeploymentWithDefinitions deployment = testRule.deploy(SINGLE_CONDITIONAL_XML);
		ProcessDefinition processDefinition = deployment.DeployedProcessDefinitions[0];

		// delete it
		repositoryService.deleteProcessDefinitions().byIds(processDefinition.Id).delete();

		// when
		IList<ProcessInstance> conditionInstances = runtimeService.createConditionEvaluation().setVariable("foo", 1).evaluateStartConditions();

		// then
		assertEquals(1, conditionInstances.Count);
		assertNotNull(conditionInstances[0]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = SINGLE_CONDITIONAL_START_EVENT_XML) public void testStartInstanceAfterDeleteLatestProcessVersion()
	  [Deployment(resources : SINGLE_CONDITIONAL_START_EVENT_XML)]
	  public virtual void testStartInstanceAfterDeleteLatestProcessVersion()
	  {
		// given a deployed process

		// deploy second version of the process
		string deploymentId = testRule.deploy(SINGLE_CONDITIONAL_XML).Id;
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeploymentQuery().deploymentId(deploymentId).singleResult();

		// delete it
		repositoryService.deleteDeployment(deployment.Id, true);

		// when
		IList<ProcessInstance> conditionInstances = runtimeService.createConditionEvaluation().setVariable("foo", 1).evaluateStartConditions();

		// then
		assertEquals(1, conditionInstances.Count);
		assertNotNull(conditionInstances[0]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVersionWithoutConditionAfterDeleteLatestProcessVersionWithCondition()
	  public virtual void testVersionWithoutConditionAfterDeleteLatestProcessVersionWithCondition()
	  {
		// given a process
		testRule.deploy(MODEL_WITHOUT_CONDITION);

		// deploy second version of the process
		string deploymentId = testRule.deploy(SINGLE_CONDITIONAL_XML).Id;
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
		testRule.deploy(SINGLE_CONDITIONAL_XML);
		testRule.deploy(SINGLE_CONDITIONAL_XML);
		testRule.deploy(SINGLE_CONDITIONAL_XML);

		// when
		repositoryService.deleteProcessDefinitions().byKey(CONDITIONAL_EVENT_PROCESS).delete();

		// then
		assertEquals(0, runtimeService.createEventSubscriptionQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubscriptionsWhenDeletingGroupsProcessDefinitionsByIds()
	  public virtual void testSubscriptionsWhenDeletingGroupsProcessDefinitionsByIds()
	  {
		// given
		string processDefId1 = deployProcess(SINGLE_CONDITIONAL_XML);
		string processDefId2 = deployProcess(SINGLE_CONDITIONAL_XML);
		string processDefId3 = deployModel(MODEL_WITHOUT_CONDITION); // with the same process definition key

		string processDefId4 = deployProcess(TRUE_CONDITION_START_XML);
		string processDefIа5 = deployProcess(TRUE_CONDITION_START_XML);
		string processDefId6 = deployProcess(TRUE_CONDITION_START_XML);

		// two versions of a process without conditional start event
		string processDefId7 = deployProcess(ONE_TASK_PROCESS);
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unused") String processDefId8 = deployProcess(ONE_TASK_PROCESS);
		string processDefId8 = deployProcess(ONE_TASK_PROCESS);

		// assume
		assertEquals(1, runtimeService.createEventSubscriptionQuery().count());

		// when
		repositoryService.deleteProcessDefinitions().byIds(processDefId4, processDefId6, processDefId3, processDefId2, processDefId7).delete();

		// then
		IList<EventSubscription> list = runtimeService.createEventSubscriptionQuery().list();
		assertEquals(2, list.Count);
		foreach (EventSubscription eventSubscription in list)
		{
		  EventSubscriptionEntity eventSubscriptionEntity = (EventSubscriptionEntity) eventSubscription;
		  if (!eventSubscriptionEntity.Configuration.Equals(processDefId1) && !eventSubscriptionEntity.Configuration.Equals(processDefIа5))
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
		string definitionId1 = deployProcess(SINGLE_CONDITIONAL_XML);
		string definitionId2 = deployProcess(SINGLE_CONDITIONAL_XML);
		string definitionId3 = deployProcess(SINGLE_CONDITIONAL_XML);

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
		string definitionId1 = deployProcess(SINGLE_CONDITIONAL_XML);
		string definitionId2 = deployProcess(SINGLE_CONDITIONAL_XML);
		string definitionId3 = deployProcess(SINGLE_CONDITIONAL_XML);

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
		string definitionId1 = deployModel(MODEL_WITHOUT_CONDITION);
		string definitionId2 = deployProcess(SINGLE_CONDITIONAL_XML);
		string definitionId3 = deployProcess(SINGLE_CONDITIONAL_XML);

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
		string definitionId1 = deployProcess(SINGLE_CONDITIONAL_XML);
		string definitionId2 = deployModel(MODEL_WITHOUT_CONDITION);
		string definitionId3 = deployProcess(SINGLE_CONDITIONAL_XML);

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
		string definitionId1 = deployProcess(SINGLE_CONDITIONAL_XML);
		string definitionId2 = deployProcess(SINGLE_CONDITIONAL_XML);
		string definitionId3 = deployModel(MODEL_WITHOUT_CONDITION);

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
		string definitionId1 = deployModel(MODEL_WITHOUT_CONDITION);
		string definitionId2 = deployProcess(SINGLE_CONDITIONAL_XML);
		string definitionId3 = deployProcess(SINGLE_CONDITIONAL_XML);

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
		string definitionId1 = deployProcess(SINGLE_CONDITIONAL_XML);
		string definitionId2 = deployModel(MODEL_WITHOUT_CONDITION);
		string definitionId3 = deployProcess(SINGLE_CONDITIONAL_XML);

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
		string definitionId1 = deployProcess(SINGLE_CONDITIONAL_XML);
		string definitionId2 = deployProcess(SINGLE_CONDITIONAL_XML);
		string definitionId3 = deployModel(MODEL_WITHOUT_CONDITION);

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
//ORIGINAL LINE: @SuppressWarnings("unused") String definitionId1 = deployProcess(SINGLE_CONDITIONAL_XML);
		  string definitionId1 = deployProcess(SINGLE_CONDITIONAL_XML);
		string definitionId2 = deployProcess(SINGLE_CONDITIONAL_XML);
		string definitionId3 = deployProcess(SINGLE_CONDITIONAL_XML);

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

		string definitionId1 = deployProcess(SINGLE_CONDITIONAL_XML);
		string definitionId2 = deployProcess(SINGLE_CONDITIONAL_XML);
		string definitionId3 = deployProcess(SINGLE_CONDITIONAL_XML); //we're deleting version 3, but as version 2 is already deleted, we must subscribe version 1

		// when
		repositoryService.deleteProcessDefinitions().byIds(definitionId2, definitionId3).delete();

		// then
		assertEquals(1, runtimeService.createEventSubscriptionQuery().count());
		assertEquals(definitionId1, ((EventSubscriptionEntity) runtimeService.createEventSubscriptionQuery().singleResult()).Configuration);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeploymentOfTwoEqualConditionalStartEvent()
	  public virtual void testDeploymentOfTwoEqualConditionalStartEvent()
	  {
		// expect
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot have more than one conditional event subscription with the same condition '${variable == 1}'");

		// when
		testRule.deploy(TWO_EQUAL_CONDITIONAL_START_EVENT_XML);

		IList<EventSubscription> eventSubscriptions = runtimeService.createEventSubscriptionQuery().list();
		assertEquals(0, eventSubscriptions.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testStartInstanceWithTrueConditionalStartEvent()
	  public virtual void testStartInstanceWithTrueConditionalStartEvent()
	  {
		// given a deployed process

		// when
		IList<ProcessInstance> conditionInstances = runtimeService.createConditionEvaluation().evaluateStartConditions();

		// then
		assertEquals(1, conditionInstances.Count);

		IList<ProcessInstance> processInstances = runtimeService.createProcessInstanceQuery().processDefinitionKey(TRUE_CONDITION_PROCESS).list();
		assertEquals(1, processInstances.Count);

		assertEquals(processInstances[0].Id, conditionInstances[0].Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = SINGLE_CONDITIONAL_START_EVENT_XML) public void testStartInstanceWithVariableCondition()
	  [Deployment(resources : SINGLE_CONDITIONAL_START_EVENT_XML)]
	  public virtual void testStartInstanceWithVariableCondition()
	  {
		// given a deployed process

		// when
		IList<ProcessInstance> instances = runtimeService.createConditionEvaluation().setVariable("foo", 1).evaluateStartConditions();

		// then
		assertEquals(1, instances.Count);

		VariableInstance vars = runtimeService.createVariableInstanceQuery().singleResult();
		assertEquals(vars.ProcessInstanceId, instances[0].Id);
		assertEquals(1, vars.Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = SINGLE_CONDITIONAL_START_EVENT_XML) public void testStartInstanceWithTransientVariableCondition()
	  [Deployment(resources : SINGLE_CONDITIONAL_START_EVENT_XML)]
	  public virtual void testStartInstanceWithTransientVariableCondition()
	  {
		// given a deployed process
		VariableMap variableMap = Variables.createVariables().putValueTyped("foo", Variables.integerValue(1, true));

		// when
		IList<ProcessInstance> instances = runtimeService.createConditionEvaluation().setVariables(variableMap).evaluateStartConditions();

		// then
		assertEquals(1, instances.Count);

		VariableInstance vars = runtimeService.createVariableInstanceQuery().singleResult();
		assertNull(vars);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = SINGLE_CONDITIONAL_START_EVENT_XML) public void testStartInstanceWithoutResult()
	  [Deployment(resources : SINGLE_CONDITIONAL_START_EVENT_XML)]
	  public virtual void testStartInstanceWithoutResult()
	  {
		// given a deployed process

		// when
		IList<ProcessInstance> processes = runtimeService.createConditionEvaluation().setVariable("foo", 0).evaluateStartConditions();

		assertNotNull(processes);
		assertEquals(0, processes.Count);

		assertNull(runtimeService.createVariableInstanceQuery().singleResult());
		assertNull(runtimeService.createProcessInstanceQuery().processDefinitionKey(CONDITIONAL_EVENT_PROCESS).singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = MULTIPLE_CONDITION_XML) public void testStartInstanceWithMultipleConditions()
	  [Deployment(resources : MULTIPLE_CONDITION_XML)]
	  public virtual void testStartInstanceWithMultipleConditions()
	  {
		// given a deployed process with three conditional start events
		IList<EventSubscription> eventSubscriptions = runtimeService.createEventSubscriptionQuery().list();

		assertEquals(3, eventSubscriptions.Count);
		foreach (EventSubscription eventSubscription in eventSubscriptions)
		{
		  assertEquals(EventType.CONDITONAL.name(), eventSubscription.EventType);
		}

		IDictionary<string, object> variableMap = new Dictionary<string, object>();
		variableMap["foo"] = 1;
		variableMap["bar"] = true;

		// when
		IList<ProcessInstance> resultInstances = runtimeService.createConditionEvaluation().setVariables(variableMap).evaluateStartConditions();

		// then
		assertEquals(2, resultInstances.Count);

		IList<ProcessInstance> instances = runtimeService.createProcessInstanceQuery().processDefinitionKey(MULTIPLE_CONDITIONS).list();
		assertEquals(2, instances.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { SINGLE_CONDITIONAL_START_EVENT_XML, MULTIPLE_CONDITION_XML, TRUE_CONDITION_START_XML }) public void testStartInstanceWithMultipleSubscriptions()
	  [Deployment(resources : { SINGLE_CONDITIONAL_START_EVENT_XML, MULTIPLE_CONDITION_XML, TRUE_CONDITION_START_XML })]
	  public virtual void testStartInstanceWithMultipleSubscriptions()
	  {
		// given three deployed processes
		IList<EventSubscription> eventSubscriptions = runtimeService.createEventSubscriptionQuery().list();

		assertEquals(5, eventSubscriptions.Count);

		IDictionary<string, object> variableMap = new Dictionary<string, object>();
		variableMap["foo"] = 1;
		variableMap["bar"] = true;

		// when
		IList<ProcessInstance> instances = runtimeService.createConditionEvaluation().setVariables(variableMap).evaluateStartConditions();

		// then
		assertEquals(4, instances.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { SINGLE_CONDITIONAL_START_EVENT_XML, MULTIPLE_CONDITION_XML, TRUE_CONDITION_START_XML }) public void testStartInstanceWithMultipleSubscriptionsWithoutProvidingAllVariables()
	  [Deployment(resources : { SINGLE_CONDITIONAL_START_EVENT_XML, MULTIPLE_CONDITION_XML, TRUE_CONDITION_START_XML })]
	  public virtual void testStartInstanceWithMultipleSubscriptionsWithoutProvidingAllVariables()
	  {
		// given three deployed processes
		IList<EventSubscription> eventSubscriptions = runtimeService.createEventSubscriptionQuery().list();

		assertEquals(5, eventSubscriptions.Count);

		IDictionary<string, object> variableMap = new Dictionary<string, object>();
		variableMap["foo"] = 1;

		// when, it should not throw PropertyNotFoundException
		IList<ProcessInstance> instances = runtimeService.createConditionEvaluation().setVariables(variableMap).evaluateStartConditions();

		// then
		assertEquals(3, instances.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { SINGLE_CONDITIONAL_START_EVENT_XML, MULTIPLE_CONDITION_XML }) public void testStartInstanceWithBusinessKey()
	  [Deployment(resources : { SINGLE_CONDITIONAL_START_EVENT_XML, MULTIPLE_CONDITION_XML })]
	  public virtual void testStartInstanceWithBusinessKey()
	  {
		// given two deployed processes
		IList<EventSubscription> eventSubscriptions = runtimeService.createEventSubscriptionQuery().list();

		assertEquals(4, eventSubscriptions.Count);

		// when
		IList<ProcessInstance> instances = runtimeService.createConditionEvaluation().setVariable("foo", 1).processInstanceBusinessKey("humuhumunukunukuapua").evaluateStartConditions();

		// then
		assertEquals(2, instances.Count);
		assertEquals(2, runtimeService.createProcessInstanceQuery().processInstanceBusinessKey("humuhumunukunukuapua").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { SINGLE_CONDITIONAL_START_EVENT_XML, TRUE_CONDITION_START_XML }) public void testStartInstanceByProcessDefinitionId()
	  [Deployment(resources : { SINGLE_CONDITIONAL_START_EVENT_XML, TRUE_CONDITION_START_XML })]
	  public virtual void testStartInstanceByProcessDefinitionId()
	  {
		// given two deployed processes
		IList<EventSubscription> eventSubscriptions = runtimeService.createEventSubscriptionQuery().list();

		assertEquals(2, eventSubscriptions.Count);

		string processDefinitionId = repositoryService.createProcessDefinitionQuery().processDefinitionKey(TRUE_CONDITION_PROCESS).singleResult().Id;

		// when
		IList<ProcessInstance> instances = runtimeService.createConditionEvaluation().setVariable("foo", 1).processDefinitionId(processDefinitionId).evaluateStartConditions();

		// then
		assertEquals(1, instances.Count);
		assertEquals(processDefinitionId, instances[0].ProcessDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { SINGLE_CONDITIONAL_START_EVENT_XML, MULTIPLE_CONDITION_XML}) public void testStartInstanceByProcessDefinitionFirstVersion()
	  [Deployment(resources : { SINGLE_CONDITIONAL_START_EVENT_XML, MULTIPLE_CONDITION_XML})]
	  public virtual void testStartInstanceByProcessDefinitionFirstVersion()
	  {
		// given two deployed processes
		string processDefinitionId = repositoryService.createProcessDefinitionQuery().processDefinitionKey(CONDITIONAL_EVENT_PROCESS).singleResult().Id;

		// assume
		IList<EventSubscription> eventSubscriptions = runtimeService.createEventSubscriptionQuery().list();
		assertEquals(4, eventSubscriptions.Count);

		// when deploy another version
		testRule.deploy(SINGLE_CONDITIONAL_START_EVENT_XML);

		IList<ProcessInstance> instances = runtimeService.createConditionEvaluation().setVariable("foo", 1).processDefinitionId(processDefinitionId).evaluateStartConditions();

		// then
		assertEquals(1, instances.Count);
		assertEquals(processDefinitionId, instances[0].ProcessDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { SINGLE_CONDITIONAL_START_EVENT_XML, TRUE_CONDITION_START_XML }) public void testStartInstanceByNonExistingProcessDefinitionId()
	  [Deployment(resources : { SINGLE_CONDITIONAL_START_EVENT_XML, TRUE_CONDITION_START_XML })]
	  public virtual void testStartInstanceByNonExistingProcessDefinitionId()
	  {
		// given two deployed processes
		IList<EventSubscription> eventSubscriptions = runtimeService.createEventSubscriptionQuery().list();

		assertEquals(2, eventSubscriptions.Count);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("no deployed process definition found with id 'nonExistingId': processDefinition is null");

		// when
		runtimeService.createConditionEvaluation().setVariable("foo", 1).processDefinitionId("nonExistingId").evaluateStartConditions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { ONE_TASK_PROCESS }) public void testStartInstanceByProcessDefinitionIdWithoutCondition()
	  [Deployment(resources : { ONE_TASK_PROCESS })]
	  public virtual void testStartInstanceByProcessDefinitionIdWithoutCondition()
	  {
		// given deployed process without conditional start event
		string processDefinitionId = repositoryService.createProcessDefinitionQuery().processDefinitionKey("oneTaskProcess").singleResult().Id;

		IList<EventSubscription> eventSubscriptions = runtimeService.createEventSubscriptionQuery().list();

		assertEquals(0, eventSubscriptions.Count);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Process definition with id '" + processDefinitionId + "' does not declare conditional start event");


		// when
		runtimeService.createConditionEvaluation().processDefinitionId(processDefinitionId).evaluateStartConditions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testStartInstanceWithVariableName()
	  public virtual void testStartInstanceWithVariableName()
	  {
		// given deployed process
		// ${true} variableName="foo"

		// assume
		IList<EventSubscription> eventSubscriptions = runtimeService.createEventSubscriptionQuery().list();
		assertEquals(1, eventSubscriptions.Count);

		// when
		IList<ProcessInstance> instances = runtimeService.createConditionEvaluation().setVariable("foo", true).evaluateStartConditions();

		// then
		assertEquals(1, instances.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = START_INSTANCE_WITH_VARIABLE_NAME_XML) public void testStartInstanceWithVariableNameNotFullfilled()
	  [Deployment(resources : START_INSTANCE_WITH_VARIABLE_NAME_XML)]
	  public virtual void testStartInstanceWithVariableNameNotFullfilled()
	  {
		// given deployed process
		// ${true} variableName="foo"

		// assume
		IList<EventSubscription> eventSubscriptions = runtimeService.createEventSubscriptionQuery().list();
		assertEquals(1, eventSubscriptions.Count);

		// when
		IList<ProcessInstance> instances = runtimeService.createConditionEvaluation().evaluateStartConditions();

		// then
		assertEquals(0, instances.Count);
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