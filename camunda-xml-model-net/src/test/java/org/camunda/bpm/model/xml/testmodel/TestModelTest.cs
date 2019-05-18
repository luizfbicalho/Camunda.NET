using System;
using System.IO;

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
namespace org.camunda.bpm.model.xml.testmodel
{
	using ModelInstanceImpl = org.camunda.bpm.model.xml.impl.ModelInstanceImpl;
	using AbstractModelParser = org.camunda.bpm.model.xml.impl.parser.AbstractModelParser;
	using org.camunda.bpm.model.xml.testmodel.instance;
	using After = org.junit.After;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public abstract class TestModelTest
	public abstract class TestModelTest
	{

	  protected internal readonly string testName;
	  private readonly ModelInstance testModelInstance;
	  private readonly AbstractModelParser modelParser;

	  // cloned model instance for every test method (see subclasses)
	  protected internal ModelInstance modelInstance;

	  public TestModelTest(string testName, ModelInstance testModelInstance, AbstractModelParser modelParser)
	  {
		this.testName = testName;
		this.testModelInstance = testModelInstance;
		this.modelParser = modelParser;
	  }

	  public virtual ModelInstance cloneModelInstance()
	  {
		return testModelInstance.clone();
	  }

	  protected internal static object[] parseModel(Type test)
	  {
		TestModelParser modelParser = new TestModelParser();
		string testXml = test.Name + ".xml";
		Stream testXmlAsStream = test.getResourceAsStream(testXml);
		ModelInstance modelInstance = modelParser.parseModelFromStream(testXmlAsStream);
		return new object[]{"parsed", modelInstance, modelParser};
	  }

	  public static Bird createBird(ModelInstance modelInstance, string id, Gender gender)
	  {
		Bird bird = modelInstance.newInstance(typeof(Bird), id);
		bird.Gender = gender;
		Animals animals = (Animals) modelInstance.DocumentElement;
		animals.getAnimals().Add(bird);
		return bird;
	  }

	  protected internal static RelationshipDefinition createRelationshipDefinition(ModelInstance modelInstance, Animal animalInRelationshipWith, Type relationshipDefinitionClass)
	  {
		RelationshipDefinition relationshipDefinition = modelInstance.newInstance(relationshipDefinitionClass, "relationship-" + animalInRelationshipWith.Id);
		relationshipDefinition.Animal = animalInRelationshipWith;
		return relationshipDefinition;
	  }

	  public static void addRelationshipDefinition(Animal animalWithRelationship, RelationshipDefinition relationshipDefinition)
	  {
		Animal animalInRelationshipWith = relationshipDefinition.Animal;
		relationshipDefinition.Id = animalWithRelationship.Id + "-" + animalInRelationshipWith.Id;
		animalWithRelationship.RelationshipDefinitions.Add(relationshipDefinition);
	  }

	  public static Egg createEgg(ModelInstance modelInstance, string id)
	  {
		Egg egg = modelInstance.newInstance(typeof(Egg), id);
		return egg;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void validateModel()
	  public virtual void validateModel()
	  {
		modelParser.validateModel(modelInstance.Document);
	  }
	}

}