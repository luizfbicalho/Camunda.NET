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
namespace org.camunda.bpm.engine.cdi.impl
{


	/// <summary>
	/// Makes the managed process engine and the provided services available for injection
	/// 
	/// @author Daniel Meyer
	/// @author Falko Menge
	/// </summary>
	public class ProcessEngineServicesProducer
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @Named @ApplicationScoped public org.camunda.bpm.engine.ProcessEngine processEngine()
	  public virtual ProcessEngine processEngine()
	  {

		ProcessEngine processEngine = BpmPlatform.ProcessEngineService.DefaultProcessEngine;
		if (processEngine != null)
		{
		  return processEngine;
		}
		else
		{
		  IList<ProcessEngine> processEngines = BpmPlatform.ProcessEngineService.ProcessEngines;
		  if (processEngines != null && processEngines.Count == 1)
		  {
			return processEngines[0];
		  }
		  else
		  {
			return ProcessEngines.getDefaultProcessEngine(false);
		  }
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @Named @ApplicationScoped public org.camunda.bpm.engine.RuntimeService runtimeService()
	  public virtual RuntimeService runtimeService()
	  {
		  return processEngine().RuntimeService;
	  }
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @Named @ApplicationScoped public org.camunda.bpm.engine.TaskService taskService()
	  public virtual TaskService taskService()
	  {
		  return processEngine().TaskService;
	  }
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @Named @ApplicationScoped public org.camunda.bpm.engine.RepositoryService repositoryService()
	  public virtual RepositoryService repositoryService()
	  {
		  return processEngine().RepositoryService;
	  }
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @Named @ApplicationScoped public org.camunda.bpm.engine.FormService formService()
	  public virtual FormService formService()
	  {
		  return processEngine().FormService;
	  }
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @Named @ApplicationScoped public org.camunda.bpm.engine.HistoryService historyService()
	  public virtual HistoryService historyService()
	  {
		  return processEngine().HistoryService;
	  }
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @Named @ApplicationScoped public org.camunda.bpm.engine.IdentityService identityService()
	  public virtual IdentityService identityService()
	  {
		  return processEngine().IdentityService;
	  }
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @Named @ApplicationScoped public org.camunda.bpm.engine.ManagementService managementService()
	  public virtual ManagementService managementService()
	  {
		  return processEngine().ManagementService;
	  }
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @Named @ApplicationScoped public org.camunda.bpm.engine.AuthorizationService authorizationService()
	  public virtual AuthorizationService authorizationService()
	  {
		  return processEngine().AuthorizationService;
	  }
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @Named @ApplicationScoped public org.camunda.bpm.engine.FilterService filterService()
	  public virtual FilterService filterService()
	  {
		  return processEngine().FilterService;
	  }
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @Named @ApplicationScoped public org.camunda.bpm.engine.ExternalTaskService externalTaskService()
	  public virtual ExternalTaskService externalTaskService()
	  {
		  return processEngine().ExternalTaskService;
	  }
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @Named @ApplicationScoped public org.camunda.bpm.engine.CaseService caseService()
	  public virtual CaseService caseService()
	  {
		  return processEngine().CaseService;
	  }
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @Named @ApplicationScoped public org.camunda.bpm.engine.DecisionService decisionService()
	  public virtual DecisionService decisionService()
	  {
		  return processEngine().DecisionService;
	  }

	}

}