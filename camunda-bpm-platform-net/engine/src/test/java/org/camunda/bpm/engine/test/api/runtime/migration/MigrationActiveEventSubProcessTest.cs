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

	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using BpmnEventFactory = org.camunda.bpm.engine.test.api.runtime.migration.util.BpmnEventFactory;
	using ConditionalEventFactory = org.camunda.bpm.engine.test.api.runtime.migration.util.ConditionalEventFactory;
	using MessageEventFactory = org.camunda.bpm.engine.test.api.runtime.migration.util.MessageEventFactory;
	using MigratingBpmnEventTrigger = org.camunda.bpm.engine.test.api.runtime.migration.util.MigratingBpmnEventTrigger;
	using SignalEventFactory = org.camunda.bpm.engine.test.api.runtime.migration.util.SignalEventFactory;
	using TimerEventFactory = org.camunda.bpm.engine.test.api.runtime.migration.util.TimerEventFactory;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;
	using Parameter = org.junit.runners.Parameterized.Parameter;
	using Parameters = org.junit.runners.Parameterized.Parameters;

	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class MigrationActiveEventSubProcessTest
	public class MigrationActiveEventSubProcessTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationActiveEventSubProcessTest()
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

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters public static java.util.Collection<Object[]> data()
		public static ICollection<object[]> data()
		{
		  return Arrays.asList(new object[][]
		  {
			  new object[]{new TimerEventFactory()},
			  new object[]{new MessageEventFactory()},
			  new object[]{new SignalEventFactory()},
			  new object[]{new ConditionalEventFactory()}
		  });
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter public org.camunda.bpm.engine.test.api.runtime.migration.util.BpmnEventFactory eventFactory;
	  public BpmnEventFactory eventFactory;

	  protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
	  protected internal MigrationTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(rule).around(testHelper);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		ClockUtil.CurrentTime = DateTime.Now; // lock time so that timer job is effectively not updated
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateActiveCompensationEventSubProcess()
	  public virtual void testMigrateActiveCompensationEventSubProcess()
	  {
		// given
		BpmnModelInstance processModel = ProcessModels.ONE_TASK_PROCESS.clone();
		MigratingBpmnEventTrigger eventTrigger = eventFactory.addEventSubProcess(rule.ProcessEngine, processModel, ProcessModels.PROCESS_KEY, "eventSubProcess", "eventSubProcessStart");
		ModifiableBpmnModelInstance.wrap(processModel).startEventBuilder("eventSubProcessStart").userTask("eventSubProcessTask").endEvent().done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(processModel);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(processModel);

		ProcessInstance processInstance = rule.RuntimeService.createProcessInstanceById(sourceProcessDefinition.Id).startBeforeActivity("eventSubProcessStart").executeWithVariablesInReturn();

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("eventSubProcess", "eventSubProcess").mapActivities("eventSubProcessStart", "eventSubProcessStart").updateEventTrigger().mapActivities("eventSubProcessTask", "eventSubProcessTask").build();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		eventTrigger.assertEventTriggerMigrated(testHelper, "eventSubProcessStart");

		// and it is possible to complete the process instance
		testHelper.completeTask("eventSubProcessTask");
		testHelper.assertProcessEnded(processInstance.Id);
	  }
	}

}