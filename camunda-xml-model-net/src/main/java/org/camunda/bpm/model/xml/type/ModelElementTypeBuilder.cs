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
namespace org.camunda.bpm.model.xml.type
{
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using AttributeBuilder = org.camunda.bpm.model.xml.type.attribute.AttributeBuilder;
	using StringAttributeBuilder = org.camunda.bpm.model.xml.type.attribute.StringAttributeBuilder;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;

	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public interface ModelElementTypeBuilder
	{

	  ModelElementTypeBuilder namespaceUri(string namespaceUri);

	  ModelElementTypeBuilder extendsType(Type extendedType);

	  ModelElementTypeBuilder instanceProvider<T>(ModelElementTypeBuilder_ModelTypeInstanceProvider<T> instanceProvider);

	  ModelElementTypeBuilder abstractType();

	  AttributeBuilder<bool> booleanAttribute(string attributeName);

	  StringAttributeBuilder stringAttribute(string attributeName);

	  AttributeBuilder<int> integerAttribute(string attributeName);

	  AttributeBuilder<double> doubleAttribute(string attributeName);

	  AttributeBuilder<V> enumAttribute<V>(string attributeName, Type<V> enumType);

	  AttributeBuilder<V> namedEnumAttribute<V>(string attributeName, Type<V> enumType);

	  SequenceBuilder sequence();

	  ModelElementType build();


	}

	  public interface ModelElementTypeBuilder_ModelTypeInstanceProvider<T> where T : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
	T newInstance(ModelTypeInstanceContext instanceContext);
	  }

}