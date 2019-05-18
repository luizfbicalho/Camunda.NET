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
	using PlaneImpl = org.camunda.bpm.model.bpmn.impl.instance.di.PlaneImpl;
	using BaseElement = org.camunda.bpm.model.bpmn.instance.BaseElement;
	using BpmnPlane = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnPlane;
	using Plane = org.camunda.bpm.model.bpmn.instance.di.Plane;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using AttributeReference = org.camunda.bpm.model.xml.type.reference.AttributeReference;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMNDI_ATTRIBUTE_BPMN_ELEMENT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMNDI_ELEMENT_BPMN_PLANE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMNDI_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMNDI BPMNPlane element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class BpmnPlaneImpl : PlaneImpl, BpmnPlane
	{

	  protected internal static AttributeReference<BaseElement> bpmnElementAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(BpmnPlane), BPMNDI_ELEMENT_BPMN_PLANE).namespaceUri(BPMNDI_NS).extendsType(typeof(Plane)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		bpmnElementAttribute = typeBuilder.stringAttribute(BPMNDI_ATTRIBUTE_BPMN_ELEMENT).qNameAttributeReference(typeof(BaseElement)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<BpmnPlane>
	  {
		  public BpmnPlane newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new BpmnPlaneImpl(instanceContext);
		  }
	  }

	  public BpmnPlaneImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual BaseElement BpmnElement
	  {
		  get
		  {
			return bpmnElementAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			bpmnElementAttribute.setReferenceTargetElement(this, value);
		  }
	  }

	}

}