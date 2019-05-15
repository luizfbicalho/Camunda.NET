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
namespace org.camunda.bpm.engine.impl.cfg
{

	/// <summary>
	/// <para>A process engine plugin allows customizing the process engine</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface ProcessEnginePlugin
	{

	  /// <summary>
	  /// <para>Invoked before the process engine configuration is initialized.</para>
	  /// </summary>
	  /// <param name="processEngineConfiguration"> the process engine configuation
	  ///  </param>
	  void preInit(ProcessEngineConfigurationImpl processEngineConfiguration);

	  /// <summary>
	  /// <para>Invoked after the process engine configuration is initialized.
	  /// and before the process engine is built.</para>
	  /// </summary>
	  /// <param name="processEngineConfiguration"> the process engine configuation
	  ///  </param>
	  void postInit(ProcessEngineConfigurationImpl processEngineConfiguration);

	  /// <summary>
	  /// <para>Invoked after the process engine has been built.</para>
	  /// </summary>
	  /// <param name="processEngine"> </param>
	  void postProcessEngineBuild(ProcessEngine processEngine);

	}

}