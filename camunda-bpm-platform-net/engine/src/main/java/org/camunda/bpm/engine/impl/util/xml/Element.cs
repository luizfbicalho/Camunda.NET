using System.Collections.Generic;
using System.Text;

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


	/// <summary>
	/// Represents one XML element.
	/// 
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public class Element
	{

	  protected internal string uri;
	  protected internal string tagName;

	  /*
	   * Key of map = 'uri':attributeName
	   *
	   * if namespace is empty, key is 'attributeName'
	   */
	  protected internal IDictionary<string, Attribute> attributeMap = new Dictionary<string, Attribute>();

	  protected internal int line;
	  protected internal int column;
	  protected internal StringBuilder text = new StringBuilder();
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal IList<Element> elements_Renamed = new List<Element>();

	  public Element(string uri, string localName, string qName, Attributes attributes, Locator locator)
	  {
		this.uri = uri;
		this.tagName = (string.ReferenceEquals(uri, null) || uri.Equals("")) ? qName : localName;

		if (attributes != null)
		{
		  for (int i = 0; i < attributes.Length; i++)
		  {
			string attributeUri = attributes.getURI(i);
			string name = (string.ReferenceEquals(attributeUri, null) || attributeUri.Equals("")) ? attributes.getQName(i) : attributes.getLocalName(i);
			string value = attributes.getValue(i);
			this.attributeMap[composeMapKey(attributeUri, name)] = new Attribute(name, value, attributeUri);
		  }
		}

		if (locator != null)
		{
		  line = locator.LineNumber;
		  column = locator.ColumnNumber;
		}
	  }

	  public virtual IList<Element> elements(string tagName)
	  {
		return elementsNS((string) null, tagName);
	  }

	  public virtual IList<Element> elementsNS(Namespace nameSpace, string tagName)
	  {
		IList<Element> elementsNS = elementsNS(nameSpace.NamespaceUri, tagName);
		if (elementsNS.Count == 0 && nameSpace.hasAlternativeUri())
		{
		  elementsNS = elementsNS(nameSpace.AlternativeUri, tagName);
		}
		return elementsNS;
	  }

	  protected internal virtual IList<Element> elementsNS(string nameSpaceUri, string tagName)
	  {
		IList<Element> selectedElements = new List<Element>();
		foreach (Element element in elements_Renamed)
		{
		  if (tagName.Equals(element.TagName))
		  {
			if (string.ReferenceEquals(nameSpaceUri, null) || (!string.ReferenceEquals(nameSpaceUri, null) && nameSpaceUri.Equals(element.Uri)))
			{
			  selectedElements.Add(element);
			}
		  }
		}
		return selectedElements;
	  }

	  public virtual Element element(string tagName)
	  {
		return elementNS(new Namespace(null), tagName);
	  }

	  public virtual Element elementNS(Namespace nameSpace, string tagName)
	  {
		IList<Element> elements = elementsNS(nameSpace.NamespaceUri, tagName);
		if (elements.Count == 0 && nameSpace.hasAlternativeUri())
		{
		  elements = elementsNS(nameSpace.AlternativeUri, tagName);
		}
		if (elements.Count == 0)
		{
		  return null;
		}
		else if (elements.Count > 1)
		{
		  throw new ProcessEngineException("Parsing exception: multiple elements with tag name " + tagName + " found");
		}
		return elements[0];
	  }

	  public virtual void add(Element element)
	  {
		elements_Renamed.Add(element);
	  }

	  public virtual string attribute(string name)
	  {
		if (attributeMap.ContainsKey(name))
		{
		  return attributeMap[name].Value;
		}
		return null;
	  }

	  public virtual ISet<string> attributes()
	  {
		return attributeMap.Keys;
	  }

	  public virtual string attributeNS(Namespace @namespace, string name)
	  {
		string attribute = attribute(composeMapKey(@namespace.NamespaceUri, name));
		if (string.ReferenceEquals(attribute, null) && @namespace.hasAlternativeUri())
		{
		  attribute = attribute(composeMapKey(@namespace.AlternativeUri, name));
		}
		return attribute;
	  }

	  public virtual string attribute(string name, string defaultValue)
	  {
		if (attributeMap.ContainsKey(name))
		{
		  return attributeMap[name].Value;
		}
		return defaultValue;
	  }

	  public virtual string attributeNS(Namespace @namespace, string name, string defaultValue)
	  {
		string attribute = attribute(composeMapKey(@namespace.NamespaceUri, name));
		if (string.ReferenceEquals(attribute, null) && @namespace.hasAlternativeUri())
		{
		  attribute = attribute(composeMapKey(@namespace.AlternativeUri, name));
		}
		if (string.ReferenceEquals(attribute, null))
		{
		  return defaultValue;
		}
		return attribute;
	  }

	  protected internal virtual string composeMapKey(string attributeUri, string attributeName)
	  {
		StringBuilder strb = new StringBuilder();
		if (!string.ReferenceEquals(attributeUri, null) && !attributeUri.Equals(""))
		{
		  strb.Append(attributeUri);
		  strb.Append(":");
		}
		strb.Append(attributeName);
		return strb.ToString();
	  }

	  public virtual IList<Element> elements()
	  {
		return elements_Renamed;
	  }

	  public override string ToString()
	  {
		return "<" + tagName + "...";
	  }


	  public virtual string Uri
	  {
		  get
		  {
			return uri;
		  }
	  }
	  public virtual string TagName
	  {
		  get
		  {
			return tagName;
		  }
	  }
	  public virtual int Line
	  {
		  get
		  {
			return line;
		  }
	  }
	  public virtual int Column
	  {
		  get
		  {
			return column;
		  }
	  }
	  /// <summary>
	  /// Due to the nature of SAX parsing, sometimes the characters of an element
	  /// are not processed at once. So instead of a setText operation, we need
	  /// to have an appendText operation.
	  /// </summary>
	  public virtual void appendText(string text)
	  {
		this.text.Append(text);
	  }
	  public virtual string Text
	  {
		  get
		  {
			return text.ToString();
		  }
	  }

	  /// <summary>
	  /// allows to recursively collect the ids of all elements in the tree.
	  /// </summary>
	  public virtual void collectIds(IList<string> ids)
	  {
		ids.Add(attribute("id"));
		foreach (Element child in elements_Renamed)
		{
		  child.collectIds(ids);
		}
	  }
	}

}