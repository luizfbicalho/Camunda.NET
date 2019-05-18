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
namespace org.camunda.bpm.model.xml.test.assertions
{
	using AbstractAssert = org.assertj.core.api.AbstractAssert;
	using QName = org.camunda.bpm.model.xml.impl.util.QName;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;


	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class ModelElementTypeAssert : AbstractAssert<ModelElementTypeAssert, ModelElementType>
	{

	  private readonly string typeName;

	  protected internal ModelElementTypeAssert(ModelElementType actual) : base(actual, typeof(ModelElementTypeAssert))
	  {
		typeName = actual.TypeName;
	  }

	  private IList<string> ActualAttributeNames
	  {
		  get
		  {
			IList<string> actualAttributeNames = new List<string>();
	//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
	//ORIGINAL LINE: for (org.camunda.bpm.model.xml.type.attribute.Attribute<?> attribute : actual.getAttributes())
			foreach (Attribute<object> attribute in actual.Attributes)
			{
			  actualAttributeNames.Add(attribute.AttributeName);
			}
			return actualAttributeNames;
		  }
	  }

	  private ICollection<string> getTypeNames(ICollection<ModelElementType> elementTypes)
	  {
		IList<string> typeNames = new List<string>();
		QName qName;
		foreach (ModelElementType elementType in elementTypes)
		{
		  qName = new QName(elementType.TypeNamespace, elementType.TypeName);
		  typeNames.Add(qName.ToString());
		}
		return typeNames;
	  }

	  public virtual ModelElementTypeAssert Abstract
	  {
		  get
		  {
			NotNull;
    
			if (!actual.Abstract)
			{
			  failWithMessage("Expected element type <%s> to be abstract but was not", typeName);
			}
    
			return this;
		  }
	  }

	  public virtual ModelElementTypeAssert NotAbstract
	  {
		  get
		  {
			NotNull;
    
			if (actual.Abstract)
			{
			  failWithMessage("Expected element type <%s> not to be abstract but was", typeName);
			}
    
			return this;
		  }
	  }

	  public virtual ModelElementTypeAssert extendsType(ModelElementType baseType)
	  {
		NotNull;

		ModelElementType actualBaseType = actual.BaseType;

		if (!actualBaseType.Equals(baseType))
		{
		  failWithMessage("Expected element type <%s> to extend type <%s> but extends <%s>", typeName, actualBaseType.TypeName, baseType.TypeName);
		}

		return this;
	  }

	  public virtual ModelElementTypeAssert extendsNoType()
	  {
		NotNull;

		ModelElementType actualBaseType = actual.BaseType;

		if (actualBaseType != null)
		{
		  failWithMessage("Expected element type <%s> to not extend any type but extends <%s>", typeName, actualBaseType.TypeName);
		}

		return this;
	  }

	  public virtual ModelElementTypeAssert hasAttributes()
	  {
		NotNull;

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.model.xml.type.attribute.Attribute<?>> actualAttributes = actual.getAttributes();
		IList<Attribute<object>> actualAttributes = actual.Attributes;

		if (actualAttributes.Count == 0)
		{
		  failWithMessage("Expected element type <%s> to have attributes but has none", typeName);
		}

		return this;
	  }

	  public virtual ModelElementTypeAssert hasAttributes(params string[] attributeNames)
	  {
		NotNull;

		IList<string> actualAttributeNames = ActualAttributeNames;

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
		if (!actualAttributeNames.containsAll(Arrays.asList(attributeNames)))
		{
		  failWithMessage("Expected element type <%s> to have attributes <%s> but has <%s>", typeName, attributeNames, actualAttributeNames);
		}

		return this;
	  }

	  public virtual ModelElementTypeAssert hasNoAttributes()
	  {
		NotNull;

		IList<string> actualAttributeNames = ActualAttributeNames;

		if (actualAttributeNames.Count > 0)
		{
		  failWithMessage("Expected element type <%s> to have no attributes but has <%s>", typeName, actualAttributeNames);
		}

		return this;
	  }

	  public virtual ModelElementTypeAssert hasChildElements()
	  {
		NotNull;

		IList<ModelElementType> childElementTypes = actual.ChildElementTypes;

		if (childElementTypes.Count == 0)
		{
		  failWithMessage("Expected element type <%s> to have child elements but has non", typeName);
		}

		return this;
	  }

	  public virtual ModelElementTypeAssert hasChildElements(params ModelElementType[] types)
	  {
		NotNull;

		IList<ModelElementType> childElementTypes = Arrays.asList(types);
		IList<ModelElementType> actualChildElementTypes = actual.ChildElementTypes;

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
		if (!actualChildElementTypes.containsAll(childElementTypes))
		{
		  ICollection<string> typeNames = getTypeNames(childElementTypes);
		  ICollection<string> actualTypeNames = getTypeNames(actualChildElementTypes);
		  failWithMessage("Expected element type <%s> to have child elements <%s> but has <%s>", typeName, typeNames, actualTypeNames);
		}

		return this;
	  }

	  public virtual ModelElementTypeAssert hasNoChildElements()
	  {
		NotNull;

		ICollection<string> actualChildElementTypeNames = getTypeNames(actual.ChildElementTypes);

		if (actualChildElementTypeNames.Count > 0)
		{
		  failWithMessage("Expected element type <%s> to have no child elements but has <%s>", typeName, actualChildElementTypeNames);
		}

		return this;
	  }

	  public virtual ModelElementTypeAssert hasTypeName(string typeName)
	  {
		NotNull;

		if (!typeName.Equals(this.typeName))
		{
		  failWithMessage("Expected element type to have name <%s> but was <%s>", typeName, this.typeName);
		}

		return this;
	  }

	  public virtual ModelElementTypeAssert hasTypeNamespace(string typeNamespace)
	  {
		NotNull;

		string actualTypeNamespace = actual.TypeNamespace;

		if (!typeNamespace.Equals(actualTypeNamespace))
		{
		  failWithMessage("Expected element type <%s> has type namespace <%s> but was <%s>", typeName, typeNamespace, actualTypeNamespace);
		}

		return this;
	  }

	  public virtual ModelElementTypeAssert hasInstanceType(Type instanceType)
	  {
		NotNull;

		Type actualInstanceType = actual.InstanceType;

		if (!instanceType.Equals(actualInstanceType))
		{
		  failWithMessage("Expected element type <%s> has instance type <%s> but was <%s>", typeName, instanceType, actualInstanceType);
		}

		return this;
	  }

	  public virtual ModelElementTypeAssert Extended
	  {
		  get
		  {
			NotNull;
    
			ICollection<ModelElementType> actualExtendingTypes = actual.ExtendingTypes;
    
			if (actualExtendingTypes.Count == 0)
			{
			  failWithMessage("Expected element type <%s> to be extended by types but was not", typeName);
			}
    
			return this;
		  }
	  }

	  public virtual ModelElementTypeAssert isExtendedBy(params ModelElementType[] types)
	  {
		NotNull;

		IList<ModelElementType> extendingTypes = Arrays.asList(types);
		ICollection<ModelElementType> actualExtendingTypes = actual.ExtendingTypes;

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
		if (!actualExtendingTypes.containsAll(extendingTypes))
		{
		  ICollection<string> typeNames = getTypeNames(extendingTypes);
		  ICollection<string> actualTypeNames = getTypeNames(actualExtendingTypes);
		  failWithMessage("Expected element type <%s> to be extended by types <%s> but is extended by <%s>", typeName, typeNames, actualTypeNames);
		}

		return this;
	  }

	  public virtual ModelElementTypeAssert NotExtended
	  {
		  get
		  {
			NotNull;
    
			ICollection<string> actualExtendingTypeNames = getTypeNames(actual.ExtendingTypes);
    
			if (actualExtendingTypeNames.Count > 0)
			{
			  failWithMessage("Expected element type <%s> to be not extend but is extended by <%s>", typeName, actualExtendingTypeNames);
			}
    
			return this;
		  }
	  }

	  public virtual ModelElementTypeAssert isNotExtendedBy(params ModelElementType[] types)
	  {
		NotNull;

		IList<ModelElementType> notExtendingTypes = Arrays.asList(types);
		ICollection<ModelElementType> actualExtendingTypes = actual.ExtendingTypes;

		IList<ModelElementType> errorTypes = new List<ModelElementType>();

		foreach (ModelElementType notExtendingType in notExtendingTypes)
		{
		  if (actualExtendingTypes.Contains(notExtendingType))
		  {
			errorTypes.Add(notExtendingType);
		  }
		}

		if (errorTypes.Count > 0)
		{
		  ICollection<string> errorTypeNames = getTypeNames(errorTypes);
		  ICollection<string> notExtendingTypeNames = getTypeNames(notExtendingTypes);
		  failWithMessage("Expected element type <%s> to be not extended by types <%s> but is extended by <%s>", typeName, notExtendingTypeNames, errorTypeNames);
		}

		return this;
	  }

	  public virtual ModelElementTypeAssert isPartOfModel(Model model)
	  {
		NotNull;

		Model actualModel = actual.Model;

		if (!model.Equals(actualModel))
		{
		  failWithMessage("Expected element type <%s> to be part of model <%s> but was part of <%s>", typeName, model.ModelName, actualModel.ModelName);
		}

		return this;
	  }
	}

}