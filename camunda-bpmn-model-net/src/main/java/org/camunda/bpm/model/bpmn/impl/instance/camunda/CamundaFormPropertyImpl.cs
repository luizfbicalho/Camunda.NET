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
	using CamundaFormProperty = org.camunda.bpm.model.bpmn.instance.camunda.CamundaFormProperty;
	using CamundaValue = org.camunda.bpm.model.bpmn.instance.camunda.CamundaValue;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN formProperty camunda extension element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class CamundaFormPropertyImpl : BpmnModelElementInstanceImpl, CamundaFormProperty
	{

	  protected internal static Attribute<string> camundaIdAttribute;
	  protected internal static Attribute<string> camundaNameAttribute;
	  protected internal static Attribute<string> camundaTypeAttribute;
	  protected internal static Attribute<bool> camundaRequiredAttribute;
	  protected internal static Attribute<bool> camundaReadableAttribute;
	  protected internal static Attribute<bool> camundaWriteableAttribute;
	  protected internal static Attribute<string> camundaVariableAttribute;
	  protected internal static Attribute<string> camundaExpressionAttribute;
	  protected internal static Attribute<string> camundaDatePatternAttribute;
	  protected internal static Attribute<string> camundaDefaultAttribute;
	  protected internal static ChildElementCollection<CamundaValue> camundaValueCollection;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(CamundaFormProperty), CAMUNDA_ELEMENT_FORM_PROPERTY).namespaceUri(CAMUNDA_NS).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		camundaIdAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_ID).@namespace(CAMUNDA_NS).build();

		camundaNameAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_NAME).@namespace(CAMUNDA_NS).build();

		camundaTypeAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_TYPE).@namespace(CAMUNDA_NS).build();

		camundaRequiredAttribute = typeBuilder.booleanAttribute(CAMUNDA_ATTRIBUTE_REQUIRED).@namespace(CAMUNDA_NS).defaultValue(false).build();

		camundaReadableAttribute = typeBuilder.booleanAttribute(CAMUNDA_ATTRIBUTE_READABLE).@namespace(CAMUNDA_NS).defaultValue(true).build();

		camundaWriteableAttribute = typeBuilder.booleanAttribute(CAMUNDA_ATTRIBUTE_WRITEABLE).@namespace(CAMUNDA_NS).defaultValue(true).build();

		camundaVariableAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_VARIABLE).@namespace(CAMUNDA_NS).build();

		camundaExpressionAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_EXPRESSION).@namespace(CAMUNDA_NS).build();

		camundaDatePatternAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_DATE_PATTERN).@namespace(CAMUNDA_NS).build();

		camundaDefaultAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_DEFAULT).@namespace(CAMUNDA_NS).build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		camundaValueCollection = sequenceBuilder.elementCollection(typeof(CamundaValue)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<CamundaFormProperty>
	  {
		  public CamundaFormProperty newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new CamundaFormPropertyImpl(instanceContext);
		  }
	  }

	  public CamundaFormPropertyImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
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


	  public virtual bool CamundaRequired
	  {
		  get
		  {
			return camundaRequiredAttribute.getValue(this);
		  }
		  set
		  {
			camundaRequiredAttribute.setValue(this, value);
		  }
	  }


	  public virtual bool CamundaReadable
	  {
		  get
		  {
			return camundaReadableAttribute.getValue(this);
		  }
		  set
		  {
			camundaReadableAttribute.setValue(this, value);
		  }
	  }


	  public virtual bool CamundaWriteable
	  {
		  get
		  {
			return camundaWriteableAttribute.getValue(this);
		  }
		  set
		  {
			camundaWriteableAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaVariable
	  {
		  get
		  {
			return camundaVariableAttribute.getValue(this);
		  }
		  set
		  {
			camundaVariableAttribute.setValue(this, value);
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


	  public virtual string CamundaDefault
	  {
		  get
		  {
			return camundaDefaultAttribute.getValue(this);
		  }
		  set
		  {
			camundaDefaultAttribute.setValue(this, value);
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