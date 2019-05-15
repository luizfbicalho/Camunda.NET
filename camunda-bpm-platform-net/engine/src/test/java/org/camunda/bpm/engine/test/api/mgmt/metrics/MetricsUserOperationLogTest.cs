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
namespace org.camunda.bpm.engine.test.api.mgmt.metrics
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// 
	/// <summary>
	/// @author Tobias Metzke
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class MetricsUserOperationLogTest
	{
		private bool InstanceFieldsInitialized = false;

		public MetricsUserOperationLogTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal ManagementService managementService;
	  protected internal HistoryService historyService;
	  protected internal IdentityService identityService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setup()
	  public virtual void setup()
	  {
		managementService = engineRule.ManagementService;
		historyService = engineRule.HistoryService;
		identityService = engineRule.IdentityService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteMetrics()
	  public virtual void testDeleteMetrics()
	  {
		// given
		identityService.AuthenticatedUserId = "userId";

		// when
		managementService.deleteMetrics(null);
		identityService.clearAuthentication();

		// then
		assertThat(historyService.createUserOperationLogQuery().count(), @is(1L));
		UserOperationLogEntry logEntry = historyService.createUserOperationLogQuery().singleResult();
		assertThat(logEntry.EntityType, @is(EntityTypes.METRICS));
		assertThat(logEntry.OperationType, @is(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE));
		assertThat(logEntry.Property, nullValue());
		assertThat(logEntry.OrgValue, nullValue());
		assertThat(logEntry.NewValue, nullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteMetricsWithTimestamp()
	  public virtual void testDeleteMetricsWithTimestamp()
	  {
		// given
		DateTime timestamp = ClockUtil.CurrentTime;
		identityService.AuthenticatedUserId = "userId";

		// when
		managementService.deleteMetrics(timestamp);
		identityService.clearAuthentication();

		// then
		assertThat(historyService.createUserOperationLogQuery().count(), @is(1L));
		UserOperationLogEntry logEntry = historyService.createUserOperationLogQuery().singleResult();
		assertThat(logEntry.EntityType, @is(EntityTypes.METRICS));
		assertThat(logEntry.OperationType, @is(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE));
		assertThat(logEntry.Property, @is("timestamp"));
		assertThat(logEntry.OrgValue, nullValue());
		assertThat(logEntry.NewValue, @is(timestamp.Ticks.ToString()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteMetricsWithReporterId()
	  public virtual void testDeleteMetricsWithReporterId()
	  {
		// given
		identityService.AuthenticatedUserId = "userId";

		// when
		managementService.deleteMetrics(null, "reporter1");
		identityService.clearAuthentication();

		// then
		assertThat(historyService.createUserOperationLogQuery().count(), @is(1L));
		UserOperationLogEntry logEntry = historyService.createUserOperationLogQuery().singleResult();
		assertThat(logEntry.EntityType, @is(EntityTypes.METRICS));
		assertThat(logEntry.OperationType, @is(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE));
		assertThat(logEntry.Property, @is("reporter"));
		assertThat(logEntry.OrgValue, nullValue());
		assertThat(logEntry.NewValue, @is("reporter1"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteMetricsWithTimestampAndReporterId()
	  public virtual void testDeleteMetricsWithTimestampAndReporterId()
	  {
		// given
		DateTime timestamp = ClockUtil.CurrentTime;
		identityService.AuthenticatedUserId = "userId";

		// when
		managementService.deleteMetrics(timestamp, "reporter1");
		identityService.clearAuthentication();

		// then
		assertThat(historyService.createUserOperationLogQuery().count(), @is(2L));
		UserOperationLogEntry logEntry = historyService.createUserOperationLogQuery().property("reporter").singleResult();
		assertThat(logEntry.EntityType, @is(EntityTypes.METRICS));
		assertThat(logEntry.OperationType, @is(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE));
		assertThat(logEntry.Property, @is("reporter"));
		assertThat(logEntry.OrgValue, nullValue());
		assertThat(logEntry.NewValue, @is("reporter1"));

		logEntry = historyService.createUserOperationLogQuery().property("timestamp").singleResult();
		assertThat(logEntry.EntityType, @is(EntityTypes.METRICS));
		assertThat(logEntry.OperationType, @is(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE));
		assertThat(logEntry.Property, @is("timestamp"));
		assertThat(logEntry.OrgValue, nullValue());
		assertThat(logEntry.NewValue, @is(timestamp.Ticks.ToString()));
	  }
	}

}