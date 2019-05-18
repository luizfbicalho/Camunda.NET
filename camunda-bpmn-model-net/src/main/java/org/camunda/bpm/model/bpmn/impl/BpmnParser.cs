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
namespace org.camunda.bpm.model.bpmn.impl
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN20_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_20_SCHEMA_LOCATION;

	using ModelImpl = org.camunda.bpm.model.xml.impl.ModelImpl;
	using AbstractModelParser = org.camunda.bpm.model.xml.impl.parser.AbstractModelParser;
	using ReflectUtil = org.camunda.bpm.model.xml.impl.util.ReflectUtil;
	using DomDocument = org.camunda.bpm.model.xml.instance.DomDocument;


	/// <summary>
	/// <para>The parser used when parsing BPMN Files</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class BpmnParser : AbstractModelParser
	{

	 private const string JAXP_SCHEMA_SOURCE = "http://java.sun.com/xml/jaxp/properties/schemaSource";
	 private const string JAXP_SCHEMA_LANGUAGE = "http://java.sun.com/xml/jaxp/properties/schemaLanguage";

	  private const string W3C_XML_SCHEMA = "http://www.w3.org/2001/XMLSchema";

	  public BpmnParser()
	  {
		this.schemaFactory = SchemaFactory.newInstance(W3C_XML_SCHEMA);
		addSchema(BPMN20_NS, createSchema(BPMN_20_SCHEMA_LOCATION, typeof(BpmnParser).ClassLoader));
	  }

	  protected internal override void configureFactory(DocumentBuilderFactory dbf)
	  {
		dbf.setAttribute(JAXP_SCHEMA_LANGUAGE, W3C_XML_SCHEMA);
		dbf.setAttribute(JAXP_SCHEMA_SOURCE, ReflectUtil.getResource(BPMN_20_SCHEMA_LOCATION, typeof(BpmnParser).ClassLoader).ToString());
		base.configureFactory(dbf);
	  }

	  protected internal override BpmnModelInstanceImpl createModelInstance(DomDocument document)
	  {
		return new BpmnModelInstanceImpl((ModelImpl) Bpmn.INSTANCE.BpmnModel, Bpmn.INSTANCE.BpmnModelBuilder, document);
	  }

	  public override BpmnModelInstanceImpl parseModelFromStream(Stream inputStream)
	  {
		return (BpmnModelInstanceImpl) base.parseModelFromStream(inputStream);
	  }

	  public override BpmnModelInstanceImpl EmptyModel
	  {
		  get
		  {
			return (BpmnModelInstanceImpl) base.EmptyModel;
		  }
	  }

	}

}