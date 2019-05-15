using System;

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
//	import static org.camunda.bpm.engine.test.util.MigrationPlanAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.MigrationPlanAssert.migrate;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.MigrationPlanValidationReportAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using MigrationPlanValidationException = org.camunda.bpm.engine.migration.MigrationPlanValidationException;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using EventSubProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.EventSubProcessModels;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using UserTaskBuilder = org.camunda.bpm.model.bpmn.builder.UserTaskBuilder;
	using CoreMatchers = org.hamcrest.CoreMatchers;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigrationPlanCreationTest
	{
		private bool InstanceFieldsInitialized = false;

		private class MigrationEventSubProcessTestConfigurationAnonymousInnerClass2 : MigrationEventSubProcessTestConfiguration
		{
			private readonly MigrationNestedEventSubProcessTest outerInstance;

			public MigrationEventSubProcessTestConfigurationAnonymousInnerClass2(MigrationNestedEventSubProcessTest outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override BpmnModelInstance SourceProcess
			{
				get
				{
				  return modify(ProcessModels.SUBPROCESS_PROCESS).addSubProcessTo(EventSubProcessModels.SUB_PROCESS_ID).triggerByEvent().embeddedSubProcess().startEvent(EVENT_SUB_PROCESS_START_ID).signal(EventSubProcessModels.SIGNAL_NAME).userTask(EVENT_SUB_PROCESS_TASK_ID).endEvent().subProcessDone().done();
				}
			}

			public override string EventName
			{
				get
				{
				  return EventSubProcessModels.SIGNAL_NAME;
				}
			}

			public override void triggerEventSubProcess(MigrationTestRule testHelper)
			{
			  testHelper.sendSignal(EventSubProcessModels.SIGNAL_NAME);
			}

			public override string ToString()
			{
			  return "MigrateSignalEventSubProcess";
			}
		}

		private class MigrationEventSubProcessTestConfigurationAnonymousInnerClass3 : MigrationEventSubProcessTestConfiguration
		{
			private readonly MigrationNestedEventSubProcessTest outerInstance;

			public MigrationEventSubProcessTestConfigurationAnonymousInnerClass3(MigrationNestedEventSubProcessTest outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override BpmnModelInstance SourceProcess
			{
				get
				{
				  return modify(ProcessModels.SUBPROCESS_PROCESS).addSubProcessTo(EventSubProcessModels.SUB_PROCESS_ID).triggerByEvent().embeddedSubProcess().startEvent(EVENT_SUB_PROCESS_START_ID).timerWithDate(TIMER_DATE).userTask(EVENT_SUB_PROCESS_TASK_ID).endEvent().subProcessDone().done();
				}
			}

			public override void assertMigration(MigrationTestRule testHelper)
			{
			  testHelper.assertEventSubProcessTimerJobRemoved(EVENT_SUB_PROCESS_START_ID);
			  testHelper.assertEventSubProcessTimerJobCreated(EVENT_SUB_PROCESS_START_ID);
			}

			public override string EventName
			{
				get
				{
				  return null;
				}
			}

			public override void triggerEventSubProcess(MigrationTestRule testHelper)
			{
			  testHelper.triggerTimer();
			}

			public override string ToString()
			{
			  return "MigrateTimerEventSubProcess";
			}
		}

		private class MigrationEventSubProcessTestConfigurationAnonymousInnerClass4 : MigrationEventSubProcessTestConfiguration
		{
			private readonly MigrationNestedEventSubProcessTest outerInstance;

			public MigrationEventSubProcessTestConfigurationAnonymousInnerClass4(MigrationNestedEventSubProcessTest outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override BpmnModelInstance SourceProcess
			{
				get
				{
				  return modify(ProcessModels.SUBPROCESS_PROCESS).addSubProcessTo(EventSubProcessModels.SUB_PROCESS_ID).triggerByEvent().embeddedSubProcess().startEvent(EVENT_SUB_PROCESS_START_ID).condition(EventSubProcessModels.VAR_CONDITION).userTask(EVENT_SUB_PROCESS_TASK_ID).endEvent().subProcessDone().done();
				}
			}

			public override string EventName
			{
				get
				{
				  return null;
				}
			}

			public override void triggerEventSubProcess(MigrationTestRule testHelper)
			{
			  testHelper.AnyVariable = testHelper.snapshotAfterMigration.ProcessInstanceId;
			}

			public override string ToString()
			{
			  return "MigrateConditionalEventSubProcess";
			}
		}

		public MigrationPlanCreationTest()
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
	  public const string ERROR_CODE = "Error";
	  public const string ESCALATION_CODE = "Escalation";

	  protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
	  protected internal MigrationTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(rule).around(testHelper);
	  public RuleChain ruleChain;

	  protected internal RuntimeService runtimeService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		runtimeService = rule.RuntimeService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExplicitInstructionGeneration()
	  public virtual void testExplicitInstructionGeneration()
	  {

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		assertThat(migrationPlan).hasSourceProcessDefinition(sourceProcessDefinition).hasTargetProcessDefinition(targetProcessDefinition).hasInstructions(migrate("userTask").to("userTask"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateNonExistingSourceDefinition()
	  public virtual void testMigrateNonExistingSourceDefinition()
	  {
		ProcessDefinition processDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		try
		{
		  runtimeService.createMigrationPlan("aNonExistingProcDefId", processDefinition.Id).mapActivities("userTask", "userTask").build();
		  fail("Should not succeed");
		}
		catch (BadUserRequestException e)
		{
		  assertExceptionMessage(e, "Source process definition with id 'aNonExistingProcDefId' does not exist");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateNullSourceDefinition()
	  public virtual void testMigrateNullSourceDefinition()
	  {
		ProcessDefinition processDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		try
		{
		  runtimeService.createMigrationPlan(null, processDefinition.Id).mapActivities("userTask", "userTask").build();
		  fail("Should not succeed");
		}
		catch (BadUserRequestException e)
		{
		  assertExceptionMessage(e, "Source process definition id is null");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateNonExistingTargetDefinition()
	  public virtual void testMigrateNonExistingTargetDefinition()
	  {
		ProcessDefinition processDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		try
		{
		  runtimeService.createMigrationPlan(processDefinition.Id, "aNonExistingProcDefId").mapActivities("userTask", "userTask").build();
		  fail("Should not succeed");
		}
		catch (BadUserRequestException e)
		{
		  assertExceptionMessage(e, "Target process definition with id 'aNonExistingProcDefId' does not exist");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateNullTargetDefinition()
	  public virtual void testMigrateNullTargetDefinition()
	  {
		ProcessDefinition processDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		try
		{
		  runtimeService.createMigrationPlan(processDefinition.Id, null).mapActivities("userTask", "userTask").build();
		  fail("Should not succeed");
		}
		catch (BadUserRequestException e)
		{
		  assertExceptionMessage(e, "Target process definition id is null");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateNonExistingSourceActivityId()
	  public virtual void testMigrateNonExistingSourceActivityId()
	  {
		ProcessDefinition sourceDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		try
		{
		  runtimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapActivities("thisActivityDoesNotExist", "userTask").build();
		  fail("Should not succeed");
		}
		catch (MigrationPlanValidationException e)
		{
		  assertThat(e.ValidationReport).hasInstructionFailures("thisActivityDoesNotExist", "Source activity 'thisActivityDoesNotExist' does not exist");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateNullSourceActivityId()
	  public virtual void testMigrateNullSourceActivityId()
	  {
		ProcessDefinition sourceDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		try
		{
		  runtimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapActivities(null, "userTask").build();
		  fail("Should not succeed");
		}
		catch (MigrationPlanValidationException e)
		{
		  assertThat(e.ValidationReport).hasInstructionFailures(null, "Source activity id is null");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateNonExistingTargetActivityId()
	  public virtual void testMigrateNonExistingTargetActivityId()
	  {
		ProcessDefinition sourceDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		try
		{
		  runtimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapActivities("userTask", "thisActivityDoesNotExist").build();
		  fail("Should not succeed");
		}
		catch (MigrationPlanValidationException e)
		{
		  assertThat(e.ValidationReport).hasInstructionFailures("userTask", "Target activity 'thisActivityDoesNotExist' does not exist");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateNullTargetActivityId()
	  public virtual void testMigrateNullTargetActivityId()
	  {
		ProcessDefinition sourceDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		try
		{
		  runtimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapActivities("userTask", null).build();
		  fail("Should not succeed");
		}
		catch (MigrationPlanValidationException e)
		{
		  assertThat(e.ValidationReport).hasInstructionFailures("userTask", "Target activity id is null");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateTaskToHigherScope()
	  public virtual void testMigrateTaskToHigherScope()
	  {
		ProcessDefinition sourceDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapActivities("userTask", "userTask").build();

		assertThat(migrationPlan).hasSourceProcessDefinition(sourceDefinition).hasTargetProcessDefinition(targetDefinition).hasInstructions(migrate("userTask").to("userTask"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateToUnsupportedActivityType()
	  public virtual void testMigrateToUnsupportedActivityType()
	  {

		ProcessDefinition sourceDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_RECEIVE_TASK_PROCESS);

		try
		{
		  runtimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapActivities("userTask", "receiveTask").build();
		  fail("Should not succeed");
		}
		catch (MigrationPlanValidationException e)
		{
		  assertThat(e.ValidationReport).hasInstructionFailures("userTask", "Activities have incompatible types (UserTaskActivityBehavior is not compatible with ReceiveTaskActivityBehavior)");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNotMigrateActivitiesOfDifferentType()
	  public virtual void testNotMigrateActivitiesOfDifferentType()
	  {
		ProcessDefinition sourceDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).swapElementIds("userTask", "subProcess"));

		try
		{
		  runtimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapActivities("userTask", "userTask").build();
		  fail("Should not succeed");
		}
		catch (MigrationPlanValidationException e)
		{
		  assertThat(e.ValidationReport).hasInstructionFailures("userTask", "Activities have incompatible types (UserTaskActivityBehavior is not " + "compatible with SubProcessActivityBehavior)");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNotMigrateBoundaryEventsOfDifferentType()
	  public virtual void testNotMigrateBoundaryEventsOfDifferentType()
	  {
		ProcessDefinition sourceDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").message(MESSAGE_NAME).done());
		ProcessDefinition targetDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").signal(SIGNAL_NAME).done());

		try
		{
		  runtimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapActivities("userTask", "userTask").mapActivities("boundary", "boundary").build();
		  fail("Should not succeed");
		}
		catch (MigrationPlanValidationException e)
		{
		  assertThat(e.ValidationReport).hasInstructionFailures("boundary", "Events are not of the same type (boundaryMessage != boundarySignal)");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateSubProcessToProcessDefinition()
	  public virtual void testMigrateSubProcessToProcessDefinition()
	  {
		ProcessDefinition sourceDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);
		ProcessDefinition targetDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		try
		{
		  runtimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapActivities("subProcess", targetDefinition.Id).build();
		  fail("Should not succeed");
		}
		catch (MigrationPlanValidationException e)
		{
		  assertThat(e.ValidationReport).hasInstructionFailures("subProcess", "Target activity '" + targetDefinition.Id + "' does not exist");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEqualActivitiesWithParallelMultiInstance()
	  public virtual void testMapEqualActivitiesWithParallelMultiInstance()
	  {
		// given
		BpmnModelInstance testProcess = modify(ProcessModels.ONE_TASK_PROCESS).getBuilderForElementById("userTask", typeof(UserTaskBuilder)).multiInstance().parallel().cardinality("3").multiInstanceDone().done();
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(testProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(testProcess);

		// when
		try
		{
		  runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();
		  fail("Should not succeed");
		}
		catch (MigrationPlanValidationException e)
		{
		  assertThat(e.ValidationReport).hasInstructionFailures("userTask", "Target activity 'userTask' is a descendant of multi-instance body 'userTask#multiInstanceBody' " + "that is not mapped from the source process definition.");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapEqualBoundaryEvents()
	  public virtual void testMapEqualBoundaryEvents()
	  {
		BpmnModelInstance testProcess = modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").message(MESSAGE_NAME).done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(testProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(testProcess);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").mapActivities("boundary", "boundary").build();

		assertThat(migrationPlan).hasSourceProcessDefinition(sourceProcessDefinition).hasTargetProcessDefinition(targetProcessDefinition).hasInstructions(migrate("userTask").to("userTask"), migrate("boundary").to("boundary"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapBoundaryEventsWithDifferentId()
	  public virtual void testMapBoundaryEventsWithDifferentId()
	  {
		BpmnModelInstance sourceProcess = modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").message(MESSAGE_NAME).done();
		BpmnModelInstance targetProcess = modify(sourceProcess).changeElementId("boundary", "newBoundary");

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").mapActivities("boundary", "newBoundary").build();

		assertThat(migrationPlan).hasSourceProcessDefinition(sourceProcessDefinition).hasTargetProcessDefinition(targetProcessDefinition).hasInstructions(migrate("userTask").to("userTask"), migrate("boundary").to("newBoundary"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapBoundaryToMigratedActivity()
	  public virtual void testMapBoundaryToMigratedActivity()
	  {
		BpmnModelInstance sourceProcess = modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").message(MESSAGE_NAME).done();
		BpmnModelInstance targetProcess = modify(sourceProcess).changeElementId("userTask", "newUserTask");

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "newUserTask").mapActivities("boundary", "boundary").build();

		assertThat(migrationPlan).hasSourceProcessDefinition(sourceProcessDefinition).hasTargetProcessDefinition(targetProcessDefinition).hasInstructions(migrate("userTask").to("newUserTask"), migrate("boundary").to("boundary"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapBoundaryToParallelActivity()
	  public virtual void testMapBoundaryToParallelActivity()
	  {
		BpmnModelInstance sourceProcess = modify(ProcessModels.PARALLEL_GATEWAY_PROCESS).activityBuilder("userTask1").boundaryEvent("boundary").message(MESSAGE_NAME).done();
		BpmnModelInstance targetProcess = modify(ProcessModels.PARALLEL_GATEWAY_PROCESS).activityBuilder("userTask2").boundaryEvent("boundary").message(MESSAGE_NAME).done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);

		try
		{
		  runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").mapActivities("boundary", "boundary").build();
		  fail("Should not succeed");
		}
		catch (MigrationPlanValidationException e)
		{
		  assertThat(e.ValidationReport).hasInstructionFailures("boundary", "The source activity's event scope (userTask1) must be mapped to the target activity's event scope (userTask2)");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapBoundaryToHigherScope()
	  public virtual void testMapBoundaryToHigherScope()
	  {
		BpmnModelInstance sourceProcess = modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").message(MESSAGE_NAME).done();
		BpmnModelInstance targetProcess = modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").message(MESSAGE_NAME).done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").mapActivities("boundary", "boundary").build();

		assertThat(migrationPlan).hasSourceProcessDefinition(sourceProcessDefinition).hasTargetProcessDefinition(targetProcessDefinition).hasInstructions(migrate("userTask").to("userTask"), migrate("boundary").to("boundary"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapBoundaryToLowerScope()
	  public virtual void testMapBoundaryToLowerScope()
	  {
		BpmnModelInstance sourceProcess = modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").message(MESSAGE_NAME).done();
		BpmnModelInstance targetProcess = modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").message(MESSAGE_NAME).done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").mapActivities("boundary", "boundary").build();

		assertThat(migrationPlan).hasSourceProcessDefinition(sourceProcessDefinition).hasTargetProcessDefinition(targetProcessDefinition).hasInstructions(migrate("userTask").to("userTask"), migrate("boundary").to("boundary"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapBoundaryToChildActivity()
	  public virtual void testMapBoundaryToChildActivity()
	  {
		BpmnModelInstance sourceProcess = modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").boundaryEvent("boundary").message(MESSAGE_NAME).done();
		BpmnModelInstance targetProcess = modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").message(MESSAGE_NAME).done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);

		try
		{
		  runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask", "userTask").mapActivities("boundary", "boundary").build();
		  fail("Should not succeed");
		}
		catch (MigrationPlanValidationException e)
		{
		  assertThat(e.ValidationReport).hasInstructionFailures("boundary", "The source activity's event scope (subProcess) must be mapped to the target activity's event scope (userTask)");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapBoundaryToParentActivity()
	  public virtual void testMapBoundaryToParentActivity()
	  {
		BpmnModelInstance sourceProcess = modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").message(MESSAGE_NAME).done();
		BpmnModelInstance targetProcess = modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").boundaryEvent("boundary").message(MESSAGE_NAME).done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);

		try
		{
		  runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask", "userTask").mapActivities("boundary", "boundary").build();

		  fail("Should not succeed");
		}
		catch (MigrationPlanValidationException e)
		{
		  assertThat(e.ValidationReport).hasInstructionFailures("boundary", "The source activity's event scope (userTask) must be mapped to the target activity's event scope (subProcess)", "The closest mapped ancestor 'subProcess' is mapped to scope 'subProcess' which is not an ancestor of target scope 'boundary'");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapAllBoundaryEvents()
	  public virtual void testMapAllBoundaryEvents()
	  {
		BpmnModelInstance testProcess = modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").boundaryEvent("error").error(ERROR_CODE).moveToActivity("subProcess").boundaryEvent("escalation").escalation(ESCALATION_CODE).done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(testProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(testProcess);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("error", "error").mapActivities("escalation", "escalation").mapActivities("userTask", "userTask").build();

		assertThat(migrationPlan).hasSourceProcessDefinition(sourceProcessDefinition).hasTargetProcessDefinition(targetProcessDefinition).hasInstructions(migrate("subProcess").to("subProcess"), migrate("error").to("error"), migrate("escalation").to("escalation"), migrate("userTask").to("userTask"));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapProcessDefinitionWithEventSubProcess()
	  public virtual void testMapProcessDefinitionWithEventSubProcess()
	  {
		BpmnModelInstance testProcess = modify(ProcessModels.ONE_TASK_PROCESS).addSubProcessTo(ProcessModels.PROCESS_KEY).triggerByEvent().embeddedSubProcess().startEvent().message(MESSAGE_NAME).endEvent().subProcessDone().done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(testProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(testProcess);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		assertThat(migrationPlan).hasSourceProcessDefinition(sourceProcessDefinition).hasTargetProcessDefinition(targetProcessDefinition).hasInstructions(migrate("userTask").to("userTask"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapSubProcessWithEventSubProcess()
	  public virtual void testMapSubProcessWithEventSubProcess()
	  {
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(EventSubProcessModels.NESTED_EVENT_SUB_PROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(EventSubProcessModels.NESTED_EVENT_SUB_PROCESS_PROCESS);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask", "userTask").build();

		assertThat(migrationPlan).hasSourceProcessDefinition(sourceProcessDefinition).hasTargetProcessDefinition(targetProcessDefinition).hasInstructions(migrate("subProcess").to("subProcess"), migrate("userTask").to("userTask"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapActivityWithUnmappedParentWhichHasAEventSubProcessChild()
	  public virtual void testMapActivityWithUnmappedParentWhichHasAEventSubProcessChild()
	  {
		BpmnModelInstance testProcess = modify(ProcessModels.SUBPROCESS_PROCESS).addSubProcessTo("subProcess").triggerByEvent().embeddedSubProcess().startEvent().message(MESSAGE_NAME).endEvent().subProcessDone().done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(testProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(testProcess);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		assertThat(migrationPlan).hasSourceProcessDefinition(sourceProcessDefinition).hasTargetProcessDefinition(targetProcessDefinition).hasInstructions(migrate("userTask").to("userTask"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapUserTaskInEventSubProcess()
	  public virtual void testMapUserTaskInEventSubProcess()
	  {
		BpmnModelInstance testProcess = modify(ProcessModels.SUBPROCESS_PROCESS).addSubProcessTo("subProcess").triggerByEvent().embeddedSubProcess().startEvent().message(MESSAGE_NAME).userTask("innerTask").endEvent().subProcessDone().done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(testProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(testProcess);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").mapActivities("innerTask", "innerTask").build();

		  assertThat(migrationPlan).hasSourceProcessDefinition(sourceProcessDefinition).hasTargetProcessDefinition(targetProcessDefinition).hasInstructions(migrate("userTask").to("userTask"), migrate("innerTask").to("innerTask"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNotMapActivitiesMoreThanOnce()
	  public virtual void testNotMapActivitiesMoreThanOnce()
	  {
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);

		try
		{
		  runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask1", "userTask2").build();
		  fail("Should not succeed");
		}
		catch (MigrationPlanValidationException e)
		{
		  assertThat(e.ValidationReport).hasInstructionFailures("userTask1", "There are multiple mappings for source activity id 'userTask1'", "There are multiple mappings for source activity id 'userTask1'");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCannotUpdateEventTriggerForNonEvent()
	  public virtual void testCannotUpdateEventTriggerForNonEvent()
	  {

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		try
		{
		  runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").updateEventTrigger().build();
		  fail("Should not succeed");
		}
		catch (MigrationPlanValidationException e)
		{
		  assertThat(e.ValidationReport).hasInstructionFailures("userTask", "Cannot update event trigger because the activity does not define a persistent event trigger");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCannotUpdateEventTriggerForEventSubProcess()
	  public virtual void testCannotUpdateEventTriggerForEventSubProcess()
	  {
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(EventSubProcessModels.TIMER_EVENT_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(EventSubProcessModels.TIMER_EVENT_SUBPROCESS_PROCESS);

		try
		{
		  runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("eventSubProcess", "eventSubProcess").updateEventTrigger().build();
		  fail("Should not succeed");
		}
		catch (MigrationPlanValidationException e)
		{
		  assertThat(e.ValidationReport).hasInstructionFailures("eventSubProcess", "Cannot update event trigger because the activity does not define a persistent event trigger");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCanUpdateEventTriggerForEventSubProcessStartEvent()
	  public virtual void testCanUpdateEventTriggerForEventSubProcessStartEvent()
	  {
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(EventSubProcessModels.TIMER_EVENT_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(EventSubProcessModels.TIMER_EVENT_SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("eventSubProcessStart", "eventSubProcessStart").updateEventTrigger().build();

		assertThat(migrationPlan).hasSourceProcessDefinition(sourceProcessDefinition).hasTargetProcessDefinition(targetProcessDefinition).hasInstructions(migrate("eventSubProcessStart").to("eventSubProcessStart").updateEventTrigger(true));
	  }

	  protected internal virtual void assertExceptionMessage(Exception e, string message)
	  {
		assertThat(e.Message, CoreMatchers.containsString(message));
	  }

	}

}