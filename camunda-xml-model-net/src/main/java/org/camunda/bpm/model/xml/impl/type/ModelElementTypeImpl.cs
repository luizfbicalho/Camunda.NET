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
namespace org.camunda.bpm.model.xml.impl.type
{

	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelTypeException = org.camunda.bpm.model.xml.impl.util.ModelTypeException;
	using ModelUtil = org.camunda.bpm.model.xml.impl.util.ModelUtil;
	using DomDocument = org.camunda.bpm.model.xml.instance.DomDocument;
	using DomElement = org.camunda.bpm.model.xml.instance.DomElement;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;
	using ModelElementTypeBuilder_ModelTypeInstanceProvider = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder_ModelTypeInstanceProvider;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ModelElementTypeImpl : ModelElementType
	{

	  private readonly ModelImpl model;

	  private readonly string typeName;

	  private readonly Type instanceType;

	  private string typeNamespace;

	  private ModelElementTypeImpl baseType;

	  private readonly IList<ModelElementType> extendingTypes = new List<ModelElementType>();

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: private final java.util.List<org.camunda.bpm.model.xml.type.attribute.Attribute<?>> attributes = new java.util.ArrayList<org.camunda.bpm.model.xml.type.attribute.Attribute<?>>();
	  private readonly IList<Attribute<object>> attributes = new List<Attribute<object>>();

	  private readonly IList<ModelElementType> childElementTypes = new List<ModelElementType>();

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: private final java.util.List<org.camunda.bpm.model.xml.type.child.ChildElementCollection<?>> childElementCollections = new java.util.ArrayList<org.camunda.bpm.model.xml.type.child.ChildElementCollection<?>>();
	  private readonly IList<ChildElementCollection<object>> childElementCollections = new List<ChildElementCollection<object>>();

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: private org.camunda.bpm.model.xml.type.ModelElementTypeBuilder_ModelTypeInstanceProvider<?> instanceProvider;
	  private ModelElementTypeBuilder_ModelTypeInstanceProvider<object> instanceProvider;

	  private bool isAbstract;

	  public ModelElementTypeImpl(ModelImpl model, string name, Type instanceType)
	  {
		this.model = model;
		this.typeName = name;
		this.instanceType = instanceType;
	  }

	  public virtual ModelElementInstance newInstance(ModelInstance modelInstance)
	  {
		ModelInstanceImpl modelInstanceImpl = (ModelInstanceImpl) modelInstance;
		DomDocument document = modelInstanceImpl.Document;
		DomElement domElement = document.createElement(typeNamespace, typeName);
		return newInstance(modelInstanceImpl, domElement);
	  }

	  public virtual ModelElementInstance newInstance(ModelInstanceImpl modelInstance, DomElement domElement)
	  {
		ModelTypeInstanceContext modelTypeInstanceContext = new ModelTypeInstanceContext(domElement, modelInstance, this);
		return createModelElementInstance(modelTypeInstanceContext);
	  }

	  public virtual void registerAttribute<T1>(Attribute<T1> attribute)
	  {
		if (!attributes.Contains(attribute))
		{
		  attributes.Add(attribute);
		}
	  }

	  public virtual void registerChildElementType(ModelElementType childElementType)
	  {
		if (!childElementTypes.Contains(childElementType))
		{
		  childElementTypes.Add(childElementType);
		}
	  }

	  public virtual void registerChildElementCollection<T1>(ChildElementCollection<T1> childElementCollection)
	  {
		if (!childElementCollections.Contains(childElementCollection))
		{
		  childElementCollections.Add(childElementCollection);
		}
	  }

	  public virtual void registerExtendingType(ModelElementType modelType)
	  {
		if (!extendingTypes.Contains(modelType))
		{
		  extendingTypes.Add(modelType);
		}
	  }

	  protected internal virtual ModelElementInstance createModelElementInstance(ModelTypeInstanceContext instanceContext)
	  {
		if (isAbstract)
		{
		  throw new ModelTypeException("Model element type " + TypeName + " is abstract and no instances can be created.");
		}
		else
		{
		  return instanceProvider.newInstance(instanceContext);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public final java.util.List<org.camunda.bpm.model.xml.type.attribute.Attribute<?>> getAttributes()
	  public IList<Attribute<object>> Attributes
	  {
		  get
		  {
			return attributes;
		  }
	  }

	  public virtual string TypeName
	  {
		  get
		  {
			return typeName;
		  }
	  }

	  public virtual Type InstanceType
	  {
		  get
		  {
			return instanceType;
		  }
	  }

	  public virtual string TypeNamespace
	  {
		  set
		  {
			this.typeNamespace = value;
		  }
		  get
		  {
			return typeNamespace;
		  }
	  }


	  public virtual void setBaseType(ModelElementTypeImpl baseType)
	  {
		if (this.baseType == null)
		{
		  this.baseType = baseType;
		}
		else if (!this.baseType.Equals(baseType))
		{
		  throw new ModelException("Type can not have multiple base types. " + this.GetType() + " already extends type " + this.baseType.GetType() + " and can not also extend type " + baseType.GetType());
		}
	  }

	  public virtual ModelElementTypeBuilder_ModelTypeInstanceProvider<T1> InstanceProvider<T1>
	  {
		  set
		  {
			this.instanceProvider = value;
		  }
	  }
	  public virtual bool Abstract
	  {
		  get
		  {
			return isAbstract;
		  }
		  set
		  {
			this.isAbstract = value;
		  }
	  }


	  public virtual ICollection<ModelElementType> ExtendingTypes
	  {
		  get
		  {
			return Collections.unmodifiableCollection(extendingTypes);
		  }
	  }

	  public virtual ICollection<ModelElementType> AllExtendingTypes
	  {
		  get
		  {
			HashSet<ModelElementType> extendingTypes = new HashSet<ModelElementType>();
			extendingTypes.Add(this);
			resolveExtendingTypes(extendingTypes);
			return extendingTypes;
		  }
	  }

	  /// <summary>
	  /// Resolve all types recursively which are extending this type
	  /// </summary>
	  /// <param name="allExtendingTypes"> set of calculated extending types </param>
	  public virtual void resolveExtendingTypes(ISet<ModelElementType> allExtendingTypes)
	  {
		foreach (ModelElementType modelElementType in extendingTypes)
		{
		  ModelElementTypeImpl modelElementTypeImpl = (ModelElementTypeImpl) modelElementType;
		  if (!allExtendingTypes.Contains(modelElementTypeImpl))
		  {
			allExtendingTypes.Add(modelElementType);
			modelElementTypeImpl.resolveExtendingTypes(allExtendingTypes);
		  }
		}
	  }

	  /// <summary>
	  /// Resolve all types which are base types of this type
	  /// </summary>
	  /// <param name="baseTypes"> list of calculated base types </param>
	  public virtual void resolveBaseTypes(IList<ModelElementType> baseTypes)
	  {
		if (baseType != null)
		{
		  baseTypes.Add(baseType);
		  baseType.resolveBaseTypes(baseTypes);
		}
	  }


	  public virtual ModelElementType getBaseType()
	  {
		return baseType;
	  }

	  public virtual Model Model
	  {
		  get
		  {
			return model;
		  }
	  }

	  public virtual IList<ModelElementType> ChildElementTypes
	  {
		  get
		  {
			return childElementTypes;
		  }
	  }

	  public virtual IList<ModelElementType> AllChildElementTypes
	  {
		  get
		  {
			IList<ModelElementType> allChildElementTypes = new List<ModelElementType>();
			if (baseType != null)
			{
			  ((IList<ModelElementType>)allChildElementTypes).AddRange(baseType.AllChildElementTypes);
			}
			((IList<ModelElementType>)allChildElementTypes).AddRange(childElementTypes);
			return allChildElementTypes;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public java.util.List<org.camunda.bpm.model.xml.type.child.ChildElementCollection<?>> getChildElementCollections()
	  public virtual IList<ChildElementCollection<object>> ChildElementCollections
	  {
		  get
		  {
			return childElementCollections;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public java.util.List<org.camunda.bpm.model.xml.type.child.ChildElementCollection<?>> getAllChildElementCollections()
	  public virtual IList<ChildElementCollection<object>> AllChildElementCollections
	  {
		  get
		  {
	//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
	//ORIGINAL LINE: java.util.List<org.camunda.bpm.model.xml.type.child.ChildElementCollection<?>> allChildElementCollections = new java.util.ArrayList<org.camunda.bpm.model.xml.type.child.ChildElementCollection<?>>();
			IList<ChildElementCollection<object>> allChildElementCollections = new List<ChildElementCollection<object>>();
			if (baseType != null)
			{
	//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
	//ORIGINAL LINE: allChildElementCollections.addAll(baseType.getAllChildElementCollections());
			  ((IList<ChildElementCollection<object>>)allChildElementCollections).AddRange(baseType.AllChildElementCollections);
			}
	//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
	//ORIGINAL LINE: allChildElementCollections.addAll(childElementCollections);
			((IList<ChildElementCollection<object>>)allChildElementCollections).AddRange(childElementCollections);
			return allChildElementCollections;
		  }
	  }

	  public virtual ICollection<ModelElementInstance> getInstances(ModelInstance modelInstance)
	  {
		ModelInstanceImpl modelInstanceImpl = (ModelInstanceImpl) modelInstance;
		DomDocument document = modelInstanceImpl.Document;

		IList<DomElement> elements = getElementsByNameNs(document, typeNamespace);

		IList<ModelElementInstance> resultList = new List<ModelElementInstance>();
		foreach (DomElement element in elements)
		{
		  resultList.Add(ModelUtil.getModelElement(element, modelInstanceImpl, this));
		}
		return resultList;
	  }

	  protected internal virtual IList<DomElement> getElementsByNameNs(DomDocument document, string namespaceURI)
	  {
		IList<DomElement> elements = document.getElementsByNameNs(namespaceURI, typeName);

		if (elements.Count == 0)
		{
		  string alternativeNamespaceURI = Model.getAlternativeNamespace(namespaceURI);
		  if (!string.ReferenceEquals(alternativeNamespaceURI, null))
		  {
			elements = getElementsByNameNs(document, alternativeNamespaceURI);
		  }
		}

		return elements;
	  }

	  /// <summary>
	  /// Test if a element type is a base type of this type. So this type extends the given element type.
	  /// </summary>
	  /// <param name="elementType"> the element type to test </param>
	  /// <returns> true if {@code childElementTypeClass} is a base type of this type, else otherwise </returns>
	  public virtual bool isBaseTypeOf(ModelElementType elementType)
	  {
		if (this.Equals(elementType))
		{
		  return true;
		}
		else
		{
		  ICollection<ModelElementType> baseTypes = ModelUtil.calculateAllBaseTypes(elementType);
		  return baseTypes.Contains(this);
		}
	  }

	  /// <summary>
	  /// Returns a list of all attributes, including the attributes of all base types.
	  /// </summary>
	  /// <returns> the list of all attributes </returns>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public java.util.Collection<org.camunda.bpm.model.xml.type.attribute.Attribute<?>> getAllAttributes()
	  public virtual ICollection<Attribute<object>> AllAttributes
	  {
		  get
		  {
	//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
	//ORIGINAL LINE: java.util.List<org.camunda.bpm.model.xml.type.attribute.Attribute<?>> allAttributes = new java.util.ArrayList<org.camunda.bpm.model.xml.type.attribute.Attribute<?>>();
			IList<Attribute<object>> allAttributes = new List<Attribute<object>>();
	//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
	//ORIGINAL LINE: allAttributes.addAll(getAttributes());
			((IList<Attribute<object>>)allAttributes).AddRange(Attributes);
			ICollection<ModelElementType> baseTypes = ModelUtil.calculateAllBaseTypes(this);
			foreach (ModelElementType baseType in baseTypes)
			{
	//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
	//ORIGINAL LINE: allAttributes.addAll(baseType.getAttributes());
			  ((IList<Attribute<object>>)allAttributes).AddRange(baseType.Attributes);
			}
			return allAttributes;
		  }
	  }

	  /// <summary>
	  /// Return the attribute for the attribute name
	  /// </summary>
	  /// <param name="attributeName"> the name of the attribute </param>
	  /// <returns> the attribute or null if it not exists </returns>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public org.camunda.bpm.model.xml.type.attribute.Attribute<?> getAttribute(String attributeName)
	  public virtual Attribute<object> getAttribute(string attributeName)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for (org.camunda.bpm.model.xml.type.attribute.Attribute<?> attribute : getAllAttributes())
		foreach (Attribute<object> attribute in AllAttributes)
		{
		  if (attribute.AttributeName.Equals(attributeName))
		  {
			return attribute;
		  }
		}
		return null;
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public org.camunda.bpm.model.xml.type.child.ChildElementCollection<?> getChildElementCollection(org.camunda.bpm.model.xml.type.ModelElementType childElementType)
	  public virtual ChildElementCollection<object> getChildElementCollection(ModelElementType childElementType)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for (org.camunda.bpm.model.xml.type.child.ChildElementCollection<?> childElementCollection : getChildElementCollections())
		foreach (ChildElementCollection<object> childElementCollection in ChildElementCollections)
		{
		  if (childElementType.Equals(childElementCollection.getChildElementType(model)))
		  {
			return childElementCollection;
		  }
		}
		return null;
	  }

	  public override int GetHashCode()
	  {
		int prime = 31;
		int result = 1;
		result = prime * result + ((model == null) ? 0 : model.GetHashCode());
		result = prime * result + ((string.ReferenceEquals(typeName, null)) ? 0 : typeName.GetHashCode());
		result = prime * result + ((string.ReferenceEquals(typeNamespace, null)) ? 0 : typeNamespace.GetHashCode());
		return result;
	  }

	  public override bool Equals(object obj)
	  {
		if (this == obj)
		{
		  return true;
		}
		if (obj == null)
		{
		  return false;
		}
		if (this.GetType() != obj.GetType())
		{
		  return false;
		}
		ModelElementTypeImpl other = (ModelElementTypeImpl) obj;
		if (model == null)
		{
		  if (other.model != null)
		  {
			return false;
		  }
		}
		else if (!model.Equals(other.model))
		{
		  return false;
		}
		if (string.ReferenceEquals(typeName, null))
		{
		  if (!string.ReferenceEquals(other.typeName, null))
		  {
			return false;
		  }
		}
		else if (!typeName.Equals(other.typeName))
		{
		  return false;
		}
		if (string.ReferenceEquals(typeNamespace, null))
		{
		  if (!string.ReferenceEquals(other.typeNamespace, null))
		  {
			return false;
		  }
		}
		else if (!typeNamespace.Equals(other.typeNamespace))
		{
		  return false;
		}
		return true;
	  }

	}

}