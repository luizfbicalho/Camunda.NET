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
namespace org.camunda.bpm.engine.variable.value
{

	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;

	/// <summary>
	/// <para>A <seealso cref="TypedValue"/> is a value with additional type information (the <seealso cref="ValueType"/>).
	/// TypedValues are used for representing variable values.</para>
	/// 
	/// @author Daniel Meyer
	/// @since 7.2
	/// </summary>
	public interface TypedValue
	{

	  /// <summary>
	  /// The actual value. May be null in case the value is null.
	  /// </summary>
	  /// <returns> the value </returns>
	  object Value {get;}

	  /// <summary>
	  /// The type of the value. See ValueType for a list of built-in ValueTypes. </summary>
	  /// <returns> the type of the value. </returns>
	  ValueType Type {get;}

	  /// <summary>
	  /// Indicator for transience of the value </summary>
	  /// <returns> isTransient </returns>
	  bool Transient {get;}

	}

}