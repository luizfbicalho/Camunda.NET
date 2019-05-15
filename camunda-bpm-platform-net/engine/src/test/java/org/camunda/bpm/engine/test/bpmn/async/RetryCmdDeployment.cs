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
namespace org.camunda.bpm.engine.test.bpmn.async
{
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using IntermediateThrowEventImpl = org.camunda.bpm.model.bpmn.impl.instance.IntermediateThrowEventImpl;
	using CamundaFailedJobRetryTimeCycle = org.camunda.bpm.model.bpmn.instance.camunda.CamundaFailedJobRetryTimeCycle;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;

	/// <summary>
	/// @author Askar Akhmerov
	/// </summary>
	public class RetryCmdDeployment
	{

	  public const string FAILING_EVENT = "failingEvent";
	  public const string PROCESS_ID = "failedIntermediateThrowingEventAsync";
	  private const string SCHEDULE = "R5/PT5M";
	  private const string PROCESS_ID_2 = "failingSignalProcess";
	  public const string MESSAGE = "start";
	  private BpmnModelInstance[] bpmnModelInstances;

	  public static RetryCmdDeployment deployment()
	  {
		return new RetryCmdDeployment();
	  }

	  public static BpmnModelInstance prepareSignalEventProcess()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(PROCESS_ID).startEvent().intermediateThrowEvent(FAILING_EVENT).camundaAsyncBefore(true).camundaFailedJobRetryTimeCycle(SCHEDULE).signal(MESSAGE).serviceTask().camundaClass(typeof(FailingDelegate).FullName).endEvent().done();
		return modelInstance;
	  }

	  public static BpmnModelInstance prepareMessageEventProcess()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		return Bpmn.createExecutableProcess(PROCESS_ID).startEvent().intermediateThrowEvent(FAILING_EVENT).camundaAsyncBefore(true).camundaFailedJobRetryTimeCycle(SCHEDULE).message(MESSAGE).serviceTask().camundaClass(typeof(FailingDelegate).FullName).done();
	  }

	  public static BpmnModelInstance prepareEscalationEventProcess()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		return Bpmn.createExecutableProcess(PROCESS_ID).startEvent().intermediateThrowEvent(FAILING_EVENT).camundaAsyncBefore(true).camundaFailedJobRetryTimeCycle(SCHEDULE).escalation(MESSAGE).serviceTask().camundaClass(typeof(FailingDelegate).FullName).endEvent().done();
	  }


	  public static BpmnModelInstance prepareCompensationEventProcess()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		return Bpmn.createExecutableProcess(PROCESS_ID).startEvent().subProcess("subProcess").embeddedSubProcess().startEvent().endEvent().subProcessDone().intermediateThrowEvent(FAILING_EVENT).camundaAsyncBefore(true).camundaFailedJobRetryTimeCycle(SCHEDULE).compensateEventDefinition().compensateEventDefinitionDone().serviceTask().camundaClass(typeof(FailingDelegate).FullName).endEvent().done();
	  }


	  public virtual RetryCmdDeployment withEventProcess(params BpmnModelInstance[] bpmnModelInstances)
	  {
		this.bpmnModelInstances = bpmnModelInstances;
		return this;
	  }

	  public static ICollection<RetryCmdDeployment[]> asParameters(params RetryCmdDeployment[] deployments)
	  {
		IList<RetryCmdDeployment[]> deploymentList = new List<RetryCmdDeployment[]>();
		foreach (RetryCmdDeployment deployment in deployments)
		{
		  deploymentList.Add(new RetryCmdDeployment[]{deployment});
		}

		return deploymentList;
	  }

	  public virtual BpmnModelInstance[] BpmnModelInstances
	  {
		  get
		  {
			return bpmnModelInstances;
		  }
		  set
		  {
			this.bpmnModelInstances = value;
		  }
	  }

	}

}