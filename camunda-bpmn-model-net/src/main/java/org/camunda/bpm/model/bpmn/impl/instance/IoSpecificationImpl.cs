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
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN20_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ELEMENT_IO_SPECIFICATION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN IoSpecification element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class IoSpecificationImpl : BaseElementImpl, IoSpecification
	{

	  protected internal static ChildElementCollection<DataInput> dataInputCollection;
	  protected internal static ChildElementCollection<DataOutput> dataOutputCollection;
	  protected internal static ChildElementCollection<InputSet> inputSetCollection;
	  protected internal static ChildElementCollection<OutputSet> outputSetCollection;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(IoSpecification), BPMN_ELEMENT_IO_SPECIFICATION).namespaceUri(BPMN20_NS).extendsType(typeof(BaseElement)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		dataInputCollection = sequenceBuilder.elementCollection(typeof(DataInput)).build();

		dataOutputCollection = sequenceBuilder.elementCollection(typeof(DataOutput)).build();

		inputSetCollection = sequenceBuilder.elementCollection(typeof(InputSet)).required().build();

		outputSetCollection = sequenceBuilder.elementCollection(typeof(OutputSet)).required().build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<IoSpecification>
	  {
		  public IoSpecification newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new IoSpecificationImpl(instanceContext);
		  }
	  }

	  public IoSpecificationImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual ICollection<DataInput> DataInputs
	  {
		  get
		  {
			return dataInputCollection.get(this);
		  }
	  }

	  public virtual ICollection<DataOutput> DataOutputs
	  {
		  get
		  {
			return dataOutputCollection.get(this);
		  }
	  }

	  public virtual ICollection<InputSet> InputSets
	  {
		  get
		  {
			return inputSetCollection.get(this);
		  }
	  }

	  public virtual ICollection<OutputSet> OutputSets
	  {
		  get
		  {
			return outputSetCollection.get(this);
		  }
	  }
	}

}