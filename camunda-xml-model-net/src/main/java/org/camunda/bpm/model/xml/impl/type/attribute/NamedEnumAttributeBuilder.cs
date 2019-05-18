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

	public class NamedEnumAttributeBuilder<T> : AttributeBuilderImpl<T> where T : Enum<T>
	{

	  public NamedEnumAttributeBuilder(string attributeName, ModelElementTypeImpl modelType, Type<T> type) : base(attributeName, modelType, new NamedEnumAttribute<T>(modelType, type))
	  {
	  }

	  public override NamedEnumAttributeBuilder<T> @namespace(string namespaceUri)
	  {
		return (NamedEnumAttributeBuilder<T>) base.@namespace(namespaceUri);
	  }

	  public override NamedEnumAttributeBuilder<T> defaultValue(T defaultValue)
	  {
		return (NamedEnumAttributeBuilder<T>) base.defaultValue(defaultValue);
	  }

	  public override NamedEnumAttributeBuilder<T> required()
	  {
		return (NamedEnumAttributeBuilder<T>) base.required();
	  }

	  public override NamedEnumAttributeBuilder<T> idAttribute()
	  {
		return (NamedEnumAttributeBuilder<T>) base.idAttribute();
	  }

	}

}