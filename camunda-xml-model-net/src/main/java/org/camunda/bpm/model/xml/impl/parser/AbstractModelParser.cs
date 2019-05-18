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
namespace org.camunda.bpm.model.xml.impl.parser
{
	using DomUtil = org.camunda.bpm.model.xml.impl.util.DomUtil;
	using ReflectUtil = org.camunda.bpm.model.xml.impl.util.ReflectUtil;
	using DomDocument = org.camunda.bpm.model.xml.instance.DomDocument;
	using DomElement = org.camunda.bpm.model.xml.instance.DomElement;
	using SAXException = org.xml.sax.SAXException;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public abstract class AbstractModelParser
	{

	  private readonly DocumentBuilderFactory documentBuilderFactory;
	  protected internal SchemaFactory schemaFactory;
	  protected internal IDictionary<string, Schema> schemas = new Dictionary<string, Schema>();

	  protected internal AbstractModelParser()
	  {
		DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
		configureFactory(dbf);
		this.documentBuilderFactory = dbf;
	  }

	  /// <summary>
	  /// allows subclasses to configure the <seealso cref="DocumentBuilderFactory"/>. </summary>
	  /// <param name="dbf"> the factory to configure </param>
	  protected internal virtual void configureFactory(DocumentBuilderFactory dbf)
	  {
		dbf.Validating = true;
		dbf.IgnoringComments = false;
		dbf.IgnoringElementContentWhitespace = false;
		dbf.NamespaceAware = true;
		protectAgainstXxeAttacks(dbf);
	  }

	  /// <summary>
	  /// Configures the DocumentBuilderFactory in a way, that it is protected against XML External Entity Attacks.
	  /// If the implementing parser does not support one or multiple features, the failed feature is ignored.
	  /// The parser might not protected, if the feature assignment fails.
	  /// </summary>
	  /// <seealso cref= <a href="https://www.owasp.org/index.php/XML_External_Entity_(XXE)_Prevention_Cheat_Sheet">OWASP Information of XXE attacks</a>
	  /// </seealso>
	  /// <param name="dbf"> The factory to configure. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void protectAgainstXxeAttacks(final javax.xml.parsers.DocumentBuilderFactory dbf)
	  private void protectAgainstXxeAttacks(DocumentBuilderFactory dbf)
	  {
		try
		{
		  dbf.setFeature("http://xml.org/sax/features/external-general-entities", false);
		}
		catch (ParserConfigurationException)
		{
		}

		try
		{
		  dbf.setFeature("http://apache.org/xml/features/disallow-doctype-decl", true);
		}
		catch (ParserConfigurationException)
		{
		}

		try
		{
		  dbf.setFeature("http://xml.org/sax/features/external-parameter-entities", false);
		}
		catch (ParserConfigurationException)
		{
		}

		dbf.XIncludeAware = false;
		dbf.ExpandEntityReferences = false;
	  }

	  public virtual ModelInstance parseModelFromStream(Stream inputStream)
	  {
		DomDocument document = null;

		lock (documentBuilderFactory)
		{
		  document = DomUtil.parseInputStream(documentBuilderFactory, inputStream);
		}

		validateModel(document);
		return createModelInstance(document);

	  }

	  public virtual ModelInstance EmptyModel
	  {
		  get
		  {
			DomDocument document = null;
    
			lock (documentBuilderFactory)
			{
			  document = DomUtil.getEmptyDocument(documentBuilderFactory);
			}
    
			return createModelInstance(document);
		  }
	  }

	  /// <summary>
	  /// Validate DOM document
	  /// </summary>
	  /// <param name="document"> the DOM document to validate </param>
	  public virtual void validateModel(DomDocument document)
	  {

		Schema schema = getSchema(document);

		if (schema == null)
		{
		  return;
		}

		Validator validator = schema.newValidator();
		try
		{
		  lock (document)
		  {
			validator.validate(document.DomSource);
		  }
		}
		catch (IOException e)
		{
		  throw new ModelValidationException("Error during DOM document validation", e);
		}
		catch (SAXException e)
		{
		  throw new ModelValidationException("DOM document is not valid", e);
		}
	  }

	  protected internal virtual Schema getSchema(DomDocument document)
	  {
		DomElement rootElement = document.RootElement;
		string namespaceURI = rootElement.NamespaceURI;
		return schemas[namespaceURI];
	  }

	  protected internal virtual void addSchema(string namespaceURI, Schema schema)
	  {
		schemas[namespaceURI] = schema;
	  }

	  protected internal virtual Schema createSchema(string location, ClassLoader classLoader)
	  {
		URL cmmnSchema = ReflectUtil.getResource(location, classLoader);
		try
		{
		  return schemaFactory.newSchema(cmmnSchema);
		}
		catch (SAXException)
		{
		  throw new ModelValidationException("Unable to parse schema:" + cmmnSchema);
		}
	  }

	  protected internal abstract ModelInstance createModelInstance(DomDocument document);

	}

}