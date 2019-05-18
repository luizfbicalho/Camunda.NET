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
namespace org.camunda.bpm.model.xml.instance
{
	using AbstractModelParser = org.camunda.bpm.model.xml.impl.parser.AbstractModelParser;
	using Gender = org.camunda.bpm.model.xml.testmodel.Gender;
	using TestModelParser = org.camunda.bpm.model.xml.testmodel.TestModelParser;
	using TestModelTest = org.camunda.bpm.model.xml.testmodel.TestModelTest;
	using Animals = org.camunda.bpm.model.xml.testmodel.instance.Animals;
	using Bird = org.camunda.bpm.model.xml.testmodel.instance.Bird;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;
	using Before = org.junit.Before;
	using Test = org.junit.Test;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.testmodel.TestModelConstants.MODEL_NAMESPACE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.runners.Parameterized.Parameters;

	/// <summary>
	/// @author Daniel Meyer
	/// @author Sebastian Menski
	/// </summary>
	public class ModelElementInstanceTest : TestModelTest
	{

	  private Animals animals;
	  private Bird tweety;
	  private Bird donald;
	  private Bird daisy;
	  private Bird hedwig;

	  public ModelElementInstanceTest(string testName, ModelInstance testModelInstance, AbstractModelParser modelParser) : base(testName, testModelInstance, modelParser)
	  {
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters(name="Model {0}") public static java.util.Collection<Object[]> models()
	  public static ICollection<object[]> models()
	  {
		object[][] models = new object[][] {createModel(), parseModel(typeof(ModelElementInstanceTest))};
		return Arrays.asList(models);
	  }

	  private static object[] createModel()
	  {
		TestModelParser modelParser = new TestModelParser();
		ModelInstance modelInstance = modelParser.EmptyModel;

		Animals animals = modelInstance.newInstance(typeof(Animals));
		modelInstance.DocumentElement = animals;

		createBird(modelInstance, "tweety", Gender.Female);
		Bird donald = createBird(modelInstance, "donald", Gender.Male);
		Bird daisy = createBird(modelInstance, "daisy", Gender.Female);
		Bird hedwig = createBird(modelInstance, "hedwig", Gender.Male);

		donald.TextContent = "some text content";
		daisy.TextContent = "\n        some text content with outer line breaks\n    ";
		hedwig.TextContent = "\n        some text content with inner\n        line breaks\n    ";

		return new object[]{"created", modelInstance, modelParser};
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void copyModelInstance()
	  public virtual void copyModelInstance()
	  {
		modelInstance = cloneModelInstance();

		animals = (Animals) modelInstance.DocumentElement;
		tweety = modelInstance.getModelElementById("tweety");
		donald = modelInstance.getModelElementById("donald");
		daisy = modelInstance.getModelElementById("daisy");
		hedwig = modelInstance.getModelElementById("hedwig");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAttribute()
	  public virtual void testAttribute()
	  {
		string tweetyName = tweety.Id + "-name";
		tweety.setAttributeValue("name", tweetyName);
		assertThat(tweety.getAttributeValue("name")).isEqualTo(tweetyName);
		tweety.removeAttribute("name");
		assertThat(tweety.getAttributeValue("name")).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAttributeWithNamespace()
	  public virtual void testAttributeWithNamespace()
	  {
		string tweetyName = tweety.Id + "-name";
		tweety.setAttributeValueNs(MODEL_NAMESPACE, "name", tweetyName);
		assertThat(tweety.getAttributeValue("name")).isEqualTo(tweetyName);
		assertThat(tweety.getAttributeValueNs(MODEL_NAMESPACE, "name")).isEqualTo(tweetyName);
		tweety.removeAttributeNs(MODEL_NAMESPACE, "name");
		assertThat(tweety.getAttributeValue("name")).Null;
		assertThat(tweety.getAttributeValueNs(MODEL_NAMESPACE, "name")).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void TestElementType()
	  public virtual void TestElementType()
	  {
		ModelElementType birdType = modelInstance.Model.getType(typeof(Bird));
		assertThat(tweety.ElementType).isEqualTo(birdType);
		assertThat(donald.ElementType).isEqualTo(birdType);
		assertThat(daisy.ElementType).isEqualTo(birdType);
		assertThat(hedwig.ElementType).isEqualTo(birdType);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void TestParentElement()
	  public virtual void TestParentElement()
	  {
		assertThat(tweety.ParentElement).isEqualTo(animals);
		assertThat(donald.ParentElement).isEqualTo(animals);
		assertThat(daisy.ParentElement).isEqualTo(animals);
		assertThat(hedwig.ParentElement).isEqualTo(animals);

		Bird timmy = modelInstance.newInstance(typeof(Bird));
		timmy.Id = "timmy";
		timmy.Gender = Gender.Male;
		assertThat(timmy.ParentElement).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void TestModelInstance()
	  public virtual void TestModelInstance()
	  {
		assertThat(tweety.ModelInstance).isEqualTo(modelInstance);
		assertThat(donald.ModelInstance).isEqualTo(modelInstance);
		assertThat(daisy.ModelInstance).isEqualTo(modelInstance);
		assertThat(hedwig.ModelInstance).isEqualTo(modelInstance);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReplaceWithElement()
	  public virtual void testReplaceWithElement()
	  {
		Bird timmy = modelInstance.newInstance(typeof(Bird));
		timmy.Id = "timmy";
		timmy.Gender = Gender.Male;

		assertThat(animals.getAnimals()).contains(tweety).doesNotContain(timmy);

		tweety.replaceWithElement(timmy);

		assertThat(animals.getAnimals()).contains(timmy).doesNotContain(tweety);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReplaceRootElement()
	  public virtual void testReplaceRootElement()
	  {
		assertThat(((Animals) modelInstance.DocumentElement).getAnimals()).NotEmpty;
		Animals newAnimals = modelInstance.newInstance(typeof(Animals));
		modelInstance.DocumentElement = newAnimals;
		assertThat(((Animals) modelInstance.DocumentElement).getAnimals()).Empty;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTextContent()
	  public virtual void testTextContent()
	  {
		assertThat(tweety.TextContent).isEqualTo("");
		assertThat(donald.TextContent).isEqualTo("some text content");
		assertThat(daisy.TextContent).isEqualTo("some text content with outer line breaks");
		assertThat(hedwig.TextContent).isEqualTo("some text content with inner\n        line breaks");

		string testContent = "\n test content \n \n \t camunda.org \t    \n   ";
		tweety.TextContent = testContent;
		assertThat(tweety.TextContent).isEqualTo(testContent.Trim());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRawTextContent()
	  public virtual void testRawTextContent()
	  {
		assertThat(tweety.RawTextContent).isEqualTo("");
		assertThat(donald.RawTextContent).isEqualTo("some text content");
		assertThat(daisy.RawTextContent).isEqualTo("\n        some text content with outer line breaks\n    ");
		assertThat(hedwig.RawTextContent).isEqualTo("\n        some text content with inner\n        line breaks\n    ");

		string testContent = "\n test content \n \n \t camunda.org \t    \n   ";
		tweety.TextContent = testContent;
		assertThat(tweety.RawTextContent).isEqualTo(testContent);
	  }

	}

}