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
namespace org.camunda.bpm.model.xml.impl.type.attribute
{
	using ReferenceImpl = org.camunda.bpm.model.xml.impl.type.reference.ReferenceImpl;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using Reference = org.camunda.bpm.model.xml.type.reference.Reference;


	/// <summary>
	/// <para>Base class for implementing primitive value attributes</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public abstract class AttributeImpl<T> : Attribute<T>
	{

	  /// <summary>
	  /// the local name of the attribute </summary>
	  private string attributeName;

	  /// <summary>
	  /// the namespace for this attribute </summary>
	  private string namespaceUri;

	  /// <summary>
	  /// the default value for this attribute: the default value is returned
	  /// by the <seealso cref="#getValue(ModelElementInstance)"/> method in case the attribute is not set on the
	  /// domElement.
	  /// </summary>
	  private T defaultValue;

	  private bool isRequired = false;

	  private bool isIdAttribute = false;

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: private final java.util.List<org.camunda.bpm.model.xml.type.reference.Reference<?>> outgoingReferences = new java.util.ArrayList<org.camunda.bpm.model.xml.type.reference.Reference<?>>();
	  private readonly IList<Reference<object>> outgoingReferences = new List<Reference<object>>();

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: private final java.util.List<org.camunda.bpm.model.xml.type.reference.Reference<?>> incomingReferences = new java.util.ArrayList<org.camunda.bpm.model.xml.type.reference.Reference<?>>();
	  private readonly IList<Reference<object>> incomingReferences = new List<Reference<object>>();

	  private readonly ModelElementType owningElementType;

	  internal AttributeImpl(ModelElementType owningElementType)
	  {
		this.owningElementType = owningElementType;
	  }

	  /// <summary>
	  /// to be implemented by subclasses: converts the raw (String) value of the
	  /// attribute to the type required by the model
	  /// </summary>
	  /// <returns> the converted value </returns>
	  protected internal abstract T convertXmlValueToModelValue(string rawValue);

	  /// <summary>
	  /// to be implemented by subclasses: converts the raw (String) value of the
	  /// attribute to the type required by the model
	  /// </summary>
	  /// <returns> the converted value </returns>
	  protected internal abstract string convertModelValueToXmlValue(T modelValue);

	  public virtual ModelElementType OwningElementType
	  {
		  get
		  {
			return owningElementType;
		  }
	  }

	  /// <summary>
	  /// returns the value of the attribute.
	  /// </summary>
	  /// <returns> the value of the attribute. </returns>
	  public virtual T getValue(ModelElementInstance modelElement)
	  {
		string value;
		if (string.ReferenceEquals(namespaceUri, null))
		{
		  value = modelElement.getAttributeValue(attributeName);
		}
		else
		{
		  value = modelElement.getAttributeValueNs(namespaceUri, attributeName);
		  if (string.ReferenceEquals(value, null))
		  {
			string alternativeNamespace = owningElementType.Model.getAlternativeNamespace(namespaceUri);
			if (!string.ReferenceEquals(alternativeNamespace, null))
			{
			  value = modelElement.getAttributeValueNs(alternativeNamespace, attributeName);
			}
		  }
		}

		// default value
		if (string.ReferenceEquals(value, null) && defaultValue != default(T))
		{
		  return defaultValue;
		}
		else
		{
		  return convertXmlValueToModelValue(value);
		}
	  }

	  /// <summary>
	  /// sets the value of the attribute.
	  /// 
	  ///  the value of the attribute.
	  /// </summary>
	  public virtual void setValue(ModelElementInstance modelElement, T value)
	  {
		setValue(modelElement, value, true);
	  }

	  public virtual void setValue(ModelElementInstance modelElement, T value, bool withReferenceUpdate)
	  {
		string xmlValue = convertModelValueToXmlValue(value);
		if (string.ReferenceEquals(namespaceUri, null))
		{
		  modelElement.setAttributeValue(attributeName, xmlValue, isIdAttribute, withReferenceUpdate);
		}
		else
		{
		  modelElement.setAttributeValueNs(namespaceUri, attributeName, xmlValue, isIdAttribute, withReferenceUpdate);
		}
	  }

	  public virtual void updateIncomingReferences(ModelElementInstance modelElement, string newIdentifier, string oldIdentifier)
	  {
		if (incomingReferences.Count > 0)
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for (org.camunda.bpm.model.xml.type.reference.Reference<?> incomingReference : incomingReferences)
		  foreach (Reference<object> incomingReference in incomingReferences)
		  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: ((org.camunda.bpm.model.xml.impl.type.reference.ReferenceImpl<?>) incomingReference).referencedElementUpdated(modelElement, oldIdentifier, newIdentifier);
			((ReferenceImpl<object>) incomingReference).referencedElementUpdated(modelElement, oldIdentifier, newIdentifier);
		  }
		}
	  }

	  public virtual T DefaultValue
	  {
		  get
		  {
			return defaultValue;
		  }
		  set
		  {
			this.defaultValue = value;
		  }
	  }



	  public virtual bool Required
	  {
		  get
		  {
			return isRequired;
		  }
		  set
		  {
			this.isRequired = value;
		  }
	  }


	  /// <param name="namespaceUri"> the namespaceUri to set </param>
	  public virtual string NamespaceUri
	  {
		  set
		  {
			this.namespaceUri = value;
		  }
		  get
		  {
			return namespaceUri;
		  }
	  }


	  public virtual bool IdAttribute
	  {
		  get
		  {
			return isIdAttribute;
		  }
	  }

	  /// <summary>
	  /// Indicate whether this attribute is an Id attribute
	  /// 
	  /// </summary>
	  public virtual void setId()
	  {
		this.isIdAttribute = true;
	  }

	  /// <returns> the attributeName </returns>
	  public virtual string AttributeName
	  {
		  get
		  {
			return attributeName;
		  }
		  set
		  {
			this.attributeName = value;
		  }
	  }


	  public virtual void removeAttribute(ModelElementInstance modelElement)
	  {
		if (string.ReferenceEquals(namespaceUri, null))
		{
		  modelElement.removeAttribute(attributeName);
		}
		else
		{
		  modelElement.removeAttributeNs(namespaceUri, attributeName);
		}
	  }

	  public virtual void unlinkReference(ModelElementInstance modelElement, object referenceIdentifier)
	  {
		if (incomingReferences.Count > 0)
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for (org.camunda.bpm.model.xml.type.reference.Reference<?> incomingReference : incomingReferences)
		  foreach (Reference<object> incomingReference in incomingReferences)
		  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: ((org.camunda.bpm.model.xml.impl.type.reference.ReferenceImpl<?>) incomingReference).referencedElementRemoved(modelElement, referenceIdentifier);
			((ReferenceImpl<object>) incomingReference).referencedElementRemoved(modelElement, referenceIdentifier);
		  }
		}
	  }

	  /// <returns> the incomingReferences </returns>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public java.util.List<org.camunda.bpm.model.xml.type.reference.Reference<?>> getIncomingReferences()
	  public virtual IList<Reference<object>> IncomingReferences
	  {
		  get
		  {
			return incomingReferences;
		  }
	  }

	  /// <returns> the outgoingReferences </returns>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public java.util.List<org.camunda.bpm.model.xml.type.reference.Reference<?>> getOutgoingReferences()
	  public virtual IList<Reference<object>> OutgoingReferences
	  {
		  get
		  {
			return outgoingReferences;
		  }
	  }

	  public virtual void registerOutgoingReference<T1>(Reference<T1> @ref)
	  {
		outgoingReferences.Add(@ref);
	  }

	  public virtual void registerIncoming<T1>(Reference<T1> @ref)
	  {
		incomingReferences.Add(@ref);
	  }

	}

}