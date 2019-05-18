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
namespace org.camunda.bpm.model.xml.type.child
{
	using AbstractModelParser = org.camunda.bpm.model.xml.impl.parser.AbstractModelParser;
	using ChildElementCollectionImpl = org.camunda.bpm.model.xml.impl.type.child.ChildElementCollectionImpl;
	using ChildElementImpl = org.camunda.bpm.model.xml.impl.type.child.ChildElementImpl;
	using Gender = org.camunda.bpm.model.xml.testmodel.Gender;
	using TestModelParser = org.camunda.bpm.model.xml.testmodel.TestModelParser;
	using TestModelTest = org.camunda.bpm.model.xml.testmodel.TestModelTest;
	using org.camunda.bpm.model.xml.testmodel.instance;
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
	public class ChildElementCollectionTest : TestModelTest
	{

	  private Bird tweety;
	  private Bird daffy;
	  private Bird daisy;
	  private Bird plucky;
	  private Bird birdo;
	  private ChildElement<FlightInstructor> flightInstructorChild;
	  private ChildElementCollection<FlightPartnerRef> flightPartnerRefCollection;

	  public ChildElementCollectionTest(string testName, ModelInstance testModelInstance, AbstractModelParser modelParser) : base(testName, testModelInstance, modelParser)
	  {
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters(name="Model {0}") public static java.util.Collection<Object[]> models()
	  public static ICollection<object[]> models()
	  {
		object[][] models = new object[][] {createModel(), parseModel(typeof(ChildElementCollectionTest))};
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
		Bird plucky = createBird(modelInstance, "plucky", Gender.Male);
		createBird(modelInstance, "birdo", Gender.Female);

		tweety.FlightInstructor = daffy;
		tweety.FlightPartnerRefs.Add(daisy);
		tweety.FlightPartnerRefs.Add(plucky);

		return new object[]{"created", modelInstance, modelParser};
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void copyModelInstance()
	  public virtual void copyModelInstance()
	  {
		modelInstance = cloneModelInstance();

		tweety = modelInstance.getModelElementById("tweety");
		daffy = modelInstance.getModelElementById("daffy");
		daisy = modelInstance.getModelElementById("daisy");
		plucky = modelInstance.getModelElementById("plucky");
		birdo = modelInstance.getModelElementById("birdo");

		flightInstructorChild = (ChildElement<FlightInstructor>) FlyingAnimal.flightInstructorChild.ReferenceSourceCollection;
		flightPartnerRefCollection = FlyingAnimal.flightPartnerRefsColl.ReferenceSourceCollection;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testImmutable()
	  public virtual void testImmutable()
	  {
		assertThat(flightInstructorChild).Mutable;
		assertThat(flightPartnerRefCollection).Mutable;

		((ChildElementImpl<FlightInstructor>) flightInstructorChild).setImmutable();
		((ChildElementCollectionImpl<FlightPartnerRef>) flightPartnerRefCollection).setImmutable();
		assertThat(flightInstructorChild).Immutable;
		assertThat(flightPartnerRefCollection).Immutable;

		((ChildElementImpl<FlightInstructor>) flightInstructorChild).Mutable = true;
		((ChildElementCollectionImpl<FlightPartnerRef>) flightPartnerRefCollection).Mutable = true;
		assertThat(flightInstructorChild).Mutable;
		assertThat(flightPartnerRefCollection).Mutable;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMinOccurs()
	  public virtual void testMinOccurs()
	  {
		assertThat(flightInstructorChild).Optional;
		assertThat(flightPartnerRefCollection).Optional;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMaxOccurs()
	  public virtual void testMaxOccurs()
	  {
		assertThat(flightInstructorChild).occursMaximal(1);
		assertThat(flightPartnerRefCollection).Unbounded;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testChildElementType()
	  public virtual void testChildElementType()
	  {
		assertThat(flightInstructorChild).containsType(typeof(FlightInstructor));
		assertThat(flightPartnerRefCollection).containsType(typeof(FlightPartnerRef));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testParentElementType()
	  public virtual void testParentElementType()
	  {
		ModelElementType flyingAnimalType = modelInstance.Model.getType(typeof(FlyingAnimal));

		assertThat(flightInstructorChild).hasParentElementType(flyingAnimalType);
		assertThat(flightPartnerRefCollection).hasParentElementType(flyingAnimalType);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetChildElements()
	  public virtual void testGetChildElements()
	  {
		assertThat(flightInstructorChild).hasSize(tweety, 1);
		assertThat(flightPartnerRefCollection).hasSize(tweety, 2);

		FlightInstructor flightInstructor = flightInstructorChild.getChild(tweety);
		assertThat(flightInstructor.TextContent).isEqualTo(daffy.Id);

		foreach (FlightPartnerRef flightPartnerRef in flightPartnerRefCollection.get(tweety))
		{
		  assertThat(flightPartnerRef.TextContent).isIn(daisy.Id, plucky.Id);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveChildElements()
	  public virtual void testRemoveChildElements()
	  {
		assertThat(flightInstructorChild).isNotEmpty(tweety);
		assertThat(flightPartnerRefCollection).isNotEmpty(tweety);

		flightInstructorChild.removeChild(tweety);
		flightPartnerRefCollection.get(tweety).Clear();

		assertThat(flightInstructorChild).isEmpty(tweety);
		assertThat(flightPartnerRefCollection).isEmpty(tweety);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testChildElementsCollection()
	  public virtual void testChildElementsCollection()
	  {
		ICollection<FlightPartnerRef> flightPartnerRefs = flightPartnerRefCollection.get(tweety);

		IEnumerator<FlightPartnerRef> iterator = flightPartnerRefs.GetEnumerator();
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		FlightPartnerRef daisyRef = iterator.next();
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		FlightPartnerRef pluckyRef = iterator.next();
		assertThat(daisyRef.TextContent).isEqualTo(daisy.Id);
		assertThat(pluckyRef.TextContent).isEqualTo(plucky.Id);

		FlightPartnerRef birdoRef = modelInstance.newInstance(typeof(FlightPartnerRef));
		birdoRef.TextContent = birdo.Id;

		ICollection<FlightPartnerRef> flightPartners = Arrays.asList(birdoRef, daisyRef, pluckyRef);

		// directly test collection methods and not use the appropriate assertion methods
		assertThat(flightPartnerRefs.Count).isEqualTo(2);
		assertThat(flightPartnerRefs.Count == 0).False;
		assertThat(flightPartnerRefs.Contains(daisyRef));
		assertThat(flightPartnerRefs.ToArray()).isEqualTo(new object[]{daisyRef, pluckyRef});
		assertThat(flightPartnerRefs.toArray(new FlightPartnerRef[1])).isEqualTo(new FlightPartnerRef[]{daisyRef, pluckyRef});

		assertThat(flightPartnerRefs.Add(birdoRef)).True;
		assertThat(flightPartnerRefs).hasSize(3).containsOnly(birdoRef, daisyRef, pluckyRef);

		assertThat(flightPartnerRefs.remove(daisyRef)).True;
		assertThat(flightPartnerRefs).hasSize(2).containsOnly(birdoRef, pluckyRef);

		assertThat(flightPartnerRefs.addAll(flightPartners)).True;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
		assertThat(flightPartnerRefs.containsAll(flightPartners)).True;
		assertThat(flightPartnerRefs).hasSize(3).containsOnly(birdoRef, daisyRef, pluckyRef);

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
		assertThat(flightPartnerRefs.removeAll(flightPartners)).True;
		assertThat(flightPartnerRefs).Empty;

		try
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'retainAll' method:
		  flightPartnerRefs.retainAll(flightPartners);
		  fail("retainAll method is not implemented");
		}
		catch (Exception e)
		{
		  assertThat(e).isInstanceOf(typeof(UnsupportedModelOperationException));
		}

		flightPartnerRefs.addAll(flightPartners);
		assertThat(flightPartnerRefs).NotEmpty;
		flightPartnerRefs.Clear();
		assertThat(flightPartnerRefs).Empty;
	  }
	}

}