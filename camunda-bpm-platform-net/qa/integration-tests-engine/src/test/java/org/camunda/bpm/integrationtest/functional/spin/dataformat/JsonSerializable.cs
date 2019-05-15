using System;
using System.Text;

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
namespace org.camunda.bpm.integrationtest.functional.spin.dataformat
{

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class JsonSerializable
	{

	  public const long ONE_DAY_IN_MILLIS = 1000 * 60 * 60 * 24;

	  private DateTime dateProperty;

	  public JsonSerializable()
	  {

	  }

	  public JsonSerializable(DateTime dateProperty)
	  {
		this.dateProperty = dateProperty;
	  }

	  public virtual DateTime DateProperty
	  {
		  get
		  {
			return dateProperty;
		  }
		  set
		  {
			this.dateProperty = value;
		  }
	  }


	  /// <summary>
	  /// Serializes the value according to the given date format
	  /// </summary>
	  public virtual string toExpectedJsonString(DateFormat dateFormat)
	  {
		StringBuilder jsonBuilder = new StringBuilder();

		jsonBuilder.Append("{\"dateProperty\":\"");
		jsonBuilder.Append(dateFormat.format(dateProperty));
		jsonBuilder.Append("\"}");

		return jsonBuilder.ToString();
	  }

	  /// <summary>
	  /// Serializes the value as milliseconds
	  /// </summary>
	  public virtual string toExpectedJsonString()
	  {
		StringBuilder jsonBuilder = new StringBuilder();

		jsonBuilder.Append("{\"dateProperty\":");
		jsonBuilder.Append(Convert.ToString(dateProperty.Ticks));
		jsonBuilder.Append("}");

		return jsonBuilder.ToString();
	  }

	  public override int GetHashCode()
	  {
		const int prime = 31;
		int result = 1;
		result = prime * result + ((dateProperty == null) ? 0 : dateProperty.GetHashCode());
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
		JsonSerializable other = (JsonSerializable) obj;
		if (dateProperty == null)
		{
		  if (other.dateProperty != null)
		  {
			return false;
		  }
		}
		else if (!dateProperty.Equals(other.dateProperty))
		{
		  return false;
		}
		return true;
	  }
	}

}