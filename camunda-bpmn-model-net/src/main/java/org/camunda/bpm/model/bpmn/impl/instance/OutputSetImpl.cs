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
	using org.camunda.bpm.model.bpmn.instance;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;
	using ElementReferenceCollection = org.camunda.bpm.model.xml.type.reference.ElementReferenceCollection;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN outputSet element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class OutputSetImpl : BaseElementImpl, OutputSet
	{

	  protected internal static Attribute<string> nameAttribute;
	  protected internal static ElementReferenceCollection<DataOutput, DataOutputRefs> dataOutputRefsCollection;
	  protected internal static ElementReferenceCollection<DataOutput, OptionalOutputRefs> optionalOutputRefsCollection;
	  protected internal static ElementReferenceCollection<DataOutput, WhileExecutingOutputRefs> whileExecutingOutputRefsCollection;
	  protected internal static ElementReferenceCollection<InputSet, InputSetRefs> inputSetInputSetRefsCollection;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(OutputSet), BPMN_ELEMENT_OUTPUT_SET).namespaceUri(BPMN20_NS).extendsType(typeof(BaseElement)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		nameAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_NAME).build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		dataOutputRefsCollection = sequenceBuilder.elementCollection(typeof(DataOutputRefs)).idElementReferenceCollection(typeof(DataOutput)).build();

		optionalOutputRefsCollection = sequenceBuilder.elementCollection(typeof(OptionalOutputRefs)).idElementReferenceCollection(typeof(DataOutput)).build();

		whileExecutingOutputRefsCollection = sequenceBuilder.elementCollection(typeof(WhileExecutingOutputRefs)).idElementReferenceCollection(typeof(DataOutput)).build();

		inputSetInputSetRefsCollection = sequenceBuilder.elementCollection(typeof(InputSetRefs)).idElementReferenceCollection(typeof(InputSet)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<OutputSet>
	  {
		  public OutputSet newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new OutputSetImpl(instanceContext);
		  }
	  }

	  public OutputSetImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
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


	  public virtual ICollection<DataOutput> DataOutputRefs
	  {
		  get
		  {
			return dataOutputRefsCollection.getReferenceTargetElements(this);
		  }
	  }

	  public virtual ICollection<DataOutput> OptionalOutputRefs
	  {
		  get
		  {
			return optionalOutputRefsCollection.getReferenceTargetElements(this);
		  }
	  }

	  public virtual ICollection<DataOutput> WhileExecutingOutputRefs
	  {
		  get
		  {
			return whileExecutingOutputRefsCollection.getReferenceTargetElements(this);
		  }
	  }

	  public virtual ICollection<InputSet> InputSetRefs
	  {
		  get
		  {
			return inputSetInputSetRefsCollection.getReferenceTargetElements(this);
		  }
	  }
	}

}