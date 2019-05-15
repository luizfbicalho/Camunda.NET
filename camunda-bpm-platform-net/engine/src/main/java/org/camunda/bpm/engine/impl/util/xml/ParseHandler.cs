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
namespace org.camunda.bpm.engine.impl.util.xml
{
	using Attributes = org.xml.sax.Attributes;
	using Locator = org.xml.sax.Locator;
	using SAXException = org.xml.sax.SAXException;
	using SAXParseException = org.xml.sax.SAXParseException;
	using DefaultHandler = org.xml.sax.helpers.DefaultHandler;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class ParseHandler : DefaultHandler
	{

	  protected internal string defaultNamespace;
	  protected internal Parse parse;
	  protected internal Locator locator;
	  protected internal Stack<Element> elementStack = new Stack<Element>();

	  public ParseHandler(Parse parse)
	  {
		this.parse = parse;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startElement(String uri, String localName, String qName, org.xml.sax.Attributes attributes) throws org.xml.sax.SAXException
	  public virtual void startElement(string uri, string localName, string qName, Attributes attributes)
	  {
		Element element = new Element(uri, localName, qName, attributes, locator);
		if (elementStack.Count == 0)
		{
		  parse.rootElement = element;
		}
		else
		{
		  elementStack.Peek().add(element);
		}
		elementStack.Push(element);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void characters(char[] ch, int start, int length) throws org.xml.sax.SAXException
	  public virtual void characters(char[] ch, int start, int length)
	  {
		elementStack.Peek().appendText(string.valueOf(ch, start, length));
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endElement(String uri, String localName, String qName) throws org.xml.sax.SAXException
	  public virtual void endElement(string uri, string localName, string qName)
	  {
		elementStack.Pop();
	  }

	  public virtual void error(SAXParseException e)
	  {
		parse.addError(e);
	  }
	  public virtual void fatalError(SAXParseException e)
	  {
		parse.addError(e);
	  }
	  public virtual void warning(SAXParseException e)
	  {
		parse.addWarning(e);
	  }
	  public virtual Locator DocumentLocator
	  {
		  set
		  {
			this.locator = value;
		  }
	  }

	  public virtual string DefaultNamespace
	  {
		  set
		  {
			this.defaultNamespace = value;
		  }
	  }


	}

}