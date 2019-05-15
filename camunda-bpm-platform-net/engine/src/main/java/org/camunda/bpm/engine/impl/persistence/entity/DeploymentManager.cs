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
namespace org.camunda.bpm.engine.impl.persistence.entity
{

	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ResourceAuthorizationProvider = org.camunda.bpm.engine.impl.cfg.auth.ResourceAuthorizationProvider;
	using DeleteProcessDefinitionsByIdsCmd = org.camunda.bpm.engine.impl.cmd.DeleteProcessDefinitionsByIdsCmd;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DecisionDefinitionManager = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionDefinitionManager;
	using DecisionRequirementsDefinitionManager = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionRequirementsDefinitionManager;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using DecisionRequirementsDefinition = org.camunda.bpm.engine.repository.DecisionRequirementsDefinition;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ResourceTypes = org.camunda.bpm.engine.repository.ResourceTypes;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Deivarayan Azhagappan
	/// @author Christopher Zell
	/// </summary>
	public class DeploymentManager : AbstractManager
	{

	  public virtual void insertDeployment(DeploymentEntity deployment)
	  {
		DbEntityManager.insert(deployment);
		createDefaultAuthorizations(deployment);

		foreach (ResourceEntity resource in deployment.Resources.Values)
		{
		  resource.DeploymentId = deployment.Id;
		  resource.Type = ResourceTypes.REPOSITORY.Value;
		  resource.CreateTime = ClockUtil.CurrentTime;
		  ResourceManager.insertResource(resource);
		}

		Context.ProcessEngineConfiguration.DeploymentCache.deploy(deployment);
	  }

	  public virtual void deleteDeployment(string deploymentId, bool cascade)
	  {
		deleteDeployment(deploymentId, cascade, false, false);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void deleteDeployment(String deploymentId, final boolean cascade, final boolean skipCustomListeners, boolean skipIoMappings)
	  public virtual void deleteDeployment(string deploymentId, bool cascade, bool skipCustomListeners, bool skipIoMappings)
	  {
		IList<ProcessDefinition> processDefinitions = ProcessDefinitionManager.findProcessDefinitionsByDeploymentId(deploymentId);
		if (cascade)
		{
		  // *NOTE*:
		  // The process instances of ALL process definitions must be
		  // deleted, before every process definition can be deleted!
		  //
		  // On deletion of all process instances, the task listeners will
		  // be deleted as well. Deletion of tasks and listeners needs
		  // the redeployment of deployments, which can cause to problems if
		  // is done sequential with deletion of process definition.
		  //
		  // For example:
		  // Deployment contains two process definiton. First process definition
		  // and instances will be removed, also cleared from the cache.
		  // Second process definition will be removed and his instances.
		  // Deletion of instances will cause redeployment this deploys again
		  // first into the cache. Only the second will be removed from cache and
		  // first remains in the cache after the deletion process.
		  //
		  // Thats why we have to clear up all instances at first, after that
		  // we can cleanly remove the process definitions.
		  foreach (ProcessDefinition processDefinition in processDefinitions)
		  {
			string processDefinitionId = processDefinition.Id;
			ProcessInstanceManager.deleteProcessInstancesByProcessDefinition(processDefinitionId, "deleted deployment", true, skipCustomListeners, skipIoMappings);
		  }
		  // delete historic job logs (for example for timer start event jobs)
		  HistoricJobLogManager.deleteHistoricJobLogsByDeploymentId(deploymentId);
		}


		foreach (ProcessDefinition processDefinition in processDefinitions)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String processDefinitionId = processDefinition.getId();
		  string processDefinitionId = processDefinition.Id;
		  // Process definition cascade true deletes the history and
		  // process instances if instances flag is set as well to true.
		  // Problem as described above, redeployes the deployment.
		  // Represents no problem if only one process definition is deleted
		  // in a transaction! We have to set the instances flag to false.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext = org.camunda.bpm.engine.impl.context.Context.getCommandContext();
		  CommandContext commandContext = Context.CommandContext;
		  commandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this, cascade, skipCustomListeners, processDefinitionId, commandContext));
		}

		deleteCaseDeployment(deploymentId, cascade);

		deleteDecisionDeployment(deploymentId, cascade);
		deleteDecisionRequirementDeployment(deploymentId);

		ResourceManager.deleteResourcesByDeploymentId(deploymentId);

		deleteAuthorizations(Resources.DEPLOYMENT, deploymentId);
		DbEntityManager.delete(typeof(DeploymentEntity), "deleteDeployment", deploymentId);

	  }

	  private class CallableAnonymousInnerClass : Callable<Void>
	  {
		  private readonly DeploymentManager outerInstance;

		  private bool cascade;
		  private bool skipCustomListeners;
		  private string processDefinitionId;
		  private CommandContext commandContext;

		  public CallableAnonymousInnerClass(DeploymentManager outerInstance, bool cascade, bool skipCustomListeners, string processDefinitionId, CommandContext commandContext)
		  {
			  this.outerInstance = outerInstance;
			  this.cascade = cascade;
			  this.skipCustomListeners = skipCustomListeners;
			  this.processDefinitionId = processDefinitionId;
			  this.commandContext = commandContext;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			DeleteProcessDefinitionsByIdsCmd cmd = new DeleteProcessDefinitionsByIdsCmd(Arrays.asList(processDefinitionId), cascade, false, skipCustomListeners, false);
			cmd.execute(commandContext);
			return null;
		  }
	  }

	  protected internal virtual void deleteCaseDeployment(string deploymentId, bool cascade)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
		if (processEngineConfiguration.CmmnEnabled)
		{
		  IList<CaseDefinition> caseDefinitions = CaseDefinitionManager.findCaseDefinitionByDeploymentId(deploymentId);

		  if (cascade)
		  {

			// delete case instances
			foreach (CaseDefinition caseDefinition in caseDefinitions)
			{
			  string caseDefinitionId = caseDefinition.Id;

			  CaseInstanceManager.deleteCaseInstancesByCaseDefinition(caseDefinitionId, "deleted deployment", true);

			}
		  }

		  // delete case definitions from db
		  CaseDefinitionManager.deleteCaseDefinitionsByDeploymentId(deploymentId);

		  foreach (CaseDefinition caseDefinition in caseDefinitions)
		  {
			string processDefinitionId = caseDefinition.Id;

			// remove case definitions from cache:
			Context.ProcessEngineConfiguration.DeploymentCache.removeCaseDefinition(processDefinitionId);
		  }
		}
	  }

	  protected internal virtual void deleteDecisionDeployment(string deploymentId, bool cascade)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
		if (processEngineConfiguration.DmnEnabled)
		{
		  DecisionDefinitionManager decisionDefinitionManager = DecisionDefinitionManager;
		  IList<DecisionDefinition> decisionDefinitions = decisionDefinitionManager.findDecisionDefinitionByDeploymentId(deploymentId);

		  if (cascade)
		  {
			// delete historic decision instances
			foreach (DecisionDefinition decisionDefinition in decisionDefinitions)
			{
			  HistoricDecisionInstanceManager.deleteHistoricDecisionInstancesByDecisionDefinitionId(decisionDefinition.Id);
			}
		  }

		  // delete decision definitions from db
		  decisionDefinitionManager.deleteDecisionDefinitionsByDeploymentId(deploymentId);

		  DeploymentCache deploymentCache = processEngineConfiguration.DeploymentCache;

		  foreach (DecisionDefinition decisionDefinition in decisionDefinitions)
		  {
			string decisionDefinitionId = decisionDefinition.Id;

			// remove decision definitions from cache:
			deploymentCache.removeDecisionDefinition(decisionDefinitionId);
		  }
		}
	  }

	  protected internal virtual void deleteDecisionRequirementDeployment(string deploymentId)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
		if (processEngineConfiguration.DmnEnabled)
		{
		  DecisionRequirementsDefinitionManager manager = DecisionRequirementsDefinitionManager;
		  IList<DecisionRequirementsDefinition> decisionRequirementsDefinitions = manager.findDecisionRequirementsDefinitionByDeploymentId(deploymentId);

		  // delete decision requirements definitions from db
		  manager.deleteDecisionRequirementsDefinitionsByDeploymentId(deploymentId);

		  DeploymentCache deploymentCache = processEngineConfiguration.DeploymentCache;

		  foreach (DecisionRequirementsDefinition decisionRequirementsDefinition in decisionRequirementsDefinitions)
		  {
			string decisionDefinitionId = decisionRequirementsDefinition.Id;

			// remove decision requirements definitions from cache:
			deploymentCache.removeDecisionRequirementsDefinition(decisionDefinitionId);
		  }
		}
	  }

	  public virtual DeploymentEntity findLatestDeploymentByName(string deploymentName)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<?> list = getDbEntityManager().selectList("selectDeploymentsByName", deploymentName, 0, 1);
		IList<object> list = DbEntityManager.selectList("selectDeploymentsByName", deploymentName, 0, 1);
		if (list != null && list.Count > 0)
		{
		  return (DeploymentEntity) list[0];
		}
		return null;
	  }

	  public virtual DeploymentEntity findDeploymentById(string deploymentId)
	  {
		return DbEntityManager.selectById(typeof(DeploymentEntity), deploymentId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<DeploymentEntity> findDeploymentsByIds(String... deploymentsIds)
	  public virtual IList<DeploymentEntity> findDeploymentsByIds(params string[] deploymentsIds)
	  {
		return DbEntityManager.selectList("selectDeploymentsByIds", deploymentsIds);
	  }

	  public virtual long findDeploymentCountByQueryCriteria(DeploymentQueryImpl deploymentQuery)
	  {
		configureQuery(deploymentQuery);
		return (long?) DbEntityManager.selectOne("selectDeploymentCountByQueryCriteria", deploymentQuery).Value;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.repository.Deployment> findDeploymentsByQueryCriteria(org.camunda.bpm.engine.impl.DeploymentQueryImpl deploymentQuery, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<Deployment> findDeploymentsByQueryCriteria(DeploymentQueryImpl deploymentQuery, Page page)
	  {
		configureQuery(deploymentQuery);
		return DbEntityManager.selectList("selectDeploymentsByQueryCriteria", deploymentQuery, page);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<String> getDeploymentResourceNames(String deploymentId)
	  public virtual IList<string> getDeploymentResourceNames(string deploymentId)
	  {
		return DbEntityManager.selectList("selectResourceNamesByDeploymentId", deploymentId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<String> findDeploymentIdsByProcessInstances(java.util.List<String> processInstanceIds)
	  public virtual IList<string> findDeploymentIdsByProcessInstances(IList<string> processInstanceIds)
	  {
		return DbEntityManager.selectList("selectDeploymentIdsByProcessInstances", processInstanceIds);
	  }

	  public override void close()
	  {
	  }

	  public override void flush()
	  {
	  }

	  // helper /////////////////////////////////////////////////

	  protected internal virtual void createDefaultAuthorizations(DeploymentEntity deployment)
	  {
		if (AuthorizationEnabled)
		{
		  ResourceAuthorizationProvider provider = ResourceAuthorizationProvider;
		  AuthorizationEntity[] authorizations = provider.newDeployment(deployment);
		  saveDefaultAuthorizations(authorizations);
		}
	  }

	  protected internal virtual void configureQuery(DeploymentQueryImpl query)
	  {
		AuthorizationManager.configureDeploymentQuery(query);
		TenantManager.configureQuery(query);
	  }

	}

}