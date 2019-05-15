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
namespace org.camunda.bpm.qa.performance.engine.loadgenerator
{

	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Process = org.camunda.bpm.model.bpmn.instance.Process;
	using PerfTestProcessEngine = org.camunda.bpm.qa.performance.engine.junit.PerfTestProcessEngine;
	using DeployFileTask = org.camunda.bpm.qa.performance.engine.loadgenerator.tasks.DeployFileTask;
	using StartProcessInstanceTask = org.camunda.bpm.qa.performance.engine.loadgenerator.tasks.StartProcessInstanceTask;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class StartProcessInstancesInDirectory
	{

	  public static readonly string[] DEPLOYABLE_FILE_EXTENSIONS = new string[] {".bpmn", ".bpmn20.xml"};

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void main(String[] args) throws InterruptedException
	  public static void Main(string[] args)
	  {

		ProcessEngine processEngine = PerfTestProcessEngine.Instance;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LoadGeneratorConfiguration config = new LoadGeneratorConfiguration();
		LoadGeneratorConfiguration config = new LoadGeneratorConfiguration();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> deployableFiles = findDeployableFiles(new java.io.File("."));
		IList<string> deployableFiles = findDeployableFiles(new File("."));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Runnable[] setupTasks = new Runnable[deployableFiles.size()];
		ThreadStart[] setupTasks = new ThreadStart[deployableFiles.Count];
		for (int i = 0; i < deployableFiles.Count; i++)
		{
		  setupTasks[i] = new DeployFileTask(processEngine, deployableFiles[i]);
		}
		config.SetupTasks = setupTasks;


//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> processKeys = extractProcessDefinitionKeys(deployableFiles);
		IList<string> processKeys = extractProcessDefinitionKeys(deployableFiles);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Runnable[] workerRunnables = new Runnable[processKeys.size()];
		ThreadStart[] workerRunnables = new ThreadStart[processKeys.Count];
		for (int i = 0; i < processKeys.Count; i++)
		{
		  workerRunnables[i] = new StartProcessInstanceTask(processEngine, processKeys[i]);
		}
		config.WorkerTasks = workerRunnables;


//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LoadGenerator loadGenerator = new LoadGenerator(config);
		LoadGenerator loadGenerator = new LoadGenerator(config);
		loadGenerator.execute();

	  }

	  private static IList<string> extractProcessDefinitionKeys(IList<string> deployableFileNames)
	  {
		List<string> keys = new List<string>();
		foreach (string file in deployableFileNames)
		{
		  if (file.EndsWith(".bpmn", StringComparison.Ordinal) || file.EndsWith(".bpmn20.xml", StringComparison.Ordinal))
		  {
			BpmnModelInstance modelInstance = Bpmn.readModelFromFile(new File(file));
			ICollection<Process> processes = modelInstance.getModelElementsByType(typeof(Process));
			foreach (Process process in processes)
			{
			  if (process.Executable)
			  {
				keys.Add(process.Id);
			  }
			}
		  }
		}
		return keys;
	  }

	  private static IList<string> findDeployableFiles(File dir)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> result = new java.util.ArrayList<String>();
		IList<string> result = new List<string>();

		string[] localNames = dir.list(new FilenameFilterAnonymousInnerClass(dir));

		if (localNames != null)
		{
		  foreach (string file in localNames)
		  {
			result.Add(dir.AbsolutePath + File.separator + file);
		  }
		}

		return result;
	  }

	  private class FilenameFilterAnonymousInnerClass : FilenameFilter
	  {
		  private File dir;

		  public FilenameFilterAnonymousInnerClass(File dir)
		  {
			  this.dir = dir;
		  }


		  public bool accept(File dir, string name)
		  {
			foreach (string extension in DEPLOYABLE_FILE_EXTENSIONS)
			{
			  if (name.EndsWith(extension, StringComparison.Ordinal))
			  {
				return true;
			  }
			}
			return false;
		  }

	  }

	}

}