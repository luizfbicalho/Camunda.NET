using System;
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
namespace org.camunda.bpm.model.bpmn
{
	using org.camunda.bpm.model.bpmn.instance;
	using BpmnModelResource = org.camunda.bpm.model.bpmn.util.BpmnModelResource;
	using ModelParseException = org.camunda.bpm.model.xml.ModelParseException;
	using ModelReferenceException = org.camunda.bpm.model.xml.ModelReferenceException;
	using ModelValidationException = org.camunda.bpm.model.xml.ModelValidationException;
	using IoUtil = org.camunda.bpm.model.xml.impl.util.IoUtil;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.XML_SCHEMA_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.XPATH_NS;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class DefinitionsTest : BpmnModelTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @BpmnModelResource public void shouldImportEmptyDefinitions()
	  public virtual void shouldImportEmptyDefinitions()
	  {

		Definitions definitions = bpmnModelInstance.Definitions;
		assertThat(definitions).NotNull;

		// provided in file
		assertThat(definitions.TargetNamespace).isEqualTo("http://camunda.org/test");

		// defaults provided in Schema
		assertThat(definitions.ExpressionLanguage).isEqualTo(XPATH_NS);
		assertThat(definitions.TypeLanguage).isEqualTo(XML_SCHEMA_NS);

		// not provided in file
		assertThat(definitions.Exporter).Null;
		assertThat(definitions.ExporterVersion).Null;
		assertThat(definitions.Id).Null;
		assertThat(definitions.Name).Null;

		// has no imports
		assertThat(definitions.Imports).Empty;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotImportWrongOrderedSequence()
	  public virtual void shouldNotImportWrongOrderedSequence()
	  {
		try
		{
		  Bpmn.readModelFromStream(this.GetType().getResourceAsStream("DefinitionsTest.shouldNotImportWrongOrderedSequence.bpmn"));
		  Assert.fail("Model is invalid and should not pass the validation");
		}
		catch (Exception e)
		{
		  assertThat(e).isInstanceOf(typeof(ModelParseException));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldAddChildElementsInCorrectOrder()
	  public virtual void shouldAddChildElementsInCorrectOrder()
	  {
		// create an empty model
		BpmnModelInstance bpmnModelInstance = Bpmn.createEmptyModel();

		// add definitions
		Definitions definitions = bpmnModelInstance.newInstance(typeof(Definitions));
		definitions.TargetNamespace = "Examples";
		bpmnModelInstance.Definitions = definitions;

		// create a Process element and add it to the definitions
		Process process = bpmnModelInstance.newInstance(typeof(Process));
		process.Id = "some-process-id";
		definitions.RootElements.Add(process);

		// create an Import element and add it to the definitions
		Import importElement = bpmnModelInstance.newInstance(typeof(Import));
		importElement.Namespace = "Imports";
		importElement.Location = "here";
		importElement.ImportType = "example";
		definitions.Imports.Add(importElement);

		// create another Process element and add it to the definitions
		process = bpmnModelInstance.newInstance(typeof(Process));
		process.Id = "another-process-id";
		definitions.RootElements.Add(process);

		// create another Import element and add it to the definitions
		importElement = bpmnModelInstance.newInstance(typeof(Import));
		importElement.Namespace = "Imports";
		importElement.Location = "there";
		importElement.ImportType = "example";
		definitions.Imports.Add(importElement);

		// validate model
		try
		{
		  Bpmn.validateModel(bpmnModelInstance);
		}
		catch (ModelValidationException)
		{
		  Assert.fail();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @BpmnModelResource public void shouldNotAffectComments() throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void shouldNotAffectComments()
	  {
		Definitions definitions = bpmnModelInstance.Definitions;
		assertThat(definitions).NotNull;

		// create another Process element and add it to the definitions
		Process process = bpmnModelInstance.newInstance(typeof(Process));
		process.Id = "another-process-id";
		definitions.RootElements.Add(process);

		// create another Import element and add it to the definitions
		Import importElement = bpmnModelInstance.newInstance(typeof(Import));
		importElement.Namespace = "Imports";
		importElement.Location = "there";
		importElement.ImportType = "example";
		definitions.Imports.Add(importElement);

		// validate model
		try
		{
		  Bpmn.validateModel(bpmnModelInstance);
		}
		catch (ModelValidationException)
		{
		  Assert.fail();
		}

		// convert the model to the XML string representation
		Stream outputStream = new MemoryStream();
		Bpmn.writeModelToStream(outputStream, bpmnModelInstance);
		Stream inputStream = IoUtil.convertOutputStreamToInputStream(outputStream);
		string modelString = IoUtil.getStringFromInputStream(inputStream);
		IoUtil.closeSilently(outputStream);
		IoUtil.closeSilently(inputStream);

		// read test process from file as string
		inputStream = this.GetType().getResourceAsStream("DefinitionsTest.shouldNotAffectCommentsResult.bpmn");
		string fileString = IoUtil.getStringFromInputStream(inputStream);
		IoUtil.closeSilently(inputStream);

		// compare strings
		assertThat(modelString).EndsWith(fileString);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldAddMessageAndMessageEventDefinition()
	  public virtual void shouldAddMessageAndMessageEventDefinition()
	  {
		// create empty model
		BpmnModelInstance bpmnModelInstance = Bpmn.createEmptyModel();

		// add definitions to model
		Definitions definitions = bpmnModelInstance.newInstance(typeof(Definitions));
		definitions.TargetNamespace = "Examples";
		bpmnModelInstance.Definitions = definitions;

		// create and add message
		Message message = bpmnModelInstance.newInstance(typeof(Message));
		message.Id = "start-message-id";
		definitions.RootElements.Add(message);

		// create and add message event definition
		MessageEventDefinition messageEventDefinition = bpmnModelInstance.newInstance(typeof(MessageEventDefinition));
		messageEventDefinition.Id = "message-event-def-id";
		messageEventDefinition.Message = message;
		definitions.RootElements.Add(messageEventDefinition);

		// test if message was set correctly
		Message setMessage = messageEventDefinition.Message;
		assertThat(setMessage).isEqualTo(message);

		// add process
		Process process = bpmnModelInstance.newInstance(typeof(Process));
		process.Id = "messageEventDefinition";
		definitions.RootElements.Add(process);

		// add start event
		StartEvent startEvent = bpmnModelInstance.newInstance(typeof(StartEvent));
		startEvent.Id = "theStart";
		process.FlowElements.Add(startEvent);

		// create and add message event definition to start event
		MessageEventDefinition startEventMessageEventDefinition = bpmnModelInstance.newInstance(typeof(MessageEventDefinition));
		startEventMessageEventDefinition.Message = message;
		startEvent.EventDefinitions.Add(startEventMessageEventDefinition);

		// create another message but do not add it
		Message anotherMessage = bpmnModelInstance.newInstance(typeof(Message));
		anotherMessage.Id = "another-message-id";

		// create a message event definition and try to add last create message
		MessageEventDefinition anotherMessageEventDefinition = bpmnModelInstance.newInstance(typeof(MessageEventDefinition));
		try
		{
		  anotherMessageEventDefinition.Message = anotherMessage;
		  Assert.fail("Message should not be added to message event definition, cause it is not part of the model");
		}
		catch (Exception e)
		{
		  assertThat(e).isInstanceOf(typeof(ModelReferenceException));
		}

		// first add message to model than to event definition
		definitions.RootElements.Add(anotherMessage);
		anotherMessageEventDefinition.Message = anotherMessage;
		startEvent.EventDefinitions.Add(anotherMessageEventDefinition);

		// message event definition and add message by id to it
		anotherMessageEventDefinition = bpmnModelInstance.newInstance(typeof(MessageEventDefinition));
		startEvent.EventDefinitions.Add(anotherMessageEventDefinition);

		// validate model
		try
		{
		  Bpmn.validateModel(bpmnModelInstance);
		}
		catch (ModelValidationException)
		{
		  Assert.fail();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldAddParentChildElementInCorrectOrder()
	  public virtual void shouldAddParentChildElementInCorrectOrder()
	  {
		// create empty model
		BpmnModelInstance bpmnModelInstance = Bpmn.createEmptyModel();

		// add definitions to model
		Definitions definitions = bpmnModelInstance.newInstance(typeof(Definitions));
		definitions.TargetNamespace = "Examples";
		bpmnModelInstance.Definitions = definitions;

		// add process
		Process process = bpmnModelInstance.newInstance(typeof(Process));
		process.Id = "messageEventDefinition";
		definitions.RootElements.Add(process);

		// add start event
		StartEvent startEvent = bpmnModelInstance.newInstance(typeof(StartEvent));
		startEvent.Id = "theStart";
		process.FlowElements.Add(startEvent);

		// create and add message
		Message message = bpmnModelInstance.newInstance(typeof(Message));
		message.Id = "start-message-id";
		definitions.RootElements.Add(message);

		// add message event definition to start event
		MessageEventDefinition startEventMessageEventDefinition = bpmnModelInstance.newInstance(typeof(MessageEventDefinition));
		startEventMessageEventDefinition.Message = message;
		startEvent.EventDefinitions.Add(startEventMessageEventDefinition);

		// add property after message event definition
		Property property = bpmnModelInstance.newInstance(typeof(Property));
		startEvent.Properties.Add(property);

		// finally add an extensions element
		ExtensionElements extensionElements = bpmnModelInstance.newInstance(typeof(ExtensionElements));
		process.ExtensionElements = extensionElements;

		// validate model
		try
		{
		  Bpmn.validateModel(bpmnModelInstance);
		}
		catch (ModelValidationException)
		{
		  Assert.fail();
		}
	  }

	}

}