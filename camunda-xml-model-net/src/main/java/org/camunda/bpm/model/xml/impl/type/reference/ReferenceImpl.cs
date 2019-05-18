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
namespace org.camunda.bpm.model.xml.impl.type.reference
{

	using AttributeImpl = org.camunda.bpm.model.xml.impl.type.attribute.AttributeImpl;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using Reference = org.camunda.bpm.model.xml.type.reference.Reference;

	/// <summary>
	/// @author Sebastian Menski
	/// 
	/// </summary>
	public abstract class ReferenceImpl<T> : Reference<T> where T : org.camunda.bpm.model.xml.instance.ModelElementInstance
	{
		public abstract ModelElementType ReferenceSourceElementType {get;}
		public abstract string getReferenceIdentifier(ModelElementInstance referenceSourceElement);

	  protected internal AttributeImpl<string> referenceTargetAttribute;

	  /// <summary>
	  /// the actual type, may be different (a subtype of) <seealso cref="AttributeImpl#getOwningElementType()"/> </summary>
	  private ModelElementTypeImpl referenceTargetElementType;


	  /// <summary>
	  /// Set the reference identifier in the reference source
	  /// </summary>
	  /// <param name="referenceSourceElement"> the reference source model element instance </param>
	  /// <param name="referenceIdentifier"> the new reference identifier </param>
	  protected internal abstract void setReferenceIdentifier(ModelElementInstance referenceSourceElement, string referenceIdentifier);

	   /// <summary>
	   /// Get the reference target model element instance
	   /// </summary>
	   /// <param name="referenceSourceElement"> the reference source model element instance </param>
	   /// <returns> the reference target model element instance or null if not set </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public T getReferenceTargetElement(org.camunda.bpm.model.xml.instance.ModelElementInstance referenceSourceElement)
	  public virtual T getReferenceTargetElement(ModelElementInstance referenceSourceElement)
	  {
		string identifier = getReferenceIdentifier(referenceSourceElement);
		ModelElementInstance referenceTargetElement = referenceSourceElement.ModelInstance.getModelElementById(identifier);
		if (referenceTargetElement != null)
		{
		  try
		  {
			return (T) referenceTargetElement;

		  }
		  catch (System.InvalidCastException)
		  {
			throw new ModelReferenceException("Element " + referenceSourceElement + " references element " + referenceTargetElement + " of wrong type. " + "Expecting " + referenceTargetAttribute.OwningElementType + " got " + referenceTargetElement.ElementType);
		  }
		}
		else
		{
		  return default(T);
		}
	  }

	  /// <summary>
	  /// Set the reference target model element instance
	  /// </summary>
	  /// <param name="referenceSourceElement"> the reference source model element instance </param>
	  /// <param name="referenceTargetElement"> the reference target model element instance </param>
	  /// <exception cref="ModelReferenceException"> if element is not already added to the model </exception>
	  public virtual void setReferenceTargetElement(ModelElementInstance referenceSourceElement, T referenceTargetElement)
	  {
		ModelInstance modelInstance = referenceSourceElement.ModelInstance;
		string referenceTargetIdentifier = referenceTargetAttribute.getValue(referenceTargetElement);
		ModelElementInstance existingElement = modelInstance.getModelElementById(referenceTargetIdentifier);

		if (existingElement == null || !existingElement.Equals(referenceTargetElement))
		{
		  throw new ModelReferenceException("Cannot create reference to model element " + referenceTargetElement + ": element is not part of model. Please connect element to the model first.");
		}
		else
		{
		  setReferenceIdentifier(referenceSourceElement, referenceTargetIdentifier);
		}
	  }

	  /// <summary>
	  /// Set the reference target attribute
	  /// </summary>
	  /// <param name="referenceTargetAttribute"> the reference target string attribute </param>
	  public virtual void setReferenceTargetAttribute(AttributeImpl<string> referenceTargetAttribute)
	  {
		this.referenceTargetAttribute = referenceTargetAttribute;
	  }

	  /// <summary>
	  /// Get the reference target attribute
	  /// </summary>
	  /// <returns> the reference target string attribute </returns>
	  public virtual Attribute<string> getReferenceTargetAttribute()
	  {
		return referenceTargetAttribute;
	  }

	  /// <summary>
	  /// Set the reference target model element type
	  /// </summary>
	  /// <param name="referenceTargetElementType"> the referenceTargetElementType to set </param>
	  public virtual ModelElementTypeImpl ReferenceTargetElementType
	  {
		  set
		  {
			this.referenceTargetElementType = value;
		  }
	  }

	  public virtual ICollection<ModelElementInstance> findReferenceSourceElements(ModelElementInstance referenceTargetElement)
	  {
		if (referenceTargetElementType.isBaseTypeOf(referenceTargetElement.ElementType))
		{
		  ModelElementType owningElementType = ReferenceSourceElementType;
		  return referenceTargetElement.ModelInstance.getModelElementsByType(owningElementType);
		}
		else
		{
		  return Collections.emptyList();
		}
	  }

	  /// <summary>
	  /// Update the reference identifier of the reference source model element instance
	  /// </summary>
	  /// <param name="referenceSourceElement"> the reference source model element instance </param>
	  /// <param name="oldIdentifier"> the old reference identifier </param>
	  /// <param name="newIdentifier"> the new reference identifier </param>
	  protected internal abstract void updateReference(ModelElementInstance referenceSourceElement, string oldIdentifier, string newIdentifier);

	  /// <summary>
	  /// Update the reference identifier
	  /// </summary>
	  /// <param name="referenceTargetElement"> the reference target model element instance </param>
	  /// <param name="oldIdentifier"> the old reference identifier </param>
	  /// <param name="newIdentifier"> the new reference identifier </param>
	  public virtual void referencedElementUpdated(ModelElementInstance referenceTargetElement, string oldIdentifier, string newIdentifier)
	  {
		foreach (ModelElementInstance referenceSourceElement in findReferenceSourceElements(referenceTargetElement))
		{
		  updateReference(referenceSourceElement, oldIdentifier, newIdentifier);
		}
	  }

	  /// <summary>
	  /// Remove the reference in the reference source model element instance
	  /// </summary>
	  /// <param name="referenceSourceElement"> the reference source model element instance </param>
	  protected internal abstract void removeReference(ModelElementInstance referenceSourceElement, ModelElementInstance referenceTargetElement);

	  /// <summary>
	  /// Remove the reference if the target element is removed
	  /// </summary>
	  /// <param name="referenceTargetElement">  the reference target model element instance, which is removed </param>
	  /// <param name="referenceIdentifier">  the identifier of the reference to filter reference source elements </param>
	  public virtual void referencedElementRemoved(ModelElementInstance referenceTargetElement, object referenceIdentifier)
	  {
		foreach (ModelElementInstance referenceSourceElement in findReferenceSourceElements(referenceTargetElement))
		{
		  if (referenceIdentifier.Equals(getReferenceIdentifier(referenceSourceElement)))
		  {
			removeReference(referenceSourceElement, referenceTargetElement);
		  }
		}
	  }

	}

}