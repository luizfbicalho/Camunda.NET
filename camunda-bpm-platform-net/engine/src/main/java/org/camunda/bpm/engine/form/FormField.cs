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
namespace org.camunda.bpm.engine.form
{

	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// <para>Represents an individual field in a form.</para>
	/// 
	/// @author Michael Siebers
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface FormField
	{

	  /// <returns> the Id of a form property. Must be unique for a given form.
	  /// The id is used for mapping the form field to a process variable. </returns>
	  string Id {get;}

	  /// <returns> the human-readable display name of a form property. </returns>
	  string Label {get;}

	  /// <returns> the type of this form field. </returns>
	  FormType Type {get;}

	  /// <returns> the name of the type of this form field </returns>
	  string TypeName {get;}

	  /// <returns> the default value for this form field. </returns>
	  [Obsolete]
	  object DefaultValue {get;}

	  /// <returns> the value for this form field </returns>
	  TypedValue Value {get;}

	  /// <returns> a list of <seealso cref="FormFieldValidationConstraint ValidationConstraints"/>. </returns>
	  IList<FormFieldValidationConstraint> ValidationConstraints {get;}

	  /// <returns> a <seealso cref="Map"/> of additional properties. This map may be used for adding additional configuration
	  /// to a form field. An example may be layout hints such as the size of the rendered form field or information
	  /// about an icon to prepend or append to the rendered form field. </returns>
	  IDictionary<string, string> Properties {get;}

	  /// <returns> true if field is defined as businessKey, false otherwise </returns>
	  bool BusinessKey {get;}

	}

}