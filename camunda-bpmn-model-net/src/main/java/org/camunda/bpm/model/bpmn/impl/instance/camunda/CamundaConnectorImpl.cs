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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ELEMENT_CONNECTOR;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_NS;

	using CamundaConnector = org.camunda.bpm.model.bpmn.instance.camunda.CamundaConnector;
	using CamundaConnectorId = org.camunda.bpm.model.bpmn.instance.camunda.CamundaConnectorId;
	using CamundaInputOutput = org.camunda.bpm.model.bpmn.instance.camunda.CamundaInputOutput;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using ModelTypeInstanceProvider = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;
	using ChildElement = org.camunda.bpm.model.xml.type.child.ChildElement;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;

	/// <summary>
	/// The BPMN connector camunda extension element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class CamundaConnectorImpl : BpmnModelElementInstanceImpl, CamundaConnector
	{

	  protected internal static ChildElement<CamundaConnectorId> camundaConnectorIdChild;
	  protected internal static ChildElement<CamundaInputOutput> camundaInputOutputChild;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(CamundaConnector), CAMUNDA_ELEMENT_CONNECTOR).namespaceUri(CAMUNDA_NS).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		camundaConnectorIdChild = sequenceBuilder.element(typeof(CamundaConnectorId)).required().build();

		camundaInputOutputChild = sequenceBuilder.element(typeof(CamundaInputOutput)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelElementTypeBuilder.ModelTypeInstanceProvider<CamundaConnector>
	  {
		  public CamundaConnector newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new CamundaConnectorImpl(instanceContext);
		  }
	  }

	  public CamundaConnectorImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual CamundaConnectorId CamundaConnectorId
	  {
		  get
		  {
			return camundaConnectorIdChild.getChild(this);
		  }
		  set
		  {
			camundaConnectorIdChild.setChild(this, value);
		  }
	  }


	  public virtual CamundaInputOutput CamundaInputOutput
	  {
		  get
		  {
			return camundaInputOutputChild.getChild(this);
		  }
		  set
		  {
			camundaInputOutputChild.setChild(this, value);
		  }
	  }


	}

}