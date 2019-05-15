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
namespace org.camunda.bpm.qa.performance.engine.steps
{
	using static org.camunda.bpm.qa.performance.engine.steps.PerfTestConstants;


	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using PerfTestRunContext = org.camunda.bpm.qa.performance.engine.framework.PerfTestRunContext;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class StartProcessInstanceStep : ProcessEngineAwareStep
	{

	  protected internal string processDefinitionKey;
	  protected internal IDictionary<string, object> processVariables;

	  public StartProcessInstanceStep(ProcessEngine processEngine, string processDefinitionKey) : this(processEngine, processDefinitionKey, null)
	  {
	  }

	  public StartProcessInstanceStep(ProcessEngine processEngine, string processDefinitionKey, IDictionary<string, object> processVariables) : base(processEngine)
	  {
		this.processDefinitionKey = processDefinitionKey;
		this.processVariables = processVariables;
	  }

	  public override void execute(PerfTestRunContext context)
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		if (processVariables != null)
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
		  variables.putAll(processVariables);
		}
		// unique run id as variable
		variables[RUN_ID] = context.getVariable(RUN_ID);

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(processDefinitionKey, variables);
		context.setVariable(PROCESS_INSTANCE_ID, processInstance.Id);
	  }

	}

}