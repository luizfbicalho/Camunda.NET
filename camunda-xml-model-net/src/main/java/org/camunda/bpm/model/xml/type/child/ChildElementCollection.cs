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
namespace org.camunda.bpm.model.xml.type.child
{
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;

	/// <summary>
	/// <para>A collection containing all or a subset of the child
	/// elements of a given <seealso cref="ModelElementInstance"/>.</para>
	/// 
	/// @author Daniel Meyer
	/// </summary>
	/// @param <T> The type of the model elements in the collection </param>
	public interface ChildElementCollection<T> where T : org.camunda.bpm.model.xml.instance.ModelElementInstance
	{

	  /// <summary>
	  /// Indicates whether the collection is immutable.
	  /// 
	  /// If the collection is immutable, all state-altering operations such
	  /// as <seealso cref="Collection#add(Object)"/> or <seealso cref="Collection#remove(Object)"/>
	  /// will throw an <seealso cref="UnsupportedOperationException"/>.
	  /// </summary>
	  /// <returns> true if the collection is mutable, false otherwise. </returns>
	  bool Immutable {get;}

	  /// <summary>
	  /// Indicates the minimal element count of a collection. Returns a positive integer or '0'. </summary>
	  /// <returns> the minimal element count of the collection. </returns>
	  int MinOccurs {get;}

	  /// <summary>
	  /// Indicates the max element count of a collection. In  a negative value is returned (like '-1'),
	  /// the collection is unbounded.
	  /// </summary>
	  /// <returns> the max element count for this collection. </returns>
	  int MaxOccurs {get;}

	  /// <summary>
	  /// Get the model element type of the elements contained in this collection.
	  /// </summary>
	  /// <param name="model">  the model of the element </param>
	  /// <returns> the containing <seealso cref="ModelElementType"/> </returns>
	  ModelElementType getChildElementType(Model model);

	  /// <summary>
	  /// Get the class of the elements contained in this collection.
	  /// </summary>
	  /// <returns> the class of contained types </returns>
	  Type<T> ChildElementTypeClass {get;}

	  /// <summary>
	  /// Get the model element type of the element owns the collection
	  /// </summary>
	  /// <returns> the parent element of the collection </returns>
	  ModelElementType ParentElementType {get;}

	  /// <summary>
	  /// returns a <seealso cref="Collection"/> containing all or a subset of the child elements of
	  /// a  <seealso cref="ModelElementInstance"/>.  
	  /// </summary>
	  ICollection<T> get(ModelElementInstance element);

	}

}