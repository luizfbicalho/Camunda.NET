using System.IO;

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

	using SqlStatementCountAggregator = org.camunda.bpm.qa.performance.engine.framework.aggregate.SqlStatementCountAggregator;
	using TabularResultSet = org.camunda.bpm.qa.performance.engine.framework.aggregate.TabularResultSet;
	using HtmlReportBuilder = org.camunda.bpm.qa.performance.engine.framework.report.HtmlReportBuilder;
	using CsvUtil = org.camunda.bpm.qa.performance.engine.util.CsvUtil;
	using FileUtil = org.camunda.bpm.qa.performance.engine.util.FileUtil;
	using JsonUtil = org.camunda.bpm.qa.performance.engine.util.JsonUtil;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class SqlStatementLogReport
	{

	  public static void Main(string[] args)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String resultsFolder = "target"+java.io.File.separatorChar+"results";
		string resultsFolder = "target" + Path.DirectorySeparatorChar + "results";
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String reportsFolder = "target"+java.io.File.separatorChar+"reports";
		string reportsFolder = "target" + Path.DirectorySeparatorChar + "reports";

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String htmlReportFilename = reportsFolder + java.io.File.separatorChar + "sql-statement-log-report.html";
		string htmlReportFilename = reportsFolder + Path.DirectorySeparatorChar + "sql-statement-log-report.html";

		const string jsonReportFilename = "sql-statement-log-report.json";
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String jsonReportPath = reportsFolder + java.io.File.separatorChar + jsonReportFilename;
		string jsonReportPath = reportsFolder + Path.DirectorySeparatorChar + jsonReportFilename;

		const string csvReportFilename = "sql-statement-log-report.csv";
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String csvReportPath = reportsFolder + java.io.File.separatorChar + csvReportFilename;
		string csvReportPath = reportsFolder + Path.DirectorySeparatorChar + csvReportFilename;

		// make sure reports folder exists
		File reportsFolderFile = new File(reportsFolder);
		if (!reportsFolderFile.exists())
		{
		  reportsFolderFile.mkdir();
		}

		SqlStatementCountAggregator aggregator = new SqlStatementCountAggregator(resultsFolder);
		TabularResultSet aggregatedResults = aggregator.execute();

		// write Json report
		JsonUtil.writeObjectToFile(jsonReportPath, aggregatedResults);
		// write CSV Report
		CsvUtil.saveResultSetToFile(csvReportPath, aggregatedResults);

		// format HTML report
		HtmlReportBuilder reportWriter = (new HtmlReportBuilder(aggregatedResults)).name("Sql Statement Log Report").resultDetailsFolder(".." + Path.DirectorySeparatorChar + "results" + Path.DirectorySeparatorChar).createImageLinks(true).jsonSource(jsonReportFilename).csvSource(csvReportFilename);

		string report = reportWriter.execute();
		FileUtil.writeStringToFile(report, htmlReportFilename);



	  }
	}

}