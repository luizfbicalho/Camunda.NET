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
namespace org.camunda.bpm.engine.impl.form.engine
{

	using HtmlWriteContext = org.camunda.bpm.engine.impl.form.engine.HtmlDocumentBuilder.HtmlWriteContext;

	/// <summary>
	/// <para>Simple writer for html elements. Used by the <seealso cref="HtmlDocumentBuilder"/>.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class HtmlElementWriter
	{

	  protected internal string tagName;

	  /// <summary>
	  /// selfClosing means that the element should not be rendered as a
	  /// start + end tag pair but as a single tag using "/" to close the tag
	  /// inline 
	  /// </summary>
	  protected internal bool isSelfClosing;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string textContent_Renamed;
	  protected internal IDictionary<string, string> attributes = new LinkedHashMap<string, string>();

	  public HtmlElementWriter(string tagName)
	  {
		this.tagName = tagName;
		this.isSelfClosing = false;
	  }

	  public HtmlElementWriter(string tagName, bool isSelfClosing)
	  {
		this.tagName = tagName;
		this.isSelfClosing = isSelfClosing;
	  }

	  public virtual void writeStartTag(HtmlWriteContext context)
	  {
		writeLeadingWhitespace(context);
		writeStartTagOpen(context);
		writeAttributes(context);
		writeStartTagClose(context);
		writeEndLine(context);
	  }

	  public virtual void writeContent(HtmlWriteContext context)
	  {
		if (!string.ReferenceEquals(textContent_Renamed, null))
		{
		  writeLeadingWhitespace(context);
		  writeTextContent(context);
		  writeEndLine(context);
		}
	  }

	  public virtual void writeEndTag(HtmlWriteContext context)
	  {
		if (!isSelfClosing)
		{
		  writeLeadingWhitespace(context);
		  writeEndTagElement(context);
		  writeEndLine(context);
		}
	  }

	  protected internal virtual void writeEndTagElement(HtmlWriteContext context)
	  {
		StringWriter writer = context.Writer;
		writer.write("</");
		writer.write(tagName);
		writer.write(">");
	  }

	  protected internal virtual void writeTextContent(HtmlWriteContext context)
	  {
		StringWriter writer = context.Writer;
		writer.write("  "); // add additional whitespace
		writer.write(textContent_Renamed);
	  }

	  protected internal virtual void writeStartTagOpen(HtmlWriteContext context)
	  {
		StringWriter writer = context.Writer;
		writer.write("<");
		writer.write(tagName);
	  }

	  protected internal virtual void writeAttributes(HtmlWriteContext context)
	  {
		StringWriter writer = context.Writer;
		foreach (KeyValuePair<string, string> attribute in attributes.SetOfKeyValuePairs())
		{
		  writer.write(" ");
		  writer.write(attribute.Key);
		  if (attribute.Value != null)
		  {
			writer.write("=\"");
			string attributeValue = escapeQuotes(attribute.Value);
			writer.write(attributeValue);
			writer.write("\"");
		  }
		}
	  }

	  protected internal virtual string escapeQuotes(string attributeValue)
	  {
		string escapedHtmlQuote = "&quot;";
		string escapedJavaQuote = "\"";
		return attributeValue.replaceAll(escapedJavaQuote, escapedHtmlQuote);
	  }

	  protected internal virtual void writeEndLine(HtmlWriteContext context)
	  {
		StringWriter writer = context.Writer;
		writer.write("\n");
	  }

	  protected internal virtual void writeStartTagClose(HtmlWriteContext context)
	  {
		StringWriter writer = context.Writer;
		if (isSelfClosing)
		{
		  writer.write(" /");
		}
		writer.write(">");
	  }

	  protected internal virtual void writeLeadingWhitespace(HtmlWriteContext context)
	  {
		int stackSize = context.ElementStackSize;
		StringWriter writer = context.Writer;
		for (int i = 0; i < stackSize; i++)
		{
		  writer.write("  ");
		}
	  }

	  // builder /////////////////////////////////////

	  public virtual HtmlElementWriter attribute(string name, string value)
	  {
		attributes[name] = value;
		return this;
	  }

	  public virtual HtmlElementWriter textContent(string text)
	  {
		if (isSelfClosing)
		{
		  throw new System.InvalidOperationException("Self-closing element cannot have text content.");
		}
		this.textContent_Renamed = text;
		return this;
	  }

	}

}