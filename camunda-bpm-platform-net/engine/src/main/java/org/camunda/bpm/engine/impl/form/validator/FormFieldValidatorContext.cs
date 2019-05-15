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
namespace org.camunda.bpm.engine.impl.form.validator
{

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using FormFieldHandler = org.camunda.bpm.engine.impl.form.handler.FormFieldHandler;

	/// <summary>
	/// <para>Object passed in to a <seealso cref="FormFieldValidator"/> providing access to validation properties</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface FormFieldValidatorContext
	{

	  FormFieldHandler FormFieldHandler {get;}

	  /// <returns> the execution
	  /// Deprecated, use <seealso cref="#getVariableScope()"/>  </returns>
	  [Obsolete]
	  DelegateExecution Execution {get;}

	  /// <returns> the variable scope in which the value is submitted </returns>
	  VariableScope VariableScope {get;}

	  /// <returns> the configuration of this validator </returns>
	  string Configuration {get;}

	  /// <returns> all values submitted in the form </returns>
	  IDictionary<string, object> SubmittedValues {get;}

	}

}