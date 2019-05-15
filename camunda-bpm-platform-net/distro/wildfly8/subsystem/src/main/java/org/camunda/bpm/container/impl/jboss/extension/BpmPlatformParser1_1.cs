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
namespace org.camunda.bpm.container.impl.jboss.extension
{
	using ProcessEngineException = org.camunda.bpm.engine.ProcessEngineException;
	using AbstractParser = org.jboss.@as.connector.util.AbstractParser;
	using ParserException = org.jboss.@as.connector.util.ParserException;
	using AttributeDefinition = org.jboss.@as.controller.AttributeDefinition;
	using SimpleAttributeDefinition = org.jboss.@as.controller.SimpleAttributeDefinition;
	using ParseUtils = org.jboss.@as.controller.parsing.ParseUtils;
	using SubsystemMarshallingContext = org.jboss.@as.controller.persistence.SubsystemMarshallingContext;
	using ModelNode = org.jboss.dmr.ModelNode;
	using Property = org.jboss.dmr.Property;
	using XMLElementReader = org.jboss.staxmapper.XMLElementReader;
	using XMLElementWriter = org.jboss.staxmapper.XMLElementWriter;
	using XMLExtendedStreamReader = org.jboss.staxmapper.XMLExtendedStreamReader;
	using XMLExtendedStreamWriter = org.jboss.staxmapper.XMLExtendedStreamWriter;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.jboss.extension.Attribute.DEFAULT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.jboss.extension.Attribute.NAME;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.jboss.@as.controller.descriptions.ModelDescriptionConstants.*;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.jboss.@as.controller.parsing.ParseUtils.*;

	public class BpmPlatformParser1_1 : AbstractParser
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void parse(final org.jboss.staxmapper.XMLExtendedStreamReader reader, final java.util.List<org.jboss.dmr.ModelNode> operations, org.jboss.dmr.ModelNode subsystemAddress) throws Exception
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual void parse(XMLExtendedStreamReader reader, IList<ModelNode> operations, ModelNode subsystemAddress)
	  {
		while (reader.hasNext() && reader.nextTag() != END_ELEMENT)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Element element = Element.forName(reader.getLocalName());
		  Element element = Element.forName(reader.LocalName);
		  switch (element.innerEnumValue)
		  {
			case org.camunda.bpm.container.impl.jboss.extension.Element.InnerEnum.PROCESS_ENGINES:
			{
			  parseProcessEngines(reader, operations, subsystemAddress);
			  break;
			}
			case org.camunda.bpm.container.impl.jboss.extension.Element.InnerEnum.JOB_EXECUTOR:
			{
			  parseJobExecutor(reader, operations, subsystemAddress);
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
//ORIGINAL LINE: protected void parseProcessEngines(final org.jboss.staxmapper.XMLExtendedStreamReader reader, final java.util.List<org.jboss.dmr.ModelNode> operations, final org.jboss.dmr.ModelNode parentAddress) throws javax.xml.stream.XMLStreamException, org.jboss.as.connector.util.ParserException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  protected internal virtual void parseProcessEngines(XMLExtendedStreamReader reader, IList<ModelNode> operations, ModelNode parentAddress)
	  {
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
			  parseProcessEngine(reader, operations, parentAddress, discoveredEngineNames);
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
//ORIGINAL LINE: protected void parseProcessEngine(org.jboss.staxmapper.XMLExtendedStreamReader reader, java.util.List<org.jboss.dmr.ModelNode> operations, org.jboss.dmr.ModelNode parentAddress, java.util.List<String> discoveredEngineNames) throws javax.xml.stream.XMLStreamException, org.jboss.as.connector.util.ParserException
	  protected internal virtual void parseProcessEngine(XMLExtendedStreamReader reader, IList<ModelNode> operations, ModelNode parentAddress, IList<string> discoveredEngineNames)
	  {
		string engineName = null;

		//Add the 'add' operation for each 'process-engine' child
		ModelNode addProcessEngineOp = new ModelNode();
		addProcessEngineOp.get(OP).set(ADD);

		for (int i = 0; i < reader.AttributeCount; i++)
		{
		  Attribute attribute = Attribute.forName(reader.getAttributeLocalName(i));
		  switch (attribute.innerEnumValue)
		  {
			case org.camunda.bpm.container.impl.jboss.extension.Attribute.InnerEnum.NAME:
			{
			  engineName = rawAttributeText(reader, NAME.LocalName);
			  if (!string.ReferenceEquals(engineName, null) && !engineName.Equals("null"))
			  {
				SubsystemAttributeDefinitons.NAME.parseAndSetParameter(engineName, addProcessEngineOp, reader);
			  }
			  else
			  {
				throw missingRequiredElement(reader, Collections.singleton(NAME.LocalName));
			  }
			  break;
			}
			case org.camunda.bpm.container.impl.jboss.extension.Attribute.InnerEnum.DEFAULT:
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String value = rawAttributeText(reader, DEFAULT.getLocalName());
			  string value = rawAttributeText(reader, DEFAULT.LocalName);
			  if (!string.ReferenceEquals(value, null))
			  {
				SubsystemAttributeDefinitons.DEFAULT.parseAndSetParameter(value, addProcessEngineOp, reader);
			  }
			  break;
			}
			default:
			  throw unexpectedAttribute(reader, i);
		  }
		}

		ModelNode processEngineAddress = parentAddress.clone();
		processEngineAddress.add(ModelConstants_Fields.PROCESS_ENGINES, engineName);
		addProcessEngineOp.get(OP_ADDR).set(processEngineAddress);

		if (discoveredEngineNames.Contains(engineName))
		{
		  throw new ProcessEngineException("A process engine with name '" + engineName + "' already exists. The process engine name must be unique.");
		}
		else
		{
		  discoveredEngineNames.Add(engineName);
		}

		operations.Add(addProcessEngineOp);

		parseAdditionalProcessEngineSettings(reader, operations, addProcessEngineOp);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void parseAdditionalProcessEngineSettings(org.jboss.staxmapper.XMLExtendedStreamReader reader, java.util.List<org.jboss.dmr.ModelNode> operations, org.jboss.dmr.ModelNode addProcessEngineOp) throws javax.xml.stream.XMLStreamException
	  protected internal virtual void parseAdditionalProcessEngineSettings(XMLExtendedStreamReader reader, IList<ModelNode> operations, ModelNode addProcessEngineOp)
	  {
		// iterate deeper
		while (reader.hasNext() && reader.nextTag() != END_ELEMENT)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Element element = Element.forName(reader.getLocalName());
		  Element element = Element.forName(reader.LocalName);
		  switch (element.innerEnumValue)
		  {
			case org.camunda.bpm.container.impl.jboss.extension.Element.InnerEnum.DATASOURCE:
			{
			  parseElement(SubsystemAttributeDefinitons.DATASOURCE, addProcessEngineOp, reader);
			  break;
			}
			case org.camunda.bpm.container.impl.jboss.extension.Element.InnerEnum.HISTORY_LEVEL:
			{
			  parseElement(SubsystemAttributeDefinitons.HISTORY_LEVEL, addProcessEngineOp, reader);
			  break;
			}
			case org.camunda.bpm.container.impl.jboss.extension.Element.InnerEnum.CONFIGURATION:
			{
			  parseElement(SubsystemAttributeDefinitons.CONFIGURATION, addProcessEngineOp, reader);
			  break;
			}
			case org.camunda.bpm.container.impl.jboss.extension.Element.InnerEnum.PROPERTIES:
			{
			  parseProperties(reader, operations, addProcessEngineOp);
			  break;
			}
			case org.camunda.bpm.container.impl.jboss.extension.Element.InnerEnum.PLUGINS:
			{
			  parsePlugins(reader, operations, addProcessEngineOp);
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
//ORIGINAL LINE: protected void parsePlugins(org.jboss.staxmapper.XMLExtendedStreamReader reader, java.util.List<org.jboss.dmr.ModelNode> operations, org.jboss.dmr.ModelNode addProcessEngine) throws javax.xml.stream.XMLStreamException
	  protected internal virtual void parsePlugins(XMLExtendedStreamReader reader, IList<ModelNode> operations, ModelNode addProcessEngine)
	  {
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
			  parsePlugin(reader, operations, plugins);
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
//ORIGINAL LINE: protected void parsePlugin(org.jboss.staxmapper.XMLExtendedStreamReader reader, java.util.List<org.jboss.dmr.ModelNode> operations, org.jboss.dmr.ModelNode plugins) throws javax.xml.stream.XMLStreamException
	  protected internal virtual void parsePlugin(XMLExtendedStreamReader reader, IList<ModelNode> operations, ModelNode plugins)
	  {
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
			  parseElement(SubsystemAttributeDefinitons.PLUGIN_CLASS, plugin, reader);
			  break;
			}
			case org.camunda.bpm.container.impl.jboss.extension.Element.InnerEnum.PROPERTIES:
			{
			  parseProperties(reader, operations, plugin);
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
//ORIGINAL LINE: protected void parseProperties(org.jboss.staxmapper.XMLExtendedStreamReader reader, java.util.List<org.jboss.dmr.ModelNode> operations, org.jboss.dmr.ModelNode parentAddress) throws javax.xml.stream.XMLStreamException
	  protected internal virtual void parseProperties(XMLExtendedStreamReader reader, IList<ModelNode> operations, ModelNode parentAddress)
	  {
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
			  string name = reader.getAttributeValue(0);
			  string value = rawElementText(reader);

			  SubsystemAttributeDefinitons.PROPERTIES.parseAndAddParameterElement(name, value, parentAddress, reader);
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
//ORIGINAL LINE: protected void parseJobExecutor(org.jboss.staxmapper.XMLExtendedStreamReader reader, java.util.List<org.jboss.dmr.ModelNode> operations, org.jboss.dmr.ModelNode parentAddress) throws javax.xml.stream.XMLStreamException
	  protected internal virtual void parseJobExecutor(XMLExtendedStreamReader reader, IList<ModelNode> operations, ModelNode parentAddress)
	  {
		// Add the 'add' operation for 'job-executor' parent
		ModelNode addJobExecutorOp = new ModelNode();
		addJobExecutorOp.get(OP).set(ADD);
		ModelNode jobExecutorAddress = parentAddress.clone();
		jobExecutorAddress.add(ModelConstants_Fields.JOB_EXECUTOR, ModelConstants_Fields.DEFAULT);
		addJobExecutorOp.get(OP_ADDR).set(jobExecutorAddress);

		operations.Add(addJobExecutorOp);

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
				  parseJobAcquisitions(reader, operations, addJobExecutorOp);
				  break;
				}
				case THREAD_POOL_NAME:
				{
				  parseElement(SubsystemAttributeDefinitons.THREAD_POOL_NAME, addJobExecutorOp, reader);
				  break;
				}
				case CORE_THREADS:
				{
				  parseElement(SubsystemAttributeDefinitons.CORE_THREADS, addJobExecutorOp, reader);
				  break;
				}
				case MAX_THREADS:
				{
				  parseElement(SubsystemAttributeDefinitons.MAX_THREADS, addJobExecutorOp, reader);
				  break;
				}
				case QUEUE_LENGTH:
				{
				  parseElement(SubsystemAttributeDefinitons.QUEUE_LENGTH, addJobExecutorOp, reader);
				  break;
				}
				case KEEPALIVE_TIME:
				{
				  parseElement(SubsystemAttributeDefinitons.KEEPALIVE_TIME, addJobExecutorOp, reader);
				  break;
				}
				case ALLOW_CORE_TIMEOUT:
				{
				  parseElement(SubsystemAttributeDefinitons.ALLOW_CORE_TIMEOUT, addJobExecutorOp, reader);
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
//ORIGINAL LINE: protected void parseJobAcquisitions(org.jboss.staxmapper.XMLExtendedStreamReader reader, java.util.List<org.jboss.dmr.ModelNode> operation, org.jboss.dmr.ModelNode parentAddress) throws javax.xml.stream.XMLStreamException
	  protected internal virtual void parseJobAcquisitions(XMLExtendedStreamReader reader, IList<ModelNode> operation, ModelNode parentAddress)
	  {
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
				  parseJobAcquisition(reader, operation, parentAddress);
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
//ORIGINAL LINE: protected void parseJobAcquisition(org.jboss.staxmapper.XMLExtendedStreamReader reader, java.util.List<org.jboss.dmr.ModelNode> operations, org.jboss.dmr.ModelNode parentAddress) throws javax.xml.stream.XMLStreamException
	  protected internal virtual void parseJobAcquisition(XMLExtendedStreamReader reader, IList<ModelNode> operations, ModelNode parentAddress)
	  {
		string acquisitionName = null;

		// Add the 'add' operation for each 'job-acquisition' child
		ModelNode addJobAcquisitionOp = new ModelNode();
		addJobAcquisitionOp.get(OP).set(ADD);

		for (int i = 0; i < reader.AttributeCount; i++)
		{
		  Attribute attribute = Attribute.forName(reader.getAttributeLocalName(i));
		  switch (attribute.innerEnumValue)
		  {
			case org.camunda.bpm.container.impl.jboss.extension.Attribute.InnerEnum.NAME:
			{
			  acquisitionName = rawAttributeText(reader, NAME.LocalName);
			  if (!string.ReferenceEquals(acquisitionName, null) && !acquisitionName.Equals("null"))
			  {
				SubsystemAttributeDefinitons.NAME.parseAndSetParameter(acquisitionName, addJobAcquisitionOp, reader);
			  }
			  else
			  {
				throw missingRequiredElement(reader, Collections.singleton(NAME.LocalName));
			  }
			  break;
			}
			default:
			  throw unexpectedAttribute(reader, i);
		  }
		}

		ModelNode jobAcquisitionAddress = parentAddress.get(OP_ADDR).clone();
		jobAcquisitionAddress.add(ModelConstants_Fields.JOB_ACQUISITIONS, acquisitionName);
		addJobAcquisitionOp.get(OP_ADDR).set(jobAcquisitionAddress);

		operations.Add(addJobAcquisitionOp);

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
				  parseProperties(reader, operations, addJobAcquisitionOp);
				  break;
				}
				case ACQUISITION_STRATEGY:
				{
				  parseElement(SubsystemAttributeDefinitons.ACQUISITION_STRATEGY, addJobAcquisitionOp, reader);
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
//ORIGINAL LINE: protected void parseElement(org.jboss.as.controller.AttributeDefinition attributeDefinition, org.jboss.dmr.ModelNode operation, org.jboss.staxmapper.XMLExtendedStreamReader reader) throws javax.xml.stream.XMLStreamException
	  protected internal virtual void parseElement(AttributeDefinition attributeDefinition, ModelNode operation, XMLExtendedStreamReader reader)
	  {
		string value = rawElementText(reader);
		((SimpleAttributeDefinition) attributeDefinition).parseAndSetParameter(value, operation, reader);
	  }



	  public sealed class BpmPlatformSubsystemParser : XMLStreamConstants, XMLElementReader<IList<ModelNode>>, XMLElementWriter<SubsystemMarshallingContext>
	  {

		internal static readonly BpmPlatformSubsystemParser INSTANCE = new BpmPlatformSubsystemParser();

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void readElement(org.jboss.staxmapper.XMLExtendedStreamReader reader, java.util.List<org.jboss.dmr.ModelNode> operations) throws javax.xml.stream.XMLStreamException
		public override void readElement(XMLExtendedStreamReader reader, IList<ModelNode> operations)
		{
		  // Require no attributes
		  ParseUtils.requireNoAttributes(reader);

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
		  operations.Add(subsystemAdd);


		  while (reader.hasNext() && !reader.EndElement)
		  {
			switch (reader.LocalName)
			{
			  case SUBSYSTEM:
			  {
				try
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BpmPlatformParser1_1 parser = new BpmPlatformParser1_1();
				  BpmPlatformParser1_1 parser = new BpmPlatformParser1_1();
				  parser.parse(reader, operations, subsystemAddress);
				}
				catch (Exception e)
				{
				  throw new XMLStreamException(e);
				}
			  }
		  break;
			}
		  }
		}

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
		protected internal void writeProcessEnginesContent(XMLExtendedStreamWriter writer, SubsystemMarshallingContext context)
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
		protected internal void writeJobExecutorContent(XMLExtendedStreamWriter writer, SubsystemMarshallingContext context)
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
		protected internal void writeJobAcquisitionsContent(XMLExtendedStreamWriter writer, SubsystemMarshallingContext context, ModelNode parentNode)
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
	  }

	}

}