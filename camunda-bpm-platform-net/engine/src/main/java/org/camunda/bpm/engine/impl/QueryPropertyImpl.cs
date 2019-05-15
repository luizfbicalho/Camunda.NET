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
namespace org.camunda.bpm.engine.impl
{
	using QueryProperty = org.camunda.bpm.engine.query.QueryProperty;

	/// 
	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	[Serializable]
	public class QueryPropertyImpl : QueryProperty
	{

	  private const long serialVersionUID = 1L;

	  protected internal string name;

	  protected internal string function;

	  public QueryPropertyImpl(string name) : this(name, null)
	  {
	  }

	  public QueryPropertyImpl(string name, string function)
	  {
		this.name = name;
		this.function = function;
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public virtual string Function
	  {
		  get
		  {
			return function;
		  }
	  }

	  public override int GetHashCode()
	  {
		const int prime = 31;
		int result = 1;
		result = prime * result + ((string.ReferenceEquals(function, null)) ? 0 : function.GetHashCode());
		result = prime * result + ((string.ReferenceEquals(name, null)) ? 0 : name.GetHashCode());
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
		QueryPropertyImpl other = (QueryPropertyImpl) obj;
		if (string.ReferenceEquals(function, null))
		{
		  if (!string.ReferenceEquals(other.function, null))
		  {
			return false;
		  }
		}
		else if (!function.Equals(other.function))
		{
		  return false;
		}
		if (string.ReferenceEquals(name, null))
		{
		  if (!string.ReferenceEquals(other.name, null))
		  {
			return false;
		  }
		}
		else if (!name.Equals(other.name))
		{
		  return false;
		}
		return true;
	  }

	  public override string ToString()
	  {
		return "QueryProperty["
		  + "name=" + name + ", function=" + function + "]";
	  }

	}

}