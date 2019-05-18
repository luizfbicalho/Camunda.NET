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
	using AfterClass = org.junit.AfterClass;
	using BeforeClass = org.junit.BeforeClass;
	using Test = org.junit.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class CollaborationParserTest
	{

	  private static BpmnModelInstance modelInstance;
	  private static Collaboration collaboration;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @BeforeClass public static void parseModel()
	  public static void parseModel()
	  {
		modelInstance = Bpmn.readModelFromStream(typeof(CollaborationParserTest).getResourceAsStream("CollaborationParserTest.bpmn"));
		collaboration = modelInstance.getModelElementById("collaboration1");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testConversations()
	  public virtual void testConversations()
	  {
		assertThat(collaboration.ConversationNodes).hasSize(1);

		ConversationNode conversationNode = collaboration.ConversationNodes.GetEnumerator().next();
		assertThat(conversationNode).isInstanceOf(typeof(Conversation));
		assertThat(conversationNode.Participants).Empty;
		assertThat(conversationNode.CorrelationKeys).Empty;
		assertThat(conversationNode.MessageFlows).Empty;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testConversationLink()
	  public virtual void testConversationLink()
	  {
		ICollection<ConversationLink> conversationLinks = collaboration.ConversationLinks;
		foreach (ConversationLink conversationLink in conversationLinks)
		{
		  assertThat(conversationLink.Id).StartsWith("conversationLink");
		  assertThat(conversationLink.Source).isInstanceOf(typeof(Participant));
		  Participant source = (Participant) conversationLink.Source;
		  assertThat(source.Name).isEqualTo("Pool");
		  assertThat(source.Id).StartsWith("participant");

		  assertThat(conversationLink.Target).isInstanceOf(typeof(Conversation));
		  Conversation target = (Conversation) conversationLink.Target;
		  assertThat(target.Id).isEqualTo("conversation1");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMessageFlow()
	  public virtual void testMessageFlow()
	  {
		ICollection<MessageFlow> messageFlows = collaboration.MessageFlows;
		foreach (MessageFlow messageFlow in messageFlows)
		{
		  assertThat(messageFlow.Id).StartsWith("messageFlow");
		  assertThat(messageFlow.Source).isInstanceOf(typeof(ServiceTask));
		  assertThat(messageFlow.Target).isInstanceOf(typeof(Event));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testParticipant()
	  public virtual void testParticipant()
	  {
		ICollection<Participant> participants = collaboration.Participants;
		foreach (Participant participant in participants)
		{
		  assertThat(participant.Process.Id).StartsWith("process");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnused()
	  public virtual void testUnused()
	  {
		assertThat(collaboration.CorrelationKeys).Empty;
		assertThat(collaboration.Artifacts).Empty;
		assertThat(collaboration.ConversationAssociations).Empty;
		assertThat(collaboration.MessageFlowAssociations).Empty;
		assertThat(collaboration.ParticipantAssociations).Empty;
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @AfterClass public static void validateModel()
	  public static void validateModel()
	  {
		Bpmn.validateModel(modelInstance);
	  }

	}

}