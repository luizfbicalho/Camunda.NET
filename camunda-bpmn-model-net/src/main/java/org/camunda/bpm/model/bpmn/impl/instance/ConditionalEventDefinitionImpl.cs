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
namespace org.camunda.bpm.model.bpmn.impl.instance
{
	using Condition = org.camunda.bpm.model.bpmn.instance.Condition;
	using ConditionalEventDefinition = org.camunda.bpm.model.bpmn.instance.ConditionalEventDefinition;
	using EventDefinition = org.camunda.bpm.model.bpmn.instance.EventDefinition;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using ChildElement = org.camunda.bpm.model.xml.type.child.ChildElement;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN20_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ELEMENT_CONDITIONAL_EVENT_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_VARIABLE_NAME;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_NS;
	using StringUtil = org.camunda.bpm.model.xml.impl.util.StringUtil;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_VARIABLE_EVENTS;

	/// <summary>
	/// The BPMN conditionalEventDefinition element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class ConditionalEventDefinitionImpl : EventDefinitionImpl, ConditionalEventDefinition
	{

	  protected internal static ChildElement<Condition> conditionChild;
	  protected internal static Attribute<string> camundaVariableName;
	  protected internal static Attribute<string> camundaVariableEvents;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(ConditionalEventDefinition), BPMN_ELEMENT_CONDITIONAL_EVENT_DEFINITION).namespaceUri(BPMN20_NS).extendsType(typeof(EventDefinition)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		conditionChild = sequenceBuilder.element(typeof(Condition)).required().build();

		/// <summary>
		/// camunda extensions </summary>

		camundaVariableName = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_VARIABLE_NAME).@namespace(CAMUNDA_NS).build();

		camundaVariableEvents = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_VARIABLE_EVENTS).@namespace(CAMUNDA_NS).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<ConditionalEventDefinition>
	  {

		  public override ConditionalEventDefinition newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new ConditionalEventDefinitionImpl(instanceContext);
		  }
	  }

	  public ConditionalEventDefinitionImpl(ModelTypeInstanceContext context) : base(context)
	  {
	  }

	  public virtual Condition Condition
	  {
		  get
		  {
			return conditionChild.getChild(this);
		  }
		  set
		  {
			conditionChild.setChild(this, value);
		  }
	  }


	  public virtual string CamundaVariableName
	  {
		  get
		  {
			return camundaVariableName.getValue(this);
		  }
		  set
		  {
			camundaVariableName.setValue(this, value);
		  }
	  }


	  public virtual string CamundaVariableEvents
	  {
		  get
		  {
			return camundaVariableEvents.getValue(this);
		  }
		  set
		  {
			camundaVariableEvents.setValue(this, value);
		  }
	  }


	  public virtual IList<string> CamundaVariableEventsList
	  {
		  get
		  {
			string variableEvents = camundaVariableEvents.getValue(this);
			return StringUtil.splitCommaSeparatedList(variableEvents);
		  }
		  set
		  {
			string variableEvents = StringUtil.joinCommaSeparatedList(value);
			camundaVariableEvents.setValue(this, variableEvents);
		  }
	  }

	}

}