using System;
using System.Collections.Generic;
using System.Threading;

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
namespace org.camunda.bpm.qa.performance.engine.query
{
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Process = org.camunda.bpm.model.bpmn.instance.Process;
	using PerfTestProcessEngine = org.camunda.bpm.qa.performance.engine.junit.PerfTestProcessEngine;
	using LoadGenerator = org.camunda.bpm.qa.performance.engine.loadgenerator.LoadGenerator;
	using LoadGeneratorConfiguration = org.camunda.bpm.qa.performance.engine.loadgenerator.LoadGeneratorConfiguration;
	using DeployModelInstancesTask = org.camunda.bpm.qa.performance.engine.loadgenerator.tasks.DeployModelInstancesTask;
	using GenerateMetricsTask = org.camunda.bpm.qa.performance.engine.loadgenerator.tasks.GenerateMetricsTask;
	using StartProcessInstanceTask = org.camunda.bpm.qa.performance.engine.loadgenerator.tasks.StartProcessInstanceTask;


	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class DefaultLoadGenerator
	{

	  /// <summary>
	  /// The reported ID for the metrics.
	  /// </summary>
	  protected internal const string REPORTER_ID = "REPORTER_ID";
	  protected internal const int NUMBER_OF_PROCESSES = 999;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void main(String[] args) throws InterruptedException
	  public static void Main(string[] args)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Properties properties = org.camunda.bpm.qa.performance.engine.junit.PerfTestProcessEngine.loadProperties();
		Properties properties = PerfTestProcessEngine.loadProperties();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.ProcessEngine processEngine = org.camunda.bpm.qa.performance.engine.junit.PerfTestProcessEngine.getInstance();
		ProcessEngine processEngine = PerfTestProcessEngine.Instance;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.qa.performance.engine.loadgenerator.LoadGeneratorConfiguration config = new org.camunda.bpm.qa.performance.engine.loadgenerator.LoadGeneratorConfiguration();
		LoadGeneratorConfiguration config = new LoadGeneratorConfiguration();
		config.Color = bool.Parse(properties.getProperty("loadGenerator.colorOutput", "false"));
		config.NumberOfIterations = int.Parse(properties.getProperty("loadGenerator.numberOfIterations", "10000"));

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.model.bpmn.BpmnModelInstance> modelInstances = createProcesses(config.getNumberOfIterations());
		IList<BpmnModelInstance> modelInstances = createProcesses(config.NumberOfIterations);

		ThreadStart[] setupTasks = new ThreadStart[] {new DeployModelInstancesTask(processEngine, modelInstances)};
		config.SetupTasks = setupTasks;

		ProcessEngineConfigurationImpl processEngineConfiguration = (ProcessEngineConfigurationImpl) processEngine.ProcessEngineConfiguration;
		processEngineConfiguration.MetricsEnabled = true;
		processEngineConfiguration.DbMetricsReporter.ReporterId = REPORTER_ID;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Runnable[] workerRunnables = new Runnable[2];
		ThreadStart[] workerRunnables = new ThreadStart[2];
		Process process = modelInstances[0].getModelElementsByType(typeof(Process)).GetEnumerator().next();
		string processDefKey = process.Id;
		workerRunnables[0] = new StartProcessInstanceTask(processEngine, processDefKey);
		workerRunnables[1] = new GenerateMetricsTask(processEngine);
		config.WorkerTasks = workerRunnables;

		(new LoadGenerator(config)).execute();

		Console.WriteLine(processEngine.HistoryService.createHistoricProcessInstanceQuery().count() + " Process Instances in DB");
		processEngineConfiguration.MetricsEnabled = false;
	  }

	  internal static IList<BpmnModelInstance> createProcesses(int numberOfProcesses)
	  {

		IList<BpmnModelInstance> result = new List<BpmnModelInstance>(numberOfProcesses);

		Console.WriteLine("Number of Processes: " + numberOfProcesses);
		for (int i = 0; i < NUMBER_OF_PROCESSES; i++)
		{
		  result.Add(createProcess(i));
		}
		return result;
	  }

	  protected internal static BpmnModelInstance createProcess(int id)
	  {
		return Bpmn.createExecutableProcess("process" + id).startEvent().userTask().camundaAssignee("demo").endEvent().done();
	  }

	}

}