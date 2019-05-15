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
namespace org.camunda.bpm.engine.test.api.runtime.migration
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.models.ConditionalModels.CONDITION_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.models.ConditionalModels.CONDITIONAL_PROCESS_KEY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.models.ConditionalModels.BOUNDARY_ID;
	using static org.camunda.bpm.engine.test.api.runtime.migration.models.EventSubProcessModels;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.MigrationPlanAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.MigrationPlanAssert.migrate;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;

	using MigrationInstructionsBuilder = org.camunda.bpm.engine.migration.MigrationInstructionsBuilder;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using MigrationPlanAssert = org.camunda.bpm.engine.test.util.MigrationPlanAssert;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using UserTask = org.camunda.bpm.model.bpmn.instance.UserTask;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigrationPlanGenerationTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationPlanGenerationTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testHelper = new MigrationTestRule(rule);
			ruleChain = RuleChain.outerRule(rule).around(testHelper);
		}


	  public const string MESSAGE_NAME = "Message";
	  public const string SIGNAL_NAME = "Signal";
	  public const string TIMER_DATE = "2016-02-11T12:13:14Z";
	  public const string ERROR_CODE = "Error";
	  public const string ESCALATION_CODE = "Escalation";

	  protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
	  protected internal MigrationTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(rule).around(testHelper);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEqualActivitiesInProcessDefinitionScope()
	  public virtual void testMapEqualActivitiesInProcessDefinitionScope()
	  {
		BpmnModelInstance sourceProcess = ProcessModels.ONE_TASK_PROCESS;
		BpmnModelInstance targetProcess = ProcessModels.ONE_TASK_PROCESS;

		assertGeneratedMigrationPlan(sourceProcess, targetProcess).hasInstructions(migrate("userTask").to("userTask"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEqualActivitiesInSameSubProcessScope()
	  public virtual void testMapEqualActivitiesInSameSubProcessScope()
	  {
		BpmnModelInstance sourceProcess = ProcessModels.SUBPROCESS_PROCESS;
		BpmnModelInstance targetProcess = ProcessModels.SUBPROCESS_PROCESS;

		assertGeneratedMigrationPlan(sourceProcess, targetProcess).hasInstructions(migrate("subProcess").to("subProcess"), migrate("userTask").to("userTask"));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEqualActivitiesToSubProcessScope()
	  public virtual void testMapEqualActivitiesToSubProcessScope()
	  {
		BpmnModelInstance sourceProcess = ProcessModels.ONE_TASK_PROCESS;
		BpmnModelInstance targetProcess = ProcessModels.SUBPROCESS_PROCESS;

		assertGeneratedMigrationPlan(sourceProcess, targetProcess).hasEmptyInstructions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEqualActivitiesToNestedSubProcessScope()
	  public virtual void testMapEqualActivitiesToNestedSubProcessScope()
	  {
		BpmnModelInstance sourceProcess = ProcessModels.SUBPROCESS_PROCESS;
		BpmnModelInstance targetProcess = modify(ProcessModels.DOUBLE_SUBPROCESS_PROCESS).changeElementId("outerSubProcess", "subProcess"); // make ID match with subprocess ID of source definition

		assertGeneratedMigrationPlan(sourceProcess, targetProcess).hasInstructions(migrate("subProcess").to("subProcess"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEqualActivitiesToSurroundingSubProcessScope()
	  public virtual void testMapEqualActivitiesToSurroundingSubProcessScope()
	  {
		BpmnModelInstance sourceProcess = ProcessModels.SUBPROCESS_PROCESS;
		BpmnModelInstance targetProcess = modify(ProcessModels.DOUBLE_SUBPROCESS_PROCESS).changeElementId("innerSubProcess", "subProcess"); // make ID match with subprocess ID of source definition

		assertGeneratedMigrationPlan(sourceProcess, targetProcess).hasEmptyInstructions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEqualActivitiesToDeeplyNestedSubProcessScope()
	  public virtual void testMapEqualActivitiesToDeeplyNestedSubProcessScope()
	  {
		BpmnModelInstance sourceProcess = ProcessModels.ONE_TASK_PROCESS;
		BpmnModelInstance targetProcess = ProcessModels.DOUBLE_SUBPROCESS_PROCESS;

		assertGeneratedMigrationPlan(sourceProcess, targetProcess).hasEmptyInstructions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEqualActivitiesToSiblingScope()
	  public virtual void testMapEqualActivitiesToSiblingScope()
	  {
		BpmnModelInstance sourceProcess = ProcessModels.PARALLEL_SUBPROCESS_PROCESS;
		BpmnModelInstance targetProcess = modify(ProcessModels.PARALLEL_SUBPROCESS_PROCESS).swapElementIds("userTask1", "userTask2");

		assertGeneratedMigrationPlan(sourceProcess, targetProcess).hasInstructions(migrate("subProcess1").to("subProcess1"), migrate("subProcess2").to("subProcess2"), migrate("fork").to("fork"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEqualActivitiesToNestedSiblingScope()
	  public virtual void testMapEqualActivitiesToNestedSiblingScope()
	  {
		BpmnModelInstance sourceProcess = ProcessModels.PARALLEL_DOUBLE_SUBPROCESS_PROCESS;
		BpmnModelInstance targetProcess = modify(ProcessModels.PARALLEL_DOUBLE_SUBPROCESS_PROCESS).swapElementIds("userTask1", "userTask2");

		assertGeneratedMigrationPlan(sourceProcess, targetProcess).hasInstructions(migrate("subProcess1").to("subProcess1"), migrate("nestedSubProcess1").to("nestedSubProcess1"), migrate("subProcess2").to("subProcess2"), migrate("nestedSubProcess2").to("nestedSubProcess2"), migrate("fork").to("fork"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEqualActivitiesWhichBecomeScope()
	  public virtual void testMapEqualActivitiesWhichBecomeScope()
	  {
		BpmnModelInstance sourceProcess = ProcessModels.ONE_TASK_PROCESS;
		BpmnModelInstance targetProcess = ProcessModels.SCOPE_TASK_PROCESS;

		assertGeneratedMigrationPlan(sourceProcess, targetProcess).hasInstructions(migrate("userTask").to("userTask"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEqualActivitiesWithParallelMultiInstance()
	  public virtual void testMapEqualActivitiesWithParallelMultiInstance()
	  {
		BpmnModelInstance testProcess = modify(ProcessModels.ONE_TASK_PROCESS).getModelElementById<UserTask>("userTask").builder().multiInstance().parallel().cardinality("3").multiInstanceDone().done();

		assertGeneratedMigrationPlan(testProcess, testProcess).hasInstructions(migrate("userTask").to("userTask"), migrate("userTask#multiInstanceBody").to("userTask#multiInstanceBody"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEqualActivitiesIgnoreUnsupportedActivities()
	  public virtual void testMapEqualActivitiesIgnoreUnsupportedActivities()
	  {
		BpmnModelInstance sourceProcess = ProcessModels.UNSUPPORTED_ACTIVITIES;
		BpmnModelInstance targetProcess = ProcessModels.UNSUPPORTED_ACTIVITIES;

		assertGeneratedMigrationPlan(sourceProcess, targetProcess).hasEmptyInstructions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEqualUnsupportedAsyncBeforeActivities()
	  public virtual void testMapEqualUnsupportedAsyncBeforeActivities()
	  {
		BpmnModelInstance testModel = modify(ProcessModels.UNSUPPORTED_ACTIVITIES).flowNodeBuilder("startEvent").camundaAsyncBefore().moveToNode("decisionTask").camundaAsyncBefore().moveToNode("throwEvent").camundaAsyncAfter().moveToNode("serviceTask").camundaAsyncBefore().moveToNode("sendTask").camundaAsyncBefore().moveToNode("scriptTask").camundaAsyncBefore().moveToNode("endEvent").camundaAsyncBefore().done();

		assertGeneratedMigrationPlan(testModel, testModel).hasInstructions(migrate("startEvent").to("startEvent"), migrate("decisionTask").to("decisionTask"), migrate("throwEvent").to("throwEvent"), migrate("serviceTask").to("serviceTask"), migrate("sendTask").to("sendTask"), migrate("scriptTask").to("scriptTask"), migrate("endEvent").to("endEvent"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEqualUnsupportedAsyncAfterActivities()
	  public virtual void testMapEqualUnsupportedAsyncAfterActivities()
	  {
		BpmnModelInstance testModel = modify(ProcessModels.UNSUPPORTED_ACTIVITIES).flowNodeBuilder("startEvent").camundaAsyncAfter().moveToNode("decisionTask").camundaAsyncAfter().moveToNode("throwEvent").camundaAsyncAfter().moveToNode("serviceTask").camundaAsyncAfter().moveToNode("sendTask").camundaAsyncAfter().moveToNode("scriptTask").camundaAsyncAfter().moveToNode("endEvent").camundaAsyncAfter().done();

		assertGeneratedMigrationPlan(testModel, testModel).hasInstructions(migrate("startEvent").to("startEvent"), migrate("decisionTask").to("decisionTask"), migrate("throwEvent").to("throwEvent"), migrate("serviceTask").to("serviceTask"), migrate("sendTask").to("sendTask"), migrate("scriptTask").to("scriptTask"), migrate("endEvent").to("endEvent"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEqualActivitiesToParentScope()
	  public virtual void testMapEqualActivitiesToParentScope()
	  {
		BpmnModelInstance sourceProcess = modify(ProcessModels.DOUBLE_SUBPROCESS_PROCESS).changeElementId("outerSubProcess", "subProcess");
		BpmnModelInstance targetProcess = ProcessModels.SUBPROCESS_PROCESS;

		assertGeneratedMigrationPlan(sourceProcess, targetProcess).hasInstructions(migrate("subProcess").to("subProcess"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEqualActivitiesFromScopeToProcessDefinition()
	  public virtual void testMapEqualActivitiesFromScopeToProcessDefinition()
	  {
		BpmnModelInstance sourceProcess = ProcessModels.SUBPROCESS_PROCESS;
		BpmnModelInstance targetProcess = ProcessModels.ONE_TASK_PROCESS;

		assertGeneratedMigrationPlan(sourceProcess, targetProcess).hasEmptyInstructions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEqualActivitiesFromDoubleScopeToProcessDefinition()
	  public virtual void testMapEqualActivitiesFromDoubleScopeToProcessDefinition()
	  {
		BpmnModelInstance sourceProcess = ProcessModels.DOUBLE_SUBPROCESS_PROCESS;
		BpmnModelInstance targetProcess = ProcessModels.ONE_TASK_PROCESS;

		assertGeneratedMigrationPlan(sourceProcess, targetProcess).hasEmptyInstructions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEqualActivitiesFromTripleScopeToProcessDefinition()
	  public virtual void testMapEqualActivitiesFromTripleScopeToProcessDefinition()
	  {
		BpmnModelInstance sourceProcess = ProcessModels.TRIPLE_SUBPROCESS_PROCESS;
		BpmnModelInstance targetProcess = ProcessModels.ONE_TASK_PROCESS;

		assertGeneratedMigrationPlan(sourceProcess, targetProcess).hasEmptyInstructions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEqualActivitiesFromTripleScopeToSingleNewScope()
	  public virtual void testMapEqualActivitiesFromTripleScopeToSingleNewScope()
	  {
		BpmnModelInstance sourceProcess = ProcessModels.TRIPLE_SUBPROCESS_PROCESS;
		BpmnModelInstance targetProcess = ProcessModels.SUBPROCESS_PROCESS;

		assertGeneratedMigrationPlan(sourceProcess, targetProcess).hasEmptyInstructions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEqualActivitiesFromTripleScopeToTwoNewScopes()
	  public virtual void testMapEqualActivitiesFromTripleScopeToTwoNewScopes()
	  {
		BpmnModelInstance sourceProcess = ProcessModels.TRIPLE_SUBPROCESS_PROCESS;
		BpmnModelInstance targetProcess = ProcessModels.DOUBLE_SUBPROCESS_PROCESS;

		assertGeneratedMigrationPlan(sourceProcess, targetProcess).hasEmptyInstructions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEqualActivitiesToNewScopes()
	  public virtual void testMapEqualActivitiesToNewScopes()
	  {
		BpmnModelInstance sourceProcess = ProcessModels.DOUBLE_SUBPROCESS_PROCESS;
		BpmnModelInstance targetProcess = modify(ProcessModels.DOUBLE_SUBPROCESS_PROCESS).changeElementId("outerSubProcess", "newOuterSubProcess").changeElementId("innerSubProcess", "newInnerSubProcess");

		assertGeneratedMigrationPlan(sourceProcess, targetProcess).hasEmptyInstructions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEqualActivitiesOutsideOfScope()
	  public virtual void testMapEqualActivitiesOutsideOfScope()
	  {
		BpmnModelInstance sourceProcess = ProcessModels.PARALLEL_GATEWAY_SUBPROCESS_PROCESS;
		BpmnModelInstance targetProcess = ProcessModels.PARALLEL_TASK_AND_SUBPROCESS_PROCESS;

		assertGeneratedMigrationPlan(sourceProcess, targetProcess).hasInstructions(migrate("subProcess").to("subProcess"), migrate("userTask1").to("userTask1"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEqualActivitiesToHorizontalScope()
	  public virtual void testMapEqualActivitiesToHorizontalScope()
	  {
		BpmnModelInstance sourceProcess = ProcessModels.PARALLEL_TASK_AND_SUBPROCESS_PROCESS;
		BpmnModelInstance targetProcess = ProcessModels.PARALLEL_GATEWAY_SUBPROCESS_PROCESS;

		assertGeneratedMigrationPlan(sourceProcess, targetProcess).hasInstructions(migrate("subProcess").to("subProcess"), migrate("userTask1").to("userTask1"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEqualActivitiesFromTaskWithBoundaryEvent()
	  public virtual void testMapEqualActivitiesFromTaskWithBoundaryEvent()
	  {
		BpmnModelInstance sourceProcess = modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder("userTask").boundaryEvent(null).message("Message").done();
		BpmnModelInstance targetProcess = ProcessModels.ONE_TASK_PROCESS;

		assertGeneratedMigrationPlan(sourceProcess, targetProcess).hasInstructions(migrate("userTask").to("userTask"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEqualActivitiesToTaskWithBoundaryEvent()
	  public virtual void testMapEqualActivitiesToTaskWithBoundaryEvent()
	  {
		BpmnModelInstance sourceProcess = ProcessModels.ONE_TASK_PROCESS;
		BpmnModelInstance targetProcess = modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder("userTask").boundaryEvent(null).message("Message").done();

		assertGeneratedMigrationPlan(sourceProcess, targetProcess).hasInstructions(migrate("userTask").to("userTask"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEqualActivitiesWithBoundaryEvent()
	  public virtual void testMapEqualActivitiesWithBoundaryEvent()
	  {
		BpmnModelInstance testProcess = modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").boundaryEvent("messageBoundary").message(MESSAGE_NAME).moveToActivity("userTask").boundaryEvent("signalBoundary").signal(SIGNAL_NAME).moveToActivity("userTask").boundaryEvent("timerBoundary").timerWithDate(TIMER_DATE).done();

		assertGeneratedMigrationPlan(testProcess, testProcess).hasInstructions(migrate("subProcess").to("subProcess"), migrate("messageBoundary").to("messageBoundary"), migrate("userTask").to("userTask"), migrate("signalBoundary").to("signalBoundary"), migrate("timerBoundary").to("timerBoundary"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNotMapBoundaryEventsWithDifferentIds()
	  public virtual void testNotMapBoundaryEventsWithDifferentIds()
	  {
		BpmnModelInstance sourceProcess = modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder("userTask").boundaryEvent("message").message(MESSAGE_NAME).done();
		BpmnModelInstance targetProcess = modify(sourceProcess).changeElementId("message", "newMessage");

		assertGeneratedMigrationPlan(sourceProcess, targetProcess).hasInstructions(migrate("userTask").to("userTask"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIgnoreNotSupportedBoundaryEvents()
	  public virtual void testIgnoreNotSupportedBoundaryEvents()
	  {
		BpmnModelInstance testProcess = modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").boundaryEvent("messageBoundary").message(MESSAGE_NAME).moveToActivity("subProcess").boundaryEvent("errorBoundary").error(ERROR_CODE).moveToActivity("subProcess").boundaryEvent("escalationBoundary").escalation(ESCALATION_CODE).moveToActivity("userTask").boundaryEvent("signalBoundary").signal(SIGNAL_NAME).done();

		assertGeneratedMigrationPlan(testProcess, testProcess).hasInstructions(migrate("subProcess").to("subProcess"), migrate("messageBoundary").to("messageBoundary"), migrate("userTask").to("userTask"), migrate("signalBoundary").to("signalBoundary"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNotMigrateBoundaryToParallelActivity()
	  public virtual void testNotMigrateBoundaryToParallelActivity()
	  {
		BpmnModelInstance sourceProcess = modify(ProcessModels.PARALLEL_GATEWAY_PROCESS).activityBuilder("userTask1").boundaryEvent("message").message(MESSAGE_NAME).done();
		BpmnModelInstance targetProcess = modify(ProcessModels.PARALLEL_GATEWAY_PROCESS).activityBuilder("userTask2").boundaryEvent("message").message(MESSAGE_NAME).done();

		assertGeneratedMigrationPlan(sourceProcess, targetProcess).hasInstructions(migrate("userTask1").to("userTask1"), migrate("userTask2").to("userTask2"), migrate("fork").to("fork"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNotMigrateBoundaryToChildActivity()
	  public virtual void testNotMigrateBoundaryToChildActivity()
	  {
		BpmnModelInstance sourceProcess = modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").boundaryEvent("message").message(MESSAGE_NAME).done();
		BpmnModelInstance targetProcess = modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("userTask").boundaryEvent("message").message(MESSAGE_NAME).done();

		assertGeneratedMigrationPlan(sourceProcess, targetProcess).hasInstructions(migrate("subProcess").to("subProcess"), migrate("userTask").to("userTask"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateProcessInstanceWithEventSubProcess()
	  public virtual void testMigrateProcessInstanceWithEventSubProcess()
	  {
		BpmnModelInstance testProcess = modify(ProcessModels.ONE_TASK_PROCESS).addSubProcessTo(ProcessModels.PROCESS_KEY).id("eventSubProcess").triggerByEvent().embeddedSubProcess().startEvent("eventSubProcessStart").message(MESSAGE_NAME).endEvent().subProcessDone().done();

		assertGeneratedMigrationPlan(testProcess, testProcess).hasInstructions(migrate("eventSubProcess").to("eventSubProcess"), migrate("eventSubProcessStart").to("eventSubProcessStart"), migrate("userTask").to("userTask"));

		assertGeneratedMigrationPlan(testProcess, ProcessModels.ONE_TASK_PROCESS).hasInstructions(migrate("userTask").to("userTask"));

		assertGeneratedMigrationPlan(ProcessModels.ONE_TASK_PROCESS, testProcess).hasInstructions(migrate("userTask").to("userTask"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateSubProcessWithEventSubProcess()
	  public virtual void testMigrateSubProcessWithEventSubProcess()
	  {
		BpmnModelInstance testProcess = modify(ProcessModels.SUBPROCESS_PROCESS).addSubProcessTo("subProcess").id("eventSubProcess").triggerByEvent().embeddedSubProcess().startEvent("eventSubProcessStart").message(MESSAGE_NAME).endEvent().subProcessDone().done();

		assertGeneratedMigrationPlan(testProcess, testProcess).hasInstructions(migrate("eventSubProcess").to("eventSubProcess"), migrate("eventSubProcessStart").to("eventSubProcessStart"), migrate("subProcess").to("subProcess"), migrate("userTask").to("userTask"));

		assertGeneratedMigrationPlan(testProcess, ProcessModels.SUBPROCESS_PROCESS).hasInstructions(migrate("subProcess").to("subProcess"), migrate("userTask").to("userTask"));

		assertGeneratedMigrationPlan(ProcessModels.SUBPROCESS_PROCESS, testProcess).hasInstructions(migrate("subProcess").to("subProcess"), migrate("userTask").to("userTask"));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateUserTaskInEventSubProcess()
	  public virtual void testMigrateUserTaskInEventSubProcess()
	  {
		BpmnModelInstance testProcess = modify(ProcessModels.SUBPROCESS_PROCESS).addSubProcessTo("subProcess").id("eventSubProcess").triggerByEvent().embeddedSubProcess().startEvent("eventSubProcessStart").message(MESSAGE_NAME).userTask("innerTask").endEvent().subProcessDone().done();

		assertGeneratedMigrationPlan(testProcess, testProcess).hasInstructions(migrate("eventSubProcess").to("eventSubProcess"), migrate("eventSubProcessStart").to("eventSubProcessStart"), migrate("innerTask").to("innerTask"), migrate("subProcess").to("subProcess"), migrate("userTask").to("userTask"));

		assertGeneratedMigrationPlan(testProcess, ProcessModels.SUBPROCESS_PROCESS).hasInstructions(migrate("subProcess").to("subProcess"), migrate("userTask").to("userTask"));

		assertGeneratedMigrationPlan(ProcessModels.SUBPROCESS_PROCESS, testProcess).hasInstructions(migrate("subProcess").to("subProcess"), migrate("userTask").to("userTask"));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNotMigrateActivitiesOfDifferentType()
	  public virtual void testNotMigrateActivitiesOfDifferentType()
	  {
		BpmnModelInstance sourceProcess = ProcessModels.ONE_TASK_PROCESS;
		BpmnModelInstance targetProcess = modify(ProcessModels.SUBPROCESS_PROCESS).swapElementIds("userTask", "subProcess");

		assertGeneratedMigrationPlan(sourceProcess, targetProcess).hasEmptyInstructions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNotMigrateBoundaryEventsOfDifferentType()
	  public virtual void testNotMigrateBoundaryEventsOfDifferentType()
	  {
		BpmnModelInstance sourceProcess = modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").message(MESSAGE_NAME).done();
		BpmnModelInstance targetProcess = modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").signal(SIGNAL_NAME).done();

		assertGeneratedMigrationPlan(sourceProcess, targetProcess).hasInstructions(migrate("userTask").to("userTask"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNotMigrateMultiInstanceOfDifferentType()
	  public virtual void testNotMigrateMultiInstanceOfDifferentType()
	  {
		BpmnModelInstance sourceProcess = MultiInstanceProcessModels.SEQ_MI_ONE_TASK_PROCESS;
		BpmnModelInstance targetProcess = MultiInstanceProcessModels.PAR_MI_ONE_TASK_PROCESS;

		assertGeneratedMigrationPlan(sourceProcess, targetProcess).hasEmptyInstructions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNotMigrateBoundaryEventsWithInvalidEventScopeInstruction()
	  public virtual void testNotMigrateBoundaryEventsWithInvalidEventScopeInstruction()
	  {
		BpmnModelInstance sourceProcess = modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").message("foo").done();
		BpmnModelInstance targetProcess = modify(ProcessModels.ONE_RECEIVE_TASK_PROCESS).changeElementId("receiveTask", "userTask").activityBuilder("userTask").boundaryEvent("boundary").message("foo").done();

		assertGeneratedMigrationPlan(sourceProcess, targetProcess).hasEmptyInstructions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapReceiveTasks()
	  public virtual void testMapReceiveTasks()
	  {
		assertGeneratedMigrationPlan(MessageReceiveModels.ONE_RECEIVE_TASK_PROCESS, MessageReceiveModels.ONE_RECEIVE_TASK_PROCESS).hasInstructions(migrate("receiveTask").to("receiveTask"), migrate("userTask").to("userTask"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapMessageCatchEvents()
	  public virtual void testMapMessageCatchEvents()
	  {
		assertGeneratedMigrationPlan(MessageReceiveModels.ONE_MESSAGE_CATCH_PROCESS, MessageReceiveModels.ONE_MESSAGE_CATCH_PROCESS).hasInstructions(migrate("messageCatch").to("messageCatch"), migrate("userTask").to("userTask"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapCallActivitiesToBpmnTest()
	  public virtual void testMapCallActivitiesToBpmnTest()
	  {
		assertGeneratedMigrationPlan(CallActivityModels.oneBpmnCallActivityProcess("foo"), CallActivityModels.oneBpmnCallActivityProcess("foo")).hasInstructions(migrate("callActivity").to("callActivity"), migrate("userTask").to("userTask"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapCallActivitiesToCmmnTest()
	  public virtual void testMapCallActivitiesToCmmnTest()
	  {
		assertGeneratedMigrationPlan(CallActivityModels.oneCmmnCallActivityProcess("foo"), CallActivityModels.oneCmmnCallActivityProcess("foo")).hasInstructions(migrate("callActivity").to("callActivity"), migrate("userTask").to("userTask"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapCallActivitiesFromBpmnToCmmnTest()
	  public virtual void testMapCallActivitiesFromBpmnToCmmnTest()
	  {
		assertGeneratedMigrationPlan(CallActivityModels.oneBpmnCallActivityProcess("foo"), CallActivityModels.oneCmmnCallActivityProcess("foo")).hasInstructions(migrate("callActivity").to("callActivity"), migrate("userTask").to("userTask"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapCallActivitiesFromCmmnToBpmnTest()
	  public virtual void testMapCallActivitiesFromCmmnToBpmnTest()
	  {
		assertGeneratedMigrationPlan(CallActivityModels.oneCmmnCallActivityProcess("foo"), CallActivityModels.oneBpmnCallActivityProcess("foo")).hasInstructions(migrate("callActivity").to("callActivity"), migrate("userTask").to("userTask"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEventBasedGateway()
	  public virtual void testMapEventBasedGateway()
	  {
		assertGeneratedMigrationPlan(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS, EventBasedGatewayModels.SIGNAL_EVENT_BASED_GW_PROCESS).hasInstructions(migrate("eventBasedGateway").to("eventBasedGateway"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEventBasedGatewayWithIdenticalFollowingEvents()
	  public virtual void testMapEventBasedGatewayWithIdenticalFollowingEvents()
	  {
		assertGeneratedMigrationPlan(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS, EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS).hasInstructions(migrate("eventBasedGateway").to("eventBasedGateway"), migrate("timerCatch").to("timerCatch"), migrate("afterTimerCatch").to("afterTimerCatch"));
	  }

	  // event sub process

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapTimerEventSubProcessAndStartEvent()
	  public virtual void testMapTimerEventSubProcessAndStartEvent()
	  {
		assertGeneratedMigrationPlan(TIMER_EVENT_SUBPROCESS_PROCESS, TIMER_EVENT_SUBPROCESS_PROCESS).hasInstructions(migrate("userTask").to("userTask"), migrate("eventSubProcess").to("eventSubProcess"), migrate("eventSubProcessStart").to("eventSubProcessStart"), migrate("eventSubProcessTask").to("eventSubProcessTask"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapMessageEventSubProcessAndStartEvent()
	  public virtual void testMapMessageEventSubProcessAndStartEvent()
	  {
		assertGeneratedMigrationPlan(MESSAGE_EVENT_SUBPROCESS_PROCESS, MESSAGE_EVENT_SUBPROCESS_PROCESS).hasInstructions(migrate("userTask").to("userTask"), migrate("eventSubProcess").to("eventSubProcess"), migrate("eventSubProcessStart").to("eventSubProcessStart"), migrate("eventSubProcessTask").to("eventSubProcessTask"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapSignalEventSubProcessAndStartEvent()
	  public virtual void testMapSignalEventSubProcessAndStartEvent()
	  {
		assertGeneratedMigrationPlan(SIGNAL_EVENT_SUBPROCESS_PROCESS, SIGNAL_EVENT_SUBPROCESS_PROCESS).hasInstructions(migrate("userTask").to("userTask"), migrate("eventSubProcess").to("eventSubProcess"), migrate("eventSubProcessStart").to("eventSubProcessStart"), migrate("eventSubProcessTask").to("eventSubProcessTask"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEscalationEventSubProcessAndStartEvent()
	  public virtual void testMapEscalationEventSubProcessAndStartEvent()
	  {
		assertGeneratedMigrationPlan(ESCALATION_EVENT_SUBPROCESS_PROCESS, ESCALATION_EVENT_SUBPROCESS_PROCESS).hasInstructions(migrate("userTask").to("userTask"), migrate("eventSubProcess").to("eventSubProcess"), migrate("eventSubProcessTask").to("eventSubProcessTask"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapErrorEventSubProcessAndStartEvent()
	  public virtual void testMapErrorEventSubProcessAndStartEvent()
	  {
		assertGeneratedMigrationPlan(ERROR_EVENT_SUBPROCESS_PROCESS, ERROR_EVENT_SUBPROCESS_PROCESS).hasInstructions(migrate("userTask").to("userTask"), migrate("eventSubProcess").to("eventSubProcess"), migrate("eventSubProcessTask").to("eventSubProcessTask"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapCompensationEventSubProcessAndStartEvent()
	  public virtual void testMapCompensationEventSubProcessAndStartEvent()
	  {
		assertGeneratedMigrationPlan(COMPENSATE_EVENT_SUBPROCESS_PROCESS, COMPENSATE_EVENT_SUBPROCESS_PROCESS).hasInstructions(migrate("subProcess").to("subProcess"), migrate("userTask").to("userTask"), migrate("eventSubProcessStart").to("eventSubProcessStart"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNotMapEventSubProcessStartEventOfDifferentType()
	  public virtual void testNotMapEventSubProcessStartEventOfDifferentType()
	  {
		assertGeneratedMigrationPlan(TIMER_EVENT_SUBPROCESS_PROCESS, SIGNAL_EVENT_SUBPROCESS_PROCESS).hasInstructions(migrate("userTask").to("userTask"), migrate("eventSubProcess").to("eventSubProcess"), migrate("eventSubProcessTask").to("eventSubProcessTask"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEventSubProcessStartEventWhenSubProcessesAreNotEqual()
	  public virtual void testMapEventSubProcessStartEventWhenSubProcessesAreNotEqual()
	  {
		BpmnModelInstance sourceModel = TIMER_EVENT_SUBPROCESS_PROCESS;
		BpmnModelInstance targetModel = modify(TIMER_EVENT_SUBPROCESS_PROCESS).changeElementId("eventSubProcess", "newEventSubProcess");

		assertGeneratedMigrationPlan(sourceModel, targetModel).hasInstructions(migrate("userTask").to("userTask"), migrate("eventSubProcessStart").to("eventSubProcessStart"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEventSubProcessToEmbeddedSubProcess()
	  public virtual void testMapEventSubProcessToEmbeddedSubProcess()
	  {
		BpmnModelInstance sourceModel = modify(TIMER_EVENT_SUBPROCESS_PROCESS).changeElementId("eventSubProcess", "subProcess");
		BpmnModelInstance targetModel = ProcessModels.SUBPROCESS_PROCESS;

		assertGeneratedMigrationPlan(sourceModel, targetModel).hasInstructions(migrate("subProcess").to("subProcess"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEmbeddedSubProcessToEventSubProcess()
	  public virtual void testMapEmbeddedSubProcessToEventSubProcess()
	  {
		BpmnModelInstance sourceModel = ProcessModels.SUBPROCESS_PROCESS;
		BpmnModelInstance targetModel = modify(TIMER_EVENT_SUBPROCESS_PROCESS).changeElementId("eventSubProcess", "subProcess");

		assertGeneratedMigrationPlan(sourceModel, targetModel).hasInstructions(migrate("subProcess").to("subProcess"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapExternalServiceTask()
	  public virtual void testMapExternalServiceTask()
	  {
		BpmnModelInstance sourceModel = ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS;
		BpmnModelInstance targetModel = ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS;

		assertGeneratedMigrationPlan(sourceModel, targetModel).hasInstructions(migrate("externalTask").to("externalTask"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapExternalServiceToDifferentType()
	  public virtual void testMapExternalServiceToDifferentType()
	  {
		BpmnModelInstance sourceModel = ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS;
		BpmnModelInstance targetModel = ProcessModels.newModel().startEvent().sendTask("externalTask").camundaType("external").camundaTopic("foo").endEvent().done();

		assertGeneratedMigrationPlan(sourceModel, targetModel).hasInstructions(migrate("externalTask").to("externalTask"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNotMapExternalToClassDelegateServiceTask()
	  public virtual void testNotMapExternalToClassDelegateServiceTask()
	  {
		BpmnModelInstance sourceModel = ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS;
		BpmnModelInstance targetModel = modify(ServiceTaskModels.oneClassDelegateServiceTask("foo.Bar")).changeElementId("serviceTask", "externalTask");

		assertGeneratedMigrationPlan(sourceModel, targetModel).hasEmptyInstructions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapParallelGateways()
	  public virtual void testMapParallelGateways()
	  {
		BpmnModelInstance model = GatewayModels.PARALLEL_GW;

		assertGeneratedMigrationPlan(model, model).hasInstructions(migrate("fork").to("fork"), migrate("join").to("join"), migrate("parallel1").to("parallel1"), migrate("parallel2").to("parallel2"), migrate("afterJoin").to("afterJoin"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapInclusiveGateways()
	  public virtual void testMapInclusiveGateways()
	  {
		BpmnModelInstance model = GatewayModels.INCLUSIVE_GW;

		assertGeneratedMigrationPlan(model, model).hasInstructions(migrate("fork").to("fork"), migrate("join").to("join"), migrate("parallel1").to("parallel1"), migrate("parallel2").to("parallel2"), migrate("afterJoin").to("afterJoin"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNotMapParallelToInclusiveGateway()
	  public virtual void testNotMapParallelToInclusiveGateway()
	  {

		assertGeneratedMigrationPlan(GatewayModels.PARALLEL_GW, GatewayModels.INCLUSIVE_GW).hasInstructions(migrate("parallel1").to("parallel1"), migrate("parallel2").to("parallel2"), migrate("afterJoin").to("afterJoin"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapTransaction()
	  public virtual void testMapTransaction()
	  {

		assertGeneratedMigrationPlan(TransactionModels.ONE_TASK_TRANSACTION, TransactionModels.ONE_TASK_TRANSACTION).hasInstructions(migrate("transaction").to("transaction"), migrate("userTask").to("userTask"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEmbeddedSubProcessToTransaction()
	  public virtual void testMapEmbeddedSubProcessToTransaction()
	  {
		BpmnModelInstance sourceProcess = ProcessModels.SUBPROCESS_PROCESS;
		BpmnModelInstance targetProcess = modify(TransactionModels.ONE_TASK_TRANSACTION).changeElementId("transaction", "subProcess");

		assertGeneratedMigrationPlan(sourceProcess, targetProcess).hasInstructions(migrate("subProcess").to("subProcess"), migrate("userTask").to("userTask"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapTransactionToEventSubProcess()
	  public virtual void testMapTransactionToEventSubProcess()
	  {

		BpmnModelInstance sourceProcess = TransactionModels.ONE_TASK_TRANSACTION;
		BpmnModelInstance targetProcess = modify(MESSAGE_EVENT_SUBPROCESS_PROCESS).changeElementId("eventSubProcess", "transaction").changeElementId("userTask", "foo").changeElementId("eventSubProcessTask", "userTask");

		assertGeneratedMigrationPlan(sourceProcess, targetProcess).hasInstructions(migrate("transaction").to("transaction"), migrate("userTask").to("userTask"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapNoUpdateEventTriggers()
	  public virtual void testMapNoUpdateEventTriggers()
	  {
		BpmnModelInstance model = MessageReceiveModels.ONE_MESSAGE_CATCH_PROCESS;

		assertGeneratedMigrationPlan(model, model, false).hasInstructions(migrate("userTask").to("userTask").updateEventTrigger(false), migrate("messageCatch").to("messageCatch").updateEventTrigger(false));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapUpdateEventTriggers()
	  public virtual void testMapUpdateEventTriggers()
	  {
		BpmnModelInstance model = MessageReceiveModels.ONE_MESSAGE_CATCH_PROCESS;

		assertGeneratedMigrationPlan(model, model, true).hasInstructions(migrate("userTask").to("userTask").updateEventTrigger(false), migrate("messageCatch").to("messageCatch").updateEventTrigger(true));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrationPlanCreationWithEmptyDeploymentCache()
	  public virtual void testMigrationPlanCreationWithEmptyDeploymentCache()
	  {
		// given
		ProcessDefinition sourceDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		rule.ProcessEngineConfiguration.DeploymentCache.discardProcessDefinitionCache();

		// when
		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapEqualActivities().build();

		// then
		assertNotNull(migrationPlan);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapCompensationBoundaryEvents()
	  public virtual void testMapCompensationBoundaryEvents()
	  {

		assertGeneratedMigrationPlan(CompensationModels.ONE_COMPENSATION_TASK_MODEL, CompensationModels.ONE_COMPENSATION_TASK_MODEL, true).hasInstructions(migrate("userTask1").to("userTask1").updateEventTrigger(false), migrate("userTask2").to("userTask2").updateEventTrigger(false), migrate("compensationBoundary").to("compensationBoundary").updateEventTrigger(false));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapCompensationStartEvents()
	  public virtual void testMapCompensationStartEvents()
	  {
		assertGeneratedMigrationPlan(CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL, CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL, true).hasInstructions(migrate("subProcess").to("subProcess").updateEventTrigger(false), migrate("userTask1").to("userTask1").updateEventTrigger(false), migrate("eventSubProcessStart").to("eventSubProcessStart").updateEventTrigger(false), migrate("userTask2").to("userTask2").updateEventTrigger(false), migrate("compensationBoundary").to("compensationBoundary").updateEventTrigger(false));

		// should not map eventSubProcess because it active compensation is not supported
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapIntermediateConditionalEvent()
	  public virtual void testMapIntermediateConditionalEvent()
	  {
		BpmnModelInstance sourceProcess = Bpmn.createExecutableProcess(CONDITIONAL_PROCESS_KEY).startEvent().intermediateCatchEvent(CONDITION_ID).condition(VAR_CONDITION).userTask(USER_TASK_ID).endEvent().done();

		assertGeneratedMigrationPlan(sourceProcess, sourceProcess, false).hasInstructions(migrate(CONDITION_ID).to(CONDITION_ID).updateEventTrigger(true), migrate(USER_TASK_ID).to(USER_TASK_ID).updateEventTrigger(false));
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapConditionalEventSubProcess()
	  public virtual void testMapConditionalEventSubProcess()
	  {
		assertGeneratedMigrationPlan(FALSE_CONDITIONAL_EVENT_SUBPROCESS_PROCESS, CONDITIONAL_EVENT_SUBPROCESS_PROCESS, false).hasInstructions(migrate(EVENT_SUB_PROCESS_START_ID).to(EVENT_SUB_PROCESS_START_ID).updateEventTrigger(true), migrate(EVENT_SUB_PROCESS_ID).to(EVENT_SUB_PROCESS_ID).updateEventTrigger(false), migrate(EVENT_SUB_PROCESS_TASK_ID).to(EVENT_SUB_PROCESS_TASK_ID), migrate(USER_TASK_ID).to(USER_TASK_ID));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapConditionalBoundaryEvents()
	  public virtual void testMapConditionalBoundaryEvents()
	  {
		BpmnModelInstance sourceProcess = modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder(USER_TASK_ID).boundaryEvent(BOUNDARY_ID).condition(VAR_CONDITION).done();

		assertGeneratedMigrationPlan(sourceProcess, sourceProcess, false).hasInstructions(migrate(BOUNDARY_ID).to(BOUNDARY_ID).updateEventTrigger(true), migrate(USER_TASK_ID).to(USER_TASK_ID).updateEventTrigger(false));
	  }

	  // helper

	  protected internal virtual MigrationPlanAssert assertGeneratedMigrationPlan(BpmnModelInstance sourceProcess, BpmnModelInstance targetProcess)
	  {
		return assertGeneratedMigrationPlan(sourceProcess, targetProcess, false);
	  }

	  protected internal virtual MigrationPlanAssert assertGeneratedMigrationPlan(BpmnModelInstance sourceProcess, BpmnModelInstance targetProcess, bool updateEventTriggers)
	  {
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);

		MigrationInstructionsBuilder migrationInstructionsBuilder = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities();

		if (updateEventTriggers)
		{
		  migrationInstructionsBuilder.updateEventTriggers();
		}

		MigrationPlan migrationPlan = migrationInstructionsBuilder.build();

		assertThat(migrationPlan).hasSourceProcessDefinition(sourceProcessDefinition).hasTargetProcessDefinition(targetProcessDefinition);

		return assertThat(migrationPlan);
	  }

	}

}