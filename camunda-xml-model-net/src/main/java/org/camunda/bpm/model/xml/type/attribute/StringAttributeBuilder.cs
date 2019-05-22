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
namespace org.camunda.bpm.model.xml.type.attribute
{
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using AttributeReferenceBuilder = org.camunda.bpm.model.xml.type.reference.AttributeReferenceBuilder;
	using AttributeReferenceCollection = org.camunda.bpm.model.xml.type.reference.AttributeReferenceCollection;
	using AttributeReferenceCollectionBuilder = org.camunda.bpm.model.xml.type.reference.AttributeReferenceCollectionBuilder;

	/// <summary>
	/// @author Sebastian Menski
	/// 
	/// </summary>
	public interface StringAttributeBuilder : AttributeBuilder<string>
	{

	  StringAttributeBuilder @namespace(string namespaceUri);

	  StringAttributeBuilder defaultValue(string defaultValue);

	  StringAttributeBuilder required();

	  StringAttributeBuilder idAttribute();

	  AttributeReferenceBuilder<V> qNameAttributeReference<V>(Type referenceTargetElement);

	  AttributeReferenceBuilder<V> idAttributeReference<V>(Type referenceTargetElement);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") <V extends org.camunda.bpm.model.xml.instance.ModelElementInstance> org.camunda.bpm.model.xml.type.reference.AttributeReferenceCollectionBuilder<V> idAttributeReferenceCollection(Class<V> referenceTargetElement, Class attributeReferenceCollection);
	  AttributeReferenceCollectionBuilder<V> idAttributeReferenceCollection<V>(Type referenceTargetElement, Type attributeReferenceCollection);

	}

}