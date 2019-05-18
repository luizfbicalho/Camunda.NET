using System.Collections.Generic;

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
	using CamundaInputOutput = org.camunda.bpm.model.bpmn.instance.camunda.CamundaInputOutput;
	using CamundaInputParameter = org.camunda.bpm.model.bpmn.instance.camunda.CamundaInputParameter;
	using CamundaOutputParameter = org.camunda.bpm.model.bpmn.instance.camunda.CamundaOutputParameter;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using ModelTypeInstanceProvider = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ELEMENT_INPUT_OUTPUT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_NS;

	/// <summary>
	/// The BPMN inputOutput camunda extension element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class CamundaInputOutputImpl : BpmnModelElementInstanceImpl, CamundaInputOutput
	{

	  protected internal static ChildElementCollection<CamundaInputParameter> camundaInputParameterCollection;
	  protected internal static ChildElementCollection<CamundaOutputParameter> camundaOutputParameterCollection;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(CamundaInputOutput), CAMUNDA_ELEMENT_INPUT_OUTPUT).namespaceUri(CAMUNDA_NS).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		camundaInputParameterCollection = sequenceBuilder.elementCollection(typeof(CamundaInputParameter)).build();

		camundaOutputParameterCollection = sequenceBuilder.elementCollection(typeof(CamundaOutputParameter)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelElementTypeBuilder.ModelTypeInstanceProvider<CamundaInputOutput>
	  {
		  public CamundaInputOutput newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new CamundaInputOutputImpl(instanceContext);
		  }
	  }

	  public CamundaInputOutputImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual ICollection<CamundaInputParameter> CamundaInputParameters
	  {
		  get
		  {
			return camundaInputParameterCollection.get(this);
		  }
	  }

	  public virtual ICollection<CamundaOutputParameter> CamundaOutputParameters
	  {
		  get
		  {
			return camundaOutputParameterCollection.get(this);
		  }
	  }
	}

}