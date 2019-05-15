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
namespace org.camunda.bpm.qa.performance.engine.benchmark
{
	using TabularResultAggregator = org.camunda.bpm.qa.performance.engine.framework.aggregate.TabularResultAggregator;
	using TabularResultSet = org.camunda.bpm.qa.performance.engine.framework.aggregate.TabularResultSet;
	using HtmlReportBuilder = org.camunda.bpm.qa.performance.engine.framework.report.HtmlReportBuilder;
	using CsvUtil = org.camunda.bpm.qa.performance.engine.util.CsvUtil;
	using FileUtil = org.camunda.bpm.qa.performance.engine.util.FileUtil;
	using JsonUtil = org.camunda.bpm.qa.performance.engine.util.JsonUtil;

	/// <summary>
	/// @author Daniel Meyer, Ingo Richtsmeier
	/// 
	/// </summary>
	public class BenchmarkReport
	{

	  public static void Main(string[] args)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String resultsFolder = "target"+java.io.File.separatorChar+"results";
		string resultsFolder = "target" + Path.DirectorySeparatorChar + "results";
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String reportsFolder = "target"+java.io.File.separatorChar+"reports";
		string reportsFolder = "target" + Path.DirectorySeparatorChar + "reports";

		string longTermBenchmarkResultFilename = System.getProperty("longTermBenchmarkResultFile");
		if (!string.ReferenceEquals(longTermBenchmarkResultFilename, null))
		{
		  File longTermBenchmarkResultFile = new File(longTermBenchmarkResultFilename);
		  longTermBenchmarkResultFile.ParentFile.mkdirs();
		  if (longTermBenchmarkResultFile.exists())
		  {
			// Do nothing, append current results later 
		  }
		  else
		  {
			FileUtil.appendStringToFile("name;" + "number of runs;" + "database;" + "history level;" + "starttime;" + "platform;" + "number of threads;" + "duration;" + "throughput", longTermBenchmarkResultFilename);
		  }
		  TabularResultSet longTermResultTable = (new BenchmarkLongtermAggregator(resultsFolder)).execute();
		  FileUtil.appendStringToFile(CsvUtil.resultSetAsCsv(longTermResultTable), longTermBenchmarkResultFilename);
		}

		writeReport(resultsFolder, reportsFolder, "benchmark", new BenchmarkAggregator(resultsFolder), "Benchmark Duration Report");
	  }

	  private static void writeReport(string resultsFolder, string reportsFolder, string benchmarkName, TabularResultAggregator aggregator, string reportDescription)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String htmlReportFilename = reportsFolder + java.io.File.separatorChar + benchmarkName+"-report.html";
		string htmlReportFilename = reportsFolder + Path.DirectorySeparatorChar + benchmarkName + "-report.html";

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String jsonReportFilename = benchmarkName+"-report.json";
		string jsonReportFilename = benchmarkName + "-report.json";
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String jsonReportPath = reportsFolder + java.io.File.separatorChar + jsonReportFilename;
		string jsonReportPath = reportsFolder + Path.DirectorySeparatorChar + jsonReportFilename;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String csvReportFilename = benchmarkName+"-report.csv";
		string csvReportFilename = benchmarkName + "-report.csv";
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String csvReportPath = reportsFolder + java.io.File.separatorChar + csvReportFilename;
		string csvReportPath = reportsFolder + Path.DirectorySeparatorChar + csvReportFilename;

		// make sure reports folder exists
		File reportsFolderFile = new File(reportsFolder);
		if (!reportsFolderFile.exists())
		{
		  reportsFolderFile.mkdir();
		}

		TabularResultSet aggregatedResults = aggregator.execute();

		// write Json report
		JsonUtil.writeObjectToFile(jsonReportPath, aggregatedResults);

		// format HTML report
		HtmlReportBuilder reportWriter = (new HtmlReportBuilder(aggregatedResults)).name(reportDescription).resultDetailsFolder(".." + Path.DirectorySeparatorChar + "results" + Path.DirectorySeparatorChar).createImageLinks(true).jsonSource(jsonReportFilename).csvSource(csvReportFilename);

		string report = reportWriter.execute();
		FileUtil.writeStringToFile(report, htmlReportFilename);

		// write CSV report
		CsvUtil.saveResultSetToFile(csvReportPath, aggregatedResults);
	  }
	}

}