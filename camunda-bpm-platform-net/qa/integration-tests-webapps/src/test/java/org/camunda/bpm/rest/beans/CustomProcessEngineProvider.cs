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
namespace org.camunda.bpm.rest.beans
{
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using ProcessEngineProvider = org.camunda.bpm.engine.rest.spi.ProcessEngineProvider;

	public class CustomProcessEngineProvider : ProcessEngineProvider
	{

	  public virtual ProcessEngine DefaultProcessEngine
	  {
		  get
		  {
			return BpmPlatform.DefaultProcessEngine;
		  }
	  }

	  public virtual ProcessEngine getProcessEngine(string name)
	  {
		return BpmPlatform.ProcessEngineService.getProcessEngine(name);
	  }

	  public virtual ISet<string> ProcessEngineNames
	  {
		  get
		  {
			return BpmPlatform.ProcessEngineService.ProcessEngineNames;
		  }
	  }

	}

}