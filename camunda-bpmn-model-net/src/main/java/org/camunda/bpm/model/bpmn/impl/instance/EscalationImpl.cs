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
	using ItemDefinition = org.camunda.bpm.model.bpmn.instance.ItemDefinition;
	using RootElement = org.camunda.bpm.model.bpmn.instance.RootElement;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using AttributeReference = org.camunda.bpm.model.xml.type.reference.AttributeReference;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN escalation element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class EscalationImpl : RootElementImpl, Escalation
	{

	  protected internal static Attribute<string> nameAttribute;
	  protected internal static Attribute<string> escalationCodeAttribute;
	  protected internal static AttributeReference<ItemDefinition> structureRefAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(Escalation), BPMN_ELEMENT_ESCALATION).namespaceUri(BPMN20_NS).extendsType(typeof(RootElement)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		nameAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_NAME).build();

		escalationCodeAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_ESCALATION_CODE).build();

		structureRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_STRUCTURE_REF).qNameAttributeReference(typeof(ItemDefinition)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<Escalation>
	  {
		  public Escalation newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new EscalationImpl(instanceContext);
		  }
	  }

	  public EscalationImpl(ModelTypeInstanceContext context) : base(context)
	  {
	  }

	  public virtual string Name
	  {
		  get
		  {
			return nameAttribute.getValue(this);
		  }
		  set
		  {
			nameAttribute.setValue(this, value);
		  }
	  }


	  public virtual string EscalationCode
	  {
		  get
		  {
			return escalationCodeAttribute.getValue(this);
		  }
		  set
		  {
			escalationCodeAttribute.setValue(this, value);
		  }
	  }


	  public virtual ItemDefinition Structure
	  {
		  get
		  {
			return structureRefAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			structureRefAttribute.setReferenceTargetElement(this, value);
		  }
	  }


	}

}