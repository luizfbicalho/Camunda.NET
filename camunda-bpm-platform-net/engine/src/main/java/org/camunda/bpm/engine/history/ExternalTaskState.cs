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
namespace org.camunda.bpm.engine.history
{

	public interface ExternalTaskState
	{

	  int StateCode {get;}

	  ///////////////////////////////////////////////////// default implementation

	}

	public static class ExternalTaskState_Fields
	{
	  public static readonly ExternalTaskState CREATED = new ExternalTaskState_ExternalTaskStateImpl(0, "created");
	  public static readonly ExternalTaskState FAILED = new ExternalTaskState_ExternalTaskStateImpl(1, "failed");
	  public static readonly ExternalTaskState SUCCESSFUL = new ExternalTaskState_ExternalTaskStateImpl(2, "successful");
	  public static readonly ExternalTaskState DELETED = new ExternalTaskState_ExternalTaskStateImpl(3, "deleted");
	}

	  public class ExternalTaskState_ExternalTaskStateImpl : ExternalTaskState
	  {

	public readonly int stateCode;
	protected internal readonly string name;

	public ExternalTaskState_ExternalTaskStateImpl(int stateCode, string @string)
	{
	  this.stateCode = stateCode;
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
	  ExternalTaskState_ExternalTaskStateImpl other = (ExternalTaskState_ExternalTaskStateImpl) obj;
	  return stateCode == other.stateCode;
	}

	public override string ToString()
	{
	  return name;
	}
	  }

}