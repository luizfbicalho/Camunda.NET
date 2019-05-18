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
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ChildElement = org.camunda.bpm.model.xml.type.child.ChildElement;
	using ElementReference = org.camunda.bpm.model.xml.type.reference.ElementReference;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class ElementReferenceImpl<Target, Source> : ElementReferenceCollectionImpl<Target, Source>, ElementReference<Target, Source> where Target : org.camunda.bpm.model.xml.instance.ModelElementInstance where Source : org.camunda.bpm.model.xml.instance.ModelElementInstance
	{


	  public ElementReferenceImpl(ChildElement<Source> referenceSourceCollection) : base(referenceSourceCollection)
	  {
	  }

	  private ChildElement<Source> ReferenceSourceChild
	  {
		  get
		  {
			return (ChildElement<Source>) ReferenceSourceCollection;
		  }
	  }

	  public virtual Source getReferenceSource(ModelElementInstance referenceSourceParent)
	  {
		return ReferenceSourceChild.getChild(referenceSourceParent);
	  }

	  private void setReferenceSource(ModelElementInstance referenceSourceParent, Source referenceSource)
	  {
		ReferenceSourceChild.setChild(referenceSourceParent, referenceSource);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public Target getReferenceTargetElement(org.camunda.bpm.model.xml.impl.instance.ModelElementInstanceImpl referenceSourceParentElement)
	  public virtual Target getReferenceTargetElement(ModelElementInstanceImpl referenceSourceParentElement)
	  {
		Source referenceSource = getReferenceSource(referenceSourceParentElement);
		if (referenceSource != null)
		{
		  string identifier = getReferenceIdentifier(referenceSource);
		  ModelElementInstance referenceTargetElement = referenceSourceParentElement.ModelInstance.getModelElementById(identifier);
		  if (referenceTargetElement != null)
		  {
			return (Target) referenceTargetElement;
		  }
		  else
		  {
			throw new ModelException("Unable to find a model element instance for id " + identifier);
		  }
		}
		else
		{
		  return default(Target);
		}
	  }

	  public virtual void setReferenceTargetElement(ModelElementInstanceImpl referenceSourceParentElement, Target referenceTargetElement)
	  {
		ModelInstanceImpl modelInstance = referenceSourceParentElement.ModelInstance;
		string identifier = referenceTargetAttribute.getValue(referenceTargetElement);
		ModelElementInstance existingElement = modelInstance.getModelElementById(identifier);

		if (existingElement == null || !existingElement.Equals(referenceTargetElement))
		{
		  throw new ModelReferenceException("Cannot create reference to model element " + referenceTargetElement + ": element is not part of model. Please connect element to the model first.");
		}
		else
		{
		  Source referenceSourceElement = modelInstance.newInstance(ReferenceSourceElementType);
		  setReferenceSource(referenceSourceParentElement, referenceSourceElement);
		  setReferenceIdentifier(referenceSourceElement, identifier);
		}
	  }

	  public virtual void clearReferenceTargetElement(ModelElementInstanceImpl referenceSourceParentElement)
	  {
		ReferenceSourceChild.removeChild(referenceSourceParentElement);
	  }
	}

}