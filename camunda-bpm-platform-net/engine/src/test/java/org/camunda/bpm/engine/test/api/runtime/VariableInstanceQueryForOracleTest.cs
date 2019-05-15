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
namespace org.camunda.bpm.engine.test.api.runtime
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.*;

	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Assume = org.junit.Assume;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	public class VariableInstanceQueryForOracleTest
	{
		private bool InstanceFieldsInitialized = false;

		public VariableInstanceQueryForOracleTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryWhen0InstancesActive()
	  public virtual void testQueryWhen0InstancesActive()
	  {
		// given
		Assume.assumeTrue(engineRule.ProcessEngineConfiguration.DatabaseType.Equals("oracle"));

		// then
		IList<VariableInstance> variables = engineRule.RuntimeService.createVariableInstanceQuery().list();
		assertEquals(0, variables.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryWhen1InstanceActive()
	  public virtual void testQueryWhen1InstanceActive()
	  {
		// given
		Assume.assumeTrue(engineRule.ProcessEngineConfiguration.DatabaseType.Equals("oracle"));
		RuntimeService runtimeService = engineRule.RuntimeService;
		testRule.deploy(ProcessModels.TWO_TASKS_PROCESS);

		// when
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process", Variables.createVariables().putValue("foo", "bar"));
		string activityInstanceId = runtimeService.getActivityInstance(processInstance.Id).Id;

		// then
		IList<VariableInstance> variables = engineRule.RuntimeService.createVariableInstanceQuery().activityInstanceIdIn(activityInstanceId).list();
		assertEquals(1, variables.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryWhen1000InstancesActive()
	  public virtual void testQueryWhen1000InstancesActive()
	  {
		// given
		Assume.assumeTrue(engineRule.ProcessEngineConfiguration.DatabaseType.Equals("oracle"));
		RuntimeService runtimeService = engineRule.RuntimeService;
		testRule.deploy(ProcessModels.TWO_TASKS_PROCESS);
		string[] ids = new string[1000];

		// when
		for (int i = 0; i < 1000; i++)
		{
		  ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process", Variables.createVariables().putValue("foo", "bar"));
		  string activityInstanceId = runtimeService.getActivityInstance(processInstance.Id).Id;
		  ids[i] = activityInstanceId;
		}

		// then
		IList<VariableInstance> variables = engineRule.RuntimeService.createVariableInstanceQuery().activityInstanceIdIn(ids).list();
		assertEquals(1000, variables.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryWhen1001InstancesActive()
	  public virtual void testQueryWhen1001InstancesActive()
	  {
		// given
		Assume.assumeTrue(engineRule.ProcessEngineConfiguration.DatabaseType.Equals("oracle"));
		RuntimeService runtimeService = engineRule.RuntimeService;
		testRule.deploy(ProcessModels.TWO_TASKS_PROCESS);
		string[] ids = new string[1001];

		// when
		for (int i = 0; i < 1001; i++)
		{
		  ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process", Variables.createVariables().putValue("foo", "bar"));
		  string activityInstanceId = runtimeService.getActivityInstance(processInstance.Id).Id;
		  ids[i] = activityInstanceId;
		}

		// then
		IList<VariableInstance> variables = engineRule.RuntimeService.createVariableInstanceQuery().activityInstanceIdIn(ids).list();
		assertEquals(1001, variables.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryWhen2001InstancesActive()
	  public virtual void testQueryWhen2001InstancesActive()
	  {
		// given
		Assume.assumeTrue(engineRule.ProcessEngineConfiguration.DatabaseType.Equals("oracle"));
		RuntimeService runtimeService = engineRule.RuntimeService;
		testRule.deploy(ProcessModels.TWO_TASKS_PROCESS);
		string[] ids = new string[2001];

		// when
		for (int i = 0; i < 2001; i++)
		{
		  ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process", Variables.createVariables().putValue("foo", "bar"));
		  string activityInstanceId = runtimeService.getActivityInstance(processInstance.Id).Id;
		  ids[i] = activityInstanceId;
		}

		// then
		IList<VariableInstance> variables = engineRule.RuntimeService.createVariableInstanceQuery().activityInstanceIdIn(ids).list();
		assertEquals(2001, variables.Count);
	  }
	}

}