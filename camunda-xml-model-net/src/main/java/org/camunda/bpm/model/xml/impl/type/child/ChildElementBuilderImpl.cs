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
namespace org.camunda.bpm.model.xml.impl.type.child
{
	using ElementReferenceBuilderImpl = org.camunda.bpm.model.xml.impl.type.reference.ElementReferenceBuilderImpl;
	using QNameElementReferenceBuilderImpl = org.camunda.bpm.model.xml.impl.type.reference.QNameElementReferenceBuilderImpl;
	using UriElementReferenceBuilderImpl = org.camunda.bpm.model.xml.impl.type.reference.UriElementReferenceBuilderImpl;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;
	using ChildElement = org.camunda.bpm.model.xml.type.child.ChildElement;
	using ChildElementBuilder = org.camunda.bpm.model.xml.type.child.ChildElementBuilder;
	using ElementReferenceBuilder = org.camunda.bpm.model.xml.type.reference.ElementReferenceBuilder;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ChildElementBuilderImpl<T> : ChildElementCollectionBuilderImpl<T>, ChildElementBuilder<T> where T : org.camunda.bpm.model.xml.instance.ModelElementInstance
	{

	  public ChildElementBuilderImpl(Type<T> childElementTypeClass, ModelElementType parentElementType) : base(childElementTypeClass, parentElementType)
	  {
	  }

	  protected internal override ChildElementCollectionImpl<T> createCollectionInstance()
	  {
		return new ChildElementImpl<T>(childElementType, parentElementType);
	  }

	  public override ChildElementBuilder<T> immutable()
	  {
		base.immutable();
		return this;
	  }

	  public override ChildElementBuilder<T> required()
	  {
		base.required();
		return this;
	  }

	  public override ChildElementBuilder<T> minOccurs(int i)
	  {
		base.minOccurs(i);
		return this;
	  }

	  public override ChildElementBuilder<T> maxOccurs(int i)
	  {
		base.maxOccurs(i);
		return this;
	  }

	  public override ChildElement<T> build()
	  {
		return (ChildElement<T>) base.build();
	  }

	  public virtual ElementReferenceBuilder<V, T> qNameElementReference<V>(Type<V> referenceTargetType) where V : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
		ChildElementImpl<T> child = (ChildElementImpl<T>) build();
		QNameElementReferenceBuilderImpl<V, T> builder = new QNameElementReferenceBuilderImpl<V, T>(childElementType, referenceTargetType, child);
		ReferenceBuilder = builder;
		return builder;
	  }

	  public virtual ElementReferenceBuilder<V, T> idElementReference<V>(Type<V> referenceTargetType) where V : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
		ChildElementImpl<T> child = (ChildElementImpl<T>) build();
		ElementReferenceBuilderImpl<V, T> builder = new ElementReferenceBuilderImpl<V, T>(childElementType, referenceTargetType, child);
		ReferenceBuilder = builder;
		return builder;
	  }

	  public virtual ElementReferenceBuilder<V, T> uriElementReference<V>(Type<V> referenceTargetType) where V : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
		ChildElementImpl<T> child = (ChildElementImpl<T>) build();
		ElementReferenceBuilderImpl<V, T> builder = new UriElementReferenceBuilderImpl<V, T>(childElementType, referenceTargetType, child);
		ReferenceBuilder = builder;
		return builder;
	  }

	}

}