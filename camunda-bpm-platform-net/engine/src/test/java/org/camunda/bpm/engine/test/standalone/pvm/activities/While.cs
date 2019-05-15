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
namespace org.camunda.bpm.engine.test.standalone.pvm.activities
{
	using PvmTransition = org.camunda.bpm.engine.impl.pvm.PvmTransition;
	using ActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityBehavior;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class While : ActivityBehavior
	{

	  internal string variableName;
	  internal int from;
	  internal int to;

	  public While(string variableName, int from, int to)
	  {
		this.variableName = variableName;
		this.from = from;
		this.to = to;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public virtual void execute(ActivityExecution execution)
	  {
		PvmTransition more = execution.Activity.findOutgoingTransition("more");
		PvmTransition done = execution.Activity.findOutgoingTransition("done");

		int? value = (int?) execution.getVariable(variableName);

		if (value == null)
		{
		  execution.setVariable(variableName, from);
		  execution.leaveActivityViaTransition(more);

		}
		else
		{
		  value = value+1;

		  if (value.Value < to)
		  {
			execution.setVariable(variableName, value);
			execution.leaveActivityViaTransition(more);

		  }
		  else
		  {
			execution.leaveActivityViaTransition(done);
		  }
		}
	  }

	}

}