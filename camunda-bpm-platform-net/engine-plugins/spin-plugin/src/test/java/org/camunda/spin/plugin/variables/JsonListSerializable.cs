using System.Collections.Generic;
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
namespace org.camunda.spin.plugin.variables
{

	public class JsonListSerializable<T>
	{

	  private IList<T> listProperty;

	  public JsonListSerializable()
	  {
		this.listProperty = new List<T>();
	  }

	  public virtual void addElement(T element)
	  {
		this.listProperty.Add(element);
	  }

	  public virtual IList<T> ListProperty
	  {
		  get
		  {
			return listProperty;
		  }
		  set
		  {
			this.listProperty = value;
		  }
	  }


	  public virtual string toExpectedJsonString()
	  {
		StringBuilder jsonBuilder = new StringBuilder();

		jsonBuilder.Append("{\"listProperty\":[");
		for (int i = 0; i < listProperty.Count; i++)
		{
		  jsonBuilder.Append(listProperty[i]);
		  if (i < listProperty.Count - 1)
		  {
			jsonBuilder.Append(",");
		  }
		}
		jsonBuilder.Append("]}");

		return jsonBuilder.ToString();
	  }

	  public override string ToString()
	  {
		return toExpectedJsonString();
	  }


	}

}