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
namespace org.camunda.bpm.model.bpmn.impl.instance
{
	using Artifact = org.camunda.bpm.model.bpmn.instance.Artifact;
	using BaseElement = org.camunda.bpm.model.bpmn.instance.BaseElement;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;

	/// <summary>
	/// The BPMN artifact element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public abstract class ArtifactImpl : BaseElementImpl, Artifact
	{

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(Artifact),BpmnModelConstants.BPMN_ELEMENT_ARTIFACT).namespaceUri(BpmnModelConstants.BPMN20_NS).extendsType(typeof(BaseElement)).abstractType();

		typeBuilder.build();
	  }

	  public ArtifactImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }
	}

}