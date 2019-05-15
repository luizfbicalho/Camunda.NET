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
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using RepositoryService = org.camunda.bpm.engine.RepositoryService;
	using RuntimeService = org.camunda.bpm.engine.RuntimeService;
	using TaskService = org.camunda.bpm.engine.TaskService;
	using PerfTestStepBehavior = org.camunda.bpm.qa.performance.engine.framework.PerfTestStepBehavior;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public abstract class ProcessEngineAwareStep : PerfTestStepBehavior
	{
		public abstract void execute(org.camunda.bpm.qa.performance.engine.framework.PerfTestRunContext context);

	  protected internal ProcessEngine processEngine;
	  protected internal RuntimeService runtimeService;
	  protected internal TaskService taskService;
	  protected internal RepositoryService repositoryService;

	  public ProcessEngineAwareStep(ProcessEngine processEngine)
	  {
		this.processEngine = processEngine;
		runtimeService = processEngine.RuntimeService;
		taskService = processEngine.TaskService;
		repositoryService = processEngine.RepositoryService;
	  }

	}

}