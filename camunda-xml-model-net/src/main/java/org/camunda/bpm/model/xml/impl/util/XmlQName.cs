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
namespace org.camunda.bpm.model.xml.impl.util
{
	using DomDocument = org.camunda.bpm.model.xml.instance.DomDocument;
	using DomElement = org.camunda.bpm.model.xml.instance.DomElement;


	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class XmlQName
	{

	  public static readonly IDictionary<string, string> KNOWN_PREFIXES;
	  static XmlQName()
	  {
		KNOWN_PREFIXES = new Dictionary<string, string>();
		KNOWN_PREFIXES["http://www.camunda.com/fox"] = "fox";
		KNOWN_PREFIXES["http://activiti.org/bpmn"] = "camunda";
		KNOWN_PREFIXES["http://camunda.org/schema/1.0/bpmn"] = "camunda";
		KNOWN_PREFIXES["http://www.omg.org/spec/BPMN/20100524/MODEL"] = "bpmn2";
		KNOWN_PREFIXES["http://www.omg.org/spec/BPMN/20100524/DI"] = "bpmndi";
		KNOWN_PREFIXES["http://www.omg.org/spec/DD/20100524/DI"] = "di";
		KNOWN_PREFIXES["http://www.omg.org/spec/DD/20100524/DC"] = "dc";
		KNOWN_PREFIXES[XMLNS_ATTRIBUTE_NS_URI] = "";
	  }

	  protected internal DomElement rootElement;
	  protected internal DomElement element;

	  protected internal string localName;
	  protected internal string namespaceUri;
	  protected internal string prefix;

	  public XmlQName(DomDocument document, string namespaceUri, string localName) : this(document, null, namespaceUri, localName)
	  {
	  }

	  public XmlQName(DomElement element, string namespaceUri, string localName) : this(element.Document, element, namespaceUri, localName)
	  {
	  }

	  public XmlQName(DomDocument document, DomElement element, string namespaceUri, string localName)
	  {
		this.rootElement = document.RootElement;
		this.element = element;
		this.localName = localName;
		this.namespaceUri = namespaceUri;
		this.prefix = null;
	  }

	  public virtual string NamespaceUri
	  {
		  get
		  {
			return namespaceUri;
		  }
	  }

	  public virtual string LocalName
	  {
		  get
		  {
			return localName;
		  }
	  }

	  public virtual string PrefixedName
	  {
		  get
		  {
			if (string.ReferenceEquals(prefix, null))
			{
			  lock (this)
			  {
				if (string.ReferenceEquals(prefix, null))
				{
				  this.prefix = determinePrefixAndNamespaceUri();
				}
			  }
			}
			return QName.combine(prefix, localName);
		  }
	  }

	  public virtual bool hasLocalNamespace()
	  {
		if (element != null)
		{
		  return element.NamespaceURI.Equals(namespaceUri);
		}
		else
		{
		  return false;
		}
	  }

	  private string determinePrefixAndNamespaceUri()
	  {
		if (!string.ReferenceEquals(namespaceUri, null))
		{
		  if (rootElement != null && namespaceUri.Equals(rootElement.NamespaceURI))
		  {
			// global namespaces do not have a prefix or namespace URI
			return null;
		  }
		  else
		  {
			// lookup for prefix
			string lookupPrefix = lookupPrefix();
			if (string.ReferenceEquals(lookupPrefix, null) && rootElement != null)
			{
			  // if no prefix is found we generate a new one
			  // search for known prefixes
			 string knownPrefix = KNOWN_PREFIXES[namespaceUri];
			  if (string.ReferenceEquals(knownPrefix, null))
			  {
				// generate namespace
				return rootElement.registerNamespace(namespaceUri);
			  }
			  else if (knownPrefix.Length == 0)
			  {
				// ignored namespace
				return null;
			  }
			  else
			  {
				// register known prefix
				rootElement.registerNamespace(knownPrefix, namespaceUri);
				return knownPrefix;
			  }
			}
			else
			{
			  return lookupPrefix;
			}
		  }
		}
		else
		{
		  // no namespace so no prefix
		  return null;
		}
	  }

	  private string lookupPrefix()
	  {
		if (!string.ReferenceEquals(namespaceUri, null))
		{
		  string lookupPrefix = null;
		  if (element != null)
		  {
			lookupPrefix = element.lookupPrefix(namespaceUri);
		  }
		  else if (rootElement != null)
		  {
			lookupPrefix = rootElement.lookupPrefix(namespaceUri);
		  }
		  return lookupPrefix;
		}
		else
		{
		  return null;
		}
	  }

	}

}