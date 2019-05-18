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
namespace org.camunda.bpm.model.bpmn.instance
{

	/// <summary>
	/// The BPMN collaboration element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public interface Collaboration : RootElement
	{

	  string Name {get;set;}


	  bool Closed {get;set;}


	  ICollection<Participant> Participants {get;}

	  ICollection<MessageFlow> MessageFlows {get;}

	  ICollection<Artifact> Artifacts {get;}

	  ICollection<ConversationNode> ConversationNodes {get;}

	  ICollection<ConversationAssociation> ConversationAssociations {get;}

	  ICollection<ParticipantAssociation> ParticipantAssociations {get;}

	  ICollection<MessageFlowAssociation> MessageFlowAssociations {get;}

	  ICollection<CorrelationKey> CorrelationKeys {get;}

	  /// <summary>
	  /// TODO: choreographyRef </summary>

	  ICollection<ConversationLink> ConversationLinks {get;}

	}

}