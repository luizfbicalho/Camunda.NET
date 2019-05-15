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

	using Page = org.camunda.bpm.engine.impl.Page;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Assume = org.junit.Assume;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	public class DeploymentAwareJobExecutorForOracleTest
	{
		private bool InstanceFieldsInitialized = false;

		public DeploymentAwareJobExecutorForOracleTest()
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
			ruleChain = RuleChain.outerRule(deploymentAwareBootstrapRule).around(engineRule).around(testRule);
		}


	  protected internal ProcessEngineBootstrapRule deploymentAwareBootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {
			configuration.JobExecutorDeploymentAware = true;
			return configuration;
		  }
	  }
	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule(deploymentAwareBootstrapRule);
	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(deploymentAwareBootstrapRule).around(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFindAcquirableJobsWhen0InstancesDeployed()
	  public virtual void testFindAcquirableJobsWhen0InstancesDeployed()
	  {
		// given
		Assume.assumeTrue(engineRule.ProcessEngineConfiguration.DatabaseType.Equals("oracle"));

		// then
		findAcquirableJobs();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFindAcquirableJobsWhen1InstanceDeployed()
	  public virtual void testFindAcquirableJobsWhen1InstanceDeployed()
	  {
		// given
		Assume.assumeTrue(engineRule.ProcessEngineConfiguration.DatabaseType.Equals("oracle"));
		// when
		testRule.deploy(ProcessModels.ONE_TASK_PROCESS);
		// then
		findAcquirableJobs();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFindAcquirableJobsWhen1000InstancesDeployed()
	  public virtual void testFindAcquirableJobsWhen1000InstancesDeployed()
	  {
		// given
		Assume.assumeTrue(engineRule.ProcessEngineConfiguration.DatabaseType.Equals("oracle"));
		// when
		for (int i = 0; i < 1000; i++)
		{
		  testRule.deploy(ProcessModels.ONE_TASK_PROCESS);
		}
		// then
		findAcquirableJobs();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFindAcquirableJobsWhen1001InstancesDeployed()
	  public virtual void testFindAcquirableJobsWhen1001InstancesDeployed()
	  {
		// given
		Assume.assumeTrue(engineRule.ProcessEngineConfiguration.DatabaseType.Equals("oracle"));
		// when
		for (int i = 0; i < 1001; i++)
		{
		  testRule.deploy(ProcessModels.ONE_TASK_PROCESS);
		}
		// then
		findAcquirableJobs();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFindAcquirableJobsWhen2000InstancesDeployed()
	  public virtual void testFindAcquirableJobsWhen2000InstancesDeployed()
	  {
		// given
		Assume.assumeTrue(engineRule.ProcessEngineConfiguration.DatabaseType.Equals("oracle"));
		// when
		for (int i = 0; i < 2000; i++)
		{
		  testRule.deploy(ProcessModels.ONE_TASK_PROCESS);
		}
		// then
		findAcquirableJobs();
	  }

	  protected internal virtual IList<JobEntity> findAcquirableJobs()
	  {
		return engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));
	  }

	  private class CommandAnonymousInnerClass : Command<IList<JobEntity>>
	  {
		  private readonly DeploymentAwareJobExecutorForOracleTest outerInstance;

		  public CommandAnonymousInnerClass(DeploymentAwareJobExecutorForOracleTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		  public IList<JobEntity> execute(CommandContext commandContext)
		  {
			return commandContext.JobManager.findNextJobsToExecute(new Page(0, 100));
		  }
	  }
	}

}