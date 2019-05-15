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
namespace org.camunda.bpm.engine.test.concurrency.partitioning
{
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public abstract class AbstractPartitioningTest : ConcurrencyTestCase
	{

	  protected internal CommandExecutor commandExecutor;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void setUp() throws Exception
	  protected internal override void setUp()
	  {
		base.setUp();

		this.commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;

		processEngine.ProcessEngineConfiguration.SkipHistoryOptimisticLockingExceptions = true;
	  }

	  protected internal readonly BpmnModelInstance PROCESS_WITH_USERTASK = Bpmn.createExecutableProcess("process").startEvent().userTask().endEvent().done();

	  protected internal virtual ProcessInstance deployAndStartProcess(BpmnModelInstance bpmnModelInstance)
	  {
		return deployAndStartProcess(bpmnModelInstance, null);
	  }

	  protected internal virtual ProcessInstance deployAndStartProcess(BpmnModelInstance bpmnModelInstance, IDictionary<string, object> variablesMap)
	  {
		deploymentId = repositoryService.createDeployment().addModelInstance("process.bpmn", bpmnModelInstance).deploy().Id;

		string processDefinitionKey = bpmnModelInstance.Definitions.RootElements.GetEnumerator().next().Id;
		return runtimeService.startProcessInstanceByKey(processDefinitionKey, variablesMap);
	  }

	}

}