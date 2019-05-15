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
namespace org.camunda.bpm.engine.impl.persistence.entity
{

	using ProcessApplicationRegistration = org.camunda.bpm.application.ProcessApplicationRegistration;
	using org.camunda.bpm.engine.repository;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ProcessApplicationDeploymentImpl : ProcessApplicationDeployment
	{

	  protected internal DeploymentWithDefinitions deployment;
	  protected internal ProcessApplicationRegistration registration;

	  public ProcessApplicationDeploymentImpl(DeploymentWithDefinitions deployment, ProcessApplicationRegistration registration)
	  {
		this.deployment = deployment;
		this.registration = registration;
	  }

	  public virtual string Id
	  {
		  get
		  {
			return deployment.Id;
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			return deployment.Name;
		  }
	  }

	  public virtual DateTime DeploymentTime
	  {
		  get
		  {
			return deployment.DeploymentTime;
		  }
	  }

	  public virtual string Source
	  {
		  get
		  {
			return deployment.Source;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return deployment.TenantId;
		  }
	  }

	  public virtual ProcessApplicationRegistration ProcessApplicationRegistration
	  {
		  get
		  {
			return registration;
		  }
	  }

	  public virtual IList<ProcessDefinition> DeployedProcessDefinitions
	  {
		  get
		  {
			return deployment.DeployedProcessDefinitions;
		  }
	  }

	  public virtual IList<CaseDefinition> DeployedCaseDefinitions
	  {
		  get
		  {
			return deployment.DeployedCaseDefinitions;
		  }
	  }

	  public virtual IList<DecisionDefinition> DeployedDecisionDefinitions
	  {
		  get
		  {
			return deployment.DeployedDecisionDefinitions;
		  }
	  }

	  public virtual IList<DecisionRequirementsDefinition> DeployedDecisionRequirementsDefinitions
	  {
		  get
		  {
			return deployment.DeployedDecisionRequirementsDefinitions;
		  }
	  }
	}

}