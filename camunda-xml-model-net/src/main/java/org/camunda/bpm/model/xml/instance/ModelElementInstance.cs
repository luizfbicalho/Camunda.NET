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
	using ModelElementInstanceImpl = org.camunda.bpm.model.xml.impl.instance.ModelElementInstanceImpl;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;

	/// <summary>
	/// An instance of a <seealso cref="ModelElementType"/>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface ModelElementInstance
	{

	  /// <summary>
	  /// Returns the represented DOM <seealso cref="DomElement"/>.
	  /// </summary>
	  /// <returns> the DOM element </returns>
	  DomElement DomElement {get;}

	  /// <summary>
	  /// Returns the model instance which contains this type instance.
	  /// </summary>
	  /// <returns> the model instance </returns>

	  ModelInstance ModelInstance {get;}
	  /// <summary>
	  /// Returns the parent element of this.
	  /// </summary>
	  /// <returns> the parent element </returns>
	  ModelElementInstance ParentElement {get;}

	  /// <summary>
	  /// Returns the element type of this.
	  /// </summary>
	  /// <returns> the element type </returns>
	  ModelElementType ElementType {get;}

	  /// <summary>
	  /// Returns the attribute value for the attribute name.
	  /// </summary>
	  /// <param name="attributeName">  the name of the attribute </param>
	  /// <returns> the value of the attribute </returns>
	  string getAttributeValue(string attributeName);

	  /// <summary>
	  /// Sets the value by name of a non-ID attribute.
	  /// </summary>
	  /// <param name="attributeName">  the name of the attribute </param>
	  /// <param name="xmlValue">  the value to set </param>
	  void setAttributeValue(string attributeName, string xmlValue);

	  /// <summary>
	  /// Sets attribute value by name.
	  /// </summary>
	  /// <param name="attributeName">  the name of the attribute </param>
	  /// <param name="xmlValue">  the value to set </param>
	  /// <param name="isIdAttribute">  true if the attribute is an ID attribute, false otherwise </param>
	  void setAttributeValue(string attributeName, string xmlValue, bool isIdAttribute);

	  /// <summary>
	  /// Sets attribute value by name.
	  /// </summary>
	  /// <param name="attributeName">  the name of the attribute </param>
	  /// <param name="xmlValue">  the value to set </param>
	  /// <param name="isIdAttribute">  true if the attribute is an ID attribute, false otherwise </param>
	  /// <param name="withReferenceUpdate">  true to update incoming references in other elements, false otherwise </param>
	  void setAttributeValue(string attributeName, string xmlValue, bool isIdAttribute, bool withReferenceUpdate);

	  /// <summary>
	  /// Removes attribute by name.
	  /// </summary>
	  /// <param name="attributeName">  the name of the attribute </param>
	  void removeAttribute(string attributeName);

	  /// <summary>
	  /// Returns the attribute value for the given attribute name and namespace URI.
	  /// </summary>
	  /// <param name="namespaceUri">  the namespace URI of the attribute </param>
	  /// <param name="attributeName">  the attribute name of the attribute </param>
	  /// <returns> the value of the attribute </returns>
	  string getAttributeValueNs(string namespaceUri, string attributeName);

	  /// <summary>
	  /// Sets the value by name and namespace of a non-ID attribute.
	  /// </summary>
	  /// <param name="namespaceUri">  the namespace URI of the attribute </param>
	  /// <param name="attributeName">  the name of the attribute </param>
	  /// <param name="xmlValue">  the XML value to set </param>
	  void setAttributeValueNs(string namespaceUri, string attributeName, string xmlValue);

	  /// <summary>
	  /// Sets the attribute value by name and namespace.
	  /// </summary>
	  /// <param name="namespaceUri">  the namespace URI of the attribute </param>
	  /// <param name="attributeName">  the name of the attribute </param>
	  /// <param name="xmlValue">  the XML value to set </param>
	  /// <param name="isIdAttribute">  true if the attribute is an ID attribute, false otherwise </param>
	  void setAttributeValueNs(string namespaceUri, string attributeName, string xmlValue, bool isIdAttribute);

	  /// <summary>
	  /// Sets the attribute value by name and namespace.
	  /// </summary>
	  /// <param name="namespaceUri">  the namespace URI of the attribute </param>
	  /// <param name="attributeName">  the name of the attribute </param>
	  /// <param name="xmlValue">  the XML value to set </param>
	  /// <param name="isIdAttribute">  true if the attribute is an ID attribute, false otherwise </param>
	  /// <param name="withReferenceUpdate">  true to update incoming references in other elements, false otherwise </param>
	  void setAttributeValueNs(string namespaceUri, string attributeName, string xmlValue, bool isIdAttribute, bool withReferenceUpdate);

	  /// <summary>
	  /// Removes the attribute by name and namespace.
	  /// </summary>
	  /// <param name="namespaceUri">  the namespace URI of the attribute </param>
	  /// <param name="attributeName">  the name of the attribute </param>
	  void removeAttributeNs(string namespaceUri, string attributeName);

	  /// <summary>
	  /// Returns the text content of the DOM element without leading and trailing spaces. For
	  /// raw text content see <seealso cref="ModelElementInstanceImpl#getRawTextContent()"/>.
	  /// </summary>
	  /// <returns> text content of underlying DOM element with leading and trailing whitespace trimmed </returns>
	  string TextContent {get;set;}

	  /// <summary>
	  /// Returns the raw text content of the DOM element including all whitespaces.
	  /// </summary>
	  /// <returns> raw text content of underlying DOM element </returns>
	  string RawTextContent {get;}


	  /// <summary>
	  /// Replaces this element with a new element and updates references.
	  /// </summary>
	  /// <param name="newElement">  the new element to replace with </param>
	  void replaceWithElement(ModelElementInstance newElement);

	  /// <summary>
	  /// Returns a child element with the given name or 'null' if no such element exists
	  /// </summary>
	  /// <param name="namespaceUri"> the local name of the element </param>
	  /// <param name="elementName"> the namespace of the element </param>
	  /// <returns> the child element or null. </returns>
	  ModelElementInstance getUniqueChildElementByNameNs(string namespaceUri, string elementName);

	  /// <summary>
	  /// Returns a child element with the given type
	  /// </summary>
	  /// <param name="elementType">  the type of the element </param>
	  /// <returns> the child element or null </returns>
	  ModelElementInstance getUniqueChildElementByType(Type elementType);

	  /// <summary>
	  /// Adds or replaces a child element by name. Replaces an existing Child Element with the same name
	  /// or adds a new child if no such element exists.
	  /// </summary>
	  /// <param name="newChild"> the child to add </param>
	  ModelElementInstance UniqueChildElementByNameNs {set;}

	  /// <summary>
	  /// Replace an existing child element with a new child element. Changes the underlying DOM element tree.
	  /// </summary>
	  /// <param name="existingChild"> the child element to replace </param>
	  /// <param name="newChild"> the new child element </param>
	  void replaceChildElement(ModelElementInstance existingChild, ModelElementInstance newChild);

	  /// <summary>
	  /// Adds a new child element to the children of this element. The child
	  /// is inserted at the correct position of the allowed child types.
	  /// Updates the underlying DOM element tree.
	  /// </summary>
	  /// <param name="newChild"> the new child element </param>
	  /// <exception cref="ModelException"> if the new child type is not an allowed child type </exception>
	  void addChildElement(ModelElementInstance newChild);

	  /// <summary>
	  /// Removes the child element from this.
	  /// </summary>
	  /// <param name="child">  the child element to remove </param>
	  /// <returns> true if the child element could be removed </returns>
	  bool removeChildElement(ModelElementInstance child);

	  /// <summary>
	  /// Return all child elements of a given type
	  /// </summary>
	  /// <param name="childElementType"> the child element type to search for </param>
	  /// <returns> a collection of elements of the given type </returns>
	  ICollection<ModelElementInstance> getChildElementsByType(ModelElementType childElementType);

	  /// <summary>
	  /// Return all child elements of a given type
	  /// </summary>
	  /// <param name="childElementClass">  the class of the child element type to search for </param>
	  /// <returns> a collection of elements to the given type </returns>
	  ICollection<T> getChildElementsByType<T>(Type childElementClass);

	  /// <summary>
	  /// Inserts the new element after the given element or at the beginning if the given element is null.
	  /// </summary>
	  /// <param name="elementToInsert">  the new element to insert </param>
	  /// <param name="insertAfterElement">  the element to insert after or null to insert at first position </param>
	  void insertElementAfter(ModelElementInstance elementToInsert, ModelElementInstance insertAfterElement);

	  /// <summary>
	  /// Execute updates after the element was inserted as a replacement of another element.
	  /// </summary>
	  void updateAfterReplacement();
	}

}