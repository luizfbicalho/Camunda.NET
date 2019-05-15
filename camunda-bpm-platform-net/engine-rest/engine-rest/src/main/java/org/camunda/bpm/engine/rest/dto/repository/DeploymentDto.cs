﻿using System;

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

	public class DeploymentDto : LinkableDto
	{

	  protected internal string id;
	  protected internal string name;
	  protected internal string source;
	  protected internal DateTime deploymentTime;
	  protected internal string tenantId;

	  public DeploymentDto()
	  {
	  }

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public virtual string Source
	  {
		  get
		  {
			return source;
		  }
	  }

	  public virtual DateTime DeploymentTime
	  {
		  get
		  {
			return deploymentTime;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
	  }

	  public static DeploymentDto fromDeployment(Deployment deployment)
	  {
		DeploymentDto dto = new DeploymentDto();
		dto.id = deployment.Id;
		dto.name = deployment.Name;
		dto.source = deployment.Source;
		dto.deploymentTime = deployment.DeploymentTime;
		dto.tenantId = deployment.TenantId;
		return dto;
	  }

	}

}