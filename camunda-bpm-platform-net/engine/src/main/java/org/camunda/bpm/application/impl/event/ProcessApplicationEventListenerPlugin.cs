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
namespace org.camunda.bpm.application.impl.@event
{

	using BpmnParseListener = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParseListener;
	using AbstractProcessEnginePlugin = org.camunda.bpm.engine.impl.cfg.AbstractProcessEnginePlugin;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ProcessEnginePlugin = org.camunda.bpm.engine.impl.cfg.ProcessEnginePlugin;

	/// <summary>
	/// <para><seealso cref="ProcessEnginePlugin"/> enabling the process application event listener support.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ProcessApplicationEventListenerPlugin : AbstractProcessEnginePlugin
	{

	  public override void preInit(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
		IList<BpmnParseListener> preParseListeners = processEngineConfiguration.CustomPreBPMNParseListeners;
		if (preParseListeners == null)
		{
		  preParseListeners = new List<BpmnParseListener>();
		  processEngineConfiguration.CustomPreBPMNParseListeners = preParseListeners;
		}
		preParseListeners.Add(new ProcessApplicationEventParseListener());
	  }

	}

}