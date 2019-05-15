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
namespace org.camunda.bpm.engine.test.jobexecutor
{

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ExecutionManager = org.camunda.bpm.engine.impl.persistence.entity.ExecutionManager;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using ThreadControl = org.camunda.bpm.engine.test.concurrency.ConcurrencyTestCase.ThreadControl;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ReuseEntityCacheTest
	{
		private bool InstanceFieldsInitialized = false;

		public ReuseEntityCacheTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			ruleChain = RuleChain.outerRule(bootstrapRule).around(engineRule);
		}


	  public const string ENTITY_ID1 = "Execution1";
	  public const string ENTITY_ID2 = "Execution2";

	  protected internal ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {
			return configuration.setJobExecutor(new ControllableJobExecutor());
		  }
	  }
	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(bootstrapRule).around(engineRule);
	  public RuleChain ruleChain;

	  protected internal bool defaultSetting;

	  protected internal ControllableJobExecutor jobExecutor;

	  protected internal static ThreadControl executionThreadControl;
	  protected internal ThreadControl acquisitionThreadControl;

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  protected internal static readonly BpmnModelInstance PROCESS = Bpmn.createExecutableProcess("process").startEvent().serviceTask().camundaClass(typeof(CreateEntitiesDelegate).FullName).camundaAsyncBefore().camundaExclusive(true).serviceTask().camundaClass(typeof(UpdateEntitiesDelegate).FullName).camundaAsyncBefore().camundaExclusive(true).serviceTask().camundaClass(typeof(RemoveEntitiesDelegate).FullName).camundaAsyncBefore().camundaExclusive(true).endEvent().done();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		defaultSetting = EngineConfig.DbEntityCacheReuseEnabled;
		EngineConfig.DbEntityCacheReuseEnabled = true;
		jobExecutor = (ControllableJobExecutor) EngineConfig.JobExecutor;
		executionThreadControl = jobExecutor.ExecutionThreadControl;
		acquisitionThreadControl = jobExecutor.AcquisitionThreadControl;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void resetEngineConfiguration()
	  public virtual void resetEngineConfiguration()
	  {
		EngineConfig.DbEntityCacheReuseEnabled = defaultSetting;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void shutdownJobExecutor()
	  public virtual void shutdownJobExecutor()
	  {
		jobExecutor.shutdown();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFlushOrderWithEntityCacheReuse()
	  public virtual void testFlushOrderWithEntityCacheReuse()
	  {
		// given
		Deployment deployment = engineRule.RepositoryService.createDeployment().addModelInstance("foo.bpmn", PROCESS).deploy();
		engineRule.manageDeployment(deployment);

		engineRule.RuntimeService.startProcessInstanceByKey("process");

		// when
		jobExecutor.start();

		// the job is acquired
		acquisitionThreadControl.waitForSync();

		// and job acquisition finishes successfully
		acquisitionThreadControl.makeContinueAndWaitForSync();
		acquisitionThreadControl.makeContinue();

		// and the first delegate is completed
		executionThreadControl.waitForSync();
		executionThreadControl.makeContinueAndWaitForSync();

		// and the second delegate is completed
		executionThreadControl.makeContinueAndWaitForSync();

		// and the third delegate is completed
		executionThreadControl.makeContinue();

		acquisitionThreadControl.waitForSync();

		// then the job has been successfully executed
		Assert.assertEquals(0, engineRule.ManagementService.createJobQuery().count());
	  }

	  protected internal virtual ProcessEngineConfigurationImpl EngineConfig
	  {
		  get
		  {
			return (ProcessEngineConfigurationImpl) engineRule.ProcessEngine.ProcessEngineConfiguration;
		  }
	  }

	  public class CreateEntitiesDelegate : JavaDelegate
	  {

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{
		  ExecutionEntity execution1 = new ExecutionEntity();
		  execution1.Id = ENTITY_ID1;
		  execution1.Executions = new List<ExecutionEntity>();

		  ExecutionEntity execution2 = new ExecutionEntity();
		  execution2.Id = ENTITY_ID2;
		  execution2.Executions = new List<ExecutionEntity>();
		  execution2.Parent = execution1;

		  ExecutionManager executionManager = Context.CommandContext.ExecutionManager;
		  executionManager.insert(execution1);
		  executionManager.insert(execution2);

		  executionThreadControl.sync();

		}

	  }

	  public class UpdateEntitiesDelegate : JavaDelegate
	  {

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{
		  ExecutionManager executionManager = Context.CommandContext.ExecutionManager;
		  ExecutionEntity execution1 = executionManager.findExecutionById(ENTITY_ID1);
		  ExecutionEntity execution2 = executionManager.findExecutionById(ENTITY_ID2);

		  // revert the references
		  execution2.Parent = null;
		  execution1.Parent = execution2;

		  executionThreadControl.sync();

		}

	  }

	  public class RemoveEntitiesDelegate : JavaDelegate
	  {

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{
		  ExecutionManager executionManager = Context.CommandContext.ExecutionManager;
		  ExecutionEntity execution1 = executionManager.findExecutionById(ENTITY_ID1);
		  ExecutionEntity execution2 = executionManager.findExecutionById(ENTITY_ID2);

		  executionManager.delete(execution1);
		  executionManager.delete(execution2);

		  executionThreadControl.sync();

		}

	  }
	}

}