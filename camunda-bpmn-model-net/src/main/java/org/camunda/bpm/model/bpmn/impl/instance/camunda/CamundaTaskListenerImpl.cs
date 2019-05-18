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

	using CamundaField = org.camunda.bpm.model.bpmn.instance.camunda.CamundaField;
	using CamundaScript = org.camunda.bpm.model.bpmn.instance.camunda.CamundaScript;
	using CamundaTaskListener = org.camunda.bpm.model.bpmn.instance.camunda.CamundaTaskListener;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using ChildElement = org.camunda.bpm.model.xml.type.child.ChildElement;
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_CLASS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_DELEGATE_EXPRESSION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_EVENT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_EXPRESSION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ELEMENT_TASK_LISTENER;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN taskListener camunda extension element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class CamundaTaskListenerImpl : BpmnModelElementInstanceImpl, CamundaTaskListener
	{

	  protected internal static Attribute<string> camundaEventAttribute;
	  protected internal static Attribute<string> camundaClassAttribute;
	  protected internal static Attribute<string> camundaExpressionAttribute;
	  protected internal static Attribute<string> camundaDelegateExpressionAttribute;
	  protected internal static ChildElementCollection<CamundaField> camundaFieldCollection;
	  protected internal static ChildElement<CamundaScript> camundaScriptChild;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(CamundaTaskListener), CAMUNDA_ELEMENT_TASK_LISTENER).namespaceUri(CAMUNDA_NS).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		camundaEventAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_EVENT).@namespace(CAMUNDA_NS).build();

		camundaClassAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_CLASS).@namespace(CAMUNDA_NS).build();

		camundaExpressionAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_EXPRESSION).@namespace(CAMUNDA_NS).build();

		camundaDelegateExpressionAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_DELEGATE_EXPRESSION).@namespace(CAMUNDA_NS).build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		camundaFieldCollection = sequenceBuilder.elementCollection(typeof(CamundaField)).build();

		camundaScriptChild = sequenceBuilder.element(typeof(CamundaScript)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<CamundaTaskListener>
	  {
		  public CamundaTaskListener newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new CamundaTaskListenerImpl(instanceContext);
		  }
	  }

	  public CamundaTaskListenerImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual string CamundaEvent
	  {
		  get
		  {
			return camundaEventAttribute.getValue(this);
		  }
		  set
		  {
			camundaEventAttribute.setValue(this, value);
		  }
	  }


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


	  public virtual ICollection<CamundaField> CamundaFields
	  {
		  get
		  {
			return camundaFieldCollection.get(this);
		  }
	  }

	  public virtual CamundaScript CamundaScript
	  {
		  get
		  {
			return camundaScriptChild.getChild(this);
		  }
		  set
		  {
			camundaScriptChild.setChild(this, value);
		  }
	  }


	}

}