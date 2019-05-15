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
namespace org.camunda.bpm.engine.impl.persistence.deploy.cache
{
	using CaseDefinitionEntity = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DecisionDefinitionEntity = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionDefinitionEntity;
	using DecisionRequirementsDefinitionEntity = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionRequirementsDefinitionEntity;
	using DecisionRequirementsDefinitionQueryImpl = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionRequirementsDefinitionQueryImpl;
	using DeploymentEntity = org.camunda.bpm.engine.impl.persistence.entity.DeploymentEntity;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using DecisionRequirementsDefinition = org.camunda.bpm.engine.repository.DecisionRequirementsDefinition;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using CmmnModelInstance = org.camunda.bpm.model.cmmn.CmmnModelInstance;
	using DmnModelInstance = org.camunda.bpm.model.dmn.DmnModelInstance;
	using Cache = org.camunda.commons.utils.cache.Cache;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Falko Menge
	/// </summary>
	public class DeploymentCache
	{

	  protected internal ProcessDefinitionCache processDefinitionEntityCache;
	  protected internal CaseDefinitionCache caseDefinitionCache;
	  protected internal DecisionDefinitionCache decisionDefinitionCache;
	  protected internal DecisionRequirementsDefinitionCache decisionRequirementsDefinitionCache;

	  protected internal BpmnModelInstanceCache bpmnModelInstanceCache;
	  protected internal CmmnModelInstanceCache cmmnModelInstanceCache;
	  protected internal DmnModelInstanceCache dmnModelInstanceCache;
	  protected internal CacheDeployer cacheDeployer = new CacheDeployer();

	  public DeploymentCache(CacheFactory factory, int cacheCapacity)
	  {
		processDefinitionEntityCache = new ProcessDefinitionCache(factory, cacheCapacity, cacheDeployer);
		caseDefinitionCache = new CaseDefinitionCache(factory, cacheCapacity, cacheDeployer);
		decisionDefinitionCache = new DecisionDefinitionCache(factory, cacheCapacity, cacheDeployer);
		decisionRequirementsDefinitionCache = new DecisionRequirementsDefinitionCache(factory, cacheCapacity, cacheDeployer);

		bpmnModelInstanceCache = new BpmnModelInstanceCache(factory, cacheCapacity, processDefinitionEntityCache);
		cmmnModelInstanceCache = new CmmnModelInstanceCache(factory, cacheCapacity, caseDefinitionCache);
		dmnModelInstanceCache = new DmnModelInstanceCache(factory, cacheCapacity, decisionDefinitionCache);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void deploy(final org.camunda.bpm.engine.impl.persistence.entity.DeploymentEntity deployment)
	  public virtual void deploy(DeploymentEntity deployment)
	  {
		cacheDeployer.deploy(deployment);
	  }

	  // PROCESS DEFINITION ////////////////////////////////////////////////////////////////////////////////

	  public virtual ProcessDefinitionEntity findProcessDefinitionFromCache(string processDefinitionId)
	  {
		return processDefinitionEntityCache.findDefinitionFromCache(processDefinitionId);
	  }

	  public virtual ProcessDefinitionEntity findDeployedProcessDefinitionById(string processDefinitionId)
	  {
		return processDefinitionEntityCache.findDeployedDefinitionById(processDefinitionId);
	  }

	  /// <returns> the latest version of the process definition with the given key (from any tenant) </returns>
	  /// <exception cref="ProcessEngineException"> if more than one tenant has a process definition with the given key </exception>
	  /// <seealso cref= #findDeployedLatestProcessDefinitionByKeyAndTenantId(String, String) </seealso>
	  public virtual ProcessDefinitionEntity findDeployedLatestProcessDefinitionByKey(string processDefinitionKey)
	  {
		return processDefinitionEntityCache.findDeployedLatestDefinitionByKey(processDefinitionKey);
	  }

	  /// <returns> the latest version of the process definition with the given key and tenant id </returns>
	  public virtual ProcessDefinitionEntity findDeployedLatestProcessDefinitionByKeyAndTenantId(string processDefinitionKey, string tenantId)
	  {
		return processDefinitionEntityCache.findDeployedLatestDefinitionByKeyAndTenantId(processDefinitionKey, tenantId);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity findDeployedProcessDefinitionByKeyVersionAndTenantId(final String processDefinitionKey, final System.Nullable<int> processDefinitionVersion, final String tenantId)
	  public virtual ProcessDefinitionEntity findDeployedProcessDefinitionByKeyVersionAndTenantId(string processDefinitionKey, int? processDefinitionVersion, string tenantId)
	  {
		return processDefinitionEntityCache.findDeployedDefinitionByKeyVersionAndTenantId(processDefinitionKey, processDefinitionVersion, tenantId);
	  }

	  public virtual ProcessDefinitionEntity findDeployedProcessDefinitionByKeyVersionTagAndTenantId(string processDefinitionKey, string processDefinitionVersionTag, string tenantId)
	  {
		return processDefinitionEntityCache.findDeployedDefinitionByKeyVersionTagAndTenantId(processDefinitionKey, processDefinitionVersionTag, tenantId);
	  }

	  public virtual ProcessDefinitionEntity findDeployedProcessDefinitionByDeploymentAndKey(string deploymentId, string processDefinitionKey)
	  {
		return processDefinitionEntityCache.findDeployedDefinitionByDeploymentAndKey(deploymentId, processDefinitionKey);
	  }

	  public virtual ProcessDefinitionEntity resolveProcessDefinition(ProcessDefinitionEntity processDefinition)
	  {
		return processDefinitionEntityCache.resolveDefinition(processDefinition);
	  }

	  public virtual BpmnModelInstance findBpmnModelInstanceForProcessDefinition(ProcessDefinitionEntity processDefinitionEntity)
	  {
		return bpmnModelInstanceCache.findBpmnModelInstanceForDefinition(processDefinitionEntity);
	  }

	  public virtual BpmnModelInstance findBpmnModelInstanceForProcessDefinition(string processDefinitionId)
	  {
		return bpmnModelInstanceCache.findBpmnModelInstanceForDefinition(processDefinitionId);
	  }

	  public virtual void addProcessDefinition(ProcessDefinitionEntity processDefinition)
	  {
		processDefinitionEntityCache.addDefinition(processDefinition);
	  }

	  public virtual void removeProcessDefinition(string processDefinitionId)
	  {
		processDefinitionEntityCache.removeDefinitionFromCache(processDefinitionId);
		bpmnModelInstanceCache.remove(processDefinitionId);
	  }

	  public virtual void discardProcessDefinitionCache()
	  {
		processDefinitionEntityCache.clear();
		bpmnModelInstanceCache.clear();
	  }

	  // CASE DEFINITION ////////////////////////////////////////////////////////////////////////////////

	  public virtual CaseDefinitionEntity findCaseDefinitionFromCache(string caseDefinitionId)
	  {
		return caseDefinitionCache.findDefinitionFromCache(caseDefinitionId);
	  }

	  public virtual CaseDefinitionEntity findDeployedCaseDefinitionById(string caseDefinitionId)
	  {
		return caseDefinitionCache.findDeployedDefinitionById(caseDefinitionId);
	  }

	  /// <returns> the latest version of the case definition with the given key (from any tenant) </returns>
	  /// <exception cref="ProcessEngineException"> if more than one tenant has a case definition with the given key </exception>
	  /// <seealso cref= #findDeployedLatestCaseDefinitionByKeyAndTenantId(String, String) </seealso>
	  public virtual CaseDefinitionEntity findDeployedLatestCaseDefinitionByKey(string caseDefinitionKey)
	  {
		return caseDefinitionCache.findDeployedLatestDefinitionByKey(caseDefinitionKey);
	  }

	  /// <returns> the latest version of the case definition with the given key and tenant id </returns>
	  public virtual CaseDefinitionEntity findDeployedLatestCaseDefinitionByKeyAndTenantId(string caseDefinitionKey, string tenantId)
	  {
		return caseDefinitionCache.findDeployedLatestDefinitionByKeyAndTenantId(caseDefinitionKey, tenantId);
	  }

	  public virtual CaseDefinitionEntity findDeployedCaseDefinitionByKeyVersionAndTenantId(string caseDefinitionKey, int? caseDefinitionVersion, string tenantId)
	  {
		return caseDefinitionCache.findDeployedDefinitionByKeyVersionAndTenantId(caseDefinitionKey, caseDefinitionVersion, tenantId);
	  }

	  public virtual CaseDefinitionEntity findDeployedCaseDefinitionByDeploymentAndKey(string deploymentId, string caseDefinitionKey)
	  {
		return caseDefinitionCache.findDeployedDefinitionByDeploymentAndKey(deploymentId, caseDefinitionKey);
	  }

	  public virtual CaseDefinitionEntity getCaseDefinitionById(string caseDefinitionId)
	  {
		return caseDefinitionCache.getCaseDefinitionById(caseDefinitionId);
	  }

	  public virtual CaseDefinitionEntity resolveCaseDefinition(CaseDefinitionEntity caseDefinition)
	  {
		return caseDefinitionCache.resolveDefinition(caseDefinition);
	  }

	  public virtual CmmnModelInstance findCmmnModelInstanceForCaseDefinition(string caseDefinitionId)
	  {
		return cmmnModelInstanceCache.findBpmnModelInstanceForDefinition(caseDefinitionId);
	  }

	  public virtual void addCaseDefinition(CaseDefinitionEntity caseDefinition)
	  {
		caseDefinitionCache.addDefinition(caseDefinition);
	  }

	  public virtual void removeCaseDefinition(string caseDefinitionId)
	  {
		caseDefinitionCache.removeDefinitionFromCache(caseDefinitionId);
		cmmnModelInstanceCache.remove(caseDefinitionId);
	  }

	  public virtual void discardCaseDefinitionCache()
	  {
		caseDefinitionCache.clear();
		cmmnModelInstanceCache.clear();
	  }

	  // DECISION DEFINITION ////////////////////////////////////////////////////////////////////////////

	  public virtual DecisionDefinitionEntity findDecisionDefinitionFromCache(string decisionDefinitionId)
	  {
		return decisionDefinitionCache.findDefinitionFromCache(decisionDefinitionId);
	  }

	  public virtual DecisionDefinitionEntity findDeployedDecisionDefinitionById(string decisionDefinitionId)
	  {
		return decisionDefinitionCache.findDeployedDefinitionById(decisionDefinitionId);
	  }

	  public virtual DecisionDefinition findDeployedLatestDecisionDefinitionByKey(string decisionDefinitionKey)
	  {
		return decisionDefinitionCache.findDeployedLatestDefinitionByKey(decisionDefinitionKey);
	  }

	  public virtual DecisionDefinition findDeployedLatestDecisionDefinitionByKeyAndTenantId(string decisionDefinitionKey, string tenantId)
	  {
		return decisionDefinitionCache.findDeployedLatestDefinitionByKeyAndTenantId(decisionDefinitionKey, tenantId);
	  }

	  public virtual DecisionDefinition findDeployedDecisionDefinitionByDeploymentAndKey(string deploymentId, string decisionDefinitionKey)
	  {
		return decisionDefinitionCache.findDeployedDefinitionByDeploymentAndKey(deploymentId, decisionDefinitionKey);
	  }

	  public virtual DecisionDefinition findDeployedDecisionDefinitionByKeyAndVersion(string decisionDefinitionKey, int? decisionDefinitionVersion)
	  {
		return decisionDefinitionCache.findDeployedDefinitionByKeyAndVersion(decisionDefinitionKey, decisionDefinitionVersion);
	  }

	  public virtual DecisionDefinition findDeployedDecisionDefinitionByKeyVersionAndTenantId(string decisionDefinitionKey, int? decisionDefinitionVersion, string tenantId)
	  {
		return decisionDefinitionCache.findDeployedDefinitionByKeyVersionAndTenantId(decisionDefinitionKey, decisionDefinitionVersion, tenantId);
	  }

	  public virtual DecisionDefinition findDeployedDecisionDefinitionByKeyVersionTagAndTenantId(string decisionDefinitionKey, string decisionDefinitionVersionTag, string tenantId)
	  {
		return decisionDefinitionCache.findDeployedDefinitionByKeyVersionTagAndTenantId(decisionDefinitionKey, decisionDefinitionVersionTag, tenantId);
	  }

	  public virtual DecisionDefinitionEntity resolveDecisionDefinition(DecisionDefinitionEntity decisionDefinition)
	  {
		return decisionDefinitionCache.resolveDefinition(decisionDefinition);
	  }

	  public virtual DmnModelInstance findDmnModelInstanceForDecisionDefinition(string decisionDefinitionId)
	  {
		return dmnModelInstanceCache.findBpmnModelInstanceForDefinition(decisionDefinitionId);
	  }

	  public virtual void addDecisionDefinition(DecisionDefinitionEntity decisionDefinition)
	  {
		decisionDefinitionCache.addDefinition(decisionDefinition);
	  }

	  public virtual void removeDecisionDefinition(string decisionDefinitionId)
	  {
		decisionDefinitionCache.removeDefinitionFromCache(decisionDefinitionId);
		dmnModelInstanceCache.remove(decisionDefinitionId);
	  }

	  public virtual void discardDecisionDefinitionCache()
	  {
		decisionDefinitionCache.clear();
		dmnModelInstanceCache.clear();
	  }

	  //DECISION REQUIREMENT DEFINITION ////////////////////////////////////////////////////////////////////////////

	  public virtual void addDecisionRequirementsDefinition(DecisionRequirementsDefinitionEntity decisionRequirementsDefinition)
	  {
		decisionRequirementsDefinitionCache.addDefinition(decisionRequirementsDefinition);
	  }

	  public virtual DecisionRequirementsDefinitionEntity findDecisionRequirementsDefinitionFromCache(string decisionRequirementsDefinitionId)
	  {
		return decisionRequirementsDefinitionCache.findDefinitionFromCache(decisionRequirementsDefinitionId);
	  }

	  public virtual DecisionRequirementsDefinitionEntity findDeployedDecisionRequirementsDefinitionById(string decisionRequirementsDefinitionId)
	  {
		return decisionRequirementsDefinitionCache.findDeployedDefinitionById(decisionRequirementsDefinitionId);
	  }

	  public virtual DecisionRequirementsDefinitionEntity resolveDecisionRequirementsDefinition(DecisionRequirementsDefinitionEntity decisionRequirementsDefinition)
	  {
		return decisionRequirementsDefinitionCache.resolveDefinition(decisionRequirementsDefinition);
	  }

	  public virtual void discardDecisionRequirementsDefinitionCache()
	  {
		decisionDefinitionCache.clear();
	  }

	  public virtual void removeDecisionRequirementsDefinition(string decisionRequirementsDefinitionId)
	  {
		decisionRequirementsDefinitionCache.removeDefinitionFromCache(decisionRequirementsDefinitionId);
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual Cache<string, BpmnModelInstance> BpmnModelInstanceCache
	  {
		  get
		  {
			return bpmnModelInstanceCache.Cache;
		  }
	  }

	  public virtual Cache<string, CmmnModelInstance> CmmnModelInstanceCache
	  {
		  get
		  {
			return cmmnModelInstanceCache.Cache;
		  }
	  }

	  public virtual Cache<string, DmnModelInstance> DmnDefinitionCache
	  {
		  get
		  {
			return dmnModelInstanceCache.Cache;
		  }
	  }

	  public virtual Cache<string, DecisionDefinitionEntity> DecisionDefinitionCache
	  {
		  get
		  {
			return decisionDefinitionCache.Cache;
		  }
	  }

	  public virtual Cache<string, DecisionRequirementsDefinitionEntity> DecisionRequirementsDefinitionCache
	  {
		  get
		  {
			return decisionRequirementsDefinitionCache.Cache;
		  }
	  }

	  public virtual Cache<string, ProcessDefinitionEntity> ProcessDefinitionCache
	  {
		  get
		  {
			return processDefinitionEntityCache.Cache;
		  }
	  }

	  public virtual Cache<string, CaseDefinitionEntity> CaseDefinitionCache
	  {
		  get
		  {
			return caseDefinitionCache.Cache;
		  }
	  }

	  public virtual IList<Deployer> Deployers
	  {
		  set
		  {
			this.cacheDeployer.Deployers = value;
		  }
	  }

	  public virtual void removeDeployment(string deploymentId)
	  {
		bpmnModelInstanceCache.removeAllDefinitionsByDeploymentId(deploymentId);
		if (Context.ProcessEngineConfiguration.CmmnEnabled)
		{
		  cmmnModelInstanceCache.removeAllDefinitionsByDeploymentId(deploymentId);
		}
		if (Context.ProcessEngineConfiguration.DmnEnabled)
		{
		  dmnModelInstanceCache.removeAllDefinitionsByDeploymentId(deploymentId);
		}
		removeAllDecisionRequirementsDefinitionsByDeploymentId(deploymentId);
	  }

	  protected internal virtual void removeAllDecisionRequirementsDefinitionsByDeploymentId(string deploymentId)
	  {
		// remove all decision requirements definitions for a specific deployment

		IList<DecisionRequirementsDefinition> allDefinitionsForDeployment = (new DecisionRequirementsDefinitionQueryImpl()).deploymentId(deploymentId).list();

		foreach (DecisionRequirementsDefinition decisionRequirementsDefinition in allDefinitionsForDeployment)
		{
		  try
		  {
			removeDecisionDefinition(decisionRequirementsDefinition.Id);
		  }
		  catch (Exception e)
		  {
			ProcessEngineLogger.PERSISTENCE_LOGGER.removeEntryFromDeploymentCacheFailure("decision requirement", decisionRequirementsDefinition.Id, e);
		  }
		}
	  }

	  public virtual CachePurgeReport purgeCache()
	  {

		CachePurgeReport result = new CachePurgeReport();
		Cache<string, ProcessDefinitionEntity> processDefinitionCache = ProcessDefinitionCache;
		if (!processDefinitionCache.Empty)
		{
		  result.addPurgeInformation(CachePurgeReport.PROCESS_DEF_CACHE, processDefinitionCache.Keys);
		  processDefinitionCache.clear();
		}

		Cache<string, BpmnModelInstance> bpmnModelInstanceCache = BpmnModelInstanceCache;
		if (!bpmnModelInstanceCache.Empty)
		{
		  result.addPurgeInformation(CachePurgeReport.BPMN_MODEL_INST_CACHE, bpmnModelInstanceCache.Keys);
		  bpmnModelInstanceCache.clear();
		}

		Cache<string, CaseDefinitionEntity> caseDefinitionCache = CaseDefinitionCache;
		if (!caseDefinitionCache.Empty)
		{
		  result.addPurgeInformation(CachePurgeReport.CASE_DEF_CACHE, caseDefinitionCache.Keys);
		  caseDefinitionCache.clear();
		}

		Cache<string, CmmnModelInstance> cmmnModelInstanceCache = CmmnModelInstanceCache;
		if (!cmmnModelInstanceCache.Empty)
		{
		  result.addPurgeInformation(CachePurgeReport.CASE_MODEL_INST_CACHE, cmmnModelInstanceCache.Keys);
		  cmmnModelInstanceCache.clear();
		}

		Cache<string, DecisionDefinitionEntity> decisionDefinitionCache = DecisionDefinitionCache;
		if (!decisionDefinitionCache.Empty)
		{
		  result.addPurgeInformation(CachePurgeReport.DMN_DEF_CACHE, decisionDefinitionCache.Keys);
		  decisionDefinitionCache.clear();
		}

		Cache<string, DmnModelInstance> dmnModelInstanceCache = DmnDefinitionCache;
		if (!dmnModelInstanceCache.Empty)
		{
		  result.addPurgeInformation(CachePurgeReport.DMN_MODEL_INST_CACHE, dmnModelInstanceCache.Keys);
		  dmnModelInstanceCache.clear();
		}

		Cache<string, DecisionRequirementsDefinitionEntity> decisionRequirementsDefinitionCache = DecisionRequirementsDefinitionCache;
		if (!decisionRequirementsDefinitionCache.Empty)
		{
		  result.addPurgeInformation(CachePurgeReport.DMN_REQ_DEF_CACHE, decisionRequirementsDefinitionCache.Keys);
		  decisionRequirementsDefinitionCache.clear();
		}

		return result;
	  }

	}

}