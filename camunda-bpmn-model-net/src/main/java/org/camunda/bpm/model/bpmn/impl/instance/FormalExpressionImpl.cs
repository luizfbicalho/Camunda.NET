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
	using Expression = org.camunda.bpm.model.bpmn.instance.Expression;
	using FormalExpression = org.camunda.bpm.model.bpmn.instance.FormalExpression;
	using ItemDefinition = org.camunda.bpm.model.bpmn.instance.ItemDefinition;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using AttributeReference = org.camunda.bpm.model.xml.type.reference.AttributeReference;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN formalExpression element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class FormalExpressionImpl : ExpressionImpl, FormalExpression
	{

	  protected internal static Attribute<string> languageAttribute;
	  protected internal static AttributeReference<ItemDefinition> evaluatesToTypeRefAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(FormalExpression), BPMN_ELEMENT_FORMAL_EXPRESSION).namespaceUri(BPMN20_NS).extendsType(typeof(Expression)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		languageAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_LANGUAGE).build();

		evaluatesToTypeRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_EVALUATES_TO_TYPE_REF).qNameAttributeReference(typeof(ItemDefinition)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<FormalExpression>
	  {
		  public FormalExpression newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new FormalExpressionImpl(instanceContext);
		  }
	  }

	  public FormalExpressionImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual string Language
	  {
		  get
		  {
			return languageAttribute.getValue(this);
		  }
		  set
		  {
			languageAttribute.setValue(this, value);
		  }
	  }


	  public virtual ItemDefinition EvaluatesToType
	  {
		  get
		  {
			return evaluatesToTypeRefAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			evaluatesToTypeRefAttribute.setReferenceTargetElement(this, value);
		  }
	  }

	}

}