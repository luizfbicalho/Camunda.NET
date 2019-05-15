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
namespace org.camunda.bpm.engine.rest.impl.application
{
	using ProcessEngineProvider = org.camunda.bpm.engine.rest.spi.ProcessEngineProvider;

	/// <summary>
	/// <para>Uses the <seealso cref="ProcessEngineService"/> and exposes the default process engine</para>
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public class ContainerManagedProcessEngineProvider : ProcessEngineProvider
	{

	  public virtual ProcessEngine DefaultProcessEngine
	  {
		  get
		  {
			ProcessEngine defaultProcessEngine = BpmPlatform.DefaultProcessEngine;
			if (defaultProcessEngine != null)
			{
			  return defaultProcessEngine;
			}
			else
			{
			  return ProcessEngines.getDefaultProcessEngine(false);
			}
		  }
	  }

	  public virtual ProcessEngine getProcessEngine(string name)
	  {
		return ProcessEngines.getProcessEngine(name);
	  }

	  public virtual ISet<string> ProcessEngineNames
	  {
		  get
		  {
			return ProcessEngines.ProcessEngines.Keys;
		  }
	  }

	}

}