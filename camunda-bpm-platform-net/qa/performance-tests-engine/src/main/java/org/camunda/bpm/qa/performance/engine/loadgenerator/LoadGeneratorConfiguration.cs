using System;
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
	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class LoadGeneratorConfiguration
	{

	  /// <summary>
	  /// the number of threads to use when generating load </summary>
	  protected internal int numOfThreads = Math.Max(1, Runtime.Runtime.availableProcessors() - 1);

	  /// <summary>
	  /// controls how often the worker runnables are executed </summary>
	  protected internal int numberOfIterations = 10000;

	  protected internal ThreadStart[] setupTasks;

	  protected internal ThreadStart[] workerTasks;

	  protected internal bool color = true;

	  public virtual int NumOfThreads
	  {
		  get
		  {
			return numOfThreads;
		  }
		  set
		  {
			this.numOfThreads = value;
		  }
	  }

	  public virtual ThreadStart[] SetupTasks
	  {
		  get
		  {
			return setupTasks;
		  }
		  set
		  {
			this.setupTasks = value;
		  }
	  }

	  public virtual ThreadStart[] WorkerTasks
	  {
		  get
		  {
			return workerTasks;
		  }
		  set
		  {
			this.workerTasks = value;
		  }
	  }




	  public virtual int NumberOfIterations
	  {
		  get
		  {
			return numberOfIterations;
		  }
		  set
		  {
			this.numberOfIterations = value;
		  }
	  }


	  public virtual bool Color
	  {
		  get
		  {
			return color;
		  }
		  set
		  {
			this.color = value;
		  }
	  }


	}

}