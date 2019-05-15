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
namespace org.camunda.bpm.engine.test.util
{
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using TestWatcher = org.junit.rules.TestWatcher;
	using Description = org.junit.runner.Description;

	public class ProcessEngineBootstrapRule : TestWatcher
	{

	  private ProcessEngine processEngine;

	  public ProcessEngineBootstrapRule() : this("camunda.cfg.xml")
	  {
	  }

	  public ProcessEngineBootstrapRule(string configurationResource)
	  {
		this.processEngine = bootstrapEngine(configurationResource);
	  }

	  public virtual ProcessEngine bootstrapEngine(string configurationResource)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = (ProcessEngineConfigurationImpl) ProcessEngineConfiguration.createProcessEngineConfigurationFromResource(configurationResource);
		configureEngine(processEngineConfiguration);
		return processEngineConfiguration.buildProcessEngine();
	  }

	  public virtual ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
	  {
		return configuration;
	  }

	  public virtual ProcessEngine ProcessEngine
	  {
		  get
		  {
			return processEngine;
		  }
	  }

	  protected internal override void finished(Description description)
	  {
		deleteHistoryCleanupJob();
		processEngine.close();
		ProcessEngines.unregister(processEngine);
		processEngine = null;
	  }

	  private void deleteHistoryCleanupJob()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.runtime.Job> jobs = processEngine.getHistoryService().findHistoryCleanupJobs();
		IList<Job> jobs = processEngine.HistoryService.findHistoryCleanupJobs();
		foreach (Job job in jobs)
		{
		  ((ProcessEngineConfigurationImpl)processEngine.ProcessEngineConfiguration).CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));
		}
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly ProcessEngineBootstrapRule outerInstance;

		  public CommandAnonymousInnerClass(ProcessEngineBootstrapRule outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			commandContext.JobManager.deleteJob((JobEntity) job);
			commandContext.HistoricJobLogManager.deleteHistoricJobLogByJobId(job.Id);
			return null;
		  }
	  }

	}

}