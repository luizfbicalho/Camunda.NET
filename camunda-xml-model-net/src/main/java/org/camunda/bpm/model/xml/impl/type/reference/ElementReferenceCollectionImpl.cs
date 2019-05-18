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
namespace org.camunda.bpm.model.xml.impl.type.reference
{
	using ModelElementInstanceImpl = org.camunda.bpm.model.xml.impl.instance.ModelElementInstanceImpl;
	using ModelUtil = org.camunda.bpm.model.xml.impl.util.ModelUtil;
	using DomDocument = org.camunda.bpm.model.xml.instance.DomDocument;
	using DomElement = org.camunda.bpm.model.xml.instance.DomElement;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;
	using ElementReferenceCollection = org.camunda.bpm.model.xml.type.reference.ElementReferenceCollection;


	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class ElementReferenceCollectionImpl<Target, Source> : ReferenceImpl<Target>, ElementReferenceCollection<Target, Source> where Target : org.camunda.bpm.model.xml.instance.ModelElementInstance where Source : org.camunda.bpm.model.xml.instance.ModelElementInstance
	{

	  private readonly ChildElementCollection<Source> referenceSourceCollection;
	  private ModelElementTypeImpl referenceSourceType;

	  public ElementReferenceCollectionImpl(ChildElementCollection<Source> referenceSourceCollection)
	  {
		this.referenceSourceCollection = referenceSourceCollection;
	  }

	  public virtual ChildElementCollection<Source> ReferenceSourceCollection
	  {
		  get
		  {
			return referenceSourceCollection;
		  }
	  }

	  protected internal override void setReferenceIdentifier(ModelElementInstance referenceSourceElement, string referenceIdentifier)
	  {
		referenceSourceElement.TextContent = referenceIdentifier;
	  }

	  protected internal virtual void performAddOperation(ModelElementInstanceImpl referenceSourceParentElement, Target referenceTargetElement)
	  {
		ModelInstanceImpl modelInstance = referenceSourceParentElement.ModelInstance;
		string referenceTargetIdentifier = referenceTargetAttribute.getValue(referenceTargetElement);
		ModelElementInstance existingElement = modelInstance.getModelElementById(referenceTargetIdentifier);

		if (existingElement == null || !existingElement.Equals(referenceTargetElement))
		{
		  throw new ModelReferenceException("Cannot create reference to model element " + referenceTargetElement + ": element is not part of model. Please connect element to the model first.");
		}
		else
		{
		  ICollection<Source> referenceSourceElements = referenceSourceCollection.get(referenceSourceParentElement);
		  Source referenceSourceElement = modelInstance.newInstance(referenceSourceType);
		  referenceSourceElements.Add(referenceSourceElement);
		  setReferenceIdentifier(referenceSourceElement, referenceTargetIdentifier);
		}
	  }

	  protected internal virtual void performRemoveOperation(ModelElementInstanceImpl referenceSourceParentElement, object referenceTargetElement)
	  {
		ICollection<ModelElementInstance> referenceSourceChildElements = referenceSourceParentElement.getChildElementsByType(referenceSourceType);
		foreach (ModelElementInstance referenceSourceChildElement in referenceSourceChildElements)
		{
		  if (getReferenceTargetElement(referenceSourceChildElement).Equals(referenceTargetElement))
		  {
			referenceSourceParentElement.removeChildElement(referenceSourceChildElement);
		  }
		}
	  }

	  protected internal virtual void performClearOperation(ModelElementInstanceImpl referenceSourceParentElement, ICollection<DomElement> elementsToRemove)
	  {
		foreach (DomElement element in elementsToRemove)
		{
		  referenceSourceParentElement.DomElement.removeChild(element);
		}
	  }

	  public override string getReferenceIdentifier(ModelElementInstance referenceSourceElement)
	  {
		return referenceSourceElement.TextContent;
	  }

	  protected internal override void updateReference(ModelElementInstance referenceSourceElement, string oldIdentifier, string newIdentifier)
	  {
		string referencingTextContent = getReferenceIdentifier(referenceSourceElement);
		if (!string.ReferenceEquals(oldIdentifier, null) && oldIdentifier.Equals(referencingTextContent))
		{
		  setReferenceIdentifier(referenceSourceElement, newIdentifier);
		}
	  }

	  protected internal override void removeReference(ModelElementInstance referenceSourceElement, ModelElementInstance referenceTargetElement)
	  {
		ModelElementInstance parentElement = referenceSourceElement.ParentElement;
		ICollection<Source> childElementCollection = referenceSourceCollection.get(parentElement);
		childElementCollection.remove(referenceSourceElement);
	  }

	  public virtual void setReferenceSourceElementType(ModelElementTypeImpl referenceSourceType)
	  {
		this.referenceSourceType = referenceSourceType;
	  }

	  public override ModelElementType getReferenceSourceElementType()
	  {
		return referenceSourceType;
	  }

	  protected internal virtual ICollection<DomElement> getView(ModelElementInstanceImpl referenceSourceParentElement)
	  {
		DomDocument document = referenceSourceParentElement.ModelInstance.Document;
		ICollection<Source> referenceSourceElements = referenceSourceCollection.get(referenceSourceParentElement);
		ICollection<DomElement> referenceTargetElements = new List<DomElement>();
		foreach (Source referenceSourceElement in referenceSourceElements)
		{
		  string identifier = getReferenceIdentifier(referenceSourceElement);
		  DomElement referenceTargetElement = document.getElementById(identifier);
		  if (referenceTargetElement != null)
		  {
			referenceTargetElements.Add(referenceTargetElement);
		  }
		  else
		  {
			throw new ModelException("Unable to find a model element instance for id " + identifier);
		  }
		}
		return referenceTargetElements;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public java.util.Collection<Target> getReferenceTargetElements(final org.camunda.bpm.model.xml.impl.instance.ModelElementInstanceImpl referenceSourceParentElement)
	  public virtual ICollection<Target> getReferenceTargetElements(ModelElementInstanceImpl referenceSourceParentElement)
	  {

		return new CollectionAnonymousInnerClass(this, referenceSourceParentElement);
	  }

	  private class CollectionAnonymousInnerClass : ICollection<Target>
	  {
		  private readonly ElementReferenceCollectionImpl<Target, Source> outerInstance;

		  private ModelElementInstanceImpl referenceSourceParentElement;

		  public CollectionAnonymousInnerClass(ElementReferenceCollectionImpl<Target, Source> outerInstance, ModelElementInstanceImpl referenceSourceParentElement)
		  {
			  this.outerInstance = outerInstance;
			  this.referenceSourceParentElement = referenceSourceParentElement;
		  }


		  public int size()
		  {
			return outerInstance.getView(referenceSourceParentElement).Count;
		  }

		  public bool Empty
		  {
			  get
			  {
				return outerInstance.getView(referenceSourceParentElement).Count == 0;
			  }
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
			  return outerInstance.getView(referenceSourceParentElement).Contains(((ModelElementInstanceImpl)o).DomElement);
			}
		  }

		  public IEnumerator<Target> iterator()
		  {
			ICollection<Target> modelElementCollection = ModelUtil.getModelElementCollection(outerInstance.getView(referenceSourceParentElement), referenceSourceParentElement.ModelInstance);
			return modelElementCollection.GetEnumerator();
		  }

		  public object[] toArray()
		  {
			ICollection<Target> modelElementCollection = ModelUtil.getModelElementCollection(outerInstance.getView(referenceSourceParentElement), referenceSourceParentElement.ModelInstance);
			return modelElementCollection.ToArray();
		  }

		  public T1[] toArray<T1>(T1[] a)
		  {
			ICollection<Target> modelElementCollection = ModelUtil.getModelElementCollection(outerInstance.getView(referenceSourceParentElement), referenceSourceParentElement.ModelInstance);
			return modelElementCollection.toArray(a);
		  }

		  public bool add(Target t)
		  {
			if (outerInstance.referenceSourceCollection.Immutable)
			{
			  throw new UnsupportedModelOperationException("add()", "collection is immutable");
			}
			else
			{
			  if (!contains(t))
			  {
				outerInstance.performAddOperation(referenceSourceParentElement, t);
			  }
			  return true;
			}
		  }

		  public bool remove(object o)
		  {
			if (outerInstance.referenceSourceCollection.Immutable)
			{
			  throw new UnsupportedModelOperationException("remove()", "collection is immutable");
			}
			else
			{
			  ModelUtil.ensureInstanceOf(o, typeof(ModelElementInstanceImpl));
			  outerInstance.performRemoveOperation(referenceSourceParentElement, o);
			  return true;
			}
		  }

		  public bool containsAll<T1>(ICollection<T1> c)
		  {
			ICollection<Target> modelElementCollection = ModelUtil.getModelElementCollection(outerInstance.getView(referenceSourceParentElement), referenceSourceParentElement.ModelInstance);
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
			return modelElementCollection.containsAll(c);
		  }

		  public bool addAll<T1>(ICollection<T1> c) where T1 : Target
		  {
			if (outerInstance.referenceSourceCollection.Immutable)
			{
			  throw new UnsupportedModelOperationException("addAll()", "collection is immutable");
			}
			else
			{
			  bool result = false;
			  foreach (Target o in c)
			  {
				result |= add(o);
			  }
			  return result;
			}

		  }

		  public bool removeAll<T1>(ICollection<T1> c)
		  {
			if (outerInstance.referenceSourceCollection.Immutable)
			{
			  throw new UnsupportedModelOperationException("removeAll()", "collection is immutable");
			}
			else
			{
			  bool result = false;
			  foreach (object o in c)
			  {
				result |= remove(o);
			  }
			  return result;
			}
		  }

		  public bool retainAll<T1>(ICollection<T1> c)
		  {
			throw new UnsupportedModelOperationException("retainAll()", "not implemented");
		  }

		  public void clear()
		  {
			if (outerInstance.referenceSourceCollection.Immutable)
			{
			  throw new UnsupportedModelOperationException("clear()", "collection is immutable");
			}
			else
			{
			  ICollection<DomElement> view = new List<DomElement>();
			  foreach (Source referenceSourceElement in outerInstance.referenceSourceCollection.get(referenceSourceParentElement))
			  {
				view.Add(referenceSourceElement.DomElement);
			  }
			  outerInstance.performClearOperation(referenceSourceParentElement, view);
			}
		  }
	  }
	}

}