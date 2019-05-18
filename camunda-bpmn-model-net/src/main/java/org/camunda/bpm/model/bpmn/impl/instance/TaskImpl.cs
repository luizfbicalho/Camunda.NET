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
namespace org.camunda.bpm.model.bpmn.impl.instance
{
	using AbstractTaskBuilder = org.camunda.bpm.model.bpmn.builder.AbstractTaskBuilder;
	using Activity = org.camunda.bpm.model.bpmn.instance.Activity;
	using Task = org.camunda.bpm.model.bpmn.instance.Task;
	using BpmnShape = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnShape;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelTypeException = org.camunda.bpm.model.xml.impl.util.ModelTypeException;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN task element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class TaskImpl : ActivityImpl, Task
	{

	  /// <summary>
	  /// camunda extensions </summary>

	  protected internal static Attribute<bool> camundaAsyncAttribute;


	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(Task), BPMN_ELEMENT_TASK).namespaceUri(BPMN20_NS).extendsType(typeof(Activity)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		/// <summary>
		/// camunda extensions </summary>

		camundaAsyncAttribute = typeBuilder.booleanAttribute(CAMUNDA_ATTRIBUTE_ASYNC).@namespace(CAMUNDA_NS).defaultValue(false).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<Task>
	  {
		  public Task newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new TaskImpl(instanceContext);
		  }
	  }

	  public TaskImpl(ModelTypeInstanceContext context) : base(context)
	  {
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public org.camunda.bpm.model.bpmn.builder.AbstractTaskBuilder builder()
	  public override AbstractTaskBuilder builder()
	  {
		throw new ModelTypeException("No builder implemented.");
	  }

	  /// <summary>
	  /// camunda extensions </summary>

	  /// @deprecated use isCamundaAsyncBefore() instead. 
	  [Obsolete("use isCamundaAsyncBefore() instead.")]
	  public virtual bool CamundaAsync
	  {
		  get
		  {
			return camundaAsyncAttribute.getValue(this);
		  }
		  set
		  {
			camundaAsyncAttribute.setValue(this, value);
		  }
	  }



	  public override BpmnShape DiagramElement
	  {
		  get
		  {
			return (BpmnShape) base.DiagramElement;
		  }
	  }

	}

}