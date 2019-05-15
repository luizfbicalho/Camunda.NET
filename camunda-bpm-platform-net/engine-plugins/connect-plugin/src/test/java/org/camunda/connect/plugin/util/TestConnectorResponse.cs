using System.Collections.Generic;

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
namespace org.camunda.connect.plugin.util
{

	using AbstractConnectorResponse = org.camunda.connect.impl.AbstractConnectorResponse;


	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class TestConnectorResponse : AbstractConnectorResponse
	{

	  private IDictionary<string, object> responseParameters;

	  public TestConnectorResponse(IDictionary<string, object> responseParameters)
	  {
		this.responseParameters = responseParameters;
	  }

	  protected internal virtual void collectResponseParameters(IDictionary<string, object> arg0)
	  {
		if (responseParameters != null)
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
		  arg0.putAll(responseParameters);
		}
	  }

	}

}