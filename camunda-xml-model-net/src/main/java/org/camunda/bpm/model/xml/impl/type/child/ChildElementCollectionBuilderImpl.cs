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
	using ElementReferenceCollectionBuilderImpl = org.camunda.bpm.model.xml.impl.type.reference.ElementReferenceCollectionBuilderImpl;
	using IdsElementReferenceCollectionBuilderImpl = org.camunda.bpm.model.xml.impl.type.reference.IdsElementReferenceCollectionBuilderImpl;
	using QNameElementReferenceCollectionBuilderImpl = org.camunda.bpm.model.xml.impl.type.reference.QNameElementReferenceCollectionBuilderImpl;
	using UriElementReferenceCollectionBuilderImpl = org.camunda.bpm.model.xml.impl.type.reference.UriElementReferenceCollectionBuilderImpl;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;
	using ChildElementCollectionBuilder = org.camunda.bpm.model.xml.type.child.ChildElementCollectionBuilder;
	using ElementReferenceCollectionBuilder = org.camunda.bpm.model.xml.type.reference.ElementReferenceCollectionBuilder;


	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ChildElementCollectionBuilderImpl<T> : ChildElementCollectionBuilder<T>, ModelBuildOperation where T : org.camunda.bpm.model.xml.instance.ModelElementInstance
	{

	  /// <summary>
	  /// The <seealso cref="ModelElementType"/> of the element containing the collection </summary>
	  protected internal readonly ModelElementTypeImpl parentElementType;
	  private readonly ChildElementCollectionImpl<T> collection;
	  protected internal readonly Type<T> childElementType;

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: private org.camunda.bpm.model.xml.type.reference.ElementReferenceCollectionBuilder<?, ?> referenceBuilder;
	  private ElementReferenceCollectionBuilder<object, ?> referenceBuilder;

	  private readonly IList<ModelBuildOperation> modelBuildOperations = new List<ModelBuildOperation>();

	  public ChildElementCollectionBuilderImpl(Type<T> childElementTypeClass, ModelElementType parentElementType)
	  {
		this.childElementType = childElementTypeClass;
		this.parentElementType = (ModelElementTypeImpl) parentElementType;
		this.collection = createCollectionInstance();

	  }

	  protected internal virtual ChildElementCollectionImpl<T> createCollectionInstance()
	  {
		return new ChildElementCollectionImpl<T>(childElementType, parentElementType);
	  }

	  public virtual ChildElementCollectionBuilder<T> immutable()
	  {
		collection.setImmutable();
		return this;
	  }

	  public virtual ChildElementCollectionBuilder<T> required()
	  {
		collection.MinOccurs = 1;
		return this;
	  }

	  public virtual ChildElementCollectionBuilder<T> maxOccurs(int i)
	  {
		collection.MaxOccurs = i;
		return this;
	  }

	  public virtual ChildElementCollectionBuilder<T> minOccurs(int i)
	  {
		collection.MinOccurs = i;
		return this;
	  }

	  public virtual ChildElementCollection<T> build()
	  {
		return collection;
	  }

	  public virtual ElementReferenceCollectionBuilder<V, T> qNameElementReferenceCollection<V>(Type<V> referenceTargetType) where V : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
		ChildElementCollectionImpl<T> collection = (ChildElementCollectionImpl<T>) build();
		QNameElementReferenceCollectionBuilderImpl<V, T> builder = new QNameElementReferenceCollectionBuilderImpl<V, T>(childElementType, referenceTargetType, collection);
		ReferenceBuilder = builder;
		return builder;
	  }

	  public virtual ElementReferenceCollectionBuilder<V, T> idElementReferenceCollection<V>(Type<V> referenceTargetType) where V : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
		ChildElementCollectionImpl<T> collection = (ChildElementCollectionImpl<T>) build();
		ElementReferenceCollectionBuilder<V, T> builder = new ElementReferenceCollectionBuilderImpl<V, T>(childElementType, referenceTargetType, collection);
		ReferenceBuilder = builder;
		return builder;
	  }

	  public virtual ElementReferenceCollectionBuilder<V, T> idsElementReferenceCollection<V>(Type<V> referenceTargetType) where V : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
		ChildElementCollectionImpl<T> collection = (ChildElementCollectionImpl<T>) build();
		ElementReferenceCollectionBuilder<V, T> builder = new IdsElementReferenceCollectionBuilderImpl<V, T>(childElementType, referenceTargetType, collection);
		ReferenceBuilder = builder;
		return builder;
	  }

	  public virtual ElementReferenceCollectionBuilder<V, T> uriElementReferenceCollection<V>(Type<V> referenceTargetType) where V : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
		ChildElementCollectionImpl<T> collection = (ChildElementCollectionImpl<T>) build();
		ElementReferenceCollectionBuilder<V, T> builder = new UriElementReferenceCollectionBuilderImpl<V, T>(childElementType, referenceTargetType, collection);
		ReferenceBuilder = builder;
		return builder;
	  }

	  protected internal virtual ElementReferenceCollectionBuilder<T1> ReferenceBuilder<T1>
	  {
		  set
		  {
			if (this.referenceBuilder != null)
			{
			  throw new ModelException("An collection cannot have more than one reference");
			}
			this.referenceBuilder = value;
			modelBuildOperations.Add(value);
		  }
	  }

	  public virtual void performModelBuild(Model model)
	  {
		ModelElementType elementType = model.getType(childElementType);
		if (elementType == null)
		{
		  throw new ModelException(parentElementType + " declares undefined child element of type " + childElementType + ".");
		}
		parentElementType.registerChildElementType(elementType);
		parentElementType.registerChildElementCollection(collection);
		foreach (ModelBuildOperation modelBuildOperation in modelBuildOperations)
		{
		  modelBuildOperation.performModelBuild(model);
		}
	  }

	}

}