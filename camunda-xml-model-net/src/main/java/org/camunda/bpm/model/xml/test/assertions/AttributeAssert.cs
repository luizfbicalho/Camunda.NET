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
namespace org.camunda.bpm.model.xml.test.assertions
{
	using AbstractAssert = org.assertj.core.api.AbstractAssert;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using Reference = org.camunda.bpm.model.xml.type.reference.Reference;


	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class AttributeAssert : AbstractAssert<AttributeAssert, Attribute<JavaToDotNetGenericWildcard>>
	{

	  private readonly string attributeName;

	  protected internal AttributeAssert<T1>(Attribute<T1> actual) : base(actual, typeof(AttributeAssert))
	  {
		attributeName = actual.AttributeName;
	  }

	  public virtual AttributeAssert Required
	  {
		  get
		  {
			NotNull;
    
			if (!actual.Required)
			{
			  failWithMessage("Expected attribute <%s> to be required but was not", attributeName);
			}
    
			return this;
		  }
	  }

	  public virtual AttributeAssert Optional
	  {
		  get
		  {
			NotNull;
    
			if (actual.Required)
			{
			  failWithMessage("Expected attribute <%s> to be optional but was required", attributeName);
			}
    
			return this;
		  }
	  }

	  public virtual AttributeAssert IdAttribute
	  {
		  get
		  {
			NotNull;
    
			if (!actual.IdAttribute)
			{
			  failWithMessage("Expected attribute <%s> to be an ID attribute but was not", attributeName);
			}
    
			return this;
		  }
	  }

	  public virtual AttributeAssert NotIdAttribute
	  {
		  get
		  {
			NotNull;
    
			if (actual.IdAttribute)
			{
			  failWithMessage("Expected attribute <%s> to be not an ID attribute but was", attributeName);
			}
    
			return this;
		  }
	  }

	  public virtual AttributeAssert hasDefaultValue(object defaultValue)
	  {
		NotNull;

		object actualDefaultValue = actual.DefaultValue;

		if (!defaultValue.Equals(actualDefaultValue))
		{
		  failWithMessage("Expected attribute <%s> to have default value <%s> but was <%s>", attributeName, defaultValue, actualDefaultValue);
		}

		return this;
	  }

	  public virtual AttributeAssert hasNoDefaultValue()
	  {
		NotNull;

		object actualDefaultValue = actual.DefaultValue;

		if (actualDefaultValue != null)
		{
		  failWithMessage("Expected attribute <%s> to have no default value but was <%s>", attributeName, actualDefaultValue);
		}

		return this;
	  }

	  public virtual AttributeAssert hasOwningElementType(ModelElementType owningElementType)
	  {
		NotNull;

		ModelElementType actualOwningElementType = actual.OwningElementType;

		if (!owningElementType.Equals(actualOwningElementType))
		{
		  failWithMessage("Expected attribute <%s> to have owning element type <%s> but was <%s>", attributeName, owningElementType, actualOwningElementType);
		}

		return this;
	  }

	  public virtual AttributeAssert hasValue(ModelElementInstance modelElementInstance)
	  {
		NotNull;

		object actualValue = actual.getValue(modelElementInstance);

		if (actualValue == null)
		{
		  failWithMessage("Expected attribute <%s> to have a value but has not", attributeName);
		}

		return this;
	  }

	  public virtual AttributeAssert hasValue(ModelElementInstance modelElementInstance, object value)
	  {
		NotNull;

		object actualValue = actual.getValue(modelElementInstance);

		if (!value.Equals(actualValue))
		{
		  failWithMessage("Expected attribute <%s> to have value <%s> but was <%s>", attributeName, value, actualValue);
		}

		return this;
	  }

	  public virtual AttributeAssert hasNoValue(ModelElementInstance modelElementInstance)
	  {
		NotNull;

		object actualValue = actual.getValue(modelElementInstance);

		if (actualValue != null)
		{
		  failWithMessage("Expected attribute <%s> to have no value but was <%s>", attributeName, actualValue);
		}

		return this;
	  }
	  public virtual AttributeAssert hasAttributeName(string attributeName)
	  {
		NotNull;

		if (!attributeName.Equals(this.attributeName))
		{
		  failWithMessage("Expected attribute to have attribute name <%s> but was <%s>", attributeName, this.attributeName);
		}

		return this;
	  }

	  public virtual AttributeAssert hasNamespaceUri(string namespaceUri)
	  {
		NotNull;

		string actualNamespaceUri1 = actual.NamespaceUri;

		if (!namespaceUri.Equals(actualNamespaceUri1))
		{
		  failWithMessage("Expected attribute <%s> to have namespace URI <%s> but was <%s>", attributeName, namespaceUri, actualNamespaceUri1);
		}

		return this;
	  }

	  public virtual AttributeAssert hasNoNamespaceUri()
	  {
		NotNull;

		string actualNamespaceUri = actual.NamespaceUri;

		if (!string.ReferenceEquals(actualNamespaceUri, null))
		{
		  failWithMessage("Expected attribute <%s> to have no namespace URI but was <%s>", attributeName, actualNamespaceUri);
		}

		return this;
	  }

	  public virtual AttributeAssert hasIncomingReferences()
	  {
		NotNull;

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.model.xml.type.reference.Reference<?>> actualIncomingReferences = actual.getIncomingReferences();
		IList<Reference<object>> actualIncomingReferences = actual.IncomingReferences;

		if (actualIncomingReferences.Count == 0)
		{
		  failWithMessage("Expected attribute <%s> to have incoming references but has not", attributeName);
		}

		return this;
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public AttributeAssert hasIncomingReferences(org.camunda.bpm.model.xml.type.reference.Reference<?>... references)
	  public virtual AttributeAssert hasIncomingReferences(params Reference<object>[] references)
	  {
		NotNull;

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.model.xml.type.reference.Reference<?>> incomingReferences = java.util.Arrays.asList(references);
		IList<Reference<object>> incomingReferences = Arrays.asList(references);
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.model.xml.type.reference.Reference<?>> actualIncomingReferences = actual.getIncomingReferences();
		IList<Reference<object>> actualIncomingReferences = actual.IncomingReferences;

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
		if (!actualIncomingReferences.containsAll(incomingReferences))
		{
		  failWithMessage("Expected attribute <%s> to have incoming references <%s> but has <%s>", attributeName, incomingReferences, actualIncomingReferences);
		}

		return this;
	  }

	  public virtual AttributeAssert hasNoIncomingReferences()
	  {
		NotNull;

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.model.xml.type.reference.Reference<?>> actualIncomingReferences = actual.getIncomingReferences();
		IList<Reference<object>> actualIncomingReferences = actual.IncomingReferences;

		if (actualIncomingReferences.Count > 0)
		{
		  failWithMessage("Expected attribute <%s> to have no incoming references but has <%s>", attributeName, actualIncomingReferences);
		}

		return this;
	  }

	  public virtual AttributeAssert hasOutgoingReferences()
	  {
		NotNull;

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.model.xml.type.reference.Reference<?>> actualOutgoingReferences = actual.getOutgoingReferences();
		IList<Reference<object>> actualOutgoingReferences = actual.OutgoingReferences;

		if (actualOutgoingReferences.Count == 0)
		{
		  failWithMessage("Expected attribute <%s> to have outgoing references but has not", attributeName);
		}

		return this;
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public AttributeAssert hasOutgoingReferences(org.camunda.bpm.model.xml.type.reference.Reference<?>... references)
	  public virtual AttributeAssert hasOutgoingReferences(params Reference<object>[] references)
	  {
		NotNull;

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.model.xml.type.reference.Reference<?>> outgoingReferences = java.util.Arrays.asList(references);
		IList<Reference<object>> outgoingReferences = Arrays.asList(references);
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.model.xml.type.reference.Reference<?>> actualOutgoingReferences = actual.getOutgoingReferences();
		IList<Reference<object>> actualOutgoingReferences = actual.OutgoingReferences;

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
		if (!actualOutgoingReferences.containsAll(outgoingReferences))
		{
		  failWithMessage("Expected attribute <%s> to have outgoing references <%s> but has <%s>", attributeName, outgoingReferences, actualOutgoingReferences);
		}

		return this;
	  }

	  public virtual AttributeAssert hasNoOutgoingReferences()
	  {
		NotNull;

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.model.xml.type.reference.Reference<?>> actualOutgoingReferences = actual.getOutgoingReferences();
		IList<Reference<object>> actualOutgoingReferences = actual.OutgoingReferences;

		if (actualOutgoingReferences.Count > 0)
		{
		  failWithMessage("Expected attribute <%s> to have no outgoing references but has <%s>", attributeName, actualOutgoingReferences);
		}

		return this;
	  }
	}

}