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
namespace org.camunda.bpm.model.xml.impl.instance
{
	using DomUtil = org.camunda.bpm.model.xml.impl.util.DomUtil;
	using XmlQName = org.camunda.bpm.model.xml.impl.util.XmlQName;
	using DomDocument = org.camunda.bpm.model.xml.instance.DomDocument;
	using DomElement = org.camunda.bpm.model.xml.instance.DomElement;
	using Document = org.w3c.dom.Document;
	using Element = org.w3c.dom.Element;
	using NodeList = org.w3c.dom.NodeList;


	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class DomDocumentImpl : DomDocument
	{

	  public const string GENERIC_NS_PREFIX = "ns";

	  private readonly Document document;

	  public DomDocumentImpl(Document document)
	  {
		this.document = document;
	  }

	  public virtual DomElement RootElement
	  {
		  get
		  {
			lock (document)
			{
			  Element documentElement = document.DocumentElement;
			  if (documentElement != null)
			  {
				return new DomElementImpl(documentElement);
			  }
			  else
			  {
				return null;
			  }
			}
    
		  }
		  set
		  {
			lock (document)
			{
			  Element documentElement = document.DocumentElement;
			  Element newDocumentElement = ((DomElementImpl) value).Element;
			  if (documentElement != null)
			  {
				document.replaceChild(newDocumentElement, documentElement);
			  }
			  else
			  {
				document.appendChild(newDocumentElement);
			  }
			}
		  }
	  }


	  public virtual DomElement createElement(string namespaceUri, string localName)
	  {
		lock (document)
		{
		  XmlQName xmlQName = new XmlQName(this, namespaceUri, localName);
		  Element element = document.createElementNS(xmlQName.NamespaceUri, xmlQName.PrefixedName);
		  return new DomElementImpl(element);
		}
	  }

	  public virtual DomElement getElementById(string id)
	  {
		lock (document)
		{
		  Element element = document.getElementById(id);
		  if (element != null)
		  {
			return new DomElementImpl(element);
		  }
		  else
		  {
			return null;
		  }
		}
	  }

	  public virtual IList<DomElement> getElementsByNameNs(string namespaceUri, string localName)
	  {
		lock (document)
		{
		  NodeList elementsByTagNameNS = document.getElementsByTagNameNS(namespaceUri, localName);
		  return DomUtil.filterNodeListByName(elementsByTagNameNS, namespaceUri, localName);
		}
	  }

	  public virtual DOMSource DomSource
	  {
		  get
		  {
			return new DOMSource(document);
		  }
	  }

	  public virtual string registerNamespace(string namespaceUri)
	  {
		lock (document)
		{
		  DomElement rootElement = RootElement;
		  if (rootElement != null)
		  {
			return rootElement.registerNamespace(namespaceUri);
		  }
		  else
		  {
			throw new ModelException("Unable to define a new namespace without a root document element");
		  }
		}
	  }

	  public virtual void registerNamespace(string prefix, string namespaceUri)
	  {
		lock (document)
		{
		  DomElement rootElement = RootElement;
		  if (rootElement != null)
		  {
			rootElement.registerNamespace(prefix, namespaceUri);
		  }
		  else
		  {
			throw new ModelException("Unable to define a new namespace without a root document element");
		  }
		}
	  }

	  protected internal virtual string UnusedGenericNsPrefix
	  {
		  get
		  {
			lock (document)
			{
			  Element documentElement = document.DocumentElement;
			  if (documentElement == null)
			  {
				return GENERIC_NS_PREFIX + "0";
			  }
			  else
			  {
				for (int i = 0; i < int.MaxValue; i++)
				{
				  if (!documentElement.hasAttributeNS(XMLNS_ATTRIBUTE_NS_URI, GENERIC_NS_PREFIX + i))
				  {
					return GENERIC_NS_PREFIX + i;
				  }
				}
				throw new ModelException("Unable to find an unused namespace prefix");
			  }
			}
		  }
	  }

	  public virtual DomDocument clone()
	  {
		lock (document)
		{
		  return new DomDocumentImpl((Document) document.cloneNode(true));
		}
	  }

	  public override bool Equals(object o)
	  {
		if (this == o)
		{
		  return true;
		}
		if (o == null || this.GetType() != o.GetType())
		{
		  return false;
		}

		DomDocumentImpl that = (DomDocumentImpl) o;
		return document.Equals(that.document);
	  }

	  public override int GetHashCode()
	  {
		return document.GetHashCode();
	  }
	}

}