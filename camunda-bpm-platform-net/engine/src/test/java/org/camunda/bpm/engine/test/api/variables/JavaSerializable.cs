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
namespace org.camunda.bpm.engine.test.api.variables
{

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	[Serializable]
	public class JavaSerializable
	{

	  private const long serialVersionUID = 1L;

	  private string property;

	  public JavaSerializable(string property)
	  {
		this.property = property;
	  }

	  public virtual string Property
	  {
		  get
		  {
			return property;
		  }
		  set
		  {
			this.property = value;
		  }
	  }


	  public override int GetHashCode()
	  {
		const int prime = 31;
		int result = 1;
		result = prime * result + ((string.ReferenceEquals(property, null)) ? 0 : property.GetHashCode());
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
		JavaSerializable other = (JavaSerializable) obj;
		if (string.ReferenceEquals(property, null))
		{
		  if (!string.ReferenceEquals(other.property, null))
		  {
			return false;
		  }
		}
		else if (!property.Equals(other.property))
		{
		  return false;
		}
		return true;
	  }

	  public override string ToString()
	  {
		return "JavaSerializable [property=" + property + "]";
	  }


	}

}