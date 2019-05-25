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
namespace org.camunda.bpm.engine.rest.util
{
	using Resource = org.camunda.bpm.engine.authorization.Resource;

	public class ResourceUtil : Resource
	{

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string resourceName_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal int resourceType_Conflict;

	  public ResourceUtil(string resourceName, int resourceType)
	  {
		this.resourceName_Conflict = resourceName;
		this.resourceType_Conflict = resourceType;
	  }

	  public virtual string resourceName()
	  {
		return resourceName_Conflict;
	  }

	  public virtual int resourceType()
	  {
		return resourceType_Conflict;
	  }

	  public override int GetHashCode()
	  {
		const int prime = 31;
		int result = 1;
		result = prime * result + ((string.ReferenceEquals(resourceName_Conflict, null)) ? 0 : resourceName_Conflict.GetHashCode());
		result = prime * result + resourceType_Conflict;
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
		ResourceUtil other = (ResourceUtil) obj;
		if (string.ReferenceEquals(resourceName_Conflict, null))
		{
		  if (!string.ReferenceEquals(other.resourceName_Conflict, null))
		  {
			return false;
		  }
		}
		else if (!resourceName_Conflict.Equals(other.resourceName_Conflict))
		{
		  return false;
		}
		if (resourceType_Conflict != other.resourceType_Conflict)
		{
		  return false;
		}
		return true;
	  }
	}

}