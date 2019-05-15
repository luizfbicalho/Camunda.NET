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
namespace org.camunda.bpm.qa.upgrade.scenarios7110.timestamp
{
	using HistoricJobLog = org.camunda.bpm.engine.history.HistoricJobLog;
	using Test = org.junit.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	/// <summary>
	/// @author Nikola Koevski
	/// </summary>
	[ScenarioUnderTest("JobTimestampsScenario"), Origin("7.11.0")]
	public class HistoricJobLogTimestampsTest : AbstractTimestampMigrationTest
	{

	  protected internal const string PROCESS_DEFINITION_KEY = "jobTimestampsMigrationTestProcess";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ScenarioUnderTest("initJobTimestamps.1") @Test public void testDueDateConversion()
	  [ScenarioUnderTest("initJobTimestamps.1")]
	  public virtual void testDueDateConversion()
	  {

		// when
		HistoricJobLog historicJobLog = historyService.createHistoricJobLogQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).singleResult();

		// assume
		assertThat(historicJobLog, @is(notNullValue()));

		// then
		assertThat(historicJobLog.JobDueDate, @is(TIMESTAMP));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ScenarioUnderTest("initJobTimestamps.1") @Test public void testTimestampConversion()
	  [ScenarioUnderTest("initJobTimestamps.1")]
	  public virtual void testTimestampConversion()
	  {

		// when
		HistoricJobLog historicJobLog = historyService.createHistoricJobLogQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).singleResult();

		// assume
		assertNotNull(historicJobLog);

		// then
		assertThat(historicJobLog.Timestamp, @is(TIMESTAMP));
	  }
	}
}