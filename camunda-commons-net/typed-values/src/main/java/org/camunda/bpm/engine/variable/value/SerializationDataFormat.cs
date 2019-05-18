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
	using SerializationDataFormats = org.camunda.bpm.engine.variable.Variables.SerializationDataFormats;

	/// <summary>
	/// <para>Represents a serialization data format.</para>
	/// 
	/// @author Daniel Meyer </summary>
	/// <seealso cref= SerializationDataFormats
	/// 
	/// @since 7.2 </seealso>
	public interface SerializationDataFormat
	{

	  /// <summary>
	  /// The name of the dataformat. Example: "application/json"
	  /// </summary>
	  /// <returns> the name of the dataformat. </returns>
	  string Name {get;}

	}

}