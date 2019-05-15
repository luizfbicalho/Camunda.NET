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
namespace org.camunda.bpm.integrationtest.functional.connect
{

	using ConnectorRequest = org.camunda.connect.spi.ConnectorRequest;

	public class TestConnectorRequest : ConnectorRequest<TestConnectorResponse>
	{

	  protected internal IDictionary<string, object> requestParameters;

	  public virtual IDictionary<string, object> RequestParameters
	  {
		  set
		  {
			requestParameters = value;
		  }
		  get
		  {
			return requestParameters;
		  }
	  }

	  public virtual void setRequestParameter(string name, object value)
	  {
		if (requestParameters == null)
		{
		  requestParameters = new Dictionary<string, object>();
		}
		requestParameters[name] = value;
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <V> V getRequestParameter(String name)
	  public virtual V getRequestParameter<V>(string name)
	  {
		return (V) requestParameters[name];
	  }

	  public virtual TestConnectorResponse execute()
	  {
		TestConnectorResponse response = new TestConnectorResponse();
		if (requestParameters != null)
		{
		  response.ResponseParameters = requestParameters;
		}
		return response;
	  }

	}

}