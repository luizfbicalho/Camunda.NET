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
namespace org.camunda.bpm.engine.form
{


	/// <summary>
	/// Represents a single property on a form.
	/// 
	/// @author Tom Baeyens
	/// </summary>
	[Obsolete]
	public interface FormProperty
	{

	  /// <summary>
	  /// The key used to submit the property in <seealso cref="FormService#submitStartFormData(String, java.util.Map)"/>
	  /// or <seealso cref="FormService#submitTaskFormData(String, java.util.Map)"/> 
	  /// </summary>
	  string Id {get;}

	  /// <summary>
	  /// The display label </summary>
	  string Name {get;}

	  /// <summary>
	  /// Type of the property. </summary>
	  FormType Type {get;}

	  /// <summary>
	  /// Optional value that should be used to display in this property </summary>
	  string Value {get;}

	  /// <summary>
	  /// Is this property read to be displayed in the form and made accessible with the methods
	  /// <seealso cref="FormService#getStartFormData(String)"/> and <seealso cref="FormService#getTaskFormData(String)"/>. 
	  /// </summary>
	  bool Readable {get;}

	  /// <summary>
	  /// Is this property expected when a user submits the form? </summary>
	  bool Writable {get;}

	  /// <summary>
	  /// Is this property a required input field </summary>
	  bool Required {get;}
	}

}