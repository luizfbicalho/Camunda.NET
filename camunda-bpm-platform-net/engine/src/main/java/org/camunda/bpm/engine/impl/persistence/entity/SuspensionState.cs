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
namespace org.camunda.bpm.engine.impl.persistence.entity
{

	/// <summary>
	/// Contains a predefined set of states for process definitions and process instances
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public interface SuspensionState
	{

	  int StateCode {get;}

	  string Name {get;}

	  ///////////////////////////////////////////////////// default implementation
	}

	public static class SuspensionState_Fields
	{
	  public static readonly SuspensionState ACTIVE = new SuspensionState_SuspensionStateImpl(1, "active");
	  public static readonly SuspensionState SUSPENDED = new SuspensionState_SuspensionStateImpl(2, "suspended");
	}

	  public class SuspensionState_SuspensionStateImpl : SuspensionState
	  {

	public readonly int stateCode;
	protected internal readonly string name;

	public SuspensionState_SuspensionStateImpl(int suspensionCode, string @string)
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
	  SuspensionState_SuspensionStateImpl other = (SuspensionState_SuspensionStateImpl) obj;
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

	public virtual string Name
	{
		get
		{
		  return name;
		}
	}
	  }

}