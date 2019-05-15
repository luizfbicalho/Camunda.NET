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
namespace org.camunda.bpm.engine.impl.migration.instance.parser
{
	using IncidentEntity = org.camunda.bpm.engine.impl.persistence.entity.IncidentEntity;
	using JobDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.JobDefinitionEntity;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class IncidentInstanceHandler : MigratingInstanceParseHandler<IncidentEntity>
	{

	  public virtual void handle(MigratingInstanceParseContext parseContext, IncidentEntity incident)
	  {
		if (!string.ReferenceEquals(incident.Configuration, null) && isFailedJobIncident(incident))
		{
		  handleFailedJobIncident(parseContext, incident);
		}
		else if (!string.ReferenceEquals(incident.Configuration, null) && isExternalTaskIncident(incident))
		{
		  handleExternalTaskIncident(parseContext, incident);
		}
		else
		{
		  handleIncident(parseContext, incident);
		}
	  }

	  protected internal virtual void handleIncident(MigratingInstanceParseContext parseContext, IncidentEntity incident)
	  {
		MigratingActivityInstance owningInstance = parseContext.getMigratingActivityInstanceById(incident.Execution.ActivityInstanceId);
		if (owningInstance != null)
		{
		  parseContext.consume(incident);
		  MigratingIncident migratingIncident = new MigratingIncident(incident, owningInstance.TargetScope);
		  owningInstance.addMigratingDependentInstance(migratingIncident);
		}
	  }

	  protected internal virtual bool isFailedJobIncident(IncidentEntity incident)
	  {
		return IncidentEntity.FAILED_JOB_HANDLER_TYPE.Equals(incident.IncidentType);
	  }

	  protected internal virtual void handleFailedJobIncident(MigratingInstanceParseContext parseContext, IncidentEntity incident)
	  {
		MigratingJobInstance owningInstance = parseContext.getMigratingJobInstanceById(incident.Configuration);
		if (owningInstance != null)
		{
		  parseContext.consume(incident);
		  if (owningInstance.migrates())
		  {
			MigratingIncident migratingIncident = new MigratingIncident(incident, owningInstance.TargetScope);
			JobDefinitionEntity targetJobDefinitionEntity = owningInstance.TargetJobDefinitionEntity;
			if (targetJobDefinitionEntity != null)
			{
			  migratingIncident.TargetJobDefinitionId = targetJobDefinitionEntity.Id;
			}
			owningInstance.addMigratingDependentInstance(migratingIncident);
		  }
		}
	  }

	  protected internal virtual bool isExternalTaskIncident(IncidentEntity incident)
	  {
		return IncidentEntity.EXTERNAL_TASK_HANDLER_TYPE.Equals(incident.IncidentType);
	  }

	  protected internal virtual void handleExternalTaskIncident(MigratingInstanceParseContext parseContext, IncidentEntity incident)
	  {
		MigratingExternalTaskInstance owningInstance = parseContext.getMigratingExternalTaskInstanceById(incident.Configuration);
		if (owningInstance != null)
		{
		  parseContext.consume(incident);
		  MigratingIncident migratingIncident = new MigratingIncident(incident, owningInstance.TargetScope);
		  owningInstance.addMigratingDependentInstance(migratingIncident);
		}
	  }

	}

}