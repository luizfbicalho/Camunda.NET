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

	using Connector = org.camunda.connect.spi.Connector;
	using ConnectorRequestInterceptor = org.camunda.connect.spi.ConnectorRequestInterceptor;
	using ConnectorResponse = org.camunda.connect.spi.ConnectorResponse;

	public class TestConnector : Connector<TestConnectorRequest>
	{

	  public const string ID = "pa-test-connector";

	  public virtual string Id
	  {
		  get
		  {
			return ID;
		  }
	  }

	  public virtual TestConnectorRequest createRequest()
	  {
		return new TestConnectorRequest();
	  }

	  public virtual IList<ConnectorRequestInterceptor> RequestInterceptors
	  {
		  get
		  {
			return null;
		  }
		  set
		  {
			// ignore
		  }
	  }


	  public virtual Connector<TestConnectorRequest> addRequestInterceptor(ConnectorRequestInterceptor connectorRequestInterceptor)
	  {
		// ignore
		return this;
	  }

	  public virtual Connector<TestConnectorRequest> addRequestInterceptors(ICollection<ConnectorRequestInterceptor> collection)
	  {
		// ignore
		return this;
	  }

	  public virtual ConnectorResponse execute(TestConnectorRequest testConnectorRequest)
	  {
		return testConnectorRequest.execute();
	  }

	}

}