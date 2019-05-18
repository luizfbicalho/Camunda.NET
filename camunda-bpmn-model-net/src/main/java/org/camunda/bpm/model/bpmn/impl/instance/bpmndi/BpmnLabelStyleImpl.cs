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
	using StyleImpl = org.camunda.bpm.model.bpmn.impl.instance.di.StyleImpl;
	using BpmnLabelStyle = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnLabelStyle;
	using Font = org.camunda.bpm.model.bpmn.instance.dc.Font;
	using Style = org.camunda.bpm.model.bpmn.instance.di.Style;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using ChildElement = org.camunda.bpm.model.xml.type.child.ChildElement;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMNDI_ELEMENT_BPMN_LABEL_STYLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMNDI_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMNDI BPMNLabelStyle element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class BpmnLabelStyleImpl : StyleImpl, BpmnLabelStyle
	{

	  protected internal static ChildElement<Font> fontChild;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(BpmnLabelStyle), BPMNDI_ELEMENT_BPMN_LABEL_STYLE).namespaceUri(BPMNDI_NS).extendsType(typeof(Style)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		fontChild = sequenceBuilder.element(typeof(Font)).required().build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<BpmnLabelStyle>
	  {
		  public BpmnLabelStyle newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new BpmnLabelStyleImpl(instanceContext);
		  }
	  }

	  public BpmnLabelStyleImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual Font Font
	  {
		  get
		  {
			return fontChild.getChild(this);
		  }
		  set
		  {
			fontChild.setChild(this, value);
		  }
	  }

	}

}