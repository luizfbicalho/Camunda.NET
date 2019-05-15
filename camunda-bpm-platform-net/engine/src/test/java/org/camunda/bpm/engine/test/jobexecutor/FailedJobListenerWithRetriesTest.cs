using System;
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
	using DateUtils = org.apache.commons.lang3.time.DateUtils;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using DefaultJobRetryCmd = org.camunda.bpm.engine.impl.cmd.DefaultJobRetryCmd;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DefaultFailedJobCommandFactory = org.camunda.bpm.engine.impl.jobexecutor.DefaultFailedJobCommandFactory;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class FailedJobListenerWithRetriesTest
	public class FailedJobListenerWithRetriesTest
	{
		private bool InstanceFieldsInitialized = false;

		public FailedJobListenerWithRetriesTest()
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


	  protected internal ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {
			configuration.FailedJobCommandFactory = new OLEFailedJobCommandFactory();
			configuration.FailedJobListenerMaxRetries = 5;
			return configuration;
		  }
	  }

	  protected internal ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);
	  public ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

	  private RuntimeService runtimeService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(0) public int failedRetriesNumber;
	  public int failedRetriesNumber;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(1) public int jobRetries;
	  public int jobRetries;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(2) public boolean jobLocked;
	  public bool jobLocked;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		runtimeService = engineRule.RuntimeService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameters public static java.util.Collection<Object[]> scenarios()
	  public static ICollection<object[]> scenarios()
	  {
		return Arrays.asList(new object[][]
		{
			new object[] {4, 0, false},
			new object[] {5, 1, true}
		});
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @org.camunda.bpm.engine.test.Deployment(resources = {"org/camunda/bpm/engine/test/api/mgmt/IncidentTest.testShouldCreateOneIncident.bpmn"}) public void testFailedJobListenerRetries()
	  public virtual void testFailedJobListenerRetries()
	  {
		//given
		runtimeService.startProcessInstanceByKey("failingProcess");

		//when the job is run several times till the incident creation
		Job job = Job;
		while (job.Retries > 0 && string.ReferenceEquals(((JobEntity)job).LockOwner, null))
		{
		  try
		  {
			lockTheJob(job.Id);
			engineRule.ManagementService.executeJob(job.Id);
		  }
		  catch (Exception)
		  {
		  }
		  job = Job;
		}

		//then
		JobEntity jobFinalState = (JobEntity)engineRule.ManagementService.createJobQuery().jobId(job.Id).list().get(0);
		assertEquals(jobRetries, jobFinalState.Retries);
		if (jobLocked)
		{
		  assertNotNull(jobFinalState.LockOwner);
		  assertNotNull(jobFinalState.LockExpirationTime);
		}
		else
		{
		  assertNull(jobFinalState.LockOwner);
		  assertNull(jobFinalState.LockExpirationTime);
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: void lockTheJob(final String jobId)
	  internal virtual void lockTheJob(string jobId)
	  {
		engineRule.ProcessEngineConfiguration.CommandExecutorTxRequiresNew.execute(new CommandAnonymousInnerClass(this, jobId));
	  }

	  private class CommandAnonymousInnerClass : Command<object>
	  {
		  private readonly FailedJobListenerWithRetriesTest outerInstance;

		  private string jobId;

		  public CommandAnonymousInnerClass(FailedJobListenerWithRetriesTest outerInstance, string jobId)
		  {
			  this.outerInstance = outerInstance;
			  this.jobId = jobId;
		  }

		  public object execute(CommandContext commandContext)
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.JobEntity job = commandContext.getJobManager().findJobById(jobId);
			JobEntity job = commandContext.JobManager.findJobById(jobId);
			job.LockOwner = "someLockOwner";
			job.LockExpirationTime = DateUtils.addHours(ClockUtil.CurrentTime, 1);
			return null;
		  }
	  }

	  private Job Job
	  {
		  get
		  {
			IList<Job> jobs = engineRule.ManagementService.createJobQuery().list();
			assertEquals(1, jobs.Count);
			return jobs[0];
		  }
	  }

	  private class OLEFailedJobCommandFactory : DefaultFailedJobCommandFactory
	  {
		  private readonly FailedJobListenerWithRetriesTest outerInstance;

		  public OLEFailedJobCommandFactory(FailedJobListenerWithRetriesTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		internal IDictionary<string, OLEFoxJobRetryCmd> oleFoxJobRetryCmds = new Dictionary<string, OLEFoxJobRetryCmd>();

		public override Command<object> getCommand(string jobId, Exception exception)
		{
		  return getOleFoxJobRetryCmds(jobId, exception);
		}

		public virtual OLEFoxJobRetryCmd getOleFoxJobRetryCmds(string jobId, Exception exception)
		{
		  if (!oleFoxJobRetryCmds.ContainsKey(jobId))
		  {
			oleFoxJobRetryCmds[jobId] = new OLEFoxJobRetryCmd(outerInstance, jobId, exception);
		  }
		  return oleFoxJobRetryCmds[jobId];
		}
	  }

	  private class OLEFoxJobRetryCmd : DefaultJobRetryCmd
	  {
		  private readonly FailedJobListenerWithRetriesTest outerInstance;


		internal int countRuns = 0;

		public OLEFoxJobRetryCmd(FailedJobListenerWithRetriesTest outerInstance, string jobId, Exception exception) : base(jobId, exception)
		{
			this.outerInstance = outerInstance;
		}

		public override object execute(CommandContext commandContext)
		{
		  Job job = Job;
		  //on last attempt the incident will be created, we imitate OLE
		  if (job.Retries == 1)
		  {
			countRuns++;
			if (countRuns <= outerInstance.failedRetriesNumber)
			{
			  base.execute(commandContext);
			  throw new OptimisticLockingException("OLE");
			}
		  }
		  return base.execute(commandContext);
		}
	  }
	}

}