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
	using ModelElementInstanceImpl = org.camunda.bpm.model.xml.impl.instance.ModelElementInstanceImpl;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using ModelElementTypeBuilder_ModelTypeInstanceProvider = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder_ModelTypeInstanceProvider;
	using ChildElement = org.camunda.bpm.model.xml.type.child.ChildElement;
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.testmodel.TestModelConstants.ELEMENT_NAME_ANIMALS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.testmodel.TestModelConstants.MODEL_NAMESPACE;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class Animals : ModelElementInstanceImpl
	{

	  protected internal static ChildElement<Description> descriptionChild;
	  protected internal static ChildElementCollection<Animal> animalColl;

	  public static void registerType(ModelBuilder modelBuilder)
	  {

		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(Animals), ELEMENT_NAME_ANIMALS).namespaceUri(MODEL_NAMESPACE).instanceProvider(new ModelElementTypeBuilder_ModelTypeInstanceProviderAnonymousInnerClass());

		SequenceBuilder sequence = typeBuilder.sequence();

		descriptionChild = sequence.element(typeof(Description)).build();

		animalColl = sequence.elementCollection(typeof(Animal)).build();

		typeBuilder.build();

	  }

	  private class ModelElementTypeBuilder_ModelTypeInstanceProviderAnonymousInnerClass : ModelElementTypeBuilder_ModelTypeInstanceProvider<Animals>
	  {
		  public Animals newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new Animals(instanceContext);
		  }
	  }

	  public Animals(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual Description Description
	  {
		  get
		  {
			return descriptionChild.getChild(this);
		  }
		  set
		  {
			descriptionChild.setChild(this, value);
		  }
	  }


	  public virtual ICollection<Animal> getAnimals()
	  {
		return animalColl.get(this);
	  }

	}

}