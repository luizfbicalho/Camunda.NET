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
namespace org.camunda.bpm.model.xml.validation
{

	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;

	/// <summary>
	/// SPI which can be implemented to print out a summary of a validation result.
	/// See <seealso cref="ValidationResults.write(StringWriter, ValidationResultFormatter)"/>
	/// 
	/// @author Daniel Meyer
	/// @since 7.6
	/// </summary>
	public interface ValidationResultFormatter
	{

	  /// <summary>
	  /// formats an element in the summary
	  /// </summary>
	  /// <param name="writer"> the writer </param>
	  /// <param name="element"> the element to write </param>
	  void formatElement(StringWriter writer, ModelElementInstance element);

	  /// <summary>
	  /// formats a validation result
	  /// </summary>
	  /// <param name="writer"> the writer </param>
	  /// <param name="result"> the result to format </param>
	  void formatResult(StringWriter writer, ValidationResult result);

	}

}