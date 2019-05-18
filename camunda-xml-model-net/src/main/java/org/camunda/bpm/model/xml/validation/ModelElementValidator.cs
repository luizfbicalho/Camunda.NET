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
namespace org.camunda.bpm.model.xml.validation
{
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;

	/// <summary>
	/// A validator for model element instances.
	/// </summary>
	/// <seealso cref= ModelInstance#validate(java.util.Collection) </seealso>
	/// @param <T> the type of the elements to validate.
	/// @since 7.6 </param>
	public interface ModelElementValidator<T> where T : org.camunda.bpm.model.xml.instance.ModelElementInstance
	{

	  /// <summary>
	  /// <para>The type of the element this validator is applied to. The validator is applied to all
	  /// instances implementing this type.</para>
	  /// 
	  /// <para>Example from BPMN: Assume the type returned is 'Task'. Then the validator is invoked for
	  /// all instances of task, including instances of 'ServiceTask', 'UserTask', ...</para>
	  /// </summary>
	  /// <returns> the type of the element this validator is applied to. </returns>
	  Type<T> ElementType {get;}

	  /// <summary>
	  /// Validate an element.
	  /// </summary>
	  /// <param name="element"> the element to validate </param>
	  /// <param name="validationResultCollector"> object used to collect validation results for this element. </param>
	  void validate(T element, ValidationResultCollector validationResultCollector);
	}

}