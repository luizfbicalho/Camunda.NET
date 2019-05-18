using System;

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
	using AttributeImpl = org.camunda.bpm.model.xml.impl.type.attribute.AttributeImpl;
	using ChildElementCollectionImpl = org.camunda.bpm.model.xml.impl.type.child.ChildElementCollectionImpl;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ElementReferenceCollection = org.camunda.bpm.model.xml.type.reference.ElementReferenceCollection;
	using ElementReferenceCollectionBuilder = org.camunda.bpm.model.xml.type.reference.ElementReferenceCollectionBuilder;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class ElementReferenceCollectionBuilderImpl<Target, Source> : ElementReferenceCollectionBuilder<Target, Source> where Target : org.camunda.bpm.model.xml.instance.ModelElementInstance where Source : org.camunda.bpm.model.xml.instance.ModelElementInstance
	{

	  private readonly Type<Source> childElementType;
	  private readonly Type<Target> referenceTargetClass;
	  protected internal ElementReferenceCollectionImpl<Target, Source> elementReferenceCollectionImpl;

	  public ElementReferenceCollectionBuilderImpl(Type<Source> childElementType, Type<Target> referenceTargetClass, ChildElementCollectionImpl<Source> collection)
	  {
		this.childElementType = childElementType;
		this.referenceTargetClass = referenceTargetClass;
		this.elementReferenceCollectionImpl = new ElementReferenceCollectionImpl<Target, Source>(collection);
	  }

	  public virtual ElementReferenceCollection<Target, Source> build()
	  {
		return elementReferenceCollectionImpl;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public void performModelBuild(org.camunda.bpm.model.xml.Model model)
	  public virtual void performModelBuild(Model model)
	  {
		ModelElementTypeImpl referenceTargetType = (ModelElementTypeImpl) model.getType(referenceTargetClass);
		ModelElementTypeImpl referenceSourceType = (ModelElementTypeImpl) model.getType(childElementType);
		elementReferenceCollectionImpl.ReferenceTargetElementType = referenceTargetType;
		elementReferenceCollectionImpl.setReferenceSourceElementType(referenceSourceType);

		// the referenced attribute may be declared on a base type of the referenced type.
		AttributeImpl<string> idAttribute = (AttributeImpl<string>) referenceTargetType.getAttribute("id");
		if (idAttribute != null)
		{
		  idAttribute.registerIncoming(elementReferenceCollectionImpl);
		  elementReferenceCollectionImpl.ReferenceTargetAttribute = idAttribute;
		}
		else
		{
		  throw new ModelException("Unable to find id attribute of " + referenceTargetClass);
		}
	  }
	}

}