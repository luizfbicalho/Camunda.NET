using System;
using System.Collections.Generic;
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
namespace org.camunda.bpm.model.xml.impl.util
{
	using DomDocumentImpl = org.camunda.bpm.model.xml.impl.instance.DomDocumentImpl;
	using DomElementImpl = org.camunda.bpm.model.xml.impl.instance.DomElementImpl;
	using DomDocument = org.camunda.bpm.model.xml.instance.DomDocument;
	using DomElement = org.camunda.bpm.model.xml.instance.DomElement;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using Element = org.w3c.dom.Element;
	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;
	using ErrorHandler = org.xml.sax.ErrorHandler;
	using SAXException = org.xml.sax.SAXException;
	using SAXParseException = org.xml.sax.SAXParseException;


	/// <summary>
	/// Helper methods which abstract some gruesome DOM specifics.
	/// It does not provide synchronization when invoked in parallel with
	/// the same objects.
	/// 
	/// @author Daniel Meyer
	/// @author Sebastian Menski
	/// 
	/// </summary>
	public sealed class DomUtil
	{

	  /// <summary>
	  /// A <seealso cref="NodeListFilter"/> allows to filter a <seealso cref="NodeList"/>,
	  /// retaining only elements in the list which match the filter.
	  /// </summary>
	  /// <seealso cref= DomUtil#filterNodeList(NodeList, NodeListFilter) </seealso>
	  public interface NodeListFilter
	  {

		/// <summary>
		/// Test if node matches the filter
		/// </summary>
		/// <param name="node"> the node to match </param>
		/// <returns> true if the filter does match the node, false otherwise </returns>
		bool matches(Node node);

	  }

	  /// <summary>
	  /// Filter retaining only Nodes of type <seealso cref="Node#ELEMENT_NODE"/>
	  /// 
	  /// </summary>
	  public class ElementNodeListFilter : NodeListFilter
	  {

		public virtual bool matches(Node node)
		{
		  return node.NodeType == Node.ELEMENT_NODE;
		}

	  }

	  /// <summary>
	  /// Filters <seealso cref="Element Elements"/> by their nodeName + namespaceUri
	  /// 
	  /// </summary>
	  public class ElementByNameListFilter : ElementNodeListFilter
	  {

		internal readonly string localName;
		internal readonly string namespaceUri;

		/// <param name="localName"> the local name to filter for </param>
		/// <param name="namespaceUri"> the namespaceUri to filter for </param>
		public ElementByNameListFilter(string localName, string namespaceUri)
		{
		  this.localName = localName;
		  this.namespaceUri = namespaceUri;
		}

		public override bool matches(Node node)
		{
		 return base.matches(node) && localName.Equals(node.LocalName) && namespaceUri.Equals(node.NamespaceURI);
		}

	  }

	  public class ElementByTypeListFilter : ElementNodeListFilter
	  {

		internal readonly Type type;
		internal readonly ModelInstanceImpl model;

		public ElementByTypeListFilter(Type type, ModelInstanceImpl modelInstance)
		{
		  this.type = type;
		  this.model = modelInstance;
		}

		public override bool matches(Node node)
		{
		  if (!base.matches(node))
		  {
			return false;
		  }
		  ModelElementInstance modelElement = ModelUtil.getModelElement(new DomElementImpl((Element) node), model);
		  return type.IsAssignableFrom(modelElement.GetType());
		}
	  }

	  /// <summary>
	  /// Allows to apply a <seealso cref="NodeListFilter"/> to a <seealso cref="NodeList"/>. This allows to remove all elements from a node list which do not match the Filter.
	  /// </summary>
	  /// <param name="nodeList"> the <seealso cref="NodeList"/> to filter </param>
	  /// <param name="filter"> the <seealso cref="NodeListFilter"/> to apply to the <seealso cref="NodeList"/> </param>
	  /// <returns> the List of all Nodes which match the filter </returns>
	  public static IList<DomElement> filterNodeList(NodeList nodeList, NodeListFilter filter)
	  {

		IList<DomElement> filteredList = new List<DomElement>();
		for (int i = 0; i < nodeList.Length; i++)
		{
		  Node node = nodeList.item(i);
		  if (filter.matches(node))
		  {
			filteredList.Add(new DomElementImpl((Element) node));
		  }
		}

		return filteredList;

	  }

	  /// <summary>
	  /// Filters a <seealso cref="NodeList"/> retaining all elements
	  /// </summary>
	  /// <param name="nodeList">  the the <seealso cref="NodeList"/> to filter </param>
	  /// <returns> the list of all elements </returns>
	  public static IList<DomElement> filterNodeListForElements(NodeList nodeList)
	  {
		return filterNodeList(nodeList, new ElementNodeListFilter());
	  }

	  /// <summary>
	  /// Filter a <seealso cref="NodeList"/> retaining all elements with a specific name
	  /// 
	  /// </summary>
	  /// <param name="nodeList"> the <seealso cref="NodeList"/> to filter </param>
	  /// <param name="namespaceUri"> the namespace for the elements </param>
	  /// <param name="localName"> the local element name to filter for </param>
	  /// <returns> the List of all Elements which match the filter </returns>
	  public static IList<DomElement> filterNodeListByName(NodeList nodeList, string namespaceUri, string localName)
	  {
		return filterNodeList(nodeList, new ElementByNameListFilter(localName, namespaceUri));
	  }

	  /// <summary>
	  /// Filter a <seealso cref="NodeList"/> retaining all elements with a specific type
	  /// 
	  /// </summary>
	  /// <param name="nodeList">  the <seealso cref="NodeList"/> to filter </param>
	  /// <param name="modelInstance">  the model instance </param>
	  /// <param name="type">  the type class to filter for </param>
	  /// <returns> the list of all Elements which match the filter </returns>
	  public static IList<DomElement> filterNodeListByType(NodeList nodeList, ModelInstanceImpl modelInstance, Type type)
	  {
		return filterNodeList(nodeList, new ElementByTypeListFilter(type, modelInstance));
	  }

	  public class DomErrorHandler : ErrorHandler
	  {

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		internal static readonly Logger LOGGER = Logger.getLogger(typeof(DomErrorHandler).FullName);

		internal virtual string getParseExceptionInfo(SAXParseException spe)
		{
		  return "URI=" + spe.SystemId + " Line="
			+ spe.LineNumber + ": " + spe.Message;
		}

		public virtual void warning(SAXParseException spe)
		{
		  LOGGER.warning(getParseExceptionInfo(spe));
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void error(org.xml.sax.SAXParseException spe) throws org.xml.sax.SAXException
		public virtual void error(SAXParseException spe)
		{
		  string message = "Error: " + getParseExceptionInfo(spe);
		  throw new SAXException(message);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void fatalError(org.xml.sax.SAXParseException spe) throws org.xml.sax.SAXException
		public virtual void fatalError(SAXParseException spe)
		{
		  string message = "Fatal Error: " + getParseExceptionInfo(spe);
		  throw new SAXException(message);
		}
	  }

	  /// <summary>
	  /// Get an empty DOM document
	  /// </summary>
	  /// <param name="documentBuilderFactory"> the factory to build to DOM document </param>
	  /// <returns> the new empty document </returns>
	  /// <exception cref="ModelParseException"> if unable to create a new document </exception>
	  public static DomDocument getEmptyDocument(DocumentBuilderFactory documentBuilderFactory)
	  {
		try
		{
		  DocumentBuilder documentBuilder = documentBuilderFactory.newDocumentBuilder();
		  return new DomDocumentImpl(documentBuilder.newDocument());
		}
		catch (ParserConfigurationException e)
		{
		  throw new ModelParseException("Unable to create a new document", e);
		}
	  }

	  /// <summary>
	  /// Create a new DOM document from the input stream
	  /// </summary>
	  /// <param name="documentBuilderFactory"> the factory to build to DOM document </param>
	  /// <param name="inputStream"> the input stream to parse </param>
	  /// <returns> the new DOM document </returns>
	  /// <exception cref="ModelParseException"> if a parsing or IO error is triggered </exception>
	  public static DomDocument parseInputStream(DocumentBuilderFactory documentBuilderFactory, Stream inputStream)
	  {

		try
		{
		  DocumentBuilder documentBuilder = documentBuilderFactory.newDocumentBuilder();
		  documentBuilder.ErrorHandler = new DomErrorHandler();
		  return new DomDocumentImpl(documentBuilder.parse(inputStream));
		}
		catch (ParserConfigurationException e)
		{
		  throw new ModelParseException("ParserConfigurationException while parsing input stream", e);

		}
		catch (SAXException e)
		{
		  throw new ModelParseException("SAXException while parsing input stream", e);

		}
		catch (IOException e)
		{
		  throw new ModelParseException("IOException while parsing input stream", e);

		}
	  }

	}

}