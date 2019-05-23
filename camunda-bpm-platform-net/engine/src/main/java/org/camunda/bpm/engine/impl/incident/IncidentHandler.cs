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
	/// The <seealso cref="IncidentHandler"/> interface may be implemented by components
	/// that handle and resolve incidents of a specific type that occur during the
	/// execution of a process instance.
	/// 
	/// <para>
	/// 
	/// Custom implementations of this interface may be wired through
	/// <seealso cref="org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl.setCustomIncidentHandlers(System.Collections.IList)"/>.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= FailedJobIncidentHandler </seealso>
	/// <seealso cref= org.camunda.bpm.engine.runtime.Incident
	/// 
	/// @author roman.smirnov </seealso>
	public interface IncidentHandler
	{

	  /// <summary>
	  /// Returns the incident type this handler activates for.
	  /// </summary>
	  string IncidentHandlerType {get;}

	  /// <summary>
	  /// Handle an incident that arose in the context of an execution.
	  /// </summary>
	  Incident handleIncident(IncidentContext context, string message);

	  /// <summary>
	  /// Called in situations in which an incidenthandler may wich to resolve existing incidents
	  /// The implementation receives this callback to enable it to resolve any open incidents that
	  /// may exist.
	  /// </summary>
	  void resolveIncident(IncidentContext context);

	  /// <summary>
	  /// Called in situations in which an incidenthandler may wich to delete existing incidents
	  /// Example: when a scope is ended or a job is deleted. The implementation receives
	  /// this callback to enable it to delete any open incidents that may exist.
	  /// </summary>
	  void deleteIncident(IncidentContext context);

	}


}