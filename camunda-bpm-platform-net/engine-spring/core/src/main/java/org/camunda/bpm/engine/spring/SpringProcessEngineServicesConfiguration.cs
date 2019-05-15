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
namespace org.camunda.bpm.engine.spring
{
	using Autowired = org.springframework.beans.factory.annotation.Autowired;
	using Bean = org.springframework.context.annotation.Bean;
	using Configuration = org.springframework.context.annotation.Configuration;

	/// <summary>
	/// Exposes all camunda process engine services as beans.
	/// 
	/// @author Jan Galinski
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Configuration public class SpringProcessEngineServicesConfiguration implements org.camunda.bpm.engine.ProcessEngineServices
	public class SpringProcessEngineServicesConfiguration : ProcessEngineServices
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Autowired private org.camunda.bpm.engine.ProcessEngine processEngine;
		private ProcessEngine processEngine;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Bean(name = "runtimeService") @Override public org.camunda.bpm.engine.RuntimeService getRuntimeService()
	  public virtual RuntimeService RuntimeService
	  {
		  get
		  {
			return processEngine.RuntimeService;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Bean(name = "repositoryService") @Override public org.camunda.bpm.engine.RepositoryService getRepositoryService()
	  public virtual RepositoryService RepositoryService
	  {
		  get
		  {
			return processEngine.RepositoryService;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Bean(name = "formService") @Override public org.camunda.bpm.engine.FormService getFormService()
	  public virtual FormService FormService
	  {
		  get
		  {
			return processEngine.FormService;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Bean(name = "taskService") @Override public org.camunda.bpm.engine.TaskService getTaskService()
	  public virtual TaskService TaskService
	  {
		  get
		  {
			return processEngine.TaskService;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Bean(name = "historyService") @Override public org.camunda.bpm.engine.HistoryService getHistoryService()
	  public virtual HistoryService HistoryService
	  {
		  get
		  {
			return processEngine.HistoryService;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Bean(name = "identityService") @Override public org.camunda.bpm.engine.IdentityService getIdentityService()
	  public virtual IdentityService IdentityService
	  {
		  get
		  {
			return processEngine.IdentityService;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Bean(name = "managementService") @Override public org.camunda.bpm.engine.ManagementService getManagementService()
	  public virtual ManagementService ManagementService
	  {
		  get
		  {
			return processEngine.ManagementService;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Bean(name = "authorizationService") @Override public org.camunda.bpm.engine.AuthorizationService getAuthorizationService()
	  public virtual AuthorizationService AuthorizationService
	  {
		  get
		  {
			return processEngine.AuthorizationService;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Bean(name = "caseService") @Override public org.camunda.bpm.engine.CaseService getCaseService()
	  public virtual CaseService CaseService
	  {
		  get
		  {
			return processEngine.CaseService;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Bean(name = "filterService") @Override public org.camunda.bpm.engine.FilterService getFilterService()
	  public virtual FilterService FilterService
	  {
		  get
		  {
			return processEngine.FilterService;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Bean(name = "externalTaskService") @Override public org.camunda.bpm.engine.ExternalTaskService getExternalTaskService()
	  public virtual ExternalTaskService ExternalTaskService
	  {
		  get
		  {
			return processEngine.ExternalTaskService;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Bean(name = "decisionService") @Override public org.camunda.bpm.engine.DecisionService getDecisionService()
	  public virtual DecisionService DecisionService
	  {
		  get
		  {
			return processEngine.DecisionService;
		  }
	  }

	}

}