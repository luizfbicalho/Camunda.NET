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
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ChildElementBuilder = org.camunda.bpm.model.xml.type.child.ChildElementBuilder;
	using ChildElementCollectionBuilder = org.camunda.bpm.model.xml.type.child.ChildElementCollectionBuilder;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;


	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class SequenceBuilderImpl : SequenceBuilder, ModelBuildOperation
	{

	  private readonly ModelElementTypeImpl elementType;

	  private readonly IList<ModelBuildOperation> modelBuildOperations = new List<ModelBuildOperation>();

	  public SequenceBuilderImpl(ModelElementTypeImpl modelType)
	  {
		this.elementType = modelType;
	  }

	  public virtual ChildElementBuilder<T> element<T>(Type childElementType) where T : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
			  childElementType = typeof(T);
		ChildElementBuilderImpl<T> builder = new ChildElementBuilderImpl<T>(childElementType, elementType);
		modelBuildOperations.Add(builder);
		return builder;
	  }

	  public virtual ChildElementCollectionBuilder<T> elementCollection<T>(Type childElementType) where T : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
			  childElementType = typeof(T);
		ChildElementCollectionBuilderImpl<T> builder = new ChildElementCollectionBuilderImpl<T>(childElementType, elementType);
		modelBuildOperations.Add(builder);
		return builder;
	  }

	  public virtual void performModelBuild(Model model)
	  {
		foreach (ModelBuildOperation operation in modelBuildOperations)
		{
		  operation.performModelBuild(model);
		}
	  }

	}

}