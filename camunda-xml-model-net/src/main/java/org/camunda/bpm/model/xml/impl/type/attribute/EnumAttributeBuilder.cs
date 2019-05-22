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

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class EnumAttributeBuilder<T> : AttributeBuilderImpl<T> where T : Enum<T>
	{

	  public EnumAttributeBuilder(string attributeName, ModelElementTypeImpl modelType, Type type)
	  {
			  type = typeof(T);
		base(attributeName, modelType, new EnumAttribute<T>(modelType, type));
	  }

	  public override EnumAttributeBuilder<T> @namespace(string namespaceUri)
	  {
		return (EnumAttributeBuilder<T>) base.@namespace(namespaceUri);
	  }

	  public override EnumAttributeBuilder<T> defaultValue(T defaultValue)
	  {
		return (EnumAttributeBuilder<T>) base.defaultValue(defaultValue);
	  }

	  public override EnumAttributeBuilder<T> required()
	  {
		return (EnumAttributeBuilder<T>) base.required();
	  }

	  public override EnumAttributeBuilder<T> idAttribute()
	  {
		return (EnumAttributeBuilder<T>) base.idAttribute();
	  }
	}

}