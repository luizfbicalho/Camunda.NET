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
	using CamundaFailedJobRetryTimeCycle = org.camunda.bpm.model.bpmn.instance.camunda.CamundaFailedJobRetryTimeCycle;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ELEMENT_FAILED_JOB_RETRY_TIME_CYCLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN failedJobRetryTimeCycle camunda extension element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class CamundaFailedJobRetryTimeCycleImpl : BpmnModelElementInstanceImpl, CamundaFailedJobRetryTimeCycle
	{

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(CamundaFailedJobRetryTimeCycle), CAMUNDA_ELEMENT_FAILED_JOB_RETRY_TIME_CYCLE).namespaceUri(CAMUNDA_NS).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<CamundaFailedJobRetryTimeCycle>
	  {
		  public CamundaFailedJobRetryTimeCycle newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new CamundaFailedJobRetryTimeCycleImpl(instanceContext);
		  }
	  }

	  public CamundaFailedJobRetryTimeCycleImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }
	}

}