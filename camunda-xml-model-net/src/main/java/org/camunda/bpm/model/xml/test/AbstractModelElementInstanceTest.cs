using System;
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
namespace org.camunda.bpm.model.xml.test
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.test.assertions.ModelAssertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

	using ModelElementTypeImpl = org.camunda.bpm.model.xml.impl.type.ModelElementTypeImpl;
	using ModelTypeException = org.camunda.bpm.model.xml.impl.util.ModelTypeException;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using AttributeAssert = org.camunda.bpm.model.xml.test.assertions.AttributeAssert;
	using ChildElementAssert = org.camunda.bpm.model.xml.test.assertions.ChildElementAssert;
	using ModelElementTypeAssert = org.camunda.bpm.model.xml.test.assertions.ModelElementTypeAssert;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;
	using Test = org.junit.Test;
	using DOMException = org.w3c.dom.DOMException;

	public abstract class AbstractModelElementInstanceTest
	{


	  protected internal class TypeAssumption
	  {
		  private readonly AbstractModelElementInstanceTest outerInstance;


		public readonly string namespaceUri;
		public readonly ModelElementType extendsType;
		public readonly bool isAbstract;

		public TypeAssumption(AbstractModelElementInstanceTest outerInstance, bool isAbstract) : this(outerInstance, outerInstance.DefaultNamespace, isAbstract)
		{
			this.outerInstance = outerInstance;
		}

		public TypeAssumption(AbstractModelElementInstanceTest outerInstance, string namespaceUri, bool isAbstract) : this(outerInstance, namespaceUri, null, isAbstract)
		{
			this.outerInstance = outerInstance;
		}

		public TypeAssumption(AbstractModelElementInstanceTest outerInstance, Type extendsType, bool isAbstract) : this(outerInstance, outerInstance.DefaultNamespace, extendsType, isAbstract)
		{
			this.outerInstance = outerInstance;
		}

		public TypeAssumption(AbstractModelElementInstanceTest outerInstance, string namespaceUri, Type extendsType, bool isAbstract)
		{
			this.outerInstance = outerInstance;
		  this.namespaceUri = namespaceUri;
		  this.extendsType = model.getType(extendsType);
		  this.isAbstract = isAbstract;
		}
	  }

	  protected internal class ChildElementAssumption
	  {
		  private readonly AbstractModelElementInstanceTest outerInstance;


		public readonly string namespaceUri;
		public readonly ModelElementType childElementType;
		public readonly int minOccurs;
		public readonly int maxOccurs;

		public ChildElementAssumption(AbstractModelElementInstanceTest outerInstance, Type childElementType) : this(outerInstance, childElementType, 0, -1)
		{
			this.outerInstance = outerInstance;
		}

		public ChildElementAssumption(AbstractModelElementInstanceTest outerInstance, string namespaceUri, Type childElementType) : this(outerInstance, namespaceUri, childElementType, 0, -1)
		{
			this.outerInstance = outerInstance;
		}

		public ChildElementAssumption(AbstractModelElementInstanceTest outerInstance, Type childElementType, int minOccurs) : this(outerInstance, childElementType, minOccurs, -1)
		{
			this.outerInstance = outerInstance;
		}

		public ChildElementAssumption(AbstractModelElementInstanceTest outerInstance, string namespaceUri, Type childElementType, int minOccurs) : this(outerInstance, namespaceUri, childElementType, minOccurs, -1)
		{
			this.outerInstance = outerInstance;
		}

		public ChildElementAssumption(AbstractModelElementInstanceTest outerInstance, Type childElementType, int minOccurs, int maxOccurs) : this(outerInstance, outerInstance.DefaultNamespace, childElementType, minOccurs, maxOccurs)
		{
			this.outerInstance = outerInstance;
		}

		public ChildElementAssumption(AbstractModelElementInstanceTest outerInstance, string namespaceUri, Type childElementType, int minOccurs, int maxOccurs)
		{
			this.outerInstance = outerInstance;
		  this.namespaceUri = namespaceUri;
		  this.childElementType = model.getType(childElementType);
		  this.minOccurs = minOccurs;
		  this.maxOccurs = maxOccurs;
		}
	  }

	  protected internal class AttributeAssumption
	  {
		  private readonly AbstractModelElementInstanceTest outerInstance;


		public readonly string attributeName;
		public readonly string @namespace;
		public readonly bool isIdAttribute;
		public readonly bool isRequired;
		public readonly object defaultValue;

		public AttributeAssumption(AbstractModelElementInstanceTest outerInstance, string attributeName) : this(outerInstance, attributeName, false, false)
		{
			this.outerInstance = outerInstance;
		}

		public AttributeAssumption(AbstractModelElementInstanceTest outerInstance, string @namespace, string attributeName) : this(outerInstance, @namespace, attributeName, false, false)
		{
			this.outerInstance = outerInstance;
		}

		public AttributeAssumption(AbstractModelElementInstanceTest outerInstance, string attributeName, bool isIdAttribute) : this(outerInstance, attributeName, isIdAttribute, false)
		{
			this.outerInstance = outerInstance;
		}

		public AttributeAssumption(AbstractModelElementInstanceTest outerInstance, string @namespace, string attributeName, bool isIdAttribute) : this(outerInstance, @namespace, attributeName, isIdAttribute, false)
		{
			this.outerInstance = outerInstance;
		}

		public AttributeAssumption(AbstractModelElementInstanceTest outerInstance, string attributeName, bool isIdAttribute, bool isRequired) : this(outerInstance, attributeName, isIdAttribute, isRequired, null)
		{
			this.outerInstance = outerInstance;
		}

		public AttributeAssumption(AbstractModelElementInstanceTest outerInstance, string @namespace, string attributeName, bool isIdAttribute, bool isRequired) : this(outerInstance, @namespace, attributeName, isIdAttribute, isRequired, null)
		{
			this.outerInstance = outerInstance;
		}

		public AttributeAssumption(AbstractModelElementInstanceTest outerInstance, string attributeName, bool isIdAttribute, bool isRequired, object defaultValue) : this(outerInstance, null, attributeName, isIdAttribute, isRequired, defaultValue)
		{
			this.outerInstance = outerInstance;
		}

		public AttributeAssumption(AbstractModelElementInstanceTest outerInstance, string @namespace, string attributeName, bool isIdAttribute, bool isRequired, object defaultValue)
		{
			this.outerInstance = outerInstance;
		  this.attributeName = attributeName;
		  this.@namespace = @namespace;
		  this.isIdAttribute = isIdAttribute;
		  this.isRequired = isRequired;
		  this.defaultValue = defaultValue;
		}
	  }

	  public static ModelInstance modelInstance;
	  public static Model model;
	  public static ModelElementType modelElementType;

	  public static void initModelElementType(GetModelElementTypeRule modelElementTypeRule)
	  {
		modelInstance = modelElementTypeRule.ModelInstance;
		model = modelElementTypeRule.Model;
		modelElementType = modelElementTypeRule.ModelElementType;
		assertThat(modelInstance).NotNull;
		assertThat(model).NotNull;
		assertThat(modelElementType).NotNull;
	  }

	  public abstract string DefaultNamespace {get;}
	  public abstract TypeAssumption getTypeAssumption();
	  public abstract ICollection<ChildElementAssumption> ChildElementAssumptions {get;}
	  public abstract ICollection<AttributeAssumption> AttributesAssumptions {get;}


	  public virtual ModelElementTypeAssert assertThatType()
	  {
		return assertThat(modelElementType);
	  }

	  public virtual AttributeAssert assertThatAttribute(string attributeName)
	  {
		return assertThat(modelElementType.getAttribute(attributeName));
	  }

	  public virtual ChildElementAssert assertThatChildElement(ModelElementType childElementType)
	  {
		ModelElementTypeImpl modelElementTypeImpl = (ModelElementTypeImpl) modelElementType;
		return assertThat(modelElementTypeImpl.getChildElementCollection(childElementType));
	  }

	  public virtual ModelElementType getType(Type instanceClass)
	  {
		return model.getType(instanceClass);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testType()
	  public virtual void testType()
	  {
		assertThatType().isPartOfModel(model);

		TypeAssumption assumption = getTypeAssumption();
		assertThatType().hasTypeNamespace(assumption.namespaceUri);

		if (assumption.isAbstract)
		{
		  assertThatType().Abstract;
		}
		else
		{
		  assertThatType().NotAbstract;
		}
		if (assumption.extendsType == null)
		{
		  assertThatType().extendsNoType();
		}
		else
		{
		  assertThatType().extendsType(assumption.extendsType);
		}

		if (assumption.isAbstract)
		{
		  try
		  {
			modelInstance.newInstance(modelElementType);
			fail("Element type " + modelElementType.TypeName + " is abstract.");
		  }
		  catch (DOMException)
		  {
			// expected exception
		  }
		  catch (ModelTypeException)
		  {
			// expected exception
		  }
		  catch (Exception e)
		  {
			fail("Unexpected exception " + e.Message);
		  }
		}
		else
		{
		  ModelElementInstance modelElementInstance = modelInstance.newInstance(modelElementType);
		  assertThat(modelElementInstance).NotNull;
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testChildElements()
	  public virtual void testChildElements()
	  {
		ICollection<ChildElementAssumption> childElementAssumptions = ChildElementAssumptions;
		if (childElementAssumptions == null)
		{
		  assertThatType().hasNoChildElements();
		}
		else
		{
		  assertThat(modelElementType.ChildElementTypes.Count).isEqualTo(childElementAssumptions.Count);
		  foreach (ChildElementAssumption assumption in childElementAssumptions)
		  {
			assertThatType().hasChildElements(assumption.childElementType);
			if (!string.ReferenceEquals(assumption.namespaceUri, null))
			{
			  assertThat(assumption.childElementType).hasTypeNamespace(assumption.namespaceUri);
			}
			assertThatChildElement(assumption.childElementType).occursMinimal(assumption.minOccurs).occursMaximal(assumption.maxOccurs);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAttributes()
	  public virtual void testAttributes()
	  {
		ICollection<AttributeAssumption> attributesAssumptions = AttributesAssumptions;
		if (attributesAssumptions == null)
		{
		  assertThatType().hasNoAttributes();
		}
		else
		{
		  assertThat(attributesAssumptions).hasSameSizeAs(modelElementType.Attributes);
		  foreach (AttributeAssumption assumption in attributesAssumptions)
		  {
			assertThatType().hasAttributes(assumption.attributeName);
			AttributeAssert attributeAssert = assertThatAttribute(assumption.attributeName);

			attributeAssert.hasOwningElementType(modelElementType);

			if (!string.ReferenceEquals(assumption.@namespace, null))
			{
			  attributeAssert.hasNamespaceUri(assumption.@namespace);
			}
			else
			{
			  attributeAssert.hasNoNamespaceUri();
			}

			if (assumption.isIdAttribute)
			{
			  attributeAssert.IdAttribute;
			}
			else
			{
			  attributeAssert.NotIdAttribute;
			}

			if (assumption.isRequired)
			{
			  attributeAssert.Required;
			}
			else
			{
			  attributeAssert.Optional;
			}

			if (assumption.defaultValue == null)
			{
			  attributeAssert.hasNoDefaultValue();
			}
			else
			{
			  attributeAssert.hasDefaultValue(assumption.defaultValue);
			}

		  }
		}
	  }
	}

}