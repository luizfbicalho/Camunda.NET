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
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using AttributeReference = org.camunda.bpm.model.xml.type.reference.AttributeReference;

	using static org.camunda.bpm.model.xml.testmodel.TestModelConstants;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public abstract class RelationshipDefinition : ModelElementInstanceImpl
	{

	  protected internal static Attribute<string> idAttr;
	  protected internal static AttributeReference<Animal> animalRef;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(RelationshipDefinition), TYPE_NAME_RELATIONSHIP_DEFINITION).namespaceUri(MODEL_NAMESPACE).abstractType();

		idAttr = typeBuilder.stringAttribute(ATTRIBUTE_NAME_ID).idAttribute().build();

		animalRef = typeBuilder.stringAttribute(ATTRIBUTE_NAME_ANIMAL_REF).idAttributeReference(typeof(Animal)).build();

		typeBuilder.build();
	  }

	  public RelationshipDefinition(ModelTypeInstanceContext instanceContext) : base(instanceContext)
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


	  public virtual Animal Animal
	  {
		  set
		  {
			animalRef.setReferenceTargetElement(this, value);
		  }
		  get
		  {
			return animalRef.getReferenceTargetElement(this);
		  }
	  }

	}

}