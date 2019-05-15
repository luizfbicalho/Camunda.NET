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

	/// <summary>
	/// Contains data about a property change.
	/// 
	/// @author Daniel Meyer
	/// @author Danny Gräf
	/// 
	/// </summary>
	public class PropertyChange
	{

	  /// <summary>
	  /// the empty change </summary>
	  public static readonly PropertyChange EMPTY_CHANGE = new PropertyChange(null, null, null);

	  /// <summary>
	  /// the name of the property which has been changed </summary>
	  protected internal string propertyName;

	  /// <summary>
	  /// the original value </summary>
	  protected internal object orgValue;

	  /// <summary>
	  /// the new value </summary>
	  protected internal object newValue;

	  public PropertyChange(string propertyName, object orgValue, object newValue)
	  {
		this.propertyName = propertyName;
		this.orgValue = orgValue;
		this.newValue = newValue;
	  }

	  public virtual string PropertyName
	  {
		  get
		  {
			return propertyName;
		  }
		  set
		  {
			this.propertyName = value;
		  }
	  }


	  public virtual object OrgValue
	  {
		  get
		  {
			return orgValue;
		  }
		  set
		  {
			this.orgValue = value;
		  }
	  }


	  public virtual object NewValue
	  {
		  get
		  {
			return newValue;
		  }
		  set
		  {
			this.newValue = value;
		  }
	  }


	  public virtual string NewValueString
	  {
		  get
		  {
			return valueAsString(newValue);
		  }
	  }

	  public virtual string OrgValueString
	  {
		  get
		  {
			return valueAsString(orgValue);
		  }
	  }

	  protected internal virtual string valueAsString(object value)
	  {
		if (value == null)
		{
		  return null;

		}
		else if (value is DateTime)
		{
		  return (((DateTime)value).Ticks).ToString();

		}
		else
		{
		  return value.ToString();

		}
	  }

	}

}