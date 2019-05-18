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
	using BaseElement = org.camunda.bpm.model.bpmn.instance.BaseElement;
	using ParticipantMultiplicity = org.camunda.bpm.model.bpmn.instance.ParticipantMultiplicity;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN participantMultiplicity element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class ParticipantMultiplicityImpl : BaseElementImpl, ParticipantMultiplicity
	{

	  protected internal static Attribute<int> minimumAttribute;
	  protected internal static Attribute<int> maximumAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(ParticipantMultiplicity), BPMN_ELEMENT_PARTICIPANT_MULTIPLICITY).namespaceUri(BPMN20_NS).extendsType(typeof(BaseElement)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		minimumAttribute = typeBuilder.integerAttribute(BPMN_ATTRIBUTE_MINIMUM).defaultValue(0).build();

		maximumAttribute = typeBuilder.integerAttribute(BPMN_ATTRIBUTE_MAXIMUM).defaultValue(1).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<ParticipantMultiplicity>
	  {
		  public ParticipantMultiplicity newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new ParticipantMultiplicityImpl(instanceContext);
		  }
	  }

	  public ParticipantMultiplicityImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual int Minimum
	  {
		  get
		  {
			return minimumAttribute.getValue(this);
		  }
		  set
		  {
			minimumAttribute.setValue(this, value);
		  }
	  }


	  public virtual int Maximum
	  {
		  get
		  {
			return maximumAttribute.getValue(this);
		  }
		  set
		  {
			maximumAttribute.setValue(this, value);
		  }
	  }

	}

}