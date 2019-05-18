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
	using StringUtil = org.camunda.bpm.model.xml.impl.util.StringUtil;
	using DomDocument = org.camunda.bpm.model.xml.instance.DomDocument;
	using DomElement = org.camunda.bpm.model.xml.instance.DomElement;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;

	public class IdsElementReferenceCollectionImpl<Target, Source> : ElementReferenceCollectionImpl<Target, Source> where Target : org.camunda.bpm.model.xml.instance.ModelElementInstance where Source : org.camunda.bpm.model.xml.instance.ModelElementInstance
	{

	  protected internal string separator = " ";

	  public IdsElementReferenceCollectionImpl(ChildElementCollection<Source> referenceSourceCollection) : base(referenceSourceCollection)
	  {
	  }

	  protected internal virtual IList<string> getReferenceIdentifiers(ModelElementInstance referenceSourceElement)
	  {
		string referenceIdentifiers = getReferenceIdentifier(referenceSourceElement);
		return StringUtil.splitListBySeparator(referenceIdentifiers, separator);
	  }

	  protected internal virtual void setReferenceIdentifiers(ModelElementInstance referenceSourceElement, IList<string> referenceIdentifiers)
	  {
		string referenceIdentifier = StringUtil.joinList(referenceIdentifiers, separator);
		referenceSourceElement.TextContent = referenceIdentifier;
	  }

	  protected internal override ICollection<DomElement> getView(ModelElementInstanceImpl referenceSourceParentElement)
	  {
		DomDocument document = referenceSourceParentElement.ModelInstance.Document;
		ICollection<Source> referenceSourceElements = ReferenceSourceCollection.get(referenceSourceParentElement);
		ICollection<DomElement> referenceTargetElements = new List<DomElement>();
		foreach (Source referenceSourceElement in referenceSourceElements)
		{
		  IList<string> identifiers = getReferenceIdentifiers(referenceSourceElement);
		  foreach (string identifier in identifiers)
		  {
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
		}
		return referenceTargetElements;
	  }

	  protected internal override void updateReference(ModelElementInstance referenceSourceElement, string oldIdentifier, string newIdentifier)
	  {
		IList<string> referenceIdentifiers = getReferenceIdentifiers(referenceSourceElement);
		if (referenceIdentifiers.Contains(oldIdentifier))
		{
		  int index = referenceIdentifiers.IndexOf(oldIdentifier);
		  referenceIdentifiers.Remove(oldIdentifier);
		  referenceIdentifiers.Insert(index, newIdentifier);
		  setReferenceIdentifiers(referenceSourceElement, referenceIdentifiers);
		}
	  }

	  public override void referencedElementRemoved(ModelElementInstance referenceTargetElement, object referenceIdentifier)
	  {
		foreach (ModelElementInstance referenceSourceElement in findReferenceSourceElements(referenceTargetElement))
		{
		  IList<string> referenceIdentifiers = getReferenceIdentifiers(referenceSourceElement);
		  if (referenceIdentifiers.Contains(referenceIdentifier))
		  {
			if (referenceIdentifiers.Count == 1)
			{
			  // remove whole element
			  removeReference(referenceSourceElement, referenceTargetElement);
			}
			else
			{
			  // remove only single identifier
			  referenceIdentifiers.Remove(referenceIdentifier);
			  setReferenceIdentifiers(referenceSourceElement, referenceIdentifiers);
			}
		  }
		}
	  }

	}

}