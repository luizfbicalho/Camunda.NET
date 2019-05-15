using System;
using System.Text;
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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.qa.performance.engine.loadgenerator.CompletionSignalingRunnable.wrap;


	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class LoadGenerator
	{

	  public const string ANSI_RESET = "\u001B[0m";
	  public const string ANSI_BLACK = "\u001B[30m";
	  public const string ANSI_RED = "\u001B[31m";
	  public const string ANSI_GREEN = "\u001B[32m";
	  public const string ANSI_YELLOW = "\u001B[33m";
	  public const string ANSI_BLUE = "\u001B[34m";
	  public const string ANSI_PURPLE = "\u001B[35m";
	  public const string ANSI_CYAN = "\u001B[36m";
	  public const string ANSI_WHITE = "\u001B[37m";

	  public const string ANSI_CLEAR_LINE = "\u001B[2K";
	  public static readonly string CLEAR_LINE = ANSI_CLEAR_LINE + "\r";

	  protected internal LoadGeneratorConfiguration configuration;

	  public LoadGenerator(LoadGeneratorConfiguration configuration)
	  {
		this.configuration = configuration;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute() throws InterruptedException
	  public virtual void execute()
	  {

		ExecutorService executorService = Executors.newFixedThreadPool(configuration.NumOfThreads);

		runSetup(executorService);

		runWorkers(executorService);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void runWorkers(java.util.concurrent.ExecutorService executorService) throws InterruptedException
	  private void runWorkers(ExecutorService executorService)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numberOfIterations = configuration.getNumberOfIterations();
		int numberOfIterations = configuration.NumberOfIterations;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int taskCount = numberOfIterations * configuration.getWorkerTasks().length;
		int taskCount = numberOfIterations * configuration.WorkerTasks.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.CountDownLatch sync = new java.util.concurrent.CountDownLatch(taskCount);
		System.Threading.CountdownEvent sync = new System.Threading.CountdownEvent(taskCount);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Timer timer = new java.util.Timer();
		Timer timer = new Timer();
		timer.scheduleAtFixedRate(new ProgressReporter(taskCount, sync, configuration.Color), 2000, 2000);

		Console.WriteLine("Generating load. Total tasks: " + taskCount + "... ");

		for (int i = 1; i <= numberOfIterations; i++)
		{

		  foreach (ThreadStart runnable in configuration.WorkerTasks)
		  {
			executorService.execute(wrap(runnable, sync));
		  }

		}

		sync.await();

		timer.cancel();

		if (configuration.Color)
		{
			Console.Write(CLEAR_LINE + ANSI_GREEN);
		}
		Console.WriteLine("Finished generating load.");
		if (configuration.Color)
		{
			Console.Write(ANSI_RESET);
		}

		executorService.shutdown();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void runSetup(java.util.concurrent.ExecutorService executorService) throws InterruptedException
	  private void runSetup(ExecutorService executorService)
	  {
		System.Threading.CountdownEvent sync = new System.Threading.CountdownEvent(configuration.SetupTasks.Length);

		Console.Write("Running setup ... ");

		foreach (ThreadStart r in configuration.SetupTasks)
		{
		  executorService.execute(wrap(r, sync));
		}


		sync.await();

		if (configuration.Color)
		{
			Console.Write(ANSI_GREEN);
		}

		Console.WriteLine("Done");

		if (configuration.Color)
		{
			Console.Write(ANSI_RESET);
		}
	  }

	  internal class ProgressReporter : TimerTask
	  {

		internal int totalWork;
		protected internal System.Threading.CountdownEvent sync;

		protected internal bool color;

		public ProgressReporter(int totalWork, System.Threading.CountdownEvent latch, bool color)
		{
		  this.totalWork = totalWork;
		  this.sync = latch;
		  this.color = color;
		}

		public override void run()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long tasksCompleted = totalWork - sync.getCount();
		  long tasksCompleted = totalWork - sync.CurrentCount;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double progress = (100d / totalWork) * tasksCompleted;
		  double progress = (100d / totalWork) * tasksCompleted;


		  StringBuilder statusMessage = new StringBuilder();

		  if (color)
		  {
			  statusMessage.Append(CLEAR_LINE + ANSI_YELLOW);
		  }

		  statusMessage.Append(string.Format("{0,6:F2}", progress));
		  statusMessage.Append("% done");

		  if (color)
		  {
			  statusMessage.Append(ANSI_RESET);
		  }
		  if (!color)
		  {
			  statusMessage.Append("\n");
		  }

		  Console.Write(statusMessage);
		}

	  }


	}

}