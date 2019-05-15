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
namespace org.camunda.bpm.integrationtest.deployment.war.beans
{
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ProcessEnginePlugin = org.camunda.bpm.engine.impl.cfg.ProcessEnginePlugin;
	using ScriptBindingsFactory = org.camunda.bpm.engine.impl.scripting.engine.ScriptBindingsFactory;
	using ScriptingEngines = org.camunda.bpm.engine.impl.scripting.engine.ScriptingEngines;

	public class GroovyProcessEnginePlugin : ProcessEnginePlugin
	{

	  public virtual void preInit(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {

	  }

	  public virtual void postInit(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
		processEngineConfiguration.setScriptingEngines(new ScriptingEngines(new ScriptBindingsFactory(processEngineConfiguration.ResolverFactories)
		   ));
	  }

	  public virtual void postProcessEngineBuild(ProcessEngine processEngine)
	  {

	  }
	}

}