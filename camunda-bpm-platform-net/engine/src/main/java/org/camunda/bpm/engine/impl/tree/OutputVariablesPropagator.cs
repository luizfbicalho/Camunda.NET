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
namespace org.camunda.bpm.engine.impl.tree
{
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using SubProcessActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.SubProcessActivityBehavior;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using PvmExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl;

	/// <summary>
	/// Pass the output variables from the process instance of a subprocess to the
	/// calling process instance.
	/// 
	/// @author Philipp Ossler
	/// 
	/// </summary>
	public class OutputVariablesPropagator : TreeVisitor<ActivityExecution>
	{

	  public virtual void visit(ActivityExecution execution)
	  {

		if (isProcessInstanceOfSubprocess(execution))
		{

		  PvmExecutionImpl superExecution = (PvmExecutionImpl) execution.SuperExecution;
		  ActivityImpl activity = superExecution.getActivity();
		  SubProcessActivityBehavior subProcessActivityBehavior = (SubProcessActivityBehavior) activity.ActivityBehavior;

		  subProcessActivityBehavior.passOutputVariables(superExecution, execution);
		}
	  }

	  protected internal virtual bool isProcessInstanceOfSubprocess(ActivityExecution execution)
	  {
		return execution.ProcessInstanceExecution && execution.SuperExecution != null;
	  }

	}

}