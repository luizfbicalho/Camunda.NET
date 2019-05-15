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
namespace org.camunda.bpm.engine.test.jobexecutor
{

	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Assert = org.junit.Assert;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class JobAcquisitionTestHelper
	{


	  /// <summary>
	  /// suspends random process instances that are active
	  /// </summary>
	  public static void suspendInstances(ProcessEngine processEngine, int numInstances)
	  {
		IList<ProcessInstance> instancesToSuspend = processEngine.RuntimeService.createProcessInstanceQuery().active().listPage(0, numInstances);
		if (instancesToSuspend.Count < numInstances)
		{
		  throw new ProcessEngineException("Cannot suspend " + numInstances + " process instances");
		}

		foreach (ProcessInstance activeInstance in instancesToSuspend)
		{
		  processEngine.RuntimeService.suspendProcessInstanceById(activeInstance.Id);
		}
	  }

	  /// <summary>
	  /// activates random process instances that are active
	  /// </summary>
	  public static void activateInstances(ProcessEngine processEngine, int numInstances)
	  {
		IList<ProcessInstance> instancesToActivate = processEngine.RuntimeService.createProcessInstanceQuery().suspended().listPage(0, numInstances);
		if (instancesToActivate.Count < numInstances)
		{
		  throw new ProcessEngineException("Cannot activate " + numInstances + " process instances");
		}

		foreach (ProcessInstance suspendedInstance in instancesToActivate)
		{
		  processEngine.RuntimeService.activateProcessInstanceById(suspendedInstance.Id);
		}
	  }

	  public static void assertInBetween(long minimum, long maximum, long actualValue)
	  {
		Assert.assertTrue("Expected '" + actualValue + "' to be between '" + minimum + "' and '" + maximum + "'", actualValue >= minimum && actualValue <= maximum);
	  }

	}

}