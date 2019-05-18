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
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ATTRIBUTE_IMPLEMENTATION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ELEMENT_BUSINESS_RULE_TASK;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_CLASS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_DECISION_REF;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_DECISION_REF_BINDING;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_DECISION_REF_VERSION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_DELEGATE_EXPRESSION;
	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_MAP_DECISION_RESULT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_RESULT_VARIABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_TOPIC;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_TYPE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_NS;

	using BusinessRuleTaskBuilder = org.camunda.bpm.model.bpmn.builder.BusinessRuleTaskBuilder;
	using BusinessRuleTask = org.camunda.bpm.model.bpmn.instance.BusinessRuleTask;
	using Rendering = org.camunda.bpm.model.bpmn.instance.Rendering;
	using Task = org.camunda.bpm.model.bpmn.instance.Task;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using ModelTypeInstanceProvider = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;

	/// <summary>
	/// The BPMN businessRuleTask element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class BusinessRuleTaskImpl : TaskImpl, BusinessRuleTask
	{

	  protected internal static Attribute<string> implementationAttribute;
	  protected internal static ChildElementCollection<Rendering> renderingCollection;

	  /// <summary>
	  /// camunda extensions </summary>

	  protected internal static Attribute<string> camundaClassAttribute;
	  protected internal static Attribute<string> camundaDelegateExpressionAttribute;
	  protected internal static Attribute<string> camundaExpressionAttribute;
	  protected internal static Attribute<string> camundaResultVariableAttribute;
	  protected internal static Attribute<string> camundaTopicAttribute;
	  protected internal static Attribute<string> camundaTypeAttribute;
	  protected internal static Attribute<string> camundaDecisionRefAttribute;
	  protected internal static Attribute<string> camundaDecisionRefBindingAttribute;
	  protected internal static Attribute<string> camundaDecisionRefVersionAttribute;
	  protected internal static Attribute<string> camundaDecisionRefVersionTagAttribute;
	  protected internal static Attribute<string> camundaDecisionRefTenantIdAttribute;
	  protected internal static Attribute<string> camundaMapDecisionResultAttribute;
	  protected internal static Attribute<string> camundaTaskPriorityAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(BusinessRuleTask), BPMN_ELEMENT_BUSINESS_RULE_TASK).namespaceUri(BPMN20_NS).extendsType(typeof(Task)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		implementationAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_IMPLEMENTATION).defaultValue("##unspecified").build();

		/// <summary>
		/// camunda extensions </summary>

		camundaClassAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_CLASS).@namespace(CAMUNDA_NS).build();

		camundaDelegateExpressionAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_DELEGATE_EXPRESSION).@namespace(CAMUNDA_NS).build();

		camundaExpressionAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_EXPRESSION).@namespace(CAMUNDA_NS).build();

		camundaResultVariableAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_RESULT_VARIABLE).@namespace(CAMUNDA_NS).build();

		camundaTopicAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_TOPIC).@namespace(CAMUNDA_NS).build();

		camundaTypeAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_TYPE).@namespace(CAMUNDA_NS).build();

		camundaDecisionRefAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_DECISION_REF).@namespace(CAMUNDA_NS).build();

		camundaDecisionRefBindingAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_DECISION_REF_BINDING).@namespace(CAMUNDA_NS).build();

		camundaDecisionRefVersionAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_DECISION_REF_VERSION).@namespace(CAMUNDA_NS).build();

		camundaDecisionRefVersionTagAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_DECISION_REF_VERSION_TAG).@namespace(CAMUNDA_NS).build();

		camundaDecisionRefTenantIdAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_DECISION_REF_TENANT_ID).@namespace(CAMUNDA_NS).build();

		camundaMapDecisionResultAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_MAP_DECISION_RESULT).@namespace(CAMUNDA_NS).build();

		camundaTaskPriorityAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_TASK_PRIORITY).@namespace(CAMUNDA_NS).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelElementTypeBuilder.ModelTypeInstanceProvider<BusinessRuleTask>
	  {
		  public BusinessRuleTask newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new BusinessRuleTaskImpl(instanceContext);
		  }
	  }

	  public BusinessRuleTaskImpl(ModelTypeInstanceContext context) : base(context)
	  {
	  }

	  public override BusinessRuleTaskBuilder builder()
	  {
		return new BusinessRuleTaskBuilder((BpmnModelInstance) modelInstance, this);
	  }

	  public virtual string Implementation
	  {
		  get
		  {
			return implementationAttribute.getValue(this);
		  }
		  set
		  {
			implementationAttribute.setValue(this, value);
		  }
	  }


	  /// <summary>
	  /// camunda extensions </summary>

	  public virtual string CamundaClass
	  {
		  get
		  {
			return camundaClassAttribute.getValue(this);
		  }
		  set
		  {
			camundaClassAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaDelegateExpression
	  {
		  get
		  {
			return camundaDelegateExpressionAttribute.getValue(this);
		  }
		  set
		  {
			camundaDelegateExpressionAttribute.setValue(this, value);
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


	  public virtual string CamundaResultVariable
	  {
		  get
		  {
			return camundaResultVariableAttribute.getValue(this);
		  }
		  set
		  {
			camundaResultVariableAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaTopic
	  {
		  get
		  {
			return camundaTopicAttribute.getValue(this);
		  }
		  set
		  {
			camundaTopicAttribute.setValue(this, value);
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


	  public virtual string CamundaDecisionRef
	  {
		  get
		  {
			return camundaDecisionRefAttribute.getValue(this);
		  }
		  set
		  {
			camundaDecisionRefAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaDecisionRefBinding
	  {
		  get
		  {
			return camundaDecisionRefBindingAttribute.getValue(this);
		  }
		  set
		  {
			camundaDecisionRefBindingAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaDecisionRefVersion
	  {
		  get
		  {
			return camundaDecisionRefVersionAttribute.getValue(this);
		  }
		  set
		  {
			camundaDecisionRefVersionAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaDecisionRefVersionTag
	  {
		  get
		  {
			return camundaDecisionRefVersionTagAttribute.getValue(this);
		  }
		  set
		  {
			camundaDecisionRefVersionTagAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaMapDecisionResult
	  {
		  get
		  {
			return camundaMapDecisionResultAttribute.getValue(this);
		  }
		  set
		  {
			camundaMapDecisionResultAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaDecisionRefTenantId
	  {
		  get
		  {
			return camundaDecisionRefTenantIdAttribute.getValue(this);
		  }
		  set
		  {
			camundaDecisionRefTenantIdAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaTaskPriority
	  {
		  get
		  {
			return camundaTaskPriorityAttribute.getValue(this);
		  }
		  set
		  {
			camundaTaskPriorityAttribute.setValue(this, value);
		  }
	  }

	}

}