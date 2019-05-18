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
namespace org.camunda.bpm.model.xml.testmodel.instance
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.testmodel.TestModelConstants.MODEL_NAMESPACE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;


	using AbstractModelParser = org.camunda.bpm.model.xml.impl.parser.AbstractModelParser;
	using Before = org.junit.Before;
	using Test = org.junit.Test;
	using Parameters = org.junit.runners.Parameterized.Parameters;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class AnimalTest : TestModelTest
	{

	  private Animal tweety;
	  private Animal hedwig;
	  private Animal birdo;
	  private Animal plucky;
	  private Animal fiffy;
	  private Animal timmy;
	  private Animal daisy;
	  private RelationshipDefinition hedwigRelationship;
	  private RelationshipDefinition birdoRelationship;
	  private RelationshipDefinition pluckyRelationship;
	  private RelationshipDefinition fiffyRelationship;
	  private RelationshipDefinition timmyRelationship;
	  private RelationshipDefinition daisyRelationship;

	  public AnimalTest(string testName, ModelInstance testModelInstance, AbstractModelParser modelParser) : base(testName, testModelInstance, modelParser)
	  {
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters(name="Model {0}") public static java.util.Collection<Object[]> models()
	  public static ICollection<object[]> models()
	  {
		object[][] models = new object[][] {createModel(), parseModel(typeof(AnimalTest))};
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

		Animal tweety = createBird(modelInstance, "tweety", Gender.Female);
		Animal hedwig = createBird(modelInstance, "hedwig", Gender.Male);
		Animal birdo = createBird(modelInstance, "birdo", Gender.Female);
		Animal plucky = createBird(modelInstance, "plucky", Gender.Unknown);
		Animal fiffy = createBird(modelInstance, "fiffy", Gender.Female);
		createBird(modelInstance, "timmy", Gender.Male);
		createBird(modelInstance, "daisy", Gender.Female);

		// create and add some relationships
		RelationshipDefinition hedwigRelationship = createRelationshipDefinition(modelInstance, hedwig, typeof(ChildRelationshipDefinition));
		addRelationshipDefinition(tweety, hedwigRelationship);
		RelationshipDefinition birdoRelationship = createRelationshipDefinition(modelInstance, birdo, typeof(ChildRelationshipDefinition));
		addRelationshipDefinition(tweety, birdoRelationship);
		RelationshipDefinition pluckyRelationship = createRelationshipDefinition(modelInstance, plucky, typeof(FriendRelationshipDefinition));
		addRelationshipDefinition(tweety, pluckyRelationship);
		RelationshipDefinition fiffyRelationship = createRelationshipDefinition(modelInstance, fiffy, typeof(FriendRelationshipDefinition));
		addRelationshipDefinition(tweety, fiffyRelationship);

		tweety.RelationshipDefinitionRefs.Add(hedwigRelationship);
		tweety.RelationshipDefinitionRefs.Add(birdoRelationship);
		tweety.RelationshipDefinitionRefs.Add(pluckyRelationship);
		tweety.RelationshipDefinitionRefs.Add(fiffyRelationship);

		tweety.BestFriends.Add(birdo);
		tweety.BestFriends.Add(plucky);

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

		hedwigRelationship = modelInstance.getModelElementById("tweety-hedwig");
		birdoRelationship = modelInstance.getModelElementById("tweety-birdo");
		pluckyRelationship = modelInstance.getModelElementById("tweety-plucky");
		fiffyRelationship = modelInstance.getModelElementById("tweety-fiffy");

		timmyRelationship = createRelationshipDefinition(modelInstance, timmy, typeof(FriendRelationshipDefinition));
		daisyRelationship = createRelationshipDefinition(modelInstance, daisy, typeof(ChildRelationshipDefinition));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetIdAttributeByHelper()
	  public virtual void testSetIdAttributeByHelper()
	  {
		string newId = "new-" + tweety.Id;
		tweety.Id = newId;
		assertThat(tweety.Id).isEqualTo(newId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetIdAttributeByAttributeName()
	  public virtual void testSetIdAttributeByAttributeName()
	  {
		tweety.setAttributeValue("id", "duffy", true);
		assertThat(tweety.Id).isEqualTo("duffy");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveIdAttribute()
	  public virtual void testRemoveIdAttribute()
	  {
		tweety.removeAttribute("id");
		assertThat(tweety.Id).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetNameAttributeByHelper()
	  public virtual void testSetNameAttributeByHelper()
	  {
		tweety.Name = "tweety";
		assertThat(tweety.Name).isEqualTo("tweety");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetNameAttributeByAttributeName()
	  public virtual void testSetNameAttributeByAttributeName()
	  {
		tweety.setAttributeValue("name", "daisy");
		assertThat(tweety.Name).isEqualTo("daisy");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveNameAttribute()
	  public virtual void testRemoveNameAttribute()
	  {
		tweety.removeAttribute("name");
		assertThat(tweety.Name).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetFatherAttributeByHelper()
	  public virtual void testSetFatherAttributeByHelper()
	  {
		tweety.Father = timmy;
		assertThat(tweety.Father).isEqualTo(timmy);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetFatherAttributeByAttributeName()
	  public virtual void testSetFatherAttributeByAttributeName()
	  {
		tweety.setAttributeValue("father", timmy.Id);
		assertThat(tweety.Father).isEqualTo(timmy);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetFatherAttributeByAttributeNameWithNamespace()
	  public virtual void testSetFatherAttributeByAttributeNameWithNamespace()
	  {
		tweety.setAttributeValue("father", "tns:hedwig");
		assertThat(tweety.Father).isEqualTo(hedwig);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveFatherAttribute()
	  public virtual void testRemoveFatherAttribute()
	  {
		tweety.Father = timmy;
		assertThat(tweety.Father).isEqualTo(timmy);
		tweety.removeAttribute("father");
		assertThat(tweety.Father).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testChangeIdAttributeOfFatherReference()
	  public virtual void testChangeIdAttributeOfFatherReference()
	  {
		tweety.Father = timmy;
		assertThat(tweety.Father).isEqualTo(timmy);
		timmy.Id = "new-" + timmy.Id;
		assertThat(tweety.Father).isEqualTo(timmy);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReplaceFatherReferenceWithNewAnimal()
	  public virtual void testReplaceFatherReferenceWithNewAnimal()
	  {
		tweety.Father = timmy;
		assertThat(tweety.Father).isEqualTo(timmy);
		timmy.replaceWithElement(plucky);
		assertThat(tweety.Father).isEqualTo(plucky);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetMotherAttributeByHelper()
	  public virtual void testSetMotherAttributeByHelper()
	  {
		tweety.Mother = daisy;
		assertThat(tweety.Mother).isEqualTo(daisy);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetMotherAttributeByAttributeName()
	  public virtual void testSetMotherAttributeByAttributeName()
	  {
		tweety.setAttributeValue("mother", fiffy.Id);
		assertThat(tweety.Mother).isEqualTo(fiffy);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveMotherAttribute()
	  public virtual void testRemoveMotherAttribute()
	  {
		tweety.Mother = daisy;
		assertThat(tweety.Mother).isEqualTo(daisy);
		tweety.removeAttribute("mother");
		assertThat(tweety.Mother).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReplaceMotherReferenceWithNewAnimal()
	  public virtual void testReplaceMotherReferenceWithNewAnimal()
	  {
		tweety.Mother = daisy;
		assertThat(tweety.Mother).isEqualTo(daisy);
		daisy.replaceWithElement(birdo);
		assertThat(tweety.Mother).isEqualTo(birdo);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testChangeIdAttributeOfMotherReference()
	  public virtual void testChangeIdAttributeOfMotherReference()
	  {
		tweety.Mother = daisy;
		assertThat(tweety.Mother).isEqualTo(daisy);
		daisy.Id = "new-" + daisy.Id;
		assertThat(tweety.Mother).isEqualTo(daisy);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetIsEndangeredAttributeByHelper()
	  public virtual void testSetIsEndangeredAttributeByHelper()
	  {
		tweety.IsEndangered = true;
		assertThat(tweety.Endangered).True;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetIsEndangeredAttributeByAttributeName()
	  public virtual void testSetIsEndangeredAttributeByAttributeName()
	  {
		tweety.setAttributeValue("isEndangered", "false");
		assertThat(tweety.Endangered).False;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveIsEndangeredAttribute()
	  public virtual void testRemoveIsEndangeredAttribute()
	  {
		tweety.removeAttribute("isEndangered");
		// default value of isEndangered: false
		assertThat(tweety.Endangered).False;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetGenderAttributeByHelper()
	  public virtual void testSetGenderAttributeByHelper()
	  {
		tweety.Gender = Gender.Male;
		assertThat(tweety.Gender).isEqualTo(Gender.Male);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetGenderAttributeByAttributeName()
	  public virtual void testSetGenderAttributeByAttributeName()
	  {
		tweety.setAttributeValue("gender", Gender.Unknown.ToString());
		assertThat(tweety.Gender).isEqualTo(Gender.Unknown);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveGenderAttribute()
	  public virtual void testRemoveGenderAttribute()
	  {
		tweety.removeAttribute("gender");
		assertThat(tweety.Gender).Null;

		// gender is required, so the model is invalid without
		try
		{
		  validateModel();
		  fail("The model is invalid cause the gender of an animal is a required attribute.");
		}
		catch (Exception e)
		{
		  assertThat(e).isInstanceOf(typeof(ModelValidationException));
		}

		// add gender to make model valid
		tweety.Gender = Gender.Female;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetAgeAttributeByHelper()
	  public virtual void testSetAgeAttributeByHelper()
	  {
		tweety.setAge(13);
		assertThat(tweety.getAge()).isEqualTo(13);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetAgeAttributeByAttributeName()
	  public virtual void testSetAgeAttributeByAttributeName()
	  {
		tweety.setAttributeValue("age", "23");
		assertThat(tweety.getAge()).isEqualTo(23);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveAgeAttribute()
	  public virtual void testRemoveAgeAttribute()
	  {
		tweety.removeAttribute("age");
		assertThat(tweety.getAge()).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddRelationshipDefinitionsByHelper()
	  public virtual void testAddRelationshipDefinitionsByHelper()
	  {
		assertThat(tweety.RelationshipDefinitions).NotEmpty.hasSize(4).containsOnly(hedwigRelationship, birdoRelationship, pluckyRelationship, fiffyRelationship);

		tweety.RelationshipDefinitions.Add(timmyRelationship);
		tweety.RelationshipDefinitions.Add(daisyRelationship);

		assertThat(tweety.RelationshipDefinitions).hasSize(6).containsOnly(hedwigRelationship, birdoRelationship, pluckyRelationship, fiffyRelationship, timmyRelationship, daisyRelationship);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateRelationshipDefinitionsByIdByHelper()
	  public virtual void testUpdateRelationshipDefinitionsByIdByHelper()
	  {
		hedwigRelationship.Id = "new-" + hedwigRelationship.Id;
		pluckyRelationship.Id = "new-" + pluckyRelationship.Id;
		assertThat(tweety.RelationshipDefinitions).hasSize(4).containsOnly(hedwigRelationship, birdoRelationship, pluckyRelationship, fiffyRelationship);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateRelationshipDefinitionsByIdByAttributeName()
	  public virtual void testUpdateRelationshipDefinitionsByIdByAttributeName()
	  {
		birdoRelationship.setAttributeValue("id", "new-" + birdoRelationship.Id, true);
		fiffyRelationship.setAttributeValue("id", "new-" + fiffyRelationship.Id, true);
		assertThat(tweety.RelationshipDefinitions).hasSize(4).containsOnly(hedwigRelationship, birdoRelationship, pluckyRelationship, fiffyRelationship);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateRelationshipDefinitionsByReplaceElements()
	  public virtual void testUpdateRelationshipDefinitionsByReplaceElements()
	  {
		hedwigRelationship.replaceWithElement(timmyRelationship);
		pluckyRelationship.replaceWithElement(daisyRelationship);
		assertThat(tweety.RelationshipDefinitions).hasSize(4).containsOnly(birdoRelationship, fiffyRelationship, timmyRelationship, daisyRelationship);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateRelationshipDefinitionsByRemoveElements()
	  public virtual void testUpdateRelationshipDefinitionsByRemoveElements()
	  {
		tweety.RelationshipDefinitions.remove(birdoRelationship);
		tweety.RelationshipDefinitions.remove(fiffyRelationship);
		assertThat(tweety.RelationshipDefinitions).hasSize(2).containsOnly(hedwigRelationship, pluckyRelationship);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testClearRelationshipDefinitions()
	  public virtual void testClearRelationshipDefinitions()
	  {
		tweety.RelationshipDefinitions.Clear();
		assertThat(tweety.RelationshipDefinitions).Empty;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddRelationsDefinitionRefsByHelper()
	  public virtual void testAddRelationsDefinitionRefsByHelper()
	  {
		assertThat(tweety.RelationshipDefinitionRefs).NotEmpty.hasSize(4).containsOnly(hedwigRelationship, birdoRelationship, pluckyRelationship, fiffyRelationship);

		addRelationshipDefinition(tweety, timmyRelationship);
		addRelationshipDefinition(tweety, daisyRelationship);
		tweety.RelationshipDefinitionRefs.Add(timmyRelationship);
		tweety.RelationshipDefinitionRefs.Add(daisyRelationship);

		assertThat(tweety.RelationshipDefinitionRefs).NotEmpty.hasSize(6).containsOnly(hedwigRelationship, birdoRelationship, pluckyRelationship, fiffyRelationship, timmyRelationship, daisyRelationship);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateRelationshipDefinitionRefsByIdByHelper()
	  public virtual void testUpdateRelationshipDefinitionRefsByIdByHelper()
	  {
		hedwigRelationship.Id = "child-relationship";
		pluckyRelationship.Id = "friend-relationship";
		assertThat(tweety.RelationshipDefinitionRefs).hasSize(4).containsOnly(hedwigRelationship, birdoRelationship, pluckyRelationship, fiffyRelationship);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateRelationshipDefinitionRefsByIdByAttributeName()
	  public virtual void testUpdateRelationshipDefinitionRefsByIdByAttributeName()
	  {
		birdoRelationship.setAttributeValue("id", "birdo-relationship", true);
		fiffyRelationship.setAttributeValue("id", "fiffy-relationship", true);
		assertThat(tweety.RelationshipDefinitionRefs).hasSize(4).containsOnly(hedwigRelationship, birdoRelationship, pluckyRelationship, fiffyRelationship);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateRelationshipDefinitionRefsByReplaceElements()
	  public virtual void testUpdateRelationshipDefinitionRefsByReplaceElements()
	  {
		hedwigRelationship.replaceWithElement(timmyRelationship);
		pluckyRelationship.replaceWithElement(daisyRelationship);
		assertThat(tweety.RelationshipDefinitionRefs).hasSize(4).containsOnly(birdoRelationship, fiffyRelationship, timmyRelationship, daisyRelationship);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateRelationshipDefinitionRefsByRemoveElements()
	  public virtual void testUpdateRelationshipDefinitionRefsByRemoveElements()
	  {
		tweety.RelationshipDefinitions.remove(birdoRelationship);
		tweety.RelationshipDefinitions.remove(fiffyRelationship);
		assertThat(tweety.RelationshipDefinitionRefs).hasSize(2).containsOnly(hedwigRelationship, pluckyRelationship);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateRelationshipDefinitionRefsByRemoveIdAttribute()
	  public virtual void testUpdateRelationshipDefinitionRefsByRemoveIdAttribute()
	  {
		birdoRelationship.removeAttribute("id");
		pluckyRelationship.removeAttribute("id");
		assertThat(tweety.RelationshipDefinitionRefs).hasSize(2).containsOnly(hedwigRelationship, fiffyRelationship);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testClearRelationshipDefinitionsRefs()
	  public virtual void testClearRelationshipDefinitionsRefs()
	  {
		tweety.RelationshipDefinitionRefs.Clear();
		assertThat(tweety.RelationshipDefinitionRefs).Empty;
		// should not affect animal relationship definitions
		assertThat(tweety.RelationshipDefinitions).hasSize(4);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testClearRelationshipDefinitionRefsByClearRelationshipDefinitions()
	  public virtual void testClearRelationshipDefinitionRefsByClearRelationshipDefinitions()
	  {
		assertThat(tweety.RelationshipDefinitionRefs).NotEmpty;
		tweety.RelationshipDefinitions.Clear();
		assertThat(tweety.RelationshipDefinitions).Empty;
		// should affect animal relationship definition refs
		assertThat(tweety.RelationshipDefinitionRefs).Empty;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddRelationshipDefinitionRefElementsByHelper()
	  public virtual void testAddRelationshipDefinitionRefElementsByHelper()
	  {
		assertThat(tweety.RelationshipDefinitionRefElements).NotEmpty.hasSize(4);

		addRelationshipDefinition(tweety, timmyRelationship);
		RelationshipDefinitionRef timmyRelationshipDefinitionRef = modelInstance.newInstance(typeof(RelationshipDefinitionRef));
		timmyRelationshipDefinitionRef.TextContent = timmyRelationship.Id;
		tweety.RelationshipDefinitionRefElements.Add(timmyRelationshipDefinitionRef);

		addRelationshipDefinition(tweety, daisyRelationship);
		RelationshipDefinitionRef daisyRelationshipDefinitionRef = modelInstance.newInstance(typeof(RelationshipDefinitionRef));
		daisyRelationshipDefinitionRef.TextContent = daisyRelationship.Id;
		tweety.RelationshipDefinitionRefElements.Add(daisyRelationshipDefinitionRef);

		assertThat(tweety.RelationshipDefinitionRefElements).NotEmpty.hasSize(6).contains(timmyRelationshipDefinitionRef, daisyRelationshipDefinitionRef);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRelationshipDefinitionRefElementsByTextContent()
	  public virtual void testRelationshipDefinitionRefElementsByTextContent()
	  {
		ICollection<RelationshipDefinitionRef> relationshipDefinitionRefElements = tweety.RelationshipDefinitionRefElements;
		ICollection<string> textContents = new List<string>();
		foreach (RelationshipDefinitionRef relationshipDefinitionRef in relationshipDefinitionRefElements)
		{
		  string textContent = relationshipDefinitionRef.TextContent;
		  assertThat(textContent).NotEmpty;
		  textContents.Add(textContent);
		}
		assertThat(textContents).NotEmpty.hasSize(4).containsOnly(hedwigRelationship.Id, birdoRelationship.Id, pluckyRelationship.Id, fiffyRelationship.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateRelationshipDefinitionRefElementsByTextContent()
	  public virtual void testUpdateRelationshipDefinitionRefElementsByTextContent()
	  {
		IList<RelationshipDefinitionRef> relationshipDefinitionRefs = new List<RelationshipDefinitionRef>(tweety.RelationshipDefinitionRefElements);

		addRelationshipDefinition(tweety, timmyRelationship);
		relationshipDefinitionRefs[0].TextContent = timmyRelationship.Id;

		addRelationshipDefinition(daisy, daisyRelationship);
		relationshipDefinitionRefs[2].TextContent = daisyRelationship.Id;

		assertThat(tweety.RelationshipDefinitionRefs).hasSize(4).containsOnly(birdoRelationship, fiffyRelationship, timmyRelationship, daisyRelationship);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateRelationshipDefinitionRefElementsByTextContentWithNamespace()
	  public virtual void testUpdateRelationshipDefinitionRefElementsByTextContentWithNamespace()
	  {
		IList<RelationshipDefinitionRef> relationshipDefinitionRefs = new List<RelationshipDefinitionRef>(tweety.RelationshipDefinitionRefElements);

		addRelationshipDefinition(tweety, timmyRelationship);
		relationshipDefinitionRefs[0].TextContent = "tns:" + timmyRelationship.Id;

		addRelationshipDefinition(daisy, daisyRelationship);
		relationshipDefinitionRefs[2].TextContent = "tns:" + daisyRelationship.Id;

		assertThat(tweety.RelationshipDefinitionRefs).hasSize(4).containsOnly(birdoRelationship, fiffyRelationship, timmyRelationship, daisyRelationship);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateRelationshipDefinitionRefElementsByRemoveElements()
	  public virtual void testUpdateRelationshipDefinitionRefElementsByRemoveElements()
	  {
		IList<RelationshipDefinitionRef> relationshipDefinitionRefs = new List<RelationshipDefinitionRef>(tweety.RelationshipDefinitionRefElements);
		tweety.RelationshipDefinitionRefElements.remove(relationshipDefinitionRefs[1]);
		tweety.RelationshipDefinitionRefElements.remove(relationshipDefinitionRefs[3]);
		assertThat(tweety.RelationshipDefinitionRefs).hasSize(2).containsOnly(hedwigRelationship, pluckyRelationship);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testClearRelationshipDefinitionRefElements()
	  public virtual void testClearRelationshipDefinitionRefElements()
	  {
		tweety.RelationshipDefinitionRefElements.Clear();
		assertThat(tweety.RelationshipDefinitionRefElements).Empty;
		assertThat(tweety.RelationshipDefinitionRefs).Empty;
		// should not affect animal relationship definitions
		assertThat(tweety.RelationshipDefinitions).NotEmpty.hasSize(4);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testClearRelationshipDefinitionRefElementsByClearRelationshipDefinitionRefs()
	  public virtual void testClearRelationshipDefinitionRefElementsByClearRelationshipDefinitionRefs()
	  {
		tweety.RelationshipDefinitionRefs.Clear();
		assertThat(tweety.RelationshipDefinitionRefs).Empty;
		assertThat(tweety.RelationshipDefinitionRefElements).Empty;
		// should not affect animal relationship definitions
		assertThat(tweety.RelationshipDefinitions).NotEmpty.hasSize(4);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testClearRelationshipDefinitionRefElementsByClearRelationshipDefinitions()
	  public virtual void testClearRelationshipDefinitionRefElementsByClearRelationshipDefinitions()
	  {
		tweety.RelationshipDefinitions.Clear();
		assertThat(tweety.RelationshipDefinitionRefs).Empty;
		assertThat(tweety.RelationshipDefinitionRefElements).Empty;
		// should affect animal relationship definitions
		assertThat(tweety.RelationshipDefinitions).Empty;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetBestFriends()
	  public virtual void testGetBestFriends()
	  {
		ICollection<Animal> bestFriends = tweety.BestFriends;

		assertThat(bestFriends).NotEmpty.hasSize(2).containsOnly(birdo, plucky);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddBestFriend()
	  public virtual void testAddBestFriend()
	  {
		tweety.BestFriends.Add(daisy);

		ICollection<Animal> bestFriends = tweety.BestFriends;

		assertThat(bestFriends).NotEmpty.hasSize(3).containsOnly(birdo, plucky, daisy);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveBestFriendRef()
	  public virtual void testRemoveBestFriendRef()
	  {
		tweety.BestFriends.remove(plucky);

		ICollection<Animal> bestFriends = tweety.BestFriends;

		assertThat(bestFriends).NotEmpty.hasSize(1).containsOnly(birdo);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testClearBestFriendRef()
	  public virtual void testClearBestFriendRef()
	  {
		tweety.BestFriends.Clear();

		ICollection<Animal> bestFriends = tweety.BestFriends;

		assertThat(bestFriends).Empty;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testClearAndAddBestFriendRef()
	  public virtual void testClearAndAddBestFriendRef()
	  {
		tweety.BestFriends.Clear();

		ICollection<Animal> bestFriends = tweety.BestFriends;

		assertThat(bestFriends).Empty;

		bestFriends.Add(daisy);

		assertThat(bestFriends).hasSize(1).containsOnly(daisy);
	  }
	}

}