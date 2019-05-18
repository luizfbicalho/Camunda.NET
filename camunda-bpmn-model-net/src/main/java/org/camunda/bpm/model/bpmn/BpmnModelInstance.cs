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
namespace org.camunda.bpm.model.bpmn
{
	using Definitions = org.camunda.bpm.model.bpmn.instance.Definitions;
	using ModelInstance = org.camunda.bpm.model.xml.ModelInstance;

	/// <summary>
	/// <para>A BPMN 2.0 Model</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface BpmnModelInstance : ModelInstance
	{

	  /// <returns> the <seealso cref="Definitions"/>, root element of the Bpmn Model.
	  ///  </returns>
	  Definitions Definitions {get;set;}


	  /// <summary>
	  /// Copies the BPMN model instance but not the model. So only the wrapped DOM document is cloned.
	  /// Changes of the model are persistent between multiple model instances.
	  /// </summary>
	  /// <returns> the new BPMN model instance </returns>
	  BpmnModelInstance clone();

	}

}