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
namespace org.camunda.bpm.application
{

	using ManagementService = org.camunda.bpm.engine.ManagementService;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;


	/// <summary>
	/// <para>Represents a registration of a process application with a process engine</para>
	/// 
	/// @author Daniel Meyer
	/// </summary>
	/// <seealso cref= ManagementService#registerProcessApplication(String, ProcessApplicationReference)
	///  </seealso>
	public interface ProcessApplicationRegistration
	{

	  /// <returns> the id of the <seealso cref="Deployment"/> for which the registration was created </returns>
	  ISet<string> DeploymentIds {get;}

	  /// <returns> the name of the process engine to which the deployment was made </returns>
	  string ProcessEngineName {get;}


	}

}