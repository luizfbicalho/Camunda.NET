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
namespace org.camunda.bpm.model.bpmn.instance
{
	using AbstractBaseElementBuilder = org.camunda.bpm.model.bpmn.builder.AbstractBaseElementBuilder;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;

	/// <summary>
	/// Interface implemented by all elements in a BPMN Model
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public interface BpmnModelElementInstance : ModelElementInstance
	{

	  /// <summary>
	  /// Returns a new fluent builder for the element if implemented.
	  /// </summary>
	  /// <returns> the builder object </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") org.camunda.bpm.model.bpmn.builder.AbstractBaseElementBuilder builder();
	  AbstractBaseElementBuilder builder();

	  /// <summary>
	  /// Tests if the element is a scope like process or sub-process.
	  /// </summary>
	  /// <returns> true if element is scope, otherwise false </returns>
	  bool Scope {get;}

	  /// <summary>
	  /// Gets the element which is the scope of this element. Like
	  /// the parent process or sub-process.
	  /// </summary>
	  /// <returns> the scope element or null if non is found </returns>
	  BpmnModelElementInstance Scope {get;}

	}

}