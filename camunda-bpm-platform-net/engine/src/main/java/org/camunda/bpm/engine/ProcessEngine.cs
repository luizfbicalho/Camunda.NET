﻿/*
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
namespace org.camunda.bpm.engine
{
	/// <summary>
	/// Provides access to all the services that expose the BPM and workflow operations.
	/// 
	/// <ul>
	/// <li>
	/// <b><seealso cref="org.camunda.bpm.engine.RuntimeService"/>: </b> Allows the creation of
	/// <seealso cref="org.camunda.bpm.engine.repository.Deployment"/>s and the starting of and searching on
	/// <seealso cref="org.camunda.bpm.engine.runtime.ProcessInstance"/>s.</li>
	/// <li>
	/// <b><seealso cref="org.camunda.bpm.engine.TaskService"/>: </b> Exposes operations to manage human
	/// (standalone) <seealso cref="org.camunda.bpm.engine.task.Task"/>s, such as claiming, completing and
	/// assigning tasks</li>
	/// <li>
	/// <b><seealso cref="org.camunda.bpm.engine.IdentityService"/>: </b> Used for managing
	/// <seealso cref="org.camunda.bpm.engine.identity.User"/>s, <seealso cref="org.camunda.bpm.engine.identity.Group"/>s and
	/// the relations between them<</li>
	/// <li>
	/// <b><seealso cref="org.camunda.bpm.engine.ManagementService"/>: </b> Exposes engine admin and
	/// maintenance operations</li>
	///  <li>
	/// <b><seealso cref="org.camunda.bpm.engine.HistoryService"/>: </b> Service exposing information about
	/// ongoing and past process instances.</li>
	/// 
	/// <li><b><seealso cref="org.camunda.bpm.engine.AuthorizationService"/>:</b> Service allowing
	/// to manage access permissions for users and groups.</b>
	/// </ul>
	/// 
	/// 
	/// Typically, there will be only one central ProcessEngine instance needed in a
	/// end-user application. Building a ProcessEngine is done through a
	/// <seealso cref="ProcessEngineConfiguration"/> instance and is a costly operation which should be
	/// avoided. For that purpose, it is advised to store it in a static field or
	/// JNDI location (or something similar). This is a thread-safe object, so no
	/// special precautions need to be taken.
	/// 
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// @author Daniel Meyer
	/// </summary>
	public interface ProcessEngine : ProcessEngineServices
	{

	  /// <summary>
	  /// the version of the process engine library </summary>

	  /// <summary>
	  /// The name as specified in 'process-engine-name' in
	  /// the camunda.cfg.xml configuration file.
	  /// The default name for a process engine is 'default 
	  /// </summary>
	  string Name {get;}

	  void close();

	  ProcessEngineConfiguration ProcessEngineConfiguration {get;}
	}

	public static class ProcessEngine_Fields
	{
	  public const string VERSION = "fox";
	}

}