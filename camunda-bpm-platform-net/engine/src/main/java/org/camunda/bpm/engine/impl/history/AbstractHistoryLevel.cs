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
namespace org.camunda.bpm.engine.impl.history
{
	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public abstract class AbstractHistoryLevel : HistoryLevel
	{
		public abstract bool isHistoryEventProduced(@event.HistoryEventType eventType, object entity);
		public abstract string Name {get;}
		public abstract int Id {get;}

	  public override int GetHashCode()
	  {
		const int prime = 31;
		int result = 1;
		result = prime * result + Id;
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
		AbstractHistoryLevel other = (AbstractHistoryLevel) obj;
		if (Id != other.Id)
		{
		  return false;
		}
		return true;
	  }

	  public override string ToString()
	  {
		return string.Format("{0}(name={1}, id={2:D})", this.GetType().Name, Name, Id);
	  }
	}

}