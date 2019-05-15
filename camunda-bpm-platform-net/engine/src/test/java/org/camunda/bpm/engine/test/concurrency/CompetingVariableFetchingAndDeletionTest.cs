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
	/// This test makes sure that if one thread loads a variable
	/// of type object, it does not fail with a OLE if the variable is also
	/// concurrently deleted.
	/// 
	/// Some context: this failed if the variable instance entity was first loaded,
	/// and, before loading the byte array, both the variable and the byte array were
	/// deleted by a concurrent transaction AND that transaction was comitted, before
	/// the bytearray was loaded.
	/// => loading the byte array returned null which triggered setting to null the
	/// byteArrayId value on the VariableInstanceEntity which in turn triggered an
	/// update of the variable instance entity itself which failed with OLE because
	/// the VariableInstanceEntity was already deleted.
	/// 
	/// +
	/// |
	/// |    Test Thread           Async Thread
	/// |   +-----------+         +------------+
	/// |      start PI
	/// |      (with var)               +
	/// |         +                     |
	/// |         |                     v
	/// |         |                 fetch VarInst
	/// |         |                 (not byte array)
	/// |         |                     +
	/// |         v                     |
	/// |      delete PI                |
	/// |         +                     v
	/// |         |                 fetch byte array (=>null)
	/// |         |                     +
	/// |         |                     |
	/// |         |                     v
	/// |         |                 flush()
	/// |         |                 (this must not perform
	/// |         v                 update to VarInst)
	/// v  time
	/// 
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class CompetingVariableFetchingAndDeletionTest : ConcurrencyTestCase
	{

	  private ThreadControl asyncThread;

	  public virtual void testConcurrentFetchAndDelete()
	  {

		deployment(createExecutableProcess("test").startEvent().userTask().endEvent().done());

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> listVar = java.util.Arrays.asList("a", "b");
		IList<string> listVar = Arrays.asList("a", "b");
		string pid = runtimeService.startProcessInstanceByKey("test", createVariables().putValue("listVar", listVar)).Id;

		// start a controlled Fetch variable command
		asyncThread = executeControllableCommand(new FetchVariableCmd(pid, "listVar"));

		// wait for async thread to load the variable (but not the byte array)
		asyncThread.waitForSync();

		// now delete the process instance
		runtimeService.deleteProcessInstance(pid, null);

		// make the second thread continue
		// => this will a flush the FetchVariableCmd Context.
		// if the flush performs an update to the variable, it will fail with an OLE
		asyncThread.makeContinue();
		asyncThread.waitUntilDone();

	  }

	  internal class FetchVariableCmd : ControllableCommand<Void>
	  {

		protected internal string executionId;
		protected internal string varName;

		public FetchVariableCmd(string executionId, string varName)
		{
		  this.executionId = executionId;
		  this.varName = varName;
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

		  // now trigger the fetching of the byte array
		  object value = varInstance.getValue();
		  assertNull("Expecting the value to be null (deleted)", value);

		  return null;
		}

	  }

	}

}