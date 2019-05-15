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
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.describeActivityInstanceTree;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ExecutionAssert.describeExecutionTree;

	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using CallActivityModels = org.camunda.bpm.engine.test.api.runtime.migration.models.CallActivityModels;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigrationCallActivityTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationCallActivityTest()
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


	  protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
	  protected internal MigrationTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(rule).around(testHelper);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void deployOneTaskProcess()
	  public virtual void deployOneTaskProcess()
	  {
		testHelper.deployAndGetDefinition(modify(ProcessModels.ONE_TASK_PROCESS).changeElementId(ProcessModels.PROCESS_KEY, "oneTaskProcess"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void deployOneTaskCase()
	  public virtual void deployOneTaskCase()
	  {
		testHelper.deploy("org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCallBpmnProcessSimpleMigration()
	  public virtual void testCallBpmnProcessSimpleMigration()
	  {
		// given
		BpmnModelInstance model = CallActivityModels.oneBpmnCallActivityProcess("oneTaskProcess");

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(model);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(model);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("callActivity", "callActivity").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("callActivity").scope().id(testHelper.getSingleExecutionIdForActivityBeforeMigration("callActivity")).done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("callActivity", testHelper.getSingleActivityInstanceBeforeMigration("callActivity").Id).done());

		// and it is possible to complete the called process instance
		testHelper.completeTask("userTask");
		// and the calling process instance
		testHelper.completeTask("userTask");

		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCallCmmnCaseSimpleMigration()
	  public virtual void testCallCmmnCaseSimpleMigration()
	  {
		// given
		BpmnModelInstance model = CallActivityModels.oneCmmnCallActivityProcess("oneTaskCase");

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(model);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(model);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("callActivity", "callActivity").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("callActivity").scope().id(testHelper.getSingleExecutionIdForActivityBeforeMigration("callActivity")).done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("callActivity", testHelper.getSingleActivityInstanceBeforeMigration("callActivity").Id).done());

		// and it is possible to complete the called case instance
		CaseExecution caseExecution = rule.CaseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();

		testHelper.completeTask("PI_HumanTask_1");
		// and the calling process instance
		testHelper.completeTask("userTask");

		testHelper.assertProcessEnded(processInstance.Id);

		// and close the called case instance
		rule.CaseService.withCaseExecution(caseExecution.CaseInstanceId).close();
		testHelper.assertCaseEnded(caseExecution.CaseInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCallBpmnProcessAddParentScope()
	  public virtual void testCallBpmnProcessAddParentScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CallActivityModels.oneBpmnCallActivityProcess("oneTaskProcess"));
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CallActivityModels.subProcessBpmnCallActivityProcess("oneTaskProcess"));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("callActivity", "callActivity").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child(null).scope().child("callActivity").scope().id(testHelper.getSingleExecutionIdForActivityBeforeMigration("callActivity")).done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginScope("subProcess").activity("callActivity", testHelper.getSingleActivityInstanceBeforeMigration("callActivity").Id).done());

		// and it is possible to complete the called process instance
		testHelper.completeTask("userTask");
		// and the calling process instance
		testHelper.completeTask("userTask");

		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCallBpmnProcessParallelMultiInstance()
	  public virtual void testCallBpmnProcessParallelMultiInstance()
	  {
		// given
		BpmnModelInstance model = modify(CallActivityModels.oneBpmnCallActivityProcess("oneTaskProcess")).activityBuilder("callActivity").multiInstance().parallel().cardinality("1").done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(model);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(model);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("callActivity#multiInstanceBody", "callActivity#multiInstanceBody").mapActivities("callActivity", "callActivity").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child(null).scope().id(testHelper.getSingleExecutionIdForActivityBeforeMigration("callActivity#multiInstanceBody")).child("callActivity").concurrent().noScope().id(testHelper.getSingleExecutionIdForActivityBeforeMigration("callActivity")).done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginMiBody("callActivity").activity("callActivity", testHelper.getSingleActivityInstanceBeforeMigration("callActivity").Id).done());

		// and the link between calling and called instance is maintained correctly

		testHelper.assertSuperExecutionOfProcessInstance(rule.RuntimeService.createProcessInstanceQuery().processDefinitionKey("oneTaskProcess").singleResult().Id, testHelper.getSingleExecutionIdForActivityAfterMigration("callActivity"));

		// and it is possible to complete the called process instance
		testHelper.completeTask("userTask");
		// and the calling process instance
		testHelper.completeTask("userTask");

		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCallCmmnCaseParallelMultiInstance()
	  public virtual void testCallCmmnCaseParallelMultiInstance()
	  {
		// given
		BpmnModelInstance model = modify(CallActivityModels.oneCmmnCallActivityProcess("oneTaskCase")).activityBuilder("callActivity").multiInstance().parallel().cardinality("1").done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(model);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(model);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("callActivity#multiInstanceBody", "callActivity#multiInstanceBody").mapActivities("callActivity", "callActivity").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child(null).scope().id(testHelper.getSingleExecutionIdForActivityBeforeMigration("callActivity#multiInstanceBody")).child("callActivity").concurrent().noScope().id(testHelper.getSingleExecutionIdForActivityBeforeMigration("callActivity")).done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginMiBody("callActivity").activity("callActivity", testHelper.getSingleActivityInstanceBeforeMigration("callActivity").Id).done());

		// and the link between calling and called instance is maintained correctly
		testHelper.assertSuperExecutionOfCaseInstance(rule.CaseService.createCaseInstanceQuery().caseDefinitionKey("oneTaskCase").singleResult().Id, testHelper.getSingleExecutionIdForActivityAfterMigration("callActivity"));

		// and it is possible to complete the called case instance
		CaseExecution caseExecution = rule.CaseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();

		testHelper.completeTask("PI_HumanTask_1");

		// and the calling process instance
		testHelper.completeTask("userTask");

		testHelper.assertProcessEnded(processInstance.Id);

		// and close the called case instance
		rule.CaseService.withCaseExecution(caseExecution.CaseInstanceId).close();
		testHelper.assertCaseEnded(caseExecution.CaseInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCallBpmnProcessParallelMultiInstanceRemoveMiBody()
	  public virtual void testCallBpmnProcessParallelMultiInstanceRemoveMiBody()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(CallActivityModels.oneBpmnCallActivityProcess("oneTaskProcess")).activityBuilder("callActivity").multiInstance().parallel().cardinality("1").done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CallActivityModels.oneBpmnCallActivityProcess("oneTaskProcess"));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("callActivity", "callActivity").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("callActivity").scope().done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("callActivity", testHelper.getSingleActivityInstanceBeforeMigration("callActivity").Id).done());

		// then the link between calling and called instance is maintained correctly

		testHelper.assertSuperExecutionOfProcessInstance(rule.RuntimeService.createProcessInstanceQuery().processDefinitionKey("oneTaskProcess").singleResult().Id, testHelper.getSingleExecutionIdForActivityAfterMigration("callActivity"));

		// then it is possible to complete the called process instance
		testHelper.completeTask("userTask");
		// and the calling process instance
		testHelper.completeTask("userTask");

		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCallCmmnCaseParallelMultiInstanceRemoveMiBody()
	  public virtual void testCallCmmnCaseParallelMultiInstanceRemoveMiBody()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(CallActivityModels.oneCmmnCallActivityProcess("oneTaskCase")).activityBuilder("callActivity").multiInstance().parallel().cardinality("1").done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CallActivityModels.oneCmmnCallActivityProcess("oneTaskCase"));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("callActivity", "callActivity").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("callActivity").scope().done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("callActivity", testHelper.getSingleActivityInstanceBeforeMigration("callActivity").Id).done());


		// and the link between calling and called instance is maintained correctly
		testHelper.assertSuperExecutionOfCaseInstance(rule.CaseService.createCaseInstanceQuery().caseDefinitionKey("oneTaskCase").singleResult().Id, testHelper.getSingleExecutionIdForActivityAfterMigration("callActivity"));

		// and it is possible to complete the called case instance
		CaseExecution caseExecution = rule.CaseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();

		testHelper.completeTask("PI_HumanTask_1");

		// and the calling process instance
		testHelper.completeTask("userTask");

		testHelper.assertProcessEnded(processInstance.Id);

		// and close the called case instance
		rule.CaseService.withCaseExecution(caseExecution.CaseInstanceId).close();
		testHelper.assertCaseEnded(caseExecution.CaseInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCallBpmnProcessSequentialMultiInstanceRemoveMiBody()
	  public virtual void testCallBpmnProcessSequentialMultiInstanceRemoveMiBody()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(CallActivityModels.oneBpmnCallActivityProcess("oneTaskProcess")).activityBuilder("callActivity").multiInstance().sequential().cardinality("1").done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CallActivityModels.oneBpmnCallActivityProcess("oneTaskProcess"));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("callActivity", "callActivity").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("callActivity").scope().done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("callActivity", testHelper.getSingleActivityInstanceBeforeMigration("callActivity").Id).done());

		// then the link between calling and called instance is maintained correctly

		testHelper.assertSuperExecutionOfProcessInstance(rule.RuntimeService.createProcessInstanceQuery().processDefinitionKey("oneTaskProcess").singleResult().Id, testHelper.getSingleExecutionIdForActivityAfterMigration("callActivity"));

		// then it is possible to complete the called process instance
		testHelper.completeTask("userTask");
		// and the calling process instance
		testHelper.completeTask("userTask");

		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCallCmmnCaseSequentialMultiInstanceRemoveMiBody()
	  public virtual void testCallCmmnCaseSequentialMultiInstanceRemoveMiBody()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(CallActivityModels.oneCmmnCallActivityProcess("oneTaskCase")).activityBuilder("callActivity").multiInstance().sequential().cardinality("1").done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CallActivityModels.oneCmmnCallActivityProcess("oneTaskCase"));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("callActivity", "callActivity").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("callActivity").scope().done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("callActivity", testHelper.getSingleActivityInstanceBeforeMigration("callActivity").Id).done());

		// then the link between calling and called instance is maintained correctly
		testHelper.assertSuperExecutionOfCaseInstance(rule.CaseService.createCaseInstanceQuery().caseDefinitionKey("oneTaskCase").singleResult().Id, testHelper.getSingleExecutionIdForActivityAfterMigration("callActivity"));

		// and it is possible to complete the called case instance
		CaseExecution caseExecution = rule.CaseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();

		testHelper.completeTask("PI_HumanTask_1");

		// and the calling process instance
		testHelper.completeTask("userTask");

		testHelper.assertProcessEnded(processInstance.Id);

		// and close the called case instance
		rule.CaseService.withCaseExecution(caseExecution.CaseInstanceId).close();
		testHelper.assertCaseEnded(caseExecution.CaseInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCallBpmnProcessReconfigureCallActivity()
	  public virtual void testCallBpmnProcessReconfigureCallActivity()
	  {
		// given
		BpmnModelInstance model = CallActivityModels.oneBpmnCallActivityProcess("oneTaskProcess");

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(model);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(model).callActivityBuilder("callActivity").calledElement("foo").done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("callActivity", "callActivity").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then the called instance has not changed (e.g. not been migrated to a different process definition)
		ProcessInstance calledInstance = rule.RuntimeService.createProcessInstanceQuery().processDefinitionKey("oneTaskProcess").singleResult();
		Assert.assertNotNull(calledInstance);

		// and it is possible to complete the called process instance
		testHelper.completeTask("userTask");
		// and the calling process instance
		testHelper.completeTask("userTask");

		testHelper.assertProcessEnded(processInstance.Id);
	  }

	}

}