using System.Collections.Generic;
using System.Threading;
using System.IO;

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
namespace org.camunda.bpm.engine.test.concurrency
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.not;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using DeployCmd = org.camunda.bpm.engine.impl.cmd.DeployCmd;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DeploymentBuilderImpl = org.camunda.bpm.engine.impl.repository.DeploymentBuilderImpl;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using DeploymentBuilder = org.camunda.bpm.engine.repository.DeploymentBuilder;
	using DeploymentQuery = org.camunda.bpm.engine.repository.DeploymentQuery;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using DatabaseHelper = org.camunda.bpm.engine.test.util.DatabaseHelper;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	/// <summary>
	/// <para>Tests the deployment from two threads simultaneously.</para>
	/// 
	/// <para><b>Note:</b> the tests are not execute on H2 because it doesn't support the
	/// exclusive lock on the deployment table.</para>
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public class ConcurrentDeploymentTest : ConcurrencyTestCase
	{

	  private static string processResource;

	  static ConcurrentDeploymentTest()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess().startEvent().done();
		MemoryStream outputStream = new MemoryStream();
		Bpmn.writeModelToStream(outputStream, modelInstance);
		processResource = StringHelper.NewString(outputStream.toByteArray());
	  }

	  /// <summary>
	  /// hook into test method invocation - after the process engine is initialized
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void runTest() throws Throwable
	  protected internal override void runTest()
	  {
		string databaseType = DatabaseHelper.getDatabaseType(processEngineConfiguration);

		if ("h2".Equals(databaseType))
		{
		  // skip test method - if database is H2
		}
		else
		{
		  // invoke the test method
		  base.runTest();
		}
	  }

	  /// <seealso cref= https://app.camunda.com/jira/browse/CAM-2128 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testDuplicateFiltering() throws InterruptedException
	  public virtual void testDuplicateFiltering()
	  {

		deployOnTwoConcurrentThreads(createDeploymentBuilder().enableDuplicateFiltering(false), createDeploymentBuilder().enableDuplicateFiltering(false));

		// ensure that although both transactions were run concurrently, only one deployment was constructed.
		DeploymentQuery deploymentQuery = repositoryService.createDeploymentQuery();
		assertThat(deploymentQuery.count(), @is(1L));
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testVersioning() throws InterruptedException
	  public virtual void testVersioning()
	  {

		deployOnTwoConcurrentThreads(createDeploymentBuilder(), createDeploymentBuilder());

		// ensure that although both transactions were run concurrently, the process definitions have different versions
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().orderByProcessDefinitionVersion().asc().list();

		assertThat(processDefinitions.Count, @is(2));
		assertThat(processDefinitions[0].Version, @is(1));
		assertThat(processDefinitions[1].Version, @is(2));
	  }

	  protected internal virtual DeploymentBuilder createDeploymentBuilder()
	  {
		return (new DeploymentBuilderImpl(null)).name("some-deployment-name").addString("foo.bpmn", processResource);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void deployOnTwoConcurrentThreads(org.camunda.bpm.engine.repository.DeploymentBuilder deploymentOne, org.camunda.bpm.engine.repository.DeploymentBuilder deploymentTwo) throws InterruptedException
	  protected internal virtual void deployOnTwoConcurrentThreads(DeploymentBuilder deploymentOne, DeploymentBuilder deploymentTwo)
	  {
		assertThat("you can not use the same deployment builder for both deployments", deploymentOne, @is(not(deploymentTwo)));

		// STEP 1: bring two threads to a point where they have
		// 1) started a new transaction
		// 2) are ready to deploy
		ThreadControl thread1 = executeControllableCommand(new ControllableDeployCommand(deploymentOne));
		thread1.waitForSync();

		ThreadControl thread2 = executeControllableCommand(new ControllableDeployCommand(deploymentTwo));
		thread2.waitForSync();

		// STEP 2: make Thread 1 proceed and wait until it has deployed but not yet committed
		// -> will still hold the exclusive lock
		thread1.makeContinue();
		thread1.waitForSync();

		// STEP 3: make Thread 2 continue
		// -> it will attempt to acquire the exclusive lock and block on the lock
		thread2.makeContinue();

		// wait for 2 seconds (Thread 2 is blocked on the lock)
		Thread.Sleep(2000);

		// STEP 4: allow Thread 1 to terminate
		// -> Thread 1 will commit and release the lock
		thread1.waitUntilDone();

		// STEP 5: wait for Thread 2 to terminate
		thread2.waitForSync();
		thread2.waitUntilDone();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {

		foreach (Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}
	  }

	  protected internal class ControllableDeployCommand : ControllableCommand<Void>
	  {

		internal readonly DeploymentBuilder deploymentBuilder;

		public ControllableDeployCommand(DeploymentBuilder deploymentBuilder)
		{
		  this.deploymentBuilder = deploymentBuilder;
		}

		public override Void execute(CommandContext commandContext)
		{
		  monitor.sync(); // thread will block here until makeContinue() is called form main thread

		  (new DeployCmd((DeploymentBuilderImpl) deploymentBuilder)).execute(commandContext);

		  monitor.sync(); // thread will block here until waitUntilDone() is called form main thread

		  return null;
		}

	  }

	}

}