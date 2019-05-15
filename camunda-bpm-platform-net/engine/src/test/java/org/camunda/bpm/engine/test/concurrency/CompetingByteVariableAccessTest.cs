using System;

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
//	import static org.camunda.bpm.engine.variable.Variables.createVariables;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess;

	using CachedDbEntity = org.camunda.bpm.engine.impl.db.entitymanager.cache.CachedDbEntity;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ByteArrayEntity = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayEntity;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using VariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity;

	/// <summary>
	/// thread1:
	///  t=1: fetch byte variable
	///  t=4: update byte variable value
	/// 
	/// thread2:
	///  t=2: fetch and delete byte variable and entity
	///  t=3: commit transaction
	/// 
	/// This test ensures that thread1's command fails with an OptimisticLockingException,
	/// not with a NullPointerException or something in that direction.
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class CompetingByteVariableAccessTest : ConcurrencyTestCase
	{

	  private ThreadControl asyncThread;

	  public virtual void testConcurrentVariableRemoval()
	  {
		deployment(createExecutableProcess("test").startEvent().userTask().endEvent().done());

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] byteVar = "asd".getBytes();
		sbyte[] byteVar = "asd".GetBytes();

		string pid = runtimeService.startProcessInstanceByKey("test", createVariables().putValue("byteVar", byteVar)).Id;

		// start a controlled Fetch and Update variable command
		asyncThread = executeControllableCommand(new FetchAndUpdateVariableCmd(pid, "byteVar", "bsd".GetBytes()));

		asyncThread.waitForSync();

		// now delete the process instance, deleting the variable and its byte array entity
		runtimeService.deleteProcessInstance(pid, null);

		// make the second thread continue
		// => this will a flush the FetchVariableCmd Context.
		// if the flush performs an update to the variable, it will fail with an OLE
		asyncThread.reportInterrupts();
		asyncThread.waitUntilDone();

		Exception exception = asyncThread.Exception;
		assertNotNull(exception);
		assertTrue(exception is OptimisticLockingException);


	  }

	  internal class FetchAndUpdateVariableCmd : ControllableCommand<Void>
	  {

		protected internal string executionId;
		protected internal string varName;
		protected internal object newValue;

		public FetchAndUpdateVariableCmd(string executionId, string varName, object newValue)
		{
		  this.executionId = executionId;
		  this.varName = varName;
		  this.newValue = newValue;
		}

		public override Void execute(CommandContext commandContext)
		{

		  ExecutionEntity execution = commandContext.ExecutionManager.findExecutionById(executionId);

		  // fetch the variable instance but not the value (make sure the byte array is lazily fetched)
		  VariableInstanceEntity varInstance = (VariableInstanceEntity) execution.getVariableInstanceLocal(varName);
		  string byteArrayValueId = varInstance.ByteArrayValueId;
		  assertNotNull("Byte array id is expected to be not null", byteArrayValueId);

		  CachedDbEntity cachedByteArray = commandContext.DbEntityManager.DbEntityCache.getCachedEntity(typeof(ByteArrayEntity), byteArrayValueId);

		  assertNull("Byte array is expected to be not fetched yet / lazily fetched.", cachedByteArray);

		  monitor.sync();

		  // now update the value
		  execution.setVariableLocal(varInstance.Name, newValue);

		  return null;
		}

	  }
	}

}