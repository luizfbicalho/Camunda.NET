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
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
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
	[ScenarioUnderTest("TimerChangeJobDefinitionScenario"), Origin("7.11.0")]
	public class TimerChangeJobDefinitionTest
	{

	  protected internal static readonly DateTime FIXED_DATE_ONE = new DateTime(1363607000000L);
	  protected internal static readonly DateTime FIXED_DATE_TWO = new DateTime(1363607500000L);
	  protected internal static readonly DateTime FIXED_DATE_THREE = new DateTime(1363607600000L);
	  protected internal static readonly DateTime FIXED_DATE_FOUR = new DateTime(1363607700000L);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.ProcessEngineRule("camunda.cfg.xml");
	  public ProcessEngineRule engineRule = new ProcessEngineRule("camunda.cfg.xml");

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void activateDefinitions()
	  public virtual void activateDefinitions()
	  {
		engineRule.ManagementService.activateJobDefinitionByProcessDefinitionKey("oneTaskProcessTimerJob_710", true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void resetClock()
	  public virtual void resetClock()
	  {
		ClockUtil.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ScenarioUnderTest("initTimerChangeJobDefinition.1") @Test public void testTimerChangeJobDefinitionByIdWithJobsIncluded()
	  [ScenarioUnderTest("initTimerChangeJobDefinition.1")]
	  public virtual void testTimerChangeJobDefinitionByIdWithJobsIncluded()
	  {
		Job job = engineRule.ManagementService.createJobQuery().processDefinitionKey("oneTaskProcessTimerJob_710").withoutTenantId().singleResult();

		JobDefinition jobDefinition = engineRule.ManagementService.createJobDefinitionQuery().processDefinitionKey("oneTaskProcessTimerJob_710").withoutTenantId().singleResult();

		// assume
		assertThat(job.Suspended, @is(false));
		assertThat(jobDefinition.Suspended, @is(false));

		Job timerJob = engineRule.ManagementService.createJobQuery().timers().duedateLowerThan(new DateTime(1_363_607_001_000L)).duedateHigherThan(new DateTime(1_363_606_999_000L)).singleResult();

		ClockUtil.CurrentTime = FIXED_DATE_ONE;

		// when
		engineRule.ManagementService.executeJob(timerJob.Id);

		job = engineRule.ManagementService.createJobQuery().processDefinitionKey("oneTaskProcessTimerJob_710").withoutTenantId().singleResult();

		jobDefinition = engineRule.ManagementService.createJobDefinitionQuery().processDefinitionKey("oneTaskProcessTimerJob_710").withoutTenantId().singleResult();

		// then
		assertThat(job.Suspended, @is(true));
		assertThat(jobDefinition.Suspended, @is(true));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ScenarioUnderTest("initTimerChangeJobDefinition.2") @Test public void testTimerChangeProcessDefinitionById()
	  [ScenarioUnderTest("initTimerChangeJobDefinition.2")]
	  public virtual void testTimerChangeProcessDefinitionById()
	  {
		JobDefinition jobDefinition = engineRule.ManagementService.createJobDefinitionQuery().processDefinitionKey("oneTaskProcessTimerJob_710").withoutTenantId().singleResult();

		// assume
		assertThat(jobDefinition.Suspended, @is(false));

		Job timerJob = engineRule.ManagementService.createJobQuery().timers().duedateLowerThan(new DateTime(1_363_607_501_000L)).duedateHigherThan(new DateTime(1_363_607_499_000L)).singleResult();

		ClockUtil.CurrentTime = FIXED_DATE_TWO;

		// when
		engineRule.ManagementService.executeJob(timerJob.Id);

		jobDefinition = engineRule.ManagementService.createJobDefinitionQuery().processDefinitionKey("oneTaskProcessTimerJob_710").withoutTenantId().singleResult();

		// then
		assertThat(jobDefinition.Suspended, @is(true));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ScenarioUnderTest("initTimerChangeJobDefinition.3") @Test public void testTimerChangeJobDefinitionByKeyWithTenantId()
	  [ScenarioUnderTest("initTimerChangeJobDefinition.3")]
	  public virtual void testTimerChangeJobDefinitionByKeyWithTenantId()
	  {
		JobDefinition jobDefinition = engineRule.ManagementService.createJobDefinitionQuery().processDefinitionKey("oneTaskProcessTimerJob_710").tenantIdIn("aTenantId").singleResult();

		// assume
		assertThat(jobDefinition.Suspended, @is(false));

		Job timerJob = engineRule.ManagementService.createJobQuery().timers().duedateLowerThan(new DateTime(1_363_607_601_000L)).duedateHigherThan(new DateTime(1_363_607_599_000L)).singleResult();

		ClockUtil.CurrentTime = FIXED_DATE_THREE;

		// when
		engineRule.ManagementService.executeJob(timerJob.Id);

		jobDefinition = engineRule.ManagementService.createJobDefinitionQuery().processDefinitionKey("oneTaskProcessTimerJob_710").tenantIdIn("aTenantId").singleResult();

		// then
		assertThat(jobDefinition.Suspended, @is(true));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ScenarioUnderTest("initTimerChangeJobDefinition.4") @Test public void testTimerChangeJobDefinitionByKeyWithoutTenantId()
	  [ScenarioUnderTest("initTimerChangeJobDefinition.4")]
	  public virtual void testTimerChangeJobDefinitionByKeyWithoutTenantId()
	  {
		JobDefinition jobDefinition = engineRule.ManagementService.createJobDefinitionQuery().processDefinitionKey("oneTaskProcessTimerJob_710").withoutTenantId().singleResult();

		// assume
		assertThat(jobDefinition.Suspended, @is(false));

		Job timerJob = engineRule.ManagementService.createJobQuery().timers().duedateLowerThan(new DateTime(1_363_607_701_000L)).duedateHigherThan(new DateTime(1_363_607_699_000L)).singleResult();

		ClockUtil.CurrentTime = FIXED_DATE_FOUR;

		// when
		engineRule.ManagementService.executeJob(timerJob.Id);

		jobDefinition = engineRule.ManagementService.createJobDefinitionQuery().processDefinitionKey("oneTaskProcessTimerJob_710").withoutTenantId().singleResult();

		// then
		assertThat(jobDefinition.Suspended, @is(true));
	  }

	}

}