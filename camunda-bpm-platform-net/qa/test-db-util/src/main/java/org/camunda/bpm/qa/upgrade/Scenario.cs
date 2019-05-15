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
namespace org.camunda.bpm.qa.upgrade
{

	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using ProcessEngineException = org.camunda.bpm.engine.ProcessEngineException;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class Scenario
	{

	  protected internal int times = 1;
	  protected internal string name;
	  protected internal string extendedScenario;

	  protected internal ScenarioSetup setup;

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
		  set
		  {
			this.name = value;
		  }
	  }


	  public virtual int Times
	  {
		  get
		  {
			return times;
		  }
		  set
		  {
			this.times = value;
		  }
	  }


	  public virtual string ExtendedScenario
	  {
		  get
		  {
			return extendedScenario;
		  }
		  set
		  {
			this.extendedScenario = value;
		  }
	  }


	  public virtual ScenarioSetup Setup
	  {
		  set
		  {
			this.setup = value;
		  }
	  }

	  public virtual Scenario perform(ScenarioSetup setup)
	  {
		this.setup = setup;
		return this;
	  }

	  public virtual void createInstances(ProcessEngine engine, IDictionary<string, Scenario> scenarios)
	  {
		for (int i = 1; i <= times; i++)
		{
		  string scenarioInstanceName = name + "." + i;
		  create(engine, scenarios, scenarioInstanceName);
		}
	  }

	  public virtual void create(ProcessEngine engine, IDictionary<string, Scenario> scenarios, string scenarioInstanceName)
	  {
		// recursively set up all extended scenarios first
		if (!string.ReferenceEquals(extendedScenario, null))
		{
		  if (scenarios.ContainsKey(extendedScenario))
		  {
			Scenario parentScenario = scenarios[extendedScenario];
			parentScenario.create(engine, scenarios, scenarioInstanceName);
		  }
		  else
		  {
			throw new ProcessEngineException("Extended scenario " + extendedScenario + " not registered");
		  }
		}

		if (setup != null)
		{
		  setup.execute(engine, scenarioInstanceName);
		}
	  }
	}

}