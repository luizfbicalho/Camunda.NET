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
	using AbstractFlowNodeBuilder = org.camunda.bpm.model.bpmn.builder.AbstractFlowNodeBuilder;
	using FlowElement = org.camunda.bpm.model.bpmn.instance.FlowElement;
	using FlowNode = org.camunda.bpm.model.bpmn.instance.FlowNode;
	using SequenceFlow = org.camunda.bpm.model.bpmn.instance.SequenceFlow;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;
	using AttributeReference = org.camunda.bpm.model.xml.type.reference.AttributeReference;
	using ElementReferenceCollection = org.camunda.bpm.model.xml.type.reference.ElementReferenceCollection;
	using Reference = org.camunda.bpm.model.xml.type.reference.Reference;


	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;

	/// <summary>
	/// The BPMN flowNode element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public abstract class FlowNodeImpl : FlowElementImpl, FlowNode
	{

	  protected internal static ElementReferenceCollection<SequenceFlow, Incoming> incomingCollection;
	  protected internal static ElementReferenceCollection<SequenceFlow, Outgoing> outgoingCollection;

	  /// <summary>
	  /// Camunda Attributes </summary>
	  protected internal static Attribute<bool> camundaAsyncAfter;
	  protected internal static Attribute<bool> camundaAsyncBefore;
	  protected internal static Attribute<bool> camundaExclusive;
	  protected internal static Attribute<string> camundaJobPriority;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(FlowNode), BPMN_ELEMENT_FLOW_NODE).namespaceUri(BPMN20_NS).extendsType(typeof(FlowElement)).abstractType();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		incomingCollection = sequenceBuilder.elementCollection(typeof(Incoming)).qNameElementReferenceCollection(typeof(SequenceFlow)).build();

		outgoingCollection = sequenceBuilder.elementCollection(typeof(Outgoing)).qNameElementReferenceCollection(typeof(SequenceFlow)).build();

		/// <summary>
		/// Camunda Attributes </summary>

		camundaAsyncAfter = typeBuilder.booleanAttribute(CAMUNDA_ATTRIBUTE_ASYNC_AFTER).@namespace(CAMUNDA_NS).defaultValue(false).build();

		camundaAsyncBefore = typeBuilder.booleanAttribute(CAMUNDA_ATTRIBUTE_ASYNC_BEFORE).@namespace(CAMUNDA_NS).defaultValue(false).build();

		camundaExclusive = typeBuilder.booleanAttribute(CAMUNDA_ATTRIBUTE_EXCLUSIVE).@namespace(CAMUNDA_NS).defaultValue(true).build();

		camundaJobPriority = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_JOB_PRIORITY).@namespace(CAMUNDA_NS).build();

		typeBuilder.build();
	  }

	  public FlowNodeImpl(ModelTypeInstanceContext context) : base(context)
	  {
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public org.camunda.bpm.model.bpmn.builder.AbstractFlowNodeBuilder builder()
	  public override AbstractFlowNodeBuilder builder()
	  {
		throw new BpmnModelException("No builder implemented for type " + ElementType.TypeNamespace + ":" + ElementType.TypeName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public void updateAfterReplacement()
	  public virtual void updateAfterReplacement()
	  {
		base.updateAfterReplacement();
		ICollection<Reference> incomingReferences = getIncomingReferencesByType(typeof(SequenceFlow));
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for (org.camunda.bpm.model.xml.type.reference.Reference<?> reference : incomingReferences)
		foreach (Reference<object> reference in incomingReferences)
		{
		  foreach (ModelElementInstance sourceElement in reference.findReferenceSourceElements(this))
		  {
			string referenceIdentifier = reference.getReferenceIdentifier(sourceElement);

			if (!string.ReferenceEquals(referenceIdentifier, null) && referenceIdentifier.Equals(Id) && reference is AttributeReference)
			{
			  string attributeName = ((AttributeReference) reference).ReferenceSourceAttribute.AttributeName;
			  if (attributeName.Equals(BPMN_ATTRIBUTE_SOURCE_REF))
			  {
				Outgoing.Add((SequenceFlow) sourceElement);
			  }
			  else if (attributeName.Equals(BPMN_ATTRIBUTE_TARGET_REF))
			  {
				Incoming.Add((SequenceFlow) sourceElement);
			  }
			}
		  }

		}
	  }

	  public virtual ICollection<SequenceFlow> Incoming
	  {
		  get
		  {
			return incomingCollection.getReferenceTargetElements(this);
		  }
	  }

	  public virtual ICollection<SequenceFlow> Outgoing
	  {
		  get
		  {
			return outgoingCollection.getReferenceTargetElements(this);
		  }
	  }

	  public virtual Query<FlowNode> PreviousNodes
	  {
		  get
		  {
			ICollection<FlowNode> previousNodes = new HashSet<FlowNode>();
			foreach (SequenceFlow sequenceFlow in Incoming)
			{
			  previousNodes.Add(sequenceFlow.Source);
			}
			return new QueryImpl<FlowNode>(previousNodes);
		  }
	  }

	  public virtual Query<FlowNode> SucceedingNodes
	  {
		  get
		  {
			ICollection<FlowNode> succeedingNodes = new HashSet<FlowNode>();
			foreach (SequenceFlow sequenceFlow in Outgoing)
			{
			  succeedingNodes.Add(sequenceFlow.Target);
			}
			return new QueryImpl<FlowNode>(succeedingNodes);
		  }
	  }

	  /// <summary>
	  /// Camunda Attributes </summary>

	  public virtual bool CamundaAsyncBefore
	  {
		  get
		  {
			return camundaAsyncBefore.getValue(this);
		  }
		  set
		  {
			camundaAsyncBefore.setValue(this, value);
		  }
	  }


	  public virtual bool CamundaAsyncAfter
	  {
		  get
		  {
			return camundaAsyncAfter.getValue(this);
		  }
		  set
		  {
			camundaAsyncAfter.setValue(this, value);
		  }
	  }


	  public virtual bool CamundaExclusive
	  {
		  get
		  {
			return camundaExclusive.getValue(this);
		  }
		  set
		  {
			camundaExclusive.setValue(this, value);
		  }
	  }


	  public virtual string CamundaJobPriority
	  {
		  get
		  {
			return camundaJobPriority.getValue(this);
		  }
		  set
		  {
			camundaJobPriority.setValue(this, value);
		  }
	  }

	}

}