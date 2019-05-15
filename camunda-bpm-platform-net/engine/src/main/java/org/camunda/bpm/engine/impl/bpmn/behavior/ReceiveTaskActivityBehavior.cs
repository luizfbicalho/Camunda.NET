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
namespace org.camunda.bpm.engine.impl.bpmn.behavior
{
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;


	/// <summary>
	/// A receive task is a wait state that waits for the receival of some message.
	/// 
	/// Currently, the only message that is supported is the external trigger,
	/// given by calling the <seealso cref="RuntimeService#signal(String)"/> operation.
	/// 
	/// @author Joram Barrez
	/// </summary>
	public class ReceiveTaskActivityBehavior : TaskActivityBehavior
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void performExecution(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public override void performExecution(ActivityExecution execution)
	  {
		// Do nothing: waitstate behavior
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void signal(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution, String signalName, Object data) throws Exception
	  public virtual void signal(ActivityExecution execution, string signalName, object data)
	  {
		leave(execution);
	  }

	}

}