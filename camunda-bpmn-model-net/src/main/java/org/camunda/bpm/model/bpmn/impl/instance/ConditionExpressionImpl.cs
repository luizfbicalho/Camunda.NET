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
	using ConditionExpression = org.camunda.bpm.model.bpmn.instance.ConditionExpression;
	using FormalExpression = org.camunda.bpm.model.bpmn.instance.FormalExpression;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN20_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ELEMENT_CONDITION_EXPRESSION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_RESOURCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.XSI_ATTRIBUTE_TYPE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.XSI_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN conditionExpression element of the BPMN tSequenceFlow type
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class ConditionExpressionImpl : FormalExpressionImpl, ConditionExpression
	{

	  protected internal static Attribute<string> typeAttribute;
	  protected internal static Attribute<string> camundaResourceAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(ConditionExpression), BPMN_ELEMENT_CONDITION_EXPRESSION).namespaceUri(BPMN20_NS).extendsType(typeof(FormalExpression)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		typeAttribute = typeBuilder.stringAttribute(XSI_ATTRIBUTE_TYPE).@namespace(XSI_NS).defaultValue("tFormalExpression").build();

		camundaResourceAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_RESOURCE).@namespace(CAMUNDA_NS).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<ConditionExpression>
	  {
		  public ConditionExpression newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new ConditionExpressionImpl(instanceContext);
		  }
	  }

	  public ConditionExpressionImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual string Type
	  {
		  get
		  {
			return typeAttribute.getValue(this);
		  }
		  set
		  {
			typeAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaResource
	  {
		  get
		  {
			return camundaResourceAttribute.getValue(this);
		  }
		  set
		  {
			camundaResourceAttribute.setValue(this, value);
		  }
	  }


	}

}