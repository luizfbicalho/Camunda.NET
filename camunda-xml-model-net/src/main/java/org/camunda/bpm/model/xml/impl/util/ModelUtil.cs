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
namespace org.camunda.bpm.model.xml.impl.util
{
	using ModelElementInstanceImpl = org.camunda.bpm.model.xml.impl.instance.ModelElementInstanceImpl;
	using ModelElementTypeImpl = org.camunda.bpm.model.xml.impl.type.ModelElementTypeImpl;
	using StringAttribute = org.camunda.bpm.model.xml.impl.type.attribute.StringAttribute;
	using DomElement = org.camunda.bpm.model.xml.instance.DomElement;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;

	/// <summary>
	/// Some Helpers useful when handling model elements.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public sealed class ModelUtil
	{

	  private const string ID_ATTRIBUTE_NAME = "id";

	  /// <summary>
	  /// Returns the <seealso cref="ModelElementInstanceImpl ModelElement"/> for a DOM element.
	  /// If the model element does not yet exist, it is created and linked to the DOM.
	  /// </summary>
	  /// <param name="domElement"> the child element to create a new <seealso cref="ModelElementInstanceImpl ModelElement"/> for </param>
	  /// <returns> the child model element </returns>
	  public static ModelElementInstance getModelElement(DomElement domElement, ModelInstanceImpl modelInstance)
	  {
		ModelElementInstance modelElement = domElement.ModelElementInstance;
		if (modelElement == null)
		{
		  ModelElementTypeImpl modelType = getModelElement(domElement, modelInstance, domElement.NamespaceURI);
		  modelElement = modelType.newInstance(modelInstance, domElement);
		  domElement.ModelElementInstance = modelElement;
		}
		return modelElement;
	  }

	  /// <summary>
	  /// Returns the <seealso cref="ModelElementInstanceImpl ModelElement"/> for a DOM element.
	  /// If the model element does not yet exist, it is created and linked to the DOM.
	  /// </summary>
	  /// <param name="domElement"> the child element to create a new <seealso cref="ModelElementInstanceImpl ModelElement"/> for </param>
	  /// <param name="modelInstance"> the <seealso cref="ModelInstanceImpl ModelInstance"/> for which the new <seealso cref="ModelElementInstanceImpl ModelElement"/> is created </param>
	  /// <param name="modelType"> the <seealso cref="ModelElementTypeImpl ModelElementType"/> to create a new <seealso cref="ModelElementInstanceImpl ModelElement"/> for </param>
	  /// <returns> the child model element </returns>
	  public static ModelElementInstance getModelElement(DomElement domElement, ModelInstanceImpl modelInstance, ModelElementTypeImpl modelType)
	  {
		ModelElementInstance modelElement = domElement.ModelElementInstance;

		if (modelElement == null)
		{
		  modelElement = modelType.newInstance(modelInstance, domElement);
		  domElement.ModelElementInstance = modelElement;
		}
		return modelElement;
	  }

	  protected internal static ModelElementTypeImpl getModelElement(DomElement domElement, ModelInstanceImpl modelInstance, string namespaceUri)
	  {
		string localName = domElement.LocalName;
		ModelElementTypeImpl modelType = (ModelElementTypeImpl) modelInstance.Model.getTypeForName(namespaceUri, localName);

		if (modelType == null)
		{

		  Model model = modelInstance.Model;
		  string actualNamespaceUri = model.getActualNamespace(namespaceUri);

		  if (!string.ReferenceEquals(actualNamespaceUri, null))
		  {
			modelType = getModelElement(domElement, modelInstance, actualNamespaceUri);
		  }
		  else
		  {
			modelType = (ModelElementTypeImpl) modelInstance.registerGenericType(namespaceUri, localName);
		  }

		}
		return modelType;
	  }

	  public static QName getQName(string namespaceUri, string localName)
	  {
		return new QName(namespaceUri, localName);
	  }

	  public static void ensureInstanceOf(object instance, Type type)
	  {
		if (!type.IsAssignableFrom(instance.GetType()))
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new ModelException("Object is not instance of type " + type.FullName);
		}
	  }

	  // String to primitive type converters ////////////////////////////////////

	  public static bool valueAsBoolean(string rawValue)
	  {
		return bool.Parse(rawValue);
	  }

	  public static int valueAsInteger(string rawValue)
	  {
		try
		{
		  return int.Parse(rawValue);
		}
		catch (System.FormatException)
		{
		  throw new ModelTypeException(rawValue, typeof(Integer));
		}
	  }

	  public static float valueAsFloat(string rawValue)
	  {
		try
		{
		  return float.Parse(rawValue);
		}
		catch (System.FormatException)
		{
		  throw new ModelTypeException(rawValue, typeof(Float));
		}
	  }

	  public static double valueAsDouble(string rawValue)
	  {
		try
		{
		  return double.Parse(rawValue);
		}
		catch (System.FormatException)
		{
		  throw new ModelTypeException(rawValue, typeof(Double));
		}
	  }

	  public static short valueAsShort(string rawValue)
	  {
		try
		{
		  return short.Parse(rawValue);
		}
		catch (System.FormatException)
		{
		  throw new ModelTypeException(rawValue, typeof(Short));
		}
	  }

	  // primitive type to string converters //////////////////////////////////////

	  public static string valueAsString(bool booleanValue)
	  {
		  return Convert.ToString(booleanValue);
	  }

	  public static string valueAsString(int integerValue)
	  {
		return Convert.ToString(integerValue);
	  }

	  public static string valueAsString(float floatValue)
	  {
		return Convert.ToString(floatValue);
	  }

	  public static string valueAsString(double doubleValue)
	  {
		return Convert.ToString(doubleValue);
	  }

	  public static string valueAsString(short shortValue)
	  {
		return Convert.ToString(shortValue);
	  }

	  /// <summary>
	  /// Get a collection of all model element instances in a view
	  /// </summary>
	  /// <param name="view"> the collection of DOM elements to find the model element instances for </param>
	  /// <param name="model"> the model of the elements </param>
	  /// <returns> the collection of model element instances of the view </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T extends org.camunda.bpm.model.xml.instance.ModelElementInstance> Collection<T> getModelElementCollection(Collection<org.camunda.bpm.model.xml.instance.DomElement> view, org.camunda.bpm.model.xml.impl.ModelInstanceImpl model)
	  public static ICollection<T> getModelElementCollection<T>(ICollection<DomElement> view, ModelInstanceImpl model) where T : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
		IList<ModelElementInstance> resultList = new List<ModelElementInstance>();
		foreach (DomElement element in view)
		{
		  resultList.Add(getModelElement(element, model));
		}
		return (ICollection<T>) resultList;
	  }

	  /// <summary>
	  /// Find the index of the type of a model element in a list of element types
	  /// </summary>
	  /// <param name="modelElement"> the model element which type is searched for </param>
	  /// <param name="childElementTypes"> the list to search the type </param>
	  /// <returns> the index of the model element type in the list or -1 if it is not found </returns>
	  public static int getIndexOfElementType(ModelElementInstance modelElement, IList<ModelElementType> childElementTypes)
	  {
		for (int index = 0; index < childElementTypes.Count; index++)
		{
		  ModelElementType childElementType = childElementTypes[index];
		  Type instanceType = childElementType.InstanceType;
		  if (instanceType.IsAssignableFrom(modelElement.GetType()))
		  {
			return index;
		  }
		}
		ICollection<string> childElementTypeNames = new List<string>();
		foreach (ModelElementType childElementType in childElementTypes)
		{
		  childElementTypeNames.Add(childElementType.TypeName);
		}
		throw new ModelException("New child is not a valid child element type: " + modelElement.ElementType.TypeName + "; valid types are: " + childElementTypeNames);
	  }

	  /// <summary>
	  /// Calculate a collection of all extending types for the given base types
	  /// </summary>
	  /// <param name="baseTypes"> the collection of types to calculate the union of all extending types </param>
	  public static ICollection<ModelElementType> calculateAllExtendingTypes(Model model, ICollection<ModelElementType> baseTypes)
	  {
		ISet<ModelElementType> allExtendingTypes = new HashSet<ModelElementType>();
		foreach (ModelElementType baseType in baseTypes)
		{
		  ModelElementTypeImpl modelElementTypeImpl = (ModelElementTypeImpl) model.getType(baseType.InstanceType);
		  modelElementTypeImpl.resolveExtendingTypes(allExtendingTypes);
		}
		return allExtendingTypes;
	  }

	  /// <summary>
	  /// Calculate a collection of all base types for the given type
	  /// </summary>
	  public static ICollection<ModelElementType> calculateAllBaseTypes(ModelElementType type)
	  {
		IList<ModelElementType> baseTypes = new List<ModelElementType>();
		ModelElementTypeImpl typeImpl = (ModelElementTypeImpl) type;
		typeImpl.resolveBaseTypes(baseTypes);
		return baseTypes;
	  }

	  /// <summary>
	  /// Set new identifier if the type has a String id attribute
	  /// </summary>
	  /// <param name="type"> the type of the model element </param>
	  /// <param name="modelElementInstance"> the model element instance to set the id </param>
	  /// <param name="newId"> new identifier </param>
	  /// <param name="withReferenceUpdate"> true to update id references in other elements, false otherwise </param>
	  public static void setNewIdentifier(ModelElementType type, ModelElementInstance modelElementInstance, string newId, bool withReferenceUpdate)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.bpm.model.xml.type.attribute.Attribute<?> id = type.getAttribute(ID_ATTRIBUTE_NAME);
		Attribute<object> id = type.getAttribute(ID_ATTRIBUTE_NAME);
		if (id != null && id is StringAttribute && id.IdAttribute)
		{
		  ((StringAttribute) id).setValue(modelElementInstance, newId, withReferenceUpdate);
		}
	  }

	  /// <summary>
	  /// Set unique identifier if the type has a String id attribute
	  /// </summary>
	  /// <param name="type"> the type of the model element </param>
	  /// <param name="modelElementInstance"> the model element instance to set the id </param>
	  public static void setGeneratedUniqueIdentifier(ModelElementType type, ModelElementInstance modelElementInstance)
	  {
		setGeneratedUniqueIdentifier(type, modelElementInstance, true);
	  }

	  /// <summary>
	  /// Set unique identifier if the type has a String id attribute
	  /// </summary>
	  /// <param name="type"> the type of the model element </param>
	  /// <param name="modelElementInstance"> the model element instance to set the id </param>
	  /// <param name="withReferenceUpdate">  true to update id references in other elements, false otherwise </param>
	  public static void setGeneratedUniqueIdentifier(ModelElementType type, ModelElementInstance modelElementInstance, bool withReferenceUpdate)
	  {
		setNewIdentifier(type, modelElementInstance, ModelUtil.getUniqueIdentifier(type), withReferenceUpdate);
	  }

	  public static string getUniqueIdentifier(ModelElementType type)
	  {
		return type.TypeName + "_" + System.Guid.randomUUID();
	  }

	}

}