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
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using AttributeBuilder = org.camunda.bpm.model.xml.type.attribute.AttributeBuilder;


	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public abstract class AttributeBuilderImpl<T> : AttributeBuilder<T>, ModelBuildOperation
	{

	  private readonly AttributeImpl<T> attribute;
	  private readonly ModelElementTypeImpl modelType;

	  internal AttributeBuilderImpl(string attributeName, ModelElementTypeImpl modelType, AttributeImpl<T> attribute)
	  {
		this.modelType = modelType;
		this.attribute = attribute;
		attribute.AttributeName = attributeName;
	  }

	  public virtual AttributeBuilder<T> @namespace(string namespaceUri)
	  {
		attribute.NamespaceUri = namespaceUri;
		return this;
	  }

	  public virtual AttributeBuilder<T> idAttribute()
	  {
		attribute.setId();
		return this;
	  }


	  public virtual AttributeBuilder<T> defaultValue(T defaultValue)
	  {
		attribute.DefaultValue = defaultValue;
		return this;
	  }

	  public virtual AttributeBuilder<T> required()
	  {
		attribute.Required = true;
		return this;
	  }

	  public virtual Attribute<T> build()
	  {
		modelType.registerAttribute(attribute);
		return attribute;
	  }

	  public virtual void performModelBuild(Model model)
	  {
		// do nothing
	  }

	}

}