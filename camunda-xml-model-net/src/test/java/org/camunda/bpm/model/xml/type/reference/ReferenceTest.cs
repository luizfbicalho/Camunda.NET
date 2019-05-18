using System;
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
namespace org.camunda.bpm.model.xml.type.reference
{
	using AbstractModelParser = org.camunda.bpm.model.xml.impl.parser.AbstractModelParser;
	using AttributeReferenceImpl = org.camunda.bpm.model.xml.impl.type.reference.AttributeReferenceImpl;
	using QNameAttributeReferenceImpl = org.camunda.bpm.model.xml.impl.type.reference.QNameAttributeReferenceImpl;
	using Gender = org.camunda.bpm.model.xml.testmodel.Gender;
	using TestModelParser = org.camunda.bpm.model.xml.testmodel.TestModelParser;
	using TestModelTest = org.camunda.bpm.model.xml.testmodel.TestModelTest;
	using org.camunda.bpm.model.xml.testmodel.instance;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using Before = org.junit.Before;
	using Test = org.junit.Test;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.test.assertions.ModelAssertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.runners.Parameterized.Parameters;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class ReferenceTest : TestModelTest
	{

	  private Bird tweety;
	  private Bird daffy;
	  private Bird daisy;
	  private Bird plucky;
	  private Bird birdo;
	  private FlightPartnerRef flightPartnerRef;

	  private ModelElementType animalType;
	  private QNameAttributeReferenceImpl<Animal> fatherReference;
	  private AttributeReferenceImpl<Animal> motherReference;
	  private ElementReferenceCollection<FlyingAnimal, FlightPartnerRef> flightPartnerRefsColl;

	  public ReferenceTest(string testName, ModelInstance testModelInstance, AbstractModelParser modelParser) : base(testName, testModelInstance, modelParser)
	  {
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters(name="Model {0}") public static java.util.Collection<Object[]> models()
	  public static ICollection<object[]> models()
	  {
		object[][] models = new object[][] {createModel(), parseModel(typeof(ReferenceTest))};
		return Arrays.asList(models);
	  }

	  public static object[] createModel()
	  {
		TestModelParser modelParser = new TestModelParser();
		ModelInstance modelInstance = modelParser.EmptyModel;

		Animals animals = modelInstance.newInstance(typeof(Animals));
		modelInstance.DocumentElement = animals;

		Bird tweety = createBird(modelInstance, "tweety", Gender.Female);
		Bird daffy = createBird(modelInstance, "daffy", Gender.Male);
		Bird daisy = createBird(modelInstance, "daisy", Gender.Female);
		createBird(modelInstance, "plucky", Gender.Male);
		createBird(modelInstance, "birdo", Gender.Female);
		tweety.Father = daffy;
		tweety.Mother = daisy;

		tweety.FlightPartnerRefs.Add(daffy);

		return new object[]{"created", modelInstance, modelParser};
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before @SuppressWarnings("unchecked") public void copyModelInstance()
	  public virtual void copyModelInstance()
	  {
		modelInstance = cloneModelInstance();

		tweety = modelInstance.getModelElementById("tweety");
		daffy = modelInstance.getModelElementById("daffy");
		daisy = modelInstance.getModelElementById("daisy");
		plucky = modelInstance.getModelElementById("plucky");
		birdo = modelInstance.getModelElementById("birdo");

		animalType = modelInstance.Model.getType(typeof(Animal));

		// QName attribute reference
		fatherReference = (QNameAttributeReferenceImpl<Animal>) animalType.getAttribute("father").OutgoingReferences.GetEnumerator().next();

		// ID attribute reference
		motherReference = (AttributeReferenceImpl<Animal>) animalType.getAttribute("mother").OutgoingReferences.GetEnumerator().next();

		// ID element reference
		flightPartnerRefsColl = FlyingAnimal.flightPartnerRefsColl;

		ModelElementType flightPartnerRefType = modelInstance.Model.getType(typeof(FlightPartnerRef));
		flightPartnerRef = (FlightPartnerRef) modelInstance.getModelElementsByType(flightPartnerRefType).GetEnumerator().next();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReferenceIdentifier()
	  public virtual void testReferenceIdentifier()
	  {
		assertThat(fatherReference).hasIdentifier(tweety, daffy.Id);
		assertThat(motherReference).hasIdentifier(tweety, daisy.Id);
		assertThat(flightPartnerRefsColl).hasIdentifier(tweety, daffy.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReferenceTargetElement()
	  public virtual void testReferenceTargetElement()
	  {
		assertThat(fatherReference).hasTargetElement(tweety, daffy);
		assertThat(motherReference).hasTargetElement(tweety, daisy);
		assertThat(flightPartnerRefsColl).hasTargetElement(tweety, daffy);

		fatherReference.setReferenceTargetElement(tweety, plucky);
		motherReference.setReferenceTargetElement(tweety, birdo);
		flightPartnerRefsColl.setReferenceTargetElement(flightPartnerRef, daisy);

		assertThat(fatherReference).hasTargetElement(tweety, plucky);
		assertThat(motherReference).hasTargetElement(tweety, birdo);
		assertThat(flightPartnerRefsColl).hasTargetElement(tweety, daisy);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReferenceTargetAttribute()
	  public virtual void testReferenceTargetAttribute()
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.bpm.model.xml.type.attribute.Attribute<?> idAttribute = animalType.getAttribute("id");
		Attribute<object> idAttribute = animalType.getAttribute("id");
		assertThat(idAttribute).hasIncomingReferences(fatherReference, motherReference);

		assertThat(fatherReference).hasTargetAttribute(idAttribute);
		assertThat(motherReference).hasTargetAttribute(idAttribute);
		assertThat(flightPartnerRefsColl).hasTargetAttribute(idAttribute);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReferenceSourceAttribute()
	  public virtual void testReferenceSourceAttribute()
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.bpm.model.xml.type.attribute.Attribute<?> fatherAttribute = animalType.getAttribute("father");
		Attribute<object> fatherAttribute = animalType.getAttribute("father");
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.bpm.model.xml.type.attribute.Attribute<?> motherAttribute = animalType.getAttribute("mother");
		Attribute<object> motherAttribute = animalType.getAttribute("mother");

		assertThat(fatherReference).hasSourceAttribute(fatherAttribute);
		assertThat(motherReference).hasSourceAttribute(motherAttribute);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveReference()
	  public virtual void testRemoveReference()
	  {
		fatherReference.referencedElementRemoved(daffy, daffy.Id);

		assertThat(fatherReference).hasNoTargetElement(tweety);
		assertThat(tweety.Father).Null;

		motherReference.referencedElementRemoved(daisy, daisy.Id);
		assertThat(motherReference).hasNoTargetElement(tweety);
		assertThat(tweety.Mother).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTargetElementsCollection()
	  public virtual void testTargetElementsCollection()
	  {
		ICollection<FlyingAnimal> referenceTargetElements = flightPartnerRefsColl.getReferenceTargetElements(tweety);
		ICollection<FlyingAnimal> flightPartners = Arrays.asList(new FlyingAnimal[]{birdo, daffy, daisy, plucky});

		// directly test collection methods and not use the	appropriate assertion methods
		assertThat(referenceTargetElements.Count).isEqualTo(1);
		assertThat(referenceTargetElements.Count == 0).False;
		assertThat(referenceTargetElements.Contains(daffy)).True;
		assertThat(referenceTargetElements.ToArray()).isEqualTo(new object[]{daffy});
		assertThat(referenceTargetElements.toArray(new FlyingAnimal[1])).isEqualTo(new FlyingAnimal[]{daffy});

		assertThat(referenceTargetElements.Add(daisy)).True;
		assertThat(referenceTargetElements).hasSize(2).containsOnly(daffy, daisy);

		assertThat(referenceTargetElements.remove(daisy)).True;
		assertThat(referenceTargetElements).hasSize(1).containsOnly(daffy);

		assertThat(referenceTargetElements.addAll(flightPartners)).True;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
		assertThat(referenceTargetElements.containsAll(flightPartners)).True;
		assertThat(referenceTargetElements).hasSize(4).containsOnly(daffy, daisy, plucky, birdo);

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
		assertThat(referenceTargetElements.removeAll(flightPartners)).True;
		assertThat(referenceTargetElements).Empty;

		try
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'retainAll' method:
		  referenceTargetElements.retainAll(flightPartners);
		  fail("retainAll method is not implemented");
		}
		catch (Exception e)
		{
		  assertThat(e).isInstanceOf(typeof(UnsupportedModelOperationException));
		}

		referenceTargetElements.addAll(flightPartners);
		assertThat(referenceTargetElements).NotEmpty;
		referenceTargetElements.Clear();
		assertThat(referenceTargetElements).Empty;
	  }

	}

}