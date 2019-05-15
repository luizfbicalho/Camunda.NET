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
namespace org.camunda.bpm.qa.performance.engine.junit
{
	using HistoryService = org.camunda.bpm.engine.HistoryService;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using RepositoryService = org.camunda.bpm.engine.RepositoryService;
	using RuntimeService = org.camunda.bpm.engine.RuntimeService;
	using TaskService = org.camunda.bpm.engine.TaskService;
	using ProcessEngineRule = org.camunda.bpm.engine.test.ProcessEngineRule;
	using PerfTestBuilder = org.camunda.bpm.qa.performance.engine.framework.PerfTestBuilder;
	using PerfTestConfiguration = org.camunda.bpm.qa.performance.engine.framework.PerfTestConfiguration;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;

	/// <summary>
	/// <para>Base class for implementing a process engine performance test</para>
	/// 
	/// @author Daniel Meyer, Ingo Richtsmeier
	/// 
	/// </summary>
	public abstract class ProcessEnginePerformanceTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule processEngineRule = new org.camunda.bpm.engine.test.ProcessEngineRule(PerfTestProcessEngine.getInstance());
	  public ProcessEngineRule processEngineRule = new ProcessEngineRule(PerfTestProcessEngine.Instance);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public PerfTestConfigurationRule testConfigurationRule = new PerfTestConfigurationRule();
	  public PerfTestConfigurationRule testConfigurationRule = new PerfTestConfigurationRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public PerfTestResultRecorderRule resultRecorderRule = new PerfTestResultRecorderRule();
	  public PerfTestResultRecorderRule resultRecorderRule = new PerfTestResultRecorderRule();

	  protected internal ProcessEngine engine;
	  protected internal TaskService taskService;
	  protected internal HistoryService historyService;
	  protected internal RuntimeService runtimeService;
	  protected internal RepositoryService repositoryService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setup()
	  public virtual void setup()
	  {
		engine = processEngineRule.ProcessEngine;
		taskService = engine.TaskService;
		historyService = engine.HistoryService;
		runtimeService = engine.RuntimeService;
		repositoryService = engine.RepositoryService;
	  }

	  protected internal virtual PerfTestBuilder performanceTest()
	  {
		PerfTestConfiguration configuration = testConfigurationRule.PerformanceTestConfiguration;
		configuration.Platform = "camunda BPM";
		return new PerfTestBuilder(configuration, resultRecorderRule);
	  }

	}

}