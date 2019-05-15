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
namespace org.camunda.bpm.qa.performance.engine.sqlstatementlog
{

	using PerfTestResults = org.camunda.bpm.qa.performance.engine.framework.PerfTestResults;
	using PerfTestStepResult = org.camunda.bpm.qa.performance.engine.framework.PerfTestStepResult;
	using TabularResultAggregator = org.camunda.bpm.qa.performance.engine.framework.aggregate.TabularResultAggregator;
	using TabularResultSet = org.camunda.bpm.qa.performance.engine.framework.aggregate.TabularResultSet;
	using SqlStatementType = org.camunda.bpm.qa.performance.engine.sqlstatementlog.StatementLogSqlSession.SqlStatementType;

	/// <summary>
	/// Aggregates the results from a Sql Statement Test run.
	/// 
	/// This aggregator will count the statement types for each <seealso cref="SqlStatementType"/>
	/// and add the counts to the resultset.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class SqlStatementCountAggregator : TabularResultAggregator
	{

	  public const string TEST_NAME = "Test Name";
	  public const string INSERTS = "Inserts";
	  public const string DELETES = "Deletes";
	  public const string UPDATES = "Updates";
	  public const string SELECTS = "Selects";

	  public SqlStatementCountAggregator(string resultsFolderPath) : base(resultsFolderPath)
	  {
	  }

	  protected internal override TabularResultSet createAggregatedResultsInstance()
	  {
		TabularResultSet tabularResultSet = new TabularResultSet();

		IList<string> resultColumnNames = tabularResultSet.ResultColumnNames;
		resultColumnNames.Add(TEST_NAME);
		resultColumnNames.Add(INSERTS);
		resultColumnNames.Add(DELETES);
		resultColumnNames.Add(UPDATES);
		resultColumnNames.Add(SELECTS);

		return tabularResultSet;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected void processResults(org.camunda.bpm.qa.performance.engine.framework.PerfTestResults results, org.camunda.bpm.qa.performance.engine.framework.aggregate.TabularResultSet tabularResultSet)
	  protected internal override void processResults(PerfTestResults results, TabularResultSet tabularResultSet)
	  {
		List<object> row = new List<object>();

		row.Add(results.TestName);

		int insertCount = 0;
		int deleteCount = 0;
		int updateCount = 0;
		int selectCount = 0;

		if (results.PassResults.Count == 0)
		{
		  return;
		}

		IList<PerfTestStepResult> stepResults = results.PassResults[0].StepResults;
		foreach (PerfTestStepResult stepResult in stepResults)
		{
		  IList<LinkedHashMap<string, string>> statementLogs = (IList<LinkedHashMap<string, string>>) stepResult.ResultData;
		  foreach (LinkedHashMap<string, string> statementLog in statementLogs)
		  {
			string type = statementLog.get("statementType");
			SqlStatementType statementType = Enum.Parse(typeof(SqlStatementType), type);

			switch (statementType)
			{
			case SqlStatementType.DELETE:
			  deleteCount++;
			  break;
			case SqlStatementType.INSERT:
			  insertCount++;
			  break;
			case SqlStatementType.UPDATE:
			  updateCount++;
			  break;
			default:
			  selectCount++;
			  break;
			}
		  }
		}

		row.Add(insertCount);
		row.Add(deleteCount);
		row.Add(updateCount);
		row.Add(selectCount);

		tabularResultSet.addResultRow(row);
	  }

	}

}