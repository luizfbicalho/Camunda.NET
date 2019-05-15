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
namespace org.camunda.bpm.qa.performance.engine.framework.report
{

	using HtmlDocumentBuilder = org.camunda.bpm.engine.impl.form.engine.HtmlDocumentBuilder;
	using HtmlElementWriter = org.camunda.bpm.engine.impl.form.engine.HtmlElementWriter;
	using TabularResultSet = org.camunda.bpm.qa.performance.engine.framework.aggregate.TabularResultSet;

	/// <summary>
	/// Uses a <seealso cref="TabularResultSet"/> and renders it as a Html Table.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class HtmlReportBuilder
	{

	  protected internal TabularResultSet resultSet;
	  protected internal string resultsBaseFolder;
	  protected internal string jsonSourceFileName;
	  protected internal string csvSourceFileName;
	  protected internal string reportName;
	  protected internal bool isCreateImageLinks;

	  public HtmlReportBuilder(TabularResultSet resultSet)
	  {
		this.resultSet = resultSet;
	  }

	  public virtual HtmlReportBuilder resultDetailsFolder(string resultsBaseFolder)
	  {
		this.resultsBaseFolder = resultsBaseFolder;
		return this;
	  }

	  public virtual HtmlReportBuilder jsonSource(string jsonSourceFileName)
	  {
		this.jsonSourceFileName = jsonSourceFileName;
		return this;
	  }

	  public virtual HtmlReportBuilder name(string reportName)
	  {
		this.reportName = reportName;
		return this;
	  }

	  public virtual HtmlReportBuilder createImageLinks(bool shouldCreateImageLinks)
	  {
		isCreateImageLinks = shouldCreateImageLinks;
		return this;
	  }

	  public virtual HtmlReportBuilder csvSource(string csvReportFilename)
	  {
		this.csvSourceFileName = csvReportFilename;
		return this;
	  }

	  public virtual string execute()
	  {

		HtmlDocumentBuilder documentBuilder = new HtmlDocumentBuilder(new HtmlElementWriter("html"));

		/// <summary>
		/// <head>...</head> </summary>
		documentBuilder.startElement(new HtmlElementWriter("head")).startElement((new HtmlElementWriter("title")).textContent(reportName)).endElement().startElement(new HtmlElementWriter("link")
			  .attribute("rel", "stylesheet").attribute("href", "http://netdna.bootstrapcdn.com/bootstrap/3.0.3/css/bootstrap.min.css")).endElement().startElement(new HtmlElementWriter("link")
			  .attribute("rel", "stylesheet").attribute("href", "http://netdna.bootstrapcdn.com/bootstrap/3.0.3/css/bootstrap-theme.min.css")).endElement().endElement();

		/// <summary>
		/// <body> </summary>
		HtmlDocumentBuilder bodyBuilder = documentBuilder.startElement(new HtmlElementWriter("body")).startElement((new HtmlElementWriter("div")).attribute("class", "container"));

		/// <summary>
		/// build Headline row </summary>
		bodyBuilder.startElement((new HtmlElementWriter("div")).attribute("class", "row")).startElement((new HtmlElementWriter("div")).attribute("class", "coll-md-12")).startElement((new HtmlElementWriter("h1")).textContent(reportName)).endElement().endElement().endElement();

		if (!string.ReferenceEquals(jsonSourceFileName, null) || !string.ReferenceEquals(csvSourceFileName, null))
		{

		 HtmlDocumentBuilder sourceRow = bodyBuilder.startElement((new HtmlElementWriter("div")).attribute("class", "row")).startElement((new HtmlElementWriter("div")).attribute("class", "coll-md-12")).startElement(new HtmlElementWriter("p"));

		 if (!string.ReferenceEquals(jsonSourceFileName, null))
		 {
			 sourceRow.startElement(new HtmlElementWriter("a")
					  .attribute("href", jsonSourceFileName).textContent("This Report as JSON")).endElement();
		 }

		 if (!string.ReferenceEquals(jsonSourceFileName, null))
		 {
			sourceRow.startElement((new HtmlElementWriter("span")).textContent("&nbsp;|&nbsp;")).endElement();
		 }

		 if (!string.ReferenceEquals(jsonSourceFileName, null))
		 {
			sourceRow.startElement(new HtmlElementWriter("a")
					 .attribute("href", csvSourceFileName).textContent("This Report as CSV")).endElement();
		 }

		 sourceRow.endElement().endElement().endElement();

		}

		bodyBuilder.startElement((new HtmlElementWriter("div")).attribute("class", "row")).startElement((new HtmlElementWriter("div")).attribute("class", "coll-md-12"));

			writeResultTable(bodyBuilder);

		bodyBuilder.endElement().endElement();


		/// <summary>
		/// </body> </summary>
		bodyBuilder.endElement().endElement();

		return documentBuilder.endElement().HtmlString;

	  }

	  protected internal virtual void writeResultTable(HtmlDocumentBuilder bodyBuilder)
	  {

		/// <summary>
		/// <table> </summary>
		HtmlDocumentBuilder tableBuilder = bodyBuilder.startElement((new HtmlElementWriter("table")).attribute("class", "table table-condensed"));

		/// <summary>
		/// <tr> </summary>
		HtmlDocumentBuilder tableHeadRowBuilder = tableBuilder.startElement(new HtmlElementWriter("tr"));

		foreach (string columnName in resultSet.ResultColumnNames)
		{
		  tableHeadRowBuilder.startElement((new HtmlElementWriter("th")).textContent(columnName)).endElement();
		}

		if (!string.ReferenceEquals(resultsBaseFolder, null))
		{
		  tableHeadRowBuilder.startElement(new HtmlElementWriter("th", true)).endElement();
		}

		/// <summary>
		/// </tr> </summary>
		tableHeadRowBuilder.endElement();

		foreach (IList<object> resultRow in resultSet.Results)
		{

		  /// <summary>
		  /// <tr> </summary>
		  HtmlDocumentBuilder tableRowBuilder = tableBuilder.startElement(new HtmlElementWriter("tr"));

		  for (int i = 0; i < resultRow.Count; i++)
		  {
			object value = resultRow[i];
			if (i == 0 && isCreateImageLinks)
			{
			  tableHeadRowBuilder.startElement(new HtmlElementWriter("td")).startElement(new HtmlElementWriter("a")
								 .attribute("href", "images/" + value + ".png").textContent(value.ToString())).endElement().endElement();

			}
			else
			{
			  tableHeadRowBuilder.startElement((new HtmlElementWriter("td")).textContent(value.ToString())).endElement();
			}
		  }

		  if (!string.ReferenceEquals(resultsBaseFolder, null))
		  {
			/// <summary>
			/// build link to Json file </summary>
			tableHeadRowBuilder.startElement(new HtmlElementWriter("td")).startElement(new HtmlElementWriter("a")
								.attribute("href", resultsBaseFolder + resultRow[0] + ".json").textContent("details")).endElement().endElement();
		  }

		  /// <summary>
		  /// </tr> </summary>
		  tableRowBuilder.endElement();

		}

		/// <summary>
		/// </table> </summary>
		tableBuilder.endElement();
	  }


	}

}