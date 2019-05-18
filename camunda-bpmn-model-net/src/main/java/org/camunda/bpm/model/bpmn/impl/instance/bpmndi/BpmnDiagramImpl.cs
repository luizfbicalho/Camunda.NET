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
namespace org.camunda.bpm.model.bpmn.impl.instance.bpmndi
{
	using DiagramImpl = org.camunda.bpm.model.bpmn.impl.instance.di.DiagramImpl;
	using BpmnDiagram = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnDiagram;
	using BpmnLabelStyle = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnLabelStyle;
	using BpmnPlane = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnPlane;
	using Diagram = org.camunda.bpm.model.bpmn.instance.di.Diagram;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using ChildElement = org.camunda.bpm.model.xml.type.child.ChildElement;
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMNDI_ELEMENT_BPMN_DIAGRAM;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMNDI_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMNDI BPMNDiagram element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class BpmnDiagramImpl : DiagramImpl, BpmnDiagram
	{

	  protected internal static ChildElement<BpmnPlane> bpmnPlaneChild;
	  protected internal static ChildElementCollection<BpmnLabelStyle> bpmnLabelStyleCollection;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(BpmnDiagram), BPMNDI_ELEMENT_BPMN_DIAGRAM).namespaceUri(BPMNDI_NS).extendsType(typeof(Diagram)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		bpmnPlaneChild = sequenceBuilder.element(typeof(BpmnPlane)).required().build();

		bpmnLabelStyleCollection = sequenceBuilder.elementCollection(typeof(BpmnLabelStyle)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<BpmnDiagram>
	  {
		  public BpmnDiagram newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new BpmnDiagramImpl(instanceContext);
		  }
	  }

	  public BpmnDiagramImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual BpmnPlane BpmnPlane
	  {
		  get
		  {
			return bpmnPlaneChild.getChild(this);
		  }
		  set
		  {
			bpmnPlaneChild.setChild(this, value);
		  }
	  }


	  public virtual ICollection<BpmnLabelStyle> BpmnLabelStyles
	  {
		  get
		  {
			return bpmnLabelStyleCollection.get(this);
		  }
	  }
	}

}