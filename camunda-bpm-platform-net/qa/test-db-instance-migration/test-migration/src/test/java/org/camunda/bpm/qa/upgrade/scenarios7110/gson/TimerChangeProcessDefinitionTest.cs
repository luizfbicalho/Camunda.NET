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
namespace org.camunda.bpm.qa.upgrade.scenarios7110.gson
{
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessEngineRule = org.camunda.bpm.engine.test.ProcessEngineRule;
	using After = org.junit.After;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	[ScenarioUnderTest("TimerChangeProcessDefinitionScenario"), Origin("7.11.0")]
	public class TimerChangeProcessDefinitionTest
	{

	  protected internal static readonly DateTime FIXED_DATE_ONE = new DateTime(1363608000000L);
	  protected internal static readonly DateTime FIXED_DATE_TWO = new DateTime(1363608500000L);
	  protected internal static readonly DateTime FIXED_DATE_THREE = new DateTime(1363608600000L);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.ProcessEngineRule("camunda.cfg.xml");
	  public ProcessEngineRule engineRule = new ProcessEngineRule("camunda.cfg.xml");

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void activateDefinitions()
	  public virtual void activateDefinitions()
	  {
		engineRule.RepositoryService.activateProcessDefinitionByKey("oneTaskProcessTimer_710");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void resetClock()
	  public virtual void resetClock()
	  {
		ClockUtil.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ScenarioUnderTest("initTimerChangeProcessDefinition.1") @Test public void testTimerChangeProcessDefinitionById()
	  [ScenarioUnderTest("initTimerChangeProcessDefinition.1")]
	  public virtual void testTimerChangeProcessDefinitionById()
	  {
		// assume
		ProcessInstance processInstance = engineRule.RuntimeService.createProcessInstanceQuery().processInstanceBusinessKey("TimerChangeProcessDefinitionScenarioV1").singleResult();

		assertThat(processInstance.Suspended, @is(false));

		Job job = engineRule.ManagementService.createJobQuery().timers().duedateLowerThan(new DateTime(1_363_608_001_000L)).duedateHigherThan(new DateTime(1_363_607_999_000L)).singleResult();

		ClockUtil.CurrentTime = FIXED_DATE_ONE;

		// when
		engineRule.ManagementService.executeJob(job.Id);

		processInstance = engineRule.RuntimeService.createProcessInstanceQuery().processInstanceBusinessKey("TimerChangeProcessDefinitionScenarioV1").singleResult();

		ProcessDefinition processDefinition = engineRule.RepositoryService.createProcessDefinitionQuery().processDefinitionId(processInstance.ProcessDefinitionId).singleResult();

		// then
		assertThat(processInstance.Suspended, @is(true));
		assertThat(processDefinition.Suspended, @is(true));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ScenarioUnderTest("initTimerChangeProcessDefinition.2") @Test public void testTimerChangeProcessDefinitionByKeyWithTenantId()
	  [ScenarioUnderTest("initTimerChangeProcessDefinition.2")]
	  public virtual void testTimerChangeProcessDefinitionByKeyWithTenantId()
	  {
		// assume
		ProcessInstance processInstance = engineRule.RuntimeService.createProcessInstanceQuery().processInstanceBusinessKey("TimerChangeProcessDefinitionScenarioV2").singleResult();

		assertThat(processInstance.Suspended, @is(false));

		Job job = engineRule.ManagementService.createJobQuery().timers().duedateLowerThan(new DateTime(1_363_608_501_000L)).duedateHigherThan(new DateTime(1_363_608_499_000L)).singleResult();

		ClockUtil.CurrentTime = FIXED_DATE_TWO;

		// when
		engineRule.ManagementService.executeJob(job.Id);

		processInstance = engineRule.RuntimeService.createProcessInstanceQuery().processInstanceBusinessKey("TimerChangeProcessDefinitionScenarioV2").singleResult();

		ProcessDefinition processDefinition = engineRule.RepositoryService.createProcessDefinitionQuery().processDefinitionId(processInstance.ProcessDefinitionId).singleResult();

		// then
		assertThat(processInstance.Suspended, @is(true));
		assertThat(processDefinition.Suspended, @is(true));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ScenarioUnderTest("initTimerChangeProcessDefinition.3") @Test public void testTimerChangeProcessDefinitionByKeyWithoutTenantId()
	  [ScenarioUnderTest("initTimerChangeProcessDefinition.3")]
	  public virtual void testTimerChangeProcessDefinitionByKeyWithoutTenantId()
	  {
		ProcessDefinition processDefinitionWithoutTenantId = engineRule.RepositoryService.createProcessDefinitionQuery().processDefinitionKey("oneTaskProcessTimer_710").withoutTenantId().singleResult();

		ProcessDefinition processDefinitionWithTenantId = engineRule.RepositoryService.createProcessDefinitionQuery().processDefinitionKey("oneTaskProcessTimer_710").tenantIdIn("aTenantId").singleResult();

		// assume
		assertThat(processDefinitionWithoutTenantId.Suspended, @is(false));
		assertThat(processDefinitionWithTenantId.Suspended, @is(false));

		// given
		Job job = engineRule.ManagementService.createJobQuery().timers().duedateLowerThan(new DateTime(1_363_608_601_000L)).duedateHigherThan(new DateTime(1_363_608_599_000L)).singleResult();

		ClockUtil.CurrentTime = FIXED_DATE_THREE;

		// when
		engineRule.ManagementService.executeJob(job.Id);

		processDefinitionWithoutTenantId = engineRule.RepositoryService.createProcessDefinitionQuery().processDefinitionKey("oneTaskProcessTimer_710").withoutTenantId().singleResult();

		processDefinitionWithTenantId = engineRule.RepositoryService.createProcessDefinitionQuery().processDefinitionKey("oneTaskProcessTimer_710").tenantIdIn("aTenantId").singleResult();

		// then
		assertThat(processDefinitionWithTenantId.Suspended, @is(false));
		assertThat(processDefinitionWithoutTenantId.Suspended, @is(true));
	  }

	}

}