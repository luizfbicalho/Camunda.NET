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
	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public abstract class TestModelConstants
	{

	  public const string MODEL_NAME = "animals";
	  public const string MODEL_NAMESPACE = "http://camunda.org/animals";
	  public const string NEWER_NAMESPACE = "http://camunda.org/electronic";

	  public const string TYPE_NAME_ANIMAL = "animal";
	  public const string TYPE_NAME_FLYING_ANIMAL = "flyingAnimal";
	  public const string TYPE_NAME_CHILD_RELATIONSHIP_DEFINITION = "childRelationshipDefinition";
	  public const string TYPE_NAME_FRIEND_RELATIONSHIP_DEFINITION = "friendRelationshipDefinition";
	  public const string TYPE_NAME_RELATIONSHIP_DEFINITION = "relationshipDefinition";
	  public const string TYPE_NAME_WINGS = "wings";

	  public const string ELEMENT_NAME_ANIMALS = "animals";
	  public const string ELEMENT_NAME_BIRD = "bird";
	  public const string ELEMENT_NAME_RELATIONSHIP_DEFINITION_REF = "relationshipDefinitionRef";
	  public const string ELEMENT_NAME_FLIGHT_PARTNER_REF = "flightPartnerRef";
	  public const string ELEMENT_NAME_FLIGHT_INSTRUCTOR = "flightInstructor";
	  public const string ELEMENT_NAME_SPOUSE_REF = "spouseRef";
	  public const string ELEMENT_NAME_EGG = "egg";
	  public const string ELEMENT_NAME_ANIMAL_REFERENCE = "animalReference";
	  public const string ELEMENT_NAME_GUARDIAN = "guardian";
	  public const string ELEMENT_NAME_MOTHER = "mother";
	  public const string ELEMENT_NAME_GUARD_EGG = "guardEgg";
	  public const string ELEMENT_NAME_DESCRIPTION = "description";

	  public const string ATTRIBUTE_NAME_ID = "id";
	  public const string ATTRIBUTE_NAME_NAME = "name";
	  public const string ATTRIBUTE_NAME_FATHER = "father";
	  public const string ATTRIBUTE_NAME_MOTHER = "mother";
	  public const string ATTRIBUTE_NAME_IS_ENDANGERED = "isEndangered";
	  public const string ATTRIBUTE_NAME_GENDER = "gender";
	  public const string ATTRIBUTE_NAME_AGE = "age";
	  public const string ATTRIBUTE_NAME_BEST_FRIEND_REFS = "bestFriendRefs";
	  public const string ATTRIBUTE_NAME_ANIMAL_REF = "animalRef";
	  public const string ATTRIBUTE_NAME_WINGSPAN = "wingspan";

	  private TestModelConstants()
	  {

	  }

	}

}