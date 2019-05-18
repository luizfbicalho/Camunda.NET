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
namespace org.camunda.bpm.model.xml.type.attribute
{
	using AbstractModelParser = org.camunda.bpm.model.xml.impl.parser.AbstractModelParser;
	using AttributeImpl = org.camunda.bpm.model.xml.impl.type.attribute.AttributeImpl;
	using Gender = org.camunda.bpm.model.xml.testmodel.Gender;
	using TestModelParser = org.camunda.bpm.model.xml.testmodel.TestModelParser;
	using TestModelTest = org.camunda.bpm.model.xml.testmodel.TestModelTest;
	using Animal = org.camunda.bpm.model.xml.testmodel.instance.Animal;
	using AnimalTest = org.camunda.bpm.model.xml.testmodel.instance.AnimalTest;
	using Animals = org.camunda.bpm.model.xml.testmodel.instance.Animals;
	using Bird = org.camunda.bpm.model.xml.testmodel.instance.Bird;
	using Before = org.junit.Before;
	using Test = org.junit.Test;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.test.assertions.ModelAssertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.runners.Parameterized.Parameters;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class AttributeTest : TestModelTest
	{

	  private Bird tweety;
	  private Attribute<string> idAttribute;
	  private Attribute<string> nameAttribute;
	  private Attribute<string> fatherAttribute;

	  public AttributeTest(string testName, ModelInstance testModelInstance, AbstractModelParser modelParser) : base(testName, testModelInstance, modelParser)
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

		createBird(modelInstance, "tweety", Gender.Female);

		return new object[]{"created", modelInstance, modelParser};
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before @SuppressWarnings("unchecked") public void copyModelInstance()
	  public virtual void copyModelInstance()
	  {
		modelInstance = cloneModelInstance();

		tweety = modelInstance.getModelElementById("tweety");

		ModelElementType animalType = modelInstance.Model.getType(typeof(Animal));
		idAttribute = (Attribute<string>) animalType.getAttribute("id");
		nameAttribute = (Attribute<string>) animalType.getAttribute("name");
		fatherAttribute = (Attribute<string>) animalType.getAttribute("father");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOwningElementType()
	  public virtual void testOwningElementType()
	  {
		ModelElementType animalType = modelInstance.Model.getType(typeof(Animal));

		assertThat(idAttribute).hasOwningElementType(animalType);
		assertThat(nameAttribute).hasOwningElementType(animalType);
		assertThat(fatherAttribute).hasOwningElementType(animalType);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetAttributeValue()
	  public virtual void testSetAttributeValue()
	  {
		string identifier = "new-" + tweety.Id;
		idAttribute.setValue(tweety, identifier);
		assertThat(idAttribute).hasValue(tweety, identifier);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetAttributeValueWithoutUpdateReference()
	  public virtual void testSetAttributeValueWithoutUpdateReference()
	  {
		string identifier = "new-" + tweety.Id;
		idAttribute.setValue(tweety, identifier, false);
		assertThat(idAttribute).hasValue(tweety, identifier);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetDefaultValue()
	  public virtual void testSetDefaultValue()
	  {
		string defaultName = "default-name";
		assertThat(tweety.Name).Null;
		assertThat(nameAttribute).hasNoDefaultValue();

		((AttributeImpl<string>) nameAttribute).DefaultValue = defaultName;
		assertThat(nameAttribute).hasDefaultValue(defaultName);
		assertThat(tweety.Name).isEqualTo(defaultName);

		tweety.Name = "not-" + defaultName;
		assertThat(tweety.Name).isNotEqualTo(defaultName);

		tweety.removeAttribute("name");
		assertThat(tweety.Name).isEqualTo(defaultName);
		((AttributeImpl<string>) nameAttribute).DefaultValue = null;
		assertThat(nameAttribute).hasNoDefaultValue();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRequired()
	  public virtual void testRequired()
	  {
		tweety.removeAttribute("name");
		assertThat(nameAttribute).Optional;

		((AttributeImpl<string>) nameAttribute).Required = true;
		assertThat(nameAttribute).Required;

		((AttributeImpl<string>) nameAttribute).Required = false;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetNamespaceUri()
	  public virtual void testSetNamespaceUri()
	  {
		string testNamespace = "http://camunda.org/test";

		((AttributeImpl<string>) idAttribute).NamespaceUri = testNamespace;
		assertThat(idAttribute).hasNamespaceUri(testNamespace);

		((AttributeImpl<string>) idAttribute).NamespaceUri = null;
		assertThat(idAttribute).hasNoNamespaceUri();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIdAttribute()
	  public virtual void testIdAttribute()
	  {
		assertThat(idAttribute).IdAttribute;
		assertThat(nameAttribute).NotIdAttribute;
		assertThat(fatherAttribute).NotIdAttribute;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAttributeName()
	  public virtual void testAttributeName()
	  {
		assertThat(idAttribute).hasAttributeName("id");
		assertThat(nameAttribute).hasAttributeName("name");
		assertThat(fatherAttribute).hasAttributeName("father");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveAttribute()
	  public virtual void testRemoveAttribute()
	  {
		tweety.Name = "test";
		assertThat(tweety.Name).NotNull;
		assertThat(nameAttribute).hasValue(tweety);

		((AttributeImpl<string>) nameAttribute).removeAttribute(tweety);
		assertThat(tweety.Name).Null;
		assertThat(nameAttribute).hasNoValue(tweety);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIncomingReferences()
	  public virtual void testIncomingReferences()
	  {
		assertThat(idAttribute).hasIncomingReferences();
		assertThat(nameAttribute).hasNoIncomingReferences();
		assertThat(fatherAttribute).hasNoIncomingReferences();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOutgoingReferences()
	  public virtual void testOutgoingReferences()
	  {
		assertThat(idAttribute).hasNoOutgoingReferences();
		assertThat(nameAttribute).hasNoOutgoingReferences();
		assertThat(fatherAttribute).hasOutgoingReferences();
	  }

	}

}