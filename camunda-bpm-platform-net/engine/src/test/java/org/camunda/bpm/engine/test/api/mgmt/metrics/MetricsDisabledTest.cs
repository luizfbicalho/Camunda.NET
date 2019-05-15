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
	using ResourceProcessEngineTestCase = org.camunda.bpm.engine.impl.test.ResourceProcessEngineTestCase;

	/// <summary>
	/// Asserts engine functionality is metrics are disabled
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class MetricsDisabledTest : ResourceProcessEngineTestCase
	{

	  public MetricsDisabledTest() : base("org/camunda/bpm/engine/test/api/mgmt/metrics/metricsDisabledTest.cfg.xml")
	  {
	  }

	  // FAILING, see https://app.camunda.com/jira/browse/CAM-4053
	  // (to run, remove "FAILING" from methodname)
	  public virtual void FAILING_testQueryMetricsIfMetricsIsDisabled()
	  {

		// given
		// that the metrics are disabled (see xml configuration referenced in constructor)
		assertFalse(processEngineConfiguration.MetricsEnabled);
		assertFalse(processEngineConfiguration.DbMetricsReporterActivate);

		// then
		// it is possible to execute a query
		managementService.createMetricsQuery().sum();

	  }

	  public virtual void testReportNowIfMetricsDisabled()
	  {

		// given
		// that the metrics reporter is disabled
		assertFalse(processEngineConfiguration.DbMetricsReporterActivate);

		try
		{
		  // then
		  // I cannot invoke
		  managementService.reportDbMetricsNow();
		  fail("Exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Metrics reporting is disabled", e.Message);
		}
	  }
	}

}