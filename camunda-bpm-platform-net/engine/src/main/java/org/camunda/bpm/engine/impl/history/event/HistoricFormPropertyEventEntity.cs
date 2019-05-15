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
	/// 
	/// </summary>
	[Serializable]
	public class HistoricFormPropertyEventEntity : HistoricDetailEventEntity
	{

	  private const long serialVersionUID = 1L;

	  protected internal string propertyId;
	  protected internal string propertyValue;

	  public HistoricFormPropertyEventEntity()
	  {
	  }

	  public virtual string PropertyId
	  {
		  get
		  {
			return propertyId;
		  }
		  set
		  {
			this.propertyId = value;
		  }
	  }


	  public virtual object getPropertyValue()
	  {
		return propertyValue;
	  }

	  public virtual void setPropertyValue(string propertyValue)
	  {
		this.propertyValue = propertyValue;
	  }

	  public virtual DateTime Time
	  {
		  get
		  {
			return timestamp;
		  }
	  }

	  public override string ToString()
	  {
		return this.GetType().Name + "[propertyId=" + propertyId + ", propertyValue=" + propertyValue + ", activityInstanceId=" + activityInstanceId + ", eventType=" + eventType + ", executionId=" + executionId + ", id=" + id + ", processDefinitionId=" + processDefinitionId + ", processInstanceId=" + processInstanceId + ", taskId=" + taskId + ", tenantId=" + tenantId + "]";
	  }

	}

}