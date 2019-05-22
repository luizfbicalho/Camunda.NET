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
namespace org.camunda.bpm.model.xml.impl.type.attribute
{
	using AttributeReferenceBuilderImpl = org.camunda.bpm.model.xml.impl.type.reference.AttributeReferenceBuilderImpl;
	using AttributeReferenceCollectionBuilderImpl = org.camunda.bpm.model.xml.impl.type.reference.AttributeReferenceCollectionBuilderImpl;
	using QNameAttributeReferenceBuilderImpl = org.camunda.bpm.model.xml.impl.type.reference.QNameAttributeReferenceBuilderImpl;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using StringAttributeBuilder = org.camunda.bpm.model.xml.type.attribute.StringAttributeBuilder;
	using AttributeReferenceBuilder = org.camunda.bpm.model.xml.type.reference.AttributeReferenceBuilder;
	using AttributeReferenceCollection = org.camunda.bpm.model.xml.type.reference.AttributeReferenceCollection;
	using AttributeReferenceCollectionBuilder = org.camunda.bpm.model.xml.type.reference.AttributeReferenceCollectionBuilder;


	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class StringAttributeBuilderImpl : AttributeBuilderImpl<string>, StringAttributeBuilder
	{

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: private org.camunda.bpm.model.xml.type.reference.AttributeReferenceBuilder<?> referenceBuilder;
	  private AttributeReferenceBuilder<object> referenceBuilder;

	  public StringAttributeBuilderImpl(string attributeName, ModelElementTypeImpl modelType) : base(attributeName, modelType, new StringAttribute(modelType))
	  {
	  }

	  public override StringAttributeBuilder @namespace(string namespaceUri)
	  {
		return (StringAttributeBuilder) base.@namespace(namespaceUri);
	  }

	  public virtual StringAttributeBuilder defaultValue(string defaultValue)
	  {
		return (StringAttributeBuilder) base.defaultValue(defaultValue);
	  }

	  public override StringAttributeBuilder required()
	  {
		return (StringAttributeBuilder) base.required();
	  }

	  public override StringAttributeBuilder idAttribute()
	  {
		return (StringAttributeBuilder) base.idAttribute();
	  }

	  /// <summary>
	  /// Create a new <seealso cref="AttributeReferenceBuilder"/> for the reference source element instance
	  /// </summary>
	  /// <param name="referenceTargetElement"> the reference target model element instance </param>
	  /// <returns> the new attribute reference builder </returns>
	  public virtual AttributeReferenceBuilder<V> qNameAttributeReference<V>(Type referenceTargetElement) where V : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
			  referenceTargetElement = typeof(V);
		AttributeImpl<string> attribute = (AttributeImpl<string>) build();
		AttributeReferenceBuilderImpl<V> referenceBuilder = new QNameAttributeReferenceBuilderImpl<V>(attribute, referenceTargetElement);
		AttributeReference = referenceBuilder;
		return referenceBuilder;
	  }

	  public virtual AttributeReferenceBuilder<V> idAttributeReference<V>(Type referenceTargetElement) where V : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
			  referenceTargetElement = typeof(V);
		AttributeImpl<string> attribute = (AttributeImpl<string>) build();
		AttributeReferenceBuilderImpl<V> referenceBuilder = new AttributeReferenceBuilderImpl<V>(attribute, referenceTargetElement);
		AttributeReference = referenceBuilder;
		return referenceBuilder;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public <V extends org.camunda.bpm.model.xml.instance.ModelElementInstance> org.camunda.bpm.model.xml.type.reference.AttributeReferenceCollectionBuilder<V> idAttributeReferenceCollection(Class<V> referenceTargetElement, Class attributeReferenceCollection)
	  public virtual AttributeReferenceCollectionBuilder<V> idAttributeReferenceCollection<V>(Type referenceTargetElement, Type attributeReferenceCollection) where V : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
			  referenceTargetElement = typeof(V);
		AttributeImpl<string> attribute = (AttributeImpl<string>) build();
		AttributeReferenceCollectionBuilder<V> referenceBuilder = new AttributeReferenceCollectionBuilderImpl<V>(attribute, referenceTargetElement, attributeReferenceCollection);
		AttributeReference = referenceBuilder;
		return referenceBuilder;
	  }

	  protected internal virtual AttributeReferenceBuilder<V> AttributeReference<V> where V : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
		  set
		  {
			if (this.referenceBuilder != null)
			{
			  throw new ModelException("An attribute cannot have more than one reference");
			}
			this.referenceBuilder = value;
		  }
	  }


	  public override void performModelBuild(Model model)
	  {
		base.performModelBuild(model);
		if (referenceBuilder != null)
		{
		  ((ModelBuildOperation) referenceBuilder).performModelBuild(model);
		}
	  }

	}

}