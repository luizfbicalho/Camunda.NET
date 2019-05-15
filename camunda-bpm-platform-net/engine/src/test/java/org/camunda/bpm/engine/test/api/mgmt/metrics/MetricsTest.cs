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
namespace org.camunda.bpm.engine.test.api.mgmt.metrics
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.fail;

	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Meter = org.camunda.bpm.engine.impl.metrics.Meter;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Metrics = org.camunda.bpm.engine.management.Metrics;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotEquals;
	using BeforeClass = org.junit.BeforeClass;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class MetricsTest
	{

	  protected internal static readonly ProcessEngineRule ENGINE_RULE = new ProvidedProcessEngineRule();
	  protected internal static readonly ProcessEngineTestRule TEST_RULE = new ProcessEngineTestRule(ENGINE_RULE);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.junit.rules.RuleChain RULE_CHAIN = org.junit.rules.RuleChain.outerRule(ENGINE_RULE).around(TEST_RULE);
	  public static RuleChain RULE_CHAIN = RuleChain.outerRule(ENGINE_RULE).around(TEST_RULE);

	  protected internal static RuntimeService runtimeService;
	  protected internal static ProcessEngineConfigurationImpl processEngineConfiguration;
	  protected internal static ManagementService managementService;

	  protected internal static void clearMetrics()
	  {
		ICollection<Meter> meters = processEngineConfiguration.MetricsRegistry.Meters.Values;
		foreach (Meter meter in meters)
		{
		  meter.AndClear;
		}
		managementService.deleteMetrics(null);
		processEngineConfiguration.DbMetricsReporterActivate = false;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @BeforeClass public static void initMetrics()
	  public static void initMetrics()
	  {
		runtimeService = ENGINE_RULE.RuntimeService;
		processEngineConfiguration = ENGINE_RULE.ProcessEngineConfiguration;
		managementService = ENGINE_RULE.ManagementService;

		//clean up before start
		clearMetrics();
		TEST_RULE.deploy(Bpmn.createExecutableProcess("testProcess").startEvent().manualTask().endEvent().done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanUp()
	  public virtual void cleanUp()
	  {
		clearMetrics();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStartAndEndMetricsAreEqual()
	  public virtual void testStartAndEndMetricsAreEqual()
	  {
		// given

		//when
		runtimeService.startProcessInstanceByKey("testProcess");
		processEngineConfiguration.DbMetricsReporter.reportNow();

		//then end and start metrics are equal
		long start = managementService.createMetricsQuery().name(Metrics.ACTIVTY_INSTANCE_START).sum();
		long end = managementService.createMetricsQuery().name(Metrics.ACTIVTY_INSTANCE_END).sum();
		assertEquals(end, start);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEndMetricWithWaitState()
	  public virtual void testEndMetricWithWaitState()
	  {
		//given
		TEST_RULE.deploy(Bpmn.createExecutableProcess("userProcess").startEvent().userTask("Task").endEvent().done());

		//when
		runtimeService.startProcessInstanceByKey("userProcess");
		processEngineConfiguration.DbMetricsReporter.reportNow();

		//then end is not equal to start since a wait state exist at Task
		long start = managementService.createMetricsQuery().name(Metrics.ACTIVTY_INSTANCE_START).sum();
		long end = managementService.createMetricsQuery().name(Metrics.ACTIVTY_INSTANCE_END).sum();
		assertNotEquals(end, start);
		assertEquals(2, start);
		assertEquals(1, end);

		//when completing the task
		string id = ENGINE_RULE.TaskService.createTaskQuery().processDefinitionKey("userProcess").singleResult().Id;
		ENGINE_RULE.TaskService.complete(id);

		//then start and end is equal
		start = managementService.createMetricsQuery().name(Metrics.ACTIVTY_INSTANCE_START).sum();
		end = managementService.createMetricsQuery().name(Metrics.ACTIVTY_INSTANCE_END).sum();
		assertEquals(end, start);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteMetrics()
	  public virtual void testDeleteMetrics()
	  {

		// given
		runtimeService.startProcessInstanceByKey("testProcess");
		processEngineConfiguration.DbMetricsReporter.reportNow();

		// a count of six (start and end)
		assertEquals(6l, managementService.createMetricsQuery().sum());

		// if
		// we delete with timestamp "null"
		managementService.deleteMetrics(null);

		// then
		// all entries are deleted
		assertEquals(0l, managementService.createMetricsQuery().sum());
	  }
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteMetricsWithTimestamp()
	  public virtual void testDeleteMetricsWithTimestamp()
	  {

		// given
		runtimeService.startProcessInstanceByKey("testProcess");
		processEngineConfiguration.DbMetricsReporter.reportNow();

		// a count of six (start and end)
		assertEquals(6l, managementService.createMetricsQuery().sum());

		// if
		// we delete with timestamp older or equal to the timestamp of the log entry
		managementService.deleteMetrics(ClockUtil.CurrentTime);

		// then
		// all entries are deleted
		assertEquals(0l, managementService.createMetricsQuery().name(Metrics.ACTIVTY_INSTANCE_START).sum());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteMetricsWithTimestampBefore()
	  public virtual void testDeleteMetricsWithTimestampBefore()
	  {

		// given
		runtimeService.startProcessInstanceByKey("testProcess");
		processEngineConfiguration.DbMetricsReporter.reportNow();

		// a count of six (start and end)
		assertEquals(6l, managementService.createMetricsQuery().sum());

		// if
		// we delete with timestamp before the timestamp of the log entry
		managementService.deleteMetrics(new DateTime(ClockUtil.CurrentTime.Ticks - 10000));

		// then
		// the entires are NOT deleted
		assertEquals(6l, managementService.createMetricsQuery().sum());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteMetricsWithReporterId()
	  public virtual void testDeleteMetricsWithReporterId()
	  {
		// indicate that db metrics reporter is active (although it is not)
		processEngineConfiguration.DbMetricsReporterActivate = true;

		// given
		processEngineConfiguration.DbMetricsReporter.ReporterId = "reporter1";
		runtimeService.startProcessInstanceByKey("testProcess");
		managementService.reportDbMetricsNow();

		processEngineConfiguration.DbMetricsReporter.ReporterId = "reporter2";
		runtimeService.startProcessInstanceByKey("testProcess");
		managementService.reportDbMetricsNow();

		assertEquals(3l, managementService.createMetricsQuery().name(Metrics.ACTIVTY_INSTANCE_START).reporter("reporter1").sum());

		// when the metrics for reporter1 are deleted
		managementService.deleteMetrics(null, "reporter1");

		// then
		assertEquals(0l, managementService.createMetricsQuery().name(Metrics.ACTIVTY_INSTANCE_START).reporter("reporter1").sum());
		assertEquals(3l, managementService.createMetricsQuery().name(Metrics.ACTIVTY_INSTANCE_START).reporter("reporter2").sum());

		// cleanup
		processEngineConfiguration.DbMetricsReporterActivate = false;
		processEngineConfiguration.DbMetricsReporter.ReporterId = null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportNow()
	  public virtual void testReportNow()
	  {
		// indicate that db metrics reporter is active (although it is not)
		processEngineConfiguration.DbMetricsReporterActivate = true;

		// given
		runtimeService.startProcessInstanceByKey("testProcess");

		// when
		managementService.reportDbMetricsNow();

		// then the metrics have been reported
		assertEquals(3l, managementService.createMetricsQuery().name(Metrics.ACTIVTY_INSTANCE_START).sum());

		// cleanup
		processEngineConfiguration.DbMetricsReporterActivate = false;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportNowIfMetricsIsDisabled()
	  public virtual void testReportNowIfMetricsIsDisabled()
	  {
		bool defaultIsMetricsEnabled = processEngineConfiguration.MetricsEnabled;

		// given
		processEngineConfiguration.MetricsEnabled = false;

	   try
	   {
		  // when
		  managementService.reportDbMetricsNow();
		  fail("Exception expected");
	   }
		catch (ProcessEngineException e)
		{
		  // then an exception is thrown
		  Assert.assertTrue(e.Message.contains("Metrics reporting is disabled"));
		}
		finally
		{
		  // reset metrics setting
		  processEngineConfiguration.MetricsEnabled = defaultIsMetricsEnabled;
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportNowIfReporterIsNotActive()
	  public virtual void testReportNowIfReporterIsNotActive()
	  {
		bool defaultIsMetricsEnabled = processEngineConfiguration.MetricsEnabled;
		bool defaultIsMetricsReporterActivate = processEngineConfiguration.DbMetricsReporterActivate;

		// given
		processEngineConfiguration.MetricsEnabled = true;
		processEngineConfiguration.DbMetricsReporterActivate = false;

		try
		{
		  // when
		  managementService.reportDbMetricsNow();
		  fail("Exception expected");
		}
		catch (ProcessEngineException e)
		{
		  // then an exception is thrown
		  Assert.assertTrue(e.Message.contains("Metrics reporting to database is disabled"));
		}
		finally
		{
		  processEngineConfiguration.MetricsEnabled = defaultIsMetricsEnabled;
		  processEngineConfiguration.DbMetricsReporterActivate = defaultIsMetricsReporterActivate;
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuery()
	  public virtual void testQuery()
	  {
		// given
		runtimeService.startProcessInstanceByKey("testProcess");
		processEngineConfiguration.DbMetricsReporter.reportNow();

		// then (query assertions)
		assertEquals(0l, managementService.createMetricsQuery().name("UNKNOWN").sum());
		assertEquals(3l, managementService.createMetricsQuery().name(Metrics.ACTIVTY_INSTANCE_START).sum());

		assertEquals(6l, managementService.createMetricsQuery().sum());
		assertEquals(6l, managementService.createMetricsQuery().startDate(new DateTime(1000)).sum());
		assertEquals(6l, managementService.createMetricsQuery().startDate(new DateTime(1000)).endDate(new DateTime(ClockUtil.CurrentTime.Ticks + 2000l)).sum()); // + 2000 for milliseconds imprecision on some databases (MySQL)
		assertEquals(0l, managementService.createMetricsQuery().startDate(new DateTime(ClockUtil.CurrentTime.Ticks + 1000l)).sum());
		assertEquals(0l, managementService.createMetricsQuery().startDate(new DateTime(ClockUtil.CurrentTime.Ticks + 1000l)).endDate(ClockUtil.CurrentTime).sum());

		// given
		runtimeService.startProcessInstanceByKey("testProcess");
		processEngineConfiguration.DbMetricsReporter.reportNow();

		// then (query assertions)
		assertEquals(12l, managementService.createMetricsQuery().sum());
		assertEquals(12l, managementService.createMetricsQuery().startDate(new DateTime(1000)).sum());
		assertEquals(12l, managementService.createMetricsQuery().startDate(new DateTime(1000)).endDate(new DateTime(ClockUtil.CurrentTime.Ticks + 2000l)).sum()); // + 2000 for milliseconds imprecision on some databases (MySQL)
		assertEquals(0l, managementService.createMetricsQuery().startDate(new DateTime(ClockUtil.CurrentTime.Ticks + 1000l)).sum());
		assertEquals(0l, managementService.createMetricsQuery().startDate(new DateTime(ClockUtil.CurrentTime.Ticks + 1000l)).endDate(ClockUtil.CurrentTime).sum());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryEndDateExclusive()
	  public virtual void testQueryEndDateExclusive()
	  {
		// given
		// note: dates should be exact seconds due to missing milliseconds precision on
		// older mysql versions
		// cannot insert 1970-01-01 00:00:00 into MySQL
		ClockUtil.CurrentTime = new DateTime(5000L);
		runtimeService.startProcessInstanceByKey("testProcess");
		processEngineConfiguration.DbMetricsReporter.reportNow();

		ClockUtil.CurrentTime = new DateTime(6000L);
		runtimeService.startProcessInstanceByKey("testProcess");
		processEngineConfiguration.DbMetricsReporter.reportNow();

		ClockUtil.CurrentTime = new DateTime(7000L);
		runtimeService.startProcessInstanceByKey("testProcess");
		processEngineConfiguration.DbMetricsReporter.reportNow();

		// then Query#startDate is inclusive and Query#endDate is exclusive
		assertEquals(18l, managementService.createMetricsQuery().sum());
		assertEquals(18l, managementService.createMetricsQuery().startDate(new DateTime()).sum());
		assertEquals(12l, managementService.createMetricsQuery().startDate(new DateTime()).endDate(new DateTime(7000L)).sum());
		assertEquals(18l, managementService.createMetricsQuery().startDate(new DateTime()).endDate(new DateTime(8000L)).sum());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportWithReporterId()
	  public virtual void testReportWithReporterId()
	  {
		// indicate that db metrics reporter is active (although it is not)
		processEngineConfiguration.DbMetricsReporterActivate = true;

		// given

		// when
		processEngineConfiguration.DbMetricsReporter.ReporterId = "reporter1";
		runtimeService.startProcessInstanceByKey("testProcess");
		managementService.reportDbMetricsNow();

		// and
		processEngineConfiguration.DbMetricsReporter.ReporterId = "reporter2";
		runtimeService.startProcessInstanceByKey("testProcess");
		managementService.reportDbMetricsNow();

		// then the metrics have been reported
		assertEquals(6l, managementService.createMetricsQuery().name(Metrics.ACTIVTY_INSTANCE_START).sum());

		// and are grouped by reporter
		assertEquals(3l, managementService.createMetricsQuery().name(Metrics.ACTIVTY_INSTANCE_START).reporter("reporter1").sum());
		assertEquals(3l, managementService.createMetricsQuery().name(Metrics.ACTIVTY_INSTANCE_START).reporter("reporter2").sum());
		assertEquals(0l, managementService.createMetricsQuery().name(Metrics.ACTIVTY_INSTANCE_START).reporter("aNonExistingReporter").sum());

		// cleanup
		processEngineConfiguration.DbMetricsReporterActivate = false;
		processEngineConfiguration.DbMetricsReporter.ReporterId = null;
	  }

	}

}