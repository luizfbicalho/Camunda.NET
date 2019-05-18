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
namespace org.camunda.bpm.model.xml
{
	using TestModel = org.camunda.bpm.model.xml.testmodel.TestModel;
	using org.camunda.bpm.model.xml.testmodel.instance;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;
	using Before = org.junit.Before;
	using Test = org.junit.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
	using static org.camunda.bpm.model.xml.testmodel.TestModelConstants;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class ModelTest
	{

	  private Model model;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void createModel()
	  public virtual void createModel()
	  {
		model = TestModel.TestModel;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTypes()
	  public virtual void testGetTypes()
	  {
		ICollection<ModelElementType> types = model.Types;
		assertThat(types).NotEmpty;
		assertThat(types).contains(model.getType(typeof(Animals)), model.getType(typeof(Animal)), model.getType(typeof(FlyingAnimal)), model.getType(typeof(Bird)), model.getType(typeof(RelationshipDefinition)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetType()
	  public virtual void testGetType()
	  {
		ModelElementType flyingAnimalType = model.getType(typeof(FlyingAnimal));
		assertThat(flyingAnimalType.InstanceType).isEqualTo(typeof(FlyingAnimal));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTypeForName()
	  public virtual void testGetTypeForName()
	  {
		ModelElementType birdType = model.getTypeForName(ELEMENT_NAME_BIRD);
		assertThat(birdType).Null;
		birdType = model.getTypeForName(MODEL_NAMESPACE, ELEMENT_NAME_BIRD);
		assertThat(birdType.InstanceType).isEqualTo(typeof(Bird));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetModelName()
	  public virtual void testGetModelName()
	  {
		assertThat(model.ModelName).isEqualTo(MODEL_NAME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEqual()
	  public virtual void testEqual()
	  {
		assertThat(model).isNotEqualTo(null);
		assertThat(model).isNotEqualTo(new object());
		Model otherModel = ModelBuilder.createInstance("Other Model").build();
		assertThat(model).isNotEqualTo(otherModel);
		otherModel = ModelBuilder.createInstance(MODEL_NAME).build();
		assertThat(model).isEqualTo(otherModel);
		otherModel = ModelBuilder.createInstance(null).build();
		assertThat(otherModel).isNotEqualTo(model);
		assertThat(model).isEqualTo(model);
	  }
	}

}