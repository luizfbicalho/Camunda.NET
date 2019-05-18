﻿/*
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
	using CamundaValue = org.camunda.bpm.model.bpmn.instance.camunda.CamundaValue;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN value camunda extension element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class CamundaValueImpl : BpmnModelElementInstanceImpl, CamundaValue
	{

	  protected internal static Attribute<string> camundaIdAttribute;
	  protected internal static Attribute<string> camundaNameAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(CamundaValue), CAMUNDA_ELEMENT_VALUE).namespaceUri(CAMUNDA_NS).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		camundaIdAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_ID).@namespace(CAMUNDA_NS).build();

		camundaNameAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_NAME).@namespace(CAMUNDA_NS).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<CamundaValue>
	  {
		  public CamundaValue newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new CamundaValueImpl(instanceContext);
		  }
	  }

	  public CamundaValueImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual string CamundaId
	  {
		  get
		  {
			return camundaIdAttribute.getValue(this);
		  }
		  set
		  {
			camundaIdAttribute.setValue(this, value);
		  }
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

	}

}