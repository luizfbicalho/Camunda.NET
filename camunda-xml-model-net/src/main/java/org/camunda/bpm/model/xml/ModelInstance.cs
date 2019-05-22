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
	using ModelElementInstanceImpl = org.camunda.bpm.model.xml.impl.instance.ModelElementInstanceImpl;
	using DomDocument = org.camunda.bpm.model.xml.instance.DomDocument;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;
	using ModelElementValidator = org.camunda.bpm.model.xml.validation.ModelElementValidator;
	using ValidationResults = org.camunda.bpm.model.xml.validation.ValidationResults;

	/// <summary>
	/// An instance of a model
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface ModelInstance
	{

	  /// <summary>
	  /// Returns the wrapped <seealso cref="DomDocument"/>.
	  /// </summary>
	  /// <returns> the DOM document </returns>
	  DomDocument Document {get;}

	  /// <summary>
	  /// Returns the <seealso cref="ModelElementInstanceImpl ModelElement"/> corresponding to the document
	  /// element of this model or null if no document element exists.
	  /// </summary>
	  /// <returns> the document element or null </returns>
	  ModelElementInstance DocumentElement {get;set;}


	  /// <summary>
	  /// Creates a new instance of type class.
	  /// </summary>
	  /// <param name="type">  the class of the type to create </param>
	  /// @param <T>   instance type </param>
	  /// <returns> the new created instance </returns>
	  T newInstance<T>(Type type);

	  /// <summary>
	  /// Creates a new instance of type class with user-defined id.
	  /// </summary>
	  /// <param name="type">  the class of the type to create </param>
	  /// <param name="id">    identifier of new element instance </param>
	  /// @param <T>   instance type </param>
	  /// <returns> the new created instance </returns>
	  T newInstance<T>(Type type, string id);

	  /// <summary>
	  /// Creates a new instance of type.
	  /// </summary>
	  /// <param name="type">  the type to create </param>
	  /// @param <T>   instance type </param>
	  /// <returns> the new created instance </returns>
	  T newInstance<T>(ModelElementType type);

	  /// <summary>
	  /// Creates a new instance of type with user-defined id.
	  /// </summary>
	  /// <param name="type">  the type to create </param>
	  /// <param name="id">    identifier of new element instance </param>
	  /// @param <T>   instance type </param>
	  /// <returns>  the new created instance </returns>
	  T newInstance<T>(ModelElementType type, string id);

	  /// <summary>
	  /// Returns the underlying model.
	  /// </summary>
	  /// <returns> the model </returns>
	  Model Model {get;}

	  /// <summary>
	  /// Find a unique element of the model by id.
	  /// </summary>
	  /// <param name="id">  the id of the element </param>
	  /// <returns> the element with the id or null </returns>
	  T getModelElementById<T>(string id);

	  /// <summary>
	  /// Find all elements of a type.
	  /// </summary>
	  /// <param name="referencingType">  the type of the elements </param>
	  /// <returns> the collection of elements of the type </returns>
	  ICollection<ModelElementInstance> getModelElementsByType(ModelElementType referencingType);

	  /// <summary>
	  /// Find all elements of a type.
	  /// </summary>
	  /// <param name="referencingClass">  the type class of the elements </param>
	  /// <returns> the collection of elements of the type </returns>
	  ICollection<T> getModelElementsByType<T>(Type referencingClass);

	  /// <summary>
	  /// Copies the model instance but not the model. So only the wrapped DOM document is cloned.
	  /// Changes of the model are persistent between multiple model instances.
	  /// </summary>
	  /// <returns> the new model instance </returns>
	  ModelInstance clone();

	  /// <summary>
	  /// Validate semantic properties of this model instance using a collection of validators.
	  /// ModelElementValidator is an SPI that can be implemented by the user to execute custom
	  /// validation logic on the model. The validation results are collected into a <seealso cref="ValidationResults"/>
	  /// object which is returned by this method.
	  /// </summary>
	  /// <param name="validators"> the validators to execute </param>
	  /// <returns> the results of the validation.
	  /// @since 7.6 </returns>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.bpm.model.xml.validation.ValidationResults validate(java.util.Collection<org.camunda.bpm.model.xml.validation.ModelElementValidator<?>> validators);
	  ValidationResults validate<T1>(ICollection<T1> validators);

	}

}