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
	using SequenceFlowBuilder = org.camunda.bpm.model.bpmn.builder.SequenceFlowBuilder;
	using ConditionExpression = org.camunda.bpm.model.bpmn.instance.ConditionExpression;
	using FlowElement = org.camunda.bpm.model.bpmn.instance.FlowElement;
	using FlowNode = org.camunda.bpm.model.bpmn.instance.FlowNode;
	using SequenceFlow = org.camunda.bpm.model.bpmn.instance.SequenceFlow;
	using BpmnEdge = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnEdge;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using ChildElement = org.camunda.bpm.model.xml.type.child.ChildElement;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;
	using AttributeReference = org.camunda.bpm.model.xml.type.reference.AttributeReference;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN sequenceFlow element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class SequenceFlowImpl : FlowElementImpl, SequenceFlow
	{

	  protected internal static AttributeReference<FlowNode> sourceRefAttribute;
	  protected internal static AttributeReference<FlowNode> targetRefAttribute;
	  protected internal static Attribute<bool> isImmediateAttribute;
	  protected internal static ChildElement<ConditionExpression> conditionExpressionCollection;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(SequenceFlow), BPMN_ELEMENT_SEQUENCE_FLOW).namespaceUri(BPMN20_NS).extendsType(typeof(FlowElement)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		sourceRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_SOURCE_REF).required().idAttributeReference(typeof(FlowNode)).build();

		targetRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_TARGET_REF).required().idAttributeReference(typeof(FlowNode)).build();

		isImmediateAttribute = typeBuilder.booleanAttribute(BPMN_ATTRIBUTE_IS_IMMEDIATE).build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		conditionExpressionCollection = sequenceBuilder.element(typeof(ConditionExpression)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<SequenceFlow>
	  {
		  public SequenceFlow newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new SequenceFlowImpl(instanceContext);
		  }
	  }

	  public SequenceFlowImpl(ModelTypeInstanceContext context) : base(context)
	  {
	  }

	  public override SequenceFlowBuilder builder()
	  {
		return new SequenceFlowBuilder((BpmnModelInstance) modelInstance, this);
	  }

	  public virtual FlowNode Source
	  {
		  get
		  {
			return sourceRefAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			sourceRefAttribute.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual FlowNode Target
	  {
		  get
		  {
			return targetRefAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			targetRefAttribute.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual bool Immediate
	  {
		  get
		  {
			return isImmediateAttribute.getValue(this);
		  }
		  set
		  {
			isImmediateAttribute.setValue(this, value);
		  }
	  }


	  public virtual ConditionExpression ConditionExpression
	  {
		  get
		  {
			return conditionExpressionCollection.getChild(this);
		  }
		  set
		  {
			conditionExpressionCollection.setChild(this, value);
		  }
	  }


	  public virtual void removeConditionExpression()
	  {
		conditionExpressionCollection.removeChild(this);
	  }

	  public override BpmnEdge DiagramElement
	  {
		  get
		  {
			return (BpmnEdge) base.DiagramElement;
		  }
	  }

	}

}