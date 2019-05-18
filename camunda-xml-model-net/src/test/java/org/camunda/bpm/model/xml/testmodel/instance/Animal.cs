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
//	import static org.camunda.bpm.model.xml.testmodel.TestModelConstants.ATTRIBUTE_NAME_AGE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.testmodel.TestModelConstants.ATTRIBUTE_NAME_BEST_FRIEND_REFS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.testmodel.TestModelConstants.ATTRIBUTE_NAME_FATHER;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.testmodel.TestModelConstants.ATTRIBUTE_NAME_GENDER;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.testmodel.TestModelConstants.ATTRIBUTE_NAME_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.testmodel.TestModelConstants.ATTRIBUTE_NAME_IS_ENDANGERED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.testmodel.TestModelConstants.ATTRIBUTE_NAME_MOTHER;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.testmodel.TestModelConstants.ATTRIBUTE_NAME_NAME;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.testmodel.TestModelConstants.MODEL_NAMESPACE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.testmodel.TestModelConstants.TYPE_NAME_ANIMAL;

	using ModelElementInstanceImpl = org.camunda.bpm.model.xml.impl.instance.ModelElementInstanceImpl;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;
	using AttributeReference = org.camunda.bpm.model.xml.type.reference.AttributeReference;
	using AttributeReferenceCollection = org.camunda.bpm.model.xml.type.reference.AttributeReferenceCollection;
	using ElementReferenceCollection = org.camunda.bpm.model.xml.type.reference.ElementReferenceCollection;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public abstract class Animal : ModelElementInstanceImpl
	{

	  protected internal static Attribute<string> idAttr;
	  protected internal static Attribute<string> nameAttr;
	  protected internal static AttributeReference<Animal> fatherRef;
	  protected internal static AttributeReference<Animal> motherRef;
	  protected internal static Attribute<bool> isEndangeredAttr;
	  protected internal static Attribute<Gender> genderAttr;
	  protected internal static Attribute<int> ageAttr;
	  protected internal static AttributeReferenceCollection<Animal> bestFriendsRefCollection;
	  protected internal static ChildElementCollection<RelationshipDefinition> relationshipDefinitionsColl;
	  protected internal static ElementReferenceCollection<RelationshipDefinition, RelationshipDefinitionRef> relationshipDefinitionRefsColl;

	  public static void registerType(ModelBuilder modelBuilder)
	  {

		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(Animal), TYPE_NAME_ANIMAL).namespaceUri(MODEL_NAMESPACE).abstractType();

		idAttr = typeBuilder.stringAttribute(ATTRIBUTE_NAME_ID).idAttribute().build();

		nameAttr = typeBuilder.stringAttribute(ATTRIBUTE_NAME_NAME).build();

		fatherRef = typeBuilder.stringAttribute(ATTRIBUTE_NAME_FATHER).qNameAttributeReference(typeof(Animal)).build();

		motherRef = typeBuilder.stringAttribute(ATTRIBUTE_NAME_MOTHER).idAttributeReference(typeof(Animal)).build();

		isEndangeredAttr = typeBuilder.booleanAttribute(ATTRIBUTE_NAME_IS_ENDANGERED).defaultValue(false).build();

		genderAttr = typeBuilder.enumAttribute(ATTRIBUTE_NAME_GENDER, typeof(Gender)).required().build();

		ageAttr = typeBuilder.integerAttribute(ATTRIBUTE_NAME_AGE).build();

		bestFriendsRefCollection = typeBuilder.stringAttribute(ATTRIBUTE_NAME_BEST_FRIEND_REFS).idAttributeReferenceCollection(typeof(Animal), typeof(AnimalAttributeReferenceCollection)).build();

		SequenceBuilder sequence = typeBuilder.sequence();

		relationshipDefinitionsColl = sequence.elementCollection(typeof(RelationshipDefinition)).build();

		relationshipDefinitionRefsColl = sequence.elementCollection(typeof(RelationshipDefinitionRef)).qNameElementReferenceCollection(typeof(RelationshipDefinition)).build();

		typeBuilder.build();
	  }

	  public Animal(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual string Id
	  {
		  get
		  {
			return idAttr.getValue(this);
		  }
		  set
		  {
			idAttr.setValue(this, value);
		  }
	  }


	  public virtual string Name
	  {
		  get
		  {
			return nameAttr.getValue(this);
		  }
		  set
		  {
			nameAttr.setValue(this, value);
		  }
	  }


	  public virtual Animal Father
	  {
		  get
		  {
			return fatherRef.getReferenceTargetElement(this);
		  }
		  set
		  {
			fatherRef.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual Animal Mother
	  {
		  get
		  {
			return motherRef.getReferenceTargetElement(this);
		  }
		  set
		  {
			motherRef.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual bool? Endangered
	  {
		  get
		  {
			return isEndangeredAttr.getValue(this);
		  }
	  }

	  public virtual bool IsEndangered
	  {
		  set
		  {
			isEndangeredAttr.setValue(this, value);
		  }
	  }

	  public virtual Gender Gender
	  {
		  get
		  {
			return genderAttr.getValue(this);
		  }
		  set
		  {
			genderAttr.setValue(this, value);
		  }
	  }


	  public virtual int? getAge()
	  {
		return ageAttr.getValue(this);
	  }

	  public virtual void setAge(int age)
	  {
		ageAttr.setValue(this, age);
	  }

	  public virtual ICollection<RelationshipDefinition> RelationshipDefinitions
	  {
		  get
		  {
			return relationshipDefinitionsColl.get(this);
		  }
	  }

	  public virtual ICollection<RelationshipDefinition> RelationshipDefinitionRefs
	  {
		  get
		  {
			return relationshipDefinitionRefsColl.getReferenceTargetElements(this);
		  }
	  }

	  public virtual ICollection<RelationshipDefinitionRef> RelationshipDefinitionRefElements
	  {
		  get
		  {
			return relationshipDefinitionRefsColl.ReferenceSourceCollection.get(this);
		  }
	  }

	  public virtual ICollection<Animal> BestFriends
	  {
		  get
		  {
			return bestFriendsRefCollection.getReferenceTargetElements(this);
		  }
	  }

	}

}