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

	public class SectionedHtmlReportBuilder
	{

	  protected internal string reportName;

	  protected internal IDictionary<string, object> sections = new SortedDictionary<string, object>();

	  public SectionedHtmlReportBuilder(string reportName)
	  {
		this.reportName = reportName;
	  }

	  public virtual SectionedHtmlReportBuilder addSection(string title, object section)
	  {
		this.sections[title] = section;
		return this;
	  }

	  public virtual string execute()
	  {
		HtmlDocumentBuilder builder = new HtmlDocumentBuilder(new HtmlElementWriter("html"));

		addHtmlHead(builder);
		addHtmlBody(builder);

		return builder.endElement().HtmlString;
	  }

	  protected internal virtual void addHtmlHead(HtmlDocumentBuilder builder)
	  {
		builder.startElement(new HtmlElementWriter("head")).startElement((new HtmlElementWriter("title")).textContent(reportName)).endElement().startElement(new HtmlElementWriter("link")
			  .attribute("rel", "stylesheet").attribute("href", "http://netdna.bootstrapcdn.com/bootstrap/3.0.3/css/bootstrap.min.css")).endElement().startElement(new HtmlElementWriter("link")
			  .attribute("rel", "stylesheet").attribute("href", "http://netdna.bootstrapcdn.com/bootstrap/3.0.3/css/bootstrap-theme.min.css")).endElement().endElement();
	  }

	  protected internal virtual void addHtmlBody(HtmlDocumentBuilder builder)
	  {
		builder.startElement(new HtmlElementWriter("body"));
		builder.startElement((new HtmlElementWriter("div")).attribute("class", "container"));
		builder.startElement((new HtmlElementWriter("div")).attribute("class", "row"));
		builder.startElement((new HtmlElementWriter("div")).attribute("class", "coll-md-12"));
		builder.startElement((new HtmlElementWriter("h1")).textContent(reportName)).endElement();
		addHtmlSections(builder, sections, 3);
		builder.endElement();
		builder.endElement();
		builder.endElement();
		builder.endElement();
	  }

	  protected internal virtual void addHtmlSections(HtmlDocumentBuilder builder, IDictionary<string, object> sections, int level)
	  {
		foreach (string section in sections.Keys)
		{
		  addHtmlSection(builder, section, sections[section], level);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected void addHtmlSection(org.camunda.bpm.engine.impl.form.engine.HtmlDocumentBuilder builder, String title, Object section, int level)
	  protected internal virtual void addHtmlSection(HtmlDocumentBuilder builder, string title, object section, int level)
	  {
		// add heading
		builder.startElement((new HtmlElementWriter("h" + level)).textContent(title)).endElement();
		if (section is System.Collections.IDictionary)
		{
		  IDictionary<string, object> sections = (IDictionary<string, object>) section;
		  addHtmlSections(builder, sections, level + 1);
		}
		else
		{
		  TabularResultSet resultSet = (TabularResultSet) section;
		  addHtmlTable(builder, resultSet);
		}
	  }

	  protected internal virtual void addHtmlTable(HtmlDocumentBuilder builder, TabularResultSet resultSet)
	  {
		/// <summary>
		/// <table> </summary>
		builder.startElement((new HtmlElementWriter("table")).attribute("class", "table table-condensed table-hover table-bordered"));

		/// <summary>
		/// <tr> </summary>

		HtmlDocumentBuilder tableHeadRowBuilder = builder.startElement(new HtmlElementWriter("tr"));

		foreach (string columnName in resultSet.ResultColumnNames)
		{
		  tableHeadRowBuilder.startElement((new HtmlElementWriter("th")).textContent(columnName)).endElement();
		}

		/// <summary>
		/// </tr> </summary>
		tableHeadRowBuilder.endElement();

		foreach (IList<object> resultRow in resultSet.Results)
		{

		  /// <summary>
		  /// <tr> </summary>
		  HtmlDocumentBuilder tableRowBuilder = builder.startElement(new HtmlElementWriter("tr"));

		  foreach (object value in resultRow)
		  {
			if (value is TableCell)
			{
			  tableRowBuilder.startElement(((TableCell) value).toHtmlElementWriter()).endElement();
			}
			else
			{
			  tableRowBuilder.startElement((new HtmlElementWriter("td")).textContent(value.ToString())).endElement();
			}
		  }

		  /// <summary>
		  /// </tr> </summary>
		  tableRowBuilder.endElement();
		}

		/// <summary>
		/// </table> </summary>
		builder.endElement();
	  }


	  public class TableCell
	  {

		internal string text;
		internal int colspan = 1;
		internal bool header = false;

		public TableCell(string text) : this(text, false)
		{
		}

		public TableCell(string text, bool header) : this(text, 1, header)
		{
		}

		public TableCell(string text, int colspan) : this(text, colspan, false)
		{
		}

		public TableCell(string text, int colspan, bool header)
		{
		  this.text = text;
		  this.colspan = colspan;
		  this.header = header;
		}

		public override string ToString()
		{
		  return text;
		}

		public virtual HtmlElementWriter toHtmlElementWriter()
		{
		  HtmlElementWriter elementWriter;
		  if (header)
		  {
			elementWriter = new HtmlElementWriter("th");
		  }
		  else
		  {
			elementWriter = new HtmlElementWriter("td");
		  }

		  if (colspan > 1)
		  {
			elementWriter.attribute("colspan", colspan.ToString()).attribute("class", "text-center");
		  }

		  elementWriter.textContent(text);

		  return elementWriter;
		}

	  }

	}

}