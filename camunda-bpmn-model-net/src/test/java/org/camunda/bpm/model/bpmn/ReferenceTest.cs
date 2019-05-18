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
namespace org.camunda.bpm.model.bpmn
{
	using org.camunda.bpm.model.bpmn.instance;
	using BpmnModelResource = org.camunda.bpm.model.bpmn.util.BpmnModelResource;
	using Before = org.junit.Before;
	using Test = org.junit.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;

	/// <summary>
	/// @author Sebastian Menski
	/// 
	/// </summary>
	public class ReferenceTest : BpmnModelTest
	{

	  private BpmnModelInstance testBpmnModelInstance;
	  private Message message;
	  private MessageEventDefinition messageEventDefinition;
	  private StartEvent startEvent;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void createModel()
	  public virtual void createModel()
	  {
		testBpmnModelInstance = Bpmn.createEmptyModel();
		Definitions definitions = testBpmnModelInstance.newInstance(typeof(Definitions));
		testBpmnModelInstance.Definitions = definitions;

		message = testBpmnModelInstance.newInstance(typeof(Message));
		message.Id = "message-id";
		definitions.RootElements.Add(message);

		Process process = testBpmnModelInstance.newInstance(typeof(Process));
		process.Id = "process-id";
		definitions.RootElements.Add(process);

		startEvent = testBpmnModelInstance.newInstance(typeof(StartEvent));
		startEvent.Id = "start-event-id";
		process.FlowElements.Add(startEvent);

		messageEventDefinition = testBpmnModelInstance.newInstance(typeof(MessageEventDefinition));
		messageEventDefinition.Id = "msg-def-id";
		messageEventDefinition.Message = message;
		startEvent.EventDefinitions.Add(messageEventDefinition);

		startEvent.EventDefinitionRefs.Add(messageEventDefinition);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testShouldUpdateReferenceOnIdChange()
	  public virtual void testShouldUpdateReferenceOnIdChange()
	  {
		assertThat(messageEventDefinition.Message).isEqualTo(message);
		message.Id = "changed-message-id";
		assertThat(message.Id).isEqualTo("changed-message-id");
		assertThat(messageEventDefinition.Message).isEqualTo(message);

		message.setAttributeValue("id", "another-message-id", true);
		assertThat(message.Id).isEqualTo("another-message-id");
		assertThat(messageEventDefinition.Message).isEqualTo(message);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testShouldRemoveReferenceIfReferencingElementIsRemoved()
	  public virtual void testShouldRemoveReferenceIfReferencingElementIsRemoved()
	  {
		assertThat(messageEventDefinition.Message).isEqualTo(message);

		Definitions definitions = testBpmnModelInstance.Definitions;
		definitions.RootElements.remove(message);

		assertThat(messageEventDefinition.Id).isEqualTo("msg-def-id");
		assertThat(messageEventDefinition.Message).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testShouldRemoveReferenceIfReferencingAttributeIsRemoved()
	  public virtual void testShouldRemoveReferenceIfReferencingAttributeIsRemoved()
	  {
		assertThat(messageEventDefinition.Message).isEqualTo(message);

		message.removeAttribute("id");

		assertThat(messageEventDefinition.Id).isEqualTo("msg-def-id");
		assertThat(messageEventDefinition.Message).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testShouldUpdateReferenceIfReferencingElementIsReplaced()
	  public virtual void testShouldUpdateReferenceIfReferencingElementIsReplaced()
	  {
		assertThat(messageEventDefinition.Message).isEqualTo(message);
		Message newMessage = testBpmnModelInstance.newInstance(typeof(Message));
		newMessage.Id = "new-message-id";

		message.replaceWithElement(newMessage);

		assertThat(messageEventDefinition.Message).isEqualTo(newMessage);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testShouldAddMessageEventDefinitionRef()
	  public virtual void testShouldAddMessageEventDefinitionRef()
	  {
		ICollection<EventDefinition> eventDefinitionRefs = startEvent.EventDefinitionRefs;
		assertThat(eventDefinitionRefs).NotEmpty;
		assertThat(eventDefinitionRefs).contains(messageEventDefinition);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testShouldUpdateMessageEventDefinitionRefOnIdChange()
	  public virtual void testShouldUpdateMessageEventDefinitionRefOnIdChange()
	  {
		ICollection<EventDefinition> eventDefinitionRefs = startEvent.EventDefinitionRefs;
		assertThat(eventDefinitionRefs).contains(messageEventDefinition);
		messageEventDefinition.Id = "changed-message-event-definition-id";
		assertThat(eventDefinitionRefs).contains(messageEventDefinition);
		messageEventDefinition.setAttributeValue("id", "another-message-event-definition-id", true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testShouldRemoveMessageEventDefinitionRefIfMessageEventDefinitionIsRemoved()
	  public virtual void testShouldRemoveMessageEventDefinitionRefIfMessageEventDefinitionIsRemoved()
	  {
		startEvent.EventDefinitions.remove(messageEventDefinition);
		ICollection<EventDefinition> eventDefinitionRefs = startEvent.EventDefinitionRefs;
		assertThat(eventDefinitionRefs).doesNotContain(messageEventDefinition);
		assertThat(eventDefinitionRefs).Empty;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testShouldReplaceMessageEventDefinitionRefIfMessageEventDefinitionIsReplaced()
	  public virtual void testShouldReplaceMessageEventDefinitionRefIfMessageEventDefinitionIsReplaced()
	  {
		MessageEventDefinition otherMessageEventDefinition = testBpmnModelInstance.newInstance(typeof(MessageEventDefinition));
		otherMessageEventDefinition.Id = "other-message-event-definition-id";
		ICollection<EventDefinition> eventDefinitionRefs = startEvent.EventDefinitionRefs;
		assertThat(eventDefinitionRefs).contains(messageEventDefinition);
		messageEventDefinition.replaceWithElement(otherMessageEventDefinition);
		assertThat(eventDefinitionRefs).doesNotContain(messageEventDefinition);
		assertThat(eventDefinitionRefs).contains(otherMessageEventDefinition);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testShouldRemoveMessageEventDefinitionRefIfIdIsRemovedOfMessageEventDefinition()
	  public virtual void testShouldRemoveMessageEventDefinitionRefIfIdIsRemovedOfMessageEventDefinition()
	  {
		ICollection<EventDefinition> eventDefinitionRefs = startEvent.EventDefinitionRefs;
		assertThat(eventDefinitionRefs).contains(messageEventDefinition);
		messageEventDefinition.removeAttribute("id");
		assertThat(eventDefinitionRefs).doesNotContain(messageEventDefinition);
		assertThat(eventDefinitionRefs).Empty;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @BpmnModelResource public void shouldFindReferenceWithNamespace()
	  public virtual void shouldFindReferenceWithNamespace()
	  {
		MessageEventDefinition messageEventDefinition = bpmnModelInstance.getModelElementById("message-event-definition");
		Message message = bpmnModelInstance.getModelElementById("message-id");
		assertThat(messageEventDefinition.Message).NotNull;
		assertThat(messageEventDefinition.Message).isEqualTo(message);
		message.Id = "changed-message";
		assertThat(messageEventDefinition.Message).NotNull;
		assertThat(messageEventDefinition.Message).isEqualTo(message);
		message.setAttributeValue("id", "again-changed-message", true);
		assertThat(messageEventDefinition.Message).NotNull;
		assertThat(messageEventDefinition.Message).isEqualTo(message);

		StartEvent startEvent = bpmnModelInstance.getModelElementById("start-event");
		ICollection<EventDefinition> eventDefinitionRefs = startEvent.EventDefinitionRefs;
		assertThat(eventDefinitionRefs).NotEmpty;
		assertThat(eventDefinitionRefs).contains(messageEventDefinition);
		messageEventDefinition.Id = "changed-message-event";
		assertThat(eventDefinitionRefs).NotEmpty;
		assertThat(eventDefinitionRefs).contains(messageEventDefinition);
		messageEventDefinition.setAttributeValue("id", "again-changed-message-event", true);
		assertThat(eventDefinitionRefs).NotEmpty;
		assertThat(eventDefinitionRefs).contains(messageEventDefinition);

		message.removeAttribute("id");
		assertThat(messageEventDefinition.Message).Null;
		messageEventDefinition.removeAttribute("id");
		assertThat(eventDefinitionRefs).doesNotContain(messageEventDefinition);
		assertThat(eventDefinitionRefs).Empty;
	  }
	}

}