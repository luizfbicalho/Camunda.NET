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
namespace org.camunda.bpm.model.bpmn.impl.instance.camunda
{
	using CamundaIn = org.camunda.bpm.model.bpmn.instance.camunda.CamundaIn;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN in camunda extension element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class CamundaInImpl : BpmnModelElementInstanceImpl, CamundaIn
	{

	  protected internal static Attribute<string> camundaSourceAttribute;
	  protected internal static Attribute<string> camundaSourceExpressionAttribute;
	  protected internal static Attribute<string> camundaVariablesAttribute;
	  protected internal static Attribute<string> camundaTargetAttribute;
	  protected internal static Attribute<string> camundaBusinessKeyAttribute;
	  protected internal static Attribute<bool> camundaLocalAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(CamundaIn), CAMUNDA_ELEMENT_IN).namespaceUri(CAMUNDA_NS).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		camundaSourceAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_SOURCE).@namespace(CAMUNDA_NS).build();

		camundaSourceExpressionAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_SOURCE_EXPRESSION).@namespace(CAMUNDA_NS).build();

		camundaVariablesAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_VARIABLES).@namespace(CAMUNDA_NS).build();

		camundaTargetAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_TARGET).@namespace(CAMUNDA_NS).build();

		camundaBusinessKeyAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_BUSINESS_KEY).@namespace(CAMUNDA_NS).build();

		camundaLocalAttribute = typeBuilder.booleanAttribute(CAMUNDA_ATTRIBUTE_LOCAL).@namespace(CAMUNDA_NS).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<CamundaIn>
	  {
		  public CamundaIn newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new CamundaInImpl(instanceContext);
		  }
	  }

	  public CamundaInImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual string CamundaSource
	  {
		  get
		  {
			return camundaSourceAttribute.getValue(this);
		  }
		  set
		  {
			camundaSourceAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaSourceExpression
	  {
		  get
		  {
			return camundaSourceExpressionAttribute.getValue(this);
		  }
		  set
		  {
			camundaSourceExpressionAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaVariables
	  {
		  get
		  {
			return camundaVariablesAttribute.getValue(this);
		  }
		  set
		  {
			camundaVariablesAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaTarget
	  {
		  get
		  {
			return camundaTargetAttribute.getValue(this);
		  }
		  set
		  {
			camundaTargetAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaBusinessKey
	  {
		  get
		  {
			return camundaBusinessKeyAttribute.getValue(this);
		  }
		  set
		  {
			camundaBusinessKeyAttribute.setValue(this, value);
		  }
	  }


	  public virtual bool CamundaLocal
	  {
		  get
		  {
			return camundaLocalAttribute.getValue(this);
		  }
		  set
		  {
			camundaLocalAttribute.setValue(this, value);
		  }
	  }


	}

}