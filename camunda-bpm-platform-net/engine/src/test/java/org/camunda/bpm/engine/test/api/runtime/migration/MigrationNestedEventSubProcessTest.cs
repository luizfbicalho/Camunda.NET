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
namespace org.camunda.bpm.engine.test.api.runtime.migration
{
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using EventSubProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.EventSubProcessModels;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.describeActivityInstanceTree;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ExecutionAssert.describeExecutionTree;

	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class MigrationNestedEventSubProcessTest
	public class MigrationNestedEventSubProcessTest
	{

	  protected internal const string USER_TASK_ID = "userTask";
	  protected internal const string EVENT_SUB_PROCESS_START_ID = "eventSubProcessStart";
	  protected internal const string EVENT_SUB_PROCESS_TASK_ID = "eventSubProcessTask";
	  public const string TIMER_DATE = "2016-02-11T12:13:14Z";

	  protected internal abstract class MigrationEventSubProcessTestConfiguration
	  {
		public abstract BpmnModelInstance SourceProcess {get;}

		public abstract string EventName {get;}

		public virtual void assertMigration(MigrationTestRule testHelper)
		{
		  testHelper.assertEventSubscriptionRemoved(EVENT_SUB_PROCESS_START_ID, EventName);
		  testHelper.assertEventSubscriptionCreated(EVENT_SUB_PROCESS_START_ID, EventName);
		}

		public abstract void triggerEventSubProcess(MigrationTestRule testHelper);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameters(name = "{index}: {0}") public static java.util.Collection<Object[]> data()
	  public static ICollection<object[]> data()
	  {
//JAVA TO C# CONVERTER TODO TASK: The following anonymous inner class could not be converted:
//		return java.util.Arrays.asList(new Object[][]{ { new MigrationEventSubProcessTestConfiguration()
	//	{
	//		  @@Override public BpmnModelInstance getSourceProcess()
	//		  {
	//			return EventSubProcessModels.NESTED_EVENT_SUB_PROCESS_PROCESS;
	//		  }
	//
	//		  @@Override public String getEventName()
	//		  {
	//			return EventSubProcessModels.MESSAGE_NAME;
	//		  }
	//
	//		  @@Override public void triggerEventSubProcess(MigrationTestRule testHelper)
	//		  {
	//			testHelper.correlateMessage(EventSubProcessModels.MESSAGE_NAME);
	//		  }
	//
	//		  @@Override public String toString()
	//		  {
	//			return "MigrateMessageEventSubProcess";
	//		  }
	//		}
	  }
	 ,
	 {
		  //signal event sub process configuration
			  new MigrationEventSubProcessTestConfigurationAnonymousInnerClass2(this)
	 }
		 ,
		 {
		  //timer event sub process configuration
			  new MigrationEventSubProcessTestConfigurationAnonymousInnerClass3(this)
		 }
		 ,
		 {
		  //conditional event sub process configuration
			  new MigrationEventSubProcessTestConfigurationAnonymousInnerClass4(this)
		 }
	}
	   );
}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter public MigrationEventSubProcessTestConfiguration configuration;
	  public MigrationEventSubProcessTestConfiguration configuration;

	  protected ProcessEngineRule rule = new ProvidedProcessEngineRule();
	  protected MigrationTestRule testHelper = new MigrationTestRule(rule);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(rule).around(testHelper);
	  public RuleChain ruleChain = RuleChain.outerRule(rule).around(testHelper);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapUserTaskSiblingOfEventSubProcess()
	  public void testMapUserTaskSiblingOfEventSubProcess()
	  {

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(configuration.SourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(configuration.SourceProcess);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_ID, USER_TASK_ID).build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child(USER_TASK_ID).scope().done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginScope(EventSubProcessModels.SUB_PROCESS_ID).activity(USER_TASK_ID, testHelper.getSingleActivityInstanceBeforeMigration(USER_TASK_ID).Id).done());

		configuration.assertMigration(testHelper);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask(USER_TASK_ID);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapUserTaskSiblingOfEventSubProcessAndTriggerEvent()
	  public void testMapUserTaskSiblingOfEventSubProcessAndTriggerEvent()
	  {
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(configuration.SourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(configuration.SourceProcess);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_ID, USER_TASK_ID).build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then it is possible to trigger event sub process and successfully complete the migrated instance
		configuration.triggerEventSubProcess(testHelper);
		testHelper.completeTask(EVENT_SUB_PROCESS_TASK_ID);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }
	}

}