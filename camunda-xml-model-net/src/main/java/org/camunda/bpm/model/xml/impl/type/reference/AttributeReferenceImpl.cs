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
	using AttributeReference = org.camunda.bpm.model.xml.type.reference.AttributeReference;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class AttributeReferenceImpl<T> : ReferenceImpl<T>, AttributeReference<T> where T : org.camunda.bpm.model.xml.instance.ModelElementInstance
	{

	  protected internal readonly AttributeImpl<string> referenceSourceAttribute;

	  public AttributeReferenceImpl(AttributeImpl<string> referenceSourceAttribute)
	  {
		this.referenceSourceAttribute = referenceSourceAttribute;
	  }

	  public override string getReferenceIdentifier(ModelElementInstance referenceSourceElement)
	  {
		return referenceSourceAttribute.getValue(referenceSourceElement);
	  }

	  protected internal override void setReferenceIdentifier(ModelElementInstance referenceSourceElement, string referenceIdentifier)
	  {
		referenceSourceAttribute.setValue(referenceSourceElement, referenceIdentifier);
	  }

	  /// <summary>
	  /// Get the reference source attribute
	  /// </summary>
	  /// <returns> the reference source attribute </returns>
	  public virtual Attribute<string> ReferenceSourceAttribute
	  {
		  get
		  {
			return referenceSourceAttribute;
		  }
	  }

	  public override ModelElementType ReferenceSourceElementType
	  {
		  get
		  {
			return referenceSourceAttribute.OwningElementType;
		  }
	  }

	  protected internal override void updateReference(ModelElementInstance referenceSourceElement, string oldIdentifier, string newIdentifier)
	  {
		string referencingAttributeValue = getReferenceIdentifier(referenceSourceElement);
		if (!string.ReferenceEquals(oldIdentifier, null) && oldIdentifier.Equals(referencingAttributeValue))
		{
		  setReferenceIdentifier(referenceSourceElement, newIdentifier);
		}
	  }

	  protected internal override void removeReference(ModelElementInstance referenceSourceElement, ModelElementInstance referenceTargetElement)
	  {
		referenceSourceAttribute.removeAttribute(referenceSourceElement);
	  }

	}

}