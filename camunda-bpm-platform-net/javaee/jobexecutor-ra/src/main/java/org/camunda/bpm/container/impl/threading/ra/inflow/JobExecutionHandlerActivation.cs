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
namespace org.camunda.bpm.container.impl.threading.ra.inflow
{


	/// <summary>
	/// Represents the activation of a <seealso cref="JobExecutionHandler"/>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class JobExecutionHandlerActivation
	{

	  protected internal JcaExecutorServiceConnector ra;

	  protected internal JobExecutionHandlerActivationSpec spec;

	  protected internal MessageEndpointFactory endpointFactory;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JobExecutionHandlerActivation() throws javax.resource.ResourceException
	  public JobExecutionHandlerActivation() : this(null, null, null)
	  {
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JobExecutionHandlerActivation(org.camunda.bpm.container.impl.threading.ra.JcaExecutorServiceConnector ra, javax.resource.spi.endpoint.MessageEndpointFactory endpointFactory, JobExecutionHandlerActivationSpec spec) throws javax.resource.ResourceException
	  public JobExecutionHandlerActivation(JcaExecutorServiceConnector ra, MessageEndpointFactory endpointFactory, JobExecutionHandlerActivationSpec spec)
	  {
		this.ra = ra;
		this.endpointFactory = endpointFactory;
		this.spec = spec;
	  }

	  public virtual JobExecutionHandlerActivationSpec ActivationSpec
	  {
		  get
		  {
			return spec;
		  }
	  }

	  public virtual MessageEndpointFactory MessageEndpointFactory
	  {
		  get
		  {
			return endpointFactory;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void start() throws javax.resource.ResourceException
	  public virtual void start()
	  {
		// nothing to do here
	  }

	  public virtual void stop()
	  {
		// nothing to do here
	  }

	}

}