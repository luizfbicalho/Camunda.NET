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
	using Error = org.camunda.bpm.model.bpmn.instance.Error;
	using ErrorEventDefinition = org.camunda.bpm.model.bpmn.instance.ErrorEventDefinition;
	using EventDefinition = org.camunda.bpm.model.bpmn.instance.EventDefinition;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using AttributeReference = org.camunda.bpm.model.xml.type.reference.AttributeReference;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN20_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ATTRIBUTE_ERROR_REF;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ELEMENT_ERROR_EVENT_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_ERROR_CODE_VARIABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_ERROR_MESSAGE_VARIABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN errorEventDefinition element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class ErrorEventDefinitionImpl : EventDefinitionImpl, ErrorEventDefinition
	{

	  protected internal static AttributeReference<Error> errorRefAttribute;

	  protected internal static Attribute<string> camundaErrorCodeVariableAttribute;

	  protected internal static Attribute<string> camundaErrorMessageVariableAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(ErrorEventDefinition), BPMN_ELEMENT_ERROR_EVENT_DEFINITION).namespaceUri(BPMN20_NS).extendsType(typeof(EventDefinition)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		errorRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_ERROR_REF).qNameAttributeReference(typeof(Error)).build();

		camundaErrorCodeVariableAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_ERROR_CODE_VARIABLE).@namespace(CAMUNDA_NS).build();

		camundaErrorMessageVariableAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_ERROR_MESSAGE_VARIABLE).@namespace(CAMUNDA_NS).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<ErrorEventDefinition>
	  {
		  public ErrorEventDefinition newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new ErrorEventDefinitionImpl(instanceContext);
		  }
	  }

	  public ErrorEventDefinitionImpl(ModelTypeInstanceContext context) : base(context)
	  {
	  }

	  public virtual Error Error
	  {
		  get
		  {
			return errorRefAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			errorRefAttribute.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual string CamundaErrorCodeVariable
	  {
		  set
		  {
			camundaErrorCodeVariableAttribute.setValue(this, value);
		  }
		  get
		  {
			return camundaErrorCodeVariableAttribute.getValue(this);
		  }
	  }


	  public virtual string CamundaErrorMessageVariable
	  {
		  set
		  {
			camundaErrorMessageVariableAttribute.setValue(this, value);
		  }
		  get
		  {
			return camundaErrorMessageVariableAttribute.getValue(this);
		  }
	  }

	}

}