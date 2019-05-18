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
namespace org.camunda.bpm.model.xml.testmodel.instance
{
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;
	using ElementReference = org.camunda.bpm.model.xml.type.reference.ElementReference;
	using ElementReferenceCollection = org.camunda.bpm.model.xml.type.reference.ElementReferenceCollection;

	using static org.camunda.bpm.model.xml.testmodel.TestModelConstants;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public abstract class FlyingAnimal : Animal
	{

	  // only public for testing (normally private)
	  public static ElementReference<FlyingAnimal, FlightInstructor> flightInstructorChild;
	  public static ElementReferenceCollection<FlyingAnimal, FlightPartnerRef> flightPartnerRefsColl;
	  public static Attribute<double> wingspanAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {

		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(FlyingAnimal), TYPE_NAME_FLYING_ANIMAL).namespaceUri(MODEL_NAMESPACE).extendsType(typeof(Animal)).abstractType();

		wingspanAttribute = typeBuilder.doubleAttribute(ATTRIBUTE_NAME_WINGSPAN).build();

		SequenceBuilder sequence = typeBuilder.sequence();

		flightInstructorChild = sequence.element(typeof(FlightInstructor)).idElementReference(typeof(FlyingAnimal)).build();

		flightPartnerRefsColl = sequence.elementCollection(typeof(FlightPartnerRef)).idElementReferenceCollection(typeof(FlyingAnimal)).build();

		typeBuilder.build();

	  }

	  internal FlyingAnimal(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual double? getWingspan()
	  {
		return wingspanAttribute.getValue(this);
	  }

	  public virtual void setWingspan(double wingspan)
	  {
		wingspanAttribute.setValue(this, wingspan);
	  }

	  public virtual FlyingAnimal FlightInstructor
	  {
		  get
		  {
			return flightInstructorChild.getReferenceTargetElement(this);
		  }
		  set
		  {
			flightInstructorChild.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual void removeFlightInstructor()
	  {
		flightInstructorChild.clearReferenceTargetElement(this);
	  }

	  public virtual ICollection<FlyingAnimal> FlightPartnerRefs
	  {
		  get
		  {
			return flightPartnerRefsColl.getReferenceTargetElements(this);
		  }
	  }

	  public virtual ICollection<FlightPartnerRef> FlightPartnerRefElements
	  {
		  get
		  {
			return flightPartnerRefsColl.ReferenceSourceCollection.get(this);
		  }
	  }
	}

}