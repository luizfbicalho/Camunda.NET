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
	using CamundaScript = org.camunda.bpm.model.bpmn.instance.camunda.CamundaScript;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using ModelTypeInstanceProvider = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_RESOURCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_SCRIPT_FORMAT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ELEMENT_SCRIPT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_NS;

	/// <summary>
	/// The BPMN script camunda extension element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class CamundaScriptImpl : BpmnModelElementInstanceImpl, CamundaScript
	{

	  protected internal static Attribute<string> camundaScriptFormatAttribute;
	  protected internal static Attribute<string> camundaResourceAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(CamundaScript), CAMUNDA_ELEMENT_SCRIPT).namespaceUri(CAMUNDA_NS).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		camundaScriptFormatAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_SCRIPT_FORMAT).required().build();

		camundaResourceAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_RESOURCE).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelElementTypeBuilder.ModelTypeInstanceProvider<CamundaScript>
	  {
		  public CamundaScript newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new CamundaScriptImpl(instanceContext);
		  }
	  }

	  public CamundaScriptImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual string CamundaScriptFormat
	  {
		  get
		  {
			return camundaScriptFormatAttribute.getValue(this);
		  }
		  set
		  {
			camundaScriptFormatAttribute.setValue(this, value);
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