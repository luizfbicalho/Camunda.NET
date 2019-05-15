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
namespace org.camunda.bpm.container.impl.jboss.extension
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.jboss.@as.controller.descriptions.ModelDescriptionConstants.ADD;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.jboss.@as.controller.descriptions.ModelDescriptionConstants.OP;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.jboss.@as.controller.descriptions.ModelDescriptionConstants.OP_ADDR;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.jboss.@as.controller.descriptions.ModelDescriptionConstants.SUBSYSTEM;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.jboss.@as.controller.parsing.ParseUtils.missingRequired;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.jboss.@as.controller.parsing.ParseUtils.missingRequiredElement;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.jboss.@as.controller.parsing.ParseUtils.requireNoAttributes;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.jboss.@as.controller.parsing.ParseUtils.requireSingleAttribute;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.jboss.@as.controller.parsing.ParseUtils.unexpectedAttribute;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.jboss.@as.controller.parsing.ParseUtils.unexpectedElement;



	using ProcessEngineException = org.camunda.bpm.engine.ProcessEngineException;
	using AttributeDefinition = org.jboss.@as.controller.AttributeDefinition;
	using PathAddress = org.jboss.@as.controller.PathAddress;
	using PathElement = org.jboss.@as.controller.PathElement;
	using SimpleAttributeDefinition = org.jboss.@as.controller.SimpleAttributeDefinition;
	using ModelDescriptionConstants = org.jboss.@as.controller.descriptions.ModelDescriptionConstants;
	using ParseUtils = org.jboss.@as.controller.parsing.ParseUtils;
	using SubsystemMarshallingContext = org.jboss.@as.controller.persistence.SubsystemMarshallingContext;
	using ModelNode = org.jboss.dmr.ModelNode;
	using Property = org.jboss.dmr.Property;
	using XMLElementReader = org.jboss.staxmapper.XMLElementReader;
	using XMLElementWriter = org.jboss.staxmapper.XMLElementWriter;
	using XMLExtendedStreamReader = org.jboss.staxmapper.XMLExtendedStreamReader;
	using XMLExtendedStreamWriter = org.jboss.staxmapper.XMLExtendedStreamWriter;


	public class BpmPlatformParser : XMLStreamConstants, XMLElementReader<IList<ModelNode>>, XMLElementWriter<SubsystemMarshallingContext>
	{

	  public const bool REQUIRED = true;
	  public const bool NOT_REQUIRED = false;

	  /// <summary>
	  /// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void readElement(org.jboss.staxmapper.XMLExtendedStreamReader reader, java.util.List<org.jboss.dmr.ModelNode> list) throws javax.xml.stream.XMLStreamException
	  public override void readElement(XMLExtendedStreamReader reader, IList<ModelNode> list)
	  {
		// Require no attributes
		ParseUtils.requireNoAttributes(reader);

		//Add the main subsystem 'add' operation
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.jboss.dmr.ModelNode subsystemAddress = new org.jboss.dmr.ModelNode();
		ModelNode subsystemAddress = new ModelNode();
		subsystemAddress.add(SUBSYSTEM, ModelConstants_Fields.SUBSYSTEM_NAME);
		subsystemAddress.protect();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.jboss.dmr.ModelNode subsystemAdd = new org.jboss.dmr.ModelNode();
		ModelNode subsystemAdd = new ModelNode();
		subsystemAdd.get(OP).set(ADD);
		subsystemAdd.get(OP_ADDR).set(subsystemAddress);
		list.Add(subsystemAdd);

		while (reader.hasNext() && reader.nextTag() != END_ELEMENT)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Element element = Element.forName(reader.getLocalName());
		Element element = Element.forName(reader.LocalName);
		switch (element.innerEnumValue)
		{
			  case org.camunda.bpm.container.impl.jboss.extension.Element.InnerEnum.PROCESS_ENGINES:
			  {
				parseProcessEngines(reader, list, subsystemAddress);
				break;
			  }
			  case org.camunda.bpm.container.impl.jboss.extension.Element.InnerEnum.JOB_EXECUTOR:
			  {
				parseJobExecutor(reader, list, subsystemAddress);
				break;
			  }
			  default:
			  {
				throw unexpectedElement(reader);
			  }
		}
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void parseProcessEngines(org.jboss.staxmapper.XMLExtendedStreamReader reader, java.util.List<org.jboss.dmr.ModelNode> list, org.jboss.dmr.ModelNode parentAddress) throws javax.xml.stream.XMLStreamException
	  private void parseProcessEngines(XMLExtendedStreamReader reader, IList<ModelNode> list, ModelNode parentAddress)
	  {
		if (!Element.PROCESS_ENGINES.LocalName.Equals(reader.LocalName))
		{
		  throw unexpectedElement(reader);
		}

		IList<string> discoveredEngineNames = new List<string>();

		while (reader.hasNext() && reader.nextTag() != END_ELEMENT)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Element element = Element.forName(reader.getLocalName());
		  Element element = Element.forName(reader.LocalName);
		  switch (element.innerEnumValue)
		  {
			case org.camunda.bpm.container.impl.jboss.extension.Element.InnerEnum.PROCESS_ENGINE:
			{
			  parseProcessEngine(reader, list, parentAddress, discoveredEngineNames);
			  break;
			}
			default:
			{
			  throw unexpectedElement(reader);
			}
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void parseProcessEngine(org.jboss.staxmapper.XMLExtendedStreamReader reader, java.util.List<org.jboss.dmr.ModelNode> list, org.jboss.dmr.ModelNode parentAddress, java.util.List<String> discoveredEngineNames) throws javax.xml.stream.XMLStreamException
	  private void parseProcessEngine(XMLExtendedStreamReader reader, IList<ModelNode> list, ModelNode parentAddress, IList<string> discoveredEngineNames)
	  {
		if (!Element.PROCESS_ENGINE.LocalName.Equals(reader.LocalName))
		{
			throw unexpectedElement(reader);
		}

		ModelNode addProcessEngine = new ModelNode();
		string engineName = null;

		for (int i = 0; i < reader.AttributeCount; i++)
		{
		  string attr = reader.getAttributeLocalName(i);
		  if (Attribute.forName(attr).Equals(Attribute.NAME))
		  {
			engineName = reader.getAttributeValue(i).ToString();
		  }
		  else if (Attribute.forName(attr).Equals(Attribute.DEFAULT))
		  {
			SubsystemAttributeDefinitons.DEFAULT.parseAndSetParameter(reader.getAttributeValue(i), addProcessEngine, reader);
		  }
		  else
		  {
			throw unexpectedAttribute(reader, i);
		  }
		}

		if ("null".Equals(engineName))
		{
		  throw missingRequiredElement(reader, Collections.singleton(Attribute.NAME.LocalName));
		}

		//Add the 'add' operation for each 'process-engine' child
		addProcessEngine.get(OP).set(ModelDescriptionConstants.ADD);
		PathAddress addr = PathAddress.pathAddress(PathElement.pathElement(SUBSYSTEM, ModelConstants_Fields.SUBSYSTEM_NAME), PathElement.pathElement(Element.PROCESS_ENGINES.LocalName, engineName));
		addProcessEngine.get(OP_ADDR).set(addr.toModelNode());

		addProcessEngine.get(Attribute.NAME.LocalName).set(engineName);

		if (discoveredEngineNames.Contains(engineName))
		{
		  throw new ProcessEngineException("A process engine with name '" + engineName + "' already exists. The process engine name must be unique.");
		}
		else
		{
		  discoveredEngineNames.Add(engineName);
		}

		list.Add(addProcessEngine);

		// iterate deeper
		while (reader.hasNext() && reader.nextTag() != END_ELEMENT)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Element element = Element.forName(reader.getLocalName());
		  Element element = Element.forName(reader.LocalName);
		  switch (element.innerEnumValue)
		  {
			case org.camunda.bpm.container.impl.jboss.extension.Element.InnerEnum.PLUGINS:
			{
			  parsePlugins(reader, list, addProcessEngine);
			  break;
			}
			case org.camunda.bpm.container.impl.jboss.extension.Element.InnerEnum.PROPERTIES:
			{
			  parseProperties(reader, list, addProcessEngine);
			  break;
			}
			case org.camunda.bpm.container.impl.jboss.extension.Element.InnerEnum.DATASOURCE:
			{
			  parseElement(SubsystemAttributeDefinitons.DATASOURCE, reader, addProcessEngine);
			  break;
			}
			case org.camunda.bpm.container.impl.jboss.extension.Element.InnerEnum.HISTORY_LEVEL:
			{
			  parseElement(SubsystemAttributeDefinitons.HISTORY_LEVEL, reader, addProcessEngine);
			  break;
			}
			case org.camunda.bpm.container.impl.jboss.extension.Element.InnerEnum.CONFIGURATION:
			{
			  parseElement(SubsystemAttributeDefinitons.CONFIGURATION, reader, addProcessEngine);
			  break;
			}
			default:
			{
			  throw unexpectedElement(reader);
			}
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void parsePlugins(org.jboss.staxmapper.XMLExtendedStreamReader reader, java.util.List<org.jboss.dmr.ModelNode> list, org.jboss.dmr.ModelNode addProcessEngine) throws javax.xml.stream.XMLStreamException
	  private void parsePlugins(XMLExtendedStreamReader reader, IList<ModelNode> list, ModelNode addProcessEngine)
	  {
		if (!Element.PLUGINS.LocalName.Equals(reader.LocalName))
		{
		  throw unexpectedElement(reader);
		}

		requireNoAttributes(reader);

		ModelNode plugins = new ModelNode();

		while (reader.hasNext() && reader.nextTag() != END_ELEMENT)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Element element = Element.forName(reader.getLocalName());
		  Element element = Element.forName(reader.LocalName);
		  switch (element.innerEnumValue)
		  {
			case org.camunda.bpm.container.impl.jboss.extension.Element.InnerEnum.PLUGIN:
			{
			  parsePlugin(reader, list, plugins);
			  break;
			}
			default:
			{
			  throw unexpectedElement(reader);
			}
		  }
		}

		addProcessEngine.get(Element.PLUGINS.LocalName).set(plugins);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void parsePlugin(org.jboss.staxmapper.XMLExtendedStreamReader reader, java.util.List<org.jboss.dmr.ModelNode> list, org.jboss.dmr.ModelNode plugins) throws javax.xml.stream.XMLStreamException
	  private void parsePlugin(XMLExtendedStreamReader reader, IList<ModelNode> list, ModelNode plugins)
	  {
		if (!Element.PLUGIN.LocalName.Equals(reader.LocalName))
		{
		  throw unexpectedElement(reader);
		}

		requireNoAttributes(reader);
		ModelNode plugin = new ModelNode();

		while (reader.hasNext() && reader.nextTag() != END_ELEMENT)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Element element = Element.forName(reader.getLocalName());
		  Element element = Element.forName(reader.LocalName);
		  switch (element.innerEnumValue)
		  {
			case org.camunda.bpm.container.impl.jboss.extension.Element.InnerEnum.PLUGIN_CLASS:
			{
			  parseElement(SubsystemAttributeDefinitons.PLUGIN_CLASS, reader, plugin);
			  break;
			}
			case org.camunda.bpm.container.impl.jboss.extension.Element.InnerEnum.PROPERTIES:
			{
			  parseProperties(reader, list, plugin);
			  break;
			}
			default:
			{
			  throw unexpectedElement(reader);
			}
		  }
		}

		plugins.add(plugin);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void parseProperties(org.jboss.staxmapper.XMLExtendedStreamReader reader, java.util.List<org.jboss.dmr.ModelNode> list, org.jboss.dmr.ModelNode parentAddress) throws javax.xml.stream.XMLStreamException
	  private void parseProperties(XMLExtendedStreamReader reader, IList<ModelNode> list, ModelNode parentAddress)
	  {
		if (!Element.PROPERTIES.LocalName.Equals(reader.LocalName))
		{
		  throw unexpectedElement(reader);
		}

		requireNoAttributes(reader);

		while (reader.hasNext() && reader.nextTag() != END_ELEMENT)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Element element = Element.forName(reader.getLocalName());
		  Element element = Element.forName(reader.LocalName);
		  switch (element.innerEnumValue)
		  {
			case org.camunda.bpm.container.impl.jboss.extension.Element.InnerEnum.PROPERTY:
			{
			  parseProperty(reader, list, parentAddress);
			  break;
			}
			default:
			{
			  throw unexpectedElement(reader);
			}
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void parseProperty(org.jboss.staxmapper.XMLExtendedStreamReader reader, java.util.List<org.jboss.dmr.ModelNode> list, org.jboss.dmr.ModelNode parentAddress) throws javax.xml.stream.XMLStreamException
	  private void parseProperty(XMLExtendedStreamReader reader, IList<ModelNode> list, ModelNode parentAddress)
	  {
		requireSingleAttribute(reader, Attribute.NAME.LocalName);
		string name = reader.getAttributeValue(0);
		string value = rawElementText(reader);

		if (string.ReferenceEquals(name, null))
		{
		  throw missingRequired(reader, Collections.singleton(Attribute.NAME));
		}

		SubsystemAttributeDefinitons.PROPERTIES.parseAndAddParameterElement(name, value, parentAddress, reader);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void parseJobExecutor(org.jboss.staxmapper.XMLExtendedStreamReader reader, java.util.List<org.jboss.dmr.ModelNode> list, org.jboss.dmr.ModelNode parentAddress) throws javax.xml.stream.XMLStreamException
	  private void parseJobExecutor(XMLExtendedStreamReader reader, IList<ModelNode> list, ModelNode parentAddress)
	  {
		if (!Element.JOB_EXECUTOR.LocalName.Equals(reader.LocalName))
		{
		  throw unexpectedElement(reader);
		}

		// Add the 'add' operation for 'job-executor' parent
		ModelNode addJobExecutor = new ModelNode();
		addJobExecutor.get(OP).set(ModelDescriptionConstants.ADD);
		PathAddress addr = PathAddress.pathAddress(PathElement.pathElement(SUBSYSTEM, ModelConstants_Fields.SUBSYSTEM_NAME), PathElement.pathElement(Element.JOB_EXECUTOR.LocalName, ModelConstants_Fields.DEFAULT));
		addJobExecutor.get(OP_ADDR).set(addr.toModelNode());

		list.Add(addJobExecutor);

		// iterate deeper
		while (reader.hasNext())
		{
		  switch (reader.nextTag())
		  {
		  case END_ELEMENT:
		  {
			if (Element.forName(reader.LocalName) == Element.JOB_EXECUTOR)
			{
			  // should mean we're done, so ignore it.
			  return;
			}
		  }
			  goto case START_ELEMENT;
		  case START_ELEMENT:
		  {
			switch (Element.forName(reader.LocalName))
			{
			  case JOB_AQUISITIONS:
			  {
				parseJobAcquisitions(reader, list, addJobExecutor);
				break;
			  }
			  case THREAD_POOL_NAME:
			  {
				parseElement(SubsystemAttributeDefinitons.THREAD_POOL_NAME, reader, addJobExecutor);
				break;
			  }
			  default:
			  {
				throw unexpectedElement(reader);
			  }
			}
			break;
		  }
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void parseJobAcquisitions(org.jboss.staxmapper.XMLExtendedStreamReader reader, java.util.List<org.jboss.dmr.ModelNode> list, org.jboss.dmr.ModelNode parentAddress) throws javax.xml.stream.XMLStreamException
	  private void parseJobAcquisitions(XMLExtendedStreamReader reader, IList<ModelNode> list, ModelNode parentAddress)
	  {
		if (!Element.JOB_AQUISITIONS.LocalName.Equals(reader.LocalName))
		{
		  throw unexpectedElement(reader);
		}

		while (reader.hasNext())
		{
		  switch (reader.nextTag())
		  {
			case END_ELEMENT:
			{
			  if (Element.forName(reader.LocalName) == Element.JOB_AQUISITIONS)
			  {
				// should mean we're done, so ignore it.
				return;
			  }
			}
				goto case START_ELEMENT;
			case START_ELEMENT:
			{
			  switch (Element.forName(reader.LocalName))
			  {
				case JOB_AQUISITION:
				{
				  parseJobAcquisition(reader, list, parentAddress);
				  break;
				}
				default:
				{
				  throw unexpectedElement(reader);
				}
			  }
			  break;
			}
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void parseJobAcquisition(org.jboss.staxmapper.XMLExtendedStreamReader reader, java.util.List<org.jboss.dmr.ModelNode> list, org.jboss.dmr.ModelNode parentAddress) throws javax.xml.stream.XMLStreamException
	  private void parseJobAcquisition(XMLExtendedStreamReader reader, IList<ModelNode> list, ModelNode parentAddress)
	  {
		if (!Element.JOB_AQUISITION.LocalName.Equals(reader.LocalName))
		{
		  throw unexpectedElement(reader);
		}

		string acquisitionName = null;

		for (int i = 0; i < reader.AttributeCount; i++)
		{
		  string attr = reader.getAttributeLocalName(i);
		  if (Attribute.forName(attr).Equals(Attribute.NAME))
		  {
			acquisitionName = reader.getAttributeValue(i).ToString();
		  }
		  else
		  {
			throw unexpectedAttribute(reader, i);
		  }
		}

		if ("null".Equals(acquisitionName))
		{
		  throw missingRequiredElement(reader, Collections.singleton(Attribute.NAME.LocalName));
		}

		// Add the 'add' operation for each 'job-acquisition' child
		ModelNode addJobAcquisition = new ModelNode();
		addJobAcquisition.get(OP).set(ADD);
		PathAddress addr = PathAddress.pathAddress(PathElement.pathElement(SUBSYSTEM, ModelConstants_Fields.SUBSYSTEM_NAME), PathElement.pathElement(Element.JOB_EXECUTOR.LocalName, ModelConstants_Fields.DEFAULT), PathElement.pathElement(Element.JOB_AQUISITIONS.LocalName, acquisitionName));
		addJobAcquisition.get(OP_ADDR).set(addr.toModelNode());

		addJobAcquisition.get(Attribute.NAME.LocalName).set(acquisitionName);

		list.Add(addJobAcquisition);

		// iterate deeper
		while (reader.hasNext())
		{
		  switch (reader.nextTag())
		  {
			case END_ELEMENT:
			{
			  if (Element.forName(reader.LocalName) == Element.JOB_AQUISITION)
			  {
				// should mean we're done, so ignore it.
				return;
			  }
			}
				goto case START_ELEMENT;
			case START_ELEMENT:
			{
			  switch (Element.forName(reader.LocalName))
			  {
				case PROPERTIES:
				{
				  parseProperties(reader, list, addJobAcquisition);
				  break;
				}
				case ACQUISITION_STRATEGY:
				{
				  parseElement(SubsystemAttributeDefinitons.ACQUISITION_STRATEGY, reader, addJobAcquisition);
				  break;
				}
				default:
				{
				  throw unexpectedElement(reader);
				}
			  }
			  break;
			}
		  }
		}
	  }
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void parseElement(org.jboss.as.controller.AttributeDefinition element, org.jboss.staxmapper.XMLExtendedStreamReader reader, org.jboss.dmr.ModelNode parentAddress) throws javax.xml.stream.XMLStreamException
	  private void parseElement(AttributeDefinition element, XMLExtendedStreamReader reader, ModelNode parentAddress)
	  {
		string value = rawElementText(reader);
		((SimpleAttributeDefinition) element).parseAndSetParameter(value, parentAddress, reader);
	  }

	  /// <summary>
	  /// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void writeContent(org.jboss.staxmapper.XMLExtendedStreamWriter writer, org.jboss.as.controller.persistence.SubsystemMarshallingContext context) throws javax.xml.stream.XMLStreamException
	  public override void writeContent(XMLExtendedStreamWriter writer, SubsystemMarshallingContext context)
	  {

		context.startSubsystemElement(Namespace.CURRENT.UriString, false);

		writeProcessEnginesContent(writer, context);

		writeJobExecutorContent(writer, context);

		// end subsystem
		writer.writeEndElement();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void writeProcessEnginesContent(final org.jboss.staxmapper.XMLExtendedStreamWriter writer, final org.jboss.as.controller.persistence.SubsystemMarshallingContext context) throws javax.xml.stream.XMLStreamException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  protected internal virtual void writeProcessEnginesContent(XMLExtendedStreamWriter writer, SubsystemMarshallingContext context)
	  {

		writer.writeStartElement(Element.PROCESS_ENGINES.LocalName);

		ModelNode node = context.ModelNode;

		ModelNode processEngineConfigurations = node.get(Element.PROCESS_ENGINES.LocalName);
		if (processEngineConfigurations.Defined)
		{
		  foreach (Property property in processEngineConfigurations.asPropertyList())
		  {
			// write each child element to xml
			writer.writeStartElement(Element.PROCESS_ENGINE.LocalName);

			ModelNode propertyValue = property.Value;
			foreach (AttributeDefinition processEngineAttribute in SubsystemAttributeDefinitons.PROCESS_ENGINE_ATTRIBUTES)
			{
			  if (processEngineAttribute.Equals(SubsystemAttributeDefinitons.NAME) || processEngineAttribute.Equals(SubsystemAttributeDefinitons.DEFAULT))
			  {
				((SimpleAttributeDefinition) processEngineAttribute).marshallAsAttribute(propertyValue, writer);
			  }
			  else
			  {
				processEngineAttribute.marshallAsElement(propertyValue, writer);
			  }
			}


			writer.writeEndElement();
		  }
		}
		// end process-engines
		writer.writeEndElement();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void writeJobExecutorContent(final org.jboss.staxmapper.XMLExtendedStreamWriter writer, final org.jboss.as.controller.persistence.SubsystemMarshallingContext context) throws javax.xml.stream.XMLStreamException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  protected internal virtual void writeJobExecutorContent(XMLExtendedStreamWriter writer, SubsystemMarshallingContext context)
	  {
		ModelNode node = context.ModelNode;
		ModelNode jobExecutorNode = node.get(Element.JOB_EXECUTOR.LocalName);

		if (jobExecutorNode.Defined)
		{
		  writer.writeStartElement(Element.JOB_EXECUTOR.LocalName);

		  foreach (Property property in jobExecutorNode.asPropertyList())
		  {
			ModelNode propertyValue = property.Value;

			foreach (AttributeDefinition jobExecutorAttribute in SubsystemAttributeDefinitons.JOB_EXECUTOR_ATTRIBUTES)
			{
			  if (jobExecutorAttribute.Equals(SubsystemAttributeDefinitons.NAME))
			  {
				((SimpleAttributeDefinition) jobExecutorAttribute).marshallAsAttribute(propertyValue, writer);
			  }
			  else
			  {
				jobExecutorAttribute.marshallAsElement(propertyValue, writer);
			  }
			}

			writeJobAcquisitionsContent(writer, context, propertyValue);
		  }

		  // end job-executor
		  writer.writeEndElement();
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void writeJobAcquisitionsContent(final org.jboss.staxmapper.XMLExtendedStreamWriter writer, final org.jboss.as.controller.persistence.SubsystemMarshallingContext context, org.jboss.dmr.ModelNode parentNode) throws javax.xml.stream.XMLStreamException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  protected internal virtual void writeJobAcquisitionsContent(XMLExtendedStreamWriter writer, SubsystemMarshallingContext context, ModelNode parentNode)
	  {
		writer.writeStartElement(Element.JOB_AQUISITIONS.LocalName);

		ModelNode jobAcquisitionConfigurations = parentNode.get(Element.JOB_AQUISITIONS.LocalName);
		if (jobAcquisitionConfigurations.Defined)
		{
		  foreach (Property property in jobAcquisitionConfigurations.asPropertyList())
		  {
			// write each child element to xml
			writer.writeStartElement(Element.JOB_AQUISITION.LocalName);

			foreach (AttributeDefinition jobAcquisitionAttribute in SubsystemAttributeDefinitons.JOB_ACQUISITION_ATTRIBUTES)
			{
			  if (jobAcquisitionAttribute.Equals(SubsystemAttributeDefinitons.NAME))
			  {
				((SimpleAttributeDefinition) jobAcquisitionAttribute).marshallAsAttribute(property.Value, writer);
			  }
			  else
			  {
				jobAcquisitionAttribute.marshallAsElement(property.Value, writer);
			  }
			}

			writer.writeEndElement();
		  }
		}
		// end job-acquisitions
		writer.writeEndElement();
	  }

	  /// <summary>
	  /// Reads and trims the element text and returns it or {@code null}
	  /// </summary>
	  /// <param name="reader">  source for the element text </param>
	  /// <returns> the string representing the trimmed element text or {@code null} if there is none or it is an empty string </returns>
	  /// <exception cref="XMLStreamException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String rawElementText(javax.xml.stream.XMLStreamReader reader) throws javax.xml.stream.XMLStreamException
	  public virtual string rawElementText(XMLStreamReader reader)
	  {
		string elementText = reader.ElementText;
		elementText = string.ReferenceEquals(elementText, null) || elementText.Trim().Length == 0 ? null : elementText.Trim();
		return elementText;
	  }

	}

}