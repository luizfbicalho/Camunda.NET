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

	using Document = org.w3c.dom.Document;

	/// <summary>
	/// Encapsulates a <seealso cref="Document"/>. Implementations of this interface must be thread-safe.
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public interface DomDocument
	{

	  /// <summary>
	  /// Returns the root element of the document.
	  /// </summary>
	  /// <returns> the root element or null if non exists </returns>
	  DomElement RootElement {get;set;}


	  /// <summary>
	  /// Creates a new element in the dom document.
	  /// </summary>
	  /// <param name="namespaceUri">  the namespaceUri of the new element </param>
	  /// <param name="localName">  the localName of the new element </param>
	  /// <returns> the new DOM element </returns>
	  DomElement createElement(string namespaceUri, string localName);

	  /// <summary>
	  /// Gets an element by its id.
	  /// </summary>
	  /// <param name="id">  the id to search for </param>
	  /// <returns> the element or null if no such element exists </returns>
	  DomElement getElementById(string id);

	  /// <summary>
	  /// Gets all elements with the namespace and name.
	  /// </summary>
	  /// <param name="namespaceUri">  the element namespaceURI to search for </param>
	  /// <param name="localName">  the element name to search for </param>
	  /// <returns> the list of matching elements </returns>
	  IList<DomElement> getElementsByNameNs(string namespaceUri, string localName);

	  /// <summary>
	  /// Returns a new <seealso cref="DOMSource"/> of the document.
	  /// 
	  /// Note that a <seealso cref="DOMSource"/> wraps the underlying <seealso cref="Document"/> which is
	  /// not thread-safe. Multiple DOMSources of the same document should be synchronized
	  /// by the calling application.
	  /// </summary>
	  /// <returns> the new <seealso cref="DOMSource"/> </returns>
	  DOMSource DomSource {get;}

	  /// <summary>
	  /// Registers a new namespace with a generic prefix.
	  /// </summary>
	  /// <param name="namespaceUri">  the namespaceUri of the new namespace </param>
	  /// <returns> the used prefix </returns>
	  string registerNamespace(string namespaceUri);

	  /// <summary>
	  /// Registers a new namespace for the prefix.
	  /// </summary>
	  /// <param name="prefix">  the prefix of the new namespace </param>
	  /// <param name="namespaceUri">  the namespaceUri of the new namespace </param>
	  void registerNamespace(string prefix, string namespaceUri);

	  /// <summary>
	  /// Clones the DOM document.
	  /// </summary>
	  /// <returns> the cloned DOM document </returns>
	  DomDocument clone();

	}

}