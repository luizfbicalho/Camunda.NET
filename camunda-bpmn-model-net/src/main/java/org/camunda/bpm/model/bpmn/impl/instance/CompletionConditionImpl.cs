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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN20_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ELEMENT_COMPLETION_CONDITION;

	using CompletionCondition = org.camunda.bpm.model.bpmn.instance.CompletionCondition;
	using Expression = org.camunda.bpm.model.bpmn.instance.Expression;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;

	/// <summary>
	/// The BPMN 2.0 completionCondition element from the
	/// tMultiInstanceLoopCharacteristics type
	/// 
	/// @author Filip Hrisafov
	/// 
	/// </summary>
	public class CompletionConditionImpl : ExpressionImpl, CompletionCondition
	{

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(CompletionCondition), BPMN_ELEMENT_COMPLETION_CONDITION).namespaceUri(BPMN20_NS).extendsType(typeof(Expression)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelElementTypeBuilder.ModelTypeInstanceProvider<CompletionCondition>
	  {
		  public CompletionCondition newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new CompletionConditionImpl(instanceContext);
		  }
	  }

	  public CompletionConditionImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	}

}