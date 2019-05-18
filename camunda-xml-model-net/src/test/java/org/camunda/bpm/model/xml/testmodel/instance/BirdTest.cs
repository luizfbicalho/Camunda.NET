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
	using StringUtil = org.camunda.bpm.model.xml.impl.util.StringUtil;
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
	public class BirdTest : TestModelTest
	{

	  private Bird tweety;
	  private Bird hedwig;
	  private Bird timmy;
	  private Egg egg1;
	  private Egg egg2;
	  private Egg egg3;

	  public BirdTest(string testName, ModelInstance testModelInstance, AbstractModelParser modelParser) : base(testName, testModelInstance, modelParser)
	  {
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters(name="Model {0}") public static java.util.Collection<Object[]> models()
	  public static ICollection<object[]> models()
	  {
		object[][] models = new object[][] {createModel(), parseModel(typeof(BirdTest))};
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

		Bird tweety = createBird(modelInstance, "tweety", Gender.Female);
		Bird hedwig = createBird(modelInstance, "hedwig", Gender.Female);
		Bird timmy = createBird(modelInstance, "timmy", Gender.Female);
		Egg egg1 = createEgg(modelInstance, "egg1");
		egg1.Mother = tweety;
		ICollection<Animal> guards = egg1.Guardians;
		guards.Add(hedwig);
		guards.Add(timmy);
		Egg egg2 = createEgg(modelInstance, "egg2");
		egg2.Mother = tweety;
		guards = egg2.Guardians;
		guards.Add(hedwig);
		guards.Add(timmy);
		Egg egg3 = createEgg(modelInstance, "egg3");
		guards = egg3.Guardians;
		guards.Add(timmy);

		tweety.Spouse = hedwig;
		tweety.Eggs.Add(egg1);
		tweety.Eggs.Add(egg2);
		tweety.Eggs.Add(egg3);

		ICollection<Egg> guardedEggs = hedwig.GuardedEggs;
		guardedEggs.Add(egg1);
		guardedEggs.Add(egg2);

		GuardEgg guardEgg = modelInstance.newInstance(typeof(GuardEgg));
		guardEgg.TextContent = egg1.Id + " " + egg2.Id;
		timmy.GuardedEggRefs.Add(guardEgg);
		timmy.GuardedEggs.Add(egg3);

		return new object[]{"created", modelInstance, modelParser};
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void copyModelInstance()
	  public virtual void copyModelInstance()
	  {
		modelInstance = cloneModelInstance();
		tweety = modelInstance.getModelElementById("tweety");
		hedwig = modelInstance.getModelElementById("hedwig");
		timmy = modelInstance.getModelElementById("timmy");
		egg1 = modelInstance.getModelElementById("egg1");
		egg2 = modelInstance.getModelElementById("egg2");
		egg3 = modelInstance.getModelElementById("egg3");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddEggsByHelper()
	  public virtual void testAddEggsByHelper()
	  {
		assertThat(tweety.Eggs).NotEmpty.hasSize(3).containsOnly(egg1, egg2, egg3);

		Egg egg4 = createEgg(modelInstance, "egg4");
		tweety.Eggs.Add(egg4);
		Egg egg5 = createEgg(modelInstance, "egg5");
		tweety.Eggs.Add(egg5);

		assertThat(tweety.Eggs).hasSize(5).containsOnly(egg1, egg2, egg3, egg4, egg5);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateEggsByIdByHelper()
	  public virtual void testUpdateEggsByIdByHelper()
	  {
		egg1.Id = "new-" + egg1.Id;
		egg2.Id = "new-" + egg2.Id;
		assertThat(tweety.Eggs).hasSize(3).containsOnly(egg1, egg2, egg3);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateEggsByIdByAttributeName()
	  public virtual void testUpdateEggsByIdByAttributeName()
	  {
		egg1.setAttributeValue("id", "new-" + egg1.Id, true);
		egg2.setAttributeValue("id", "new-" + egg2.Id, true);
		assertThat(tweety.Eggs).hasSize(3).containsOnly(egg1, egg2, egg3);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateEggsByReplaceElements()
	  public virtual void testUpdateEggsByReplaceElements()
	  {
		Egg egg4 = createEgg(modelInstance, "egg4");
		Egg egg5 = createEgg(modelInstance, "egg5");
		egg1.replaceWithElement(egg4);
		egg2.replaceWithElement(egg5);
		assertThat(tweety.Eggs).hasSize(3).containsOnly(egg3, egg4, egg5);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateEggsByRemoveElement()
	  public virtual void testUpdateEggsByRemoveElement()
	  {
		tweety.Eggs.remove(egg1);
		assertThat(tweety.Eggs).hasSize(2).containsOnly(egg2, egg3);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testClearEggs()
	  public virtual void testClearEggs()
	  {
		tweety.Eggs.Clear();
		assertThat(tweety.Eggs).Empty;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetSpouseRefByHelper()
	  public virtual void testSetSpouseRefByHelper()
	  {
		tweety.Spouse = timmy;
		assertThat(tweety.Spouse).isEqualTo(timmy);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateSpouseByIdHelper()
	  public virtual void testUpdateSpouseByIdHelper()
	  {
		hedwig.Id = "new-" + hedwig.Id;
		assertThat(tweety.Spouse).isEqualTo(hedwig);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateSpouseByIdByAttributeName()
	  public virtual void testUpdateSpouseByIdByAttributeName()
	  {
		hedwig.setAttributeValue("id", "new-" + hedwig.Id, true);
		assertThat(tweety.Spouse).isEqualTo(hedwig);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateSpouseByReplaceElement()
	  public virtual void testUpdateSpouseByReplaceElement()
	  {
		hedwig.replaceWithElement(timmy);
		assertThat(tweety.Spouse).isEqualTo(timmy);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateSpouseByRemoveElement()
	  public virtual void testUpdateSpouseByRemoveElement()
	  {
		Animals animals = (Animals) modelInstance.DocumentElement;
		animals.getAnimals().remove(hedwig);
		assertThat(tweety.Spouse).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testClearSpouse()
	  public virtual void testClearSpouse()
	  {
		tweety.removeSpouse();
		assertThat(tweety.Spouse).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetSpouseRefsByHelper()
	  public virtual void testSetSpouseRefsByHelper()
	  {
		SpouseRef spouseRef = modelInstance.newInstance(typeof(SpouseRef));
		spouseRef.TextContent = timmy.Id;
		tweety.SpouseRef.replaceWithElement(spouseRef);
		assertThat(tweety.Spouse).isEqualTo(timmy);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSpouseRefsByTextContent()
	  public virtual void testSpouseRefsByTextContent()
	  {
		SpouseRef spouseRef = tweety.SpouseRef;
		assertThat(spouseRef.TextContent).isEqualTo(hedwig.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateSpouseRefsByTextContent()
	  public virtual void testUpdateSpouseRefsByTextContent()
	  {
		SpouseRef spouseRef = tweety.SpouseRef;
		spouseRef.TextContent = timmy.Id;
		assertThat(tweety.Spouse).isEqualTo(timmy);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateSpouseRefsByTextContentWithNamespace()
	  public virtual void testUpdateSpouseRefsByTextContentWithNamespace()
	  {
		SpouseRef spouseRef = tweety.SpouseRef;
		spouseRef.TextContent = "tns:" + timmy.Id;
		assertThat(tweety.Spouse).isEqualTo(timmy);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetMother()
	  public virtual void testGetMother()
	  {
		Animal mother = egg1.Mother;
		assertThat(mother).isEqualTo(tweety);

		mother = egg2.Mother;
		assertThat(mother).isEqualTo(tweety);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetMotherRefByHelper()
	  public virtual void testSetMotherRefByHelper()
	  {
		egg1.Mother = timmy;
		assertThat(egg1.Mother).isEqualTo(timmy);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateMotherByIdHelper()
	  public virtual void testUpdateMotherByIdHelper()
	  {
		tweety.Id = "new-" + tweety.Id;
		assertThat(egg1.Mother).isEqualTo(tweety);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateMotherByIdByAttributeName()
	  public virtual void testUpdateMotherByIdByAttributeName()
	  {
		tweety.setAttributeValue("id", "new-" + tweety.Id, true);
		assertThat(egg1.Mother).isEqualTo(tweety);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateMotherByReplaceElement()
	  public virtual void testUpdateMotherByReplaceElement()
	  {
		tweety.replaceWithElement(timmy);
		assertThat(egg1.Mother).isEqualTo(timmy);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateMotherByRemoveElement()
	  public virtual void testUpdateMotherByRemoveElement()
	  {
		egg1.Mother = hedwig;
		Animals animals = (Animals) modelInstance.DocumentElement;
		animals.getAnimals().remove(hedwig);
		assertThat(egg1.Mother).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testClearMother()
	  public virtual void testClearMother()
	  {
		egg1.removeMother();
		assertThat(egg1.Mother).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetMotherRefsByHelper()
	  public virtual void testSetMotherRefsByHelper()
	  {
		Mother mother = modelInstance.newInstance(typeof(Mother));
		mother.Href = "#" + timmy.Id;
		egg1.MotherRef.replaceWithElement(mother);
		assertThat(egg1.Mother).isEqualTo(timmy);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMotherRefsByTextContent()
	  public virtual void testMotherRefsByTextContent()
	  {
		Mother mother = egg1.MotherRef;
		assertThat(mother.Href).isEqualTo("#" + tweety.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateMotherRefsByTextContent()
	  public virtual void testUpdateMotherRefsByTextContent()
	  {
		Mother mother = egg1.MotherRef;
		mother.Href = "#" + timmy.Id;
		assertThat(egg1.Mother).isEqualTo(timmy);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetGuards()
	  public virtual void testGetGuards()
	  {
		ICollection<Animal> guards = egg1.Guardians;
		assertThat(guards).NotEmpty.hasSize(2);
		assertThat(guards).contains(hedwig, timmy);

		guards = egg2.Guardians;
		assertThat(guards).NotEmpty.hasSize(2);
		assertThat(guards).contains(hedwig, timmy);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddGuardianRefsByHelper()
	  public virtual void testAddGuardianRefsByHelper()
	  {
		assertThat(egg1.GuardianRefs).NotEmpty.hasSize(2);

		Guardian tweetyGuardian = modelInstance.newInstance(typeof(Guardian));
		tweetyGuardian.Href = "#" + tweety.Id;
		egg1.GuardianRefs.Add(tweetyGuardian);

		assertThat(egg1.GuardianRefs).NotEmpty.hasSize(3).contains(tweetyGuardian);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGuardianRefsByTextContent()
	  public virtual void testGuardianRefsByTextContent()
	  {
		ICollection<Guardian> guardianRefs = egg1.GuardianRefs;
		ICollection<string> hrefs = new List<string>();
		foreach (Guardian guardianRef in guardianRefs)
		{
		  string href = guardianRef.Href;
		  assertThat(href).NotEmpty;
		  hrefs.Add(href);
		}
		assertThat(hrefs).NotEmpty.hasSize(2).containsOnly("#" + hedwig.Id, "#" + timmy.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateGuardianRefsByTextContent()
	  public virtual void testUpdateGuardianRefsByTextContent()
	  {
		IList<Guardian> guardianRefs = new List<Guardian>(egg1.GuardianRefs);

		guardianRefs[0].Href = "#" + tweety.Id;

		assertThat(egg1.Guardians).hasSize(2).containsOnly(tweety, timmy);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateGuardianRefsByRemoveElements()
	  public virtual void testUpdateGuardianRefsByRemoveElements()
	  {
		IList<Guardian> guardianRefs = new List<Guardian>(egg1.GuardianRefs);
		egg1.GuardianRefs.remove(guardianRefs[1]);
		assertThat(egg1.Guardians).hasSize(1).containsOnly(hedwig);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testClearGuardianRefs()
	  public virtual void testClearGuardianRefs()
	  {
		egg1.GuardianRefs.Clear();
		assertThat(egg1.GuardianRefs).Empty;

		// should not affect animals collection
		Animals animals = (Animals) modelInstance.DocumentElement;
		assertThat(animals.getAnimals()).NotEmpty.hasSize(3);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetGuardedEggs()
	  public virtual void testGetGuardedEggs()
	  {
		ICollection<Egg> guardedEggs = hedwig.GuardedEggs;
		assertThat(guardedEggs).NotEmpty.hasSize(2).contains(egg1, egg2);

		guardedEggs = timmy.GuardedEggs;
		assertThat(guardedEggs).NotEmpty.hasSize(3).contains(egg1, egg2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddGuardedEggRefsByHelper()
	  public virtual void testAddGuardedEggRefsByHelper()
	  {
		assertThat(hedwig.GuardedEggRefs).NotEmpty.hasSize(2);

		GuardEgg egg3GuardedEgg = modelInstance.newInstance(typeof(GuardEgg));
		egg3GuardedEgg.TextContent = egg3.Id;
		hedwig.GuardedEggRefs.Add(egg3GuardedEgg);

		assertThat(hedwig.GuardedEggRefs).NotEmpty.hasSize(3).contains(egg3GuardedEgg);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGuardedEggRefsByTextContent()
	  public virtual void testGuardedEggRefsByTextContent()
	  {
		ICollection<GuardEgg> guardianRefs = timmy.GuardedEggRefs;
		ICollection<string> textContents = new List<string>();
		foreach (GuardEgg guardianRef in guardianRefs)
		{
		  string textContent = guardianRef.TextContent;
		  assertThat(textContent).NotEmpty;
		  textContents.addAll(StringUtil.splitListBySeparator(textContent, " "));
		}
		assertThat(textContents).NotEmpty.hasSize(3).containsOnly(egg1.Id, egg2.Id, egg3.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateGuardedEggRefsByTextContent()
	  public virtual void testUpdateGuardedEggRefsByTextContent()
	  {
		IList<GuardEgg> guardianRefs = new List<GuardEgg>(hedwig.GuardedEggRefs);

		guardianRefs[0].TextContent = egg1.Id + " " + egg3.Id;

		assertThat(hedwig.GuardedEggs).hasSize(3).containsOnly(egg1, egg2, egg3);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateGuardedEggRefsByRemoveElements()
	  public virtual void testUpdateGuardedEggRefsByRemoveElements()
	  {
		IList<GuardEgg> guardianRefs = new List<GuardEgg>(timmy.GuardedEggRefs);
		timmy.GuardedEggRefs.remove(guardianRefs[0]);
		assertThat(timmy.GuardedEggs).hasSize(1).containsOnly(egg3);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testClearGuardedEggRefs()
	  public virtual void testClearGuardedEggRefs()
	  {
		timmy.GuardedEggRefs.Clear();
		assertThat(timmy.GuardedEggRefs).Empty;

		// should not affect animals collection
		Animals animals = (Animals) modelInstance.DocumentElement;
		assertThat(animals.getAnimals()).NotEmpty.hasSize(3);
	  }

	}

}