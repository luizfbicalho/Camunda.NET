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
namespace org.camunda.bpm.qa.upgrade.scenarios.multiinstance
{
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using Deployment = org.camunda.bpm.engine.test.Deployment;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MultiInstanceReceiveTaskScenario
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static String deployProcessParallel()
	  public static string deployProcessParallel()
	  {
		return "org/camunda/bpm/qa/upgrade/multiinstance/parallelMultiInstanceReceiveTask.bpmn20.xml";
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static String deployProcessSequential()
	  public static string deployProcessSequential()
	  {
		return "org/camunda/bpm/qa/upgrade/multiinstance/sequentialMultiInstanceReceiveTask.bpmn20.xml";
	  }

	  [DescribesScenario("initParallel"), Times(4)]
	  public static ScenarioSetup instantiateParallel()
	  {
		return new ScenarioSetupAnonymousInnerClass();
	  }

	  private class ScenarioSetupAnonymousInnerClass : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {
			engine.RuntimeService.startProcessInstanceByKey("ParallelMultiInstanceReceiveTask", scenarioName);
		  }
	  }

	  [DescribesScenario("initSequential"), Times(4)]
	  public static ScenarioSetup instantiateSequential()
	  {
		return new ScenarioSetupAnonymousInnerClass2();
	  }

	  private class ScenarioSetupAnonymousInnerClass2 : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {
			engine.RuntimeService.startProcessInstanceByKey("SequentialMultiInstanceReceiveTask", scenarioName);
		  }
	  }
	}

}