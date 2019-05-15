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
namespace org.camunda.bpm.engine.rest.util
{
	using ProcessApplicationInfo = org.camunda.bpm.application.ProcessApplicationInfo;
	using Authentication = org.camunda.bpm.engine.impl.identity.Authentication;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;

	public class ApplicationContextPathUtil
	{

	  public static string getApplicationPathByProcessDefinitionId(ProcessEngine engine, string processDefinitionId)
	  {
		ProcessDefinition processDefinition = engine.RepositoryService.getProcessDefinition(processDefinitionId);

		if (processDefinition == null)
		{
		  return null;
		}

		return getApplicationPathForDeployment(engine, processDefinition.DeploymentId);
	  }

	  public static string getApplicationPathByCaseDefinitionId(ProcessEngine engine, string caseDefinitionId)
	  {
		CaseDefinition caseDefinition = engine.RepositoryService.getCaseDefinition(caseDefinitionId);

		if (caseDefinition == null)
		{
		  return null;
		}

		return getApplicationPathForDeployment(engine, caseDefinition.DeploymentId);
	  }

	  public static string getApplicationPathForDeployment(ProcessEngine engine, string deploymentId)
	  {

		// get the name of the process application that made the deployment
		string processApplicationName = null;
		IdentityService identityService = engine.IdentityService;
		Authentication currentAuthentication = identityService.CurrentAuthentication;
		try
		{
		  identityService.clearAuthentication();
		  processApplicationName = engine.ManagementService.getProcessApplicationForDeployment(deploymentId);
		}
		finally
		{
		  identityService.Authentication = currentAuthentication;
		}

		if (string.ReferenceEquals(processApplicationName, null))
		{
		  // no a process application deployment
		  return null;
		}
		else
		{
		  ProcessApplicationService processApplicationService = BpmPlatform.ProcessApplicationService;
		  ProcessApplicationInfo processApplicationInfo = processApplicationService.getProcessApplicationInfo(processApplicationName);
		  return processApplicationInfo.Properties[org.camunda.bpm.application.ProcessApplicationInfo_Fields.PROP_SERVLET_CONTEXT_PATH];
		}
	  }
	}

}