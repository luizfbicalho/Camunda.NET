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
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;
	using ElementReference = org.camunda.bpm.model.xml.type.reference.ElementReference;
	using ElementReferenceCollection = org.camunda.bpm.model.xml.type.reference.ElementReferenceCollection;

	using static org.camunda.bpm.model.xml.testmodel.TestModelConstants;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class Egg : ModelElementInstanceImpl
	{

	  protected internal static Attribute<string> idAttr;
	  protected internal static ElementReference<Animal, Mother> motherRefChild;
	  protected internal static ElementReferenceCollection<Animal, Guardian> guardianRefCollection;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(Egg), ELEMENT_NAME_EGG).namespaceUri(MODEL_NAMESPACE).instanceProvider(new ModelElementTypeBuilder_ModelTypeInstanceProviderAnonymousInnerClass());

		idAttr = typeBuilder.stringAttribute(ATTRIBUTE_NAME_ID).idAttribute().build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		motherRefChild = sequenceBuilder.element(typeof(Mother)).uriElementReference(typeof(Animal)).build();

		guardianRefCollection = sequenceBuilder.elementCollection(typeof(Guardian)).uriElementReferenceCollection(typeof(Animal)).build();

		typeBuilder.build();
	  }

	  private class ModelElementTypeBuilder_ModelTypeInstanceProviderAnonymousInnerClass : org.camunda.bpm.model.xml.type.ModelElementTypeBuilder_ModelTypeInstanceProvider<Egg>
	  {
		  public Egg newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new Egg(instanceContext);
		  }
	  }

	  public Egg(ModelTypeInstanceContext instanceContext) : base(instanceContext)
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


	  public virtual Animal Mother
	  {
		  get
		  {
			return motherRefChild.getReferenceTargetElement(this);
		  }
		  set
		  {
			motherRefChild.setReferenceTargetElement(this, value);
		  }
	  }

	  public virtual void removeMother()
	  {
		motherRefChild.clearReferenceTargetElement(this);
	  }


	  public virtual Mother MotherRef
	  {
		  get
		  {
			return motherRefChild.getReferenceSource(this);
		  }
	  }

	  public virtual ICollection<Animal> Guardians
	  {
		  get
		  {
			return guardianRefCollection.getReferenceTargetElements(this);
		  }
	  }

	  public virtual ICollection<Guardian> GuardianRefs
	  {
		  get
		  {
			return guardianRefCollection.ReferenceSourceCollection.get(this);
		  }
	  }

	}

}