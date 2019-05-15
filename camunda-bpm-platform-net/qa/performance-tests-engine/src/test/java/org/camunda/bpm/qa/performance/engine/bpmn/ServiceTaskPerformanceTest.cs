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
namespace org.camunda.bpm.qa.performance.engine.bpmn
{

	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using NoopDelegate = org.camunda.bpm.qa.performance.engine.bpmn.@delegate.NoopDelegate;
	using ProcessEnginePerformanceTestCase = org.camunda.bpm.qa.performance.engine.junit.ProcessEnginePerformanceTestCase;
	using StartProcessInstanceStep = org.camunda.bpm.qa.performance.engine.steps.StartProcessInstanceStep;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ServiceTaskPerformanceTest : ProcessEnginePerformanceTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void threeServiceTasksAndAGateway()
	  public virtual void threeServiceTasksAndAGateway()
	  {

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["approved"] = true;

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance process = Bpmn.createExecutableProcess("process").startEvent().serviceTask().camundaClass(typeof(NoopDelegate).FullName).exclusiveGateway("decision").condition("approved", "${approved}").serviceTask().camundaClass(typeof(NoopDelegate).FullName).moveToLastGateway().condition("not approved", "${not approved}").serviceTask().camundaClass(typeof(NoopDelegate).FullName).endEvent().done();

		Deployment deployment = repositoryService.createDeployment().addModelInstance("process.bpmn", process).deploy();

		performanceTest().step(new StartProcessInstanceStep(engine, "process", variables)).run();

	  }

	}

}