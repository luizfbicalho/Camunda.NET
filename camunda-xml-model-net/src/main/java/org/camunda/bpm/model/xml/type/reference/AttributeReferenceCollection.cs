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
namespace org.camunda.bpm.model.xml.type.reference
{

	using ModelInstanceImpl = org.camunda.bpm.model.xml.impl.ModelInstanceImpl;
	using ModelElementInstanceImpl = org.camunda.bpm.model.xml.impl.instance.ModelElementInstanceImpl;
	using AttributeImpl = org.camunda.bpm.model.xml.impl.type.attribute.AttributeImpl;
	using AttributeReferenceImpl = org.camunda.bpm.model.xml.impl.type.reference.AttributeReferenceImpl;
	using ModelUtil = org.camunda.bpm.model.xml.impl.util.ModelUtil;
	using StringUtil = org.camunda.bpm.model.xml.impl.util.StringUtil;
	using DomDocument = org.camunda.bpm.model.xml.instance.DomDocument;
	using DomElement = org.camunda.bpm.model.xml.instance.DomElement;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;

	/// <summary>
	/// @author Roman Smirnov
	/// @author Sebastian Menski
	/// 
	/// </summary>
	public abstract class AttributeReferenceCollection<T> : AttributeReferenceImpl<T>, AttributeReference<T> where T : org.camunda.bpm.model.xml.instance.ModelElementInstance
	{
		public override abstract org.camunda.bpm.model.xml.type.attribute.Attribute<string> ReferenceSourceAttribute {get;}

	  protected internal string separator = " ";

	  public AttributeReferenceCollection(AttributeImpl<string> referenceSourceAttribute) : base(referenceSourceAttribute)
	  {
	  }

	  protected internal override void updateReference(ModelElementInstance referenceSourceElement, string oldIdentifier, string newIdentifier)
	  {
		string referencingIdentifier = getReferenceIdentifier(referenceSourceElement);
		IList<string> references = StringUtil.splitListBySeparator(referencingIdentifier, separator);
		if (!string.ReferenceEquals(oldIdentifier, null) && references.Contains(oldIdentifier))
		{
		  referencingIdentifier = referencingIdentifier.Replace(oldIdentifier, newIdentifier);
		  setReferenceIdentifier(referenceSourceElement, newIdentifier);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") protected void removeReference(org.camunda.bpm.model.xml.instance.ModelElementInstance referenceSourceElement, org.camunda.bpm.model.xml.instance.ModelElementInstance referenceTargetElement)
	  protected internal override void removeReference(ModelElementInstance referenceSourceElement, ModelElementInstance referenceTargetElement)
	  {
		string identifier = getReferenceIdentifier(referenceSourceElement);
		IList<string> references = StringUtil.splitListBySeparator(identifier, separator);
		string identifierToRemove = getTargetElementIdentifier((T) referenceTargetElement);
		references.Remove(identifierToRemove);
		identifier = StringUtil.joinList(references, separator);
		setReferenceIdentifier(referenceSourceElement, identifier);
	  }

	  protected internal abstract string getTargetElementIdentifier(T referenceTargetElement);

	  private ICollection<DomElement> getView(ModelElementInstance referenceSourceElement)
	  {
		DomDocument document = referenceSourceElement.ModelInstance.Document;

		string identifier = getReferenceIdentifier(referenceSourceElement);
		IList<string> references = StringUtil.splitListBySeparator(identifier, separator);

		ICollection<DomElement> referenceTargetElements = new List<DomElement>();
		foreach (string reference in references)
		{
		  DomElement referenceTargetElement = document.getElementById(reference);
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
//ORIGINAL LINE: public java.util.Collection<T> getReferenceTargetElements(final org.camunda.bpm.model.xml.instance.ModelElementInstance referenceSourceElement)
	  public virtual ICollection<T> getReferenceTargetElements(ModelElementInstance referenceSourceElement)
	  {

		return new CollectionAnonymousInnerClass(this, referenceSourceElement);

	  }

	  private class CollectionAnonymousInnerClass : ICollection<T>
	  {
		  private readonly AttributeReferenceCollection<T> outerInstance;

		  private ModelElementInstance referenceSourceElement;

		  public CollectionAnonymousInnerClass(AttributeReferenceCollection<T> outerInstance, ModelElementInstance referenceSourceElement)
		  {
			  this.outerInstance = outerInstance;
			  this.referenceSourceElement = referenceSourceElement;
		  }


		  public int size()
		  {
			return outerInstance.getView(referenceSourceElement).Count;
		  }

		  public bool Empty
		  {
			  get
			  {
				return outerInstance.getView(referenceSourceElement).Count == 0;
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
			  return outerInstance.getView(referenceSourceElement).Contains(((ModelElementInstanceImpl)o).DomElement);
			}
		  }

		  public IEnumerator<T> iterator()
		  {
			ICollection<T> modelElementCollection = ModelUtil.getModelElementCollection(outerInstance.getView(referenceSourceElement), (ModelInstanceImpl) referenceSourceElement.ModelInstance);
			return modelElementCollection.GetEnumerator();
		  }

		  public object[] toArray()
		  {
			ICollection<T> modelElementCollection = ModelUtil.getModelElementCollection(outerInstance.getView(referenceSourceElement), (ModelInstanceImpl) referenceSourceElement.ModelInstance);
			return modelElementCollection.ToArray();
		  }

		  public T1[] toArray<T1>(T1[] a)
		  {
			ICollection<T> modelElementCollection = ModelUtil.getModelElementCollection(outerInstance.getView(referenceSourceElement), (ModelInstanceImpl) referenceSourceElement.ModelInstance);
			return modelElementCollection.toArray(a);
		  }

		  public bool add(T t)
		  {
			if (!contains(t))
			{
			  outerInstance.performAddOperation(referenceSourceElement, t);
			}
			return true;
		  }

		  public bool remove(object o)
		  {
			ModelUtil.ensureInstanceOf(o, typeof(ModelElementInstanceImpl));
			outerInstance.performRemoveOperation(referenceSourceElement, o);
			return true;
		  }

		  public bool containsAll<T1>(ICollection<T1> c)
		  {
			ICollection<T> modelElementCollection = ModelUtil.getModelElementCollection(outerInstance.getView(referenceSourceElement), (ModelInstanceImpl) referenceSourceElement.ModelInstance);
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
			return modelElementCollection.containsAll(c);
		  }

		  public bool addAll<T1>(ICollection<T1> c) where T1 : T
		  {
			bool result = false;
			foreach (T o in c)
			{
			  result |= add(o);
			}
			return result;
		  }

		  public bool removeAll<T1>(ICollection<T1> c)
		  {
			bool result = false;
			foreach (object o in c)
			{
			  result |= remove(o);
			}
			return result;
		  }

		  public bool retainAll<T1>(ICollection<T1> c)
		  {
			throw new UnsupportedModelOperationException("retainAll()", "not implemented");
		  }

		  public void clear()
		  {
			outerInstance.performClearOperation(referenceSourceElement);
		  }
	  }

	  protected internal virtual void performClearOperation(ModelElementInstance referenceSourceElement)
	  {
		setReferenceIdentifier(referenceSourceElement, "");
	  }

	  protected internal override void setReferenceIdentifier(ModelElementInstance referenceSourceElement, string referenceIdentifier)
	  {
		if (!string.ReferenceEquals(referenceIdentifier, null) && referenceIdentifier.Length > 0)
		{
		  base.setReferenceIdentifier(referenceSourceElement, referenceIdentifier);
		}
		else
		{
		  referenceSourceAttribute.removeAttribute(referenceSourceElement);
		}
	  }

	  /// <param name="referenceSourceElement"> </param>
	  /// <param name="o"> </param>
	  protected internal virtual void performRemoveOperation(ModelElementInstance referenceSourceElement, object o)
	  {
		removeReference(referenceSourceElement, (ModelElementInstance) o);
	  }

	  protected internal virtual void performAddOperation(ModelElementInstance referenceSourceElement, T referenceTargetElement)
	  {
		string identifier = getReferenceIdentifier(referenceSourceElement);
		IList<string> references = StringUtil.splitListBySeparator(identifier, separator);

		string targetIdentifier = getTargetElementIdentifier(referenceTargetElement);
		references.Add(targetIdentifier);

		identifier = StringUtil.joinList(references, separator);

		setReferenceIdentifier(referenceSourceElement, identifier);
	  }


	}

}