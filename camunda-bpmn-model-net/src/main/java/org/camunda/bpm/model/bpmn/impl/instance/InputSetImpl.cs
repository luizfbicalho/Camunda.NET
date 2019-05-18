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
	using BaseElement = org.camunda.bpm.model.bpmn.instance.BaseElement;
	using DataInput = org.camunda.bpm.model.bpmn.instance.DataInput;
	using InputSet = org.camunda.bpm.model.bpmn.instance.InputSet;
	using OutputSet = org.camunda.bpm.model.bpmn.instance.OutputSet;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;
	using ElementReferenceCollection = org.camunda.bpm.model.xml.type.reference.ElementReferenceCollection;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN20_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ELEMENT_INPUT_SET;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN inputSet element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class InputSetImpl : BaseElementImpl, InputSet
	{

	  protected internal static Attribute<string> nameAttribute;
	  protected internal static ElementReferenceCollection<DataInput, DataInputRefs> dataInputDataInputRefsCollection;
	  protected internal static ElementReferenceCollection<DataInput, OptionalInputRefs> optionalInputRefsCollection;
	  protected internal static ElementReferenceCollection<DataInput, WhileExecutingInputRefs> whileExecutingInputRefsCollection;
	  protected internal static ElementReferenceCollection<OutputSet, OutputSetRefs> outputSetOutputSetRefsCollection;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(InputSet), BPMN_ELEMENT_INPUT_SET).namespaceUri(BPMN20_NS).extendsType(typeof(BaseElement)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		nameAttribute = typeBuilder.stringAttribute("name").build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		dataInputDataInputRefsCollection = sequenceBuilder.elementCollection(typeof(DataInputRefs)).idElementReferenceCollection(typeof(DataInput)).build();

		optionalInputRefsCollection = sequenceBuilder.elementCollection(typeof(OptionalInputRefs)).idElementReferenceCollection(typeof(DataInput)).build();

		whileExecutingInputRefsCollection = sequenceBuilder.elementCollection(typeof(WhileExecutingInputRefs)).idElementReferenceCollection(typeof(DataInput)).build();

		outputSetOutputSetRefsCollection = sequenceBuilder.elementCollection(typeof(OutputSetRefs)).idElementReferenceCollection(typeof(OutputSet)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<InputSet>
	  {
		  public InputSet newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new InputSetImpl(instanceContext);
		  }
	  }

	  public InputSetImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual string Name
	  {
		  get
		  {
			return nameAttribute.getValue(this);
		  }
		  set
		  {
			nameAttribute.setValue(this, value);
		  }
	  }


	  public virtual ICollection<DataInput> DataInputs
	  {
		  get
		  {
			return dataInputDataInputRefsCollection.getReferenceTargetElements(this);
		  }
	  }

	  public virtual ICollection<DataInput> OptionalInputs
	  {
		  get
		  {
			return optionalInputRefsCollection.getReferenceTargetElements(this);
		  }
	  }

	  public virtual ICollection<DataInput> WhileExecutingInput
	  {
		  get
		  {
			return whileExecutingInputRefsCollection.getReferenceTargetElements(this);
		  }
	  }

	  public virtual ICollection<OutputSet> OutputSets
	  {
		  get
		  {
			return outputSetOutputSetRefsCollection.getReferenceTargetElements(this);
		  }
	  }
	}

}