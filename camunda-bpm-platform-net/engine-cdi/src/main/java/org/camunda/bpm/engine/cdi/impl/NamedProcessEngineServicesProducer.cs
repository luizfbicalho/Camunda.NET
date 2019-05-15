using System;

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

	using ProcessEngineName = org.camunda.bpm.engine.cdi.annotation.ProcessEngineName;


	/// <summary>
	/// This bean provides producers for the process engine services such
	/// that the injection point can choose the process engine it wants to
	/// inject by its name:
	/// 
	/// @Inject
	/// @ProcessEngineName("second-engine")
	/// private RuntimeService runtimeService;
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public class NamedProcessEngineServicesProducer
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @ProcessEngineName("") public org.camunda.bpm.engine.ProcessEngine processEngine(javax.enterprise.inject.spi.InjectionPoint ip)
	  [ProcessEngineName("")]
	  public virtual ProcessEngine processEngine(InjectionPoint ip)
	  {

		ProcessEngineName annotation = ip.Annotated.getAnnotation(typeof(ProcessEngineName));
		string processEngineName = annotation.value();
		if (string.ReferenceEquals(processEngineName, null) || processEngineName.Length == 0)
		{
		 throw new ProcessEngineException("Cannot determine which process engine to inject: @ProcessEngineName must specify the name of a process engine.");
		}
		try
		{
		  ProcessEngineService processEngineService = BpmPlatform.ProcessEngineService;
		  return processEngineService.getProcessEngine(processEngineName);
		}
		catch (Exception e)
		{
		  throw new ProcessEngineException("Cannot find process engine named '" + processEngineName + "' specified using @ProcessEngineName: " + e.Message, e);
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @ProcessEngineName("") public org.camunda.bpm.engine.RuntimeService runtimeService(javax.enterprise.inject.spi.InjectionPoint ip)
	  [ProcessEngineName("")]
	  public virtual RuntimeService runtimeService(InjectionPoint ip)
	  {
		  return processEngine(ip).RuntimeService;
	  }
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @ProcessEngineName("") public org.camunda.bpm.engine.TaskService taskService(javax.enterprise.inject.spi.InjectionPoint ip)
	  [ProcessEngineName("")]
	  public virtual TaskService taskService(InjectionPoint ip)
	  {
		  return processEngine(ip).TaskService;
	  }
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @ProcessEngineName("") public org.camunda.bpm.engine.RepositoryService repositoryService(javax.enterprise.inject.spi.InjectionPoint ip)
	  [ProcessEngineName("")]
	  public virtual RepositoryService repositoryService(InjectionPoint ip)
	  {
		  return processEngine(ip).RepositoryService;
	  }
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @ProcessEngineName("") public org.camunda.bpm.engine.FormService formService(javax.enterprise.inject.spi.InjectionPoint ip)
	  [ProcessEngineName("")]
	  public virtual FormService formService(InjectionPoint ip)
	  {
		  return processEngine(ip).FormService;
	  }
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @ProcessEngineName("") public org.camunda.bpm.engine.HistoryService historyService(javax.enterprise.inject.spi.InjectionPoint ip)
	  [ProcessEngineName("")]
	  public virtual HistoryService historyService(InjectionPoint ip)
	  {
		  return processEngine(ip).HistoryService;
	  }
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @ProcessEngineName("") public org.camunda.bpm.engine.IdentityService identityService(javax.enterprise.inject.spi.InjectionPoint ip)
	  [ProcessEngineName("")]
	  public virtual IdentityService identityService(InjectionPoint ip)
	  {
		  return processEngine(ip).IdentityService;
	  }
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @ProcessEngineName("") public org.camunda.bpm.engine.ManagementService managementService(javax.enterprise.inject.spi.InjectionPoint ip)
	  [ProcessEngineName("")]
	  public virtual ManagementService managementService(InjectionPoint ip)
	  {
		  return processEngine(ip).ManagementService;
	  }
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @ProcessEngineName("") public org.camunda.bpm.engine.AuthorizationService authorizationService(javax.enterprise.inject.spi.InjectionPoint ip)
	  [ProcessEngineName("")]
	  public virtual AuthorizationService authorizationService(InjectionPoint ip)
	  {
		  return processEngine(ip).AuthorizationService;
	  }
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @ProcessEngineName("") public org.camunda.bpm.engine.FilterService filterService(javax.enterprise.inject.spi.InjectionPoint ip)
	  [ProcessEngineName("")]
	  public virtual FilterService filterService(InjectionPoint ip)
	  {
		  return processEngine(ip).FilterService;
	  }
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @ProcessEngineName("") public org.camunda.bpm.engine.ExternalTaskService externalTaskService(javax.enterprise.inject.spi.InjectionPoint ip)
	  [ProcessEngineName("")]
	  public virtual ExternalTaskService externalTaskService(InjectionPoint ip)
	  {
		  return processEngine(ip).ExternalTaskService;
	  }
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @ProcessEngineName("") public org.camunda.bpm.engine.CaseService caseService(javax.enterprise.inject.spi.InjectionPoint ip)
	  [ProcessEngineName("")]
	  public virtual CaseService caseService(InjectionPoint ip)
	  {
		  return processEngine(ip).CaseService;
	  }
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @ProcessEngineName("") public org.camunda.bpm.engine.DecisionService decisionService(javax.enterprise.inject.spi.InjectionPoint ip)
	  [ProcessEngineName("")]
	  public virtual DecisionService decisionService(InjectionPoint ip)
	  {
		  return processEngine(ip).DecisionService;
	  }

	}

}