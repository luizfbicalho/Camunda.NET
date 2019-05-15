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
namespace org.camunda.bpm.engine.test.history
{


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class SerializableVariable
	{

	  private const long serialVersionUID = 1L;

	  public string text;

	  public SerializableVariable(string text) : base()
	  {
		this.text = text;
	  }

	  public override int GetHashCode()
	  {
		const int prime = 31;
		int result = 1;
		result = prime * result + ((string.ReferenceEquals(text, null)) ? 0 : text.GetHashCode());
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
		SerializableVariable other = (SerializableVariable) obj;
		if (string.ReferenceEquals(text, null))
		{
		  if (!string.ReferenceEquals(other.text, null))
		  {
			return false;
		  }
		}
		else if (!text.Equals(other.text))
		{
		  return false;
		}
		return true;
	  }
	}

}