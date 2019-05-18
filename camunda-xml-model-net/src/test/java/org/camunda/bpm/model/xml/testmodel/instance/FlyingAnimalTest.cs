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
	using AbstractModelParser = org.camunda.bpm.model.xml.impl.parser.AbstractModelParser;
	using Before = org.junit.Before;
	using Test = org.junit.Test;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.testmodel.TestModelConstants.MODEL_NAMESPACE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.runners.Parameterized.Parameters;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class FlyingAnimalTest : TestModelTest
	{

	  private FlyingAnimal tweety;
	  private FlyingAnimal hedwig;
	  private FlyingAnimal birdo;
	  private FlyingAnimal plucky;
	  private FlyingAnimal fiffy;
	  private FlyingAnimal timmy;
	  private FlyingAnimal daisy;

	  public FlyingAnimalTest(string testName, ModelInstance testModelInstance, AbstractModelParser modelParser) : base(testName, testModelInstance, modelParser)
	  {
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters(name="Model {0}") public static java.util.Collection<Object[]> models()
	  public static ICollection<object[]> models()
	  {
		object[][] models = new object[][] {createModel(), parseModel(typeof(FlyingAnimalTest))};
		return Arrays.asList(models);
	  }

	  public static object[] createModel()
	  {
		TestModelParser modelParser = new TestModelParser();
		ModelInstance modelInstance = modelParser.EmptyModel;

		Animals animals = modelInstance.newInstance(typeof(Animals));
		modelInstance.DocumentElement = animals;

		// add a tns namespace prefix for QName testing
		animals.DomElement.registerNamespace("tns", MODEL_NAMESPACE);

		FlyingAnimal tweety = createBird(modelInstance, "tweety", Gender.Female);
		FlyingAnimal hedwig = createBird(modelInstance, "hedwig", Gender.Male);
		FlyingAnimal birdo = createBird(modelInstance, "birdo", Gender.Female);
		FlyingAnimal plucky = createBird(modelInstance, "plucky", Gender.Unknown);
		FlyingAnimal fiffy = createBird(modelInstance, "fiffy", Gender.Female);
		createBird(modelInstance, "timmy", Gender.Male);
		createBird(modelInstance, "daisy", Gender.Female);

		tweety.FlightInstructor = hedwig;

		tweety.FlightPartnerRefs.Add(hedwig);
		tweety.FlightPartnerRefs.Add(birdo);
		tweety.FlightPartnerRefs.Add(plucky);
		tweety.FlightPartnerRefs.Add(fiffy);


		return new object[]{"created", modelInstance, modelParser};
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void copyModelInstance()
	  public virtual void copyModelInstance()
	  {
		modelInstance = cloneModelInstance();
		tweety = modelInstance.getModelElementById("tweety");
		hedwig = modelInstance.getModelElementById("hedwig");
		birdo = modelInstance.getModelElementById("birdo");
		plucky = modelInstance.getModelElementById("plucky");
		fiffy = modelInstance.getModelElementById("fiffy");
		timmy = modelInstance.getModelElementById("timmy");
		daisy = modelInstance.getModelElementById("daisy");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetWingspanAttributeByHelper()
	  public virtual void testSetWingspanAttributeByHelper()
	  {
		double wingspan = 2.123;
		tweety.setWingspan(wingspan);
		assertThat(tweety.getWingspan()).isEqualTo(wingspan);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetWingspanAttributeByAttributeName()
	  public virtual void testSetWingspanAttributeByAttributeName()
	  {
		double? wingspan = 2.123;
		tweety.setAttributeValue("wingspan", wingspan.ToString(), false);
		assertThat(tweety.getWingspan()).isEqualTo(wingspan);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveWingspanAttribute()
	  public virtual void testRemoveWingspanAttribute()
	  {
		double wingspan = 2.123;
		tweety.setWingspan(wingspan);
		assertThat(tweety.getWingspan()).isEqualTo(wingspan);

		tweety.removeAttribute("wingspan");

		assertThat(tweety.getWingspan()).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetFlightInstructorByHelper()
	  public virtual void testSetFlightInstructorByHelper()
	  {
		tweety.FlightInstructor = timmy;
		assertThat(tweety.FlightInstructor).isEqualTo(timmy);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateFlightInstructorByIdHelper()
	  public virtual void testUpdateFlightInstructorByIdHelper()
	  {
		hedwig.Id = "new-" + hedwig.Id;
		assertThat(tweety.FlightInstructor).isEqualTo(hedwig);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateFlightInstructorByIdAttributeName()
	  public virtual void testUpdateFlightInstructorByIdAttributeName()
	  {
		hedwig.setAttributeValue("id", "new-" + hedwig.Id, true);
		assertThat(tweety.FlightInstructor).isEqualTo(hedwig);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateFlightInstructorByReplaceElement()
	  public virtual void testUpdateFlightInstructorByReplaceElement()
	  {
		hedwig.replaceWithElement(timmy);
		assertThat(tweety.FlightInstructor).isEqualTo(timmy);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateFlightInstructorByRemoveElement()
	  public virtual void testUpdateFlightInstructorByRemoveElement()
	  {
		Animals animals = (Animals) modelInstance.DocumentElement;
		animals.getAnimals().remove(hedwig);
		assertThat(tweety.FlightInstructor).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testClearFlightInstructor()
	  public virtual void testClearFlightInstructor()
	  {
		tweety.removeFlightInstructor();
		assertThat(tweety.FlightInstructor).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddFlightPartnerRefsByHelper()
	  public virtual void testAddFlightPartnerRefsByHelper()
	  {
		assertThat(tweety.FlightPartnerRefs).NotEmpty.hasSize(4).containsOnly(hedwig, birdo, plucky, fiffy);

		tweety.FlightPartnerRefs.Add(timmy);
		tweety.FlightPartnerRefs.Add(daisy);

		assertThat(tweety.FlightPartnerRefs).NotEmpty.hasSize(6).containsOnly(hedwig, birdo, plucky, fiffy, timmy, daisy);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateFlightPartnerRefsByIdByHelper()
	  public virtual void testUpdateFlightPartnerRefsByIdByHelper()
	  {
		hedwig.Id = "new-" + hedwig.Id;
		plucky.Id = "new-" + plucky.Id;
		assertThat(tweety.FlightPartnerRefs).hasSize(4).containsOnly(hedwig, birdo, plucky, fiffy);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateFlightPartnerRefsByIdByAttributeName()
	  public virtual void testUpdateFlightPartnerRefsByIdByAttributeName()
	  {
		birdo.setAttributeValue("id", "new-" + birdo.Id, true);
		fiffy.setAttributeValue("id", "new-" + fiffy.Id, true);
		assertThat(tweety.FlightPartnerRefs).hasSize(4).containsOnly(hedwig, birdo, plucky, fiffy);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateFlightPartnerRefsByReplaceElements()
	  public virtual void testUpdateFlightPartnerRefsByReplaceElements()
	  {
		hedwig.replaceWithElement(timmy);
		plucky.replaceWithElement(daisy);
		assertThat(tweety.FlightPartnerRefs).hasSize(4).containsOnly(birdo, fiffy, timmy,daisy);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateFlightPartnerRefsByRemoveElements()
	  public virtual void testUpdateFlightPartnerRefsByRemoveElements()
	  {
		tweety.FlightPartnerRefs.remove(birdo);
		tweety.FlightPartnerRefs.remove(fiffy);
		assertThat(tweety.FlightPartnerRefs).hasSize(2).containsOnly(hedwig, plucky);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testClearFlightPartnerRefs()
	  public virtual void testClearFlightPartnerRefs()
	  {
		tweety.FlightPartnerRefs.Clear();
		assertThat(tweety.FlightPartnerRefs).Empty;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddFlightPartnerRefElementsByHelper()
	  public virtual void testAddFlightPartnerRefElementsByHelper()
	  {
		assertThat(tweety.FlightPartnerRefElements).NotEmpty.hasSize(4);

		FlightPartnerRef timmyFlightPartnerRef = modelInstance.newInstance(typeof(FlightPartnerRef));
		timmyFlightPartnerRef.TextContent = timmy.Id;
		tweety.FlightPartnerRefElements.Add(timmyFlightPartnerRef);

		FlightPartnerRef daisyFlightPartnerRef = modelInstance.newInstance(typeof(FlightPartnerRef));
		daisyFlightPartnerRef.TextContent = daisy.Id;
		tweety.FlightPartnerRefElements.Add(daisyFlightPartnerRef);

		assertThat(tweety.FlightPartnerRefElements).NotEmpty.hasSize(6).contains(timmyFlightPartnerRef, daisyFlightPartnerRef);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFlightPartnerRefElementsByTextContent()
	  public virtual void testFlightPartnerRefElementsByTextContent()
	  {
		ICollection<FlightPartnerRef> flightPartnerRefElements = tweety.FlightPartnerRefElements;
		ICollection<string> textContents = new List<string>();
		foreach (FlightPartnerRef flightPartnerRefElement in flightPartnerRefElements)
		{
		  string textContent = flightPartnerRefElement.TextContent;
		  assertThat(textContent).NotEmpty;
		  textContents.Add(textContent);
		}
		assertThat(textContents).NotEmpty.hasSize(4).containsOnly(hedwig.Id, birdo.Id, plucky.Id, fiffy.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateFlightPartnerRefElementsByTextContent()
	  public virtual void testUpdateFlightPartnerRefElementsByTextContent()
	  {
		IList<FlightPartnerRef> flightPartnerRefs = new List<FlightPartnerRef>(tweety.FlightPartnerRefElements);

		flightPartnerRefs[0].TextContent = timmy.Id;
		flightPartnerRefs[2].TextContent = daisy.Id;

		assertThat(tweety.FlightPartnerRefs).hasSize(4).containsOnly(birdo, fiffy, timmy, daisy);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateFlightPartnerRefElementsByRemoveElements()
	  public virtual void testUpdateFlightPartnerRefElementsByRemoveElements()
	  {
		IList<FlightPartnerRef> flightPartnerRefs = new List<FlightPartnerRef>(tweety.FlightPartnerRefElements);
		tweety.FlightPartnerRefElements.remove(flightPartnerRefs[1]);
		tweety.FlightPartnerRefElements.remove(flightPartnerRefs[3]);
		assertThat(tweety.FlightPartnerRefs).hasSize(2).containsOnly(hedwig, plucky);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testClearFlightPartnerRefElements()
	  public virtual void testClearFlightPartnerRefElements()
	  {
		tweety.FlightPartnerRefElements.Clear();
		assertThat(tweety.FlightPartnerRefElements).Empty;

		// should not affect animals collection
		Animals animals = (Animals) modelInstance.DocumentElement;
		assertThat(animals.getAnimals()).NotEmpty.hasSize(7);
	  }

	}

}