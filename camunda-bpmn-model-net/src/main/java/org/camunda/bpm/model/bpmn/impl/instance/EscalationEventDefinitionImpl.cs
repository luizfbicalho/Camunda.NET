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
	using Escalation = org.camunda.bpm.model.bpmn.instance.Escalation;
	using EscalationEventDefinition = org.camunda.bpm.model.bpmn.instance.EscalationEventDefinition;
	using EventDefinition = org.camunda.bpm.model.bpmn.instance.EventDefinition;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using AttributeReference = org.camunda.bpm.model.xml.type.reference.AttributeReference;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN escalationEventDefinition element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class EscalationEventDefinitionImpl : EventDefinitionImpl, EscalationEventDefinition
	{

	  protected internal static AttributeReference<Escalation> escalationRefAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(EscalationEventDefinition), BPMN_ELEMENT_ESCALATION_EVENT_DEFINITION).namespaceUri(BPMN20_NS).extendsType(typeof(EventDefinition)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		escalationRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_ESCALATION_REF).qNameAttributeReference(typeof(Escalation)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<EscalationEventDefinition>
	  {
		  public EscalationEventDefinition newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new EscalationEventDefinitionImpl(instanceContext);
		  }
	  }

	  public EscalationEventDefinitionImpl(ModelTypeInstanceContext context) : base(context)
	  {
	  }

	  public virtual Escalation Escalation
	  {
		  get
		  {
			return escalationRefAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			escalationRefAttribute.setReferenceTargetElement(this, value);
		  }
	  }


	}

}