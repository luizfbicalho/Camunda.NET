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
namespace org.camunda.bpm.model.bpmn.impl.instance.camunda
{
	using CamundaFormField = org.camunda.bpm.model.bpmn.instance.camunda.CamundaFormField;
	using CamundaProperties = org.camunda.bpm.model.bpmn.instance.camunda.CamundaProperties;
	using CamundaValidation = org.camunda.bpm.model.bpmn.instance.camunda.CamundaValidation;
	using CamundaValue = org.camunda.bpm.model.bpmn.instance.camunda.CamundaValue;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using ChildElement = org.camunda.bpm.model.xml.type.child.ChildElement;
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN formField camunda extension element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class CamundaFormFieldImpl : BpmnModelElementInstanceImpl, CamundaFormField
	{

	  protected internal static Attribute<string> camundaIdAttribute;
	  protected internal static Attribute<string> camundaLabelAttribute;
	  protected internal static Attribute<string> camundaTypeAttribute;
	  protected internal static Attribute<string> camundaDatePatternAttribute;
	  protected internal static Attribute<string> camundaDefaultValueAttribute;
	  protected internal static ChildElement<CamundaProperties> camundaPropertiesChild;
	  protected internal static ChildElement<CamundaValidation> camundaValidationChild;
	  protected internal static ChildElementCollection<CamundaValue> camundaValueCollection;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(CamundaFormField), CAMUNDA_ELEMENT_FORM_FIELD).namespaceUri(CAMUNDA_NS).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		camundaIdAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_ID).@namespace(CAMUNDA_NS).build();

		camundaLabelAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_LABEL).@namespace(CAMUNDA_NS).build();

		camundaTypeAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_TYPE).@namespace(CAMUNDA_NS).build();

		camundaDatePatternAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_DATE_PATTERN).@namespace(CAMUNDA_NS).build();

		camundaDefaultValueAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_DEFAULT_VALUE).@namespace(CAMUNDA_NS).build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		camundaPropertiesChild = sequenceBuilder.element(typeof(CamundaProperties)).build();

		camundaValidationChild = sequenceBuilder.element(typeof(CamundaValidation)).build();

		camundaValueCollection = sequenceBuilder.elementCollection(typeof(CamundaValue)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<CamundaFormField>
	  {
		  public CamundaFormField newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new CamundaFormFieldImpl(instanceContext);
		  }
	  }

	  public CamundaFormFieldImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual string CamundaId
	  {
		  get
		  {
			return camundaIdAttribute.getValue(this);
		  }
		  set
		  {
			camundaIdAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaLabel
	  {
		  get
		  {
			return camundaLabelAttribute.getValue(this);
		  }
		  set
		  {
			camundaLabelAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaType
	  {
		  get
		  {
			return camundaTypeAttribute.getValue(this);
		  }
		  set
		  {
			camundaTypeAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaDatePattern
	  {
		  get
		  {
			return camundaDatePatternAttribute.getValue(this);
		  }
		  set
		  {
			camundaDatePatternAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaDefaultValue
	  {
		  get
		  {
			return camundaDefaultValueAttribute.getValue(this);
		  }
		  set
		  {
			camundaDefaultValueAttribute.setValue(this, value);
		  }
	  }


	  public virtual CamundaProperties CamundaProperties
	  {
		  get
		  {
			return camundaPropertiesChild.getChild(this);
		  }
		  set
		  {
			camundaPropertiesChild.setChild(this, value);
		  }
	  }


	  public virtual CamundaValidation CamundaValidation
	  {
		  get
		  {
			return camundaValidationChild.getChild(this);
		  }
		  set
		  {
			camundaValidationChild.setChild(this, value);
		  }
	  }


	  public virtual ICollection<CamundaValue> CamundaValues
	  {
		  get
		  {
			return camundaValueCollection.get(this);
		  }
	  }
	}

}