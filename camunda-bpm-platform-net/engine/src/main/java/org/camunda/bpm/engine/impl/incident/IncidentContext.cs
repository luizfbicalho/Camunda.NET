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
namespace org.camunda.bpm.engine.impl.incident
{
	using Incident = org.camunda.bpm.engine.runtime.Incident;

	/// <summary>
	/// The context of an <seealso cref="Incident"/>.
	/// </summary>
	public class IncidentContext
	{

	  protected internal string processDefinitionId;
	  protected internal string activityId;
	  protected internal string executionId;
	  protected internal string configuration;
	  protected internal string tenantId;
	  protected internal string jobDefinitionId;

	  public IncidentContext()
	  {
	  }

	  public IncidentContext(Incident incident)
	  {
		this.processDefinitionId = incident.ProcessDefinitionId;
		this.activityId = incident.ActivityId;
		this.executionId = incident.ExecutionId;
		this.configuration = incident.Configuration;
		this.tenantId = incident.TenantId;
		this.jobDefinitionId = incident.JobDefinitionId;
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId;
		  }
		  set
		  {
			this.processDefinitionId = value;
		  }
	  }


	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId;
		  }
		  set
		  {
			this.activityId = value;
		  }
	  }


	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId;
		  }
		  set
		  {
			this.executionId = value;
		  }
	  }


	  public virtual string Configuration
	  {
		  get
		  {
			return configuration;
		  }
		  set
		  {
			this.configuration = value;
		  }
	  }


	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
		  set
		  {
			this.tenantId = value;
		  }
	  }


	  public virtual string JobDefinitionId
	  {
		  get
		  {
			return jobDefinitionId;
		  }
		  set
		  {
			this.jobDefinitionId = value;
		  }
	  }


	}

}