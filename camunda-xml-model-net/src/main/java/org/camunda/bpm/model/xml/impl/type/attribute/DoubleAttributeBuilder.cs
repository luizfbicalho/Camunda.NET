﻿/*
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
	/// @author Sebastian Menski
	/// </summary>
	public class DoubleAttributeBuilder : AttributeBuilderImpl<double>
	{

	  public DoubleAttributeBuilder(string attributeName, ModelElementTypeImpl modelType) : base(attributeName, modelType, new DoubleAttribute((modelType)))
	  {
	  }

	  public override DoubleAttributeBuilder @namespace(string namespaceUri)
	  {
		return (DoubleAttributeBuilder) base.@namespace(namespaceUri);
	  }

	  public virtual DoubleAttributeBuilder defaultValue(double? defaultValue)
	  {
		return (DoubleAttributeBuilder) base.defaultValue(defaultValue);
	  }

	  public override DoubleAttributeBuilder required()
	  {
		return (DoubleAttributeBuilder) base.required();
	  }

	  public override DoubleAttributeBuilder idAttribute()
	  {
		return (DoubleAttributeBuilder) base.idAttribute();
	  }
	}

}