﻿/*
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
namespace org.camunda.bpm.engine.test.api.task
{
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Task = org.camunda.bpm.engine.task.Task;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	public class TaskServiceWithJdbcSimpleProcessingTest
	{
		private bool InstanceFieldsInitialized = false;

		public TaskServiceWithJdbcSimpleProcessingTest()
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
			ruleChain = RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
		}


	  protected internal ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {
			configuration.JdbcBatchProcessing = false;
			return configuration;
		  }
	  }

	  public ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);
	  public ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();
	  private RuntimeService runtimeService;
	  private TaskService taskService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		runtimeService = engineRule.RuntimeService;
		taskService = engineRule.TaskService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" }) public void testUserTaskOptimisticLocking()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testUserTaskOptimisticLocking()
	  {
		runtimeService.startProcessInstanceByKey("oneTaskProcess");

		thrown.expect(typeof(OptimisticLockingException));

		Task task1 = taskService.createTaskQuery().singleResult();
		Task task2 = taskService.createTaskQuery().singleResult();

		task1.Description = "test description one";
		taskService.saveTask(task1);

		task2.Description = "test description two";
		taskService.saveTask(task2);
	  }

	}

}