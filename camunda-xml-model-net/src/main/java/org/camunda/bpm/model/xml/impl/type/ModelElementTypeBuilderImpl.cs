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
namespace org.camunda.bpm.model.xml.impl.type
{
	using org.camunda.bpm.model.xml.impl.type.attribute;
	using SequenceBuilderImpl = org.camunda.bpm.model.xml.impl.type.child.SequenceBuilderImpl;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using AttributeBuilder = org.camunda.bpm.model.xml.type.attribute.AttributeBuilder;
	using StringAttributeBuilder = org.camunda.bpm.model.xml.type.attribute.StringAttributeBuilder;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;


	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ModelElementTypeBuilderImpl : ModelElementTypeBuilder, ModelBuildOperation
	{

	  private readonly ModelElementTypeImpl modelType;
	  private readonly ModelImpl model;
	  private readonly Type instanceType;

	  private readonly IList<ModelBuildOperation> modelBuildOperations = new List<ModelBuildOperation>();
	  private Type extendedType;

	  public ModelElementTypeBuilderImpl(Type instanceType, string name, ModelImpl model)
	  {
		this.instanceType = instanceType;
		this.model = model;
		modelType = new ModelElementTypeImpl(model, name, instanceType);
	  }

	  public virtual ModelElementTypeBuilder extendsType(Type extendedType)
	  {
		this.extendedType = extendedType;
		return this;
	  }

	  public virtual ModelElementTypeBuilder instanceProvider<T>(org.camunda.bpm.model.xml.type.ModelElementTypeBuilder_ModelTypeInstanceProvider<T> instanceProvider) where T : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
		modelType.InstanceProvider = instanceProvider;
		return this;
	  }

	  public virtual ModelElementTypeBuilder namespaceUri(string namespaceUri)
	  {
		modelType.TypeNamespace = namespaceUri;
		return this;
	  }

	  public virtual AttributeBuilder<bool> booleanAttribute(string attributeName)
	  {
		BooleanAttributeBuilder builder = new BooleanAttributeBuilder(attributeName, modelType);
		modelBuildOperations.Add(builder);
		return builder;
	  }

	  public virtual StringAttributeBuilder stringAttribute(string attributeName)
	  {
		StringAttributeBuilderImpl builder = new StringAttributeBuilderImpl(attributeName, modelType);
		modelBuildOperations.Add(builder);
		return builder;
	  }

	  public virtual AttributeBuilder<int> integerAttribute(string attributeName)
	  {
		IntegerAttributeBuilder builder = new IntegerAttributeBuilder(attributeName, modelType);
		modelBuildOperations.Add(builder);
		return builder;
	  }

	  public virtual AttributeBuilder<double> doubleAttribute(string attributeName)
	  {
		DoubleAttributeBuilder builder = new DoubleAttributeBuilder(attributeName, modelType);
		modelBuildOperations.Add(builder);
		return builder;
	  }

	  public virtual AttributeBuilder<V> enumAttribute<V>(string attributeName, Type<V> enumType) where V : Enum<V>
	  {
		EnumAttributeBuilder<V> builder = new EnumAttributeBuilder<V>(attributeName, modelType, enumType);
		modelBuildOperations.Add(builder);
		return builder;
	  }

	  public virtual AttributeBuilder<V> namedEnumAttribute<V>(string attributeName, Type<V> enumType) where V : Enum<V>
	  {
		NamedEnumAttributeBuilder<V> builder = new NamedEnumAttributeBuilder<V>(attributeName, modelType, enumType);
		modelBuildOperations.Add(builder);
		return builder;
	  }

	  public virtual ModelElementType build()
	  {
		model.registerType(modelType, instanceType);
		return modelType;
	  }

	  public virtual ModelElementTypeBuilder abstractType()
	  {
		modelType.Abstract = true;
		return this;
	  }

	  public virtual SequenceBuilder sequence()
	  {
		SequenceBuilderImpl builder = new SequenceBuilderImpl(modelType);
		modelBuildOperations.Add(builder);
		return builder;
	  }

	  public virtual void buildTypeHierarchy(Model model)
	  {

		// build type hierarchy
		if (extendedType != null)
		{
		  ModelElementTypeImpl extendedModelElementType = (ModelElementTypeImpl) model.getType(extendedType);
		  if (extendedModelElementType == null)
		  {
			throw new ModelException("Type " + modelType + " is defined to extend " + extendedType + " but no such type is defined.");

		  }
		  else
		  {
			modelType.setBaseType(extendedModelElementType);
			extendedModelElementType.registerExtendingType(modelType);
		  }
		}
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