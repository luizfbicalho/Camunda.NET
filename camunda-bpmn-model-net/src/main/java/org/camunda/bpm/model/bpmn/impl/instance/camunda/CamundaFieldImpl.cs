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
namespace org.camunda.bpm.model.bpmn.impl.instance.camunda
{
	using CamundaExpression = org.camunda.bpm.model.bpmn.instance.camunda.CamundaExpression;
	using CamundaField = org.camunda.bpm.model.bpmn.instance.camunda.CamundaField;
	using CamundaString = org.camunda.bpm.model.bpmn.instance.camunda.CamundaString;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using ChildElement = org.camunda.bpm.model.xml.type.child.ChildElement;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN field camunda extension element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class CamundaFieldImpl : BpmnModelElementInstanceImpl, CamundaField
	{

	  protected internal static Attribute<string> camundaNameAttribute;
	  protected internal static Attribute<string> camundaExpressionAttribute;
	  protected internal static Attribute<string> camundaStringValueAttribute;
	  protected internal static ChildElement<CamundaExpression> camundaExpressionChild;
	  protected internal static ChildElement<CamundaString> camundaStringChild;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(CamundaField), CAMUNDA_ELEMENT_FIELD).namespaceUri(CAMUNDA_NS).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		camundaNameAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_NAME).@namespace(CAMUNDA_NS).build();

		camundaExpressionAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_EXPRESSION).@namespace(CAMUNDA_NS).build();

		camundaStringValueAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_STRING_VALUE).@namespace(CAMUNDA_NS).build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		camundaExpressionChild = sequenceBuilder.element(typeof(CamundaExpression)).build();

		camundaStringChild = sequenceBuilder.element(typeof(CamundaString)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<CamundaField>
	  {
		  public CamundaField newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new CamundaFieldImpl(instanceContext);
		  }
	  }

	  public CamundaFieldImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual string CamundaName
	  {
		  get
		  {
			return camundaNameAttribute.getValue(this);
		  }
		  set
		  {
			camundaNameAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaExpression
	  {
		  get
		  {
			return camundaExpressionAttribute.getValue(this);
		  }
		  set
		  {
			camundaExpressionAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaStringValue
	  {
		  get
		  {
			return camundaStringValueAttribute.getValue(this);
		  }
		  set
		  {
			camundaStringValueAttribute.setValue(this, value);
		  }
	  }


	  public virtual CamundaString CamundaString
	  {
		  get
		  {
			return camundaStringChild.getChild(this);
		  }
		  set
		  {
			camundaStringChild.setChild(this, value);
		  }
	  }


	  public virtual CamundaExpression CamundaExpressionChild
	  {
		  get
		  {
			return camundaExpressionChild.getChild(this);
		  }
		  set
		  {
			camundaExpressionChild.setChild(this, value);
		  }
	  }

	}

}