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
	using Connector = org.camunda.connect.spi.Connector;
	using ConnectorProvider = org.camunda.connect.spi.ConnectorProvider;

	public class TestConnectorProvider : ConnectorProvider
	{

	  public virtual string ConnectorId
	  {
		  get
		  {
			return TestConnector.ID;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public org.camunda.connect.spi.Connector<?> createConnectorInstance()
	  public virtual Connector<object> createConnectorInstance()
	  {
		return new TestConnector(TestConnector.ID);
	  }
	}

}