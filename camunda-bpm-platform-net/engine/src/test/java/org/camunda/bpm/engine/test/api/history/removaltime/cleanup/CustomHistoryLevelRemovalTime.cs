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
namespace org.camunda.bpm.engine.test.api.history.removaltime.cleanup
{
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using HistoryEventType = org.camunda.bpm.engine.impl.history.@event.HistoryEventType;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class CustomHistoryLevelRemovalTime : HistoryLevel
	{

	  private HistoryEventTypes[] eventTypes;

	  public virtual int Id
	  {
		  get
		  {
			return 47;
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			return "customHistoryLevel";
		  }
	  }

	  public virtual bool isHistoryEventProduced(HistoryEventType eventType, object entity)
	  {
		if (eventTypes != null)
		{
		  foreach (HistoryEventType historyEventType in this.eventTypes)
		  {
			if (eventType.Equals(historyEventType))
			{
			  return true;
			}
		  }
		}

		return eventType.Equals(HistoryEventTypes.PROCESS_INSTANCE_END) || isRootProcessInstance(entity);
	  }

	  public virtual params HistoryEventTypes[] EventTypes
	  {
		  set
		  {
			this.eventTypes = value;
		  }
	  }

	  protected internal virtual bool isRootProcessInstance(object entity)
	  {
		if (entity is ProcessInstance)
		{
		  ProcessInstance processInstance = (ProcessInstance) entity;
		  return processInstance.Id.Equals(processInstance.RootProcessInstanceId);
		}

		return false;
	  }
	}

}