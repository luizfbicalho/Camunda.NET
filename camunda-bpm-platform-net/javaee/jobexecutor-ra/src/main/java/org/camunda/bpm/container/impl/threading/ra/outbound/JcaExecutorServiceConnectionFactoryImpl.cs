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
namespace org.camunda.bpm.container.impl.threading.ra.outbound
{




	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	[Serializable]
	public class JcaExecutorServiceConnectionFactoryImpl : JcaExecutorServiceConnectionFactory
	{

	  private const long serialVersionUID = 1L;

	  protected internal Reference reference;
	  protected internal JcaExecutorServiceManagedConnectionFactory mcf;
	  protected internal ConnectionManager connectionManager;

	  public JcaExecutorServiceConnectionFactoryImpl()
	  {
	  }

	  public JcaExecutorServiceConnectionFactoryImpl(JcaExecutorServiceManagedConnectionFactory mcf, ConnectionManager cxManager)
	  {
		this.mcf = mcf;
		this.connectionManager = cxManager;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JcaExecutorServiceConnection getConnection() throws javax.resource.ResourceException
	  public virtual JcaExecutorServiceConnection Connection
	  {
		  get
		  {
			return (JcaExecutorServiceConnection) connectionManager.allocateConnection(mcf, null);
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public javax.naming.Reference getReference() throws javax.naming.NamingException
	  public virtual Reference Reference
	  {
		  get
		  {
			return reference;
		  }
		  set
		  {
			this.reference = value;
		  }
	  }


	}

}