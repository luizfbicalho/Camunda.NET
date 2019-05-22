using System;

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
namespace org.camunda.bpm.qa.upgrade.useroperationlog
{

	using IdentityService = org.camunda.bpm.engine.IdentityService;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Deployment = org.camunda.bpm.engine.test.Deployment;

	/// <summary>
	/// @author Yana.Vasileva
	/// 
	/// </summary>
	public class SuspendProcessDefinitionDeleteScenario
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static String deploy()
	  public static string deploy()
	  {
		return "org/camunda/bpm/qa/upgrade/useroperationlog/timerBoundaryEventProcess.bpmn20.xml";
	  }

	  [DescribesScenario("createUserOperationLogEntriesForDelete")]
	  public static ScenarioSetup createUserOperationLogEntries()
	  {
		return new ScenarioSetupAnonymousInnerClass();
	  }

	  private class ScenarioSetupAnonymousInnerClass : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {
			string processInstanceBusinessKey = "SuspendProcessDefinitionDeleteScenario";
			ProcessInstance processInstance1 = engine.RuntimeService.startProcessInstanceByKey("timerBoundaryProcess", processInstanceBusinessKey);
			ProcessInstance processInstance2 = engine.RuntimeService.startProcessInstanceByKey("timerBoundaryProcess", processInstanceBusinessKey);
			ProcessInstance processInstance3 = engine.RuntimeService.startProcessInstanceByKey("timerBoundaryProcess", processInstanceBusinessKey);

			IdentityService identityService = engine.IdentityService;
			identityService.setAuthentication("jane01", null);

			engine.ProcessEngineConfiguration.AuthorizationEnabled = false;
			ClockUtil.CurrentTime = new DateTime(1549000000000l);
			engine.RuntimeService.suspendProcessInstanceById(processInstance1.Id);
			ClockUtil.CurrentTime = new DateTime(1549100000000l);
			engine.RuntimeService.suspendProcessInstanceById(processInstance2.Id);
			ClockUtil.CurrentTime = new DateTime(1549200000000l);
			engine.RuntimeService.suspendProcessInstanceById(processInstance3.Id);

			ClockUtil.reset();
			identityService.clearAuthentication();
		  }
	  }
	}

}