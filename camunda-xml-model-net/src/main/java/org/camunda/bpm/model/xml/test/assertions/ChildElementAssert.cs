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
namespace org.camunda.bpm.model.xml.test.assertions
{
	using AbstractAssert = org.assertj.core.api.AbstractAssert;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class ChildElementAssert : AbstractAssert<ChildElementAssert, ChildElementCollection<JavaToDotNetGenericWildcard>>
	{

	  private readonly Type typeClass;

	  protected internal ChildElementAssert<T1>(ChildElementCollection<T1> actual) : base(actual, typeof(ChildElementAssert))
	  {
		typeClass = actual.ChildElementTypeClass;
	  }

	  public virtual ChildElementAssert occursMinimal(int minOccurs)
	  {
		NotNull;

		int actualMinOccurs = actual.MinOccurs;

		if (actualMinOccurs != minOccurs)
		{
		  failWithMessage("Expected child element <%s> to have a min occurs of <%s> but was <%s>", typeClass, minOccurs, actualMinOccurs);
		}

		return this;
	  }

	  public virtual ChildElementAssert occursMaximal(int maxOccurs)
	  {
		NotNull;

		int actualMaxOccurs = actual.MaxOccurs;

		if (actualMaxOccurs != maxOccurs)
		{
		  failWithMessage("Expected child element <%s> to have a max occurs of <%s> but was <%s>", typeClass, maxOccurs, actualMaxOccurs);
		}

		return this;
	  }

	  public virtual ChildElementAssert Optional
	  {
		  get
		  {
			NotNull;
    
			int actualMinOccurs = actual.MinOccurs;
    
			if (actualMinOccurs != 0)
			{
			  failWithMessage("Expected child element <%s> to be optional but has min occurs of <%s>", typeClass, actualMinOccurs);
			}
    
			return this;
		  }
	  }

	  public virtual ChildElementAssert Unbounded
	  {
		  get
		  {
			NotNull;
    
			int actualMaxOccurs = actual.MaxOccurs;
    
			if (actualMaxOccurs != -1)
			{
			  failWithMessage("Expected child element <%s> to be unbounded but has a max occurs of <%s>", typeClass, actualMaxOccurs);
			}
    
			return this;
		  }
	  }

	  public virtual ChildElementAssert Mutable
	  {
		  get
		  {
			NotNull;
    
			bool actualImmutable = actual.Immutable;
    
			if (actualImmutable)
			{
			  failWithMessage("Expected child element <%s> to be mutable but was not", typeClass);
			}
    
			return this;
		  }
	  }

	  public virtual ChildElementAssert Immutable
	  {
		  get
		  {
			NotNull;
    
			bool actualImmutable = actual.Immutable;
    
			if (!actualImmutable)
			{
			  failWithMessage("Expected child element <%s> to be immutable but was not", typeClass);
			}
    
			return this;
		  }
	  }

	  public virtual ChildElementAssert containsType(Type childElementTypeClass)
	  {
		NotNull;

		Type actualChildElementTypeClass = actual.ChildElementTypeClass;

		if (!childElementTypeClass.Equals(actualChildElementTypeClass))
		{
		  failWithMessage("Expected child element <%s> to contain elements of type <%s> but contains elements of type <%s>", typeClass, childElementTypeClass, actualChildElementTypeClass);
		}

		return this;
	  }

	  public virtual ChildElementAssert hasParentElementType(ModelElementType parentElementType)
	  {
		NotNull;

		ModelElementType actualParentElementType = actual.ParentElementType;

		if (!parentElementType.Equals(actualParentElementType))
		{
		  failWithMessage("Expected child element <%s> to have parent element type <%s> but has <%s>", typeClass, parentElementType.TypeName, actualParentElementType.TypeName);
		}

		return this;
	  }

	  public virtual ChildElementAssert isNotEmpty(ModelElementInstance instance)
	  {
		NotNull;

		int actualNumberOfChildElements = actual.get(instance).size();

		if (actualNumberOfChildElements == 0)
		{
		  failWithMessage("Expected child element <%s> to contain elements but was not", typeClass);
		}

		return this;
	  }

	  public virtual ChildElementAssert hasSize(ModelElementInstance instance, int numberOfChildElements)
	  {
		NotNull;

		int actualNumberOfChildElements = actual.get(instance).size();

		if (actualNumberOfChildElements != numberOfChildElements)
		{
		  failWithMessage("Expected child element <%s> to contain <%s> elements but has <%s>", typeClass, numberOfChildElements, actualNumberOfChildElements);
		}

		return this;
	  }

	  public virtual ChildElementAssert isEmpty(ModelElementInstance instance)
	  {
		NotNull;

		int actualNumberOfChildElements = actual.get(instance).size();

		if (actualNumberOfChildElements > 0)
		{
		  failWithMessage("Expected child element <%s> to contain no elements but contains <%s> elements", typeClass, actualNumberOfChildElements);
		}

		return this;
	  }
	}

}