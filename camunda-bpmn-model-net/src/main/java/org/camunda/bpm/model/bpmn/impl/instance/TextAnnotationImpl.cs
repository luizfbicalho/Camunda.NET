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
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ATTRIBUTE_TEXT_FORMAT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ELEMENT_TEXT_ANNOTATION;

	using Artifact = org.camunda.bpm.model.bpmn.instance.Artifact;
	using Script = org.camunda.bpm.model.bpmn.instance.Script;
	using Text = org.camunda.bpm.model.bpmn.instance.Text;
	using TextAnnotation = org.camunda.bpm.model.bpmn.instance.TextAnnotation;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using ModelTypeInstanceProvider = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using ChildElement = org.camunda.bpm.model.xml.type.child.ChildElement;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;

	/// <summary>
	/// The BPMN 2.0 textAnnotation element
	/// 
	/// @author Filip Hrisafov
	/// </summary>
	public class TextAnnotationImpl : ArtifactImpl, TextAnnotation
	{

	  protected internal static Attribute<string> textFormatAttribute;
	  protected internal static ChildElement<Text> textChild;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(TextAnnotation), BPMN_ELEMENT_TEXT_ANNOTATION).namespaceUri(BPMN20_NS).extendsType(typeof(Artifact)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		textFormatAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_TEXT_FORMAT).defaultValue("text/plain").build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		textChild = sequenceBuilder.element(typeof(Text)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelElementTypeBuilder.ModelTypeInstanceProvider<TextAnnotation>
	  {
		  public TextAnnotation newInstance(ModelTypeInstanceContext context)
		  {
			return new TextAnnotationImpl(context);
		  }
	  }

	  public TextAnnotationImpl(ModelTypeInstanceContext context) : base(context)
	  {
	  }

	  public virtual string TextFormat
	  {
		  get
		  {
			return textFormatAttribute.getValue(this);
		  }
		  set
		  {
			textFormatAttribute.setValue(this, value);
		  }
	  }


	  public virtual Text Text
	  {
		  get
		  {
			return textChild.getChild(this);
		  }
		  set
		  {
			textChild.setChild(this, value);
		  }
	  }

	}

}