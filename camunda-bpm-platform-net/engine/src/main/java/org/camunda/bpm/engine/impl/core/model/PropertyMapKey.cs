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
namespace org.camunda.bpm.engine.impl.core.model
{
	/// <summary>
	/// Key of a map property.
	/// </summary>
	/// <param name="K"> the type of keys maintained by the map </param>
	/// <param name="V"> the type of mapped values
	/// 
	/// @author Philipp Ossler
	///  </param>
	public class PropertyMapKey<K, V>
	{

	  protected internal readonly string name;
	  protected internal bool allowOverwrite = true;

	  public PropertyMapKey(string name) : this(name, true)
	  {
	  }

	  public PropertyMapKey(string name, bool allowOverwrite)
	  {
		this.name = name;
		this.allowOverwrite = allowOverwrite;
	  }

	  public virtual bool allowsOverwrite()
	  {
		return allowOverwrite;
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public override string ToString()
	  {
		return "PropertyMapKey [name=" + name + "]";
	  }

	  public override int GetHashCode()
	  {
		const int prime = 31;
		int result = 1;
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
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: PropertyMapKey<?,?> other = (PropertyMapKey<?,?>) obj;
		PropertyMapKey<object, ?> other = (PropertyMapKey<object, ?>) obj;
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
	}

}