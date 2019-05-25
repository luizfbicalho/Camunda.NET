using System;
using System.Collections.Generic;
using System.Text;
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
namespace org.camunda.bpm.engine.impl.util.xml
{

	using InputStreamSource = org.camunda.bpm.engine.impl.util.io.InputStreamSource;
	using ResourceStreamSource = org.camunda.bpm.engine.impl.util.io.ResourceStreamSource;
	using StreamSource = org.camunda.bpm.engine.impl.util.io.StreamSource;
	using StringStreamSource = org.camunda.bpm.engine.impl.util.io.StringStreamSource;
	using UrlStreamSource = org.camunda.bpm.engine.impl.util.io.UrlStreamSource;
	using SAXParseException = org.xml.sax.SAXParseException;
	using DefaultHandler = org.xml.sax.helpers.DefaultHandler;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class Parse : DefaultHandler
	{

	  private static readonly EngineUtilLogger LOG = ProcessEngineLogger.UTIL_LOGGER;

	  private const string JAXP_SCHEMA_SOURCE = "http://java.sun.com/xml/jaxp/properties/schemaSource";
	  private const string JAXP_SCHEMA_LANGUAGE = "http://java.sun.com/xml/jaxp/properties/schemaLanguage";
	  private const string W3C_XML_SCHEMA = "http://www.w3.org/2001/XMLSchema";
	  private const string XXE_PROCESSING = "http://xml.org/sax/features/external-general-entities";

	  private static readonly string NEW_LINE = System.getProperty("line.separator");

	  protected internal Parser parser;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string name_Conflict;
	  protected internal StreamSource streamSource;
	  protected internal Element rootElement = null;
	  protected internal IList<Problem> errors = new List<Problem>();
	  protected internal IList<Problem> warnings = new List<Problem>();
	  protected internal string schemaResource;
	  protected internal bool enableXxeProcessing = true;

	  public Parse(Parser parser)
	  {
		this.parser = parser;
	  }

	  public virtual Parse name(string name)
	  {
		this.name_Conflict = name;
		return this;
	  }

	  public virtual Parse sourceInputStream(Stream inputStream)
	  {
		if (string.ReferenceEquals(name_Conflict, null))
		{
		  name("inputStream");
		}
		StreamSource = new InputStreamSource(inputStream);
		return this;
	  }

	  public virtual Parse sourceResource(string resource)
	  {
		return sourceResource(resource, null);
	  }

	  public virtual Parse sourceUrl(URL url)
	  {
		if (string.ReferenceEquals(name_Conflict, null))
		{
		  name(url.ToString());
		}
		StreamSource = new UrlStreamSource(url);
		return this;
	  }

	  public virtual Parse sourceUrl(string url)
	  {
		try
		{
		  return sourceUrl(new URL(url));
		}
		catch (MalformedURLException e)
		{
		  throw LOG.malformedUrlException(url, e);
		}
	  }

	  public virtual Parse sourceResource(string resource, ClassLoader classLoader)
	  {
		if (string.ReferenceEquals(name_Conflict, null))
		{
		  name(resource);
		}
		StreamSource = new ResourceStreamSource(resource, classLoader);
		return this;
	  }

	  public virtual Parse sourceString(string @string)
	  {
		if (string.ReferenceEquals(name_Conflict, null))
		{
		  name("string");
		}
		StreamSource = new StringStreamSource(@string);
		return this;
	  }

	  public virtual Parse xxeProcessing(bool enable)
	  {
		EnableXxeProcessing = enable;
		return this;
	  }

	  protected internal virtual StreamSource StreamSource
	  {
		  set
		  {
			if (this.streamSource != null)
			{
			  throw LOG.multipleSourcesException(this.streamSource, value);
			}
			this.streamSource = value;
		  }
	  }

	  public virtual bool EnableXxeProcessing
	  {
		  set
		  {
			this.enableXxeProcessing = value;
		  }
	  }

	  public virtual Parse execute()
	  {
		try
		{
		  Stream inputStream = streamSource.InputStream;

		  parser.SaxParserFactory.setFeature(XXE_PROCESSING, enableXxeProcessing);

		  if (string.ReferenceEquals(schemaResource, null))
		  { // must be done before parser is created
			parser.SaxParserFactory.NamespaceAware = false;
			parser.SaxParserFactory.Validating = false;
		  }

		  SAXParser saxParser = parser.SaxParser;
		  if (!string.ReferenceEquals(schemaResource, null))
		  {
			saxParser.setProperty(JAXP_SCHEMA_LANGUAGE, W3C_XML_SCHEMA);
			saxParser.setProperty(JAXP_SCHEMA_SOURCE, schemaResource);
		  }
		  saxParser.parse(inputStream, new ParseHandler(this));

		}
		catch (Exception e)
		{
		  throw LOG.parsingFailureException(name_Conflict, e);
		}

		return this;
	  }

	  public virtual Element RootElement
	  {
		  get
		  {
			return rootElement;
		  }
	  }

	  public virtual IList<Problem> Problems
	  {
		  get
		  {
			return errors;
		  }
	  }

	  public virtual void addError(SAXParseException e)
	  {
		errors.Add(new Problem(e, name_Conflict));
	  }

	  public virtual void addError(string errorMessage, Element element)
	  {
		errors.Add(new Problem(errorMessage, name_Conflict, element));
	  }

	  public virtual void addError(BpmnParseException e)
	  {
		errors.Add(new Problem(e, name_Conflict));
	  }

	  public virtual bool hasErrors()
	  {
		return errors != null && errors.Count > 0;
	  }

	  public virtual void addWarning(SAXParseException e)
	  {
		warnings.Add(new Problem(e, name_Conflict));
	  }

	  public virtual void addWarning(string errorMessage, Element element)
	  {
		warnings.Add(new Problem(errorMessage, name_Conflict, element));
	  }

	  public virtual bool hasWarnings()
	  {
		return warnings != null && warnings.Count > 0;
	  }

	  public virtual void logWarnings()
	  {
		StringBuilder builder = new StringBuilder();
		foreach (Problem warning in warnings)
		{
		  builder.Append("\n* ");
		  builder.Append(warning.ToString());
		}
		LOG.logParseWarnings(builder.ToString());
	  }

	  public virtual void throwExceptionForErrors()
	  {
		StringBuilder strb = new StringBuilder();
		foreach (Problem error in errors)
		{
		  strb.Append("\n* ");
		  strb.Append(error.ToString());
		}
		throw LOG.exceptionDuringParsing(strb.ToString());
	  }

	  public virtual string SchemaResource
	  {
		  set
		  {
			SAXParserFactory saxParserFactory = parser.SaxParserFactory;
			saxParserFactory.NamespaceAware = true;
			saxParserFactory.Validating = true;
			try
			{
			  saxParserFactory.setFeature("http://xml.org/sax/features/namespace-prefixes", true);
			}
			catch (Exception e)
			{
			  LOG.unableToSetSchemaResource(e);
			}
			this.schemaResource = value;
		  }
	  }
	}

}