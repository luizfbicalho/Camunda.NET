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
	using CamundaConstraint = org.camunda.bpm.model.bpmn.instance.camunda.CamundaConstraint;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN constraint camunda extension element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class CamundaConstraintImpl : BpmnModelElementInstanceImpl, CamundaConstraint
	{

	  protected internal static Attribute<string> camundaNameAttribute;
	  protected internal static Attribute<string> camundaConfigAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(CamundaConstraint), CAMUNDA_ELEMENT_CONSTRAINT).namespaceUri(CAMUNDA_NS).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		camundaNameAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_NAME).@namespace(CAMUNDA_NS).build();

		camundaConfigAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_CONFIG).@namespace(CAMUNDA_NS).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<CamundaConstraint>
	  {
		  public CamundaConstraint newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new CamundaConstraintImpl(instanceContext);
		  }
	  }

	  public CamundaConstraintImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual string CamundaName
	  {
		  get
		  {
			return camundaNameAttribute.getValue(this);
		  }
		  set
		  {
			camundaNameAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaConfig
	  {
		  get
		  {
			return camundaConfigAttribute.getValue(this);
		  }
		  set
		  {
			camundaConfigAttribute.setValue(this, value);
		  }
	  }

	}

}