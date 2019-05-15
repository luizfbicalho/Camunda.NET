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
namespace org.camunda.bpm.engine.impl.incident
{

	using Context = org.camunda.bpm.engine.impl.context.Context;
	using IncidentEntity = org.camunda.bpm.engine.impl.persistence.entity.IncidentEntity;
	using Incident = org.camunda.bpm.engine.runtime.Incident;

	/// <summary>
	/// <para>
	/// An incident handler that logs incidents of a certain type
	/// as instances of <seealso cref="Incident"/> to the engine database.</para>
	/// 
	/// <para>
	/// By default, the process engine has two default handlers:
	/// <ul>
	/// <li>type <code>failedJob</code>: Indicates jobs without retries left. This incident handler is active by default and must be disabled
	/// via <seealso cref="org.camunda.bpm.engine.ProcessEngineConfiguration#setCreateIncidentOnFailedJobEnabled(boolean)"/>.
	/// <li>type <code>failedExternalTask</code>: Indicates external tasks without retries left
	/// </para>
	/// </summary>
	/// <seealso cref= IncidentHandler
	/// 
	/// @author nico.rehwaldt
	/// @author roman.smirnov
	/// @author Falko Menge
	/// @author Thorben Lindhauer </seealso>
	public class DefaultIncidentHandler : IncidentHandler
	{

	  protected internal string type;

	  public DefaultIncidentHandler(string type)
	  {
		this.type = type;
	  }

	  public virtual string IncidentHandlerType
	  {
		  get
		  {
			return type;
		  }
	  }

	  public virtual Incident handleIncident(IncidentContext context, string message)
	  {
		return createIncident(context, message);
	  }

	  public virtual Incident createIncident(IncidentContext context, string message)
	  {
		IncidentEntity newIncident = IncidentEntity.createAndInsertIncident(type, context, message);

		if (!string.ReferenceEquals(context.ExecutionId, null))
		{
		  newIncident.createRecursiveIncidents();
		}

		return newIncident;
	  }

	  public virtual void resolveIncident(IncidentContext context)
	  {
		removeIncident(context, true);
	  }

	  public virtual void deleteIncident(IncidentContext context)
	  {
		removeIncident(context, false);
	  }

	  protected internal virtual void removeIncident(IncidentContext context, bool incidentResolved)
	  {
		IList<Incident> incidents = Context.CommandContext.IncidentManager.findIncidentByConfiguration(context.Configuration);

		foreach (Incident currentIncident in incidents)
		{
		  IncidentEntity incident = (IncidentEntity) currentIncident;
		  if (incidentResolved)
		  {
			incident.resolve();
		  }
		  else
		  {
			incident.delete();
		  }
		}
	  }
	}

}