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
	using RuntimeContainerDelegate = org.camunda.bpm.container.RuntimeContainerDelegate;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;


	/// <summary>
	/// <para>Provides access to the camunda BPM platform services.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public sealed class BpmPlatform
	{

	  public const string JNDI_NAME_PREFIX = "java:global";
	  public const string APP_JNDI_NAME = "camunda-bpm-platform";
	  public const string MODULE_JNDI_NAME = "process-engine";

	  public const string PROCESS_ENGINE_SERVICE_NAME = "ProcessEngineService!org.camunda.bpm.ProcessEngineService";
	  public const string PROCESS_APPLICATION_SERVICE_NAME = "ProcessApplicationService!org.camunda.bpm.ProcessApplicationService";

	  public static readonly string PROCESS_ENGINE_SERVICE_JNDI_NAME = JNDI_NAME_PREFIX + "/" + APP_JNDI_NAME + "/" + MODULE_JNDI_NAME + "/" + PROCESS_ENGINE_SERVICE_NAME;
	  public static readonly string PROCESS_APPLICATION_SERVICE_JNDI_NAME = JNDI_NAME_PREFIX + "/" + APP_JNDI_NAME + "/" + MODULE_JNDI_NAME + "/" + PROCESS_APPLICATION_SERVICE_NAME;

	  public static ProcessEngineService ProcessEngineService
	  {
		  get
		  {
			return org.camunda.bpm.container.RuntimeContainerDelegate_Fields.INSTANCE.get().ProcessEngineService;
		  }
	  }

	  public static ProcessApplicationService ProcessApplicationService
	  {
		  get
		  {
			return org.camunda.bpm.container.RuntimeContainerDelegate_Fields.INSTANCE.get().ProcessApplicationService;
		  }
	  }

	  public static ProcessEngine DefaultProcessEngine
	  {
		  get
		  {
			return ProcessEngineService.DefaultProcessEngine;
		  }
	  }

	}
}