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
namespace org.camunda.bpm.qa.performance.engine.framework.activitylog
{

	using SectionedHtmlReportBuilder = org.camunda.bpm.qa.performance.engine.framework.report.SectionedHtmlReportBuilder;
	using FileUtil = org.camunda.bpm.qa.performance.engine.util.FileUtil;

	public class ActivityCountReporter
	{

	  public static void Main(string[] args)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String resultsFolder = "target"+ java.io.File.separatorChar+"results";
		string resultsFolder = "target" + Path.DirectorySeparatorChar + "results";
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String reportsFolder = "target"+java.io.File.separatorChar+"reports";
		string reportsFolder = "target" + Path.DirectorySeparatorChar + "reports";

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String htmlReportFilename = reportsFolder + java.io.File.separatorChar + "activity-count-report.html";
		string htmlReportFilename = reportsFolder + Path.DirectorySeparatorChar + "activity-count-report.html";

		// make sure reports folder exists
		File reportsFolderFile = new File(reportsFolder);
		if (!reportsFolderFile.exists())
		{
		  reportsFolderFile.mkdir();
		}

		SectionedHtmlReportBuilder htmlBuilder = new SectionedHtmlReportBuilder("Activity Count Report");

		ActivityCountAggregator activityCountAggregator = new ActivityCountAggregator(resultsFolder, htmlBuilder);
		activityCountAggregator.execute();

		string report = htmlBuilder.execute();
		FileUtil.writeStringToFile(report, htmlReportFilename);
	  }

	}

}