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
namespace org.camunda.bpm.model.xml.instance
{

	using ModelInstanceImpl = org.camunda.bpm.model.xml.impl.ModelInstanceImpl;
	using Element = org.w3c.dom.Element;

	/// <summary>
	/// Encapsulates <seealso cref="Element"/>. Implementations of this interface must be thread-safe.
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public interface DomElement
	{

	  /// <summary>
	  /// Returns the namespace URI for this element.
	  /// </summary>
	  /// <returns> the namespace URI </returns>
	  string NamespaceURI {get;}

	  /// <summary>
	  /// Returns the local name of this element.
	  /// </summary>
	  /// <returns> the local name </returns>
	  string LocalName {get;}

	  /// <summary>
	  /// Returns the prefix of this element.
	  /// </summary>
	  /// <returns> the prefix </returns>
	  string Prefix {get;}

	  /// <summary>
	  /// Returns the DOM document which contains this element.
	  /// </summary>
	  /// <returns> the DOM document or null if the element itself is a document </returns>
	  DomDocument Document {get;}

	  /// <summary>
	  /// Returns the root element of the document which contains this element.
	  /// </summary>
	  /// <returns> the root element of the document or null if non exists </returns>
	  DomElement RootElement {get;}

	  /// <summary>
	  /// Returns the parent element of this element.
	  /// </summary>
	  /// <returns> the parent element or null if not part of a tree </returns>
	  DomElement ParentElement {get;}

	  /// <summary>
	  /// Returns all child elements of this element.
	  /// </summary>
	  /// <returns> the list of child elements </returns>
	  IList<DomElement> ChildElements {get;}

	  /// <summary>
	  /// Returns all child elements of this element with a specific namespace + name
	  /// </summary>
	  /// <returns> the list of child elements </returns>
	  IList<DomElement> getChildElementsByNameNs(string namespaceUris, string elementName);

	  /// <summary>
	  /// Returns all child elements of this element with specific namespaces + name.
	  /// </summary>
	  /// <returns> the list of child elements </returns>
	  IList<DomElement> getChildElementsByNameNs(ISet<string> namespaceUris, string elementName);

	  /// <summary>
	  /// Returns all child elements of this element with a specific type.
	  /// </summary>
	  /// <returns> the list of child elements matching the type </returns>
	  IList<DomElement> getChildElementsByType(ModelInstanceImpl modelInstance, Type elementType);

	  /// <summary>
	  /// Replaces a child element with a new element.
	  /// </summary>
	  /// <param name="newChildDomElement">  the new child element </param>
	  /// <param name="existingChildDomElement">  the existing child element </param>
	  /// <exception cref="ModelException"> if the child cannot be replaced </exception>
	  void replaceChild(DomElement newChildDomElement, DomElement existingChildDomElement);

	  /// <summary>
	  /// Removes a child element of this element.
	  /// </summary>
	  /// <param name="domElement">  the child element to remove </param>
	  /// <returns> true if the child element was removed otherwise false </returns>
	  bool removeChild(DomElement domElement);

	  /// <summary>
	  /// Appends the element to the child elements of this element.
	  /// </summary>
	  /// <param name="childElement">  the element to append </param>
	  void appendChild(DomElement childElement);

	  /// <summary>
	  /// Inserts the new child element after another child element. If the child element to
	  /// insert after is null the new child element will be inserted at the beginning.
	  /// </summary>
	  /// <param name="elementToInsert">  the new element to insert </param>
	  /// <param name="insertAfter">  the existing child element to insert after or null </param>
	  void insertChildElementAfter(DomElement elementToInsert, DomElement insertAfter);

	  /// <summary>
	  /// Checks if this element has a attribute under the namespace of this element.
	  /// </summary>
	  /// <param name="localName">  the name of the attribute </param>
	  /// <returns> true if the attribute exists otherwise false </returns>
	  bool hasAttribute(string localName);

	  /// <summary>
	  /// Checks if this element has a attribute with the given namespace.
	  /// </summary>
	  /// <param name="namespaceUri">  the namespaceUri of the namespace </param>
	  /// <param name="localName">  the name of the attribute </param>
	  /// <returns> true if the attribute exists otherwise false </returns>
	  bool hasAttribute(string namespaceUri, string localName);

	  /// <summary>
	  /// Returns the attribute value for the namespace of this element.
	  /// </summary>
	  /// <param name="attributeName">  the name of the attribute </param>
	  /// <returns> the value of the attribute or the empty string </returns>
	  string getAttribute(string attributeName);

	  /// <summary>
	  /// Returns the attribute value for the given namespace.
	  /// </summary>
	  /// <param name="namespaceUri">  the namespaceUri of the namespace </param>
	  /// <param name="localName">  the name of the attribute </param>
	  /// <returns> the value of the attribute or the empty string </returns>
	  string getAttribute(string namespaceUri, string localName);

	  /// <summary>
	  /// Sets the attribute value for the namespace of this element.
	  /// </summary>
	  /// <param name="localName">  the name of the attribute </param>
	  /// <param name="value">  the value to set </param>
	  void setAttribute(string localName, string value);

	  /// <summary>
	  /// Sets the attribute value for the given namespace.
	  /// </summary>
	  /// <param name="namespaceUri">  the namespaceUri of the namespace </param>
	  /// <param name="localName">  the name of the attribute </param>
	  /// <param name="value">  the value to set </param>
	  void setAttribute(string namespaceUri, string localName, string value);

	  /// <summary>
	  /// Sets the value of a id attribute for the namespace of this element.
	  /// </summary>
	  /// <param name="localName">  the name of the attribute </param>
	  /// <param name="value">  the value to set </param>
	  void setIdAttribute(string localName, string value);

	  /// <summary>
	  /// Sets the value of a id attribute for the given namespace.
	  /// </summary>
	  /// <param name="namespaceUri">  the namespaceUri of the namespace </param>
	  /// <param name="localName">  the name of the attribute </param>
	  /// <param name="value">  the value to set </param>
	  void setIdAttribute(string namespaceUri, string localName, string value);

	  /// <summary>
	  /// Removes the attribute for the namespace of this element.
	  /// </summary>
	  /// <param name="localName">  the name of the attribute </param>
	  void removeAttribute(string localName);

	  /// <summary>
	  /// Removes the attribute for the given namespace.
	  /// </summary>
	  /// <param name="namespaceUri">  the namespaceUri of the namespace </param>
	  /// <param name="localName">  the name of the attribute </param>
	  void removeAttribute(string namespaceUri, string localName);

	  /// <summary>
	  /// Gets the text content of this element all its descendants.
	  /// </summary>
	  /// <returns> the text content </returns>
	  string TextContent {get;set;}


	  /// <summary>
	  /// Adds a CDATA section to this element.
	  /// </summary>
	  /// <param name="textContent">  the CDATA content to set </param>
	  void addCDataSection(string data);

	  /// <summary>
	  /// Returns the <seealso cref="ModelElementInstance"/> which is associated with this element.
	  /// </summary>
	  /// <returns> the <seealso cref="ModelElementInstance"/> or null if non is associated </returns>
	  ModelElementInstance ModelElementInstance {get;set;}


	  /// <summary>
	  /// Adds a new namespace with a generated prefix to this element.
	  /// </summary>
	  /// <param name="namespaceUri">  the namespaceUri of the namespace </param>
	  /// <returns> the generated prefix for the new namespace </returns>
	  string registerNamespace(string namespaceUri);

	  /// <summary>
	  /// Adds a new namespace with prefix to this element.
	  /// </summary>
	  /// <param name="prefix">  the prefix of the namespace </param>
	  /// <param name="namespaceUri">  the namespaceUri of the namespace </param>
	  void registerNamespace(string prefix, string namespaceUri);

	  /// <summary>
	  /// Returns the prefix of the namespace starting from this node upwards.
	  /// The default namespace has the prefix {@code null}.
	  /// </summary>
	  /// <param name="namespaceUri">  the namespaceUri of the namespace </param>
	  /// <returns> the prefix or null if non is defined </returns>
	  string lookupPrefix(string namespaceUri);
	}

}