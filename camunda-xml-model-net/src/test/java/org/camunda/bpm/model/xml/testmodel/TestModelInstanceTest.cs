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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using Animal = org.camunda.bpm.model.xml.testmodel.instance.Animal;
	using Animals = org.camunda.bpm.model.xml.testmodel.instance.Animals;
	using Bird = org.camunda.bpm.model.xml.testmodel.instance.Bird;
	using Test = org.junit.Test;

	public class TestModelInstanceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testClone() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testClone()
	  {
		ModelInstance modelInstance = (new TestModelParser()).EmptyModel;

		Animals animals = modelInstance.newInstance(typeof(Animals));
		modelInstance.DocumentElement = animals;

		Animal animal = modelInstance.newInstance(typeof(Bird));
		animal.Id = "TestId";
		animals.addChildElement(animal);

		ModelInstance cloneInstance = modelInstance.clone();
		getFirstAnimal(cloneInstance).Id = "TestId2";

		assertThat(getFirstAnimal(modelInstance).Id, @is(equalTo("TestId")));
		assertThat(getFirstAnimal(cloneInstance).Id, @is(equalTo("TestId2")));
	  }

	  protected internal virtual Animal getFirstAnimal(ModelInstance modelInstance)
	  {
		Animals animals = (Animals) modelInstance.DocumentElement;
		return animals.getAnimals().GetEnumerator().next();
	  }

	}

}