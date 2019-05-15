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
namespace org.camunda.bpm.engine.impl.form.validator
{
	/// <summary>
	/// <para>Interface for implenting form field validators.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface FormFieldValidator
	{

	  /// <summary>
	  /// return true if the submitted value is valid for the given form field.
	  /// </summary>
	  /// <param name="submittedValue">
	  ///          the value submitted to the form field </param>
	  /// <param name="validatorContext">
	  ///          object providing access to additional information useful wile
	  ///          validating the form </param>
	  /// <returns> true if the value is valid, false otherwise. </returns>
	  bool validate(object submittedValue, FormFieldValidatorContext validatorContext);

	}

}