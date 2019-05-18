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
	using StartEventBuilder = org.camunda.bpm.model.bpmn.builder.StartEventBuilder;
	using CatchEvent = org.camunda.bpm.model.bpmn.instance.CatchEvent;
	using StartEvent = org.camunda.bpm.model.bpmn.instance.StartEvent;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;

	/// <summary>
	/// The BPMN startEvent element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class StartEventImpl : CatchEventImpl, StartEvent
	{

	  protected internal static Attribute<bool> isInterruptingAttribute;

	  /// <summary>
	  /// camunda extensions </summary>

	  protected internal static Attribute<bool> camundaAsyncAttribute;
	  protected internal static Attribute<string> camundaFormHandlerClassAttribute;
	  protected internal static Attribute<string> camundaFormKeyAttribute;
	  protected internal static Attribute<string> camundaInitiatorAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {

		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(StartEvent), BPMN_ELEMENT_START_EVENT).namespaceUri(BPMN20_NS).extendsType(typeof(CatchEvent)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		isInterruptingAttribute = typeBuilder.booleanAttribute(BPMN_ATTRIBUTE_IS_INTERRUPTING).defaultValue(true).build();

		/// <summary>
		/// camunda extensions </summary>

		camundaAsyncAttribute = typeBuilder.booleanAttribute(CAMUNDA_ATTRIBUTE_ASYNC).@namespace(CAMUNDA_NS).defaultValue(false).build();

		camundaFormHandlerClassAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_FORM_HANDLER_CLASS).@namespace(CAMUNDA_NS).build();

		camundaFormKeyAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_FORM_KEY).@namespace(CAMUNDA_NS).build();

		camundaInitiatorAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_INITIATOR).@namespace(CAMUNDA_NS).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelElementTypeBuilder.ModelTypeInstanceProvider<StartEvent>
	  {
		  public StartEvent newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new StartEventImpl(instanceContext);
		  }
	  }

	  public StartEventImpl(ModelTypeInstanceContext context) : base(context)
	  {
	  }

	  public override StartEventBuilder builder()
	  {
		return new StartEventBuilder((BpmnModelInstance) modelInstance, this);
	  }

	  public virtual bool Interrupting
	  {
		  get
		  {
			return isInterruptingAttribute.getValue(this);
		  }
		  set
		  {
			isInterruptingAttribute.setValue(this, value);
		  }
	  }


	  /// <summary>
	  /// camunda extensions </summary>

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


	  public virtual string CamundaFormHandlerClass
	  {
		  get
		  {
			return camundaFormHandlerClassAttribute.getValue(this);
		  }
		  set
		  {
			camundaFormHandlerClassAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaFormKey
	  {
		  get
		  {
			return camundaFormKeyAttribute.getValue(this);
		  }
		  set
		  {
			camundaFormKeyAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaInitiator
	  {
		  get
		  {
			return camundaInitiatorAttribute.getValue(this);
		  }
		  set
		  {
			camundaInitiatorAttribute.setValue(this, value);
		  }
	  }

	}

}