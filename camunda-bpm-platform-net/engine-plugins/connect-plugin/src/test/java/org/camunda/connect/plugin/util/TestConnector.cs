using System;
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

	using AbstractConnector = org.camunda.connect.impl.AbstractConnector;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class TestConnector : AbstractConnector<TestConnectorRequest, TestConnectorResponse>
	{

	  public const string ID = "testConnector";

	  public static IDictionary<string, object> requestParameters;
	  public static IDictionary<string, object> responseParameters = new Dictionary<string, object>();

	  public TestConnector(string connectorId) : base(connectorId)
	  {
	  }

	  public virtual TestConnectorRequest createRequest()
	  {
		return new TestConnectorRequest(this);
	  }

	  public virtual TestConnectorResponse execute(TestConnectorRequest req)
	  {
		// capture request parameters
		requestParameters = req.RequestParameters;

		TestRequestInvocation testRequestInvocation = new TestRequestInvocation(null, req, requestInterceptors);

		try
		{
		  testRequestInvocation.proceed();
		  // use response parameters
		  return new TestConnectorResponse(responseParameters);

		}
		catch (Exception e)
		{
		  throw new Exception(e);
		}
	  }

	}

}