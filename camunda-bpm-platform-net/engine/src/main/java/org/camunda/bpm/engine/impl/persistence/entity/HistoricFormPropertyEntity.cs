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
namespace org.camunda.bpm.engine.impl.persistence.entity
{
	using HistoricFormField = org.camunda.bpm.engine.history.HistoricFormField;
	using HistoricFormProperty = org.camunda.bpm.engine.history.HistoricFormProperty;
	using HistoricFormPropertyEventEntity = org.camunda.bpm.engine.impl.history.@event.HistoricFormPropertyEventEntity;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class HistoricFormPropertyEntity : HistoricFormPropertyEventEntity, HistoricFormProperty, HistoricFormField
	{

	  private const long serialVersionUID = 1L;

	  public override string PropertyValue
	  {
		  get
		  {
			if (!string.ReferenceEquals(propertyValue, null))
			{
			  return propertyValue.ToString();
			}
			else
			{
			  return null;
			}
		  }
	  }

	  public virtual string FieldId
	  {
		  get
		  {
			return propertyId;
		  }
	  }

	  public virtual object FieldValue
	  {
		  get
		  {
			return propertyValue;
		  }
	  }

	}

}