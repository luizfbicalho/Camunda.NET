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
namespace org.camunda.bpm.model.xml.validation
{

	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;

	/// <summary>
	/// Object in which the results of a model validation are collected.
	/// See: <seealso cref="ModelInstance.validate(System.Collections.ICollection)"/>.
	/// 
	/// @author Daniel Meyer
	/// @since 7.6
	/// </summary>
	public interface ValidationResults
	{

	  /// <returns> true if there are <seealso cref="ValidationResult"/> of type <seealso cref="ValidationResultType.ERROR"/> </returns>
	  bool hasErrors();

	  /// <returns> the count of <seealso cref="ValidationResult"/> of type <seealso cref="ValidationResultType.ERROR"/> </returns>
	  int ErrorCount {get;}

	  /// <returns> the count of <seealso cref="ValidationResult"/> of type <seealso cref="ValidationResultType.WARNING"/> </returns>
	  int WarinigCount {get;}

	  /// <returns> the individual results of the validation grouped by element. </returns>
	  IDictionary<ModelElementInstance, IList<ValidationResult>> Results {get;}

	  /// <summary>
	  /// Utility method to print out a summary of the validation results.
	  /// </summary>
	  /// <param name="writer"> a <seealso cref="StringWriter"/> to which the result should be printed </param>
	  /// <param name="printer"> formatter for printing elements and validation results </param>
	  void write(StringWriter writer, ValidationResultFormatter printer);

	}

}