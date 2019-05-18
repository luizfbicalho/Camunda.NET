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
namespace org.camunda.bpm.model.xml.type.reference
{
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;

	/// 
	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	/// @param <T> the type of the referenced element </param>
	public interface Reference<T> where T : org.camunda.bpm.model.xml.instance.ModelElementInstance
	{

	  /// <summary>
	  /// Get the reference identifier which is set in the reference source
	  /// </summary>
	  /// <param name="referenceSourceElement"> the reference source model element instance </param>
	  /// <returns> the reference identifier </returns>
	  string getReferenceIdentifier(ModelElementInstance referenceSourceElement);

	  T getReferenceTargetElement(ModelElementInstance modelElement);

	  void setReferenceTargetElement(ModelElementInstance referenceSourceElement, T referenceTargetElement);

	  Attribute<string> ReferenceTargetAttribute {get;}

	  /// <summary>
	  /// Find all reference source element instances of the reference target model element instance
	  /// </summary>
	  /// <param name="referenceTargetElement"> the reference target model element instance </param>
	  /// <returns> the collection of all reference source element instances </returns>
	  ICollection<ModelElementInstance> findReferenceSourceElements(ModelElementInstance referenceTargetElement);

	  /// <returns> the <seealso cref="ModelElementType"/> of the source element.
	  ///  </returns>
	  ModelElementType ReferenceSourceElementType {get;}
	}

}