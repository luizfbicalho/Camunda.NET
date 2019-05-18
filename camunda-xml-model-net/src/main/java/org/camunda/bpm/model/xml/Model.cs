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
namespace org.camunda.bpm.model.xml
{
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;

	/// <summary>
	/// A model contains all defined types and the relationship between them.
	/// See <seealso cref="ModelBuilder#createInstance"/> to create a new model.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface Model
	{

	  /// <summary>
	  /// Gets the collection of all <seealso cref="ModelElementType"/> defined in the model.
	  /// </summary>
	  /// <returns> the list of all defined element types of this model </returns>
	  ICollection<ModelElementType> Types {get;}

	  /// <summary>
	  /// Gets the defined <seealso cref="ModelElementType"/> of a <seealso cref="ModelElementInstance"/>.
	  /// </summary>
	  /// <param name="instanceClass">  the instance class to find the type for </param>
	  /// <returns> the corresponding element type or null if no type is defined for the instance </returns>
	  ModelElementType getType(Type instanceClass);

	  /// <summary>
	  /// Gets the defined <seealso cref="ModelElementType"/> for a type by its name.
	  /// </summary>
	  /// <param name="typeName">  the name of the type </param>
	  /// <returns> the element type or null if no type is defined for the name </returns>
	  ModelElementType getTypeForName(string typeName);

	  /// <summary>
	  /// Gets the defined <seealso cref="ModelElementType"/> for a type by its name and namespace URI.
	  /// 
	  /// </summary>
	  /// <param name="namespaceUri">  the namespace URI for the type </param>
	  /// <param name="typeName">  the name of the type </param>
	  /// <returns> the element type or null if no type is defined for the name and namespace URI </returns>
	  ModelElementType getTypeForName(string namespaceUri, string typeName);

	  /// <summary>
	  /// Returns the model name, which is the identifier of this model.
	  /// </summary>
	  /// <returns> the model name </returns>
	  string ModelName {get;}

	  /// <summary>
	  /// Returns the actual namespace URI for an alternative namespace URI </summary>
	  /// <param name="alternativeNs"> the alternative namespace URI </param>
	  /// <returns> the actual namespace URI or null if none is set </returns>
	  string getActualNamespace(string alternativeNs);

	  /// <summary>
	  /// Returns the alternative namespace URI for a namespace URI </summary>
	  /// <param name="actualNs"> the actual namespace URI </param>
	  /// <returns> the alternative namespace URI or null if none is set </returns>
	  string getAlternativeNamespace(string actualNs);

	}

}