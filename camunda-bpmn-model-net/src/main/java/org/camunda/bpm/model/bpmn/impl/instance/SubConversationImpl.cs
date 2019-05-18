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
namespace org.camunda.bpm.model.bpmn.impl.instance
{
	using ConversationNode = org.camunda.bpm.model.bpmn.instance.ConversationNode;
	using SubConversation = org.camunda.bpm.model.bpmn.instance.SubConversation;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN20_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ELEMENT_SUB_CONVERSATION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN subConversation element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class SubConversationImpl : ConversationNodeImpl, SubConversation
	{

	  protected internal static ChildElementCollection<ConversationNode> conversationNodeCollection;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(SubConversation), BPMN_ELEMENT_SUB_CONVERSATION).namespaceUri(BPMN20_NS).extendsType(typeof(ConversationNode)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		conversationNodeCollection = sequenceBuilder.elementCollection(typeof(ConversationNode)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<SubConversation>
	  {
		  public SubConversation newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new SubConversationImpl(instanceContext);
		  }
	  }

	  public SubConversationImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual ICollection<ConversationNode> ConversationNodes
	  {
		  get
		  {
			return conversationNodeCollection.get(this);
		  }
	  }
	}

}