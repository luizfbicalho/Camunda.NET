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
namespace org.camunda.bpm.engine.impl.cmmn.execution
{



	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public interface CaseExecutionState
	{

	  int StateCode {get;}

	  ///////////////////////////////////////////////////// default implementation

	}

	public static class CaseExecutionState_Fields
	{
	  public static readonly IDictionary<int, CaseExecutionState> CASE_EXECUTION_STATES = new Dictionary<int, CaseExecutionState>();
	  public static readonly CaseExecutionState NEW = new CaseExecutionState_CaseExecutionStateImpl(0, "new");
	  public static readonly CaseExecutionState AVAILABLE = new CaseExecutionState_CaseExecutionStateImpl(1, "available");
	  public static readonly CaseExecutionState ENABLED = new CaseExecutionState_CaseExecutionStateImpl(2, "enabled");
	  public static readonly CaseExecutionState DISABLED = new CaseExecutionState_CaseExecutionStateImpl(3, "disabled");
	  public static readonly CaseExecutionState ACTIVE = new CaseExecutionState_CaseExecutionStateImpl(4, "active");
	  public static readonly CaseExecutionState SUSPENDED = new CaseExecutionState_CaseExecutionStateImpl(5, "suspended");
	  public static readonly CaseExecutionState TERMINATED = new CaseExecutionState_CaseExecutionStateImpl(6, "terminated");
	  public static readonly CaseExecutionState COMPLETED = new CaseExecutionState_CaseExecutionStateImpl(7, "completed");
	  public static readonly CaseExecutionState FAILED = new CaseExecutionState_CaseExecutionStateImpl(8, "failed");
	  public static readonly CaseExecutionState CLOSED = new CaseExecutionState_CaseExecutionStateImpl(9, "closed");
	  public static readonly CaseExecutionState TERMINATING_ON_TERMINATION = new CaseExecutionState_CaseExecutionStateImpl(10, "terminatingOnTermination");
	  public static readonly CaseExecutionState TERMINATING_ON_PARENT_TERMINATION = new CaseExecutionState_CaseExecutionStateImpl(11, "terminatingOnParentTermination");
	  public static readonly CaseExecutionState TERMINATING_ON_EXIT = new CaseExecutionState_CaseExecutionStateImpl(12, "terminatingOnExit");
	  public static readonly CaseExecutionState SUSPENDING_ON_SUSPENSION = new CaseExecutionState_CaseExecutionStateImpl(13, "suspendingOnSuspension");
	  public static readonly CaseExecutionState SUSPENDING_ON_PARENT_SUSPENSION = new CaseExecutionState_CaseExecutionStateImpl(14, "suspendingOnParentSuspension");
	}

	  public class CaseExecutionState_CaseExecutionStateImpl : CaseExecutionState
	  {

	public readonly int stateCode;
	protected internal readonly string name;

	public CaseExecutionState_CaseExecutionStateImpl(int stateCode, string @string)
	{
	  this.stateCode = stateCode;
	  this.name = @string;

	  CaseExecutionState_Fields.CASE_EXECUTION_STATES[stateCode] = this;
	}

	public static CaseExecutionState getStateForCode(int? stateCode)
	{
	  return CaseExecutionState_Fields.CASE_EXECUTION_STATES[stateCode];
	}

	public virtual int StateCode
	{
		get
		{
		  return stateCode;
		}
	}

	public override int GetHashCode()
	{
	  const int prime = 31;
	  int result = 1;
	  result = prime * result + stateCode;
	  return result;
	}

	public override bool Equals(object obj)
	{
	  if (this == obj)
	  {
		return true;
	  }
	  if (obj == null)
	  {
		return false;
	  }
	  if (this.GetType() != obj.GetType())
	  {
		return false;
	  }
	  CaseExecutionState_CaseExecutionStateImpl other = (CaseExecutionState_CaseExecutionStateImpl) obj;
	  if (stateCode != other.stateCode)
	  {
		return false;
	  }
	  return true;
	}

	public override string ToString()
	{
	  return name;
	}
	  }

}