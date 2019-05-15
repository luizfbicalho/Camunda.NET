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
namespace org.camunda.bpm
{

	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;

	/// <summary>
	/// <para>The <seealso cref="ProcessEngineService"/> provides access to the list of Managed Process Engines.</para>
	/// 
	/// <para>Users of this class may look up an instance of the service through a lookup strategy
	/// appropriate for the platform they are using (Examples: Jndi, OSGi Service Registry ...)</para>
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public interface ProcessEngineService
	{

	  /// 
	  /// <returns> the default process engine. </returns>
	  ProcessEngine DefaultProcessEngine {get;}

	  /// <returns> all <seealso cref="ProcessEngine ProcessEngines"/> managed by the camunda BPM platform. </returns>
	  IList<ProcessEngine> ProcessEngines {get;}

	  /// 
	  /// <returns> the names of all <seealso cref="ProcessEngine ProcessEngines"/> managed by the camunda BPM platform. </returns>
	  ISet<string> ProcessEngineNames {get;}

	  /// 
	  /// <returns> the <seealso cref="ProcessEngine"/> for the given name or null if no such process engine exists. </returns>
	  ProcessEngine getProcessEngine(string name);

	}

}