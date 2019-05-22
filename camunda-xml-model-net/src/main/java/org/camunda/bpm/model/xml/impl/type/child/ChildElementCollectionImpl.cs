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
namespace org.camunda.bpm.model.xml.impl.type.child
{
	using ModelElementInstanceImpl = org.camunda.bpm.model.xml.impl.instance.ModelElementInstanceImpl;
	using ModelUtil = org.camunda.bpm.model.xml.impl.util.ModelUtil;
	using DomElement = org.camunda.bpm.model.xml.instance.DomElement;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;


	/// <summary>
	/// <para>This collection is a view on an the children of a Model Element.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ChildElementCollectionImpl<T> : ChildElementCollection<T> where T : org.camunda.bpm.model.xml.instance.ModelElementInstance
	{

	  protected internal readonly Type<T> childElementTypeClass;

	  /// <summary>
	  /// the containing type of the collection </summary>
	  private readonly ModelElementType parentElementType;

	  /// <summary>
	  /// the minimal count of child elements in the collection </summary>
	  private int minOccurs = 0;

	  /// <summary>
	  /// the maximum count of child elements in the collection.
	  /// An unbounded collection has a negative maxOccurs.
	  /// </summary>
	  protected internal int maxOccurs = -1;

	  /// <summary>
	  /// indicates whether this collection is mutable. </summary>
	  private bool isMutable = true;

	  public ChildElementCollectionImpl(Type childElementTypeClass, ModelElementTypeImpl parentElementType)
	  {
			  childElementTypeClass = typeof(T);
		this.childElementTypeClass = childElementTypeClass;
		this.parentElementType = parentElementType;
	  }

	  public virtual void setImmutable()
	  {
		Mutable = false;
	  }

	  public virtual bool Mutable
	  {
		  set
		  {
			this.isMutable = value;
		  }
	  }

	  public virtual bool Immutable
	  {
		  get
		  {
			return !isMutable;
		  }
	  }

	  // view /////////////////////////////////////////////////////////

	  /// <summary>
	  /// Internal method providing access to the view represented by this collection.
	  /// </summary>
	  /// <returns> the view represented by this collection </returns>
	  private ICollection<DomElement> getView(ModelElementInstanceImpl modelElement)
	  {
		return modelElement.DomElement.getChildElementsByType(modelElement.ModelInstance, childElementTypeClass);
	  }

	  public virtual int MinOccurs
	  {
		  get
		  {
			return minOccurs;
		  }
		  set
		  {
			this.minOccurs = value;
		  }
	  }


	  public virtual int MaxOccurs
	  {
		  get
		  {
			return maxOccurs;
		  }
		  set
		  {
			this.maxOccurs = value;
		  }
	  }

	  public virtual ModelElementType getChildElementType(Model model)
	  {
		return model.getType(childElementTypeClass);
	  }

	  public virtual Type<T> ChildElementTypeClass
	  {
		  get
		  {
			return childElementTypeClass;
		  }
	  }

	  public virtual ModelElementType ParentElementType
	  {
		  get
		  {
			return parentElementType;
		  }
	  }


	  /// <summary>
	  /// the "add" operation used by the collection </summary>
	  private void performAddOperation(ModelElementInstanceImpl modelElement, T e)
	  {
		modelElement.addChildElement(e);
	  }

	  /// <summary>
	  /// the "remove" operation used by this collection </summary>
	  private bool performRemoveOperation(ModelElementInstanceImpl modelElement, object e)
	  {
		return modelElement.removeChildElement((ModelElementInstanceImpl)e);
	  }

	  /// <summary>
	  /// the "clear" operation used by this collection </summary>
	  private void performClearOperation(ModelElementInstanceImpl modelElement, ICollection<DomElement> elementsToRemove)
	  {
		ICollection<ModelElementInstance> modelElements = ModelUtil.getModelElementCollection(elementsToRemove, modelElement.ModelInstance);
		foreach (ModelElementInstance element in modelElements)
		{
		  modelElement.removeChildElement(element);
		}
	  }

	  public virtual ICollection<T> get(ModelElementInstance element)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.xml.impl.instance.ModelElementInstanceImpl modelElement = (org.camunda.bpm.model.xml.impl.instance.ModelElementInstanceImpl) element;
		ModelElementInstanceImpl modelElement = (ModelElementInstanceImpl) element;

		return new CollectionAnonymousInnerClass(this, modelElement);
	  }

	  private class CollectionAnonymousInnerClass : ICollection<T>
	  {
		  private readonly ChildElementCollectionImpl<T> outerInstance;

		  private ModelElementInstanceImpl modelElement;

		  public CollectionAnonymousInnerClass(ChildElementCollectionImpl<T> outerInstance, ModelElementInstanceImpl modelElement)
		  {
			  this.outerInstance = outerInstance;
			  this.modelElement = modelElement;
		  }


		  public bool contains(object o)
		  {
			if (o == null)
			{
			  return false;

			}
			else if (!(o is ModelElementInstanceImpl))
			{
			  return false;

			}
			else
			{
			  return outerInstance.getView(modelElement).Contains(((ModelElementInstanceImpl)o).DomElement);

			}
		  }

		  public bool containsAll<T1>(ICollection<T1> c)
		  {
			foreach (object elementToCheck in c)
			{
			  if (!contains(elementToCheck))
			  {
				return false;
			  }
			}
			return true;
		  }

		  public bool Empty
		  {
			  get
			  {
				return outerInstance.getView(modelElement).Count == 0;
			  }
		  }

		  public IEnumerator<T> iterator()
		  {
			ICollection<T> modelElementCollection = ModelUtil.getModelElementCollection(outerInstance.getView(modelElement), modelElement.ModelInstance);
			return modelElementCollection.GetEnumerator();
		  }

		  public object[] toArray()
		  {
			ICollection<T> modelElementCollection = ModelUtil.getModelElementCollection(outerInstance.getView(modelElement), modelElement.ModelInstance);
			return modelElementCollection.ToArray();
		  }

		  public U[] toArray<U>(U[] a)
		  {
			ICollection<T> modelElementCollection = ModelUtil.getModelElementCollection(outerInstance.getView(modelElement), modelElement.ModelInstance);
			return modelElementCollection.toArray(a);
		  }

		  public int size()
		  {
			return outerInstance.getView(modelElement).Count;
		  }

		  public bool add(T e)
		  {
			if (!outerInstance.isMutable)
			{
			  throw new UnsupportedModelOperationException("add()", "collection is immutable");
			}
			outerInstance.performAddOperation(modelElement, e);
			return true;
		  }

		  public bool addAll<T1>(ICollection<T1> c) where T1 : T
		  {
			if (!outerInstance.isMutable)
			{
			  throw new UnsupportedModelOperationException("addAll()", "collection is immutable");
			}
			bool result = false;
			foreach (T t in c)
			{
			  result |= add(t);
			}
			return result;
		  }

		  public void clear()
		  {
			if (!outerInstance.isMutable)
			{
			  throw new UnsupportedModelOperationException("clear()", "collection is immutable");
			}
			ICollection<DomElement> view = outerInstance.getView(modelElement);
			outerInstance.performClearOperation(modelElement, view);
		  }

		  public bool remove(object e)
		  {
			if (!outerInstance.isMutable)
			{
			  throw new UnsupportedModelOperationException("remove()", "collection is immutable");
			}
			ModelUtil.ensureInstanceOf(e, typeof(ModelElementInstanceImpl));
			return outerInstance.performRemoveOperation(modelElement, e);
		  }

		  public bool removeAll<T1>(ICollection<T1> c)
		  {
			if (!outerInstance.isMutable)
			{
			  throw new UnsupportedModelOperationException("removeAll()", "collection is immutable");
			}
			bool result = false;
			foreach (object t in c)
			{
			  result |= remove(t);
			}
			return result;
		  }

		  public bool retainAll<T1>(ICollection<T1> c)
		  {
			throw new UnsupportedModelOperationException("retainAll()", "not implemented");
		  }

	  }

	}

}