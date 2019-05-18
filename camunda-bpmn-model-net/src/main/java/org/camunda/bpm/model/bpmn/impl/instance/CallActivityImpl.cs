using System;

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
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ATTRIBUTE_CALLED_ELEMENT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ELEMENT_CALL_ACTIVITY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_ASYNC;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_CALLED_ELEMENT_BINDING;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_CALLED_ELEMENT_TENANT_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_CALLED_ELEMENT_VERSION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_CALLED_ELEMENT_VERSION_TAG;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_CASE_BINDING;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_CASE_REF;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_CASE_TENANT_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_CASE_VERSION;

	using CallActivityBuilder = org.camunda.bpm.model.bpmn.builder.CallActivityBuilder;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_VARIABLE_MAPPING_CLASS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_VARIABLE_MAPPING_DELEGATE_EXPRESSION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_NS;
	using Activity = org.camunda.bpm.model.bpmn.instance.Activity;
	using CallActivity = org.camunda.bpm.model.bpmn.instance.CallActivity;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using ModelTypeInstanceProvider = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;

	/// <summary>
	/// The BPMN callActivity element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class CallActivityImpl : ActivityImpl, CallActivity
	{

	  protected internal static Attribute<string> calledElementAttribute;


	  /// <summary>
	  /// camunda extensions </summary>

	  protected internal static Attribute<bool> camundaAsyncAttribute;
	  protected internal static Attribute<string> camundaCalledElementBindingAttribute;
	  protected internal static Attribute<string> camundaCalledElementVersionAttribute;
	  protected internal static Attribute<string> camundaCalledElementVersionTagAttribute;
	  protected internal static Attribute<string> camundaCalledElementTenantIdAttribute;

	  protected internal static Attribute<string> camundaCaseRefAttribute;
	  protected internal static Attribute<string> camundaCaseBindingAttribute;
	  protected internal static Attribute<string> camundaCaseVersionAttribute;
	  protected internal static Attribute<string> camundaCaseTenantIdAttribute;
	  protected internal static Attribute<string> camundaVariableMappingClassAttribute;
	  protected internal static Attribute<string> camundaVariableMappingDelegateExpressionAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(CallActivity), BPMN_ELEMENT_CALL_ACTIVITY).namespaceUri(BPMN20_NS).extendsType(typeof(Activity)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		calledElementAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_CALLED_ELEMENT).build();

		/// <summary>
		/// camunda extensions </summary>

		camundaAsyncAttribute = typeBuilder.booleanAttribute(CAMUNDA_ATTRIBUTE_ASYNC).@namespace(CAMUNDA_NS).defaultValue(false).build();

		camundaCalledElementBindingAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_CALLED_ELEMENT_BINDING).@namespace(CAMUNDA_NS).build();

		camundaCalledElementVersionAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_CALLED_ELEMENT_VERSION).@namespace(CAMUNDA_NS).build();

		camundaCalledElementVersionTagAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_CALLED_ELEMENT_VERSION_TAG).@namespace(CAMUNDA_NS).build();

		camundaCaseRefAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_CASE_REF).@namespace(CAMUNDA_NS).build();

		camundaCaseBindingAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_CASE_BINDING).@namespace(CAMUNDA_NS).build();

		camundaCaseVersionAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_CASE_VERSION).@namespace(CAMUNDA_NS).build();

		camundaCalledElementTenantIdAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_CALLED_ELEMENT_TENANT_ID).@namespace(CAMUNDA_NS).build();

		camundaCaseTenantIdAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_CASE_TENANT_ID).@namespace(CAMUNDA_NS).build();

		camundaVariableMappingClassAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_VARIABLE_MAPPING_CLASS).@namespace(CAMUNDA_NS).build();

		camundaVariableMappingDelegateExpressionAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_VARIABLE_MAPPING_DELEGATE_EXPRESSION).@namespace(CAMUNDA_NS).build();


		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelElementTypeBuilder.ModelTypeInstanceProvider<CallActivity>
	  {
		  public CallActivity newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new CallActivityImpl(instanceContext);
		  }
	  }

	  public CallActivityImpl(ModelTypeInstanceContext context) : base(context)
	  {
	  }

	  public override CallActivityBuilder builder()
	  {
		return new CallActivityBuilder((BpmnModelInstance) modelInstance, this);
	  }

	  public virtual string CalledElement
	  {
		  get
		  {
			return calledElementAttribute.getValue(this);
		  }
		  set
		  {
			calledElementAttribute.setValue(this, value);
		  }
	  }


	  /// @deprecated use isCamundaAsyncBefore() instead. 
	  [Obsolete("use isCamundaAsyncBefore() instead.")]
	  public virtual bool CamundaAsync
	  {
		  get
		  {
			return camundaAsyncAttribute.getValue(this);
		  }
		  set
		  {
			camundaAsyncAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaCalledElementBinding
	  {
		  get
		  {
			return camundaCalledElementBindingAttribute.getValue(this);
		  }
		  set
		  {
			camundaCalledElementBindingAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaCalledElementVersion
	  {
		  get
		  {
			return camundaCalledElementVersionAttribute.getValue(this);
		  }
		  set
		  {
			camundaCalledElementVersionAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaCalledElementVersionTag
	  {
		  get
		  {
			return camundaCalledElementVersionTagAttribute.getValue(this);
		  }
		  set
		  {
			camundaCalledElementVersionTagAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaCaseRef
	  {
		  get
		  {
			return camundaCaseRefAttribute.getValue(this);
		  }
		  set
		  {
			camundaCaseRefAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaCaseBinding
	  {
		  get
		  {
			return camundaCaseBindingAttribute.getValue(this);
		  }
		  set
		  {
			camundaCaseBindingAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaCaseVersion
	  {
		  get
		  {
			return camundaCaseVersionAttribute.getValue(this);
		  }
		  set
		  {
			camundaCaseVersionAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaCalledElementTenantId
	  {
		  get
		  {
			return camundaCalledElementTenantIdAttribute.getValue(this);
		  }
		  set
		  {
			camundaCalledElementTenantIdAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaCaseTenantId
	  {
		  get
		  {
			return camundaCaseTenantIdAttribute.getValue(this);
		  }
		  set
		  {
			camundaCaseTenantIdAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaVariableMappingClass
	  {
		  get
		  {
			return camundaVariableMappingClassAttribute.getValue(this);
		  }
		  set
		  {
			camundaVariableMappingClassAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaVariableMappingDelegateExpression
	  {
		  get
		  {
			return camundaVariableMappingDelegateExpressionAttribute.getValue(this);
		  }
		  set
		  {
			camundaVariableMappingDelegateExpressionAttribute.setValue(this, value);
		  }
	  }

	}

}