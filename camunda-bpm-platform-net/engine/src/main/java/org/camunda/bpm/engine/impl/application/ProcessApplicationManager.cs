using System;
using System.Collections.Generic;
using System.Text;

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
namespace org.camunda.bpm.engine.impl.application
{
	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using ProcessApplicationRegistration = org.camunda.bpm.application.ProcessApplicationRegistration;
	using ProcessApplicationLogger = org.camunda.bpm.application.impl.ProcessApplicationLogger;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using TransactionState = org.camunda.bpm.engine.impl.cfg.TransactionState;
	using CaseDefinitionEntity = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity;
	using CaseDefinitionManager = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionManager;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DeploymentFailListener = org.camunda.bpm.engine.impl.persistence.deploy.DeploymentFailListener;
	using DeploymentEntity = org.camunda.bpm.engine.impl.persistence.entity.DeploymentEntity;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ProcessDefinitionManager = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionManager;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ProcessApplicationManager
	{

	  public static readonly ProcessApplicationLogger LOG = ProcessEngineLogger.PROCESS_APPLICATION_LOGGER;

	  protected internal IDictionary<string, DefaultProcessApplicationRegistration> registrationsByDeploymentId = new Dictionary<string, DefaultProcessApplicationRegistration>();

	  public virtual ProcessApplicationReference getProcessApplicationForDeployment(string deploymentId)
	  {
		DefaultProcessApplicationRegistration registration = registrationsByDeploymentId[deploymentId];
		if (registration != null)
		{
		  return registration.Reference;
		}
		else
		{
		  return null;
		}
	  }

	  public virtual ProcessApplicationRegistration registerProcessApplicationForDeployments(ISet<string> deploymentsToRegister, ProcessApplicationReference reference)
	  {
		  lock (this)
		  {
			// create process application registration
			DefaultProcessApplicationRegistration registration = createProcessApplicationRegistration(deploymentsToRegister, reference);
			// register with job executor
			createJobExecutorRegistrations(deploymentsToRegister);
			logRegistration(deploymentsToRegister, reference);
			return registration;
		  }
	  }

	  public virtual void clearRegistrations()
	  {
		  lock (this)
		  {
			registrationsByDeploymentId.Clear();
		  }
	  }

	  public virtual void unregisterProcessApplicationForDeployments(ISet<string> deploymentIds, bool removeProcessesFromCache)
	  {
		  lock (this)
		  {
			removeJobExecutorRegistrations(deploymentIds);
			removeProcessApplicationRegistration(deploymentIds, removeProcessesFromCache);
		  }
	  }

	  public virtual bool hasRegistrations()
	  {
		return registrationsByDeploymentId.Count > 0;
	  }

	  protected internal virtual DefaultProcessApplicationRegistration createProcessApplicationRegistration(ISet<string> deploymentsToRegister, ProcessApplicationReference reference)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String processEngineName = org.camunda.bpm.engine.impl.context.Context.getProcessEngineConfiguration().getProcessEngineName();
		string processEngineName = Context.ProcessEngineConfiguration.ProcessEngineName;

		DefaultProcessApplicationRegistration registration = new DefaultProcessApplicationRegistration(reference, deploymentsToRegister, processEngineName);
		// add to registration map
		foreach (string deploymentId in deploymentsToRegister)
		{
		  registrationsByDeploymentId[deploymentId] = registration;
		}
		return registration;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void removeProcessApplicationRegistration(final java.util.Set<String> deploymentIds, boolean removeProcessesFromCache)
	  protected internal virtual void removeProcessApplicationRegistration(ISet<string> deploymentIds, bool removeProcessesFromCache)
	  {
		foreach (string deploymentId in deploymentIds)
		{
		  try
		  {
			if (removeProcessesFromCache)
			{
			  Context.ProcessEngineConfiguration.DeploymentCache.removeDeployment(deploymentId);
			}
		  }
		  catch (Exception t)
		  {
			LOG.couldNotRemoveDefinitionsFromCache(t);
		  }
		  finally
		  {
			if (!string.ReferenceEquals(deploymentId, null))
			{
			  registrationsByDeploymentId.Remove(deploymentId);
			}
		  }
		}
	  }

	  protected internal virtual void createJobExecutorRegistrations(ISet<string> deploymentIds)
	  {
		try
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.deploy.DeploymentFailListener deploymentFailListener = new org.camunda.bpm.engine.impl.persistence.deploy.DeploymentFailListener(deploymentIds, org.camunda.bpm.engine.impl.context.Context.getProcessEngineConfiguration().getCommandExecutorTxRequiresNew());
		  DeploymentFailListener deploymentFailListener = new DeploymentFailListener(deploymentIds, Context.ProcessEngineConfiguration.CommandExecutorTxRequiresNew);
		  Context.CommandContext.TransactionContext.addTransactionListener(TransactionState.ROLLED_BACK, deploymentFailListener);

		  ISet<string> registeredDeployments = Context.ProcessEngineConfiguration.RegisteredDeployments;
		  registeredDeployments.addAll(deploymentIds);

		}
		catch (Exception e)
		{
		  throw LOG.exceptionWhileRegisteringDeploymentsWithJobExecutor(e);
		}
	  }

	  protected internal virtual void removeJobExecutorRegistrations(ISet<string> deploymentIds)
	  {
		try
		{
		  ISet<string> registeredDeployments = Context.ProcessEngineConfiguration.RegisteredDeployments;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
		  registeredDeployments.removeAll(deploymentIds);

		}
		catch (Exception e)
		{
		  LOG.exceptionWhileUnregisteringDeploymentsWithJobExecutor(e);
		}
	  }

	  // logger ////////////////////////////////////////////////////////////////////////////

	  protected internal virtual void logRegistration(ISet<string> deploymentIds, ProcessApplicationReference reference)
	  {

		if (!LOG.InfoEnabled)
		{
		  // building the log message is expensive (db queries) so we avoid it if we can
		  return;
		}

		try
		{
		  StringBuilder builder = new StringBuilder();
		  builder.Append("ProcessApplication '");
		  builder.Append(reference.Name);
		  builder.Append("' registered for DB deployments ");
		  builder.Append(deploymentIds);
		  builder.Append(". ");

		  IList<ProcessDefinition> processDefinitions = new List<ProcessDefinition>();
		  IList<CaseDefinition> caseDefinitions = new List<CaseDefinition>();

		  CommandContext commandContext = Context.CommandContext;
		  ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
		  bool cmmnEnabled = processEngineConfiguration.CmmnEnabled;

		  foreach (string deploymentId in deploymentIds)
		  {

			DeploymentEntity deployment = commandContext.DbEntityManager.selectById(typeof(DeploymentEntity), deploymentId);

			if (deployment != null)
			{

			  ((IList<ProcessDefinition>)processDefinitions).AddRange(getDeployedProcessDefinitionArtifacts(deployment));

			  if (cmmnEnabled)
			  {
				((IList<CaseDefinition>)caseDefinitions).AddRange(getDeployedCaseDefinitionArtifacts(deployment));
			  }

			}
		  }

		  logProcessDefinitionRegistrations(builder, processDefinitions);

		  if (cmmnEnabled)
		  {
			logCaseDefinitionRegistrations(builder, caseDefinitions);
		  }

		  LOG.registrationSummary(builder.ToString());

		}
		catch (Exception e)
		{
		  LOG.exceptionWhileLoggingRegistrationSummary(e);
		}
	  }

	  protected internal virtual IList<ProcessDefinition> getDeployedProcessDefinitionArtifacts(DeploymentEntity deployment)
	  {
		CommandContext commandContext = Context.CommandContext;

		// in case deployment was created by this command
		IList<ProcessDefinition> entities = deployment.DeployedProcessDefinitions;

		if (entities == null)
		{
		  string deploymentId = deployment.Id;
		  ProcessDefinitionManager manager = commandContext.ProcessDefinitionManager;
		  return manager.findProcessDefinitionsByDeploymentId(deploymentId);
		}

		return entities;

	  }

	  protected internal virtual IList<CaseDefinition> getDeployedCaseDefinitionArtifacts(DeploymentEntity deployment)
	  {
		CommandContext commandContext = Context.CommandContext;

		// in case deployment was created by this command
		IList<CaseDefinition> entities = deployment.DeployedCaseDefinitions;

		if (entities == null)
		{
		  string deploymentId = deployment.Id;
		  CaseDefinitionManager caseDefinitionManager = commandContext.CaseDefinitionManager;
		  return caseDefinitionManager.findCaseDefinitionByDeploymentId(deploymentId);
		}

		return entities;

	  }

	  protected internal virtual void logProcessDefinitionRegistrations(StringBuilder builder, IList<ProcessDefinition> processDefinitions)
	  {
		if (processDefinitions.Count == 0)
		{
		  builder.Append("Deployment does not provide any process definitions.");

		}
		else
		{
		  builder.Append("Will execute process definitions ");
		  builder.Append("\n");
		  foreach (ProcessDefinition processDefinition in processDefinitions)
		  {
			builder.Append("\n");
			builder.Append("        ");
			builder.Append(processDefinition.Key);
			builder.Append("[version: ");
			builder.Append(processDefinition.Version);
			builder.Append(", id: ");
			builder.Append(processDefinition.Id);
			builder.Append("]");
		  }
		  builder.Append("\n");
		}
	  }

	  protected internal virtual void logCaseDefinitionRegistrations(StringBuilder builder, IList<CaseDefinition> caseDefinitions)
	  {
		if (caseDefinitions.Count == 0)
		{
		  builder.Append("Deployment does not provide any case definitions.");

		}
		else
		{
		  builder.Append("\n");
		  builder.Append("Will execute case definitions ");
		  builder.Append("\n");
		  foreach (CaseDefinition caseDefinition in caseDefinitions)
		  {
			builder.Append("\n");
			builder.Append("        ");
			builder.Append(caseDefinition.Key);
			builder.Append("[version: ");
			builder.Append(caseDefinition.Version);
			builder.Append(", id: ");
			builder.Append(caseDefinition.Id);
			builder.Append("]");
		  }
		  builder.Append("\n");
		}
	  }

	  public virtual string RegistrationSummary
	  {
		  get
		  {
			StringBuilder builder = new StringBuilder();
			foreach (KeyValuePair<string, DefaultProcessApplicationRegistration> entry in registrationsByDeploymentId.SetOfKeyValuePairs())
			{
			  if (builder.Length > 0)
			  {
				builder.Append(", ");
			  }
			  builder.Append(entry.Key);
			  builder.Append("->");
			  builder.Append(entry.Value.Reference.Name);
			}
			return builder.ToString();
		  }
	  }

	}

}