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
namespace org.camunda.bpm.engine.test.standalone.history
{

	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using HistoryEventType = org.camunda.bpm.engine.impl.history.@event.HistoryEventType;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;

	public class CustomHistoryLevelIdentityLink : HistoryLevel
	{

	  private IList<HistoryEventTypes> eventTypes;

	  public CustomHistoryLevelIdentityLink()
	  {
	  }

	  public CustomHistoryLevelIdentityLink(IList<HistoryEventTypes> eventTypes)
	  {
		this.eventTypes = eventTypes;
	  }

	  public virtual int Id
	  {
		  get
		  {
			return 91;
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			return "aCustomHistoryLevelIL";
		  }
	  }

	  public virtual IList<HistoryEventTypes> EventTypes
	  {
		  get
		  {
			return eventTypes;
		  }
		  set
		  {
			this.eventTypes = value;
		  }
	  }


	  public virtual bool isHistoryEventProduced(HistoryEventType eventType, object entity)
	  {
		if (eventTypes != null)
		{
		  foreach (HistoryEventTypes eventTypeConfig in eventTypes)
		  {
			if (eventType.Equals(eventTypeConfig))
			{
			  return true;
			}
		  }
		}
		if (eventType.Equals(HistoryEventTypes.TASK_INSTANCE_CREATE))
		{
		  return true;
		}
		return false;
	  }
	}

}