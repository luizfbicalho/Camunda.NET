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
namespace org.camunda.bpm.model.bpmn.impl
{
	using DefinitionsImpl = org.camunda.bpm.model.bpmn.impl.instance.DefinitionsImpl;
	using Definitions = org.camunda.bpm.model.bpmn.instance.Definitions;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelImpl = org.camunda.bpm.model.xml.impl.ModelImpl;
	using ModelInstanceImpl = org.camunda.bpm.model.xml.impl.ModelInstanceImpl;
	using DomDocument = org.camunda.bpm.model.xml.instance.DomDocument;

	/// <summary>
	/// <para>The Bpmn Model</para>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class BpmnModelInstanceImpl : ModelInstanceImpl, BpmnModelInstance
	{

	  public BpmnModelInstanceImpl(ModelImpl model, ModelBuilder modelBuilder, DomDocument document) : base(model, modelBuilder, document)
	  {
	  }

	  public virtual Definitions Definitions
	  {
		  get
		  {
			return (DefinitionsImpl) DocumentElement;
		  }
		  set
		  {
			DocumentElement = value;
		  }
	  }


	  public virtual BpmnModelInstance clone()
	  {
		return new BpmnModelInstanceImpl(model, modelBuilder, document.clone());
	  }

	}

}