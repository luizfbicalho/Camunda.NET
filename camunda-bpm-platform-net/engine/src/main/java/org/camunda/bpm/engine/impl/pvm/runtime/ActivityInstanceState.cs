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
namespace org.camunda.bpm.engine.impl.pvm.runtime
{
	/// <summary>
	/// Contains a predefined set of states activity instances may be in
	/// during the execution of a process instance.
	/// 
	/// @author nico.rehwaldt
	/// </summary>
	public interface ActivityInstanceState
	{

	  int StateCode {get;}

	  ///////////////////////////////////////////////////// default implementation
	}

	public static class ActivityInstanceState_Fields
	{
	  public static readonly ActivityInstanceState DEFAULT = new ActivityInstanceState_ActivityInstanceStateImpl(0, "default");
	  public static readonly ActivityInstanceState SCOPE_COMPLETE = new ActivityInstanceState_ActivityInstanceStateImpl(1, "scopeComplete");
	  public static readonly ActivityInstanceState CANCELED = new ActivityInstanceState_ActivityInstanceStateImpl(2, "canceled");
	  public static readonly ActivityInstanceState STARTING = new ActivityInstanceState_ActivityInstanceStateImpl(3, "starting");
	  public static readonly ActivityInstanceState ENDING = new ActivityInstanceState_ActivityInstanceStateImpl(4, "ending");
	}

	  public class ActivityInstanceState_ActivityInstanceStateImpl : ActivityInstanceState
	  {

	public readonly int stateCode;
	protected internal readonly string name;

	public ActivityInstanceState_ActivityInstanceStateImpl(int suspensionCode, string @string)
	{
	  this.stateCode = suspensionCode;
	  this.name = @string;
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
	  ActivityInstanceState_ActivityInstanceStateImpl other = (ActivityInstanceState_ActivityInstanceStateImpl) obj;
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