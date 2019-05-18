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
namespace org.camunda.bpm.model.bpmn.impl.instance.di
{
	using Edge = org.camunda.bpm.model.bpmn.instance.di.Edge;
	using LabeledEdge = org.camunda.bpm.model.bpmn.instance.di.LabeledEdge;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.DI_ELEMENT_LABELED_EDGE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.DI_NS;

	/// <summary>
	/// The DI LabeledEdge element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public abstract class LabeledEdgeImpl : EdgeImpl, LabeledEdge
	{
		public override abstract org.camunda.bpm.model.bpmn.instance.Extension Extension {set;}

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(LabeledEdge), DI_ELEMENT_LABELED_EDGE).namespaceUri(DI_NS).extendsType(typeof(Edge)).abstractType();

		typeBuilder.build();
	  }

	  public LabeledEdgeImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }
	}

}