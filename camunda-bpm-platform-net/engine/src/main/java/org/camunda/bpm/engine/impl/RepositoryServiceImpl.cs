using System;
using System.Collections.Generic;
using System.IO;

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

	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using DeploymentResourceNotFoundException = org.camunda.bpm.engine.exception.DeploymentResourceNotFoundException;
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using CaseDefinitionNotFoundException = org.camunda.bpm.engine.exception.cmmn.CaseDefinitionNotFoundException;
	using CmmnModelInstanceNotFoundException = org.camunda.bpm.engine.exception.cmmn.CmmnModelInstanceNotFoundException;
	using DecisionDefinitionNotFoundException = org.camunda.bpm.engine.exception.dmn.DecisionDefinitionNotFoundException;
	using DmnModelInstanceNotFoundException = org.camunda.bpm.engine.exception.dmn.DmnModelInstanceNotFoundException;
	using AddIdentityLinkForProcessDefinitionCmd = org.camunda.bpm.engine.impl.cmd.AddIdentityLinkForProcessDefinitionCmd;
	using DeleteDeploymentCmd = org.camunda.bpm.engine.impl.cmd.DeleteDeploymentCmd;
	using DeleteIdentityLinkForProcessDefinitionCmd = org.camunda.bpm.engine.impl.cmd.DeleteIdentityLinkForProcessDefinitionCmd;
	using DeployCmd = org.camunda.bpm.engine.impl.cmd.DeployCmd;
	using GetDeployedProcessDefinitionCmd = org.camunda.bpm.engine.impl.cmd.GetDeployedProcessDefinitionCmd;
	using GetDeploymentBpmnModelInstanceCmd = org.camunda.bpm.engine.impl.cmd.GetDeploymentBpmnModelInstanceCmd;
	using GetDeploymentProcessDiagramCmd = org.camunda.bpm.engine.impl.cmd.GetDeploymentProcessDiagramCmd;
	using GetDeploymentProcessDiagramLayoutCmd = org.camunda.bpm.engine.impl.cmd.GetDeploymentProcessDiagramLayoutCmd;
	using GetDeploymentProcessModelCmd = org.camunda.bpm.engine.impl.cmd.GetDeploymentProcessModelCmd;
	using GetDeploymentResourceCmd = org.camunda.bpm.engine.impl.cmd.GetDeploymentResourceCmd;
	using GetDeploymentResourceForIdCmd = org.camunda.bpm.engine.impl.cmd.GetDeploymentResourceForIdCmd;
	using GetDeploymentResourceNamesCmd = org.camunda.bpm.engine.impl.cmd.GetDeploymentResourceNamesCmd;
	using GetDeploymentResourcesCmd = org.camunda.bpm.engine.impl.cmd.GetDeploymentResourcesCmd;
	using GetIdentityLinksForProcessDefinitionCmd = org.camunda.bpm.engine.impl.cmd.GetIdentityLinksForProcessDefinitionCmd;
	using UpdateDecisionDefinitionHistoryTimeToLiveCmd = org.camunda.bpm.engine.impl.cmd.UpdateDecisionDefinitionHistoryTimeToLiveCmd;
	using UpdateProcessDefinitionHistoryTimeToLiveCmd = org.camunda.bpm.engine.impl.cmd.UpdateProcessDefinitionHistoryTimeToLiveCmd;
	using GetDeploymentCaseDefinitionCmd = org.camunda.bpm.engine.impl.cmmn.cmd.GetDeploymentCaseDefinitionCmd;
	using GetDeploymentCaseDiagramCmd = org.camunda.bpm.engine.impl.cmmn.cmd.GetDeploymentCaseDiagramCmd;
	using GetDeploymentCaseModelCmd = org.camunda.bpm.engine.impl.cmmn.cmd.GetDeploymentCaseModelCmd;
	using GetDeploymentCmmnModelInstanceCmd = org.camunda.bpm.engine.impl.cmmn.cmd.GetDeploymentCmmnModelInstanceCmd;
	using UpdateCaseDefinitionHistoryTimeToLiveCmd = org.camunda.bpm.engine.impl.cmmn.cmd.UpdateCaseDefinitionHistoryTimeToLiveCmd;
	using CaseDefinitionQueryImpl = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionQueryImpl;
	using GetDeploymentDecisionDefinitionCmd = org.camunda.bpm.engine.impl.dmn.cmd.GetDeploymentDecisionDefinitionCmd;
	using GetDeploymentDecisionDiagramCmd = org.camunda.bpm.engine.impl.dmn.cmd.GetDeploymentDecisionDiagramCmd;
	using GetDeploymentDecisionModelCmd = org.camunda.bpm.engine.impl.dmn.cmd.GetDeploymentDecisionModelCmd;
	using GetDeploymentDecisionRequirementsDefinitionCmd = org.camunda.bpm.engine.impl.dmn.cmd.GetDeploymentDecisionRequirementsDefinitionCmd;
	using GetDeploymentDecisionRequirementsDiagramCmd = org.camunda.bpm.engine.impl.dmn.cmd.GetDeploymentDecisionRequirementsDiagramCmd;
	using GetDeploymentDecisionRequirementsModelCmd = org.camunda.bpm.engine.impl.dmn.cmd.GetDeploymentDecisionRequirementsModelCmd;
	using GetDeploymentDmnModelInstanceCmd = org.camunda.bpm.engine.impl.dmn.cmd.GetDeploymentDmnModelInstanceCmd;
	using DecisionDefinitionQueryImpl = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionDefinitionQueryImpl;
	using DecisionRequirementsDefinitionQueryImpl = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionRequirementsDefinitionQueryImpl;
	using ReadOnlyProcessDefinition = org.camunda.bpm.engine.impl.pvm.ReadOnlyProcessDefinition;
	using DeleteProcessDefinitionsBuilderImpl = org.camunda.bpm.engine.impl.repository.DeleteProcessDefinitionsBuilderImpl;
	using DeploymentBuilderImpl = org.camunda.bpm.engine.impl.repository.DeploymentBuilderImpl;
	using ProcessApplicationDeploymentBuilderImpl = org.camunda.bpm.engine.impl.repository.ProcessApplicationDeploymentBuilderImpl;
	using UpdateProcessDefinitionSuspensionStateBuilderImpl = org.camunda.bpm.engine.impl.repository.UpdateProcessDefinitionSuspensionStateBuilderImpl;
	using org.camunda.bpm.engine.repository;
	using IdentityLink = org.camunda.bpm.engine.task.IdentityLink;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using CmmnModelInstance = org.camunda.bpm.model.cmmn.CmmnModelInstance;
	using DmnModelInstance = org.camunda.bpm.model.dmn.DmnModelInstance;

	/// <summary>
	/// @author Tom Baeyens
	/// @author Falko Menge
	/// @author Joram Barrez
	/// </summary>
	public class RepositoryServiceImpl : ServiceImpl, RepositoryService
	{

	  protected internal Charset deploymentCharset;

	  public virtual Charset DeploymentCharset
	  {
		  get
		  {
			return deploymentCharset;
		  }
		  set
		  {
			this.deploymentCharset = value;
		  }
	  }


	  public virtual DeploymentBuilder createDeployment()
	  {
		return new DeploymentBuilderImpl(this);
	  }

	  public virtual ProcessApplicationDeploymentBuilder createDeployment(ProcessApplicationReference processApplication)
	  {
		return new ProcessApplicationDeploymentBuilderImpl(this, processApplication);
	  }

	  public virtual DeploymentWithDefinitions deployWithResult(DeploymentBuilderImpl deploymentBuilder)
	  {
		return commandExecutor.execute(new DeployCmd(deploymentBuilder));
	  }

	  public virtual void deleteDeployment(string deploymentId)
	  {
		commandExecutor.execute(new DeleteDeploymentCmd(deploymentId, false, false, false));
	  }

	  public virtual void deleteDeploymentCascade(string deploymentId)
	  {
		commandExecutor.execute(new DeleteDeploymentCmd(deploymentId, true, false, false));
	  }

	  public virtual void deleteDeployment(string deploymentId, bool cascade)
	  {
		commandExecutor.execute(new DeleteDeploymentCmd(deploymentId, cascade, false, false));
	  }

	  public virtual void deleteDeployment(string deploymentId, bool cascade, bool skipCustomListeners)
	  {
		commandExecutor.execute(new DeleteDeploymentCmd(deploymentId, cascade, skipCustomListeners, false));
	  }

	  public virtual void deleteDeployment(string deploymentId, bool cascade, bool skipCustomListeners, bool skipIoMappings)
	  {
		commandExecutor.execute(new DeleteDeploymentCmd(deploymentId, cascade, skipCustomListeners, skipIoMappings));
	  }

	  public virtual void deleteProcessDefinition(string processDefinitionId)
	  {
		deleteProcessDefinition(processDefinitionId, false);
	  }

	  public virtual void deleteProcessDefinition(string processDefinitionId, bool cascade)
	  {
		deleteProcessDefinition(processDefinitionId, cascade, false);
	  }

	  public virtual void deleteProcessDefinition(string processDefinitionId, bool cascade, bool skipCustomListeners)
	  {
		deleteProcessDefinition(processDefinitionId, cascade, skipCustomListeners, false);
	  }

	  public virtual void deleteProcessDefinition(string processDefinitionId, bool cascade, bool skipCustomListeners, bool skipIoMappings)
	  {
		DeleteProcessDefinitionsBuilder builder = deleteProcessDefinitions().byIds(processDefinitionId);

		if (cascade)
		{
		  builder.cascade();
		}

		if (skipCustomListeners)
		{
		  builder.skipCustomListeners();
		}

		if (skipIoMappings)
		{
		  builder.skipIoMappings();
		}

		builder.delete();
	  }

	  public virtual DeleteProcessDefinitionsSelectBuilder deleteProcessDefinitions()
	  {
		return new DeleteProcessDefinitionsBuilderImpl(commandExecutor);
	  }

	  public virtual ProcessDefinitionQuery createProcessDefinitionQuery()
	  {
		return new ProcessDefinitionQueryImpl(commandExecutor);
	  }

	  public virtual CaseDefinitionQuery createCaseDefinitionQuery()
	  {
		return new CaseDefinitionQueryImpl(commandExecutor);
	  }

	  public virtual DecisionDefinitionQuery createDecisionDefinitionQuery()
	  {
		return new DecisionDefinitionQueryImpl(commandExecutor);
	  }

	  public virtual DecisionRequirementsDefinitionQuery createDecisionRequirementsDefinitionQuery()
	  {
		return new DecisionRequirementsDefinitionQueryImpl(commandExecutor);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<String> getDeploymentResourceNames(String deploymentId)
	  public virtual IList<string> getDeploymentResourceNames(string deploymentId)
	  {
		return commandExecutor.execute(new GetDeploymentResourceNamesCmd(deploymentId));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<Resource> getDeploymentResources(String deploymentId)
	  public virtual IList<Resource> getDeploymentResources(string deploymentId)
	  {
		return commandExecutor.execute(new GetDeploymentResourcesCmd(deploymentId));
	  }

	  public virtual Stream getResourceAsStream(string deploymentId, string resourceName)
	  {
		return commandExecutor.execute(new GetDeploymentResourceCmd(deploymentId, resourceName));
	  }

	  public virtual Stream getResourceAsStreamById(string deploymentId, string resourceId)
	  {
		return commandExecutor.execute(new GetDeploymentResourceForIdCmd(deploymentId, resourceId));
	  }

	  public virtual DeploymentQuery createDeploymentQuery()
	  {
		return new DeploymentQueryImpl(commandExecutor);
	  }

	  public virtual ProcessDefinition getProcessDefinition(string processDefinitionId)
	  {
		return commandExecutor.execute(new GetDeployedProcessDefinitionCmd(processDefinitionId, true));
	  }

	  public virtual ReadOnlyProcessDefinition getDeployedProcessDefinition(string processDefinitionId)
	  {
		return commandExecutor.execute(new GetDeployedProcessDefinitionCmd(processDefinitionId, true));
	  }

	  public virtual void suspendProcessDefinitionById(string processDefinitionId)
	  {
		updateProcessDefinitionSuspensionState().byProcessDefinitionId(processDefinitionId).suspend();
	  }

	  public virtual void suspendProcessDefinitionById(string processDefinitionId, bool suspendProcessInstances, DateTime suspensionDate)
	  {
		updateProcessDefinitionSuspensionState().byProcessDefinitionId(processDefinitionId).includeProcessInstances(suspendProcessInstances).executionDate(suspensionDate).suspend();
	  }

	  public virtual void suspendProcessDefinitionByKey(string processDefinitionKey)
	  {
		updateProcessDefinitionSuspensionState().byProcessDefinitionKey(processDefinitionKey).suspend();
	  }

	  public virtual void suspendProcessDefinitionByKey(string processDefinitionKey, bool suspendProcessInstances, DateTime suspensionDate)
	  {
		updateProcessDefinitionSuspensionState().byProcessDefinitionKey(processDefinitionKey).includeProcessInstances(suspendProcessInstances).executionDate(suspensionDate).suspend();
	  }

	  public virtual void activateProcessDefinitionById(string processDefinitionId)
	  {
		updateProcessDefinitionSuspensionState().byProcessDefinitionId(processDefinitionId).activate();
	  }

	  public virtual void activateProcessDefinitionById(string processDefinitionId, bool activateProcessInstances, DateTime activationDate)
	  {
		updateProcessDefinitionSuspensionState().byProcessDefinitionId(processDefinitionId).includeProcessInstances(activateProcessInstances).executionDate(activationDate).activate();
	  }

	  public virtual void activateProcessDefinitionByKey(string processDefinitionKey)
	  {
		updateProcessDefinitionSuspensionState().byProcessDefinitionKey(processDefinitionKey).activate();
	  }

	  public virtual void activateProcessDefinitionByKey(string processDefinitionKey, bool activateProcessInstances, DateTime activationDate)
	  {
		updateProcessDefinitionSuspensionState().byProcessDefinitionKey(processDefinitionKey).includeProcessInstances(activateProcessInstances).executionDate(activationDate).activate();
	  }

	  public virtual UpdateProcessDefinitionSuspensionStateSelectBuilder updateProcessDefinitionSuspensionState()
	  {
		return new UpdateProcessDefinitionSuspensionStateBuilderImpl(commandExecutor);
	  }

	  public virtual void updateProcessDefinitionHistoryTimeToLive(string processDefinitionId, int? historyTimeToLive)
	  {
		commandExecutor.execute(new UpdateProcessDefinitionHistoryTimeToLiveCmd(processDefinitionId, historyTimeToLive));
	  }

	  public virtual void updateDecisionDefinitionHistoryTimeToLive(string decisionDefinitionId, int? historyTimeToLive)
	  {
		commandExecutor.execute(new UpdateDecisionDefinitionHistoryTimeToLiveCmd(decisionDefinitionId, historyTimeToLive));
	  }

	  public virtual void updateCaseDefinitionHistoryTimeToLive(string caseDefinitionId, int? historyTimeToLive)
	  {
		commandExecutor.execute(new UpdateCaseDefinitionHistoryTimeToLiveCmd(caseDefinitionId, historyTimeToLive));
	  }

	  public virtual Stream getProcessModel(string processDefinitionId)
	  {
		return commandExecutor.execute(new GetDeploymentProcessModelCmd(processDefinitionId));
	  }

	  public virtual Stream getProcessDiagram(string processDefinitionId)
	  {
		return commandExecutor.execute(new GetDeploymentProcessDiagramCmd(processDefinitionId));
	  }

	  public virtual Stream getCaseDiagram(string caseDefinitionId)
	  {
		return commandExecutor.execute(new GetDeploymentCaseDiagramCmd(caseDefinitionId));
	  }

	  public virtual DiagramLayout getProcessDiagramLayout(string processDefinitionId)
	  {
		return commandExecutor.execute(new GetDeploymentProcessDiagramLayoutCmd(processDefinitionId));
	  }

	  public virtual BpmnModelInstance getBpmnModelInstance(string processDefinitionId)
	  {
		return commandExecutor.execute(new GetDeploymentBpmnModelInstanceCmd(processDefinitionId));
	  }

	  public virtual CmmnModelInstance getCmmnModelInstance(string caseDefinitionId)
	  {
		try
		{
		  return commandExecutor.execute(new GetDeploymentCmmnModelInstanceCmd(caseDefinitionId));

		}
		catch (NullValueException e)
		{
		  throw new NotValidException(e.Message, e);

		}
		catch (CmmnModelInstanceNotFoundException e)
		{
		  throw new NotFoundException(e.Message, e);

		}
		catch (DeploymentResourceNotFoundException e)
		{
		  throw new NotFoundException(e.Message, e);

		}
	  }

	  public virtual DmnModelInstance getDmnModelInstance(string decisionDefinitionId)
	  {
		try
		{
		  return commandExecutor.execute(new GetDeploymentDmnModelInstanceCmd(decisionDefinitionId));

		}
		catch (NullValueException e)
		{
		  throw new NotValidException(e.Message, e);

		}
		catch (DmnModelInstanceNotFoundException e)
		{
		  throw new NotFoundException(e.Message, e);

		}
		catch (DeploymentResourceNotFoundException e)
		{
		  throw new NotFoundException(e.Message, e);

		}
	  }

	  public virtual void addCandidateStarterUser(string processDefinitionId, string userId)
	  {
		commandExecutor.execute(new AddIdentityLinkForProcessDefinitionCmd(processDefinitionId, userId, null));
	  }

	  public virtual void addCandidateStarterGroup(string processDefinitionId, string groupId)
	  {
		commandExecutor.execute(new AddIdentityLinkForProcessDefinitionCmd(processDefinitionId, null, groupId));
	  }

	  public virtual void deleteCandidateStarterGroup(string processDefinitionId, string groupId)
	  {
		commandExecutor.execute(new DeleteIdentityLinkForProcessDefinitionCmd(processDefinitionId, null, groupId));
	  }

	  public virtual void deleteCandidateStarterUser(string processDefinitionId, string userId)
	  {
		commandExecutor.execute(new DeleteIdentityLinkForProcessDefinitionCmd(processDefinitionId, userId, null));
	  }

	  public virtual IList<IdentityLink> getIdentityLinksForProcessDefinition(string processDefinitionId)
	  {
		return commandExecutor.execute(new GetIdentityLinksForProcessDefinitionCmd(processDefinitionId));
	  }

	  public virtual CaseDefinition getCaseDefinition(string caseDefinitionId)
	  {
		try
		{
		  return commandExecutor.execute(new GetDeploymentCaseDefinitionCmd(caseDefinitionId));

		}
		catch (NullValueException e)
		{
		  throw new NotValidException(e.Message, e);

		}
		catch (CaseDefinitionNotFoundException e)
		{
		  throw new NotFoundException(e.Message, e);

		}
	  }

	  public virtual Stream getCaseModel(string caseDefinitionId)
	  {
		try
		{
		  return commandExecutor.execute(new GetDeploymentCaseModelCmd(caseDefinitionId));

		}
		catch (NullValueException e)
		{
		  throw new NotValidException(e.Message, e);

		}
		catch (CaseDefinitionNotFoundException e)
		{
		  throw new NotFoundException(e.Message, e);

		}
		catch (DeploymentResourceNotFoundException e)
		{
		  throw new NotFoundException(e.Message, e);

		}

	  }

	  public virtual DecisionDefinition getDecisionDefinition(string decisionDefinitionId)
	  {
		try
		{
		  return commandExecutor.execute(new GetDeploymentDecisionDefinitionCmd(decisionDefinitionId));
		}
		catch (NullValueException e)
		{
		  throw new NotValidException(e.Message, e);
		}
		catch (DecisionDefinitionNotFoundException e)
		{
		  throw new NotFoundException(e.Message, e);
		}
	  }

	  public virtual DecisionRequirementsDefinition getDecisionRequirementsDefinition(string decisionRequirementsDefinitionId)
	  {
		try
		{
		  return commandExecutor.execute(new GetDeploymentDecisionRequirementsDefinitionCmd(decisionRequirementsDefinitionId));
		}
		catch (NullValueException e)
		{
		  throw new NotValidException(e.Message, e);
		}
		catch (DecisionDefinitionNotFoundException e)
		{
		  throw new NotFoundException(e.Message, e);
		}
	  }

	  public virtual Stream getDecisionModel(string decisionDefinitionId)
	  {
		try
		{
		  return commandExecutor.execute(new GetDeploymentDecisionModelCmd(decisionDefinitionId));
		}
		catch (NullValueException e)
		{
		  throw new NotValidException(e.Message, e);
		}
		catch (DecisionDefinitionNotFoundException e)
		{
		  throw new NotFoundException(e.Message, e);
		}
		catch (DeploymentResourceNotFoundException e)
		{
		  throw new NotFoundException(e.Message, e);
		}
	  }

	  public virtual Stream getDecisionRequirementsModel(string decisionRequirementsDefinitionId)
	  {
		try
		{
		  return commandExecutor.execute(new GetDeploymentDecisionRequirementsModelCmd(decisionRequirementsDefinitionId));
		}
		catch (NullValueException e)
		{
		  throw new NotValidException(e.Message, e);
		}
		catch (DecisionDefinitionNotFoundException e)
		{
		  throw new NotFoundException(e.Message, e);
		}
		catch (DeploymentResourceNotFoundException e)
		{
		  throw new NotFoundException(e.Message, e);
		}
	  }

	  public virtual Stream getDecisionDiagram(string decisionDefinitionId)
	  {
		return commandExecutor.execute(new GetDeploymentDecisionDiagramCmd(decisionDefinitionId));
	  }

	  public virtual Stream getDecisionRequirementsDiagram(string decisionRequirementsDefinitionId)
	  {
		return commandExecutor.execute(new GetDeploymentDecisionRequirementsDiagramCmd(decisionRequirementsDefinitionId));
	  }
	}

}