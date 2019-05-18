using System;
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
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using org.w3c.dom;



	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class DomElementImpl : DomElement
	{

	  private const string MODEL_ELEMENT_KEY = "camunda.modelElementRef";

	  private readonly Element element;
	  private readonly Document document;

	  public DomElementImpl(Element element)
	  {
		this.element = element;
		this.document = element.OwnerDocument;
	  }

	  protected internal virtual Element Element
	  {
		  get
		  {
			return element;
		  }
	  }

	  public virtual string NamespaceURI
	  {
		  get
		  {
			lock (document)
			{
			  return element.NamespaceURI;
			}
		  }
	  }

	  public virtual string LocalName
	  {
		  get
		  {
			lock (document)
			{
			  return element.LocalName;
			}
		  }
	  }

	  public virtual string Prefix
	  {
		  get
		  {
			lock (document)
			{
			  return element.Prefix;
			}
		  }
	  }

	  public virtual DomDocument Document
	  {
		  get
		  {
			lock (document)
			{
			  Document ownerDocument = element.OwnerDocument;
			  if (ownerDocument != null)
			  {
				return new DomDocumentImpl(ownerDocument);
			  }
			  else
			  {
				return null;
			  }
			}
		  }
	  }

	  public virtual DomElement RootElement
	  {
		  get
		  {
			lock (document)
			{
			  DomDocument document = Document;
			  if (document != null)
			  {
				return document.RootElement;
			  }
			  else
			  {
				return null;
			  }
			}
		  }
	  }

	  public virtual DomElement ParentElement
	  {
		  get
		  {
			lock (document)
			{
			  Node parentNode = element.ParentNode;
			  if (parentNode != null && parentNode is Element)
			  {
				return new DomElementImpl((Element) parentNode);
			  }
			  else
			  {
				return null;
			  }
			}
		  }
	  }

	  public virtual IList<DomElement> ChildElements
	  {
		  get
		  {
			lock (document)
			{
			  NodeList childNodes = element.ChildNodes;
			  return DomUtil.filterNodeListForElements(childNodes);
			}
		  }
	  }

	  public virtual IList<DomElement> getChildElementsByNameNs(string namespaceUri, string elementName)
	  {
		lock (document)
		{
		  NodeList childNodes = element.ChildNodes;
		  return DomUtil.filterNodeListByName(childNodes, namespaceUri, elementName);
		}
	  }

	  public virtual IList<DomElement> getChildElementsByNameNs(ISet<string> namespaceUris, string elementName)
	  {
		IList<DomElement> result = new List<DomElement>();
		foreach (string @namespace in namespaceUris)
		{
		  if (!string.ReferenceEquals(@namespace, null))
		  {
			((IList<DomElement>)result).AddRange(getChildElementsByNameNs(@namespace, elementName));
		  }
		}
		return result;
	  }

	  public virtual IList<DomElement> getChildElementsByType(ModelInstanceImpl modelInstance, Type elementType)
	  {
		lock (document)
		{
		  NodeList childNodes = element.ChildNodes;
		  return DomUtil.filterNodeListByType(childNodes, modelInstance, elementType);
		}
	  }

	  public virtual void replaceChild(DomElement newChildDomElement, DomElement existingChildDomElement)
	  {
		lock (document)
		{
		  Element newElement = ((DomElementImpl) newChildDomElement).Element;
		  Element existingElement = ((DomElementImpl) existingChildDomElement).Element;
		  try
		  {
			element.replaceChild(newElement, existingElement);
		  }
		  catch (DOMException e)
		  {
			throw new ModelException("Unable to replace child <" + existingElement + "> of element <" + element + "> with element <" + newElement + ">", e);
		  }
		}
	  }

	  public virtual bool removeChild(DomElement childDomElement)
	  {
		lock (document)
		{
		  Element childElement = ((DomElementImpl) childDomElement).Element;
		  try
		  {
			element.removeChild(childElement);
			return true;
		  }
		  catch (DOMException)
		  {
			return false;
		  }
		}
	  }

	  public virtual void appendChild(DomElement childDomElement)
	  {
		lock (document)
		{
		  Element childElement = ((DomElementImpl) childDomElement).Element;
		  element.appendChild(childElement);
		}
	  }

	  public virtual void insertChildElementAfter(DomElement elementToInsert, DomElement insertAfter)
	  {
		lock (document)
		{
		  Element newElement = ((DomElementImpl) elementToInsert).Element;
		  // find node to insert before
		  Node insertBeforeNode;
		  if (insertAfter == null)
		  {
			insertBeforeNode = element.FirstChild;
		  }
		  else
		  {
			insertBeforeNode = ((DomElementImpl) insertAfter).Element.NextSibling;
		  }

		  // insert before node or append if no node was found
		  if (insertBeforeNode != null)
		  {
			element.insertBefore(newElement, insertBeforeNode);
		  }
		  else
		  {
			element.appendChild(newElement);
		  }
		}
	  }

	  public virtual bool hasAttribute(string localName)
	  {
		return hasAttribute(null, localName);
	  }

	  public virtual bool hasAttribute(string namespaceUri, string localName)
	  {
		lock (document)
		{
		  return element.hasAttributeNS(namespaceUri, localName);
		}
	  }

	  public virtual string getAttribute(string attributeName)
	  {
		return getAttribute(null, attributeName);
	  }


	  public virtual string getAttribute(string namespaceUri, string localName)
	  {
		lock (document)
		{
		  XmlQName xmlQName = new XmlQName(this, namespaceUri, localName);
		  string value;
		  if (xmlQName.hasLocalNamespace())
		  {
			value = element.getAttributeNS(null, xmlQName.LocalName);
		  }
		  else
		  {
			value = element.getAttributeNS(xmlQName.NamespaceUri, xmlQName.LocalName);
		  }
		  if (value.Length == 0)
		  {
			return null;
		  }
		  else
		  {
			return value;
		  }
		}
	  }

	  public virtual void setAttribute(string localName, string value)
	  {
		setAttribute(null, localName, value);
	  }

	  public virtual void setAttribute(string namespaceUri, string localName, string value)
	  {
		setAttribute(namespaceUri, localName, value, false);
	  }

	  private void setAttribute(string namespaceUri, string localName, string value, bool isIdAttribute)
	  {
		lock (document)
		{
		  XmlQName xmlQName = new XmlQName(this, namespaceUri, localName);
		  if (xmlQName.hasLocalNamespace())
		  {
			element.setAttributeNS(null, xmlQName.LocalName, value);
			if (isIdAttribute)
			{
			  element.setIdAttributeNS(null, xmlQName.LocalName, true);
			}
		  }
		  else
		  {
			element.setAttributeNS(xmlQName.NamespaceUri, xmlQName.PrefixedName, value);
			if (isIdAttribute)
			{
			  element.setIdAttributeNS(xmlQName.NamespaceUri, xmlQName.LocalName, true);
			}
		  }
		}
	  }

	  public virtual void setIdAttribute(string localName, string value)
	  {
		setIdAttribute(NamespaceURI, localName, value);
	  }

	  public virtual void setIdAttribute(string namespaceUri, string localName, string value)
	  {
		setAttribute(namespaceUri, localName, value, true);
	  }

	  public virtual void removeAttribute(string localName)
	  {
		removeAttribute(NamespaceURI, localName);
	  }

	  public virtual void removeAttribute(string namespaceUri, string localName)
	  {
		lock (document)
		{
		  XmlQName xmlQName = new XmlQName(this, namespaceUri, localName);
		  if (xmlQName.hasLocalNamespace())
		  {
			element.removeAttributeNS(null, xmlQName.LocalName);
		  }
		  else
		  {
			element.removeAttributeNS(xmlQName.NamespaceUri, xmlQName.LocalName);
		  }
		}
	  }

	  public virtual string TextContent
	  {
		  get
		  {
			lock (document)
			{
			  return element.TextContent;
			}
		  }
		  set
		  {
			lock (document)
			{
			  element.TextContent = value;
			}
		  }
	  }


	  public virtual void addCDataSection(string data)
	  {
		lock (document)
		{
		  CDATASection cdataSection = document.createCDATASection(data);
		  element.appendChild(cdataSection);
		}
	  }

	  public virtual ModelElementInstance ModelElementInstance
	  {
		  get
		  {
			lock (document)
			{
			  return (ModelElementInstance) element.getUserData(MODEL_ELEMENT_KEY);
			}
		  }
		  set
		  {
			lock (document)
			{
			  element.setUserData(MODEL_ELEMENT_KEY, value, null);
			}
		  }
	  }


	  public virtual string registerNamespace(string namespaceUri)
	  {
		lock (document)
		{
		  string lookupPrefix = lookupPrefix(namespaceUri);
		  if (string.ReferenceEquals(lookupPrefix, null))
		  {
			// check if a prefix is known
			string prefix = XmlQName.KNOWN_PREFIXES[namespaceUri];
			// check if prefix is not already used
			if (!string.ReferenceEquals(prefix, null) && RootElement != null && RootElement.hasAttribute(XMLNS_ATTRIBUTE_NS_URI, prefix))
			{
			  prefix = null;
			}
			if (string.ReferenceEquals(prefix, null))
			{
			  // generate prefix
			  prefix = ((DomDocumentImpl) Document).UnusedGenericNsPrefix;
			}
			registerNamespace(prefix, namespaceUri);
			return prefix;
		  }
		  else
		  {
			return lookupPrefix;
		  }
		}
	  }

	  public virtual void registerNamespace(string prefix, string namespaceUri)
	  {
		lock (document)
		{
		  element.setAttributeNS(XMLNS_ATTRIBUTE_NS_URI, XMLNS_ATTRIBUTE + ":" + prefix, namespaceUri);
		}
	  }

	  public virtual string lookupPrefix(string namespaceUri)
	  {
		lock (document)
		{
		  return element.lookupPrefix(namespaceUri);
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

		DomElementImpl that = (DomElementImpl) o;
		return element.Equals(that.element);
	  }

	  public override int GetHashCode()
	  {
		return element.GetHashCode();
	  }

	}

}