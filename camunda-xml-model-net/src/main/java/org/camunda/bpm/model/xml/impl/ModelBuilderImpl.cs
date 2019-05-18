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
namespace org.camunda.bpm.model.xml.impl
{
	using ModelElementInstanceImpl = org.camunda.bpm.model.xml.impl.instance.ModelElementInstanceImpl;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilderImpl = org.camunda.bpm.model.xml.impl.type.ModelElementTypeBuilderImpl;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
	using static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder_ModelTypeInstanceProvider;

	/// <summary>
	/// This builder is used to define and create a new model.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ModelBuilderImpl : ModelBuilder
	{

	  private readonly IList<ModelElementTypeBuilderImpl> typeBuilders = new List<ModelElementTypeBuilderImpl>();
	  private readonly ModelImpl model;

	  public ModelBuilderImpl(string modelName)
	  {
		model = new ModelImpl(modelName);
	  }

	  public override ModelBuilder alternativeNamespace(string alternativeNs, string actualNs)
	  {
		model.declareAlternativeNamespace(alternativeNs, actualNs);
		return this;
	  }

	  public override ModelElementTypeBuilder defineType(Type modelInstanceType, string typeName)
	  {
		ModelElementTypeBuilderImpl typeBuilder = new ModelElementTypeBuilderImpl(modelInstanceType, typeName, model);
		typeBuilders.Add(typeBuilder);
		return typeBuilder;
	  }

	  public override ModelElementType defineGenericType(string typeName, string typeNamespaceUri)
	  {
		ModelElementTypeBuilder typeBuilder = defineType(typeof(ModelElementInstance), typeName).namespaceUri(typeNamespaceUri).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass(this));

		return typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<ModelElementInstance>
	  {
		  private readonly ModelBuilderImpl outerInstance;

		  public ModelTypeInstanceProviderAnonymousInnerClass(ModelBuilderImpl outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public ModelElementInstance newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new ModelElementInstanceImpl(instanceContext);
		  }
	  }

	  public override Model build()
	  {
		foreach (ModelElementTypeBuilderImpl typeBuilder in typeBuilders)
		{
		  typeBuilder.buildTypeHierarchy(model);
		}
		foreach (ModelElementTypeBuilderImpl typeBuilder in typeBuilders)
		{
		  typeBuilder.performModelBuild(model);
		}
		return model;
	  }

	}

}