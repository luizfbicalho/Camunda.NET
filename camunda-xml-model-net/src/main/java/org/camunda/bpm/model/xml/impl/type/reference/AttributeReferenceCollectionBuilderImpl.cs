using System;

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
namespace org.camunda.bpm.model.xml.impl.type.reference
{
	using AttributeImpl = org.camunda.bpm.model.xml.impl.type.attribute.AttributeImpl;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using AttributeReferenceCollection = org.camunda.bpm.model.xml.type.reference.AttributeReferenceCollection;
	using AttributeReferenceCollectionBuilder = org.camunda.bpm.model.xml.type.reference.AttributeReferenceCollectionBuilder;

	/// <summary>
	/// @author Roman Smirnov
	/// @author Sebastian Menski
	/// 
	/// </summary>
	public class AttributeReferenceCollectionBuilderImpl<T> : AttributeReferenceCollectionBuilder<T>, ModelBuildOperation where T : org.camunda.bpm.model.xml.instance.ModelElementInstance
	{

	  private readonly AttributeImpl<string> referenceSourceAttribute;
	  protected internal AttributeReferenceCollection<T> attributeReferenceCollection;
	  private readonly Type<T> referenceTargetElement;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "unchecked", "rawtypes" }) public AttributeReferenceCollectionBuilderImpl(org.camunda.bpm.model.xml.impl.type.attribute.AttributeImpl<String> attribute, Class<T> referenceTargetElement, Class attributeReferenceCollection)
	  public AttributeReferenceCollectionBuilderImpl(AttributeImpl<string> attribute, Type<T> referenceTargetElement, Type attributeReferenceCollection)
	  {
		this.referenceSourceAttribute = attribute;
		this.referenceTargetElement = referenceTargetElement;
		try
		{
		  this.attributeReferenceCollection = (AttributeReferenceCollection<T>) attributeReferenceCollection.GetConstructor(typeof(AttributeImpl)).newInstance(referenceSourceAttribute);
		}
		catch (Exception e)
		{
		  throw new Exception(e);
		}
	  }

	  public virtual AttributeReferenceCollection<T> build()
	  {
		referenceSourceAttribute.registerOutgoingReference(attributeReferenceCollection);
		return attributeReferenceCollection;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public void performModelBuild(org.camunda.bpm.model.xml.Model model)
	  public virtual void performModelBuild(Model model)
	  {
		// register declaring type as a referencing type of referenced type
		ModelElementTypeImpl referenceTargetType = (ModelElementTypeImpl) model.getType(referenceTargetElement);

		// the actual referenced type
		attributeReferenceCollection.ReferenceTargetElementType = referenceTargetType;

		// the referenced attribute may be declared on a base type of the referenced type.
		AttributeImpl<string> idAttribute = (AttributeImpl<string>) referenceTargetType.getAttribute("id");
		if (idAttribute != null)
		{
		  idAttribute.registerIncoming(attributeReferenceCollection);
		  attributeReferenceCollection.ReferenceTargetAttribute = idAttribute;
		}
		else
		{
		  throw new ModelException("Element type " + referenceTargetType.TypeNamespace + ":" + referenceTargetType.TypeName + " has no id attribute");
		}
	  }

	}

}