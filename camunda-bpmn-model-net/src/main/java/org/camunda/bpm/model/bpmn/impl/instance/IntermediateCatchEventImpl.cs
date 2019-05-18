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
	using IntermediateCatchEventBuilder = org.camunda.bpm.model.bpmn.builder.IntermediateCatchEventBuilder;
	using CatchEvent = org.camunda.bpm.model.bpmn.instance.CatchEvent;
	using IntermediateCatchEvent = org.camunda.bpm.model.bpmn.instance.IntermediateCatchEvent;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ELEMENT_INTERMEDIATE_CATCH_EVENT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN intermediateCatchEvent element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class IntermediateCatchEventImpl : CatchEventImpl, IntermediateCatchEvent
	{

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(IntermediateCatchEvent), BPMN_ELEMENT_INTERMEDIATE_CATCH_EVENT).namespaceUri(BpmnModelConstants.BPMN20_NS).extendsType(typeof(CatchEvent)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<IntermediateCatchEvent>
	  {
		  public IntermediateCatchEvent newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new IntermediateCatchEventImpl(instanceContext);
		  }
	  }

	  public IntermediateCatchEventImpl(ModelTypeInstanceContext context) : base(context)
	  {
	  }

	  public override IntermediateCatchEventBuilder builder()
	  {
		return new IntermediateCatchEventBuilder((BpmnModelInstance) modelInstance, this);
	  }
	}

}