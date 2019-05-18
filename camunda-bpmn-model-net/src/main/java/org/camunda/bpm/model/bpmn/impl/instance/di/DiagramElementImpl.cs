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
namespace org.camunda.bpm.model.bpmn.impl.instance.di
{
	using DiagramElement = org.camunda.bpm.model.bpmn.instance.di.DiagramElement;
	using Extension = org.camunda.bpm.model.bpmn.instance.di.Extension;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using ChildElement = org.camunda.bpm.model.xml.type.child.ChildElement;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.DI_ATTRIBUTE_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.DI_ELEMENT_DIAGRAM_ELEMENT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.DI_NS;

	/// <summary>
	/// The DI DiagramElement element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public abstract class DiagramElementImpl : BpmnModelElementInstanceImpl, DiagramElement
	{
		public abstract org.camunda.bpm.model.bpmn.instance.Extension Extension {set;}

	  protected internal static Attribute<string> idAttribute;
	  protected internal static ChildElement<Extension> extensionChild;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(DiagramElement), DI_ELEMENT_DIAGRAM_ELEMENT).namespaceUri(DI_NS).abstractType();

		idAttribute = typeBuilder.stringAttribute(DI_ATTRIBUTE_ID).idAttribute().build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		extensionChild = sequenceBuilder.element(typeof(Extension)).build();

		typeBuilder.build();
	  }

	  public DiagramElementImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual string Id
	  {
		  get
		  {
			return idAttribute.getValue(this);
		  }
		  set
		  {
			idAttribute.setValue(this, value);
		  }
	  }


	  public virtual Extension Extension
	  {
		  get
		  {
			return extensionChild.getChild(this);
		  }
		  set
		  {
			extensionChild.setChild(this, value);
		  }
	  }

	}

}