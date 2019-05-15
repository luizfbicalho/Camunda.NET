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
namespace org.camunda.bpm.engine.rest.dto.repository
{
	using org.camunda.bpm.engine.repository;


	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class DeploymentWithDefinitionsDto : DeploymentDto
	{

	  protected internal IDictionary<string, ProcessDefinitionDto> deployedProcessDefinitions;
	  protected internal IDictionary<string, CaseDefinitionDto> deployedCaseDefinitions;
	  protected internal IDictionary<string, DecisionDefinitionDto> deployedDecisionDefinitions;
	  protected internal IDictionary<string, DecisionRequirementsDefinitionDto> deployedDecisionRequirementsDefinitions;

	  public virtual IDictionary<string, ProcessDefinitionDto> DeployedProcessDefinitions
	  {
		  get
		  {
			return deployedProcessDefinitions;
		  }
	  }

	  public virtual IDictionary<string, CaseDefinitionDto> DeployedCaseDefinitions
	  {
		  get
		  {
			return deployedCaseDefinitions;
		  }
	  }

	  public virtual IDictionary<string, DecisionDefinitionDto> DeployedDecisionDefinitions
	  {
		  get
		  {
			return deployedDecisionDefinitions;
		  }
	  }

	  public virtual IDictionary<string, DecisionRequirementsDefinitionDto> DeployedDecisionRequirementsDefinitions
	  {
		  get
		  {
			return deployedDecisionRequirementsDefinitions;
		  }
	  }

	  public static DeploymentWithDefinitionsDto fromDeployment(DeploymentWithDefinitions deployment)
	  {
		DeploymentWithDefinitionsDto dto = new DeploymentWithDefinitionsDto();
		dto.id = deployment.Id;
		dto.name = deployment.Name;
		dto.source = deployment.Source;
		dto.deploymentTime = deployment.DeploymentTime;
		dto.tenantId = deployment.TenantId;

		initDeployedResourceLists(deployment, dto);

		return dto;
	  }

	  private static void initDeployedResourceLists(DeploymentWithDefinitions deployment, DeploymentWithDefinitionsDto dto)
	  {
		IList<ProcessDefinition> deployedProcessDefinitions = deployment.DeployedProcessDefinitions;
		if (deployedProcessDefinitions != null)
		{
		  dto.deployedProcessDefinitions = new Dictionary<string, ProcessDefinitionDto>();
		  foreach (ProcessDefinition processDefinition in deployedProcessDefinitions)
		  {
			dto.deployedProcessDefinitions[processDefinition.Id] = ProcessDefinitionDto.fromProcessDefinition(processDefinition);
		  }
		}

		IList<CaseDefinition> deployedCaseDefinitions = deployment.DeployedCaseDefinitions;
		if (deployedCaseDefinitions != null)
		{
		  dto.deployedCaseDefinitions = new Dictionary<string, CaseDefinitionDto>();
		  foreach (CaseDefinition caseDefinition in deployedCaseDefinitions)
		  {
			dto.deployedCaseDefinitions[caseDefinition.Id] = CaseDefinitionDto.fromCaseDefinition(caseDefinition);
		  }
		}

		IList<DecisionDefinition> deployedDecisionDefinitions = deployment.DeployedDecisionDefinitions;
		if (deployedDecisionDefinitions != null)
		{
		  dto.deployedDecisionDefinitions = new Dictionary<string, DecisionDefinitionDto>();
		  foreach (DecisionDefinition decisionDefinition in deployedDecisionDefinitions)
		  {
			dto.deployedDecisionDefinitions[decisionDefinition.Id] = DecisionDefinitionDto.fromDecisionDefinition(decisionDefinition);
		  }
		}

		IList<DecisionRequirementsDefinition> deployedDecisionRequirementsDefinitions = deployment.DeployedDecisionRequirementsDefinitions;
		if (deployedDecisionRequirementsDefinitions != null)
		{
		  dto.deployedDecisionRequirementsDefinitions = new Dictionary<string, DecisionRequirementsDefinitionDto>();
		  foreach (DecisionRequirementsDefinition drd in deployedDecisionRequirementsDefinitions)
		  {
			dto.deployedDecisionRequirementsDefinitions[drd.Id] = DecisionRequirementsDefinitionDto.fromDecisionRequirementsDefinition(drd);
		  }
		}
	  }
	}

}