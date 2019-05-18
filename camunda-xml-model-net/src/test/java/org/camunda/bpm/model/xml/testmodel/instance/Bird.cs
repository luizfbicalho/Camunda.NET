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
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using ModelElementTypeBuilder_ModelTypeInstanceProvider = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder_ModelTypeInstanceProvider;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using ChildElement = org.camunda.bpm.model.xml.type.child.ChildElement;
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;
	using ElementReference = org.camunda.bpm.model.xml.type.reference.ElementReference;
	using ElementReferenceCollection = org.camunda.bpm.model.xml.type.reference.ElementReferenceCollection;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.testmodel.TestModelConstants.ELEMENT_NAME_BIRD;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.testmodel.TestModelConstants.MODEL_NAMESPACE;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class Bird : FlyingAnimal
	{

	  protected internal static ChildElementCollection<Egg> eggColl;
	  protected internal static ElementReference<Bird, SpouseRef> spouseRefsColl;
	  protected internal static ElementReferenceCollection<Egg, GuardEgg> guardEggRefCollection;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal static Attribute<bool> canHazExtendedWings_Renamed;
	  protected internal static ChildElement<Wings> wings;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(Bird), ELEMENT_NAME_BIRD).namespaceUri(MODEL_NAMESPACE).extendsType(typeof(FlyingAnimal)).instanceProvider(new ModelElementTypeBuilder_ModelTypeInstanceProviderAnonymousInnerClass());

		SequenceBuilder sequence = typeBuilder.sequence();

		eggColl = sequence.elementCollection(typeof(Egg)).minOccurs(0).maxOccurs(6).build();

		spouseRefsColl = sequence.element(typeof(SpouseRef)).qNameElementReference(typeof(Bird)).build();

		guardEggRefCollection = sequence.elementCollection(typeof(GuardEgg)).idsElementReferenceCollection(typeof(Egg)).build();

		canHazExtendedWings_Renamed = typeBuilder.booleanAttribute("canHazExtendedWings").@namespace(TestModelConstants.NEWER_NAMESPACE).build();

		wings = sequence.element(typeof(Wings)).build();

		typeBuilder.build();

	  }

	  private class ModelElementTypeBuilder_ModelTypeInstanceProviderAnonymousInnerClass : ModelElementTypeBuilder_ModelTypeInstanceProvider<Bird>
	  {
		  public Bird newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new Bird(instanceContext);
		  }
	  }

	  public Bird(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual ICollection<Egg> Eggs
	  {
		  get
		  {
			return eggColl.get(this);
		  }
	  }

	  public virtual Bird Spouse
	  {
		  get
		  {
			return spouseRefsColl.getReferenceTargetElement(this);
		  }
		  set
		  {
			spouseRefsColl.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual void removeSpouse()
	  {
		spouseRefsColl.clearReferenceTargetElement(this);
	  }

	  public virtual SpouseRef SpouseRef
	  {
		  get
		  {
			return spouseRefsColl.getReferenceSource(this);
		  }
	  }

	  public virtual ICollection<Egg> GuardedEggs
	  {
		  get
		  {
			return guardEggRefCollection.getReferenceTargetElements(this);
		  }
	  }

	  public virtual ICollection<GuardEgg> GuardedEggRefs
	  {
		  get
		  {
			return guardEggRefCollection.ReferenceSourceCollection.get(this);
		  }
	  }

	  public virtual bool? canHazExtendedWings()
	  {
		return canHazExtendedWings_Renamed.getValue(this);
	  }

	  public virtual bool CanHazExtendedWings
	  {
		  set
		  {
			canHazExtendedWings_Renamed.setValue(this, value);
		  }
	  }

	  public virtual Wings Wings
	  {
		  get
		  {
			return wings.getChild(this);
		  }
		  set
		  {
			wings.setChild(this, value);
		  }
	  }


	}

}