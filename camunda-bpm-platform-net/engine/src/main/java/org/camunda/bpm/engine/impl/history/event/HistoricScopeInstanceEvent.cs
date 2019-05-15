using System;

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
namespace org.camunda.bpm.engine.impl.history.@event
{

	/// <summary>
	/// @author Daniel Meyer
	/// @author Christian Lipphardt
	/// 
	/// </summary>
	[Serializable]
	public class HistoricScopeInstanceEvent : HistoryEvent
	{

	  private const long serialVersionUID = 1L;

	  protected internal long? durationInMillis;
	  protected internal DateTime startTime;
	  protected internal DateTime endTime;

	  // getters / setters ////////////////////////////////////

	  public virtual DateTime EndTime
	  {
		  get
		  {
			return endTime;
		  }
		  set
		  {
			this.endTime = value;
		  }
	  }


	  public virtual DateTime StartTime
	  {
		  get
		  {
			return startTime;
		  }
		  set
		  {
			this.startTime = value;
		  }
	  }


	  public virtual long? DurationInMillis
	  {
		  get
		  {
			if (durationInMillis != null)
			{
			  return durationInMillis;
    
			}
			else if (startTime != null && endTime != null)
			{
			  return endTime.Ticks - startTime.Ticks;
    
			}
			else
			{
			  return null;
    
			}
		  }
		  set
		  {
			this.durationInMillis = value;
		  }
	  }


	  public virtual long? DurationRaw
	  {
		  get
		  {
			return durationInMillis;
		  }
	  }

	}

}