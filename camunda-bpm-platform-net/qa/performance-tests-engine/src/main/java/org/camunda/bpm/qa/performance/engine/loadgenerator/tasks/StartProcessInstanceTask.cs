﻿using System.Threading;

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
namespace org.camunda.bpm.qa.performance.engine.loadgenerator.tasks
{
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using RuntimeService = org.camunda.bpm.engine.RuntimeService;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class StartProcessInstanceTask : ThreadStart
	{

	  protected internal ProcessEngine processEngine;
	  protected internal string processDefinitionKey;

	  public StartProcessInstanceTask(ProcessEngine processEngine, string processDefinitionKey)
	  {
		this.processEngine = processEngine;
		this.processDefinitionKey = processDefinitionKey;
	  }

	  public virtual void run()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.RuntimeService runtimeService = processEngine.getRuntimeService();
		RuntimeService runtimeService = processEngine.RuntimeService;

		runtimeService.startProcessInstanceByKey(processDefinitionKey);

	  }

	}

}