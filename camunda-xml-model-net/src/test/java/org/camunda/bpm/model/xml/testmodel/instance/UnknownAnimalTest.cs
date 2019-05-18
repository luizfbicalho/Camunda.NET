using System;
using System.Collections.Generic;
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
namespace org.camunda.bpm.model.xml.testmodel.instance
{
	using ModelInstanceImpl = org.camunda.bpm.model.xml.impl.ModelInstanceImpl;
	using AbstractModelParser = org.camunda.bpm.model.xml.impl.parser.AbstractModelParser;
	using DomDocument = org.camunda.bpm.model.xml.instance.DomDocument;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Test = org.junit.Test;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.testmodel.TestModelConstants.MODEL_NAMESPACE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class UnknownAnimalTest
	{

	  private AbstractModelParser modelParser;
	  private ModelInstance modelInstance;
	  private ModelElementInstance wanda;
	  private ModelElementInstance flipper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void parseModel()
	  public virtual void parseModel()
	  {
		modelParser = new TestModelParser();
		string testXml = this.GetType().Name + ".xml";
		Stream testXmlAsStream = this.GetType().getResourceAsStream(testXml);
		modelInstance = modelParser.parseModelFromStream(testXmlAsStream);
		wanda = modelInstance.getModelElementById("wanda");
		flipper = modelInstance.getModelElementById("flipper");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void validateModel()
	  public virtual void validateModel()
	  {
		DomDocument document = modelInstance.Document;
		modelParser.validateModel(document);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetUnknownAnimalById()
	  public virtual void testGetUnknownAnimalById()
	  {
		assertThat(wanda).NotNull;
		assertThat(wanda.getAttributeValue("id")).isEqualTo("wanda");
		assertThat(wanda.getAttributeValue("gender")).isEqualTo("Female");
		assertThat(wanda.getAttributeValue("species")).isEqualTo("fish");

		assertThat(flipper).NotNull;
		assertThat(flipper.getAttributeValue("id")).isEqualTo("flipper");
		assertThat(flipper.getAttributeValue("gender")).isEqualTo("Male");
		assertThat(flipper.getAttributeValue("species")).isEqualTo("dolphin");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetUnknownAnimalByType()
	  public virtual void testGetUnknownAnimalByType()
	  {
		ModelInstanceImpl modelInstanceImpl = (ModelInstanceImpl) modelInstance;
		ModelElementType unknownAnimalType = modelInstanceImpl.registerGenericType(MODEL_NAMESPACE, "unknownAnimal");
		IList<ModelElementInstance> unknownAnimals = new List<ModelElementInstance>(modelInstance.getModelElementsByType(unknownAnimalType));
		assertThat(unknownAnimals).hasSize(2);

		ModelElementInstance wanda = unknownAnimals[0];
		assertThat(wanda.getAttributeValue("id")).isEqualTo("wanda");
		assertThat(wanda.getAttributeValue("gender")).isEqualTo("Female");
		assertThat(wanda.getAttributeValue("species")).isEqualTo("fish");

		ModelElementInstance flipper = unknownAnimals[1];
		assertThat(flipper.getAttributeValue("id")).isEqualTo("flipper");
		assertThat(flipper.getAttributeValue("gender")).isEqualTo("Male");
		assertThat(flipper.getAttributeValue("species")).isEqualTo("dolphin");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddUnknownAnimal()
	  public virtual void testAddUnknownAnimal()
	  {
		ModelInstanceImpl modelInstanceImpl = (ModelInstanceImpl) modelInstance;
		ModelElementType unknownAnimalType = modelInstanceImpl.registerGenericType(MODEL_NAMESPACE, "unknownAnimal");
		ModelElementType animalsType = modelInstance.Model.getType(typeof(Animals));
		ModelElementType animalType = modelInstance.Model.getType(typeof(Animal));

		ModelElementInstance unknownAnimal = modelInstance.newInstance(unknownAnimalType);
		assertThat(unknownAnimal).NotNull;
		unknownAnimal.setAttributeValue("id", "new-animal", true);
		unknownAnimal.setAttributeValue("gender", "Unknown");
		unknownAnimal.setAttributeValue("species", "unknown");

		ModelElementInstance animals = modelInstance.getModelElementsByType(animalsType).GetEnumerator().next();
		IList<ModelElementInstance> childElementsByType = new List<ModelElementInstance>(animals.getChildElementsByType(animalType));
		animals.insertElementAfter(unknownAnimal, childElementsByType[2]);
		assertThat(animals.getChildElementsByType(unknownAnimalType)).hasSize(3);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetUnknownAttribute()
	  public virtual void testGetUnknownAttribute()
	  {
		assertThat(flipper.getAttributeValue("famous")).isEqualTo("true");

		assertThat(wanda.getAttributeValue("famous")).isNotEqualTo("true");
		wanda.setAttributeValue("famous", "true");
		assertThat(wanda.getAttributeValue("famous")).isEqualTo("true");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddRelationshipDefinitionToUnknownAnimal()
	  public virtual void testAddRelationshipDefinitionToUnknownAnimal()
	  {
		RelationshipDefinition friendRelationshipDefinition = modelInstance.newInstance(typeof(FriendRelationshipDefinition));
		friendRelationshipDefinition.Id = "friend-relationship";
		friendRelationshipDefinition.setAttributeValue("animalRef", flipper.getAttributeValue("id"));

		try
		{
		  wanda.addChildElement(friendRelationshipDefinition);
		  fail("Cannot add relationship definition to UnknownAnimal cause no child types are defined");
		}
		catch (Exception e)
		{
		  assertThat(e).isInstanceOf(typeof(ModelException));
		}

		wanda.insertElementAfter(friendRelationshipDefinition, null);

		Animal tweety = modelInstance.getModelElementById("tweety");
		RelationshipDefinition childRelationshipDefinition = modelInstance.newInstance(typeof(ChildRelationshipDefinition));
		childRelationshipDefinition.Id = "child-relationship";
		childRelationshipDefinition.Animal = tweety;

		wanda.insertElementAfter(childRelationshipDefinition, friendRelationshipDefinition);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddChildToUnknownAnimal()
	  public virtual void testAddChildToUnknownAnimal()
	  {
		assertThat(wanda.getChildElementsByType(flipper.ElementType)).hasSize(0);
		wanda.insertElementAfter(flipper, null);
		assertThat(wanda.getChildElementsByType(flipper.ElementType)).hasSize(1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveChildOfUnknownAnimal()
	  public virtual void testRemoveChildOfUnknownAnimal()
	  {
		assertThat(wanda.removeChildElement(flipper)).False;
		wanda.insertElementAfter(flipper, null);
		assertThat(wanda.removeChildElement(flipper)).True;
		assertThat(wanda.getChildElementsByType(flipper.ElementType)).Empty;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReplaceChildOfUnknownAnimal()
	  public virtual void testReplaceChildOfUnknownAnimal()
	  {
		ModelElementInstance yogi = modelInstance.newInstance(flipper.ElementType);
		yogi.setAttributeValue("id", "yogi-bear", true);
		yogi.setAttributeValue("gender", "Male");
		yogi.setAttributeValue("species", "bear");

		assertThat(wanda.getChildElementsByType(flipper.ElementType)).Empty;
		wanda.insertElementAfter(flipper, null);
		assertThat(wanda.getChildElementsByType(flipper.ElementType)).hasSize(1);
		wanda.replaceChildElement(flipper, yogi);
		assertThat(wanda.getChildElementsByType(flipper.ElementType)).hasSize(1);
		assertThat(wanda.getChildElementsByType(flipper.ElementType).GetEnumerator().next()).isEqualTo(yogi);
	  }

	}

}