using System;

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
namespace org.camunda.bpm.model.xml.type
{
	using ModelTypeException = org.camunda.bpm.model.xml.impl.util.ModelTypeException;
	using TestModelParser = org.camunda.bpm.model.xml.testmodel.TestModelParser;
	using org.camunda.bpm.model.xml.testmodel.instance;
	using Before = org.junit.Before;
	using Test = org.junit.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.test.assertions.ModelAssertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.testmodel.TestModelConstants.MODEL_NAMESPACE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class ModelElementTypeTest
	{

	  private ModelInstance modelInstance;
	  private Model model;
	  private ModelElementType animalsType;
	  private ModelElementType animalType;
	  private ModelElementType flyingAnimalType;
	  private ModelElementType birdType;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void getTypes()
	  public virtual void getTypes()
	  {
		TestModelParser modelParser = new TestModelParser();
		modelInstance = modelParser.EmptyModel;
		model = modelInstance.Model;
		animalsType = model.getType(typeof(Animals));
		animalType = model.getType(typeof(Animal));
		flyingAnimalType = model.getType(typeof(FlyingAnimal));
		birdType = model.getType(typeof(Bird));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTypeName()
	  public virtual void testTypeName()
	  {
		assertThat(animalsType).hasTypeName("animals");
		assertThat(animalType).hasTypeName("animal");
		assertThat(flyingAnimalType).hasTypeName("flyingAnimal");
		assertThat(birdType).hasTypeName("bird");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTypeNamespace()
	  public virtual void testTypeNamespace()
	  {
		assertThat(animalsType).hasTypeNamespace(MODEL_NAMESPACE);
		assertThat(animalType).hasTypeNamespace(MODEL_NAMESPACE);
		assertThat(flyingAnimalType).hasTypeNamespace(MODEL_NAMESPACE);
		assertThat(birdType).hasTypeNamespace(MODEL_NAMESPACE);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInstanceType()
	  public virtual void testInstanceType()
	  {
		assertThat(animalsType).hasInstanceType(typeof(Animals));
		assertThat(animalType).hasInstanceType(typeof(Animal));
		assertThat(flyingAnimalType).hasInstanceType(typeof(FlyingAnimal));
		assertThat(birdType).hasInstanceType(typeof(Bird));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAttributes()
	  public virtual void testAttributes()
	  {
		assertThat(animalsType).hasNoAttributes();
		assertThat(animalType).hasAttributes("id", "name", "father", "mother", "isEndangered", "gender", "age");
		assertThat(flyingAnimalType).hasAttributes("wingspan");
		assertThat(birdType).hasAttributes("canHazExtendedWings");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBaseType()
	  public virtual void testBaseType()
	  {
		assertThat(animalsType).extendsNoType();
		assertThat(animalType).extendsNoType();
		assertThat(flyingAnimalType).extendsType(animalType);
		assertThat(birdType).extendsType(flyingAnimalType);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAbstractType()
	  public virtual void testAbstractType()
	  {
		assertThat(animalsType).NotAbstract;
		assertThat(animalType).Abstract;
		assertThat(flyingAnimalType).Abstract;
		assertThat(birdType).NotAbstract;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExtendingTypes()
	  public virtual void testExtendingTypes()
	  {
		assertThat(animalsType).NotExtended;
		assertThat(animalType).isExtendedBy(flyingAnimalType).isNotExtendedBy(birdType);
		assertThat(flyingAnimalType).isExtendedBy(birdType);
		assertThat(birdType).NotExtended;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testModel()
	  public virtual void testModel()
	  {
		assertThat(animalsType).isPartOfModel(model);
		assertThat(animalType).isPartOfModel(model);
		assertThat(flyingAnimalType).isPartOfModel(model);
		assertThat(birdType).isPartOfModel(model);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInstances()
	  public virtual void testInstances()
	  {
		assertThat(animalsType.getInstances(modelInstance)).Empty;
		assertThat(animalType.getInstances(modelInstance)).Empty;
		assertThat(flyingAnimalType.getInstances(modelInstance)).Empty;
		assertThat(birdType.getInstances(modelInstance)).Empty;

		Animals animals = (Animals) animalsType.newInstance(modelInstance);
		modelInstance.DocumentElement = animals;

		try
		{
		  animalType.newInstance(modelInstance);
		  fail("Animal is a abstract type and not instance can be created.");
		}
		catch (Exception e)
		{
		  assertThat(e).isInstanceOf(typeof(ModelTypeException));
		}

		try
		{
		  flyingAnimalType.newInstance(modelInstance);
		  fail("Flying animal is a abstract type and not instance can be created.");
		}
		catch (Exception e)
		{
		  assertThat(e).isInstanceOf(typeof(ModelTypeException));
		}

		animals.getAnimals().Add((Animal) birdType.newInstance(modelInstance));
		animals.getAnimals().Add((Animal) birdType.newInstance(modelInstance));
		animals.getAnimals().Add((Animal) birdType.newInstance(modelInstance));

		assertThat(animalsType.getInstances(modelInstance)).hasSize(1);
		assertThat(animalType.getInstances(modelInstance)).Empty;
		assertThat(flyingAnimalType.getInstances(modelInstance)).Empty;
		assertThat(birdType.getInstances(modelInstance)).hasSize(3);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testChildElementTypes()
	  public virtual void testChildElementTypes()
	  {
		ModelElementType relationshipDefinitionType = model.getType(typeof(RelationshipDefinition));
		ModelElementType relationshipDefinitionRefType = model.getType(typeof(RelationshipDefinitionRef));
		ModelElementType flightPartnerRefType = model.getType(typeof(FlightPartnerRef));
		ModelElementType eggType = model.getType(typeof(Egg));
		ModelElementType spouseRefType = model.getType(typeof(SpouseRef));

		assertThat(animalsType).hasChildElements(animalType);
		assertThat(animalType).hasChildElements(relationshipDefinitionType, relationshipDefinitionRefType);
		assertThat(flyingAnimalType).hasChildElements(flightPartnerRefType);
		assertThat(birdType).hasChildElements(eggType, spouseRefType);
	  }

	}

}