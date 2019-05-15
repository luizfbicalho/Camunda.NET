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
namespace org.camunda.bpm.engine.test.api.runtime.util
{

	[Serializable]
	public class SimpleSerializableBean
	{

	  private const long serialVersionUID = 1L;

	  protected internal int intProperty;

	  public SimpleSerializableBean()
	  {

	  }

	  public SimpleSerializableBean(int intProperty)
	  {
		this.intProperty = intProperty;
	  }

	  public virtual int IntProperty
	  {
		  get
		  {
			return intProperty;
		  }
		  set
		  {
			this.intProperty = value;
		  }
	  }


	  public override int GetHashCode()
	  {
		const int prime = 31;
		int result = 1;
		result = prime * result + intProperty;
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
		SimpleSerializableBean other = (SimpleSerializableBean) obj;
		if (intProperty != other.intProperty)
		{
		  return false;
		}
		return true;
	  }
	}

}