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
	using AttributeReference = org.camunda.bpm.model.xml.type.reference.AttributeReference;
	using AttributeReferenceBuilder = org.camunda.bpm.model.xml.type.reference.AttributeReferenceBuilder;

	/// <summary>
	/// A builder for a attribute model reference based on a QName
	/// 
	/// @author Sebastian Menski
	/// 
	/// </summary>
	public class AttributeReferenceBuilderImpl<T> : AttributeReferenceBuilder<T>, ModelBuildOperation where T : org.camunda.bpm.model.xml.instance.ModelElementInstance
	{

	  private readonly AttributeImpl<string> referenceSourceAttribute;
	  protected internal AttributeReferenceImpl<T> attributeReferenceImpl;
	  private readonly Type<T> referenceTargetElement;

	  /// <summary>
	  /// Create a new <seealso cref="AttributeReferenceBuilderImpl"/> from the reference source attribute
	  /// to the reference target model element instance
	  /// </summary>
	  /// <param name="referenceSourceAttribute"> the reference source attribute </param>
	  /// <param name="referenceTargetElement"> the reference target model element instance </param>
	  public AttributeReferenceBuilderImpl(AttributeImpl<string> referenceSourceAttribute, Type<T> referenceTargetElement)
	  {
		this.referenceSourceAttribute = referenceSourceAttribute;
		this.referenceTargetElement = referenceTargetElement;
		this.attributeReferenceImpl = new AttributeReferenceImpl<T>(referenceSourceAttribute);
	  }

	  public virtual AttributeReference<T> build()
	  {
		referenceSourceAttribute.registerOutgoingReference(attributeReferenceImpl);
		return attributeReferenceImpl;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public void performModelBuild(org.camunda.bpm.model.xml.Model model)
	  public virtual void performModelBuild(Model model)
	  {
		// register declaring type as a referencing type of referenced type
		ModelElementTypeImpl referenceTargetType = (ModelElementTypeImpl) model.getType(referenceTargetElement);

		// the actual referenced type
		attributeReferenceImpl.ReferenceTargetElementType = referenceTargetType;

		// the referenced attribute may be declared on a base type of the referenced type.
		AttributeImpl<string> idAttribute = (AttributeImpl<string>) referenceTargetType.getAttribute("id");
		if (idAttribute != null)
		{
		  idAttribute.registerIncoming(attributeReferenceImpl);
		  attributeReferenceImpl.ReferenceTargetAttribute = idAttribute;
		}
		else
		{
		  throw new ModelException("Element type " + referenceTargetType.TypeNamespace + ":" + referenceTargetType.TypeName + " has no id attribute");
		}
	  }

	}

}