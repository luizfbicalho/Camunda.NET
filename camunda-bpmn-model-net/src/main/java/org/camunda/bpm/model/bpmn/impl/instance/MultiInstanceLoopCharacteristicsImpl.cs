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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN20_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ELEMENT_BEHAVIOR;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ELEMENT_IS_SEQUENTIAL;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ELEMENT_MULTI_INSTANCE_LOOP_CHARACTERISTICS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ELEMENT_NONE_BEHAVIOR_EVENT_REF;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ELEMENT_ONE_BEHAVIOR_EVENT_REF;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_ASYNC_AFTER;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_ASYNC_BEFORE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_EXCLUSIVE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_COLLECTION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_ELEMENT_VARIABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_NS;

	using MultiInstanceLoopCharacteristicsBuilder = org.camunda.bpm.model.bpmn.builder.MultiInstanceLoopCharacteristicsBuilder;
	using CompletionCondition = org.camunda.bpm.model.bpmn.instance.CompletionCondition;
	using ComplexBehaviorDefinition = org.camunda.bpm.model.bpmn.instance.ComplexBehaviorDefinition;
	using DataInput = org.camunda.bpm.model.bpmn.instance.DataInput;
	using DataOutput = org.camunda.bpm.model.bpmn.instance.DataOutput;
	using EventDefinition = org.camunda.bpm.model.bpmn.instance.EventDefinition;
	using InputDataItem = org.camunda.bpm.model.bpmn.instance.InputDataItem;
	using LoopCardinality = org.camunda.bpm.model.bpmn.instance.LoopCardinality;
	using LoopCharacteristics = org.camunda.bpm.model.bpmn.instance.LoopCharacteristics;
	using MultiInstanceLoopCharacteristics = org.camunda.bpm.model.bpmn.instance.MultiInstanceLoopCharacteristics;
	using OutputDataItem = org.camunda.bpm.model.bpmn.instance.OutputDataItem;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using ModelTypeInstanceProvider = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using ChildElement = org.camunda.bpm.model.xml.type.child.ChildElement;
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;
	using AttributeReference = org.camunda.bpm.model.xml.type.reference.AttributeReference;
	using ElementReference = org.camunda.bpm.model.xml.type.reference.ElementReference;

	/// <summary>
	/// The BPMN 2.0 multiInstanceLoopCharacteristics element
	/// 
	/// @author Filip Hrisafov
	/// </summary>
	public class MultiInstanceLoopCharacteristicsImpl : LoopCharacteristicsImpl, MultiInstanceLoopCharacteristics
	{

	  protected internal static Attribute<bool> isSequentialAttribute;
	  protected internal static Attribute<MultiInstanceFlowCondition> behaviorAttribute;
	  protected internal static AttributeReference<EventDefinition> oneBehaviorEventRefAttribute;
	  protected internal static AttributeReference<EventDefinition> noneBehaviorEventRefAttribute;
	  protected internal static ChildElement<LoopCardinality> loopCardinalityChild;
	  protected internal static ElementReference<DataInput, LoopDataInputRef> loopDataInputRefChild;
	  protected internal static ElementReference<DataOutput, LoopDataOutputRef> loopDataOutputRefChild;
	  protected internal static ChildElement<InputDataItem> inputDataItemChild;
	  protected internal static ChildElement<OutputDataItem> outputDataItemChild;
	  protected internal static ChildElementCollection<ComplexBehaviorDefinition> complexBehaviorDefinitionCollection;
	  protected internal static ChildElement<CompletionCondition> completionConditionChild;
	  protected internal static Attribute<bool> camundaAsyncAfter;
	  protected internal static Attribute<bool> camundaAsyncBefore;
	  protected internal static Attribute<bool> camundaExclusive;
	  protected internal static Attribute<string> camundaCollection;
	  protected internal static Attribute<string> camundaElementVariable;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(MultiInstanceLoopCharacteristics), BPMN_ELEMENT_MULTI_INSTANCE_LOOP_CHARACTERISTICS).namespaceUri(BPMN20_NS).extendsType(typeof(LoopCharacteristics)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		isSequentialAttribute = typeBuilder.booleanAttribute(BPMN_ELEMENT_IS_SEQUENTIAL).defaultValue(false).build();

		behaviorAttribute = typeBuilder.enumAttribute(BPMN_ELEMENT_BEHAVIOR, typeof(MultiInstanceFlowCondition)).defaultValue(MultiInstanceFlowCondition.All).build();

		oneBehaviorEventRefAttribute = typeBuilder.stringAttribute(BPMN_ELEMENT_ONE_BEHAVIOR_EVENT_REF).qNameAttributeReference(typeof(EventDefinition)).build();

		noneBehaviorEventRefAttribute = typeBuilder.stringAttribute(BPMN_ELEMENT_NONE_BEHAVIOR_EVENT_REF).qNameAttributeReference(typeof(EventDefinition)).build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		loopCardinalityChild = sequenceBuilder.element(typeof(LoopCardinality)).build();

		loopDataInputRefChild = sequenceBuilder.element(typeof(LoopDataInputRef)).qNameElementReference(typeof(DataInput)).build();

		loopDataOutputRefChild = sequenceBuilder.element(typeof(LoopDataOutputRef)).qNameElementReference(typeof(DataOutput)).build();

		outputDataItemChild = sequenceBuilder.element(typeof(OutputDataItem)).build();

		inputDataItemChild = sequenceBuilder.element(typeof(InputDataItem)).build();

		complexBehaviorDefinitionCollection = sequenceBuilder.elementCollection(typeof(ComplexBehaviorDefinition)).build();

		completionConditionChild = sequenceBuilder.element(typeof(CompletionCondition)).build();

		camundaAsyncAfter = typeBuilder.booleanAttribute(CAMUNDA_ATTRIBUTE_ASYNC_AFTER).@namespace(CAMUNDA_NS).defaultValue(false).build();

		camundaAsyncBefore = typeBuilder.booleanAttribute(CAMUNDA_ATTRIBUTE_ASYNC_BEFORE).@namespace(CAMUNDA_NS).defaultValue(false).build();

		camundaExclusive = typeBuilder.booleanAttribute(CAMUNDA_ATTRIBUTE_EXCLUSIVE).@namespace(CAMUNDA_NS).defaultValue(true).build();

		camundaCollection = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_COLLECTION).@namespace(CAMUNDA_NS).build();

		camundaElementVariable = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_ELEMENT_VARIABLE).@namespace(CAMUNDA_NS).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelElementTypeBuilder.ModelTypeInstanceProvider<MultiInstanceLoopCharacteristics>
	  {

		  public MultiInstanceLoopCharacteristics newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new MultiInstanceLoopCharacteristicsImpl(instanceContext);
		  }
	  }

	  public MultiInstanceLoopCharacteristicsImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public override MultiInstanceLoopCharacteristicsBuilder builder()
	  {
		return new MultiInstanceLoopCharacteristicsBuilder((BpmnModelInstance) modelInstance, this);
	  }

	  public virtual LoopCardinality LoopCardinality
	  {
		  get
		  {
			return loopCardinalityChild.getChild(this);
		  }
		  set
		  {
			loopCardinalityChild.setChild(this, value);
		  }
	  }


	  public virtual DataInput LoopDataInputRef
	  {
		  get
		  {
			return loopDataInputRefChild.getReferenceTargetElement(this);
		  }
		  set
		  {
			loopDataInputRefChild.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual DataOutput LoopDataOutputRef
	  {
		  get
		  {
			return loopDataOutputRefChild.getReferenceTargetElement(this);
		  }
		  set
		  {
			loopDataOutputRefChild.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual InputDataItem InputDataItem
	  {
		  get
		  {
			return inputDataItemChild.getChild(this);
		  }
		  set
		  {
			inputDataItemChild.setChild(this, value);
		  }
	  }


	  public virtual OutputDataItem OutputDataItem
	  {
		  get
		  {
			return outputDataItemChild.getChild(this);
		  }
		  set
		  {
			outputDataItemChild.setChild(this, value);
		  }
	  }


	  public virtual ICollection<ComplexBehaviorDefinition> ComplexBehaviorDefinitions
	  {
		  get
		  {
			return complexBehaviorDefinitionCollection.get(this);
		  }
	  }

	  public virtual CompletionCondition CompletionCondition
	  {
		  get
		  {
			return completionConditionChild.getChild(this);
		  }
		  set
		  {
			completionConditionChild.setChild(this, value);
		  }
	  }


	  public virtual bool Sequential
	  {
		  get
		  {
			return isSequentialAttribute.getValue(this);
		  }
		  set
		  {
			isSequentialAttribute.setValue(this, value);
		  }
	  }


	  public virtual MultiInstanceFlowCondition Behavior
	  {
		  get
		  {
			return behaviorAttribute.getValue(this);
		  }
		  set
		  {
			behaviorAttribute.setValue(this, value);
		  }
	  }


	  public virtual EventDefinition OneBehaviorEventRef
	  {
		  get
		  {
			return oneBehaviorEventRefAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			oneBehaviorEventRefAttribute.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual EventDefinition NoneBehaviorEventRef
	  {
		  get
		  {
			return noneBehaviorEventRefAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			noneBehaviorEventRefAttribute.setReferenceTargetElement(this, value);
		  }
	  }


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


	  public virtual string CamundaCollection
	  {
		  get
		  {
			return camundaCollection.getValue(this);
		  }
		  set
		  {
			camundaCollection.setValue(this, value);
		  }
	  }


	  public virtual string CamundaElementVariable
	  {
		  get
		  {
			return camundaElementVariable.getValue(this);
		  }
		  set
		  {
			camundaElementVariable.setValue(this, value);
		  }
	  }

	}

}