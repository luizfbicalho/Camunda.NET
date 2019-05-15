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
namespace org.camunda.bpm.engine.test.api.runtime.migration
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;


	using DateUtils = org.apache.commons.lang3.time.DateUtils;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	public class MigrationTimerBoundryEventTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationTimerBoundryEventTest()
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


	  private const string DUE_DATE_IN_THE_PAST = "2018-02-11T12:13:14Z";
	  protected internal static readonly SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
	  protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
	  protected internal MigrationTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(rule).around(testHelper);
	  public RuleChain ruleChain;
	  protected internal ManagementService managementService;
	  protected internal RuntimeService runtimeService;
	  protected internal TaskService taskService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		managementService = rule.ManagementService;
		runtimeService = rule.RuntimeService;
		taskService = rule.TaskService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanUpJobs()
	  public virtual void cleanUpJobs()
	  {
		IList<Job> jobs = managementService.createJobQuery().list();
		if (jobs.Count > 0)
		{
		  foreach (Job job in jobs)
		  {
			managementService.deleteJob(job.Id);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrationNonInterruptingTimerEvent()
	  public virtual void testMigrationNonInterruptingTimerEvent()
	  {
		// given
		BpmnModelInstance model = createModel(false, DUE_DATE_IN_THE_PAST);
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(model);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(model);

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);
		managementService.executeJob(job.Id);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		IList<Job> list = managementService.createJobQuery().list();
		assertTrue(list.Count == 0);
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey("afterTimer").count());
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey("userTask").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrationInterruptingTimerEvent()
	  public virtual void testMigrationInterruptingTimerEvent()
	  {
		// given
		BpmnModelInstance model = createModel(true, DUE_DATE_IN_THE_PAST);
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(model);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(model);

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);
		managementService.executeJob(job.Id);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		IList<Job> list = managementService.createJobQuery().list();
		assertTrue(list.Count == 0);
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey("afterTimer").count());
		assertEquals(0, taskService.createTaskQuery().taskDefinitionKey("userTask").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrationNonTriggeredInterruptingTimerEvent()
	  public virtual void testMigrationNonTriggeredInterruptingTimerEvent()
	  {
		// given
		DateTime futureDueDate = DateUtils.addYears(ClockUtil.CurrentTime, 1);
		BpmnModelInstance model = createModel(true, sdf.format(futureDueDate));
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(model);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(model);

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		IList<Job> list = managementService.createJobQuery().list();
		assertEquals(1, list.Count);
		assertEquals(0, taskService.createTaskQuery().taskDefinitionKey("afterTimer").count());
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey("userTask").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrationTwoNonInterruptingTimerEvents()
	  public virtual void testMigrationTwoNonInterruptingTimerEvents()
	  {
		// given
		DateTime futureDueDate = DateUtils.addYears(ClockUtil.CurrentTime, 1);
		BpmnModelInstance model = Bpmn.createExecutableProcess().startEvent("startEvent").userTask("userTask").name("User Task").boundaryEvent("timerPast").cancelActivity(false).timerWithDate(DUE_DATE_IN_THE_PAST).userTask("past").moveToActivity("userTask").boundaryEvent("timerFuture").cancelActivity(false).timerWithDate(sdf.format(futureDueDate)).userTask("future").done();
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(model);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(model);

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		Job job = managementService.createJobQuery().duedateLowerThan(ClockUtil.CurrentTime).singleResult();
		assertNotNull(job);
		managementService.executeJob(job.Id);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		IList<Job> list = managementService.createJobQuery().list();
		assertEquals(1, list.Count);
		assertEquals(1, managementService.createJobQuery().duedateHigherThan(ClockUtil.CurrentTime).count());
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey("past").count());
		assertEquals(0, taskService.createTaskQuery().taskDefinitionKey("future").count());
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey("userTask").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrationWithTargetNonInterruptingTimerEvent()
	  public virtual void testMigrationWithTargetNonInterruptingTimerEvent()
	  {
		// given
		BpmnModelInstance sourceModel = Bpmn.createExecutableProcess().startEvent("startEvent").userTask("userTask").name("User Task").userTask("afterTimer").endEvent("endEvent").done();
		BpmnModelInstance targetModel = createModel(false, DUE_DATE_IN_THE_PAST);
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceModel);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetModel);

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey("userTask").count());
		assertEquals(0, taskService.createTaskQuery().taskDefinitionKey("afterTimer").count());
		assertEquals(1, managementService.createJobQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrationWithSourceNonInterruptingTimerEvent()
	  public virtual void testMigrationWithSourceNonInterruptingTimerEvent()
	  {
		// given
		BpmnModelInstance sourceModel = createModel(false, DUE_DATE_IN_THE_PAST);
		BpmnModelInstance targetModel = Bpmn.createExecutableProcess().startEvent("startEvent").userTask("userTask").name("User Task").userTask("afterTimer").endEvent("endEvent").done();
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceModel);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetModel);

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);
		managementService.executeJob(job.Id);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		IList<Job> list = managementService.createJobQuery().list();
		assertTrue(list.Count == 0);
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey("userTask").count());
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey("afterTimer").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrationTwoToOneNonInterruptingTimerEvents()
	  public virtual void testMigrationTwoToOneNonInterruptingTimerEvents()
	  {
		// given
		DateTime futureDueDate = DateUtils.addYears(ClockUtil.CurrentTime, 1);
		BpmnModelInstance sourceModel = Bpmn.createExecutableProcess().startEvent("startEvent").userTask("userTask").name("User Task").boundaryEvent("timerPast").cancelActivity(false).timerWithDate(DUE_DATE_IN_THE_PAST).userTask("past").moveToActivity("userTask").boundaryEvent("timerFuture").cancelActivity(false).timerWithDate(sdf.format(futureDueDate)).userTask("future").done();
		BpmnModelInstance targetModel = Bpmn.createExecutableProcess().startEvent("startEvent").userTask("userTask").name("User Task").boundaryEvent("timerFuture").cancelActivity(false).timerWithDate(sdf.format(futureDueDate)).userTask("future").done();
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceModel);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetModel);

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		Job job = managementService.createJobQuery().activityId("timerPast").singleResult();
		assertNotNull(job);
		managementService.executeJob(job.Id);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().mapActivities("timerPast", "timerFuture").mapActivities("past", "future").build();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		IList<Job> list = managementService.createJobQuery().duedateHigherThan(ClockUtil.CurrentTime).list();
		assertEquals(1, list.Count);
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey("userTask").count());
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey("future").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrationNonInterruptingTimerEventDifferentActivityId()
	  public virtual void testMigrationNonInterruptingTimerEventDifferentActivityId()
	  {
		// given
		BpmnModelInstance sourceModel = createModel(false, DUE_DATE_IN_THE_PAST);
		BpmnModelInstance targetModel = Bpmn.createExecutableProcess().startEvent("startEvent").userTask("userTask").name("User Task").boundaryEvent("timer2").cancelActivity(false).timerWithDate(DUE_DATE_IN_THE_PAST).userTask("afterTimer").endEvent("endEvent").done();
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceModel);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetModel);

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);
		managementService.executeJob(job.Id);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().mapActivities("timer", "timer2").build();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		IList<Job> list = managementService.createJobQuery().list();
		assertTrue(list.Count == 0);
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey("afterTimer").count());
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey("userTask").count());
	  }

	  protected internal virtual BpmnModelInstance createModel(bool isCancelActivity, string date)
	  {
		BpmnModelInstance model = Bpmn.createExecutableProcess().startEvent("startEvent").userTask("userTask").name("User Task").boundaryEvent("timer").cancelActivity(isCancelActivity).timerWithDate(date).userTask("afterTimer").endEvent("endEvent").done();
		return model;
	  }
	}

}