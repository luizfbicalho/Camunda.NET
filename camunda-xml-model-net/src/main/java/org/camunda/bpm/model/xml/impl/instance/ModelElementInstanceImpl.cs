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
namespace org.camunda.bpm.model.xml.impl.instance
{

	using ModelElementTypeImpl = org.camunda.bpm.model.xml.impl.type.ModelElementTypeImpl;
	using AttributeImpl = org.camunda.bpm.model.xml.impl.type.attribute.AttributeImpl;
	using ReferenceImpl = org.camunda.bpm.model.xml.impl.type.reference.ReferenceImpl;
	using ModelUtil = org.camunda.bpm.model.xml.impl.util.ModelUtil;
	using DomElement = org.camunda.bpm.model.xml.instance.DomElement;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using Reference = org.camunda.bpm.model.xml.type.reference.Reference;

	/// <summary>
	/// Base class for implementing Model Elements.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ModelElementInstanceImpl : ModelElementInstance
	{

	  /// <summary>
	  /// the containing model instance </summary>
	  protected internal readonly ModelInstanceImpl modelInstance;
	  /// <summary>
	  /// the wrapped DOM <seealso cref="DomElement"/> </summary>
	  private readonly DomElement domElement;
	  /// <summary>
	  /// the implementing model element type </summary>
	  private readonly ModelElementTypeImpl elementType;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(ModelElementInstance), "").abstractType();

		typeBuilder.build();
	  }

	  public ModelElementInstanceImpl(ModelTypeInstanceContext instanceContext)
	  {
		this.domElement = instanceContext.DomElement;
		this.modelInstance = instanceContext.Model;
		this.elementType = instanceContext.ModelType;
	  }

	  public virtual DomElement DomElement
	  {
		  get
		  {
			return domElement;
		  }
	  }

	  public virtual ModelInstanceImpl ModelInstance
	  {
		  get
		  {
			return modelInstance;
		  }
	  }

	  public virtual ModelElementInstance ParentElement
	  {
		  get
		  {
			DomElement parentElement = domElement.ParentElement;
			if (parentElement != null)
			{
			  return ModelUtil.getModelElement(parentElement, modelInstance);
			}
			else
			{
			  return null;
			}
		  }
	  }

	  public virtual ModelElementType ElementType
	  {
		  get
		  {
			return elementType;
		  }
	  }

	  public virtual string getAttributeValue(string attributeName)
	  {
		return domElement.getAttribute(attributeName);
	  }

	  public virtual string getAttributeValueNs(string namespaceUri, string attributeName)
	  {
		return domElement.getAttribute(namespaceUri, attributeName);
	  }

	  public virtual void setAttributeValue(string attributeName, string xmlValue)
	  {
		setAttributeValue(attributeName, xmlValue, false, true);
	  }

	  public virtual void setAttributeValue(string attributeName, string xmlValue, bool isIdAttribute)
	  {
		setAttributeValue(attributeName, xmlValue, isIdAttribute, true);
	  }

	  public virtual void setAttributeValue(string attributeName, string xmlValue, bool isIdAttribute, bool withReferenceUpdate)
	  {
		string oldValue = getAttributeValue(attributeName);
		if (isIdAttribute)
		{
		  domElement.setIdAttribute(attributeName, xmlValue);
		}
		else
		{
		  domElement.setAttribute(attributeName, xmlValue);
		}
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.bpm.model.xml.type.attribute.Attribute<?> attribute = elementType.getAttribute(attributeName);
		Attribute<object> attribute = elementType.getAttribute(attributeName);
		if (attribute != null && withReferenceUpdate)
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: ((org.camunda.bpm.model.xml.impl.type.attribute.AttributeImpl<?>) attribute).updateIncomingReferences(this, xmlValue, oldValue);
		  ((AttributeImpl<object>) attribute).updateIncomingReferences(this, xmlValue, oldValue);
		}
	  }

	  public virtual void setAttributeValueNs(string namespaceUri, string attributeName, string xmlValue)
	  {
		setAttributeValueNs(namespaceUri, attributeName, xmlValue, false, true);
	  }

	  public virtual void setAttributeValueNs(string namespaceUri, string attributeName, string xmlValue, bool isIdAttribute)
	  {
		setAttributeValueNs(namespaceUri, attributeName, xmlValue, isIdAttribute, true);
	  }

	  public virtual void setAttributeValueNs(string namespaceUri, string attributeName, string xmlValue, bool isIdAttribute, bool withReferenceUpdate)
	  {
		string namespaceForSetting = namespaceUri;
		if (hasValueToBeSetForAlternativeNs(namespaceUri, attributeName))
		{
		  namespaceForSetting = modelInstance.Model.getAlternativeNamespace(namespaceUri);
		}
		string oldValue = getAttributeValueNs(namespaceForSetting, attributeName);
		if (isIdAttribute)
		{
		  domElement.setIdAttribute(namespaceForSetting, attributeName, xmlValue);
		}
		else
		{
		  domElement.setAttribute(namespaceForSetting, attributeName, xmlValue);
		}
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.bpm.model.xml.type.attribute.Attribute<?> attribute = elementType.getAttribute(attributeName);
		Attribute<object> attribute = elementType.getAttribute(attributeName);
		if (attribute != null && withReferenceUpdate)
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: ((org.camunda.bpm.model.xml.impl.type.attribute.AttributeImpl<?>) attribute).updateIncomingReferences(this, xmlValue, oldValue);
		  ((AttributeImpl<object>) attribute).updateIncomingReferences(this, xmlValue, oldValue);
		}
	  }

	  private bool hasValueToBeSetForAlternativeNs(string namespaceUri, string attributeName)
	  {
		string alternativeNs = modelInstance.Model.getAlternativeNamespace(namespaceUri);
		return string.ReferenceEquals(getAttributeValueNs(namespaceUri, attributeName), null) && !string.ReferenceEquals(alternativeNs, null) && !string.ReferenceEquals(getAttributeValueNs(alternativeNs, attributeName), null);
	  }

	  public virtual void removeAttribute(string attributeName)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.bpm.model.xml.type.attribute.Attribute<?> attribute = elementType.getAttribute(attributeName);
		Attribute<object> attribute = elementType.getAttribute(attributeName);
		if (attribute != null)
		{
		  object identifier = attribute.getValue(this);
		  if (identifier != null)
		  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: ((org.camunda.bpm.model.xml.impl.type.attribute.AttributeImpl<?>) attribute).unlinkReference(this, identifier);
			((AttributeImpl<object>) attribute).unlinkReference(this, identifier);
		  }
		}
		domElement.removeAttribute(attributeName);
	  }

	  public virtual void removeAttributeNs(string namespaceUri, string attributeName)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.bpm.model.xml.type.attribute.Attribute<?> attribute = elementType.getAttribute(attributeName);
		Attribute<object> attribute = elementType.getAttribute(attributeName);
		if (attribute != null)
		{
		  object identifier = attribute.getValue(this);
		  if (identifier != null)
		  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: ((org.camunda.bpm.model.xml.impl.type.attribute.AttributeImpl<?>) attribute).unlinkReference(this, identifier);
			((AttributeImpl<object>) attribute).unlinkReference(this, identifier);
		  }
		}
		domElement.removeAttribute(namespaceUri, attributeName);
	  }

	  public virtual string TextContent
	  {
		  get
		  {
			return RawTextContent.Trim();
		  }
		  set
		  {
			domElement.TextContent = value;
		  }
	  }


	  public virtual string RawTextContent
	  {
		  get
		  {
			return domElement.TextContent;
		  }
	  }

	  public virtual ModelElementInstance getUniqueChildElementByNameNs(string namespaceUri, string elementName)
	  {
		Model model = modelInstance.Model;
		IList<DomElement> childElements = domElement.getChildElementsByNameNs(asSet(namespaceUri, model.getAlternativeNamespace(namespaceUri)), elementName);
		if (childElements.Count > 0)
		{
		  return ModelUtil.getModelElement(childElements[0], modelInstance);
		}
		else
		{
		  return null;
		}
	  }


	  public virtual ModelElementInstance getUniqueChildElementByType(Type elementType)
	  {
		IList<DomElement> childElements = domElement.getChildElementsByType(modelInstance, elementType);

		if (childElements.Count > 0)
		{
		  return ModelUtil.getModelElement(childElements[0], modelInstance);
		}
		else
		{
		  return null;
		}
	  }

	  public virtual ModelElementInstance UniqueChildElementByNameNs
	  {
		  set
		  {
			ModelUtil.ensureInstanceOf(value, typeof(ModelElementInstanceImpl));
			ModelElementInstanceImpl newChildElement = (ModelElementInstanceImpl) value;
    
			DomElement childElement = newChildElement.DomElement;
			ModelElementInstance existingChild = getUniqueChildElementByNameNs(childElement.NamespaceURI, childElement.LocalName);
			if (existingChild == null)
			{
			  addChildElement(value);
			}
			else
			{
			  replaceChildElement(existingChild, newChildElement);
			}
		  }
	  }

	  public virtual void replaceChildElement(ModelElementInstance existingChild, ModelElementInstance newChild)
	  {
		DomElement existingChildDomElement = existingChild.DomElement;
		DomElement newChildDomElement = newChild.DomElement;

		// unlink (remove all references) of child elements
		((ModelElementInstanceImpl) existingChild).unlinkAllChildReferences();

		// update incoming references from old to new child element
		updateIncomingReferences(existingChild, newChild);

		// replace the existing child with the new child in the DOM
		domElement.replaceChild(newChildDomElement, existingChildDomElement);

		// execute after replacement updates
		newChild.updateAfterReplacement();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private void updateIncomingReferences(org.camunda.bpm.model.xml.instance.ModelElementInstance oldInstance, org.camunda.bpm.model.xml.instance.ModelElementInstance newInstance)
	  private void updateIncomingReferences(ModelElementInstance oldInstance, ModelElementInstance newInstance)
	  {
		string oldId = oldInstance.getAttributeValue("id");
		string newId = newInstance.getAttributeValue("id");

		if (string.ReferenceEquals(oldId, null) || string.ReferenceEquals(newId, null))
		{
		  return;
		}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.Collection<org.camunda.bpm.model.xml.type.attribute.Attribute<?>> attributes = ((org.camunda.bpm.model.xml.impl.type.ModelElementTypeImpl) oldInstance.getElementType()).getAllAttributes();
		ICollection<Attribute<object>> attributes = ((ModelElementTypeImpl) oldInstance.ElementType).AllAttributes;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for (org.camunda.bpm.model.xml.type.attribute.Attribute<?> attribute : attributes)
		foreach (Attribute<object> attribute in attributes)
		{
		  if (attribute.IdAttribute)
		  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for (org.camunda.bpm.model.xml.type.reference.Reference<?> incomingReference : attribute.getIncomingReferences())
			foreach (Reference<object> incomingReference in attribute.IncomingReferences)
			{
			  ((ReferenceImpl<ModelElementInstance>) incomingReference).referencedElementUpdated(newInstance, oldId, newId);
			}
		  }
		}

	  }

	  public virtual void replaceWithElement(ModelElementInstance newElement)
	  {
		ModelElementInstanceImpl parentElement = (ModelElementInstanceImpl) ParentElement;
		if (parentElement != null)
		{
		  parentElement.replaceChildElement(this, newElement);
		}
		else
		{
		  throw new ModelException("Unable to remove replace without parent");
		}
	  }

	  public virtual void addChildElement(ModelElementInstance newChild)
	  {
		ModelUtil.ensureInstanceOf(newChild, typeof(ModelElementInstanceImpl));
		ModelElementInstance elementToInsertAfter = findElementToInsertAfter(newChild);
		insertElementAfter(newChild, elementToInsertAfter);
	  }

	  public virtual bool removeChildElement(ModelElementInstance child)
	  {
		ModelElementInstanceImpl childImpl = (ModelElementInstanceImpl) child;
		childImpl.unlinkAllReferences();
		childImpl.unlinkAllChildReferences();
		return domElement.removeChild(child.DomElement);
	  }

	  public virtual ICollection<ModelElementInstance> getChildElementsByType(ModelElementType childElementType)
	  {
		IList<ModelElementInstance> instances = new List<ModelElementInstance>();
		foreach (ModelElementType extendingType in childElementType.ExtendingTypes)
		{
		  ((IList<ModelElementInstance>)instances).AddRange(getChildElementsByType(extendingType));
		}
		Model model = modelInstance.Model;
		string alternativeNamespace = model.getAlternativeNamespace(childElementType.TypeNamespace);
		IList<DomElement> elements = domElement.getChildElementsByNameNs(asSet(childElementType.TypeNamespace, alternativeNamespace), childElementType.TypeName);
		((IList<ModelElementInstance>)instances).AddRange(ModelUtil.getModelElementCollection(elements, modelInstance));
		return instances;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.camunda.bpm.model.xml.instance.ModelElementInstance> java.util.Collection<T> getChildElementsByType(Class<T> childElementClass)
	  public virtual ICollection<T> getChildElementsByType<T>(Type<T> childElementClass) where T : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
		return (ICollection<T>) getChildElementsByType(ModelInstance.Model.getType(childElementClass));
	  }

	  /// <summary>
	  /// Returns the element after which the new element should be inserted in the DOM document.
	  /// </summary>
	  /// <param name="elementToInsert">  the new element to insert </param>
	  /// <returns> the element to insert after or null </returns>
	  private ModelElementInstance findElementToInsertAfter(ModelElementInstance elementToInsert)
	  {
		IList<ModelElementType> childElementTypes = elementType.AllChildElementTypes;
		IList<DomElement> childDomElements = domElement.ChildElements;
		ICollection<ModelElementInstance> childElements = ModelUtil.getModelElementCollection(childDomElements, modelInstance);

		ModelElementInstance insertAfterElement = null;
		int newElementTypeIndex = ModelUtil.getIndexOfElementType(elementToInsert, childElementTypes);
		foreach (ModelElementInstance childElement in childElements)
		{
		  int childElementTypeIndex = ModelUtil.getIndexOfElementType(childElement, childElementTypes);
		  if (newElementTypeIndex >= childElementTypeIndex)
		  {
			insertAfterElement = childElement;
		  }
		  else
		  {
			break;
		  }
		}
		return insertAfterElement;
	  }

	  public virtual void insertElementAfter(ModelElementInstance elementToInsert, ModelElementInstance insertAfterElement)
	  {
		if (insertAfterElement == null || insertAfterElement.DomElement == null)
		{
		  domElement.insertChildElementAfter(elementToInsert.DomElement, null);
		}
		else
		{
		  domElement.insertChildElementAfter(elementToInsert.DomElement, insertAfterElement.DomElement);
		}
	  }

	  public virtual void updateAfterReplacement()
	  {
		// do nothing
	  }

	  /// <summary>
	  /// Removes all reference to this.
	  /// </summary>
	  private void unlinkAllReferences()
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.Collection<org.camunda.bpm.model.xml.type.attribute.Attribute<?>> attributes = elementType.getAllAttributes();
		ICollection<Attribute<object>> attributes = elementType.AllAttributes;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for (org.camunda.bpm.model.xml.type.attribute.Attribute<?> attribute : attributes)
		foreach (Attribute<object> attribute in attributes)
		{
		  object identifier = attribute.getValue(this);
		  if (identifier != null)
		  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: ((org.camunda.bpm.model.xml.impl.type.attribute.AttributeImpl<?>) attribute).unlinkReference(this, identifier);
			((AttributeImpl<object>) attribute).unlinkReference(this, identifier);
		  }
		}
	  }

	  /// <summary>
	  /// Removes every reference to children of this.
	  /// </summary>
	  private void unlinkAllChildReferences()
	  {
		IList<ModelElementType> childElementTypes = elementType.AllChildElementTypes;
		foreach (ModelElementType type in childElementTypes)
		{
		  ICollection<ModelElementInstance> childElementsForType = getChildElementsByType(type);
		  foreach (ModelElementInstance childElement in childElementsForType)
		  {
			((ModelElementInstanceImpl) childElement).unlinkAllReferences();
		  }
		}
	  }

	  protected internal virtual ISet<T> asSet<T>(params T[] elements)
	  {
		return new HashSet<T>(Arrays.asList(elements));
	  }

	  public override int GetHashCode()
	  {
		return domElement.GetHashCode();
	  }

	  public override bool Equals(object obj)
	  {
		if (obj == null)
		{
		  return false;
		}
		else if (obj == this)
		{
		  return true;
		}
		else if (!(obj is ModelElementInstanceImpl))
		{
		  return false;
		}
		else
		{
		  ModelElementInstanceImpl other = (ModelElementInstanceImpl) obj;
		  return other.domElement.Equals(domElement);
		}
	  }

	}

}