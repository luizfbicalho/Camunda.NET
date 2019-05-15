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
namespace org.camunda.bpm.engine.impl
{

	using org.camunda.bpm.engine;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using TransactionContextFactory = org.camunda.bpm.engine.impl.cfg.TransactionContextFactory;
	using ExpressionManager = org.camunda.bpm.engine.impl.el.ExpressionManager;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using SessionFactory = org.camunda.bpm.engine.impl.interceptor.SessionFactory;
	using JobExecutor = org.camunda.bpm.engine.impl.jobexecutor.JobExecutor;
	using DbMetricsReporter = org.camunda.bpm.engine.impl.metrics.reporter.DbMetricsReporter;
	using CompositeCondition = org.camunda.bpm.engine.impl.util.CompositeCondition;

	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class ProcessEngineImpl : ProcessEngine
	{

	  /// <summary>
	  /// external task conditions used to signal long polling in rest API </summary>
	  public static readonly CompositeCondition EXT_TASK_CONDITIONS = new CompositeCondition();

	  private static readonly ProcessEngineLogger LOG = ProcessEngineLogger.INSTANCE;

	  protected internal string name;

	  protected internal RepositoryService repositoryService;
	  protected internal RuntimeService runtimeService;
	  protected internal HistoryService historicDataService;
	  protected internal IdentityService identityService;
	  protected internal TaskService taskService;
	  protected internal FormService formService;
	  protected internal ManagementService managementService;
	  protected internal AuthorizationService authorizationService;
	  protected internal CaseService caseService;
	  protected internal FilterService filterService;
	  protected internal ExternalTaskService externalTaskService;
	  protected internal DecisionService decisionService;

	  protected internal string databaseSchemaUpdate;
	  protected internal JobExecutor jobExecutor;
	  protected internal CommandExecutor commandExecutor;
	  protected internal CommandExecutor commandExecutorSchemaOperations;
	  protected internal IDictionary<Type, SessionFactory> sessionFactories;
	  protected internal ExpressionManager expressionManager;
	  protected internal HistoryLevel historyLevel;
	  protected internal TransactionContextFactory transactionContextFactory;
	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;

	  public ProcessEngineImpl(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {

		this.processEngineConfiguration = processEngineConfiguration;
		this.name = processEngineConfiguration.ProcessEngineName;

		this.repositoryService = processEngineConfiguration.RepositoryService;
		this.runtimeService = processEngineConfiguration.RuntimeService;
		this.historicDataService = processEngineConfiguration.HistoryService;
		this.identityService = processEngineConfiguration.IdentityService;
		this.taskService = processEngineConfiguration.TaskService;
		this.formService = processEngineConfiguration.FormService;
		this.managementService = processEngineConfiguration.ManagementService;
		this.authorizationService = processEngineConfiguration.AuthorizationService;
		this.caseService = processEngineConfiguration.CaseService;
		this.filterService = processEngineConfiguration.FilterService;
		this.externalTaskService = processEngineConfiguration.ExternalTaskService;
		this.decisionService = processEngineConfiguration.DecisionService;

		this.databaseSchemaUpdate = processEngineConfiguration.DatabaseSchemaUpdate;
		this.jobExecutor = processEngineConfiguration.JobExecutor;
		this.commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		commandExecutorSchemaOperations = processEngineConfiguration.CommandExecutorSchemaOperations;
		this.sessionFactories = processEngineConfiguration.SessionFactories;
		this.historyLevel = processEngineConfiguration.HistoryLevel;
		this.transactionContextFactory = processEngineConfiguration.TransactionContextFactory;

		executeSchemaOperations();

		if (string.ReferenceEquals(name, null))
		{
		  LOG.processEngineCreated(ProcessEngines.NAME_DEFAULT);
		}
		else
		{
		  LOG.processEngineCreated(name);
		}

		ProcessEngines.registerProcessEngine(this);

		if ((jobExecutor != null))
		{
		  // register process engine with Job Executor
		  jobExecutor.registerProcessEngine(this);
		}

		if (processEngineConfiguration.MetricsEnabled)
		{
		  string reporterId = processEngineConfiguration.MetricsReporterIdProvider.provideId(this);
		  DbMetricsReporter dbMetricsReporter = processEngineConfiguration.DbMetricsReporter;
		  dbMetricsReporter.ReporterId = reporterId;

		  if (processEngineConfiguration.DbMetricsReporterActivate)
		  {
			dbMetricsReporter.start();
		  }
		}
	  }

	  protected internal virtual void executeSchemaOperations()
	  {
		commandExecutorSchemaOperations.execute(processEngineConfiguration.SchemaOperationsCommand);
		commandExecutorSchemaOperations.execute(processEngineConfiguration.HistoryLevelCommand);

		try
		{
		  commandExecutorSchemaOperations.execute(processEngineConfiguration.ProcessEngineBootstrapCommand);
		}
		catch (OptimisticLockingException ole)
		{
		  LOG.historyCleanupJobReconfigurationFailure(ole);
		}
	  }

	  public virtual void close()
	  {

		ProcessEngines.unregister(this);

		if (processEngineConfiguration.MetricsEnabled)
		{
		  processEngineConfiguration.DbMetricsReporter.stop();
		}

		if ((jobExecutor != null))
		{
		  // unregister process engine with Job Executor
		  jobExecutor.unregisterProcessEngine(this);
		}

		commandExecutorSchemaOperations.execute(new SchemaOperationProcessEngineClose());

		processEngineConfiguration.close();

		LOG.processEngineClosed(name);
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl ProcessEngineConfiguration
	  {
		  get
		  {
			return processEngineConfiguration;
		  }
	  }

	  public virtual IdentityService IdentityService
	  {
		  get
		  {
			return identityService;
		  }
	  }

	  public virtual ManagementService ManagementService
	  {
		  get
		  {
			return managementService;
		  }
	  }

	  public virtual TaskService TaskService
	  {
		  get
		  {
			return taskService;
		  }
	  }

	  public virtual HistoryService HistoryService
	  {
		  get
		  {
			return historicDataService;
		  }
	  }

	  public virtual RuntimeService RuntimeService
	  {
		  get
		  {
			return runtimeService;
		  }
	  }

	  public virtual RepositoryService RepositoryService
	  {
		  get
		  {
			return repositoryService;
		  }
	  }

	  public virtual FormService FormService
	  {
		  get
		  {
			return formService;
		  }
	  }

	  public virtual AuthorizationService AuthorizationService
	  {
		  get
		  {
			return authorizationService;
		  }
	  }

	  public virtual CaseService CaseService
	  {
		  get
		  {
			return caseService;
		  }
	  }

	  public virtual FilterService FilterService
	  {
		  get
		  {
			return filterService;
		  }
	  }

	  public virtual ExternalTaskService ExternalTaskService
	  {
		  get
		  {
			return externalTaskService;
		  }
	  }

	  public virtual DecisionService DecisionService
	  {
		  get
		  {
			return decisionService;
		  }
	  }

	}

}