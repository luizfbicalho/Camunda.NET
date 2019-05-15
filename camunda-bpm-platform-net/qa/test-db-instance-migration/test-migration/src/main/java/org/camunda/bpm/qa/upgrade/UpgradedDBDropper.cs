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
namespace org.camunda.bpm.qa.upgrade
{
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using ProcessEngineConfiguration = org.camunda.bpm.engine.ProcessEngineConfiguration;
	using RepositoryService = org.camunda.bpm.engine.RepositoryService;
	using ProcessEngineImpl = org.camunda.bpm.engine.impl.ProcessEngineImpl;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;

	/// <summary>
	/// @author Thorben Lindhauer
	/// </summary>
	public class UpgradedDBDropper
	{

	  public static void Main(string[] args)
	  {
		ProcessEngine engine = ProcessEngineConfiguration.createProcessEngineConfigurationFromResource("camunda.cfg.xml").buildProcessEngine();

		UpgradedDBDropper fixture = new UpgradedDBDropper();
		fixture.cleanDatabase(engine);
	  }

	  public UpgradedDBDropper()
	  {
	  }

	  public virtual void cleanDatabase(ProcessEngine engine)
	  {

		// delete all deployments
		RepositoryService repositoryService = engine.RepositoryService;
		IList<Deployment> deployments = repositoryService.createDeploymentQuery().list();
		foreach (Deployment deployment in deployments)
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}

		// drop DB
		((ProcessEngineImpl)engine).ProcessEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));

		engine.close();
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly UpgradedDBDropper outerInstance;

		  public CommandAnonymousInnerClass(UpgradedDBDropper outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {

			commandContext.DbSqlSession.dbSchemaDrop();

			return null;
		  }
	  }
	}

}